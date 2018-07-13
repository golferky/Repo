'Imports Microsoft.Office.Interop.Excel
Public Class Finance

    Dim oHelper As LeagueManager.Helper
    Private Sub Finance_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim oXL As Microsoft.Office.Interop.Excel.Application
        Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
        Dim oWB As Microsoft.Office.Interop.Excel.Workbook
        Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
        Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
        Dim oRG As Microsoft.Office.Interop.Excel.Range
        oXL = New Microsoft.Office.Interop.Excel.Application
        oXL.Visible = False
        oYB = oXL.Workbooks

        oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20180703_Payments.csv")
        oWB = oYB.Item(1)
        oSheets = oWB.Worksheets
        oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
        oSheet.Name = "Sheet1"

        oWB.SaveAs("\\wdmycloud\Gary\LeagueManager\Files\20180703_Payments_" +
Date.Now.ToString("yyyyMMddhhmmss") + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7)
        oWB.Close()
        oXL.Quit()

        oHelper = Main.oHelper
        oHelper.dt = New DataTable
        oHelper.CSV2DataTable(oHelper.dt, "\\wdmycloud\Gary\LeagueManager\Files\20180703_Payments.csv")
        Dim dtr As New DataTable
        dtr.Columns.Add("Player")
        dtr.Columns.Add("Balance Due", GetType(Decimal))
        dtr.Columns.Add("Earned", GetType(Decimal))
        dtr.Columns.Add("Skins", GetType(Decimal))
        dtr.Columns.Add("CTP", GetType(Decimal))
        dtr.Columns.Add("Reg Season Champ", GetType(Decimal))
        dtr.Columns.Add("Club Champ", GetType(Decimal))
        dtr.Columns.Add("EOY Skins", GetType(Decimal))

        dtr.PrimaryKey = New DataColumn() {dtr.Columns("Player")}
        dtr.DefaultView.Sort = "Player Asc"
        Dim idue = 0, icollected = 0, iccCollected = 0, iLeagueBalance = 0, irscollected = 0, irspaidout = 0, iccpaidout = 0
        For Each row As DataRow In oHelper.dt.Rows
            Dim xy = row(1).ToString
            oHelper.sPlayer = row("Player").ToString
            Dim sdesc = row("Desc")
            Dim sDetail = row("Detail")
            'Dim sKeys() As Object = {"Gary Scudder"}
            If row("Detail").contains("Invoice") Then
                calcAmount(dtr, row)
            ElseIf row("DatePaid") Is DBNull.Value Then
                calcAmount(dtr, row)
            End If
            Dim sKeys() As Object = {row("Player")}
            Dim dr As DataRow = dtr.Rows.Find(sKeys)

            If dr Is Nothing Then
                dtr.Rows.Add(row("Player"), 0, 0, 0, 0)
                dr = dtr.Rows.Find(sKeys)
                'Debug.Print(dr("Player") & "-" & dr("Skins") & "-" & dr("CTP") & "-")
            End If

            If row("Desc") = "Skin " Then
                dr("Skins") += 1
                dr("Earned") += row("Due")
            End If
            If row("Desc") = "CTP" Then
                dr("CTP") += 1
                dr("Earned") += row("Due")
            End If
            If row("Detail") = "Invoice" Then
                If row("DatePaid") Is DBNull.Value Then
                    idue += row("Due") * -1
                End If
            ElseIf row("Detail") = "Payment" Then
                icollected += row("Due")
                If sdesc = "Club Champ" Then
                    iccCollected += row("Due")
                ElseIf sdesc = "League Dues" Then
                    irscollected += row("Due")
                End If
            End If

            'Debug.Print(row("Player") & "-" & row("Desc") & "-" & row("Due") & "-")
            'Console.WriteLine(row("Player") & "-" & row("Desc") & "-")

        Next
        tbDue.Text = idue
        tbCollected.Text = icollected
        tbRSCollected.Text = irscollected
        tbCCCollected.Text = iccCollected
        For Each col As DataColumn In dtr.Columns
            Dim dgc As New DataGridViewTextBoxColumn
            dgc.Name = col.ColumnName
            dgc.ValueType = GetType(System.String)
            dgFinance.Columns.Add(dgc)
        Next

        For Each row As DataRow In dtr.Rows
            If row("Earned") Is DBNull.Value Then row("Earned") = 0
            If row("Skins") Is DBNull.Value Then row("Skins") = 0
            If row("CTP") Is DBNull.Value Then row("CTP") = 0
            dgFinance.Rows.Add(row.ItemArray)
        Next

    End Sub
    Sub calcAmount(dtr As DataTable, row As DataRow)
        Dim sKeys() As Object = {row("Player")}
        Dim dr As DataRow = dtr.Rows.Find(sKeys)
        If dr Is Nothing Then
            dtr.Rows.Add(row("Player"), row("Due"))
        Else
            dr("Balance Due") += row("Due")
        End If

    End Sub
    Private Sub dgFinance_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgFinance.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            Dim row As DataGridViewRow = sender.currentrow
            '20180225-fix Mouse click to expand columns
            If e.ColumnIndex = 0 Then
                If cell.OwningColumn.Name = "Player" Then
                    Dim mbResult As MsgBoxResult = MsgBox("List detailed finances For For " & cell.Value & "?", MsgBoxStyle.YesNo)
                    If mbResult = MsgBoxResult.Yes Then
                        oHelper.sPlayer = cell.Value
                        FinanceDetails.Show()
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub dgFinance_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgFinance.SortCompare
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        Try
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            Main.oHelper.SortCompare(sender, e)
            'e.SortResult = If(CInt(e.CellValue1) < CInt(e.CellValue2), -1, 1)
            'e.Handled = True
        Catch
            Dim x = ""
        End Try
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
End Class