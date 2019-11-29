'*************************************************************************************************
Imports System.IO.Packaging
Imports LeagueManager.FileLayout
Public Class Main
    Dim cVersion = "Version : 2019.09.05"
    Public oHelper As Helper
    Private dsLeague As New dsLeague
    Dim bload As Boolean = True
    'Private Teams As ArrayList = New ArrayList
    'Private Teams2 As ArrayList = New ArrayList
    Dim rs As New Resizer
    Dim bwait As Boolean
    Dim sParmFile As String
    Public iScreenWidth As Integer
    Public iScreenHeight As Integer
    Public sWorkingYear As String

    Private Sub Main_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Show()
            oHelper = New Helper
            'Genschedule()

            Application.EnableVisualStyles()
            lblProcessMsg.Text = "Loading League Tables..."
            oHelper.status_Msg(lblProcessMsg, Me)
            'this code uses filelayout and populates a datatable
            'Dim PlayerFileLayout As New List(Of FileLayout)
            'PlayerFileLayout.Add(New FileLayout("Name", 0, 30, GetType(System.String), FieldType.Key))
            'PlayerFileLayout.Add(New FileLayout("Street", 31, 30, GetType(System.String), FieldType.Ignore))
            'PlayerFileLayout.Add(New FileLayout("City", 61, 20, GetType(System.String), FieldType.Ignore))
            'PlayerFileLayout.Add(New FileLayout("State", 81, 2, GetType(System.String), FieldType.Ignore))
            'PlayerFileLayout.Add(New FileLayout("Zip", 83, 5, GetType(System.Int16), FieldType.Ignore))
            'Dim drNEW As DataRow
            'Dim dtAddress = New DataTable
            'For Each fl In PlayerFileLayout
            '    dtAddress.Columns.Add(fl.sDataField)
            'Next

            'drNEW = dtAddress.NewRow
            'Dim xline = "Gary Scudder                   6197 Ridgewood Ct             Florence            KY41042"
            'For Each fl In PlayerFileLayout
            '    drNEW(fl.sDataField) = xline.Substring(fl.iDataPos, fl.iDataLen)
            'Next

            'dtAddress.Rows.Add(drNEW)

            iScreenWidth = Screen.PrimaryScreen.Bounds.Width
            'greg test
            'iScreenWidth = 1368
            iScreenHeight = Screen.PrimaryScreen.Bounds.Height
            If Debugger.IsAttached Then Me.Text &= " - Debug Mode"

            '20180106 remove for now
            btnShowScores.Visible = False

            'rs.FindAllControls(Me)
            'Me.Size = My.Computer.Screen.WorkingArea.Size
            'Me.WindowState = FormWindowState.Maximized
            Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent

            Dim sDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            '12/19/2017 note this is removed for now 
            Me.Icon = New Icon(Me.Icon, New Size(Me.Icon.Width * 5, Me.Icon.Height * 5))
            'oHelper.bloghelper = True
            lblProcessMsg.Text = String.Format("Loading League Cookie from {0},", sDocs)
            oHelper.status_Msg(lblProcessMsg, Me)

            'load cookie settings
            If IO.File.Exists(sDocs & "\Leaguemanager.ini") Then
                Using sr As New IO.StreamReader(sDocs & "\Leaguemanager.ini", False)
                    Do
                        Dim sline = sr.ReadLine
                        If sline = Nothing Then Exit Do
                        Dim slineparts = sline.Split("=")
                        Select Case slineparts(0)
                            Case "LeagueName"
                                oHelper.sLeagueName = slineparts(1)
                            Case "GroupNumber"
                                oHelper.sGroupNumber = slineparts(1)
                            Case "Date"
                                oHelper.dDate = Date.ParseExact(slineparts(1), "MM-dd-yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                                sWorkingYear = slineparts(1).Substring(6, 4)
                            Case "FilePath"
                                If IO.Directory.Exists(slineparts(1)) Then
                                    oHelper.sFilePath = slineparts(1)
                                Else
                                    oHelper.sFilePath = ""
                                End If
                            Case "ReportPath"
                                If IO.Directory.Exists(slineparts(1)) Then
                                    oHelper.sReportPath = slineparts(1)
                                Else
                                    oHelper.sReportPath = ""
                                End If
                            Case "Logging"
                                If slineparts(1).ToUpper = "Y" Then
                                    oHelper.LOGIT("Logging On")
                                    cbLogging.Checked = True
                                Else
                                    cbLogging.Checked = False
                                End If
                            Case "DateOverlapReminder"
                                If slineparts(1).ToUpper = "Y" Then
                                    oHelper.bDateOverlap = True
                                Else
                                    oHelper.bDateOverlap = False
                                End If
                        End Select
                    Loop
                End Using
            End If

            'oHelper.sFilePath = "" '"\\wdmycloud\Gary\LeagueManager\Files"
            If oHelper.sFilePath = "" Then
                Dim dialog As New FolderBrowserDialog With
                {
                .RootFolder = Environment.SpecialFolder.Desktop,
                .SelectedPath = "C:\LeagueManager\Files",
                .Description = "Select League Files Path"
                }
                If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    oHelper.sFilePath = dialog.SelectedPath
                Else
                    End
                End If
            End If

            If oHelper.sReportPath = "" Then
                Dim dialog As New FolderBrowserDialog With
                {
                .RootFolder = Environment.SpecialFolder.Desktop,
                .SelectedPath = "C:\LeagueManager\Reports",
                .Description = "Select League Reports Path"
                }
                If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    oHelper.sReportPath = dialog.SelectedPath
                Else
                    End
                End If
            End If

            lblProcessMsg.Text = String.Format("Finished Loading League INI from {0},", sDocs)
            oHelper.status_Msg(lblProcessMsg, Me)

            'save ini file if league present
            'If oHelper.sLeagueName <> "" Then oHelper.UpdateINI()
            Me.Text = Me.Text & " " & String.Format("{0} - {1}, Resolution {2} x {3}", cVersion, My.Computer.Name, iScreenWidth, iScreenHeight)
            oHelper.LOGIT(Me.Text)

            txtFolder.Text = oHelper.sFilePath

            'build dropdown list of leagues
            ' DsLeague = New dsLeague
            'If Not oHelper.CSV2DataTable(dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv")) Then
            '    MsgBox(String.Format("File in use, close file and restart {0} {1}", vbCrLf, oHelper.getLatestFile("*LeagueParms.csv")))
            '    End
            'End If
            bwait = True
            sParmFile = oHelper.getLatestFile(sWorkingYear, "*LeagueParms.csv")
            Do While bwait
                If Not oHelper.WaitForFile(dsLeague.Tables("dtLeagueParms"), sParmFile, lblProcessMsg, Me) Then
                    Dim mbr = MessageBox.Show(String.Format("File in use {0}Press <OK> to close file and proceed or <Cancel>", vbCrLf, sParmFile), sParmFile, MessageBoxButtons.OKCancel)
                    If mbr = DialogResult.Cancel Then
                        End
                    End If
                Else
                    bwait = False
                End If
            Loop
            'oHelper.dsLeague.ReadXml(oHelper.getXMLFile("*LeagueParms.xml"))
            Dim dvLeagues = New DataView(dsLeague.Tables("dtLeagueParms"))
            dvLeagues.Sort = "Name Asc, StartDate Desc"

            If dvLeagues.Count = 0 Then
                Dim mbr = MsgBox("League parm file not found, want to create one?", MsgBoxStyle.YesNo)
                If mbr = MsgBoxResult.Yes Then
                    frmLeagueSetup.ShowDialog()
                    End
                End If
            End If

            For Each row In dvLeagues
                '20181017  put in due to incomplete scores < 2018 and player lookup needs to change for those scores also
                If row("Startdate").year < "2018" Then Continue For
                cbLeagues.Items.Add(row("Name") & " (" & row("StartDate").year & ")")
            Next

            cbLeagues.SelectedIndex = 0
            For i = 0 To cbLeagues.Items.Count - 1
                Dim sitem As String = cbLeagues.Items(i)
                Dim sYear As String = sitem.Substring(sitem.IndexOf("(") + 1, 4)
                If sYear = sWorkingYear Then
                    cbLeagues.SelectedIndex = i
                    Exit For
                End If
            Next

            GetLeague()

            'setup email function
            oHelper.GGmail = New GGSMTP_GMAIL(oHelper.rLeagueParmrow("Email"), oHelper.rLeagueParmrow("EmailPassword"))
            'GetXSDNameByFileName(oHelper.dsLeague.Tables("dtScores"),
            oHelper.dsLeague = dsLeague
            ''get the date of the schedule for this week
            ''just use the Column names which have dates of the schedule table
            ''this loop will compare the league start date and flip the hole marker based on front/back
            bload = False
            lblProcessMsg.Text = "Finished Loading League Tables"
            oHelper.status_Msg(lblProcessMsg, Me)
            'oHelper.bloghelper = False

            'debugging playerstats, uncomment below
            'PlayerStats_Click(sender, e)

        Catch ex As InvalidCastException
            Dim iexc = ""
        End Try
    End Sub
    Function WaitForFile(ByVal file As String) As String
        If file = "Schedule" Then
            WaitForFile = oHelper.getLatestFile(sWorkingYear, String.Format("*{0}.csv", file))
        Else
            WaitForFile = oHelper.getLatestFile("9999", String.Format("*{0}.csv", file))
        End If
        If WaitForFile = "" Then
            Dim mbr = MessageBox.Show(String.Format("File {1} missing{0} Press <OK> to move file and proceed Or <Cancel>", vbCrLf, file), file, MessageBoxButtons.OKCancel)
            If mbr = DialogResult.Cancel Then End
            Dim i = 30
            'MsgBox(String.Format("File {0} is in use, will wait up for {1} seconds to free up", file, i))
            Do Until i = 0
                lblProcessMsg.Text = String.Format("Waiting for file {0} for {1} seconds", file, i)
                oHelper.status_Msg(lblProcessMsg, Me)
                Threading.Thread.Sleep(1000)
                i -= 1
            Loop
            lblProcessMsg.Text = String.Format("Finished Waiting for file {0}", file)
            oHelper.status_Msg(lblProcessMsg, Me)
        End If

    End Function
    Sub newBuildTablesForLeague()

        'Scores
        Dim ofiles As String = "Schedule,Scores,Payments,Players,Courses"
        '20190724-progress bar
        tspb.ProgressBar.Value = 0
        tspb.ProgressBar.Minimum = 0
        tspb.ProgressBar.Maximum = ofiles.Split(",").Count
        tssl.Text = String.Format("Loading {0} files", tspb.ProgressBar.Maximum)
        Dim et As TimeSpan
        Dim sStartTime As DateTime = Now

        For Each file As String In ofiles.Split(",")
            Dim sLatestFile As String = ""
            'check for file not found
            Do Until sLatestFile <> ""
                sLatestFile = WaitForFile(file)
            Loop

            Dim sfileparts = sLatestFile.ToString.Split("\")
            Dim sfilename = sfileparts(UBound(sfileparts))
            Dim sFileLeagueName = sfilename.Split("_")(1)
            Dim sfile = "dt" & oHelper.getSubstring(sLatestFile, "_", ".")

            oHelper.LOGIT(String.Format("Checking {0}", file))
            oHelper.LOGIT(String.Format("Keeping File {0}", sLatestFile.ToString))
            oHelper.LOGIT(String.Format("File name {0}", sLatestFile))
            oHelper.LOGIT(String.Format("Removing table {0}", sfile))
            If file = "Schedule" Then
                'match year
                ''match league name
                oHelper.LOGIT(String.Format("Matching League Name and Year {0} - {1}", sWorkingYear, sFileLeagueName))
                If sWorkingYear.Contains(sWorkingYear) And cbLeagues.SelectedItem.ToString.Contains(sFileLeagueName) Then oHelper.LOGIT(String.Format("Matched League Name and Year"))
                oHelper.bsch = True
                If dsLeague.Tables.Contains(sfile) Then
                    oHelper.LOGIT(String.Format("dsleague.tables contains table dtschedule {0}", sfile))
                    Try
                        dsLeague.Tables.Remove(sfile)
                    Catch ex As Exception

                    End Try
                End If
                dsLeague.Tables.Add(sfile)
            Else
                If file = "Scores" Then lbScoresFile.Text = sfilename
                Dim dt As DataTable = dsLeague.Tables(sfile)
                If sfile.contains("Payments") Then
                    dt.PrimaryKey = New DataColumn() {dt.Columns("Player"), dt.Columns("Date"), dt.Columns("Desc"), dt.Columns("Detail")}
                    oHelper.LOGIT("Payments file " & sfile)
                End If
                dt.Rows.Clear()
            End If

            bwait = True
            Do While bwait
                If Not oHelper.WaitForFile(dsLeague.Tables(sfile), sLatestFile, lblProcessMsg, Me) Then
                    Dim mbr = MessageBox.Show(String.Format("File in use {0}Press <OK> to close file and proceed or <Cancel>", vbCrLf, sParmFile), sParmFile, MessageBoxButtons.OKCancel)
                    If mbr = DialogResult.Cancel Then
                        End
                    End If
                Else
                    bwait = False
                End If
            Loop
            tspb.ProgressBar.Value += 1
            tssl.Text = String.Format("Loading file {0} of {1}", tspb.ProgressBar.Value, ofiles.Count)
            'tspb.ProgressBar.Refresh()
            Application.DoEvents()
        Next

        et = Now - sStartTime
        If et.TotalMinutes >= 1 Then
            tssl.Text = String.Format("Loaded {0} files {1} elapsed time", ofiles.Count, CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs")
        Else
            tssl.Text = String.Format("Loaded {0} files {1} elapsed time", ofiles.Count, CInt(et.TotalSeconds) Mod 60 & " Secs")
        End If

        Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
        dvscores.Sort = "Date desc"
        'change mm/dd/yyyy to yyyymmdd
        Dim sdate = cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) + 1 & "0101"
        dvscores.RowFilter = String.Format("Date <{0}", sdate)

        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) & "0101" > dvscores(0)("Date") Then
            oHelper.sDateLastScore = CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        Else
            oHelper.sDateLastScore = dvscores(0)("Date")
        End If
        oHelper.MyCourse = dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")

    End Sub

    Sub BuildTablesForLeague()

        Try
            'this subroutine will load the files related to the league/year that resides in the dropdown selected item 
            Dim sLeagueName = cbLeagues.SelectedItem

            Dim oFiles() As IO.FileInfo
            Dim oDirectory As New IO.DirectoryInfo(oHelper.sFilePath)
            'oFiles = oDirectory.GetFiles("*.xml")
            oFiles = oDirectory.GetFiles("*.csv")

            'sort by date desc
            Helper.arraySort(oFiles)
            'oHelper.arraySort(oFiles)
            Dim sArrayOfFiles As New List(Of String)

            '20190724-progress bar
            tspb.ProgressBar.Value = 0
            tspb.ProgressBar.Minimum = 0
            tspb.ProgressBar.Maximum = oFiles.Count
            tssl.Text = String.Format("Loading {0} files", tspb.ProgressBar.Maximum)
            Dim et As TimeSpan
            Dim sStartTime As DateTime = Now

            'build an array list of the most recent files for each table
            For Each sfile In oFiles
                oHelper.LOGIT(String.Format("Examining File {0}", sfile.FullName.ToString))
                'league parm file already read into datatable above
                If sfile.FullName.Contains("LeagueParm") Or sfile.FullName.Contains("Standings") Then
                    tspb.ProgressBar.Value += 1
                    tssl.Text = String.Format("Loading file {0} of {1}", tspb.ProgressBar.Value, oFiles.Count)
                    'tspb.ProgressBar.Refresh()
                    Application.DoEvents()
                    Continue For
                End If
                If sfile.FullName.Contains("Schedule") Then
                    oHelper.LOGIT("Checking Schedule")
                    Dim sfileparts = sfile.FullName.ToString.Split("\")
                    Dim sfilename = sfileparts(UBound(sfileparts))
                    oHelper.LOGIT(String.Format("File name {0}", sfile.FullName.ToString))
                    'match year
                    Dim sYear = sfilename.Substring(0, 4)
                    Dim sFileLeagueName = sfilename.Split("_")(1)
                    'match league name
                    oHelper.LOGIT(String.Format("Matching League Name and Year {0} - {1}", sLeagueName, sFileLeagueName))
                    If sLeagueName.contains(sYear) And sLeagueName.contains(sFileLeagueName) Then
                        oHelper.LOGIT(String.Format("Matched League Name and Year"))
                        If Not oHelper.bsch Then
                            oHelper.bsch = True
                            oHelper.LOGIT(String.Format("Keeping Schedule {0}", sfile.FullName.ToString))
                            sArrayOfFiles.Add(sfile.FullName)
                        End If
                    Else
                        tspb.ProgressBar.Value += 1
                        tssl.Text = String.Format("Loading file {0} of {1}", tspb.ProgressBar.Value, oFiles.Count)
                        'tspb.ProgressBar.Refresh()
                        Application.DoEvents()
                        Continue For
                    End If
                ElseIf sfile.FullName.Contains("Courses") Then
                    If Not oHelper.bcourses Then
                        oHelper.bcourses = True
                        oHelper.LOGIT(String.Format("Set table from {0}", sfile.FullName.ToString))
                        sArrayOfFiles.Add(sfile.FullName)
                    End If
                ElseIf sfile.FullName.Contains("Players") Then
                    If Not oHelper.bplayer Then
                        oHelper.bplayer = True
                        oHelper.LOGIT(String.Format("Set table from {0}", sfile.FullName.ToString))
                        sArrayOfFiles.Add(sfile.FullName)
                    End If
                ElseIf sfile.FullName.EndsWith("_Scores.csv") Then
                    If Not oHelper.bscores Then
                        oHelper.bscores = True
                        oHelper.LOGIT(String.Format("Set table from {0}", sfile.FullName.ToString))
                        sArrayOfFiles.Add(sfile.FullName)
                    End If
                ElseIf sfile.FullName.EndsWith("_Payments.csv") Then
                    If Not oHelper.bpayments Then
                        oHelper.bpayments = True
                        oHelper.LOGIT(String.Format("Set table from {0}", sfile.FullName.ToString))
                        sArrayOfFiles.Add(sfile.FullName)
                    End If
                End If
                tspb.ProgressBar.Value += 1
                tssl.Text = String.Format("Loading file {0} of {1}", tspb.ProgressBar.Value, oFiles.Count)
                'tspb.ProgressBar.Refresh()
                Application.DoEvents()
            Next


            et = Now - sStartTime
            If et.TotalMinutes >= 1 Then
                tssl.Text = String.Format("Loaded {0} files {1} elapsed time", oFiles.Count, CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs")
            Else
                tssl.Text = String.Format("Loaded {0} files {1} elapsed time", oFiles.Count, CInt(et.TotalSeconds) Mod 60 & " Secs")
            End If

            If Not oHelper.bsch Then
                MsgBox(String.Format("Cant find a Schedule file {0}...exiting", oHelper.sFilePath))
                End
            ElseIf Not oHelper.bscores Then
                MsgBox(String.Format("Cant find a Scores file {0}...exiting", oHelper.sFilePath))
                End
            ElseIf Not oHelper.bcourses Then
                MsgBox(String.Format("Cant find a Course file {0}...exiting", oHelper.sFilePath))
                End
            ElseIf Not oHelper.bplayer Then
                MsgBox(String.Format("Cant find a Player file {0}...exiting", oHelper.sFilePath))
                End
            ElseIf Not oHelper.bpayments Then
                MsgBox(String.Format("Cant find a Payments file {0}...exiting", oHelper.sFilePath))
                End
            End If
            'save this in helper so other subroutines can use
            oHelper.sArrayOfFiles = sArrayOfFiles
            For Each saFile In sArrayOfFiles
                Dim sfile = "dt" & oHelper.getSubstring(saFile, "_", ".")
                sfile = sfile.Replace(oHelper.getSubstring(cbLeagues.SelectedItem, 0, " ") & "_", "")
                'Dim sfilename = "Temp_" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & sfile & ".xml"
                'DsLeague.Tables(sfile).WriteXml(sfilename, XmlWriteMode.WriteSchema)
                'DsLeague.ReadXml(sfilename)
                'System.IO.File.Delete(sfilename)
                If dsLeague.Tables.Contains(sfile) Then
                    If sfile = "dtSchedule" Then
                        '20190322-this code gets hit when league name changes
                        oHelper.LOGIT("removing table dtschedule")
                        If dsLeague.Tables.Contains(sfile) Then
                            oHelper.LOGIT("dsleague.tables contains table dtschedule")
                            Try
                                dsLeague.Tables.Remove(sfile)
                            Catch ex As Exception

                            End Try

                        End If
                        dsLeague.Tables.Add(sfile)
                    Else
                        Dim dt As DataTable = dsLeague.Tables(sfile)
                        If sfile.contains("Payments") Then
                            dt.PrimaryKey = New DataColumn() {dt.Columns("Player"), dt.Columns("Date"), dt.Columns("Desc"), dt.Columns("Detail")}
                            oHelper.LOGIT("Payments file " & saFile)
                        End If
                        dt.Rows.Clear()
                    End If
                Else
                    '20190322-this code only executes when were building the schedule
                    dsLeague.Tables.Add(sfile)
                End If

                'If Not oHelper.CSV2DataTable(dsLeague.Tables(sfile), saFile) Then
                '    MsgBox(String.Format("File in use - {1} {0} close file And restart", vbCrLf, saFile))
                '    End
                'End If
                bwait = True
                sParmFile = saFile
                Do While bwait
                    If Not oHelper.WaitForFile(dsLeague.Tables(sfile), sParmFile, lblProcessMsg, Me) Then
                        Dim mbr = MessageBox.Show(String.Format("File in use {0}Press <OK> to close file and proceed or <Cancel>", vbCrLf, sParmFile), sParmFile, MessageBoxButtons.OKCancel)
                        If mbr = DialogResult.Cancel Then
                            End
                        End If
                    Else
                        bwait = False
                    End If
                Loop

                If sfile = "dtScores" Then
                    lbScoresFile.Text = saFile
                    'ElseIf sfile = "dtSchedule" Then
                    '    oHelper.dsLeague = dsLeague
                    '    Dim dt As DataTable = dsLeague.Tables(sfile)
                    '    dt.Rows.Clear()
                    '    dsLeague.Tables(sfile) = oHelper.buildSchedule()
                End If

                'this is used for xml 
                'If oHelper.dsLeague.Tables.Contains(sfile) Then
                '    oHelper.dsLeague.Tables(sfile).ReadXml(saFile)
                'Else
                '    oHelper.dsLeague.Tables.Add(sfile).ReadXml(saFile)
                'End If
                ''Dim sfilename = "\" & oHelper.dsLeague.Tables(sfile).TableName.Substring(2) & ".csv"
                'oHelper.DataTable2CSV(oHelper.dsLeague.Tables(sfile), oFile.FullName.Replace(".xml", ".csv"))
            Next

            'If DsLeague.Tables.Contains("dtScores") Then DsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {DsLeague.Tables("dtScores").Columns("Player"), DsLeague.Tables("dtScores").Columns("Date")}
            'Dim x = oHelper.dsLeague.Tables("dtPlayers").Select name from Table A Group By name having count(*) > 1"
            '20180126-only build dates for the last date of completed scores
            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
            dvscores.Sort = "Date desc"
            'change mm/dd/yyyy to yyyymmdd
            'Dim wkdate As Date = oHelper.dDate
            'Dim reformatted1 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            Dim sdate = cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) + 1 & "0101"
            dvscores.RowFilter = String.Format("Date < {0}", sdate)

            If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) & "0101" > dvscores(0)("Date") Then
                oHelper.sDateLastScore = CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            Else
                oHelper.sDateLastScore = dvscores(0)("Date")
            End If
            oHelper.MyCourse = dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")

            'If dsLeague.Tables.Contains("dtPlayers") Then dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {dsLeague.Tables("dtPlayers").Columns("Name")}
        Catch exception As Exception
            MessageBox.Show(exception.Message, "GetLeagues", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Sub GetLeague()
        'oHelper.dsLeague.Tables.RemoveAt(0)
        'since xml oHelper.files are named yyyymmdd_***.csv. we will have multiple files for each data table so we want to get the most recent one
        'oHelper.dsLeague = New dsLeague
        'oHelper.dsLeague.ReadXml(oHelper.getLatestFile("*LeagueParms.csv"))
        'oHelper.CSV2DataTable(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv"))
        'oHelper.CSV2DataTable(dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv"))
        Dim dvLeagues = New DataView(dsLeague.Tables("dtLeagueParms"))
        dvLeagues.Sort = "Name Asc, StartDate Desc"

        For Each row As DataRowView In dvLeagues
            Dim wkdate As Date = row("startDate")
            Dim reformatted As String = wkdate.ToString("MM/dd/yyyy", Globalization.CultureInfo.InvariantCulture)
            Dim sLeagueName = row("Name") & " (" & row("Startdate").year & ")"
            If sLeagueName = cbLeagues.SelectedItem Then
                'save row of league so all routines can use
                With oHelper
                    .rLeagueParmrow = row
                    .sLeagueName = .rLeagueParmrow("Name")
                    oHelper.UpdateINI()
                    If .rLeagueParmrow("ScoresLocked") Is DBNull.Value Then .rLeagueParmrow("ScoresLocked") = "N"

                    newBuildTablesForLeague()
                    '20180930 -setup dates
                    dtScore.Text = DateTime.ParseExact(.sDateLastScore, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0")
                    cbScoreDt.Text = .sDateLastScore
                    Dim sscoreDate As Date = .rLeagueParmrow("StartDate")
                    Dim daydiff As Integer = Date.Today.DayOfWeek - DayOfWeek.Tuesday
                    Dim lasttuesday As Date = Date.Today.AddDays(-daydiff + 7)
                    Dim sstartdate As Date = .rLeagueParmrow("StartDate")
                    Dim senddate As Date = .rLeagueParmrow("EndDate")
                    Do While lasttuesday >= sstartdate
                        cbScoreDt.Items.Add(lasttuesday.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
                        lasttuesday = lasttuesday.AddDays(-7)
                    Loop

                    'senddate = .rLeagueParmrow("EndDate")
                    'Do While senddate >= sstartdate
                    '    cbRSStart.Items.Add(senddate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
                    '    senddate = senddate.AddDays(-7)
                    'Loop
                    'senddate = .rLeagueParmrow("EndDate")
                    'Do While senddate >= sstartdate
                    '    cbRSEnd.Items.Add(senddate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
                    '    senddate = senddate.AddDays(-7)
                    'Loop
                    'senddate = .rLeagueParmrow("EndDate")
                    'cbRSStart.SelectedItem = sstartdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                    'cbRSEnd.SelectedItem = senddate.ToString("yyyyMMdd")

                    Dim iweeks As Integer = (((.rLeagueParmrow("Teams") - 1) * 2) - 1)
                    dtRSStart.Text = .rLeagueParmrow("StartDate")
                    tbPSStart.Text = CDate(.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                    dtRSEnd.Text = CDate(.rLeagueParmrow("EndDate")) 'CDate(.rLeagueParmrow("StartDate")).AddDays(iweeks * 7)
                    '.rLeagueParmrow("EndDate") = dtRSEnd.Text
                    '.rLeagueParmrow("PostSeasonDt") = dtPSStart.Text
                    tbPSEnd.Text = CDate(.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                End With
            End If
        Next

    End Sub

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
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show("Scores < 2018 are not fully entered, cannot view yet,check with developer")
            Exit Sub
        End If

        If oHelper.bsch Then
            lblProcessMsg.Text = String.Format("Loading Scores from {0}", lbScoresFile.Text)
            oHelper.status_Msg(lblProcessMsg, Me)

            frmScoreCard.Show()

            lblProcessMsg.Text = String.Format("Finished Loading Scores")
            oHelper.status_Msg(lblProcessMsg, Me)

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
            Dim sfile = oHelper.sReportPath & "\" & Now.ToString("yyyyMMdd") & "_LeagueFiles.zip"
            If IO.File.Exists((sfile)) Then
                Dim mbr = MsgBox(String.Format("File already sent, want to create and send again"), MsgBoxStyle.YesNo)
                If mbr <> MsgBoxResult.Yes Then Exit Sub
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
        oHelper.UpdateINI()
        Close()
        End
    End Sub

    Private Sub btnStandings_Click(sender As Object, e As EventArgs) Handles btnStandings.Click
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show("Scores < 2018 are not fully entered, cannot view yet,check with developer")
            Exit Sub
        End If
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
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show("Scores < 2018 are not fully entered, cannot view yet,check with developer")
            Exit Sub
        End If
        If oHelper.bsch Then
            Matches.Show()
        Else
            MsgBox("Schedule not available, cant do Matches")
        End If
    End Sub
    Private Sub btnSkinsRpt_Click(sender As Object, e As EventArgs) Handles btnSkinsRpt.Click
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show("Scores < 2018 are not fully entered, cannot view yet,check with developer")
            Exit Sub
        End If
        SkinRpt.Show()
    End Sub
    Private Sub btnSkins_Click(sender As Object, e As EventArgs) Handles btnSkins.Click
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show("Scores < 2018 are not fully entered, cannot view yet,check with developer")
            Exit Sub
        End If
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
        'rs.ResizeAllControls(Me)
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
        If bload Then Exit Sub
        oHelper.bsch = False
        oHelper.bscores = False
        oHelper.bplayer = False
        oHelper.bcourses = False

        With lblProcessMsg
            .Text = String.Format("Saving League Tables for {0} ...", cbLeagues.SelectedItem)
            oHelper.status_Msg(lblProcessMsg, Me)
            oHelper.DataTable2CSV(dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
            .Text = String.Format("Finished Saving League Tables for {0} ...", cbLeagues.SelectedItem)
            oHelper.status_Msg(lblProcessMsg, Me)
            .Text = String.Format("Loading League Tables for {0} ...", cbLeagues.SelectedItem)
            sWorkingYear = cbLeagues.SelectedItem.ToString.Split("(")(1).Substring(0, 4)
            oHelper.status_Msg(lblProcessMsg, Me)
            If Not bload Then GetLeague()
            .Text = String.Format("Finished Loading League Tables for {0}", cbLeagues.SelectedItem)
            oHelper.status_Msg(lblProcessMsg, Me)
        End With

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
            '20190307 - Update INI file after folder change
            oHelper.UpdateINI()
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
        Helper.arraySort(oFiles)
        'oHelper.arraySort(oFiles)
        Dim sArrayOfFiles As New List(Of String)

        Dim sfile = oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd_hhmmss") & "_LeagueFiles.zip"
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

                oHelper.LOGIT(siFile.Name & " is zipped successfully")
                'MessageBox.Show((sifile.Name & " is zipped successfully."), "League Manager", MessageBoxButtons.OK, MessageBoxIcon.Information)
                'this opens the directory in explorer
                'Process.Start(oHelper.sFilePath)

            Catch exception As Exception
                MessageBox.Show(exception.Message, "Encrypt Compress", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        zipPackage.Close()

    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Finance.Show()
    End Sub

    'Private Sub TbRSStart_TextChanged(sender As Object, e As EventArgs)
    '    If bload Then Exit Sub
    '    Dim sStartDate As Date = Date.ParseExact(tbRSStart.Text, "MM-dd-yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    '    Dim sWorkingYear As String = cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4)
    '    'Year should be same year as the selected year were processing
    '    If sStartDate.Year <> sWorkingYear Then
    '        MessageBox.Show(String.Format("Year {0} must be {1}, try again",
    '                sStartDate.Year, sWorkingYear), String.Format("Year Must be {0}", sWorkingYear), MessageBoxButtons.OK)
    '    End If
    '    'month should be april
    '    If sStartDate.Month <> 4 Then
    '        Dim smsg = String.Format("Start month {0} is before April, you sure you want to use this date?", tbRSStart.Text, sStartDate.Month)
    '        Dim result As Integer = MessageBox.Show(smsg, "Start Month before April", MessageBoxButtons.YesNo)
    '        If result = DialogResult.No Then
    '            tbPSStart.Text = CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
    '        End If
    '    End If

    '    'day of week must be a tuesday
    '    If sStartDate.DayOfWeek <> DayOfWeek.Tuesday Then
    '        MessageBox.Show(String.Format("Date {0} must be a Tuesday and its a {1}, try again",
    '               tbRSStart.Text, sStartDate.DayOfWeek), "Date Must be a Tuesday", MessageBoxButtons.OK)
    '    End If

    '    If tbRSEnd.Text >= tbPSStart.Text Then
    '        oHelper.bCCLeague = True
    '        If oHelper.bDateOverlap Then
    '            Dim smsg = String.Format("Regular Season is played the same time as your League Championship" & vbCrLf & vbCrLf & "Regular Season {0}" & vbCrLf & "League Championship {1}", tbRSEnd.Text, tbPSStart.Text)
    '            smsg &= String.Format(vbCrLf & "Keep being Reminded?" & vbCrLf & "Press Yes or No")

    '            Dim result As Integer = MessageBox.Show(smsg, "Warning Dates Overlap", MessageBoxButtons.YesNo)
    '            If result = DialogResult.Yes Then
    '                oHelper.bDateOverlap = True
    '            ElseIf result = DialogResult.No Then
    '                oHelper.bDateOverlap = False
    '            End If
    '        End If
    '    Else
    '        oHelper.bCCLeague = False
    '    End If
    '    oHelper.rLeagueParmrow("PostSeasonDt") = tbPSStart.Text
    '    '20190804 - dont need to do because its saved on exit every time
    '    '       oHelper.DataTable2CSV(dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
    '    tbPSEnd.Text = CDate(tbPSStart.Text).AddDays(7)

    'End Sub

    'Private Sub dtPSStart_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)

    '    If bload Then Exit Sub
    '    If dtPSStart.Text = "1/1/1900" Then Exit Sub
    '    'change mm/dd/yyyy to yyyymmdd
    '    Dim wkdate As Date = dtRSEnd.Text
    '    Dim reformatted1 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
    '    wkdate = dtPSStart.Text
    '    Dim reformatted2 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
    '    If reformatted1 >= reformatted2 Then
    '        oHelper.bCCLeague = True
    '        If oHelper.bDateOverlap Then
    '            Dim smsg = String.Format("Regular Season is played the same time as your League Championship" & vbCrLf & vbCrLf & "Regular Season {0}" & vbCrLf & "League Championship {1}", dtRSEnd.Text, dtPSStart.Text)
    '            smsg &= String.Format(vbCrLf & "Keep being Reminded?" & vbCrLf & "Press Yes or No")

    '            Dim result As Integer = MessageBox.Show(smsg, "Warning Dates Overlap", MessageBoxButtons.YesNo)
    '            If result = DialogResult.Yes Then
    '                oHelper.bDateOverlap = True
    '            ElseIf result = DialogResult.No Then
    '                oHelper.bDateOverlap = False
    '            End If
    '        End If
    '    Else
    '        oHelper.bCCLeague = False
    '    End If
    '    oHelper.rLeagueParmrow("PostSeasonDt") = dtPSStart.Text
    '    oHelper.DataTable2CSV(dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
    '    tbPSEnd.Text = CDate(dtPSStart.Text).AddDays(7)
    '    gbPS.Visible = True
    'End Sub
    ''20181016 below remove handlers and add handlers were added because datetime picker changed the date every time we click the arrow keys
    'Private Sub dtPSStart_DropDown(ByVal sender As Object, ByVal e As EventArgs)
    '    RemoveHandler dtPSStart.ValueChanged, AddressOf dtPSStart_ValueChanged
    'End Sub

    'Private Sub dtPSStart_CloseUp(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    AddHandler dtPSStart.ValueChanged, AddressOf dtPSStart_ValueChanged
    '    Call dtPSStart_ValueChanged(sender, EventArgs.Empty)
    'End Sub
    'Sub CheckPSDate()
    '    If dtPSStart.Text = "1/1/1900" Then Exit Sub
    '    'change mm/dd/yyyy to yyyymmdd
    '    Dim wkdate As Date = dtRSEnd.Text
    '    Dim reformatted1 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
    '    wkdate = dtPSStart.Text
    '    Dim reformatted2 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
    '    If reformatted1 >= reformatted2 Then
    '        oHelper.bCCLeague = True
    '        'If dtRSEnd.Text >= dtPSStart.Text Then
    '        Dim smsg = String.Format("Regular Season is played the same time as your League Championship" & vbCrLf & vbCrLf & "Regular Season {0}" & vbCrLf & "League Championship {1}", dtRSEnd.Text, dtPSStart.Text)
    '        MessageBox.Show(smsg, "Warning Dates Overlap", MessageBoxButtons.OK)
    '        'Dim mbr As DialogResult = MessageBox.Show(smsg, "Warning Dates Overlap", MessageBoxButtons.YesNoCancel) = Windows.Forms.DialogResult.Yes
    '        'If mbr = DialogResult.Yes Then
    '        '    oHelper.bCCLeague = True
    '        '    CheckPSDate = True
    '        'ElseIf mbr = DialogResult.No Then
    '        '    oHelper.bCCLeague = False
    '        '    CheckPSDate = False
    '        'Else Exit Function
    '        'End If
    '    Else
    '        oHelper.bCCLeague = False
    '    End If
    'End Sub

    'Private Sub btnPostSeason_Click(sender As Object, e As EventArgs)
    '    gbPS.Visible = True
    'End Sub

    Private Sub dtRSEnd_ValueChanged(sender As Object, e As EventArgs) Handles dtRSEnd.ValueChanged
        oHelper.rLeagueParmrow("EndDate") = dtRSEnd.Text
    End Sub

    Private Sub btnUndoScores_Click(sender As Object, e As EventArgs) Handles btnUndoScores.Click
        Dim sfile = oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd_hhmmss") & "_LeagueFiles.zip"
        With lblProcessMsg
            .Text = String.Format("Gathering Scores file to Undo")
            oHelper.status_Msg(lblProcessMsg, Me)
            Dim oFiles() As IO.FileInfo
            Dim oDirectory As New IO.DirectoryInfo(oHelper.sFilePath)
            'oFiles = oDirectory.GetFiles("*.xml")
            oFiles = oDirectory.GetFiles("*Scores.csv")
            'sort by date desc
            Helper.arraySort(oFiles)

            Dim oFiles2() As IO.FileInfo
            oFiles2 = oDirectory.GetFiles("*Payments.csv")
            'sort by date desc
            Helper.arraySort(oFiles2)

            Dim mbr = MsgBox(String.Format("Do you want to delete scores file from {0}?" & vbCrLf & String.Format("Files will be backed up before delete to {0}", sfile), oFiles(0).FullName), MsgBoxStyle.YesNo)
            If mbr = MsgBoxResult.Yes Then
                .Text = String.Format("Zipping League Files to {0} ...", sfile)
                oHelper.status_Msg(lblProcessMsg, Me)
                Zipit()
                If IO.File.Exists((oFiles(0).FullName)) Then IO.File.Delete(oFiles(0).FullName)
                If IO.File.Exists((oFiles2(0).FullName)) Then IO.File.Delete(oFiles2(0).FullName)
                newBuildTablesForLeague()
            End If
            .Text = String.Format("Finished Undoing Scores")
            oHelper.status_Msg(lblProcessMsg, Me)
        End With
    End Sub

    Private Sub btnPayments_Click(sender As Object, e As EventArgs) Handles btnPayments.Click
        Payments.Show()
    End Sub

    Private Sub btnSchedule_Click(sender As Object, e As EventArgs) Handles btnSchedule.Click
        ScheduleBuilder.Show()
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

    Private Sub dtScore_DropDown(sender As Object, e As EventArgs) Handles dtScore.DropDown
        RemoveHandler dtScore.MouseUp, AddressOf dtScore_MouseUp
    End Sub

    Private Sub dtScore_CloseUp(sender As Object, e As EventArgs) Handles dtScore.CloseUp
        AddHandler dtScore.MouseUp, AddressOf dtScore_MouseUp
        CheckDTPSelection(dtScore)
    End Sub

    Private Sub dtScore_KeyUp(sender As Object, e As KeyEventArgs) Handles dtScore.KeyUp
        If e.KeyValue = Keys.Left OrElse e.KeyValue = Keys.Right Then
            CheckDTPSelection(dtScore)
        End If
    End Sub
    Private Sub dtScore_MouseUp(sender As Object, e As MouseEventArgs) Handles dtScore.MouseUp
        CheckDTPSelection(dtScore)
    End Sub

    Private Sub dtScore_ValueChanged(sender As Object, e As EventArgs) Handles dtScore.ValueChanged
        Dim dtp As DateTimePicker = DirectCast(sender, DateTimePicker)

        RefreshSelection(dtp)

    End Sub

    'Private Sub Btn_WhatsSelected_Click(sender As Object, e As EventArgs) Handles Btn_WhatsSelected.Click
    '    'Show the current selected value in a MessageBox
    '    MessageBox.Show(currSelected.ToString())
    'End Sub
    'Private Sub dtScore_ValueChanged(sender As Object, e As EventArgs) Handles dtScore.ValueChanged
    Sub fixdate()
        If bload Then Exit Sub
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
        For Each row In dtschedule.Rows
            If CDate(dtScore.Text).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) = row("Date") Then
                oHelper.sDateLastScore = CDate(dtScore.Text).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                Exit For
            End If
        Next
        If oHelper.sDateLastScore = "" Then
            MsgBox("this date is not on the schedule, pick another")
            oHelper.sDateLastScore = solddate
        End If
        'oHelper.dDate = dtScore.Text
    End Sub

    Private Sub BtnCheckEmail_Click(sender As Object, e As EventArgs) Handles btnEmailAnnouncement.Click
        EmailMessage.Show()
    End Sub

    Private Sub BtnStandingsSnapshot_Click(sender As Object, e As EventArgs) Handles btnStandingsSnapshot.Click
        'Dim dt() = oHelper.dsLeague.Tables("dtScores").Select("Date < '20190813' and Points > 0")
        'Dim dv As New DataView(oHelper.dsLeague.Tables("dtScores"))
        'dv.RowFilter = "Date < '20190813' and Points > 0"
        'Dim dt As DataTable = dv.ToTable("Player,Team,Points,Team_Points")
        StandingsSnapshot.Show()
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


