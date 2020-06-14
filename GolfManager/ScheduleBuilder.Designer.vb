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
        Me.dgSchedule = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbPR = New System.Windows.Forms.CheckBox()
        Me.cbSplitRounds = New System.Windows.Forms.CheckBox()
        Me.cbPET = New System.Windows.Forms.CheckBox()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.btnEmail = New System.Windows.Forms.Button()
        Me.cbPS = New System.Windows.Forms.CheckBox()
        Me.tbStart = New System.Windows.Forms.TextBox()
        Me.tbEnd = New System.Windows.Forms.TextBox()
        Me.dgTeams = New System.Windows.Forms.DataGridView()
        Me.gbControls = New System.Windows.Forms.GroupBox()
        Me.gbPR = New System.Windows.Forms.GroupBox()
        Me.cbPRRS = New System.Windows.Forms.CheckBox()
        Me.cbPRAW = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lbRounds = New System.Windows.Forms.Label()
        Me.tbRounds = New System.Windows.Forms.TextBox()
        Me.btnCalcSch = New System.Windows.Forms.Button()
        CType(Me.dgSchedule, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgTeams, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbControls.SuspendLayout()
        Me.gbPR.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgSchedule
        '
        Me.dgSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgSchedule.Location = New System.Drawing.Point(22, 487)
        Me.dgSchedule.Margin = New System.Windows.Forms.Padding(2)
        Me.dgSchedule.Name = "dgSchedule"
        Me.dgSchedule.RowHeadersWidth = 82
        Me.dgSchedule.RowTemplate.Height = 33
        Me.dgSchedule.Size = New System.Drawing.Size(1428, 293)
        Me.dgSchedule.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 28)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Start Date"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(144, 28)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "End Date"
        '
        'cbPR
        '
        Me.cbPR.AutoSize = True
        Me.cbPR.Location = New System.Drawing.Point(16, 93)
        Me.cbPR.Margin = New System.Windows.Forms.Padding(2)
        Me.cbPR.Name = "cbPR"
        Me.cbPR.Size = New System.Drawing.Size(103, 17)
        Me.cbPR.TabIndex = 7
        Me.cbPR.Text = "Position Rounds"
        Me.cbPR.UseVisualStyleBackColor = True
        Me.cbPR.Visible = False
        '
        'cbSplitRounds
        '
        Me.cbSplitRounds.AutoSize = True
        Me.cbSplitRounds.Location = New System.Drawing.Point(16, 30)
        Me.cbSplitRounds.Margin = New System.Windows.Forms.Padding(2)
        Me.cbSplitRounds.Name = "cbSplitRounds"
        Me.cbSplitRounds.Size = New System.Drawing.Size(85, 17)
        Me.cbSplitRounds.TabIndex = 8
        Me.cbSplitRounds.Text = "Split Season"
        Me.cbSplitRounds.UseVisualStyleBackColor = True
        '
        'cbPET
        '
        Me.cbPET.AutoSize = True
        Me.cbPET.Location = New System.Drawing.Point(6, 36)
        Me.cbPET.Margin = New System.Windows.Forms.Padding(2)
        Me.cbPET.Name = "cbPET"
        Me.cbPET.Size = New System.Drawing.Size(103, 17)
        Me.cbPET.TabIndex = 9
        Me.cbPET.Text = "Play each Team"
        Me.cbPET.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(62, 191)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(116, 25)
        Me.btnExit.TabIndex = 10
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(106, 264)
        Me.lbStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(37, 13)
        Me.lbStatus.TabIndex = 11
        Me.lbStatus.Text = "Status"
        '
        'btnEmail
        '
        Me.btnEmail.Location = New System.Drawing.Point(62, 84)
        Me.btnEmail.Margin = New System.Windows.Forms.Padding(2)
        Me.btnEmail.Name = "btnEmail"
        Me.btnEmail.Size = New System.Drawing.Size(116, 25)
        Me.btnEmail.TabIndex = 12
        Me.btnEmail.Text = "Email Schedule"
        Me.btnEmail.UseVisualStyleBackColor = True
        '
        'cbPS
        '
        Me.cbPS.AutoSize = True
        Me.cbPS.Location = New System.Drawing.Point(16, 61)
        Me.cbPS.Margin = New System.Windows.Forms.Padding(2)
        Me.cbPS.Name = "cbPS"
        Me.cbPS.Size = New System.Drawing.Size(86, 17)
        Me.cbPS.TabIndex = 13
        Me.cbPS.Text = "Post Season"
        Me.cbPS.UseVisualStyleBackColor = True
        '
        'tbStart
        '
        Me.tbStart.Location = New System.Drawing.Point(21, 44)
        Me.tbStart.Margin = New System.Windows.Forms.Padding(2)
        Me.tbStart.Name = "tbStart"
        Me.tbStart.Size = New System.Drawing.Size(77, 20)
        Me.tbStart.TabIndex = 14
        '
        'tbEnd
        '
        Me.tbEnd.Location = New System.Drawing.Point(145, 44)
        Me.tbEnd.Margin = New System.Windows.Forms.Padding(2)
        Me.tbEnd.Name = "tbEnd"
        Me.tbEnd.Size = New System.Drawing.Size(77, 20)
        Me.tbEnd.TabIndex = 15
        '
        'dgTeams
        '
        Me.dgTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTeams.Location = New System.Drawing.Point(559, 11)
        Me.dgTeams.Margin = New System.Windows.Forms.Padding(2)
        Me.dgTeams.Name = "dgTeams"
        Me.dgTeams.RowHeadersWidth = 82
        Me.dgTeams.RowTemplate.Height = 33
        Me.dgTeams.Size = New System.Drawing.Size(463, 420)
        Me.dgTeams.TabIndex = 16
        '
        'gbControls
        '
        Me.gbControls.Controls.Add(Me.cbSplitRounds)
        Me.gbControls.Controls.Add(Me.cbPR)
        Me.gbControls.Controls.Add(Me.cbPS)
        Me.gbControls.Location = New System.Drawing.Point(268, 44)
        Me.gbControls.Margin = New System.Windows.Forms.Padding(2)
        Me.gbControls.Name = "gbControls"
        Me.gbControls.Padding = New System.Windows.Forms.Padding(2)
        Me.gbControls.Size = New System.Drawing.Size(136, 122)
        Me.gbControls.TabIndex = 17
        Me.gbControls.TabStop = False
        Me.gbControls.Text = "Display Controls"
        '
        'gbPR
        '
        Me.gbPR.Controls.Add(Me.cbPRRS)
        Me.gbPR.Controls.Add(Me.cbPRAW)
        Me.gbPR.Location = New System.Drawing.Point(408, 235)
        Me.gbPR.Margin = New System.Windows.Forms.Padding(2)
        Me.gbPR.Name = "gbPR"
        Me.gbPR.Padding = New System.Windows.Forms.Padding(2)
        Me.gbPR.Size = New System.Drawing.Size(136, 122)
        Me.gbPR.TabIndex = 18
        Me.gbPR.TabStop = False
        Me.gbPR.Text = "Position Round Controls"
        Me.gbPR.Visible = False
        '
        'cbPRRS
        '
        Me.cbPRRS.AutoSize = True
        Me.cbPRRS.Location = New System.Drawing.Point(16, 30)
        Me.cbPRRS.Margin = New System.Windows.Forms.Padding(2)
        Me.cbPRRS.Name = "cbPRRS"
        Me.cbPRRS.Size = New System.Drawing.Size(68, 17)
        Me.cbPRRS.TabIndex = 8
        Me.cbPRRS.Text = "Reg Sch"
        Me.cbPRRS.UseVisualStyleBackColor = True
        '
        'cbPRAW
        '
        Me.cbPRAW.AutoSize = True
        Me.cbPRAW.Location = New System.Drawing.Point(16, 61)
        Me.cbPRAW.Margin = New System.Windows.Forms.Padding(2)
        Me.cbPRAW.Name = "cbPRAW"
        Me.cbPRAW.Size = New System.Drawing.Size(77, 17)
        Me.cbPRAW.TabIndex = 9
        Me.cbPRAW.Text = "Add Week"
        Me.cbPRAW.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lbRounds)
        Me.GroupBox1.Controls.Add(Me.tbRounds)
        Me.GroupBox1.Controls.Add(Me.cbPET)
        Me.GroupBox1.Location = New System.Drawing.Point(408, 44)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(2)
        Me.GroupBox1.Size = New System.Drawing.Size(136, 164)
        Me.GroupBox1.TabIndex = 19
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Split Season Controls"
        '
        'lbRounds
        '
        Me.lbRounds.AutoSize = True
        Me.lbRounds.Location = New System.Drawing.Point(26, 70)
        Me.lbRounds.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbRounds.Name = "lbRounds"
        Me.lbRounds.Size = New System.Drawing.Size(44, 13)
        Me.lbRounds.TabIndex = 16
        Me.lbRounds.Text = "Rounds"
        '
        'tbRounds
        '
        Me.tbRounds.Location = New System.Drawing.Point(24, 84)
        Me.tbRounds.Margin = New System.Windows.Forms.Padding(2)
        Me.tbRounds.Name = "tbRounds"
        Me.tbRounds.Size = New System.Drawing.Size(47, 20)
        Me.tbRounds.TabIndex = 15
        '
        'btnCalcSch
        '
        Me.btnCalcSch.Location = New System.Drawing.Point(62, 136)
        Me.btnCalcSch.Margin = New System.Windows.Forms.Padding(2)
        Me.btnCalcSch.Name = "btnCalcSch"
        Me.btnCalcSch.Size = New System.Drawing.Size(116, 25)
        Me.btnCalcSch.TabIndex = 17
        Me.btnCalcSch.Text = "Calc Schedule"
        Me.btnCalcSch.UseVisualStyleBackColor = True
        '
        'ScheduleBuilder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1515, 873)
        Me.Controls.Add(Me.btnCalcSch)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.gbPR)
        Me.Controls.Add(Me.gbControls)
        Me.Controls.Add(Me.dgTeams)
        Me.Controls.Add(Me.tbEnd)
        Me.Controls.Add(Me.tbStart)
        Me.Controls.Add(Me.btnEmail)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.dgSchedule)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "ScheduleBuilder"
        Me.Text = "ScheduleBuilder"
        CType(Me.dgSchedule, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgTeams, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbControls.ResumeLayout(False)
        Me.gbControls.PerformLayout()
        Me.gbPR.ResumeLayout(False)
        Me.gbPR.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

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
