Public Class FinanceDetails
    Dim oHelper As New Helper
    Private Sub FinanceDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper

        BldScoresDataGridFromFile()
        '    rs.FindAllControls(Me)
    End Sub
    Sub BldScoresDataGridFromFile()
        Try
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            Dim sDetails = ""
            Dim dv As New DataView(oHelper.dt)
            dv.Sort = "Date"
            dv.RowFilter = "Player = " & "'" & oHelper.sPlayer & "'"

            For Each col As DataColumn In oHelper.dt.Columns
                Dim scol As String = col.ColumnName
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = scol
                dgc.ValueType = GetType(System.String)
                dgFinance.Columns.Add(dgc)
                sDetails = sDetails + dgc.Name & ","
            Next
            'sDetails = sDetails + "Earned,Skins,CTPs"
            'remove trailing column
            sDetails = sDetails.Substring(0, Len(sDetails) - 1)

            Dim wdt As DataTable = dv.ToTable(True, sDetails.Split(",").ToArray)
            wdt.Columns("Player").ColumnName = "Balance"
            wdt.Columns.Add("Earned")
            wdt.Columns.Add("Skins")
            wdt.Columns.Add("CTP")
            Dim dgcw As New DataGridViewTextBoxColumn
            dgcw.Name = "Earned"
            dgcw.ValueType = GetType(System.String)
            dgFinance.Columns.Add(dgcw)
            dgcw = New DataGridViewTextBoxColumn
            dgcw.Name = "Skins"
            dgFinance.Columns.Add(dgcw)
            dgcw = New DataGridViewTextBoxColumn
            dgcw.Name = "CTP"
            dgFinance.Columns.Add(dgcw)

            Dim ibal As Integer = 0, iEarned = 0, iSkins = 0, iCTP = 0
            For Each row As DataRow In wdt.Rows
                If row("Detail").contains("Invoice") Then
                    ibal += row("Due") * -1
                ElseIf row("DatePaid") Is DBNull.Value Then
                    ibal += row("Due") * -1
                End If
                If row("Desc") = "Skin " Then
                    iEarned += row("Due")
                    row("Earned") = iEarned
                    iSkins += 1
                ElseIf row("Desc") = "CTP" Then
                    iEarned += row("Due")
                    row("Earned") = iEarned
                    iCTP += 1
                End If
                row("Balance") = ibal
                row("Skins") = iSkins
                row("CTP") = iCTP
                dgFinance.Rows.Add(row.ItemArray)
            Next
            dgFinance.Columns.Item("Player").HeaderText = "Balance"
            Me.Text = "Financial Details for Player: " & oHelper.sPlayer
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
    Private Sub dgFinance_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgFinance.SortCompare
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        Try
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            oHelper.SortCompare(sender, e)
            '    Dim c1 As Integer
            '    If e.CellValue1 Is DBNull.Value Then
            '        c1 = 0
            '    Else
            '        c1 = e.CellValue1
            '    End If
            '    Dim c2 As Integer
            '    If e.CellValue2 Is DBNull.Value Then
            '        c2 = 0
            '    Else
            '        c2 = e.CellValue2
            '    End If
            '    e.SortResult = If(CInt(c1) < CInt(c2), -1, 1)

            '    e.Handled = True
        Catch
            Dim x = ""
        End Try
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
End Class