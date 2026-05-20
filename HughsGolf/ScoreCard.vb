Imports System.Buffers
Imports System.Data.SQLite
Imports System.Text
Imports PdfSharp.Pdf
Imports PdfSharp.Drawing

Public Class ScoreCard
    Public ohelper As Helper
    Public connection As New SQLiteConnection
    Friend dtScores As DataTable
    Public dvScores = New DataView
    Dim dt As DataTable
    Dim dtTeams As DataTable
    Dim dtMatches As DataTable
    Dim dtView As New DataTable()

    Friend dtPM As DataTable
    Public dtCTP As DataTable
    Dim dtLD As DataTable
    Dim SkinsTotals As DataRow
    Public dgScores As DataGridView

    Dim sb = New StringBuilder
    Dim sDateLastScore As String
    Dim sLeagueName = ""
    Dim cr = vbCrLf
    Dim dCTPWorth As Decimal
    Dim icellpaint As Long
    Dim sOldCellValue As Object
    Dim sWrk As String
    Dim lrc As Integer = 0
    Dim sKeys() As Object
    Dim sskinctpcols As String
    Dim sfb As String
    'lists
    Public sPar3s As List(Of String)
    Dim lskins As List(Of String)
    Dim lallHoles As List(Of String)
    Dim lLowRows = New List(Of String)
    Dim lEOYPlayers As List(Of String)
    Dim lnumClosests As List(Of String)
    Dim changes As New List(Of String)

    'switches
    Public bFormLoad As Boolean = False
    Dim bshowpaint As Boolean = True
    Dim bstrokehole As Boolean = False
    'dim bStableFord As Boolean = False
    Dim bChanges As Boolean = False
    Dim sCTPFlds As String = "CTP1,CTP2"

    ' Prevents CellPainting from firing during bulk updates
    Private _suspendPainting As Boolean = False
    Private _suppressLeaderCheck As Boolean = False
    ' Enables smooth scrolling + flicker-free painting
    Private Sub EnableDoubleBuffering(grid As DataGridView)
        Dim t = grid.GetType()
        Dim pi = t.GetProperty("DoubleBuffered",
        Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        pi.SetValue(grid, True, Nothing)
    End Sub
    Private Sub SetupPostSeasonRecords()
        Dim lastHID As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID), 0) FROM Handicaps"))
        Dim lastSID As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID), 0) FROM Scores"))

        For Each row As DataRow In dtTeams.Rows
            Dim playerName As String = row("Player").ToString()
            Dim sHdcp As String = ohelper.SQLiteExecuteScalar(
                "SELECT Hdcp FROM Handicaps WHERE League = @League AND Player = @Player AND Hdcp <> '' ORDER BY Date DESC LIMIT 1",
                New Dictionary(Of String, Object) From {
                    {"@League", ctx.sLeagueName},
                    {"@Player", playerName}
                }).ToString()

            ' Insert Handicap
            lastHID += 1
            ohelper.SqliteTrans("INSERT INTO Handicaps (ID, League, Player, Date, Hdcp) VALUES (@ID, @Lg, @Pl, @Dt, @H)",
            New Dictionary(Of String, Object) From {{"@ID", lastHID}, {"@Lg", ctx.sLeagueName}, {"@Pl", playerName}, {"@Dt", ctx.ActiveDate}, {"@H", sHdcp}})

            ' Insert Score (Front)
            lastSID += 1
            ohelper.SqliteTrans("INSERT INTO Scores (ID, League, Player, Date, FrontBack) VALUES (@ID, @Lg, @Pl, @Dt, 'Front')",
            New Dictionary(Of String, Object) From {{"@ID", lastSID}, {"@Lg", ctx.sLeagueName}, {"@Pl", playerName}, {"@Dt", ohelper.sPSDate}})

            ' Insert Score (Back) - 7 days later
            lastSID += 1
            Dim nextDate = CDate(ohelper.sPSDate).AddDays(7).ToString("yyyy-MM-dd")
            ohelper.SqliteTrans("INSERT INTO Scores (ID, League, Player, Date, FrontBack) VALUES (@ID, @Lg, @Pl, @Dt, 'Back')",
            New Dictionary(Of String, Object) From {{"@ID", lastSID}, {"@Lg", ctx.sLeagueName}, {"@Pl", playerName}, {"@Dt", nextDate}})
        Next

        dtScores = ohelper.sqliteda("Scores", "SELECT * FROM Scores ORDER BY Date DESC")
        ohelper.dt = ohelper.sqliteda("Handicaps")
    End Sub

    Sub GetMatches()
        ' Fetch the highest current ID from both tables
        ' If the table is empty, we use 0
        Dim lastHandicapID As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID), 0) FROM Handicaps"))
        ' 1. Get starting IDs once to avoid redundant DB hits
        Dim lastSID As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID), 0) FROM Scores"))
        Dim lastMatchID As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID), 0) FROM Matches"))
        dtTeams = ohelper.sqliteda("Teams", $"Select * FROM Teams WHERE SUBSTR(Year,1,4) = '{ctx.SeasonYear}' ORDER BY Year ASC ")
        dtMatches = ohelper.sqliteda("Matches", $"Select DISTINCT Date FROM Matches WHERE Date = '{ctx.ActiveDate}' ORDER BY Date ASC ")

        ' --- REGULAR SEASON / MATCH LOGIC ---
        Dim dtSchRow As DataRow =
            ohelper.sqliteda("Schedule",
                             $"SELECT * FROM Schedule WHERE Date = '{ctx.ActiveDate}'").Rows(0)

        ohelper.iHoleMarker = If(ctx.sFrontBack = "Front", 1, 10)

        'Dim existingMatches As Integer =
        '    CInt(ohelper.SQLiteExecuteScalar(
        '         $"SELECT COUNT(*) FROM Matches WHERE League = '{ctx.safeLeagueName}' AND Date = '{ctx.ActiveDate}'"))
        ' Using Parameters is safer and cleaner
        Dim sql As String = "SELECT COUNT(*) FROM Matches WHERE League = @Lg AND Date = @Dt"

        ' Corrected Dictionary (removed the extra braces around ActiveDate)
        Dim params As New Dictionary(Of String, Object) From {
    {"@Lg", ctx.sLeagueName},
    {"@Dt", ctx.ActiveDate}
}

        ' This will now correctly pass the string values to your helper
        Dim existingMatches As Integer = 0
        Dim result = ohelper.SQLiteExecuteScalar(sql, params)

        If result IsNot Nothing AndAlso Not IsDBNull(result) Then
            existingMatches = Convert.ToInt32(result)
        End If

        If existingMatches = 0 AndAlso ctx.ActiveDate < ctx.sPSDate Then

            ' Initialize Counters
            Dim partnerCounter As Integer = 0
            Dim MID = lastMatchID
            Dim HID = lastHandicapID

            ' Loop through EVERY column in the schedule row
            For Each col As DataColumn In dtSchRow.Table.Columns
                ' 1. DYNAMIC CHECK: Skip metadata; only process numeric match columns (1, 2, 3...)
                Dim matchNum As Integer
                If Not Integer.TryParse(col.ColumnName, matchNum) Then Continue For

                Dim cell As String = dtSchRow(col.ColumnName).ToString()

                ' 2. Skip if the cell is empty or doesn't have the "v" separator
                If String.IsNullOrWhiteSpace(cell) OrElse Not cell.Contains("v") Then Continue For

                ' 3. Parse the Match (e.g., "5v6")
                Dim teams = cell.Split("v"c)
                Dim t1 = teams(0).Trim()
                Dim t2 = teams(1).Trim()

                ' 4. Pull Players from dtTeams
                Try
                    ' Team 1
                    Dim p1A = dtTeams.Select($"Team = {t1} AND Grade = 'A'")(0)("Player").ToString()
                    Dim p1B = dtTeams.Select($"Team = {t1} AND Grade = 'B'")(0)("Player").ToString()
                    ' Team 2
                    Dim p2A = dtTeams.Select($"Team = {t2} AND Grade = 'A'")(0)("Player").ToString()
                    Dim p2B = dtTeams.Select($"Team = {t2} AND Grade = 'B'")(0)("Player").ToString()

                    ' Array of the 4 players involved in this specific foursome
                    Dim matchSetup = {
                        New With {.Pl = p1A, .Opp = p2A},
                        New With {.Pl = p1B, .Opp = p2B},
                        New With {.Pl = p2A, .Opp = p1A},
                        New With {.Pl = p2B, .Opp = p1B}
                    }

                    ' 5. Loop through the 4 players to Insert Records
                    For Each m In matchSetup
                        ' Insert into MATCHES table (with sorting ID 0-23)
                        MID += 1
                        ohelper.SqliteTrans(
                            "INSERT INTO Matches (ID, League, Player, Date, Opponent, Partner) VALUES (@ID, @Lg, @Pl, @Dt, @Opp, @Part)",
                            New Dictionary(Of String, Object) From {
                                {"@ID", MID},
                                {"@Lg", ctx.sLeagueName},
                                {"@Pl", m.Pl},
                                {"@Dt", ctx.ActiveDate},
                                {"@Opp", m.Opp},
                                {"@Part", partnerCounter} ' Your sortable 0-23 ID
                            })

                        ' Increment for the next player (Moves from 0 up to 23)
                        partnerCounter += 1
                    Next
                Catch ex As Exception
                    ' Logs if a team number in the schedule doesn't exist in dtTeams
                    Console.WriteLine($"Skipping match {cell}: {ex.Message}")
                End Try
            Next
        End If

    End Sub

    Public Sub dp(fld As String)
        Debug.Print(fld)
    End Sub

    Function setMaxInDG(R As DataGridViewRow, sHole As String, sScore As Integer) As Integer
        Dim scorewithmax = CInt(ohelper.ChkForMax(sScore, sHole))
        Try
            If sScore > scorewithmax Then
                R.Cells($"{sHole}").Style.BackColor = Color.DarkMagenta
                R.Cells($"{sHole}").Value = scorewithmax
            End If

        Catch ex As Exception

        End Try
        setMaxInDG = scorewithmax
    End Function

    Public Function SafeInt(value As Object) As Object
        ' Treat Nothing, empty string, or whitespace as NULL
        If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString()) Then
            Return DBNull.Value
        End If

        ' If numeric, return integer
        If IsNumeric(value) Then
            Return Convert.ToInt32(value)
        End If

        ' Otherwise return NULL
        Return DBNull.Value
    End Function

    Function Stableford(score As String, hole As String, hdcp As Int16) As String
        'If Not ctx.rLeagueParmrow("Format") = "Stableford" Then Exit Function
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Stableford = 0
        'double bogey,bogey,par,birdie,eagle, double eagle
        Dim spoints As String = "0,1,2,3,4,8"
        Dim spar = ohelper.thisCourse("Hole" & hole) + 2
        Dim spointer = spar - score
        'check stroke index
        Dim isi As Int16 = ohelper.SBPCalcStrokeIndex("Hole" & hole)
        Dim iHdcpStrokes As Int16 = 0
        'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
        'if the handicap > stroke index make color beige
        If hdcp >= isi Then
            iHdcpStrokes += 1
            'if double stroke hole, make color b/a
            If hdcp - ohelper.iHoles >= isi Then iHdcpStrokes += 1
        End If
        score -= iHdcpStrokes
        'check score against max points (par - netscore) 
        If score < 0 Then score = 0
        Try
            Dim x = spoints.Split(",")(0)
            Dim xx = (ohelper.thisCourse("Hole" & hole) + 2)
            'get double bogey max score and compare to max in array
            If (ohelper.thisCourse("Hole" & hole) + 2) - score <= 0 Then
                Stableford = spoints.Split(",")(0)
            Else
                Dim xy = (ohelper.thisCourse("Hole" & hole) + 2) - score
                If xy > UBound(spoints.Split(",")) Then
                    xy = UBound(spoints.Split(","))
                End If
                Stableford = spoints.Split(",")(xy)
            End If

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
        LOGIT(String.Format("calculating hole {0} par {1} score {2} stableford {3}", hole, spar - 2, score, Stableford))
    End Function

    Private Sub Grid_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        Dim grid = DirectCast(sender, DataGridView)
        If grid.IsCurrentCellDirty Then
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub
    Private Sub dgScores_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs)

        If e.RowIndex < 0 Then Exit Sub

        Dim dg = DirectCast(sender, DataGridView)

        If dg.Columns(e.ColumnIndex).Name = "Player" Then
            Dim val = dg.Rows(e.RowIndex).Cells("Player").Value
            EditScoresEngine._OldPlayer = If(val IsNot Nothing, val.ToString().Trim(), "")
        End If

    End Sub

#Region "Cell Painting"
    Sub EraseCell(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        Try
            'erase the cell
            Dim rectangle As New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
            Dim gridBrush As Brush = New SolidBrush(dgScores.GridColor)
            Dim backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
            e.Graphics.FillRectangle(backColorBrush, e.CellBounds)
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub CircleBirdies(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        Try
            Dim newRect As Rectangle = New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
            Using gridBrush As Brush = New SolidBrush(dgScores.GridColor), backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
                Using gridLinePen As Pen = New Pen(gridBrush)
                    ' e.Graphics.FillRectangle(backColorBrush, e.CellBounds)
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1)
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom)
                    e.Graphics.DrawEllipse(Pens.Blue, newRect)
                    'this makes the number red and font bold
                    If e.Value IsNot Nothing Then
                        Dim boldFont As New Drawing.Font("Tahoma", 9, FontStyle.Bold)
                        If e.CellStyle.Font.Strikeout Then boldFont = New Drawing.Font(boldFont.FontFamily, boldFont.Size, FontStyle.Bold Or FontStyle.Strikeout)
                        e.CellStyle.Font = boldFont
                        e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Crimson, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                    End If
                    'e.Handled = True
                End Using
            End Using

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub CircleEagles(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If

        EraseCell(e)
        Dim r As Rectangle = dgScores.GetCellDisplayRectangle(e.RowIndex, e.ColumnIndex, False)
        Dim b As Brush = Brushes.Purple
        Dim p As New Pen(b)

        dgScores.Invalidate()
        Dim g As Drawing.Graphics = dgScores.CreateGraphics()
        g.DrawEllipse(p, r)
        Exit Sub
        Try
            Dim newRect As Rectangle = New Rectangle(e.CellBounds.X + 3, e.CellBounds.Y + 2, e.CellBounds.Width - 8, e.CellBounds.Height - 8)
            Using gridBrush As Brush = New SolidBrush(dgScores.GridColor), backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
                Using gridLinePen As Pen = New Pen(gridBrush)
                    ' e.Graphics.FillRectangle(backColorBrush, e.CellBounds)
                    'e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 8, e.CellBounds.Right - 8, e.CellBounds.Bottom - 8)
                    'e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 8, e.CellBounds.Top, e.CellBounds.Right - 8, e.CellBounds.Bottom)
                    e.Graphics.DrawEllipse(Pens.Blue, newRect)
                    'this makes the number red and font bold
                    If e.Value IsNot Nothing Then
                        Dim boldFont10 As New Drawing.Font("Tahoma", 10, FontStyle.Bold)
                        If e.CellStyle.Font.Strikeout Then boldFont10 = New Drawing.Font("Tahoma", 10, FontStyle.Bold Or FontStyle.Strikeout)
                        e.CellStyle.Font = boldFont10
                        e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Crimson, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                    End If
                    'e.Handled = True
                End Using
            End Using

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub PaintStrokes(e As DataGridViewCellPaintingEventArgs)

        LOGIT(String.Format("Paintstrokes - row {0} Column {1},", e.RowIndex, dgScores.Columns(e.ColumnIndex).Name))
        Try
            Dim iHdcp As String = ohelper.IHdcp
            ohelper.dt = ohelper.sqliteda(connection, "Handicaps")
            ohelper.dt = ohelper.sqlitedaFromSql(connection, "", $"Select PHdcp FROM Handicaps WHERE League = '{ctx.SafeLeagueName}' AND Player = '{ohelper.sPlayer}' AND Date = {ctx.ActiveDate}")
            If ohelper.dt.Rows.Count = 0 Then Exit Sub
            iHdcp = ohelper.dt.Rows(0)("Phdcp")
            iHdcp = ohelper.convDBNulltoSpaces(dgScores.Rows(e.RowIndex).Cells("PHdcp").Value)
            Dim isi As String = ohelper.CalcStrokeIndex(dgScores.Columns(e.ColumnIndex).Name)
            'LOGIT(String.Format("{0} - Hdcp({1})-SI({2})-{3}", dgScores.Rows(e.RowIndex).Cells("Player").Value, IHdcp, isi, dgScores.Columns(e.ColumnIndex).Name)) '  "-" &  & "-" & isi & "-" & isi & "-" & dgScores.Columns(e.ColumnIndex).Name)
            'if the handicap > stroke index make color beige
            If Not IsNumeric(iHdcp) Then Exit Sub
            If CInt(iHdcp) >= CInt(isi) Then
                dgScores.Columns(e.ColumnIndex).Tag = "Stroke"
                Dim mybrush As New SolidBrush(Color.Green)   ' creates a solid fill of shape   
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Border)
                Dim newrect As New Rectangle(e.CellBounds.Right - 8, e.CellBounds.Y + 2, 4, 4)
                e.Graphics.FillEllipse(mybrush, newrect)
                e.Handled = True
                'if double stroke hole, make color b/a
                If iHdcp - ohelper.iHoles >= isi Then
                    newrect = New Rectangle(e.CellBounds.Right - 16, e.CellBounds.Y + 2, 4, 4)
                    e.Graphics.FillEllipse(mybrush, newrect)
                    e.Handled = True
                    dgScores.Columns(e.ColumnIndex).Tag = "Double Stroke"
                End If
                bstrokehole = True
            Else
                Dim mybrush As New SolidBrush(dgScores.Rows(e.RowIndex).DefaultCellStyle.BackColor)
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Border)
                Dim newrect As New Rectangle(e.CellBounds.Right - 8, e.CellBounds.Y + 2, 4, 4)
                e.Graphics.FillEllipse(mybrush, newrect)
                e.Handled = True
            End If

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Private Sub Grid_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        Dim grid = DirectCast(sender, DataGridView)
        grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

