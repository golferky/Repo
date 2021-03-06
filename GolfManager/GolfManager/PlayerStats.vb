﻿Public Class PlayerStats
    Dim oHelper As New Helper
    Dim fromsizeW As Integer, lvsizeW As Integer
    Dim rs As New Resizer

    Private Sub frmPlayerStats_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Me.Cursor = Cursors.WaitCursor
        'Application.DoEvents()
        'testing only
        'oHelper.sPlayer = "Gary Scudder"
        oHelper.bScoresbyPlayer = True
        'oHelper.iHdcp = row.Cells("Phdcp").Value
        'end testing only
        Dim dtPlayers As DataTable = Nothing
        Try
            '20190408-this makes a copy of tables
            oHelper = Main.oHelper
            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvScores.Sort = "Player"
            dtPlayers = dvScores.ToTable(True, "Player")
            'dtPlayers = dvPlayers.ToTable(True, "Name".Split(",").ToArray)
            For Each col As DataColumn In dtPlayers.Columns
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = col.ColumnName
                dgc.ValueType = GetType(System.String)
                dgPlayers.Columns.Add(dgc)
            Next

            For Each row As DataRow In dtPlayers.Rows
                dgPlayers.Rows.Add(row.ItemArray)
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
        End Try

        Me.Cursor = Cursors.Default
        Application.DoEvents()
        'Me.Controls.Add(lv1)
        'lv1.Top = 129
        'lv1.Left = 15
        'lv1.Width = 1187
        'lv1.Height = 500
        'lv1.View = View.Details
        'Scores.Show()
    End Sub

    Private Sub dgPlayers_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgPlayers.CellContentClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            Dim row As DataGridViewRow = sender.currentrow
            oHelper.bScoresbyPlayer = True
            oHelper.sPlayer = cell.Value
            Scores.Show()
            oHelper.bScoresbyPlayer = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub frmPlayerStats_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            rs.ResizeAllControls(Me)
            oHelper.LOGIT(String.Format("Form Height {0} Width {1}", Me.Height, Me.Width))
        Catch ex As Exception

        End Try
    End Sub
End Class