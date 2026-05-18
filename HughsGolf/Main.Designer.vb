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
        btnScoreCard = New Button()
        cbSeasons = New ComboBox()
        cbLeagues = New ComboBox()
        rb1stHalf = New RadioButton()
        gbDateFilter = New GroupBox()
        rbAll = New RadioButton()
        rb2ndHalf = New RadioButton()
        btnSchBuilder = New Button()
        btnXLSXBuilder = New Button()
        ts = New ToolStrip()
        tspb = New ToolStripProgressBar()
        tssl = New ToolStripLabel()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        cbDates = New ComboBox()
        btnAddCSV2XLSX = New Button()
        btnCreateTeams = New Button()
        lblProcessMsg = New Label()
        GroupBox2 = New GroupBox()
        cbPostSeason = New CheckBox()
        btnTeams = New Button()
        btnPlayers = New Button()
        btnCreateScores = New Button()
        btnStandings = New Button()
        btnLeaders = New Button()
        btnCleanupScores = New Button()
        btnEmailMsg = New Button()
        gb1Utilities = New GroupBox()
        Button2 = New Button()
        GroupBox1 = New GroupBox()
        Button1 = New Button()
        btnRO = New Button()
        btnCreateLC = New Button()
        rbCTPs = New RadioButton()
        rbSkins = New RadioButton()
        btnTblCleanup = New Button()
        cbToday = New CheckBox()
        cbTables = New ComboBox()
        Label4 = New Label()
        gbUtilities = New GroupBox()
        btnLeagueMessenger = New Button()
        cbLogging = New CheckBox()
        cbTestEOY = New CheckBox()
        gbEdits = New GroupBox()
        gbDateFilter.SuspendLayout()
        ts.SuspendLayout()
        GroupBox2.SuspendLayout()
        gb1Utilities.SuspendLayout()
        GroupBox1.SuspendLayout()
        gbUtilities.SuspendLayout()
        gbEdits.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnScoreCard
        ' 
        btnScoreCard.Location = New Point(41, 240)
        btnScoreCard.Name = "btnScoreCard"
        btnScoreCard.Size = New Size(108, 23)
        btnScoreCard.TabIndex = 0
        btnScoreCard.Text = "Scorecard"
        btnScoreCard.UseVisualStyleBackColor = True
        ' 
        ' cbSeasons
        ' 
        cbSeasons.FormattingEnabled = True
        cbSeasons.Location = New Point(18, 91)
        cbSeasons.Name = "cbSeasons"
        cbSeasons.Size = New Size(121, 23)
        cbSeasons.TabIndex = 1
        ' 
        ' cbLeagues
        ' 
        cbLeagues.FormattingEnabled = True
        cbLeagues.Location = New Point(18, 41)
        cbLeagues.Name = "cbLeagues"
        cbLeagues.Size = New Size(121, 23)
        cbLeagues.TabIndex = 2
        ' 
        ' rb1stHalf
        ' 
        rb1stHalf.AutoSize = True
        rb1stHalf.Location = New Point(17, 22)
        rb1stHalf.Name = "rb1stHalf"
        rb1stHalf.Size = New Size(65, 19)
        rb1stHalf.TabIndex = 3
        rb1stHalf.Text = "1st Half"
        rb1stHalf.UseVisualStyleBackColor = True
        ' 
        ' gbDateFilter
        ' 
        gbDateFilter.Controls.Add(rbAll)
        gbDateFilter.Controls.Add(rb2ndHalf)
        gbDateFilter.Controls.Add(rb1stHalf)
        gbDateFilter.Location = New Point(23, 291)
        gbDateFilter.Name = "gbDateFilter"
        gbDateFilter.Size = New Size(156, 100)
        gbDateFilter.TabIndex = 4
        gbDateFilter.TabStop = False
        gbDateFilter.Text = "Date Filter"
        gbDateFilter.Visible = False
        ' 
        ' rbAll
        ' 
        rbAll.AutoSize = True
        rbAll.Checked = True
        rbAll.Location = New Point(17, 72)
        rbAll.Name = "rbAll"
        rbAll.Size = New Size(39, 19)
        rbAll.TabIndex = 5
        rbAll.TabStop = True
        rbAll.Text = "All"
        rbAll.UseVisualStyleBackColor = True
        ' 
        ' rb2ndHalf
        ' 
        rb2ndHalf.AutoSize = True
        rb2ndHalf.Location = New Point(17, 47)
        rb2ndHalf.Name = "rb2ndHalf"
        rb2ndHalf.Size = New Size(70, 19)
        rb2ndHalf.TabIndex = 4
        rb2ndHalf.Text = "2nd Half"
        rb2ndHalf.UseVisualStyleBackColor = True
        ' 
        ' btnSchBuilder
        ' 
        btnSchBuilder.Location = New Point(6, 17)
        btnSchBuilder.Name = "btnSchBuilder"
        btnSchBuilder.Size = New Size(108, 23)
        btnSchBuilder.TabIndex = 5
        btnSchBuilder.Text = "Schedule Builder"
        btnSchBuilder.UseVisualStyleBackColor = True
        ' 
        ' btnXLSXBuilder
        ' 
        btnXLSXBuilder.Location = New Point(42, 141)
        btnXLSXBuilder.Name = "btnXLSXBuilder"
        btnXLSXBuilder.Size = New Size(108, 23)
        btnXLSXBuilder.TabIndex = 6
        btnXLSXBuilder.Text = "XLSX Builder"
        btnXLSXBuilder.UseVisualStyleBackColor = True
        ' 
        ' ts
        ' 
        ts.Dock = DockStyle.Bottom
        ts.Items.AddRange(New ToolStripItem() {tspb, tssl})
        ts.Location = New Point(0, 480)
        ts.Name = "ts"
        ts.Size = New Size(1060, 25)
        ts.TabIndex = 7
        ts.Text = "ToolStrip1"
        ' 
        ' tspb
        ' 
        tspb.Name = "tspb"
        tspb.Size = New Size(100, 22)
        ' 
        ' tssl
        ' 
        tssl.Name = "tssl"
        tssl.Size = New Size(88, 22)
        tssl.Text = "ToolStripLabel1"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(18, 23)
        Label1.Name = "Label1"
        Label1.Size = New Size(45, 15)
        Label1.TabIndex = 8
        Label1.Text = "League"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(18, 73)
        Label2.Name = "Label2"
        Label2.Size = New Size(49, 15)
        Label2.TabIndex = 9
        Label2.Text = "Seasons"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(18, 120)
        Label3.Name = "Label3"
        Label3.Size = New Size(41, 15)
        Label3.TabIndex = 11
        Label3.Text = "Weeks"
        ' 
        ' cbDates
        ' 
        cbDates.FormattingEnabled = True
        cbDates.Location = New Point(18, 138)
        cbDates.Name = "cbDates"
        cbDates.Size = New Size(121, 23)
        cbDates.TabIndex = 10
        ' 
        ' btnAddCSV2XLSX
        ' 
        btnAddCSV2XLSX.Location = New Point(42, 108)
        btnAddCSV2XLSX.Name = "btnAddCSV2XLSX"
        btnAddCSV2XLSX.Size = New Size(108, 23)
        btnAddCSV2XLSX.TabIndex = 12
        btnAddCSV2XLSX.Text = "Add CSV to XLSX"
        btnAddCSV2XLSX.UseVisualStyleBackColor = True
        ' 
        ' btnCreateTeams
        ' 
        btnCreateTeams.Location = New Point(102, 22)
        btnCreateTeams.Name = "btnCreateTeams"
        btnCreateTeams.Size = New Size(82, 23)
        btnCreateTeams.TabIndex = 14
        btnCreateTeams.Text = "Teams"
        btnCreateTeams.UseVisualStyleBackColor = True
        ' 
        ' lblProcessMsg
        ' 
        lblProcessMsg.AutoSize = True
        lblProcessMsg.Location = New Point(23, 9)
        lblProcessMsg.Name = "lblProcessMsg"
        lblProcessMsg.Size = New Size(83, 15)
        lblProcessMsg.TabIndex = 15
        lblProcessMsg.Text = "lblProcessMsg"
        lblProcessMsg.Visible = False
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(cbPostSeason)
        GroupBox2.Controls.Add(cbSeasons)
        GroupBox2.Controls.Add(cbLeagues)
        GroupBox2.Controls.Add(Label1)
        GroupBox2.Controls.Add(Label2)
        GroupBox2.Controls.Add(cbDates)
        GroupBox2.Controls.Add(Label3)
        GroupBox2.Location = New Point(23, 37)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(156, 197)
        GroupBox2.TabIndex = 38
        GroupBox2.TabStop = False
        GroupBox2.Text = "Info"
        ' 
        ' cbPostSeason
        ' 
        cbPostSeason.AutoSize = True
        cbPostSeason.Location = New Point(18, 172)
        cbPostSeason.Name = "cbPostSeason"
        cbPostSeason.Size = New Size(121, 19)
        cbPostSeason.TabIndex = 48
        cbPostSeason.Text = "Show Post Season"
        cbPostSeason.UseVisualStyleBackColor = True
        cbPostSeason.Visible = False
        ' 
        ' btnTeams
        ' 
        btnTeams.Location = New Point(9, 22)
        btnTeams.Name = "btnTeams"
        btnTeams.Size = New Size(108, 23)
        btnTeams.TabIndex = 39
        btnTeams.Text = "Edit Teams"
        btnTeams.UseVisualStyleBackColor = True
        ' 
        ' btnPlayers
        ' 
        btnPlayers.Location = New Point(6, 59)
        btnPlayers.Name = "btnPlayers"
        btnPlayers.Size = New Size(108, 23)
        btnPlayers.TabIndex = 40
        btnPlayers.Text = "Edit Players"
        btnPlayers.UseVisualStyleBackColor = True
        ' 
        ' btnCreateScores
        ' 
        btnCreateScores.Location = New Point(102, 51)
        btnCreateScores.Name = "btnCreateScores"
        btnCreateScores.Size = New Size(82, 23)
        btnCreateScores.TabIndex = 41
        btnCreateScores.Text = "Scores"
        btnCreateScores.UseVisualStyleBackColor = True
        ' 
        ' btnStandings
        ' 
        btnStandings.Location = New Point(6, 102)
        btnStandings.Name = "btnStandings"
        btnStandings.Size = New Size(108, 23)
        btnStandings.TabIndex = 42
        btnStandings.Text = "Standings"
        btnStandings.UseVisualStyleBackColor = True
        btnStandings.Visible = False
        ' 
        ' btnLeaders
        ' 
        btnLeaders.Location = New Point(138, 59)
        btnLeaders.Name = "btnLeaders"
        btnLeaders.Size = New Size(108, 23)
        btnLeaders.TabIndex = 43
        btnLeaders.Text = "Leaders"
        btnLeaders.UseVisualStyleBackColor = True
        ' 
        ' btnCleanupScores
        ' 
        btnCleanupScores.Location = New Point(9, 56)
        btnCleanupScores.Name = "btnCleanupScores"
        btnCleanupScores.Size = New Size(108, 23)
        btnCleanupScores.TabIndex = 44
        btnCleanupScores.Text = "Cleanup Scores"
        btnCleanupScores.UseVisualStyleBackColor = True
        ' 
        ' btnEmailMsg
        ' 
        btnEmailMsg.Location = New Point(138, 18)
        btnEmailMsg.Name = "btnEmailMsg"
        btnEmailMsg.Size = New Size(108, 23)
        btnEmailMsg.TabIndex = 45
        btnEmailMsg.Text = "Email Message"
        btnEmailMsg.UseVisualStyleBackColor = True
        ' 
        ' gb1Utilities
        ' 
        gb1Utilities.Controls.Add(Button2)
        gb1Utilities.Controls.Add(GroupBox1)
        gb1Utilities.Controls.Add(rbCTPs)
        gb1Utilities.Controls.Add(rbSkins)
        gb1Utilities.Controls.Add(btnTblCleanup)
        gb1Utilities.Controls.Add(btnXLSXBuilder)
        gb1Utilities.Controls.Add(btnAddCSV2XLSX)
        gb1Utilities.Location = New Point(720, 37)
        gb1Utilities.Name = "gb1Utilities"
        gb1Utilities.Size = New Size(316, 376)
        gb1Utilities.TabIndex = 46
        gb1Utilities.TabStop = False
        gb1Utilities.Text = "One time Utilities"
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(42, 176)
        Button2.Name = "Button2"
        Button2.Size = New Size(108, 23)
        Button2.TabIndex = 55
        Button2.Text = "XLSX Research"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(Button1)
        GroupBox1.Controls.Add(btnRO)
        GroupBox1.Controls.Add(btnCreateLC)
        GroupBox1.Controls.Add(btnCreateTeams)
        GroupBox1.Controls.Add(btnCreateScores)
        GroupBox1.Location = New Point(6, 203)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(188, 125)
        GroupBox1.TabIndex = 51
        GroupBox1.TabStop = False
        GroupBox1.Text = "Table Create"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(14, 79)
        Button1.Name = "Button1"
        Button1.Size = New Size(82, 23)
        Button1.TabIndex = 45
        Button1.Text = "Hdcp Detail"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' btnRO
        ' 
        btnRO.Location = New Point(14, 51)
        btnRO.Name = "btnRO"
        btnRO.Size = New Size(82, 23)
        btnRO.TabIndex = 43
        btnRO.Text = "Rainouts"
        btnRO.UseVisualStyleBackColor = True
        ' 
        ' btnCreateLC
        ' 
        btnCreateLC.Location = New Point(14, 22)
        btnCreateLC.Name = "btnCreateLC"
        btnCreateLC.Size = New Size(82, 23)
        btnCreateLC.TabIndex = 44
        btnCreateLC.Text = "Post Season"
        btnCreateLC.UseVisualStyleBackColor = True
        ' 
        ' rbCTPs
        ' 
        rbCTPs.AutoSize = True
        rbCTPs.Location = New Point(55, 83)
        rbCTPs.Name = "rbCTPs"
        rbCTPs.Size = New Size(52, 19)
        rbCTPs.TabIndex = 44
        rbCTPs.Text = "CTPs"
        rbCTPs.UseVisualStyleBackColor = True
        ' 
        ' rbSkins
        ' 
        rbSkins.AutoSize = True
        rbSkins.Location = New Point(55, 62)
        rbSkins.Name = "rbSkins"
        rbSkins.Size = New Size(52, 19)
        rbSkins.TabIndex = 43
        rbSkins.Text = "Skins"
        rbSkins.UseVisualStyleBackColor = True
        ' 
        ' btnTblCleanup
        ' 
        btnTblCleanup.Location = New Point(42, 33)
        btnTblCleanup.Name = "btnTblCleanup"
        btnTblCleanup.Size = New Size(108, 23)
        btnTblCleanup.TabIndex = 42
        btnTblCleanup.Text = "Table Cleanup"
        btnTblCleanup.UseVisualStyleBackColor = True
        ' 
        ' cbToday
        ' 
        cbToday.AutoSize = True
        cbToday.Checked = True
        cbToday.CheckState = CheckState.Checked
        cbToday.Location = New Point(445, 348)
        cbToday.Name = "cbToday"
        cbToday.Size = New Size(86, 19)
        cbToday.TabIndex = 54
        cbToday.Text = "Today Only"
        cbToday.UseVisualStyleBackColor = True
        ' 
        ' cbTables
        ' 
        cbTables.FormattingEnabled = True
        cbTables.Location = New Point(429, 321)
        cbTables.Name = "cbTables"
        cbTables.Size = New Size(121, 23)
        cbTables.TabIndex = 52
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(429, 303)
        Label4.Name = "Label4"
        Label4.Size = New Size(75, 15)
        Label4.TabIndex = 53
        Label4.Text = "Table Review"
        ' 
        ' gbUtilities
        ' 
        gbUtilities.Controls.Add(btnLeagueMessenger)
        gbUtilities.Controls.Add(btnSchBuilder)
        gbUtilities.Controls.Add(btnStandings)
        gbUtilities.Controls.Add(btnEmailMsg)
        gbUtilities.Controls.Add(btnPlayers)
        gbUtilities.Controls.Add(btnLeaders)
        gbUtilities.Location = New Point(414, 41)
        gbUtilities.Name = "gbUtilities"
        gbUtilities.Size = New Size(250, 131)
        gbUtilities.TabIndex = 47
        gbUtilities.TabStop = False
        gbUtilities.Text = "Utilities"
        ' 
        ' btnLeagueMessenger
        ' 
        btnLeagueMessenger.Location = New Point(136, 102)
        btnLeagueMessenger.Name = "btnLeagueMessenger"
        btnLeagueMessenger.Size = New Size(108, 23)
        btnLeagueMessenger.TabIndex = 46
        btnLeagueMessenger.Text = "Messenger"
        btnLeagueMessenger.UseVisualStyleBackColor = True
        ' 
        ' cbLogging
        ' 
        cbLogging.AutoSize = True
        cbLogging.Location = New Point(23, 394)
        cbLogging.Name = "cbLogging"
        cbLogging.Size = New Size(185, 19)
        cbLogging.TabIndex = 49
        cbLogging.Text = "Turn On Non-Critical Logging"
        cbLogging.UseVisualStyleBackColor = True
        ' 
        ' cbTestEOY
        ' 
        cbTestEOY.AutoSize = True
        cbTestEOY.Location = New Point(41, 266)
        cbTestEOY.Name = "cbTestEOY"
        cbTestEOY.Size = New Size(72, 19)
        cbTestEOY.TabIndex = 50
        cbTestEOY.Text = "Test EOY"
        cbTestEOY.UseVisualStyleBackColor = True
        ' 
        ' gbEdits
        ' 
        gbEdits.Controls.Add(btnTeams)
        gbEdits.Controls.Add(btnCleanupScores)
        gbEdits.Location = New Point(414, 182)
        gbEdits.Name = "gbEdits"
        gbEdits.Size = New Size(250, 100)
        gbEdits.TabIndex = 55
        gbEdits.TabStop = False
        gbEdits.Text = "Edits"
        ' 
        ' Main
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1060, 505)
        Controls.Add(gbEdits)
        Controls.Add(cbTestEOY)
        Controls.Add(cbToday)
        Controls.Add(cbLogging)
        Controls.Add(cbTables)
        Controls.Add(gbUtilities)
        Controls.Add(Label4)
        Controls.Add(gb1Utilities)
        Controls.Add(GroupBox2)
        Controls.Add(lblProcessMsg)
        Controls.Add(ts)
        Controls.Add(gbDateFilter)
        Controls.Add(btnScoreCard)
        Name = "Main"
        Text = "Main"
        gbDateFilter.ResumeLayout(False)
        gbDateFilter.PerformLayout()
        ts.ResumeLayout(False)
        ts.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        gb1Utilities.ResumeLayout(False)
        gb1Utilities.PerformLayout()
        GroupBox1.ResumeLayout(False)
        gbUtilities.ResumeLayout(False)
        gbEdits.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnScoreCard As Button
    Friend WithEvents cbSeasons As ComboBox
    Friend WithEvents cbLeagues As ComboBox
    Friend WithEvents rb1stHalf As RadioButton
    Friend WithEvents gbDateFilter As GroupBox
    Friend WithEvents rb2ndHalf As RadioButton
    Friend WithEvents rbAll As RadioButton
    Friend WithEvents btnSchBuilder As Button
    Friend WithEvents btnXLSXBuilder As Button
    Friend WithEvents ts As ToolStrip
    Friend WithEvents tspb As ToolStripProgressBar
    Friend WithEvents tssl As ToolStripLabel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents btnAddCSV2XLSX As Button
    Friend WithEvents btnCreateTeams As Button
    Friend WithEvents lblProcessMsg As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnTeams As Button
    Friend WithEvents btnPlayers As Button
    Friend WithEvents btnCreateScores As Button
    Friend WithEvents btnStandings As Button
    Friend WithEvents btnLeaders As Button
    Friend WithEvents btnCleanupScores As Button
    Friend WithEvents btnEmailMsg As Button
    Friend WithEvents gb1Utilities As GroupBox
    Friend WithEvents gbUtilities As GroupBox
    Friend WithEvents cbPostSeason As CheckBox
    Friend WithEvents btnTblCleanup As Button
    Friend WithEvents cbLogging As CheckBox
    Friend WithEvents cbTestEOY As CheckBox
    Friend WithEvents btnRO As Button
    Friend WithEvents btnCreateLC As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents rbSkins As RadioButton
    Friend WithEvents rbCTPs As RadioButton
    Friend WithEvents Button1 As Button
    Friend WithEvents cbTables As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents cbToday As CheckBox
    Friend WithEvents Button2 As Button
    Friend WithEvents gbEdits As GroupBox
    Friend WithEvents btnLeagueMessenger As Button

End Class
