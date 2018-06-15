'*************************************************************************************************
Imports System.IO.Packaging
Public Class Main
    Dim cVersion = "Version : 6.15"
    Public oHelper As Helper.Controls.Helper
    Dim bLoad = True
    'Private Teams As ArrayList = New ArrayList
    Private Teams2 As ArrayList = New ArrayList
    Dim rs As New Resizer

    Private Sub Main_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Genschedule()
        lbVersion.Text = cVersion
        lblProcessMsg.Text = "Loading League Tables..."

        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()

        '20180106 remove for now
        btnShowScores.Visible = False

        rs.FindAllControls(Me)
        'Me.Size = My.Computer.Screen.WorkingArea.Size
        'Me.WindowState = FormWindowState.Maximized
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.BackColor = Color.Transparent
        oHelper = New Helper.Controls.Helper
        Dim sDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        '12/19/2017 note this is removed for now 
        Me.Icon = New Icon(Me.Icon, New Size(Me.Icon.Width * 5, Me.Icon.Height * 5))

        'load cookie settings
        If IO.File.Exists(sDocs & "\Leaguemanager.ini") Then
            Using sr As New IO.StreamReader(sDocs & "\Leaguemanager.ini", False)
                Do
                    Dim sline = sr.ReadLine
                    If sline = Nothing Then
                        Exit Do
                    End If
                    Dim slineparts = sline.Split("=")
                    Select Case slineparts(0)
                        Case "LeagueName"
                            oHelper.sLeagueName = slineparts(1)
                        Case "GroupNumber"
                            oHelper.sGroupNumber = slineparts(1)
                        Case "Date"
                            oHelper.dDate = Date.ParseExact(slineparts(1), "MM-dd-yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                        Case "FilePath"
                            oHelper.sFilePath = slineparts(1)
                    End Select
                Loop
            End Using
        End If

        'oHelper.sFilePath = "" '"\\wdmycloud\Gary\LeagueManager\Files"
        If oHelper.sFilePath = "" Then
            Dim dialog As New FolderBrowserDialog With
            {
            .RootFolder = Environment.SpecialFolder.Desktop,
            .SelectedPath = "e:\LeagueManager\Files",
            .Description = "Select League Files Path"
            }
            If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                oHelper.sFilePath = dialog.SelectedPath
            Else
                End
            End If
        End If

        'save ini file if league present
        If oHelper.sLeagueName <> "" Then oHelper.UpdateINI()

        txtFolder.Text = oHelper.sFilePath

        'build dropdown list of leagues
        oHelper.dsLeague = New dsLeague
        If Not oHelper.CSV2DataTable(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv")) Then
            MsgBox(String.Format("File in use, close file and restart {0} {1}", vbCrLf, oHelper.getLatestFile("*LeagueParms.csv")))
            End
        End If
        'oHelper.dsLeague.ReadXml(oHelper.getXMLFile("*LeagueParms.xml"))
        Dim dvLeagues = New DataView(oHelper.dsLeague.Tables("dtLeagueParms"))
        dvLeagues.Sort = "Name Asc, StartDate Desc"

        If dvLeagues.Count = 0 Then
            Dim mbr = MsgBox("Leageue parm file not found, want to create one?", MsgBoxStyle.YesNo)
            If mbr = MsgBoxResult.Yes Then
                frmLeagueSetup.ShowDialog()
                End
            End If
        End If

        For Each row In dvLeagues
            cbLeagues.Items.Add(row("Name") & " (" & row("StartDate").year & ")")
        Next

        cbLeagues.SelectedIndex = 0

        bLoad = False
        GetLeague()
        'setup email function
        oHelper.GGmail = New GGSMTP_GMAIL(oHelper.rLeagueParmrow("Email"), oHelper.rLeagueParmrow("EmailPassword"))
        'GetXSDNameByFileName(oHelper.dsLeague.Tables("dtScores"),
        oHelper.MyCourse = oHelper.dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")
        lblProcessMsg.Text = "Done-Loading League Tables"
        Me.Cursor = Cursors.Default
        Application.DoEvents()

    End Sub
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
                J = oHelper.GetRandomNumber(0, iTotHTeams - 1)
                sHteams.Add(AL(J) & " ")
                AL.RemoveAt(J)
            Next
            'create a random extra b player if uneven(Bye)
            If iTotHTeams <> iTotATeams Then
                J = oHelper.GetRandomNumber(1, AL.Count)
                sAteams.Add(AL(J) & " ")
                iextra = AL(J)
            End If

            'make B players random
            For i = 0 To AL.Count - 1
                J = oHelper.GetRandomNumber(0, AL.Count - 1)
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
    Sub BuildTablesForLeague()
        'this subroutine will load the files related to the league/year that resides in the dropdown selected item 
        Dim sLeagueName = cbLeagues.SelectedItem

        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(oHelper.sFilePath)
        'oFiles = oDirectory.GetFiles("*.xml")
        oFiles = oDirectory.GetFiles("*.csv")
        'sort by date desc
        Helper.Controls.Helper.arraySort(oFiles)
        'oHelper.arraySort(oFiles)
        Dim sArrayOfFiles As New List(Of String)

        'build an array list of the most recent files for each table
        For Each sfile In oFiles
            'league parm file aready read into datatable above
            If sfile.FullName.Contains("LeagueParm") Or sfile.FullName.Contains("Standings") Then Continue For
            If sfile.FullName.Contains("Schedule") Then
                Dim sfileparts = sfile.FullName.ToString.Split("_")
                'match year
                Dim sYear = sfileparts(0).Substring(sfileparts(0).LastIndexOf("\") + 1, 4)
                'match league name
                If sLeagueName.contains(sYear) And sLeagueName.contains(sfileparts(1)) Then
                    oHelper.bsch = True
                Else
                    Continue For
                End If
            ElseIf sfile.FullName.Contains("Courses") Then
                oHelper.bcourses = True
            ElseIf sfile.FullName.Contains("Players") Then
                oHelper.bplayer = True
            ElseIf sfile.FullName.Contains("Scores") Then
                oHelper.bscores = True
            End If

            Dim stbl = oHelper.getSubstring(sfile.Name, "_", ".")
            'load first entry
            If sArrayOfFiles.Count = 0 Then
                sArrayOfFiles.Add(sfile.FullName)
                Continue For
            End If
            Dim bnoupd = False
            For Each saf In sArrayOfFiles
                If saf.Contains(stbl) Then
                    bnoupd = True
                    Exit For
                End If
            Next
            If Not bnoupd Then sArrayOfFiles.Add(sfile.FullName)
        Next

        'save this in helper so other subroutines can use
        oHelper.sArrayOfFiles = sArrayOfFiles
        For Each saFile In sArrayOfFiles
            Dim sfile = "dt" & oHelper.getSubstring(saFile, "_", ".")
            sfile = sfile.Replace(oHelper.getSubstring(cbLeagues.SelectedItem, 0, " ") & "_", "")
            If Not oHelper.dsLeague.Tables.Contains(sfile) Then oHelper.dsLeague.Tables.Add(sfile)
            If Not oHelper.CSV2DataTable(oHelper.dsLeague.Tables(sfile), saFile) Then
                MsgBox(String.Format("File in use - {1} {0} close file and restart", vbCrLf, saFile))
                End
            End If
            If sfile = "dtScores" Then lbScoresFile.Text = saFile

            'this is used for xml 
            'If oHelper.dsLeague.Tables.Contains(sfile) Then
            '    oHelper.dsLeague.Tables(sfile).ReadXml(saFile)
            'Else
            '    oHelper.dsLeague.Tables.Add(sfile).ReadXml(saFile)
            'End If
            ''Dim sfilename = "\" & oHelper.dsLeague.Tables(sfile).TableName.Substring(2) & ".csv"
            'oHelper.DataTable2CSV(oHelper.dsLeague.Tables(sfile), oFile.FullName.Replace(".xml", ".csv"))
        Next

        If oHelper.dsLeague.Tables.Contains("dtScores") Then
            oHelper.dsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtScores").Columns("Player"), oHelper.dsLeague.Tables("dtScores").Columns("Date")}
        End If
        'Dim x = oHelper.dsLeague.Tables("dtPlayers").Select name from Table A Group By name having count(*) > 1"

        If oHelper.dsLeague.Tables.Contains("dtPlayers") Then
            oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}
        End If

    End Sub
    Sub GetLeague()
        'oHelper.dsLeague.Tables.RemoveAt(0)
        'since xml files are named yyyymmdd_***.csv. we will have multiple files for each data table so we want to get the most recent one
        oHelper.dsLeague = New dsLeague
        'oHelper.dsLeague.ReadXml(oHelper.getLatestFile("*LeagueParms.csv"))
        oHelper.CSV2DataTable(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv"))
        Dim dvLeagues = New DataView(oHelper.dsLeague.Tables("dtLeagueParms"))
        dvLeagues.Sort = "Name Asc, StartDate Desc"

        For Each row In dvLeagues
            Dim wkdate As Date = row("startDate")
            Dim reformatted As String = wkdate.ToString("mm/dd/yyyy", Globalization.CultureInfo.InvariantCulture)
            Dim sLeagueName = row("Name") & " (" & row("Startdate").year & ")"
            If sLeagueName = cbLeagues.SelectedItem Then
                'save row of league so all routines can use
                oHelper.rLeagueParmrow = row
                oHelper.sLeagueName = oHelper.rLeagueParmrow("Name")
                BuildTablesForLeague()
            End If
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
    Private Sub LeagueSetup_Click(sender As Object, e As EventArgs) Handles btnLeagueSetup.Click
        frmLeagueSetup.Show()
    End Sub
    Private Sub CourseSetup_Click(sender As System.Object, e As System.EventArgs) Handles btnCourseSetup.Click
        frmCourse.Show()
    End Sub

    Private Sub PlayerSetup_Click(sender As System.Object, e As System.EventArgs) Handles btnPlayerSetup.Click
        frmPlayer.Show()
        'reload player table after updating
    End Sub

    Private Sub ScoreSetup_Click(sender As System.Object, e As System.EventArgs)
        frmScoring.Show()
    End Sub

    Private Sub Games_Click(sender As System.Object, e As System.EventArgs)
        frmSkins.Show()
    End Sub

    Private Sub PlayerStats_Click(sender As System.Object, e As System.EventArgs) Handles btnPlayerStats.Click
        frmPlayerStats.Show()
    End Sub

    Private Sub Handicaps_Click(sender As System.Object, e As System.EventArgs)
        frmHandicap.Show()
    End Sub

    Private Sub ScoreCard_Click(sender As System.Object, e As System.EventArgs) Handles btnScoreCard.Click

        If oHelper.bsch Then
            lblProcessMsg.Text = String.Format("Loading Scores")
            lblProcessMsg.BackColor = Color.Red
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            frmScoreCard.Show()

            lblProcessMsg.Text = String.Format("Finished Loading Scores")
            lblProcessMsg.BackColor = Color.LightGreen
            Me.Cursor = Cursors.Default
            Application.DoEvents()

        Else
            MsgBox("Schedule not available, cant do Scores")
        End If

    End Sub

    Private Sub EnterScores_Click(sender As Object, e As EventArgs)
        frmChooseScores.Show()
    End Sub
    Private Sub btnLast5_Click(sender As Object, e As EventArgs) Handles btnLast5.Click
        LastFive.Show()
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        '20180306-added checkbox for email backup option
        If cbMail.Checked Then
            Dim sfile = oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueFiles.zip"
            If IO.File.Exists((sfile)) Then
                Dim mbr = MsgBox(String.Format("File aready sent, want to create and send again"), MsgBoxStyle.YesNo)
                If mbr <> MsgBoxResult.Yes Then
                    Exit Sub
                End If
            End If

            Zipit()

            Dim ToAddresses As New List(Of String)({"gary_scudder@yahoo.com"})
            'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
            Dim attachs() As String = {sfile}
            Dim subject As String = "League Files"
            Dim body As String = "League Files"
            Dim bresult = False
            bresult = oHelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
            If bresult Then
                MsgBox("secretary mails sent successfully", MsgBoxStyle.Information)
            Else
                MsgBox(oHelper.GGmail.ErrorText, MsgBoxStyle.Critical)
            End If
        End If

        Close()
        End
    End Sub

    Private Sub btnStandings_Click(sender As Object, e As EventArgs) Handles btnStandings.Click
        If oHelper.bsch Then
            Standings.Show()
        Else
            MsgBox("Schedule not available, cant do Standings")
        End If

    End Sub

    Private Sub btnShowScores_Click(sender As Object, e As EventArgs) Handles btnShowScores.Click
        oHelper.sPlayer = ""
        Scores.Show()
    End Sub

    Private Sub btnMatches_Click(sender As Object, e As EventArgs) Handles btnMatches.Click

        If oHelper.bsch Then
            Matches.Show()
        Else
            MsgBox("Schedule not available, cant do Matches")
        End If
    End Sub
    Private Sub btnSkins_Click(sender As Object, e As EventArgs) Handles btnSkins.Click
        If oHelper.bsch Then
            Skins.Show()
        Else
            MsgBox("Schedule not available, cant do Skins")
        End If

    End Sub

    Private Sub cbLogging_CheckedChanged(sender As Object, e As EventArgs) Handles cbLogging.CheckedChanged
        If cbLogging.Checked Then
            oHelper.bloghelper = True
        Else
            oHelper.bloghelper = False
        End If

    End Sub

    Private Sub Main_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        rs.ResizeAllControls(Me)
    End Sub

    Private Sub btnXML_Click(sender As Object, e As EventArgs)

        'oHelper.sFilePath = "" '"\\wdmycloud\Gary\LeagueManager\Files"
        Dim sFile = ""
        Dim dialog As New OpenFileDialog With
        {
        .InitialDirectory = "e:\LeagueManager\Files",
        .Filter = "CSV Files (*.CSV) |*.csv|All Files (*.*)|*.*",
        .FilterIndex = 1,
        .FileName = ""
        }
        dialog.ShowDialog()

        If dialog.FileName <> "" Then
            Dim sdestFile As String = dialog.FileName & "_" & Now.ToString("yyyyMMdd_HHmmss")
            IO.File.Copy(dialog.FileName, sdestFile)
            Dim ds As New DataSet
            Dim dt As New DataTable
            ds.Tables.Add(dt)
            oHelper.CSV2DataTable(dt, dialog.FileName)
            ds.Tables(0).WriteXml(dialog.FileName.Replace(".csv", "_With_Schema.xml"), XmlWriteMode.WriteSchema)
            ds.Tables(0).WriteXml(dialog.FileName.Replace(".csv", ".xml"), XmlWriteMode.IgnoreSchema)
        Else
            End
        End If

    End Sub

    Private Sub btnCSV_Click(sender As Object, e As EventArgs)
        Dim oFiles() As IO.FileInfo
        Dim oFile As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(oHelper.sFilePath)
        oFiles = oDirectory.GetFiles("*.xml")
        Dim sLeagueName = cbLeagues.SelectedItem
        For Each oFile In oFiles
            Dim TempdsLeague As New DataSet
            Dim sfile = "dt" & oHelper.getSubstring(oFile.Name, "_", ".xml")
            sfile = sfile.Replace(sLeagueName & "_", "")
            'TempdsLeague.Tables.Add(sfile).ReadXml(oFile.FullName) ' oHelper.sFilePath & "\" & sfile)
            TempdsLeague.ReadXml(oFile.FullName) ' oHelper.sFilePath & "\" & sfile)
            'Dim sfilename = "\" & oHelper.dsLeague.Tables(sfile).TableName.Substring(2) & ".csv"
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables(sfile), oFile.FullName.Replace(".xml", ".csv"))
        Next
    End Sub

    Private Sub cbLeagues_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbLeagues.SelectedIndexChanged
        With lblProcessMsg
            .Text = String.Format("Loading League Tables for {0} ...", cbLeagues.SelectedItem)
            .BackColor = Color.Red

            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            If Not bLoad Then GetLeague()
            .Text = String.Format("Done Loading League Tables for {0}", cbLeagues.SelectedItem)
            .BackColor = Color.LightGreen
            Me.Cursor = Cursors.Default
            Application.DoEvents()
        End With
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        MsgBox("Hi Scott")
    End Sub

    Private Sub btnChangeFolder_Click(sender As Object, e As EventArgs) Handles btnChangeFolder.Click
        Dim dialog As New FolderBrowserDialog With
            {
            .RootFolder = Environment.SpecialFolder.Desktop,
            .SelectedPath = txtFolder.Text,
            .Description = "Select League Files Path"
            }
        If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            oHelper.sFilePath = dialog.SelectedPath
            txtFolder.Text = oHelper.sFilePath
        Else
            End
        End If

    End Sub

    Private Sub btnZip_Click(sender As Object, e As EventArgs) Handles btnZip.Click
        Zipit()
    End Sub
    Sub Zipit()
        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(oHelper.sFilePath)
        'oFiles = oDirectory.GetFiles("*.xml")
        oFiles = oDirectory.GetFiles("*.csv")
        'sort by date desc
        Helper.Controls.Helper.arraySort(oFiles)
        'oHelper.arraySort(oFiles)
        Dim sArrayOfFiles As New List(Of String)

        Dim sfile = oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueFiles.zip"
        If IO.File.Exists((sfile)) Then IO.File.Delete(sfile)

        Dim zipPackage As Package = Package.Open((sfile), IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

        'build an array list of the most recent files for each table
        For Each siFile In oFiles
            Try

                Dim zipPartUri As Uri = PackUriHelper.CreatePartUri(New Uri(siFile.Name, UriKind.Relative))
                Dim zipPackagePart As PackagePart = zipPackage.CreatePart(zipPartUri, "", CompressionOption.Normal)
                Dim sourceFileStream As IO.FileStream = New IO.FileStream(siFile.FullName, IO.FileMode.Open, IO.FileAccess.Read)

                Dim destinationFileStream As IO.Stream = zipPackagePart.GetStream

                'Dim contentType As String = Net.Mime.MediaTypeNames.Application.Zip
                Dim zipContent As Byte() = IO.File.ReadAllBytes(siFile.FullName)
                zipPackagePart.GetStream().Write(zipContent, 0, zipContent.Length)
                sourceFileStream.Close()

                'Dim zipPackage As Package = Package.Open(((sourceFileDirectory + "\Output.zip")), IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)
                'encryptedFileName = Path.GetFileName((sourceFileDirectory + ("\" _
                '                + (sourceFileName + "_encrypted.csv"))))
                'Dim zipPartUri As Uri = PackUriHelper.CreatePartUri(New Uri(encryptedFileName, UriKind.Relative))
                'Dim zipPackagePart As PackagePart = zipPackage.CreatePart(zipPartUri, "", CompressionOption.Normal)
                'Dim sourceFileStream As FileStream = New FileStream((sourceFileDirectory + ("\" _
                '                + (sourceFileName + "_encrypted.csv"))), FileMode.Open, FileAccess.Read)

                'Dim destinationFileStream As Stream = zipPackagePart.GetStream

                'Dim contentType As String = Net.Mime.MediaTypeNames.Application.Zip
                'Dim zipContent As Byte() = File.ReadAllBytes((sourceFileDirectory + ("\" + (sourceFileName + "_encrypted.csv"))))
                'zipPackagePart.GetStream().Write(zipContent, 0, zipContent.Length)

                'zipPackage.Close()
                'sourceFileStream.Close()

                Debug.Print(siFile.Name & " is zipped successfully")
                'MessageBox.Show((sifile.Name & " is zipped successfully."), "League Manager", MessageBoxButtons.OK, MessageBoxIcon.Information)
                'this opens the directory in explorer
                'Process.Start(oHelper.sFilePath)

            Catch exception As Exception
                MessageBox.Show(exception.Message, "Encrypt Compress", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        zipPackage.Close()

    End Sub
End Class
Public Class Team
    Implements IComparable

    Private m_strName As String
    Private m_strGUID As String

    Public Sub New(ByVal TeamName As String)
        m_strName = TeamName
        m_strGUID = Guid.NewGuid.ToString()
    End Sub

    Public ReadOnly Property TeamName() As String
        Get
            Return m_strName
        End Get
    End Property

    Public ReadOnly Property TeamGuid() As String
        Get
            Return m_strGUID
        End Get
    End Property

    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
        Dim team2 As Team = CType(obj, Team)
        Return String.Compare(m_strGUID, team2.TeamGuid)
    End Function

    Public Overrides Function ToString() As System.String
        Dim strValue As System.String

        strValue = m_strName

        Return strValue
    End Function

End Class


