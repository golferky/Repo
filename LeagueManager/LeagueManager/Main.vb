'*************************************************************************************************
Imports System.IO.Packaging
Public Class Main
    Dim cVersion = "Version : 2018.10.04"
    Public oHelper As Helper
    'Public owa As WA
    Dim bLoad = True
    'Private Teams As ArrayList = New ArrayList
    Private Teams2 As ArrayList = New ArrayList
    Dim rs As New Resizer

    Private Sub Main_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Genschedule()
        Me.Text = Me.Text & " " & cVersion
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
        oHelper = New Helper
        Dim sDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        '12/19/2017 note this is removed for now 
        Me.Icon = New Icon(Me.Icon, New Size(Me.Icon.Width * 5, Me.Icon.Height * 5))
        oHelper.bloghelper = True

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
            .SelectedPath = "G:\LeagueManager\Files",
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
            '20181017  put in due to incomplete scores < 2018 and player lookup needs to change for those scores also
            If row("Startdate").year < "2018" Then Continue For
            cbLeagues.Items.Add(row("Name") & " (" & row("StartDate").year & ")")
        Next

        cbLeagues.SelectedIndex = 0

        bLoad = False
        GetLeague()

        'setup email function
        oHelper.GGmail = New GGSMTP_GMAIL(oHelper.rLeagueParmrow("Email"), oHelper.rLeagueParmrow("EmailPassword"))
        'GetXSDNameByFileName(oHelper.dsLeague.Tables("dtScores"),
        oHelper.MyCourse = oHelper.dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")
        ''get the date of the schedule for this week
        ''just use the Column names which have dates of the schedule table
        ''this loop will compare the league start date and flip the hole marker based on front/back
        lblProcessMsg.Text = "Finished Loading League Tables"
        oHelper.status_Msg(lblProcessMsg, Me)
        oHelper.bloghelper = False

        'debugging playerstats, uncomment below
        'PlayerStats_Click(sender, e)
    End Sub

    Sub BuildTablesForLeague()
        'this subroutine will load the files related to the league/year that resides in the dropdown selected item 
        Dim sLeagueName = cbLeagues.SelectedItem

        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(oHelper.sFilePath)
        'oFiles = oDirectory.GetFiles("*.xml")
        oFiles = oDirectory.GetFiles("*.csv")
        'sort by date desc
        oHelper.arraySort(oFiles)
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
                oHelper.LOGIT(String.Format("File used : {0}", sfile.FullName))
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

        If oHelper.dsLeague.Tables.Contains("dtScores") Then oHelper.dsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtScores").Columns("Player"), oHelper.dsLeague.Tables("dtScores").Columns("Date")}
        'Dim x = oHelper.dsLeague.Tables("dtPlayers").Select name from Table A Group By name having count(*) > 1"
        '20180126-only build dates for the last date of completed scores
        Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
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


        If oHelper.dsLeague.Tables.Contains("dtPlayers") Then oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}

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
            Dim reformatted As String = wkdate.ToString("MM/dd/yyyy", Globalization.CultureInfo.InvariantCulture)
            Dim sLeagueName = row("Name") & " (" & row("Startdate").year & ")"
            If sLeagueName = cbLeagues.SelectedItem Then
                'save row of league so all routines can use
                With oHelper
                    .rLeagueParmrow = row
                    .sLeagueName = .rLeagueParmrow("Name")
                    If .rLeagueParmrow("ScoresLocked") Is DBNull.Value Then .rLeagueParmrow("ScoresLocked") = "N"
                    BuildTablesForLeague()
                    '20180930 -setup dates
                    dtScore.Text = DateTime.ParseExact(.sDateLastScore, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0")
                    Dim iweeks As Integer = (((.rLeagueParmrow("Teams") - 1) * 2) - 1)
                    dtRSStart.Text = .rLeagueParmrow("StartDate")
                    dtRSEnd.Text = CDate(.rLeagueParmrow("StartDate")).AddDays(iweeks * 7)
                    dtPSStart.Text = CDate(dtRSEnd.Text).AddDays(7)
                    tbPSEnd.Text = CDate(dtPSStart.Text).AddDays(7)
                    .rLeagueParmrow("EndDate") = dtRSEnd.Text
                    .rLeagueParmrow("PostSeasonDt") = dtPSStart.Text
                    If dtPSStart.Text = "01/01/1900" Then
                        gbPS.Visible = False
                    Else
                        gbPS.Visible = True
                    End If
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
            oHelper.status_Msg(lblProcessMsg, Me)
            If Not bLoad Then GetLeague()
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
        oHelper.arraySort(oFiles)
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

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Finance.Show()
    End Sub

    Private Sub dtPSStart_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles dtPSStart.ValueChanged

        If CheckPSDate() Then
            oHelper.rLeagueParmrow("PostSeasonDt") = dtPSStart.Text
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
            tbPSEnd.Text = CDate(dtPSStart.Text).AddDays(7)
            gbPS.Visible = True
        Else
            dtPSStart.Text = "1/1/1900"
            gbPS.Visible = False
        End If
    End Sub
    '20181016 below remove handlers and add handlers were added because datetime picker changed the date every time we click the arrow keys
    Private Sub dtPSStart_DropDown(ByVal sender As Object, ByVal e As EventArgs) Handles dtPSStart.DropDown
        RemoveHandler dtPSStart.ValueChanged, AddressOf dtPSStart_ValueChanged
    End Sub

    Private Sub dtPSStart_CloseUp(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtPSStart.CloseUp
        AddHandler dtPSStart.ValueChanged, AddressOf dtPSStart_ValueChanged
        Call dtPSStart_ValueChanged(sender, EventArgs.Empty)
    End Sub
    Function CheckPSDate() As Boolean
        If dtPSStart.Text = "1/1/1900" Then Exit Function
        'change mm/dd/yyyy to yyyymmdd
        Dim wkdate As Date = dtRSEnd.Text
        Dim reformatted1 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        wkdate = dtPSStart.Text
        Dim reformatted2 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        If reformatted1 >= reformatted2 Then
            oHelper.bCCLeague = True
            'If dtRSEnd.Text >= dtPSStart.Text Then
            Dim smsg = String.Format("Regular Season is played the same time as your League Championship" & vbCrLf & vbCrLf & "Regular Season {0}" & vbCrLf & "League Championship {1}", dtRSEnd.Text, dtPSStart.Text)
            MessageBox.Show(smsg, "Warning Dates Overlap", MessageBoxButtons.OK)
            'Dim mbr As DialogResult = MessageBox.Show(smsg, "Warning Dates Overlap", MessageBoxButtons.YesNoCancel) = Windows.Forms.DialogResult.Yes
            'If mbr = DialogResult.Yes Then
            '    oHelper.bCCLeague = True
            '    CheckPSDate = True
            'ElseIf mbr = DialogResult.No Then
            '    oHelper.bCCLeague = False
            '    CheckPSDate = False
            'Else Exit Function
            'End If
        Else
            oHelper.bCCLeague = False
            CheckPSDate = True
        End If
    End Function

    Private Sub btnPostSeason_Click(sender As Object, e As EventArgs)
        gbPS.Visible = True
    End Sub

    Private Sub dtScore_ValueChanged(sender As Object, e As EventArgs) Handles dtScore.ValueChanged
        oHelper.sDateLastScore = CDate(dtScore.Text).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        'oHelper.dDate = dtScore.Text
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


