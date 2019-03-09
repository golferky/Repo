Public Class FinanceDetails
    Dim oHelper As New Helper
    Private Sub FinanceDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper

        BldScoresDataGridFromFile()
        '    rs.FindAllControls(Me)
        Me.Text = Me.Text & " - " & Main.cbLeagues.SelectedItem
    End Sub
    Sub BldScoresDataGridFromFile()
        Try
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            Dim sDetails = ""
            Dim dv As New DataView(oHelper.dsLeague.Tables("dtPayments"))
            dv.Sort = "Date"
            dv.RowFilter = "Player = " & "'" & oHelper.sPlayer & "'"

            For Each col As DataColumn In oHelper.dsLeague.Tables("dtPayments").Columns
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
            'wdt.Columns.Add("Earned")
            wdt.Columns.Add("Skins")
            wdt.Columns.Add("CTP")
            wdt.Columns.Add("Earned To Date")

            Dim dgcw As New DataGridViewTextBoxColumn
            'dgcw.Name = "Earned"
            'dgcw.ValueType = GetType(System.String)
            'dgFinance.Columns.Add(dgcw)
            'dgcw = New DataGridViewTextBoxColumn
            dgcw.Name = "Skins"
            dgcw.Width = 30
            dgFinance.Columns.Add(dgcw)
            dgcw = New DataGridViewTextBoxColumn
            dgcw.Name = "CTP"
            dgcw.Width = 30
            dgFinance.Columns.Add(dgcw)
            dgcw = New DataGridViewTextBoxColumn
            dgcw.Name = "Earned To Date"
            dgFinance.Columns.Add(dgcw)

            Dim ibal As Integer = 0, iEarned = 0, iSkins = 0, iCTP = 0
            For Each row As DataRow In wdt.Rows
                If row("Detail").contains("Invoice") Then
                    ibal += row("Earned") * -1
                ElseIf row("Detail").contains("Payment") Then
                    ibal += row("Earned") * -1
                ElseIf row("DatePaid") Is DBNull.Value Then
                    ibal += row("Earned")
                End If

                If row("Desc").ToString.Contains("Skin") Then
                    iEarned += row("Earned")
                    iSkins += 1
                ElseIf row("Desc").ToString.Contains("CTP") Then
                    iEarned += row("Earned")
                    iCTP += 1
                ElseIf row("Desc").ToString.Contains("Reg Season") Or row("Desc").ToString.Contains("Club Champ") Then
                    iEarned += row("Earned")
                End If

                If row("Detail").contains("Payment") Then iEarned += row("Earned") * -1

                row("Balance") = ibal
                row("Skins") = iSkins
                row("CTP") = iCTP
                row("Earned To Date") = iEarned
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
        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try
    End Sub
End Class