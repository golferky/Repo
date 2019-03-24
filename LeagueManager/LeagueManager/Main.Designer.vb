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
        Me.gbPS = New System.Windows.Forms.GroupBox()
        Me.tbPSEnd = New System.Windows.Forms.TextBox()
        Me.dtPSStart = New System.Windows.Forms.DateTimePicker()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.dtRSStart = New System.Windows.Forms.DateTimePicker()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.dtRSEnd = New System.Windows.Forms.DateTimePicker()
        Me.lblProcessMsg = New System.Windows.Forms.Label()
        Me.gbControls = New System.Windows.Forms.GroupBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.gbPS.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.gbControls.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnLeagueSetup
        '
        Me.btnLeagueSetup.Location = New System.Drawing.Point(40, 60)
        Me.btnLeagueSetup.Margin = New System.Windows.Forms.Padding(4)
        Me.btnLeagueSetup.Name = "btnLeagueSetup"
        Me.btnLeagueSetup.Size = New System.Drawing.Size(262, 46)
        Me.btnLeagueSetup.TabIndex = 4
        Me.btnLeagueSetup.Text = "League Setup"
        Me.btnLeagueSetup.UseVisualStyleBackColor = True
        '
        'btnCourseSetup
        '
        Me.btnCourseSetup.Location = New System.Drawing.Point(352, 60)
        Me.btnCourseSetup.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCourseSetup.Name = "btnCourseSetup"
        Me.btnCourseSetup.Size = New System.Drawing.Size(262, 46)
        Me.btnCourseSetup.TabIndex = 6
        Me.btnCourseSetup.Text = "Course Setup"
        Me.btnCourseSetup.UseVisualStyleBackColor = True
        '
        'btnPlayerSetup
        '
        Me.btnPlayerSetup.Location = New System.Drawing.Point(352, 156)
        Me.btnPlayerSetup.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPlayerSetup.Name = "btnPlayerSetup"
        Me.btnPlayerSetup.Size = New System.Drawing.Size(262, 46)
        Me.btnPlayerSetup.TabIndex = 7
        Me.btnPlayerSetup.Text = "Player Setup"
        Me.btnPlayerSetup.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(367, 155)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(4)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(262, 46)
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
        Me.GroupBox1.Location = New System.Drawing.Point(83, 188)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(650, 229)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scoring and Games"
        '
        'btnPayments
        '
        Me.btnPayments.Location = New System.Drawing.Point(40, 140)
        Me.btnPayments.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPayments.Name = "btnPayments"
        Me.btnPayments.Size = New System.Drawing.Size(262, 46)
        Me.btnPayments.TabIndex = 13
        Me.btnPayments.Text = "Payments"
        Me.btnPayments.UseVisualStyleBackColor = True
        '
        'btnMatches
        '
        Me.btnMatches.Location = New System.Drawing.Point(352, 50)
        Me.btnMatches.Margin = New System.Windows.Forms.Padding(4)
        Me.btnMatches.Name = "btnMatches"
        Me.btnMatches.Size = New System.Drawing.Size(262, 46)
        Me.btnMatches.TabIndex = 8
        Me.btnMatches.Text = "Matches"
        Me.btnMatches.UseVisualStyleBackColor = True
        '
        'btnSkins
        '
        Me.btnSkins.Location = New System.Drawing.Point(352, 140)
        Me.btnSkins.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSkins.Name = "btnSkins"
        Me.btnSkins.Size = New System.Drawing.Size(262, 46)
        Me.btnSkins.TabIndex = 7
        Me.btnSkins.Text = "Skins"
        Me.btnSkins.UseVisualStyleBackColor = True
        '
        'btnScoreCard
        '
        Me.btnScoreCard.Location = New System.Drawing.Point(40, 50)
        Me.btnScoreCard.Margin = New System.Windows.Forms.Padding(4)
        Me.btnScoreCard.Name = "btnScoreCard"
        Me.btnScoreCard.Size = New System.Drawing.Size(262, 46)
        Me.btnScoreCard.TabIndex = 4
        Me.btnScoreCard.Text = "Enter Scores"
        Me.btnScoreCard.UseVisualStyleBackColor = True
        '
        'btnSkinsRpt
        '
        Me.btnSkinsRpt.Location = New System.Drawing.Point(352, 187)
        Me.btnSkinsRpt.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSkinsRpt.Name = "btnSkinsRpt"
        Me.btnSkinsRpt.Size = New System.Drawing.Size(262, 46)
        Me.btnSkinsRpt.TabIndex = 12
        Me.btnSkinsRpt.Text = "Skins Report-YTD"
        Me.btnSkinsRpt.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(348, 114)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(262, 46)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Financial"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnLast5
        '
        Me.btnLast5.Location = New System.Drawing.Point(40, 114)
        Me.btnLast5.Margin = New System.Windows.Forms.Padding(4)
        Me.btnLast5.Name = "btnLast5"
        Me.btnLast5.Size = New System.Drawing.Size(262, 46)
        Me.btnLast5.TabIndex = 10
        Me.btnLast5.Text = "Last 5 Scores"
        Me.btnLast5.UseVisualStyleBackColor = True
        '
        'btnPlayerStats
        '
        Me.btnPlayerStats.Location = New System.Drawing.Point(348, 45)
        Me.btnPlayerStats.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPlayerStats.Name = "btnPlayerStats"
        Me.btnPlayerStats.Size = New System.Drawing.Size(262, 46)
        Me.btnPlayerStats.TabIndex = 9
        Me.btnPlayerStats.Text = " Player Stats"
        Me.btnPlayerStats.UseVisualStyleBackColor = True
        '
        'btnStandings
        '
        Me.btnStandings.Location = New System.Drawing.Point(40, 45)
        Me.btnStandings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnStandings.Name = "btnStandings"
        Me.btnStandings.Size = New System.Drawing.Size(262, 46)
        Me.btnStandings.TabIndex = 5
        Me.btnStandings.Text = "Standings"
        Me.btnStandings.UseVisualStyleBackColor = True
        '
        'btnShowScores
        '
        Me.btnShowScores.Location = New System.Drawing.Point(22, 252)
        Me.btnShowScores.Margin = New System.Windows.Forms.Padding(4)
        Me.btnShowScores.Name = "btnShowScores"
        Me.btnShowScores.Size = New System.Drawing.Size(262, 46)
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
        Me.GroupBox2.Location = New System.Drawing.Point(83, 816)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Size = New System.Drawing.Size(650, 256)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Setup/Cleanup Functions"
        '
        'btnUndoScores
        '
        Me.btnUndoScores.Location = New System.Drawing.Point(40, 156)
        Me.btnUndoScores.Margin = New System.Windows.Forms.Padding(4)
        Me.btnUndoScores.Name = "btnUndoScores"
        Me.btnUndoScores.Size = New System.Drawing.Size(262, 46)
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
        Me.txtFolder.Location = New System.Drawing.Point(15, 68)
        Me.txtFolder.Margin = New System.Windows.Forms.Padding(6)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(610, 31)
        Me.txtFolder.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(78, 73)
        Me.Label1.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(146, 25)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "League Name"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.btnZip)
        Me.GroupBox3.Controls.Add(Me.btnSchedule)
        Me.GroupBox3.Controls.Add(Me.btnShowScores)
        Me.GroupBox3.Location = New System.Drawing.Point(775, 886)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Size = New System.Drawing.Size(328, 327)
        Me.GroupBox3.TabIndex = 14
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Utilities"
        '
        'btnZip
        '
        Me.btnZip.Location = New System.Drawing.Point(18, 158)
        Me.btnZip.Margin = New System.Windows.Forms.Padding(4)
        Me.btnZip.Name = "btnZip"
        Me.btnZip.Size = New System.Drawing.Size(262, 46)
        Me.btnZip.TabIndex = 8
        Me.btnZip.Text = "Zip files"
        Me.btnZip.UseVisualStyleBackColor = True
        '
        'btnSchedule
        '
        Me.btnSchedule.Location = New System.Drawing.Point(22, 60)
        Me.btnSchedule.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSchedule.Name = "btnSchedule"
        Me.btnSchedule.Size = New System.Drawing.Size(262, 46)
        Me.btnSchedule.TabIndex = 7
        Me.btnSchedule.Text = "Schedule (TBD)"
        Me.btnSchedule.UseVisualStyleBackColor = True
        '
        'cbLeagues
        '
        Me.cbLeagues.FormattingEnabled = True
        Me.cbLeagues.Location = New System.Drawing.Point(84, 123)
        Me.cbLeagues.Margin = New System.Windows.Forms.Padding(6)
        Me.cbLeagues.Name = "cbLeagues"
        Me.cbLeagues.Size = New System.Drawing.Size(258, 33)
        Me.cbLeagues.TabIndex = 15
        '
        'lbScoresFile
        '
        Me.lbScoresFile.AutoSize = True
        Me.lbScoresFile.Location = New System.Drawing.Point(9, 37)
        Me.lbScoresFile.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lbScoresFile.Name = "lbScoresFile"
        Me.lbScoresFile.Size = New System.Drawing.Size(179, 25)
        Me.lbScoresFile.TabIndex = 17
        Me.lbScoresFile.Text = "Scores File name"
        '
        'btnChangeFolder
        '
        Me.btnChangeFolder.Location = New System.Drawing.Point(15, 155)
        Me.btnChangeFolder.Margin = New System.Windows.Forms.Padding(4)
        Me.btnChangeFolder.Name = "btnChangeFolder"
        Me.btnChangeFolder.Size = New System.Drawing.Size(262, 46)
        Me.btnChangeFolder.TabIndex = 19
        Me.btnChangeFolder.Text = "Change Folder"
        Me.btnChangeFolder.UseVisualStyleBackColor = True
        '
        'cbLogging
        '
        Me.cbLogging.AutoSize = True
        Me.cbLogging.Location = New System.Drawing.Point(15, 235)
        Me.cbLogging.Margin = New System.Windows.Forms.Padding(6)
        Me.cbLogging.Name = "cbLogging"
        Me.cbLogging.Size = New System.Drawing.Size(121, 29)
        Me.cbLogging.TabIndex = 13
        Me.cbLogging.Text = "Logging"
        Me.cbLogging.UseVisualStyleBackColor = True
        '
        'cbMail
        '
        Me.cbMail.AutoSize = True
        Me.cbMail.Location = New System.Drawing.Point(223, 235)
        Me.cbMail.Margin = New System.Windows.Forms.Padding(6)
        Me.cbMail.Name = "cbMail"
        Me.cbMail.Size = New System.Drawing.Size(186, 29)
        Me.cbMail.TabIndex = 20
        Me.cbMail.Text = "Email Backups"
        Me.cbMail.UseVisualStyleBackColor = True
        '
        'dtScore
        '
        Me.dtScore.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtScore.Location = New System.Drawing.Point(70, 81)
        Me.dtScore.Margin = New System.Windows.Forms.Padding(6)
        Me.dtScore.Name = "dtScore"
        Me.dtScore.Size = New System.Drawing.Size(186, 31)
        Me.dtScore.TabIndex = 21
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(64, 50)
        Me.Label2.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(119, 25)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Score Date"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.gbPS)
        Me.GroupBox4.Controls.Add(Me.GroupBox5)
        Me.GroupBox4.Controls.Add(Me.Label2)
        Me.GroupBox4.Controls.Add(Me.dtScore)
        Me.GroupBox4.Location = New System.Drawing.Point(771, 188)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox4.Size = New System.Drawing.Size(332, 665)
        Me.GroupBox4.TabIndex = 23
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Control Dates"
        '
        'gbPS
        '
        Me.gbPS.Controls.Add(Me.tbPSEnd)
        Me.gbPS.Controls.Add(Me.dtPSStart)
        Me.gbPS.Controls.Add(Me.Label5)
        Me.gbPS.Controls.Add(Me.Label6)
        Me.gbPS.Location = New System.Drawing.Point(58, 404)
        Me.gbPS.Margin = New System.Windows.Forms.Padding(6)
        Me.gbPS.Name = "gbPS"
        Me.gbPS.Padding = New System.Windows.Forms.Padding(6)
        Me.gbPS.Size = New System.Drawing.Size(214, 192)
        Me.gbPS.TabIndex = 28
        Me.gbPS.TabStop = False
        Me.gbPS.Text = "Post Season"
        '
        'tbPSEnd
        '
        Me.tbPSEnd.Location = New System.Drawing.Point(12, 142)
        Me.tbPSEnd.Margin = New System.Windows.Forms.Padding(6)
        Me.tbPSEnd.Name = "tbPSEnd"
        Me.tbPSEnd.ReadOnly = True
        Me.tbPSEnd.Size = New System.Drawing.Size(186, 31)
        Me.tbPSEnd.TabIndex = 29
        '
        'dtPSStart
        '
        Me.dtPSStart.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtPSStart.Location = New System.Drawing.Point(12, 65)
        Me.dtPSStart.Margin = New System.Windows.Forms.Padding(6)
        Me.dtPSStart.Name = "dtPSStart"
        Me.dtPSStart.Size = New System.Drawing.Size(186, 31)
        Me.dtPSStart.TabIndex = 23
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 117)
        Me.Label5.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(50, 25)
        Me.Label5.TabIndex = 26
        Me.Label5.Text = "End"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 35)
        Me.Label6.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(57, 25)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Start"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.dtRSStart)
        Me.GroupBox5.Controls.Add(Me.Label4)
        Me.GroupBox5.Controls.Add(Me.Label3)
        Me.GroupBox5.Controls.Add(Me.dtRSEnd)
        Me.GroupBox5.Location = New System.Drawing.Point(58, 140)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox5.Size = New System.Drawing.Size(214, 192)
        Me.GroupBox5.TabIndex = 27
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Regular Season"
        '
        'dtRSStart
        '
        Me.dtRSStart.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtRSStart.Location = New System.Drawing.Point(12, 65)
        Me.dtRSStart.Margin = New System.Windows.Forms.Padding(6)
        Me.dtRSStart.Name = "dtRSStart"
        Me.dtRSStart.Size = New System.Drawing.Size(186, 31)
        Me.dtRSStart.TabIndex = 23
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 117)
        Me.Label4.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(50, 25)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "End"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 35)
        Me.Label3.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 25)
        Me.Label3.TabIndex = 24
        Me.Label3.Text = "Start"
        '
        'dtRSEnd
        '
        Me.dtRSEnd.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtRSEnd.Location = New System.Drawing.Point(12, 148)
        Me.dtRSEnd.Margin = New System.Windows.Forms.Padding(6)
        Me.dtRSEnd.Name = "dtRSEnd"
        Me.dtRSEnd.Size = New System.Drawing.Size(186, 31)
        Me.dtRSEnd.TabIndex = 25
        '
        'lblProcessMsg
        '
        Me.lblProcessMsg.AutoSize = True
        Me.lblProcessMsg.Location = New System.Drawing.Point(412, 73)
        Me.lblProcessMsg.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.lblProcessMsg.Name = "lblProcessMsg"
        Me.lblProcessMsg.Size = New System.Drawing.Size(184, 25)
        Me.lblProcessMsg.TabIndex = 16
        Me.lblProcessMsg.Text = "Process Message"
        '
        'gbControls
        '
        Me.gbControls.Controls.Add(Me.lbScoresFile)
        Me.gbControls.Controls.Add(Me.btnExit)
        Me.gbControls.Controls.Add(Me.cbMail)
        Me.gbControls.Controls.Add(Me.txtFolder)
        Me.gbControls.Controls.Add(Me.btnChangeFolder)
        Me.gbControls.Controls.Add(Me.cbLogging)
        Me.gbControls.Location = New System.Drawing.Point(82, 1110)
        Me.gbControls.Name = "gbControls"
        Me.gbControls.Size = New System.Drawing.Size(651, 279)
        Me.gbControls.TabIndex = 24
        Me.gbControls.TabStop = False
        Me.gbControls.Text = "Controls"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.btnSkinsRpt)
        Me.GroupBox6.Controls.Add(Me.btnLast5)
        Me.GroupBox6.Controls.Add(Me.Button1)
        Me.GroupBox6.Controls.Add(Me.btnPlayerStats)
        Me.GroupBox6.Controls.Add(Me.btnStandings)
        Me.GroupBox6.Location = New System.Drawing.Point(82, 462)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(650, 303)
        Me.GroupBox6.TabIndex = 9
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Finance and Reports"
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1159, 1455)
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
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Main"
        Me.Text = "League Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.gbPS.ResumeLayout(False)
        Me.gbPS.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.gbControls.ResumeLayout(False)
        Me.gbControls.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
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
    Friend WithEvents gbPS As GroupBox
    Friend WithEvents dtPSStart As DateTimePicker
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents dtRSStart As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents dtRSEnd As DateTimePicker
    Friend WithEvents tbPSEnd As TextBox
    Friend WithEvents btnSkinsRpt As Button
    Friend WithEvents lblProcessMsg As Label
    Friend WithEvents btnUndoScores As Button
    Friend WithEvents btnPayments As Button
    Friend WithEvents gbControls As GroupBox
    Friend WithEvents GroupBox6 As GroupBox
End Class
