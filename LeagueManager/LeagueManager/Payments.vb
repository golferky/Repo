Public Class Payments
    Dim oHelper As New Helper
    Dim dsLeague As New dsLeague
    Dim sOldCellValue As String
    Dim semail As String = ""
    Dim sphone As String = ""
    Private Sub Payments_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            lbStatus.Text = String.Format("Loading Payments")
            oHelper.status_Msg(lbStatus, Me)
            'copy main's helper
            oHelper = Main.oHelper
            dsLeague = oHelper.dsLeague
            oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
            'dsLeague.Tables("dtPayments").PrimaryKey = New DataColumn() {dsLeague.Tables("dtPayments").Columns("Date"), dsLeague.Tables("dtPayments").Columns("Player"), dsLeague.Tables("dtPayments").Columns("Desc")}

            Dim dtschedule As New DataTable()
            'build a table of schedule with dates in rows instead of columns
            dtschedule = oHelper.buildSchedule()
            'reformat dates into yyyymmdd format
            For Each row In dtschedule.Rows
                If row(1) Is DBNull.Value Then
                    dtschedule.Rows.Remove(row)
                    Continue For
                End If
                Dim wkdate As Date = row("Date")
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                row("Date") = reformatted
            Next

            cbDate.Items.Clear()
            cbDate.Items.Add("All Dates")
            Dim dvsch As New DataView(dtschedule)
            For Each rv As DataRowView In dvsch
                'check the sch date against the last score date
                '20190314-only load dates < today plus one more score
                Dim x = Main.dtScore.Value.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                If rv(0).ToString > Main.dtScore.Value.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) Then Exit For
                cbDate.Items.Add(rv(0))
            Next

            cbDate.SelectedIndex = 0
            'cbDate.SelectedIndex = cbDate.Items.IndexOf(oHelper.sDateLastScore)
            'cbDate.SelectedItem = oHelper.sDateLastScore

            cbPlayers.Items.Clear()
            cbPlayers.Items.Add("All Players")

            For Each row In dsLeague.Tables("dtPlayers").Rows
                cbPlayers.Items.Add(row("Name"))
                cbPlayers.Sorted = True
            Next
            cbPlayers.SelectedIndex = 0
            Dim cbState As DataGridViewComboBoxColumn

            For Each sPlayer In cbPlayers.Items
                If sPlayer <> "All Players" Then
                    cbState = dgPayments.Columns("Player")
                    cbState.Items.Insert(0, sPlayer)
                    cbState.Sorted = True
                End If
            Next

            dgPayments.AutoSize = True

            If dgPayments.Width > Me.Width Then Me.Size = New System.Drawing.Size(dgPayments.Width + 100, Me.Size.Height) 'Me.Width = dgPayments.Width
            '20190320 - Increase the height of the form based on the datagrid + position
            If dgPayments.Height + dgPayments.Top > Me.Height Then
                Me.Size = New System.Drawing.Size(Me.Width, Me.Height + dgPayments.Top + 100) 'Me.Width = dgPayments.Width
            End If

            lbStatus.Text = String.Format("Finished Loading Payments")
            oHelper.status_Msg(lbStatus, Me)

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click

        If dgPayments.RowCount > 1 Then
            If sOldCellValue IsNot Nothing Then
                Dim mbr = MsgBox("Do you want to save Payments before you reload this screen", MsgBoxStyle.YesNo)
                If mbr = MsgBoxResult.No Then
                    lbStatus.Text = " Payments not saved"
                    lbStatus.BackColor = Color.LightGreen
                    oHelper.bDGSError = False
                Else
                    SavePayments()
                End If
            End If
        End If

        For Each R As DataGridViewRow In dgPayments.Rows
            If R.Cells("Comment").Value <> "No Receipt sent" Then
                'Dim mbr = MsgBox(String.Format("are you ready to sent emails to {0} players?", ToAddresses.Count), MsgBoxStyle.YesNo)
                'If mbr <> MsgBoxResult.Yes Then Exit Sub

                'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
                Dim sKeys() As Object = {R.Cells("Player").Value}
                Dim arow As DataRow = dsLeague.Tables("dtPlayers").Rows.Find(sKeys)
                Dim semail As String = ""
                If R.Cells("EmailText").Value = "Text" Then
                    If arow("Phone") IsNot DBNull.Value Then
                        semail = arow("Phone").ToString.Replace("-", "")
                        If semail.StartsWith("859962") Or
                       semail.StartsWith("859620") Then
                            semail = semail & "@txt.att.net"
                        ElseIf semail.StartsWith("859750") Then
                            'semail = semail & "@sms.myboostmobile.com"
                            semail = semail & "@myboostmobile.com"
                        ElseIf semail.StartsWith("859609") Then
                            semail = semail & "@vtext.com"
                        End If
                    End If
                ElseIf R.Cells("EmailText").Value = "Email" Then
                    semail = arow("Email").ToString
                End If
                If semail <> "" Then
                    Dim ToAddresses As New List(Of String)({semail}) '({"8599628088@txt.att.net", "‭8596200465@txt.att.net"}) '{"garyrscudder@gmail.com", "glemker@fuse.net"}
                    Dim attachs() As String = Nothing '{semailfile}
                    Dim subject As String = String.Format("***Test*** Receipt of Payment, reply to Gary Scudder if you get this message")
                    Dim body As String = String.Format("For {0} - ${1}", R.Cells("Description").Value, R.Cells("Amount").Value)
                    Dim bresult = False
                    bresult = oHelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
                    If bresult Then
                        oHelper.LOGIT(String.Format("payment text/email sent to {0}", R.Cells("Player").Value))
                    Else
                        oHelper.LOGIT(String.Format("payment text/email failed {0}", R.Cells("Player").Value))
                    End If
                End If
            End If
        Next

        Me.Close()
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        oHelper.LOGIT("--------------------------------------------------------------")
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            lbStatus.Text = "Getting Payments for this screen..."
            oHelper.status_Msg(lbStatus, Me)

            'SavePayments()

            '20190319 - columns build in designer
            Dim dv As New DataView(dsLeague.dtPayments)
            dv.RowFilter = "Detail in('Payment','Charge') "
                    If cbPlayers.SelectedItem <> "All Players" Then dv.RowFilter = dv.RowFilter & String.Format("AND Player = '{0}'", cbPlayers.SelectedItem)
            If cbDate.SelectedItem <> "All Dates" Then
                dv.RowFilter = dv.RowFilter & String.Format("AND Date = '{0}'", cbDate.SelectedItem)
            Else
                dv.RowFilter = dv.RowFilter & String.Format(" AND Date >= '{0}' AND Date <= '{1}'", Main.dtRSStart.Value.ToString("yyyyMMdd"), CDate(Main.tbPSEnd.Text).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
            End If
            dgPayments.Rows.Clear()

            For Each row As DataRowView In dv
                If row("PayMethod") Is DBNull.Value Then row("PayMethod") = "Cash"
                Dim sCol As String = row("Player") & "," & row("DatePaid") & "," & row("Desc")
                If row("Desc") = "EOY Skins" Then
                    If row("Comment") Is DBNull.Value Then
                        sCol = sCol & "," & row("Detail") & "," & row("Earned") & "," & row("PayMethod") & "," & row("Comment")
                        dgPayments.Rows.Add(sCol.Split(",").ToArray)
                    ElseIf row("Comment").ToString.Contains("1st Week") Then
                        sCol = sCol & " Wk1" & "," & row("Detail") & "," & row("Earned") & "," & row("PayMethod") & "," & row("Comment")
                        dgPayments.Rows.Add(sCol.Split(",").ToArray)
                    ElseIf row("Comment").ToString.Contains("2nd Week") Then
                        sCol = sCol & " Wk2" & "," & row("Detail") & "," & row("Earned") & "," & row("PayMethod") & "," & row("Comment")
                        dgPayments.Rows.Add(sCol.Split(",").ToArray)
                    End If
                ElseIf row("Desc") = "League Dues" Then
                    sCol = sCol & "," & row("Detail") & "," & row("Earned") & "," & row("PayMethod") & "," & row("Comment")
                    dgPayments.Rows.Add(sCol.Split(",").ToArray)
                ElseIf row("Detail") = "Charge" Then
                    sCol = sCol &  "," & row("Detail")  & "," & row("Earned") & "," & row("PayMethod") & "," & row("Comment")
                    dgPayments.Rows.Add(sCol.Split(",").ToArray)
                End If
            Next

            Dim x = dgPayments.RowCount
            lbStatus.Text = "Finished Payments for this screen..."
            oHelper.status_Msg(lbStatus, Me)

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Sub SavePayments()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If dgPayments.RowCount = 0 Then Exit Sub
            lbStatus.Text = "Saving payments from this screen..."
            oHelper.status_Msg(lbStatus, Me)
            'oHelper.bNoRowLeave = True
            For Each row As DataGridViewRow In dgPayments.Rows
                If row.IsNewRow Then Continue For
                oHelper.sPlayer = oHelper.convDBNulltoSpaces(row.Cells("Player").Value)
                If oHelper.sPlayer.Trim = "" Then
                    lbStatus.Text = String.Format("Invalid Player {0},  fix before saving", oHelper.sPlayer)
                    Exit Sub
                End If
                Dim arow As DataRow
                arow = dsLeague.dtPayments.NewRow
                '20181003 clear scores in table before saving from gridview
                For Each cell As DataGridViewCell In row.Cells
                    Dim col = cell.OwningColumn.Name
                    If col = "PayDate" Then
                        col = "Date"
                    ElseIf col = "Description" Then
                        col = "Desc"
                    ElseIf col = "Amount" Then
                        col = "Earned"
                    End If
                    arow(col) = cell.Value
                Next
                arow("Date") = oHelper.dDate.ToString("yyyyMMdd")
                arow("DatePaid") = Now.ToString("yyyyMMdd")
                arow("Detail") = "Payment"

                dsLeague.dtPayments.Rows.Add(arow)
            Next

            'oHelper.CopyDataGridViewToClipboard(dgLast5)
            Dim sfilename = oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "Pymts.csv"
            lbStatus.Text = String.Format("Creating spreadsheet({0}) of payments from this screen...", sfilename)
            oHelper.status_Msg(lbStatus, Me)
            oHelper.dgv2csv(dgPayments, sfilename)
            'oHelper.DataTable2XML("dtScores", "Scores")
            oHelper.DataTable2CSV(dsLeague.dtPayments, oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Payments.csv")
            lbStatus.Text = "Finished saving payments from this screen"
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub dgPayments_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgPayments.DataError
        'MsgBox(e.Exception.Message)

        Try
            '    dgPayments.EndEdit()
            '    MsgBox(e.Context.ToString)
        Catch ex As Exception
            '    MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub dgPayments_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgPayments.CellContentClick

        If dgPayments.ReadOnly Or e.RowIndex < 0 Then Exit Sub

        Dim R As DataGridViewRow = sender.CurrentRow

        oHelper.sPlayer = R.Cells("Player").Value
        Dim dgc As DataGridViewCell = sender.currentcell
        '20180810 added Clear column

    End Sub
    Private Sub dgPayments_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgPayments.CellBeginEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        sOldCellValue = ""
        If sender.currentcell.value IsNot DBNull.Value Then
            If sender.currentcell.value IsNot Nothing Then sOldCellValue = sender.currentcell.value
        End If
    End Sub
    Private Sub dgPayments_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgPayments.CellEndEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bDGSError = False
            Dim dgr As DataGridView = sender
            Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name

            If sCurrColName = "Clear" Then
                dgr.CurrentCell.Value = sOldCellValue
                Exit Sub
            End If
            '20180201-first time through if boolean changed-skins and ctp get edited in another event
            If sCurrColName = "Player" Then
                dgPayments.CurrentRow.Cells("PayDate").Value = Now.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                dgPayments.CurrentRow.Cells("Description").Value = "League Dues"
                dgPayments.CurrentRow.Cells("Amount").Value = "35"
                dgPayments.CurrentRow.Cells("PayMethod").Value = "Cash"
                Dim sKeys() As Object = {dgr.CurrentCell.Value}
                Dim arow As DataRow = dsLeague.Tables("dtPlayers").Rows.Find(sKeys)
                '20190327-this should never happen
                If arow Is Nothing Then
                    MsgBox(String.Format("Player doesnt exist {0}.  contact developer", sKeys(0)))
                Else
                    Dim dcb As New DataGridViewComboBoxCell
                    dcb = dgPayments.CurrentRow.Cells("EmailText")
                    dcb.Items.Clear()
                    dcb.Items.Add("None")
                    If arow("Email") IsNot DBNull.Value Then
                        dcb.Items.Add("Email")
                        semail = arow("Email")
                    End If
                    If arow("Phone") IsNot DBNull.Value Then
                        dcb.Items.Add("Text")
                        sphone = arow("Phone")
                    End If
                End If

            ElseIf sCurrColName = "Description" Then
                If dgr.CurrentCell.Value = "League Dues" Then
                    dgPayments.CurrentRow.Cells("Amount").Value = "35"
                    dgPayments.CurrentRow.Cells("PayMethod").Value = "Cash"
                ElseIf dgr.CurrentCell.Value = "EOY Skins" Then
                    dgPayments.CurrentRow.Cells("Amount").Value = "20"
                    dgPayments.CurrentRow.Cells("PayMethod").Value = "Cash"
                End If
                'if no change, then exit 
                If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
            ElseIf sCurrColName = "EmailText" Then
                If dgr.CurrentCell.Value = "Email" Then
                    dgPayments.CurrentRow.Cells("Comment").Value = String.Format("Emailed Receipt to {0}", semail)
                ElseIf dgr.CurrentCell.Value = "Text" Then
                    dgPayments.CurrentRow.Cells("Comment").Value = String.Format("Texted Receipt to {0}", sphone)
                Else
                    dgPayments.CurrentRow.Cells("Comment").Value = String.Format("No Receipt sent")
                End If
                'if no change, then exit 
                If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
            End If
            '20190321-check to see if this player has banked up money from skins
            'Dim sKeys() As Object = {dgPayments.CurrentRow.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
            'Dim arow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
            'Dim arows As DataRow() = dsLeague.dtPayments.Select(String.Format("#{0}# >= StartDate and #{0}# <=EndDate", dDate.ToString("MM/dd/yyyy")))

            'If arows IsNot Nothing Then
            '    Dim dEarned As Decimal = 0
            '    For Each row In arows
            '        dEarned += row("Earned")
            '    Next
            '    If dEarned < dgPayments.CurrentRow.Cells("Amount").Value Then
            '        lbStatus.Text = String.Format("Not enough money {0} made for Player {1}{2} you need to fix before saving", cell.Value, row.Cells("Player").Value, vbCrLf)
            '        lbStatus.BackColor = Color.Red
            '    End If
            'End If
            Dim R As DataGridViewRow = dgr.CurrentRow
            'If R.Cells("Phdcp").Value Is Nothing Then oHelper.IHdcp = 99

            If oHelper.bDGSError Then SendKeys.Send("+{TAB}")

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        SavePayments()
    End Sub

    Private Sub dgPayments_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgPayments.RowEnter
        semail = ""
        sphone = ""
    End Sub
#Region "Not_Used"
#End Region

End Class