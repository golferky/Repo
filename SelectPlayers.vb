Public Class frmSelectPlayers

    Private Sub SelectPlayers_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim oHelper = Main.oHelper
        Dim edt As DataTable = oHelper.CSV2DataTableOLEDB("\Scores.csv")
        Dim dvscores As DataView = New DataView(edt)
        dvscores.Sort = "Name,  Date desc"
        Dim sPrevPlayer = ""
        Try
            For Each row As DataRowView In dvscores
                If sPrevPlayer <> row.Item("Name").ToString Then
                    Dim xxx As String = row.Item("Name").ToString & "(" & row.Item("Hdcp").ToString & ")"
                    Dim lvrec = New ListViewItem(row.Item("Name").ToString & "(" & row.Item("Hdcp").ToString & ")")
                    clbPlayers.Items.Add(lvrec)
                    sPrevPlayer = row.Item("Name").ToString
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class