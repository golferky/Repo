<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ScheduleBuilder
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
        dgSchedule = New DataGridView()
        Label1 = New Label()
        Label2 = New Label()
        cbPR = New CheckBox()
        cbSplitRounds = New CheckBox()
        cbPET = New CheckBox()
        btnExit = New Button()
        lbStatus = New Label()
        btnEmail = New Button()
        cbPS = New CheckBox()
        tbStart = New TextBox()
        tbEnd = New TextBox()
        dgTeams = New DataGridView()
        gbControls = New GroupBox()
        gbPR = New GroupBox()
        cbPRRS = New CheckBox()
        cbPRAW = New CheckBox()
        GroupBox1 = New GroupBox()
        lbRounds = New Label()
        tbRounds = New TextBox()
        btnCalcSch = New Button()
        CType(dgSchedule, ComponentModel.ISupportInitialize).BeginInit()
        CType(dgTeams, ComponentModel.ISupportInitialize).BeginInit()
        gbControls.SuspendLayout()
        gbPR.SuspendLayout()
        GroupBox1.SuspendLayout()
        SuspendLayout()
        ' 
        ' dgSchedule
        ' 
        dgSchedule.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgSchedule.Location = New Point(72, 355)
        dgSchedule.Margin = New Padding(2)
        dgSchedule.Name = "dgSchedule"
        dgSchedule.RowHeadersWidth = 82
        dgSchedule.RowTemplate.Height = 33
        dgSchedule.Size = New Size(1002, 338)
        dgSchedule.TabIndex = 0
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(23, 32)
        Label1.Margin = New Padding(2, 0, 2, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(58, 15)
        Label1.TabIndex = 2
        Label1.Text = "Start Date"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(168, 32)
        Label2.Margin = New Padding(2, 0, 2, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(54, 15)
        Label2.TabIndex = 4
        Label2.Text = "End Date"
        ' 
        ' cbPR
        ' 
        cbPR.AutoSize = True
        cbPR.Location = New Point(19, 107)
        cbPR.Margin = New Padding(2)
        cbPR.Name = "cbPR"
        cbPR.Size = New Size(112, 19)
        cbPR.TabIndex = 7
        cbPR.Text = "Position Rounds"
        cbPR.UseVisualStyleBackColor = True
        cbPR.Visible = False
        ' 
        ' cbSplitRounds
        ' 
        cbSplitRounds.AutoSize = True
        cbSplitRounds.Location = New Point(19, 35)
        cbSplitRounds.Margin = New Padding(2)
        cbSplitRounds.Name = "cbSplitRounds"
        cbSplitRounds.Size = New Size(89, 19)
        cbSplitRounds.TabIndex = 8
        cbSplitRounds.Text = "Split Season"
        cbSplitRounds.UseVisualStyleBackColor = True
        ' 
        ' cbPET
        ' 
        cbPET.AutoSize = True
        cbPET.Location = New Point(7, 42)
        cbPET.Margin = New Padding(2)
        cbPET.Name = "cbPET"
        cbPET.Size = New Size(108, 19)
        cbPET.TabIndex = 9
        cbPET.Text = "Play each Team"
        cbPET.UseVisualStyleBackColor = True
        ' 
        ' btnExit
        ' 
        btnExit.Location = New Point(72, 220)
        btnExit.Margin = New Padding(2)
        btnExit.Name = "btnExit"
        btnExit.Size = New Size(135, 29)
        btnExit.TabIndex = 10
        btnExit.Text = "Exit"
        btnExit.UseVisualStyleBackColor = True
        ' 
        ' lbStatus
        ' 
        lbStatus.AutoSize = True
        lbStatus.Location = New Point(124, 305)
        lbStatus.Margin = New Padding(2, 0, 2, 0)
        lbStatus.Name = "lbStatus"
        lbStatus.Size = New Size(39, 15)
        lbStatus.TabIndex = 11
        lbStatus.Text = "Status"
        ' 
        ' btnEmail
        ' 
        btnEmail.Location = New Point(72, 97)
        btnEmail.Margin = New Padding(2)
        btnEmail.Name = "btnEmail"
        btnEmail.Size = New Size(135, 29)
        btnEmail.TabIndex = 12
        btnEmail.Text = "Email Schedule"
        btnEmail.UseVisualStyleBackColor = True
        ' 
        ' cbPS
        ' 
        cbPS.AutoSize = True
        cbPS.Location = New Point(19, 70)
        cbPS.Margin = New Padding(2)
        cbPS.Name = "cbPS"
        cbPS.Size = New Size(89, 19)
        cbPS.TabIndex = 13
        cbPS.Text = "Post Season"
        cbPS.UseVisualStyleBackColor = True
        ' 
        ' tbStart
        ' 
        tbStart.Location = New Point(24, 51)
        tbStart.Margin = New Padding(2)
        tbStart.Name = "tbStart"
        tbStart.Size = New Size(89, 23)
        tbStart.TabIndex = 14
        ' 
        ' tbEnd
        ' 
        tbEnd.Location = New Point(169, 51)
        tbEnd.Margin = New Padding(2)
        tbEnd.Name = "tbEnd"
        tbEnd.Size = New Size(89, 23)
        tbEnd.TabIndex = 15
        ' 
        ' dgTeams
        ' 
        dgTeams.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgTeams.Location = New Point(652, 13)
        dgTeams.Margin = New Padding(2)
        dgTeams.Name = "dgTeams"
        dgTeams.RowHeadersWidth = 82
        dgTeams.RowTemplate.Height = 20
        dgTeams.Size = New Size(411, 321)
        dgTeams.TabIndex = 16
        ' 
        ' gbControls
        ' 
        gbControls.Controls.Add(cbSplitRounds)
        gbControls.Controls.Add(cbPR)
        gbControls.Controls.Add(cbPS)
        gbControls.Location = New Point(313, 51)
        gbControls.Margin = New Padding(2)
        gbControls.Name = "gbControls"
        gbControls.Padding = New Padding(2)
        gbControls.Size = New Size(159, 141)
        gbControls.TabIndex = 17
        gbControls.TabStop = False
        gbControls.Text = "Display Controls"
        ' 
        ' gbPR
        ' 
        gbPR.Controls.Add(cbPRRS)
        gbPR.Controls.Add(cbPRAW)
        gbPR.Location = New Point(476, 271)
        gbPR.Margin = New Padding(2)
        gbPR.Name = "gbPR"
        gbPR.Padding = New Padding(2)
        gbPR.Size = New Size(159, 141)
        gbPR.TabIndex = 18
        gbPR.TabStop = False
        gbPR.Text = "Position Round Controls"
        gbPR.Visible = False
        ' 
        ' cbPRRS
        ' 
        cbPRRS.AutoSize = True
        cbPRRS.Location = New Point(19, 35)
        cbPRRS.Margin = New Padding(2)
        cbPRRS.Name = "cbPRRS"
        cbPRRS.Size = New Size(68, 19)
        cbPRRS.TabIndex = 8
        cbPRRS.Text = "Reg Sch"
        cbPRRS.UseVisualStyleBackColor = True
        ' 
        ' cbPRAW
        ' 
        cbPRAW.AutoSize = True
        cbPRAW.Location = New Point(19, 70)
        cbPRAW.Margin = New Padding(2)
        cbPRAW.Name = "cbPRAW"
        cbPRAW.Size = New Size(80, 19)
        cbPRAW.TabIndex = 9
        cbPRAW.Text = "Add Week"
        cbPRAW.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(lbRounds)
        GroupBox1.Controls.Add(tbRounds)
        GroupBox1.Controls.Add(cbPET)
        GroupBox1.Location = New Point(476, 51)
        GroupBox1.Margin = New Padding(2)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(2)
        GroupBox1.Size = New Size(159, 189)
        GroupBox1.TabIndex = 19
        GroupBox1.TabStop = False
        GroupBox1.Text = "Split Season Controls"
        ' 
        ' lbRounds
        ' 
        lbRounds.AutoSize = True
        lbRounds.Location = New Point(30, 81)
        lbRounds.Margin = New Padding(2, 0, 2, 0)
        lbRounds.Name = "lbRounds"
        lbRounds.Size = New Size(47, 15)
        lbRounds.TabIndex = 16
        lbRounds.Text = "Rounds"
        ' 
        ' tbRounds
        ' 
        tbRounds.Location = New Point(28, 97)
        tbRounds.Margin = New Padding(2)
        tbRounds.Name = "tbRounds"
        tbRounds.Size = New Size(54, 23)
        tbRounds.TabIndex = 15
        ' 
        ' btnCalcSch
        ' 
        btnCalcSch.Location = New Point(72, 157)
        btnCalcSch.Margin = New Padding(2)
        btnCalcSch.Name = "btnCalcSch"
        btnCalcSch.Size = New Size(135, 29)
        btnCalcSch.TabIndex = 17
        btnCalcSch.Text = "Calc Schedule"
        btnCalcSch.UseVisualStyleBackColor = True
        ' 
        ' ScheduleBuilder
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        AutoScroll = True
        AutoSize = True
        ClientSize = New Size(1106, 733)
        Controls.Add(btnCalcSch)
        Controls.Add(GroupBox1)
        Controls.Add(gbPR)
        Controls.Add(gbControls)
        Controls.Add(dgTeams)
        Controls.Add(tbEnd)
        Controls.Add(tbStart)
        Controls.Add(btnEmail)
        Controls.Add(lbStatus)
        Controls.Add(btnExit)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(dgSchedule)
        Margin = New Padding(2)
        MinimumSize = New Size(16, 39)
        Name = "ScheduleBuilder"
        Text = "ScheduleBuilder"
        CType(dgSchedule, ComponentModel.ISupportInitialize).EndInit()
        CType(dgTeams, ComponentModel.ISupportInitialize).EndInit()
        gbControls.ResumeLayout(False)
        gbControls.PerformLayout()
        gbPR.ResumeLayout(False)
        gbPR.PerformLayout()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents dgSchedule As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents cbPR As CheckBox
    Friend WithEvents cbSplitRounds As CheckBox
    Friend WithEvents cbPET As CheckBox
    Friend WithEvents btnExit As Button
    Friend WithEvents lbStatus As Label
    Friend WithEvents btnEmail As Button
    Friend WithEvents cbPS As CheckBox
    Friend WithEvents tbStart As TextBox
    Friend WithEvents tbEnd As TextBox
    Friend WithEvents dgTeams As DataGridView
    Friend WithEvents gbControls As GroupBox
    Friend WithEvents gbPR As GroupBox
    Friend WithEvents cbPRRS As CheckBox
    Friend WithEvents cbPRAW As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lbRounds As Label
    Friend WithEvents tbRounds As TextBox
    Friend WithEvents btnCalcSch As Button
End Class
