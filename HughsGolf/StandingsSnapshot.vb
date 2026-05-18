Imports System.Text

Public Class StandingsSnapshot
    Public ohelper As New Helper
    Private dvTeam As New DataView
    Private dtTeam As DataTable
    Public sFromDate As String
    Public sToDate As String
    Private iTeam As String = ""
    Private aPlayer As String = ""
    Public iRnds As Integer = 0
    Dim ihalf As Integer
    Public dvscores As DataView
    Dim strsql = New Text.StringBuilder
    Private Sub StandingsSnapshot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = Main.oHelper
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        For Each item In cbDates.Items
            If item >= CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then Continue For
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

    Function BuildSqlForMatches() As String

        Dim strsql = New StringBuilder
        strsql.AppendLine($"Select")
        strsql.AppendLine($"Ma.Player")
        strsql.AppendLine($",Ma.Date")
        strsql.AppendLine($",IFNULL(T.Team,Su.Team) As Team")
        strsql.AppendLine($",IFNULL(T.Grade,Su.Grade) As Grade")
        strsql.AppendLine($",Points")
        strsql.AppendLine($",Team_Points")

        strsql.AppendLine($"FROM [Matches] Ma")
        strsql.AppendLine($"LEFT JOIN [Scores]      S ON (S.League = Ma.League and S.Date = Ma.Date AND S.Player = Ma.Player)")
        strsql.AppendLine($"LEFT JOIN [Teams]       T ON (T.League = Ma.League and T.Year = SUBSTRING(Ma.Date,1,4) AND T.Player = Ma.Player)")
        strsql.AppendLine($"LEFT JOIN [Subs]        Su ON (Su.League = Ma.League and Su.Date = Ma.Date AND Su.Player = Ma.Player)")

        BuildSqlForMatches = strsql.ToString
    End Function
    Function CreatePoints() As DataTable

        Try
            Dim sFrom = sFromDate
            Dim sTo = sToDate
            CreatePoints = New DataTable("Points")

            strsql = New Text.StringBuilder
            strsql.AppendLine($"SELECT t.Team, player, grade FROM Teams t")
            strsql.AppendLine($"WHERE Year = {sFromDate.Substring(0, 4)}")
            'Debug.Print(strsql.ToString)

            Dim dtTeams = New DataTable("Teams")
            dtTeams = ohelper.sqlitedaFromSql(ctx.Conn, "Teams", strsql.ToString)
            dtTeams.PrimaryKey = New DataColumn() {dtTeams.Columns(Constants.Player)}

            Dim dtTeams2 = New DataTable("Teams")
            dtTeams2 = ohelper.sqlitedaFromSql(ctx.Conn, "Teams", strsql.ToString)
            dtTeams2.PrimaryKey = New DataColumn() {dtTeams2.Columns("Team"), dtTeams2.Columns("Grade")}

            strsql = New Text.StringBuilder
            strsql.AppendLine($"SELECT * FROM Subs ")
            strsql.AppendLine($"WHERE Date >= {sFromDate} AND Date <= {sToDate}")
            'Debug.Print(strsql.ToString)

            Dim dtSubs = New DataTable("Subs")
            dtSubs = ohelper.sqlitedaFromSql(ctx.Conn, "Subs", strsql.ToString)
            dtSubs.PrimaryKey = New DataColumn() {dtSubs.Columns(Constants.Player), dtSubs.Columns("Date")}

            strsql = New Text.StringBuilder
            Debug.Print("")
            strsql.AppendLine($"SELECT '' as SortOrder, A.Team, A.Player APlayer, B.Player BPlayer,SUM(S.Points) as Points,'' as PtsBack 
                        FROM [Matches] S
            LEFT JOIN [Teams] A ON (A.Year = {sFromDate.Substring(0, 4)} AND A.Player)
            LEFT JOIN [Teams] B ON (A.Year = {sFromDate.Substring(0, 4)} AND B.Player)
            WHERE S.Date >= {sFromDate} And S.Date <= {sToDate}
            GROUP BY A.Team,A.Player,B.Player")
            strsql = New Text.StringBuilder
            strsql.AppendLine($"SELECT '' as SortOrder, SUM(S.Points) as Points,'' as PtsBack 
                        FROM [Matches] S
            WHERE S.Date >= {sFromDate} And S.Date <= {sToDate}")

            ohelper.dt = ohelper.sqlitedaFromSql(ScoreCard.connection, "Points", strsql.ToString)

            CreatePoints.Columns.Add("SortOrder")
            CreatePoints.Columns.Add("Team")
            CreatePoints.Columns.Add("APlayer")
            CreatePoints.Columns.Add("BPlayer")
            CreatePoints.Columns.Add("Points", GetType(Decimal))
            CreatePoints.Columns.Add("PtsBack")
            CreatePoints.PrimaryKey = New DataColumn() {CreatePoints.Columns(Constants.Team)}
            Dim et As TimeSpan
            Dim sst As DateTime = Now
            Dim sql As String = BuildSqlForMatches()
            Dim dtScores = ohelper.sqlitedaFromSql(ctx.Conn, "Standings", $"select * from matches where date >= {sFromDate} and date <= {sToDate}")
            Dim trow As DataRow
            Dim sKey() As Object = Nothing
            For Each row In dtScores.Rows
                trow = dtTeams.Rows.Find(row(Constants.Player))
                If trow Is Nothing Then
                    sKey = {row(Constants.Player), row("Date")}
                    trow = dtSubs.Rows.Find(sKey)
                End If
                Dim arow = CreatePoints.Rows.Find(trow("Team"))
                If arow Is Nothing Then
                    arow = CreatePoints.NewRow
                    arow("Team") = trow("Team")
                    If IsNumeric(row("Points")) Then
                        arow("Points") = row("Points")
                    Else
                        arow("Points") = 0
                    End If
                    If IsNumeric(row("Team_Points")) Then arow("Points") += row("Team_Points")
                    sKey = {trow("Team"), trow("Grade")}
                    trow = dtTeams2.Rows.Find(sKey)
                    arow("APlayer") = trow("Player")
                    CreatePoints.Rows.Add(arow)
                Else
                    If IsNumeric(row("Points")) Then arow("Points") += row("Points")
                    If IsNumeric(row("Team_Points")) Then arow("Points") += row("Team_Points")
                    sKey = {trow("Team"), trow("Grade")}
                    trow = dtTeams2.Rows.Find(sKey)
                    arow("BPlayer") = trow("Player")
                End If
            Next
            'sum doest work with decimal fields using ms access must accum manually
            Dim x = ""
            'clear old points first
            For Each row As DataRow In CreatePoints.Rows
                row("SortOrder") = 0
                'row("Points") = 0
                row("PtsBack") = 0
            Next

            Dim iTotPts As Decimal = 0.0, iptr = 0
            trow = CreatePoints.NewRow
            trow("SortOrder") = 9
            trow("Team") = 99
            trow("APlayer") = "Total"
            trow("BPlayer") = ""
            trow("Points") = iTotPts
            trow("PtsBack") = ""
            CreatePoints.Rows.Add(trow)
            Dim dvpts = New DataView(CreatePoints)
            dvpts.Sort = "SortOrder,Points Desc"
            For Each row As DataRowView In dvpts
                If row("APlayer") = "Total" Then Continue For
                row("PtsBack") = dvpts(0)("Points") - row("Points")
                iptr += 1
                iTotPts += row("Points")
            Next
            trow = CreatePoints.Rows.Find(("99"))
            trow("Points") = iTotPts
            et = Now - sst
            'CreatedtFromfile.DefaultView.Sort = "SortOrder, Points Desc"
            CreatePoints = dvpts.ToTable
        Catch ex As Exception
            Debug.Print(strsql)
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function

    Private Sub dgTeams_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgTeams.SortCompare, dgTeams2.SortCompare

        Try
            ohelper.SortCompare_Dec(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub

    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        '20250105
        Exit Sub
        Main.lblProcessMsg.Text = String.Format("Calculating Standings Snapshot")
        ohelper.status_Msg(Main.lblProcessMsg, Me)

        dvscores = New DataView(ohelper.dsLeague.Tables("dtScores"))
        'this changes the date back to mm/dd/yyyy
        ohelper.dDate = Date.ParseExact(cbDates.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)

        Dim dvsch = New DataView(ohelper.dsLeague.Tables("dtSchedule"))
        ihalf = (dvsch.Count / 2) - 1
        iRnds = dvsch.Count
        'sFromDate = cbDates.Items(cbDates.Items.Count - 1)
        'If cbDates.Items.Count > ihalf And cbDates.SelectedIndex < cbDates.Items.Count - ihalf Then
        '    sToDate = cbDates.Items(cbDates.Items.Count - ihalf)
        'Else
        '    sToDate = cbDates.SelectedItem
        'End If
        sFromDate = cbDates.Items(0)
        sToDate = cbDates.SelectedItem
        If sToDate >= cbDates.Items(ihalf) Then
            sToDate = cbDates.Items(ihalf - 1)
        End If
        Main.lblProcessMsg.Text = String.Format("Calculating 1st Half Standings Snapshot")
        ohelper.status_Msg(Main.lblProcessMsg, Me)

        'LoadDGV(dgTeams)
        Dim sfn = ohelper.sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & ohelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) & String.Format("_StandingsSnapshot_1st.csv")

        Main.lblProcessMsg.Text = String.Format("Creating CSV of 1st Half Standings Snapshot")
        ohelper.status_Msg(Main.lblProcessMsg, Me)
        ohelper.dgv2csv(dgTeams, sfn)

        Main.lblProcessMsg.Text = String.Format("Creating XLS of 1st Half Standings Snapshot")
        ohelper.status_Msg(Main.lblProcessMsg, Me)
        createSpreadsheet(sfn)

        If cbDates.SelectedItem >= cbDates.Items(ihalf) Then
            gb2ndHalf.Visible = True
            'todate for first half is computed by
            'take all dates in schedule substract 2(league championship) divide by 2
            'todate for first half is computed by
            sFromDate = dvsch(ihalf)(Constants.datecon)
            sToDate = cbDates.SelectedItem
            Main.lblProcessMsg.Text = String.Format("Creating CSV of 2nd Half Standings Snapshot")
            ohelper.status_Msg(Main.lblProcessMsg, Me)

            'LoadDGV(dgTeams2)
            sfn = ohelper.sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & ohelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) & String.Format("_StandingsSnapshot_2nd.csv")
            ohelper.dgv2csv(dgTeams2, sfn)

            Main.lblProcessMsg.Text = String.Format("Creating XLS of 2nd Half Standings Snapshot")
            ohelper.status_Msg(Main.lblProcessMsg, Me)
            createSpreadsheet(sfn)
        Else
            gb2ndHalf.Visible = False
        End If


        Main.lblProcessMsg.Text = String.Format("Finished Creating Standings Snapshot")
        ohelper.status_Msg(Main.lblProcessMsg, Me)

        If Me.Height < dgTeams.Height + 150 Then Me.Height *= 1.1
        If gb1stHalf.Height <= dgTeams.Height Then
            gb1stHalf.Height *= 1.1
            gb2ndHalf.Height *= 1.1
        End If

    End Sub
    Sub createSpreadsheet(sfn As String)

    End Sub
End Class