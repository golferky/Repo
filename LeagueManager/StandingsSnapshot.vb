Public Class StandingsSnapshot
    Dim ohelper As New Helper
    Dim dvTeam As New DataView
    Dim dtTeam As DataTable

    Private Sub StandingsSnapshot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = Main.oHelper
        ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        LoadDGV(dgTeams, "1")
        LoadDGV(dgTeams2, "2")
    End Sub
    Sub LoadDGV(dgTeams As DataGridView, sHalf As String)
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
                Dim iTeam = "", aPlayer = "", iRnds As Integer = 0
                Dim dvscores As New DataView(ohelper.dsLeague.Tables("dtScores"))
                Dim wksdate As Date = ohelper.rLeagueParmrow("StartDate")
                Dim wkedate As Date = ohelper.dDate
                iRnds = DateDiff("w", wksdate, wkedate)
                If ohelper.rLeagueParmrow("SplitSeason") = "Y" Then iRnds /= 2
                If sHalf = "2" Then
                    wksdate = wksdate.AddDays((iRnds - 1) * 7)
                Else
                    wkedate = wksdate.AddDays((iRnds - 2) * 7)
                End If
                Dim sToDate As String = wkedate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                Dim sFromDate As String = wksdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                For Each row As DataRow In dtTeam.Rows
                    'combine a and b players into 1 row 
                    If iTeam <> "" Then
                        If row("Team") = iTeam Then
                            row("BPlayer") = row("APlayer")
                            row("APlayer") = aPlayer
                            Dim srowfilter = String.Format("Team = {0} AND Date >= '{1}' AND Date <= '{2}'", iTeam, sFromDate, sToDate)
                            ohelper.LOGIT(String.Format("rowfilter={0}", srowfilter))
                            dvscores.RowFilter = srowfilter
                            For Each drow As DataRowView In dvscores
                                If drow("Team_Points") Is DBNull.Value Then drow("Team_Points") = "0"
                            Next
                            Dim dPoints As Decimal = dvscores.ToTable.Compute("SUM(Points)", "")
                            dPoints += dvscores.ToTable.Compute("SUM(Team_Points)", "")
                            row("Points") = dPoints.ToString("##.0")
                            .Rows.Add(row.ItemArray)
                        Else
                            aPlayer = row("APlayer")
                            iTeam = row("Team")
                        End If
                    Else
                        aPlayer = row("APlayer")
                        iTeam = row("Team")
                        'row.Delete()
                    End If
                Next
                .Width += 10
                .Height += .Rows.Count * .Rows(0).Height
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
    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub
End Class