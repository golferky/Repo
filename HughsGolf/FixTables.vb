Imports System.Data.SQLite
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ListView
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Drawing
Imports DocumentFormat.OpenXml.EMMA
Imports DocumentFormat.OpenXml.Vml
Imports DocumentFormat.OpenXml.Vml.Office
Imports iText.Kernel.Pdf
Imports iText.StyledXmlParser.Jsoup.Helper
Imports Org.BouncyCastle.Utilities.IO
Public Class FixTables
    Public oHelper As New Helper
    Dim oClosedXML As New ClosedXML
    Dim sLeagueName As String = Main.cbLeagues.SelectedItem.ToString.Replace("'", "''")
    Dim connection = ctx.Conn
    Dim sb As StringBuilder

    Dim dtlp As DataTable
    Dim dtsch As DataTable
    Dim dtOldScores As DataTable
    Dim dtNewScores As DataTable
    Dim dtSubs As DataTable
    Dim dtTeams As DataTable
    Dim dtPartners As DataTable
    Dim dtHandicaps As DataTable
    Dim dtHandicapDetails As DataTable
    Dim dtMatches As DataTable
    Dim dtpSkins As DataTable
    Dim dtMisMatch As DataTable

    Dim dvscores As DataView
    Dim dvthisPlayer As DataView
    Dim mmflds As String() = {"Date", "Player", "Field", "Bad Value", "Good Value", "Difference-d", "Hole", "Score", "Comments"}

    Dim wkrow As DataRow = Nothing
    Dim skeys As Object() = Nothing
    Dim drScoresfind As DataRow = Nothing
    Private MLogit As Action(Of String) = Sub(msg)
                                              LOGIT(msg, True) ' Let the method handle the optional parameter
                                          End Sub

    Public Sub Fixit()
        oHelper = Main.oHelper
        Dim connection = ctx.Conn
        '20251119 - make sure earnings table can be removed
        TableCleanup()
        Dim dtlp As DataTable = oHelper.sqlitedaFromSql(connection, "LeagueParms", $"SELECT * FROM LeagueParms WHERE Name = '{ctx.sLeagueName}' ORDER BY Season Asc")
        Dim dtsch As DataTable = oHelper.sqlitedaFromSql(connection, "Schedule", $"SELECT * FROM Schedule ORDER BY Date Asc")
        Dim dtsc As DataTable = oHelper.sqlitedaFromSql(connection, "Scores", $"SELECT distinct Date FROM Scores ORDER BY Date Asc")
        Dim dtpmts As DataTable = oHelper.sqlitedaFromSql(connection, "Payments", $"SELECT distinct Date FROM Payments ORDER BY Date Asc")
        dtsc.PrimaryKey = New DataColumn() {dtsc.Columns("Date")}
        dtsch.PrimaryKey = New DataColumn() {dtsch.Columns("Date")}
        dtpmts.PrimaryKey = New DataColumn() {dtpmts.Columns("Date")}
        'use scores table to build schedule and league parms
        Dim dtWeekControl As DataTable = New DataTable
        dtWeekControl.Columns.AddRange(New DataColumn() {
                                       New DataColumn("ID"),
                                        New DataColumn("League"),
                                        New DataColumn("Season"),
                                        New DataColumn("Week"),
                                        New DataColumn("Date"),
                                        New DataColumn("Description")
                                       })
        Dim iID As Integer = 0

        'make sure rainouts are included in league parms table
        For Each lprow As DataRow In dtlp.Rows
            If lprow("Season") < 2018 Then Continue For
            oHelper.sPSDate = CDate(lprow("PostSeasonDt")).ToString("yyyyMMdd")
            Dim sStartDate As String = CDate(lprow("StartDate")).ToString("yyyyMMdd")
            Dim sEndDate As String = CDate(lprow("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd")
            MLogit($"Checking {lprow("Season")}")
            Dim sDayOfWeek As String = dtlp.Rows(0)("DayOfWeek").ToString
            Dim lHolidays = oHelper.GetHolidayList(lprow("Season"), sDayOfWeek)
            Dim ituesHolidays As Integer = 0
            'Dim wDate As DateTime = lprow("StartDate")
            Dim iweeks As Integer = (lprow("Teams") - 1) * 2

            Dim iweek As Integer = 0
            Dim sseason As String = ""
            'loop thru each date from startdate to enddate to build weekcontrol table
            Dim currentDate As String = sStartDate 'Date.ParseExact(sStartDate, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            Dim dv As DataView = New DataView(dtsc)
            dv.RowFilter = $"Date LIKE '{lprow("Season")}%'"
            oHelper.wk = dv(dv.Count - 1)("Date").ToString
            Do Until currentDate > dv(dv.Count - 1)("Date").ToString
                iID += 1
                Dim newrow As DataRow = dtWeekControl.NewRow()
                newrow("Id") = iID
                newrow("League") = sLeagueName
                newrow("Season") = CInt(currentDate.Substring(0, 4))
                newrow("Week") = ""
                newrow("Date") = currentDate
                newrow("Description") = ""
                Dim row As DataRow = dtsc.Rows.Find(currentDate)
                If row Is Nothing Then
                    newrow("Description") = "Rainout"
                    For Each holiday In lHolidays
                        Dim wdate As String = holiday.DateValue.ToString("yyyyMMdd")
                        If wdate = currentDate Then
                            newrow("Description") = "Holiday"
                            Exit For
                        End If
                    Next
                Else
                    If row("Date").ToString.Substring(0, 4) <> sseason Then
                        iweek = 1
                        sseason = row("Date").ToString.Substring(0, 4)
                    Else
                        iweek += 1
                    End If
                    newrow("Week") = iweek
                    If currentDate >= oHelper.sPSDate Then
                        newrow("Description") = "Post Season"
                        Dim schrow As DataRow = dtsch.Rows.Find(currentDate)
                        If schrow IsNot Nothing Then
                            newrow("Description") = $"Combined {newrow("Description")} with Reg"
                        Else
                            newrow("Week") = ""
                        End If
                    End If
                    If iweek > 1 Then
                        Dim wkdate As Date = Date.ParseExact(currentDate, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture).AddDays(-7)
                        Dim schrow As DataRow = dtsch.Rows.Find(wkdate.ToString("yyyyMMdd"))
                        If schrow IsNot Nothing Then
                            Dim dtScores1 As DataTable = oHelper.sqlitedaFromSql(connection, "Scores", $"SELECT sum(Gross) FROM Scores WHERE Date = {currentDate} ")
                            Dim dtScores2 As DataTable = oHelper.sqlitedaFromSql(connection, "Scores", $"SELECT sum(Gross) FROM Scores WHERE Date = {wkdate.ToString("yyyyMMdd")} ")
                            Dim drPmt As DataRow = dtpmts.Rows.Find(currentDate)
                            If dtScores1.Rows(0)(0) = dtScores2.Rows(0)(0) And drPmt Is Nothing Then
                                newrow("Description") = $"Combined {iweek - 1}"
                            End If
                        End If
                    End If
                End If

                dtWeekControl.Rows.Add(newrow)
                Dim sdate As Date = Date.ParseExact(currentDate, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture).AddDays(7)
                currentDate = sdate.ToString("yyyyMMdd")
            Loop
            'Dim dthd As DataTable = oHelper.sqlitedaFromSql(connection, 
            'fix leagueparms

            oHelper.wk = ""
            Continue For

            '20251118-add rainouts to league parms table not needed since were creating a week control table
#Region "Not Needed"
            Dim dvsch = New DataView(dtsch)
            'Dim sToday As String = $"{CDate(Now):yyyyMMdd}"
            dvsch.RowFilter = $"Date LIKE '{lprow("Season")}%'"
            Dim lactDates = New List(Of String)

            For i As Integer = 0 To dvsch.Count - 1
                lactDates.Add(dvsch(i)("Date").ToString())
            Next
            Dim lexpdates = New List(Of String)
            For Each holiday In lHolidays
                If holiday.DayOfWeek = sDayOfWeek Then
                    ituesHolidays += 1
                    Dim wdate As String = holiday.DateValue.ToString("yyyyMMdd")
                    If wdate >= sStartDate And wdate <= sEndDate Then
                        lexpdates.Add(wdate)
                    End If
                End If
            Next
            MLogit($"{ituesHolidays} Total Tuesday Holidays")
            Dim missingInList = lexpdates.Except(lactDates).ToList()
            Dim irainouts As Integer = missingInList.Count

            For Each ro As String In missingInList
                If ro = missingInList(missingInList.Count - 1) Then
                    Dim smsg = ":"
                    If missingInList.Count > 1 Then
                        smsg = "s:"
                    End If

                    MLogit($"{irainouts} Rainout{smsg} {ro} Is missing from Schedule")
                Else
                    MLogit($"{irainouts} Rainouts: {ro} Is missing from Schedule,")
                End If
            Next
            Dim sEnd As String = lactDates(lactDates.Count - 1)
            If sEnd = $"{CDate(lprow("EndDate")):yyyyMMdd}" Then
                MLogit($"{irainouts} Rainouts Calculated End Date {sEnd} matches LeagueParms EndDate {CDate(lprow("EndDate")):yyyyMMdd}")
            Else
                MLogit($"{irainouts} Rainouts Calculated End Date {sEnd} does not match LeagueParms EndDate  {CDate(lprow("EndDate")):yyyyMMdd},true")
            End If
            'MLogit($"Year: {lprow("Season")} Weeks: {iweeks} - Rainouts: {irainouts} = {dv.Count} weeks")
            oHelper.wk = ""
            Dim format As String = "yyyyMMdd"
            Dim parsedDate As DateTime = DateTime.ParseExact(sEnd, format, System.Globalization.CultureInfo.InvariantCulture)
            Dim lastScoreDate As String
            oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT Date FROM Scores WHERE SUBSTR(Date,1,4) = '{lprow("season")}' ORDER BY Date Desc LIMIT 1")
            If oHelper.dt.Rows.Count > 0 Then
                lastScoreDate = oHelper.dt.Rows(0)(0)
            End If

            Dim PSDate As DateTime = parsedDate.AddDays(7)
            If lprow("Season") = Now.Year And parsedDate > Now Then
                lastScoreDate = ""
            Else
                If lastScoreDate IsNot Nothing Then
                    parsedDate = DateTime.ParseExact(lastScoreDate, format, System.Globalization.CultureInfo.InvariantCulture)
                    PSDate = parsedDate.AddDays(-7)
                End If
            End If
            Dim insertUpdQuery = $"UPDATE LeagueParms SET Rainouts = '{irainouts}', EndDate = '{parsedDate}', PostSeasonDt = '{PSDate}' WHERE Name = '{ctx.sLeagueName}' and Season = {lprow("Season")}"
            Dim lrc = oHelper.SqliteTrans(insertUpdQuery)
            oHelper.dt = oHelper.sqliteda(connection, "LeagueParms")
#End Region
            oHelper.wk = ""
        Next
        dtWeekControl.TableName = "WeekControl"
        'dt.Columns.Add("Kitty", GetType(String))
        oHelper.SQLiteCreateTableFromDT(connection, dtWeekControl)

        Main.showtables()
        oHelper.CreateWorkbookFromSQLite(connection)

        dtlp = oHelper.sqliteda(connection, "LeagueParms")

    End Sub
    Function createDT(tflds As String()) As DataTable
        createDT = New DataTable
        For Each fld As String In tflds
            If fld.Split("-").Length > 1 Then
                If fld.Split("-")(1) = "d" Then
                    createDT.Columns.Add(fld.Split("-")(0), GetType(Decimal))
                Else
                    MsgBox("Unknown field type")
                End If
            Else
                createDT.Columns.Add(fld, GetType(String))
            End If
        Next
        Return createDT
    End Function
    Function createDT(tfldsdel As String) As DataTable
        createDT = New DataTable
        createDT = createDT(tfldsdel.Split(","c))
        Return createDT
    End Function
    Function createDTWithPK(dtName As String, flds As String, Optional sql As String = "") As DataTable

        Dim dt As DataTable
        If sql <> "" Then
            dt = oHelper.sqlitedaFromSql(connection, dtName, sql)
        Else
            dt = oHelper.sqliteda(connection, dtName)
        End If

        ' Set the PrimaryKey property
        createPK(dt, flds)
        Return dt
    End Function
    Sub createPK(dt As DataTable, flds As String)

        ' Split the field list into an array
        Dim fieldNames() As String = flds.Split(","c)

        ' Create an array of DataColumn objects sized to the number of fields
        Dim keys(fieldNames.Length - 1) As DataColumn

        ' Loop through each field name and assign the corresponding DataColumn
        For i As Integer = 0 To fieldNames.Length - 1
            Dim fldName As String = fieldNames(i).Trim()
            keys(i) = dt.Columns(fldName)
        Next

        ' Set the PrimaryKey property
        dt.PrimaryKey = keys
    End Sub
    Function findDr(dt As DataTable, flds As Object) As DataRow
        ' Split the field list into an array
        Dim fieldNames() As String = flds.Split(","c)
        ' Set the PrimaryKey property
        findDr = dt.Rows.Find(fieldNames)
    End Function
    Sub fix1()
        '==============================
        'fix #1-fix payments table amounts all positive
        '==============================
        Dim lrcu = oHelper.SqliteTrans("UPDATE Payments SET Earned = ABS(Earned) WHERE Earned < 0;")
        Debug.Print($"{System.Reflection.MethodBase.GetCurrentMethod.Name}-{lrcu} Payments Records fixed")
        buildMM("All", "All", "Earned (Payments)", "Negative", "Positive", "", "", "", $"{lrcu} records updated- made Payments Earned ammouts positive")
        oHelper.wk = ""
    End Sub
    Sub fix2()
        '==============================
        'fix #2-fix Teams/Subs tables players who are regulars and should be subs, uses old scores to get team number from that year
        '==============================
        '20251211-one time fix for teams table
        'Year~Good Player/Bad Player
        '"2018~Tony Fey/Todd Fey,Jerry Arrasmith/Gary Reeves,Don Johnston/Mike Arnold
        oHelper.dt = oHelper.sqliteda(connection, "Teams")
        Dim sRegs As String =
"2018~Jerry Arrasmith/Gary Reeves,Don Johnston/Mike Arnold
2020~Paul Wilburn/Will Huenefeld
2024~Tony Smith/"
        Dim lrcu As Integer = 0
        For Each seasonToFix As String In sRegs.Split(vbCrLf)
            Dim sSubsToDel As String = "", sSubsToAdd As String = ""
            Dim sYear As String = seasonToFix.Split("~")(0)
            Dim sPlayersToReplace = seasonToFix.Split("~")(1)
            For Each reg As String In sPlayersToReplace.Split(",")
                Dim splayerToDel As String = reg.Split("/")(1)
                Dim splayerToKeep As String = reg.Split("/")(0)
                'print the result of the update
                oHelper.wk = $"UPDATE Teams SET Player = '{splayerToKeep}' WHERE Year = {sYear} AND Player = '{splayerToDel}'"
                lrcu = oHelper.SqliteTrans(oHelper.wk)
                Dim newrow As DataRow = dtMisMatch.NewRow()
                newrow("Date") = sYear
                newrow("Player") = splayerToKeep
                newrow("Field") = "Player (Teams)"
                newrow("Bad Value") = splayerToDel
                newrow("Good Value") = splayerToKeep
                newrow("Comments") = $"{lrcu} records updated for year {sYear}"
                dtMisMatch.Rows.Add(newrow)
                oHelper.wk = ""
                Debug.Print($"{System.Reflection.MethodBase.GetCurrentMethod.Name}-{oHelper.wk}{vbCrLf}{lrcu} Teams Records Updated")
                If sSubsToDel <> "" Then
                    sSubsToDel &= ","
                End If
                sSubsToDel &= $"'{reg.Split("/")(0)}'"
                If sSubsToAdd <> "" Then
                    sSubsToAdd &= ","
                End If
                sSubsToAdd &= $"'{reg.Split("/")(1)}'"
            Next
            If sSubsToDel <> "" Then
                sb = New StringBuilder
                sb.AppendLine($"DELETE FROM Subs WHERE Date BETWEEN {sYear}0101 AND {sYear}1231 AND Player IN ({sSubsToDel})")
                oHelper.wk = sb.ToString
                lrcu = oHelper.SqliteTrans(oHelper.wk)
                Dim newrow As DataRow = dtMisMatch.NewRow()
                newrow("Date") = sYear
                newrow("Player") = "Subs Delete"
                newrow("Field") = "Player (Subs)"
                newrow("Bad Value") = sSubsToDel
                newrow("Comments") = $"{lrcu} records deleted for year {sYear}"
                dtMisMatch.Rows.Add(newrow)
                oHelper.wk = ""

                Debug.Print($"{System.Reflection.MethodBase.GetCurrentMethod.Name}-{oHelper.wk}-{lrcu} Subs Records Deleted")
                'Debug.Print(oHelper.SqliteTrans($"DELETE FROM Subs WHERE Player IN ('Tony Smith','Paul Wilburn')"))
            End If
            If sSubsToAdd <> "" Then
                'For Each ssub As String In sSubsToAdd.Split(",")
                Dim dvSubsScores As New DataView(dtNewScores)
                dvSubsScores.RowFilter = $"Date > {sYear}0101  AND Date < {sYear}1231 AND Player in ({sSubsToAdd})"
                sb = New StringBuilder
                For Each srow As DataRowView In dvSubsScores
                    oHelper.sPlayer = srow("Player")
                    ctx.ActiveDate = srow("Date")
                    wkrow = dtOldScores.Rows.Find(New Object() {srow("Player"), srow("Date")})
                    Dim sTeam As String = wkrow("Team"), sGrade As String = wkrow("Grade")
                    sb = New StringBuilder
                    sb.AppendLine($"INSERT INTO Subs (ID,League,Player,Date,Team,Grade) VALUES('0','{oHelper.sLeagueName}','{oHelper.sPlayer}','{ctx.ActiveDate}','{sTeam}','{sGrade}') ")
                    oHelper.wk = sb.ToString
                    lrcu = oHelper.SqliteTrans(oHelper.wk)
                    Debug.Print($"{System.Reflection.MethodBase.GetCurrentMethod.Name}-{oHelper.wk}-{lrcu} Subs Records Inserted")
                Next
                Dim newrow As DataRow = dtMisMatch.NewRow()
                newrow("Date") = sYear
                newrow("Player") = "Subs Added"
                newrow("Field") = "Player (Subs)"
                newrow("Good Value") = sSubsToAdd
                newrow("Comments") = $"{sSubsToAdd.Split(",").Count} records Added for year {sYear}"
                dtMisMatch.Rows.Add(newrow)
                oHelper.wk = ""
                'Next
            End If
        Next
        'Debug.Print(oHelper.SqliteTrans("UPDATE Teams SET Player = 'Tony Fey' WHERE Player = 'Todd Fey'"))
        'Debug.Print(oHelper.SqliteTrans("UPDATE Teams SET Player = 'Jerry Arrasmith' WHERE Year = 2018 AND Player = 'Gary Reeves'"))
        'Debug.Print(oHelper.SqliteTrans("UPDATE Teams SET Player = 'Don Johnston' WHERE Year = 2018 AND Player = 'Mike Arnold'"))
    End Sub
    Sub report3()
        'Dim dtPartners As DataTable = createDTWithPK("Partners", "Player,Date", oHelper.wk)   'must fix payments table amounts to match earnings table before removing earnings table
        '==============================
        'Report #3-
        '==============================
        sb = New StringBuilder
        sb.AppendLine($"
SELECT 
    S.Date,
    T.Team,
    COUNT(S.Player) AS Total
FROM Teams T
LEFT JOIN Scores S 
    ON T.Year = SUBSTRING(S.Date,1,4) 
   AND T.Player = S.Player
   AND S.Date > 20180101
where s.date <> ''
GROUP BY S.Date, T.Team
ORDER BY S.Date,T.Team;
")
        oHelper.wk = sb.ToString
        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", oHelper.wk)
        oHelper.dt.PrimaryKey = New DataColumn() {oHelper.dt.Columns("Date"), oHelper.dt.Columns("Team")}

        For Each score As DataRow In dtNewScores.Rows
            wkrow = dtSubs.Rows.Find(New Object() {score("Date"), score("Player")})
            If wkrow Is Nothing Then Continue For
            Dim sTeam As String = wkrow("Team")
            wkrow = oHelper.dt.Rows.Find(New Object() {score("Date"), wkrow("Team")})
            If wkrow Is Nothing Then
                Dim newrow As DataRow = oHelper.dt.NewRow
                newrow("Date") = score("Date")
                newrow("Team") = sTeam
                newrow("Total") = 1
                oHelper.dt.Rows.Add(newrow)
            Else
                wkrow("Total") += 1
            End If
        Next
        Dim dvTot = New DataView(oHelper.dt)
        dvTot.Sort = "Date,Team"
    End Sub
    Sub report4()
        '==============================
        'Report #4-
        '==============================
        'this loop will look at all scores and determine each players partner based on team/grade, some are messed up in teams/subs table, 20180417 has 2 players with same grade on same team
        Dim savedate As String = String.Empty
        Dim dtthisweekscores As DataTable = createDT("Date,Player,Team,Partner,Order-d")
        dtthisweekscores.PrimaryKey = New DataColumn() {dtthisweekscores.Columns("Date"), dtthisweekscores.Columns("Player")}
        Dim dvt As DataView
        For Each score As DataRow In dtNewScores.Rows
            'lets balance each date to insure all teams have scores for every player correctly
            If score("Date") < "20180101" Then Continue For
            oHelper.sPlayer = score("Player")
            'If score("Date") = "20180501" Then
            '    oHelper.wk = ""
            'End If
            'Debug.Print($"{score("Date")} {score("Player")} {score("Partner")}")
            Dim newrow As DataRow = dtPartners.NewRow
            newrow("Date") = score("Date")
            newrow("Player") = score("Player")
            newrow("Order") = CDbl(score("Partner"))
            wkrow = findDr(dtTeams, $"{score("Date").ToString.Substring(0, 4)},{score("Player")}")
            Dim sPartner As String = "", sGrade = "", sSubGrade As String = ""
            If wkrow Is Nothing Then
                dvt = New DataView(dtSubs)
                dvt.RowFilter = $"Date = '{score("Date")}' AND Player = '{score("Player")}'"
                'drScoresfind = findDr(dtSubs, $"{score("Date")},{dvt(0)("Player")}")
                If dvt.Count > 0 Then
                    newrow("Team") = dvt(0)("Team")
                    newrow("Grade") = dvt(0)("Grade")
                End If
                'save grade of sub 
                sSubGrade = dvt(0)("Grade")
                dvt = New DataView(dtTeams)
                dvt.RowFilter = $"Year = '{score("Date").ToString.Substring(0, 4)}' AND Team = '{newrow("Team")}' AND Grade <> '{sSubGrade}'"
                If dvt.Count = 0 Then
                    dvt = New DataView(dtSubs)
                    dvt.RowFilter = $"Date = '{score("Date")}' AND Player = '{score("Player")}'"
                End If
                newrow("Partner") = dvt(0)("Player")
                newrow("PartnerGrade") = dvt(0)("Grade")
                'Continue For
            Else
                newrow("Team") = wkrow("Team")
                newrow("Grade") = wkrow("Grade")
                dvt = New DataView(dtTeams)
                dvt.RowFilter = $"Year = '{score("Date").ToString.Substring(0, 4)}' AND Team = '{newrow("Team")}' AND Player <> '{score("Player")}'"
                newrow("Partner") = dvt(0)("Player")
                newrow("PartnerGrade") = dvt(0)("Grade")
            End If
            'look for a team mate for this player in the scores file from the teams table 
            skeys = {dvt(0)("Player"), score("Date")}
            drScoresfind = dtNewScores.Rows.Find(skeys)
            'If Not found, try the subs table
            If drScoresfind Is Nothing Then
                dvt.RowFilter = $"Year = '{score("Date").ToString.Substring(0, 4)}' AND Team = '{newrow("Team")}' AND Player <> '{score("Player")}'"
                dvt = New DataView(dtSubs)
                dvt.RowFilter = $"Date = '{score("Date")}' AND Team = '{newrow("Team")}'"
                'drScoresfind = findDr(dtSubs, $"{score("Date")},{dvt(0)("Player")}")
                If dvt.Count > 0 Then
                    sPartner = dvt(0)("Player")
                    sSubGrade = dvt(0)("Grade")
                    newrow("Partner") = sPartner
                    newrow("PartnerGrade") = sSubGrade
                End If
            Else
                newrow("Partner") = dvt(0)("Player")
                newrow("PartnerGrade") = dvt(0)("Grade")
            End If
            dtPartners.Rows.Add(newrow)
            If savedate = String.Empty Then
                savedate = score("Date")
            End If
            If score("Date") = savedate Then
                wkrow = findDr(dtthisweekscores, $"{score("Date").ToString.Substring(0, 4)},{score("Player")}")
                If wkrow IsNot Nothing Then
                    Continue For
                Else
                    wkrow = dtthisweekscores.NewRow
                    wkrow("Date") = newrow("Date")
                    wkrow("Player") = newrow("Player")
                    wkrow("Team") = newrow("Team")
                    wkrow("Partner") = newrow("Partner")
                    wkrow("Order") = score("Partner")
                    dtthisweekscores.Rows.Add(wkrow)
                End If
            Else
                oHelper.wk = ""
            End If
        Next

        Dim dvtemp As New DataView(dtPartners)
        dvtemp.Sort = ("Date,Order")
        dtPartners = dvtemp.ToTable
        dtPartners.TableName = "Partners"
        dtPartners.PrimaryKey = New DataColumn() {dtPartners.Columns("Date"), dtPartners.Columns("Player")}

    End Sub
    Sub report5()
        '==============================
        'Report #5-
        '==============================
        sb = New StringBuilder
#Region "20251128-Compare Earnings to Payments SQL"
        sb.AppendLine($"
Select 
    E.Date,
    E.OldSkins,
    E.OldEarned,
    P.NewSkins,
    P.NewEarned,
    (E.OldSkins - COALESCE(P.NewSkins, 0)) AS CountDiff,
    (E.OldEarned - COALESCE(P.NewEarned, 0)) AS AmountDiff
FROM (
    SELECT 
        Date,
        --COUNT(*) AS OldSkins,
        SUM(SkinsNum) AS OldSkins,
        SUM(SkinEarned) AS OldEarned
    FROM Earnings
    WHERE SkinEarned <> ''
    GROUP BY Date
) E
LEFT JOIN (
    SELECT 
        Date,
        COUNT(*) AS NewSkins,
        SUM(Earned) AS NewEarned
    FROM Payments
    WHERE Desc = 'Skin' AND SUBSTRING(Detail,1,1) = '#'
    GROUP BY Date
) P
ON E.Date = P.Date;
")
        Dim dtCompare As DataTable = oHelper.sqlitedaFromSql(connection, "", sb.ToString)
        dtCompare.PrimaryKey = New DataColumn() {dtCompare.Columns("Date")}
#End Region
    End Sub
    Sub fix6()
        dtlp = createDTWithPK("LeagueParms", "Name,Season")
        dtsch = createDTWithPK("Schedule", "Date")
        dtHandicaps = createDTWithPK("Handicaps", "Date,Player")
        dtHandicapDetails = createDTWithPK("HandicapDetail", "Date,Player")
        dtMatches = createDTWithPK("Matches", "Player,Date")
        dvscores = New DataView(dtNewScores)
        dvscores.Sort = "Date ASC,Player ASC"
        Dim iBadHdcp As Integer = 0
        dtMisMatch.PrimaryKey = New DataColumn() {dtMisMatch.Columns("Date"), dtMisMatch.Columns("Player"), dtMisMatch.Columns("Field")}
        For Each season As DataRow In dtlp.Rows
            Debug.Print($"Checking {season("Season")}")
            If season("Season") < 2018 Then Continue For
            oHelper.sPSDate = $"{CDate(season("PostSeasonDt")):yyyyMMdd}"
            Dim StartDate = $"{CDate(season("StartDate")):yyyyMMdd}"
            Dim EndDate = $"{CDate(season("EndDate")):yyyyMMdd}"
            'Dim sStart = $"{CDate(drlp("StartDate")):yyyyMMdd}"
            dvscores.RowFilter = $"Date >= '{StartDate}' AND Date <= '{EndDate}'"
            For Each score As DataRowView In dvscores
                'check to see if this is a reg season score, if not skip
                Dim dr As DataRow = dtsch.Rows.Find(score("Date"))
                If dr Is Nothing Then
                    'Debug.Print($"{score("Date")} is postseason Only Skipping")
                    Continue For
                End If
                '20251201-check last 5 to make sure handicaps are good
                oHelper.sPlayer = score("Player")
                ctx.ActiveDate = score("Date")
                oHelper.wk = ""
                Dim sHandicap As String = dtHandicaps.Rows.Find(New Object() {score("Date"), score("Player")})("PHdcp").ToString
                'rem p1 
                Dim lfix = New List(Of String)
                Dim ddiff As Decimal = 0
                If findMM(score("Player"),
                       score("Date"),
                       dtOldScores,
                       dtNewScores,
                       dtMatches,
                       dtPartners,
                       dtHandicaps,
                       dtMisMatch) Then
                End If
            Next
        Next
    End Sub
    Sub fix7()
        Dim ifixed = 0
        Dim dCTP = New List(Of Decimal)
        Dim dTotCTP As Decimal = 0
        'For i = 1 To Main.lPar3s.count
        '    dCTP.Add(0)
        'Next
        Const sCTPfld As String = "CTP_"
        oHelper.iNumClosests = 0
        Dim inf As Integer = 0
        Dim idiff As Integer = 0
        Dim drfind As DataRow
        Dim iHole As Integer

        Dim sSeason As String = ""
        Dim drlp As DataRow
        Dim spsDate As String = ""
        'Dim drcomp As DataRow
        Dim dte As DataTable = createDTWithPK("Earnings", "Date,Player")
        For Each col As DataColumn In dte.Columns
            If col.ColumnName.ToLower.StartsWith("ctp_") Then
                oHelper.iNumClosests += 1
            End If
        Next
        Dim dv As DataView = New DataView(dtpSkins)
        Dim dtpCTP As DataTable = createDTWithPK("CTPs", "Desc,Date,Player,Detail", "SELECT * FROM Payments WHERE Desc = 'CTP' AND Detail LIKE '#%' ORDER BY Date Asc")
        Dim dtFrontBack As DataTable = createDTWithPK("Scores", "Date", $"SELECT distinct Date, FrontBack FROM Scores ORDER BY Date Asc")
        Dim dve = New DataView(dte)
        dve.Sort = "Date, Player"
        For Each row As DataRowView In dve
            oHelper.sPSDate = ""
            If row("Date").ToString.Substring(0, 4) > sSeason Then
                sSeason = row("Date").ToString.Substring(0, 4)
                Dim keys() As Object = {row("League"), sSeason}
                drlp = dtlp.Rows.Find(keys)
                Dim sStart = $"{CDate(drlp("StartDate")):yyyyMMdd}"
                Dim sEnd = $"{CDate(drlp("EndDate")):yyyyMMdd}"
                spsDate = $"{CDate(drlp("PostSeasonDt")):yyyyMMdd}"
                oHelper.dt = oHelper.sqliteda(connection, "Payments")
                dtpSkins = oHelper.sqlitedaFromSql(connection, "Payments", $"SELECT * FROM Payments WHERE Desc LIKE 'Skin%' AND Detail LIKE '#%' AND Date >= {sStart} AND Date <= {sEnd.Substring(0, 4)}1231 ORDER BY Date Asc")
                dtpSkins.PrimaryKey = New DataColumn() {dtpSkins.Columns("Date"), dtpSkins.Columns("Player"), dtpSkins.Columns("Detail")}
                dv = New DataView(dtpSkins)
            End If
            'If row("Date") >= spsDate Then Continue For
            Dim drfFB As DataRow = dtFrontBack.Rows.Find(row("Date"))
            If row("Earned") = "" Then
                MsgBox("Bad Earned amount")
            Else
                'look for skins
                If Main.rbSkins.Checked Or (Not Main.rbSkins.Checked And Not Main.rbCTPs.Checked) Then
                    If IsNumeric(row("SkinEarned")) Then
                        'reconcile multiple skins for this player
                        Dim Eskinvalue As String = String.Format("{0:0.00}", row("SkinEarned"))
                        Dim dskinvalue As Decimal = 0
                        Dim sScores As String = ""
                        Dim sHoles As String = ""
                        'Debug.WriteLine($"Payments for multiple skins for {row("Date")}-{row("Player")}-Skin")
                        'Continue For
                        dv.RowFilter = $"Date = '{row("Date")}' and Player = '{row("Player")}'"
                        If dv.Count = 0 Then
                            buildMM(row("Date"), row("Player"), "Skin", Eskinvalue, dskinvalue, "", "", Eskinvalue - dskinvalue, "New Skin not found")
                            Continue For
                        End If
                        For Each drow As DataRowView In dv
                            If dv.Count > 1 Then
                                dskinvalue += drow("Earned")
                                If sHoles <> "" Then
                                    sHoles &= "-"
                                    sScores &= "-"
                                End If
                                sHoles &= drow("Detail").ToString.Replace("#", "")
                                sScores &= drow("Comment").ToString.Substring(drow("Comment").ToString.IndexOf("=") + 1)
                            Else
                                dskinvalue += drow("Earned")
                                sHoles &= drow("Detail").ToString.Replace("#", "")
                                sScores &= drow("Comment").ToString.Substring(drow("Comment").ToString.IndexOf("=") + 1)
                            End If
                        Next
                        If Eskinvalue <> dskinvalue Then
                            If Eskinvalue = dskinvalue * -1 Then
                                Debug.Print("")
                            End If
                            Dim ddiff As Decimal = dskinvalue - CDec(row("SkinEarned"))
                            buildMM(row("Date"), row("Player"), "Skin", Eskinvalue, dskinvalue, sScores, sHoles, ddiff)
                            oHelper.wk = ""
                        End If
                    End If
                End If

                'look for CTPs
                If Main.rbCTPs.Checked Or (Not Main.rbSkins.Checked And Not Main.rbCTPs.Checked) Then
                    For i = 1 To oHelper.iNumClosests
                        'only look for ctps that match front or back
                        If Not IsNumeric(row($"{sCTPfld}{i }")) Then Continue For
                        If drfFB("FrontBack") = "Front" Then
                            Dim x = Main.lPar3sFront
                            iHole = Main.lPar3sFront(i - 1)
                        Else
                            iHole = Main.lpar3sBack(i - 1)
                        End If

                        Dim ctpvalue As String = String.Format("{0:0.00}", row($"{sCTPfld}{i }"))
                        drfind = dtpCTP.Rows.Find(New Object() {"CTP", row("Date"), row("Player"), $"#{iHole}"})
                        If drfind Is Nothing Then
                            Debug.WriteLine($"CTP Payments Record not found for {row("Date")}-{row("Player")}-#{iHole}-{ctpvalue}")
                            inf += 1
                            buildMM(row("Date"), row("Player"), $"CTP-{i}", CDec(ctpvalue), "", $"{iHole}", "", "", "New CTP not found ")
                            Continue For
                        Else
                            'Debug.WriteLine($"Earnings Record found for {row("Date")}-{row("Player")}-#{iHole}-{ctpvalue}")
                        End If
                        If ctpvalue <> drfind("Earned") Then
                            Dim ddiff As Decimal = CDec(ctpvalue) - CDec(drfind("Earned"))
                            idiff += 1
                            Dim damt As Decimal = ctpvalue - drfind("Earned")
                            If damt > 2 Then
                                Debug.WriteLine($"CTP Payments amount different for {row("Date")}~{row("Player")}~#{iHole}~{ctpvalue}~{drfind("Earned")}~{ddiff}")
                            End If
                            buildMM(row("Date"), row("Player"), $"CTP({i})", CDec(ctpvalue), CDec(drfind("Earned")), "", iHole, ddiff)
                        End If
                    Next
                End If
            End If
        Next
        Debug.Print($"CTP Payments not found: {inf}{vbCrLf}CTP Payments different amounts: {idiff}")

    End Sub
    Sub fix8NotUsed()
        sb = New StringBuilder
        sb.AppendLine($"SELECT Handicaps.PHdcp,0 AS CalcHdcp,Handicaps.HDcp AS Hdcp,'' AS NewHdcp,'' AS Last5,Scores.* FROM Scores 
LEFT OUTER JOIN Handicaps ON Scores.League = Handicaps.League AND Scores.Date = Handicaps.Date AND Scores.Player = Handicaps.Player
WHERE Scores.League = '{oHelper.sLeagueName}' AND Scores.Date NOT IN ({oHelper.sexclDates})
ORDER BY Date Asc
")

        Dim dts As DataTable = oHelper.sqlitedaFromSql(connection, "", sb.ToString)
        dts.PrimaryKey = New DataColumn() {dts.Columns("Date"), dts.Columns("Player")}
        Dim dtscores As New DataTable
        For Each col As DataColumn In dts.Columns
            If col.ColumnName = "Date" OrElse col.ColumnName = "Player" Then
                dtscores.Columns.Add(col.ColumnName, col.DataType)
            ElseIf col.ColumnName.StartsWith("$") Then
                dtscores.Columns.Add(col.ColumnName, GetType(Double))
            Else
                dtscores.Columns.Add(col.ColumnName, GetType(String))
            End If
        Next
        For Each season As DataRow In dtlp.Rows
            'Debug.Print($"Checking {season("Season")}")
            If season("Season") < 2018 Then Continue For
            oHelper.sPSDate = CDate(season("PostSeasonDt")).ToString("yyyyMMdd")
            Dim dvsch As DataView = New DataView(dtsch)
            '20250615 - Future... need to add league name to schedule table
            'dvsch.RowFilter = $"League = '{oHelper.sLeagueName}' AND Date >= '20180101'"
            dvsch.RowFilter = $"Date LIKE '{season("Season")}%'"
            dvsch.Sort = "Date Asc"
            Dim ibadhdcps As Integer = 0
            Dim dvlp As DataView = New DataView(dtlp)
            dvlp.RowFilter = $"Season = {season("Season")}"
            Dim dvcourse As DataView = New DataView(oHelper.sqlitedaFromSql(connection, "", $"SELECT * FROM Courses WHERE Name = '{dvlp(0)("Course")}'"))
            For Each schrow As DataRowView In dvsch
                dtscores.Clear()
                ctx.ActiveDate = schrow("Date").ToString
                'If ctx.ActiveDate > Now.ToString("yyyyMMdd") Then Exit For
                'dtscores = New DataTable
                'dtscores.Columns.Add("CalcHdcp", GetType(Integer))
                'dtscores.Columns.Add("NewHdcp", GetType(String))
                'dtscores.Columns.Add("Last5", GetType(String))

                'this code is here,everything from this if to endif, to fix prior league handicaps which calc was wrong because it included post season scores and it shouldnt have

                If schrow("Date").ToString < $"2025" Then

                    Dim dvos = New DataView(dtOldScores)
                    'dvOS.RowFilter = $"League = '{oHelper.sLeagueName}' AND Date NOT IN ({oHelper.sexclDates})"
                    dvos.RowFilter = $"League = '{oHelper.sLeagueName}' AND Date ={ctx.ActiveDate} AND (OUT_GROSS > 0 OR IN_GROSS > 0)"
                    'dvOS.RowFilter = $"League = '{oHelper.sLeagueName}' AND Date >= '{season("Season")}0101' AND Date <= '{season("Season")}1231' AND Date NOT IN ({oHelper.sexclDates})"
                    'dvOS.Sort = "Date Asc"
                    For Each row As DataRowView In dvos
                        Dim newrow As DataRow = dtscores.NewRow()
                        newrow("League") = oHelper.sLeagueName
                        newrow("Player") = row("Player")
                        newrow("Date") = row("Date")
                        newrow("PHdcp") = row("PHdcp")
                        Dim istart
                        If row("Out_Gross") Is DBNull.Value Then
                            newrow("Gross") = row("In_Gross")
                            newrow("Net") = row("In_Net")
                            newrow("FrontBack") = "Back"
                            istart = 10
                        Else
                            newrow("Gross") = row("Out_Gross")
                            newrow("Net") = row("Out_Net")
                            newrow("FrontBack") = "Front"
                            istart = 1
                        End If

                        For i = 1 To 9
                            newrow($"{i}") = row($"Hole{(istart - 1) + i}")
                        Next
                        dtscores.Rows.Add(newrow)
                    Next
                Else
                    Dim dv2025 = New DataView(dts)
                    dv2025.RowFilter = $"League = '{oHelper.sLeagueName}' AND Date ={ctx.ActiveDate}"
                    dtscores = dv2025.ToTable
                    'dtscores.Columns.Add("CalcHdcp", GetType(Integer))
                    'dtscores.Columns.Add("NewHdcp", GetType(String))
                    'dtscores.Columns.Add("Last5", GetType(String))
                End If
                'Dim filteredRows() As DataRow = dtscores.Select($"Date = {ctx.ActiveDate}")

                'Dim dvScores As DataView = New DataView(dtscores)
                'dvScores.RowFilter = $"Date = '{schrow("Date")}'"
                'dvScores.Sort = "Date Asc"

                For Each row As DataRow In dtscores.Rows
                    oHelper.sPlayer = row("Player")
                    Dim dvthisPlayerLast5 As DataView = New DataView(dtOldScores)
                    dvthisPlayerLast5.Sort = "Date Desc"
                    dvthisPlayerLast5.RowFilter = $"Player = '{oHelper.sPlayer}' AND Date < '{schrow("Date")}' AND (Out_Gross > 0 OR In_Gross > 0)"
                    Dim iHdcp As Integer
                    Dim sL5 = ""
                    If dvthisPlayerLast5.Count = 0 Then
                        'no previous scores, so skip this player
                        iHdcp = row("Phdcp")
                        GoTo skiplast5
                    End If
                    Dim l5 As DataTable = dvthisPlayerLast5.ToTable().AsEnumerable().Take(5).CopyToDataTable()
                    Dim llast5Scores As New List(Of String)
                    For i = 0 To l5.Rows.Count - 1
                        Dim sGross As String
                        Dim sPar As String
                        Dim scorefld As String = If(l5.Rows(i)("Out_Gross") IsNot DBNull.Value, "Out_Gross", "In_Gross")
                        sGross = l5.Rows(i)(scorefld).ToString
                        sPar = If(row("FrontBack") = "Front", dvcourse(0)("Out").ToString, dvcourse(0)("In").ToString)
                        sL5 += sGross
                        If i < l5.Rows.Count - 1 Then
                            sL5 += "-"
                        End If
                        llast5Scores.Add($"{sGross}-{sPar}")
                    Next
                    oHelper.wk = ""
                    'Dim pHdcp As String = dthdcp.Rows.Find(New Object() {row("Date"), row("Player")})("Hdcp").ToString
                    GoTo skipoldl5
                    If llast5Scores.Count > 0 Then
                        If llast5Scores.Count = 5 Then
                            llast5Scores.Remove(llast5Scores.Min)
                            llast5Scores.Remove(llast5Scores.Max)
                        ElseIf llast5Scores.Count = 4 Then
                            llast5Scores.Remove(llast5Scores.Max)
                        End If
                    End If
skipoldl5:
                    Dim iScoreTot = 0, iParTot = 0
                    For Each score In llast5Scores
                        iScoreTot += score.Split("-")(0)
                        iParTot += score.Split("-")(1)
                    Next
                    iHdcp = IIf(llast5Scores.Count = 0, 0, ((iScoreTot / llast5Scores.Count) - (iParTot / llast5Scores.Count)) * 0.8)
                    row("CalcHdcp") = iHdcp '.ToString("0.00")
skiplast5:
                    'For Each col As DataColumn In dts.Columns
                    '    Try
                    '        row("Last5") = sL5
                    '    Catch ex As Exception

                    '    End Try
                    'Next
                    row("Last5") = sL5
                    'sb = New StringBuilder
                    'sb.AppendLine($"SELECT * FROM Handicaps WHERE League = '{ctx.safeLeagueName}' AND Date = {ctx.ActiveDate} AND Player = '{oHelper.sPlayer}'")
                    'sb = New StringBuilder
                    'sb.AppendLine($"SELECT * FROM Handicaps WHERE Date = {ctx.ActiveDate}")
                    'oHelper.wk = sb.ToString
                    'oHelper.dt = oHelper.sqlitedaFromSql(connection, "", oHelper.wk)
                    sb = New StringBuilder
                    sb.AppendLine($"SELECT * FROM Handicaps WHERE Date = {ctx.ActiveDate} AND Player = '{oHelper.sPlayer}' ")
                    oHelper.wk = sb.ToString
                    Dim dvhdcp As DataView = New DataView(oHelper.sqlitedaFromSql(connection, "", sb.ToString))
                    oHelper.dt = oHelper.sqliteda(connection, "Handicaps")
                    If dvhdcp.Count = 0 Then
                        row("pHdcp") = dvthisPlayerLast5(0)("PHdcp")
                    Else
                        row("pHdcp") = dvhdcp(0)("Phdcp")
                    End If
                    'dvhdcp = New DataView(oHelper.sqlitedaFromSql(connection, "", sb.ToString))

                    If row("Phdcp") <> CDbl(iHdcp) Then
                        MLogit($"{ibadhdcps}-{ctx.ActiveDate}|{oHelper.sPlayer}|Last 5 {String.Join(", ", sL5)}|Old Scores Hdcp {row("Phdcp")}|fixed Hdcp {iHdcp}")
                        ibadhdcps += 1
                        'GoTo skipfix
                        oHelper.dt = oHelper.sqlitedaFromSql(connection, "Handicaps", $"SELECT * FROM Handicaps WHERE CONCAT(League,Date,Player) = '{oHelper.sLeagueName}{ctx.ActiveDate}{oHelper.sPlayer}' ")
                        Dim insertUpdQuery = $"UPDATE Handicaps SET PHdcp = '{iHdcp}' WHERE CONCAT(League,Date,Player) = '{oHelper.sLeagueName}{ctx.ActiveDate}{oHelper.sPlayer}' "
                        Dim lrc = oHelper.SqliteTrans(insertUpdQuery)
                        If lrc = 0 Then
                            insertUpdQuery = $"INSERT INTO Handicaps (ID,League,Player,Date,Round,Hdcp,PHdcp) VALUES('0','{oHelper.sLeagueName}','{oHelper.sPlayer}','{ctx.ActiveDate}','','{iHdcp}','{iHdcp}' "
                        End If
                        MLogit(insertUpdQuery)
                        oHelper.dt = oHelper.sqlitedaFromSql(connection, "Handicaps", $"SELECT * FROM Handicaps WHERE CONCAT(League,Date,Player) = '{oHelper.sLeagueName}{ctx.ActiveDate}{oHelper.sPlayer}' ")
                        oHelper.wk = ""
                        Dim ddiff As Decimal = CDbl(iHdcp) - CDbl(row("Phdcp"))
                        Dim newrow As DataRow = dtMisMatch.NewRow()
                        newrow("Date") = row("Date")
                        newrow("Player") = row("Player")
                        newrow("Field") = "Handicap"
                        newrow("Bad Value") = row("Phdcp")
                        newrow("Good Value") = CDbl(iHdcp)
                        newrow("Difference") = ddiff
                        newrow("Comments") = $"Bad Handicap {row("Last5")}"
                        dtMisMatch.Rows.Add(newrow)

skipfix:
                    End If
                Next
                'this code is here to fix prior league handicaps which calc was wrong because it included post season scores and it shouldnt have but ignore post season now (2018 had combined post season reg season)


                If dtscores.Rows.Count > 0 Then
                    If ctx.ActiveDate >= oHelper.sPSDate Then
                        oHelper.wk = ""
                        'this is post season skins, need to handle seperately in near future
                        Continue For
                    End If
                    'Dim lallHoles As List(Of String) = oHelper.CalcSkins(filteredRows.CopyToDataTable)
                    Dim lallHoles As List(Of String) = oHelper.CalcSkins(dtscores)
                    ' Pare down the list to just the holes that have skins
                    'lskins has hole~score-rowids seperated by |
                    'dim below identifies skins only (only 1 rowid(player)
                    Dim lskins As List(Of String) = lallHoles.Where(Function(indskin) Not indskin.Contains("|")).ToList()
                    Dim deachskin As Decimal = 0
                    Dim strsql As String = ""
                    sb = New StringBuilder
                    sb.AppendLine($"
SELECT
    COUNT(*) AS TotalPlayers,
    CASE 
        WHEN P.Date >= {oHelper.sPSDate} THEN LP.SkinsPS 
        ELSE LP.Skins 
    END AS PerSkin,
    SUM(CASE 
        WHEN P.Date >= {oHelper.sPSDate} THEN LP.SkinsPS 
        ELSE LP.Skins 
    END * 1) AS SkinsCollected
FROM Payments P
JOIN LeagueParms LP ON LP.Season = SUBSTR(CAST(P.Date AS TEXT), 1, 4)")
                    If ctx.ActiveDate >= oHelper.sPSDate Then
                        sb.AppendLine($"WHERE P.Date in ({oHelper.sPSDate},{oHelper.sPSDate2}) ")
                    Else
                        sb.AppendLine($"WHERE CONCAT(P.Date, P.[Desc], P.Detail) = '{ctx.ActiveDate}SkinPayment'")
                    End If
                    sb.AppendLine("GROUP BY P.League, LP.Skins, LP.SkinsPS;")
                    'mlogit(strsql.ToString)
                    oHelper.wk = sb.ToString
                    Dim dtTotalScoreCard = oHelper.sqlitedaFromSql(connection, "TotalScoreCard", sb.ToString)
                    'if no skins collected, exit
                    If dtTotalScoreCard.Rows.Count = 0 Then Exit Sub
                    If lskins.Count > 0 Then
                        deachskin = Math.Floor(dtTotalScoreCard.Rows(0)(Constants.skinscol) / lskins.Count)
                    End If
                    Dim sWHERE As String = $"WHERE Date = '{ctx.ActiveDate}' AND [Desc] LIKE '%Skin%' AND Detail LIKE '#%'"
                    sb = New StringBuilder
                    sb.AppendLine($"
SELECT * FROM Payments {sWHERE}")
                    strsql = sb.ToString
                    oHelper.dt = oHelper.sqlitedaFromSql(connection, "TotalScoreCard", strsql)
                    '20250630-at this point, we should delete all payments for skins on this date, rebuild new payments for this date for skins.

                    Dim todeleteSkins = oHelper.sqlitedaFromSql(connection, "", $"select * from Payments {sWHERE}")

                    Dim Query = $"DELETE FROM Payments {sWHERE} "
                    Dim drc = oHelper.SqliteTrans(Query)
                    oHelper.wk = ""
                    Dim sID As String = oHelper.sqlitedaFromSql(connection, "", $"SELECT ID FROM Payments ORDER BY ID DESC LIMIT 1").Rows(0)(0).ToString()
                    'Dim dvscores As DataView = New DataView(dtscores)
                    dvscores.RowFilter = $"Date = '{ctx.ActiveDate}'"
                    For Each indskin In lskins
                        oHelper.sPlayer = dvscores(indskin.Split("~")(1))("Player")
                        Dim sHole As String = indskin.Split("-")(0)
                        If dvscores(0)("FrontBack") = "Back" Then sHole = sHole + 9
                        Dim sScore As String = $"Score={indskin.Split("-")(1).Split("~")(0)}"
                        sb = New StringBuilder
                        sb.AppendLine($"
INSERT INTO Payments (ID, League, Date, Player, Desc, Detail, Earned, DatePaid, Comment, Paymethod) VALUES ({sID + 1},'{oHelper.sLeagueName}','{ctx.ActiveDate}','{oHelper.sPlayer}','Skin','#{sHole}',{deachskin},'{ctx.ActiveDate}','{sScore}',''   
)")
                        strsql = sb.ToString
                        drc = oHelper.SqliteTrans(strsql)
                        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT * FROM Payments ORDER BY ID DESC LIMIT 1")
                        sID += 1
                        Dim keys() As Object = {ctx.ActiveDate, oHelper.sPlayer, oHelper.dt.Rows(0)("Detail")}
                        Dim drfind = dtpSkins.Rows.Find(keys)
                        If drfind IsNot Nothing Then
                            ''MLogit($"Skin Record found for {ctx.ActiveDate}-{oHelper.sPlayer}-{oHelper.dt.Rows(0)("Detail")}-{oHelper.dt.Rows(0)("Comment")}-{oHelper.dt.Rows(0)("Earned")}")
                            Dim sFld As String = "Comment"
                            If drfind(sFld) <> oHelper.dt.Rows(0)(sFld) Then
                                MLogit($"Score({sFld}) Difference Old {drfind(sFld)} New {oHelper.dt.Rows(0)(sFld)}")
                                Dim ddiff As Decimal = drfind(sFld).ToString.Replace("Score=", "") - oHelper.dt.Rows(0)(sFld).ToString.Replace("Score=", "")
                                Dim newrow As DataRow = dtMisMatch.NewRow()
                                newrow("Date") = ctx.ActiveDate
                                newrow("Player") = oHelper.sPlayer
                                newrow("Field") = "Recalculated Skin Score"
                                newrow("Bad Value") = drfind(sFld).ToString.Replace("Score=", "")
                                newrow("Good Value") = oHelper.dt.Rows(0)(sFld).ToString.Replace("Score=", "")
                                newrow("Difference") = ddiff
                                newrow("Hole") = oHelper.dt.Rows(0)("Detail").ToString.Replace("#", "")
                                newrow("Score") = oHelper.dt.Rows(0)("Comment").ToString.Replace("Score=", "")
                                dtMisMatch.Rows.Add(newrow)
                                oHelper.wk = ""
                            End If
                            sFld = "Earned"
                            If drfind(sFld).ToString.Replace("-", "") <> oHelper.dt.Rows(0)(sFld) Then
                                Dim ddiff As Decimal = drfind(sFld) - oHelper.dt.Rows(0)(sFld)
                                MLogit($"{ctx.ActiveDate}-{oHelper.sPlayer}-{oHelper.dt.Rows(0)("Detail")}-Score({sFld}) Difference Old {drfind(sFld)} New {oHelper.dt.Rows(0)(sFld)}")
                                Dim newrow As DataRow = dtMisMatch.NewRow()
                                newrow("Date") = ctx.ActiveDate
                                newrow("Player") = oHelper.sPlayer
                                newrow("Field") = "Recalculated Skin Value"
                                newrow("Bad Value") = drfind(sFld)
                                newrow("Good Value") = oHelper.dt.Rows(0)(sFld)
                                newrow("Difference") = ddiff
                                newrow("Hole") = oHelper.dt.Rows(0)("Detail").ToString.Replace("#", "")
                                newrow("Score") = oHelper.dt.Rows(0)("Comment").ToString.Replace("Score=", "")
                                dtMisMatch.Rows.Add(newrow)
                                oHelper.wk = ""
                            End If
                        Else
                            MLogit($"No Skin Record found for {ctx.ActiveDate}-{oHelper.sPlayer}-{oHelper.dt.Rows(0)("Detail")}")
                            Dim newrow As DataRow = dtMisMatch.NewRow()
                            Dim sfld As String = "Comment"
                            newrow("Date") = ctx.ActiveDate
                            newrow("Player") = oHelper.sPlayer
                            newrow("Field") = "Skin Value"
                            newrow("Good Value") = oHelper.dt.Rows(0)(sfld).ToString.Replace("Score=", "")
                            newrow("Hole") = oHelper.dt.Rows(0)("Detail").ToString.Replace("#", "")
                            newrow("Score") = oHelper.dt.Rows(0)("Comment").ToString.Replace("Score=", "")
                            dtMisMatch.Rows.Add(newrow)
                        End If
                        oHelper.wk = ""
                    Next
                End If

            Next
            MLogit($"{ctx.ActiveDate}|Bad handicaps {ibadhdcps} for {season("Season")}")
#Region "Not Needed"
            'fix players with nolastname no need, done manually
            Dim dtPlayers As DataTable = oHelper.sqliteda(connection, "Players")
            Dim dtNewScores As DataTable = oHelper.sqliteda(connection, "Scores")
            Dim dvp As DataView = New DataView(dtPlayers)
            oHelper.dt = oHelper.sqlitedaFromSql(connection, "Players", $"SELECT * FROM Players WHERE LastName LIKE '%NoLast%'")
            Dim dtScoresNLN = oHelper.sqlitedaFromSql(connection, "Scores", $"SELECT * FROM Scores WHERE Player LIKE '%NoLast%'")
            Dim dvos2 As DataView = New DataView(dtOldScores)
            dvos2.RowFilter = $"Player LIKE '%nolast%'"
            If dvos2.Count > 0 Then
                MLogit($"Found {dvos2.Count} Old Scores with NoLastName")
            End If
            For Each prow In dvos2
                MLogit($"Found {dvos2.Count} Old Scores with {prow("firstname")}")
            Next
            dvos2.RowFilter = $"Player LIKE '%nolast%'"
            For Each row As DataRow In oHelper.dt.Rows
                dvp.RowFilter = $"FirstName = '{row("FirstName")}' AND LastName <> '{row("LastName")}'"
                Dim dvpossibles As DataView = New DataView(dtNewScores)
                dvpossibles.RowFilter = $"Player LIKE '%{row("FirstName")}' AND LastName <> '{row("LastName")}'"
                For Each prow As DataRowView In dvp
                    MLogit($"{ibadhdcps}-{ctx.ActiveDate}-{row("Player")}-Old Scores Hdcp {row("Phdcp")}-fixed Hdcp {row("CalcHdcp")}")
                Next
            Next
#End Region

            GoTo skipnolastname

skipnolastname:
            oHelper.dt = oHelper.sqliteda(connection, "Payments")
            oHelper.wk = ""
            'at this point, we can fix the following:
            '   LeagueParms:EndDate
            '   LeagueParms:PostSeasonDt
            '   LeagueParms:Rainouts
            '   Players:nolastname
            '   Handicaps:Phdcp (calchdcp is corrected one)
            '   Payments (delete all skins and replace with new recalculated skins)
            '
            '            For Each row As DataRowView In dvOS
            '                Dim dtscores As DataTable
            '                dtscores = New DataTable
            '                For Each col As DataColumn In dts.Columns
            '                    If col.ColumnName = "Date" OrElse col.ColumnName = "Player" Then
            '                        dtscores.Columns.Add(col.ColumnName, col.DataType)
            '                    ElseIf col.ColumnName.StartsWith("$") Then
            '                        dtscores.Columns.Add(col.ColumnName, GetType(Double))
            '                    Else
            '                        dtscores.Columns.Add(col.ColumnName, GetType(String))
            '                    End If
            '                Next

            '                Dim lallHoles As List(Of String) = oHelper.FCalcSkins(dtscores)
            '                If Not String.IsNullOrEmpty(row("$Skins").ToString) Then
            '                    Dim dv = New DataView(dtpSkins)
            '                    dv.RowFilter = $"Player = '{row("Player")}' AND Date = {row("Date")}"
            '                    If dv.Count = 0 Then
            '                        Dim sb = New StringBuilder
            '                        sb.AppendLine($"
            'INSERT INTO Payments (ID,League,Date,Player,Desc,Detail,Earned,DatePaid,Comment,Paymethod) 
            'VALUES (0,'{oHelper.sLeagueName}','{row("Date")}','{row("Player")}','Skin','#{row("$Skins")}',{row("$Skins")}).'','Score=',''")
            '                        Dim insertUpdQuery = sb.ToString
            '                        Dim lrc = oHelper.SqliteTrans(insertUpdQuery)
            '                    End If

            '                    For Each drow As DataRowView In dv
            '                        Dim bupd As Boolean = False
            '                        Dim skinsearch As String = $"{drow("Detail")}"
            '                        Dim drfskin = dtpSkins.Rows.Find(New Object() {row("Date"), row("Player"), drow("Detail")})
            '                        If drfskin IsNot Nothing Then
            '                            If drfskin("Earned") < 0 Then
            '                                drfskin("Earned") *= -1
            '                                bupd = True
            '                            End If
            '                        Else
            '                            'Missing Payment record
            '                            oHelper.wk = ""
            '                        End If
            '                        If bupd Then
            '                            Dim insertUpdQuery = $"UPDATE Payments SET Earned = '{drfskin("Earned")}' WHERE CONCAT(League,Date,Desc,Player) = '{drfskin("League").ToString.Replace("'", "''")}{drfskin("Date")}Skin{drfskin("Player")}' "
            '                            Dim lrc = oHelper.SqliteTrans(insertUpdQuery)
            '                            mlogit(insertUpdQuery)
            '                        End If
            '                        oHelper.dt = oHelper.sqlitedaFromSql(connection, "Payments", $"SELECT * FROM Payments WHERE Desc = 'Skin' AND Detail LIKE '#%' ORDER BY Date Asc")
            '                        oHelper.wk = ""
            '                    Next
            '                End If
            '                'Dim drfs = dts.Rows.Find(New Object() {row("Date"), row("Player")})

            '                'If drfs("FrontBack") = "Front" Then

            '                'End If
            '                'Dim drfp = dtp.Rows.Find(New Object() {row("Date"), row("Player")}, "Skins")
            '                'mlogit("")

            '            Next
        Next
    End Sub
    Function HoleByHoleFix(newScoreRow As DataRow, dros As DataRow, offset As Integer) As Integer
        HoleByHoleFix = 0
        For i As Integer = 1 To 9

            Dim newValObj = newScoreRow(i.ToString())
            If Not IsNumeric(newValObj) Then Continue For
            Dim newVal As Integer = CInt(newValObj)

            Dim oldColName As String = $"Hole{i + offset}"
            Dim oldVal As Integer = If(IsDBNull(dros(oldColName)), 0, CInt(dros(oldColName)))
            HoleByHoleFix += oldVal
            If oldVal = newVal Then Continue For
            buildMM(dros("Date").ToString,
                    dros("Player").ToString,
                    $"Hole ({i})",
                    newVal.ToString,
                    oldVal.ToString,
                    oldVal.ToString,
                    i.ToString,
                    (newVal - oldVal).ToString,
                    "Hole Value")
            Dim lrc As Integer
            sb = New StringBuilder
            sb.AppendLine($"UPDATE Scores SET [{i}] = {newVal}")
            sb.AppendLine($"
WHERE League = '{ctx.SafeLeagueName}' '
AND Player = '{oHelper.sPlayer}'
AND Date = {ctx.ActiveDate};")
            oHelper.wk = sb.ToString
            lrc = oHelper.SqliteTrans(sb.ToString)
            oHelper.wk = ""
        Next
    End Function
    Sub buildMM(matchDate As String,
                player As String,
                field As String,
                bv As String,
                gv As String,
                Optional score As String = "",
                Optional hole As String = "",
                Optional diff As String = "",
                Optional comment As String = "")
        Try

            Dim mm As DataRow
            ' Log mismatch
            mm = dtMisMatch.NewRow()
            mm("Date") = matchDate
            mm("Player") = player
            mm("Field") = field
            If bv IsNot "" Then mm("Bad Value") = bv
            If gv IsNot "" Then mm("Good Value") = gv
            If hole IsNot "" Then mm("Hole") = hole
            If score IsNot "" Then mm("Score") = score
            If diff IsNot "" Then mm("Difference") = diff
            If comment IsNot "" Then mm("Comments") = comment
            dtMisMatch.Rows.Add(mm)
        Catch ex As Exception

        End Try
    End Sub
    Public Function findMM(player As String,
                                    matchDate As String,
                                    dtOldScores As DataTable,
                                    dtNewScores As DataTable,
                                    dtMatches As DataTable,
                                    dtPartners As DataTable,
                                    dtHandicaps As DataTable,
                                    dtMisMatch As DataTable
                                  ) As Boolean

        '---------------------------------------------------------
        ' 1. GET OLD SCORE ROW
        '---------------------------------------------------------
        Dim dros As DataRow = dtOldScores.Rows.Find(New Object() {player, matchDate})
        If dros Is Nothing Then Return False

        '---------------------------------------------------------
        ' 2. DETERMINE FRONT/BACK AND OLD GROSS
        '---------------------------------------------------------
        Dim sfb As String = "Front"
        Dim oldGross As Integer = 0

        If IsNumeric(dros("Out_Gross")) AndAlso IsNumeric(dros("Hole1")) Then
            oldGross = CInt(dros("Out_Gross"))
            sfb = "Front"
        ElseIf IsNumeric(dros("In_Gross")) AndAlso IsNumeric(dros("Hole10")) Then
            oldGross = CInt(dros("In_Gross"))
            sfb = "Back"
        End If

        '---------------------------------------------------------
        ' 3. HOLE‑BY‑HOLE MISMATCH CHECK
        '---------------------------------------------------------
        Dim offset As Integer = If(sfb = "Front", 0, 9)
        Dim newScoreRow As DataRow = dtNewScores.Rows.Find(New Object() {player, matchDate})
        If newScoreRow Is Nothing Then Return False
        Dim newcalcGross As Integer = HoleByHoleFix(newScoreRow, dros, offset)

        If newcalcGross <> oldGross Then
            buildMM(dros("Date").ToString, dros("Player").ToString, $"Gross", oldGross, newcalcGross, "", "", newcalcGross - oldGross)
        End If

        Dim hdcpRow As DataRow = dtHandicaps.Rows.Find(New Object() {matchDate, player})
        If hdcpRow Is Nothing Then Return False

        Dim iHdcp As Integer = CInt(hdcpRow("PHdcp"))
        dvthisPlayer.RowFilter = $"Player = '{player}' AND Date < {matchDate} AND Date NOT IN ({ctx.sExcludeDates})"
        If dvthisPlayer.Count = 0 Then
            Dim dvos = New DataView(dtOldScores)
            dvos.RowFilter = $"Player = '{player}' AND Date < {matchDate} AND Date NOT IN ({ctx.sExcludeDates})"
            If dvos.Count = 0 Then
                oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT * FROM Scores WHERE Player = '{player}' AND Date = {matchDate}")
            End If
        Else
            oHelper.dt = dvthisPlayer.ToTable
        End If

        oHelper.dt.Columns.Add("Par", GetType(Integer))
        oHelper.dt.Columns("Gross").ColumnName = "Score"

        For Each drscore As DataRow In oHelper.dt.Rows
            drscore("Par") = If(drscore("FrontBack") = "Front", oHelper.thisCourse("In"), oHelper.thisCourse("Out"))
        Next
        Dim hdcpinfo As String = oHelper.calcHdcp(oHelper.dt, False)

        Dim parts = hdcpinfo.Split("-"c)
        Dim inewHdcp As Integer = CInt(parts(0))

        If inewHdcp <> iHdcp Then
            buildMM(matchDate, player, $"Handicap", iHdcp, inewHdcp, "", "", inewHdcp - iHdcp, $"Last 5 = {parts(1)}")
        End If

        If newScoreRow("Net") = "" Then
            buildMM(matchDate, player, $"Net", newScoreRow("Net"), newcalcGross - iHdcp)
        End If

        If dros("Method") = "Net" Then
            newcalcGross += iHdcp
        End If

        Dim newNet As Integer = newcalcGross - iHdcp
        Dim oldNet As Integer = oldGross - iHdcp
        If oldNet <> newNet Then
            buildMM(matchDate, player, $"Net", oldNet, newNet, "", "", newNet - oldNet)
        End If
        'check to see if this player/date had any adjustments
        Dim dvmm = New DataView(dtMisMatch)
        dvmm.RowFilter = $"player = '{player}' AND date = {matchDate}"
        If dvmm.Count = 0 Then Return False
        'setup sql to update the scores record
        Using transaction = connection.BeginTransaction()
            '---------------------------------------------------------
            ' 7. OPPONENT NET DOES NOT CHANGE
            '---------------------------------------------------------
            Dim Opponent As String = dtMatches.Rows.Find(New Object() {player, matchDate})("Opponent").ToString()
            Dim drfind As DataRow = dtNewScores.Rows.Find(New Object() {Opponent, matchDate})
            Dim OpponentNet As Integer = 999
            If drfind IsNot Nothing Then
                If drfind("Net") = "" Then
                    drfind = dtMisMatch.Rows.Find(New Object() {matchDate, Opponent, "Net"})
                    OpponentNet = drfind("Good Value")
                Else
                    OpponentNet = CInt(drfind("Net"))
                End If
            End If
            '---------------------------------------------------------
            ' 8. POINTS CHANGE CHECK
            '---------------------------------------------------------
            Dim oldPoints As Integer = CInt(dtMatches.Rows.Find(New Object() {player, matchDate})("Points"))

            Dim newPoints As Integer = 0
            If newNet < OpponentNet Then
                newPoints = 1
            ElseIf newNet > OpponentNet Then
                newPoints = 0
            Else
                newPoints = 0.5
            End If

            If newPoints <> oldPoints Then
                buildMM(matchDate, player, $"Points", oldPoints, newPoints, "", "", newPoints - oldPoints)
                sb.AppendLine($"
Update Matches set Points = {newPoints}
WHERE League = '{ctx.SafeLeagueName}' '
AND Player = '{player}'
AND Date = {ctx.ActiveDate};
")
                'no need to do this,well loop through again later
                GoTo skipopponentpoints
                oldPoints = CInt(dtMatches.Rows.Find(New Object() {Opponent, matchDate})("Points"))
                If newNet < OpponentNet Then
                    newPoints = 0
                ElseIf newNet > OpponentNet Then
                    newPoints = 1
                Else
                    newPoints = 0.5
                End If
                buildMM(matchDate, Opponent, $"Points", oldPoints, newPoints, "", "", newPoints - oldPoints)

                sb.AppendLine($"
Update Matches set Points = {newPoints}
WHERE League = '{ctx.SafeLeagueName}' '
AND Player = '{Opponent}'
AND Date = {ctx.ActiveDate};
")
skipopponentpoints:
                'oHelper.wk &= sb.ToString
                'lrc = oHelper.SqliteTrans(oHelper.wk)
                'oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"select * from scores where player = '{player}' and date = {matchDate}")

                'Return True    
                '---------------------------------------------------------
                ' 9. TEAM POINTS CHANGE CHECK (A‑player only)
                '---------------------------------------------------------
                Dim partnerNum As Integer = If(IsDBNull(newScoreRow("Partner")), 0, CInt(newScoreRow("Partner")))
                Dim isAPlayer As Boolean = (partnerNum Mod 2 = 0)
                Dim partner As String = dtPartners.Rows.Find(New Object() {matchDate, player})("Partner").ToString()

                ' A-player is always the one with even partner number
                Dim aPlayer As String = If(isAPlayer, player, partner)
                Dim oldTeamPoints As Integer = CInt(dtMatches.Rows.Find(New Object() {aPlayer, matchDate})("Team_Points"))

                Dim teamnet As Integer = CInt(dtNewScores.Rows.Find(New Object() {partner, matchDate})("Net"))
                teamnet += newNet

                Dim opartner As String = dtPartners.Rows.Find(New Object() {matchDate, Opponent})("Partner").ToString()
                Dim OpponentTeamNet As Integer = OpponentNet
                OpponentNet += CInt(dtNewScores.Rows.Find(New Object() {opartner, matchDate})("Net"))

                Dim newTeamPoints As Integer = 0
                If teamnet < OpponentNet Then
                    newTeamPoints = 1
                ElseIf teamnet = OpponentNet Then
                    newTeamPoints = 0.5
                End If
                If newTeamPoints <> oldTeamPoints Then
                    buildMM(matchDate, Opponent, $"Team Points", oldTeamPoints, newTeamPoints, "", "", newTeamPoints - oldTeamPoints)
                    sb.AppendLine($"
Update Matches set Team_Points = {newTeamPoints}
WHERE League = '{ctx.SafeLeagueName}' 
AND Player = '{aPlayer}'
AND Date = {ctx.ActiveDate};
")
                End If
                'next fix handicaps
                Return True

                'Else
                'Debug.Print("Points did not change")
            End If

            '---------------------------------------------------------
            ' 11. NO IMPACT
            '---------------------------------------------------------
            oHelper.wk = sb.ToString
            Dim lrc = oHelper.SqliteTrans(oHelper.wk)
            Try
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                LOGIT($"{ex.Message}")
            End Try
            oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"select * from scores where player = '{player}' and date = {matchDate}")
            oHelper.wk = ""

        End Using

        Return False

    End Function

#Region "20251118-Not Needed-remove earnings table"
    Sub TableCleanup()
        '
        '   Fix recap
        '   1)  Make All payments amounts positive
        '   2)  Fix Subs/Regulars
        '       2018 Todd Fey,Gary Reeves,Mike Arnold      were marked as a regulars and were a subs
        '       2018 Tony Fey,Jerry Arrasmith,Don Johnston should have been the regulars
        '       2020 Will Huenefeld had to quit being a regular mid-season and was replaced by Paul Wilburn, decided to make Paul the regular
        '       2024 Tony Smith was a regular but somehow Sub records were added for him, so we must delete
        ''
        dtOldScores = oHelper.ReadCsvIntoDataTable("C:\HughsGolf\Files\Scores.csv")
        dtOldScores.PrimaryKey = New DataColumn() {dtOldScores.Columns("Player"), dtOldScores.Columns("Date")}
        dtNewScores = createDTWithPK("AllScores", "Player,Date", $"SELECT * FROM Scores WHERE CAST(GROSS AS INTEGER) > 0 ORDER BY Date, Partner")
        dtMisMatch = createDT(mmflds)
        dtpSkins = oHelper.sqlitedaFromSql(connection, "Payments", $"SELECT * FROM Payments WHERE Desc LIKE 'Skin%' AND Detail LIKE '#%' ORDER BY Date Asc")
        dtpSkins.PrimaryKey = New DataColumn() {dtpSkins.Columns("Date"), dtpSkins.Columns("Player"), dtpSkins.Columns("Detail")}
        dvthisPlayer = New DataView(dtNewScores)

        fix1()
        fix2()
        'used by report3, but cant build until fix2 is complete
        dtSubs = createDTWithPK("Subs", "Date,Player", $"SELECT * FROM Subs ORDER BY Date")
        report3()

        dtTeams = createDTWithPK("Teams", "Year,Player", $"SELECT * FROM Teams ORDER BY Year,Team,Grade")
        'used by report4,must be built before report4
        'report4 creates a partners table for research if a partner is missing
        dtPartners = createDT("Date,Player,Grade,Team,Partner,PartnerGrade,Order-d")
        report4()
        'report5 creates a compare table for earnings vs payments
        report5()

        '20251123-scores for 20190416 are fried, recreate from oldscores from lggram one drive
        Dim ifixed As Integer = 0

        fix6()
        fix7()
        'MLogit($"{ctx.ActiveDate}-Bad handicaps so far {ibadhdcps}")
        oHelper.wk = ""
        'at this point, lets create a workbook from the mismatch table and save it
#End Region
    End Sub

End Class
