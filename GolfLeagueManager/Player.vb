Public Class Player
    Dim oHelper As New Helper
    Dim stable = "dtPlayers"
    Dim sOldCellValue = ""
    Private Sub Player_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        rbAll.Checked = True
    End Sub
    Private Sub rbRegulars_CheckedChanged(sender As Object, e As EventArgs) Handles rbRegulars.CheckedChanged
        Dim BindingSource = New BindingSource()
        BindingSource.DataSource = oHelper.dsLeague.Tables(stable)
        BindingSource.Filter = "Team > 0 "
        DtPlayersDataGridView.DataSource = BindingSource
    End Sub

    Private Sub rbAll_CheckedChanged(sender As Object, e As EventArgs) Handles rbAll.CheckedChanged
        Dim BindingSource = New BindingSource()
        BindingSource.DataSource = oHelper.dsLeague.Tables(stable)
        BindingSource.Filter = ""
        DtPlayersDataGridView.DataSource = BindingSource
    End Sub
    Private Sub dtPlayersDataGridView_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DtPlayersDataGridView.CellBeginEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        If sender.currentcell.value IsNot DBNull.Value Then
            If sender.currentcell.value IsNot Nothing Then sOldCellValue = sender.currentcell.value
        End If

    End Sub
    Private Sub dtPlayersDataGridView_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DtPlayersDataGridView.CellEndEdit

        Dim dgc As DataGridViewCell = sender.currentcell
        If dgc.OwningColumn.HeaderText = "Name" Then

            If dgc.Value <> sOldCellValue Then
                Dim result = MsgBox(String.Format("Do you want to rename player {0} to {1} in all files?", sOldCellValue, dgc.Value), MsgBoxStyle.YesNo)
                If result = MsgBoxResult.Yes Then
                    Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                    dvScores.RowFilter = String.Format("Player = '{0}'", sOldCellValue)
                    For Each score As DataRowView In dvScores
                        score("Player") = dgc.Value
                        oHelper.LOGIT(String.Format("Updated Score {0}", score("Date")))
                    Next
                    oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\Scores.csv")
                    oHelper.LOGIT(String.Format("Updating {0}\Scores.csv", oHelper.sFilePath))
                    Dim dvPmts As New DataView(oHelper.dsLeague.Tables("dtPayments"))
                    dvPmts.RowFilter = String.Format("Player = '{0}'", sOldCellValue)
                    For Each pmt As DataRowView In dvPmts
                        pmt("Player") = dgc.Value
                        oHelper.LOGIT(String.Format("Updated Payment {0}", pmt("Date")))
                    Next
                    oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtPayments"), oHelper.sFilePath & "\Payments.csv")
                    oHelper.LOGIT(String.Format("Updating {0}\Payments.csv", oHelper.sFilePath))
                    'DtPlayersBindingSource.EndEdit()
                    oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtPlayers"), oHelper.sFilePath & "\Players.csv")
                    oHelper.LOGIT(String.Format("Updating {0}\Players.csv", oHelper.sFilePath))
                Else
                    dgc.Value = sOldCellValue
                End If
            End If
        End If

    End Sub

End Class