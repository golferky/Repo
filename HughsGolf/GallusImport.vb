Imports System.Data.SQLite
Partial Class ScoreCard

    Private Sub btnGallus_Click(sender As Object, e As EventArgs)
        Try
            ' Step 1: Get URL
            Dim url As String = InputBox("Paste Gallus scorecard URL:", "Import from Gallus")
            If String.IsNullOrEmpty(url) Then Exit Sub

            ' Step 2: Parse Gallus data
            Dim playerDataList = ohelper.ParseGallusScorecard(url)
            If playerDataList.Count = 0 Then
                MessageBox.Show("Could not parse scorecard.", "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' Step 2a: Check for duplicate URL
            Dim existingImport As Object = ohelper.SQLiteExecuteScalar(
            "SELECT COUNT(*) FROM GallusImport WHERE League=@League AND Date=@Date AND URL=@URL",
            New Dictionary(Of String, Object) From {
                {"@League", ctx.sLeagueName},
                {"@Date", ctx.ActiveDate},
                {"@URL", url}
            })

            If existingImport IsNot Nothing AndAlso CInt(existingImport) > 0 Then
                Dim scoresExist As Object = ohelper.SQLiteExecuteScalar(
                "SELECT COUNT(*) FROM Scores s
                 JOIN GallusImport g ON g.League = s.League
                     AND g.Date = s.Date
                     AND g.ResolvedName = s.Player
                 WHERE g.League=@League AND g.Date=@Date AND g.URL=@URL
                 AND s.Gross IS NOT NULL AND s.Gross > 0",
                New Dictionary(Of String, Object) From {
                    {"@League", ctx.sLeagueName},
                    {"@Date", ctx.ActiveDate},
                    {"@URL", url}
                })

                If scoresExist IsNot Nothing AndAlso CInt(scoresExist) > 0 Then
                    Dim mbr = MessageBox.Show(
                    "This scorecard has already been imported and scores exist." & vbCrLf &
                    "Do you want to import again?",
                    "Duplicate Import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    If mbr = DialogResult.No Then Exit Sub
                Else
                    LOGIT("Gallus: URL exists in GallusImport but scores missing — proceeding with reimport")
                End If
            End If

            ' Step 3: Front/Back from scores grid context
            Dim isPlayingFront As Boolean = (ctx.sFrontBack = "Front")
            LOGIT($"Gallus Import: FrontBack={ctx.sFrontBack} players={playerDataList.Count}")

            ' Step 3a: Fix score arrays if back 9 scores landed in front9Scores
            For Each pd In playerDataList
                Dim front9 = DirectCast(pd("Front9Scores"), Integer())
                Dim back9 = DirectCast(pd("Back9Scores"), Integer())
                Dim frontHasData As Boolean = front9.Any(Function(score) score > 0)
                Dim backHasData As Boolean = back9.Any(Function(score) score > 0)
                If Not isPlayingFront AndAlso frontHasData AndAlso Not backHasData Then
                    pd("Back9Scores") = front9
                    pd("Front9Scores") = New Integer(8) {}
                    LOGIT($"Gallus: swapped scores to back9 for {pd("PlayerName")}")
                End If
            Next

            ' Step 4: Resolution

            ' Pass 1 — Full name match only
            Dim matchedGallus As New List(Of String)
            Dim matchedGrid As New List(Of String)

            For Each pd In playerDataList
                Dim gallusName As String = pd("PlayerName").ToString()
                Dim subName As String = ResolveProperPlayerName(gallusName.ToLower())
                pd("SubName") = subName
                pd("OrigName") = ""
                pd("Resolved") = False

                For Each r As DataGridViewRow In dgScores.Rows
                    If r.IsNewRow Then Continue For
                    Dim gridPlayer As String = r.Cells("Player").Value?.ToString()
                    If String.IsNullOrEmpty(gridPlayer) Then Continue For
                    If gridPlayer.Equals(subName, StringComparison.OrdinalIgnoreCase) Then
                        pd("Resolved") = True
                        pd("TargetPlayer") = gridPlayer
                        matchedGallus.Add(subName)
                        matchedGrid.Add(gridPlayer)
                        LOGIT($"Pass1: matched {subName}")
                        Exit For
                    End If
                Next
            Next

            ' Pass 2 — Find group from matched players
            Dim groupNum As Integer = 0
            If matchedGrid.Count > 0 Then
                Dim inClause = String.Join(",", matchedGrid.Select(Function(n) $"'{n.Replace("'", "''")}'"))
                Using cmd As New SQLiteCommand($"
                SELECT [Group] FROM Teams
                WHERE League=@League AND Year=@Year
                AND Player IN ({inClause})
                GROUP BY [Group]
                ORDER BY COUNT(*) DESC
                LIMIT 1", ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then groupNum = CInt(result)
                End Using
            End If
            LOGIT($"Gallus: resolved group={groupNum}")

            ' Pass 3 — Find missing group members
            Dim missingGroupMembers As New List(Of String)
            If groupNum > 0 Then
                Using cmd As New SQLiteCommand("
                SELECT Player FROM Teams
                WHERE League=@League AND Year=@Year AND [Group]=@Group
                ORDER BY Team, Grade", ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                    cmd.Parameters.AddWithValue("@Group", groupNum)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Using dr As SQLiteDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim member As String = dr("Player").ToString()
                            If Not matchedGrid.Any(Function(m) m.Equals(member, StringComparison.OrdinalIgnoreCase)) Then
                                missingGroupMembers.Add(member)
                                LOGIT($"Gallus: missing group member {member}")
                            End If
                        End While
                    End Using
                End Using
            End If

            ' Pass 4 — Find unmatched Gallus players
            Dim unmatchedGallus = playerDataList.Where(Function(pd) Not CBool(pd("Resolved"))).ToList()

            ' Pass 5 — Prompt for each missing group member vs unmatched Gallus player
            For i As Integer = 0 To Math.Min(missingGroupMembers.Count, unmatchedGallus.Count) - 1
                Dim missing As String = missingGroupMembers(i)
                Dim pd = unmatchedGallus(i)
                Dim subName As String = pd("SubName").ToString()

                Dim mbr = MessageBox.Show(
                $"'{subName}' is not in the scorecard as a regular player.{vbCrLf}Is '{subName}' subbing for '{missing}'?",
                "Confirm Sub",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

                If mbr = DialogResult.Yes Then
                    pd("Resolved") = True
                    pd("OrigName") = missing
                    pd("TargetPlayer") = missing
                    LOGIT($"Gallus: {subName} confirmed subbing for {missing}")
                Else
                    Dim gridPlayers As New List(Of String)
                    For Each r As DataGridViewRow In dgScores.Rows
                        If r.IsNewRow Then Continue For
                        Dim gp = r.Cells("Player").Value?.ToString()
                        If Not String.IsNullOrEmpty(gp) Then gridPlayers.Add(gp)
                    Next
                    Dim picked = ShowPickerDialog(
                    $"Who is '{subName}' subbing for?",
                    "Select Player",
                    gridPlayers,
                    "Skip - No Show")
                    If Not String.IsNullOrEmpty(picked) Then
                        pd("Resolved") = True
                        pd("OrigName") = picked
                        pd("TargetPlayer") = picked
                        LOGIT($"Gallus: {subName} manually mapped to {picked}")
                    Else
                        LOGIT($"Gallus: {subName} skipped - treated as no-show")
                    End If
                End If
            Next

            ' Step 5: Import resolved players
            Dim imported As Integer = 0
            Dim notFound As New List(Of String)

            For Each pd In playerDataList
                If Not CBool(pd("Resolved")) Then
                    notFound.Add(pd("SubName").ToString())
                    Continue For
                End If

                Dim subName As String = pd("SubName").ToString()
                Dim origName As String = pd("OrigName").ToString()
                Dim targetName As String = pd("TargetPlayer").ToString()

                ' Find target row
                Dim targetRow As DataGridViewRow = FindGridRowByFullName(targetName)
                If targetRow Is Nothing Then
                    notFound.Add(subName)
                    Continue For
                End If

                ' Register sub if needed
                If Not String.IsNullOrEmpty(origName) AndAlso
               Not origName.Equals(subName, StringComparison.OrdinalIgnoreCase) Then
                    Dim nameParts = subName.Split(" "c)
                    Dim firstName As String = If(nameParts.Length > 1, nameParts(0), "")
                    Dim lastName As String = nameParts.Last()
                    RegisterSub(subName, origName, firstName, lastName)
                End If

                ' Get scores
                Dim scores() As Integer = If(isPlayingFront,
                DirectCast(pd("Front9Scores"), Integer()),
                DirectCast(pd("Back9Scores"), Integer()))

                ' Fill grid
                EditScoresEngine._suppressEvents = True
                targetRow.Cells("Player").Value = subName
                For i As Integer = 1 To 9
                    If dgScores.Columns.Contains(i.ToString()) Then
                        targetRow.Cells(i.ToString()).Value = If(scores(i - 1) > 0, scores(i - 1), Nothing)
                    End If
                Next
                EditScoresEngine._suppressEvents = False
                EditScoresEngine.UpsertScore(targetRow, "1", scores(0))

                ' Save to GallusImport
                SaveToGallusImport(url, pd("PlayerName").ToString(), subName, isPlayingFront, pd)

                imported += 1
                LOGIT($"Gallus: imported {subName}")
            Next

            FinalizeImportedScores(dgScores, playerDataList)
            LoadAllTabs()

            Dim msg As String = $"Imported {imported} of {playerDataList.Count} players."
            If notFound.Count > 0 Then
                msg &= vbCrLf & "Not imported:"
                For Each n In notFound
                    msg &= vbCrLf & $"  • {n}"
                Next
            End If
            MessageBox.Show(msg, "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            LOGIT($"btnGallus_Click Error: {ex.Message}")
            MessageBox.Show("Import error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' --- HELPER: Register sub in DB and update Matches ---
    Private Sub RegisterSub(subName As String, origName As String, firstName As String, lastName As String)
        Try
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            ' Check if player already exists with different casing
            Dim existingName As Object = Nothing
            Using cmd As New SQLiteCommand("SELECT Player FROM Players WHERE LOWER(Player) = LOWER(@Player) LIMIT 1", ctx.Conn)
                cmd.Parameters.AddWithValue("@Player", subName)
                existingName = cmd.ExecuteScalar()
            End Using

            ' Only insert if truly new
            If existingName Is Nothing Then
                Using cmd As New SQLiteCommand("
        INSERT INTO Players (Player, FirstName, LastName)
        VALUES (@Player, @First, @Last)", ctx.Conn)
                    cmd.Parameters.AddWithValue("@Player", subName)
                    cmd.Parameters.AddWithValue("@First", firstName)
                    cmd.Parameters.AddWithValue("@Last", lastName)
                    cmd.ExecuteNonQuery()
                End Using
            Else
                ' Use the existing properly cased name
                subName = existingName.ToString()
            End If

            Dim nextId As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID),0)+1 FROM Subs"))

            Using cmd As New SQLiteCommand("DELETE FROM Subs WHERE League=@League AND Player=@Sub AND Date=@Date", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Sub", subName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.ExecuteNonQuery()
            End Using

            Using cmd As New SQLiteCommand("
            INSERT INTO Subs (ID, League, Player, Date, Team, Grade)
            SELECT @ID, @League, @Sub, @Date, Team, Grade FROM Teams
            WHERE League=@League AND Player=@Orig COLLATE NOCASE AND Year=@Year", ctx.Conn)
                cmd.Parameters.AddWithValue("@ID", nextId)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Sub", subName)
                cmd.Parameters.AddWithValue("@Orig", origName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                cmd.ExecuteNonQuery()
            End Using

            Using cmd As New SQLiteCommand("
            UPDATE Matches SET Player=@Sub 
            WHERE League=@League AND Date=@Date AND Player=@Orig", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Sub", subName)
                cmd.Parameters.AddWithValue("@Orig", origName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.ExecuteNonQuery()
            End Using

            Using cmd As New SQLiteCommand("
            UPDATE Matches SET Opponent=@Sub 
            WHERE League=@League AND Date=@Date AND Opponent=@Orig", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Sub", subName)
                cmd.Parameters.AddWithValue("@Orig", origName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.ExecuteNonQuery()
            End Using

            LOGIT($"RegisterSub: {subName} for {origName}")
        Catch ex As Exception
            LOGIT($"RegisterSub Error: {ex.Message}")
        End Try
    End Sub
    Private Function FindGridRowByFullName(name As String) As DataGridViewRow
        For Each r As DataGridViewRow In dgScores.Rows
            If r.IsNewRow Then Continue For
            Dim gridPlayer = r.Cells("Player").Value?.ToString()
            If String.IsNullOrEmpty(gridPlayer) Then Continue For
            If gridPlayer.Equals(name, StringComparison.OrdinalIgnoreCase) Then Return r
        Next
        Return Nothing
    End Function
    ' --- HELPER: Save parsed data to GallusImport table ---
    Private Sub SaveToGallusImport(url As String, gallusName As String, resolvedName As String,
                            playingFront As Boolean, pd As Dictionary(Of String, Object))
        Try
            Dim front9Scores() As Integer = DirectCast(pd("Front9Scores"), Integer())
            Dim back9Scores() As Integer = DirectCast(pd("Back9Scores"), Integer())
            Dim front9Par() As Integer = DirectCast(pd("Front9Par"), Integer())
            Dim back9Par() As Integer = DirectCast(pd("Back9Par"), Integer())
            Dim front9Hcp() As Integer = DirectCast(pd("Front9Hcp"), Integer())
            Dim back9Hcp() As Integer = DirectCast(pd("Back9Hcp"), Integer())

            ' Map to actual hole numbers H1-H18
            Dim h(18) As Integer
            Dim par(18) As Integer
            Dim hcp(18) As Integer

            For i As Integer = 0 To 8
                h(i + 1) = front9Scores(i)
                h(i + 10) = back9Scores(i)
                par(i + 1) = front9Par(i)
                par(i + 10) = back9Par(i)
                hcp(i + 1) = front9Hcp(i)
                hcp(i + 10) = back9Hcp(i)
            Next

            ' Calculate Out/In/Total gross
            Dim outGross As Integer = front9Scores.Sum()
            Dim inGross As Integer = back9Scores.Sum()
            Dim totalGross As Integer = outGross + inGross

            ' Build column and value lists
            Dim colList As New List(Of String)
            Dim paramList As New List(Of String)

            colList.AddRange({"League", "Date", "ImportDate", "URL", "Owner", "GallusName", "ResolvedName", "FrontBack"})
            paramList.AddRange({"@League", "@Date", "@ImportDate", "@URL", "@Owner", "@GallusName", "@ResolvedName", "@FrontBack"})

            For i As Integer = 1 To 18
                colList.Add($"H{i}") : paramList.Add($"@H{i}")
                colList.Add($"Par{i}") : paramList.Add($"@Par{i}")
                colList.Add($"Hcp{i}") : paramList.Add($"@Hcp{i}")
            Next

            colList.AddRange({"OutGross", "InGross", "TotalGross", "Note"})
            paramList.AddRange({"@OutGross", "@InGross", "@TotalGross", "@Note"})

            Dim sql As String = $"INSERT INTO GallusImport ({String.Join(",", colList)}) VALUES ({String.Join(",", paramList)})"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.Parameters.AddWithValue("@ImportDate", DateTime.Now.ToString("M/d/yyyy h:mm:ss tt"))
                cmd.Parameters.AddWithValue("@URL", url)
                cmd.Parameters.AddWithValue("@Owner", resolvedName)
                cmd.Parameters.AddWithValue("@GallusName", gallusName)
                cmd.Parameters.AddWithValue("@ResolvedName", resolvedName)
                cmd.Parameters.AddWithValue("@FrontBack", ctx.sFrontBack)

                For i As Integer = 1 To 18
                    cmd.Parameters.AddWithValue($"@H{i}", If(h(i) > 0, h(i), DBNull.Value))
                    cmd.Parameters.AddWithValue($"@Par{i}", If(par(i) > 0, par(i), DBNull.Value))
                    cmd.Parameters.AddWithValue($"@Hcp{i}", If(hcp(i) > 0, hcp(i), DBNull.Value))
                Next

                cmd.Parameters.AddWithValue("@OutGross", If(outGross > 0, outGross, DBNull.Value))
                cmd.Parameters.AddWithValue("@InGross", If(inGross > 0, inGross, DBNull.Value))
                cmd.Parameters.AddWithValue("@TotalGross", If(totalGross > 0, totalGross, DBNull.Value))
                cmd.Parameters.AddWithValue("@Note", "")

                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                cmd.ExecuteNonQuery()
                LOGIT($"SaveToGallusImport: saved {resolvedName}")
            End Using

        Catch ex As Exception
            LOGIT($"SaveToGallusImport Error: {ex.Message}")
        End Try
    End Sub

    Sub ImportPhotoButton(tp As TabPage, btnGallus As Button, dgv As DataGridView)
#Region "Import Photo Button"
        Dim btnPhoto As New Button()
        btnPhoto.Name = "btnImportPhoto"
        btnPhoto.Text = "Import Photo"
        btnPhoto.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnPhoto.Size = New Size(110, 25)
        btnPhoto.Location = New Point(btnGallus.Left, btnGallus.Bottom + 5)
        btnPhoto.BackColor = Color.FromArgb(0, 100, 180)
        btnPhoto.ForeColor = Color.White
        btnPhoto.FlatStyle = FlatStyle.Flat
        btnPhoto.FlatAppearance.BorderSize = 0
        tp.Controls.Add(btnPhoto)

        AddHandler btnPhoto.Click, Sub(s, ev)
                                       Dim ofd As New OpenFileDialog()
                                       ofd.Title = "Select Scorecard Photo"
                                       ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
                                       If ofd.ShowDialog() <> DialogResult.OK Then Exit Sub

                                       Dim imagePath As String = ofd.FileName
                                       Dim playerDataList As List(Of Dictionary(Of String, Object)) = Nothing

                                       Try
                                           Dim apiKey As String = System.IO.File.ReadAllText("C:\HughsGolf\Files\config.json")
                                           Dim doc As System.Text.Json.JsonDocument = System.Text.Json.JsonDocument.Parse(apiKey)
                                           Dim key As String = doc.RootElement.GetProperty("AnthropicApiKey").GetString()
                                           playerDataList = ohelper.ParseScorecardViaClaude(imagePath, key)
                                       Catch ex As Exception
                                           LOGIT($"Claude API failed: {ex.Message}")
                                           MessageBox.Show("Could not read scorecard." & vbCrLf & ex.Message,
                                               "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                           Exit Sub
                                       End Try

                                       If playerDataList Is Nothing OrElse playerDataList.Count = 0 Then
                                           MessageBox.Show("Could not parse any scores from the photo.",
                                               "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                           Exit Sub
                                       End If

                                       Dim selectedDate As String = ctx.ActiveDate
                                       Dim selectedGroup As Integer = 0
                                       Dim groupPlayers As New List(Of String)

                                       Using setupDlg As New Form()
                                           setupDlg.Text = "Scorecard Setup"
                                           setupDlg.Size = New Size(360, 180)
                                           setupDlg.StartPosition = FormStartPosition.CenterParent
                                           setupDlg.FormBorderStyle = FormBorderStyle.FixedDialog
                                           setupDlg.MaximizeBox = False

                                           Dim lblDate As New Label()
                                           lblDate.Text = "Round Date:"
                                           lblDate.Location = New Point(10, 15)
                                           lblDate.AutoSize = True
                                           setupDlg.Controls.Add(lblDate)

                                           Dim dtp As New DateTimePicker()
                                           dtp.Value = DateTime.Today
                                           dtp.Format = DateTimePickerFormat.Short
                                           dtp.Location = New Point(120, 12)
                                           dtp.Width = 120
                                           setupDlg.Controls.Add(dtp)

                                           Dim lblGroup As New Label()
                                           lblGroup.Text = "Scorecard Group:"
                                           lblGroup.Location = New Point(10, 50)
                                           lblGroup.AutoSize = True
                                           setupDlg.Controls.Add(lblGroup)

                                           Dim cboGroup As New ComboBox()
                                           cboGroup.Location = New Point(120, 47)
                                           cboGroup.Width = 210
                                           cboGroup.DropDownStyle = ComboBoxStyle.DropDownList
                                           setupDlg.Controls.Add(cboGroup)

                                           Using cmd As New SQLiteCommand("
                                               SELECT [Group], GROUP_CONCAT(Player, ' / ') AS Players
                                               FROM Teams 
                                               WHERE League=@League AND Year=@Year
                                               AND [Group] IS NOT NULL
                                               GROUP BY [Group]
                                               ORDER BY [Group]", ctx.Conn)
                                               cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                               cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                                               If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                               Using dr As SQLiteDataReader = cmd.ExecuteReader()
                                                   While dr.Read()
                                                       cboGroup.Items.Add($"Group {dr("Group")} - {dr("Players")}")
                                                   End While
                                               End Using
                                           End Using
                                           If cboGroup.Items.Count > 0 Then cboGroup.SelectedIndex = 0

                                           Dim btnNext As New Button()
                                           btnNext.Text = "Next"
                                           btnNext.Location = New Point(120, 95)
                                           btnNext.Size = New Size(75, 25)
                                           btnNext.BackColor = Color.FromArgb(0, 150, 50)
                                           btnNext.ForeColor = Color.White
                                           btnNext.FlatStyle = FlatStyle.Flat
                                           setupDlg.Controls.Add(btnNext)
                                           setupDlg.AcceptButton = btnNext

                                           Dim btnCancel2 As New Button()
                                           btnCancel2.Text = "Cancel"
                                           btnCancel2.Location = New Point(205, 95)
                                           btnCancel2.Size = New Size(75, 25)
                                           setupDlg.Controls.Add(btnCancel2)
                                           setupDlg.CancelButton = btnCancel2

                                           AddHandler btnNext.Click, Sub(od, oe)
                                                                         selectedDate = dtp.Value.ToString("yyyyMMdd")
                                                                         Dim groupText As String = cboGroup.SelectedItem?.ToString()
                                                                         If Not String.IsNullOrEmpty(groupText) Then
                                                                             selectedGroup = CInt(groupText.Split(" "c)(1))
                                                                         End If
                                                                         setupDlg.Close()
                                                                     End Sub

                                           AddHandler btnCancel2.Click, Sub(cd, ce)
                                                                            setupDlg.Close()
                                                                        End Sub

                                           setupDlg.ShowDialog()
                                       End Using

                                       If selectedGroup = 0 Then Exit Sub

                                       Using cmd As New SQLiteCommand("
                                           SELECT Player FROM Teams 
                                           WHERE League=@League AND Year=@Year 
                                           AND [Group]=@Group
                                           ORDER BY Grade", ctx.Conn)
                                           cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                           cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                                           cmd.Parameters.AddWithValue("@Group", selectedGroup)
                                           If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                           Using dr As SQLiteDataReader = cmd.ExecuteReader()
                                               While dr.Read()
                                                   groupPlayers.Add(dr("Player").ToString())
                                               End While
                                           End Using
                                       End Using

                                       Dim matchedSlots(groupPlayers.Count - 1) As Boolean

                                       For Each pd As Dictionary(Of String, Object) In playerDataList
                                           Dim claudeName As String = pd("PlayerName").ToString()
                                           Dim claudeFirst As String = claudeName.Split(" "c).First().ToLower()
                                           Dim claudeLast As String = claudeName.Split(" "c).Last().ToLower()
                                           Dim matched As Boolean = False

                                           For i As Integer = 0 To groupPlayers.Count - 1
                                               If matchedSlots(i) Then Continue For
                                               Dim groupPlayer As String = groupPlayers(i)
                                               Dim groupFirst As String = groupPlayer.Split(" "c).First().ToLower()
                                               Dim groupLast As String = groupPlayer.Split(" "c).Last().ToLower()

                                               If groupLast.Contains(claudeLast) OrElse
                                                  groupFirst.Contains(claudeFirst) OrElse
                                                  claudeLast.Contains(groupLast) OrElse
                                                  claudeFirst.Contains(groupFirst) Then
                                                   pd("PlayerName") = groupPlayer
                                                   pd("Resolved") = True
                                                   pd("IsSub") = False
                                                   pd("SubbingFor") = ""
                                                   matchedSlots(i) = True
                                                   matched = True
                                                   LOGIT($"Matched {claudeName} → {groupPlayer}")
                                                   Exit For
                                               End If
                                           Next

                                           If Not matched Then
                                               LOGIT($"No group match for {claudeName} - likely sub")
                                               pd("Resolved") = False
                                               pd("IsSub") = True
                                               pd("SubbingFor") = ""
                                           End If
                                       Next

                                       For i As Integer = 0 To groupPlayers.Count - 1
                                           If matchedSlots(i) Then Continue For
                                           For Each pd As Dictionary(Of String, Object) In playerDataList
                                               If CBool(pd("Resolved")) Then Continue For
                                               If Not String.IsNullOrEmpty(pd("SubbingFor").ToString()) Then Continue For
                                               pd("SubbingFor") = groupPlayers(i)
                                               LOGIT($"Unresolved player subbing for {groupPlayers(i)}")
                                               Exit For
                                           Next
                                       Next

                                       Dim confirmed As Boolean = False
                                       Dim playedFrontOverride As Boolean = (ctx.sFrontBack = "Front")

                                       Using dlg As New Form()
                                           dlg.Text = "Confirm Scorecard Import"
                                           dlg.Size = New Size(640, 380)
                                           dlg.StartPosition = FormStartPosition.CenterParent
                                           dlg.FormBorderStyle = FormBorderStyle.FixedDialog
                                           dlg.MaximizeBox = False

                                           Dim parsedDate As DateTime = DateTime.ParseExact(selectedDate, "yyyyMMdd", Nothing)
                                           Dim lblInfo As New Label()
                                           lblInfo.Text = $"Date: {parsedDate.ToString("M/d/yyyy")}   Group: {selectedGroup}   Source: Claude API"
                                           lblInfo.Location = New Point(10, 10)
                                           lblInfo.AutoSize = True
                                           lblInfo.ForeColor = Color.DarkBlue
                                           dlg.Controls.Add(lblInfo)

                                           Dim preview As New DataGridView()
                                           preview.Location = New Point(10, 35)
                                           preview.Size = New Size(610, 280)
                                           preview.AllowUserToAddRows = False
                                           preview.ReadOnly = False
                                           preview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
                                           preview.EditMode = DataGridViewEditMode.EditOnEnter
                                           dlg.Controls.Add(preview)

                                           AddHandler preview.DataError, Sub(es, ee)
                                                                             ee.Cancel = True
                                                                         End Sub

                                           Dim colPlayer As New DataGridViewComboBoxColumn()
                                           colPlayer.Name = "Player"
                                           colPlayer.HeaderText = "Player"
                                           colPlayer.Width = 160

                                           Dim allPlayersList As New List(Of String)
                                           allPlayersList.Add("--- Group Members ---")
                                           allPlayersList.AddRange(groupPlayers)
                                           allPlayersList.Add("--- Subs ---")
                                           Using cmd As New SQLiteCommand("
                                               SELECT Player FROM Players 
                                               WHERE Player NOT IN (
                                                   SELECT Player FROM Teams 
                                                   WHERE League=@League AND Year=@Year)
                                               ORDER BY Player", ctx.Conn)
                                               cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                               cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                                               If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                               Using dr As SQLiteDataReader = cmd.ExecuteReader()
                                                   While dr.Read()
                                                       allPlayersList.Add(dr("Player").ToString())
                                                   End While
                                               End Using
                                           End Using
                                           colPlayer.DataSource = allPlayersList
                                           preview.Columns.Add(colPlayer)

                                           For i As Integer = 1 To 9
                                               Dim col As New DataGridViewTextBoxColumn()
                                               col.Name = $"H{i}"
                                               col.HeaderText = i.ToString()
                                               col.Width = 32
                                               col.ReadOnly = True
                                               preview.Columns.Add(col)
                                           Next

                                           Dim colGross As New DataGridViewTextBoxColumn()
                                           colGross.Name = "Gross"
                                           colGross.HeaderText = "Tot"
                                           colGross.Width = 40
                                           colGross.ReadOnly = True
                                           preview.Columns.Add(colGross)

                                           Dim colStatus As New DataGridViewTextBoxColumn()
                                           colStatus.Name = "Status"
                                           colStatus.HeaderText = "Status"
                                           colStatus.Width = 80
                                           colStatus.ReadOnly = True
                                           preview.Columns.Add(colStatus)

                                           For Each pd As Dictionary(Of String, Object) In playerDataList
                                               Dim scores() As Integer = DirectCast(pd("Front9Scores"), Integer())
                                               Dim gross As Integer = scores.Skip(1).Sum()
                                               Dim isResolved As Boolean = CBool(pd("Resolved"))
                                               Dim rowIdx As Integer = preview.Rows.Add()
                                               Dim row As DataGridViewRow = preview.Rows(rowIdx)
                                               row.Cells("Player").Value = pd("PlayerName").ToString()
                                               For i As Integer = 1 To 9
                                                   row.Cells($"H{i}").Value = If(scores(i) > 0, scores(i), Nothing)
                                               Next
                                               row.Cells("Gross").Value = If(gross > 0, gross, Nothing)
                                               Dim subbingFor As String = If(pd.ContainsKey("SubbingFor"), pd("SubbingFor").ToString(), "")
                                               row.Cells("Status").Value = If(isResolved, "✓ Matched",
                                                   If(Not String.IsNullOrEmpty(subbingFor), $"Sub/{subbingFor.Split(" "c).Last()}", "? Unresolved"))
                                               If Not isResolved Then
                                                   row.DefaultCellStyle.BackColor = Color.LightYellow
                                               End If
                                           Next

                                           Dim btnOK As New Button()
                                           btnOK.Text = "Import"
                                           btnOK.Location = New Point(450, 325)
                                           btnOK.Size = New Size(75, 25)
                                           btnOK.BackColor = Color.FromArgb(0, 150, 50)
                                           btnOK.ForeColor = Color.White
                                           btnOK.FlatStyle = FlatStyle.Flat
                                           dlg.Controls.Add(btnOK)

                                           Dim btnCancel As New Button()
                                           btnCancel.Text = "Cancel"
                                           btnCancel.Location = New Point(535, 325)
                                           btnCancel.Size = New Size(75, 25)
                                           dlg.Controls.Add(btnCancel)

                                           AddHandler btnOK.Click, Sub(os, oev)
                                                                       confirmed = True
                                                                       For i As Integer = 0 To preview.Rows.Count - 1
                                                                           If i < playerDataList.Count Then
                                                                               Dim cellVal = preview.Rows(i).Cells("Player").Value
                                                                               If cellVal IsNot Nothing Then
                                                                                   Dim pickedName As String = cellVal.ToString()
                                                                                   If pickedName.StartsWith("---") Then Continue For
                                                                                   playerDataList(i)("PlayerName") = pickedName
                                                                                   If Not groupPlayers.Any(Function(p) p = pickedName) Then
                                                                                       playerDataList(i)("IsSub") = True
                                                                                       If i < groupPlayers.Count AndAlso
                                                                                          String.IsNullOrEmpty(playerDataList(i)("SubbingFor").ToString()) Then
                                                                                           playerDataList(i)("SubbingFor") = groupPlayers(i)
                                                                                       End If
                                                                                   End If
                                                                               End If
                                                                           End If
                                                                       Next
                                                                       For Each pd As Dictionary(Of String, Object) In playerDataList
                                                                           pd("PlayedFront") = playedFrontOverride
                                                                       Next
                                                                       dlg.Close()
                                                                   End Sub

                                           AddHandler btnCancel.Click, Sub(cs, cev)
                                                                           dlg.Close()
                                                                       End Sub

                                           dlg.ShowDialog()
                                       End Using

                                       If Not confirmed Then Exit Sub

                                       Dim imported As Integer = 0
                                       Dim notFound As New List(Of String)

                                       For Each data As Dictionary(Of String, Object) In playerDataList
                                           Dim playerName As String = data("PlayerName").ToString()
                                           If playerName.StartsWith("---") Then Continue For
                                           Dim playedFront As Boolean = CBool(data("PlayedFront"))
                                           Dim isSub As Boolean = data.ContainsKey("IsSub") AndAlso CBool(data("IsSub"))
                                           Dim subbingFor As String = If(data.ContainsKey("SubbingFor"), data("SubbingFor").ToString(), "")

                                           Dim subName As String = playerName.Trim()
                                           Dim nameParts() As String = subName.Split(" "c)
                                           Dim lastName As String = If(nameParts.Length > 1, nameParts(nameParts.Length - 1), subName)

                                           Dim targetRow As DataGridViewRow = Nothing

                                           If isSub AndAlso Not String.IsNullOrEmpty(subbingFor) Then
                                               Dim subLastName As String = subbingFor.Split(" "c).Last()
                                               For Each r As DataGridViewRow In dgScores.Rows
                                                   If r.IsNewRow Then Continue For
                                                   Dim gridPlayer As String = r.Cells("Player").Value?.ToString()
                                                   If String.IsNullOrEmpty(gridPlayer) Then Continue For
                                                   If gridPlayer.ToLower().Contains(subLastName.ToLower()) Then
                                                       targetRow = r
                                                       LOGIT($"Sub {subName} found row via subbingFor {subbingFor}")
                                                       Exit For
                                                   End If
                                               Next
                                           End If

                                           If targetRow Is Nothing Then
                                               For Each r As DataGridViewRow In dgScores.Rows
                                                   If r.IsNewRow Then Continue For
                                                   Dim gridPlayer As String = r.Cells("Player").Value?.ToString()
                                                   If String.IsNullOrEmpty(gridPlayer) Then Continue For
                                                   If gridPlayer.ToLower().Contains(lastName.ToLower()) Then
                                                       targetRow = r
                                                       Exit For
                                                   End If
                                               Next
                                           End If

                                           If targetRow Is Nothing Then
                                               Dim matchedPlayer As String = ""
                                               Using cmd As New SQLiteCommand("
                                                   SELECT Player FROM Teams 
                                                   WHERE League=@League AND Year=@Year 
                                                   AND Player LIKE @Name", ctx.Conn)
                                                   cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                                   cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                                                   cmd.Parameters.AddWithValue("@Name", $"%{lastName}%")
                                                   If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                                   Dim result = cmd.ExecuteScalar()
                                                   If result IsNot Nothing Then matchedPlayer = result.ToString()
                                               End Using
                                               If Not String.IsNullOrEmpty(matchedPlayer) Then
                                                   For Each r As DataGridViewRow In dgScores.Rows
                                                       If r.IsNewRow Then Continue For
                                                       If r.Cells("Player").Value?.ToString() = matchedPlayer Then
                                                           targetRow = r
                                                           Exit For
                                                       End If
                                                   Next
                                               End If
                                           End If

                                           If targetRow Is Nothing Then
                                               notFound.Add(subName)
                                               Continue For
                                           End If

                                           Dim scores() As Integer = If(playedFront,
                                               DirectCast(data("Front9Scores"), Integer()),
                                               DirectCast(data("Back9Scores"), Integer()))

                                           EditScoresEngine._suppressEvents = True
                                           targetRow.Cells("Player").Value = subName
                                           For i As Integer = 1 To 9
                                               If dgScores.Columns.Contains(i.ToString()) Then
                                                   targetRow.Cells(i.ToString()).Value = If(scores(i) > 0, scores(i), Nothing)
                                               End If
                                           Next
                                           EditScoresEngine._suppressEvents = False

                                           EditScoresEngine.UpsertScore(targetRow, "1", scores(1))
                                           imported += 1
                                           LOGIT($"Photo Import: {subName} imported successfully")
                                       Next

                                       FinalizeImportedScores(dgv, playerDataList)

                                       For Each data As Dictionary(Of String, Object) In playerDataList
                                           If Not data.ContainsKey("IsSub") Then Continue For
                                           If Not CBool(data("IsSub")) Then Continue For
                                           Dim subName As String = data("PlayerName").ToString()
                                           If subName.StartsWith("---") Then Continue For
                                           Dim subbingFor As String = If(data.ContainsKey("SubbingFor"), data("SubbingFor").ToString(), "")
                                           If String.IsNullOrEmpty(subbingFor) Then Continue For

                                           Dim nextId As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID),0)+1 FROM Subs"))

                                           Using cmd As New SQLiteCommand("
                                               DELETE FROM Subs WHERE League=@League AND Player=@Sub AND Date=@Date", ctx.Conn)
                                               cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                               cmd.Parameters.AddWithValue("@Sub", subName)
                                               cmd.Parameters.AddWithValue("@Date", selectedDate)
                                               If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                               cmd.ExecuteNonQuery()
                                           End Using

                                           Using cmd As New SQLiteCommand("
                                               INSERT INTO Subs (ID, League, Player, Date, Team, Grade)
                                               SELECT @ID, @League, @Sub, @Date, Team, Grade FROM Teams
                                               WHERE League=@League AND Player=@Orig COLLATE NOCASE AND Year=@Year", ctx.Conn)
                                               cmd.Parameters.AddWithValue("@ID", nextId)
                                               cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                               cmd.Parameters.AddWithValue("@Sub", subName)
                                               cmd.Parameters.AddWithValue("@Orig", subbingFor)
                                               cmd.Parameters.AddWithValue("@Date", selectedDate)
                                               cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                                               If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                               cmd.ExecuteNonQuery()
                                           End Using

                                           Using cmd As New SQLiteCommand("
                                               UPDATE Matches SET Player=@Sub 
                                               WHERE League=@League AND Date=@Date AND Player=@Orig", ctx.Conn)
                                               cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                               cmd.Parameters.AddWithValue("@Sub", subName)
                                               cmd.Parameters.AddWithValue("@Orig", subbingFor)
                                               cmd.Parameters.AddWithValue("@Date", selectedDate)
                                               If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                               cmd.ExecuteNonQuery()
                                           End Using

                                           Using cmd As New SQLiteCommand("
                                               UPDATE Matches SET Opponent=@Sub 
                                               WHERE League=@League AND Date=@Date AND Opponent=@Orig", ctx.Conn)
                                               cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                                               cmd.Parameters.AddWithValue("@Sub", subName)
                                               cmd.Parameters.AddWithValue("@Orig", subbingFor)
                                               cmd.Parameters.AddWithValue("@Date", selectedDate)
                                               If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                                               cmd.ExecuteNonQuery()
                                           End Using

                                           LOGIT($"Sub registered: {subName} for {subbingFor}")
                                       Next

                                       LoadAllTabs()

                                       Dim msg As String = $"Imported {imported} of {playerDataList.Count} players."
                                       If notFound.Count > 0 Then
                                           msg &= vbCrLf & "Not found:"
                                           For Each n As String In notFound
                                               msg &= vbCrLf & $"  • {n}"
                                           Next
                                       End If
                                       MessageBox.Show(msg, "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                   End Sub

#End Region
    End Sub

    Private Function FindSubbingFor(detectedGroup As Integer, resolvedNames As List(Of String), activeDate As String, Optional subPlayerName As String = "") As String
        Try
            If Not String.IsNullOrEmpty(subPlayerName) Then
                Using cmd As New SQLiteCommand("
                SELECT Ma2.Player FROM Matches Ma
                INNER JOIN Matches Ma2 ON Ma2.League=Ma.League
                    AND Ma2.Date < Ma.Date
                    AND Ma2.Partner = Ma.Partner
                WHERE Ma.League=@League
                AND Ma.Player=@SubPlayer
                AND Ma.Date=@Date
                AND Ma2.Player != @SubPlayer
                ORDER BY Ma2.Date DESC
                LIMIT 1", ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@SubPlayer", subPlayerName)
                    cmd.Parameters.AddWithValue("@Date", activeDate)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        LOGIT($"FindSubbingFor: {subPlayerName} replaced {result.ToString()} via Partner slot history")
                        Return result.ToString()
                    End If
                End Using
            End If

            Dim inClause As String = If(resolvedNames.Count > 0,
            String.Join(",", resolvedNames.Select(Function(n) $"'{n.Replace("'", "''")}'")), "")

            Dim sql As String = "
            SELECT T.Player FROM Teams T
            LEFT JOIN Matches Ma ON Ma.League=T.League 
                AND Ma.Date=@Date 
                AND Ma.Player=T.Player
            WHERE T.League=@League
            AND T.Year=@Year
            AND T.[Group]=@Group
            AND Ma.Player IS NULL"

            If Not String.IsNullOrEmpty(inClause) Then
                sql &= $" AND T.Player NOT IN ({inClause})"
            End If
            sql &= " ORDER BY T.Team LIMIT 1"

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                cmd.Parameters.AddWithValue("@Group", detectedGroup)
                cmd.Parameters.AddWithValue("@Date", activeDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    LOGIT($"FindSubbingFor fallback: {result.ToString()} has no Matches row for {activeDate}")
                    Return result.ToString()
                End If
            End Using

        Catch ex As Exception
            LOGIT($"FindSubbingFor error: {ex.Message}")
        End Try
        Return ""
    End Function

    Private Function ShowPickerDialog(prompt As String, title As String, items As List(Of String), cancelText As String) As String
        Dim result As String = ""
        Using frm As New Form()
            frm.Text = title
            frm.Size = New Size(300, 400)
            frm.StartPosition = FormStartPosition.CenterParent
            frm.FormBorderStyle = FormBorderStyle.FixedDialog
            frm.MaximizeBox = False

            Dim lbl As New Label()
            lbl.Text = prompt
            lbl.Location = New Point(10, 10)
            lbl.Size = New Size(270, 40)
            frm.Controls.Add(lbl)

            Dim lst As New ListBox()
            lst.Location = New Point(10, 55)
            lst.Size = New Size(265, 250)
            lst.DataSource = items
            frm.Controls.Add(lst)

            Dim btnPick As New Button()
            btnPick.Text = "Select"
            btnPick.Location = New Point(10, 315)
            btnPick.Size = New Size(75, 25)
            btnPick.BackColor = Color.FromArgb(0, 150, 50)
            btnPick.ForeColor = Color.White
            btnPick.FlatStyle = FlatStyle.Flat
            frm.Controls.Add(btnPick)

            Dim btnSkip As New Button()
            btnSkip.Text = cancelText
            btnSkip.Location = New Point(100, 315)
            btnSkip.Size = New Size(140, 25)
            frm.Controls.Add(btnSkip)

            AddHandler btnPick.Click, Sub(s, e)
                                          If lst.SelectedItem IsNot Nothing Then
                                              result = lst.SelectedItem.ToString()
                                              frm.Close()
                                          End If
                                      End Sub

            AddHandler btnSkip.Click, Sub(s, e)
                                          frm.Close()
                                      End Sub

            AddHandler lst.DoubleClick, Sub(s, e)
                                            If lst.SelectedItem IsNot Nothing Then
                                                result = lst.SelectedItem.ToString()
                                                frm.Close()
                                            End If
                                        End Sub

            frm.ShowDialog()
        End Using
        Return result
    End Function

    Private Sub CheckAndPromptGallusImport()
        Try
            LOGIT($"CheckAndPromptGallusImport stack: {New System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}")

            Dim missingPlayers As New List(Of String)
            Using cmd As New SQLiteCommand("
    SELECT DISTINCT ResolvedName FROM GallusImport
    WHERE League=@League AND Date=@Date
    AND ResolvedName NOT IN (
        SELECT Player FROM Scores 
        WHERE League=@League AND Date=@Date
        AND Gross IS NOT NULL AND Gross > 0
    ) COLLATE NOCASE", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using dr As SQLiteDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        missingPlayers.Add(dr("ResolvedName").ToString())
                    End While
                End Using
            End Using

            If missingPlayers.Count = 0 Then Exit Sub

            Dim mbr = MessageBox.Show(
            $"Gallus scores found for {missingPlayers.Count} player(s) not yet in scorecard:{vbCrLf}{String.Join(vbCrLf, missingPlayers)}{vbCrLf}{vbCrLf}Would you like to import?",
            "Gallus Data Available",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

            If mbr <> DialogResult.Yes Then Exit Sub

            Dim playerDataList As New List(Of Dictionary(Of String, Object))
            Using cmd As New SQLiteCommand("
            SELECT * FROM GallusImport
            WHERE League=@League AND Date=@Date", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    Dim dtGallus As New DataTable()
                    da.Fill(dtGallus)
                    For Each row As DataRow In dtGallus.Rows
                        Dim front9Scores(9) As Integer
                        Dim back9Scores(9) As Integer
                        For i As Integer = 1 To 9
                            If Not IsDBNull(row($"H{i}")) Then front9Scores(i) = CInt(row($"H{i}"))
                            If Not IsDBNull(row($"O{i}")) Then back9Scores(i) = CInt(row($"O{i}"))
                        Next
                        Dim frontGross As Integer = front9Scores.Sum()
                        Dim backGross As Integer = back9Scores.Sum()
                        Dim data As New Dictionary(Of String, Object)
                        data("PlayerName") = row("GallusName").ToString()
                        data("ResolvedName") = row("ResolvedName").ToString()
                        data("Front9Scores") = front9Scores
                        data("Back9Scores") = back9Scores
                        data("Front9Par") = New Integer(9) {}
                        data("Back9Par") = New Integer(9) {}
                        data("Front9Hcp") = New Integer(9) {}
                        data("Back9Hcp") = New Integer(9) {}
                        data("FrontGross") = frontGross
                        data("BackGross") = backGross
                        data("PlayedFront") = frontGross > 0
                        data("PlayedBack") = backGross > 0
                        playerDataList.Add(data)
                    Next
                End Using
            End Using

            Dim gridPlayers As New List(Of String)
            For Each r As DataGridViewRow In dgScores.Rows
                If r.IsNewRow Then Continue For
                Dim gp As String = r.Cells("Player").Value?.ToString()
                If Not String.IsNullOrEmpty(gp) Then gridPlayers.Add(gp)
            Next

            For Each data As Dictionary(Of String, Object) In playerDataList
                Dim gallusName As String = data("PlayerName").ToString()
                Dim subName As String = ResolveProperPlayerName(data("ResolvedName").ToString().Trim().ToLower())
                Dim origName As String = ""
                Dim playedFront As Boolean = CBool(data("PlayedFront"))
                Dim scores() As Integer = If(playedFront,
                DirectCast(data("Front9Scores"), Integer()),
                DirectCast(data("Back9Scores"), Integer()))

                If gallusName.Contains("-") Then
                    origName = gallusName.Substring(gallusName.IndexOf("-") + 1).Trim()
                    origName = ResolveProperPlayerName(origName.ToLower())
                End If

                Dim searchName As String = If(String.IsNullOrEmpty(origName), subName, origName)

                If Not String.IsNullOrEmpty(origName) Then
                    Dim nextId As Integer = CInt(ohelper.SQLiteExecuteScalar("SELECT IFNULL(MAX(ID),0)+1 FROM Subs"))
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

                    Using cmd As New SQLiteCommand("DELETE FROM Subs WHERE League=@League AND Player=@Sub AND Date=@Date", ctx.Conn)
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Sub", subName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SQLiteCommand("
                    INSERT INTO Subs (ID, League, Player, Date, Team, Grade)
                    SELECT @ID, @League, @Sub, @Date, Team, Grade FROM Teams
                    WHERE League=@League AND Player=@Orig COLLATE NOCASE AND Year=@Year", ctx.Conn)
                        cmd.Parameters.AddWithValue("@ID", nextId)
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Sub", subName)
                        cmd.Parameters.AddWithValue("@Orig", origName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SQLiteCommand("UPDATE Matches SET Player=@Sub WHERE League=@League AND Date=@Date AND Player=@Orig", ctx.Conn)
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Sub", subName)
                        cmd.Parameters.AddWithValue("@Orig", origName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.ExecuteNonQuery()
                    End Using

                    Using cmd As New SQLiteCommand("UPDATE Matches SET Opponent=@Sub WHERE League=@League AND Date=@Date AND Opponent=@Orig", ctx.Conn)
                        cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                        cmd.Parameters.AddWithValue("@Sub", subName)
                        cmd.Parameters.AddWithValue("@Orig", origName)
                        cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                        cmd.ExecuteNonQuery()
                    End Using

                    LOGIT($"Sub registered: {subName} for {origName}")
                End If

                Dim foundRow As Boolean = False
                For Each r As DataGridViewRow In dgScores.Rows
                    If r.IsNewRow Then Continue For
                    Dim gridPlayer As String = r.Cells("Player").Value?.ToString()
                    If String.IsNullOrEmpty(gridPlayer) Then Continue For
                    If gridPlayer.ToLower().Contains(searchName.ToLower()) Then
                        r.Cells("Player").Value = subName

                        If Not String.IsNullOrEmpty(origName) Then
                            Dim subHdcp As Object = ohelper.SQLiteExecuteScalar($"
                            SELECT Hdcp FROM Handicaps 
                            WHERE League='{ctx.SafeLeagueName}' 
                            AND Player='{subName.Replace("'", "''")}'
                            AND Hdcp <> '' ORDER BY Date DESC LIMIT 1")
                            If subHdcp IsNot Nothing AndAlso IsNumeric(subHdcp) Then
                                r.Cells("phdcp").Value = CInt(subHdcp)
                            End If
                        End If

                        EditScoresEngine._suppressEvents = True
                        For i As Integer = 1 To 9
                            If dgScores.Columns.Contains(i.ToString()) Then
                                r.Cells(i.ToString()).Value = If(scores(i) > 0, scores(i), Nothing)
                            End If
                        Next
                        EditScoresEngine._suppressEvents = False

                        EditScoresEngine.UpsertScore(r, "1", scores(1))
                        foundRow = True
                        LOGIT($"Imported: {subName}")
                        Exit For
                    End If
                Next

                If Not foundRow Then
                    ' Find group from already imported players
                    Dim importedNames = playerDataList.
        Where(Function(d) d.ContainsKey("Imported") AndAlso CBool(d("Imported"))).
        Select(Function(d) d("ResolvedName").ToString()).ToList()

                    Dim missingMember As String = ""
                    If importedNames.Count > 0 Then
                        Dim inClause = String.Join(",", importedNames.Select(Function(n) $"'{n.Replace("'", "''")}'"))
                        Using cmd As New SQLiteCommand($"
            SELECT T2.Player FROM Teams T2
            WHERE T2.League=@League AND T2.Year=@Year
            AND T2.[Group] = (
                SELECT [Group] FROM Teams 
                WHERE League=@League AND Year=@Year
                AND Player IN ({inClause})
                GROUP BY [Group] ORDER BY COUNT(*) DESC LIMIT 1
            )
            AND T2.Player NOT IN ({inClause})
            AND T2.Player NOT IN (
                SELECT Player FROM Scores 
                WHERE League=@League AND Date=@Date 
                AND Gross IS NOT NULL
            )
            LIMIT 1", ctx.Conn)
                            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                            cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                            cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                            Dim result = cmd.ExecuteScalar()
                            If result IsNot Nothing Then missingMember = result.ToString()
                        End Using
                    End If

                    If Not String.IsNullOrEmpty(missingMember) Then
                        Dim mbr1 = MessageBox.Show(
            $"'{subName}' is not in the scorecard." & vbCrLf &
            $"Is '{subName}' subbing for '{missingMember}'?",
            "Confirm Sub", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                        If mbr1 = DialogResult.Yes Then
                            For Each r As DataGridViewRow In dgScores.Rows
                                If r.IsNewRow Then Continue For
                                If r.Cells("Player").Value?.ToString() = missingMember Then
                                    Dim nameParts() As String = subName.Split(" "c)
                                    RegisterSub(subName, missingMember,
                        If(nameParts.Length > 1, nameParts(0), ""), nameParts.Last())
                                    r.Cells("Player").Value = subName
                                    EditScoresEngine._suppressEvents = True
                                    For i As Integer = 1 To 9
                                        If dgScores.Columns.Contains(i.ToString()) Then
                                            r.Cells(i.ToString()).Value = If(scores(i) > 0, scores(i), Nothing)
                                        End If
                                    Next
                                    EditScoresEngine._suppressEvents = False
                                    EditScoresEngine.UpsertScore(r, "1", scores(1))
                                    LOGIT($"Sub confirmed: {subName} for {missingMember}")
                                    Exit For
                                End If
                            Next
                        End If
                    Else
                        ' Fall back to manual picker
                        Dim gridPlayerNames As New List(Of String)
                        For Each r As DataGridViewRow In dgScores.Rows
                            If r.IsNewRow Then Continue For
                            Dim gp = r.Cells("Player").Value?.ToString()
                            If Not String.IsNullOrEmpty(gp) Then gridPlayerNames.Add(gp)
                        Next
                        Dim picked = ShowPickerDialog(
            $"Who is '{subName}' subbing for?",
            "Select Player", gridPlayerNames, "Skip")
                        If Not String.IsNullOrEmpty(picked) Then
                            For Each r As DataGridViewRow In dgScores.Rows
                                If r.IsNewRow Then Continue For
                                If r.Cells("Player").Value?.ToString() = picked Then
                                    Dim nameParts() As String = subName.Split(" "c)
                                    RegisterSub(subName, picked,
                        If(nameParts.Length > 1, nameParts(0), ""), nameParts.Last())
                                    r.Cells("Player").Value = subName
                                    EditScoresEngine._suppressEvents = True
                                    For i As Integer = 1 To 9
                                        If dgScores.Columns.Contains(i.ToString()) Then
                                            r.Cells(i.ToString()).Value = If(scores(i) > 0, scores(i), Nothing)
                                        End If
                                    Next
                                    EditScoresEngine._suppressEvents = False
                                    EditScoresEngine.UpsertScore(r, "1", scores(1))
                                    LOGIT($"Manual sub: {subName} for {picked}")
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If

            Next

            FinalizeImportedScores(dgScores, playerDataList)

            dtScores = ohelper.sqliteda("ScoreCard", $"Select * from vwMatchesScores where date={ctx.ActiveDate} ORDER BY Partner")
            dtScores.PrimaryKey = New DataColumn() {dtScores.Columns(Player)}
            dgScores.DataSource = dtScores
            EditScoresEngine.RefreshScoresGrid()
            dgScores.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty
            For i As Integer = 0 To dgScores.Rows.Count - 1 Step 8
                For ii As Integer = 0 To 3
                    Dim rowIndex As Integer = i + ii
                    If rowIndex < dgScores.Rows.Count Then
                        dgScores.Rows(rowIndex).DefaultCellStyle.BackColor = Color.LightBlue
                    End If
                Next
            Next

        Catch ex As Exception
            LOGIT($"CheckAndPromptGallusImport Error: {ex.Message}")
        End Try
    End Sub
    Private Function NormalizeImportedPlayerName(rawName As String) As String
        If String.IsNullOrWhiteSpace(rawName) Then Return ""

        Dim cleaned As String = System.Net.WebUtility.HtmlDecode(rawName).Trim()
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, "\s+", " ").Trim()

        If cleaned.Contains(",") Then
            Dim parts = cleaned.Split({","c}, 2)
            Dim lastName As String = parts(0).Trim()
            Dim firstName As String = parts(1).Trim()
            If firstName <> "" AndAlso lastName <> "" Then
                cleaned = firstName & " " & lastName
            End If
        End If

        Return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned.ToLower())
    End Function

    Private Function ResolveProperPlayerName(gallusName As String) As String
        Dim normalizedName As String = NormalizeImportedPlayerName(gallusName)

        ' Try to find exact match in Players table
        Using cmd As New SQLiteCommand("
        SELECT Player FROM Players 
        WHERE LOWER(Player) = LOWER(@Name)
        LIMIT 1", ctx.Conn)
            cmd.Parameters.AddWithValue("@Name", normalizedName)
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then Return result.ToString()
        End Using

        ' Try last name match
        Dim lastName As String = normalizedName.Trim().Split(" "c).Last()
        Using cmd As New SQLiteCommand("
        SELECT Player FROM Players 
        WHERE LOWER(Player) LIKE LOWER(@Name)
        LIMIT 1", ctx.Conn)
            cmd.Parameters.AddWithValue("@Name", "%" & lastName & "%")
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then Return result.ToString()
        End Using

        Return normalizedName
    End Function
    Friend Sub ImportGallusData(tp As TabPage)
        Dim btnGallus As New Button()
        btnGallus.Name = "btnImportGallus"
        btnGallus.Text = "Import Gallus"
        btnGallus.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnGallus.Size = New Size(110, 25)
        btnGallus.Location = New Point(dgScores.Right + 10, dgScores.Top)
        btnGallus.BackColor = Color.FromArgb(0, 150, 50)
        btnGallus.ForeColor = Color.White
        btnGallus.FlatStyle = FlatStyle.Flat
        btnGallus.FlatAppearance.BorderSize = 0
        tp.Controls.Add(btnGallus)
        AddHandler btnGallus.Click, AddressOf btnGallus_Click

        Dim btnReimport As New Button()
        btnReimport.Name = "btnReimportGallus"
        btnReimport.Text = "Reimport Gallus"
        btnReimport.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnReimport.Size = New Size(110, 25)
        btnReimport.Location = New Point(btnGallus.Left, btnGallus.Bottom + 5)
        btnReimport.BackColor = Color.FromArgb(150, 100, 0)
        btnReimport.ForeColor = Color.White
        btnReimport.FlatStyle = FlatStyle.Flat
        btnReimport.FlatAppearance.BorderSize = 0
        tp.Controls.Add(btnReimport)
        AddHandler btnReimport.Click, AddressOf btnReimportGallus_Click

        ImportPhotoButton(tp, btnGallus, dgScores)
    End Sub
    Private Sub btnReimportGallus_Click(sender As Object, e As EventArgs)
        Try
            Dim count As Integer = CInt(ohelper.SQLiteExecuteScalar(
            "SELECT COUNT(*) FROM GallusImport WHERE League=@League AND Date=@Date",
            New Dictionary(Of String, Object) From {
                {"@League", ctx.sLeagueName},
                {"@Date", ctx.ActiveDate}
            }))

            If count = 0 Then
                MessageBox.Show("No Gallus import data found for this date.",
                "Reimport", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            Dim importlist As New List(Of String)
            Dim urls As New List(Of String)
            Using cmd As New SQLiteCommand(
            "SELECT DISTINCT URL, ImportDate FROM GallusImport WHERE League=@League AND Date=@Date ORDER BY ImportDate DESC", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using dr As SQLiteDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Dim urlStr As String = dr("URL").ToString()
                        importlist.Add(dr("ImportDate").ToString() & " - " & urlStr.Substring(0, Math.Min(50, urlStr.Length)) & "...")
                        urls.Add(urlStr)
                    End While
                End Using
            End Using

            Dim selectedUrl As String = ""
            If urls.Count = 1 Then
                selectedUrl = urls(0)
            Else
                Dim picked As String = ShowPickerDialog("Select which import to reload:", "Reimport Gallus", importlist, "Cancel")
                If String.IsNullOrEmpty(picked) Then Exit Sub
                selectedUrl = urls(importlist.IndexOf(picked))
            End If

            Dim imported As Integer = 0

            Using cmd As New SQLiteCommand(
            "SELECT * FROM GallusImport WHERE League=@League AND Date=@Date AND URL=@URL", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                cmd.Parameters.AddWithValue("@URL", selectedUrl)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using da As New SQLiteDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)

                    For Each importRow As DataRow In dt.Rows
                        Dim subName As String = ResolveProperPlayerName(importRow("ResolvedName").ToString())
                        Dim gallusName As String = importRow("GallusName").ToString()

                        Dim origName As String = ""
                        If gallusName.Contains("-") Then
                            origName = gallusName.Substring(gallusName.IndexOf("-") + 1).Trim()
                            origName = ResolveProperPlayerName(origName)
                        End If

                        ' Read all 18 holes
                        Dim allScores(17) As Integer
                        For i As Integer = 0 To 17
                            Dim colName As String = "H" & (i + 1).ToString()
                            If Not IsDBNull(importRow(colName)) Then
                                allScores(i) = CInt(importRow(colName))
                            End If
                        Next

                        ' Use correct 9 based on FrontBack stored in record
                        Dim frontBack As String = importRow("FrontBack").ToString()
                        Dim scores(8) As Integer
                        If frontBack = "Front" Then
                            For i As Integer = 0 To 8
                                scores(i) = allScores(i)        ' H1-H9
                            Next
                        Else
                            For i As Integer = 0 To 8
                                scores(i) = allScores(i + 9)    ' H10-H18
                            Next
                        End If

                        ' Find target row
                        Dim searchName As String = If(String.IsNullOrEmpty(origName), subName, origName)
                        Dim targetRow As DataGridViewRow = FindGridRowByFullName(searchName)
                        If targetRow Is Nothing Then
                            LOGIT("Reimport: could not find row for " & subName)
                            Continue For
                        End If

                        ' Register sub if needed
                        If Not String.IsNullOrEmpty(origName) AndAlso
                       Not origName.Equals(subName, StringComparison.OrdinalIgnoreCase) Then
                            Dim nameParts() As String = subName.Split(" "c)
                            Dim firstName As String = If(nameParts.Length > 1, nameParts(0), "")
                            Dim lastName As String = nameParts.Last()
                            RegisterSub(subName, origName, firstName, lastName)
                        End If

                        ' Fill grid
                        EditScoresEngine._suppressEvents = True
                        targetRow.Cells("Player").Value = subName
                        For i As Integer = 1 To 9
                            If dgScores.Columns.Contains(i.ToString()) Then
                                targetRow.Cells(i.ToString()).Value = If(scores(i - 1) > 0, scores(i - 1), Nothing)
                            End If
                        Next
                        EditScoresEngine._suppressEvents = False
                        EditScoresEngine.UpsertScore(targetRow, "1", scores(0))

                        imported += 1
                        LOGIT("Reimport: " & subName & " imported")
                    Next
                End Using
            End Using

            FinalizeImportedScores(dgScores, New List(Of Dictionary(Of String, Object)))
            LoadAllTabs()

            MessageBox.Show("Reimported " & imported.ToString() & " players from Gallus.",
            "Reimport Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            LOGIT("btnReimportGallus_Click Error: " & ex.Message)
            MessageBox.Show("Reimport error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class