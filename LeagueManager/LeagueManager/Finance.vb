'Imports Microsoft.Office.Interop.Excel
Public Class Finance
    Dim sdesc As String
    Dim sDetail As String
    Dim idue = 0
    Dim iesCollected = 0
    Dim iLeagueBalance = 0
    Dim irscollected = 0
    Dim irspaidout = 0
    Dim iccpaidout = 0
    Dim iSkins = 0
    Dim iSkinspaidout = 0
    Dim iCTP = 0
    Dim iCTPpaidout = 0
    Dim iesPaidOut1 = 0
    Dim iesPaidOut2 = 0
    Dim iecPaidOut1 = 0
    Dim iecPaidOut2 = 0
    Dim iExpenses = 0
    Dim bfirst As Boolean = False
    Dim oHelper As LeagueManager.Helper
    Private Sub Finance_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '20180928 to use interop excel for worksheets
        '        Dim oXL As Microsoft.Office.Interop.Excel.Application
        '        Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
        '        Dim oWB As Microsoft.Office.Interop.Excel.Workbook
        '        Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
        '        Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
        '        Dim oRG As Microsoft.Office.Interop.Excel.Range
        '        oXL = New Microsoft.Office.Interop.Excel.Application
        '        oXL.Visible = False
        '        oYB = oXL.Workbooks

        '        oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20180917_Payments.csv")
        '        oWB = oYB.Item(1)
        '        oSheets = oWB.Worksheets
        '        oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
        '        oSheet.Name = "Sheet1"

        '        oWB.SaveAs("\\wdmycloud\Gary\LeagueManager\Files\20180917_Payments_" +
        'Date.Now.ToString("yyyyMMddhhmmss") + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7)
        '        oWB.Close()
        '        oXL.Quit()

        oHelper = Main.oHelper
        'oHelper.dt = New DataTable
        'Dim safile = "\\wdmycloud\Gary\LeagueManager\Files\20180917_Payments.csv"
        'If Not oHelper.CSV2DataTable(oHelper.dt, safile) Then
        '    MsgBox(String.Format("File in use - {1} {0} close file and restart", vbCrLf, safile))
        '    End
        'End If
        Dim dtr As New DataTable
        dtr.Columns.Add("Player")
        dtr.Columns.Add("Balance Due", GetType(Decimal))
        dtr.Columns.Add("Earned", GetType(Decimal))
        dtr.Columns.Add("#Skins", GetType(Decimal))
        dtr.Columns.Add("#CTP", GetType(Decimal))
        dtr.Columns.Add("$Skins", GetType(Decimal))
        dtr.Columns.Add("$CTP", GetType(Decimal))
        dtr.Columns.Add("Reg Season Champ", GetType(Decimal))
        dtr.Columns.Add("Club Champ", GetType(Decimal))
        dtr.Columns.Add("EOY Skins Wk1", GetType(Decimal))
        dtr.Columns.Add("EOY Skins Wk2", GetType(Decimal))
        dtr.Columns.Add("EOY CTP Wk1", GetType(Decimal))
        dtr.Columns.Add("EOY CTP Wk2", GetType(Decimal))

        dtr.PrimaryKey = New DataColumn() {dtr.Columns("Player")}
        dtr.DefaultView.Sort = "Player Asc"
        Dim dv As New DataView(oHelper.dsLeague.Tables("dtPayments"))
        Dim sdate = Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) & "0101"
        dv.RowFilter = String.Format("Date > {0} And Date < {1} ", sdate, sdate.Replace("0101", "1231"))
        For Each row As DataRowView In dv
            Dim xy = row(1).ToString
            oHelper.sPlayer = row("Player").ToString
            sdesc = row("Desc")
            sDetail = row("Detail")
            calcAmount(dtr, row)
        Next

        Dim iEarned = iSkinspaidout + iCTPpaidout + irspaidout + iccpaidout + iesPaidOut1 + iesPaidOut2 + iecPaidOut1 + iecPaidOut2
        dtr.Rows.Add("*** Totals ***", idue, iEarned, iSkins, iCTP, iSkinspaidout, iCTPpaidout, irspaidout, iccpaidout,
                     iesPaidOut1, iesPaidOut2, iecPaidOut1, iecPaidOut2)

        tbDue.Text = idue
        tbRSCollected.Text = irscollected
        tbRSPaidOut.Text = irspaidout
        tbESCollected.Text = iesCollected
        tbESPaidOut.Text = iesPaidOut1 + iesPaidOut2 + iecPaidOut1 + iecPaidOut2
        tbExpenses.Text = iExpenses
        tbTotalPO.Text = iExpenses + irspaidout
        For Each col As DataColumn In dtr.Columns
            Dim dgc As New DataGridViewTextBoxColumn
            dgc.Name = col.ColumnName
            dgc.ValueType = GetType(System.String)
            If dgc.Name = "Player" Then
                dgc.Width = 100
                dgc.HeaderText = "Player      (Blue <> EOY)"
            ElseIf dgc.Name = "Earned" Then
                dgc.HeaderText = "Earned (Pays Excl)"
                dgc.Width = 60
            Else
                dgc.Width = 60
            End If

            dgFinance.Columns.Add(dgc)
        Next

        For Each row As DataRow In dtr.Rows
            If row("Earned") Is DBNull.Value Then row("Earned") = 0
            If row("#Skins") Is DBNull.Value Then row("Skins") = 0
            If row("#CTP") Is DBNull.Value Then row("CTP") = 0
            If row("$Skins") Is DBNull.Value Then row("Skins") = 0
            If row("$CTP") Is DBNull.Value Then row("CTP") = 0
            'create a dataview of just payments
            If row("Player").ToString.Contains("***") Or row("Balance Due") > 0 Then
            Else
                'this code checks to see if a player participated in EOY skins
                Dim dvs As New DataView(oHelper.dsLeague.Tables("dtPayments"))
                dvs.RowFilter = String.Format("Desc = '{0}' And Detail = '{1}' And Date >= '{2}'", "EOY Skins", "Payment", sdate)
                Dim dt = dvs.ToTable
                dt.PrimaryKey = New DataColumn() {dt.Columns("Player")}
                Dim sKeys() As Object = {row("Player")}
                Dim dr As DataRow = dt.Rows.Find(sKeys)
                If dr Is Nothing Then row("Player") = "* " & row("Player")
            End If

            dgFinance.Rows.Add(row.ItemArray)
        Next
        For Each row As DataGridViewRow In dgFinance.Rows
            Dim splayer = row.Cells("Player").Value
            If splayer IsNot Nothing Then
                If splayer.StartsWith("* ") Then
                    row.Cells("Player").Value = splayer.Replace("* ", "")
                    row.Cells("Player").Style.BackColor = Color.LightBlue
                End If
            End If
        Next
        Me.Text = Me.Text & " - " & Main.cbLeagues.SelectedItem
        lbStatus.Text = ""

    End Sub
    Sub CalcAmount(dtr As DataTable, row As DataRowView)
        Dim sKeys() As Object = {row("Player")}
        Dim dr As DataRow = dtr.Rows.Find(sKeys)
        'build a new row if it doesnt exist
        If dr Is Nothing Then
            dtr.Rows.Add(row("Player"), 0, 0, 0, 0, 0, 0)
            dr = dtr.Rows.Find(sKeys)
        End If

        If row("Desc") = "Skin" Then
            dr("#Skins") += 1
            dr("$Skins") += row("Earned")
            iSkins += 1
            iSkinspaidout += row("Earned")
        ElseIf row("Desc") = "CTP" Then
            dr("#CTP") += 1
            dr("$CTP") += row("Earned")
            iCTP += 1
            iCTPpaidout += row("Earned")
        ElseIf sdesc = "League Dues" Then
            If row("Detail") = "Invoice" Then
                If row("DatePaid") Is DBNull.Value Then
                    idue += row("Earned") * -1
                    dr("Balance Due") = row("Earned") * -1
                End If
            ElseIf row("Detail") = "Payment" Then
                'add to total collected
                irscollected += row("Earned")
            End If
            Exit Sub
        ElseIf sdesc = "Reg Season Champ" Then
            dr("Reg Season Champ") += row("Earned")
            irspaidout += row("Earned")
        ElseIf sdesc = "Club Champion" Then
            dr("Club Champ") += row("Earned")
            iccpaidout += row("Earned")
            '    irspaidout += row("Earned")
        ElseIf sdesc.Contains("EOY Skins") Then
            If row("Detail") = "Payment" Then
                iesCollected += row("Earned")
                Exit Sub
            ElseIf row("Detail") = "Invoice" Then
                If row("DatePaid") Is DBNull.Value Then idue += row("Earned") * -1
                Exit Sub
            Else
                dr("EOY Skins Wk" & sdesc.Substring(Len(sdesc) - 1, 1)) += row("Earned")
                If sdesc.Substring(Len(sdesc) - 1, 1) = 1 Then iesPaidOut1 += row("Earned")
                If sdesc.Substring(Len(sdesc) - 1, 1) = 2 Then iesPaidOut2 += row("Earned")
            End If
        ElseIf sdesc.Contains("EOY CTP") Then
            If row("Detail") = "Payment" Then
                iesCollected += row("Earned")
                Exit Sub
            Else
                dr("EOY CTP Wk" & sdesc.Substring(Len(sdesc) - 1, 1)) += row("Earned")
                If sdesc.Substring(Len(sdesc) - 1, 1) = 1 Then iecPaidOut1 += row("Earned")
                If sdesc.Substring(Len(sdesc) - 1, 1) = 2 Then iecPaidOut2 += row("Earned")
            End If
        ElseIf row("Detail") = "Charge" Then
            iExpenses += row("Earned")
            dr("Balance Due") = row("Earned")
            row("Earned") = 0
        End If
        'roll into this players earned amount
        dr("Earned") += row("Earned")

    End Sub
    Private Sub dgFinance_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgFinance.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            Dim row As DataGridViewRow = sender.currentrow
            '20180225-fix Mouse click to expand columns
            If e.ColumnIndex = 0 Then
                If cell.OwningColumn.Name = "Player" Then
                    Dim mbResult As MsgBoxResult = MsgBox("List detailed finances For " & cell.Value.ToString.Replace("* ", "") & "?", MsgBoxStyle.YesNo)
                    If mbResult = MsgBoxResult.Yes Then
                        oHelper.sPlayer = cell.Value.ToString.Replace("* ", "")
                        FinanceDetails.Show()
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub dgFinance_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgFinance.SortCompare

        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub dgFinance_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgFinance.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)

        lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
    End Sub

End Class