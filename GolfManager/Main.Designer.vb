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
        Me.btnExit = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnMatches = New System.Windows.Forms.Button()
        Me.btnScoreCard = New System.Windows.Forms.Button()
        Me.btnPayments = New System.Windows.Forms.Button()
        Me.btnSkinsRpt = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnLast5 = New System.Windows.Forms.Button()
        Me.btnPlayerStats = New System.Windows.Forms.Button()
        Me.btnStandings = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnUndoScores = New System.Windows.Forms.Button()
        Me.cbLogging = New System.Windows.Forms.CheckBox()
        Me.cbMail = New System.Windows.Forms.CheckBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.txtFolder = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btnSetup = New System.Windows.Forms.Button()
        Me.btnEmailAnnouncement = New System.Windows.Forms.Button()
        Me.btnZip = New System.Windows.Forms.Button()
        Me.cbLeagues = New System.Windows.Forms.ComboBox()
        Me.lbScoresFile = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnChangeFolder = New System.Windows.Forms.Button()
        Me.lblProcessMsg = New System.Windows.Forms.Label()
        Me.gbControls = New System.Windows.Forms.GroupBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btnStandingsSnapshot = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.tspb = New System.Windows.Forms.ToolStripProgressBar()
        Me.tssl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lbMonitor = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MenuStrip2 = New System.Windows.Forms.MenuStrip()
        Me.MenuStrip3 = New System.Windows.Forms.MenuStrip()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.btnUpdPmts = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.gbControls.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.MenuStrip3.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(8, 120)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(134, 24)
        Me.btnExit.TabIndex = 8
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnMatches)
        Me.GroupBox1.Controls.Add(Me.btnScoreCard)
        Me.GroupBox1.Location = New System.Drawing.Point(5, 17)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(325, 106)
        Me.GroupBox1.TabIndex = 9
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scoring and Games"
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
        'btnScoreCard
        '
        Me.btnScoreCard.Location = New System.Drawing.Point(20, 26)
        Me.btnScoreCard.Margin = New System.Windows.Forms.Padding(2)
        Me.btnScoreCard.Name = "btnScoreCard"
        Me.btnScoreCard.Size = New System.Drawing.Size(131, 24)
        Me.btnScoreCard.TabIndex = 4
        Me.btnScoreCard.Text = "ScoreCard"
        Me.btnScoreCard.UseVisualStyleBackColor = True
        '
        'btnPayments
        '
        Me.btnPayments.Location = New System.Drawing.Point(21, 140)
        Me.btnPayments.Margin = New System.Windows.Forms.Padding(2)
        Me.btnPayments.Name = "btnPayments"
        Me.btnPayments.Size = New System.Drawing.Size(131, 24)
        Me.btnPayments.TabIndex = 13
        Me.btnPayments.Text = "Payments"
        Me.btnPayments.UseVisualStyleBackColor = True
        '
        'btnSkinsRpt
        '
        Me.btnSkinsRpt.Location = New System.Drawing.Point(179, 62)
        Me.btnSkinsRpt.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSkinsRpt.Name = "btnSkinsRpt"
        Me.btnSkinsRpt.Size = New System.Drawing.Size(131, 24)
        Me.btnSkinsRpt.TabIndex = 12
        Me.btnSkinsRpt.Text = "Skins Report-YTD"
        Me.btnSkinsRpt.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(179, 23)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(131, 24)
        Me.Button1.TabIndex = 11
        Me.Button1.Text = "Financial"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnLast5
        '
        Me.btnLast5.Location = New System.Drawing.Point(21, 101)
        Me.btnLast5.Margin = New System.Windows.Forms.Padding(2)
        Me.btnLast5.Name = "btnLast5"
        Me.btnLast5.Size = New System.Drawing.Size(131, 24)
        Me.btnLast5.TabIndex = 10
        Me.btnLast5.Text = "Last 5 Scores"
        Me.btnLast5.UseVisualStyleBackColor = True
        '
        'btnPlayerStats
        '
        Me.btnPlayerStats.Location = New System.Drawing.Point(179, 101)
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
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnUndoScores)
        Me.GroupBox2.Location = New System.Drawing.Point(34, 31)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox2.Size = New System.Drawing.Size(325, 133)
        Me.GroupBox2.TabIndex = 10
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Cleanup Functions"
        '
        'btnUndoScores
        '
        Me.btnUndoScores.Location = New System.Drawing.Point(20, 31)
        Me.btnUndoScores.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUndoScores.Name = "btnUndoScores"
        Me.btnUndoScores.Size = New System.Drawing.Size(131, 24)
        Me.btnUndoScores.TabIndex = 8
        Me.btnUndoScores.Text = "Undo Scores"
        Me.btnUndoScores.UseVisualStyleBackColor = True
        '
        'cbLogging
        '
        Me.cbLogging.AutoSize = True
        Me.cbLogging.Location = New System.Drawing.Point(174, 75)
        Me.cbLogging.Name = "cbLogging"
        Me.cbLogging.Size = New System.Drawing.Size(64, 17)
        Me.cbLogging.TabIndex = 13
        Me.cbLogging.Text = "Logging"
        Me.cbLogging.UseVisualStyleBackColor = True
        '
        'cbMail
        '
        Me.cbMail.AutoSize = True
        Me.cbMail.Location = New System.Drawing.Point(244, 75)
        Me.cbMail.Name = "cbMail"
        Me.cbMail.Size = New System.Drawing.Size(96, 17)
        Me.cbMail.TabIndex = 20
        Me.cbMail.Text = "Email Backups"
        Me.cbMail.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'txtFolder
        '
        Me.txtFolder.Location = New System.Drawing.Point(8, 35)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(148, 20)
        Me.txtFolder.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(34, 46)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "League Name"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.btnUpdPmts)
        Me.GroupBox3.Controls.Add(Me.btnSetup)
        Me.GroupBox3.Controls.Add(Me.btnEmailAnnouncement)
        Me.GroupBox3.Controls.Add(Me.btnZip)
        Me.GroupBox3.Location = New System.Drawing.Point(26, 5)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox3.Size = New System.Drawing.Size(164, 178)
        Me.GroupBox3.TabIndex = 14
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Utilities"
        '
        'btnSetup
        '
        Me.btnSetup.Location = New System.Drawing.Point(14, 26)
        Me.btnSetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSetup.Name = "btnSetup"
        Me.btnSetup.Size = New System.Drawing.Size(131, 24)
        Me.btnSetup.TabIndex = 10
        Me.btnSetup.Text = "Setups"
        Me.btnSetup.UseVisualStyleBackColor = True
        '
        'btnEmailAnnouncement
        '
        Me.btnEmailAnnouncement.Location = New System.Drawing.Point(14, 65)
        Me.btnEmailAnnouncement.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEmailAnnouncement.Name = "btnEmailAnnouncement"
        Me.btnEmailAnnouncement.Size = New System.Drawing.Size(131, 24)
        Me.btnEmailAnnouncement.TabIndex = 9
        Me.btnEmailAnnouncement.Text = "Email Message"
        Me.btnEmailAnnouncement.UseVisualStyleBackColor = True
        '
        'btnZip
        '
        Me.btnZip.Location = New System.Drawing.Point(14, 106)
        Me.btnZip.Margin = New System.Windows.Forms.Padding(2)
        Me.btnZip.Name = "btnZip"
        Me.btnZip.Size = New System.Drawing.Size(131, 24)
        Me.btnZip.TabIndex = 8
        Me.btnZip.Text = "Zip files"
        Me.btnZip.UseVisualStyleBackColor = True
        '
        'cbLeagues
        '
        Me.cbLeagues.FormattingEnabled = True
        Me.cbLeagues.Location = New System.Drawing.Point(37, 72)
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
        Me.btnChangeFolder.Size = New System.Drawing.Size(134, 24)
        Me.btnChangeFolder.TabIndex = 19
        Me.btnChangeFolder.Text = "Change Folder"
        Me.btnChangeFolder.UseVisualStyleBackColor = True
        '
        'lblProcessMsg
        '
        Me.lblProcessMsg.AutoSize = True
        Me.lblProcessMsg.Location = New System.Drawing.Point(34, 102)
        Me.lblProcessMsg.Name = "lblProcessMsg"
        Me.lblProcessMsg.Size = New System.Drawing.Size(91, 13)
        Me.lblProcessMsg.TabIndex = 16
        Me.lblProcessMsg.Text = "Process Message"
        '
        'gbControls
        '
        Me.gbControls.Controls.Add(Me.lbScoresFile)
        Me.gbControls.Controls.Add(Me.btnExit)
        Me.gbControls.Controls.Add(Me.txtFolder)
        Me.gbControls.Controls.Add(Me.btnChangeFolder)
        Me.gbControls.Location = New System.Drawing.Point(19, 5)
        Me.gbControls.Margin = New System.Windows.Forms.Padding(2)
        Me.gbControls.Name = "gbControls"
        Me.gbControls.Padding = New System.Windows.Forms.Padding(2)
        Me.gbControls.Size = New System.Drawing.Size(164, 188)
        Me.gbControls.TabIndex = 24
        Me.gbControls.TabStop = False
        Me.gbControls.Text = "Controls"
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Button2)
        Me.GroupBox6.Controls.Add(Me.btnPayments)
        Me.GroupBox6.Controls.Add(Me.btnStandingsSnapshot)
        Me.GroupBox6.Controls.Add(Me.btnSkinsRpt)
        Me.GroupBox6.Controls.Add(Me.btnLast5)
        Me.GroupBox6.Controls.Add(Me.Button1)
        Me.GroupBox6.Controls.Add(Me.btnPlayerStats)
        Me.GroupBox6.Controls.Add(Me.btnStandings)
        Me.GroupBox6.Location = New System.Drawing.Point(28, 14)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox6.Size = New System.Drawing.Size(325, 169)
        Me.GroupBox6.TabIndex = 9
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Finance and Reports"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(179, 141)
        Me.Button2.Margin = New System.Windows.Forms.Padding(2)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(131, 24)
        Me.Button2.TabIndex = 14
        Me.Button2.Text = "Stats Leaders"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btnStandingsSnapshot
        '
        Me.btnStandingsSnapshot.Location = New System.Drawing.Point(20, 62)
        Me.btnStandingsSnapshot.Margin = New System.Windows.Forms.Padding(2)
        Me.btnStandingsSnapshot.Name = "btnStandingsSnapshot"
        Me.btnStandingsSnapshot.Size = New System.Drawing.Size(131, 24)
        Me.btnStandingsSnapshot.TabIndex = 13
        Me.btnStandingsSnapshot.Text = "Standings Snapshot"
        Me.btnStandingsSnapshot.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tspb, Me.tssl})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 405)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 7, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(488, 26)
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
        'lbMonitor
        '
        Me.lbMonitor.AutoSize = True
        Me.lbMonitor.Location = New System.Drawing.Point(34, 46)
        Me.lbMonitor.Name = "lbMonitor"
        Me.lbMonitor.Size = New System.Drawing.Size(63, 13)
        Me.lbMonitor.TabIndex = 26
        Me.lbMonitor.Text = "Monitor Info"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(352, 46)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 13)
        Me.Label2.TabIndex = 28
        Me.Label2.Text = "Score Date"
        '
        'cbDates
        '
        Me.cbDates.FormattingEnabled = True
        Me.cbDates.Location = New System.Drawing.Point(355, 75)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(81, 21)
        Me.cbDates.TabIndex = 29
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Location = New System.Drawing.Point(37, 145)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(399, 236)
        Me.TabControl1.TabIndex = 31
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(391, 210)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Scoring and Games"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.GroupBox6)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(391, 210)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Finance and Reports"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.GroupBox3)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(391, 210)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Utilities"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.gbControls)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(391, 210)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Controls"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.GroupBox2)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(391, 210)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "Cleanup"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 48)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(488, 24)
        Me.MenuStrip1.TabIndex = 32
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'MenuStrip2
        '
        Me.MenuStrip2.Location = New System.Drawing.Point(0, 24)
        Me.MenuStrip2.Name = "MenuStrip2"
        Me.MenuStrip2.Size = New System.Drawing.Size(488, 24)
        Me.MenuStrip2.TabIndex = 33
        Me.MenuStrip2.Text = "MenuStrip2"
        '
        'MenuStrip3
        '
        Me.MenuStrip3.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutToolStripMenuItem})
        Me.MenuStrip3.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip3.Name = "MenuStrip3"
        Me.MenuStrip3.Size = New System.Drawing.Size(488, 24)
        Me.MenuStrip3.TabIndex = 34
        Me.MenuStrip3.Text = "MenuStrip3"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(80, 20)
        Me.AboutToolStripMenuItem.Text = "ChangeLog"
        '
        'btnUpdPmts
        '
        Me.btnUpdPmts.Location = New System.Drawing.Point(14, 147)
        Me.btnUpdPmts.Name = "btnUpdPmts"
        Me.btnUpdPmts.Size = New System.Drawing.Size(131, 23)
        Me.btnUpdPmts.TabIndex = 11
        Me.btnUpdPmts.Text = "Update Payments"
        Me.btnUpdPmts.UseVisualStyleBackColor = True
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(488, 431)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.cbLogging)
        Me.Controls.Add(Me.cbMail)
        Me.Controls.Add(Me.cbDates)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lbMonitor)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.MenuStrip2)
        Me.Controls.Add(Me.MenuStrip3)
        Me.Controls.Add(Me.lblProcessMsg)
        Me.Controls.Add(Me.cbLeagues)
        Me.Controls.Add(Me.Label1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Main"
        Me.Text = "League Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.gbControls.ResumeLayout(False)
        Me.gbControls.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.MenuStrip3.ResumeLayout(False)
        Me.MenuStrip3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnScoreCard As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents btnStandings As Button
    Friend WithEvents btnMatches As Button
    Friend WithEvents txtFolder As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents cbLeagues As ComboBox
    Friend WithEvents btnPlayerStats As Button
    Friend WithEvents btnLast5 As Button
    Friend WithEvents lbScoresFile As Label
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents btnChangeFolder As Button
    Friend WithEvents btnZip As Button
    Friend WithEvents cbLogging As CheckBox
    Friend WithEvents cbMail As CheckBox
    Friend WithEvents Button1 As Button
    Friend WithEvents btnSkinsRpt As Button
    Friend WithEvents lblProcessMsg As Label
    Friend WithEvents btnUndoScores As Button
    Friend WithEvents btnPayments As Button
    Friend WithEvents gbControls As GroupBox
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents tspb As ToolStripProgressBar
    Friend WithEvents tssl As ToolStripStatusLabel
    Friend WithEvents btnEmailAnnouncement As Button
    Friend WithEvents btnStandingsSnapshot As Button
    Friend WithEvents btnSetup As Button
    Friend WithEvents lbMonitor As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents TabPage4 As TabPage
    Friend WithEvents TabPage5 As TabPage
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents MenuStrip2 As MenuStrip
    Friend WithEvents MenuStrip3 As MenuStrip
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Button2 As Button
    Friend WithEvents btnUpdPmts As Button
End Class