#End Region
    Private Sub dgScores_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)

        Dim dg = DirectCast(sender, DataGridView)

        If e.RowIndex < 0 Then Exit Sub
        If bFormLoad Then Exit Sub

        If Not EditScoresEngine.IsReady() Then
            EditScoresEngine.Initialize(dg, ctx.oHelper)
            EditScoresEngine.LoadScoresFromView()
        End If

        Dim row = dg.Rows(e.RowIndex)
        Dim colName As String = dg.Columns(e.ColumnIndex).Name

        ' skip system updates
        If row.Cells(e.ColumnIndex).Tag?.ToString() = "SYS" Then
            row.Cells(e.ColumnIndex).Tag = Nothing
            Exit Sub
        End If

        ' =========================
        ' PLAYER CHANGE
        ' =========================
        If colName = "Player" Then

            Dim raw = row.Cells("Player").Value?.ToString()?.Trim()
            If String.IsNullOrEmpty(raw) Then Exit Sub

            Dim resolved = ctx.oHelper.fGetPlayer(raw, dg)

            If String.IsNullOrEmpty(resolved) Then
                MessageBox.Show("Player not found", "Error")
                row.Cells("Player").Value = _OldPlayer
                Exit Sub
            End If

            row.Cells("Player").Tag = "SYS"
            row.Cells("Player").Value = resolved.Trim()

            ctx.sPlayer = resolved
            ChangePlayer(row)
            BuildScoreCache()
            Exit Sub
        End If

        ' =========================
        ' HOLE ENTRY
        ' =========================
        Dim holeNum As Integer
        If Integer.TryParse(colName, holeNum) Then

            Dim cellVal = row.Cells(colName).Value

            If IsNumeric(cellVal) AndAlso CInt(cellVal) > 0 Then
                ctx.sPlayer = row.Cells("Player").Value?.ToString()

                EditScoresEngine.HandleHoleEntry(dg, row, colName, cellVal.ToString())
            End If

        End If

    End Sub

    Private Sub ScoreCard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ctx = LeagueContext.Instance
        sPar3s = ctx.lPar3s
        Me.ohelper = ctx.oHelper

        cbDates.Items.Clear()
        cbDates.Items.AddRange(Main.cbDates.Items.Cast(Of String).ToArray())

        RemoveHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged
        AddHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged

        bFormLoad = True
        cbDates.SelectedIndex = Main.cbDates.SelectedIndex
        bFormLoad = False

        ctx.SetDate(cbDates.SelectedItem.ToString())

        ' In ScoreCard_Load after other controls:
        Dim btnPDF As New Button()
        btnPDF.Name = "btnGeneratePDF"
        btnPDF.Text = "PDF"
        btnPDF.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnPDF.Size = New Size(70, 25)
        btnPDF.Location = New Point(cbDates.Right + 20, cbDates.Top + 3)
        btnPDF.BackColor = Color.FromArgb(180, 0, 0)
        btnPDF.ForeColor = Color.White
        btnPDF.FlatStyle = FlatStyle.Flat
        btnPDF.FlatAppearance.BorderSize = 0
        Me.Controls.Add(btnPDF)

        AddHandler btnPDF.Click, Sub(s, ev)
                                     ' Find active tab
                                     Dim tc = TryCast(Me.Controls.Find("tabControl", True).FirstOrDefault(), TabControl)
                                     If tc Is Nothing Then Exit Sub

                                     Dim activeTab = tc.SelectedTab
                                     If activeTab Is Nothing Then Exit Sub

                                     ' Find first DataGridView in active tab
                                     Dim dgv As DataGridView = Nothing
                                     For Each ctrl As Control In activeTab.Controls
                                         If TypeOf ctrl Is DataGridView Then
                                             dgv = DirectCast(ctrl, DataGridView)
                                             Exit For
                                         End If
                                     Next

                                     If dgv Is Nothing Then
                                         MessageBox.Show("No grid found on this tab.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                         Exit Sub
                                     End If

                                     Dim tabName As String = activeTab.Name
                                     Dim path As String = GetReportPdfPath($"{tabName}_{ctx.ActiveDate}.pdf")

                                     GeneratePDF(dgv, $"{tabName} Report", path)
                                 End Sub
        ctx.ShowLegacyTabs = False  ' set before creating checkbox

        ' Create legacy checkbox
        Dim chkLegacy As New CheckBox()
        chkLegacy.Name = "chkShowLegacy"
        chkLegacy.Text = "Show Legacy Tabs"
        chkLegacy.Font = New Font("Segoe UI", 8)
        chkLegacy.AutoSize = True
        chkLegacy.Checked = False
        chkLegacy.Location = New Point(btnPDF.Right + 10, btnPDF.Top + 3)
        Me.Controls.Add(chkLegacy)
        ' Add handler AFTER adding to controls to avoid premature firing
        AddHandler chkLegacy.CheckedChanged, Sub(s, ev)
                                                 ctx.ShowLegacyTabs = chkLegacy.Checked
                                                 LoadAllTabs()
                                             End Sub

        ' Now load tabs
        LoadAllTabs()
    End Sub

    ' --- THE NEW MASTER CONTROLLER ---
    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs)
        LOGIT($"cbDates_SelectedIndexChanged: SelectedItem={cbDates.SelectedItem} bFormLoad={bFormLoad}")
        If bFormLoad Then Exit Sub
        If cbDates.SelectedItem Is Nothing Then Exit Sub

        BackupDatabase()
        ctx.SetDate(cbDates.SelectedItem.ToString())
        LoadAllTabs()
    End Sub
    Private Sub BackupDatabase()
        Try
            Dim dbPath As String = ctx.Conn.ConnectionString.Replace("Data Source=", "").Replace(";Version=3;", "").Trim()
            Dim backupDir As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dbPath), "Backups")
            System.IO.Directory.CreateDirectory(backupDir)
            Dim backupPath As String = System.IO.Path.Combine(backupDir, $"hughsgolf_{ctx.ActiveDate}_{DateTime.Now:yyyyMMdd_HHmmss}.db")
            LOGIT($"BackupDatabase: {backupPath}")

            ' Always close source first
            If ctx.Conn.State = ConnectionState.Open Then ctx.Conn.Close()

            ' Open fresh source connection
            Using srcConn As New SQLiteConnection(ctx.Conn.ConnectionString)
                srcConn.Open()
                Using destConn As New SQLiteConnection($"Data Source={backupPath};Version=3;")
                    destConn.Open()
                    srcConn.BackupDatabase(destConn, "main", "main", -1, Nothing, 0)
                End Using
            End Using

            ' Reopen ctx connection
            ctx.Conn.Open()
            LOGIT($"BackupDatabase: SUCCESS")
        Catch ex As Exception
            LOGIT($"BackupDatabase Error: {ex.Message}")
            ' Make sure connection is reopened even on error
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
        End Try
    End Sub
    Private Sub btnPrevDate_Click(sender As Object, e As EventArgs) Handles btnPrevDate.Click
        Dim dates = ctx.lDates
        Dim idx As Integer = dates.IndexOf(ctx.ActiveDate)
        If idx >= dates.Count - 1 Then Exit Sub
        ctx.SetDate(dates(idx + 1))
        Try
            bFormLoad = True
            cbDates.SelectedItem = ctx.ActiveDate
        Finally
            bFormLoad = False
        End Try
        BackupDatabase()
        LoadAllTabs()
    End Sub

    Private Sub btnNextDate_Click(sender As Object, e As EventArgs) Handles btnNextDate.Click
        Dim dates = ctx.lDates
        Dim idx As Integer = dates.IndexOf(ctx.ActiveDate)
        If idx <= 0 Then Exit Sub
        ctx.SetDate(dates(idx - 1))
        Try
            bFormLoad = True
            cbDates.SelectedItem = ctx.ActiveDate
        Finally
            bFormLoad = False
        End Try
        BackupDatabase()
        LoadAllTabs()
    End Sub
    Private _loadingTabs As Boolean = False

    Friend Sub LoadAllTabs()
        If _loadingTabs Then Exit Sub
        _loadingTabs = True
        Try

            LOGIT($"ScoreCard_Load: ActiveDate={ctx.ActiveDate} FrontBack={ctx.sFrontBack}")
            LOGIT($"LoadAllTabs stack: {New System.Diagnostics.StackTrace().ToString()}")
            EditScoresEngine.LoadScoresFromView()

            lblCurrentDate.Text = DateTime.ParseExact(ctx.ActiveDate, "yyyyMMdd", Nothing).ToString("MM/dd/yyyy")

            ' Update nav button visibility
            Dim dates = ctx.lDates
            Dim idx As Integer = dates.IndexOf(ctx.ActiveDate)
            btnPrevDate.Visible = (idx < dates.Count - 1)
            btnNextDate.Visible = (idx > 0)

            ' Remove old tabControl
            For Each ctrl As Control In Me.Controls.Find("tabControl", True)
                Me.Controls.Remove(ctrl)
                ctrl.Dispose()
            Next

            ' Create new tabControl
            Dim tabControl As New TabControl()
            ctx.tabControl = tabControl
            tabControl.Name = "tabControl"
            tabControl.Height = 600
            tabControl.Location = New Point(10, 60)
            tabControl.Size = New Size(Me.ClientSize.Width - 20, Me.ClientSize.Height)
            tabControl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
            BuildTabShells(tabControl)
            Me.Controls.Add(tabControl)
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed
            AddHandler tabControl.DrawItem, AddressOf TabControl_DrawItem

            ' -- Active tabs - always load ------------------------------
            LoadScoresTab(tabControl)
            LoadStablefordTab(tabControl)
            LoadStablefordStandingsTab(tabControl)
            CheckAndPromptGallusImport()

            If Not ctx.IsPostSeason Then
                LoadMatchesTab(tabControl)
                LoadStandingsTab(tabControl)
                LoadScheduleTab(tabControl)
            End If
            LoadPaymentsTab(tabControl)
            LoadCTPTab(tabControl)

            ' -- Secondary tabs -----------------------------------------
            LoadLast5Tab(tabControl)
            LoadPrizeMoneyTab(tabControl)
            LoadPMRecapTab(tabControl)
            LoadStrokeHolesTab(tabControl)
            LoadExpensesTab(tabControl)
            LoadLeadersTab(tabControl)
            LoadGallusImportTab(tabControl)
            ' -- Legacy tabs - only load if checkbox checked ------------
            If ctx.ShowLegacyTabs Then
                LoadSkinsCTPsTab(tabControl)
                LoadKittyTab(tabControl)
                LoadWeeklyPaymentsTab(tabControl)
                LoadLeagueDuesTab(tabControl)
            End If

            AddHandler tabControl.SelectedIndexChanged, AddressOf TabControl_SelectedIndexChanged
            tabControl.Invalidate()
        Finally
            _loadingTabs = False
        End Try
    End Sub
    Private Sub TabControl_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim tc = DirectCast(sender, TabControl)
        Dim tab = tc.TabPages(e.Index)

        Dim legacyTabs As New List(Of String) From {
        "WeeklyPayments", "LeagueDues", "SkinsCTPs", "Kitty"
    }
        Dim newTabs As New List(Of String) From {"Payments", "Expenses"}

        Dim backColor As Color
        If legacyTabs.Contains(tab.Name) Then
            backColor = Color.LightSalmon
        ElseIf newTabs.Contains(tab.Name) Then
            backColor = Color.LightGreen
        Else
            backColor = SystemColors.Control
        End If

        Using b As New SolidBrush(backColor)
            e.Graphics.FillRectangle(b, e.Bounds)
        End Using

        Dim textColor As Color = Color.Black
        TextRenderer.DrawText(e.Graphics, tab.Text, tc.Font, e.Bounds, textColor,
        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

#Region "Modular Tab Loaders"
    Private Sub BuildTabShells(tc As TabControl)
        Dim chkShowLegacy = TryCast(Me.Controls.Find("chkShowLegacy", True).FirstOrDefault(), CheckBox)
        Dim showLegacy As Boolean = If(chkShowLegacy IsNot Nothing, chkShowLegacy.Checked, False)
        Dim legacyTabs As New List(Of String) From {
        "WeeklyPayments", "LeagueDues", "SkinsCTPs", "Kitty"
    }
        Dim newTabs As New List(Of String) From {"Payments", "Expenses"}

        Dim tabs As String() = {"Scores", "Stableford", "StablefordStandings", "Last5", "Matches", "Schedule", "Standings", "PrizeMoney", "PMRecap", "StrokeHoles", "CTPs", "SkinsCTPs", "Kitty", "WeeklyPayments", "Payments", "Expenses", "LeagueDues", "Leaders", "GallusImport"}
        For Each tabName As String In tabs
            If ctx.IsPostSeason AndAlso (tabName = "Matches" OrElse tabName = "Standings") Then Continue For
            ' Skip legacy tabs if checkbox unchecked
            If legacyTabs.Contains(tabName) AndAlso Not chkShowLegacy.Checked Then
                Continue For
            End If

            Dim tabPage As New TabPage()
            tabPage.Name = tabName
            tabPage.Text = If(tabName = "SkinsCTPs", "Skins/CTPs", If(tabName = "StablefordStandings", "SF Stdg", tabName))

            Dim dgv As New DataGridView()
            dgv.Name = $"dg{tabName}"
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.RowTemplate.Height -= CInt(dgv.RowTemplate.Height * 0.25)
            dgv.ColumnHeadersHeight += CInt(dgv.ColumnHeadersHeight * 0.75)

            If tabName <> "Scores" AndAlso tabName <> "Stableford" AndAlso tabName <> "CTPs" AndAlso
   tabName <> "SkinsCTPs" AndAlso tabName <> "Kitty" AndAlso
   tabName <> "Payments" AndAlso tabName <> "Expenses" Then
                dgv.Dock = DockStyle.Fill
            End If

            tabPage.Controls.Add(dgv)
            tc.TabPages.Add(tabPage)
        Next
    End Sub
    Private Sub dgScores_DataError(sender As Object, e As DataGridViewDataErrorEventArgs)
        LOGIT($"DataGridView DataError: col={dgScores.Columns(e.ColumnIndex).Name} row={e.RowIndex} error={e.Exception.Message}")
        e.ThrowException = False  ' suppress the dialog
    End Sub
    Private Sub WireGridEvents(dgv As DataGridView)

        RemoveHandler dgv.CellBeginEdit, AddressOf dgScores_CellBeginEdit
        AddHandler dgv.CellBeginEdit, AddressOf dgScores_CellBeginEdit

        RemoveHandler dgv.CellEndEdit, AddressOf dgScores_CellEndEdit
        AddHandler dgv.CellEndEdit, AddressOf dgScores_CellEndEdit

        RemoveHandler dgv.CellPainting, AddressOf dgScores_CellPainting
        AddHandler dgv.CellPainting, AddressOf dgScores_CellPainting

        RemoveHandler dgv.CellFormatting, AddressOf dgScores_CellFormatting
        AddHandler dgv.CellFormatting, AddressOf dgScores_CellFormatting

    End Sub
    Private Sub LoadScoresTab(tc As TabControl)
        LOGIT($"LoadScoresTab called from: {New System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}")
        Dim tp = tc.TabPages("Scores")
        tp.Location = New Point(10, 10)

        ' Fix LINQ
        Dim dgv As DataGridView = Nothing
        For Each ctrl As Control In tp.Controls
            If TypeOf ctrl Is DataGridView Then
                dgv = DirectCast(ctrl, DataGridView)
                Exit For
            End If
        Next
        If dgv Is Nothing Then Exit Sub

        WireGridEvents(dgv)

        ' Skip matches in postseason
        If Not ctx.IsPostSeason Then
            GetMatches()
        End If

        ' 1. Data Fetch & Bind
        doPostSeason()
        dtScores.PrimaryKey = New DataColumn() {dtScores.Columns(Player)}
        dgv.DataSource = dtScores
        dgScores = dgv

        AddHandler dgScores.EditingControlShowing, AddressOf grid_EditingControlShowing
        EditScoresEngine.Initialize(dgScores, Me.ohelper)
        RemoveHandler dgScores.DataError, AddressOf dgScores_DataError
        AddHandler dgScores.DataError, AddressOf dgScores_DataError

        ChangeColAttributes(dgv)
        addGB2DGV(tp, dgScores)
        EditScoresEngine.RefreshScoresGrid()

        Dim mostRecentDate As String = ctx.lDates.FirstOrDefault()
        If ctx.ActiveDate = mostRecentDate Then
            EditScoresEngine.RecalculateSkinCarryovers()
            EditScoresEngine.AwardSkins()
            EditScoresEngine.RecalculateCTPCarryovers()
        End If

        dgScores.BackgroundColor = Color.White
        dgScores.Size = New Size(
        dgScores.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + dgScores.RowHeadersWidth + 3,
        (dgScores.Rows.Count * dgScores.RowTemplate.Height) + dgScores.ColumnHeadersHeight + 3)
        dgScores.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty

        For i As Integer = 0 To dgScores.Rows.Count - 1 Step 8
            For ii As Integer = 0 To 3
                Dim rowIndex As Integer = i + ii
                If rowIndex < dgScores.Rows.Count Then
                    dgScores.Rows(rowIndex).DefaultCellStyle.BackColor = Color.LightBlue
                End If
            Next
        Next

        ImportGallusData(tp)
        dgScores.Refresh()
        dgScores.Invalidate()
    End Sub
    Sub doPostSeason()
        If ctx.IsPostSeason Then
            Dim thisWeekFB As String = ctx.sFrontBack
            Dim otherWeekFB As String = If(thisWeekFB = "Front", "Back", "Front")
            Dim otherWeekDate As String = DateTime.ParseExact(ctx.ActiveDate, "yyyyMMdd", Nothing).AddDays(+7).ToString("yyyyMMdd")

            Dim otherWeekExists As Boolean = False
            Using cmd As New SQLiteCommand("
            SELECT COUNT(*) FROM Scores 
            WHERE League=@League AND Date=@Date 
            AND FrontBack=@FrontBack AND Gross IS NOT NULL", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", otherWeekDate)
                cmd.Parameters.AddWithValue("@FrontBack", otherWeekFB)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                otherWeekExists = CInt(cmd.ExecuteScalar()) > 0
            End Using

            Dim frontBackCols As String
            Dim otherCols As String

            If thisWeekFB = "Front" Then
                frontBackCols = $"S1.Gross AS FrontGross, S1.Net AS FrontNet, {If(otherWeekExists, "S2.Gross AS BackGross, S2.Net AS BackNet,", "NULL AS BackGross, NULL AS BackNet,")}"
            Else
                frontBackCols = $"S1.Gross AS BackGross, S1.Net AS BackNet, {If(otherWeekExists, "S2.Gross AS FrontGross, S2.Net AS FrontNet,", "NULL AS FrontGross, NULL AS FrontNet,")}"
            End If

            If otherWeekExists Then
                otherCols = "S2.[1] AS O1, S2.[2] AS O2, S2.[3] AS O3, S2.[4] AS O4, S2.[5] AS O5, S2.[6] AS O6, S2.[7] AS O7, S2.[8] AS O8, S2.[9] AS O9, S2.Net AS OtherNet, S2.Gross AS OtherGross,"
            Else
                otherCols = "NULL AS O1, NULL AS O2, NULL AS O3, NULL AS O4, NULL AS O5, NULL AS O6, NULL AS O7, NULL AS O8, NULL AS O9, NULL AS OtherNet, NULL AS OtherGross,"
            End If

            Dim sql As String = $"
            SELECT 
                T.Player,
                COALESCE(T.Grade, 'N/A') AS Grade,
                COALESCE(H.Hdcp, 0) AS PHdcp,
                S1.[1], S1.[2], S1.[3], S1.[4], S1.[5],
                S1.[6], S1.[7], S1.[8], S1.[9],
                {frontBackCols}
                {otherCols}
                COALESCE(S1.Net, 0) + {If(otherWeekExists, "COALESCE(S2.Net, 0)", "0")} AS TotalNet,
                COALESCE(S1.Gross, 0) + {If(otherWeekExists, "COALESCE(S2.Gross, 0)", "0")} AS TotalGross,
                CASE WHEN SkP.Earned IS NULL THEN 'N' ELSE 'Y' END AS InSkins,
                CASE WHEN CPP.Earned IS NULL THEN 'N' ELSE 'Y' END AS InCTPs,
                'N' AS IsSub,
                T.Player AS Partner,
                '' AS Opponent,
                0 AS Points,
                0 AS Team_Points,
                NULL AS Team_Net,
                T.Team,
                '{ctx.ActiveDate}' AS Date,
                '{ctx.SafeLeagueName}' AS League
            FROM Teams T
            LEFT JOIN Scores S1 ON S1.Player = T.Player 
                AND S1.League = T.League 
                AND S1.Date = '{ctx.ActiveDate}'
                AND S1.FrontBack = '{thisWeekFB}'
            {If(otherWeekExists, $"LEFT JOIN Scores S2 ON S2.Player = T.Player AND S2.League = T.League AND S2.Date = '{otherWeekDate}' AND S2.FrontBack = '{otherWeekFB}'", "")}
            LEFT JOIN Handicaps H ON H.League = T.League
                AND H.Player = T.Player
                AND H.Date = (
                    SELECT MAX(H2.Date) FROM Handicaps H2
                    WHERE H2.League = T.League
                    AND H2.Player = T.Player
                    AND H2.Date < '{ctx.ActiveDate}'
                    AND H2.Hdcp <> ''
                )
            LEFT JOIN Payments SkP ON SkP.League = T.League
                AND SkP.Date = '{ctx.ActiveDate}'
                AND SkP.Player = T.Player
                AND SkP.Detail = 'Payment' AND SkP.[Desc] = 'Skin'
            LEFT JOIN Payments CPP ON CPP.League = T.League
                AND CPP.Date = '{ctx.ActiveDate}'
                AND CPP.Player = T.Player
                AND CPP.[Desc] = 'CTP' AND CPP.Detail = 'Payment'
            WHERE T.League = '{ctx.SafeLeagueName}'
              AND T.Year = {ctx.SeasonYear}
            ORDER BY TotalNet ASC"

            dtScores = New DataTable()
            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dtScores)
                End Using
            End Using
        Else
            dtScores = ohelper.sqliteda("ScoreCard", $"Select * from vwMatchesScores where date={ctx.ActiveDate} ORDER BY Partner")
        End If
    End Sub
    Public Sub FinalizeImportedScores(dgv As DataGridView, playerDataList As List(Of Dictionary(Of String, Object)))
        ' Initialize once before loop
        If Not IsReady() Then
            Initialize(dgv, ctx.oHelper)
            LoadScoresFromView()
        End If

        For Each data As Dictionary(Of String, Object) In playerDataList
            Dim gallusName As String = data("PlayerName").ToString()

            ' Use ResolvedName if available otherwise parse from PlayerName
            Dim subName As String = If(data.ContainsKey("ResolvedName") AndAlso
            Not String.IsNullOrEmpty(data("ResolvedName").ToString()),
            data("ResolvedName").ToString().Trim(),
            gallusName.Trim())

            ' Also handle hyphen format
            If gallusName.Contains("-") AndAlso subName = gallusName Then
                subName = gallusName.Substring(0, gallusName.IndexOf("-")).Trim()
            End If

            Dim lastName As String = subName.Split(" "c).Last()

            For Each r As DataGridViewRow In dgv.Rows
                If r.IsNewRow Then Continue For
                Dim gridPlayer As String = r.Cells("Player").Value?.ToString()
                If String.IsNullOrEmpty(gridPlayer) Then Continue For
                If gridPlayer.ToLower().Contains(lastName.ToLower()) Then
                    dgv.CurrentCell = r.Cells("Player")
                    HandleSkinBuyIn(r)
                    FinalizeScoreEntry(dgv)
                    Exit For
                End If
            Next
        Next

        AwardSkins()
        RecalculateSkinCarryovers()
        RecalculateCTPCarryovers()
    End Sub
    Sub ChangeColAttributes(dgscores As DataGridView)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            dgscores.EnableHeadersVisualStyles = False

            ' Create action column
            ohelper.CreateDGCol(dgscores, "Clear Scores", 1, Clear)

            ' Determine front/back for postseason
            Dim thisWeekFB As String = ctx.sFrontBack
            Dim otherWeekFB As String = If(thisWeekFB = "Front", "Back", "Front")
            Dim thisWeekStart As Integer = If(thisWeekFB = "Back", 10, 1)
            Dim otherWeekStart As Integer = If(otherWeekFB = "Back", 10, 1)

            For Each col As DataGridViewColumn In dgscores.Columns
                With col
                    .Name = .DataPropertyName
                    .ReadOnly = True
                    .SortMode = DataGridViewColumnSortMode.NotSortable
                    .HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter
                    .Visible = False

                    ' --- THIS WEEK HOLE COLUMNS (1-9) ---
                    If IsNumeric(.Name) Then
                        .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                        .Width = 25
                        .ReadOnly = If(ctx.ActiveDate > DateTime.Now.ToString("yyyyMMdd"), True, False)
                        .Visible = True
                        Dim isBackHole As Boolean = (ctx.sFrontBack.ToLower = "back")
                        .HeaderText = If(ohelper.iHoles < 18 AndAlso isBackHole,
                        (CInt(.Name) + 9).ToString(), .Name)
                        .HeaderCell.Style.BackColor = If(isBackHole, Color.Green, Color.Gold)
                        .HeaderCell.Style.ForeColor = Color.White

                        ' --- OTHER WEEK HOLE COLUMNS (O1-O9) postseason only ---
                    ElseIf ctx.IsPostSeason AndAlso .Name.StartsWith("O") AndAlso
                       IsNumeric(.Name.Substring(1)) Then
                        .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                        .Width = 25
                        .ReadOnly = True  ' other week is read only
                        .Visible = True
                        Dim holeNum As Integer = CInt(.Name.Substring(1))
                        Dim actualHole As Integer = If(otherWeekFB = "Back", holeNum + 9, holeNum)
                        .HeaderText = actualHole.ToString()
                        .HeaderCell.Style.BackColor = If(otherWeekFB = "Back", Color.Green, Color.Gold)
                        .HeaderCell.Style.ForeColor = Color.White

                        ' --- GROSS/NET ---
                    ElseIf ohelper.sIn(.Name, "Gross,Net,ThisGross,ThisNet,OtherGross,OtherNet,TotalGross,TotalNet", True) Then
                        .Width = 40
                        .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                        If ctx.IsPostSeason Then
                            Select Case .Name
                                Case "ThisGross"
                                    .HeaderText = $"{thisWeekFB} Gross"
                                    .Visible = True
                                Case "OtherGross"
                                    .HeaderText = $"{otherWeekFB} Gross"
                                    .Visible = True
                                Case "TotalGross"
                                    .HeaderText = "Total"
                                    .Visible = True
                                Case "TotalNet"
                                    .HeaderText = "Net"
                                    .Visible = True
                            End Select
                        Else
                            If Not .Name.Contains("Team") Then .Visible = True
                        End If

                        ' --- HDCP ---
                    ElseIf ohelper.sIn(.HeaderText, "PHdcp,Hdcp", False) Then
                        .Width = 35
                        .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                        .HeaderText = If(.HeaderText.Contains("P"), "Prev Hdcp", "New Hdcp")
                        .Visible = True
                    End If

                    .HeaderText = .HeaderText.Replace("_", " ")

                    If .HeaderText = Player Then
                        .ReadOnly = ohelper.bPostSeason
                        .Visible = True
                    End If

                    If ohelper.sIn(.Name, $"{CTP1.Replace("_", "")},{CTP2.Replace("_", "")}", False) Then
                        .HeaderCell.Style.BackColor = Color.Cyan
                    ElseIf ohelper.sIn(.Name, $"{earned}, {skinamt}, {closestamt}, {skinnum}", False) Then
                        .HeaderCell.Style.BackColor = Color.LavenderBlush
                    End If
                End With
            Next

            ' --- ROW STRIPING ---
            If Not ohelper.bPostSeason Then
                For i As Integer = 0 To dgscores.Rows.Count - 1 Step 8
                    For ii As Integer = 0 To 3
                        Dim rowIndex As Integer = i + ii
                        If rowIndex < dgscores.Rows.Count Then
                            dgscores.Rows(rowIndex).DefaultCellStyle.BackColor = Color.LightBlue
                        End If
                    Next
                Next
            End If

        Catch ex As Exception
            MsgBox(ohelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Friend Sub LoadPMRecapTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("PMRecap")
            Dim dgv = DirectCast(tp.Controls("dgPMRecap"), DataGridView)

            ' --- FIELD LIST ---
            Dim sPayFlds As String = "ID,League,Player,Date,Desc,Detail,Earned-d,DatePaid,Comment,PayMethod"

            ' --- BUILD DATATABLE ---
            Dim dtPay As DataTable = ohelper.createDT(sPayFlds)

            ' --- PULL PAYMENTS FROM DB ---
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

            Dim sql As String = $"
            SELECT ID, League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod
            FROM   Payments
            WHERE  League  = '{ctx.SafeLeagueName}'
              AND  Date    = {ctx.ActiveDate}
              AND  Detail  LIKE '#%'
            ORDER  BY Desc, DatePaid"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                Using rdr As SQLiteDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim nr = dtPay.NewRow()
                        nr("ID") = rdr("ID").ToString()
                        nr("League") = rdr("League").ToString()
                        nr("Player") = rdr("Player").ToString()
                        nr("Date") = rdr("Date").ToString()
                        nr("Desc") = rdr("Desc").ToString()
                        nr("Detail") = rdr("Detail").ToString()
                        nr("Earned") = If(IsDBNull(rdr("Earned")), 0D, CDec(rdr("Earned")))
                        nr("DatePaid") = rdr("DatePaid").ToString()
                        nr("Comment") = rdr("Comment").ToString()
                        nr("PayMethod") = rdr("PayMethod").ToString()
                        dtPay.Rows.Add(nr)
                    End While
                End Using
            End Using

            ' --- SUBTOTALS ---
            Dim skinRows = dtPay.AsEnumerable().Where(Function(r) r("Desc").ToString().Trim().ToUpper() = "SKIN")
            Dim ctpRows = dtPay.AsEnumerable().Where(Function(r) r("Desc").ToString().Trim().ToUpper() = "CTP")

            Dim skinTotal As Decimal = If(skinRows.Any(), skinRows.Sum(Function(r) CDec(r("Earned"))), 0D)
            Dim ctpTotal As Decimal = If(ctpRows.Any(), ctpRows.Sum(Function(r) CDec(r("Earned"))), 0D)
            Dim grandTotal As Decimal = skinTotal + ctpTotal

            ' Skin subtotal row
            Dim srSkin = dtPay.NewRow()
            srSkin("Player") = "Skin Total"
            srSkin("Desc") = "Skin"
            srSkin("Earned") = skinTotal
            srSkin("Comment") = $"{skinRows.Count()} winner(s)"
            dtPay.Rows.Add(srSkin)

            ' CTP subtotal row
            Dim srCtp = dtPay.NewRow()
            srCtp("Player") = "CTP Total"
            srCtp("Desc") = "CTP"
            srCtp("Earned") = ctpTotal
            srCtp("Comment") = $"{ctpRows.Count()} winner(s)"
            dtPay.Rows.Add(srCtp)

            ' Grand total row
            Dim tr = dtPay.NewRow()
            tr("Player") = "TOTAL"
            tr("Earned") = grandTotal
            tr("Comment") = $"{dtPay.Rows.Count - 3} payment(s)"
            dtPay.Rows.Add(tr)

            ' --- BIND DATA ---
            dgv.DataSource = dtPay

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 24
            dgv.ColumnHeadersHeight = 28
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.ReadOnly = True
            dgv.ScrollBars = ScrollBars.None
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            ' --- COLUMN WIDTHS AND FORMATTING ---
            Dim colSettings As New Dictionary(Of String, (Width As Integer, Format As String, Align As DataGridViewContentAlignment)) From {
            {"Player", (120, "", DataGridViewContentAlignment.MiddleLeft)},
            {"Desc", (55, "", DataGridViewContentAlignment.MiddleLeft)},
            {"Detail", (55, "", DataGridViewContentAlignment.MiddleCenter)},
            {"Earned", (75, "C2", DataGridViewContentAlignment.MiddleRight)},
            {"DatePaid", (150, "", DataGridViewContentAlignment.MiddleLeft)},
            {"Comment", (120, "", DataGridViewContentAlignment.MiddleLeft)}
        }

            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    Dim settings = colSettings(col.Name)
                    col.Visible = True
                    col.Width = settings.Width
                    col.DefaultCellStyle.Alignment = settings.Align
                    col.HeaderCell.Style.Alignment = settings.Align
                    If settings.Format <> "" Then
                        col.DefaultCellStyle.Format = settings.Format
                    End If
                Else
                    col.Visible = False
                End If
            Next

            ' --- SIZE TO CONTENT ---
            Dim totalWidth As Integer = 0
            For Each s In colSettings.Values
                totalWidth += s.Width
            Next
            Dim targetHeight As Integer = (dgv.Rows.Count * 24) + dgv.ColumnHeadersHeight + 2
            dgv.Size = New Size(totalWidth + 2, targetHeight)

            ' --- WIRE ROW FORMATTING ---
            RemoveHandler dgv.CellFormatting, AddressOf dgPMRecap_CellFormatting
            AddHandler dgv.CellFormatting, AddressOf dgPMRecap_CellFormatting

            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadPMRecapTab Error: " & ex.Message)
        End Try
    End Sub

    ' -----------------------------------------------------------------------------
    ' CellFormatting - styles subtotal and grand total rows
    ' -----------------------------------------------------------------------------
    Private Sub dgPMRecap_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 OrElse e.RowIndex >= dgv.Rows.Count Then Return

        Dim playerVal As String = dgv.Rows(e.RowIndex).Cells("Player").Value?.ToString()

        Select Case playerVal
            Case "TOTAL"
                e.CellStyle.BackColor = Color.LightSteelBlue
                e.CellStyle.ForeColor = Color.DarkSlateBlue
                e.CellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
                e.CellStyle.SelectionBackColor = Color.SteelBlue

            Case "Skin Total"
                e.CellStyle.BackColor = Color.FromArgb(255, 243, 205)  ' soft amber
                e.CellStyle.ForeColor = Color.DarkGoldenrod
                e.CellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
                e.CellStyle.SelectionBackColor = Color.Goldenrod

            Case "CTP Total"
                e.CellStyle.BackColor = Color.FromArgb(220, 240, 220)  ' soft green
                e.CellStyle.ForeColor = Color.DarkGreen
                e.CellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
                e.CellStyle.SelectionBackColor = Color.MediumSeaGreen
        End Select
    End Sub

    Dim sPMflds As String = "Category,Prior-d,Current-d,TotalAmount-d,TotalNum-d,Collected-d,Leftover-d"
    Friend Sub LoadPrizeMoneyTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("PrizeMoney")
            Dim dgv = DirectCast(tp.Controls("dgPrizeMoney"), DataGridView)

            ctx.SetDate(ctx.ActiveDate)
            sPar3s = ctx.lPar3s
            Dim activeFrontBack As String = LeagueDateService.GetFrontBack(ctx.ActiveDate, ctx.rLeagueParmrow("Start9").ToString())
            Dim activePar3s As List(Of String) = ctx.lPar3s

            ' Build categories (Skins + CTPs based on current date par 3s)
            Dim lCategorys As New List(Of String) From {"Skin"}
            If activePar3s IsNot Nothing Then
                For i = 1 To activePar3s.Count
                    lCategorys.Add($"CTP{i}")
                Next
            End If

            ' Build DataTable
            dtPM = ohelper.createDT(sPMflds)
            dtPM.PrimaryKey = New DataColumn() {dtPM.Columns("Category")}
            For Each category In lCategorys
                Dim nr = dtPM.NewRow
                nr("Category") = category
                For Each fld In sPMflds.Split(","c)
                    If fld.Contains("-d") Then nr(fld.Replace("-d", "")) = 0
                Next
                dtPM.Rows.Add(nr)
            Next

            ' --- POPULATE PRIOR FROM KITTY CARRYOVER ---
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

            Dim skinCarry As Decimal = 0
            Dim skinSql As String = "
            SELECT IFNULL((
                SELECT Earned
                FROM Payments
                WHERE League = @League
                  AND Player = 'Kitty'
                  AND Desc = 'Skin'
                  AND Detail = 'Carryover'
                  AND Date < @Date
                ORDER BY Date DESC
                LIMIT 1
            ), 0)"
            Using cmd As New SQLiteCommand(skinSql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                skinCarry = CDec(cmd.ExecuteScalar())
            End Using
            SetPrizeMoneyValue("Skin", "Prior", skinCarry)

            ' -----------------------------------------------------------------------------
            ' SCORECARD.VB - LoadPrizeMoneyTab (CTP carryover section only - rest unchanged)
            ' -----------------------------------------------------------------------------
            If activePar3s IsNot Nothing Then
                For i = 1 To activePar3s.Count
                    Dim ctpCarry As Decimal = 0
                    Dim carryoverDetail As String = ScoreRulesService.CtpCarryoverDetail(i, activeFrontBack)
                    Dim ctpSql As String = "
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
                    Using cmd As New SQLiteCommand(ctpSql, ctx.Conn)
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Detail", carryoverDetail)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        ctpCarry = CDec(cmd.ExecuteScalar())
                    End Using
                    LOGIT($"LoadPrizeMoneyTab CTP{i}: ActiveDate={ctx.ActiveDate} FrontBack={activeFrontBack} Detail={carryoverDetail} Prior={ctpCarry}", True)
                    SetPrizeMoneyValue($"CTP{i}", "Prior", ctpCarry)
                Next
            End If

            Dim paymentSql As String = "
            SELECT Player, Desc, Detail, Earned
            FROM Payments
            WHERE League = @League
              AND Date = @Date"
            Using cmd As New SQLiteCommand(paymentSql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                Using rdr As SQLiteDataReader = cmd.ExecuteReader()
                    While rdr.Read()
                        ApplyPrizeMoneyPaymentRow(
                            rdr("Player").ToString(),
                            rdr("Desc").ToString(),
                            rdr("Detail").ToString(),
                            If(IsDBNull(rdr("Earned")), 0D, CDec(rdr("Earned"))),
                            activePar3s)
                    End While
                End Using
            End Using
            RecalculatePrizeMoneyTotals()

            ' --- BIND DATA ---
            dgv.DataSource = dtPM

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 24
            dgv.ColumnHeadersHeight = 28
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.ReadOnly = True
            dgv.ScrollBars = ScrollBars.None
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            ' --- COLUMN WIDTHS AND FORMATTING ---
            Dim colSettings As New Dictionary(Of String, (Width As Integer, Format As String, Align As DataGridViewContentAlignment)) From {
            {"Category", (85, "", DataGridViewContentAlignment.MiddleLeft)},
            {"Prior", (60, "C2", DataGridViewContentAlignment.MiddleRight)},
            {"Current", (70, "C2", DataGridViewContentAlignment.MiddleRight)},
            {"TotalAmount", (95, "C2", DataGridViewContentAlignment.MiddleRight)},
            {"TotalNum", (70, "N0", DataGridViewContentAlignment.MiddleCenter)},
            {"Collected", (80, "C2", DataGridViewContentAlignment.MiddleRight)},
            {"Leftover", (70, "C2", DataGridViewContentAlignment.MiddleRight)}
        }

            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    Dim settings = colSettings(col.Name)
                    col.Visible = True
                    col.Width = settings.Width
                    col.DefaultCellStyle.Alignment = settings.Align
                    col.HeaderCell.Style.Alignment = settings.Align
                    If settings.Format <> "" Then
                        col.DefaultCellStyle.Format = settings.Format
                    End If
                Else
                    col.Visible = False
                End If
            Next

            ' --- COLOR CODE LEFTOVER ---
            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

            ' --- SIZE TO CONTENT ---
            Dim totalWidth As Integer = 0
            For Each s In colSettings.Values
                totalWidth += s.Width
            Next
            Dim targetHeight As Integer = (dgv.Rows.Count * 24) + dgv.ColumnHeadersHeight + 2
            dgv.Size = New Size(totalWidth + 2, targetHeight)

            ' --- WIRE CELL FORMATTING FOR LEFTOVER COLOR ---
            RemoveHandler dgv.CellFormatting, AddressOf dgPrizeMoney_CellFormatting
            AddHandler dgv.CellFormatting, AddressOf dgPrizeMoney_CellFormatting

        Catch ex As Exception
            LOGIT("LoadPrizeMoneyTab Error: " & ex.Message)
        End Try
    End Sub

    Private Sub ApplyPrizeMoneyPaymentRow(player As String, desc As String, detail As String, earned As Decimal, par3s As List(Of String))
        If String.Equals(desc, "CTP", StringComparison.OrdinalIgnoreCase) Then
            If detail.StartsWith("#") Then
                Dim hole As String = detail.Replace("#", "")
                For i = 1 To par3s.Count
                    If par3s(i - 1) = hole Then
                        AddPrizeMoneyValue($"CTP{i}", "Current", earned)
                        AddPrizeMoneyValue($"CTP{i}", "TotalNum", 1D)
                        Exit For
                    End If
                Next
            ElseIf Not String.Equals(player, "Kitty", StringComparison.OrdinalIgnoreCase) Then
                Dim share As Decimal = If(par3s.Count > 0, earned / par3s.Count, earned)
                For i = 1 To par3s.Count
                    AddPrizeMoneyValue($"CTP{i}", "Collected", share)
                Next
            End If
        ElseIf String.Equals(desc, "Skin", StringComparison.OrdinalIgnoreCase) Then
            If detail.StartsWith("#") Then
                AddPrizeMoneyValue("Skin", "Current", earned)
                AddPrizeMoneyValue("Skin", "TotalNum", 1D)
            ElseIf Not String.Equals(player, "Kitty", StringComparison.OrdinalIgnoreCase) Then
                AddPrizeMoneyValue("Skin", "Collected", earned)
            End If
        End If
    End Sub

    Private Sub SetPrizeMoneyValue(category As String, fieldName As String, value As Decimal)
        If dtPM Is Nothing OrElse Not dtPM.Columns.Contains(fieldName) Then Exit Sub

        For Each row As DataRow In dtPM.Rows
            If String.Equals(row("Category").ToString(), category, StringComparison.OrdinalIgnoreCase) Then
                row(fieldName) = value
                Exit Sub
            End If
        Next
    End Sub

    Private Sub AddPrizeMoneyValue(category As String, fieldName As String, value As Decimal)
        If dtPM Is Nothing OrElse Not dtPM.Columns.Contains(fieldName) Then Exit Sub

        For Each row As DataRow In dtPM.Rows
            If String.Equals(row("Category").ToString(), category, StringComparison.OrdinalIgnoreCase) Then
                row(fieldName) = CDec(row(fieldName)) + value
                Exit Sub
            End If
        Next
    End Sub

    Private Sub RecalculatePrizeMoneyTotals()
        If dtPM Is Nothing Then Exit Sub

        For Each row As DataRow In dtPM.Rows
            Dim prior As Decimal = CDec(row("Prior"))
            Dim current As Decimal = CDec(row("Current"))
            Dim collected As Decimal = CDec(row("Collected"))
            row("TotalAmount") = prior + current
            row("Leftover") = prior + collected - current
        Next
    End Sub

    Private Sub dgPrizeMoney_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)
            If e.RowIndex < 0 Then Exit Sub
            If dgv.Columns(e.ColumnIndex).Name <> "Leftover" Then Exit Sub
            If e.Value Is Nothing OrElse Not IsNumeric(e.Value) Then Exit Sub

            Dim val As Decimal = CDec(e.Value)
            If val > 0 Then
                e.CellStyle.ForeColor = Color.DarkOrange  ' money still in pot
            ElseIf val = 0 Then
                e.CellStyle.ForeColor = Color.Gray
            End If
        Catch ex As Exception
            ' swallow
        End Try
    End Sub
    Friend Sub LoadStrokeHolesTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("StrokeHoles")
            Dim dgv = DirectCast(tp.Controls("dgStrokeHoles"), DataGridView)
            If dtScores Is Nothing Then Exit Sub

            ' Build hole column list using actual hole numbers (front or back)
            Dim holearray As String = ""
            For Each hole As DataColumn In dtScores.Columns
                If IsNumeric(hole.ColumnName) Then
                    Dim holeNum As Integer = CInt(hole.ColumnName)
                    Dim actualHole As Integer = If(ctx.sFrontBack = "Back", holeNum + 9, holeNum)
                    holearray &= If(holearray = "", actualHole.ToString(), "," & actualHole.ToString())
                End If
            Next

            ' Build DataTable with actual hole numbers as column names
            Dim dtSH = ohelper.createDT($"Player,Hdcp,{holearray}")

            For Each row As DataRow In dtScores.Rows
                Dim nr = dtSH.NewRow
                nr("Player") = row("Player")
                nr("Hdcp") = row("Phdcp")
                For Each col As DataColumn In dtSH.Columns
                    If IsNumeric(col.ColumnName) AndAlso row("Phdcp") IsNot DBNull.Value Then
                        Dim actualHole As Integer = CInt(col.ColumnName)
                        nr(col.ColumnName) = showStrokeHoles(actualHole, row("Phdcp"))
                    End If
                Next
                dtSH.Rows.Add(nr)
            Next

            ' --- BIND ---
            dgv.DataSource = dtSH

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersHeight = 28
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.ReadOnly = True
            dgv.ScrollBars = ScrollBars.None
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            ' --- COLUMN WIDTHS ---
            For Each col As DataGridViewColumn In dgv.Columns
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                If col.Name = "Player" Then
                    col.Width = 130
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                ElseIf col.Name = "Hdcp" Then
                    col.Width = 45
                Else
                    col.Width = 30
                End If
            Next

            ' --- SIZE TO CONTENT ---
            dgv.Size = New Size(
            dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3,
            (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 3)

            ' --- COLOR CODE STROKE DOTS ---
            RemoveHandler dgv.CellFormatting, AddressOf dgStrokeHoles_CellFormatting
            AddHandler dgv.CellFormatting, AddressOf dgStrokeHoles_CellFormatting

            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadStrokeHolesTab Error: " & ex.Message)
        End Try
    End Sub
    Private Sub dgStrokeHoles_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)
            If e.RowIndex < 0 Then Exit Sub
            If Not IsNumeric(dgv.Columns(e.ColumnIndex).Name) Then Exit Sub
            If e.Value Is Nothing OrElse Not IsNumeric(e.Value) Then Return

            Dim strokes As Integer = CInt(e.Value)
            Select Case strokes
                Case 0
                    e.CellStyle.BackColor = Color.White
                    e.CellStyle.ForeColor = Color.White  ' invisible text
                    e.Value = ""                          ' or blank the value
                    e.FormattingApplied = True            ' tell grid we handled it
                Case 1
                    e.CellStyle.BackColor = Color.FromArgb(220, 255, 220)  ' light green
                    e.CellStyle.ForeColor = Color.DarkGreen
                    e.CellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                Case 2
                    e.CellStyle.BackColor = Color.FromArgb(255, 255, 180)  ' light yellow
                    e.CellStyle.ForeColor = Color.DarkGoldenrod
                    e.CellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                Case Else
                    e.CellStyle.BackColor = Color.FromArgb(255, 200, 200)  ' light red
                    e.CellStyle.ForeColor = Color.DarkRed
                    e.CellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            End Select
        Catch ex As Exception
            ' swallow
        End Try
    End Sub
    Friend Sub LoadCTPTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("CTPs")
            Dim dgv = DirectCast(tp.Controls("dgCTPs"), DataGridView)

            dgv.Rows.Clear()
            dgv.Columns.Clear()

            ' Build columns
            dgv.Columns.Add("Hole", "Hole")
            dgv.Columns.Add("Player", "Winner")

            Dim btnCol As New DataGridViewButtonColumn()
            btnCol.Name = "Award"
            btnCol.HeaderText = "Action"
            btnCol.UseColumnTextForButtonValue = False
            dgv.Columns.Add(btnCol)

            ' Styling
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 24
            dgv.ColumnHeadersHeight = 28
            dgv.ReadOnly = False
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.ScrollBars = ScrollBars.None

            ' Column widths
            dgv.Columns("Hole").Width = 60
            dgv.Columns("Hole").ReadOnly = True
            dgv.Columns("Award").Width = 90

            ' Player dropdown column
            dgv.Columns.Remove("Player")
            Dim playerCol As New DataGridViewComboBoxColumn()
            playerCol.Name = "Player"
            playerCol.HeaderText = "Winner"
            playerCol.Width = 180
            playerCol.FlatStyle = FlatStyle.Flat

            ' Load players for this date
            Dim dtPlayers = EditScoresEngine.GetPlayersForDate()
            playerCol.Items.Add("")
            For Each p In dtPlayers
                playerCol.Items.Add(p)
            Next
            dgv.Columns.Insert(1, playerCol)

            ' Load par 3 holes - one row each
            Dim bPostSeason As Boolean = ctx.ActiveDate >= ctx.sPSDate
            Dim ctpAmount As Decimal = If(bPostSeason,
            CDec(ctx.rLeagueParmrow("ClosestPS")),
            CDec(ctx.rLeagueParmrow("Closest")))

            For Each hole In ctx.lPar3s
                Dim r = dgv.Rows.Add()
                dgv.Rows(r).Cells("Hole").Value = hole

                Dim existing = GetExistingCTPWinner(hole)
                If existing <> "" Then
                    ' Already awarded - show Reset
                    Dim playerCombo = DirectCast(dgv.Columns("Player"), DataGridViewComboBoxColumn)
                    If Not playerCombo.Items.Contains(existing) Then
                        playerCombo.Items.Add(existing)
                    End If
                    dgv.Rows(r).Cells("Player").Value = existing
                    dgv.Rows(r).Cells("Player").ReadOnly = True
                    dgv.Rows(r).Cells("Award").Value = "Reset"
                    dgv.Rows(r).Cells("Award").Style.ForeColor = Color.DarkRed
                    dgv.Rows(r).DefaultCellStyle.BackColor = Color.LightGreen
                Else
                    ' Not awarded - show Award
                    dgv.Rows(r).Cells("Award").Value = "Award"
                    dgv.Rows(r).Cells("Award").Style.ForeColor = Color.DarkGreen
                End If
            Next

            ' Size grid to content
            Dim targetHeight As Integer = (dgv.Rows.Count * 24) + dgv.ColumnHeadersHeight + 2
            dgv.Size = New Size(340, targetHeight)

            ' Wire events
            RemoveHandler dgv.CellContentClick, AddressOf dgCTPs_CellContentClick
            AddHandler dgv.CellContentClick, AddressOf dgCTPs_CellContentClick

        Catch ex As Exception
            LOGIT("LoadCTPTab Error: " & ex.Message)
        End Try
    End Sub
    Private Function GetExistingCTPWinner(hole As String) As String
        Try
            Dim sql As String = $"
            SELECT Player FROM Payments
            WHERE League = '{ctx.SafeLeagueName}'
              AND Date = {ctx.ActiveDate}
              AND Desc = 'CTP'
              AND Detail = '#{hole}'"

            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                Using rdr = cmd.ExecuteReader()
                    If rdr.Read() Then
                        Return rdr("Player").ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            LOGIT("GetExistingCTPWinner Error: " & ex.Message)
        End Try
        Return ""
    End Function
    Private Sub dgCTPs_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)
            If e.RowIndex < 0 Then Exit Sub
            If dgv.Columns(e.ColumnIndex).Name <> "Award" Then Exit Sub

            Dim hole As String = dgv.Rows(e.RowIndex).Cells("Hole").Value?.ToString()
            If String.IsNullOrEmpty(hole) Then Exit Sub

            Dim buttonVal As String = dgv.Rows(e.RowIndex).Cells("Award").Value?.ToString()

            ' -----------------------------------------------------------------------------
            ' SCORECARD.VB - Reset block in dgCTPs_CellContentClick
            ' -----------------------------------------------------------------------------
            If buttonVal = "Reset" Then
                If MessageBox.Show($"Remove CTP award for hole {hole}?",
                       "Reset CTP Winner",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) = DialogResult.No Then Exit Sub

                ' Delete payment
                ohelper.SqliteTrans($"
        DELETE FROM Payments
        WHERE League = '{ctx.SafeLeagueName}'
          AND Date = {ctx.ActiveDate}
          AND Desc = 'CTP'
          AND Detail = '#{hole}'")

                ' Delete Kitty carryover for this slot - frontback aware
                Dim ctpSlot As Integer = 0
                For i = 1 To ctx.lPar3s.Count
                    If ctx.lPar3s(i - 1) = hole Then
                        ctpSlot = i
                        Exit For
                    End If
                Next
                If ctpSlot > 0 Then
                    ohelper.SqliteTrans($"
            DELETE FROM Payments
            WHERE League = '{ctx.SafeLeagueName}'
              AND Date = {ctx.ActiveDate}
              AND Player = 'Kitty'
              AND Desc = 'CTP'
              AND Detail = 'Carryover{ctpSlot}-{ctx.sFrontBack}'")
                End If

                ' Reset row visuals
                dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
                dgv.Rows(e.RowIndex).Cells("Player").Value = ""
                dgv.Rows(e.RowIndex).Cells("Player").ReadOnly = False
                dgv.Rows(e.RowIndex).ReadOnly = False
                dgv.Rows(e.RowIndex).Cells("Award").Value = "Award"
                dgv.Rows(e.RowIndex).Cells("Award").Style.ForeColor = Color.DarkGreen
                dgv.Rows(e.RowIndex).Cells("Award").Style.BackColor = Color.Empty

                ' Refresh prize money
                UpdatePrizeMoney(suppressDuesPrompt:=True)
                LOGIT($"CTP reset: hole {hole} award removed")
                Exit Sub
            End If
            ' -- AWARD ------------------------------------------------------------
            Dim player As String = dgv.Rows(e.RowIndex).Cells("Player").Value?.ToString()

            If String.IsNullOrEmpty(player) Then
                MessageBox.Show("Please select a winner first.", "No Winner Selected",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Find CTP slot
            Dim slot As Integer = 0
            For i = 1 To ctx.lPar3s.Count
                If ctx.lPar3s(i - 1) = hole Then
                    slot = i
                    Exit For
                End If
            Next

            ' Calculate actual payout from pot - not entry fee from LeagueParms
            Dim par3Count As Integer = If(ctx.lPar3s IsNot Nothing AndAlso ctx.lPar3s.Count > 0, ctx.lPar3s.Count, 1)
            Dim ctpAmount As Decimal = Math.Floor(EditScoresEngine.GetCTPCollected() / par3Count) +
                                   EditScoresEngine.GetCTPCarryover(slot)

            ' Confirm
            If MessageBox.Show($"Award CTP on hole {hole} to {player} for ${ctpAmount:F2}?",
                           "Confirm CTP Award",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) = DialogResult.No Then Exit Sub

            ' Check for duplicate
            Dim existing = GetExistingCTPWinner(hole)
            If existing <> "" Then
                MessageBox.Show($"Hole {hole} CTP already awarded to {existing}.",
                            "Already Awarded",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Award it
            AwardCTP(player, hole, ctpAmount, slot)

            ' Mark row green
            dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightGreen
            dgv.Rows(e.RowIndex).Cells("Player").ReadOnly = True
            dgv.Rows(e.RowIndex).Cells("Award").Value = "Reset"
            dgv.Rows(e.RowIndex).Cells("Award").Style.ForeColor = Color.DarkRed
            dgv.Rows(e.RowIndex).Cells("Award").Style.BackColor = Color.Empty

            ' Refresh prize money
            UpdatePrizeMoney(suppressDuesPrompt:=True)
            LOGIT($"CTP awarded: Hole {hole} ? {player} ${ctpAmount:F2}")

        Catch ex As Exception
            LOGIT("dgCTPs_CellContentClick Error: " & ex.Message)
        Finally
        End Try
    End Sub
    Friend Sub LoadLast5Tab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Last5")

            Dim dt As New DataTable
            Dim season As Integer = CInt(ctx.rLeagueParmrow("Season"))

            Dim showRegularsOnly As Boolean = True
            Dim chk = TryCast(tp.Controls.Find("chkRegulars", True).FirstOrDefault(), CheckBox)
            If chk IsNot Nothing Then showRegularsOnly = chk.Checked

            Dim sql As String = $"
SELECT 
    L.Player,
    P.FirstName,
    P.LastName,
    (
        SELECT H.Hdcp
        FROM Handicaps H
        WHERE H.Player = L.Player
          AND H.League = L.League
          AND H.Date = L.LastDate
        LIMIT 1
    ) AS Hdcp,
    L.Last5,
    L.LastDate AS Date
FROM PlayerLast5 L
LEFT JOIN Players P ON P.Player = L.Player
LEFT JOIN Teams T 
    ON T.League = L.League 
   AND T.Player = L.Player 
   AND T.Year = {season}
WHERE L.League = @League
  AND L.LastDate = (
      SELECT MAX(L2.LastDate) FROM PlayerLast5 L2
      WHERE L2.League = L.League
        AND L2.Player = L.Player
        AND L2.LastDate <= @ActiveDate
  )
{If(showRegularsOnly, "AND T.Player IS NOT NULL", "")}
ORDER BY COALESCE(P.LastName, L.Player), COALESCE(P.FirstName, '')"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@ActiveDate", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using

            ' --- UI ---
            tp.Controls.Clear()
            tp.AutoScroll = True

            Dim chkNew As New CheckBox()
            chkNew.Name = "chkRegulars"
            chkNew.Text = "Regulars Only"
            chkNew.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            chkNew.Location = New Point(10, 12)
            chkNew.AutoSize = True
            chkNew.Checked = showRegularsOnly
            tp.Controls.Add(chkNew)

            AddHandler chkNew.CheckedChanged, Sub(s, ev) LoadLast5Tab(tc)

            Dim lbl As New Label()
            lbl.Text = "Last 5 Scores"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(130, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            Dim dgv As New DataGridView()
            dgv.Name = "dgLast5"
            dgv.Location = New Point(10, 40)
            tp.Controls.Add(dgv)
            dgv.DataSource = dt

            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            dgv.ColumnHeadersHeight = 42
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.ReadOnly = True
            dgv.ScrollBars = ScrollBars.None
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            Dim colSettings As New Dictionary(Of String, (Width As Integer, Header As String, Align As DataGridViewContentAlignment)) From {
            {"Player", (140, "Player", DataGridViewContentAlignment.MiddleLeft)},
            {"Hdcp", (60, "Hdcp", DataGridViewContentAlignment.MiddleCenter)},
            {"Last5", (160, "Last 5 Scores", DataGridViewContentAlignment.MiddleCenter)},
            {"Date", (80, "Date", DataGridViewContentAlignment.MiddleCenter)}
        }

            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    Dim settings = colSettings(col.Name)
                    col.Visible = True
                    col.Width = settings.Width
                    col.HeaderText = settings.Header
                    col.DefaultCellStyle.Alignment = settings.Align
                    col.HeaderCell.Style.Alignment = settings.Align
                Else
                    col.Visible = False
                End If
            Next

            dgv.Size = New Size(
            dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3,
            (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 3)

            If dgv.Height > 500 Then
                dgv.Height = 500
                dgv.ScrollBars = ScrollBars.Vertical
            End If

            tp.AutoScroll = False
            dgv.ClearSelection()

            AddHandler dgv.CellFormatting, Sub(s, ev)
                                               If ev.RowIndex < 0 Then Exit Sub
                                               If dgv.Columns(ev.ColumnIndex).Name <> "Last5" Then Exit Sub
                                               If ev.Value Is Nothing OrElse IsDBNull(ev.Value) Then Exit Sub

                                               Dim last5Str As String = ev.Value.ToString()
                                               If String.IsNullOrEmpty(last5Str) Then Exit Sub

                                               Dim parts() As String = last5Str.Split("-"c)
                                               Dim scores As New List(Of Integer)
                                               For Each p As String In parts
                                                   Dim n As Integer
                                                   If Integer.TryParse(p.Trim(), n) Then scores.Add(n)
                                               Next

                                               Dim usedIndices As New List(Of Integer)
                                               Select Case scores.Count
                                                   Case 5
                                                       Dim maxVal As Integer = scores.Max()
                                                       Dim minVal As Integer = scores.Min()
                                                       Dim maxIdx As Integer = scores.LastIndexOf(maxVal)
                                                       Dim minIdx As Integer = scores.IndexOf(minVal)
                                                       If maxIdx = minIdx Then
                                                           For i As Integer = 0 To 4
                                                               usedIndices.Add(i)
                                                           Next
                                                       Else
                                                           For i As Integer = 0 To scores.Count - 1
                                                               If i <> maxIdx AndAlso i <> minIdx Then usedIndices.Add(i)
                                                           Next
                                                       End If
                                                   Case 4
                                                       Dim maxVal As Integer = scores.Max()
                                                       Dim maxIdx As Integer = scores.LastIndexOf(maxVal)
                                                       For i As Integer = 0 To scores.Count - 1
                                                           If i <> maxIdx Then usedIndices.Add(i)
                                                       Next
                                                   Case Else
                                                       For i As Integer = 0 To scores.Count - 1
                                                           usedIndices.Add(i)
                                                       Next
                                               End Select

                                               Dim sb As New System.Text.StringBuilder()
                                               For i As Integer = 0 To scores.Count - 1
                                                   If i > 0 Then sb.Append("  ")
                                                   If usedIndices.Contains(i) Then
                                                       sb.Append($"[{scores(i)}]")
                                                   Else
                                                       sb.Append($" {scores(i)} ")
                                                   End If
                                               Next

                                               ev.Value = sb.ToString()
                                               ev.FormattingApplied = True
                                               ev.CellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                                               ev.CellStyle.ForeColor = Color.DarkGreen
                                               ev.CellStyle.BackColor = Color.FromArgb(240, 255, 240)
                                           End Sub

            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadLast5Tab Error: " & ex.Message)
        End Try
    End Sub
    Friend Sub LoadMatchesTab(tc As TabControl)
        LOGIT($"LoadMatchesTab querying Date={ctx.ActiveDate} League={ctx.sLeagueName}")
        ohelper.wk = ""
        Try
            Dim tp = tc.TabPages("Matches")
            Dim dgv = DirectCast(tp.Controls("dgMatches"), DataGridView)

            ' --- Read directly from Matches + Scores tables ---
            Dim freshData As New DataTable
            Using cmd As New SQLiteCommand("
            SELECT 
                M.Partner,
                M.Player,
                COALESCE(T.Team, Su.Team, 0) AS Team,
                COALESCE(T.Grade, Su.Grade, 'N/A') AS Grade,
                S.Net,
                S.Gross,
                COALESCE(
                    (SELECT H.Hdcp FROM Handicaps H 
                     WHERE H.Player = M.Player AND H.League = M.League 
                       AND H.Date < M.Date AND H.Hdcp <> '' 
                     ORDER BY H.Date DESC LIMIT 1),
                    H2.Hdcp) AS PHdcp,
                CASE 
                    WHEN NOT EXISTS (
                        SELECT 1 FROM Scores s3
                        WHERE s3.League = M.League
                          AND s3.Date = M.Date
                          AND s3.Player = M.Player
                          AND s3.Gross IS NOT NULL AND s3.Gross > 0
                    ) THEN 999
                    ELSE (
                        SELECT SUM(CAST(s2.Net AS INTEGER))
                        FROM Matches m2
                        JOIN Scores s2 ON s2.League = m2.League
                            AND s2.Date = m2.Date
                            AND s2.Player = m2.Player
                        LEFT JOIN Teams t2 ON t2.League = m2.League
                            AND t2.Player = m2.Player
                            AND t2.Year = CAST(SUBSTR(m2.Date,1,4) AS INTEGER)
                        LEFT JOIN Subs su2 ON su2.League = m2.League
                            AND su2.Date = m2.Date
                            AND su2.Player = m2.Player
                        WHERE m2.League = M.League
                          AND m2.Date = M.Date
                          AND COALESCE(t2.Team, su2.Team) = COALESCE(T.Team, Su.Team)
                          AND s2.Net IS NOT NULL AND s2.Net != ''
                    )
                END AS Team_Net,
                M.Opponent,
                M.Points,
                M.Team_Points
            FROM Matches M
            LEFT JOIN Scores S ON S.League = M.League 
                AND S.Date = M.Date AND S.Player = M.Player
            LEFT JOIN Teams T ON T.League = M.League AND T.Player = M.Player 
                AND T.Year = CAST(SUBSTR(M.Date,1,4) AS INTEGER)
            LEFT JOIN Subs Su ON Su.League = M.League 
                AND Su.Date = M.Date AND Su.Player = M.Player
            LEFT JOIN Handicaps H2 ON H2.League = M.League 
                AND H2.Date = M.Date AND H2.Player = M.Player
            WHERE M.League = @League AND M.Date = @Date
            ORDER BY M.Partner", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(freshData)
                End Using
            End Using

            If freshData Is Nothing OrElse freshData.Rows.Count = 0 Then Exit Sub
            RecalculateMatchPoints(freshData)

            ' --- Bind directly ---
            dgv.AutoGenerateColumns = True
            dgv.DataSource = freshData

            ' Hide columns we don't need
            Dim showCols As New List(Of String) From {
    "Team", "Player", "Grade", "Net", "PHdcp", "Team_Net", "Opponent", "Points", "Team_Points"
}
            For Each col As DataGridViewColumn In dgv.Columns
                col.Visible = showCols.Contains(col.Name)
            Next

            ' --- Reset row colors ---
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty
            dgv.RowsDefaultCellStyle.BackColor = Color.White
            dgv.DefaultCellStyle.BackColor = Color.White

            ' --- Styling ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersHeight = 28
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

            ' --- Size ---
            Dim targetHeight As Integer = (dgv.Rows.Count * 22) + dgv.ColumnHeadersHeight + 2
            dgv.Size = New Size(550, targetHeight)

            ' --- Lockdown ---
            dgv.ReadOnly = True
            dgv.RowHeadersVisible = False
            dgv.AllowUserToResizeColumns = False
            dgv.AllowUserToResizeRows = False
            dgv.ScrollBars = ScrollBars.None

            For Each col As DataGridViewColumn In dgv.Columns
                col.SortMode = DataGridViewColumnSortMode.NotSortable
            Next

            ' --- Team grouping colors ---
            For i As Integer = 0 To dgv.Rows.Count - 1
                If i Mod 8 < 4 Then
                    dgv.Rows(i).DefaultCellStyle.BackColor = Color.LightBlue
                Else
                    dgv.Rows(i).DefaultCellStyle.BackColor = Color.White
                End If
            Next

            dgv.ClearSelection()

            ' --- Formatting handler ---
            RemoveHandler dgv.CellFormatting, AddressOf dgvMatches_CellFormatting
            AddHandler dgv.CellFormatting, AddressOf dgvMatches_CellFormatting

        Catch ex As Exception
            LOGIT("LoadMatchesTab Error: " & ex.Message)
        End Try
    End Sub

    Private Sub RecalculateMatchPoints(matches As DataTable)
        If matches Is Nothing OrElse matches.Rows.Count = 0 Then Exit Sub

        If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
        Using tran = ctx.Conn.BeginTransaction()
            Using cmd As New SQLiteCommand("
                UPDATE Matches
                SET Points = @Points,
                    Team_Points = @TeamPoints,
                    Team_Net = @TeamNet
                WHERE League = @League
                  AND Date = @Date
                  AND Player = @Player", ctx.Conn, tran)

                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.Parameters.Add("@Player", DbType.String)
                cmd.Parameters.Add("@Points", DbType.Decimal)
                cmd.Parameters.Add("@TeamPoints", DbType.Decimal)
                cmd.Parameters.Add("@TeamNet", DbType.Object)

                For matchStart As Integer = 0 To matches.Rows.Count - 1 Step 4
                    If matchStart + 3 >= matches.Rows.Count Then Exit For

                    Dim team1 = New DataRow() {matches.Rows(matchStart), matches.Rows(matchStart + 1)}
                    Dim team2 = New DataRow() {matches.Rows(matchStart + 2), matches.Rows(matchStart + 3)}
                    Dim team1Net As Decimal = MatchTeamNet(team1)
                    Dim team2Net As Decimal = MatchTeamNet(team2)

                    For i As Integer = 0 To 3
                        Dim row = matches.Rows(matchStart + i)
                        Dim isTeam1 As Boolean = i < 2
                        Dim isAPlayer As Boolean = i = 0 OrElse i = 2
                        Dim myNet As Decimal = MatchPlayerNet(row)
                        Dim oppNet As Decimal = MatchPlayerNet(matches.Rows(matchStart + If(isTeam1, i + 2, i - 2)))
                        Dim points As Decimal = MatchPoints(myNet, oppNet)
                        Dim teamPoints As Decimal = 0D
                        Dim teamNetValue As Object = DBNull.Value

                        If isAPlayer Then
                            Dim myTeamNet As Decimal = If(isTeam1, team1Net, team2Net)
                            Dim oppTeamNet As Decimal = If(isTeam1, team2Net, team1Net)
                            teamPoints = MatchPoints(myTeamNet, oppTeamNet)
                            teamNetValue = If(myTeamNet >= 999D, CObj(999D), CObj(myTeamNet))
                        End If

                        row("Points") = points
                        row("Team_Points") = teamPoints
                        row("Team_Net") = teamNetValue

                        cmd.Parameters("@Player").Value = row("Player").ToString()
                        cmd.Parameters("@Points").Value = points
                        cmd.Parameters("@TeamPoints").Value = teamPoints
                        cmd.Parameters("@TeamNet").Value = teamNetValue
                        cmd.ExecuteNonQuery()
                    Next
                Next
            End Using
            tran.Commit()
        End Using
    End Sub

    Private Function MatchPlayerNet(row As DataRow) As Decimal
        If row Is Nothing Then Return 999D
        If IsDBNull(row("Gross")) OrElse Not IsNumeric(row("Gross")) OrElse CDec(row("Gross")) <= 0 Then Return 999D
        If IsDBNull(row("Net")) OrElse Not IsNumeric(row("Net")) Then Return 999D
        Return CDec(row("Net"))
    End Function

    Private Function MatchTeamNet(rows As IEnumerable(Of DataRow)) As Decimal
        Dim rowList = rows.ToList()
        If rowList.Count = 0 OrElse MatchPlayerNet(rowList(0)) >= 999D Then Return 999D
        Return rowList.Sum(Function(r)
                               Dim net = MatchPlayerNet(r)
                               Return If(net >= 999D, 0D, net)
                           End Function)
    End Function

    Private Function MatchPoints(myNet As Decimal, oppNet As Decimal) As Decimal
        If myNet >= 999D AndAlso oppNet >= 999D Then Return 0.5D
        If myNet >= 999D Then Return 0D
        If oppNet >= 999D Then Return 1D
        If myNet < oppNet Then Return 1D
        If myNet > oppNet Then Return 0D
        Return 0.5D
    End Function

    Private Sub dgvMatches_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)
            If e.RowIndex < 0 Then Exit Sub
            Dim row = dgv.Rows(e.RowIndex)
            Dim colName As String = dgv.Columns(e.ColumnIndex).Name

            ' 1. THE 4-ROW BLUE GROUPING
            Dim matchBlue = Color.FromArgb(210, 230, 255)
            If (e.RowIndex \ 4) Mod 2 = 0 Then
                e.CellStyle.BackColor = matchBlue
            Else
                e.CellStyle.BackColor = Color.White
            End If

            ' 2. PINK STATUS (Player & Opponent Name Only)
            Dim netVal = row.Cells("Net").Value
            Dim hasScore As Boolean = (netVal IsNot DBNull.Value AndAlso Not String.IsNullOrWhiteSpace(netVal.ToString()))
            If Not hasScore AndAlso (colName = "Player" OrElse colName = "Opponent") Then
                e.CellStyle.BackColor = Color.Pink
            End If

            ' 3. SHOW 999 AS BLANK for Team_Net and Net
            If colName = "Team_Net" OrElse colName = "Net" Then
                If e.Value IsNot Nothing AndAlso IsNumeric(e.Value) AndAlso CInt(e.Value) = 999 Then
                    e.Value = ""
                    e.FormattingApplied = True
                    e.CellStyle.SelectionBackColor = e.CellStyle.BackColor
                    e.CellStyle.SelectionForeColor = Color.Black
                    Exit Sub
                End If
            End If

            ' 4. POINTS HIGHLIGHTS
            If colName = "Points" OrElse colName = "Team_Points" Then
                If e.Value IsNot Nothing AndAlso IsNumeric(e.Value) Then
                    Dim val As Double = Convert.ToDouble(e.Value)
                    If val = 1 Then
                        e.CellStyle.BackColor = Color.LightGreen
                    ElseIf val = 0.5 Then
                        e.CellStyle.BackColor = Color.Yellow
                    End If
                End If
            End If

            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor
            e.CellStyle.SelectionForeColor = Color.Black

        Catch ex As Exception
            ' swallow
        End Try
    End Sub
    Friend Sub LoadScheduleTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Schedule")
            tp.Controls.Clear()
            tp.AutoScroll = True

            Dim season As Integer = CInt(ctx.rLeagueParmrow("Season"))
            Dim teams As Integer = CInt(ctx.rLeagueParmrow("Teams"))
            Dim halfWeeks As Integer = teams - 1
            Dim startDate As Integer = CInt(CDate(ctx.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"))
            Dim endDate As Integer = CInt(CDate(ctx.rLeagueParmrow("EndDate")).ToString("yyyyMMdd"))

            ' --- LOAD SCHEDULE ROWS ---
            Dim dtSched As New DataTable
            Using cmd As New SQLiteCommand(
            $"SELECT * FROM Schedule WHERE League = '{ctx.SafeLeagueName}' AND Date BETWEEN {startDate} AND {endDate} ORDER BY Date", ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dtSched)
                End Using
            End Using

            ' --- LOAD TEAMS ---
            Dim dtTeams As New DataTable
            Using cmd As New SQLiteCommand(
            $"SELECT Team, Grade, Player FROM Teams WHERE League = '{ctx.SafeLeagueName}' AND Year = {season} ORDER BY Team, Grade", ctx.Conn)
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dtTeams)
                End Using
            End Using

            ' --- BUILD TEAM LOOKUP ---
            ' teamPlayers(teamNum) = (APlayer, BPlayer)
            Dim teamPlayers As New Dictionary(Of Integer, (A As String, B As String))
            For Each row As DataRow In dtTeams.Rows
                Dim teamNum As Integer = CInt(row("Team"))
                Dim grade As String = row("Grade").ToString()
                Dim player As String = row("Player").ToString()

                If Not teamPlayers.ContainsKey(teamNum) Then
                    teamPlayers(teamNum) = ("", "")
                End If
                Dim current = teamPlayers(teamNum)
                If grade = "A" Then
                    teamPlayers(teamNum) = (player, current.B)
                Else
                    teamPlayers(teamNum) = (current.A, player)
                End If
            Next

            ' --- FORMAT PLAYER NAME: First + Last Initial ---
            Dim dtPlayerNames As New DataTable
            Using cmd As New SQLiteCommand(
            "SELECT Player, FirstName, LastName FROM Players", ctx.Conn)
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dtPlayerNames)
                End Using
            End Using
            dtPlayerNames.PrimaryKey = New DataColumn() {dtPlayerNames.Columns("Player")}

            Dim formatName As Func(Of String, String) = Function(fullName As String)
                                                            Dim pr = dtPlayerNames.Rows.Find(fullName)
                                                            If pr IsNot Nothing Then
                                                                Dim fn = pr("FirstName").ToString()
                                                                Dim ln = pr("LastName").ToString()
                                                                If ln.Length > 0 Then
                                                                    Return $"{fn} {ln.Substring(0, 1)}"
                                                                End If
                                                            End If
                                                            ' Fallback - split on space
                                                            Dim parts = fullName.Trim().Split(" "c)
                                                            If parts.Length >= 2 Then
                                                                Return $"{parts(0)} {parts(parts.Length - 1).Substring(0, 1)}"
                                                            End If
                                                            Return fullName
                                                        End Function

            ' --- GET PLAYED WEEKS FOR MIDPOINT ---
            Dim playedDates As New List(Of Integer)
            For Each row As DataRow In dtSched.Rows
                playedDates.Add(CInt(row("Date")))
            Next

            Dim midDate As Integer = If(playedDates.Count >= halfWeeks, playedDates(halfWeeks - 1), 99991231)
            Dim secondHalfStart As Integer = If(playedDates.Count >= halfWeeks + 1, playedDates(halfWeeks), 99991231)

            ' --- BUILD SCHEDULE DATATABLE ---
            ' Columns: Team, APlayer, BPlayer, then one per date
            Dim dt1 As New DataTable  ' 1st half
            Dim dt2 As New DataTable  ' 2nd half

            dt1.Columns.Add("Team", GetType(Integer))
            dt1.Columns.Add("A Player", GetType(String))
            dt1.Columns.Add("B Player", GetType(String))

            dt2.Columns.Add("Team", GetType(Integer))
            dt2.Columns.Add("A Player", GetType(String))
            dt2.Columns.Add("B Player", GetType(String))

            ' Add date columns
            For Each row As DataRow In dtSched.Rows
                Dim d As Integer = CInt(row("Date"))
                Dim dateStr As String = DateTime.ParseExact(d.ToString(), "yyyyMMdd", Nothing).ToString("MM/dd")
                If d <= midDate Then
                    If Not dt1.Columns.Contains(dateStr) Then
                        dt1.Columns.Add(dateStr, GetType(String))
                    End If
                Else
                    If Not dt2.Columns.Contains(dateStr) Then
                        dt2.Columns.Add(dateStr, GetType(String))
                    End If
                End If
            Next

            LOGIT($"dt1 columns: {dt1.Columns.Count}")
            LOGIT($"dt1 rows: {dt1.Rows.Count}")
            For Each col As DataColumn In dt1.Columns
                LOGIT($"  col: {col.ColumnName}")
            Next

            Dim sortedTeams = New List(Of Integer)(teamPlayers.Keys)
            sortedTeams.Sort()
            For Each teamNum In sortedTeams
                Dim nr1 = dt1.NewRow()
                nr1("Team") = teamNum
                nr1("A Player") = formatName(teamPlayers(teamNum).A)
                nr1("B Player") = formatName(teamPlayers(teamNum).B)
                dt1.Rows.Add(nr1)

                Dim nr2 = dt2.NewRow()
                nr2("Team") = teamNum
                nr2("A Player") = formatName(teamPlayers(teamNum).A)
                nr2("B Player") = formatName(teamPlayers(teamNum).B)
                dt2.Rows.Add(nr2)
            Next

            ' --- FILL OPPONENT DATA ---
            For Each schedRow As DataRow In dtSched.Rows
                Dim d As Integer = CInt(schedRow("Date"))
                ' Building date columns
                Dim dateStr As String = DateTime.ParseExact(d.ToString(), "yyyyMMdd", Nothing).ToString("MM/dd")

                Dim isFirstHalf As Boolean = d <= midDate
                Dim targetDt As DataTable = If(isFirstHalf, dt1, dt2)

                If Not targetDt.Columns.Contains(dateStr) Then Continue For

                ' Loop matchup columns 1-6
                For col As Integer = 1 To 6
                    Dim matchup As String = If(schedRow(col.ToString()) Is DBNull.Value, "", schedRow(col.ToString()).ToString().Trim())
                    If matchup = "" Then Continue For

                    Dim parts = matchup.Split("v"c)
                    If parts.Length <> 2 Then Continue For

                    Dim teamA As Integer = CInt(parts(0).Trim())
                    Dim teamB As Integer = CInt(parts(1).Trim())

                    ' Find rows for each team and set opponent team number
                    For Each teamRow As DataRow In targetDt.Rows
                        Dim rowTeam As Integer = CInt(teamRow("Team"))
                        If rowTeam = teamA Then
                            teamRow(dateStr) = teamB.ToString()
                        ElseIf rowTeam = teamB Then
                            teamRow(dateStr) = teamA.ToString()
                        End If
                    Next
                Next
            Next

            ' --- BUILD GRIDS ---
            ' Current date highlight
            Dim currentDateStr As String = DateTime.ParseExact(ctx.ActiveDate.ToString(), "yyyyMMdd", Nothing).ToString("MM/dd")
            Dim yOffset As Integer = 10

            BuildScheduleGrid(tp, dt1, "1st Half Schedule", yOffset, currentDateStr)
            yOffset += (teams * 24) + 28 + 50

            If secondHalfStart < 99991231 Then
                BuildScheduleGrid(tp, dt2, "2nd Half Schedule", yOffset, currentDateStr)
                yOffset += (teams * 24) + 28 + 50
            End If

            ' --- TEAM REFERENCE GRID ---
            Dim lblRef As New Label()
            lblRef.Text = "Team Reference"
            lblRef.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lblRef.Location = New Point(10, yOffset)
            lblRef.AutoSize = True
            tp.Controls.Add(lblRef)
            yOffset += 25

            Dim dtRef As New DataTable
            dtRef.Columns.Add("Team", GetType(Integer))
            dtRef.Columns.Add("A Player", GetType(String))
            dtRef.Columns.Add("B Player", GetType(String))

            For Each kvp In teamPlayers.OrderBy(Function(k) k.Key)
                Dim nr = dtRef.NewRow()
                nr("Team") = kvp.Key
                nr("A Player") = kvp.Value.A  ' full name for reference
                nr("B Player") = kvp.Value.B
                dtRef.Rows.Add(nr)
            Next

            Dim dgvRef As New DataGridView()
            dgvRef.Name = "dgTeamRef"
            dgvRef.Location = New Point(10, yOffset)
            tp.Controls.Add(dgvRef)
            dgvRef.DataSource = dtRef
            StyleScheduleGrid(dgvRef)

            For Each col As DataGridViewColumn In dgvRef.Columns
                col.Width = If(col.Name = "Team", 50, 150)
                col.DefaultCellStyle.Alignment = If(col.Name = "Team",
                DataGridViewContentAlignment.MiddleCenter,
                DataGridViewContentAlignment.MiddleLeft)
            Next

            dgvRef.Size = New Size(
            dgvRef.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3,
            (dgvRef.Rows.Count * dgvRef.RowTemplate.Height) + dgvRef.ColumnHeadersHeight + 3)

        Catch ex As Exception
            LOGIT("LoadScheduleTab Error: " & ex.Message)
        End Try
    End Sub
    Private Sub BuildScheduleGrid(tp As TabPage,
                               dt As DataTable,
                               title As String,
                               yOffset As Integer,
                               currentDateStr As String)
        Dim lbl As New Label()
        lbl.Text = title
        lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        lbl.Location = New Point(10, yOffset)
        lbl.AutoSize = True
        tp.Controls.Add(lbl)

        Dim dgv As New DataGridView()
        dgv.Name = $"dg{title.Replace(" ", "")}"
        dgv.Location = New Point(10, yOffset + 25)
        tp.Controls.Add(dgv)
        dgv.DataSource = dt
        StyleScheduleGrid(dgv)

        For Each col As DataGridViewColumn In dgv.Columns
            If col.Name = "Team" Then
                col.Width = 45
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            ElseIf col.Name = "A Player" OrElse col.Name = "B Player" Then
                col.Width = 90
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            Else
                col.Width = 55
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                ' Highlight current week
                If col.Name = currentDateStr Then
                    col.HeaderCell.Style.BackColor = Color.SteelBlue
                    col.HeaderCell.Style.ForeColor = Color.White
                    col.DefaultCellStyle.BackColor = Color.LightSteelBlue
                End If
            End If
        Next

        dgv.Size = New Size(
        dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3,
        (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 3)

        dgv.ClearSelection()
        dgv.CurrentCell = Nothing
    End Sub
    Private Sub StyleScheduleGrid(dgv As DataGridView)
        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 8.5!)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 8.5!, FontStyle.Bold)
        dgv.RowTemplate.Height = 22
        dgv.ColumnHeadersHeight = 28
        dgv.RowHeadersVisible = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToResizeRows = False
        dgv.ReadOnly = True
        dgv.ScrollBars = ScrollBars.None
        dgv.BorderStyle = BorderStyle.None
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgv.GridColor = Color.LightGray
        dgv.BackgroundColor = Color.White
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        dgv.AutoGenerateColumns = True
    End Sub
    Friend Sub LoadStandingsTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Standings")
            Dim splitSeason As Boolean = ctx.rLeagueParmrow("SplitSeason").ToString() = "Y"
            Dim season As Integer = CInt(ctx.rLeagueParmrow("Season"))
            Dim teams As Integer = CInt(ctx.rLeagueParmrow("Teams"))
            Dim halfWeeks As Integer = teams - 1

            Dim startDate As Integer = CInt(CDate(ctx.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"))
            Dim endDate As Integer = Math.Min(
    CInt(CDate(ctx.rLeagueParmrow("EndDate")).ToString("yyyyMMdd")),
    CInt(ctx.ActiveDate)
)

            Dim sql As String = BuildStandingsSQL(season, startDate, endDate, halfWeeks, splitSeason)

            Dim dt As New DataTable
            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using

            tp.Controls.Clear()
            tp.AutoScroll = False

            If splitSeason Then
                Dim dt1 As DataTable = dt.Clone()
                Dim dt2 As DataTable = dt.Clone()
                For Each row As DataRow In dt.Rows
                    If row("Half").ToString() = "1st Half" Then
                        dt1.ImportRow(row)
                    Else
                        dt2.ImportRow(row)
                    End If
                Next

                AddPointsBack(dt1)

                ' Available height split between two grids
                Dim availableHeight As Integer = (tc.Height - 150) \ 2

                ' 1st Half label
                Dim lbl1 As New Label()
                lbl1.Text = "1st Half Standings"
                lbl1.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                lbl1.Location = New Point(10, 10)
                lbl1.AutoSize = True
                tp.Controls.Add(lbl1)

                ' 1st Half grid
                Dim dgv1 As New DataGridView()
                dgv1.Name = "dgStandings1"
                dgv1.Location = New Point(10, 35)
                tp.Controls.Add(dgv1)
                dgv1.DataSource = dt1
                StyleStandingsGrid(dgv1)
                ApplyStandingsColumns(dgv1)
                Dim h1 As Integer = (dgv1.Rows.Count * dgv1.RowTemplate.Height) + dgv1.ColumnHeadersHeight + 2
                If h1 > availableHeight Then
                    dgv1.Size = New Size(770, availableHeight)
                    dgv1.ScrollBars = ScrollBars.Vertical
                Else
                    dgv1.Size = New Size(770, h1)
                    dgv1.ScrollBars = ScrollBars.None
                End If
                ColorTopThree(dgv1)

                ' 2nd Half label
                Dim lbl2 As New Label()
                lbl2.Text = "2nd Half Standings"
                lbl2.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                lbl2.Location = New Point(10, 35 + dgv1.Height + 20)
                lbl2.AutoSize = True
                tp.Controls.Add(lbl2)

                Dim secondHalfStarted As Boolean = dt2.Rows.Count > 0 AndAlso CDec(dt2.Rows(0)("GrandTotal")) > 0

                If secondHalfStarted Then
                    AddPointsBack(dt2)
                    Dim dgv2 As New DataGridView()
                    dgv2.Name = "dgStandings2"
                    dgv2.Location = New Point(10, 35 + dgv1.Height + 45)
                    tp.Controls.Add(dgv2)
                    dgv2.DataSource = dt2
                    StyleStandingsGrid(dgv2)
                    ApplyStandingsColumns(dgv2)
                    Dim h2 As Integer = (dgv2.Rows.Count * dgv2.RowTemplate.Height) + dgv2.ColumnHeadersHeight + 2
                    If h2 > availableHeight Then
                        dgv2.Size = New Size(770, availableHeight)
                        dgv2.ScrollBars = ScrollBars.Vertical
                    Else
                        dgv2.Size = New Size(770, h2)
                        dgv2.ScrollBars = ScrollBars.None
                    End If
                    ColorTopThree(dgv2)
                End If

            Else
                Dim lbl As New Label()
                lbl.Text = "Season Standings"
                lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                lbl.Location = New Point(10, 10)
                lbl.AutoSize = True
                tp.Controls.Add(lbl)

                AddPointsBack(dt)

                Dim dgv As New DataGridView()
                dgv.Name = "dgStandings"
                dgv.Location = New Point(10, 35)
                tp.Controls.Add(dgv)
                dgv.DataSource = dt
                StyleStandingsGrid(dgv)
                ApplyStandingsColumns(dgv)

                Dim availableHeight As Integer = tc.Height - 100
                Dim h As Integer = (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 2
                If h > availableHeight Then
                    dgv.Size = New Size(730, availableHeight)
                    dgv.ScrollBars = ScrollBars.Vertical
                Else
                    dgv.Size = New Size(730, h)
                    dgv.ScrollBars = ScrollBars.None
                End If
                ColorTopThree(dgv)
            End If

        Catch ex As Exception
            LOGIT("LoadStandingsTab Error: " & ex.Message)
        End Try
    End Sub
    Private Sub AddPointsBack(dt As DataTable)
        dt.Columns.Add("PointsBack", GetType(Decimal))
        If dt.Rows.Count = 0 Then Exit Sub

        ' First row is leader since sorted by GrandTotal DESC
        Dim leader As Decimal = CDec(dt.Rows(0)("GrandTotal"))
        For Each row As DataRow In dt.Rows
            row("PointsBack") = leader - CDec(row("GrandTotal"))
        Next
    End Sub
    Private Function BuildStandingsSQL(season As Integer,
                                        startDate As Integer,
                                        endDate As Integer,
                                        halfWeeks As Integer,
                                        splitSeason As Boolean) As String
        Dim sb As New StringBuilder()

        sb.AppendLine("WITH PlayedWeeks AS (")
        sb.AppendLine("    SELECT Date, ROW_NUMBER() OVER (ORDER BY Date) AS WeekNum")
        sb.AppendLine("    FROM (")
        sb.AppendLine("        SELECT DISTINCT Date")
        sb.AppendLine("        FROM Matches")
        sb.AppendLine($"        WHERE League = '{ctx.SafeLeagueName}'")
        sb.AppendLine($"          AND Date >= {startDate}")
        sb.AppendLine($"          AND Date <= {endDate}")
        sb.AppendLine("    ) UniqueDates")
        sb.AppendLine("),")
        sb.AppendLine("MidDate AS (")
        sb.AppendLine($"    SELECT COALESCE(")
        sb.AppendLine($"        (SELECT Date FROM PlayedWeeks WHERE WeekNum = {halfWeeks}),")
        sb.AppendLine($"        99991231")
        sb.AppendLine($"    ) AS MidDate")
        sb.AppendLine("),")
        sb.AppendLine("SecondHalfStart AS (")
        sb.AppendLine($"    SELECT COALESCE(")
        sb.AppendLine($"        (SELECT Date FROM PlayedWeeks WHERE WeekNum = {halfWeeks + 1}),")
        sb.AppendLine($"        99991231")
        sb.AppendLine($"    ) AS StartDate")
        sb.AppendLine("),")
        sb.AppendLine("PlayerTeams AS (")
        sb.AppendLine("    SELECT")
        sb.AppendLine("        M.League, M.Player, M.Date,")
        sb.AppendLine("        CAST(M.Points AS DECIMAL) AS Points,")
        sb.AppendLine("        CAST(M.Team_Points AS DECIMAL) AS TeamPoints,")
        sb.AppendLine("        COALESCE(S.Team, T.Team) AS Team,")
        sb.AppendLine("        COALESCE(S.Grade, T.Grade) AS Grade")
        sb.AppendLine("    FROM Matches M")
        sb.AppendLine("    LEFT JOIN Subs S")
        sb.AppendLine("        ON S.League = M.League")
        sb.AppendLine("       AND S.Player = M.Player")
        sb.AppendLine("       AND S.Date = M.Date")
        sb.AppendLine("    LEFT JOIN Teams T")
        sb.AppendLine("        ON T.League = M.League")
        sb.AppendLine("       AND T.Player = M.Player")
        sb.AppendLine($"       AND T.Year = {season}")
        sb.AppendLine($"    WHERE M.League = '{ctx.SafeLeagueName}'")
        sb.AppendLine($"      AND M.Date BETWEEN {startDate} AND {endDate}")
        sb.AppendLine("      AND COALESCE(S.Team, T.Team) IS NOT NULL")
        sb.AppendLine(")")

        ' Build the SELECT - used for both halves
        Dim selectClause As String = "
    SELECT
        @Half AS Half,
        T_A.Team,
        T_A.Player AS APlayer,
        T_B.Player AS BPlayer,
        ROUND(IFNULL(SUM(CASE WHEN PT.Grade = 'A' AND PT.Team = T_A.Team THEN PT.Points ELSE 0 END), 0), 1) AS APoints,
        ROUND(IFNULL(SUM(CASE WHEN PT.Grade = 'B' AND PT.Team = T_A.Team THEN PT.Points ELSE 0 END), 0), 1) AS BPoints,
        ROUND(IFNULL(SUM(CASE WHEN PT.Team = T_A.Team THEN PT.Points ELSE 0 END), 0), 1) AS PlayerPoints,
        ROUND(IFNULL(SUM(CASE WHEN PT.Grade = 'A' AND PT.Team = T_A.Team THEN PT.TeamPoints ELSE 0 END), 0), 1) AS TeamPoints,
        ROUND(
            IFNULL(SUM(CASE WHEN PT.Team = T_A.Team THEN PT.Points ELSE 0 END), 0) +
            IFNULL(SUM(CASE WHEN PT.Grade = 'A' AND PT.Team = T_A.Team THEN PT.TeamPoints ELSE 0 END), 0)
        , 1) AS GrandTotal
    FROM Teams T_A
    JOIN Teams T_B
        ON T_B.League = T_A.League
       AND T_B.Year = T_A.Year
       AND T_B.Team = T_A.Team
       AND T_B.Grade = 'B'
    LEFT JOIN PlayerTeams PT
        ON PT.Team = T_A.Team
       AND PT.League = T_A.League
       AND @DateFilter
    WHERE T_A.League = '@League'
      AND T_A.Year = @Year
      AND T_A.Grade = 'A'
    GROUP BY T_A.Team, T_A.Player, T_B.Player"

        If splitSeason Then
            sb.AppendLine(selectClause.
                Replace("@Half", "'1st Half'").
                Replace("@DateFilter", $"PT.Date BETWEEN {startDate} AND (SELECT MidDate FROM MidDate)").
                Replace("@League", ctx.SafeLeagueName).
                Replace("@Year", season.ToString()))

            sb.AppendLine("UNION ALL")

            sb.AppendLine(selectClause.
                Replace("@Half", "'2nd Half'").
                Replace("@DateFilter", $"PT.Date BETWEEN (SELECT StartDate FROM SecondHalfStart) AND {endDate}").
                Replace("@League", ctx.SafeLeagueName).
                Replace("@Year", season.ToString()))
        Else
            sb.AppendLine(selectClause.
                Replace("@Half", "'Full Season'").
                Replace("@DateFilter", $"PT.Date BETWEEN {startDate} AND {endDate}").
                Replace("@League", ctx.SafeLeagueName).
                Replace("@Year", season.ToString()))
        End If

        sb.AppendLine("ORDER BY Half, GrandTotal DESC")
        Return sb.ToString()
    End Function
    Private Sub StyleStandingsGrid(dgv As DataGridView)
        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
        dgv.RowTemplate.Height = 24
        dgv.ColumnHeadersHeight = 28
        dgv.RowHeadersVisible = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToResizeRows = False
        dgv.AllowUserToResizeColumns = False
        dgv.ReadOnly = True
        dgv.ScrollBars = ScrollBars.None
        dgv.BorderStyle = BorderStyle.None
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgv.GridColor = Color.LightGray
        dgv.BackgroundColor = Color.White
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        dgv.AutoGenerateColumns = True
    End Sub
    Private Sub ApplyStandingsColumns(dgv As DataGridView)
        If dgv.Columns.Contains("Half") Then dgv.Columns("Half").Visible = False

        Dim colSettings As New Dictionary(Of String, (Width As Integer, Format As String, Align As DataGridViewContentAlignment)) From {
        {"Team", (50, "", DataGridViewContentAlignment.MiddleCenter)},
        {"APlayer", (130, "", DataGridViewContentAlignment.MiddleLeft)},
        {"BPlayer", (130, "", DataGridViewContentAlignment.MiddleLeft)},
        {"APoints", (65, "N1", DataGridViewContentAlignment.MiddleRight)},
        {"BPoints", (65, "N1", DataGridViewContentAlignment.MiddleRight)},
        {"PlayerPoints", (75, "N1", DataGridViewContentAlignment.MiddleRight)},
        {"TeamPoints", (75, "N1", DataGridViewContentAlignment.MiddleRight)},
        {"GrandTotal", (75, "N1", DataGridViewContentAlignment.MiddleRight)},
        {"PointsBack", (55, "N1", DataGridViewContentAlignment.MiddleRight)}
    }

        For Each col As DataGridViewColumn In dgv.Columns
            If colSettings.ContainsKey(col.Name) Then
                Dim settings = colSettings(col.Name)
                col.Visible = True
                col.Width = settings.Width
                col.DefaultCellStyle.Alignment = settings.Align
                col.HeaderCell.Style.Alignment = settings.Align
                If settings.Format <> "" Then
                    col.DefaultCellStyle.Format = settings.Format
                End If
            Else
                col.Visible = False
            End If
        Next

        If dgv.Columns.Contains("APlayer") Then dgv.Columns("APlayer").HeaderText = "A Player"
        If dgv.Columns.Contains("BPlayer") Then dgv.Columns("BPlayer").HeaderText = "B Player"
        If dgv.Columns.Contains("APoints") Then dgv.Columns("APoints").HeaderText = "A Pts"
        If dgv.Columns.Contains("BPoints") Then dgv.Columns("BPoints").HeaderText = "B Pts"
        If dgv.Columns.Contains("PlayerPoints") Then dgv.Columns("PlayerPoints").HeaderText = "Player Pts"
        If dgv.Columns.Contains("TeamPoints") Then dgv.Columns("TeamPoints").HeaderText = "Team Pts"
        If dgv.Columns.Contains("GrandTotal") Then dgv.Columns("GrandTotal").HeaderText = "Total"
        If dgv.Columns.Contains("PointsBack") Then dgv.Columns("PointsBack").HeaderText = "GB"
    End Sub
    Private Sub ColorTopThree(dgv As DataGridView)
        Dim colors() As Color = {
            Color.FromArgb(255, 215, 0),    ' Gold   - 1st
            Color.FromArgb(192, 192, 192),  ' Silver - 2nd
            Color.FromArgb(205, 127, 50)    ' Bronze - 3rd
        }
        For i As Integer = 0 To Math.Min(2, dgv.Rows.Count - 1)
            dgv.Rows(i).DefaultCellStyle.BackColor = colors(i)
            dgv.Rows(i).DefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
        Next
    End Sub
    Friend Sub LoadSkinsCTPsTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("SkinsCTPs")
            tp.Controls.Clear()
            tp.AutoScroll = True

            ' --- FETCH SKIN AND CTP WINNINGS FOR CURRENT DATE ---
            Dim dt As New DataTable
            Dim sql As String = $"
            SELECT
                P.Player,
                SUM(CASE WHEN P.Desc = 'Skin' AND P.Detail LIKE '#%' THEN 1 ELSE 0 END) AS SkinsWon,
                SUM(CASE WHEN P.Desc = 'Skin' AND P.Detail LIKE '#%' THEN P.Earned ELSE 0 END) AS SkinEarnings,
                SUM(CASE WHEN P.Desc = 'CTP' AND P.Detail LIKE '#%' THEN 1 ELSE 0 END) AS CTPsWon,
                SUM(CASE WHEN P.Desc = 'CTP' AND P.Detail LIKE '#%' THEN P.Earned ELSE 0 END) AS CTPEarnings,
                GROUP_CONCAT(CASE WHEN P.Desc = 'CTP' AND P.Detail LIKE '#%' THEN P.Detail ELSE NULL END) AS CTPHoles,
                SUM(CASE WHEN (P.Desc = 'Skin' OR P.Desc = 'CTP') AND P.Detail LIKE '#%' THEN P.Earned ELSE 0 END) AS TotalEarnings
            FROM Payments P
            WHERE P.League = '{ctx.SafeLeagueName}'
              AND P.Date = {ctx.ActiveDate}
              AND P.Player <> 'Kitty'
              AND (P.Desc = 'Skin' OR P.Desc = 'CTP')
              AND P.Detail LIKE '#%'
            GROUP BY P.Player
            ORDER BY TotalEarnings DESC"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using

            ' --- LABEL ---
            Dim lbl As New Label()
            lbl.Text = $"Skins & CTP Winnings - {DateTime.ParseExact(ctx.ActiveDate.ToString(), "yyyyMMdd", Nothing).ToString("MM/dd/yyyy")}"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(10, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            ' --- GRID ---
            Dim dgv As New DataGridView()
            dgv.Name = "dgSkinsCTPs"
            dgv.Location = New Point(10, 35)
            tp.Controls.Add(dgv)
            dgv.DataSource = dt

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 24
            dgv.ColumnHeadersHeight = 42
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.ReadOnly = True
            dgv.ScrollBars = ScrollBars.None
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            ' --- COLUMNS ---
            Dim colSettings As New Dictionary(Of String, (Width As Integer, Header As String, Format As String, Align As DataGridViewContentAlignment)) From {
            {"Player", (140, "Player", "", DataGridViewContentAlignment.MiddleLeft)},
            {"SkinsWon", (70, "Skins Won", "N0", DataGridViewContentAlignment.MiddleCenter)},
            {"SkinEarnings", (90, "Skin $", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"CTPsWon", (70, "CTPs Won", "N0", DataGridViewContentAlignment.MiddleCenter)},
            {"CTPHoles", (80, "CTP Holes", "", DataGridViewContentAlignment.MiddleCenter)},
            {"CTPEarnings", (80, "CTP $", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"TotalEarnings", (90, "Total $", "C2", DataGridViewContentAlignment.MiddleRight)}
        }

            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    Dim settings = colSettings(col.Name)
                    col.Visible = True
                    col.Width = settings.Width
                    col.HeaderText = settings.Header
                    col.DefaultCellStyle.Alignment = settings.Align
                    col.HeaderCell.Style.Alignment = settings.Align
                    If settings.Format <> "" Then
                        col.DefaultCellStyle.Format = settings.Format
                    End If
                Else
                    col.Visible = False
                End If
            Next

            ' --- SIZE TO CONTENT ---
            Dim availableHeight As Integer = tc.Height - 100
            Dim h As Integer = (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 2
            If h > availableHeight Then
                dgv.Size = New Size(dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3, availableHeight)
                dgv.ScrollBars = ScrollBars.Vertical
            Else
                dgv.Size = New Size(dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3, h)
                dgv.ScrollBars = ScrollBars.None
            End If

            ' --- HIGHLIGHT TOP EARNER ---
            If dgv.Rows.Count > 0 Then
                dgv.Rows(0).DefaultCellStyle.BackColor = Color.FromArgb(255, 215, 0)  ' Gold
                dgv.Rows(0).DefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            End If

            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadSkinsCTPsTab Error: " & ex.Message)
        End Try
    End Sub
    Friend Sub LoadKittyTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Kitty")
            tp.Controls.Clear()
            tp.AutoScroll = True

            ' --- FETCH ALL KITTY CARRYOVER ENTRIES ---
            Dim dt As New DataTable
            Dim sql As String = $"
            SELECT
                P.Date,
                P.Desc,
                P.Detail,
                P.Earned,
                P.Comment,
                P.DatePaid
            FROM Payments P
            WHERE P.League = '{ctx.SafeLeagueName}'
              AND P.Player = 'Kitty'
            ORDER BY P.Date DESC, P.Desc, P.Detail"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using

            ' --- TOTAL KITTY BALANCE ---
            Dim totalKitty As Decimal = 0
            For Each row As DataRow In dt.Rows
                totalKitty += CDec(row("Earned"))
            Next

            ' --- LABEL ---
            Dim lbl As New Label()
            lbl.Text = $"Kitty / Carryover Balance - Total: {totalKitty:C2}"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(10, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            ' --- GRID ---
            Dim dgv As New DataGridView()
            dgv.Name = "dgKitty"
            dgv.Location = New Point(10, 35)
            tp.Controls.Add(dgv)
            dgv.DataSource = dt

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 24
            dgv.ColumnHeadersHeight = 28
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.AllowUserToResizeColumns = False
            dgv.ReadOnly = True
            dgv.ScrollBars = ScrollBars.None
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            ' --- COLUMNS ---
            Dim colSettings As New Dictionary(Of String, (Width As Integer, Header As String, Format As String, Align As DataGridViewContentAlignment)) From {
            {"Date", (80, "Date", "", DataGridViewContentAlignment.MiddleCenter)},
            {"Desc", (60, "Type", "", DataGridViewContentAlignment.MiddleCenter)},
            {"Detail", (100, "Detail", "", DataGridViewContentAlignment.MiddleCenter)},
            {"Earned", (80, "Amount", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"Comment", (180, "Comment", "", DataGridViewContentAlignment.MiddleLeft)}
        }

            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    Dim settings = colSettings(col.Name)
                    col.Visible = True
                    col.Width = settings.Width
                    col.HeaderText = settings.Header
                    col.DefaultCellStyle.Alignment = settings.Align
                    col.HeaderCell.Style.Alignment = settings.Align
                    If settings.Format <> "" Then
                        col.DefaultCellStyle.Format = settings.Format
                    End If
                Else
                    col.Visible = False
                End If
            Next

            ' --- FORMAT DATE COLUMN ---
            RemoveHandler dgv.CellFormatting, AddressOf dgKitty_CellFormatting
            AddHandler dgv.CellFormatting, AddressOf dgKitty_CellFormatting

            ' --- SIZE TO CONTENT ---
            Dim availableHeight As Integer = tc.Height - 100
            Dim h As Integer = (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 2
            If h > availableHeight Then
                dgv.Size = New Size(dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3, availableHeight)
                dgv.ScrollBars = ScrollBars.Vertical
            Else
                dgv.Size = New Size(dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3, h)
                dgv.ScrollBars = ScrollBars.None
            End If

            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadKittyTab Error: " & ex.Message)
        End Try
    End Sub
    Private Sub dgKitty_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)
            If e.RowIndex < 0 Then Exit Sub
            If dgv.Columns(e.ColumnIndex).Name <> "Date" Then Exit Sub
            If e.Value Is Nothing OrElse e.Value.ToString() = "" Then Exit Sub

            ' Format integer date to MM/dd/yyyy
            Dim dateInt As Integer = CInt(e.Value)
            e.Value = DateTime.ParseExact(dateInt.ToString(), "yyyyMMdd", Nothing).ToString("MM/dd/yyyy")
            e.FormattingApplied = True
        Catch
            ' swallow
        End Try
    End Sub
    Friend Sub LoadWeeklyPaymentsTab(tc As TabControl)
        ohelper.wk = ""
        Try

            Dim tp = tc.TabPages("WeeklyPayments")
            Dim dgv = DirectCast(tp.Controls("dgWeeklyPayments"), DataGridView)
            dgv.DataSource = Nothing
            If ctx.ActiveDate > DateTime.Now.ToString("yyyyMMdd") Then
                ' Remove existing message label if any
                For Each ctrl As Control In tp.Controls.OfType(Of Label)()
                    If ctrl.Name = "lblFutureDate" Then
                        tp.Controls.Remove(ctrl)
                        ctrl.Dispose()
                    End If
                Next
                ' Show message in tab
                Dim lbl As New Label()
                lbl.Text = "Payments not available for future dates"
                lbl.Font = New Font("Segoe UI", 10.0!, FontStyle.Italic)
                lbl.ForeColor = Color.Gray
                lbl.AutoSize = True
                lbl.Location = New Point(20, 20)
                tp.Controls.Add(lbl)
                Exit Sub
            End If
            If dtScores Is Nothing Then Exit Sub

            ' --- BUILD DATA TABLE ---
            Dim dt As New DataTable
            dt.Columns.Add("Player", GetType(String))
            dt.Columns.Add("Skins In", GetType(String))
            dt.Columns.Add("CTP In", GetType(String))
            dt.Columns.Add("Total In", GetType(String))
            dt.Columns.Add("Skins Out", GetType(String))
            dt.Columns.Add("CTP Out", GetType(String))
            dt.Columns.Add("Total Out", GetType(String))
            ' Hidden helper columns
            dt.Columns.Add("InSkins", GetType(String))
            dt.Columns.Add("InCTPs", GetType(String))
            dt.Columns.Add("SkinsWon", GetType(Decimal))
            dt.Columns.Add("CTPWon", GetType(Decimal))
            dt.Columns.Add("SkinsOutAmt", GetType(Decimal))
            dt.Columns.Add("CTPOutAmt", GetType(Decimal))
            dt.Columns.Add("IsSub", GetType(String))
            dt.Columns.Add("Gross", GetType(Integer))

            Dim mSkins As Decimal = CDec(ctx.rLeagueParmrow("Skins"))
            Dim mCTP As Decimal = CDec(ctx.rLeagueParmrow("Closest"))

            Dim totSkinsIn As Decimal = 0
            Dim totCTPIn As Decimal = 0
            Dim totSkinsOut As Decimal = 0
            Dim totCTPOut As Decimal = 0

            ' Sort by last name
            Dim sortedRows As New List(Of DataRow)
            For Each row As DataRow In dtScores.Rows
                sortedRows.Add(row)
            Next
            sortedRows.Sort(Function(a, b)
                                Dim nameA = a("Player").ToString().Trim().Split(" "c)
                                Dim nameB = b("Player").ToString().Trim().Split(" "c)
                                Dim keyA = If(nameA.Length > 1, nameA(nameA.Length - 1) & nameA(0), nameA(0))
                                Dim keyB = If(nameB.Length > 1, nameB(nameB.Length - 1) & nameB(0), nameB(0))
                                Return String.Compare(keyA, keyB)
                            End Function)

            For Each row As DataRow In sortedRows
                Dim playerName As String = row("Player").ToString()
                Dim safeName = playerName.Replace("'", "''")
                Dim isSub As String = row("IsSub").ToString()
                Dim gross As Integer = CInt(If(IsDBNull(row("Gross")), 0, row("Gross")))
                Dim inSkins As String = row("InSkins").ToString()
                Dim inCTPs As String = row("InCTPs").ToString()
                Dim skinsWon As Decimal = CDec(If(IsDBNull(row("$Skins")), 0, row("$Skins")))
                Dim ctpWon As Decimal = CDec(If(IsDBNull(row("$Closest")), 0, row("$Closest")))

                ' Get skins/ctp paid out this week
                Dim skinsOutAmt As Decimal = CDec(ohelper.SQLiteExecuteScalar($"
                SELECT IFNULL(SUM(ABS(Earned)), 0) FROM Payments
                WHERE League = '{ctx.SafeLeagueName}'
                AND Player = '{safeName}'
                AND Date = {ctx.ActiveDate}
                AND Desc = 'Skin'
                AND Detail LIKE '#%'"))

                Dim ctpOutAmt As Decimal = CDec(ohelper.SQLiteExecuteScalar($"
                SELECT IFNULL(SUM(ABS(Earned)), 0) FROM Payments
                WHERE League = '{ctx.SafeLeagueName}'
                AND Player = '{safeName}'
                AND Date = {ctx.ActiveDate}
                AND Desc = 'CTP'
                AND Detail LIKE '#%'"))

                Dim totalIn As Decimal = If(inSkins = "Y", mSkins, 0) + If(inCTPs = "Y", mCTP, 0)
                Dim totalOut As Decimal = skinsOutAmt + ctpOutAmt

                Dim nr = dt.NewRow()
                nr("Player") = playerName
                nr("Skins In") = If(inSkins = "Y", mSkins.ToString("C2"), "Pay")
                nr("CTP In") = If(inCTPs = "Y", mCTP.ToString("C2"), "Pay")
                nr("Total In") = totalIn.ToString("C2")
                nr("Skins Out") = If(skinsOutAmt > 0, skinsOutAmt.ToString("C2"), "")
                nr("CTP Out") = If(ctpOutAmt > 0, ctpOutAmt.ToString("C2"), "")
                nr("Total Out") = totalOut.ToString("C2")
                nr("InSkins") = inSkins
                nr("InCTPs") = inCTPs
                nr("SkinsWon") = skinsWon
                nr("CTPWon") = ctpWon
                nr("SkinsOutAmt") = skinsOutAmt
                nr("CTPOutAmt") = ctpOutAmt
                nr("IsSub") = isSub
                nr("Gross") = gross
                dt.Rows.Add(nr)

                If inSkins = "Y" Then totSkinsIn += mSkins
                If inCTPs = "Y" Then totCTPIn += mCTP
                totSkinsOut += skinsOutAmt
                totCTPOut += ctpOutAmt
            Next

            ' --- TOTALS ROW ---
            Dim totIn As Decimal = totSkinsIn + totCTPIn
            Dim totOut As Decimal = totSkinsOut + totCTPOut
            Dim totRow = dt.NewRow()
            totRow("Player") = $"TOTALS ({sortedRows.Count})"
            totRow("Skins In") = totSkinsIn.ToString("C2")
            totRow("CTP In") = totCTPIn.ToString("C2")
            totRow("Total In") = totIn.ToString("C2")
            totRow("Skins Out") = totSkinsOut.ToString("C2")
            totRow("CTP Out") = totCTPOut.ToString("C2")
            totRow("Total Out") = totOut.ToString("C2")
            totRow("InSkins") = "Y"
            totRow("InCTPs") = "Y"
            totRow("SkinsWon") = 0
            totRow("CTPWon") = 0
            totRow("SkinsOutAmt") = 0
            totRow("CTPOutAmt") = 0
            totRow("IsSub") = "N"
            totRow("Gross") = 1
            dt.Rows.Add(totRow)

            dgv.DataSource = dt

            ' --- HIDE HELPER COLUMNS ---
            For Each colName As String In {"InSkins", "InCTPs", "SkinsWon", "CTPWon", "SkinsOutAmt", "CTPOutAmt", "IsSub", "Gross"}
                If dgv.Columns.Contains(colName) Then dgv.Columns(colName).Visible = False
            Next

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersHeight = 28
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            dgv.ReadOnly = True
            dgv.RowHeadersVisible = False
            dgv.AllowUserToResizeColumns = False
            dgv.AllowUserToResizeRows = False
            dgv.ScrollBars = ScrollBars.None
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty
            dgv.RowsDefaultCellStyle.BackColor = Color.White
            dgv.DefaultCellStyle.BackColor = Color.White
            For Each col As DataGridViewColumn In dgv.Columns
                If col.Name = "Skins In" OrElse col.Name = "CTP In" OrElse
       col.Name = "Skins Out" OrElse col.Name = "CTP Out" Then
                    col.Width = 100
                End If
                col.SortMode = DataGridViewColumnSortMode.NotSortable
            Next
            ' Fit width to columns
            Dim totalWidth As Integer = 0
            For Each col As DataGridViewColumn In dgv.Columns
                If col.Visible Then totalWidth += col.Width
            Next
            dgv.Width = totalWidth

            ' --- HANDLERS ---
            RemoveHandler dgv.CellPainting, AddressOf dgvWeeklyPayments_CellPainting
            AddHandler dgv.CellPainting, AddressOf dgvWeeklyPayments_CellPainting
            RemoveHandler dgv.CellClick, AddressOf dgvWeeklyPayments_CellClick
            AddHandler dgv.CellClick, AddressOf dgvWeeklyPayments_CellClick
            RemoveHandler dgv.CellMouseEnter, AddressOf dgvWeeklyPayments_CellMouseEnter
            AddHandler dgv.CellMouseEnter, AddressOf dgvWeeklyPayments_CellMouseEnter

            dgv.ClearSelection()

        Catch ex As Exception
            Debug.WriteLine("WeeklyPayments Load Failed: " & ex.Message)
        End Try
    End Sub
    Private Sub dgvWeeklyPayments_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        Dim actionCols As String() = {"Skins In", "CTP In"}
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex = dgv.Rows.Count - 1 Then Return
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        Dim row = dgv.Rows(e.RowIndex)
        Dim isSub As Boolean = row.Cells("IsSub").Value.ToString() = "Y"
        Dim gross As Integer = CInt(If(row.Cells("Gross").Value Is Nothing, 0, row.Cells("Gross").Value))

        ' No show - pink
        If gross = 0 Then
            e.CellStyle.BackColor = Color.LightPink
            Return
        End If

        ' Sub - player cell aqua
        If isSub AndAlso colName = "Player" Then
            e.CellStyle.BackColor = Color.Aqua
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
            e.Handled = True
            Return
        End If

        ' Skins Out / CTP Out - green if has value, plain if empty
        If colName = "Skins Out" OrElse colName = "CTP Out" Then
            Dim cellVal As String = If(e.Value IsNot Nothing, e.Value.ToString(), "")
            If cellVal <> "" Then
                e.CellStyle.BackColor = Color.LightGreen
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
                e.Handled = True
            End If
            Return
        End If

        If Not actionCols.Contains(colName) Then Return

        Dim val As String = If(e.Value IsNot Nothing, e.Value.ToString(), "")
        e.Paint(e.ClipBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)

        Select Case val
            Case "Pay"
                Dim btnRect = Rectangle.Inflate(e.CellBounds, -4, -4)
                ButtonRenderer.DrawButton(e.Graphics, btnRect, VisualStyles.PushButtonState.Normal)
                TextRenderer.DrawText(e.Graphics, val, dgv.Font, btnRect, Color.Black,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Case ""
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
            Case Else
                ' Paid - green cell
                e.CellStyle.BackColor = Color.LightGreen
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
        End Select
        e.Handled = True
    End Sub
    Private Sub dgvWeeklyPayments_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        Dim actionCols As String() = {"Skins In", "CTP In"}
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex = dgv.Rows.Count - 1 Then Return
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        If Not actionCols.Contains(colName) Then Return

        Dim row = dgv.Rows(e.RowIndex)
        Dim cellVal As String = If(row.Cells(colName).Value IsNot Nothing, row.Cells(colName).Value.ToString(), "")
        Dim playerName As String = row.Cells("Player").Value.ToString()
        Dim isSub As Boolean = row.Cells("IsSub").Value.ToString() = "Y"

        Select Case cellVal
            Case "Pay"
                Dim showSkins As Boolean = (colName = "Skins In")
                Dim showCTP As Boolean = (colName = "CTP In")
                Dim mSkins As Decimal = CDec(ctx.rLeagueParmrow("Skins"))
                Dim mCTP As Decimal = CDec(ctx.rLeagueParmrow("Closest"))
                Using frm As New frmPayDues(
                playerName, isSub,
                showSkins, mSkins,
                showCTP, mCTP,
                0, 0, 0, True, True)
                    If frm.ShowDialog() = DialogResult.OK Then
                        ctx.sPlayer = playerName
                        If frm.PaySkins Then MakePmt("Skin")
                        If frm.PayCTP Then MakePmt("CTP")
                        RefreshScoresGrid()
                        LoadWeeklyPaymentsTab(ctx.tabControl)
                    End If
                End Using

            Case Else
                ' Undo - only for In columns
                If cellVal <> "" Then
                    Dim desc As String = If(colName = "Skins In", "Skin", "CTP")
                    UndoPayment(playerName, desc, "= 'Payment'")
                    RefreshScoresGrid()
                    LoadWeeklyPaymentsTab(ctx.tabControl)
                End If
        End Select
    End Sub
    Private Sub dgvWeeklyPayments_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex = dgv.Rows.Count - 1 Then Return
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        If colName <> "Skins In" AndAlso colName <> "CTP In" Then Return
        Dim cellVal As String = If(dgv.Rows(e.RowIndex).Cells(colName).Value IsNot Nothing,
                               dgv.Rows(e.RowIndex).Cells(colName).Value.ToString(), "")
        If cellVal <> "Pay" AndAlso cellVal <> "" Then
            dgv.Rows(e.RowIndex).Cells(colName).ToolTipText = "Click to undo"
        Else
            dgv.Rows(e.RowIndex).Cells(colName).ToolTipText = ""
        End If
    End Sub
    Private Function GetPaymentSum(safeName As String, season As String, desc As String, detailFilter As String) As Decimal
        Dim sql As String = $"
        SELECT IFNULL(SUM(ABS(Earned)), 0) FROM Payments
        WHERE League = '{ctx.SafeLeagueName}'
        AND Player = '{safeName}'
        AND Desc = '{desc}'
        AND Detail {detailFilter}
        AND SUBSTR(Date, 1, 4) = '{season}'"
        Return CDec(ohelper.SQLiteExecuteScalar(sql))
    End Function
    Dim mEOYWeekAmt As Decimal = CDec(ctx.rLeagueParmrow("EOY Skins")) / 2
    Friend Sub LoadLeagueDuesTab(tc As TabControl)
        ohelper.wk = ""
        Try
            Dim tp = tc.TabPages("LeagueDues")
            Dim dgv = DirectCast(tp.Controls("dgLeagueDues"), DataGridView)

            Dim season As String = ctx.rLeagueParmrow("Season").ToString()
            Dim mDues As Decimal = CDec(ctx.rLeagueParmrow("Cost"))
            Dim mEOY As Decimal = CDec(ctx.rLeagueParmrow("EOY Skins"))
            Dim mEOYWeekAmt As Decimal = mEOY / 2

            ' --- BUILD DATA TABLE ---
            Dim dt As New DataTable
            dt.Columns.Add("Player", GetType(String))
            dt.Columns.Add("Dues In", GetType(String))
            dt.Columns.Add("Dues Owed", GetType(String))
            dt.Columns.Add("EOY In", GetType(String))
            dt.Columns.Add("Total In", GetType(String))
            dt.Columns.Add("1st Half Out", GetType(String))
            dt.Columns.Add("2nd Half Out", GetType(String))
            dt.Columns.Add("Club Champ Out", GetType(String))
            dt.Columns.Add("Total Out", GetType(String))
            dt.Columns.Add("Net", GetType(String))
            ' Hidden helper columns
            dt.Columns.Add("IsSub", GetType(String))
            dt.Columns.Add("LDBalance", GetType(Decimal))
            dt.Columns.Add("EOYBalance", GetType(Decimal))
            dt.Columns.Add("InEOY1", GetType(String))
            dt.Columns.Add("InEOY2", GetType(String))

            ' --- GET DISTINCT PLAYERS FOR SEASON ---
            Dim dtPlayers = ohelper.SQLiteGetDataTable($"
            SELECT Player FROM Teams
            WHERE League = '{ctx.SafeLeagueName}'
            AND Year = {season}
            ORDER BY Player")
            ' Sort by last name
            Dim playerList As New List(Of String)
            For Each pRow As DataRow In dtPlayers.Rows
                playerList.Add(pRow("Player").ToString())
            Next
            playerList.Sort(Function(a, b)
                                Dim nameA = a.Trim().Split(" "c)
                                Dim nameB = b.Trim().Split(" "c)
                                Dim keyA = If(nameA.Length > 1, nameA(nameA.Length - 1) & nameA(0), nameA(0))
                                Dim keyB = If(nameB.Length > 1, nameB(nameB.Length - 1) & nameB(0), nameB(0))
                                Return String.Compare(keyA, keyB)
                            End Function)

            ' Totals
            Dim totDuesIn As Decimal = 0
            Dim totEOYIn As Decimal = 0
            Dim tot1stOut As Decimal = 0
            Dim tot2ndOut As Decimal = 0
            Dim totCCOut As Decimal = 0

            For Each playerName As String In playerList
                Dim safeName = playerName.Replace("'", "''")

                ' Check if regular
                Dim isRegular As Boolean = CInt(ohelper.SQLiteExecuteScalar($"
                SELECT COUNT(*) FROM Teams
                WHERE League = '{ctx.SafeLeagueName}'
                AND Player = '{safeName}'
                AND Year = {season}")) > 0

                ' Collected
                Dim duesIn = If(isRegular, GetPaymentSum(safeName, season, "League Dues", "= 'Payment'"), 0)
                Dim eoyIn = If(isRegular, GetPaymentSum(safeName, season, "EOY Skins", "= 'Payment'"), 0)

                ' Paid Out
                Dim firstHalfOut = GetPaymentSum(safeName, season, "Reg Season Champ-1st half", "LIKE 'Earned%'")
                Dim secondHalfOut = GetPaymentSum(safeName, season, "Reg Season Champ-2nd half", "LIKE 'Earned%'")
                Dim ccOut = GetPaymentSum(safeName, season, "Club Champion-1st", "LIKE 'Earned%'") +
                        GetPaymentSum(safeName, season, "Club Champion-2nd", "LIKE 'Earned%'")

                ' Balances
                Dim ldBalance As Decimal = If(isRegular, mDues - duesIn, 0)
                Dim eoyBalance As Decimal = If(isRegular, mEOY - eoyIn, 0)

                ' EOY week flags
                Dim inEOY1 As String = If(CInt(ohelper.SQLiteExecuteScalar($"
                SELECT COUNT(*) FROM Payments
                WHERE League = '{ctx.SafeLeagueName}'
                AND Player = '{safeName}'
                AND Desc = 'EOY Skins'
                AND Detail = 'Payment'
                AND (Comment = '(1st Week)' OR Comment = '')
                AND SUBSTR(Date, 1, 4) = '{season}'")) > 0, "Y", "N")

                Dim inEOY2 As String = If(CInt(ohelper.SQLiteExecuteScalar($"
                SELECT COUNT(*) FROM Payments
                WHERE League = '{ctx.SafeLeagueName}'
                AND Player = '{safeName}'
                AND Desc = 'EOY Skins'
                AND Detail = 'Payment'
                AND (Comment = '(2nd Week)' OR Comment = '')
                AND SUBSTR(Date, 1, 4) = '{season}'")) > 0, "Y", "N")

                Dim totalIn As Decimal = duesIn + eoyIn
                Dim totalOut As Decimal = firstHalfOut + secondHalfOut + ccOut
                Dim net As Decimal = totalIn - totalOut

                Dim nr = dt.NewRow()
                nr("Player") = playerName
                nr("Dues In") = If(isRegular, If(ldBalance <= 0, duesIn.ToString("C2"), $"Pay ${ldBalance:F2} left"), "")
                nr("Dues Owed") = If(ldBalance > 0, ldBalance.ToString("C2"), "")
                nr("EOY In") = If(isRegular, If(eoyBalance <= 0, eoyIn.ToString("C2"), $"Pay ${eoyBalance:F2} left"), "")
                nr("Total In") = totalIn.ToString("C2")
                nr("1st Half Out") = If(firstHalfOut > 0, firstHalfOut.ToString("C2"), "")
                nr("2nd Half Out") = If(secondHalfOut > 0, secondHalfOut.ToString("C2"), "")
                nr("Club Champ Out") = If(ccOut > 0, ccOut.ToString("C2"), "")
                nr("Total Out") = totalOut.ToString("C2")
                nr("Net") = net.ToString("C2")
                nr("IsSub") = If(isRegular, "N", "Y")
                nr("LDBalance") = ldBalance
                nr("EOYBalance") = eoyBalance
                nr("InEOY1") = inEOY1
                nr("InEOY2") = inEOY2
                dt.Rows.Add(nr)

                totDuesIn += duesIn
                totEOYIn += eoyIn
                tot1stOut += firstHalfOut
                tot2ndOut += secondHalfOut
                totCCOut += ccOut
            Next

            ' --- TOTALS ROW ---
            Dim totIn As Decimal = totDuesIn + totEOYIn
            Dim totOut As Decimal = tot1stOut + tot2ndOut + totCCOut
            Dim totRow = dt.NewRow()
            totRow("Player") = $"TOTALS ({playerList.Count})"
            totRow("Dues In") = totDuesIn.ToString("C2")
            totRow("Dues Owed") = ""
            totRow("EOY In") = totEOYIn.ToString("C2")
            totRow("Total In") = totIn.ToString("C2")
            totRow("1st Half Out") = tot1stOut.ToString("C2")
            totRow("2nd Half Out") = tot2ndOut.ToString("C2")
            totRow("Club Champ Out") = totCCOut.ToString("C2")
            totRow("Total Out") = totOut.ToString("C2")
            totRow("Net") = (totIn - totOut).ToString("C2")
            totRow("IsSub") = "N"
            totRow("LDBalance") = 0
            totRow("EOYBalance") = 0
            totRow("InEOY1") = "Y"
            totRow("InEOY2") = "Y"
            dt.Rows.Add(totRow)

            dgv.DataSource = dt

            ' --- HIDE HELPER COLUMNS ---
            For Each colName As String In {"IsSub", "LDBalance", "EOYBalance", "InEOY1", "InEOY2"}
                If dgv.Columns.Contains(colName) Then dgv.Columns(colName).Visible = False
            Next

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersHeight = 28
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            dgv.ReadOnly = True
            dgv.RowHeadersVisible = False
            dgv.AllowUserToResizeColumns = False
            dgv.AllowUserToResizeRows = False
            dgv.ScrollBars = ScrollBars.Horizontal
            dgv.Dock = DockStyle.Fill
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty
            dgv.RowsDefaultCellStyle.BackColor = Color.White
            dgv.DefaultCellStyle.BackColor = Color.White
            For Each col As DataGridViewColumn In dgv.Columns
                col.SortMode = DataGridViewColumnSortMode.NotSortable
            Next

            Dim today As String = DateTime.Now.ToString("yyyyMMdd")
            dgv.Columns("1st Half Out").Visible = (today >= ctx.sFirstHalfEndDate)
            dgv.Columns("2nd Half Out").Visible = (today >= ctx.sSecondHalfEndDate)
            dgv.Columns("Club Champ Out").Visible = (today >= ctx.sPSDate)

            ' Fit width to columns
            Dim totalWidth As Integer = 0
            For Each col As DataGridViewColumn In dgv.Columns
                If col.Visible Then totalWidth += col.Width
            Next
            dgv.Width = totalWidth

            ' --- HANDLERS ---
            RemoveHandler dgv.CellFormatting, AddressOf dgvLeagueDues_CellFormatting
            AddHandler dgv.CellFormatting, AddressOf dgvLeagueDues_CellFormatting
            RemoveHandler dgv.CellPainting, AddressOf dgvLeagueDues_CellPainting
            AddHandler dgv.CellPainting, AddressOf dgvLeagueDues_CellPainting
            RemoveHandler dgv.CellClick, AddressOf dgvLeagueDues_CellClick
            AddHandler dgv.CellClick, AddressOf dgvLeagueDues_CellClick
            RemoveHandler dgv.CellMouseEnter, AddressOf dgvLeagueDues_CellMouseEnter
            AddHandler dgv.CellMouseEnter, AddressOf dgvLeagueDues_CellMouseEnter

            dgv.ClearSelection()

        Catch ex As Exception
            Debug.WriteLine("LeagueDues Load Failed: " & ex.Message)
        End Try
    End Sub
    Private Sub dgvLeagueDues_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        Dim inCols As String() = {"Dues In", "EOY In"}
        Dim outCols As String() = {"1st Half Out", "2nd Half Out", "Club Champ Out"}
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex = dgv.Rows.Count - 1 Then Return
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        Dim row = dgv.Rows(e.RowIndex)
        Dim isSub As Boolean = row.Cells("IsSub").Value.ToString() = "Y"

        ' Sub - player cell aqua
        If isSub AndAlso colName = "Player" Then
            e.CellStyle.BackColor = Color.Aqua
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
            e.Handled = True
            Return
        End If

        ' Out cols - display only, green if value
        If outCols.Contains(colName) Then
            Dim cellVal As String = If(e.Value IsNot Nothing, e.Value.ToString(), "")
            If cellVal <> "" Then
                e.CellStyle.BackColor = Color.LightGreen
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
                e.Handled = True
            End If
            Return
        End If

        If Not inCols.Contains(colName) Then Return

        Dim val As String = If(e.Value IsNot Nothing, e.Value.ToString(), "")
        e.Paint(e.ClipBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)

        Select Case True
            Case val.StartsWith("Pay")
                ' Button
                Dim btnRect = Rectangle.Inflate(e.CellBounds, -4, -4)
                ButtonRenderer.DrawButton(e.Graphics, btnRect, VisualStyles.PushButtonState.Normal)
                TextRenderer.DrawText(e.Graphics, val, dgv.Font, btnRect, Color.Black,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Case val = ""
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
            Case Else
                ' Paid - green
                e.CellStyle.BackColor = Color.LightGreen
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All)
        End Select
        e.Handled = True
    End Sub
    Private Sub dgvLeagueDues_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        Dim inCols As String() = {"Dues In", "EOY In"}
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex = dgv.Rows.Count - 1 Then Return
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        If Not inCols.Contains(colName) Then Return

        Dim row = dgv.Rows(e.RowIndex)
        Dim cellVal As String = If(row.Cells(colName).Value IsNot Nothing, row.Cells(colName).Value.ToString(), "")
        If cellVal = "" Then Return

        Dim playerName As String = row.Cells("Player").Value.ToString()
        Dim ldBalance As Decimal = CDec(row.Cells("LDBalance").Value)
        Dim eoyBalance As Decimal = CDec(row.Cells("EOYBalance").Value)
        Dim inEOY1 As Boolean = (row.Cells("InEOY1").Value.ToString() = "Y")
        Dim inEOY2 As Boolean = (row.Cells("InEOY2").Value.ToString() = "Y")
        Dim mEOYWeekAmt As Decimal = CDec(ctx.rLeagueParmrow("EOY Skins")) / 2

        If cellVal.StartsWith("Pay") Then
            ' Pay
            Using frm As New frmPayDues(
            playerName, False,
            False, 0,
            False, 0,
            ldBalance,
            eoyBalance,
            mEOYWeekAmt,
            inEOY1, inEOY2)
                If frm.ShowDialog() = DialogResult.OK Then
                    ctx.sPlayer = playerName
                    If frm.PayLeagueDues > 0 Then MakePmtAmount("League Dues", frm.PayLeagueDues)
                    If frm.PayEOY > 0 Then MakePmtAmount("EOY Skins", frm.PayEOY, frm.EOYWeekComment)
                    LoadLeagueDuesTab(ctx.tabControl)
                End If
            End Using
        Else
            ' Undo
            Dim desc As String = If(colName = "Dues In", "League Dues", "EOY Skins")
            UndoPayment(playerName, desc, "= 'Payment'", useDate:=False)
            LoadLeagueDuesTab(ctx.tabControl)
        End If
    End Sub
    Private Sub dgvLeagueDues_CellMouseEnter(sender As Object, e As DataGridViewCellEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex = dgv.Rows.Count - 1 Then Return
        Dim inCols As String() = {"Dues In", "EOY In"}
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        If Not inCols.Contains(colName) Then Return
        Dim cellVal As String = If(dgv.Rows(e.RowIndex).Cells(colName).Value IsNot Nothing,
                               dgv.Rows(e.RowIndex).Cells(colName).Value.ToString(), "")
        If cellVal <> "" AndAlso Not cellVal.StartsWith("Pay") Then
            dgv.Rows(e.RowIndex).Cells(colName).ToolTipText = "Click to undo"
        Else
            dgv.Rows(e.RowIndex).Cells(colName).ToolTipText = ""
        End If
    End Sub
    Private Sub dgvLeagueDues_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Dim dgv = DirectCast(sender, DataGridView)
        If e.RowIndex < 0 Then Return
        Dim colName As String = dgv.Columns(e.ColumnIndex).Name
        Dim isTotalsRow As Boolean = (e.RowIndex = dgv.Rows.Count - 1)

        ' Totals row - bold no color
        If isTotalsRow Then
            e.CellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            Return
        End If

        ' Net column coloring
        If colName = "Net" Then
            Dim netVal As Decimal = 0
            Decimal.TryParse(dgv.Rows(e.RowIndex).Cells("Net").Value.ToString().Replace("$", "").Replace(",", ""), netVal)
            If netVal > 0 Then
                e.CellStyle.BackColor = Color.LightCoral
            ElseIf netVal < 0 Then
                e.CellStyle.BackColor = Color.Gold
            Else
                e.CellStyle.BackColor = Color.LightGreen
            End If
        ElseIf colName = "Dues Owed" Then
            Dim val As String = If(dgv.Rows(e.RowIndex).Cells("Dues Owed").Value IsNot Nothing,
                                      dgv.Rows(e.RowIndex).Cells("Dues Owed").Value.ToString(), "")
            If val <> "" Then
                e.CellStyle.BackColor = Color.LightCoral
                e.CellStyle.ForeColor = Color.DarkRed
            End If
        End If

        ' Sub - player cell aqua
        Dim isSub As Boolean = dgv.Rows(e.RowIndex).Cells("IsSub").Value.ToString() = "Y"
        If isSub AndAlso colName = "Player" Then
            e.CellStyle.BackColor = Color.Aqua
        End If
    End Sub
    Private Sub UndoPayment(playerName As String, desc As String, detailFilter As String, Optional useDate As Boolean = True)
        Try
            Dim safeName = playerName.Replace("'", "''")
            Dim dateFilter As String = If(useDate,
            $"AND Date = {ctx.ActiveDate}",
            $"AND SUBSTR(Date, 1, 4) = '{ctx.rLeagueParmrow("Season")}'")

            Dim deleteSql As String = $"
            DELETE FROM Payments
            WHERE rowid = (
                SELECT rowid FROM Payments
                WHERE League = '{ctx.SafeLeagueName}'
                AND Player = '{safeName}'
                {dateFilter}
                AND Desc = '{desc}'
                AND Detail {detailFilter}
                ORDER BY DatePaid DESC LIMIT 1)"
            ohelper.SQLiteExecuteScalar(deleteSql)
            LOGIT($"UndoPayment: {playerName} {desc} undone")

            If desc = "Skin" Then
                EditScoresEngine.RecalculateSkinCarryovers()
            ElseIf desc = "CTP" Then
                EditScoresEngine.RecalculateCTPCarryovers()
            End If
        Catch ex As Exception
            LOGIT($"UndoPayment Error: {ex.Message}")
        End Try
    End Sub
    Public Sub RecordPayout(playerName As String, desc As String, detail As String, amount As Decimal)
        Try
            Dim sql As String = "
            INSERT INTO Payments
            (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
            VALUES
            (@League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, @PayMethod)"
            Dim p As New Dictionary(Of String, Object) From {
            {"@League", ctx.sLeagueName},
            {"@Player", playerName},
            {"@Date", ctx.ActiveDate},
            {"@Desc", desc},
            {"@Detail", detail},
            {"@Earned", amount},
            {"@DatePaid", DateTime.Now.ToString(DATE_FORMAT)},
            {"@Comment", ""},
            {"@PayMethod", ""}
        }
            ohelper.SqliteExec(sql, p)
            LOGIT($"RecordPayout: {playerName} received {amount:C2} for {desc} {detail}")
        Catch ex As Exception
            LOGIT($"RecordPayout Error: {ex.Message}")
        End Try
    End Sub
    Friend Sub LoadPaymentsTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Payments")

            ' Store checkbox state BEFORE clearing controls
            Dim showRegularsOnly As Boolean = True
            For Each ctrl As Control In tp.Controls
                If ctrl.Name = "chkRegulars" Then
                    showRegularsOnly = DirectCast(ctrl, CheckBox).Checked
                    Exit For
                End If
            Next

            tp.Controls.Clear()
            tp.AutoScroll = True

            Dim season As Integer = CInt(ctx.rLeagueParmrow("Season"))
            Dim today As String = DateTime.Now.ToString("yyyyMMdd")
            Dim eoyWeekAmt As Decimal = CDec(ctx.rLeagueParmrow("SkinsPS")) + CDec(ctx.rLeagueParmrow("ClosestPS"))
            Dim eoyTotal As Decimal = eoyWeekAmt * 2

            ' --- QUERY REGULARS ---
            Dim sql As String = $"
        SELECT 
            P.LastName || ', ' || P.FirstName AS Player,
            T.Player AS PlayerKey,
            T.Grade,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'League Dues' AND Pay.Detail = 'Payment' THEN Pay.Earned ELSE 0 END), 0) AS LDPaid,
            IFNULL(MAX(LP.Cost), 0) - IFNULL(SUM(CASE WHEN Pay.Desc = 'League Dues' AND Pay.Detail = 'Payment' THEN Pay.Earned ELSE 0 END), 0) AS LDBalance,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'Skin' AND Pay.Detail = 'Payment' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS SkinsPaid,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'CTP' AND Pay.Detail = 'Payment' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS CTPPaid,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'Skin' AND Pay.Detail LIKE '#%' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS SkinsWonWeek,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'Skin' AND Pay.Detail LIKE '#%' THEN Pay.Earned ELSE 0 END), 0) AS SkinsWonSeason,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'CTP' AND Pay.Detail LIKE '#%' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS CTPWonWeek,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'CTP' AND Pay.Detail LIKE '#%' THEN Pay.Earned ELSE 0 END), 0) AS CTPWonSeason,
            IFNULL(SUM(CASE WHEN Pay.Desc = 'EOY Skins' AND Pay.Detail = 'Payment' THEN Pay.Earned ELSE 0 END), 0) AS EOYPaid,
            IFNULL(SUM(CASE WHEN Pay.Desc LIKE 'Reg Season Champ%' THEN ABS(Pay.Earned) ELSE 0 END), 0) AS SeasonChampEarned,
            IFNULL(SUM(CASE WHEN Pay.Desc LIKE 'Club Champion%' THEN ABS(Pay.Earned) ELSE 0 END), 0) AS ClubChampEarned,
            MAX(CASE WHEN Pay.Detail = 'Payment' THEN Pay.DatePaid ELSE NULL END) AS LastPaid
        FROM Teams T
        JOIN Players P ON P.Player = T.Player
        LEFT JOIN Payments Pay ON Pay.Player = T.Player 
            AND Pay.League = T.League 
            AND SUBSTR(Pay.Date, 1, 4) = '{season}'
            AND Pay.Date <= '{ctx.ActiveDate}'
        LEFT JOIN LeagueParms LP ON LP.Name = T.League AND LP.Season = T.Year
        WHERE T.League = @League
          AND T.Year = {season}
        GROUP BY T.Player
        ORDER BY P.LastName, P.FirstName"

            Dim dt As New DataTable
            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using

            ' --- QUERY SUBS (only if not regulars only) ---
            If Not showRegularsOnly Then
                Dim subSql As String = $"
SELECT 
    P.LastName || ', ' || P.FirstName AS Player,
    Su.Player AS PlayerKey,
    'Sub' AS Grade,
    0 AS LDPaid, 0 AS LDBalance,
    IFNULL(SUM(CASE WHEN Pay.Desc = 'Skin' AND Pay.Detail = 'Payment' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS SkinsPaid,
    IFNULL(SUM(CASE WHEN Pay.Desc = 'CTP' AND Pay.Detail = 'Payment' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS CTPPaid,
    IFNULL(SUM(CASE WHEN Pay.Desc = 'Skin' AND Pay.Detail LIKE '#%' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS SkinsWonWeek,
    IFNULL(SUM(CASE WHEN Pay.Desc = 'Skin' AND Pay.Detail LIKE '#%' THEN Pay.Earned ELSE 0 END), 0) AS SkinsWonSeason,
    IFNULL(SUM(CASE WHEN Pay.Desc = 'CTP' AND Pay.Detail LIKE '#%' AND Pay.Date = '{ctx.ActiveDate}' THEN Pay.Earned ELSE 0 END), 0) AS CTPWonWeek,
    IFNULL(SUM(CASE WHEN Pay.Desc = 'CTP' AND Pay.Detail LIKE '#%' THEN Pay.Earned ELSE 0 END), 0) AS CTPWonSeason,
    0 AS EOYPaid,
    0 AS SeasonChampEarned, 0 AS ClubChampEarned,
    MAX(CASE WHEN Pay.Detail = 'Payment' THEN Pay.DatePaid ELSE NULL END) AS LastPaid
FROM Subs Su
JOIN Players P ON P.Player = Su.Player
LEFT JOIN Payments Pay ON Pay.Player = Su.Player
    AND Pay.League = Su.League
    AND SUBSTR(Pay.Date, 1, 4) = '{season}'
    AND Pay.Date <= '{ctx.ActiveDate}'
WHERE Su.League = @League
  AND Su.Date = '{ctx.ActiveDate}'
  AND Su.Player NOT IN (
      SELECT Player FROM Teams 
      WHERE League = @League AND Year = {season})
GROUP BY Su.Player
ORDER BY P.LastName, P.FirstName"

                Dim dtSubs As New DataTable
                Using cmd As New SQLiteCommand(subSql, ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Using da As New SQLiteDataAdapter(cmd)
                        da.Fill(dtSubs)
                    End Using
                End Using
                For Each row As DataRow In dtSubs.Rows
                    dt.ImportRow(row)
                Next
            End If

            ' --- LABEL ---
            Dim lbl As New Label()
            lbl.Text = $"Payments - {season} Season"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(10, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            ' --- REGULARS ONLY CHECKBOX ---
            Dim chkNew As New CheckBox()
            chkNew.Name = "chkRegulars"
            chkNew.Text = "Regulars Only"
            chkNew.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            chkNew.Location = New Point(200, 12)
            chkNew.AutoSize = True
            chkNew.Checked = showRegularsOnly
            tp.Controls.Add(chkNew)
            AddHandler chkNew.CheckedChanged, Sub(s, ev) LoadPaymentsTab(tc)

            ' --- PAY BUTTON ---
            Dim btnPay As New Button()
            btnPay.Name = "btnPay"
            btnPay.Text = "Pay"
            btnPay.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            btnPay.Size = New Size(60, 25)
            btnPay.Location = New Point(320, 8)
            btnPay.BackColor = Color.FromArgb(0, 120, 215)
            btnPay.ForeColor = Color.White
            btnPay.FlatStyle = FlatStyle.Flat
            btnPay.FlatAppearance.BorderSize = 0
            btnPay.Visible = False
            tp.Controls.Add(btnPay)

            ' --- REFUND BUTTON ---
            Dim btnRefund As New Button()
            btnRefund.Name = "btnRefund"
            btnRefund.Text = "Refund"
            btnRefund.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            btnRefund.Size = New Size(70, 25)
            btnRefund.Location = New Point(390, 8)
            btnRefund.BackColor = Color.FromArgb(200, 80, 80)
            btnRefund.ForeColor = Color.White
            btnRefund.FlatStyle = FlatStyle.Flat
            btnRefund.FlatAppearance.BorderSize = 0
            btnRefund.Visible = False
            tp.Controls.Add(btnRefund)

            ' --- GRID ---
            Dim dgv As New DataGridView()
            dgv.Name = "dgPayments"
            dgv.Location = New Point(10, 40)
            tp.Controls.Add(dgv)
            dgv.DataSource = dt

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersHeight = 42
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.ReadOnly = True
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

            ' --- COLUMN SETTINGS ---
            Dim colSettings As New Dictionary(Of String, (Width As Integer, Header As String, Format As String, Align As DataGridViewContentAlignment)) From {
            {"Player", (160, "Player", "", DataGridViewContentAlignment.MiddleLeft)},
            {"LDPaid", (60, "LD Paid", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"LDBalance", (60, "LD Bal", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"SkinsPaid", (60, "Skins Paid", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"SkinsWonWeek", (65, "Skins Won Wk", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"SkinsWonSeason", (70, "Skins Won Sea", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"CTPPaid", (60, "CTP Paid", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"CTPWonWeek", (65, "CTP Won Wk", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"CTPWonSeason", (70, "CTP Won Sea", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"EOYPaid", (60, "EOY Paid", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"SeasonChampEarned", (70, "Season Champ", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"ClubChampEarned", (70, "Club Champ", "C2", DataGridViewContentAlignment.MiddleRight)},
            {"LastPaid", (120, "Last Paid", "", DataGridViewContentAlignment.MiddleCenter)}
        }

            ' --- ADD TOTALS ROW ---
            Dim totalRow As DataRow = dt.NewRow()
            totalRow("Player") = "TOTALS"
            totalRow("LDPaid") = 0D : totalRow("LDBalance") = 0D
            totalRow("SkinsPaid") = 0D : totalRow("SkinsWonWeek") = 0D : totalRow("SkinsWonSeason") = 0D
            totalRow("CTPPaid") = 0D : totalRow("CTPWonWeek") = 0D : totalRow("CTPWonSeason") = 0D
            totalRow("EOYPaid") = 0D : totalRow("SeasonChampEarned") = 0D : totalRow("ClubChampEarned") = 0D

            For Each row As DataRow In dt.Rows
                totalRow("LDPaid") = CDec(totalRow("LDPaid")) + CDec(row("LDPaid"))
                totalRow("LDBalance") = CDec(totalRow("LDBalance")) + CDec(row("LDBalance"))
                totalRow("SkinsPaid") = CDec(totalRow("SkinsPaid")) + CDec(row("SkinsPaid"))
                totalRow("SkinsWonWeek") = CDec(totalRow("SkinsWonWeek")) + CDec(row("SkinsWonWeek"))
                totalRow("SkinsWonSeason") = CDec(totalRow("SkinsWonSeason")) + CDec(row("SkinsWonSeason"))
                totalRow("CTPPaid") = CDec(totalRow("CTPPaid")) + CDec(row("CTPPaid"))
                totalRow("CTPWonWeek") = CDec(totalRow("CTPWonWeek")) + CDec(row("CTPWonWeek"))
                totalRow("CTPWonSeason") = CDec(totalRow("CTPWonSeason")) + CDec(row("CTPWonSeason"))
                totalRow("EOYPaid") = CDec(totalRow("EOYPaid")) + CDec(row("EOYPaid"))
                totalRow("SeasonChampEarned") = CDec(totalRow("SeasonChampEarned")) + CDec(row("SeasonChampEarned"))
                totalRow("ClubChampEarned") = CDec(totalRow("ClubChampEarned")) + CDec(row("ClubChampEarned"))
            Next
            dt.Rows.Add(totalRow)

            ' --- APPLY COLUMN SETTINGS ---
            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    Dim settings = colSettings(col.Name)
                    col.Visible = True
                    col.Width = settings.Width
                    col.HeaderText = settings.Header
                    col.DefaultCellStyle.Alignment = settings.Align
                    col.HeaderCell.Style.Alignment = settings.Align
                    If settings.Format <> "" Then col.DefaultCellStyle.Format = settings.Format
                Else
                    col.Visible = False
                End If
            Next

            ' Hide Season/Club Champ unless postseason
            Dim postSeasonDt As String = CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
            If ctx.ActiveDate < postSeasonDt Then
                If dgv.Columns.Contains("SeasonChampEarned") Then dgv.Columns("SeasonChampEarned").Visible = False
                If dgv.Columns.Contains("ClubChampEarned") Then dgv.Columns("ClubChampEarned").Visible = False
            End If

            ' --- SIZE TO CONTENT ---
            Dim availableHeight As Integer = tc.Height - 150
            Dim h As Integer = (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 2
            If h > availableHeight Then
                dgv.Size = New Size(dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3, availableHeight)
                dgv.ScrollBars = ScrollBars.Vertical
            Else
                dgv.Size = New Size(dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3, h)
                dgv.ScrollBars = ScrollBars.None
            End If

            ' --- COLOR CODE ROWS ---
            For i As Integer = 0 To dgv.Rows.Count - 1
                If dgv.Rows(i).IsNewRow Then Continue For
                If dgv.Rows(i).Cells("Player").Value?.ToString() = "TOTALS" Then
                    dgv.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200)
                    dgv.Rows(i).DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                    Continue For
                End If
                Dim ldBal = dgv.Rows(i).Cells("LDBalance").Value
                If IsNumeric(ldBal) AndAlso CDec(ldBal) > 0 Then
                    dgv.Rows(i).Cells("LDBalance").Style.ForeColor = Color.DarkRed
                    dgv.Rows(i).Cells("LDBalance").Style.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                End If
                Dim skinsWon = dgv.Rows(i).Cells("SkinsWonWeek").Value
                If IsNumeric(skinsWon) AndAlso CDec(skinsWon) > 0 Then
                    dgv.Rows(i).Cells("SkinsWonWeek").Style.ForeColor = Color.DarkGreen
                    dgv.Rows(i).Cells("SkinsWonWeek").Style.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                End If
                Dim ctpWon = dgv.Rows(i).Cells("CTPWonWeek").Value
                If IsNumeric(ctpWon) AndAlso CDec(ctpWon) > 0 Then
                    dgv.Rows(i).Cells("CTPWonWeek").Style.ForeColor = Color.DarkGreen
                    dgv.Rows(i).Cells("CTPWonWeek").Style.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                End If
            Next

            ' --- HELPER: GET PLAYER KEY FROM CURRENT ROW ---
            Dim getPlayerKey As Func(Of String) = Function()
                                                      If dgv.CurrentRow Is Nothing Then Return ""
                                                      Return dgv.CurrentRow.Cells("PlayerKey").Value?.ToString()
                                                  End Function

            ' --- HELPER: CHECK PAYMENT ---
            Dim hasPayment As Func(Of String, String, Boolean) = Function(playerKey As String, desc As String)
                                                                     Using cmd As New SQLiteCommand("
                SELECT COUNT(*) FROM Payments 
                WHERE League=@League AND Player=@Player 
                AND Date=@Date AND Desc=@Desc AND Detail='Payment'", ctx.Conn)
                                                                         cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                                                         cmd.Parameters.AddWithValue("@Player", playerKey)
                                                                         cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                                                                         cmd.Parameters.AddWithValue("@Desc", desc)
                                                                         If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                                         Return CInt(cmd.ExecuteScalar()) > 0
                                                                     End Using
                                                                 End Function

            ' --- HELPER: DELETE PAYMENT ---
            Dim deletePayment As Action(Of String, String) = Sub(playerKey As String, desc As String)
                                                                 Using cmd As New SQLiteCommand("
                DELETE FROM Payments 
                WHERE League=@League AND Player=@Player 
                AND Date=@Date AND Desc=@Desc AND Detail='Payment'", ctx.Conn)
                                                                     cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                                                     cmd.Parameters.AddWithValue("@Player", playerKey)
                                                                     cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                                                                     cmd.Parameters.AddWithValue("@Desc", desc)
                                                                     If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                                     cmd.ExecuteNonQuery()
                                                                     LOGIT($"Refunded {desc} payment for {playerKey} on {ctx.ActiveDate}")
                                                                 End Using
                                                             End Sub

            ' --- SELECTION CHANGED ---
            AddHandler dgv.SelectionChanged, Sub(s, ev)
                                                 If dgv.CurrentRow Is Nothing Then
                                                     btnPay.Visible = False
                                                     btnRefund.Visible = False
                                                     Exit Sub
                                                 End If

                                                 Dim playerKey As String = dgv.CurrentRow.Cells("PlayerKey").Value?.ToString()
                                                 Dim selectedPlayer As String = dgv.CurrentRow.Cells("Player").Value?.ToString()

                                                 If selectedPlayer = "TOTALS" OrElse String.IsNullOrEmpty(playerKey) Then
                                                     btnPay.Visible = False
                                                     btnRefund.Visible = False
                                                     Exit Sub
                                                 End If

                                                 Dim isSub As Boolean = dgv.CurrentRow.Cells("Grade").Value?.ToString() = "Sub"
                                                 Dim ldBal As Decimal = CDec(dgv.CurrentRow.Cells("LDBalance").Value)
                                                 Dim isFutureDate As Boolean = ctx.ActiveDate > today
                                                 Dim eoyPaid As Decimal = CDec(dgv.CurrentRow.Cells("EOYPaid").Value)
                                                 Dim eoyBal As Decimal = eoyTotal - eoyPaid

                                                 Dim skinsPaid As Boolean = True
                                                 Dim ctpPaid As Boolean = True
                                                 Dim hasSkinsPayment As Boolean = False
                                                 Dim hasCTPPayment As Boolean = False

                                                 If Not isFutureDate Then
                                                     Dim mSkins As Decimal = CDec(ctx.rLeagueParmrow("Skins"))
                                                     Dim mCTP As Decimal = CDec(ctx.rLeagueParmrow("Closest"))
                                                     hasSkinsPayment = hasPayment(playerKey, "Skin")
                                                     hasCTPPayment = hasPayment(playerKey, "CTP")
                                                     skinsPaid = hasSkinsPayment AndAlso CDec(dgv.CurrentRow.Cells("SkinsPaid").Value) >= mSkins
                                                     ctpPaid = hasCTPPayment AndAlso CDec(dgv.CurrentRow.Cells("CTPPaid").Value) >= mCTP
                                                 End If

                                                 Dim somethingToPay As Boolean = False
                                                 If Not isSub AndAlso ldBal > 0 Then somethingToPay = True
                                                 If Not skinsPaid Then somethingToPay = True
                                                 If Not ctpPaid Then somethingToPay = True
                                                 If Not isSub AndAlso eoyBal > 0 Then somethingToPay = True
                                                 btnPay.Visible = somethingToPay
                                                 btnRefund.Visible = (Not isFutureDate) AndAlso (hasSkinsPayment OrElse hasCTPPayment)
                                             End Sub

            ' --- PAY BUTTON CLICK ---
            AddHandler btnPay.Click, Sub(s, ev)
                                         Dim playerKey As String = getPlayerKey()
                                         If String.IsNullOrEmpty(playerKey) Then Exit Sub

                                         Dim isSub As Boolean = dgv.CurrentRow.Cells("Grade").Value?.ToString() = "Sub"
                                         Dim ldBal As Decimal = CDec(dgv.CurrentRow.Cells("LDBalance").Value)
                                         Dim mSkins As Decimal = CDec(ctx.rLeagueParmrow("Skins"))
                                         Dim mCTP As Decimal = CDec(ctx.rLeagueParmrow("Closest"))
                                         Dim skinsPaid As Boolean = hasPayment(playerKey, "Skin")
                                         Dim ctpPaid As Boolean = hasPayment(playerKey, "CTP")
                                         Dim eoyPaid As Decimal = CDec(dgv.CurrentRow.Cells("EOYPaid").Value)
                                         Dim eoyBal As Decimal = eoyTotal - eoyPaid

                                         Dim inEOY1 As Boolean = False
                                         Dim inEOY2 As Boolean = False
                                         Using cmd As New SQLiteCommand("
                SELECT IFNULL(SUM(Earned),0) FROM Payments 
                WHERE League=@League AND Player=@Player 
                AND Desc='EOY Skins' AND Comment='(1st Week)'", ctx.Conn)
                                             cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                             cmd.Parameters.AddWithValue("@Player", playerKey)
                                             If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                             inEOY1 = CDec(cmd.ExecuteScalar()) > 0
                                         End Using
                                         Using cmd As New SQLiteCommand("
                SELECT IFNULL(SUM(Earned),0) FROM Payments 
                WHERE League=@League AND Player=@Player 
                AND Desc='EOY Skins' AND Comment='(2nd Week)'", ctx.Conn)
                                             cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                             cmd.Parameters.AddWithValue("@Player", playerKey)
                                             inEOY2 = CDec(cmd.ExecuteScalar()) > 0
                                         End Using

                                         Using frm As New frmPayDues(
                playerKey, isSub,
                Not skinsPaid, mSkins,
                Not ctpPaid, mCTP,
                If(isSub, 0, ldBal),
                If(isSub, 0, eoyBal),
                If(isSub, 0, eoyWeekAmt),
                inEOY1, inEOY2)

                                             If frm.ShowDialog() = DialogResult.OK Then
                                                 ctx.sPlayer = playerKey
                                                 If frm.PaySkins Then EditScoresEngine.MakePmt("Skin")
                                                 If frm.PayCTP Then EditScoresEngine.MakePmt("CTP")
                                                 If frm.PayLeagueDues > 0 Then EditScoresEngine.MakePmtAmount("League Dues", frm.PayLeagueDues)
                                                 If frm.PayEOY > 0 Then EditScoresEngine.MakePmtAmount("EOY Skins", frm.PayEOY, frm.EOYWeekComment)
                                                 LoadPaymentsTab(tc)
                                             End If
                                         End Using
                                     End Sub

            ' --- REFUND BUTTON CLICK ---
            AddHandler btnRefund.Click, Sub(s, ev)
                                            Dim playerKey As String = getPlayerKey()
                                            If String.IsNullOrEmpty(playerKey) Then Exit Sub

                                            Dim hasSkinsPayment As Boolean = hasPayment(playerKey, "Skin")
                                            Dim hasCTPPayment As Boolean = hasPayment(playerKey, "CTP")

                                            Dim msg As String = $"Refund payments for {playerKey} on {ctx.ActiveDate}?"
                                            If hasSkinsPayment Then msg &= vbCrLf & "- Delete Skins payment"
                                            If hasCTPPayment Then msg &= vbCrLf & "- Delete CTP payment"

                                            Dim mbr = MessageBox.Show(msg, "Confirm Refund", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                            If mbr <> DialogResult.Yes Then Exit Sub

                                            If hasSkinsPayment Then
                                                deletePayment(playerKey, "Skin")
                                                ' Update scores grid if visible
                                                If dgScores IsNot Nothing Then
                                                    For Each r As DataGridViewRow In dgScores.Rows
                                                        If r.Cells("Player").Value?.ToString() = playerKey Then
                                                            r.Cells("InSkins").Value = "N"
                                                        End If
                                                    Next
                                                End If
                                            End If

                                            If hasCTPPayment Then
                                                deletePayment(playerKey, "CTP")
                                                If dgScores IsNot Nothing Then
                                                    For Each r As DataGridViewRow In dgScores.Rows
                                                        If r.Cells("Player").Value?.ToString() = playerKey Then
                                                            r.Cells("InCTPs").Value = "N"
                                                        End If
                                                    Next
                                                End If
                                            End If

                                            LoadPaymentsTab(tc)
                                        End Sub

            ' --- RIGHT CLICK EOY PAID ---
            AddHandler dgv.CellMouseClick, Sub(s, ev)
                                               If ev.Button <> MouseButtons.Right Then Exit Sub
                                               If ev.RowIndex < 0 Then Exit Sub
                                               If dgv.Columns(ev.ColumnIndex).Name <> "EOYPaid" Then Exit Sub

                                               Dim playerKey As String = dgv.Rows(ev.RowIndex).Cells("PlayerKey").Value?.ToString()
                                               Dim isSub As Boolean = dgv.Rows(ev.RowIndex).Cells("Grade").Value?.ToString() = "Sub"
                                               If isSub OrElse String.IsNullOrEmpty(playerKey) Then Exit Sub

                                               Dim eoyPaid As Decimal = CDec(dgv.Rows(ev.RowIndex).Cells("EOYPaid").Value)
                                               Dim eoyBal As Decimal = eoyTotal - eoyPaid
                                               If eoyBal <= 0 Then Exit Sub

                                               Dim inEOY1 As Boolean = False
                                               Dim inEOY2 As Boolean = False
                                               Using cmd As New SQLiteCommand("
                SELECT IFNULL(SUM(Earned),0) FROM Payments 
                WHERE League=@League AND Player=@Player 
                AND Desc='EOY Skins' AND Comment='(1st Week)'", ctx.Conn)
                                                   cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                                   cmd.Parameters.AddWithValue("@Player", playerKey)
                                                   If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                   inEOY1 = CDec(cmd.ExecuteScalar()) > 0
                                               End Using
                                               Using cmd As New SQLiteCommand("
                SELECT IFNULL(SUM(Earned),0) FROM Payments 
                WHERE League=@League AND Player=@Player 
                AND Desc='EOY Skins' AND Comment='(2nd Week)'", ctx.Conn)
                                                   cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                                   cmd.Parameters.AddWithValue("@Player", playerKey)
                                                   inEOY2 = CDec(cmd.ExecuteScalar()) > 0
                                               End Using

                                               Using frm As New frmPayDues(
                playerKey, isSub,
                False, 0, False, 0,
                0, eoyBal, eoyWeekAmt,
                inEOY1, inEOY2)

                                                   If frm.ShowDialog() = DialogResult.OK Then
                                                       ctx.sPlayer = playerKey
                                                       If frm.PayEOY > 0 Then EditScoresEngine.MakePmtAmount("EOY Skins", frm.PayEOY, frm.EOYWeekComment)
                                                       LoadPaymentsTab(tc)
                                                   End If
                                               End Using
                                           End Sub

            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadPaymentsTab Error: " & ex.Message)
        End Try
    End Sub
    Friend Sub LoadExpensesTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Expenses")
            tp.Controls.Clear()

            Dim season As String = ctx.SeasonYear
            Dim eoyWeekAmt As Decimal = CDec(ctx.rLeagueParmrow("SkinsPS")) + CDec(ctx.rLeagueParmrow("ClosestPS"))
            Dim eoyTotal As Decimal = eoyWeekAmt * 2
            Dim numRegulars As Integer = 0
            Dim ldCost As Decimal = CDec(ctx.rLeagueParmrow("Cost"))

            ' Count regulars
            Using cmd As New SQLiteCommand("
            SELECT COUNT(*) FROM Teams 
            WHERE League=@League AND Year=@Year", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Year", CInt(season))
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                numRegulars = CInt(cmd.ExecuteScalar())
            End Using

            ' --- LABEL ---
            Dim lbl As New Label()
            lbl.Text = $"Balance Sheet - {season} Season"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(10, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            ' --- ENTER EXPENSES BUTTON ---
            Dim btnExpenses As New Button()
            btnExpenses.Name = "btnEnterExpenses"
            btnExpenses.Text = "Enter Expenses"
            btnExpenses.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            btnExpenses.Size = New Size(120, 30)
            btnExpenses.Location = New Point(10, 35)
            btnExpenses.BackColor = Color.FromArgb(0, 120, 215)
            btnExpenses.ForeColor = Color.White
            btnExpenses.FlatStyle = FlatStyle.Flat
            btnExpenses.FlatAppearance.BorderSize = 0
            tp.Controls.Add(btnExpenses)
            AddHandler btnExpenses.Click, Sub(s, ev)
                                              Using frm As New frmExpenses(tc)
                                                  frm.ShowDialog()
                                                  LoadExpensesTab(tc)  ' reload after closing
                                              End Using
                                          End Sub

            ' --- BUILD BALANCE SHEET ---
            Dim dt As New DataTable()
            dt.Columns.Add("Category", GetType(String))
            dt.Columns.Add("Due", GetType(Decimal))
            dt.Columns.Add("Collected", GetType(Decimal))
            dt.Columns.Add("PaidOut", GetType(Decimal))
            dt.Columns.Add("Balance", GetType(Decimal))

            ' Helper to query payments
            Dim getAmount As Func(Of String, String, Decimal) = Function(descFilter As String, detailFilter As String)
                                                                    Dim qsql As String = $"
                SELECT IFNULL(SUM(Earned),0) FROM Payments
                WHERE League=@League
                AND SUBSTR(Date,1,4)=@Season
                AND Date<=@Date
                AND {descFilter}
                AND {detailFilter}"
                                                                    Using cmd As New SQLiteCommand(qsql, ctx.Conn)
                                                                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                                                        cmd.Parameters.AddWithValue("@Season", season)
                                                                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                                                                        If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                                        Return CDec(cmd.ExecuteScalar())
                                                                    End Using
                                                                End Function

            ' --- LEAGUE DUES ---
            Dim ldCollected As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Date<=@Date AND Desc='League Dues' AND Detail='Payment'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                ldCollected = CDec(cmd.ExecuteScalar())
            End Using
            Dim ldDue As Decimal = numRegulars * ldCost
            Dim ldRow = dt.NewRow()
            ldRow("Category") = "League Dues"
            ldRow("Due") = ldDue
            ldRow("Collected") = ldCollected
            ldRow("PaidOut") = 0
            ldRow("Balance") = ldDue - ldCollected
            dt.Rows.Add(ldRow)

            Dim subTot1 = dt.NewRow()
            subTot1("Category") = "-- League Dues Total --"
            subTot1("Due") = ldDue
            subTot1("Collected") = ldCollected
            subTot1("PaidOut") = 0
            subTot1("Balance") = ldDue - ldCollected
            dt.Rows.Add(subTot1)

            ' --- SKINS ---
            Dim skinsCollected As Decimal = 0
            Dim skinsPaidOut As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Date<=@Date AND Desc='Skin' AND Detail='Payment'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                skinsCollected = CDec(cmd.ExecuteScalar())
            End Using
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Date<=@Date AND Desc='Skin' AND Detail LIKE '#%'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                skinsPaidOut = CDec(cmd.ExecuteScalar())
            End Using

            Dim skinsRow = dt.NewRow()
            skinsRow("Category") = "Skins"
            skinsRow("Due") = 0
            ' --- SKINS ---
            skinsRow("Collected") = If(ctx.IsPostSeason, skinsCollected, 0)
            skinsRow("PaidOut") = If(ctx.IsPostSeason, skinsPaidOut, 0)
            skinsRow("Balance") = If(ctx.IsPostSeason, skinsCollected - skinsPaidOut, 0)
            dt.Rows.Add(skinsRow)

            ' --- CTP ---
            Dim ctpCollected As Decimal = 0
            Dim ctpPaidOut As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Date<=@Date AND Desc='CTP' AND Detail='Payment'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                ctpCollected = CDec(cmd.ExecuteScalar())
            End Using
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Date<=@Date AND Desc='CTP' AND Detail LIKE '#%'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                ctpPaidOut = CDec(cmd.ExecuteScalar())
            End Using
            Dim ctpRow = dt.NewRow()
            ctpRow("Category") = "CTP"
            ctpRow("Due") = 0
            ' --- CTP ---
            ctpRow("Collected") = If(ctx.IsPostSeason, ctpCollected, 0)
            ctpRow("PaidOut") = If(ctx.IsPostSeason, ctpPaidOut, 0)
            ctpRow("Balance") = If(ctx.IsPostSeason, ctpCollected - ctpPaidOut, 0)
            dt.Rows.Add(ctpRow)

            ' --- EOY SKINS ---
            Dim eoySkinsCollected As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc='EOY Skins' AND Detail='Payment'
            AND (Comment='(1st Week)' OR Comment='')", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                eoySkinsCollected = CDec(cmd.ExecuteScalar())
            End Using

            ' --- EOY SKINS ---
            Dim eoySkinsRow = dt.NewRow()
            eoySkinsRow("Category") = "EOY Skins"
            eoySkinsRow("Due") = 0
            eoySkinsRow("Collected") = eoySkinsCollected
            eoySkinsRow("PaidOut") = 0
            eoySkinsRow("Balance") = 0
            dt.Rows.Add(eoySkinsRow)

            ' --- EOY CTP ---
            Dim eoyCtpCollected As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc='EOY Skins' AND Detail='Payment'
            AND Comment='(2nd Week)'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                eoyCtpCollected = CDec(cmd.ExecuteScalar())
            End Using

            ' --- FOOD ---
            Dim foodCollected As Decimal = 0
            Dim foodCharge As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc='Food' AND Detail='Payment'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                foodCollected = CDec(cmd.ExecuteScalar())
            End Using
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc='Food' AND Detail='Charge'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                foodCharge = CDec(cmd.ExecuteScalar())
            End Using
            Dim foodRow = dt.NewRow()
            foodRow("Category") = "Food"
            foodRow("Due") = 0
            foodRow("Collected") = foodCollected
            foodRow("PaidOut") = foodCharge
            foodRow("Balance") = foodCollected - foodCharge
            dt.Rows.Add(foodRow)

            ' --- DRINKS ---
            Dim drinksCollected As Decimal = 0
            Dim drinksCharge As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc='Drinks' AND Detail='Payment'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                drinksCollected = CDec(cmd.ExecuteScalar())
            End Using
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc='Drinks' AND Detail='Charge'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                drinksCharge = CDec(cmd.ExecuteScalar())
            End Using
            Dim drinksRow = dt.NewRow()
            drinksRow("Category") = "Drinks"
            drinksRow("Due") = 0
            drinksRow("Collected") = drinksCollected
            drinksRow("PaidOut") = drinksCharge
            drinksRow("Balance") = drinksCollected - drinksCharge
            dt.Rows.Add(drinksRow)

            Dim subTot2 = dt.NewRow()
            subTot2("Category") = "-- Food/Drinks Total --"
            subTot2("Due") = 0
            subTot2("Collected") = foodCollected + drinksCollected
            subTot2("PaidOut") = foodCharge + drinksCharge
            subTot2("Balance") = (foodCollected + drinksCollected) - (foodCharge + drinksCharge)
            dt.Rows.Add(subTot2)

            ' --- REG SEASON CHAMP ---
            Dim regChampPaidOut As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc LIKE 'Reg Season Champ%'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                regChampPaidOut = CDec(cmd.ExecuteScalar())
            End Using
            Dim regChampRow = dt.NewRow()
            regChampRow("Category") = "Reg Season Champ"
            regChampRow("Due") = 0
            regChampRow("Collected") = 0
            regChampRow("PaidOut") = regChampPaidOut
            regChampRow("Balance") = -regChampPaidOut
            dt.Rows.Add(regChampRow)

            ' --- CLUB CHAMP ---
            Dim clubChampPaidOut As Decimal = 0
            Using cmd As New SQLiteCommand("
            SELECT IFNULL(SUM(Earned),0) FROM Payments
            WHERE League=@League AND SUBSTR(Date,1,4)=@Season
            AND Desc LIKE 'Club Champion%'", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Season", season)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                clubChampPaidOut = CDec(cmd.ExecuteScalar())
            End Using
            Dim clubChampRow = dt.NewRow()
            clubChampRow("Category") = "Club Champ"
            clubChampRow("Due") = 0
            clubChampRow("Collected") = 0
            clubChampRow("PaidOut") = clubChampPaidOut
            clubChampRow("Balance") = -clubChampPaidOut
            dt.Rows.Add(clubChampRow)

            ' --- TOTALS ROW ---
            Dim totalRow = dt.NewRow()
            totalRow("Category") = "TOTALS"
            totalRow("Due") = 0D
            totalRow("Collected") = 0D
            totalRow("PaidOut") = 0D
            totalRow("Balance") = 0D
            dt.Rows.Add(totalRow)
            For Each row As DataRow In dt.Rows
                If row("Category").ToString().StartsWith("--") Then Continue For  ' skip subtotals
                If row("Category").ToString() = "TOTALS" Then Continue For
                totalRow("Due") = CDec(totalRow("Due")) + CDec(row("Due"))
                totalRow("Collected") = CDec(totalRow("Collected")) + CDec(row("Collected"))
                totalRow("PaidOut") = CDec(totalRow("PaidOut")) + CDec(row("PaidOut"))
                totalRow("Balance") = CDec(totalRow("Balance")) + CDec(row("Balance"))
            Next
            LOGIT($"LoadExpensesTab: dt has {dt.Rows.Count} rows")
            ' --- GRID ---
            Dim dgv As New DataGridView()
            dgv.Name = "dgExpenses"
            dgv.Location = New Point(10, 75)
            dgv.DataSource = dt
            dgv.AutoGenerateColumns = True
            tp.Controls.Add(dgv)

            ' --- STYLING ---
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9.0!)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0!, FontStyle.Bold)
            dgv.RowTemplate.Height = 22
            dgv.ColumnHeadersHeight = 28
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToResizeRows = False
            dgv.ReadOnly = True
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            dgv.Location = New Point(10, 70)
            dgv.Size = New Size(500, 350)

            ' --- COLUMN SETTINGS ---
            Dim colSettings As New Dictionary(Of String, (Width As Integer, Header As String)) From {
            {"Category", (150, "Category")},
            {"Due", (80, "Due")},
            {"Collected", (80, "Collected")},
            {"PaidOut", (80, "Paid Out")},
            {"Balance", (80, "Balance")}
        }

            For Each col As DataGridViewColumn In dgv.Columns
                If colSettings.ContainsKey(col.Name) Then
                    col.Visible = True
                    col.Width = colSettings(col.Name).Width
                    col.HeaderText = colSettings(col.Name).Header
                    If col.Name <> "Category" Then
                        col.DefaultCellStyle.Format = "C2"
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    End If
                Else
                    col.Visible = False
                End If
            Next

            ' --- COLOR CODE ---
            For i As Integer = 0 To dgv.Rows.Count - 1
                If dgv.Rows(i).IsNewRow Then Continue For
                Dim cat = dgv.Rows(i).Cells("Category").Value?.ToString()

                ' Totals row
                If cat = "TOTALS" Then
                    dgv.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200)
                    dgv.Rows(i).DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                    Continue For
                End If

                ' Balance color
                Dim bal = dgv.Rows(i).Cells("Balance").Value
                If IsNumeric(bal) Then
                    If CDec(bal) < 0 Then
                        dgv.Rows(i).Cells("Balance").Style.ForeColor = Color.DarkRed
                        dgv.Rows(i).Cells("Balance").Style.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                    ElseIf CDec(bal) > 0 Then
                        dgv.Rows(i).Cells("Balance").Style.ForeColor = Color.DarkGreen
                        dgv.Rows(i).Cells("Balance").Style.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                    End If
                End If
                If cat.StartsWith("--") Then
                    dgv.Rows(i).DefaultCellStyle.BackColor = Color.FromArgb(220, 220, 220)
                    dgv.Rows(i).DefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
                    Continue For
                End If
            Next

            dgv.ClearSelection()
            dgv.CurrentCell = Nothing

        Catch ex As Exception
            LOGIT("LoadExpensesTab Error: " & ex.Message)
        End Try
    End Sub
    Friend Sub LoadLeadersTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("Leaders")
            tp.Controls.Clear()
            tp.AutoScroll = False

            ' --- LABEL ---
            Dim lbl As New Label()
            lbl.Text = $"Leaders - {ctx.SeasonYear} Season"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(10, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            ' --- YEAR FILTER ---
            Dim lblYear As New Label()
            lblYear.Text = "Season:"
            lblYear.Font = New Font("Segoe UI", 9)
            lblYear.Location = New Point(200, 12)
            lblYear.AutoSize = True
            tp.Controls.Add(lblYear)

            Dim cmbYear As New ComboBox()
            cmbYear.Name = "cmbLeaderYear"
            cmbYear.Width = 80
            cmbYear.Location = New Point(250, 10)
            cmbYear.DropDownStyle = ComboBoxStyle.DropDownList
            cmbYear.Items.Add("Career")
            Using cmd As New SQLiteCommand("
        SELECT DISTINCT SUBSTR(Date,1,4) AS Yr 
        FROM Scores WHERE League=@League
        ORDER BY Yr DESC", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using dr As SQLiteDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        cmbYear.Items.Add(dr("Yr").ToString())
                    End While
                End Using
            End Using
            For Each item As Object In cmbYear.Items
                If item.ToString() = ctx.SeasonYear Then
                    cmbYear.SelectedItem = item
                    Exit For
                End If
            Next
            If cmbYear.SelectedIndex = -1 Then cmbYear.SelectedIndex = 0
            tp.Controls.Add(cmbYear)

            ' --- REGULARS ONLY CHECKBOX ---
            Dim chkReg As New CheckBox()
            chkReg.Name = "chkLeaderRegulars"
            chkReg.Text = "Regulars Only"
            chkReg.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            chkReg.Location = New Point(345, 12)
            chkReg.AutoSize = True
            chkReg.Checked = True
            tp.Controls.Add(chkReg)

            ' --- SCROLL PANEL ---
            ' AutoScroll handles BOTH directions - no manual scrollbars needed
            Dim scrollPanel As New Panel()
            scrollPanel.Name = "pnlLeaders"
            scrollPanel.Location = New Point(0, 40)
            scrollPanel.Size = New Size(tc.Width - 5, tc.Height - 45)
            scrollPanel.AutoScroll = True
            tp.Controls.Add(scrollPanel)

            ' Wire events
            AddHandler cmbYear.SelectedIndexChanged, Sub(s, ev)
                                                         LoadLeaderBoards(scrollPanel, cmbYear, chkReg)
                                                     End Sub
            AddHandler chkReg.CheckedChanged, Sub(s, ev)
                                                  LoadLeaderBoards(scrollPanel, cmbYear, chkReg)
                                              End Sub

            ' Initial load
            LoadLeaderBoards(scrollPanel, cmbYear, chkReg)

        Catch ex As Exception
            LOGIT("LoadLeadersTab Error: " & ex.Message)
        End Try
    End Sub
    Private Sub LoadLeaderBoards(scrollPanel As Panel, cmbYear As ComboBox, chkReg As CheckBox)
        Try
            ' Remove existing controls
            Dim toRemove As New List(Of Control)
            For Each ctrl As Control In scrollPanel.Controls
                toRemove.Add(ctrl)
            Next
            For Each ctrl As Control In toRemove
                scrollPanel.Controls.Remove(ctrl)
                ctrl.Dispose()
            Next

            Dim selectedYear As String = cmbYear.SelectedItem?.ToString()
            Dim showRegularsOnly As Boolean = chkReg.Checked

            ' Load regulars
            Dim dtRegulars As New DataTable()
            Using cmd As New SQLiteCommand("
        SELECT Player, Year FROM Teams WHERE League=@League", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dtRegulars)
                End Using
            End Using

            ' Load stats
            Dim dtStats As DataTable = QueryLeaderStats(selectedYear)

            Dim stats As String() = {"Rounds", "Points", "F9Avg", "B9Avg", "AvgGross",
                              "TotalEarned", "SkinsWon", "CTPsWon",
                              "Eagles", "Birdies", "Pars", "Bogeys", "DoubleBogeys", "Others"}
            Dim ascStats As New List(Of String) From {"F9Avg", "B9Avg", "AvgGross", "Bogeys", "DoubleBogeys", "Others"}

            Dim colWidth As Integer = 140
            Dim xStart As Integer = 5
            Dim yStart As Integer = 20
            Dim maxDgvHeight As Integer = 0

            Dim i As Integer = 0
            For Each stat As String In stats

                ' Filter rows
                Dim filteredRows As New List(Of DataRow)
                For Each row As DataRow In dtStats.Rows
                    If showRegularsOnly Then
                        Dim yr As String = If(selectedYear = "Career", DateTime.Now.Year.ToString(), selectedYear)
                        Dim isReg As Boolean = False
                        For Each tr As DataRow In dtRegulars.Rows
                            If tr("Player").ToString() = row("Player").ToString() AndAlso
                       tr("Year").ToString() = yr Then
                                isReg = True
                                Exit For
                            End If
                        Next
                        If Not isReg Then Continue For
                    End If
                    If IsNumeric(row(stat)) AndAlso CDec(row(stat)) > 0 Then
                        filteredRows.Add(row)
                    End If
                Next

                ' Sort
                If ascStats.Contains(stat) Then
                    filteredRows.Sort(Function(a, b) CDec(a(stat)).CompareTo(CDec(b(stat))))
                Else
                    filteredRows.Sort(Function(a, b) CDec(b(stat)).CompareTo(CDec(a(stat))))
                End If

                ' Stat label
                Dim statLbl As New Label()
                statLbl.Name = $"lblStat{stat}"
                statLbl.Text = stat.Replace("Avg", " Avg").Replace("Won", " Won").Replace("Earned", " $")
                statLbl.Font = New Font("Segoe UI", 8, FontStyle.Bold)
                statLbl.Location = New Point(xStart + i * colWidth + 12, yStart)
                statLbl.AutoSize = True
                scrollPanel.Controls.Add(statLbl)

                ' Grid
                Dim dgv As New DataGridView()
                dgv.Name = $"dgLeader{stat}"
                dgv.Location = New Point(xStart + i * colWidth, yStart + 18)
                dgv.Width = colWidth - 5
                dgv.RowHeadersVisible = False
                dgv.AllowUserToAddRows = False
                dgv.AllowUserToDeleteRows = False
                dgv.AllowUserToResizeRows = False
                dgv.ReadOnly = True
                dgv.AutoGenerateColumns = False
                dgv.RowTemplate.Height = 20
                dgv.DefaultCellStyle.Font = New Font("Tahoma", 7.5)
                dgv.ScrollBars = ScrollBars.None
                dgv.BorderStyle = BorderStyle.None
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                dgv.GridColor = Color.LightGray
                dgv.BackgroundColor = Color.White
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

                ' Columns
                Dim playerCol As New DataGridViewTextBoxColumn()
                playerCol.Name = "Player"
                playerCol.HeaderText = "Player"
                playerCol.Width = 95
                dgv.Columns.Add(playerCol)

                Dim valCol As New DataGridViewTextBoxColumn()
                valCol.Name = "Val"
                valCol.HeaderText = ""
                valCol.Width = 40
                valCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dgv.Columns.Add(valCol)

                ' Rows
                Dim rank As Integer = 1
                For Each row As DataRow In filteredRows
                    Dim rowIdx = dgv.Rows.Add(row("Player").ToString(), row(stat).ToString())

                    ' Top 3 colors
                    Select Case rank
                        Case 1
                            dgv.Rows(rowIdx).DefaultCellStyle.BackColor = Color.Gold
                            dgv.Rows(rowIdx).DefaultCellStyle.Font = New Font("Tahoma", 7.5, FontStyle.Bold)
                        Case 2
                            dgv.Rows(rowIdx).DefaultCellStyle.BackColor = Color.Silver
                        Case 3
                            dgv.Rows(rowIdx).DefaultCellStyle.BackColor = Color.FromArgb(205, 127, 50)
                    End Select

                    ' Highlight non-regulars aqua
                    If Not showRegularsOnly Then
                        Dim yr As String = If(selectedYear = "Career", DateTime.Now.Year.ToString(), selectedYear)
                        Dim isReg As Boolean = False
                        For Each tr As DataRow In dtRegulars.Rows
                            If tr("Player").ToString() = row("Player").ToString() AndAlso
                       tr("Year").ToString() = yr Then
                                isReg = True
                                Exit For
                            End If
                        Next
                        If Not isReg Then
                            dgv.Rows(rowIdx).Cells("Player").Style.BackColor = Color.Aqua
                        End If
                    End If

                    rank += 1
                Next

                ' Size DGV height to fit content exactly
                Dim dgvHeight As Integer = (filteredRows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 2
                dgv.Height = dgvHeight

                If dgvHeight > maxDgvHeight Then maxDgvHeight = dgvHeight

                scrollPanel.Controls.Add(dgv)
                i += 1
            Next

            ' Set scroll area to cover all columns wide and tallest column tall
            scrollPanel.AutoScrollMinSize = New Size(stats.Length * colWidth + 20, maxDgvHeight + yStart + 40)

        Catch ex As Exception
            LOGIT("LoadLeaderBoards Error: " & ex.Message)
        End Try
    End Sub
    Private Function QueryLeaderStats(selectedYear As String) As DataTable
        Dim dt As New DataTable()
        Try
            Dim courseName As String = ctx.rLeagueParmrow("Course").ToString()
            Dim isCareer As Boolean = String.IsNullOrEmpty(selectedYear) OrElse selectedYear = "Career"

            Dim yearClause As String = If(isCareer, "", "AND SUBSTR(S.Date,1,4)=@Year ")

            Dim sql As String = "
SELECT 
    SC.Player,
    SC.Rounds, SC.F9, SC.B9, SC.F9Avg, SC.B9Avg, SC.AvgGross,
    SC.Eagles, SC.Birdies, SC.Pars, SC.Bogeys, SC.DoubleBogeys, SC.Others,
    COALESCE(PY.SkinsWon,0) AS SkinsWon,
    COALESCE(PY.SkinsEarned,0) AS SkinsEarned,
    COALESCE(PY.CTPsWon,0) AS CTPsWon,
    COALESCE(PY.CTPsEarned,0) AS CTPsEarned,
    COALESCE(PY.SkinsEarned,0) + COALESCE(PY.CTPsEarned,0) AS TotalEarned,
    COALESCE(MA.Points,0) AS Points,
    COALESCE(MA.TeamPoints,0) AS TeamPoints
FROM (
    SELECT S.Player,
        COUNT(*) AS Rounds,
        COUNT(CASE WHEN S.FrontBack='Front' THEN 1 END) AS F9,
        COUNT(CASE WHEN S.FrontBack <> 'Front' THEN 1 END) AS B9,
        COALESCE(ROUND(AVG(CASE WHEN S.FrontBack='Front' THEN CAST(S.Gross AS REAL) END),1),0) AS F9Avg,
        COALESCE(ROUND(AVG(CASE WHEN S.FrontBack <> 'Front' THEN CAST(S.Gross AS REAL) END),1),0) AS B9Avg,
        COALESCE(ROUND(AVG(CAST(S.Gross AS REAL)),1),0) AS AvgGross,
        SUM(CASE WHEN S.FrontBack='Front' THEN
            (CASE WHEN S.[1]=C.Hole1-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole2-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole3-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole4-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole5-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole6-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole7-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole8-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole9-2 THEN 1 ELSE 0 END)
        ELSE (CASE WHEN S.[1]=C.Hole10-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole11-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole12-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole13-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole14-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole15-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole16-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole17-2 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole18-2 THEN 1 ELSE 0 END) END) AS Eagles,
        SUM(CASE WHEN S.FrontBack='Front' THEN
            (CASE WHEN S.[1]=C.Hole1-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole2-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole3-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole4-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole5-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole6-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole7-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole8-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole9-1 THEN 1 ELSE 0 END)
        ELSE (CASE WHEN S.[1]=C.Hole10-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole11-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole12-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole13-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole14-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole15-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole16-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole17-1 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole18-1 THEN 1 ELSE 0 END) END) AS Birdies,
        SUM(CASE WHEN S.FrontBack='Front' THEN
            (CASE WHEN S.[1]=C.Hole1 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole3 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole4 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole5 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole6 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole7 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole8 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole9 THEN 1 ELSE 0 END)
        ELSE (CASE WHEN S.[1]=C.Hole10 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole11 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole12 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole13 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole14 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole15 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole16 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole17 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole18 THEN 1 ELSE 0 END) END) AS Pars,
        SUM(CASE WHEN S.FrontBack='Front' THEN
            (CASE WHEN S.[1]=C.Hole1+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole2+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole3+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole4+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole5+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole6+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole7+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole8+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole9+1 THEN 1 ELSE 0 END)
        ELSE (CASE WHEN S.[1]=C.Hole10+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole11+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole12+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole13+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole14+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole15+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole16+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole17+1 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole18+1 THEN 1 ELSE 0 END) END) AS Bogeys,
        SUM(CASE WHEN S.FrontBack='Front' THEN
            (CASE WHEN S.[1]=C.Hole1+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole2+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole3+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole4+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole5+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole6+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole7+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole8+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole9+2 THEN 1 ELSE 0 END)
        ELSE (CASE WHEN S.[1]=C.Hole10+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]=C.Hole11+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]=C.Hole12+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]=C.Hole13+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]=C.Hole14+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]=C.Hole15+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]=C.Hole16+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]=C.Hole17+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]=C.Hole18+2 THEN 1 ELSE 0 END) END) AS DoubleBogeys,
        SUM(CASE WHEN S.FrontBack='Front' THEN
            (CASE WHEN S.[1]>C.Hole1+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]>C.Hole2+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]>C.Hole3+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]>C.Hole4+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]>C.Hole5+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]>C.Hole6+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]>C.Hole7+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]>C.Hole8+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]>C.Hole9+2 THEN 1 ELSE 0 END)
        ELSE (CASE WHEN S.[1]>C.Hole10+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[2]>C.Hole11+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[3]>C.Hole12+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[4]>C.Hole13+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[5]>C.Hole14+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[6]>C.Hole15+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[7]>C.Hole16+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[8]>C.Hole17+2 THEN 1 ELSE 0 END)+(CASE WHEN S.[9]>C.Hole18+2 THEN 1 ELSE 0 END) END) AS Others
    FROM Scores S
    LEFT JOIN Courses C ON C.Name=@Course
    WHERE S.League=@League
    AND S.Gross IS NOT NULL AND S.Gross > 0
    " & yearClause & "
    GROUP BY S.Player
) SC
LEFT JOIN (
    SELECT Player,
        SUM(CASE WHEN [Desc]='Skin' AND Detail LIKE '#%' THEN 1 ELSE 0 END) AS SkinsWon,
        SUM(CASE WHEN [Desc]='Skin' AND Detail LIKE '#%' THEN Earned ELSE 0 END) AS SkinsEarned,
        SUM(CASE WHEN [Desc]='CTP' AND Detail LIKE '#%' THEN 1 ELSE 0 END) AS CTPsWon,
        SUM(CASE WHEN [Desc]='CTP' AND Detail LIKE '#%' THEN Earned ELSE 0 END) AS CTPsEarned
    FROM Payments
    WHERE 1=1 " & If(isCareer, "", "AND SUBSTR(Date,1,4)=@Year ") & "
    GROUP BY Player
) PY ON PY.Player=SC.Player
LEFT JOIN (
    SELECT Player,
        ROUND(SUM(Points),1) AS Points,
        ROUND(SUM(Team_Points),1) AS TeamPoints
    FROM Matches
    WHERE League=@League
    " & If(isCareer, "", "AND SUBSTR(Date,1,4)=@Year ") & "
    GROUP BY Player
) MA ON MA.Player=SC.Player"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Course", courseName)
                If Not isCareer Then
                    cmd.Parameters.AddWithValue("@Year", selectedYear)
                End If
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using

        Catch ex As Exception
            LOGIT($"QueryLeaderStats Error: {ex.Message}")
        End Try
        Return dt
    End Function
    Friend Sub LoadGallusImportTab(tc As TabControl)
        Try
            Dim tp = tc.TabPages("GallusImport")
            tp.Controls.Clear()

            ' --- LABEL ---
            Dim lbl As New Label()
            lbl.Text = "Gallus Import History"
            lbl.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            lbl.Location = New Point(10, 10)
            lbl.AutoSize = True
            tp.Controls.Add(lbl)

            ' --- DATE FILTER ---
            Dim lblDate As New Label()
            lblDate.Text = "Date:"
            lblDate.Font = New Font("Segoe UI", 9)
            lblDate.Location = New Point(200, 12)
            lblDate.AutoSize = True
            tp.Controls.Add(lblDate)

            Dim cmbDate As New ComboBox()
            cmbDate.Name = "cmbGallusDate"
            cmbDate.Width = 100
            cmbDate.Location = New Point(235, 10)
            cmbDate.DropDownStyle = ComboBoxStyle.DropDownList
            cmbDate.Items.Add("All Dates")
            Using cmd As New SQLiteCommand("
            SELECT DISTINCT Date FROM GallusImport
            WHERE League=@League
            ORDER BY Date DESC", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using dr As SQLiteDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        cmbDate.Items.Add(dr("Date").ToString())
                    End While
                End Using
            End Using
            cmbDate.SelectedIndex = 0
            tp.Controls.Add(cmbDate)

            ' --- DELETE SELECTED BUTTON ---
            Dim btnDelete As New Button()
            btnDelete.Text = "Delete Selected"
            btnDelete.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            btnDelete.Size = New Size(120, 25)
            btnDelete.Location = New Point(350, 8)
            btnDelete.BackColor = Color.FromArgb(180, 0, 0)
            btnDelete.ForeColor = Color.White
            btnDelete.FlatStyle = FlatStyle.Flat
            btnDelete.FlatAppearance.BorderSize = 0
            tp.Controls.Add(btnDelete)

            ' --- GRID ---
            Dim dgv As New DataGridView()
            dgv.Name = "dgGallusImport"
            dgv.Location = New Point(10, 45)
            dgv.Size = New Size(tp.Width - 20, tp.Height - 60)
            dgv.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
            dgv.ScrollBars = ScrollBars.Both  ' change from None to Both
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            dgv.RowHeadersVisible = False
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToDeleteRows = False
            dgv.AllowUserToResizeRows = False
            dgv.ReadOnly = False
            dgv.AutoGenerateColumns = False
            dgv.RowTemplate.Height = 22
            dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9)
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgv.MultiSelect = True
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.GridColor = Color.LightGray
            dgv.BackgroundColor = Color.White
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
            tp.Controls.Add(dgv)
            Dim scrollPanel As New Panel()
            scrollPanel.Location = New Point(10, 45)
            scrollPanel.Size = New Size(tp.Width - 20, tp.Height - 60)

            scrollPanel.AutoScroll = False
            scrollPanel.HorizontalScroll.Enabled = True
            scrollPanel.HorizontalScroll.Visible = True
            scrollPanel.VerticalScroll.Enabled = True
            scrollPanel.VerticalScroll.Visible = True
            scrollPanel.AutoScroll = True
            scrollPanel.AutoScrollMinSize = New Size(dgv.Width + 100, dgv.Height + 100)
            tp.Controls.Add(scrollPanel)


            dgv.Location = New Point(0, 0)
            dgv.ScrollBars = ScrollBars.Vertical
            dgv.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            scrollPanel.Controls.Add(dgv)

            AddHandler tc.SizeChanged, Sub(s, ev)
                                           scrollPanel.Size = New Size(tc.Width - 5, tc.Height - 60)
                                       End Sub
            ' --- COLUMNS ---
            ' --- LOAD COLUMNS DYNAMICALLY FROM TABLE ---
            Dim dtSchema As New DataTable()
            Using cmd As New SQLiteCommand("SELECT * FROM GallusImport LIMIT 0", ctx.Conn)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    da.Fill(dtSchema)
                End Using
            End Using

            For Each col As DataColumn In dtSchema.Columns
                Dim c As New DataGridViewTextBoxColumn()
                c.Name = col.ColumnName
                c.HeaderText = col.ColumnName
                c.DataPropertyName = col.ColumnName
                c.ReadOnly = (col.ColumnName <> "Note")  ' only Note is editable
                ' Set widths
                Select Case col.ColumnName
                    Case "ID" : c.Width = 40
                    Case "Date", "FrontBack", "Owner" : c.Width = 80
                    Case "ImportDate" : c.Width = 130
                    Case "URL" : c.Width = 200
                    Case "GallusName", "ResolvedName", "Note" : c.Width = 150
                    Case "Gross", "OtherGross", "TotalGross" : c.Width = 45
                    Case Else : c.Width = 28  ' hole columns
                End Select
                dgv.Columns.Add(c)
            Next
            ' --- LOAD DATA ---
            Dim loadData As Action = Sub()
                                         Dim dt As New DataTable()
                                         Dim whereSql As String = If(cmbDate.SelectedItem?.ToString() = "All Dates",
                "", $"AND Date='{cmbDate.SelectedItem}'")
                                         Using cmd As New SQLiteCommand($"
                SELECT * FROM GallusImport
                WHERE League=@League {whereSql}
                ORDER BY Date DESC, ImportDate DESC", ctx.Conn)
                                             cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                             If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                             Using da As New SQLiteDataAdapter(cmd)
                                                 da.Fill(dt)
                                             End Using
                                         End Using
                                         dgv.DataSource = dt
                                         Dim rowsHeight As Integer = (dgv.Rows.Count * dgv.RowTemplate.Height) + dgv.ColumnHeadersHeight + 20
                                         Dim dgvWidth As Integer = dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3
                                         dgv.Size = New Size(dgvWidth, rowsHeight)
                                         scrollPanel.AutoScrollMinSize = New Size(dgvWidth, rowsHeight)
                                     End Sub

            loadData()

            ' Set hole headers based on FrontBack and hide O columns
            Dim frontBack As String = "Front"
            Dim dtfb As DataTable = TryCast(dgv.DataSource, DataTable)
            If dtfb IsNot Nothing AndAlso dtfb.Rows.Count > 0 Then
                frontBack = dtfb.Rows(0)("FrontBack").ToString()
            End If

            Dim startHole As Integer = If(frontBack = "Back", 10, 1)
            For i As Integer = 1 To 9
                Dim actualHole As Integer = startHole + i - 1
                If dgv.Columns.Contains($"H{i}") Then
                    dgv.Columns($"H{i}").HeaderText = $"H{actualHole}"
                End If
                If dgv.Columns.Contains($"Par{i}") Then
                    dgv.Columns($"Par{i}").HeaderText = $"Par{actualHole}"
                End If
                If dgv.Columns.Contains($"Hcp{i}") Then
                    dgv.Columns($"Hcp{i}").HeaderText = $"Hcp{actualHole}"
                End If
                ' Hide O columns
                If dgv.Columns.Contains($"O{i}") Then
                    dgv.Columns($"O{i}").Visible = False
                End If
            Next

            If dgv.Columns.Contains("OtherGross") Then
                dgv.Columns("OtherGross").Visible = False
            End If

            ' --- SAVE NOTE ON CELL END EDIT ---
            AddHandler dgv.CellEndEdit, Sub(s, ev)
                                            If dgv.Columns(ev.ColumnIndex).Name <> "Note" Then Exit Sub
                                            If ev.RowIndex < 0 Then Exit Sub
                                            Dim id As Integer = CInt(dgv.Rows(ev.RowIndex).Cells("ID").Value)
                                            Dim note As String = dgv.Rows(ev.RowIndex).Cells("Note").Value?.ToString()
                                            Using cmd As New SQLiteCommand("
                UPDATE GallusImport SET Note=@Note WHERE ID=@ID", ctx.Conn)
                                                cmd.Parameters.AddWithValue("@Note", note)
                                                cmd.Parameters.AddWithValue("@ID", id)
                                                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                cmd.ExecuteNonQuery()
                                            End Using
                                            LOGIT($"GallusImport note updated ID={id}")
                                        End Sub

            ' --- DATE FILTER CHANGE ---
            AddHandler cmbDate.SelectedIndexChanged, Sub(s, ev)
                                                         loadData()
                                                     End Sub
            ' --- DELETE SELECTED ---
            AddHandler btnDelete.Click, Sub(s, ev)
                                            If dgv.SelectedRows.Count = 0 Then Exit Sub
                                            Dim mbr = MessageBox.Show(
                $"Delete {dgv.SelectedRows.Count} selected row(s)?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning)
                                            If mbr <> DialogResult.Yes Then Exit Sub

                                            For Each row As DataGridViewRow In dgv.SelectedRows
                                                Dim id As Integer = CInt(row.Cells("ID").Value)
                                                Using cmd As New SQLiteCommand("
                    DELETE FROM GallusImport WHERE ID=@ID", ctx.Conn)
                                                    cmd.Parameters.AddWithValue("@ID", id)
                                                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                    cmd.ExecuteNonQuery()
                                                End Using
                                                LOGIT($"GallusImport deleted ID={id}")
                                            Next
                                            loadData()
                                        End Sub

        Catch ex As Exception
            LOGIT("LoadGallusImportTab Error: " & ex.Message)
        End Try
    End Sub
#End Region
    Private Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim tabControl As TabControl = DirectCast(sender, TabControl)

        ' Refresh the selected tab
        Select Case tabControl.SelectedTab.Name
            Case "Stableford"
                RefreshStablefordTab()
                Exit Sub
            Case "PrizeMoney"
                LoadPrizeMoneyTab(tabControl)
                Exit Sub
            Case "Schedule"
                LoadScheduleTab(tabControl)
                Exit Sub
            Case "Standings"
                LoadStandingsTab(tabControl)
                Exit Sub
            Case "Last5"
                LoadLast5Tab(tabControl)
                Exit Sub
            Case "SkinsCTPs"
                LoadSkinsCTPsTab(tabControl)
                Exit Sub
            Case "Kitty"
                LoadKittyTab(tabControl)
                Exit Sub
            Case "Payments"
                LoadPaymentsTab(tabControl)
                Exit Sub
        End Select

        ' Try to invalidate the grid on the selected tab
        Try
            Dim dgvname = $"dg{tabControl.SelectedTab.Name}"
            If dgvname.Contains("Standings") Then dgvname &= "1"
            Dim grid = TryCast(tabControl.SelectedTab.Controls(dgvname), DataGridView)
            If grid IsNot Nothing Then
                grid.Invalidate()
                grid.Refresh()
            End If
        Catch ex As Exception
            ' Tab may not have a matching grid - safe to ignore
        End Try
    End Sub
    Private Sub dgv_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) 'Handles dgv.CurrentCellDirtyStateChanged

        Dim grid = CType(sender, DataGridView)
        If grid.IsCurrentCellDirty AndAlso
       grid.CurrentCell IsNot Nothing AndAlso
       TypeOf grid.CurrentCell Is DataGridViewComboBoxCell Then

            grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub
    Private Sub grid_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs)
        Dim tb = TryCast(e.Control, TextBox)

        If IsNumeric(dgScores.CurrentCell.OwningColumn.Name) Then
            If tb IsNot Nothing Then
                tb.Multiline = True
                'CheckHandler(tb)
                ' Remove any existing event handler
                RemoveHandler tb.KeyPress, AddressOf EditingTextBox_KeyPress

                ' Add a new event handler to restrict input to numeric values
                AddHandler tb.KeyPress, AddressOf EditingTextBox_KeyPress

                If dgScores.CurrentCell.OwningColumn.Name = 1 Or dgScores.CurrentCell.OwningColumn.Name = 10 Then
                    tb.MaxLength = 9
                Else
                    tb.MaxLength = 1
                End If
            End If
        Else
            ' Remove any existing event handler
            RemoveHandler tb.KeyPress, AddressOf EditingTextBox_KeyPress
        End If
    End Sub
    Private Sub EditingTextBox_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Allow control keys, digits, and only one decimal point
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c OrElse CType(sender, TextBox).Text.Contains("."c)) Then
            e.Handled = True
        End If
    End Sub
#Region "Unified Scoring Logic"
    ''' <summary>
    ''' Returns the number of strokes (dots) awarded to a player on a specific hole.
    ''' </summary>
    Function showStrokeHoles(hole As Integer, hdcp As Integer) As Integer
        Dim strokes As Integer = 0
        Try
            Dim tempSI As Integer = CInt(ctx.thiscourse($"H{hole}"))
            Do While tempSI <= hdcp
                strokes += 1
                tempSI += 9
            Loop
        Catch
            ' Silent return 0 on error
        End Try
        Return strokes
    End Function

#End Region
    Private Sub dgScores_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        Try
            ' 1. QUICK EXIT
            If e.ColumnIndex < 0 Then Exit Sub

            ' ---------------------------------------------------------
            ' 2. STACKED HEADER LOGIC
            ' ---------------------------------------------------------
            If e.RowIndex = -1 Then
                Dim dgvHeader = DirectCast(sender, DataGridView)
                Dim colName As String = dgvHeader.Columns(e.ColumnIndex).Name
                Dim holeNum As Integer
                Dim isOtherWeek As Boolean = False

                ' Check for this week holes (1-9) or other week holes (O1-O9)
                If colName.StartsWith("O") AndAlso Integer.TryParse(colName.Substring(1), holeNum) AndAlso ctx.IsPostSeason Then
                    isOtherWeek = True
                ElseIf Not Integer.TryParse(colName, holeNum) Then
                    Exit Sub
                End If

                If holeNum >= 1 AndAlso holeNum <= 9 Then
                    Dim thisFB As String = ctx.sFrontBack
                    Dim otherFB As String = If(thisFB = "Front", "Back", "Front")
                    Dim activeFB As String = If(isOtherWeek, otherFB, thisFB)
                    Dim displayHole As Integer = If(activeFB = "Back", holeNum + 9, holeNum)

                    Dim strokeIndex As String = "?"
                    Try
                        Dim colKey As String = "H" & displayHole
                        If ctx.thiscourse IsNot Nothing AndAlso ctx.thiscourse.Table.Columns.Contains(colKey) Then
                            strokeIndex = ctx.thiscourse(colKey).ToString()
                        End If
                    Catch
                        strokeIndex = "!"
                    End Try

                    Dim par As String = "?"
                    Try
                        par = ctx.thiscourse($"Hole{displayHole}").ToString()
                    Catch
                    End Try

                    Dim headerColor As Color = If(activeFB = "Back", Color.ForestGreen, Color.DarkGoldenrod)
                    Using b As New SolidBrush(headerColor)
                        e.Graphics.FillRectangle(b, e.CellBounds)
                    End Using

                    Dim fHole As New Font("Segoe UI", 8, FontStyle.Bold)
                    Dim third As Integer = e.CellBounds.Height \ 3

                    Dim topRect As New Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, third)
                    TextRenderer.DrawText(e.Graphics, displayHole.ToString(), fHole, topRect, Color.White,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

                    Dim midRect As New Rectangle(e.CellBounds.X, e.CellBounds.Y + third, e.CellBounds.Width, third)
                    Dim parColor As Color = If(activeFB = "Back", Color.FromArgb(34, 139, 34), Color.FromArgb(184, 134, 11))
                    Using bMid As New SolidBrush(parColor)
                        e.Graphics.FillRectangle(bMid, midRect)
                    End Using
                    TextRenderer.DrawText(e.Graphics, par, fHole, midRect, Color.White,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

                    Dim bottomRect As New Rectangle(e.CellBounds.X, e.CellBounds.Y + (third * 2), e.CellBounds.Width, e.CellBounds.Height - (third * 2))
                    Using bIndex As New SolidBrush(Color.FromArgb(144, 238, 144))
                        e.Graphics.FillRectangle(bIndex, bottomRect)
                    End Using
                    TextRenderer.DrawText(e.Graphics, strokeIndex, dgvHeader.Font, bottomRect, Color.Black,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

                    e.Paint(e.CellBounds, DataGridViewPaintParts.Border)
                    e.Handled = True
                End If
                Exit Sub
            End If

            ' ---------------------------------------------------------
            ' DATA ROW LOGIC
            ' ---------------------------------------------------------
            Dim dgv = DirectCast(sender, DataGridView)
            Dim colNameCell As String = dgv.Columns(e.ColumnIndex).Name

            Dim playerName As String = dgv.Rows(e.RowIndex).Cells("Player").Value?.ToString().Trim()
            If String.IsNullOrEmpty(playerName) Then Exit Sub

            Dim playerKey As String = playerName.ToUpper()
            Dim rowInfo As EditScoresEngine.RowScoreInfo = Nothing
            EditScoresEngine.scoreCache.TryGetValue(playerKey, rowInfo)

            ' ---------------------------------------------------------
            ' 2.5 PLAYER CELL - paint aqua for subs
            ' ---------------------------------------------------------
            If colNameCell = "Player" Then
                If rowInfo IsNot Nothing AndAlso rowInfo.IsSub Then
                    e.PaintBackground(e.ClipBounds, True)
                    Using b As New SolidBrush(Color.Aqua)
                        e.Graphics.FillRectangle(b, e.CellBounds)
                    End Using
                    e.Paint(e.CellBounds, DataGridViewPaintParts.Border)
                    Using f As New Font("Segoe UI", 9, FontStyle.Regular)
                        TextRenderer.DrawText(e.Graphics, playerName, f, e.CellBounds, Color.Black,
                        TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
                    End Using
                    e.Handled = True
                End If
                Exit Sub
            End If

            ' ---------------------------------------------------------
            ' 3. CACHED CELL LOGIC - hole columns only
            ' ---------------------------------------------------------
            Dim holeCell As Integer
            Dim isOtherWeekCell As Boolean = False

            If colNameCell.StartsWith("O") AndAlso
           Integer.TryParse(colNameCell.Substring(1), holeCell) AndAlso
           ctx.IsPostSeason Then
                isOtherWeekCell = True
            ElseIf Not Integer.TryParse(colNameCell, holeCell) Then
                Exit Sub
            End If

            If rowInfo Is Nothing Then Exit Sub

            Dim info As EditScoresEngine.HoleInfo = Nothing
            If isOtherWeekCell Then
                If Not rowInfo.Holes.TryGetValue(holeCell + 100, info) Then Exit Sub
            Else
                If Not rowInfo.Holes.TryGetValue(holeCell, info) Then Exit Sub
            End If

            e.Handled = True
            Dim g = e.Graphics
            Dim cell = e.CellBounds

            e.PaintBackground(e.ClipBounds, True)

            If info.IsLowest Then
                Using b As New SolidBrush(Color.FromArgb(200, 220, 255, 200))
                    g.FillRectangle(b, cell)
                End Using
            End If

            If info.IsSkinWinner Then
                Using b As New SolidBrush(Color.Gold)
                    g.FillRectangle(b, cell)
                End Using
            ElseIf info.IsTie Then
                Using b As New SolidBrush(Color.LightSteelBlue)
                    g.FillRectangle(b, cell)
                End Using
            End If

            If info.IsCapped Then
                Using b As New SolidBrush(Color.DarkMagenta)
                    g.FillRectangle(b, cell)
                End Using
            End If

            If info.StrokeCount > 0 Then
                Dim dotDiameter As Integer = 4
                Dim spacing As Integer = 2
                Dim totalWidth As Integer = info.StrokeCount * dotDiameter + (info.StrokeCount - 1) * spacing
                Dim startX As Integer = cell.Left + (cell.Width - totalWidth) \ 2
                For i As Integer = 0 To info.StrokeCount - 1
                    Dim x = startX + i * (dotDiameter + spacing)
                    g.FillEllipse(Brushes.Green, x, cell.Top + 2, dotDiameter, dotDiameter)
                Next
            End If

            If info.IsBirdie Or info.IsEagle Then
                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                Dim scoreArea As New Rectangle(cell.X, cell.Y + 4, cell.Width, cell.Height - 4)
                Dim diameter As Integer = 18
                Dim x As Integer = scoreArea.Left + (scoreArea.Width - diameter) \ 2
                Dim y As Integer = scoreArea.Top + (scoreArea.Height - diameter) \ 2 - 2
                Using p As New Pen(Color.Red, 1.5)
                    p.Alignment = Drawing2D.PenAlignment.Center
                    g.DrawEllipse(p, x, y, diameter, diameter)
                    If info.IsEagle Then g.DrawEllipse(p, x + 3, y + 3, diameter - 6, diameter - 6)
                End Using
            End If

            Dim scoreText As String = If(e.Value IsNot Nothing, e.Value.ToString(), "")
            If scoreText <> "" Then
                Dim textColor As Color = If(info.IsUnderPar, Color.Red, Color.Black)
                Using f As New Font("Segoe UI", 9, FontStyle.Regular)
                    Dim textRect As New Rectangle(cell.X, cell.Y + 4, cell.Width, cell.Height - 4)
                    TextRenderer.DrawText(g, scoreText, f, textRect, textColor,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
                End Using
            End If

            If dgv.CurrentCell IsNot Nothing AndAlso
           dgv.CurrentCell.RowIndex = e.RowIndex AndAlso
           dgv.CurrentCell.ColumnIndex = e.ColumnIndex Then
                ControlPaint.DrawFocusRectangle(g, cell)
            End If

            e.Paint(e.CellBounds, DataGridViewPaintParts.Border)

        Catch ex As Exception
            ' swallow
        End Try
    End Sub
    Private Sub dgScores_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        Try
            Dim dgv = DirectCast(sender, DataGridView)
            If e.RowIndex < 0 Then Exit Sub
            If dgv.Columns(e.ColumnIndex).Name <> "Gross" AndAlso
           dgv.Columns(e.ColumnIndex).Name <> "Net" Then Exit Sub
            If e.Value Is Nothing OrElse Not IsNumeric(e.Value) Then Exit Sub

            ' Get par total for this 9
            Dim wk As String = If(ctx.sFrontBack = "Back", "Out", "In")
            Dim par As Integer = CInt(ctx.thiscourse(wk))

            If CInt(e.Value) < par Then
                e.CellStyle.ForeColor = Color.Red
                e.CellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            Else
                e.CellStyle.ForeColor = Color.Black
                e.CellStyle.Font = New Font("Segoe UI", 9, FontStyle.Regular)
            End If
        Catch ex As Exception
            ' swallow
        End Try
    End Sub
    Private Sub btnPrintScores_Click(sender As Object, e As EventArgs) Handles btnPrintScores.Click
        Dim path As String = GetReportPdfPath($"Scores_{ctx.ActiveDate}.pdf")
        If dgScores IsNot Nothing Then
            If Not EditScoresEngine.IsReady() Then
                EditScoresEngine.Initialize(dgScores, Me.ohelper)
            End If
            EditScoresEngine.BuildScoreCache()
            dgScores.Refresh()
        End If
        GeneratePDF(dgScores, "Scores Report", path)
    End Sub
    Private Sub btnPrintMatches_Click(sender As Object, e As EventArgs) Handles btnPrintMatches.Click
        Dim tp = ctx.tabControl.TabPages("Matches")
        If tp Is Nothing Then Exit Sub
        Dim dgv = DirectCast(tp.Controls("dgMatches"), DataGridView)
        If dgv Is Nothing Then Exit Sub
        Dim path As String = GetReportPdfPath($"Matches_{ctx.ActiveDate}.pdf")
        GeneratePDF(dgv, "Matches Report", path)
    End Sub
    Private Sub btnPrintStandings_Click(sender As Object, e As EventArgs) Handles btnPrintStandings.Click
        Dim tp = ctx.tabControl.TabPages("Standings")
        If tp Is Nothing Then Exit Sub

        Dim splitSeason As Boolean = ctx.rLeagueParmrow("SplitSeason").ToString() = "Y"

        If splitSeason Then
            ' Print both halves
            Dim dgv1 = DirectCast(tp.Controls("dgStandings1"), DataGridView)
            Dim dgv2 = DirectCast(tp.Controls("dgStandings2"), DataGridView)
            If dgv1 IsNot Nothing Then
                Dim path1 As String = GetReportPdfPath($"Standings1st_{ctx.ActiveDate}.pdf")
                GeneratePDF(dgv1, "1st Half Standings", path1)
            End If
            If dgv2 IsNot Nothing Then
                Dim path2 As String = GetReportPdfPath($"Standings2nd_{ctx.ActiveDate}.pdf")
                GeneratePDF(dgv2, "2nd Half Standings", path2)
            End If
        Else
            Dim dgv = DirectCast(tp.Controls("dgStandings"), DataGridView)
            If dgv Is Nothing Then Exit Sub
            Dim path As String = GetReportPdfPath($"Standings_{ctx.ActiveDate}.pdf")
            GeneratePDF(dgv, "Season Standings", path)
        End If
    End Sub

    Private Function GetReportPdfPath(fileName As String) As String
        Dim reportFileName As String = fileName
        Dim activeDateSuffix As String = "_" & ctx.ActiveDate
        Dim fileStem As String = System.IO.Path.GetFileNameWithoutExtension(fileName)
        Dim extension As String = System.IO.Path.GetExtension(fileName)

        If fileStem.EndsWith(activeDateSuffix, StringComparison.OrdinalIgnoreCase) Then
            reportFileName = fileStem.Substring(0, fileStem.Length - activeDateSuffix.Length) & extension
        End If

        Return System.IO.Path.Combine(ctx.ReportPath, ctx.SeasonYear, ctx.ActiveDate.Substring(4, 4), reportFileName)
    End Function

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Set focus to the TextBox control
        tbPlayer.Focus()
    End Sub
    Private Sub ScoreCard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ohelper.dt = ohelper.sqlitedaFromSql(connection, "", $"Select * FROM Payments  WHERE League = '{ctx.SafeLeagueName}' AND Date = '{ctx.ActiveDate}' AND Desc = 'CTP' AND Detail LIKE '#%' ")
        If ohelper.dt.Rows.Count < 2 Then
            If Not Debugger.IsAttached Then
                If MessageBox.Show($"Only {ohelper.dt.Rows.Count} have been awarded, Press <Yes> to Close Or <No>", "Not Enough CTPS Awarded", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) = DialogResult.No Then
                    e.Cancel = True
                End If
            End If
        End If

    End Sub
    Dim dgv As DataGridView
    Sub addGB2DGV(tabpage As TabPage, dgv As DataGridView)
#Region "Add GroupBox And Labels"
        ' Define your label text + color pairs
        Dim items As (Text As String, Color As Color)() =
{
    ("No Show", Color.Pink),
    ("Sub", Color.Aqua),
    ("In Skins", Color.LightGreen),
    ("Under Par", Color.Red)
}
        Dim x As Integer = 10   ' starting X position
        Dim y As Integer = 20   ' vertical position inside the groupbox

        Dim gb As New GroupBox()
        For Each item In items
            Dim lbl As New Label()
            lbl.AutoSize = True
            lbl.Text = item.Text
            lbl.BackColor = item.Color
            lbl.Location = New Point(x, y)
            gb.Controls.Add(lbl)
            x += lbl.Width + 15   ' move right for next label
        Next

        ' Add Clear Scores button after the legend labels
        Dim btnClearScores As New Button()
        btnClearScores.Name = "btnClearScores"
        btnClearScores.Text = "Clear Scores"
        btnClearScores.Size = New Size(180, 23)
        btnClearScores.Location = New Point(x + 20, y - 3)
        AddHandler btnClearScores.Click, Sub(s As Object, ev As EventArgs)

                                             If dgv Is Nothing OrElse dgv.CurrentRow Is Nothing Then
                                                 MessageBox.Show("Please select a player row first.")
                                                 Return
                                             End If

                                             EditScoresEngine.ClearPlayerScores(dgv, dgv.CurrentRow)

                                         End Sub
        AddHandler dgv.SelectionChanged, Sub(s As Object, ev As EventArgs)

                                             If _suppressEvents Then Exit Sub   ' THIS IS THE FIX

                                             Dim btn = TryCast(gb.Controls("btnClearScores"), Button)
                                             If btn Is Nothing Then Exit Sub

                                             If dgv.CurrentRow IsNot Nothing Then
                                                 Dim playerName = dgv.CurrentRow.Cells("Player").Value?.ToString()

                                                 If Not String.IsNullOrEmpty(playerName) Then
                                                     btn.Text = $"Clear Scores: {playerName}"
                                                 Else
                                                     btn.Text = "Clear Scores"
                                                 End If
                                             Else
                                                 btn.Text = "Clear Scores"
                                             End If

                                         End Sub

        gb.Controls.Add(btnClearScores)

        gb.Name = "gbScoresInfo"
        gb.Text = "Scores Information"
        gb.Size = New Size(725, 50)
        gb.Location = New Point(10, 10)
        tabpage.Controls.Add(gb)
        '--- Create the DataGridView ---
        ' Add the grid BEFORE positioning it
        tabpage.Controls.Add(dgv)
        ' Now position it under the groupbox
        dgv.Location = New Point(gb.Left, gb.Bottom + 5)
#End Region

    End Sub
    Sub AddCB2DG(f As Form, hole As String)
        ' Create a new ComboBox
        Dim cbDynamic As New ComboBox()
        Dim ctl As Control = f.Controls("cbplayers")   ' by Name

        If ctl IsNot Nothing Then
            Dim cb As ComboBox = TryCast(ctl, ComboBox)
            If cb IsNot Nothing Then
                ' Set basic properties
                cbDynamic.Name = "Holes"
                cbDynamic.Location = New Point(200, cb.Location.Y)   ' X,Y coordinates on the form
                cbDynamic.Size = New Size(50, 25)

                ' Add items from your list
                cbDynamic.Items.Add(hole) '.AddRange(sPar3s.ToArray())

                ' Optional: set default selection
                If cbDynamic.Items.Count > 0 Then
                    cbDynamic.SelectedIndex = 0
                End If

                ' Create the label
                Dim lblHole As New Label()
                lblHole.Name = "lblHole"
                lblHole.Text = "Hole"
                lblHole.AutoSize = True

                ' Center the label above the ComboBox
                lblHole.Location = New Point(
                cbDynamic.Left + (cbDynamic.Width \ 2) - (lblHole.PreferredWidth \ 2),
                cbDynamic.Top - lblHole.PreferredHeight - 5
            )

                ' Add both controls to the form
                f.Controls.Add(cbDynamic)
                f.Controls.Add(lblHole)
            End If
        End If
    End Sub
    Public dvCTPs As New DataView
    Public Sub UpdateCTPsFromScores()
        dgv.Height = 100
        dgv.Width = 750
        'use prize money to determine how much to award
        dvCTPs = New DataView(dtScores)
        dvCTPs.RowFilter = "InCTPS = 'Y'"
        'dgv.AutoGenerateColumns = False
        dtCTP = ohelper.createDT("Hole,Player,Prior,Earned,Leftover,DatePaid")
        dtCTP.PrimaryKey = New DataColumn() {dtCTP.Columns("Hole")}
        For i = 1 To sPar3s.Count
            Dim nr As DataRow = dtCTP.NewRow()
            nr("Hole") = sPar3s(i - 1)
            dtCTP.Rows.Add(nr)
        Next
        dgv.Columns.Clear()
        dgv.DataSource = dtCTP
        ' Let the data decide the width
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv.AutoResizeColumns()
        dgv.Width = dgv.PreferredSize.Width + 5
        dgv.MinimumSize = New Size(0, 100)
        dgv.Refresh()
    End Sub
    Function AssignCTPWinners(holekey As String)
        Using f As New SelectPlayer(dvCTPs)
            AddCB2DG(f, holekey)
            f.Text = "CTP Winners"
            If f.ShowDialog() = DialogResult.OK Then
                ohelper.sPlayer = f.SelectedPlayer
            End If
        End Using
    End Function
    Function check4Winners() As String
        Dim addtnlRF As String = ""
        Dim ctpfld As String = "CTP"
        If ohelper.bPostSeason Then
            If sPar3s(0) < 10 Then
                ctpfld = $"F{ctpfld}"
            Else
                ctpfld = $"B{ctpfld}"
            End If
        End If
        'now check for winners of a ctp
        For i = 1 To sPar3s.Count
            If i = 1 Then
                addtnlRF = $" AND ({ctpfld}{i} Is Not Null"
            Else
                addtnlRF &= $" OR {ctpfld}{i} Is Not Null"
            End If
        Next
        addtnlRF &= $" )"
        dvCTPs.RowFilter &= addtnlRF
        If dvCTPs.Count > 0 Then
            Dim iptr As Integer = 1
            For Each row As DataRowView In dvCTPs
                'Update the winners amounts in dtscores
                Dim drfind = dtCTP.Rows.Find(sPar3s($"{iptr - 1}"))
                If drfind IsNot Nothing Then
                    drfind("Player") = row("Player")
                    drfind("Earned") = row($"{ctpfld}{iptr}")
                    drfind("DatePaid") = row("CPDP")
                Else
                    MsgBox("Error: Could not find CTP Hole " & row("Detail").ToString.Replace("#", ""))
                End If
                iptr += 1
            Next
            'dgv.Columns("Clear").Visible = True
        End If
        Return dvCTPs.Count
    End Function
    Public Sub UpdateMatchesFromScores()
        'dgv.DataSource = dtCTP
        ohelper.wk = ""

        'dgv.Columns.Clear()
        dgv.DataSource = dtScores
        ' Columns you want to move to the end
        Dim teamInfoCols = {"Team", "Team_Net", "Team_Points"}

        ' First pass: hide/show + width logic (your existing code)
        Dim smatchflds As String = "Match#,Team-40,Team_Net-40,Team_Points-60,Player-100,Net-40,Points-50,Opponent-100"
        Dim fields = smatchflds.Split(","c)

        For Each col As DataGridViewColumn In dgv.Columns
            col.Visible = False

            Dim match = fields.FirstOrDefault(Function(f)
                                                  Return f.Split("-"c)(0).Equals(col.Name, StringComparison.OrdinalIgnoreCase)
                                              End Function)

            If match IsNot Nothing Then
                col.Visible = True

                Dim dashIndex = match.IndexOf("-"c)
                If dashIndex > -1 Then
                    Dim widthText = match.Substring(dashIndex + 1)
                    Dim w As Integer
                    If Integer.TryParse(widthText, w) Then
                        col.Width = w
                    End If
                End If
            End If
        Next

        ' Second pass: move team info columns to the end
        'For Each colName In teamInfoCols
        '    If dgv.Columns.Contains(colName) Then
        '        dgv.Columns(colName).DisplayIndex = dgv.Columns.Count - 1
        '        dgv.Columns(colName).HeaderText = dgv.Columns(colName).HeaderText.Replace("Team_", "").Replace(" ", "")
        '    End If
        'Next

        ' Pre-filter the columns ONCE
        Dim pointCols As List(Of DataGridViewColumn) =
            dgv.Columns.Cast(Of DataGridViewColumn)().
                Where(Function(c) c.Name.IndexOf("points", StringComparison.OrdinalIgnoreCase) >= 0).
                ToList()

        For Each row As DataGridViewRow In dgv.Rows
            For Each col As DataGridViewColumn In pointCols

                Dim cell = row.Cells(col.Index)
                Dim valObj = cell.Value

                If valObj IsNot Nothing Then
                    Dim val As Decimal

                    ' Try to treat the value as a number
                    If Decimal.TryParse(valObj.ToString(), val) Then
                        If val = 1 Then
                            cell.Style.BackColor = Color.LightGreen
                        ElseIf val < 1 Then
                            cell.Style.BackColor = Color.Yellow
                        End If
                    End If
                End If

            Next
        Next
    End Sub
    Private Function IsMatchesPdfGrid(dgv As DataGridView) As Boolean
        If dgv Is Nothing Then Return False
        If String.Equals(dgv.Name, "dgMatches", StringComparison.OrdinalIgnoreCase) Then Return True
        If dgv.Parent IsNot Nothing AndAlso String.Equals(dgv.Parent.Name, "Matches", StringComparison.OrdinalIgnoreCase) Then Return True

        Return dgv.Columns.Contains("Points") AndAlso
               dgv.Columns.Contains("Team_Points") AndAlso
               Not dgv.Columns.Contains("1")
    End Function
    Public Sub GeneratePDF(dgv As DataGridView, title As String, filePath As String)
        Try
            PdfSharp.Fonts.GlobalFontSettings.FontResolver = New WindowsFontResolver()
            Dim doc As New PdfDocument()
            doc.Info.Title = title

            Dim page As PdfPage = doc.AddPage()
            page.Orientation = PdfSharp.PageOrientation.Landscape
            Dim gfx As XGraphics = XGraphics.FromPdfPage(page)

            ' Fonts
            Dim titleFont As New XFont("Arial", 14, XFontStyleEx.Bold)
            Dim headerFont As New XFont("Arial", 8, XFontStyleEx.Bold)
            Dim cellFont As New XFont("Arial", 8, XFontStyleEx.Regular)
            Dim dateFont As New XFont("Arial", 9, XFontStyleEx.Italic)

            ' Margins
            Dim marginLeft As Double = 20
            Dim marginTop As Double = 20
            Dim y As Double = marginTop

            ' Title
            gfx.DrawString(title, titleFont, XBrushes.Black, New XPoint(marginLeft, y))
            y += 20
            gfx.DrawString($"Date: {DateTime.ParseExact(ctx.ActiveDate, "yyyyMMdd", Nothing).ToString("MM/dd/yyyy")}   League: {ctx.sLeagueName}",
                   dateFont, XBrushes.Gray, New XPoint(marginLeft, y))
            y += 15

            ' Get visible columns
            Dim visibleCols As New List(Of DataGridViewColumn)
            For Each col As DataGridViewColumn In dgv.Columns
                If col.Visible Then visibleCols.Add(col)
            Next

            ' Calculate column widths
            Dim pageWidth As Double = page.Width.Point - (marginLeft * 2)
            Dim colWidth As Double = pageWidth / visibleCols.Count
            Dim rowHeight As Double = 16

            ' Draw header row
            Dim x As Double = marginLeft
            For Each col As DataGridViewColumn In visibleCols
                Dim rect As New XRect(x, y, colWidth, rowHeight)
                Dim holeNum As Integer
                If Integer.TryParse(col.Name, holeNum) Then
                    Dim displayHole As Integer = If(ctx.sFrontBack = "Back", holeNum + 9, holeNum)
                    Dim headerColor As XColor = If(ctx.sFrontBack = "Back",
                    XColor.FromArgb(34, 139, 34),
                    XColor.FromArgb(184, 134, 11))
                    Dim third As Double = rowHeight / 3

                    ' Top - hole number
                    Dim topRect As New XRect(x, y, colWidth, third)
                    gfx.DrawRectangle(New XSolidBrush(headerColor), topRect)
                    Dim sf As New XStringFormat()
                    sf.Alignment = XStringAlignment.Center
                    sf.LineAlignment = XLineAlignment.Center
                    gfx.DrawString(displayHole.ToString(), headerFont, XBrushes.White, topRect, sf)

                    ' Middle - par
                    Dim par As String = "?"
                    Try
                        par = ctx.thiscourse($"Hole{displayHole}").ToString()
                    Catch
                    End Try
                    Dim midColor As XColor = If(ctx.sFrontBack = "Back",
                    XColor.FromArgb(34, 100, 34),
                    XColor.FromArgb(150, 100, 10))
                    Dim midRect As New XRect(x, y + third, colWidth, third)
                    gfx.DrawRectangle(New XSolidBrush(midColor), midRect)
                    gfx.DrawString(par, headerFont, XBrushes.White, midRect, sf)

                    ' Bottom - stroke index
                    Dim strokeIndex As String = "?"
                    Try
                        Dim colKey As String = "H" & displayHole
                        If ctx.thiscourse IsNot Nothing AndAlso ctx.thiscourse.Table.Columns.Contains(colKey) Then
                            strokeIndex = ctx.thiscourse(colKey).ToString()
                        End If
                    Catch
                    End Try
                    Dim botRect As New XRect(x, y + (third * 2), colWidth, third)
                    gfx.DrawRectangle(New XSolidBrush(XColor.FromArgb(144, 238, 144)), botRect)
                    gfx.DrawString(strokeIndex, cellFont, XBrushes.Black, botRect, sf)
                Else
                    ' Regular column header
                    gfx.DrawRectangle(New XSolidBrush(XColor.FromArgb(70, 130, 180)), rect)
                    gfx.DrawRectangle(XPens.White, rect)
                    Dim sf As New XStringFormat()
                    sf.Alignment = XStringAlignment.Center
                    sf.LineAlignment = XLineAlignment.Center
                    gfx.DrawString(col.HeaderText, headerFont, XBrushes.White, rect, sf)
                End If
                x += colWidth
            Next
            y += rowHeight

            ' Draw rows
            For rowIdx As Integer = 0 To dgv.Rows.Count - 1
                If y + rowHeight > page.Height.Point - 20 Then
                    page = doc.AddPage()
                    page.Orientation = PdfSharp.PageOrientation.Landscape
                    gfx = XGraphics.FromPdfPage(page)
                    y = marginTop
                End If

                Dim row = dgv.Rows(rowIdx)
                x = marginLeft

                ' -- Totals detection: check Player cell value, not row index --
                Dim isTotals As Boolean = False
                If dgv.Columns.Contains("Player") Then
                    Dim playerVal As String = If(row.Cells("Player").Value IsNot Nothing,
                                             row.Cells("Player").Value.ToString().Trim(), "")
                    isTotals = (playerVal = "" OrElse playerVal.ToUpper().StartsWith("TOTAL"))
                End If

                ' Get scoreCache info for this player (Scores grid)
                Dim playerName As String = ""
                Dim rowInfo As EditScoresEngine.RowScoreInfo = Nothing
                If dgv.Columns.Contains("Player") Then
                    playerName = If(row.Cells("Player").Value IsNot Nothing,
                                row.Cells("Player").Value.ToString().Trim(), "")
                    EditScoresEngine.scoreCache.TryGetValue(playerName.ToUpper(), rowInfo)
                End If

                For Each col As DataGridViewColumn In visibleCols
                    Dim rect As New XRect(x, y, colWidth, rowHeight)
                    Dim cellVal As String = If(row.Cells(col.Name).Value IsNot Nothing,
                                           row.Cells(col.Name).Value.ToString(), "")
                    Dim sf As New XStringFormat()
                    sf.Alignment = XStringAlignment.Center
                    sf.LineAlignment = XLineAlignment.Center

                    ' -- Determine background color --
                    Dim bgColor As XColor = DataGridViewCellBackColor(row, col, rowIdx)

                    If isTotals Then
                        bgColor = XColor.FromArgb(211, 211, 211)

                    ElseIf col.Name = "Player" AndAlso rowInfo IsNot Nothing AndAlso rowInfo.IsSub Then
                        bgColor = XColor.FromArgb(0, 255, 255) ' Aqua - sub player

                    ElseIf IsMatchesPdfGrid(dgv) Then
                        ' -- Matches grid --

                        ' 1. 4-row blue grouping (base)
                        If (rowIdx \ 4) Mod 2 = 0 Then
                            bgColor = XColor.FromArgb(210, 230, 255) ' matchBlue
                        Else
                            bgColor = XColor.FromArgb(255, 255, 255)
                        End If

                        ' 2. Pink for Player/Opponent when no score
                        If col.Name = "Player" OrElse col.Name = "Opponent" Then
                            Dim netVal = row.Cells("Net").Value
                            Dim hasScore As Boolean = (netVal IsNot DBNull.Value AndAlso
                                                   Not String.IsNullOrWhiteSpace(netVal.ToString()))
                            If Not hasScore Then
                                bgColor = XColor.FromArgb(255, 192, 203) ' Pink
                            End If
                        End If

                        ' 3. Points / Team_Points override (always wins)
                        If col.Name = "Points" OrElse col.Name = "Team_Points" Then
                            Dim cellRaw = row.Cells(col.Name).Value
                            If cellRaw IsNot Nothing AndAlso IsNumeric(cellRaw) Then
                                Dim val As Double = Convert.ToDouble(cellRaw)
                                If val = 1 Then
                                    bgColor = XColor.FromArgb(144, 238, 144) ' LightGreen
                                ElseIf val = 0.5 Then
                                    bgColor = XColor.FromArgb(255, 255, 0)   ' Yellow
                                End If
                            End If
                        End If

                    Else
                        ' -- Scores grid --
                        Dim holeNum As Integer
                        If Integer.TryParse(col.Name, holeNum) AndAlso rowInfo IsNot Nothing Then
                            Dim info As EditScoresEngine.HoleInfo = Nothing
                            If rowInfo.Holes.TryGetValue(holeNum, info) Then
                                If info.IsCapped Then
                                    bgColor = XColor.FromArgb(139, 0, 139)
                                ElseIf info.IsSkinWinner Then
                                    bgColor = XColor.FromArgb(255, 215, 0)
                                ElseIf info.IsTie Then
                                    bgColor = XColor.FromArgb(176, 196, 222)
                                ElseIf info.IsLowest Then
                                    bgColor = XColor.FromArgb(200, 220, 255, 200)
                                End If
                            End If
                        End If
                    End If

                    ' Draw cell background and border
                    gfx.DrawRectangle(New XSolidBrush(bgColor), rect)
                    gfx.DrawRectangle(XPens.LightGray, rect)

                    ' -- Blank out 999 in Net / Team_Net --
                    If (col.Name = "Net" OrElse col.Name = "Team_Net") AndAlso
                   IsNumeric(cellVal) AndAlso CInt(cellVal) = 999 Then
                        cellVal = ""
                    End If

                    ' -- Draw cell content --
                    Dim holeNum2 As Integer
                    If Integer.TryParse(col.Name, holeNum2) AndAlso rowInfo IsNot Nothing Then
                        Dim info As EditScoresEngine.HoleInfo = Nothing
                        If rowInfo.Holes.TryGetValue(holeNum2, info) AndAlso cellVal <> "" Then
                            ' Stroke dots
                            If info.StrokeCount > 0 Then
                                Dim dotDiameter As Double = 3
                                Dim spacing As Double = 2
                                Dim totalDotWidth As Double = info.StrokeCount * dotDiameter + (info.StrokeCount - 1) * spacing
                                Dim startDotX As Double = rect.X + (rect.Width - totalDotWidth) / 2
                                For i As Integer = 0 To info.StrokeCount - 1
                                    Dim dotX As Double = startDotX + i * (dotDiameter + spacing)
                                    gfx.DrawEllipse(XBrushes.Green,
                                    New XRect(dotX, rect.Y + 2, dotDiameter, dotDiameter))
                                Next
                            End If

                            ' Birdie/eagle circle
                            If info.IsBirdie OrElse info.IsEagle Then
                                Dim diameter As Double = 12
                                Dim circleX As Double = rect.X + (rect.Width - diameter) / 2
                                Dim circleY As Double = rect.Y + (rect.Height - diameter) / 2
                                Dim circlePen As New XPen(XColors.Red, 1)
                                gfx.DrawEllipse(circlePen, New XRect(circleX, circleY, diameter, diameter))
                                If info.IsEagle Then
                                    gfx.DrawEllipse(circlePen,
                                    New XRect(circleX + 2, circleY + 2, diameter - 4, diameter - 4))
                                End If
                            End If

                            ' Score text
                            Dim textColor As XColor = If(info.IsUnderPar, XColors.Red, XColors.Black)
                            gfx.DrawString(cellVal, cellFont, New XSolidBrush(textColor), rect, sf)
                        End If
                    Else
                        ' Non-hole column
                        Dim font As XFont = If(isTotals, headerFont, cellFont)
                        gfx.DrawString(cellVal, font, XBrushes.Black, rect, sf)
                    End If

                    x += colWidth
                Next
                y += rowHeight
            Next

            Dim dirPath As String = System.IO.Path.GetDirectoryName(filePath)
            If Not System.IO.Directory.Exists(dirPath) Then
                System.IO.Directory.CreateDirectory(dirPath)
            End If

            doc.Save(filePath)
            Process.Start(New ProcessStartInfo(filePath) With {.UseShellExecute = True})
            LOGIT($"GeneratePDF: {filePath}")

        Catch ex As Exception
            LOGIT($"GeneratePDF Error: {ex.Message}")
            MsgBox($"PDF generation failed: {ex.Message}", MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Function DataGridViewCellBackColor(row As DataGridViewRow,
                                               col As DataGridViewColumn,
                                               rowIdx As Integer) As XColor
        Dim color As Color = row.Cells(col.Name).Style.BackColor

        If color.IsEmpty Then color = row.DefaultCellStyle.BackColor
        If color.IsEmpty AndAlso rowIdx Mod 2 <> 0 Then color = row.DataGridView.AlternatingRowsDefaultCellStyle.BackColor
        If color.IsEmpty Then color = row.DataGridView.RowsDefaultCellStyle.BackColor
        If color.IsEmpty Then color = row.DataGridView.DefaultCellStyle.BackColor
        If color.IsEmpty Then color = Color.White

        Return XColor.FromArgb(color.R, color.G, color.B)
    End Function

    Public Class WindowsFontResolver
        Implements PdfSharp.Fonts.IFontResolver

        Public Function GetFont(faceName As String) As Byte() Implements PdfSharp.Fonts.IFontResolver.GetFont
            Dim fontsFolder As String = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), faceName)
            If System.IO.File.Exists(fontsFolder) Then
                Return System.IO.File.ReadAllBytes(fontsFolder)
            End If
            ' Fallback to Arial
            Dim arial As String = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf")
            Return System.IO.File.ReadAllBytes(arial)
        End Function

        Public Function ResolveTypeface(familyName As String, isBold As Boolean, isItalic As Boolean) As PdfSharp.Fonts.FontResolverInfo Implements PdfSharp.Fonts.IFontResolver.ResolveTypeface
            Dim fileName As String = familyName.ToLower().Replace(" ", "") & ".ttf"
            If isBold AndAlso isItalic Then
                fileName = familyName.ToLower().Replace(" ", "") & "bi.ttf"
            ElseIf isBold Then
                fileName = familyName.ToLower().Replace(" ", "") & "bd.ttf"
            ElseIf isItalic Then
                fileName = familyName.ToLower().Replace(" ", "") & "i.ttf"
            End If
            Return New PdfSharp.Fonts.FontResolverInfo(fileName)
        End Function
    End Class
    Private Function GetTabControlFromForm(ctrl As Control) As TabControl

        Dim form = ctrl.FindForm()
        If form Is Nothing Then Return Nothing

        Return form.Controls.OfType(Of TabControl)().FirstOrDefault()

    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FindPlayerFromForm(tbPlayer, tbPlayer.Text)
    End Sub
    Private Sub tbPlayer_KeyDown(sender As Object, e As KeyEventArgs) Handles tbPlayer.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            FindPlayerFromForm(tbPlayer, tbPlayer.Text)
        End If
    End Sub
    Public Sub FindPlayerFromForm(ctrl As Control, playerName As String)

        If String.IsNullOrWhiteSpace(playerName) Then Exit Sub

        Dim tc = GetTabControlFromForm(ctrl)
        If tc Is Nothing Then
            MessageBox.Show("No TabControl found.")
            Exit Sub
        End If

        Dim tp = tc.SelectedTab
        If tp Is Nothing Then Exit Sub

        For Each dgv As DataGridView In tp.Controls.OfType(Of DataGridView)()

            If Not dgv.Columns.Contains("Player") Then Continue For

            For Each row As DataGridViewRow In dgv.Rows

                Dim val = row.Cells("Player").Value?.ToString()

                ' UPDATED LINE (partial + case-insensitive)
                If Not String.IsNullOrEmpty(val) AndAlso
               val.IndexOf(playerName, StringComparison.OrdinalIgnoreCase) >= 0 Then

                    dgv.ClearSelection()
                    row.Selected = True
                    dgv.CurrentCell = row.Cells("Player")
                    dgv.FirstDisplayedScrollingRowIndex = row.Index

                    Exit Sub
                End If
            Next
        Next

        MessageBox.Show($"Player '{playerName}' not found on this tab.", "Quick Find")

    End Sub
    Public Sub FindPlayerInActiveTab(tc As TabControl, playerName As String)

        If String.IsNullOrWhiteSpace(playerName) Then Exit Sub

        Dim tp = tc.SelectedTab
        If tp Is Nothing Then Exit Sub

        ' check ALL grids on active tab
        For Each dgv As DataGridView In tp.Controls.OfType(Of DataGridView)()

            If Not dgv.Columns.Contains("Player") Then Continue For

            For Each row As DataGridViewRow In dgv.Rows

                Dim val = row.Cells("Player").Value?.ToString()

                If Not String.IsNullOrEmpty(val) AndAlso
               val.Trim().Equals(playerName.Trim(), StringComparison.OrdinalIgnoreCase) Then

                    dgv.ClearSelection()
                    row.Selected = True
                    dgv.CurrentCell = row.Cells("Player")

                    ' scroll into view
                    dgv.FirstDisplayedScrollingRowIndex = row.Index

                    Exit Sub
                End If
            Next
        Next

        MessageBox.Show($"Player '{playerName}' not found on this tab.", "Quick Find")

    End Sub
End Class

#Region "Matches Calc"
Public Class matchforlist
    Property matchPtr As Integer
    Property teamstuff As List(Of TeamScores)
End Class
Public Class TeamScores
    Property Home As Boolean
    Property Net As Integer
    Property Points As Decimal
    Property playerstuff As List(Of PlayerScores)
End Class
Public Class PlayerScores
    Property Team As String
    Property Name As String
    Property Net As String
    Property Points As Decimal
End Class
Public Class TeamMatch
    ' Optional property to hold matches
    Public Property MatchList As List(Of matchforlist)
    Public ohelper As Helper
    ' Parameterless constructor
    Public Sub New()
        ohelper = Main.oHelper

    End Sub
    Private Function calcPoints(Home As Integer, Away As Integer) As Decimal
        Select Case True
            Case Home < Away : Return 1D
            Case Home > Away : Return 0D
            Case Else : Return 0.5D
        End Select
    End Function
End Class
#End Region

