'Imports System.IO.Packaging
'Imports System.Text

Public Class ScheduleBuilder

    Dim oHelper As New Helper
    Dim dtSchedule As DataTable
    'put here cause 2 subs use these
    Dim iNumWeeksSplit As Integer = 0
    Dim sHalfwayDate As String = 0
    Dim sPositionRound As String = "N"
    Dim sPlayEachTeam As String = "N"
    Dim iWeeks As Integer
    Dim iRnds As Integer
    Dim sPS As List(Of String)
    Dim bload As Boolean = True
    'Dim semailfile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    Dim semailfile = "" 'oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    Dim rs As New Resizer
    Private Sub Scheduler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'CenterGroupBoxText(gbCell)
        'CenterGroupBoxText(gbOptions)
        'CenterGroupBoxText(gbPoints)
        'rs.FindAllControls(Me)

        oHelper = Main.oHelper
        oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        If oHelper.rLeagueParmrow("SplitSeason") = "Y" Then cbSplitRounds.Checked = True
        If oHelper.rLeagueParmrow("PostSeasonDt").ToString IsNot DBNull.Value Then cbPS.Checked = True
        If sPlayEachTeam = "Y" Then cbPET.Checked = True
        If sPositionRound = "Y" Then
            cbPR.Checked = True
        End If
        gbPR.Visible = False
        lbStatus.Text = ""
        dgSchedule.Visible = False

        doScheduler()
        bload = False
    End Sub
    Sub doScheduler()
        'Sub BldStandings()
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

            With dgTeams
                .Visible = True
                .Columns.Clear()
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AutoGenerateColumns = False
                .ReadOnly = True
                .Width = 40
                .Height = 33
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
                .Width += 40
                .Height += .Rows.Count * dgTeams.Rows(0).Height
            End With

            dgSchedule.Visible = True

            Dim dtScheduler = New DataTable
            dtScheduler = BuilddtScheduler()

            Dim dvScheduler As New DataView(dtScheduler)

            sPS = New List(Of String)
            With dgSchedule
                .Columns.Clear()
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False

                .Width = 45
                '.Columns.Clear()
                .DataSource = dvScheduler
                .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
                .DefaultCellStyle.Font = New Font("Tahoma", 12)
                'adjust columns for each field being added
                Dim i = 1
                For Each col As DataGridViewTextBoxColumn In .Columns
                    'col.ReadOnly = True            'make all columns read only
                    If col.Name.Contains("/") Then      'is this a date column
                        .Columns(col.Name).Width = 45  'yep, make width 40
                        .Columns(col.Name).HeaderText = col.Name.Substring(0, col.Name.LastIndexOf("/")) 'just use mm/dd
                        .Width += .Columns(col.Name).Width
                    End If
                    If cbSplitRounds.Checked Then
                        If i = oHelper.rLeagueParmrow("Teams") - 1 Then
                            .Columns(col.Name).DefaultCellStyle.BackColor = Color.LightBlue
                            i = 0
                        End If
                    End If
                    If cbPS.Checked Then
                        If col.Index > iWeeks - 2 Then
                            .Columns(col.Name).DefaultCellStyle.BackColor = Color.Orange
                            sPS.Add(col.Name)
                        End If
                    End If
                    i += 1
                Next

                .Height = 33
                For Each row As DataGridViewRow In .Rows
                    Debug.Print(String.Format("Row {0}", row.Index))
                    If row.IsNewRow Then Exit For
                    If row.Cells(0).Value.ToString.Contains("v") Then row.ReadOnly = True
                    For Each ps As String In sPS
                        row.Cells(ps).Value = ""
                    Next
                    .Height += row.Height
                Next
                '.Width += 45

            End With

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Function BuilddtScheduler() As DataTable
        Return BuilddtScheduler(False)
    End Function
    Function BuilddtScheduler(email As Boolean) As DataTable
        BuilddtScheduler = Nothing
        Try
            'copy the schedule table 
            Dim dtScheduler As DataTable = oHelper.dsLeague.Tables("dtSchedule").Clone()

            Dim dstdate As DateTime = dtScheduler.Columns(0).ColumnName
            tbStart.Text = dstdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

            Dim denddate = dstdate.AddDays((dtScheduler.Columns.Count - 1) * 7)
            tbEnd.Text = denddate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

            iWeeks = DateDiff("w", dstdate, denddate)
            iRnds = iWeeks / oHelper.rLeagueParmrow("Teams")

            With dtScheduler
                .TableName = "dtScheduler"
                For i = iRnds To 0 Step -1
                    Dim iround As Integer = CInt(oHelper.rLeagueParmrow("Teams") - 1) * (i + 1)
                    If iround > iWeeks Then
                        iround = iWeeks
                    End If
                    If cbPR.Checked Then
                        .Columns.Add(String.Format("Round {0}", i + 1), GetType(String)).SetOrdinal(iround - 1)
                    End If
                Next
                Try
                    For Each sch As DataRow In oHelper.dsLeague.Tables("dtSchedule").Rows
                        Dim drow As DataRow
                        drow = .NewRow
                        For Each match As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                            drow(match.ColumnName) = sch(match.ColumnName)
                        Next
                        .Rows.Add(drow)
                    Next

                    Dim brow As DataRow
                    brow = .NewRow
                    For Each match As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                        brow(match.ColumnName) = ""
                    Next
                    .Rows.Add(brow)

                Catch ex As Exception

                End Try
            End With


            'dtScheduler.DefaultView.Sort = "Team"
            Return dtScheduler.DefaultView.ToTable
        Catch ex As Exception
            If ex.Message.ToUpper.Contains("CANNOT OPEN THE FILE") Then
                MsgBox("File is in use, close it and retry" & vbCrLf & oHelper.sFilePath & "Scores.csv" Or "-Players.csv")
            Else
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End If
        End Try
    End Function

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
        'If MousePosition.Y < 0 Then
        'If sender.mouseenteredcelladdress < 0 Then
        '    Exit Sub
        'End If

        If e.ColumnIndex = 0 Then
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            If cell.OwningColumn.Name = "Player" Then
                Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
                If mbResult = MsgBoxResult.Yes Then
                    oHelper.bScoresbyPlayer = True
                    oHelper.sPlayer = cell.Value
                    Scores.Show()
                    oHelper.bScoresbyPlayer = False
                End If
            End If
        End If

    End Sub

    Function getPts(sPts) As Decimal
        getPts = 0.0
        If sPts Is DBNull.Value Then Exit Function
        If sPts.Contains("-") Then
            Dim aPts As New List(Of String)(sPts.ToString.Split("-"))
            getPts += CDbl(aPts(0))
            If aPts.Count > 0 Then getPts += CDbl(aPts(1))
        Else
            getPts = sPts
        End If
    End Function
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

        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)

        lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
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
        If Not bload Then doScheduler()
    End Sub

    Private Sub cbPS_CheckedChanged(sender As Object, e As EventArgs) Handles cbPS.CheckedChanged
        If Not bload Then doScheduler()
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
        If cbPET.Checked Then
            tbRounds.Visible = True
        Else
            tbRounds.Visible = False
        End If

        doScheduler()
    End Sub

    Private Sub tbRounds_TextChanged(sender As Object, e As EventArgs) Handles tbRounds.TextChanged
        Dim mbResult As MsgBoxResult = MsgBox("Note: Default is this will change the ending Date press OK to continue", MsgBoxStyle.OkCancel)
        If mbResult = MsgBoxResult.Ok Then
            tbRounds.Text = 2
        End If

    End Sub
End Class