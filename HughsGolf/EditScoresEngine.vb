' ================= FULL DROP-IN EditScoresEngine (SAFE / OPTION A) =================
' ✔ Same public surface
' ✔ Same behavior
' ✔ Faster (no DB reload per hole)
' ✔ Safe subs caching with fallback

Imports System.Text
Imports System.Data.SQLite

Module EditScoresEngine

    ' --- DATA STRUCTURES ---
    Public Class HoleInfo
        Public Property Score As Integer = 0
        Public Property Par As Integer = 0
        Public Property StrokeCount As Integer = 0
        Public Property IsBirdie As Boolean = False
        Public Property IsEagle As Boolean = False
        Public Property IsUnderPar As Boolean = False
        Public Property IsSkinWinner As Boolean = False
        Public Property IsTie As Boolean = False
        Public Property IsLowest As Boolean = False
        Public Property IsCapped As Boolean = False
    End Class

    Public Class RowScoreInfo
        Public Property PlayerName As String = ""
        Public Property Holes As New Dictionary(Of Integer, HoleInfo)
        Public Property IsSub As Boolean = False
    End Class

    Public Class PlayerScores
        Public Name As String
        Public Net As Integer
        Public Points As Decimal
        Public League As String
        Public MatchDate As String
        Public Round As String
    End Class

    ' --- PRIVATE STATE ---
    Private _dgv As DataGridView
    Private _helper As Object
    Private _thisDate As String
    Private _holeColumns As Dictionary(Of Integer, String)
    Public _suppressEvents As Boolean = False
    Private _isInitialized As Boolean = False
    Private _sb As StringBuilder
    Friend _OldPlayer As String

    ' Public so ScoreCard.vb CellPainting can access it
    Public scoreCache As New Dictionary(Of String, RowScoreInfo)
    Public SkinMapCache As Dictionary(Of String, HashSet(Of Integer))
    ' --- NEW CACHE ---
    Private _subsLookup As HashSet(Of String)
    Private _dtScores As DataTable
    Public Function IsReady() As Boolean

        If Not _isInitialized OrElse _dgv Is Nothing Then Return False

        If _dtScores Is Nothing OrElse _dtScores.Rows.Count = 0 Then Return False

        Return True

    End Function

    ' ================= INITIALIZE =================
    Public Sub Initialize(dgv As DataGridView, helper As Object)
        If helper Is Nothing Then Exit Sub

        _dgv = dgv
        _helper = helper
        _thisDate = ctx.ActiveDate

        BuildHoleColumnMap()

        PreloadSubs() ' NEW

        EnsureLast5Built()
        Dim sc = TryCast(_dgv.FindForm(), ScoreCard)
        If sc Is Nothing Then Exit Sub

        _dtScores = sc.dtScores
        _isInitialized = True
    End Sub

    Private Sub BuildHoleColumnMap()
        _holeColumns = New Dictionary(Of Integer, String)
        For i = 1 To 9
            _holeColumns.Add(i, i.ToString())
        Next
    End Sub

    ' ================= HANDLE ENTRY (OPTIMIZED) =================
    Friend Sub HandleHoleEntry(dgv As DataGridView,
                    row As DataGridViewRow,
                    colName As String,
                    newVal As String)

        ' ── Rapid 9-digit entry (only on Hole 1) ─────────────────────────────────
        If colName = "1" AndAlso newVal.Length > 1 AndAlso IsNumeric(newVal) Then
            If newVal.Length > 9 Then
                MessageBox.Show("Please enter exactly 9 scores (one per hole).",
                    "Too Many Scores",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)
                row.Cells("1").Value = Nothing
                _dgv.BeginEdit(True)
                Exit Sub
            End If

            _suppressEvents = True
            Try
                row.Cells("1").Value = Nothing

                For i As Integer = 0 To newVal.Length - 1
                    Dim targetCol As String = (i + 1).ToString()
                    If dgv.Columns.Contains(targetCol) Then
                        Dim scoreVal As Integer = CInt(newVal.Substring(i, 1))
                        Dim actualHole As String = If(ctx.sFrontBack = "Back",
                                          (i + 10).ToString(),
                                          (i + 1).ToString())
                        Dim cappedScore As Integer = CInt(_helper.ChkForMax(scoreVal, actualHole))
                        ' Update UI
                        row.Cells(targetCol).Value = cappedScore
                        ' Persist
                        UpsertScore(row, targetCol, scoreVal)
                        ' Update memory
                        UpdateDtScores(row, targetCol, cappedScore, dgv)
                    End If
                Next

            Finally
                _suppressEvents = False
            End Try

            HandleSkinBuyIn(row)
            FinalizeScoreEntry(dgv)
            Exit Sub
        End If

        ' ── Normal single entry ────────────────────────────────────────
        Dim score As Integer
        If Not Integer.TryParse(newVal, score) OrElse score <= 0 OrElse score > 10 Then Exit Sub

        ' Persist
        UpsertScore(row, colName, score)

        ' Update memory
        UpdateDtScores(row, colName, score, dgv)

        ' Prompt for skins/CTP on first hole
        HandleSkinBuyIn(row)

        ' Finalize
        FinalizeScoreEntry(dgv)

    End Sub

    ' --- SCORE OPS ---
    Friend Sub UpsertScore(row As DataGridViewRow, holeCol As String, score As Object)
        Dim player As String = row.Cells("Player").Value.ToString()
        Dim sDate As String = row.Cells("Date").Value.ToString()

        Dim actualHole As String = If(ctx.sFrontBack = "Back",
                          (CInt(holeCol) + 9).ToString(),
                          holeCol)

        ' Cap score
        Dim holeValue As Object = DBNull.Value
        If score IsNot Nothing Then
            Dim scoreInt As Integer = CInt(score)
            Dim cappedScore As Integer = CInt(_helper.ChkForMax(scoreInt, actualHole))
            If cappedScore < scoreInt Then
                LOGIT($"Score capped: {player} hole {actualHole} {scoreInt}→{cappedScore}")
            End If
            holeValue = cappedScore
        End If

        ' Build all 9 hole values from grid cells — not just the one being entered
        Dim h(9) As Object
        For i As Integer = 1 To 9
            Dim colName As String = i.ToString()
            If colName = holeCol Then
                h(i) = holeValue  ' use the new value being entered
            Else
                Dim cellVal = row.Cells(colName).Value
                h(i) = If(IsNumeric(cellVal) AndAlso CInt(cellVal) > 0, CInt(cellVal), DBNull.Value)
            End If
        Next

        ' Calculate gross from all 9
        Dim enteredHoleScores As New List(Of Object)
        For i As Integer = 1 To 9
            enteredHoleScores.Add(h(i))
        Next
        Dim gross As Integer = ScoreRulesService.CalculateGross(enteredHoleScores)
        Dim handicap As Integer = GetRowPreviousHandicap(row)
        Dim sql As String = "
    INSERT OR IGNORE INTO Scores (League, Player, Date, FrontBack)
    VALUES (@League, @Player, @Date, @FrontBack);
    UPDATE Scores SET
        [1]=@H1,[2]=@H2,[3]=@H3,[4]=@H4,[5]=@H5,
        [6]=@H6,[7]=@H7,[8]=@H8,[9]=@H9,
        Gross = CASE WHEN @Gross = 0 THEN NULL ELSE @Gross END,
        Net = CASE WHEN @Gross = 0 THEN NULL ELSE @Gross - @Handicap END
    WHERE League=@League AND Player=@Player AND Date=@Date"

        Using cmd As New SQLiteCommand(sql, ctx.Conn)
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            cmd.Parameters.AddWithValue("@Date", sDate)
            cmd.Parameters.AddWithValue("@FrontBack", ctx.sFrontBack)
            cmd.Parameters.AddWithValue("@Gross", gross)
            cmd.Parameters.AddWithValue("@Handicap", handicap)
            For i As Integer = 1 To 9
                cmd.Parameters.AddWithValue($"@H{i}", h(i))
            Next
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function GetRowPreviousHandicap(row As DataGridViewRow) As Integer
        If row Is Nothing Then Return 0

        Dim hdcpValue As Object = Nothing
        If row.DataGridView IsNot Nothing AndAlso row.DataGridView.Columns.Contains("PHdcp") Then
            hdcpValue = row.Cells("PHdcp").Value
        ElseIf row.DataGridView IsNot Nothing AndAlso row.DataGridView.Columns.Contains("phdcp") Then
            hdcpValue = row.Cells("phdcp").Value
        End If

        If hdcpValue IsNot Nothing AndAlso Not IsDBNull(hdcpValue) AndAlso IsNumeric(hdcpValue) Then
            Return CInt(hdcpValue)
        End If

        Return 0
    End Function

    ' ================= MEMORY UPDATE =================
    Private Sub UpdateDtScores(row As DataGridViewRow, colName As String, score As Object, dgv As DataGridView)

        Dim player = row.Cells("Player").Value.ToString()

        Dim dtRow As DataRow = Nothing
        For Each r As DataRow In _dtScores.Rows
            If r("Player").ToString().Trim().Equals(player.Trim(), StringComparison.OrdinalIgnoreCase) Then
                dtRow = r
                Exit For
            End If
        Next

        If dtRow Is Nothing Then Exit Sub

        ' --- SET VALUE ---
        If score Is Nothing Then
            dtRow(colName) = DBNull.Value
        Else
            dtRow(colName) = score
        End If

        ' --- GROSS ---
        Dim holeScores As New List(Of Object)
        For i = 1 To 9
            holeScores.Add(dtRow(i.ToString()))
        Next
        Dim gross As Integer = ScoreRulesService.CalculateGross(holeScores)

        dtRow("Gross") = If(gross = 0, DBNull.Value, gross)

        ' --- NET ---
        Dim hdcp As Integer = If(IsNumeric(dtRow("phdcp")), CInt(dtRow("phdcp")), 0)

        If IsDBNull(dtRow("Gross")) Then
            dtRow("Net") = DBNull.Value
        Else
            dtRow("Net") = ScoreRulesService.CalculateNet(gross, hdcp)
        End If

    End Sub
    Private Function IsRoundComplete(row As DataRow) As Boolean

        For i As Integer = 1 To 9
            Dim val = row(i.ToString())

            If IsDBNull(val) OrElse Not IsNumeric(val) Then
                Return False
            End If
        Next

        Return True

    End Function
    ' ================= FINALIZE (LIGHT) =================
    Public Sub FinalizeScoreEntry(dgv As DataGridView)
        LOGIT("FINALIZE → " & dgv.CurrentRow?.Cells("Player").Value?.ToString())
        Dim row = dgv.CurrentRow
        If row Is Nothing Then Exit Sub

        Dim player = row.Cells("Player").Value?.ToString()?.Trim()
        If String.IsNullOrEmpty(player) Then Exit Sub

        ' 🔥 existing logic continues below...
        If Not EditScoresEngine.IsReady() Then
            EditScoresEngine.Initialize(dgv, ctx.oHelper)
            EditScoresEngine.LoadScoresFromView()
        End If

        If Not ctx.IsPostSeason Then
            Dim tm As New TeamMatch()
            tm.CalculateThisMatch(row)
        End If

        Dim dtRow As DataRow = Nothing
        For Each r As DataRow In _dtScores.Rows
            If r("Player").ToString().Trim().Equals(player.Trim(), StringComparison.OrdinalIgnoreCase) Then
                dtRow = r
                Exit For
            End If
        Next
        If dtRow IsNot Nothing AndAlso IsRoundComplete(dtRow) Then
            SaveSideGames()
            AwardSkins()

            If Not ctx.IsPostSeason Then
                Dim gross As String = If(IsDBNull(dtRow("Gross")), "0", dtRow("Gross").ToString())
                ctx.sPlayer = player
                UpdatePlayerLast5(player)  ' ← also inside this block
                Dim newHdcp As String = _helper.GetNewHdcp(gross)
                dtRow("PHdcp") = newHdcp
            End If
        End If

        BuildScoreCache()

        ' Refresh matches
        If Not String.IsNullOrEmpty(player) Then
            RefreshMatchesForPlayer(player)
        End If

        dgv.Refresh()

    End Sub
    Friend Sub HandleSkinBuyIn(row As DataGridViewRow)
        If _suppressEvents Then Exit Sub

        Dim skinsVal As String = If(row.Cells("InSkins").Value, "N").ToString().Trim().ToUpper()
        Dim ctpVal As String = If(row.Cells("InCTPs").Value, "N").ToString().Trim().ToUpper()
        If skinsVal = "Y" AndAlso ctpVal = "Y" Then Exit Sub

        ' Check if already paid this date
        Dim player As String = row.Cells("Player").Value?.ToString()
        Dim alreadyPaid As Boolean = False
        Using cmd As New SQLiteCommand("
        SELECT COUNT(*) FROM Payments 
        WHERE League=@League AND Player=@Player AND Date=@Date
        AND Desc IN ('Skin','CTP') AND Detail='Payment'", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            alreadyPaid = CInt(cmd.ExecuteScalar()) > 0
        End Using
        If alreadyPaid Then Exit Sub

        Dim isSub As Boolean = False
        Dim playerKey As String = player?.Trim().ToUpper()
        If scoreCache.ContainsKey(playerKey) Then
            isSub = scoreCache(playerKey).IsSub
        End If

        Dim bPostSeason As Boolean = ctx.ActiveDate >= ctx.sPSDate
        Dim skinAmt As Decimal = If(bPostSeason, CDec(ctx.rLeagueParmrow("SkinsPS")), CDec(ctx.rLeagueParmrow("Skins")))
        Dim ctpAmt As Decimal = If(bPostSeason, CDec(ctx.rLeagueParmrow("ClosestPS")), CDec(ctx.rLeagueParmrow("Closest")))
        Dim eoyWeekAmt As Decimal = CDec(ctx.rLeagueParmrow("SkinsPS")) + CDec(ctx.rLeagueParmrow("ClosestPS"))
        Dim season As String = ctx.ActiveDate.Substring(0, 4)

        Dim ldPaid As Decimal = 0
        Dim eoyPaid As Decimal = 0
        Dim inEOY1 As Boolean = False
        Dim inEOY2 As Boolean = False

        Using cmd As New SQLiteCommand("
        SELECT IFNULL(SUM(Earned),0) FROM Payments 
        WHERE League=@League AND Player=@Player 
        AND Desc='League Dues' AND Detail='Payment' 
        AND SUBSTR(Date,1,4)=@Season", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            cmd.Parameters.AddWithValue("@Season", season)
            ldPaid = CDec(cmd.ExecuteScalar())
        End Using

        Using cmd As New SQLiteCommand("
        SELECT IFNULL(SUM(Earned),0) FROM Payments 
        WHERE League=@League AND Player=@Player 
        AND Desc='EOY Skins' AND Detail='Payment' 
        AND SUBSTR(Date,1,4)=@Season", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            cmd.Parameters.AddWithValue("@Season", season)
            eoyPaid = CDec(cmd.ExecuteScalar())
        End Using

        Using cmd As New SQLiteCommand("
        SELECT IFNULL(SUM(Earned),0) FROM Payments 
        WHERE League=@League AND Player=@Player 
        AND Desc='EOY Skins' AND Comment='(1st Week)'", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            inEOY1 = CDec(cmd.ExecuteScalar()) > 0
        End Using

        Using cmd As New SQLiteCommand("
        SELECT IFNULL(SUM(Earned),0) FROM Payments 
        WHERE League=@League AND Player=@Player 
        AND Desc='EOY Skins' AND Comment='(2nd Week)'", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            inEOY2 = CDec(cmd.ExecuteScalar()) > 0
        End Using

        Dim ldBalance As Decimal = CDec(ctx.rLeagueParmrow("Cost")) - ldPaid
        Dim eoyBalance As Decimal = (eoyWeekAmt * 2) - eoyPaid

        Using frm As New frmPayDues(
        player, isSub,
        skinsVal <> "Y", skinAmt,
        ctpVal <> "Y", ctpAmt,
        If(isSub, 0, ldBalance),
        If(isSub, 0, eoyBalance),
        If(isSub, 0, eoyWeekAmt),
        inEOY1, inEOY2)

            If frm.ShowDialog() = DialogResult.OK Then
                ctx.sPlayer = player
                If frm.PaySkins Then
                    MakePmt("Skin")
                    row.Cells("InSkins").Value = "Y"  ' ← update grid
                End If
                If frm.PayCTP Then
                    MakePmt("CTP")
                    row.Cells("InCTPs").Value = "Y"  ' ← update grid
                End If
                If frm.PayLeagueDues > 0 Then MakePmtAmount("League Dues", frm.PayLeagueDues)
                If frm.PayEOY > 0 Then MakePmtAmount("EOY Skins", frm.PayEOY, frm.EOYWeekComment)
            End If

        End Using
    End Sub
    Private Sub SaveSideGames()
        SaveCTPResults()
        LoadSkinMap()
    End Sub
    ' --- SKINS ---
    Public Sub SaveSkinResults()
        Try

            Dim skinWinners = CalculateSkinWinners(_dtScores)

            Dim playersInSkins As Integer = 0
            For Each row As DataRow In _dtScores.Rows
                If row("InSkins").ToString().ToUpper() = "Y" Then playersInSkins += 1
            Next
            If playersInSkins = 0 Then Exit Sub

            Dim skinEntryFee As Decimal =
        If(ctx.ActiveDate < ctx.sPSDate,
            CDec(ctx.rLeagueParmrow("skins")),
            CDec(ctx.rLeagueParmrow("skinsps")))

            Dim conn = ctx.Conn
            If conn.State <> ConnectionState.Open Then conn.Open()

            Using trans = conn.BeginTransaction()

                Dim firstRow = _dtScores.Rows(0)

                ' --- DELETE OLD SKIN WIN RECORDS ---
                Dim delSql As String = "
            DELETE FROM Payments
            WHERE Desc = 'Skin'
              AND League = @League
              AND Date = @Date
              AND Detail LIKE '#%'"

                Dim delParams As New Dictionary(Of String, Object) From {
                {"@League", firstRow("League").ToString()},
                {"@Date", firstRow("Date").ToString()}
            }

                _helper.SqliteExec(delSql, delParams, trans)

                ' --- DELETE OLD KITTY CARRYOVER ---
                Dim delKittySql As String = "
            DELETE FROM Payments
            WHERE Desc = 'Skin'
              AND League = @League
              AND Date = @Date
              AND Player = 'Kitty'
              AND Detail = 'Carryover'"

                _helper.SqliteExec(delKittySql, delParams, trans)

                ' --- DELETE OLD PLAYER PAYMENTS ---
                Dim delPaySql As String = "
            DELETE FROM Payments
            WHERE Desc = 'Skin'
              AND League = @League
              AND Date = @Date
              AND Detail = 'Payment'"

                _helper.SqliteExec(delPaySql, delParams, trans)

                ' --- INSERT PLAYER PAYMENTS ---
                For Each row As DataRow In _dtScores.Rows

                    If row("InSkins").ToString().ToUpper() <> "Y" Then Continue For

                    Dim sql As String = "
                INSERT INTO Payments
                (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                VALUES
                (@League, @Player, @Date, 'Skin', 'Payment', @Earned, @DatePaid, '', '')"

                    Dim p As New Dictionary(Of String, Object) From {
                    {"@League", row("League").ToString()},
                    {"@Player", row("Player").ToString()},
                    {"@Date", row("Date").ToString()},
                    {"@Earned", skinEntryFee},
                    {"@DatePaid", DateTime.Now.ToString("M/d/yyyy h:mm:ss tt")}
                }

                    _helper.SqliteExec(sql, p, trans)

                Next

                ' --- LOOK UP CARRYOVER ---
                Dim carryover As Decimal = 0
                Dim carrySql As String = $"
            SELECT IFNULL(SUM(Earned), 0)
            FROM Payments
            WHERE League = '{ctx.SafeLeagueName}'
              AND Player = 'Kitty'
              AND Desc = 'Skin'
              AND Detail = 'Carryover'
              AND Date < {ctx.ActiveDate}"

                Using cmd As New SQLiteCommand(carrySql, conn)
                    cmd.Transaction = trans
                    carryover = CDec(cmd.ExecuteScalar())
                End Using

                LOGIT($"Skin carryover from previous weeks: ${carryover}")

                Dim totalPot As Decimal = (playersInSkins * skinEntryFee) + carryover

                Dim totalSkins As Integer = 0
                For Each winners In skinWinners.Values
                    If winners.Count = 1 Then totalSkins += 1
                Next

                ' --- NO WINNERS → FULL CARRYOVER ---
                If totalSkins = 0 Then
                    LOGIT("No skin winners this week — carrying entire pot to Kitty")
                    InsertKittyCarryover(conn, trans,
                    firstRow("League").ToString(),
                    firstRow("Date").ToString(),
                    CInt(totalPot))
                    trans.Commit()
                    Exit Sub
                End If

                Dim payoutPerSkin As Integer = Math.Floor(totalPot / totalSkins)

                If payoutPerSkin = 0 Then
                    LOGIT("Skin payout < $1 — carrying entire pot to Kitty")
                    InsertKittyCarryover(conn, trans,
                    firstRow("League").ToString(),
                    firstRow("Date").ToString(),
                    CInt(totalPot))
                    trans.Commit()
                    Exit Sub
                End If

                ' --- INSERT WINNERS ---
                For Each hole In skinWinners.Keys

                    Dim winners = skinWinners(hole)
                    If winners.Count <> 1 Then Continue For

                    Dim row = _dtScores.Rows(winners(0))
                    If row("InSkins").ToString().ToUpper() <> "Y" Then Continue For

                    Dim gross As Integer = CInt(row(hole.ToString()))
                    Dim hdcp As Integer = If(IsNumeric(row("phdcp")), CInt(row("phdcp")), 0)
                    Dim sActhole = If(ctx.sFrontBack = "Back", hole + 9, hole)
                    Dim netScore As Integer = gross - ScoreCard.showStrokeHoles(sActhole, hdcp)

                    Dim sql As String = "
                INSERT INTO Payments
                (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                VALUES
                (@League, @Player, @Date, 'Skin', @Detail, @Earned, @DatePaid, @Comment, '')"

                    Dim p As New Dictionary(Of String, Object) From {
                    {"@League", row("League").ToString()},
                    {"@Player", row("Player").ToString()},
                    {"@Date", row("Date").ToString()},
                    {"@Detail", "#" & hole},
                    {"@Earned", payoutPerSkin},
                    {"@DatePaid", DateTime.Now.ToString("M/d/yyyy h:mm:ss tt")},
                    {"@Comment", "Score=" & netScore}
                }

                    _helper.SqliteExec(sql, p, trans)

                Next

                ' --- REMAINDER ---
                Dim remainder As Integer = CInt(totalPot) Mod totalSkins
                If remainder > 0 Then
                    LOGIT($"Skin remainder: ${remainder} → Kitty carryover")
                    InsertKittyCarryover(conn, trans,
                    firstRow("League").ToString(),
                    firstRow("Date").ToString(),
                    remainder)
                End If

                trans.Commit()

                LOGIT($"SaveSkinResults complete — pot=${totalPot}, winners={totalSkins}, payout=${payoutPerSkin}, remainder=${remainder}")

            End Using

        Catch ex As Exception
            LOGIT("SaveSkinResults Error: " & ex.Message)
        End Try
    End Sub
    Public Sub SaveCTPResults()
        Try
            Dim playersInCTP As Integer = 0
            For Each row As DataRow In _dtScores.Rows
                If row("InCTPs").ToString().ToUpper() = "Y" Then playersInCTP += 1
            Next
            If playersInCTP = 0 Then Exit Sub

            Dim ctpEntryFee As Decimal =
        If(ctx.ActiveDate < ctx.sPSDate,
            CDec(ctx.rLeagueParmrow("Closest")),
            CDec(ctx.rLeagueParmrow("ClosestPS")))

            Dim totalCTPPot As Decimal = playersInCTP * ctpEntryFee
            Dim slotPot As Decimal = Math.Floor(totalCTPPot / ctx.lPar3s.Count)

            Dim conn = ctx.Conn
            If conn.State <> ConnectionState.Open Then conn.Open()

            Dim firstRow = _dtScores.Rows(0)

            Using trans = conn.BeginTransaction()

                ' --- DELETE OLD CTP PAYMENTS (ENTRY FEES) ---
                Dim delPaySql As String = "
            DELETE FROM Payments
            WHERE Desc = 'CTP'
              AND League = @League
              AND Date = @Date
              AND Detail = 'Payment'"

                Dim delParams As New Dictionary(Of String, Object) From {
                {"@League", firstRow("League").ToString()},
                {"@Date", firstRow("Date").ToString()}
            }

                _helper.SqliteExec(delPaySql, delParams, trans)

                ' --- INSERT PLAYER PAYMENTS ---
                For Each row As DataRow In _dtScores.Rows

                    If row("InCTPs").ToString().ToUpper() <> "Y" Then Continue For

                    Dim sql As String = "
                INSERT INTO Payments
                (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                VALUES
                (@League, @Player, @Date, 'CTP', 'Payment', @Earned, @DatePaid, '', '')"

                    Dim p As New Dictionary(Of String, Object) From {
                    {"@League", row("League").ToString()},
                    {"@Player", row("Player").ToString()},
                    {"@Date", row("Date").ToString()},
                    {"@Earned", ctpEntryFee},
                    {"@DatePaid", DateTime.Now.ToString("M/d/yyyy h:mm:ss tt")}
                }

                    _helper.SqliteExec(sql, p, trans)

                Next

                ' --- PROCESS EACH PAR 3 SLOT ---
                For slotIndex As Integer = 0 To ctx.lPar3s.Count - 1

                    Dim par3Hole As String = ctx.lPar3s(slotIndex)
                    Dim slot As Integer = slotIndex + 1
                    Dim carryoverDetail As String = ScoreRulesService.CtpCarryoverDetail(slot, ctx.sFrontBack)

                    ' --- LOOK UP CARRYOVER ---
                    Dim carryover As Decimal = 0
                    Dim carrySql As String = "
                SELECT IFNULL((
                    SELECT Earned
                    FROM Payments
                    WHERE League = @League
                      AND Player = 'Kitty'
                      AND Desc = 'CTP'
                      AND Detail = @Detail
                      AND Date < @Date
                    ORDER BY Date DESC
                    LIMIT 1
                ), 0)"

                    Using cmd As New SQLiteCommand(carrySql, conn)
                        cmd.Transaction = trans
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Detail", carryoverDetail)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        carryover = CDec(cmd.ExecuteScalar())
                    End Using

                    LOGIT($"CTP slot {slot} carryover: ${carryover}")

                    Dim slotTotal As Decimal = slotPot + carryover

                    ' --- DELETE OLD WINNERS ---
                    Dim delSql As String = "
                DELETE FROM Payments
                WHERE Desc = 'CTP'
                  AND League = @League
                  AND Date = @Date
                  AND Detail = @Detail"

                    _helper.SqliteExec(delSql,
                        New Dictionary(Of String, Object) From {
                            {"@League", firstRow("League").ToString()},
                            {"@Date", firstRow("Date").ToString()},
                            {"@Detail", $"#{par3Hole}"}
                        }, trans)

                    ' --- DELETE OLD KITTY CARRYOVER ---
                    Dim delKittySql As String = "
                DELETE FROM Payments
                WHERE Desc = 'CTP'
                  AND League = @League
                  AND Date = @Date
                  AND Player = 'Kitty'
                  AND Detail = @Detail"

                    _helper.SqliteExec(delKittySql,
                        New Dictionary(Of String, Object) From {
                            {"@League", firstRow("League").ToString()},
                            {"@Date", firstRow("Date").ToString()},
                            {"@Detail", carryoverDetail}
                        }, trans)

                    ' --- CHECK FOR EXISTING WINNER ---
                    Dim winnerSql As String = "
                SELECT Player FROM Payments
                WHERE League = @League
                  AND Date = @Date
                  AND Desc = 'CTP'
                  AND Detail = @Detail
                  AND Player <> 'Kitty'"

                    Dim hasWinner As Boolean = False
                    Dim winnerName As String = ""

                    Using cmd As New SQLiteCommand(winnerSql, conn)
                        cmd.Transaction = trans
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.Parameters.AddWithValue("@Detail", $"#{par3Hole}")
                        Dim result = cmd.ExecuteScalar()
                        If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                            hasWinner = True
                            winnerName = result.ToString()
                        End If
                    End Using

                    LOGIT($"CTP slot={slot} hasWinner={hasWinner} winnerName={winnerName} slotTotal={slotTotal}")

                    If hasWinner Then
                        ' --- UPDATE WINNER PAYOUT ---
                        Dim updateSql As String = "
                    UPDATE Payments 
                    SET Earned = @Earned
                    WHERE League = @League
                      AND Date = @Date
                      AND Desc = 'CTP'
                      AND Detail = @Detail
                      AND Player <> 'Kitty'"

                        Using updateCmd As New SQLiteCommand(updateSql, conn)
                            updateCmd.Transaction = trans
                            updateCmd.Parameters.AddWithValue("@Earned", CInt(slotTotal))
                            updateCmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                            updateCmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                            updateCmd.Parameters.AddWithValue("@Detail", $"#{par3Hole}")
                            updateCmd.ExecuteNonQuery()
                        End Using

                    Else
                        ' --- NO WINNER → CARRYOVER ---
                        InsertKittyCarryover(conn, trans,
                        firstRow("League").ToString(),
                        firstRow("Date").ToString(),
                        CInt(slotTotal), slot)
                    End If

                Next

                trans.Commit()

                LOGIT($"SaveCTPResults complete — totalPot=${totalCTPPot}, slotPot=${slotPot}")

            End Using

        Catch ex As Exception
            LOGIT("SaveCTPResults Error: " & ex.Message)
        End Try
    End Sub

    Private Sub InsertKittyCarryover(conn As SQLiteConnection,
                                  trans As SQLiteTransaction,
                                  league As String,
                                  dateVal As String,
                                  amount As Decimal,
                                  Optional ctpSlot As Integer = 0)
        If amount <= 0 Then Exit Sub

        Dim desc As String = If(ctpSlot = 0, "Skin", "CTP")
        ' Skins carryover has no frontback suffix — CTPs do
        Dim detail As String = If(ctpSlot = 0,
                              "Carryover",
                              $"Carryover{ctpSlot}-{ctx.sFrontBack}")

        Dim sql As String = "
        INSERT INTO Payments
        (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
        VALUES
        (@League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, @PayMethod)"

        Dim p As New Dictionary(Of String, Object) From {
        {"@League", league},
        {"@Player", "Kitty"},
        {"@Date", dateVal},
        {"@Desc", desc},
        {"@Detail", detail},
        {"@Earned", amount},
        {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
        {"@Comment", If(ctpSlot = 0, "Skin pot remainder", $"CTP{ctpSlot} no winner - {ctx.sFrontBack}")},
        {"@PayMethod", ""}
    }
        _helper.SqliteExec(sql, p, trans)
        LOGIT($"Kitty carryover: {desc} {detail} ${amount}")
    End Sub

    Public Sub LoadSkinMap()
        SkinMapCache = New Dictionary(Of String, HashSet(Of Integer))
        Try
            Dim sql As String = "
                SELECT Player, Detail
                FROM Payments
                WHERE Desc = 'Skin'
                  AND League = @League
                  AND Date = @Date"

            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)

                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim player = rdr("Player").ToString()
                        Dim detail = rdr("Detail").ToString()
                        If Not detail.StartsWith("#") Then Continue While
                        Dim hole As Integer = CInt(detail.Replace("#", ""))
                        If Not SkinMapCache.ContainsKey(player) Then
                            SkinMapCache(player) = New HashSet(Of Integer)
                        End If
                        SkinMapCache(player).Add(hole)
                    End While
                End Using
            End Using

        Catch ex As Exception
            LOGIT("LoadSkinMap Error: " & ex.Message)
        End Try
    End Sub
    Private Function CalculateSkinWinners(dt As DataTable) As Dictionary(Of Integer, List(Of Integer))
        Dim winners As New Dictionary(Of Integer, List(Of Integer))
        Try
            ' Count players in skins
            Dim skinPlayers As Integer = 0
            For Each row As DataRow In dt.Rows
                If row("InSkins").ToString().ToUpper() = "Y" Then skinPlayers += 1
            Next

            For hole As Integer = 1 To 9
                Dim minScore As Integer = Integer.MaxValue
                Dim rowIndices As New List(Of Integer)
                Dim sActhole = If(ctx.sFrontBack = "Back", hole + 9, hole)
                Dim scoresEntered As Integer = 0

                For r As Integer = 0 To dt.Rows.Count - 1
                    Dim row = dt.Rows(r)
                    If row("InSkins").ToString().ToUpper() <> "Y" Then Continue For

                    Dim cellVal = row(hole.ToString())
                    If Not IsNumeric(cellVal) OrElse CInt(cellVal) <= 0 Then Continue For

                    scoresEntered += 1
                    Dim gross As Integer = CInt(cellVal)
                    Dim hdcp As Integer = If(IsNumeric(row("phdcp")), CInt(row("phdcp")), 0)
                    Dim netScore As Integer = gross - ScoreCard.showStrokeHoles(sActhole, hdcp)

                    If netScore < minScore Then
                        minScore = netScore
                        rowIndices.Clear()
                        rowIndices.Add(r)
                    ElseIf netScore = minScore Then
                        rowIndices.Add(r)
                    End If
                Next

                ' Only award if ALL skin players have scores AND exactly 1 winner
                If scoresEntered = skinPlayers AndAlso rowIndices.Count = 1 Then
                    winners(hole) = rowIndices
                End If
            Next
        Catch ex As Exception
            LOGIT("CalculateSkinWinners Error: " & ex.Message)
        End Try
        Return winners
    End Function

    ' ================= SAFE SUB CACHE =================
    Public Sub PreloadSubs()
        _subsLookup = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Try
            Dim sql As String = "SELECT Player FROM Subs WHERE League = @League AND Date = @Date"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)

                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        _subsLookup.Add(rdr("Player").ToString().Trim().ToUpper())
                    End While
                End Using
            End Using

        Catch ex As Exception
            LOGIT("PreloadSubs Error: " & ex.Message)
        End Try
    End Sub
    Public Sub ClearPlayerScores(dgv As DataGridView, row As DataGridViewRow)
        If row Is Nothing Then Exit Sub

        Dim player As String = row.Cells("Player").Value?.ToString()
        If String.IsNullOrEmpty(player) Then Exit Sub

        ' Confirm before clearing
        Dim mbr = MessageBox.Show(
        $"Clear scores for {player} and partner?",
        "Confirm Clear",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning)
        If mbr <> DialogResult.Yes Then Exit Sub

        Dim matchDate As String = row.Cells("Date").Value.ToString()

        LOGIT($"ClearPlayerScores: player={player} date={matchDate}")

        _suppressEvents = True
        Try
            ' --- FIND PARTNER ROW ---
            Dim partnerRow As DataGridViewRow = Nothing
            Dim startIndex As Integer = (row.Index \ 4) * 4
            Dim endIndex As Integer = Math.Min(startIndex + 3, dgv.Rows.Count - 1)
            Dim myGrade As String = row.Cells("Grade").Value?.ToString()

            For i As Integer = startIndex To endIndex
                Dim r As DataGridViewRow = dgv.Rows(i)
                If r.IsNewRow OrElse r Is row Then Continue For
                If r.Cells("Grade").Value?.ToString() = myGrade Then
                    partnerRow = r
                    Exit For
                End If
            Next

            Dim rowsToClear As New List(Of DataGridViewRow) From {row}
            If partnerRow IsNot Nothing Then rowsToClear.Add(partnerRow)

            ' --- SINGLE PASS: clear scores, restore subs ---
            For Each r As DataGridViewRow In rowsToClear
                Dim currentPlayer As String = r.Cells("Player").Value?.ToString()

                ' Clear scores in DB
                Using cmd As New SQLiteCommand("
                UPDATE Scores SET 
                    [1]=NULL,[2]=NULL,[3]=NULL,[4]=NULL,[5]=NULL,
                    [6]=NULL,[7]=NULL,[8]=NULL,[9]=NULL,
                    Gross=NULL, Net=NULL
                WHERE League=@League AND Player=@Player AND Date=@Date", ctx.Conn)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Player", currentPlayer)
                    cmd.Parameters.AddWithValue("@Date", matchDate)
                    cmd.ExecuteNonQuery()
                End Using

                LOGIT($"ClearPlayerScores: scores cleared for {currentPlayer}")
                ' Clear grid cells
                For i As Integer = 1 To 9
                    r.Cells(i.ToString()).Value = Nothing
                Next
                ' ← Add these
                r.Cells("Gross").Value = DBNull.Value
                r.Cells("Net").Value = DBNull.Value
                r.Cells("phdcp").Value = DBNull.Value

                ' Clear handicap for this date
                Using cmd As New SQLiteCommand("
    DELETE FROM Handicaps 
    WHERE League=@League AND Player=@Player AND Date=@Date", ctx.Conn)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Player", currentPlayer)
                    cmd.Parameters.AddWithValue("@Date", matchDate)
                    cmd.ExecuteNonQuery()
                End Using

                ' Update Last5
                UpdatePlayerLast5(currentPlayer)

                ' Clear Skin/CTP payments for this date/player
                Using cmd As New SQLiteCommand("
    DELETE FROM Payments 
    WHERE League=@League AND Player=@Player AND Date=@Date
    AND Desc IN ('Skin','CTP') AND Detail='Payment'", ctx.Conn)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Player", currentPlayer)
                    cmd.Parameters.AddWithValue("@Date", matchDate)
                    cmd.ExecuteNonQuery()
                End Using

                ' Check if sub and restore original player
                Dim dtSub As New DataTable
                Using cmd As New SQLiteCommand("
                SELECT Team, Grade FROM Subs 
                WHERE League=@League AND Player=@Player AND Date=@Date", ctx.Conn)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Player", currentPlayer)
                    cmd.Parameters.AddWithValue("@Date", matchDate)
                    Using da As New SQLiteDataAdapter(cmd)
                        da.Fill(dtSub)
                    End Using
                End Using

                If dtSub.Rows.Count > 0 Then
                    Dim team As String = dtSub.Rows(0)("Team").ToString()
                    Dim grade As String = dtSub.Rows(0)("Grade").ToString()
                    Dim season As Integer = CInt(matchDate.Substring(0, 4))

                    Dim origPlayer As String = ""
                    Using cmd As New SQLiteCommand("
                    SELECT Player FROM Teams
                    WHERE League=@League AND Team=@Team AND Grade=@Grade AND Year=@Year", ctx.Conn)
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Team", team)
                        cmd.Parameters.AddWithValue("@Grade", grade)
                        cmd.Parameters.AddWithValue("@Year", season)
                        Dim result = cmd.ExecuteScalar()
                        If result IsNot Nothing Then origPlayer = result.ToString().Trim()
                    End Using

                    If Not String.IsNullOrEmpty(origPlayer) Then
                        LOGIT($"Restoring sub {currentPlayer} back to {origPlayer}")
                        Using cmd As New SQLiteCommand("
                        UPDATE Matches SET Player=@New 
                        WHERE League=@League AND Date=@Date AND Player=@Old;
                        UPDATE Matches SET Opponent=@New 
                        WHERE League=@League AND Date=@Date AND Opponent=@Old;
                        DELETE FROM Subs 
                        WHERE League=@League AND Player=@Old AND Date=@Date;", ctx.Conn)
                            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                            cmd.Parameters.AddWithValue("@Date", matchDate)
                            cmd.Parameters.AddWithValue("@Old", currentPlayer)
                            cmd.Parameters.AddWithValue("@New", origPlayer)
                            cmd.ExecuteNonQuery()
                        End Using
                        r.Cells("Player").Value = origPlayer
                    End If
                End If
            Next

            ' --- RESET MATCH POINTS ---
            Using cmd As New SQLiteCommand("
            UPDATE Matches SET Points=0.5, Team_Points=0, Team_Net=NULL
            WHERE League=@League AND Date=@Date
            AND Partner BETWEEN @StartPartner AND @EndPartner", ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", matchDate)
                cmd.Parameters.AddWithValue("@StartPartner", startIndex)
                cmd.Parameters.AddWithValue("@EndPartner", startIndex + 3)
                cmd.ExecuteNonQuery()
            End Using

        Finally
            _suppressEvents = False
        End Try

        ' --- RECALCULATE MATCHES ---
        Dim tm As New TeamMatch()
        tm.CalculateThisMatch(row)

        RefreshScoresGrid()

        ' --- RELOAD ALL TABS ---
        Dim sc = TryCast(dgv.FindForm(), ScoreCard)
        If sc IsNot Nothing Then
            sc.LoadAllTabs()
        End If
    End Sub
    Public Function GetCTPCollected() As Decimal
        Try
            Dim sql As String = "
            SELECT IFNULL(SUM(Earned), 0)
            FROM Payments
            WHERE League = @League
              AND Date = @Date
              AND Desc = 'CTP'
              AND Detail = 'Payment'"
            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Return CDec(cmd.ExecuteScalar())
            End Using
        Catch ex As Exception
            LOGIT("GetCTPCollected Error: " & ex.Message)
            Return 0
        End Try
    End Function
    Public Function GetCTPCarryover(ctpSlot As Integer) As Decimal
        Try
            Dim detail As String = ScoreRulesService.CtpCarryoverDetail(ctpSlot, ctx.sFrontBack)
            Dim sql As String = "
            SELECT IFNULL((
                SELECT Earned
                FROM Payments
                WHERE League = @League
                  AND Player = 'Kitty'
                  AND Desc = 'CTP'
                  AND Detail = @Detail
                  AND Date < @Date
                ORDER BY Date DESC
                LIMIT 1
            ), 0)"
            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Detail", detail)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Return CDec(cmd.ExecuteScalar())
            End Using
        Catch ex As Exception
            LOGIT("GetCTPCarryover Error: " & ex.Message)
            Return 0
        End Try
    End Function
    Public Sub LoadScoresFromView()

        Try
            Dim freshData As New DataTable

            Dim sql As String = "
            SELECT *
            FROM vwMatchesScores
            WHERE Date = @Date
            ORDER BY Partner"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)

                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(freshData)
                End Using
            End Using

            ' Set grid
            If _dgv IsNot Nothing Then
                _dgv.DataSource = freshData
            End If
            ' Sync ScoreCard dtScores
            _dtScores = freshData

            ' Build UI cache
            If _dgv IsNot Nothing AndAlso _dgv.Rows.Count > 0 Then
                BuildScoreCache()
            End If

        Catch ex As Exception
            LOGIT("LoadScoresFromView Error: " & ex.Message)
        End Try

    End Sub

    ' ================= LIGHT REFRESH =================
    Public Sub RefreshScoresGrid()
        Try
            _suppressEvents = True
            _dgv.SuspendLayout()

            ' Refresh phdcp from DB for all rows
            For Each r As DataGridViewRow In _dgv.Rows
                If r.IsNewRow Then Continue For
                Dim p As String = r.Cells("Player").Value?.ToString()
                If String.IsNullOrEmpty(p) Then Continue For
                Using cmd As New SQLiteCommand("
                SELECT Hdcp FROM Handicaps 
                WHERE League=@League AND Player=@Player 
                AND Date < @Date AND Hdcp <> ''
                ORDER BY Date DESC LIMIT 1", ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Player", p)
                    cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso IsNumeric(result) Then
                        r.Cells("phdcp").Value = CInt(result)
                    End If
                End Using
            Next

            BuildScoreCache()
            _dgv.Invalidate()

        Catch ex As Exception
            LOGIT("RefreshScoresGrid Error: " & ex.Message)
        Finally
            _suppressEvents = False
            _dgv.ResumeLayout()
        End Try
    End Sub
    ' --- SCORE CACHE ---
    Public Sub BuildScoreCache()
        If Not _isInitialized OrElse _helper Is Nothing OrElse _dgv Is Nothing Then Exit Sub

        Try
            scoreCache = New Dictionary(Of String, RowScoreInfo)

            ' Always reload SkinMap for the current date — never rely on stale cache
            LoadSkinMap()

            Dim skinMap = SkinMapCache

            ' Determine column names based on season type
            Dim grossColName As String = If(ctx.IsPostSeason, "ThisGross", "Gross")
            Dim hdcpColName As String = If(_dgv.Columns.Contains("phdcp"), "phdcp", "PHdcp")
            Dim otherFB As String = If(ctx.sFrontBack = "Front", "Back", "Front")

            ' ── Pass 1: Build per-player hole info ───────────────────────────────
            For r As Integer = 0 To _dgv.Rows.Count - 1
                Dim dgvRow = _dgv.Rows(r)
                If dgvRow.IsNewRow Then Continue For

                Dim playerKey As String = dgvRow.Cells("Player").Value.ToString().Trim().ToUpper()
                Dim playerName As String = dgvRow.Cells("Player").Value.ToString()
                Dim rowInfo As New RowScoreInfo
                scoreCache(playerKey) = rowInfo
                rowInfo.PlayerName = playerName

                ' Subs only apply in regular season
                If Not ctx.IsPostSeason Then
                    Dim subCheck As String = $"
                SELECT COUNT(*) FROM Subs 
                WHERE League = '{ctx.SafeLeagueName}' 
                  AND Player = '{playerName.Replace("'", "''")}' 
                  AND Date = '{ctx.ActiveDate}'"
                    rowInfo.IsSub = CInt(_helper.SQLiteExecuteScalar(subCheck)) > 0
                Else
                    rowInfo.IsSub = False
                End If

                ' Get handicap
                Dim hdcpVal As Object = Nothing
                If _dgv.Columns.Contains(hdcpColName) Then
                    hdcpVal = dgvRow.Cells(hdcpColName).Value
                End If
                Dim hdcp As Integer = If(IsNumeric(hdcpVal), CInt(hdcpVal), 0)

                ' Pink = missing scores
                Dim grossVal As Object = Nothing
                If _dgv.Columns.Contains(grossColName) Then
                    grossVal = dgvRow.Cells(grossColName).Value
                End If
                Dim isMissingScore As Boolean = (grossVal Is DBNull.Value OrElse
                                             grossVal Is Nothing OrElse
                                             Val(grossVal) = 0)
                If isMissingScore Then
                    If dgvRow.Cells("Player").Style.BackColor <> Color.Aqua Then
                        dgvRow.Cells("Player").Style.BackColor = Color.LightPink
                    End If
                Else
                    If dgvRow.Cells("Player").Style.BackColor = Color.LightPink Then
                        dgvRow.Cells("Player").Style.BackColor = dgvRow.DefaultCellStyle.BackColor
                    End If
                End If

                ' ── This week holes (1-9) ─────────────────────────────────────
                For Each colName In _holeColumns.Values
                    Dim hole As Integer = CInt(colName)
                    Dim info As New HoleInfo
                    Dim sActhole = If(ctx.sFrontBack = "Back", hole + 9, hole)

                    info.StrokeCount = ScoreCard.showStrokeHoles(sActhole, hdcp)
                    info.Par = CInt(_helper.thisCourse($"Hole{sActhole}"))

                    If Not _dgv.Columns.Contains(colName) Then
                        rowInfo.Holes(hole) = info
                        Continue For
                    End If

                    Dim cellVal = dgvRow.Cells(colName).Value
                    If IsNumeric(cellVal) AndAlso CInt(cellVal) > 0 Then
                        Dim score As Integer = CInt(cellVal)
                        info.Score = score
                        info.IsBirdie = (score = info.Par - 1)
                        info.IsEagle = (score <= info.Par - 2)
                        info.IsUnderPar = (score < info.Par)

                        Dim parMaxKey As String = $"Par{info.Par}Max"
                        Dim parMax As Integer = CInt(ctx.rLeagueParmrow(parMaxKey))
                        info.IsCapped = (score > parMax AndAlso parMax < 10)

                        If skinMap.ContainsKey(playerName) AndAlso skinMap(playerName).Contains(sActhole) Then
                            info.IsSkinWinner = True
                        End If
                    End If

                    rowInfo.Holes(hole) = info
                Next

                ' ── Other week holes (O1-O9) postseason only ──────────────────
                If ctx.IsPostSeason Then
                    For Each colName In _holeColumns.Values
                        Dim hole As Integer = CInt(colName)
                        Dim otherColName As String = $"O{hole}"
                        If Not _dgv.Columns.Contains(otherColName) Then Continue For

                        Dim sActhole = If(otherFB = "Back", hole + 9, hole)
                        Dim info As New HoleInfo
                        info.StrokeCount = ScoreCard.showStrokeHoles(sActhole, hdcp)
                        info.Par = CInt(_helper.thisCourse($"Hole{sActhole}"))

                        Dim cellVal = dgvRow.Cells(otherColName).Value
                        If IsNumeric(cellVal) AndAlso CInt(cellVal) > 0 Then
                            Dim score As Integer = CInt(cellVal)
                            info.Score = score
                            info.IsBirdie = (score = info.Par - 1)
                            info.IsEagle = (score <= info.Par - 2)
                            info.IsUnderPar = (score < info.Par)

                            Dim parMaxKey As String = $"Par{info.Par}Max"
                            Dim parMax As Integer = CInt(ctx.rLeagueParmrow(parMaxKey))
                            info.IsCapped = (score > parMax AndAlso parMax < 10)

                            If skinMap.ContainsKey(playerName) AndAlso skinMap(playerName).Contains(sActhole) Then
                                info.IsSkinWinner = True
                            End If
                        End If

                        ' Store with 100+ offset to avoid conflict with this week
                        rowInfo.Holes(hole + 100) = info
                    Next
                End If
            Next

            ' ── Pass 2: Tie detection — net scores, skins players only ────────────
            For Each colName In _holeColumns.Values
                Dim hole As Integer = CInt(colName)
                Dim sActhole = If(ctx.sFrontBack = "Back", hole + 9, hole)
                Dim holeNetScores As New Dictionary(Of String, Integer)

                For r As Integer = 0 To _dgv.Rows.Count - 1
                    Dim dgvRow = _dgv.Rows(r)
                    If dgvRow.IsNewRow Then Continue For

                    Dim skinsVal As String = "N"
                    If _dgv.Columns.Contains("InSkins") Then
                        skinsVal = If(dgvRow.Cells("InSkins").Value, "N").ToString().Trim().ToUpper()
                    End If
                    If skinsVal <> "Y" Then Continue For

                    If Not _dgv.Columns.Contains(colName) Then Continue For
                    Dim cellVal = dgvRow.Cells(colName).Value
                    If Not IsNumeric(cellVal) OrElse CInt(cellVal) <= 0 Then Continue For

                    Dim playerKey As String = dgvRow.Cells("Player").Value.ToString().Trim().ToUpper()
                    If Not scoreCache.ContainsKey(playerKey) Then Continue For

                    Dim info As HoleInfo = Nothing
                    If Not scoreCache(playerKey).Holes.TryGetValue(hole, info) Then Continue For

                    Dim hdcp As Integer = 0
                    If _dgv.Columns.Contains(hdcpColName) Then
                        Dim hdcpVal = dgvRow.Cells(hdcpColName).Value
                        hdcp = If(IsNumeric(hdcpVal), CInt(hdcpVal), 0)
                    End If

                    Dim netScore As Integer = info.Score - ScoreCard.showStrokeHoles(sActhole, hdcp)
                    holeNetScores(playerKey) = netScore
                Next

                If holeNetScores.Count < 2 Then Continue For

                Dim lowest As Integer = Integer.MaxValue
                For Each netScore In holeNetScores.Values
                    If netScore < lowest Then lowest = netScore
                Next

                Dim tied As New List(Of String)
                For Each kv In holeNetScores
                    If kv.Value = lowest Then tied.Add(kv.Key)
                Next

                If tied.Count > 1 Then
                    For Each pk In tied
                        Dim info As HoleInfo = Nothing
                        If scoreCache.ContainsKey(pk) AndAlso scoreCache(pk).Holes.TryGetValue(hole, info) Then
                            info.IsTie = True
                            info.IsSkinWinner = False
                        End If
                    Next
                ElseIf tied.Count = 1 Then
                    Dim winnerKey As String = tied(0)
                    For Each pk In holeNetScores.Keys
                        Dim info As HoleInfo = Nothing
                        If scoreCache.ContainsKey(pk) AndAlso scoreCache(pk).Holes.TryGetValue(hole, info) Then
                            info.IsSkinWinner = False
                        End If
                    Next
                    Dim winInfo As HoleInfo = Nothing
                    If scoreCache.ContainsKey(winnerKey) AndAlso scoreCache(winnerKey).Holes.TryGetValue(hole, winInfo) Then
                        winInfo.IsSkinWinner = True
                    End If
                End If
            Next

        Catch ex As Exception
            LOGIT("BuildScoreCache Error: " & ex.Message)
        End Try
    End Sub
    ' --- PLAYER CHANGE ---
    Sub ChangePlayer(R As DataGridViewRow)

        Dim newPlayer = R.Cells("Player").Value?.ToString()?.Trim()
        If String.IsNullOrEmpty(newPlayer) Then Exit Sub

        If newPlayer.Length < 2 Then
            MessageBox.Show("Player must be at least 2 chars", "Player Error")
            R.Cells("Player").Value = _OldPlayer
            Exit Sub
        End If

        ' 🔥 check if already subbing elsewhere
        Dim sqlCheck As String = "
    SELECT Team 
    FROM Subs 
    WHERE League = @League 
      AND Player = @Player 
      AND Date = @Date"

        Dim pCheck As New Dictionary(Of String, Object) From {
        {"@League", ctx.sLeagueName},
        {"@Player", newPlayer},
        {"@Date", ctx.ActiveDate}
    }

        Dim existingTeamObj = _helper.SQLiteExecuteScalar(sqlCheck, pCheck)

        If existingTeamObj IsNot Nothing AndAlso existingTeamObj.ToString() <> "0" Then

            Dim existingTeam = existingTeamObj.ToString().Trim()
            Dim currentTeam = R.Cells("Team").Value.ToString().Trim()

            If Not existingTeam.Equals(currentTeam, StringComparison.OrdinalIgnoreCase) Then
                MsgBox($"This Player {newPlayer} is already subbing for Team {existingTeam}, Try Again")
                R.Cells("Player").Value = _OldPlayer
                Exit Sub
            End If

        End If

        ' 🔥 handicap
        Dim phdcp As String = "0"
        Try
            phdcp = _helper.GetPHdcp()
        Catch
        End Try

        R.Cells("Phdcp").Value = phdcp

        ' 🔥 color (sub vs normal)
        R.Cells("IsSub").Value = If(newPlayer <> _OldPlayer, "Y", "N")

        If R.Cells("IsSub").Value = "Y" Then
            R.Cells("Player").Style.BackColor = Color.Aqua
        Else
            R.Cells("Player").Style.BackColor = Color.LightPink
        End If

        ' 🔥 next ID
        Dim nextId = CInt(_helper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID),0)+1 FROM Subs"))

        ' 🔥 DB updates
        Dim sql As String = "
    DELETE FROM Subs 
    WHERE League = @League AND Player = @Player AND Date = @Date;

    INSERT INTO Subs (ID, League, Player, Date, Team, Grade)
    VALUES (@ID, @League, @Player, @Date, @Team, @Grade);

    UPDATE Handicaps 
    SET Player = @Player, Phdcp = @Phdcp 
    WHERE League = @League AND Date = @Date AND Player = @OldPlayer;

    UPDATE Matches 
    SET Player = @Player 
    WHERE League = @League AND Date = @Date AND Player = @OldPlayer;

    UPDATE Matches 
    SET Opponent = @Player 
    WHERE League = @League AND Date = @Date AND Opponent = @OldPlayer;
    "

        Dim p As New Dictionary(Of String, Object) From {
        {"@ID", nextId},
        {"@League", ctx.sLeagueName},
        {"@Player", newPlayer},
        {"@Date", ctx.ActiveDate},
        {"@Team", R.Cells("Team").Value},
        {"@Grade", R.Cells("Grade").Value},
        {"@OldPlayer", _OldPlayer},
        {"@Phdcp", phdcp}
    }

        Try
            _helper.SqliteExec(sql, p)
            LOGIT($"Swapped {_OldPlayer} → {newPlayer}")
            For Each dr As DataRow In _dtScores.Rows

                If dr("Player").ToString().Trim().Equals(_OldPlayer, StringComparison.OrdinalIgnoreCase) Then
                    dr("Player") = newPlayer
                End If

                If dr("Opponent").ToString().Trim().Equals(_OldPlayer, StringComparison.OrdinalIgnoreCase) Then
                    dr("Opponent") = newPlayer
                End If

            Next

        Catch ex As Exception
            LOGIT($"Swap failed: {ex.Message}")
            MsgBox("Database update failed.", MsgBoxStyle.Critical)
        End Try

    End Sub
    Private Sub UpdateClearButtonText(dgv As DataGridView, row As DataGridViewRow)
        If row Is Nothing Then Exit Sub
        Dim player = row.Cells("Player").Value?.ToString()?.Trim()
        Dim tp = dgv.Parent
        Dim btn As Button = Nothing
        For Each ctrl As Control In tp.Controls
            If TypeOf ctrl Is Button AndAlso ctrl.Name = "btnClearScores" Then
                btn = DirectCast(ctrl, Button)
                Exit For
            End If
        Next
        If btn Is Nothing Then Exit Sub
        If String.IsNullOrEmpty(player) Then
            btn.Text = "Clear Scores"
        Else
            btn.Text = $"Clear {player}"
        End If
    End Sub

    Public Function GetPlayersForDate() As List(Of String)
        Dim players As New List(Of String)
        Try
            Dim sql As String = $"
            SELECT DISTINCT Player 
            FROM Payments
            WHERE League = '{ctx.SafeLeagueName}'
              AND Date = {ctx.ActiveDate}
              AND Desc = 'CTP'
              AND Detail = 'Payment'
            ORDER BY Player"

            LOGIT($"GetPlayersForDate SQL: {sql}")

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim p = rdr("Player").ToString()
                        LOGIT($"GetPlayersForDate found: {p}")
                        players.Add(p)
                    End While
                End Using
            End Using

            LOGIT($"GetPlayersForDate total: {players.Count}")

        Catch ex As Exception
            LOGIT("GetPlayersForDate Error: " & ex.Message)
        End Try
        Return players
    End Function

    Public Sub UpdatePrizeMoney(Optional suppressDuesPrompt As Boolean = False)
        Dim tc = ctx.tabControl
        Dim tp = tc.TabPages("PrizeMoney")
        Dim dgv = DirectCast(tp.Controls("dgPrizeMoney"), DataGridView)

        ' Reset all numeric amounts to 0 before re-accumulating
        For Each dr As DataGridViewRow In dgv.Rows
            For Each col As DataGridViewColumn In dgv.Columns
                If TypeOf dgv.DataSource Is DataTable Then
                    Dim dt = DirectCast(dgv.DataSource, DataTable)
                    If dt.Columns.Contains(col.Name) AndAlso
                   dt.Columns(col.Name).DataType = GetType(Decimal) Then
                        dr.Cells(col.Index).Value = 0D
                    End If
                End If
            Next
        Next

        ' Get payments for this league/date
        _helper.dt = _helper.sqlitedaFromSql(ctx.Conn, "", $"SELECT * FROM Payments WHERE League||Date = '{ctx.SafeLeagueName}{ctx.ActiveDate}'")

        LOGIT($"UpdatePrizeMoney payment rows: {_helper.dt.Rows.Count}")

        ' Update earned amounts in prize money table
        For Each prow As DataRow In _helper.dt.Rows
            UpdPMAmounts(prow)
        Next

        ' Grid column visibility/width
        Dim smatchflds As String = "Category-60,Prior,Current-60,TotalAmount-80,TotalNum-65,Collected-60,Leftover-60"
        Dim fields = smatchflds.Split(","c)

        dgv.CurrentCell = Nothing
        dgv.ClearSelection()

        For Each col As DataGridViewColumn In dgv.Columns
            col.Visible = False
            Dim match As String = Nothing
            For Each f As String In fields
                If f.Split("-"c)(0).Equals(col.Name, StringComparison.OrdinalIgnoreCase) Then
                    match = f
                    Exit For
                End If
            Next
            If match IsNot Nothing Then
                col.Visible = True
                Dim dashIndex = match.IndexOf("-"c)
                If dashIndex > -1 Then
                    Dim widthText = match.Substring(dashIndex + 1)
                    Dim w As Integer
                    If Integer.TryParse(widthText, w) Then col.Width = w
                End If
            End If
        Next

        ' Set focus to first visible cell
        If dgv.Rows.Count > 0 Then
            For Each col As DataGridViewColumn In dgv.Columns
                If col.Visible Then
                    dgv.CurrentCell = dgv.Rows(0).Cells(col.Index)
                    Exit For
                End If
            Next
        End If
        ' Determine if post season
        Dim bPostSeason As Boolean = ctx.ActiveDate >= ctx.sPSDate

        ' Get amounts from LeagueParms
        Dim mSkins As Decimal = If(bPostSeason, CDec(ctx.rLeagueParmrow("SkinsPS")), CDec(ctx.rLeagueParmrow("Skins")))
        Dim mCTP As Decimal = If(bPostSeason, CDec(ctx.rLeagueParmrow("ClosestPS")), CDec(ctx.rLeagueParmrow("Closest")))

    End Sub

    ' --- PAYMENTS ---
    Public Sub MakePmtAmount(desc As String, amount As Decimal, Optional comment As String = "")
        Try
            Dim sb As New StringBuilder
            sb.AppendLine("INSERT INTO Payments (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)")
            sb.AppendLine("VALUES (@League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, @PayMethod)")

            Dim p As New Dictionary(Of String, Object) From {
            {"@League", ctx.sLeagueName},
            {"@Player", ctx.sPlayer},
            {"@Date", ctx.ActiveDate},
            {"@Desc", desc},
            {"@Detail", "Payment"},
            {"@Earned", amount},
            {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
            {"@Comment", ""},
            {"@PayMethod", ""}
        }
            _helper.SqliteExec(sb.ToString(), p)
            LOGIT($"MakePmtAmount: {ctx.sPlayer} paid {amount:C2} for {desc}")
        Catch ex As Exception
            LOGIT($"MakePmtAmount Error: {ex.Message}")
        End Try
    End Sub

    Public Sub MakePmt(sSkinCTP As String)
        Try
            _helper.dt = _helper.sqlitedaFromSql(ctx.Conn, "", "SELECT * FROM Payments ORDER BY ID DESC LIMIT 1")
            Dim sID As Integer = 1
            If _helper.dt.Rows.Count > 0 Then
                sID = CInt(_helper.dt.Rows(0)(0)) + 1
            End If
            Dim dpmtamt As Decimal = 0
            Dim isPostSeason As Boolean = (ctx.ActiveDate >= _helper.sPSDate)
            Select Case True
                Case sSkinCTP.ToLower.Contains("eoy skins")
                    dpmtamt = CDec(ctx.rLeagueParmrow("EOY Skins"))
                Case sSkinCTP.ToLower.Contains("ctp")
                    RecalculateCTPCarryovers()
                    dpmtamt = If(isPostSeason, CDec(ctx.rLeagueParmrow("ClosestPS")), CDec(ctx.rLeagueParmrow(Constants.closest)))
                Case sSkinCTP.ToLower.Contains("skin")
                    dpmtamt = If(isPostSeason, CDec(ctx.rLeagueParmrow("SkinsPS")), CDec(ctx.rLeagueParmrow(Constants.skin)))
                    RecalculateSkinCarryovers()
                Case sSkinCTP.ToLower.Contains("league dues")
                    dpmtamt = CDec(ctx.rLeagueParmrow("Cost"))
            End Select

            Dim sql As String = "
            INSERT INTO Payments
            (ID, League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
            VALUES
            (@ID, @League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, @PayMethod)"
            Dim p As New Dictionary(Of String, Object) From {
            {"@ID", sID},
            {"@League", ctx.sLeagueName},
            {"@Player", ctx.sPlayer},
            {"@Date", ctx.ActiveDate},
            {"@Desc", sSkinCTP},
            {"@Detail", "Payment"},
            {"@Earned", dpmtamt},
            {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
            {"@Comment", ""},
            {"@PayMethod", ""}
        }
            _helper.SqliteExec(sql, p)
            LOGIT($"MakePmt SUCCESS: {ctx.sPlayer} paid {dpmtamt:C2} for {sSkinCTP}")
            If sSkinCTP.ToLower.Contains("ctp") Then
                RecalculateCTPCarryovers()
            End If
        Catch ex As Exception
            LOGIT("Error in MakePmt: " & ex.Message)
        Finally
            _helper.wk = ""
        End Try
    End Sub
    Public Sub RecalculateCTPCarryovers()
        Try
            Dim conn = ctx.Conn
            If conn.State <> ConnectionState.Open Then conn.Open()

            ' --- TOTAL COLLECTED FOR CTP THIS DATE ---
            Dim collectSql As String = "
            SELECT IFNULL(SUM(Earned), 0)
            FROM Payments
            WHERE League = @League
              AND Date = @Date
              AND Desc = 'CTP'
              AND Detail = 'Payment'"
            Dim totalCollected As Decimal = 0
            Using cmd As New SQLiteCommand(collectSql, conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                totalCollected = CDec(cmd.ExecuteScalar())
            End Using

            Dim par3Count As Integer = If(ctx.lPar3s IsNot Nothing AndAlso ctx.lPar3s.Count > 0, ctx.lPar3s.Count, 1)
            Dim thisWeekSlotPot As Decimal = totalCollected / par3Count

            Using trans = conn.BeginTransaction()
                For slotIndex As Integer = 0 To ctx.lPar3s.Count - 1
                    Dim par3Hole As String = ctx.lPar3s(slotIndex)
                    Dim slot As Integer = slotIndex + 1
                    Dim carryoverDetail As String = ScoreRulesService.CtpCarryoverDetail(slot, ctx.sFrontBack)

                    ' --- GET LATEST PRIOR CARRYOVER FOR THIS SLOT ---
                    Dim carrySql As String = "
                    SELECT IFNULL((
                        SELECT Earned
                        FROM Payments
                        WHERE League = @League
                          AND Player = 'Kitty'
                          AND Desc = 'CTP'
                          AND Detail = @Detail
                          AND Date < @Date
                        ORDER BY Date DESC
                        LIMIT 1
                    ), 0)"
                    Dim priorCarry As Decimal = 0
                    Using cmd As New SQLiteCommand(carrySql, conn)
                        cmd.Transaction = trans
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Detail", carryoverDetail)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        priorCarry = CDec(cmd.ExecuteScalar())
                    End Using

                    ' --- DELETE TODAY'S EXISTING CARRYOVER FOR THIS SLOT ---
                    Dim delSql As String = "
                    DELETE FROM Payments
                    WHERE League = @League
                      AND Player = 'Kitty'
                      AND Date = @Date
                      AND Desc = 'CTP'
                      AND Detail = @Detail"
                    Using cmd As New SQLiteCommand(delSql, conn)
                        cmd.Transaction = trans
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.Parameters.AddWithValue("@Detail", carryoverDetail)
                        cmd.ExecuteNonQuery()
                    End Using

                    ' --- CHECK IF WINNER EXISTS TODAY FOR THIS SLOT ---
                    Dim winnerSql As String = "
                    SELECT IFNULL(Earned, 0) FROM Payments
                    WHERE League = @League
                      AND Date = @Date
                      AND Desc = 'CTP'
                      AND Detail = @Detail
                      AND Player <> 'Kitty'"
                    Dim winnerAmount As Decimal = 0
                    Using cmd As New SQLiteCommand(winnerSql, conn)
                        cmd.Transaction = trans
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.Parameters.AddWithValue("@Detail", $"#{par3Hole}")
                        winnerAmount = CDec(cmd.ExecuteScalar())
                    End Using
                    Dim hasWinner As Boolean = winnerAmount > 0

                    Dim slotTotal As Decimal = thisWeekSlotPot + priorCarry
                    LOGIT($"CTP slot {slot}: thisWeekSlotPot={thisWeekSlotPot} priorCarry={priorCarry} slotTotal={slotTotal} winnerAmount={winnerAmount}")

                    If hasWinner Then
                        ' --- WINNER: INSERT LEFTOVER IF ANY ---
                        Dim leftover As Decimal = slotTotal - winnerAmount
                        If leftover > 0 Then
                            InsertKittyCarryover(conn, trans,
                                             ctx.sLeagueName,
                                             ctx.ActiveDate.ToString(),
                                             leftover, slot)
                            LOGIT($"CTP slot {slot} leftover=${leftover} → Kitty")
                        End If
                    Else
                        ' --- NO WINNER: INSERT FULL CARRYOVER ---
                        If slotTotal > 0 Then
                            InsertKittyCarryover(conn, trans,
                                             ctx.sLeagueName,
                                             ctx.ActiveDate.ToString(),
                                             slotTotal, slot)
                            LOGIT($"CTP slot {slot} carryover updated: ${slotTotal}")
                        End If
                    End If
                Next
                trans.Commit()
            End Using
        Catch ex As Exception
            LOGIT("RecalculateCTPCarryovers Error: " & ex.Message)
        End Try
    End Sub
    Public Sub RecalculateSkinCarryovers()
        Try
            ' --- TOTAL COLLECTED FOR SKINS THIS DATE ---
            Dim totalCollected As Decimal = CDec(_helper.SQLiteExecuteScalar("
            SELECT IFNULL(SUM(Earned), 0) FROM Payments
            WHERE League = @League
            AND Date = @Date
            AND Desc = 'Skin'
            AND Detail = 'Payment'",
            New Dictionary(Of String, Object) From {
                {"@League", ctx.sLeagueName},
                {"@Date", ctx.ActiveDate}
            }))

            ' --- SUM ALL PRIOR CARRYOVERS ---
            Dim priorCarry As Decimal = CDec(_helper.SQLiteExecuteScalar("
            SELECT IFNULL(SUM(Earned), 0) FROM Payments
            WHERE League = @League
            AND Player = 'Kitty'
            AND Desc = 'Skin'
            AND Detail = 'CarryoverSkins'
            AND Date < @Date",
            New Dictionary(Of String, Object) From {
                {"@League", ctx.sLeagueName},
                {"@Date", ctx.ActiveDate}
            }))

            Dim skinTotal As Decimal = totalCollected + priorCarry
            LOGIT($"RecalculateSkinCarryovers: totalCollected={totalCollected} priorCarry={priorCarry} skinTotal={skinTotal}")

            Dim conn = ctx.Conn
            If conn.State <> ConnectionState.Open Then conn.Open()
            Using trans = conn.BeginTransaction()
                ' --- DELETE TODAY'S EXISTING CARRYOVER ---
                Dim delSql As String = "
                DELETE FROM Payments
                WHERE League = @League
                  AND Player = 'Kitty'
                  AND Date = @Date
                  AND Desc = 'Skin'
                  AND Detail = 'CarryoverSkins'"
                Using cmd As New SQLiteCommand(delSql, conn)
                    cmd.Transaction = trans
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                    cmd.ExecuteNonQuery()
                End Using

                ' --- INSERT UPDATED CARRYOVER ---
                If skinTotal > 0 Then
                    Dim sql As String = "
                    INSERT INTO Payments
                    (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                    VALUES
                    (@League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, @PayMethod)"
                    Dim p As New Dictionary(Of String, Object) From {
                    {"@League", ctx.sLeagueName},
                    {"@Player", "Kitty"},
                    {"@Date", ctx.ActiveDate},
                    {"@Desc", "Skin"},
                    {"@Detail", "CarryoverSkins"},
                    {"@Earned", skinTotal},
                    {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
                    {"@Comment", "Skins carryover"},
                    {"@PayMethod", ""}
                }
                    _helper.SqliteExec(sql, p, trans)
                    LOGIT($"Skin carryover updated: ${skinTotal}")
                End If
                trans.Commit()
            End Using
        Catch ex As Exception
            LOGIT("RecalculateSkinCarryovers Error: " & ex.Message)
        End Try
    End Sub
    Public Sub AwardSkins()
        Try
            If _dtScores Is Nothing OrElse _dtScores.Rows.Count = 0 Then
                LOGIT("AwardSkins: no dtScores data, skipping")
                Exit Sub
            End If

            ' Use CalculateSkinWinners directly
            Dim skinWinners = CalculateSkinWinners(_dtScores)
            LOGIT($"AwardSkins: {skinWinners.Count} winning holes found")

            ' Build winningHoles from skinWinners
            Dim winningHoles As New Dictionary(Of Integer, String)
            For Each kv In skinWinners
                If kv.Value.Count = 1 Then
                    Dim winnerRow As DataRow = _dtScores.Rows(kv.Value(0))
                    winningHoles(kv.Key) = winnerRow("Player").ToString()
                End If
            Next

            LOGIT($"AwardSkins: {winningHoles.Count} outright winners")

            ' --- GET FULL POT ---
            Dim season As String = ctx.rLeagueParmrow("Season").ToString()

            Dim totalCollected As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned), 0) FROM Payments
            WHERE League=@League AND Desc='Skin' AND Detail='Payment'
            AND SUBSTR(Date,1,4)=@Season
            AND Date<=@Date", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                totalCollected = CDec(cmd.ExecuteScalar())
            End Using

            Dim paidOut As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned), 0) FROM Payments
            WHERE League=@League AND Desc='Skin' AND Detail LIKE '#%'
            AND SUBSTR(Date,1,4)=@Season
            AND Date<@Date", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                paidOut = CDec(cmd.ExecuteScalar())
            End Using

            Dim fullPot As Decimal = totalCollected - paidOut
            Dim perHolePot As Decimal = 0
            Dim leftover As Decimal = fullPot

            If winningHoles.Count > 0 Then
                perHolePot = Math.Floor(fullPot / winningHoles.Count)
                leftover = fullPot - (perHolePot * winningHoles.Count)
            End If

            LOGIT($"AwardSkins: totalCollected={totalCollected} paidOut={paidOut} fullPot={fullPot} perHolePot={perHolePot} winners={winningHoles.Count} leftover={leftover}")

            Dim conn = ctx.Conn
            If conn.State <> ConnectionState.Open Then conn.Open()
            Using trans = conn.BeginTransaction()

                ' --- DELETE TODAY'S EXISTING WINNER RECORDS ---
                Using cmd As New SQLiteCommand("
                DELETE FROM Payments
                WHERE League=@League AND Desc='Skin' AND Detail LIKE '#%'
                AND Date=@Date", conn)
                    cmd.Transaction = trans
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                    cmd.ExecuteNonQuery()
                End Using

                ' --- DELETE TODAY'S CARRYOVER ---
                Using cmd As New SQLiteCommand("
                DELETE FROM Payments
                WHERE League=@League AND Player='Kitty'
                AND Desc='Skin' AND Detail='CarryoverSkins'
                AND Date=@Date", conn)
                    cmd.Transaction = trans
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                    cmd.ExecuteNonQuery()
                End Using

                If winningHoles.Count > 0 Then
                    ' --- INSERT WINNER RECORDS ---
                    For Each kv In winningHoles
                        Dim hole As Integer = kv.Key
                        Dim playerName As String = kv.Value
                        Dim sActhole As Integer = If(ctx.sFrontBack = "Back", hole + 9, hole)

                        ' Get net score for comment
                        Dim netScore As Integer = 0
                        For Each dr As DataRow In _dtScores.Rows
                            If dr("Player").ToString() = playerName Then
                                Dim gross As Integer = If(IsNumeric(dr(hole.ToString())), CInt(dr(hole.ToString())), 0)
                                Dim hdcp As Integer = If(IsNumeric(dr("phdcp")), CInt(dr("phdcp")), 0)
                                netScore = gross - ScoreCard.showStrokeHoles(sActhole, hdcp)
                                Exit For
                            End If
                        Next

                        Dim sql As String = "
                        INSERT INTO Payments
                        (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                        VALUES
                        (@League, @Player, @Date, 'Skin', @Detail, @Earned, @DatePaid, @Comment, '')"
                        Dim p As New Dictionary(Of String, Object) From {
                        {"@League", ctx.sLeagueName},
                        {"@Player", playerName},
                        {"@Date", ctx.ActiveDate},
                        {"@Detail", $"#{sActhole}"},
                        {"@Earned", perHolePot},
                        {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
                        {"@Comment", $"Score={netScore}"}
                    }
                        _helper.SqliteExec(sql, p, trans)
                        LOGIT($"AwardSkins: {playerName} won hole {hole} for {perHolePot:C2}")
                    Next

                    ' --- INSERT LEFTOVER CARRYOVER IF ANY ---
                    If leftover > 0 Then
                        Dim sql As String = "
                        INSERT INTO Payments
                        (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                        VALUES
                        (@League, @Player, @Date, 'Skin', 'CarryoverSkins', @Earned, @DatePaid, 'Skins leftover', '')"
                        Dim p As New Dictionary(Of String, Object) From {
                        {"@League", ctx.sLeagueName},
                        {"@Player", "Kitty"},
                        {"@Date", ctx.ActiveDate},
                        {"@Earned", leftover},
                        {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)}
                    }
                        _helper.SqliteExec(sql, p, trans)
                        LOGIT($"AwardSkins: leftover={leftover:C2} → Kitty")
                    End If

                Else
                    ' --- NO WINNERS — FULL POT CARRIES OVER ---
                    If fullPot > 0 Then
                        Dim sql As String = "
                        INSERT INTO Payments
                        (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                        VALUES
                        (@League, @Player, @Date, 'Skin', 'CarryoverSkins', @Earned, @DatePaid, 'Skins no winners', '')"
                        Dim p As New Dictionary(Of String, Object) From {
                        {"@League", ctx.sLeagueName},
                        {"@Player", "Kitty"},
                        {"@Date", ctx.ActiveDate},
                        {"@Earned", fullPot},
                        {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)}
                    }
                        _helper.SqliteExec(sql, p, trans)
                        LOGIT($"AwardSkins: no winners fullPot={fullPot:C2} → Kitty")
                    End If
                End If

                trans.Commit()
            End Using

            LoadSkinMap()

        Catch ex As Exception
            LOGIT("AwardSkins Error: " & ex.Message)
        End Try
    End Sub

    Public Sub AwardCTP(playerName As String, hole As String, amount As Decimal, ctpSlot As Integer)
        Try
            Dim conn = ctx.Conn
            If conn.State <> ConnectionState.Open Then conn.Open()
            Using trans = conn.BeginTransaction()
                ' --- INSERT CTP WINNER ---
                Dim sql As String = "
                INSERT INTO Payments
                (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
                VALUES
                (@League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, @PayMethod)"
                Dim p As New Dictionary(Of String, Object) From {
                {"@League", ctx.sLeagueName},
                {"@Player", playerName},
                {"@Date", ctx.ActiveDate},
                {"@Desc", "CTP"},
                {"@Detail", "#" & hole},
                {"@Earned", amount},
                {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
                {"@Comment", "CTP Winner"},
                {"@PayMethod", ""}
            }
                _helper.SqliteExec(sql, p, trans)
                LOGIT($"AwardCTP SUCCESS: {playerName} won CTP hole {hole} for {amount:C2}")
                trans.Commit()
            End Using
            ' Let RecalculateCTPCarryovers handle all carryover logic
            RecalculateCTPCarryovers()
        Catch ex As Exception
            LOGIT("AwardCTP Error: " & ex.Message)
        End Try
    End Sub
    Sub UpdPMAmounts(row As DataRow)
        Try
            LOGIT($"UpdPMAmounts: Desc={row("Desc")} Detail={row("Detail")} Player={row("Player")} Earned={row("Earned")}")
            Dim sc = TryCast(_dgv.FindForm(), ScoreCard)
            If sc Is Nothing OrElse sc.dtPM Is Nothing Then
                LOGIT("UpdPMAmounts: ScoreCard instance or dtPM is Nothing")
                Exit Sub
            End If

            Dim sPar3s As List(Of String) = ctx.lPar3s
            Dim sdiv As Decimal = If(sPar3s.Count > 0, sPar3s.Count, 1)

            Select Case row("Desc").ToString().ToUpper()

                Case "CTP"
                    LOGIT($"CTP branch — hole={row("Detail")} par3s={String.Join(",", sPar3s)}")
                    If row("Detail").ToString().StartsWith("#") Then
                        Dim hole As Integer = CInt(row("Detail").ToString().Replace("#", ""))
                        For i = 1 To sPar3s.Count
                            If sPar3s(i - 1) = hole.ToString() Then
                                Dim drfind As DataRow = sc.dtPM.Rows.Find($"CTP{i}")
                                If drfind IsNot Nothing Then
                                    drfind("Current") += CDec(row("Earned"))
                                    drfind("TotalAmount") += CDec(row("Earned"))
                                    drfind("TotalNum") += 1
                                End If
                                Exit For
                            End If
                        Next
                    ElseIf row("Player").ToString() <> "Kitty" Then
                        LOGIT($"CTP collected branch — Player={row("Player")} Earned={row("Earned")} sdiv={sdiv}")
                        Dim share As Decimal = CDec(row("Earned")) / sdiv
                        For i = 1 To sPar3s.Count
                            Dim drfind As DataRow = sc.dtPM.Rows.Find($"CTP{i}")
                            If drfind IsNot Nothing Then
                                drfind("Collected") += share
                            End If
                        Next
                    End If

                    ' ← Recalculate leftover for ALL CTP slots after every payment row
                    For i = 1 To sPar3s.Count
                        Dim drfind As DataRow = sc.dtPM.Rows.Find($"CTP{i}")
                        If drfind IsNot Nothing Then
                            drfind("Leftover") = drfind("Collected") - drfind("Current")
                        End If
                    Next

                Case "SKIN"
                    Dim drSkin As DataRow = sc.dtPM.Rows.Find("Skin")
                    If drSkin IsNot Nothing Then
                        If row("Detail").ToString().StartsWith("#") Then
                            drSkin("Current") += CDec(row("Earned"))
                            drSkin("TotalAmount") += CDec(row("Earned"))
                            drSkin("TotalNum") += 1
                        ElseIf row("Player").ToString() <> "Kitty" Then
                            drSkin("Collected") += CDec(row("Earned"))
                        End If
                        ' ← Always recalculate leftover after every row
                        drSkin("Leftover") = drSkin("Collected") - drSkin("Current")
                    Else
                        LOGIT("UpdPMAmounts: no Skin row found in dtPM")
                    End If

            End Select

            _helper.wk = ""

        Catch ex As Exception
            LOGIT($"UpdPMAmounts Error: {ex.Message} | Desc={row("Desc")} Detail={row("Detail")}")
        End Try
    End Sub
    Public Sub BuildAllPlayerLast5()

        Dim sqlPlayers As String = "
    SELECT DISTINCT Player
    FROM Scores
    WHERE League = @League;"

        Dim dtPlayers As New DataTable

        Using cmd As New SQLiteCommand(sqlPlayers, ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)

            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(dtPlayers)
            End Using
        End Using

        If dtPlayers.Rows.Count = 0 Then Exit Sub

        ' Optional: clear table first (safe rebuild)
        _helper.SqliteExec("DELETE FROM PlayerLast5 WHERE League = @League",
        New Dictionary(Of String, Object) From {
            {"@League", ctx.SafeLeagueName}
        })

        For Each r As DataRow In dtPlayers.Rows
            Dim player = r("Player").ToString()
            UpdatePlayerLast5(player)
        Next

    End Sub
    Public Sub UpdatePlayerLast5(player As String)
        ' Delete existing records for this player/league first
        Dim del As String = "DELETE FROM PlayerLast5 WHERE League=@League AND Player=@Player"
        Dim delParams As New Dictionary(Of String, Object) From {
        {"@League", ctx.sLeagueName},
        {"@Player", player}
    }
        _helper.SqliteExec(del, delParams)

        ' Get all scores for this player ordered ascending
        Dim dt As New DataTable
        Dim sql As String = "
        SELECT Date, Gross
        FROM Scores
        WHERE League = @League
          AND Player = @Player
          AND Gross IS NOT NULL
        ORDER BY Date ASC"
        Using cmd As New SQLiteCommand(sql, ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        If dt.Rows.Count = 0 Then Exit Sub

        ' Insert one row per date with Last5 as of that date
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim scores As New List(Of String)
            Dim startIdx As Integer = Math.Max(0, i - 4)
            For j As Integer = i To startIdx Step -1
                scores.Add(dt.Rows(j)("Gross").ToString())
            Next
            Dim last5 As String = String.Join("-", scores)
            Dim lastDate As String = dt.Rows(i)("Date").ToString()

            Dim ins As String = "
            INSERT INTO PlayerLast5 (League, Player, Last5, LastDate)
            VALUES (@League, @Player, @Last5, @LastDate)"
            Dim p As New Dictionary(Of String, Object) From {
            {"@League", ctx.sLeagueName},
            {"@Player", player},
            {"@Last5", last5},
            {"@LastDate", lastDate}
        }
            _helper.SqliteExec(ins, p)
        Next
    End Sub
    Private Sub EnsureLast5Built()
        Dim count As Integer = CInt(_helper.SQLiteExecuteScalar($"
        SELECT COUNT(*) FROM PlayerLast5 WHERE League='{ctx.SafeLeagueName}'"))
        If count = 0 Then
            BuildAllPlayerLast5()
        End If
    End Sub
    Private Sub RefreshSingleRow(row As DataGridViewRow)

        If row Is Nothing OrElse _dgv Is Nothing Then Exit Sub

        Dim player = row.Cells("Player").Value?.ToString()
        If String.IsNullOrEmpty(player) Then Exit Sub

        Dim dt As New DataTable

        Dim sql As String = "
    SELECT * FROM vwMatchesScores
    WHERE League = @League
      AND Date = @Date
      AND Player = @Player;"

        Using cmd As New SQLiteCommand(sql, ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
            cmd.Parameters.AddWithValue("@Player", player)

            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        If dt.Rows.Count = 0 Then Exit Sub

        Dim newRow = dt.Rows(0)

        _suppressEvents = True

        Try
            For Each col As DataGridViewColumn In _dgv.Columns
                Dim colName = col.Name

                If dt.Columns.Contains(colName) Then
                    row.Cells(colName).Value = If(IsDBNull(newRow(colName)), Nothing, newRow(colName))
                End If
            Next
        Finally
            _suppressEvents = False
        End Try

    End Sub
    Private Sub RefreshMatchesForPlayer(player As String)

        If String.IsNullOrEmpty(player) Then Exit Sub

        ' Find Matches grid
        Dim tc = ctx.tabControl
        If tc Is Nothing Then Exit Sub

        Dim tp = tc.TabPages("Matches")
        If tp Is Nothing Then Exit Sub

        Dim dgv = TryCast(tp.Controls("dgMatches"), DataGridView)
        If dgv Is Nothing OrElse dgv.DataSource Is Nothing Then Exit Sub

        Dim dt = TryCast(dgv.DataSource, DataTable)
        If dt Is Nothing Then Exit Sub

        ' Find updated data from dtScores
        Dim src As DataRow = Nothing

        For Each r As DataRow In _dtScores.Rows
            If r("Player").ToString() = player AndAlso r("Date").ToString() = ctx.ActiveDate Then
                src = r
                Exit For
            End If
        Next

        If src Is Nothing Then Exit Sub

        ' Update Matches row
        For Each row As DataRow In dt.Rows
            If row("Player").ToString() = player Then

                row("Team") = src("Team")
                row("Grade") = src("Grade")
                row("Net") = src("Net")
                row("PHdcp") = src("PHdcp")
                row("Team_Net") = src("Team_Net")
                row("Opponent") = src("Opponent")
                row("Points") = src("Points")
                row("Team_Points") = src("Team_Points")

            End If
        Next

    End Sub
    ' --- TEAM MATCH ---
    Public Class TeamMatch
        Public Property MatchList As List(Of matchforlist)
        Private _helper As Helper

        Public Sub New()
            _helper = Main.oHelper
        End Sub

        Private Function calcPoints(val1 As Integer, val2 As Integer) As Decimal
            If val1 < val2 Then Return 1D
            If val1 > val2 Then Return 0D
            Return 0.5D
        End Function

        Private Function SafeDec(val As Object) As Decimal
            If val Is Nothing OrElse IsDBNull(val) Then Return 0D
            Dim d As Decimal
            If Decimal.TryParse(val.ToString(), d) Then Return d
            Return 0D
        End Function
        Public Sub CalculateAllPoints(dtScores As DataTable)
            Try
                If dtScores Is Nothing OrElse dtScores.Rows.Count = 0 Then Exit Sub

                Dim matchesCount As Integer = dtScores.Rows.Count \ 4
                Dim conn = ctx.Conn
                If conn.State <> ConnectionState.Open Then conn.Open()

                Using trans = conn.BeginTransaction()
                    For m As Integer = 0 To matchesCount - 1
                        Dim baseIdx As Integer = m * 4

                        Dim r1 = dtScores.Rows(baseIdx)
                        Dim r2 = dtScores.Rows(baseIdx + 1)
                        Dim r3 = dtScores.Rows(baseIdx + 2)
                        Dim r4 = dtScores.Rows(baseIdx + 3)
                        LOGIT($"Match {m}: h1={r1("Player")} partner={r1("Partner")} h2={r2("Player")} partner={r2("Partner")} a1={r3("Player")} partner={r3("Partner")} a2={r4("Player")} partner={r4("Partner")}")
                        Dim h1Net = GetMatchNet(r1)
                        Dim h2Net = GetMatchNet(r2)
                        Dim a1Net = GetMatchNet(r3)
                        Dim a2Net = GetMatchNet(r4)

                        Dim h1 = New PlayerScores With {.Name = r1("Player").ToString(), .League = r1("League").ToString(), .MatchDate = r1("Date").ToString(), .Round = r1("Round").ToString(), .Net = h1Net}
                        Dim h2 = New PlayerScores With {.Name = r2("Player").ToString(), .League = r2("League").ToString(), .MatchDate = r2("Date").ToString(), .Round = r2("Round").ToString(), .Net = h2Net}
                        Dim a1 = New PlayerScores With {.Name = r3("Player").ToString(), .League = r3("League").ToString(), .MatchDate = r3("Date").ToString(), .Round = r3("Round").ToString(), .Net = a1Net}
                        Dim a2 = New PlayerScores With {.Name = r4("Player").ToString(), .League = r4("League").ToString(), .MatchDate = r4("Date").ToString(), .Round = r4("Round").ToString(), .Net = a2Net}

                        Dim hNet = h1.Net + h2.Net
                        Dim aNet = a1.Net + a2.Net

                        Dim hTeamPts = calcPoints(hNet, aNet)
                        Dim aTeamPts = calcPoints(aNet, hNet)

                        h1.Points = calcPoints(h1.Net, a1.Net)
                        h2.Points = calcPoints(h2.Net, a2.Net)
                        a1.Points = calcPoints(a1.Net, h1.Net)
                        a2.Points = calcPoints(a2.Net, h2.Net)
                        Dim sql As String = "
    UPDATE Matches
    SET
        Points = CASE Player
            WHEN @H1 THEN @H1Pts
            WHEN @H2 THEN @H2Pts
            WHEN @A1 THEN @A1Pts
            WHEN @A2 THEN @A2Pts
        END,

        Team_Points = CASE Player
            WHEN @H1 THEN @HTeamPts
            WHEN @A1 THEN @ATeamPts
        END,

        -- 🔥 NEW
        Team_Net = CASE Player
            WHEN @H1 THEN @HTeamNet
            WHEN @A1 THEN @ATeamNet
        END,

        Opponent = CASE Player
            WHEN @H1 THEN @H1Opp
            WHEN @H2 THEN @H2Opp
            WHEN @A1 THEN @A1Opp
            WHEN @A2 THEN @A2Opp
        END

    WHERE League = @League
      AND Date = @Date
      AND (Round = @Round OR IFNULL(Round, '') = '')
      AND Player IN (@H1, @H2, @A1, @A2)"

                        Dim p As New Dictionary(Of String, Object) From {
                            {"@H1", h1.Name}, {"@H2", h2.Name}, {"@A1", a1.Name}, {"@A2", a2.Name},
                            {"@H1Pts", h1.Points}, {"@H2Pts", h2.Points}, {"@A1Pts", a1.Points}, {"@A2Pts", a2.Points},
                            {"@HTeamPts", hTeamPts}, {"@ATeamPts", aTeamPts},
                            {"@H1Opp", a1.Name}, {"@H2Opp", a2.Name}, {"@A1Opp", h1.Name}, {"@A2Opp", h2.Name},
                            {"@H1Part", h2.Name}, {"@H2Part", h1.Name}, {"@A1Part", a2.Name}, {"@A2Part", a1.Name},
                            {"@HTeamNet", hNet},
                            {"@ATeamNet", aNet},
                            {"@League", h1.League}, {"@Date", h1.MatchDate}, {"@Round", h1.Round}
                                                                         }
                        _helper.SqliteExec(sql, p, trans)
                    Next

                    trans.Commit()
                End Using

            Catch ex As Exception
                LOGIT("CalculateAllPoints Error: " & ex.Message)
            End Try
        End Sub
        Public Sub CalculateThisMatch(row As DataGridViewRow)
            If row Is Nothing OrElse row.IsNewRow Then Exit Sub

            Dim startIndex As Integer = (row.Index \ 4) * 4
            Dim endIndex As Integer = Math.Min(startIndex + 3, _dgv.Rows.Count - 1)

            Dim matchRows As New List(Of DataGridViewRow)
            For i As Integer = startIndex To endIndex
                Dim r = _dgv.Rows(i)
                If Not r.IsNewRow Then matchRows.Add(r)
            Next

            If matchRows.Count = 0 Then Exit Sub

            ' --- GROUP BY POSITION ---
            Dim team1 As New List(Of DataGridViewRow)
            Dim team2 As New List(Of DataGridViewRow)

            For i As Integer = 0 To matchRows.Count - 1
                If i < 2 Then
                    team1.Add(matchRows(i))
                Else
                    team2.Add(matchRows(i))
                End If
            Next

            ' --- CALCULATE TEAM NETS FROM DB ---
            Dim team1Net As Decimal = 0
            Dim team2Net As Decimal = 0
            Dim team1HasScore As Boolean = False
            Dim team2HasScore As Boolean = False

            ' If A player has no score, team forfeits regardless of B player
            Dim team1ANet As Decimal = GetNetFromDB(team1(0))
            Dim team2ANet As Decimal = If(team2.Count > 0, GetNetFromDB(team2(0)), 999D)

            If team1ANet = 999D Then
                team1Net = 999D
            Else
                For Each r As DataGridViewRow In team1
                    Dim net As Decimal = GetNetFromDB(r)
                    If net < 999D Then
                        team1Net += net
                        team1HasScore = True
                    End If
                Next
                If Not team1HasScore Then team1Net = 999D
            End If

            If team2ANet = 999D Then
                team2Net = 999D
            Else
                For Each r As DataGridViewRow In team2
                    Dim net As Decimal = GetNetFromDB(r)
                    If net < 999D Then
                        team2Net += net
                        team2HasScore = True
                    End If
                Next
                If Not team2HasScore Then team2Net = 999D
            End If

            ' --- DETERMINE TEAM WINNER ---
            Dim team1Wins As Boolean = team1Net < team2Net
            Dim team2Wins As Boolean = team2Net < team1Net
            Dim isTie As Boolean = team1Net = team2Net

            ' --- INDIVIDUAL NETS ---
            Dim h1Net As Decimal = GetNetFromDB(team1(0))
            Dim h2Net As Decimal = If(team1.Count > 1, GetNetFromDB(team1(1)), 999D)
            Dim a1Net As Decimal = If(team2.Count > 0, GetNetFromDB(team2(0)), 999D)
            Dim a2Net As Decimal = If(team2.Count > 1, GetNetFromDB(team2(1)), 999D)

            ' --- PERSIST TO DB ---
            Using tran = ctx.Conn.BeginTransaction()
                Dim sql As String = "
    UPDATE Matches
    SET Points = @Points,
        Team_Points = @TeamPoints,
        Team_Net = @TeamNet
    WHERE League = @League
      AND Date = @Date
      AND Player = @Player"

                Using cmd As New SQLiteCommand(sql, ctx.Conn, tran)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                    cmd.Parameters.Add("@Points", DbType.Decimal)
                    cmd.Parameters.Add("@TeamPoints", DbType.Decimal)
                    cmd.Parameters.Add("@TeamNet", DbType.Object)
                    cmd.Parameters.Add("@Player", DbType.String)

                    For i As Integer = 0 To matchRows.Count - 1
                        Dim r As DataGridViewRow = matchRows(i)
                        If r Is Nothing OrElse r.IsNewRow Then Continue For

                        Dim player As String = r.Cells("Player").Value?.ToString()
                        If String.IsNullOrEmpty(player) Then Continue For

                        Dim isTeam1 As Boolean = (i < 2)
                        Dim isAPlayer As Boolean = (i = 0 OrElse i = 2)

                        ' Individual net and opponent net
                        Dim myNet As Decimal = GetNetFromDB(r)
                        Dim oppNet As Decimal = 999D

                        If isTeam1 Then
                            oppNet = If(i = 0, a1Net, a2Net)
                        Else
                            oppNet = If(i = 2, h1Net, h2Net)
                        End If

                        ' Individual points
                        Dim indPoints As Decimal = 0D
                        If myNet = 999D AndAlso oppNet = 999D Then
                            indPoints = 0.5D
                        ElseIf myNet < oppNet Then
                            indPoints = 1D
                        ElseIf myNet > oppNet Then
                            indPoints = 0D
                        Else
                            indPoints = 0.5D
                        End If

                        ' Team points — only for A player
                        Dim teamPoints As Decimal = 0D
                        If isAPlayer Then
                            If team1Net = 999D AndAlso team2Net = 999D Then
                                teamPoints = 0.5D
                            ElseIf isTie Then
                                teamPoints = 0.5D
                            ElseIf isTeam1 Then
                                teamPoints = If(team1Wins, 1D, 0D)
                            Else
                                teamPoints = If(team2Wins, 1D, 0D)
                            End If
                        End If

                        ' Team_Net — only for A player, NULL for B player
                        ' Store 999 if team forfeited, NULL for B player
                        Dim teamNetForDB As Object = DBNull.Value
                        If isAPlayer Then
                            If isTeam1 Then
                                teamNetForDB = If(team1Net = 999D, CObj(999D),
                                        If(team1HasScore, CObj(team1Net), DBNull.Value))
                            Else
                                teamNetForDB = If(team2Net = 999D, CObj(999D),
                                        If(team2HasScore, CObj(team2Net), DBNull.Value))
                            End If
                        End If

                        cmd.Parameters("@Points").Value = indPoints
                        cmd.Parameters("@TeamPoints").Value = teamPoints
                        cmd.Parameters("@TeamNet").Value = teamNetForDB
                        cmd.Parameters("@Player").Value = player
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tran.Commit()
            End Using
        End Sub
        Private Function GetNetFromDB(r As DataGridViewRow) As Decimal
            Dim p As String = r.Cells("Player").Value?.ToString()
            Try
                Using cmd As New SQLiteCommand("
            SELECT Net FROM Scores 
            WHERE League=@League AND Player=@Player 
            AND Date=@Date
            AND Gross IS NOT NULL AND Gross > 0", ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Player", p)
                    cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) AndAlso CDec(result) > 0 Then
                        Return CDec(result)
                    End If
                End Using
            Catch ex As Exception
                LOGIT($"GetNetFromDB Error: {ex.Message}")
            End Try
            Return 999D
        End Function

        Private Function GetMatchNet(row As DataRow) As Integer
            Dim gross As Integer = If(IsNumeric(row("Gross")), CInt(row("Gross")), 0)
            If gross = 0 Then Return 999
            Dim hdcp As Integer = If(IsNumeric(row("phdcp")), CInt(row("phdcp")), 0)
            Return ScoreRulesService.CalculateNet(gross, hdcp)
        End Function

    End Class

End Module
