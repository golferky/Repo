Public Class Unused_Routines
    Public oHelper As Helper
    'Dim lvwColumnSorter As ListViewColumnSorter
    Dim fromsizeW As Integer, lvsizeW As Integer
    Sub CreateSch()
        '20180115 - latest attempt to build a schedule
        Dim iTotTeams As Integer = oHelper.rLeagueParmrow("Teams")
        Dim iTotHTeams As Integer = iTotTeams / 2
        Dim iTotATeams As Integer = iTotTeams - iTotHTeams

        Dim sWeeks As New ArrayList
        'Dim Rndm As New Random(System.DateTime.Now.Millisecond)
        For idx = 0 To iTotTeams - 1
            Dim sAllTeams As New List(Of String), sHteams As New List(Of String), sAteams As New List(Of String), iextra As Integer
            Dim J As Integer, AL As New List(Of String)
            'create a string list of A players based on lowest half being A players
            'AL is the list of all players
            For i = 0 To iTotTeams - 1
                AL.Add(i.ToString)
            Next
            'loop and create an A players list from the lowest half
            'For i = 0 To iTotHTeams - 1
            '    If i < iTotHTeams Then
            '        sHteams.Add(i)
            '        AL.Remove(i)
            '    End If
            'Next
            For i = 0 To iTotHTeams - 1
                J = GetRandomNumber(0, iTotHTeams - 1)
                sHteams.Add(AL(J) & " ")
                AL.RemoveAt(J)
            Next
            'create a random extra b player if uneven(Bye)
            If iTotHTeams <> iTotATeams Then
                J = GetRandomNumber(1, AL.Count)
                sAteams.Add(AL(J) & " ")
                iextra = AL(J)
            End If

            'make B players random
            For i = 0 To AL.Count - 1
                J = GetRandomNumber(0, AL.Count - 1)
                sAteams.Add(AL(J) & " ")
                AL.RemoveAt(J)
            Next
            'now collect pairings
            For i = 0 To sHteams.Count - 1
                sAllTeams.Add(sHteams(i) + 1 & "v" & sAteams(i) + 1)
            Next

            sWeeks.Add(sAllTeams)

        Next
    End Sub
    'Public Sub CreateTables(sTables As List(Of String))
    '	For Each sTable In sTables
    '		dsLeague.Tables.Add(sTable).TableName = sTable
    '		Dim sFile = oHelper.sFilePath & "\" & sTable.Substring(2) & ".xml"
    '		If File.Exists(sFile) Then
    '			dsLeague.Tables(sTable).ReadXml(sFile)
    '		End If
    '	Next
    'End Sub
    'Sub Genschedule()
    '    Dim sHolidays = oHelper.getHolidayList(Now.Year)
    '    Dim sTuesHol As New List(Of Date)
    '    For Each sholiday In sHolidays
    '        If sholiday.DayOfWeek = DayOfWeek.Tuesday Then
    '            sTuesHol.Add(sholiday)
    '        End If
    '    Next
    '    'Dim xx = GenerateRoundRobinEven(12)
    '    'For Each x In xx
    '    '    Dim zz = x

    '    'Next
    '    Dim iteams As Integer = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("Teams")
    '    Dim iNumWeeks = DateDiff("w", oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("EndDate"), oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("StartDate"))
    '    Dim dt As New DataTable
    '    Dim istartdate As Date = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("StartDate")
    '    Dim iweeks As Integer = ((oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("Teams") - 1) * 2) + sTuesHol.Count - 1
    '    For i = 1 To iweeks
    '        Dim bitstues = False
    '        For Each stdate In sTuesHol
    '            If stdate = istartdate Then
    '                bitstues = True
    '                Exit For
    '            End If
    '        Next
    '        If Not bitstues Then
    '            dt.Columns.Add(istartdate)
    '        End If
    '        istartdate = istartdate.AddDays(7)
    '    Next

    '    'build a table for each team for each week (12 teams x 11 weeks)
    '    'Dim allteams As New List(Of String)
    '    'For ii = 1 To iteams
    '    '    allteams.Add(ii)
    '    'Next

    '    '-----
    '    Dim aAllTeams As New List(Of ArrayList)
    '    For icurrteam = 0 To iteams - 1
    '        'Teams.Clear() if this is declared above
    '        Dim Teams As ArrayList = New ArrayList
    '        Teams2.Clear()

    '        Dim delimStr As String = vbCrLf
    '        Dim delimiter As Char() = delimStr.ToCharArray()

    '        For s = 0 To iteams - 1
    '            Teams.Add(New Team(s))
    '        Next

    '        If Teams.Count Mod 2 <> 0 Then
    '            Teams.Add(New Team("Bye"))
    '        End If

    '        Teams.Sort()
    '        'at this point, teams has all 12 teams in it at random
    '        'find team 1 and remove it

    '        Dim iteam2delete = 0
    '        For Each team As Team In Teams
    '            If team.TeamName = icurrteam Then
    '                Teams.RemoveAt(iteam2delete)
    '                Exit For
    '            End If
    '            iteam2delete += 1
    '        Next
    '        'at this point we have all of a teams opponents in teams array
    '        aAllTeams.Add(Teams)
    '    Next

    '    Dim iteam = 1
    '    oHelper.bloghelper = True
    '    For Each team In aAllTeams
    '        Dim xxxxxxxxxxxxxxxxxxxxxxxxx = ""
    '        Dim steams = ""
    '        For Each steam In team
    '            steams = steams & (steam.teamname + 1) & "-"
    '        Next
    '        oHelper.LOGIT("Team #" & iteam & "-" & steams)
    '        iteam += 1

    '    Next

    '    oHelper.bloghelper = False

    '    ' Teams2 = New ArrayList(Teams.ToArray())
    '    'For Each team In Teams
    '    '    Dim x = ""
    '    '    Dim j = oHelper.GetRandomNumber(0, Teams2.Count - 1)
    '    'Next
    '    '-----

    '    'Dim OneTeamSch As New List(Of String)
    '    'Dim AllTeamsSch As New List(Of String)

    '    'For i = 1 To iteams
    '    '    AllTeamsSch.Add(i)
    '    'Next

    '    'For i = 0 To iteams - 1
    '    '    'generate a number of 0 to 11, - 10, etc
    '    '    Dim j = oHelper.GetRandomNumber(0, allteams.Count - 1)
    '    '    'use that number as a subscript into the all teams list for its opponent
    '    '    AllTeamsSch(i) = AllTeamsSch(i) & "-" & allteams(j)
    '    '    Dim y = ""
    '    '    'OneTeamSch.Add(allteams(j))

    '    'Next

    '    'Dim x = ""
    '    'oHelper.dsLeague = New dsLeague
    '    'this converts the csvs to xml eliminating the need for microsoft office
    '    'this replaces csv2datable
    '    'CreateTables(New List(Of String)("dtLeagueParms,dtCourses,dtPlayers,dtScores".Split(",")))

    '    'oHelper.dsLeague.Tables("dtLeagueParms").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtLeagueParms").Columns("Name")}
    '    'oHelper.dsLeague.Tables("dtCourses").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtCourses").Columns("Name")}
    '    'oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}
    '    'Helper.dsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtScores").Columns("Player"), oHelper.dsLeague.Tables("dtScores").Columns("Date")}

    '    'test loading a new schedule

    '    'If oHelper.sLeagueName <> "" Then
    '    '    CreateTables(New List(Of String)("dtSchedule".Split(",")))
    '    '    oHelper.buildSchedule()
    '    'End If
    '    'Dim line = File.ReadAllText(oHelper.sFilePath & "\" & "t.csv")
    '    'Dim x = line.Replace(vbLf, String.Empty).Split(vbCrLf)
    '    'Dim z = ""

    '    'For Each table As DataTable In oHelper.dsLeague.Tables
    '    '    Dim sfilename = "\" & table.TableName.Substring(2) & ".csv"
    '    '    oHelper.DataTable2CSV(table, oHelper.sFilePath & sfilename)
    '    'Next
    'End Sub
    Private Const BYE As Integer = -1

    ' Return an array where results(i, j) gives the opponent of
    ' team i in round j.
    ' Note: num_teams must be odd.
    Private Function GenerateRoundRobinOdd(ByVal num_teams As _
    Integer) As Integer(,)
        Dim n2 As Integer
        Dim results(,) As Integer
        Dim teams() As Integer
        Dim i As Integer
        Dim round As Integer
        Dim team1 As Integer
        Dim team2 As Integer

        n2 = num_teams \ 2
        ReDim results(num_teams - 1, num_teams - 1)

        ' Initialize the list of teams.
        ReDim teams(num_teams - 1)
        For i = 0 To num_teams - 1
            teams(i) = i
        Next i

        ' Start the rounds.
        For round = 0 To num_teams - 1
            For i = 0 To n2 - 1
                team1 = teams(n2 - i)
                team2 = teams(n2 + i + 1)
                results(team1, round) = team2
                results(team2, round) = team1
            Next i

            ' Set the team with the bye.
            team1 = teams(0)
            results(team1, round) = BYE

            ' Rotate the array.
            RotateArray(teams)
        Next round

        Return results
    End Function
    ' Rotate the entries one position.
    Private Sub RotateArray(ByVal teams() As Integer)
        Dim tmp As Integer
        Dim i As Integer

        tmp = teams(UBound(teams))
        For i = UBound(teams) To 1 Step -1
            teams(i) = teams(i - 1)
        Next i
        teams(0) = tmp
    End Sub
    ' Return an array where results(i, j) gives the opponent of
    ' team i in round j.
    ' Note: num_teams must be even.
    Private Function GenerateRoundRobinEven(ByVal num_teams As _
    Integer) As Integer(,)
        Dim results(,) As Integer
        Dim results2(,) As Integer
        Dim round As Integer
        Dim team As Integer

        ' Generate the result for one fewer teams.
        results = GenerateRoundRobinOdd(num_teams - 1)

        ' Copy the results into a bigger array,
        ' replacing the byes with the extra team.
        ReDim results2(num_teams - 1, num_teams - 2)
        For team = 0 To num_teams - 2
            For round = 0 To num_teams - 2
                If results(team, round) = BYE Then
                    ' Change the bye to the new team.
                    results2(team, round) = num_teams - 1
                    results2(num_teams - 1, round) = team
                Else
                    results2(team, round) = results(team, round)
                End If
            Next round
        Next team

        Return results2
    End Function
    ' Return an array where results(i, j) gives the opponent of
    ' team i in round j.
    Private Function GenerateRoundRobin(ByVal num_teams As _
    Integer) As Integer(,)
        If num_teams Mod 2 = 0 Then
            GenerateRoundRobin =
            GenerateRoundRobinEven(num_teams)
        Else
            GenerateRoundRobin =
            GenerateRoundRobinOdd(num_teams)
        End If
    End Function
    'Imports System.Web

    '20180802-dont use yet
    'Public Class WA
    '    Dim fromsizeW As Integer, lvsizeW As Integer

    '    Public dsLeague As DataSet
    '    Public sFilePath As String
    '    Public sLeagueName As String
    '    Public sGroupNumber As Integer
    '    Public sFrontBack As String
    '    Public dDate As Date
    '    Public iHoles As Integer
    '    Public iHoleMarker As Integer
    '    Public iHdcp As Integer
    '    Public sCourse As String
    '    Public sTeam As String
    '    Public sPlayer As String
    '    Public sPlayerToFind As String
    '    Public sFindPlayerOption As String
    '    Public bexit As Boolean = False
    '    Public bloghelper As Boolean = False
    '    Public bReorderCols As Boolean = False
    '    Public bDGSError As Boolean = False
    '    Public bNoRowLeave As Boolean = False
    '    Public bCalcSkins As Boolean = False
    '    Public bScoreCard As Boolean = False
    '    Public bScoresbyPlayer As Boolean = False
    '    Public bDots As Boolean = False
    '    Public bColors As Boolean = False
    '    Public dt As DataTable
    '    Public bScreenChanged As Boolean = False

    '    'fields with (Number) are key fields
    '    'field-width-read only-tabstop-MiddleRight
    '    Public Const cPat40 = "40-false-true-mr"
    '    Public Const cPat40nt = "40-true-false-mr"
    '    Public Const cPathole = "40-false-true-ml"
    '    Public Const cPat60 = "60-false-true-mc"
    '    Public Const cPatMeth = "80-false-true-mc"
    '    Public Const cPat120 = "120-false-true-ml"
    '    Public Const cPat170 = "170-false-false-ml"
    '    Public Const cPat170nt = "170-true-false-ml"
    '    Public Const cBaseScoreCard As String = "Player(1)-cPat170,Method-cPatMeth,Team,Group(3),Holes,Hdcp-cPat40nt"
    '    '20180224-change to read only
    '    'Public Const cSkinsFields As String = "Skins-cPat40,Closest-cPat60,$Earn-cPat40nt,$Skins-cPat40nt,$Closest-cPat40nt,#Skins-cPat40nt,#Closests-cPat60"
    '    Public Const cSkinsFields As String = "Skins-cPat40nt,Closest-cPat60nt,$Earn-cPat40nt,$Skins-cPat40nt,$Closest-cPat40nt,#Skins-cPat40nt,#Closests-cPat40nt"
    '    Public MyCourse() As Data.DataRow
    '    Public bAllHolesEntered = False
    '    Public sArrayOfFiles As New List(Of String)
    '    Public rLeagueParmrow As DataRowView
    '    Public bsch = False
    '    Public bscores = False
    '    Public bplayer = False
    '    Public bcourses = False
    '    Public GGmail As GGSMTP_GMAIL
    '    Public sFileInUseMessage As String
    '    ' Create the ToolTip and associate with the Form container.
    '    Public toolTipHdcp As New ToolTip()
    '    '20180130-num of Closests
    '    Public iNumClosests = 0
    '    Public bByeFound = False
    'End Class
    'Namespace Helper
    'End Namespace

    'this code put in for the extra b player, put an * by his name (2nd instance) if hes in there twice
    'Dim hash As New System.Collections.Hashtable()
    '' Dim sDups As New List(Of String)
    'Dim itemKey As String
    ''save all the players in hash
    'For Each R As DataGridViewRow In dgScoreCard.Rows
    '    itemKey = R.Cells("Player").Value
    '    If Not hash.ContainsKey(itemKey) Then
    '        hash.Add(itemKey, R.Cells("Player").Value)
    '    End If
    'Next
    Sub buildSub()
        '            For Each row As DataRowView In dvPlayers

        '                Dim sKeys() As Object = {row("Name"), sdate}
        '                Dim drow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
        '                If no Then score Is found For this player/Date, check To see If he has a Sub
        '                Dim bsub = False
        '                    If drow Is Nothing Then
        '                        find this players partner in players file
        '                    For Each player As DataRowView In dvPlayers
        '                            find this players team number in players file
        '                        If player("Team") = row("Team") Then
        '                                If the Then player name <> the missing player, we have his partner
        '                            If player("Name") <> row("Name") Then
        '                                        Now find all the scores for that team And eliminate his partner
        '                                Dim dvSubScore As DataView
        '                                        dvSubScore = New DataView(dsLeague.Tables("dtScores")) With
        '                                dvSubScore = New DataView() With
        '                                    {
        '                                        .RowFilter = String.Format("Team = {0} and Player <> '{1}' and Date = '{2}'", row("Team"), player("Name"), sdate)
        '                                    }
        '                                        If dvSubScore.Count > 0 Then
        '                                            save the regulars name for later
        '                                    Dim sSchPlayer = row("Name")
        '                                            save the subs name for later
        '                                    Dim ssub = dvSubScore(0)("Player")
        '                                            Dim bnoshow = False
        '                                            For Each spl As String In sPlayers
        '                                                If spl.Split(",")(0) = dvSubScore(0)("Player") Then
        '                                                    bnoshow = True
        '                                                    Exit For
        '                                                End If
        '                                            Next
        '                                            If bnoshow Then
        '                                                Exit For
        '                                            End If
        '20171015 - account for player subbing And his partner noshows
        '                                    sPlayers.Add(dvSubScore(0)("Player") & "," & iMatch & ip# & "," & row("Name"))
        '                                            bsub = True
        '                                        End If
        '                                        Exit For
        '                                    End If
        '                                End If
        '                    Next
        '                        If bsub Then
        '                            ip += 1
        '                            Continue For
        '                        End If
        '                        this assumes the player Is a no-show And And empty row gets built
        '                    Dim dvscores = New DataView(dsLeague.Tables("dtScores"))
        '                        Dim rowView As DataRowView = dvscores.AddNew
        '                        Change values in the DataRow.
        '                    rowView("League") = sLeagueName
        '                        rowView("Player") = row("Name")
        '                        rowView("Method") = "Score"
        '                        rowView("Group") = 0
        '                        rowView("Team") = row("Team")
        '                        rowView("Hdcp") = row("Handicap")
        '                        rowView("Date") = sdate
        '                        rowView("Skins") = "N"
        '                        rowView("Closest") = "N"
        '                        rowView("Partner") = iMatch & ip#
        '                        rowView.EndEdit()
        '                    End If
        '            Next
        '            Now build a filter for scores
        '            Dim srowfilter = "League = '" & sLeagueName.Replace("'", "''") & "' and Date = '" & sdate & "' and Player in ('"
        '            search the players file And match to the subs array list replacing regulars with subs for the filter
        '            For Each prow As String In sPlayers
        '                srowfilter = srowfilter & prow.Split(",")(0).ToString & "','"
        '            Next
        '            srowfilter = srowfilter & ")"
        '            srowfilter = srowfilter.Replace(",')", ")")
        '            For Each sPlayer In sPlayers
        '                Dim sKeys() As Object = {sPlayer.Split(",")(0), sdate}
        '                Dim drow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
        '                If no Then Score Is found For this player/Date, check To see If he has a Sub
        '                If drow IsNot Nothing Then
        '                    drow("Partner") = sPlayer.Split(",")(1)
        '                End If
        '            Next

    End Sub
    '20180325-changes for bye team
    'Sub getMatchScores_20180409(sdate As String)

    '    If bloghelper Then LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    CalcHoleMarker(sdate)
    '    Dim dtschedule As New DataTable
    '    dtschedule = buildSchedule()
    '    Dim sKey = DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0")
    '    Dim rSch As DataRow = dtschedule.Rows.Find(sKey)
    '    If rSch Is Nothing Then
    '        MsgBox("No scheduled matches found for this date, must exit")
    '        Exit Sub
    '    End If
    '    Dim dvPlayers As DataView
    '    dvPlayers = New DataView(dsLeague.Tables("dtPlayers"))
    '    Dim sPlayersRowFilter = ""
    '    Dim sSD = rLeagueParmrow("StartDate")
    '    Dim sED = rLeagueParmrow("EndDate")
    '    Dim ip# = 0
    '    Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
    '    For iMatch = 1 To rLeagueParmrow("Teams") / 2
    '        Dim sMatch = rSch(iMatch.ToString).ToString
    '        Dim sHt = sMatch.Split("v")(0)
    '        Dim sVt = sMatch.Split("v")(1)
    '        sPlayersRowFilter = String.Format("Team IN ('{0}','{1}') AND ISNULL(DateJoined,'00010101') <= '{2}' AND ISNULL(DateLeft,'99999999') > '{3}'", sHt, sVt, sSD, sED)
    '        dvPlayers = New DataView(dsLeague.Tables("dtPlayers")) With
    '        {
    '         .RowFilter = sPlayersRowFilter, .Sort = "Team, Grade"
    '        }

    '        For Each player As DataRowView In dvPlayers
    '            Dim sKeys() As Object = {player("Name"), sdate}
    '            Dim drow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
    '            'if player is not in the scores for that day, check to see if he has a sub
    '            If drow Is Nothing Then
    '                'find this players partner in filtered players file
    '                For Each partner As DataRowView In dvPlayers
    '                    'find this players team number in players file
    '                    If partner("Team") = player("Team") Then
    '                        'if the player name <> the missing player, we have his partner
    '                        If player("Name") <> partner("Name") Then
    '                            'now find all the scores for that team and eliminate his partner
    '                            Dim dvSubScore As DataView
    '                            dvSubScore = New DataView(dsLeague.Tables("dtScores")) With
    '                            {
    '                                .RowFilter = String.Format("Team = {0} and Player <> '{1}' and Date = '{2}'", player("Team"), player("Name"), sdate)
    '                            }
    '                            'add the missing player as an empty score
    '                            If dvSubScore.Count <> 2 Then
    '                                Dim rowView As DataRowView = dvscores.AddNew
    '                                ' Change values in the DataRow.
    '                                rowView("League") = sLeagueName
    '                                rowView("Player") = player("Name")
    '                                'rowView("Method") = "Score"
    '                                rowView("Group") = 0
    '                                rowView("Team") = player("Team")
    '                                'rowView("Hdcp") = row("Handicap")
    '                                rowView("Date") = sdate
    '                                'rowView("Skins") = "N"
    '                                'rowView("Closest") = "N"
    '                                rowView("Partner") = ip#
    '                                rowView.EndEdit()
    '                            End If
    '                            Exit For
    '                        End If
    '                    End If
    '                Next
    '            Else
    '                drow("Partner") = CStr(ip#).PadLeft(2, "0")
    '                drow.EndEdit()
    '            End If
    '            ip# += 1
    '        Next
    '    Next

    '    dvscores.Sort = "Partner"

    'End Sub
    Sub MakeCellsStringsDV(row As DataRowView)
        'this forces each cell to be string prevent errors on resorting columns
        Dim x = ""
        For Each cell As String In row.Row.ItemArray
            If cell Is DBNull.Value Then
                cell = ""
            End If

            '    'Debug.Print(cell.OwningColumn.Name & "-" & cell.Value & "-" & cell.FormattedValueType.Name)
        Next
    End Sub

    Public Function getHolidayList(ByVal vYear As Integer) As List(Of Date)

        Dim FirstWeek As Integer = 1
        Dim SecondWeek As Integer = 2
        Dim ThirdWeek As Integer = 3
        Dim FourthWeek As Integer = 4
        Dim LastWeek As Integer = 5

        Dim HolidayList As New List(Of Date)

        '   http://www.usa.gov/citizens/holidays.shtml      
        '   http://archive.opm.gov/operating_status_schedules/fedhol/2013.asp

        ' New Year's Day            Jan 1
        HolidayList.Add(DateSerial(vYear, 1, 1))

        ' Martin Luther King, Jr. third Mon in Jan
        HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 1, 1), DayOfWeek.Monday, ThirdWeek))

        ' Washington's Birthday third Mon in Feb
        HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 2, 1), DayOfWeek.Monday, ThirdWeek))

        ' Memorial Day          last Mon in May
        HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 5, 1), DayOfWeek.Monday, LastWeek))

        ' Independence Day      July 4
        HolidayList.Add(DateSerial(vYear, 7, 4))

        ' Labor Day             first Mon in Sept
        HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 9, 1), DayOfWeek.Monday, FirstWeek))

        ' Columbus Day          second Mon in Oct
        HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 10, 1), DayOfWeek.Monday, SecondWeek))

        ' Veterans Day          Nov 11
        HolidayList.Add(DateSerial(vYear, 11, 11))

        ' Thanksgiving Day      fourth Thur in Nov
        HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 11, 1), DayOfWeek.Thursday, FourthWeek))

        ' Christmas Day         Dec 25
        HolidayList.Add(DateSerial(vYear, 12, 25))

        'saturday holidays are moved to Fri; Sun to Mon
        For i As Integer = 0 To HolidayList.Count - 1
            Dim dt As Date = HolidayList(i)
            If dt.DayOfWeek = DayOfWeek.Saturday Then
                HolidayList(i) = dt.AddDays(-1)
            End If
            If dt.DayOfWeek = DayOfWeek.Sunday Then
                HolidayList(i) = dt.AddDays(1)
            End If
        Next

        'return
        Return HolidayList

    End Function

    Private Function GetNthDayOfNthWeek(ByVal dt As Date, ByVal DayofWeek As Integer, ByVal WhichWeek As Integer) As Date
        'specify which day of which week of a month and this function will get the date
        'this function uses the month and year of the date provided

        'get first day of the given date
        Dim dtFirst As Date = DateSerial(dt.Year, dt.Month, 1)

        'get first DayOfWeek of the month
        Dim dtRet As Date = dtFirst.AddDays(6 - dtFirst.AddDays(-(DayofWeek + 1)).DayOfWeek)

        'get which week
        dtRet = dtRet.AddDays((WhichWeek - 1) * 7)

        'if day is past end of month then adjust backwards a week
        If dtRet >= dtFirst.AddMonths(1) Then
            dtRet = dtRet.AddDays(-7)
        End If

        'return
        Return dtRet

    End Function


    Public Function CreateLabel(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String)
        Return CreateLabel(ctrl, top, left, width, name, text, "REG", "H")
    End Function
    Public Function CreateLabel(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String, dir As String)
        Return CreateLabel(ctrl, top, left, width, name, text, "REG", "H")
    End Function
    Public Function CreateLabel(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String, font As String, dir As String)
        Dim B As New Label
        ctrl.Add(B)
        B.Top = top
        B.Height = 21
        B.Width = width
        B.Left = left + 15
        B.TabStop = False
        If font = "B" Then
            'B.Font = New System.Drawing.Font("Arial", 8)
            B.Font = New System.Drawing.Font(B.Font, FontStyle.Bold)
        ElseIf font = "BU" Then
            ' B.Font = New System.Drawing.Font("Arial", 6.5)
            B.Font = New System.Drawing.Font(B.Font, FontStyle.Bold Or FontStyle.Underline)
        Else
            B.Font = New System.Drawing.Font(B.Font, FontStyle.Regular)
        End If
        B.Name = name
        B.Text = text
        If dir = "V" Then
            Return top
        Else
            Return left + B.Width + 5
        End If

    End Function
    Public Function CreateTextbox(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String, dir As String)
        Dim B As New MaskedTextBox
        ctrl.Add(B)
        B.Top = top
        B.Height = 21
        B.Width = width
        B.Left = left
        B.Name = name
        B.Text = text
        B.TabStop = True
        If dir = "V" Then
            Return top + 20
        Else
            Return left + B.Width + 5
        End If
    End Function

    Public Function CreateNumericTextbox(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String)
        Dim B As New MaskedTextBox
        ctrl.Add(B)
        B.Top = top
        B.Height = 21
        B.Width = width
        B.Left = left
        B.Name = name
        B.Text = text
        B.Mask = "99"
        B.TabStop = True
        If name.Contains("Par") Then
            B.Mask = "9"
        End If

        B.ValidatingType = GetType(System.Int16)
        Return left + B.Width + 5
    End Function

    Sub DisplayScores(sPlayer As String, frm As Form, lv1 As SI.Controls.LvSort)
        With oHelper
            .LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
            Try
                Dim lvRec As ListViewItem = Nothing

                With lv1
                    .Visible = True
                    fromsizeW = frm.Size.Width
                    lvsizeW = .Size.Width
                    .ListViewItemSorter = Nothing
                    .View = View.Details
                    .Visible = True
                    .Items.Clear()
                    .Sorting = SortOrder.None
                    .Columns.Clear()
                    .Columns.Add("Date", 100, HorizontalAlignment.Left)
                    'lvFiles.Columns.Add("File Type", 80, HorizontalAlignment.Left) 
                    For i = 1 To 18
                        .Columns.Add(i.ToString, 30, HorizontalAlignment.Left)
                        If i = 9 Then
                            .Columns.Add("Out", 40, HorizontalAlignment.Left)
                        End If
                    Next

                    .Columns.Add("In", 40, HorizontalAlignment.Left)
                    .Columns.Add("Tot", 40, HorizontalAlignment.Left)
                    .Columns.Add("Hdcp", 50, HorizontalAlignment.Left)
                    .Columns.Add("Net", 40, HorizontalAlignment.Left)
                End With

                Dim dvScores = New DataView(.dsLeague.Tables("dtScores")), sFirstField = "Date"

                ' dvScores.RowFilter = "Player = '" & sPlayer & "'" & " and Course = '" & dvCourse.Item(0).Row.Item(0) & "' and Hole1 > 0"
                dvScores.RowFilter = String.Format("Player = '{0}'", sPlayer) 'and Hole1 > 0"
                dvScores.Sort = "Date"
                'testing 
                'dvScores.RowFilter = "Date > 20130609 and Name = '" & cbPlayer.SelectedItem.ToString & "'"
                Dim iStats As New List(Of Integer)

                'create 5 stats
                For i = 0 To 5
                    iStats.Add(0)
                Next
                'create a bucket for each hole
                Dim iScores As New List(Of Decimal)
                For i = 0 To 17
                    iScores.Add(0)
                Next
                Dim iFrontScores As Integer = 0, iBackScores As Integer = 0

                .iHoles = .rLeagueParmrow("Holes")
                Dim iTotScore As Decimal, iInScore As Decimal, iOutScore As Decimal

                For Each row As DataRowView In dvScores
                    '20180123 - show scores with scoring method "Score'
                    'If row("Method") = "Score" Then Continue For
                    Console.WriteLine(row("Date"))
                    'figure out the front/back nine score
                    .CalcHoleMarker(row("Date"))
                    Dim sMethod As String = row("Method").Value
                    '20181004 - override calcholemarker because of cc and league in same sch
                    If sMethod = "Gross" Or sMethod = "Net" Then
                        If row("Hole1") IsNot DBNull.Value Then
                            .iHoleMarker = 1
                        Else
                            .iHoleMarker = 10
                        End If
                    End If

                    Dim ohelperlv As New Helper_LV
                    ohelperlv.ListScores(lv1, row, sFirstField, True, 0, .iHoleMarker)
                    If row("Method") <> "Score" Then
                        For ihole = 1 To .iHoles
                            Dim ihptr = ihole + (.iHoleMarker - 1)
                            Dim iadjscore = row("Hole" & ihptr)
                            If row("Date") = "20170919" Then
                                If ihptr = 17 Then
                                    Dim xxx = ""
                                End If
                            End If
                            If row("Method") = "Net" Then
                                Dim ish As Integer = .CalcStrokeIndex(ihptr)
                                If row("Phdcp") >= ish Then
                                    iadjscore += 1
                                    If row("Phdcp") - 9 >= ish Then iadjscore += 1
                                End If
                                iScores(ihptr - 1) += iadjscore
                            Else
                                iScores(ihptr - 1) += iadjscore
                            End If
                            Dim icnt = 2
                            Dim spar = .MyCourse(0)("Hole" & ihptr)
                            For i = 0 To iStats.Count - 1
                                If iadjscore <= spar - icnt Then
                                    iStats(i) += 1
                                    Exit For
                                End If
                                icnt -= 1
                                If i = iStats.Count - 1 Then
                                    iStats(i) += 1
                                End If
                            Next
                        Next
                        If .iHoleMarker = 1 Then
                            iFrontScores += 1
                        Else
                            iBackScores += 1
                        End If
                    End If

                    If row("Out_Gross") <> "" Then iOutScore += row("Out_Gross")
                    If row("In_Gross") <> "" Then iInScore += row("In_Gross")

                    If .iHoles > 9 Then
                        iTotScore += row("18_Gross")
                    End If
                Next

                Dim ilcols = lv1.Columns.Count

                lvRec = New ListViewItem("--------------------")
                For i = 0 To lv1.Columns.Count - 1
                    lvRec.SubItems.Add("---------------------------".Substring(0, lv1.Columns(i).Width / 10))
                Next
                lv1.Items.Add(lvRec)

                lvRec = New ListViewItem("Total")
                For i = 1 To 18
                    'Dim spar = dvCourse.Item(0).Row(i + 2)
                    If i < 10 Then
                        iScores(i - 1) = (iScores(i - 1) / iFrontScores)
                    Else
                        iScores(i - 1) = (iScores(i - 1) / iBackScores)
                    End If
                    lvRec.SubItems.Add(iScores(i - 1).ToString("#.0"))
                    If i = 9 Then
                        lvRec.SubItems.Add((iOutScore / iFrontScores).ToString("##.0"))
                    ElseIf i = 18 Then
                        lvRec.SubItems.Add((iInScore / iBackScores).ToString("##.0"))
                        lvRec.SubItems.Add((iTotScore / dvScores.Count).ToString("###.0"))
                    End If
                Next

                lv1.Items.Add(lvRec)
                Dim sStatsDesc As String() = {"Eagles", "Birdies", "Pars", "Bogeys", "Double Bogeys", "Others"}

                For i = 0 To iStats.Count - 1
                    lvRec = New ListViewItem(sStatsDesc(i))
                    lvRec.SubItems.Add(iStats(i))
                    lv1.Items.Add(lvRec)
                Next
                'CalcLowScore(lv1, Color.OrangeRed, False)
                frm.Controls.Add(lv1)

                'lv1.Top = 200
                'lv1.Left = 35
                'lv1.Width = 1100
                'lv1.Height = 350
                'lv1.FullRowSelect = True
                'lv1.GridLines = True
                'lv1.MultiSelect = False
                'lv1.Name = "lv1"
                'lv1.LabelEdit = True
                'AddHandler lv1.SelectedIndexChanged, AddressOf lv1_SelectedIndexChanged
                'AddHandler lv1.MouseDoubleClick, AddressOf lv1_MouseDoubleClick
                'CalcHdcp(lv1, edt, dvScores, dvCourse, "Name|Date")

            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End With
    End Sub


    'Public Function dtBuildScores() As DataTable
    '    dtBuildScores = New DataTable
    '    Dim column As DataColumn

    '    column = New DataColumn()
    '    column.DataType = System.Type.GetType("System.String")
    '    column.ColumnName = "Player"
    '    ' Add the column to the DataTable.Columns collection.
    '    dtScores.Columns.Add(column)

    '    column = New DataColumn()
    '    'column.DataType = System.Type.GetType("System.Date")
    '    column.DataType = System.Type.GetType("System.String")
    '    column.ColumnName = "Date"
    '    dtScores.Columns.Add(column)

    '    dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}

    '    dtScores.Columns.Add("Group")

    '    For i As Integer = 1 To iHoles
    '        dtScores.Columns.Add(i)
    '    Next
    '    If iHoles > 9 Then
    '        dtScores.Columns.Add("Out")
    '        dtScores.Columns.Add("In")
    '    End If
    '    dtScores.Columns.Add("Total")
    '    dtScores.Columns.Add("Hdcp")

    '    For i As Integer = 1 To iHoles
    '        dtScores.Columns.Add(i)
    '    Next

    'End Function
    Public Sub BuildControls(gb As GroupBox, top As Integer, left As Integer, holes As Integer, holetitle As String)
        Try
            Dim ileft = left
            Dim itop = top
            Dim iwidth = 35
            'If holes < 18 Then
            '    gb.Width = gb.Width / 2
            'End If
            For i = 0 To holes - 1
                ileft = CreateLabel(gb.Controls, itop, ileft, iwidth, holetitle & i + 1, i + 1, "B", "")
                If i = 8 Then
                    'ileft = CreateLabel(gb.Controls, itop, ileft, 40, "Out", "", "B", "")
                    ileft = CreateLabel(gb.Controls, itop, ileft, iwidth, "OutScore", String.Empty, "B", "")
                ElseIf i = 17 Then
                    ileft = CreateLabel(gb.Controls, itop, ileft, 40, "In", "", "B", "")
                    CreateLabel(gb.Controls, itop + iwidth, ileft - 40, iwidth, "InScore", String.Empty)
                    ileft = CreateLabel(gb.Controls, itop, ileft, 40, "lbTotal", "", "B", "")
                    CreateLabel(gb.Controls, itop + iwidth, ileft - 40, iwidth, "TotScore", String.Empty)
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Public Sub CreateComboBoxField(ByVal dgv As DataGridView, ByVal sField As String, ByVal imax As Integer)

        Dim index As Integer
        ' find the location of the column
        index = dgv.Columns.IndexOf(dgv.Columns(sField))
        ' remove the existing column
        dgv.Columns.RemoveAt(index)
        ' create a new custom column
        Dim dgvCombox As New DataGridViewComboBoxColumn
        dgvCombox.ValueType = GetType(String)
        dgvCombox.Name = sField
        dgvCombox.DataPropertyName = sField
        ' some more tweaking
        dgvCombox.SortMode = DataGridViewColumnSortMode.Automatic

        For i = 0 To imax
            dgvCombox.Items.Add(i.ToString)
        Next
        ' insert the new column at the same location
        dgv.Columns.Insert(index, dgvCombox)
        dgv.Visible = True

    End Sub
    Public Sub CreateMaskField(ByVal dgv As DataGridView, sField As String)

        Dim index As Integer
        ' find the location of the column
        index = dgv.Columns.IndexOf(dgv.Columns(sField))
        ' remove the existing column
        dgv.Columns.RemoveAt(index)
        ' create a new custom column
        Dim dgvMaskedEdit As New DataGridViewMaskedEditColumn

        If sField = "Phone" Then
            dgvMaskedEdit.Mask = "###-###-####"
            dgvMaskedEdit.ValidatingType = GetType(String)
        ElseIf sField.StartsWith("Hole") Then
            dgvMaskedEdit.Mask = "#"      ' this mask will allow only numbers
            dgvMaskedEdit.ValidatingType = GetType(String)
            dgvMaskedEdit.Width = 40
        Else
            dgvMaskedEdit.Mask = "##"      ' this mask will allow only numbers
            dgvMaskedEdit.ValidatingType = GetType(String)
        End If
        dgvMaskedEdit.Name = sField
        dgvMaskedEdit.DataPropertyName = sField
        ' some more tweaking
        dgvMaskedEdit.SortMode = DataGridViewColumnSortMode.Automatic

        ' insert the new column at the same location
        dgv.Columns.Insert(index, dgvMaskedEdit)
        dgv.Visible = True

    End Sub

    Public Sub CreateRowfromLine(sAry As String(), edt As DataTable)
        Dim aRow As DataRow
        aRow = edt.NewRow
        ' now create an empty datarow
        aRow = edt.NewRow
        'i is the index into the string array
        Dim i = 0
        'loop thru each column in the data table determining if numeric values are present for numeric fields
        For Each col As DataColumn In edt.Columns
            If i > UBound(sAry) Then
                aRow(col) = ""
            Else
                Dim scolName = col.ColumnName
                Select Case col.DataType
                    Case GetType(Int32)
                        If IsNumeric(sAry(i)) Then
                            aRow(col) = sAry(i)
                        Else
                            aRow(col) = 0
                        End If
                    Case Else
                        aRow(col) = sAry(i)
                End Select
                i += 1
            End If
        Next
        'add the full row to the table
        edt.Rows.Add(aRow)
    End Sub
    Sub CreateLabel(controlCollection As Control.ControlCollection, itop As Object, p3 As Integer, p4 As Integer, p5 As String, p6 As Char)
        Throw New NotImplementedException
    End Sub
    'Sub InOutScore(sInOut As String, row As DataGridViewRow, arow As DataRow)
    '    'arow(sInOut) = row.Cells(sInOut).Value
    '    arow(sInOut & "_Gross") = row.Cells(sInOut & "_Gross").Value
    '    arow(sInOut & "_Net") = row.Cells(sInOut & "_Net").Value

    'End Sub
    Public Sub soMarkSubPar(cell As DataGridViewCell, iscore As Integer, iPar As Integer)
        With oHelper
            'cell.Style.Font = New Font("Arial", 19, FontStyle.Regular)
            cell.Style.ForeColor = Color.Black
            cell.Style.BackColor = Color.White
            cell.Value = .RemoveSpcChar(cell.Value)
            Dim sFont = "Tahoma"
            Dim iFontSize = 12
            Dim bFontStrikeout = False

            If cell.Style.Font IsNot Nothing Then
                If cell.Style.Font.Strikeout = True Then bFontStrikeout = True
            End If

            'cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
            If .IHdcp = 99 Then Exit Sub
            'check scores against handicap
            'is this a stroke hole?
            If .iHoles = 0 Then
                .iHoles = .rLeagueParmrow("Holes")
            End If

            'check stroke index
            Dim isi = ""
            isi = .CalcStrokeIndex(cell.OwningColumn.Name)
            'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
            'if the handicap > stroke index make color beige
            If .IHdcp >= isi Then
                If .bColors Then cell.Style.BackColor = Color.Beige
                If .bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                'if double stroke hole, make color b/a
                If .IHdcp - .iHoles >= isi Then
                    If .bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                    If .bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                End If
            End If

            If .bColors Then
                If iscore < iPar Then
                    iFontSize += 3
                    If bFontStrikeout Then
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout Or FontStyle.Bold)
                    Else
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                    End If
                    cell.Style.ForeColor = Color.OrangeRed
                    'if this is a total score (> 10) of some sort, dont check for birdies, eagles
                    If iscore > 10 Then Exit Sub
                    'birdie
                    If iscore < iPar - 1 Then
                        'cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
                        cell.Style.ForeColor = Color.DarkRed
                    End If
                    'eagle
                    If iscore < iPar - 2 Then
                        iFontSize += 3
                        If bFontStrikeout Then
                            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout Or FontStyle.Bold)
                        Else
                            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                        End If
                    End If
                    'over par is black,even is gray
                ElseIf iscore > iPar Then
                    If bFontStrikeout Then
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
                    Else
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout Or FontStyle.Regular)
                    End If
                    cell.Style.ForeColor = Color.Blue
                End If
            End If
        End With
    End Sub
    Sub SetDefaultCellStyles(dgscores As DataGridView)

        dgscores.DefaultCellStyle.BackColor = Color.White
        dgscores.DefaultCellStyle.ForeColor = Color.Black
        dgscores.DefaultCellStyle.Font = New Font("Tahoma", 12)

        Dim highlightCellStyle = New DataGridViewCellStyle()
        highlightCellStyle.BackColor = Color.Red

        Dim currencyCellStyle = New DataGridViewCellStyle()
        currencyCellStyle.Format = "C"
        currencyCellStyle.ForeColor = Color.Green

        'sample to set select cells
        'dgscores.Rows(3).DefaultCellStyle = highlightCellStyle
        'dgscores.Rows(8).DefaultCellStyle = highlightCellStyle
        'dgscores.Columns("UnitPrice").DefaultCellStyle = currencyCellStyle
        'dgscores.Columns("TotalPrice").DefaultCellStyle = currencyCellStyle

    End Sub
    Function GetInt(o As Object) As Integer
        If IsDBNull(o) Then Return 0 Else Return CInt(o)
    End Function
    'Public Function fCreateScoreCardDT() As DataTable
    '    fCreateScoreCardDT = Nothing
    '    fCreateScoreCardDT = New DataTable
    '    Dim sNewScoreCard As String = "League,Method,Team,Player(1),Date(2),Group(3),Hole1,Hole2,Hole3,Hole4,Hole5,Hole6,Hole7,Hole8,Hole9,Out,Out_Gross,Out_Net,Hole10,Hole11,Hole12,Hole13,Hole14,Hole15,Hole16,Hole17,Hole18,In,In_Gross,In_Net,18_Gross,18_Net,Hdcp,PHdcp,Skins,Closest,$Earn,$Skins,$Closest,Partner,Scorecard"
    '    Try
    '        Dim sArray As String() = sNewScoreCard.Split(",")
    '        For Each parm As String In sArray
    '            If parm.Contains("(") Then
    '                parm = parm.Substring(0, parm.IndexOf("("))
    '            End If

    '            Dim column As DataColumn
    '            column = New DataColumn()
    '            column.ColumnName = parm
    '            fCreateScoreCardDT.Columns.Add(column)
    '        Next

    '        Dim PrimarykeyColumn(0) As DataColumn
    '        Dim ikey = 0
    '        For Each parm As String In sArray
    '            If parm.Contains("(") Then
    '                ReDim Preserve PrimarykeyColumn(ikey)
    '                PrimarykeyColumn(ikey) = fCreateScoreCardDT.Columns(parm.Substring(0, parm.IndexOf("(")))
    '                ikey += 1
    '            End If
    '        Next
    '        fCreateScoreCardDT.PrimaryKey = PrimarykeyColumn
    '        'CreateRowfromLine(sLeagueParmValues.Split(vbTab), fCreateScoreCardDT)
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    End Try
    'End Function
    Public Function BuildScoreCardMethodsCB() As DataGridViewComboBoxCell
        Dim gridComboBox As New DataGridViewComboBoxCell
        gridComboBox.Items.Add("Net") 'Populate the Combobox
        gridComboBox.Items.Add("Gross") 'Populate the Combobox
        gridComboBox.Items.Add("Score") 'Populate the Combobox

        BuildScoreCardMethodsCB = gridComboBox
    End Function

    'The trick is to declare the new Random class outside of the function that retrieves the next random number. 
    'This way you generate the seed only once and are getting the “randomizer” formula to cycle through its formula and ensure the next chosen number is truly random.
    'Here’s my code. 
    'Note that you no longer have to declare new objects (such as objRandom, here) at the top of your class or module; 
    'you can do it just above the function, to aid clarity of code:
    Dim objRandom As New System.Random(
CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))

    Public Function GetRandomNumber(
    Optional ByVal Low As Integer = 1,
    Optional ByVal High As Integer = 100) As Integer
        ' Returns a random number,
        ' between the optional Low and High parameters
        Return objRandom.Next(Low, High + 1)
    End Function
    'from scorecard

    Sub editnumCTP(R As DataGridViewRow, sCurrColName As String)

        Dim iCTP = 0
        For Each row As DataGridViewRow In frmScoreCard.dgScores.Rows
            Dim cell As DataGridViewCheckBoxCell = row.Cells("#Closests")
            If cell.Value = True Then
                iCTP += 1
            End If
        Next

        If iCTP > oHelper.iNumClosests Then
            MsgBox(String.Format("There are only {0} closests to pins, cant have more, try again", oHelper.iNumClosests))
            oHelper.bDGSError = True
            R.Cells(sCurrColName).Value = frmScoreCard.sOldCellValue
        End If
    End Sub

