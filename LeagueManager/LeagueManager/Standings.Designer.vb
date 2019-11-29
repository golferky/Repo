<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Standings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.dgStandings = New System.Windows.Forms.DataGridView()
        Me.gbOptions = New System.Windows.Forms.GroupBox()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbExcel = New System.Windows.Forms.CheckBox()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cb1stHalf = New System.Windows.Forms.CheckBox()
        Me.cb2ndHalf = New System.Windows.Forms.CheckBox()
        Me.gbPoints = New System.Windows.Forms.GroupBox()
        Me.rbCum = New System.Windows.Forms.RadioButton()
        Me.rbWeekly = New System.Windows.Forms.RadioButton()
        Me.gbCell = New System.Windows.Forms.GroupBox()
        Me.cbIPoints = New System.Windows.Forms.CheckBox()
        Me.cbHdcp = New System.Windows.Forms.CheckBox()
        Me.cbOpp = New System.Windows.Forms.CheckBox()
        Me.cbScore = New System.Windows.Forms.CheckBox()
        Me.cbTeam = New System.Windows.Forms.CheckBox()
        Me.cbSubs = New System.Windows.Forms.CheckBox()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.btnEmail = New System.Windows.Forms.Button()
        Me.cbTotalPts = New System.Windows.Forms.CheckBox()
        Me.cbIndPts = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnDisplay = New System.Windows.Forms.Button()
        CType(Me.dgStandings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbOptions.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.gbPoints.SuspendLayout()
        Me.gbCell.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgStandings
        '
        Me.dgStandings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgStandings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgStandings.Location = New System.Drawing.Point(32, 126)
        Me.dgStandings.Name = "dgStandings"
        Me.dgStandings.Size = New System.Drawing.Size(1031, 536)
        Me.dgStandings.TabIndex = 70
        '
        'gbOptions
        '
        Me.gbOptions.Controls.Add(Me.cbDates)
        Me.gbOptions.Controls.Add(Me.Label2)
        Me.gbOptions.Controls.Add(Me.cbExcel)
        Me.gbOptions.Controls.Add(Me.lbStatus)
        Me.gbOptions.Controls.Add(Me.GroupBox1)
        Me.gbOptions.Controls.Add(Me.gbPoints)
        Me.gbOptions.Controls.Add(Me.gbCell)
        Me.gbOptions.Controls.Add(Me.cbSubs)
        Me.gbOptions.Location = New System.Drawing.Point(12, 2)
        Me.gbOptions.Name = "gbOptions"
        Me.gbOptions.Size = New System.Drawing.Size(814, 108)
        Me.gbOptions.TabIndex = 71
        Me.gbOptions.TabStop = False
        Me.gbOptions.Text = "Display Options"
        '
        'cbDates
        '
        Me.cbDates.FormattingEnabled = True
        Me.cbDates.Location = New System.Drawing.Point(699, 45)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(81, 21)
        Me.cbDates.TabIndex = 78
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(719, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 13)
        Me.Label2.TabIndex = 77
        Me.Label2.Text = "Score Date"
        '
        'cbExcel
        '
        Me.cbExcel.AutoSize = True
        Me.cbExcel.Location = New System.Drawing.Point(551, 29)
        Me.cbExcel.Name = "cbExcel"
        Me.cbExcel.Size = New System.Drawing.Size(52, 17)
        Me.cbExcel.TabIndex = 76
        Me.cbExcel.Text = "Excel"
        Me.cbExcel.UseVisualStyleBackColor = True
        Me.cbExcel.Visible = False
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(494, 68)
        Me.lbStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(37, 13)
        Me.lbStatus.TabIndex = 75
        Me.lbStatus.Text = "Status"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cb1stHalf)
        Me.GroupBox1.Controls.Add(Me.cb2ndHalf)
        Me.GroupBox1.Location = New System.Drawing.Point(260, 19)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(112, 79)
        Me.GroupBox1.TabIndex = 74
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Season Options"
        '
        'cb1stHalf
        '
        Me.cb1stHalf.AutoSize = True
        Me.cb1stHalf.Checked = True
        Me.cb1stHalf.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cb1stHalf.Location = New System.Drawing.Point(6, 19)
        Me.cb1stHalf.Name = "cb1stHalf"
        Me.cb1stHalf.Size = New System.Drawing.Size(62, 17)
        Me.cb1stHalf.TabIndex = 7
        Me.cb1stHalf.Text = "1st Half"
        Me.cb1stHalf.UseVisualStyleBackColor = True
        '
        'cb2ndHalf
        '
        Me.cb2ndHalf.AutoSize = True
        Me.cb2ndHalf.Checked = True
        Me.cb2ndHalf.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cb2ndHalf.Location = New System.Drawing.Point(6, 42)
        Me.cb2ndHalf.Name = "cb2ndHalf"
        Me.cb2ndHalf.Size = New System.Drawing.Size(66, 17)
        Me.cb2ndHalf.TabIndex = 8
        Me.cb2ndHalf.Text = "2nd Half"
        Me.cb2ndHalf.UseVisualStyleBackColor = True
        '
        'gbPoints
        '
        Me.gbPoints.Controls.Add(Me.rbCum)
        Me.gbPoints.Controls.Add(Me.rbWeekly)
        Me.gbPoints.Location = New System.Drawing.Point(378, 19)
        Me.gbPoints.Name = "gbPoints"
        Me.gbPoints.Size = New System.Drawing.Size(111, 79)
        Me.gbPoints.TabIndex = 72
        Me.gbPoints.TabStop = False
        Me.gbPoints.Text = "Points Options"
        '
        'rbCum
        '
        Me.rbCum.AutoSize = True
        Me.rbCum.Location = New System.Drawing.Point(22, 42)
        Me.rbCum.Name = "rbCum"
        Me.rbCum.Size = New System.Drawing.Size(77, 17)
        Me.rbCum.TabIndex = 1
        Me.rbCum.TabStop = True
        Me.rbCum.Text = "Cumulative"
        Me.rbCum.UseVisualStyleBackColor = True
        '
        'rbWeekly
        '
        Me.rbWeekly.AutoSize = True
        Me.rbWeekly.Checked = True
        Me.rbWeekly.Location = New System.Drawing.Point(22, 19)
        Me.rbWeekly.Name = "rbWeekly"
        Me.rbWeekly.Size = New System.Drawing.Size(61, 17)
        Me.rbWeekly.TabIndex = 0
        Me.rbWeekly.TabStop = True
        Me.rbWeekly.Text = "Weekly"
        Me.rbWeekly.UseVisualStyleBackColor = True
        '
        'gbCell
        '
        Me.gbCell.Controls.Add(Me.cbIPoints)
        Me.gbCell.Controls.Add(Me.cbHdcp)
        Me.gbCell.Controls.Add(Me.cbOpp)
        Me.gbCell.Controls.Add(Me.cbScore)
        Me.gbCell.Controls.Add(Me.cbTeam)
        Me.gbCell.Location = New System.Drawing.Point(6, 19)
        Me.gbCell.Name = "gbCell"
        Me.gbCell.Size = New System.Drawing.Size(248, 79)
        Me.gbCell.TabIndex = 74
        Me.gbCell.TabStop = False
        Me.gbCell.Text = "Cell Contents"
        '
        'cbIPoints
        '
        Me.cbIPoints.AutoSize = True
        Me.cbIPoints.Location = New System.Drawing.Point(101, 42)
        Me.cbIPoints.Name = "cbIPoints"
        Me.cbIPoints.Size = New System.Drawing.Size(73, 17)
        Me.cbIPoints.TabIndex = 4
        Me.cbIPoints.Text = "Ind Points"
        Me.cbIPoints.UseVisualStyleBackColor = True
        '
        'cbHdcp
        '
        Me.cbHdcp.AutoSize = True
        Me.cbHdcp.Checked = True
        Me.cbHdcp.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbHdcp.Location = New System.Drawing.Point(6, 43)
        Me.cbHdcp.Name = "cbHdcp"
        Me.cbHdcp.Size = New System.Drawing.Size(72, 17)
        Me.cbHdcp.TabIndex = 2
        Me.cbHdcp.Text = "Handicap"
        Me.cbHdcp.UseVisualStyleBackColor = True
        '
        'cbOpp
        '
        Me.cbOpp.AutoSize = True
        Me.cbOpp.Location = New System.Drawing.Point(180, 20)
        Me.cbOpp.Name = "cbOpp"
        Me.cbOpp.Size = New System.Drawing.Size(73, 17)
        Me.cbOpp.TabIndex = 6
        Me.cbOpp.Text = "Opponent"
        Me.cbOpp.UseVisualStyleBackColor = True
        '
        'cbScore
        '
        Me.cbScore.AutoSize = True
        Me.cbScore.Checked = True
        Me.cbScore.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbScore.Location = New System.Drawing.Point(6, 20)
        Me.cbScore.Name = "cbScore"
        Me.cbScore.Size = New System.Drawing.Size(54, 17)
        Me.cbScore.TabIndex = 1
        Me.cbScore.Text = "Score"
        Me.cbScore.UseVisualStyleBackColor = True
        '
        'cbTeam
        '
        Me.cbTeam.AutoSize = True
        Me.cbTeam.Location = New System.Drawing.Point(101, 20)
        Me.cbTeam.Name = "cbTeam"
        Me.cbTeam.Size = New System.Drawing.Size(71, 17)
        Me.cbTeam.TabIndex = 5
        Me.cbTeam.Text = "Team Pts"
        Me.cbTeam.UseVisualStyleBackColor = True
        '
        'cbSubs
        '
        Me.cbSubs.AutoSize = True
        Me.cbSubs.Checked = True
        Me.cbSubs.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbSubs.Location = New System.Drawing.Point(495, 29)
        Me.cbSubs.Name = "cbSubs"
        Me.cbSubs.Size = New System.Drawing.Size(50, 17)
        Me.cbSubs.TabIndex = 3
        Me.cbSubs.Text = "Subs"
        Me.cbSubs.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(959, 79)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(106, 37)
        Me.btnExit.TabIndex = 73
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'btnEmail
        '
        Me.btnEmail.Location = New System.Drawing.Point(959, 46)
        Me.btnEmail.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEmail.Name = "btnEmail"
        Me.btnEmail.Size = New System.Drawing.Size(106, 37)
        Me.btnEmail.TabIndex = 74
        Me.btnEmail.Text = "Email Standings"
        Me.btnEmail.UseVisualStyleBackColor = True
        '
        'cbTotalPts
        '
        Me.cbTotalPts.AutoSize = True
        Me.cbTotalPts.Location = New System.Drawing.Point(10, 29)
        Me.cbTotalPts.Name = "cbTotalPts"
        Me.cbTotalPts.Size = New System.Drawing.Size(82, 17)
        Me.cbTotalPts.TabIndex = 7
        Me.cbTotalPts.Text = "Total Points"
        Me.cbTotalPts.UseVisualStyleBackColor = True
        '
        'cbIndPts
        '
        Me.cbIndPts.AutoSize = True
        Me.cbIndPts.Location = New System.Drawing.Point(10, 52)
        Me.cbIndPts.Name = "cbIndPts"
        Me.cbIndPts.Size = New System.Drawing.Size(73, 17)
        Me.cbIndPts.TabIndex = 76
        Me.cbIndPts.Text = "Ind Points"
        Me.cbIndPts.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cbIndPts)
        Me.GroupBox2.Controls.Add(Me.cbTotalPts)
        Me.GroupBox2.Location = New System.Drawing.Point(847, 2)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(107, 108)
        Me.GroupBox2.TabIndex = 75
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Optional Columns"
        '
        'btnDisplay
        '
        Me.btnDisplay.Location = New System.Drawing.Point(959, 11)
        Me.btnDisplay.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDisplay.Name = "btnDisplay"
        Me.btnDisplay.Size = New System.Drawing.Size(106, 37)
        Me.btnDisplay.TabIndex = 76
        Me.btnDisplay.Text = "ReDisplay Standings"
        Me.btnDisplay.UseVisualStyleBackColor = True
        '
        'Standings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(1094, 729)
        Me.Controls.Add(Me.btnDisplay)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.btnEmail)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.gbOptions)
        Me.Controls.Add(Me.dgStandings)
        Me.Name = "Standings"
        Me.Text = "Standings"
        CType(Me.dgStandings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbOptions.ResumeLayout(False)
        Me.gbOptions.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.gbPoints.ResumeLayout(False)
        Me.gbPoints.PerformLayout()
        Me.gbCell.ResumeLayout(False)
        Me.gbCell.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgStandings As DataGridView
    Friend WithEvents gbOptions As GroupBox
    Friend WithEvents cbHdcp As CheckBox
    Friend WithEvents cbScore As CheckBox
    Friend WithEvents cbSubs As CheckBox
    Friend WithEvents cbIPoints As CheckBox
    Friend WithEvents gbPoints As GroupBox
    Friend WithEvents rbCum As RadioButton
    Friend WithEvents rbWeekly As RadioButton
    Friend WithEvents cbTeam As CheckBox
    Friend WithEvents btnExit As Button
    Friend WithEvents cb2ndHalf As CheckBox
    Friend WithEvents cb1stHalf As CheckBox
    Friend WithEvents cbOpp As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents gbCell As GroupBox
    Friend WithEvents btnEmail As Button
    Friend WithEvents lbStatus As Label
    Friend WithEvents cbTotalPts As CheckBox
    Friend WithEvents cbIndPts As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents cbExcel As CheckBox
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btnDisplay As Button
End Class
