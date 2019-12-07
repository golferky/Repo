'Imports System.IO.Packaging
'Imports System.Text

Public Class ScheduleBuilder

    Dim oHelper As New Helper
    Dim dtSchedule As DataTable
    'put here cause 2 subs use these
    Dim iNumWeeksSplit As Integer = 0
    Dim sHalfwayDate As String = 0
    'future move to league parm
    Dim sPositionRound As String = "N"
    Dim sPlayEachTeam As String = "Y"

    Dim iWeeks As Integer
    Dim iRnds As Integer
    Dim sPS As List(Of String)
    Dim bload As Boolean = True
    'Dim semailfile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    Dim semailfile = "" 'oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    Dim rs As New Resizer
    'Dim dtScheduler = New DataTable
    'Dim dvScheduler As DataView

    Private Sub Scheduler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'CenterGroupBoxText(gbCell)
        'CenterGroupBoxText(gbOptions)
        'CenterGroupBoxText(gbPoints)
        'rs.FindAllControls(Me)

        oHelper = Main.oHelper
        oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        lbStatus.Text = ""
        If tbStart.Text < oHelper.dDate.ToString("yyyyMMdd") Then
            cbSplitRounds.Enabled = False
            cbPS.Enabled = False
            cbPET.Enabled = False
            cbPR.Enabled = False
            tbRounds.Enabled = False
        End If

        If oHelper.rLeagueParmrow("SplitSeason") = "Y" Then cbSplitRounds.Checked = True
        If oHelper.rLeagueParmrow("PostSeasonDt").ToString IsNot DBNull.Value Then cbPS.Checked = True
        If sPlayEachTeam = "Y" Then
            tbRounds.Visible = True
            lbRounds.Visible = True
            cbPET.Checked = True
        End If
        If sPositionRound = "Y" Then
            cbPR.Checked = True
            gbPR.Visible = True
        End If
        'change to use league parm future for all 4 controls
        tbRounds.Text = "2"
        dgSchedule.Visible = False

        'dvScheduler = New DataView(dtScheduler)
        'dtScheduler = BuilddtScheduler()
        'Dim dstdate As DateTime = oHelper.dsLeague.Tables("dtSchedule").Columns(1).ColumnName & "/" & oHelper.dDate.ToString("yyyyMMdd").Substring(0, 4)
        Dim dstdate As DateTime = CDate(oHelper.rLeagueParmrow("StartDate"))
        tbStart.Text = dstdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

        'Dim denddate = dstdate.AddDays((oHelper.dsLeague.Tables("dtSchedule").Columns.Count - 1) * 7)
        Dim denddate As DateTime = CDate(oHelper.rLeagueParmrow("EndDate"))
        tbEnd.Text = denddate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

        iWeeks = DateDiff("w", dstdate, denddate)
        iRnds = iWeeks / oHelper.rLeagueParmrow("Teams")

        With dgTeams
            .RowHeadersVisible = False
            .Visible = True
            .Columns.Clear()
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AutoGenerateColumns = False
            .ReadOnly = True
            .Width = 0
            .Height = 30
            .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Tahoma", 12)

            Dim dgc As New DataGridViewTextBoxColumn
            dgc.Name = "Team"
            dgc.MaxInputLength = 2
            dgc.ValueType = GetType(System.String)
            dgc.HeaderText = dgc.Name
            dgc.Width = 40
            .Columns.Add(dgc)
            .Width += dgc.Width

            Dim dgc2 As New DataGridViewTextBoxColumn
            dgc2.Name = "A Player"
            dgc2.MaxInputLength = 30
            dgc2.ValueType = GetType(System.String)
            dgc2.HeaderText = dgc2.Name
            dgc2.Width = 150
            .Columns.Add(dgc2)
            .Width += dgc2.Width

            Dim dgc3 As New DataGridViewTextBoxColumn
            dgc3.Name = "B Player"
            dgc3.MaxInputLength = 30
            dgc3.ValueType = GetType(System.String)
            dgc3.HeaderText = dgc3.Name
            dgc3.Width = 150
            .Columns.Add(dgc3)
            .Width += dgc3.Width

            'create array from above defined fields we want out of scorecard
            Dim dvTeam As New DataView(oHelper.dsLeague.Tables("dtPlayers"))
            dvTeam.RowFilter = "Grade = 'A' or Grade = 'B'"
            'added sort by match(partner)
            dvTeam.Sort = "Team"
            Dim dtTeam As DataTable = dvTeam.ToTable(True, "Team,Name".Split(",").ToArray)
            dtTeam.Columns.Add("BPlayer")
            dtTeam.Columns("Name").ColumnName = "APlayer"
            Dim iTeam = "", aPlayer = ""
            For Each row As DataRow In dtTeam.Rows
                'combine a and b players into 1 row 
                If iTeam <> "" Then
                    If row("Team") = iTeam Then
                        row("BPlayer") = row("APlayer")
                        row("APlayer") = aPlayer
                        .Rows.Add(row.ItemArray)
                    Else
                        aPlayer = row("APlayer")
                        iTeam = row("Team")
                    End If
                Else
                    aPlayer = row("APlayer")
                    iTeam = row("Team")
                    'row.Delete()
                End If
            Next
            .Width += 10
            .Height += .Rows.Count * dgTeams.Rows(0).Height
        End With

        doScheduler()
        bload = False
    End Sub
    Sub doScheduler()
        'Sub BldStandings()
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            dgSchedule.Visible = True
            Dim sfilename As String = Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) &
                               "_" & Main.cbLeagues.SelectedItem.ToString.Substring(0, Main.cbLeagues.SelectedItem.ToString.IndexOf("(") - 1) & "_Schedule.csv"
            Dim dt = New DataTable
            oHelper.CSV2DataTable(dt, oHelper.sFilePath & "\" & sfilename)
            sPS = New List(Of String)
            With dgSchedule
                '.Columns.Clear()
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .RowHeadersVisible = False
                .Width = 30
                '.Columns.Clear()
                .DataSource = dt
                .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
                .DefaultCellStyle.Font = New Font("Tahoma", 12)
                'adjust columns for each field being added
                Dim i = 1
                For Each col As DataGridViewTextBoxColumn In .Columns
                    'col.ReadOnly = True            'make all columns read only
                    'If col.Name.Contains("/") Then      'is this a date column
                    .Columns(col.Name).Width = 45  'yep, make width 40
                    oHelper.LOGIT(String.Format("col {0} ", col.Name))
                    .Columns(col.Name).HeaderText = col.Name.Substring(0, col.Name.LastIndexOf("/")) 'just use mm/dd
                    .Columns(col.Name).DefaultCellStyle.BackColor = Color.White
                    .Width += .Columns(col.Name).Width
                    col.SortMode = DataGridViewColumnSortMode.NotSortable
                    'End If
                    If cbSplitRounds.Checked Then
                        If dgSchedule.Rows(0).Cells(col.Name).Value IsNot DBNull.Value Then
                            If i = oHelper.rLeagueParmrow("Teams") Then
                                .Columns(col.Name).DefaultCellStyle.BackColor = Color.LightBlue
                                i = 0
                            End If
                        Else
                            .Columns(col.Name).DefaultCellStyle.BackColor = Color.LightPink
                        End If
                    End If
                    If cbPS.Checked Then
                        If col.Index > .Columns.Count - 3 Then
                            .Columns(col.Name).DefaultCellStyle.BackColor = Color.Orange
                            sPS.Add(col.Name)
                        End If
                    End If
                    i += 1
                Next

                .Height = 30
                For Each row As DataGridViewRow In .Rows
                    oHelper.LOGIT(String.Format("Row {0}", row.Index))
                    .Height += row.Height
                Next
                .Width -= 20

            End With

            '20190824-create html of schedule
            'oHelper.CreateHtmlFromDGV(dgSchedule, "Schedule", Me, lbStatus)

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub CenterGroupBoxText(Groupbox As GroupBox)
        Dim label As New Label
        label.Text = Groupbox.Text
        Groupbox.Text = ""
        label.Left = Groupbox.Left + (Groupbox.Width - label.Width) / 2
        label.Top = Groupbox.Top + 2 ' // 2 Is an example : adjust the constant
        label.Parent = Groupbox.Parent
        label.BringToFront()
    End Sub


    Private Sub dgScheduler_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSchedule.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim mbr As MsgBoxResult = MsgBox(String.Format("Do you want to flag this week {0} as a rainout or skip?", dgSchedule.Columns(e.ColumnIndex).Name), MsgBoxStyle.YesNo)
        If mbr = MsgBoxResult.No Then Exit Sub

        lbStatus.Text = String.Format("Skipping Column {0}", dgSchedule.Columns(e.ColumnIndex).Name)
        oHelper.status_Msg(lbStatus, Me)
        Dim iCurrCol As Integer = e.ColumnIndex
        Dim iadddays As Int16 = 7
        If cbPS.Checked Then iadddays += 14
        tbEnd.Text = Date.ParseExact(tbEnd.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo).AddDays(iadddays).ToString("yyyyMMdd")
        Dim aRow As DataRow
        aRow = oHelper.dsLeague.Tables("dtSchedule").NewRow
        aRow("Date") = tbEnd.Text
        oHelper.dsLeague.Tables("dtSchedule").Rows.Add(aRow)

        For i = oHelper.dsLeague.Tables("dtSchedule").Rows.Count - 2 To 0 Step -1
            For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                If col.ColumnName = dgSchedule.Columns(iCurrCol).Name Then Exit For
                If col.ColumnName <> "Date" Then
                    oHelper.dsLeague.Tables("dtSchedule").Rows(i + 1)(col.ColumnName) = oHelper.dsLeague.Tables("dtSchedule").Rows(i)(col.ColumnName)
                End If
            Next
        Next

        For Each row In oHelper.dsLeague.Tables("dtSchedule").Rows
            If row("Date") = CDate(dgSchedule.Columns(iCurrCol).Name).ToString("yyyyMMdd") Then
                oHelper.dsLeague.Tables("dtSchedule").Rows.Remove(row)
                Exit For
            End If
        Next

        lbStatus.Text = String.Format("Finished Skipping Column {0}", dgSchedule.Columns(e.ColumnIndex).Name)
        oHelper.status_Msg(lbStatus, Me)
        Dim dt As New DataTable

        For Each row As DataRow In oHelper.dsLeague.Tables("dtSchedule").Rows
            Dim sdate As String = Date.ParseExact(row("Date"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
            dt.Columns.Add(sdate)
        Next

        For i = 1 To oHelper.dsLeague.Tables("dtSchedule").Columns.Count - 1
            Dim newrow As DataRow
            newrow = dt.NewRow
            For Each row As DataRow In oHelper.dsLeague.Tables("dtSchedule").Rows
                Dim sdate As String = Date.ParseExact(row("Date"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                newrow(sdate) = row(i)
            Next
            dt.Rows.Add(newrow)
        Next

        Main.rebuildDates(oHelper.dsLeague.Tables("dtSchedule"))
        Dim sfilename As String = Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) &
                               "_" & Main.cbLeagues.SelectedItem.ToString.Substring(0, Main.cbLeagues.SelectedItem.ToString.IndexOf("(") - 1) & "_Schedule.csv"
        'col.ColumnName 
        '2019-12-07 - Update league parm with new dates
        oHelper.rLeagueParmrow("EndDate") = CDate(oHelper.rLeagueParmrow("EndDate")).AddDays(7).ToString("MM/dd/yyyy")
        oHelper.rLeagueParmrow("PostSeasonDt") = CDate(oHelper.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("MM/dd/yyyy")
        oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\LeagueParms.csv")
        lbStatus.Text = "Updated League Parm post season date"
        oHelper.status_Msg(lbStatus, Me)

        oHelper.DataTable2CSV(dt, oHelper.sFilePath & "\" & sfilename)
        lbStatus.Text = "Finished saving Schedule"
        oHelper.status_Msg(lbStatus, Me)
        'this restores the screen with the new schedule
        oHelper.CSV2DataTable(dt, oHelper.sFilePath & "\" & sfilename)
        doScheduler()
        'If MousePosition.Y < 0 Then
        'If sender.mouseenteredcelladdress < 0 Then
        '    Exit Sub
        'End If

        'If e.ColumnIndex = 0 Then
        '    Dim cell As DataGridViewTextBoxCell = sender.currentcell
        '    If cell.OwningColumn.Name = "Player" Then
        '        Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
        '        If mbResult = MsgBoxResult.Yes Then
        '            oHelper.bScoresbyPlayer = True
        '            oHelper.sPlayer = cell.Value
        '            Scores.Show()
        '            oHelper.bScoresbyPlayer = False
        '        End If
        '    End If
        'End If

    End Sub

    Private Sub dgSchedule_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        'rs.ResizeAllControls(Me)
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub dgSchedule_ColumnDividerDoubleClick(sender As Object, e As DataGridViewColumnDividerDoubleClickEventArgs) Handles dgSchedule.ColumnDividerDoubleClick
        Dim x = ""
    End Sub

    Private Sub btnEmail_Click(sender As Object, e As EventArgs) Handles btnEmail.Click

        Dim ToAddresses As New List(Of String)  '{"garyrscudder@gmail.com", "garyrscudder@gmail.com"}
        For Each player In oHelper.dsLeague.Tables("dtPlayers").Rows
            If oHelper.convDBNulltoSpaces(player("Email")).Trim <> "" Then
                ToAddresses.Add(player("Email"))
            End If
        Next

        Dim mbr = MsgBox(String.Format("are you ready to sent emails to {0} players?", ToAddresses.Count), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then
            Exit Sub
        End If
        'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
        Dim attachs() As String = {semailfile}
        Dim subject As String = "Standings Sheet"
        Dim body As String = semailfile
        Dim bresult = False
        If ToAddresses.Count > 0 Then
            bresult = oHelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
            If bresult Then
                MsgBox("mails sent successfully", MsgBoxStyle.Information)
            Else
                MsgBox(oHelper.GGmail.ErrorText, MsgBoxStyle.Critical)
            End If
        End If

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Private Sub dgSchedule_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSchedule.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        'lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        'oHelper.status_Msg(lbStatus, Me)

        'lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        'oHelper.status_Msg(lbStatus, Me)
    End Sub
    Private Sub dgSchedule_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgSchedule.SortCompare

        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub dgSchedule_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgSchedule.CellValueChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            'Dim dgr As DataGridViewRow = sender.currentrow
            'If dgr Is Nothing Then Exit Sub
            'If dgr.IsNewRow Then dgr.ReadOnly = False
            Dim dgc As DataGridViewCell
            dgc = sender.currentrow.cells(e.ColumnIndex)
            'If dgc.OwningColumn.Name = "Skins" Then
            '    recalcSkins(dgc)
            'ElseIf dgc.OwningColumn.Name = "Closest" Then
            '    recalcCTP(dgc)
            'End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Private Sub cbSplitRounds_CheckedChanged(sender As Object, e As EventArgs) Handles cbSplitRounds.CheckedChanged
        If bload Then Exit Sub
        doScheduler()

    End Sub

    Private Sub cbPS_CheckedChanged(sender As Object, e As EventArgs) Handles cbPS.CheckedChanged
        If bload Then Exit Sub
        doScheduler()
    End Sub

    Private Sub cbPR_CheckedChanged(sender As Object, e As EventArgs) Handles cbPR.CheckedChanged

        If Not bload Then
            If cbPR.Checked Then
                cbPRRS.Checked = True ' future get from parm
                gbPR.Visible = True
            Else
                cbPRRS.Checked = False ' future get from parm
                gbPR.Visible = False
            End If

            doScheduler()
        End If
    End Sub

    Private Sub cbPRRS_CheckedChanged(sender As Object, e As EventArgs) Handles cbPRRS.CheckedChanged

    End Sub

    Private Sub cbPRAW_CheckedChanged(sender As Object, e As EventArgs) Handles cbPRAW.CheckedChanged

    End Sub

    Private Sub cbPET_CheckedChanged(sender As Object, e As EventArgs) Handles cbPET.CheckedChanged
        If Not bload Then
            If cbPET.Checked Then
                tbRounds.Visible = True
                lbRounds.Visible = True
            Else
                tbRounds.Visible = False
                lbRounds.Visible = False
            End If
        End If

    End Sub

    Private Sub tbRounds_TextChanged(sender As Object, e As EventArgs) Handles tbRounds.TextChanged
        If bload Then Exit Sub
        Dim mbResult As MsgBoxResult
        If Not IsNumeric(tbRounds.Text) Then
            mbResult = MsgBox(String.Format("Rounds must be numeric > 0, you entered {0}...try again", tbRounds.Text), MsgBoxStyle.OkCancel)
            If mbResult = MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        mbResult = MsgBox("Note: Default is this will change the ending Date press OK to continue", MsgBoxStyle.OkCancel)
        If mbResult = MsgBoxResult.Ok Then
            'tbRounds.Text = 2
            'Date.ParseExact(cbDatesPlayers.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
            tbEnd.Text = Date.ParseExact(tbStart.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo) _
                        .AddDays(CInt(oHelper.rLeagueParmrow("Teams") - 1) * tbRounds.Text * 7).ToString("yyyyMMdd")
            doScheduler()
        End If

    End Sub

    Private Sub BtnCalcSch_Click(sender As Object, e As EventArgs) Handles btnCalcSch.Click
        doScheduler()
    End Sub

    Private Sub dgSchedule_RowHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSchedule.RowHeaderMouseClick
        Dim x = ""
    End Sub
End Class