#Region "OldCode"
    'Private Sub dgScores_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellClick
    '    Dim x = ""

    '    'Dim rowsCheckedList As List(Of DataGridViewRow) = GetCheckedRows1(dgScores, "Skins")
    '    'Dim xx = ""
    'End Sub

    'Sub gvGraphics(poDG As DataGridView)
    '    ' Code for printing DataGridView
    '    Dim leftMargin As Integer = 30
    '    Dim position As Integer = leftMargin
    '    Dim yPosition As Integer = 329  ' Change value to change the y-position of the DataGridView
    '    Dim height As Integer = poDG.ColumnHeadersHeight / 2

    '    position = leftMargin
    '    yPosition = yPosition + poDG.ColumnHeadersHeight / 2
    '    For Each dr As DataGridViewColumn In poDG.Columns
    '        Dim totalWidth As Double = dr.Width
    '        e.Graphics.FillRectangle(New SolidBrush(Color.White), New Rectangle(position, yPosition, totalWidth, height))
    '        e.Graphics.DrawRectangle(Pens.Black, New Rectangle(position, yPosition, totalWidth, height))
    '        e.Graphics.DrawString(dr.HeaderText, New Font("Times New Roman", 10, FontStyle.Bold Or FontStyle.Italic), Brushes.Black, position, yPosition, fmt)
    '        position = position + totalWidth
    '    Next

    '    For Each dr As DataGridViewRow In poDG.Rows
    '        position = leftMargin
    '        yPosition = yPosition + poDG.ColumnHeadersHeight / 2
    '        For Each dc As DataGridViewCell In dr.Cells
    '            Dim totalWidth As Double = dc.OwningColumn.Width
    '            e.Graphics.FillRectangle(New SolidBrush(Color.White), New Rectangle(position, yPosition, totalWidth, height))
    '            e.Graphics.DrawRectangle(Pens.White, New Rectangle(position, yPosition, dc.OwningColumn.Width, height))
    '            e.Graphics.DrawString(dc.Value, New Font("Verdana", 8, FontStyle.Regular), Brushes.Black, position, yPosition)
    '            position = position + totalWidth
    '        Next
    '    Next
    'End Sub
    ' Assumes you have created a ToolTip object named ToolTip1
    ' This event fired when the tooltip needs to draw its surface.

    'Private Sub frmScoreCard_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
    'width
    'If gvSsizeW <> 0 Then
    '    dgScores.Width = gvSsizeW + (Me.Size.Width - fromsizeW)
    'End If
    'If gvSCsizeW <> 0 Then
    '    dgScoreCard.Width = gvSCsizeW + (Me.Size.Width - fromsizeW)
    'End If
    'If gbSCsizeW <> 0 Then
    '    gbScoring.Width = gbSCsizeW + (Me.Size.Width - fromsizeW)
    'End If
    'height-not working
    'If gvSsizeH <> 0 Then
    '    dgScores.Height = gvSsizeH + (Me.Size.Height - fromsizeH)
    'End If
    'If gvSCsizeH <> 0 Then
    '    dgScoreCard.Height = gvSCsizeH + (Me.Size.Height - fromsizeH)
    'End If
    'If gbSCsizeH <> 0 Then
    '    gbScoring.Height = gbSCsizeH + (Me.Size.Height - fromsizeH)
    'End If
    'End Sub
#End Region

End Class
