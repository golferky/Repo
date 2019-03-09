Public Class LastFive
    Dim oHelper As Helper
    Dim fromsizeW As Integer, lvsizeW As Integer
    Dim lv1 As New SI.Controls.LvSort

    Private Sub btnDisplayScores_Click(sender As Object, e As EventArgs) Handles btnDisplayScores.Click
        '20180220-fix issue when no date entered
        If cbDates.SelectedItem Is Nothing Then
            MsgBox("Please select a date and try again")
            Exit Sub
        End If
        oHelper.DisplayLast5(cbDates.SelectedItem, Me, lv1)
    End Sub

    Private Sub frmPlayerStats_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper

        If oHelper.bsch Then
            For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                Dim wkdate As DateTime = col.ColumnName
                'Dim wkdate As DateTime = DateTime.ParseExact(col.ColumnName, "MM/dd/yy", Globalization.CultureInfo.InvariantCulture)
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                cbDates.Items.Add(reformatted)
            Next
        End If

        Me.Controls.Add(lv1)
        lv1.Top = 129
        lv1.Left = 15
        lv1.Width = 400
        lv1.Height = 1000
        lv1.View = View.Details
        lv1.Name = "Last 5"
    End Sub


End Class