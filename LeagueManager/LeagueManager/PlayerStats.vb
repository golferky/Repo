Public Class frmPlayerStats
    Dim oHelper As Helper
    Dim fromsizeW As Integer, lvsizeW As Integer
    Dim lv1 As New SI.Controls.LvSort

    Private Sub frmPlayerStats_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper

        With cbPlayer
            .DataSource = oHelper.dsLeague.Tables("dtPlayers")
            .DisplayMember = "Player"
            .ValueMember = "Name"
            .SelectedIndex = 0
        End With

        Me.Controls.Add(lv1)
        lv1.Top = 129
        lv1.Left = 15
        lv1.Width = 1187
        lv1.Height = 500
        lv1.View = View.Details

    End Sub

    Private Sub btnDisplayStats_Click(sender As System.Object, e As System.EventArgs) Handles btnDisplayStats.Click
        oHelper.DisplayScores(cbPlayer.SelectedValue, Me, lv1)
    End Sub
End Class