'*************************************************************************************************
Imports System.IO.Packaging
Imports LeagueManager.FileLayout

Public Class Main
    Dim cVersion = "Version : 2019.12.16"
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
    Dim toolTipHdcp As New ToolTip
    Dim schanges As String = ""

    Private Sub Main_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            oHelper = New Helper
            Dim allScreens = Screen.AllScreens
            Dim Current_Screen As Screen = Screen.FromControl(Me)
            If Current_Screen.Primary Then
                Dim HCenter = Current_Screen.Bounds.Left +
            (((Current_Screen.Bounds.Right - Current_Screen.Bounds.Left) / 2) - ((Me.Width) / 2))
                Dim VCenter = (Current_Screen.Bounds.Bottom / 2) - ((Me.Height) / 2)
                Me.StartPosition = FormStartPosition.Manual
                Me.Location = New Point(HCenter, VCenter)
            Else
                Me.StartPosition = FormStartPosition.CenterScreen
            End If
            'tooltip if needed
            'toolTipHdcp = oHelper.toolTipHdcp
            'toolTipHdcp.AutoPopDelay = 5000
            'toolTipHdcp.InitialDelay = 10
            'toolTipHdcp.ReshowDelay = 500
            '' Force the ToolTip text to be displayed whether or not the form is active.
            'toolTipHdcp.ShowAlways = True
            '' Set up the ToolTip text for the datagridviewcell.
            'toolTipHdcp.SetToolTip(Me, String.Format("Changes: {0}{1}", vbCrLf, schanges))

            'Me.Location = Point.Add(Screen.PrimaryScreen.Bounds.Location, New Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            'Dim primaryScreen = Screen.PrimaryScreen
            'Dim allScreens = Screen.AllScreens

            'For Each screen In allScreens
            '    Dim location = Point.Add(screen.Bounds.Location, New Size(100, 100))
            '    Dim text = screen.DeviceName

            '    If screen Is primaryScreen Then
            '        text &= " (Primary)"
            '    End If

            '    Using dialogue As New Form With {.Text = text,
            '                         .StartPosition = FormStartPosition.Manual,
            '                         .Location = location}
            '        dialogue.ShowDialog()
            '    End Using
            'Next
            Me.Enabled = False
            Me.Show()
            'Genschedule()  TabControl2.TabPages.Add("Test")
            'Dim tp = TabControl2.TabPages(TabControl2.TabPages.Count - 1)
            'Dim b = New Button()
            'b.Text = "My Button"
            'tp.Controls.Add(b)
            'AddHandler b.Click, AddressOf MyButton_Click

            Application.EnableVisualStyles()

            lblProcessMsg.Text = "Loading League Tables..."
            oHelper.status_Msg(lblProcessMsg, Me)
            iScreenWidth = Screen.PrimaryScreen.Bounds.Width
            'greg test
            'iScreenWidth = 1368
            iScreenHeight = Screen.PrimaryScreen.Bounds.Height
            If Debugger.IsAttached Then Me.Text &= " - Debug Mode"

            '20180106 remove for now
            'btnShowScores.Visible = False

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

            lblProcessMsg.Text = String.Format("Loading Changelog from {0},", oHelper.sFilePath)
            oHelper.status_Msg(lblProcessMsg, Me)
            Using sr As New IO.StreamReader(oHelper.sFilePath & "\Changelog.txt", False)
                schanges &= sr.ReadToEnd
            End Using
            'schanges &= vbCrLf & "Change font on schedulebuilder grid"
            'schanges &= vbCrLf & "Payments Screen-loading(top line missing)"
            'schanges &= vbCrLf & "Build Schedule change for Byes"
            'schanges &= vbCrLf & "Fixed issue with blank Scores on Last 5"

            lblProcessMsg.Text = String.Format("Finished Loading Changelog from {0},", oHelper.sFilePath)
            oHelper.status_Msg(lblProcessMsg, Me)

            Me.Text = Me.Text & " " & String.Format("{0}", cVersion)

            lbMonitor.Text = String.Format("{0}, Resolution {1} x {2}, Menu {3} x {4}", My.Computer.Name, iScreenWidth, iScreenHeight, Me.Width, Me.Height)
            oHelper.LOGIT(Me.Text)

            txtFolder.Text = oHelper.sFilePath

            'build dropdown list of leagues
            ' DsLeague = New dsLeague
            'If Not oHelper.CSV2DataTable(dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv")) Then
            '    MsgBox(String.Format("File in use, close file and restart {0} {1}", vbCrLf, oHelper.getLatestFile("*LeagueParms.csv")))
            '    End
            'End If
            bwait = True
            sParmFile = oHelper.getLatestFile("*LeagueParms.csv")
            oHelper.LOGIT(String.Format("Loaded league file {0}", sParmFile))
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
            'Scores
            Dim ofiles As String = "Scores,Payments,Players,Courses"
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

                Dim sfileparts As String() = sLatestFile.ToString.Split("\")
                Dim sfilename As String = sfileparts(UBound(sfileparts))
                Dim sfile = "dt"
                If sfilename.Contains("_") Then
                    sfileparts = sfilename.Split("_")
                    sfilename = sfileparts(UBound(sfileparts))
                End If
                sfile &= sfilename.Split(".")(0)

                oHelper.LOGIT(String.Format("Checking {0}", file))
                oHelper.LOGIT(String.Format("Keeping File {0}", sLatestFile.ToString))
                oHelper.LOGIT(String.Format("File name {0}", sLatestFile))
                oHelper.LOGIT(String.Format("Removing table {0}", sfile))
                If file = "Scores" Then lbScoresFile.Text = sfilename
                Dim dt As DataTable = dsLeague.Tables(sfile)
                If sfile.Contains("Payments") Then
                    dt.PrimaryKey = New DataColumn() {dt.Columns("Player"), dt.Columns("Date"), dt.Columns("Desc"), dt.Columns("Detail")}
                    oHelper.LOGIT("Payments file " & sfile)
                End If
                dt.Rows.Clear()
                bwait = True
                Do While bwait
                    If Not oHelper.WaitForFile(dsLeague.Tables(sfile), sLatestFile, lblProcessMsg, Me) Then
                        'Dim mbr = MessageBox.Show(String.Format("File In use {0}Press <OK> To close file And proceed Or <Cancel>", vbCrLf, sParmFile), sParmFile, MessageBoxButtons.OKCancel)
                        'If mbr = DialogResult.Cancel Then
                        '    End
                        'End If
                    Else
                        bwait = False
                    End If
                Loop
                tspb.ProgressBar.Value += 1
                tssl.Text = String.Format("Loading file {0} Of {1}", tspb.ProgressBar.Value, ofiles.Split(",").Count)
                'tspb.ProgressBar.Refresh()
                Application.DoEvents()
            Next

            et = Now - sStartTime
            If et.TotalMinutes >= 1 Then
                tssl.Text = String.Format("Loaded {0} files {1} elapsed time", ofiles.Split(",").Count, CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs")
            Else
                tssl.Text = String.Format("Loaded {0} files {1} elapsed time", ofiles.Split(",").Count, CInt(et.TotalSeconds) Mod 60 & " Secs")
            End If
            ''get the date of the schedule for this week
            ''just use the Column names which have dates of the schedule table
            ''this loop will compare the league start date and flip the hole marker based on front/back

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

            bload = False
            lblProcessMsg.Text = "Finished Loading League Tables"
            oHelper.status_Msg(lblProcessMsg, Me)
            'oHelper.bloghelper = False
            oHelper.iHoles = oHelper.rLeagueParmrow("Holes")
            Me.Enabled = True
        Catch ex As InvalidCastException
            Dim iexc = ""
        End Try
    End Sub
    Function WaitForFile(ByVal file As String) As String
        'If file = "Schedule" Then
        '    WaitForFile = oHelper.getLatestFile(String.Format("*{0}Schedule.csv", file))
        'Else
        '    WaitForFile = oHelper.getLatestFile(String.Format("*{0}.csv", file))
        'End If
        WaitForFile = oHelper.getLatestFile(String.Format("*{0}.csv", file))

        If WaitForFile = "" Then
            Dim mbr = MessageBox.Show(String.Format("File {1} missing{0} Press <OK> To move file And proceed Or <Cancel>", vbCrLf, file), file, MessageBoxButtons.OKCancel)
            If mbr = DialogResult.Cancel Then End
            Dim i = 30
            'MsgBox(String.Format("File {0} Is In use, will wait up For {1} seconds To free up", file, i))
            Do Until i = 0
                lblProcessMsg.Text = String.Format("Waiting For file {0} For {1} seconds", file, i)
                oHelper.status_Msg(lblProcessMsg, Me)
                Threading.Thread.Sleep(1000)
                i -= 1
            Loop
            lblProcessMsg.Text = String.Format("Finished Waiting For file {0}", file)
            oHelper.status_Msg(lblProcessMsg, Me)
        End If

    End Function
    Sub BuildTablesForLeague()

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
            'Dim wkdate As Date = row("startDate")
            'Dim reformatted As String = wkdate.ToString("MM/dd/yyyy", Globalization.CultureInfo.InvariantCulture)
            Dim sLeagueName = row("Name") & " (" & row("Startdate").year & ")"
            If sLeagueName = cbLeagues.SelectedItem Then
                'save row of league so all routines can use
                With oHelper
                    .rLeagueParmrow = row
                    .sLeagueName = .rLeagueParmrow("Name")
                    oHelper.UpdateINI()
                    BuildTablesForLeague()
                    If row("StartDate").year <> oHelper.dDate.Year Then oHelper.dDate = row("StartDate")
                    Exit For
                End With
            End If
        Next

    End Sub

    Private Sub ScoreCard_Click(sender As System.Object, e As System.EventArgs) Handles btnScoreCard.Click
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show("Scores < 2018 are not fully entered, cannot view yet,check with developer")
            Exit Sub
        End If

        'If oHelper.bsch Then
        lblProcessMsg.Text = String.Format("Loading Scores from {0}", lbScoresFile.Text)
        oHelper.status_Msg(lblProcessMsg, Me)

        frmScoreCard.Show()

        lblProcessMsg.Text = String.Format("Finished Loading Scores")
        oHelper.status_Msg(lblProcessMsg, Me)

        'Else
        '    MsgBox("Schedule not available, cant do Scores")
        'End If

    End Sub

    Private Sub EnterScores_Click(sender As Object, e As EventArgs)
        frmChooseScores.Show()
    End Sub
    Private Sub btnLast5_Click(sender As Object, e As EventArgs) Handles btnLast5.Click
        LastFive.Show()
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        '20180306-added checkbox for email backup option
        oHelper.DataTable2CSV(dsLeague.dtLeagueParms, oHelper.sFilePath & "\LeagueParms.csv")
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
            MessageBox.Show(Me, "Scores < 2018 are not fully entered, cannot view yet,check with developer", MessageBoxIcon.Hand)
            Exit Sub
        End If
        'If oHelper.bsch Then
        Standings.Show()
        'Else
        '    MsgBox("Schedule not available, cant do Standings")
        'End If

    End Sub

    Private Sub btnMatches_Click(sender As Object, e As EventArgs) Handles btnMatches.Click
        If cbLeagues.SelectedItem.ToString.Substring(cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) < "2018" Then
            MessageBox.Show(Me, "Scores < 2018 are not fully entered, cannot view yet,check with developer", MessageBoxIcon.Hand)
            Exit Sub
        End If
        If cbDates.SelectedItem > oHelper.sDateLastScore Then
            MessageBox.Show(String.Format("Selected Score {0} cant be greater than the last entered score for matches {1}", cbDates.SelectedItem, oHelper.sDateLastScore),
                            "Critical Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            Exit Sub
        End If

        'Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
        'dvscores.RowFilter = String.Format("Date = '{0}'", cbDates.SelectedItem)
        'Dim iscores As Integer = 0
        'For Each srow As DataRowView In dvscores
        '    If srow("Hole1") IsNot DBNull.Value Then
        '        If IsNumeric(srow("Hole1")) Then iscores += 1
        '    ElseIf srow("Hole10") IsNot DBNull.Value Then
        '        If IsNumeric(srow("Hole10")) Then iscores += 1
        '    End If
        'Next
        'If iscores <> oHelper.rLeagueParmrow("Teams") * 2 Then
        '    MessageBox.Show(String.Format("All {0} Scores werent entered, Cant Calculate Matches until all {0} scores are entered", oHelper.rLeagueParmrow("Teams") * 2),
        '                    "Warning-cant proceed",
        '                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        '    Exit Sub
        'End If
        'If oHelper.bsch Then
        Matches.Show()
        '    Else
        '        MsgBox("Schedule not available, cant do Matches")
        '    End If
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
        'If oHelper.bsch Then

        Skins.Show()
        If oHelper.iHoleMarker = 0 Then Skins.Close()
        'Else
        '    MsgBox("Schedule not available, cant do Skins")
        'End If
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
        lbMonitor.Text = String.Format("{0}, Resolution {1} x {2}, Menu {3} x {4}", My.Computer.Name, iScreenWidth, iScreenHeight, Me.Width, Me.Height)
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

        GetLeague()
        'GetXSDNameByFileName(oHelper.dsLeague.Tables("dtScores"),
        oHelper.dsLeague = dsLeague
        'setup email function
        oHelper.GGmail = New GGSMTP_GMAIL(oHelper.rLeagueParmrow("Email"), oHelper.rLeagueParmrow("EmailPassword"))

        Dim wkrow As DataRow
        If dsLeague.Tables.Contains("dtschedule") Then
            oHelper.LOGIT(String.Format("dsleague.tables contains table dtschedule {0}", "dtSchedule"))
            Try
                dsLeague.Tables.Remove("dtSchedule")
            Catch ex As Exception

            End Try
        End If

        Dim dtschedule As New DataTable()
        'build a table of schedule with dates in rows instead of columns
        dtschedule = oHelper.buildSchedule()
        wkrow = dtschedule.NewRow
        wkrow("Date") = CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("MM/dd/yyyy")
        dtschedule.Rows.Add(wkrow)
        wkrow = dtschedule.NewRow
        wkrow("Date") = CDate(oHelper.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("MM/dd/yyyy")
        dtschedule.Rows.Add(wkrow)
        oHelper.dsLeague.Tables.Add(dtschedule)
        'reformat dates into yyyymmdd format
        Dim x = oHelper.dsLeague.Tables.Count

        oHelper.dsLeague.Tables(oHelper.dsLeague.Tables.Count - 1).TableName = "dtSchedule"

        For Each row In dtschedule.Rows
            row("Date") = CDate(row("Date")).ToString("yyyyMMdd")
        Next
        rebuildDates(dtschedule)
    End Sub
    Sub rebuildDates(dtschedule As DataTable)
        Dim dv As New DataView(dtschedule)
        dv.Sort = "Date desc"
        cbDates.Items.Clear()
        For Each row In dv
            cbDates.Items.Add(row("Date"))
        Next

        If oHelper.dDate.ToString("yyyyMMdd").Substring(0, 4) > cbDates.Items(0).ToString.Substring(0, 4) Then
            oHelper.dDate = Date.ParseExact(cbDates.Items(0), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        End If
        'remove non-match dates from dates combobox
        'cdateToyyyyMMdd converts a string from 1/1/1900 to 19000101
        Do While cbDates.Items(0) > oHelper.sDateLastScore
            cbDates.Items.Remove(cbDates.Items(0))
        Loop

        cbDates.SelectedIndex = 0
        cbDates.SelectedItem = oHelper.dDate.ToString("yyyyMMdd")

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
            'Else
            '    End
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

    Private Sub btnUndoScores_Click(sender As Object, e As EventArgs) Handles btnUndoScores.Click
        Dim dvscores As New DataView(dsLeague.dtScores)
        dvscores.RowFilter = String.Format("Date <> '{0}'", cbDates.SelectedItem)
        With lblProcessMsg
            .Text = String.Format("Gathering Scores file to Undo")
            oHelper.status_Msg(lblProcessMsg, Me)

            'Dim sfile As String = oHelper.sFilePath & "\Backup" & Now.ToString("yyyyMMdd hhMMss") & "_Scores.csv"
            Dim sfile As String = oHelper.sFilePath & String.Format("\Backup\{0}_Scores.csv", Now.ToString("yyyyMMdd_hhMMss"))
            Dim mbr = MsgBox(String.Format("Do you want to delete scores from {0}?" & vbCrLf & String.Format("Files will be backed up before delete to {0}", sfile), cbDates.SelectedItem), MsgBoxStyle.YesNo)
            If mbr = MsgBoxResult.Yes Then
                .Text = String.Format("creating Scores File Backup to {0} ...", sfile)
                oHelper.status_Msg(lblProcessMsg, Me)
                If IO.File.Exists((sfile)) Then IO.File.Delete(sfile)
                oHelper.DataTable2CSV(dsLeague.dtScores, sfile)
                Dim rows = dsLeague.dtScores.Select(String.Format("Date = '{0}' ", cbDates.SelectedItem))
                For Each score In rows
                    score.Delete()
                Next
                oHelper.DataTable2CSV(dsLeague.dtScores, oHelper.sFilePath & "\Scores.csv")

                BuildTablesForLeague()
            End If
            .Text = String.Format("Finished Undoing Scores")
            oHelper.status_Msg(lblProcessMsg, Me)
        End With
    End Sub
    Private Sub btnPayments_Click(sender As Object, e As EventArgs) Handles btnPayments.Click
        Payments.Show()
    End Sub

    Private Sub btnSchedule_Click(sender As Object, e As EventArgs)
        ScheduleBuilder.Show()
    End Sub

    'Private Sub Btn_WhatsSelected_Click(sender As Object, e As EventArgs) Handles Btn_WhatsSelected.Click
    '    'Show the current selected value in a MessageBox
    '    MessageBox.Show(currSelected.ToString())
    'End Sub
    'Private Sub dtScore_ValueChanged(sender As Object, e As EventArgs) Handles dtScore.ValueChanged

    Private Sub BtnCheckEmail_Click(sender As Object, e As EventArgs) Handles btnEmailAnnouncement.Click
        EmailMessage.Show()
    End Sub

    Private Sub BtnStandingsSnapshot_Click(sender As Object, e As EventArgs) Handles btnStandingsSnapshot.Click
        StandingsSnapshot.Show()
    End Sub

    Private Sub btnSetup_Click(sender As Object, e As EventArgs) Handles btnSetup.Click
        Setup.Show()
    End Sub

    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        oHelper.dDate = Date.ParseExact(cbDates.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    End Sub

    Private Sub btnPlayerStats_Click(sender As Object, e As EventArgs) Handles btnPlayerStats.Click
        oHelper.sPlayer = ""
        frmPlayerStats.Show()
    End Sub

    Private Sub btnSkinGame_Click(sender As Object, e As EventArgs)
        frmSkins.Show()
    End Sub

    Private Sub btnEnterScores_Click(sender As Object, e As EventArgs)
        frmEnterScore.Show()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox(schanges, MsgBoxStyle.Information, "Change Log")
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


