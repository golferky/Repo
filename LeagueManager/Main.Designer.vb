<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.btnLeagueSetup = New System.Windows.Forms.Button()
        Me.btnCourseSetup = New System.Windows.Forms.Button()
        Me.btnPlayerSetup = New System.Windows.Forms.Button()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnPayments = New System.Windows.Forms.Button()
        Me.btnMatches = New System.Windows.Forms.Button()
        Me.btnSkins = New System.Windows.Forms.Button()
        Me.btnScoreCard = New System.Windows.Forms.Button()
        Me.btnSkinsRpt = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnLast5 = New System.Windows.Forms.Button()
        Me.btnPlayerStats = New System.Windows.Forms.Button()
        Me.btnStandings = New System.Windows.Forms.Button()
        Me.btnShowScores = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnUndoScores = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.txtFolder = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btnEmailAnnouncement = New System.Windows.Forms.Button()
        Me.btnZip = New System.Windows.Forms.Button()
        Me.btnSchedule = New System.Windows.Forms.Button()
        Me.cbLeagues = New System.Windows.Forms.ComboBox()
        Me.lbScoresFile = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnChangeFolder = New System.Windows.Forms.Button()
        Me.cbLogging = New System.Windows.Forms.CheckBox()
        Me.cbMail = New System.Windows.Forms.CheckBox()
        Me.dtScore = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.tbPSEnd = New System.Windows.Forms.TextBox()
        Me.tbPSStart = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.cbScoreDt = New System.Windows.Forms.ComboBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.dtRSStart = New System.Windows.Forms.DateTimePicker()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.dtRSEnd = New System.Windows.Forms.DateTimePicker()
        Me.lblProcessMsg = New System.Windows.Forms.Label()
        Me.gbControls = New System.Windows.Forms.GroupBox()
        Me.cbDebug = New System.Windows.Forms.CheckBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.tspb = New System.Windows.Forms.ToolStripProgressBar()
        Me.tssl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.btnStandingsSnapshot = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.gbControls.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnLeagueSetup
        '
        Me.btnLeagueSetup.Location = New System.Drawing.Point(20, 31)
        Me.btnLeagueSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnLeagueSetup.Name = "btnLeagueSetup"
        Me.btnLeagueSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnLeagueSetup.TabIndex = 4
        Me.btnLeagueSetup.Text = "League Setup"
        Me.btnLeagueSetup.UseVisualStyleBackColor = True
        '
        'btnCourseSetup
        '
        Me.btnCourseSetup.Location = New System.Drawing.Point(176, 31)
        Me.btnCourseSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnCourseSetup.Name = "btnCourseSetup"
        Me.btnCourseSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnCourseSetup.TabIndex = 6
        Me.btnCourseSetup.Text = "Course Setup"
        Me.btnCourseSetup.UseVisualStyleBackColor = True
        '
        'btnPlayerSetup
        '
        Me.btnPlayerSetup.Location = New System.Drawing.Point(176, 81)
        Me.btnPlayerSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPlayerSetup.Name = "btnPlayerSetup"
        Me.btnPlayerSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnPlayerSetup.TabIndex = 7
        Me.btnPlayerSetup.Text = "Player Setup"
        Me.btnPlayerSetup.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(184, 81)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(131, 24)
        Me.btnExit.TabIndex = 8
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnPayments)
        Me.GroupBox1.Controls.Add(Me.btnMatches)
        Me.GroupBox1.Controls.Add(Me.btnSkins)
        Me.GroupBox1.Controls.Add(Me.btnScoreCard)
        Me.GroupBox1.Location = New System.Drawing.Point(42, 98)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(325, 119)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scoring and Games"
        '
        'btnPayments
        '
        Me.btnPayments.Location = New System.Drawing.Point(20, 73)
        Me.btnPayments.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPayments.Name = "btnPayments"
        Me.btnPayments.Size = New System.Drawing.Size(131, 24)
        Me.btnPayments.TabIndex = 13
        Me.btnPayments.Text = "Payments"
        Me.btnPayments.UseVisualStyleBackColor = True
        '
        'btnMatches
        '
        Me.btnMatches.Location = New System.Drawing.Point(176, 26)
        Me.btnMatches.Margin = New System.Windows.Forms.Padding(2)
        Me.btnMatches.Name = "btnMatches"
        Me.btnMatches.Size = New System.Drawing.Size(131, 24)
        Me.btnMatches.TabIndex = 8
        Me.btnMatches.Text = "Matches"
        Me.btnMatches.UseVisualStyleBackColor = True
        '
        'btnSkins
        '
        Me.btnSkins.Location = New System.Drawing.Point(176, 73)
        Me.btnSkins.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSkins.Name = "btnSkins"
        Me.btnSkins.Size = New System.Drawing.Size(131, 24)
        Me.btnSkins.TabIndex = 7
        Me.btnSkins.Text = "Skins"
        Me.btnSkins.UseVisualStyleBackColor = True
        '
        'btnScoreCard
        '
        Me.btnScoreCard.Location = New System.Drawing.Point(20, 26)
        Me.btnScoreCard.Margin = New System.Windows.Forms.Padding(2)
        Me.btnScoreCard.Name = "btnScoreCard"
        Me.btnScoreCard.Size = New System.Drawing.Size(131, 24)
        Me.btnScoreCard.TabIndex = 4
        Me.btnScoreCard.Text = "Enter Scores"
        Me.btnScoreCard.UseVisualStyleBackColor = True
        '
        'btnSkinsRpt
        '
        Me.btnSkinsRpt.Location = New System.Drawing.Point(176, 97)
        Me.btnSkinsRpt.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSkinsRpt.Name = "btnSkinsRpt"
        Me.btnSkinsRpt.Size = New System.Drawing.Size(131, 24)
        Me.btnSkinsRpt.TabIndex = 12
        Me.btnSkinsRpt.Text = "Skins Report-YTD"
        Me.btnSkinsRpt.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(174, 59)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(131, 24)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Financial"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnLast5
        '
        Me.btnLast5.Location = New System.Drawing.Point(21, 97)
        Me.btnLast5.Margin = New System.Windows.Forms.Padding(2)
        Me.btnLast5.Name = "btnLast5"
        Me.btnLast5.Size = New System.Drawing.Size(131, 24)
        Me.btnLast5.TabIndex = 10
        Me.btnLast5.Text = "Last 5 Scores"
        Me.btnLast5.UseVisualStyleBackColor = True
        '
        'btnPlayerStats
        '
        Me.btnPlayerStats.Location = New System.Drawing.Point(174, 23)
        Me.btnPlayerStats.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPlayerStats.Name = "btnPlayerStats"
        Me.btnPlayerStats.Size = New System.Drawing.Size(131, 24)
        Me.btnPlayerStats.TabIndex = 9
        Me.btnPlayerStats.Text = " Player Stats"
        Me.btnPlayerStats.UseVisualStyleBackColor = True
        '
        'btnStandings
        '
        Me.btnStandings.Location = New System.Drawing.Point(20, 23)
        Me.btnStandings.Margin = New System.Windows.Forms.Padding(2)
        Me.btnStandings.Name = "btnStandings"
        Me.btnStandings.Size = New System.Drawing.Size(131, 24)
        Me.btnStandings.TabIndex = 5
        Me.btnStandings.Text = "Standings"
        Me.btnStandings.UseVisualStyleBackColor = True
        '
        'btnShowScores
        '
        Me.btnShowScores.Location = New System.Drawing.Point(11, 131)
        Me.btnShowScores.Margin = New System.Windows.Forms.Padding(2)
        Me.btnShowScores.Name = "btnShowScores"
        Me.btnShowScores.Size = New System.Drawing.Size(131, 24)
        Me.btnShowScores.TabIndex = 6
        Me.btnShowScores.Text = "Show Scores"
        Me.btnShowScores.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnUndoScores)
        Me.GroupBox2.Controls.Add(Me.btnLeagueSetup)
        Me.GroupBox2.Controls.Add(Me.btnCourseSetup)
        Me.GroupBox2.Controls.Add(Me.btnPlayerSetup)
        Me.GroupBox2.Location = New System.Drawing.Point(42, 424)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Size = New System.Drawing.Size(325, 133)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Setup/Cleanup Functions"
        '
        'btnUndoScores
        '
        Me.btnUndoScores.Location = New System.Drawing.Point(20, 81)
        Me.btnUndoScores.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUndoScores.Name = "btnUndoScores"
        Me.btnUndoScores.Size = New System.Drawing.Size(131, 24)
        Me.btnUndoScores.TabIndex = 8
        Me.btnUndoScores.Text = "Undo Scores"
        Me.btnUndoScores.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'txtFolder
        '
        Me.txtFolder.Location = New System.Drawing.Point(8, 35)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(307, 20)
        Me.txtFolder.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(46, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "League Name"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.btnEmailAnnouncement)
        Me.GroupBox3.Controls.Add(Me.btnZip)
        Me.GroupBox3.Controls.Add(Me.btnSchedule)
        Me.GroupBox3.Controls.Add(Me.btnShowScores)
        Me.GroupBox3.Location = New System.Drawing.Point(390, 424)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(164, 298)
        Me.GroupBox3.TabIndex = 14
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Utilities"
        '
        'btnEmailAnnouncement
        '
        Me.btnEmailAnnouncement.Location = New System.Drawing.Point(9, 184)
        Me.btnEmailAnnouncement.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEmailAnnouncement.Name = "btnEmailAnnouncement"
        Me.btnEmailAnnouncement.Size = New System.Drawing.Size(131, 24)
        Me.btnEmailAnnouncement.TabIndex = 9
        Me.btnEmailAnnouncement.Text = "Email Message"
        Me.btnEmailAnnouncement.UseVisualStyleBackColor = True
        '
        'btnZip
        '
        Me.btnZip.Location = New System.Drawing.Point(9, 82)
        Me.btnZip.Margin = New System.Windows.Forms.Padding(2)
        Me.btnZip.Name = "btnZip"
        Me.btnZip.Size = New System.Drawing.Size(131, 24)
        Me.btnZip.TabIndex = 8
        Me.btnZip.Text = "Zip files"
        Me.btnZip.UseVisualStyleBackColor = True
        '
        'btnSchedule
        '
        Me.btnSchedule.Location = New System.Drawing.Point(11, 31)
        Me.btnSchedule.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSchedule.Name = "btnSchedule"
        Me.btnSchedule.Size = New System.Drawing.Size(131, 24)
        Me.btnSchedule.TabIndex = 7
        Me.btnSchedule.Text = "Schedule (TBD)"
        Me.btnSchedule.UseVisualStyleBackColor = True
        '
        'cbLeagues
        '
        Me.cbLeagues.FormattingEnabled = True
        Me.cbLeagues.Location = New System.Drawing.Point(49, 35)
        Me.cbLeagues.Name = "cbLeagues"
        Me.cbLeagues.Size = New System.Drawing.Size(131, 21)
        Me.cbLeagues.TabIndex = 15
        '
        'lbScoresFile
        '
        Me.lbScoresFile.AutoSize = True
        Me.lbScoresFile.Location = New System.Drawing.Point(4, 19)
        Me.lbScoresFile.Name = "lbScoresFile"
        Me.lbScoresFile.Size = New System.Drawing.Size(88, 13)
        Me.lbScoresFile.TabIndex = 17
        Me.lbScoresFile.Text = "Scores File name"
        '
        'btnChangeFolder
        '
        Me.btnChangeFolder.Location = New System.Drawing.Point(8, 81)
        Me.btnChangeFolder.Margin = New System.Windows.Forms.Padding(2)
        Me.btnChangeFolder.Name = "btnChangeFolder"
        Me.btnChangeFolder.Size = New System.Drawing.Size(131, 24)
        Me.btnChangeFolder.TabIndex = 19
        Me.btnChangeFolder.Text = "Change Folder"
        Me.btnChangeFolder.UseVisualStyleBackColor = True
        '
        'cbLogging
        '
        Me.cbLogging.AutoSize = True
        Me.cbLogging.Location = New System.Drawing.Point(8, 122)
        Me.cbLogging.Name = "cbLogging"
        Me.cbLogging.Size = New System.Drawing.Size(64, 17)
        Me.cbLogging.TabIndex = 13
        Me.cbLogging.Text = "Logging"
        Me.cbLogging.UseVisualStyleBackColor = True
        '
        'cbMail
        '
        Me.cbMail.AutoSize = True
        Me.cbMail.Location = New System.Drawing.Point(69, 122)
        Me.cbMail.Name = "cbMail"
        Me.cbMail.Size = New System.Drawing.Size(96, 17)
        Me.cbMail.TabIndex = 20
        Me.cbMail.Text = "Email Backups"
        Me.cbMail.UseVisualStyleBackColor = True
        '
        'dtScore
        '
        Me.dtScore.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtScore.Location = New System.Drawing.Point(35, 42)
        Me.dtScore.Name = "dtScore"
        Me.dtScore.Size = New System.Drawing.Size(95, 20)
        Me.dtScore.TabIndex = 21
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(32, 26)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 13)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Score Date"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.GroupBox8)
        Me.GroupBox4.Controls.Add(Me.Label7)
        Me.GroupBox4.Controls.Add(Me.cbScoreDt)
        Me.GroupBox4.Controls.Add(Me.GroupBox5)
        Me.GroupBox4.Controls.Add(Me.Label2)
        Me.GroupBox4.Controls.Add(Me.dtScore)
        Me.GroupBox4.Location = New System.Drawing.Point(388, 16)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(166, 382)
        Me.GroupBox4.TabIndex = 23
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Control Dates"
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.tbPSEnd)
        Me.GroupBox8.Controls.Add(Me.tbPSStart)
        Me.GroupBox8.Controls.Add(Me.Label10)
        Me.GroupBox8.Controls.Add(Me.Label11)
        Me.GroupBox8.Location = New System.Drawing.Point(29, 245)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(107, 100)
        Me.GroupBox8.TabIndex = 32
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Post Season"
        '
        'tbPSEnd
        '
        Me.tbPSEnd.Location = New System.Drawing.Point(0, 75)
        Me.tbPSEnd.Name = "tbPSEnd"
        Me.tbPSEnd.Size = New System.Drawing.Size(96, 20)
        Me.tbPSEnd.TabIndex = 35
        '
        'tbPSStart
        '
        Me.tbPSStart.Location = New System.Drawing.Point(0, 34)
        Me.tbPSStart.Name = "tbPSStart"
        Me.tbPSStart.Size = New System.Drawing.Size(96, 20)
        Me.tbPSStart.TabIndex = 34
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(6, 61)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(26, 13)
        Me.Label10.TabIndex = 26
        Me.Label10.Text = "End"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(3, 18)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(29, 13)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "Start"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(32, 188)
        Me.Label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(61, 13)
        Me.Label7.TabIndex = 30
        Me.Label7.Text = "Score Date"
        '
        'cbScoreDt
        '
        Me.cbScoreDt.FormattingEnabled = True
        Me.cbScoreDt.Location = New System.Drawing.Point(32, 204)
        Me.cbScoreDt.Margin = New System.Windows.Forms.Padding(2)
        Me.cbScoreDt.Name = "cbScoreDt"
        Me.cbScoreDt.Size = New System.Drawing.Size(92, 21)
        Me.cbScoreDt.TabIndex = 29
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.dtRSStart)
        Me.GroupBox5.Controls.Add(Me.Label4)
        Me.GroupBox5.Controls.Add(Me.Label3)
        Me.GroupBox5.Controls.Add(Me.dtRSEnd)
        Me.GroupBox5.Location = New System.Drawing.Point(29, 73)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(107, 100)
        Me.GroupBox5.TabIndex = 27
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Regular Season"
        '
        'dtRSStart
        '
        Me.dtRSStart.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtRSStart.Location = New System.Drawing.Point(6, 34)
        Me.dtRSStart.Name = "dtRSStart"
        Me.dtRSStart.Size = New System.Drawing.Size(95, 20)
        Me.dtRSStart.TabIndex = 23
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 61)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(26, 13)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "End"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 24
        Me.Label3.Text = "Start"
        '
        'dtRSEnd
        '
        Me.dtRSEnd.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtRSEnd.Location = New System.Drawing.Point(6, 77)
        Me.dtRSEnd.Name = "dtRSEnd"
        Me.dtRSEnd.Size = New System.Drawing.Size(95, 20)
        Me.dtRSEnd.TabIndex = 25
        '
        'lblProcessMsg
        '
        Me.lblProcessMsg.AutoSize = True
        Me.lblProcessMsg.Location = New System.Drawing.Point(46, 65)
        Me.lblProcessMsg.Name = "lblProcessMsg"
        Me.lblProcessMsg.Size = New System.Drawing.Size(91, 13)
        Me.lblProcessMsg.TabIndex = 16
        Me.lblProcessMsg.Text = "Process Message"
        '
        'gbControls
        '
        Me.gbControls.Controls.Add(Me.cbDebug)
        Me.gbControls.Controls.Add(Me.lbScoresFile)
        Me.gbControls.Controls.Add(Me.btnExit)
        Me.gbControls.Controls.Add(Me.cbMail)
        Me.gbControls.Controls.Add(Me.txtFolder)
        Me.gbControls.Controls.Add(Me.btnChangeFolder)
        Me.gbControls.Controls.Add(Me.cbLogging)
        Me.gbControls.Location = New System.Drawing.Point(41, 577)
        Me.gbControls.Margin = New System.Windows.Forms.Padding(2)
        Me.gbControls.Name = "gbControls"
        Me.gbControls.Padding = New System.Windows.Forms.Padding(2)
        Me.gbControls.Size = New System.Drawing.Size(326, 145)
        Me.gbControls.TabIndex = 24
        Me.gbControls.TabStop = False
        Me.gbControls.Text = "Controls"
        '
        'cbDebug
        '
        Me.cbDebug.AutoSize = True
        Me.cbDebug.Location = New System.Drawing.Point(171, 122)
        Me.cbDebug.Name = "cbDebug"
        Me.cbDebug.Size = New System.Drawing.Size(88, 17)
        Me.cbDebug.TabIndex = 21
        Me.cbDebug.Text = "Debug Mode"
        Me.cbDebug.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.btnStandingsSnapshot)
        Me.GroupBox6.Controls.Add(Me.btnSkinsRpt)
        Me.GroupBox6.Controls.Add(Me.btnLast5)
        Me.GroupBox6.Controls.Add(Me.Button1)
        Me.GroupBox6.Controls.Add(Me.btnPlayerStats)
        Me.GroupBox6.Controls.Add(Me.btnStandings)
        Me.GroupBox6.Location = New System.Drawing.Point(41, 240)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox6.Size = New System.Drawing.Size(325, 158)
        Me.GroupBox6.TabIndex = 9
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Finance and Reports"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tspb, Me.tssl})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 772)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 7, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(605, 26)
        Me.StatusStrip1.TabIndex = 25
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'tspb
        '
        Me.tspb.Name = "tspb"
        Me.tspb.Size = New System.Drawing.Size(200, 20)
        Me.tspb.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'tssl
        '
        Me.tssl.Name = "tssl"
        Me.tssl.Size = New System.Drawing.Size(119, 21)
        Me.tssl.Text = "ToolStripStatusLabel1"
        '
        'btnStandingsSnapshot
        '
        Me.btnStandingsSnapshot.Location = New System.Drawing.Point(20, 59)
        Me.btnStandingsSnapshot.Margin = New System.Windows.Forms.Padding(2)
        Me.btnStandingsSnapshot.Name = "btnStandingsSnapshot"
        Me.btnStandingsSnapshot.Size = New System.Drawing.Size(131, 24)
        Me.btnStandingsSnapshot.TabIndex = 13
        Me.btnStandingsSnapshot.Text = "Standings Snapshot"
        Me.btnStandingsSnapshot.UseVisualStyleBackColor = True
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(605, 798)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.GroupBox6)
        Me.Controls.Add(Me.gbControls)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.lblProcessMsg)
        Me.Controls.Add(Me.cbLeagues)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Main"
        Me.Text = "League Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.gbControls.ResumeLayout(False)
        Me.gbControls.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnLeagueSetup As System.Windows.Forms.Button
    Friend WithEvents btnCourseSetup As System.Windows.Forms.Button
    Friend WithEvents btnPlayerSetup As System.Windows.Forms.Button
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnScoreCard As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents btnStandings As Button
    Friend WithEvents btnShowScores As Button
    Friend WithEvents btnMatches As Button
    Friend WithEvents btnSkins As Button
    Friend WithEvents txtFolder As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents cbLeagues As ComboBox
    Friend WithEvents btnPlayerStats As Button
    Friend WithEvents btnLast5 As Button
    Friend WithEvents lbScoresFile As Label
    Friend WithEvents btnSchedule As Button
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents btnChangeFolder As Button
    Friend WithEvents btnZip As Button
    Friend WithEvents cbLogging As CheckBox
    Friend WithEvents cbMail As CheckBox
    Friend WithEvents Button1 As Button
    Friend WithEvents dtScore As DateTimePicker
    Friend WithEvents Label2 As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents dtRSStart As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents dtRSEnd As DateTimePicker
    Friend WithEvents btnSkinsRpt As Button
    Friend WithEvents lblProcessMsg As Label
    Friend WithEvents btnUndoScores As Button
    Friend WithEvents btnPayments As Button
    Friend WithEvents gbControls As GroupBox
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents cbDebug As CheckBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents tspb As ToolStripProgressBar
    Friend WithEvents tssl As ToolStripStatusLabel
    Friend WithEvents Label7 As Label
    Friend WithEvents cbScoreDt As ComboBox
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents tbPSEnd As TextBox
    Friend WithEvents tbPSStart As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents btnEmailAnnouncement As Button
    Friend WithEvents btnStandingsSnapshot As Button
End Class
