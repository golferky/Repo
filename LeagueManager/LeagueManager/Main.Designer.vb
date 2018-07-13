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
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnLast5 = New System.Windows.Forms.Button()
        Me.btnPlayerStats = New System.Windows.Forms.Button()
        Me.btnMatches = New System.Windows.Forms.Button()
        Me.btnSkins = New System.Windows.Forms.Button()
        Me.btnStandings = New System.Windows.Forms.Button()
        Me.btnScoreCard = New System.Windows.Forms.Button()
        Me.btnShowScores = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.txtFolder = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btnZip = New System.Windows.Forms.Button()
        Me.btnSchedule = New System.Windows.Forms.Button()
        Me.cbLeagues = New System.Windows.Forms.ComboBox()
        Me.lblProcessMsg = New System.Windows.Forms.Label()
        Me.lbScoresFile = New System.Windows.Forms.Label()
        Me.lbVersion = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnChangeFolder = New System.Windows.Forms.Button()
        Me.cbLogging = New System.Windows.Forms.CheckBox()
        Me.cbMail = New System.Windows.Forms.CheckBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnLeagueSetup
        '
        Me.btnLeagueSetup.Location = New System.Drawing.Point(6, 31)
        Me.btnLeagueSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnLeagueSetup.Name = "btnLeagueSetup"
        Me.btnLeagueSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnLeagueSetup.TabIndex = 4
        Me.btnLeagueSetup.Text = "League Setup"
        Me.btnLeagueSetup.UseVisualStyleBackColor = True
        '
        'btnCourseSetup
        '
        Me.btnCourseSetup.Location = New System.Drawing.Point(6, 81)
        Me.btnCourseSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnCourseSetup.Name = "btnCourseSetup"
        Me.btnCourseSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnCourseSetup.TabIndex = 6
        Me.btnCourseSetup.Text = "Course Setup"
        Me.btnCourseSetup.UseVisualStyleBackColor = True
        '
        'btnPlayerSetup
        '
        Me.btnPlayerSetup.Location = New System.Drawing.Point(6, 131)
        Me.btnPlayerSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPlayerSetup.Name = "btnPlayerSetup"
        Me.btnPlayerSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnPlayerSetup.TabIndex = 7
        Me.btnPlayerSetup.Text = "Player Setup"
        Me.btnPlayerSetup.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(211, 656)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(131, 24)
        Me.btnExit.TabIndex = 8
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Button1)
        Me.GroupBox1.Controls.Add(Me.btnLast5)
        Me.GroupBox1.Controls.Add(Me.btnPlayerStats)
        Me.GroupBox1.Controls.Add(Me.btnMatches)
        Me.GroupBox1.Controls.Add(Me.btnSkins)
        Me.GroupBox1.Controls.Add(Me.btnStandings)
        Me.GroupBox1.Controls.Add(Me.btnScoreCard)
        Me.GroupBox1.Location = New System.Drawing.Point(35, 158)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(325, 234)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scoring and Games"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(20, 173)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(131, 24)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Financial"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnLast5
        '
        Me.btnLast5.Location = New System.Drawing.Point(20, 73)
        Me.btnLast5.Margin = New System.Windows.Forms.Padding(2)
        Me.btnLast5.Name = "btnLast5"
        Me.btnLast5.Size = New System.Drawing.Size(131, 24)
        Me.btnLast5.TabIndex = 10
        Me.btnLast5.Text = "Last 5 Scores"
        Me.btnLast5.UseVisualStyleBackColor = True
        '
        'btnPlayerStats
        '
        Me.btnPlayerStats.Location = New System.Drawing.Point(20, 123)
        Me.btnPlayerStats.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPlayerStats.Name = "btnPlayerStats"
        Me.btnPlayerStats.Size = New System.Drawing.Size(131, 24)
        Me.btnPlayerStats.TabIndex = 9
        Me.btnPlayerStats.Text = " Player Stats"
        Me.btnPlayerStats.UseVisualStyleBackColor = True
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
        'btnStandings
        '
        Me.btnStandings.Location = New System.Drawing.Point(176, 123)
        Me.btnStandings.Margin = New System.Windows.Forms.Padding(2)
        Me.btnStandings.Name = "btnStandings"
        Me.btnStandings.Size = New System.Drawing.Size(131, 24)
        Me.btnStandings.TabIndex = 5
        Me.btnStandings.Text = "Standings"
        Me.btnStandings.UseVisualStyleBackColor = True
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
        Me.GroupBox2.Controls.Add(Me.btnLeagueSetup)
        Me.GroupBox2.Controls.Add(Me.btnCourseSetup)
        Me.GroupBox2.Controls.Add(Me.btnPlayerSetup)
        Me.GroupBox2.Location = New System.Drawing.Point(41, 396)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Size = New System.Drawing.Size(151, 170)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Setup Functions"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'txtFolder
        '
        Me.txtFolder.Location = New System.Drawing.Point(35, 611)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(307, 20)
        Me.txtFolder.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(38, 46)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "League Name"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.btnZip)
        Me.GroupBox3.Controls.Add(Me.btnSchedule)
        Me.GroupBox3.Controls.Add(Me.btnShowScores)
        Me.GroupBox3.Location = New System.Drawing.Point(196, 396)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(149, 170)
        Me.GroupBox3.TabIndex = 14
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Utilities"
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
        Me.cbLeagues.Location = New System.Drawing.Point(35, 73)
        Me.cbLeagues.Name = "cbLeagues"
        Me.cbLeagues.Size = New System.Drawing.Size(325, 21)
        Me.cbLeagues.TabIndex = 15
        '
        'lblProcessMsg
        '
        Me.lblProcessMsg.AutoSize = True
        Me.lblProcessMsg.Location = New System.Drawing.Point(38, 116)
        Me.lblProcessMsg.Name = "lblProcessMsg"
        Me.lblProcessMsg.Size = New System.Drawing.Size(91, 13)
        Me.lblProcessMsg.TabIndex = 16
        Me.lblProcessMsg.Text = "Process Message"
        '
        'lbScoresFile
        '
        Me.lbScoresFile.AutoSize = True
        Me.lbScoresFile.Location = New System.Drawing.Point(32, 595)
        Me.lbScoresFile.Name = "lbScoresFile"
        Me.lbScoresFile.Size = New System.Drawing.Size(88, 13)
        Me.lbScoresFile.TabIndex = 17
        Me.lbScoresFile.Text = "Scores File name"
        '
        'lbVersion
        '
        Me.lbVersion.AutoSize = True
        Me.lbVersion.Location = New System.Drawing.Point(287, 29)
        Me.lbVersion.Name = "lbVersion"
        Me.lbVersion.Size = New System.Drawing.Size(42, 13)
        Me.lbVersion.TabIndex = 18
        Me.lbVersion.Text = "Version"
        '
        'btnChangeFolder
        '
        Me.btnChangeFolder.Location = New System.Drawing.Point(35, 656)
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
        Me.cbLogging.Location = New System.Drawing.Point(35, 698)
        Me.cbLogging.Name = "cbLogging"
        Me.cbLogging.Size = New System.Drawing.Size(64, 17)
        Me.cbLogging.TabIndex = 13
        Me.cbLogging.Text = "Logging"
        Me.cbLogging.UseVisualStyleBackColor = True
        '
        'cbMail
        '
        Me.cbMail.AutoSize = True
        Me.cbMail.Location = New System.Drawing.Point(139, 698)
        Me.cbMail.Name = "cbMail"
        Me.cbMail.Size = New System.Drawing.Size(96, 17)
        Me.cbMail.TabIndex = 20
        Me.cbMail.Text = "Email Backups"
        Me.cbMail.UseVisualStyleBackColor = True
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(406, 741)
        Me.Controls.Add(Me.cbMail)
        Me.Controls.Add(Me.btnChangeFolder)
        Me.Controls.Add(Me.lbVersion)
        Me.Controls.Add(Me.lbScoresFile)
        Me.Controls.Add(Me.lblProcessMsg)
        Me.Controls.Add(Me.cbLeagues)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.cbLogging)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtFolder)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnExit)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Main"
        Me.Text = "League Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
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
    Friend WithEvents lblProcessMsg As Label
    Friend WithEvents btnPlayerStats As Button
    Friend WithEvents btnLast5 As Button
    Friend WithEvents lbScoresFile As Label
    Friend WithEvents btnSchedule As Button
    Friend WithEvents lbVersion As Label
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents btnChangeFolder As Button
    Friend WithEvents btnZip As Button
    Friend WithEvents cbLogging As CheckBox
    Friend WithEvents cbMail As CheckBox
    Friend WithEvents Button1 As Button
End Class
