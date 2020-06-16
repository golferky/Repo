Public Class Setup
    Public oHelper As Helper
    Private Sub Setup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
    End Sub
    Private Sub LeagueSetup_Click(sender As Object, e As EventArgs) Handles btnLeagueSetup.Click
        'frmLeagueSetup.Show()
        'NewLeague.Show()
        Dim bs As New BindingSource
        bs.DataSource = oHelper.dsLeague.Tables("dtLeagueParms")
        LeagueSetup.dgvNL.DataSource = bs
        LeagueSetup.dgvNL.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        LeagueSetup.dgvNL.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        LeagueSetup.Show()

    End Sub
    Private Sub CourseSetup_Click(sender As System.Object, e As System.EventArgs) Handles btnCourseSetup.Click
        'Course.Show()
    End Sub

    Private Sub PlayerSetup_Click(sender As System.Object, e As System.EventArgs) Handles btnPlayerSetup.Click
        'frmPlayer.Show()
        Player.Show()
        'reload player table after updating
    End Sub

    Private Sub ScoreSetup_Click(sender As System.Object, e As System.EventArgs)
        'frmScoring.Show()
    End Sub

    Private Sub dtScore_ValueChanged(sender As Object, e As EventArgs)
        Dim dtp As DateTimePicker = DirectCast(sender, DateTimePicker)

        RefreshSelection(dtp)

    End Sub
    Dim currVal As DateTime
    Dim newVal As DateTime
    Dim valCheck As Boolean
    Dim currSelected As Selection = Selection.None

    Public Enum Selection
        None = 0
        Year = 1
        Month = 2
        Day = 3
    End Enum

    Private Sub CheckDTPSelection(dtp As DateTimePicker)
        valCheck = True
        currVal = dtp.Value
        SendKeys.Send("{UP}")
    End Sub

    Sub RefreshSelection(dtp As DateTimePicker)
        If valCheck Then
            newVal = dtp.Value

            If currVal.Year <> newVal.Year Then
                currSelected = Selection.Year
            ElseIf currVal.Month <> newVal.Month Then
                currSelected = Selection.Month
            ElseIf currVal.Day <> newVal.Day Then
                currSelected = Selection.Day
            End If

            dtp.Value = currVal
            valCheck = False
            fixdate()
        End If
    End Sub
    Sub fixdate()
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
        Dim solddate = oHelper.sDateLastScore
        oHelper.sDateLastScore = ""
        'For Each row In dtschedule.Rows
        '    If CDate(dtScore.Text).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) = row("Date") Then
        '        oHelper.sDateLastScore = CDate(dtScore.Text).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        '        Exit For
        '    End If
        'Next
        If oHelper.sDateLastScore = "" Then
            MsgBox("this date is not on the schedule, pick another")
            oHelper.sDateLastScore = solddate
        End If
        'oHelper.dDate = dtScore.Text
    End Sub
    Private Sub btnSchedule_Click(sender As Object, e As EventArgs) Handles btnSchedule.Click
        ScheduleBuilder.Show()
    End Sub

    Private Sub btnScoresSetup_Click(sender As Object, e As EventArgs) Handles btnScoresSetup.Click
        'frmScoring.Show()
    End Sub
End Class