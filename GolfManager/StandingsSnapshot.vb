Public Class StandingsSnapshot
    Private ohelper As New Helper
    Private dvTeam As New DataView
    Private dtTeam As DataTable
    Private dvscores As DataView
    Private sFromDate As String
    Private sToDate As String
    Private iTeam As String = ""
    Private aPlayer As String = ""
    Private iRnds As Integer = 0

    Private Sub StandingsSnapshot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = Main.oHelper
        ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        For Each item In Main.cbDates.Items
            If item >= CDate(ohelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then Continue For
            cbDates.Items.Add(item)
        Next
        If cbDates.Items.Contains(ohelper.dDate.ToString("yyyyMMdd")) Then cbDates.SelectedIndex = cbDates.Items.IndexOf(ohelper.dDate.ToString("yyyyMMdd"))
        Dim sWH As String = ohelper.ScreenResize()
        If Me.Width >= sWH.Split(":")(0) Then
            Me.Width = sWH.Split(":")(0) - (sWH.Split(":")(0) * 0.1)
        End If
        If Me.Height >= sWH.Split(":")(1) Then
            Me.Height = sWH.Split(":")(1) - (sWH.Split(":")(1) * 0.1)
        End If

    End Sub

    Public Sub LoadDGV(dgTeams As DataGridView)
        ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Try
            dvTeam = New DataView(ohelper.dsLeague.Tables("dtPlayers"))
            dvTeam.RowFilter = "Grade = 'A' or Grade = 'B'"
            'added sort by match(partner)
            dvTeam.Sort = "Team"
            dtTeam = dvTeam.ToTable(True, "Team,Name".Split(",").ToArray)
            dtTeam.Columns.Add("BPlayer")
            dtTeam.Columns.Add("Points", GetType(Decimal))
            dtTeam.Columns("Name").ColumnName = "APlayer"

            With dgTeams
                .RowHeadersVisible = False
                .Visible = True
                .Columns.Clear()
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AutoGenerateColumns = False
                .ReadOnly = True
                .Width = 0
                .Height = 30
                .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
                .DefaultCellStyle.Font = New Font("Tahoma", 12)

                ohelper.AddColumnToDGV(dgTeams, "Team", 2, 40)
                ohelper.AddColumnToDGV(dgTeams, "A Player", 30, 150)
                ohelper.AddColumnToDGV(dgTeams, "B Player", 30, 150)
                ohelper.AddColumnToDGV(dgTeams, "Points", 5, 50)

                'create array from above defined fields we want out of scorecard
                For Each row As DataRow In dtTeam.Rows
                    'combine a and b players into 1 row 
                    If iTeam <> "" Then
                        If row("Team") = iTeam Then
                            row("BPlayer") = row("APlayer")
                            row("APlayer") = aPlayer
                            Dim srowfilter = String.Format("Team = {0} AND Date >= '{1}' AND Date <= '{2}'", iTeam, sFromDate, sToDate)
                            ohelper.LOGIT(String.Format("rowfilter={0}", srowfilter))
                            Dim dPoints As Decimal = 0
                            dvscores.RowFilter = srowfilter
                            If dvscores.Count > 0 Then
                                For Each drow As DataRowView In dvscores
                                    If drow("Team_Points") Is DBNull.Value Then
                                        drow("Team_Points") = "0"
                                    End If
                                Next
                                dPoints += dvscores.ToTable.Compute("SUM(Points)", "")
                                dPoints += dvscores.ToTable.Compute("SUM(Team_Points)", "")
                            End If
                            row("Points") = dPoints.ToString("##.0")
                            .Rows.Add(row.ItemArray)
                        Else
                            aPlayer = row("APlayer")
                            iTeam = row("Team")
                        End If
                    Else
                        aPlayer = row("APlayer")
                        iTeam = row("Team")
                    End If
                Next
                .Width += 10
                .Height += (.Rows.Count * .Rows(0).Height)
                .Sort(dgTeams.Columns("Points"), System.ComponentModel.ListSortDirection.Descending)
            End With

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub dgTeams_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgTeams.SortCompare, dgTeams2.SortCompare

        Try
            ohelper.SortCompare_Dec(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub

    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged

        dvscores = New DataView(ohelper.dsLeague.Tables("dtScores"))
        'this changes the date back to mm/dd/yyyy
        ohelper.dDate = Date.ParseExact(cbDates.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)

        Dim dvsch = New DataView(ohelper.dsLeague.Tables("dtSchedule"))
        Dim ihalf = (dvsch.Count / 2) - 1

        sFromDate = cbDates.Items(cbDates.Items.Count - 1)
        If cbDates.SelectedIndex = 0 Then iRnds = cbDates.Items.Count

        If cbDates.Items.Count > ihalf And cbDates.SelectedIndex < ihalf Then
            gb2ndHalf.Visible = True
            'todate for first half is computed by
            'take all dates in schedule substract 2(league championship) divide by 2
            sToDate = dvsch((iRnds - 2) / 2)(Constants.datecon)
            LoadDGV(dgTeams)
            'todate for first half is computed by
            sFromDate = dvsch(iRnds / 2)(Constants.datecon)
            sToDate = cbDates.SelectedItem
            LoadDGV(dgTeams2)
        Else
            gb2ndHalf.Visible = False
            sToDate = cbDates.SelectedItem
            LoadDGV(dgTeams)
        End If
        If Me.Height < dgTeams.Height + 150 Then Me.Height *= 1.1
        If gb1stHalf.Height <= dgTeams.Height Then
            gb1stHalf.Height *= 1.1
            gb2ndHalf.Height *= 1.1
        End If
    End Sub

End Class