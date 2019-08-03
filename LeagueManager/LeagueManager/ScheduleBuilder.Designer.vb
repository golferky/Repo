<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ScheduleBuilder
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
        Me.tbRounds = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
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
        Me.dgSchedule.Location = New System.Drawing.Point(44, 937)
        Me.dgSchedule.Name = "dgSchedule"
        Me.dgSchedule.RowTemplate.Height = 33
        Me.dgSchedule.Size = New System.Drawing.Size(2857, 563)
        Me.dgSchedule.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(39, 53)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(108, 25)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Start Date"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(287, 53)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(101, 25)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "End Date"
        '
        'cbPR
        '
        Me.cbPR.AutoSize = True
        Me.cbPR.Location = New System.Drawing.Point(32, 178)
        Me.cbPR.Name = "cbPR"
        Me.cbPR.Size = New System.Drawing.Size(201, 29)
        Me.cbPR.TabIndex = 7
        Me.cbPR.Text = "Position Rounds"
        Me.cbPR.UseVisualStyleBackColor = True
        Me.cbPR.Visible = False
        '
        'cbSplitRounds
        '
        Me.cbSplitRounds.AutoSize = True
        Me.cbSplitRounds.Location = New System.Drawing.Point(32, 58)
        Me.cbSplitRounds.Name = "cbSplitRounds"
        Me.cbSplitRounds.Size = New System.Drawing.Size(165, 29)
        Me.cbSplitRounds.TabIndex = 8
        Me.cbSplitRounds.Text = "Split Season"
        Me.cbSplitRounds.UseVisualStyleBackColor = True
        '
        'cbPET
        '
        Me.cbPET.AutoSize = True
        Me.cbPET.Location = New System.Drawing.Point(11, 69)
        Me.cbPET.Name = "cbPET"
        Me.cbPET.Size = New System.Drawing.Size(199, 29)
        Me.cbPET.TabIndex = 9
        Me.cbPET.Text = "Play each Team"
        Me.cbPET.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(125, 281)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(233, 49)
        Me.btnExit.TabIndex = 10
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(212, 381)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(73, 25)
        Me.lbStatus.TabIndex = 11
        Me.lbStatus.Text = "Status"
        '
        'btnEmail
        '
        Me.btnEmail.Location = New System.Drawing.Point(125, 162)
        Me.btnEmail.Name = "btnEmail"
        Me.btnEmail.Size = New System.Drawing.Size(233, 49)
        Me.btnEmail.TabIndex = 12
        Me.btnEmail.Text = "Email Schedule"
        Me.btnEmail.UseVisualStyleBackColor = True
        '
        'cbPS
        '
        Me.cbPS.AutoSize = True
        Me.cbPS.Location = New System.Drawing.Point(32, 117)
        Me.cbPS.Name = "cbPS"
        Me.cbPS.Size = New System.Drawing.Size(166, 29)
        Me.cbPS.TabIndex = 13
        Me.cbPS.Text = "Post Season"
        Me.cbPS.UseVisualStyleBackColor = True
        '
        'tbStart
        '
        Me.tbStart.Location = New System.Drawing.Point(42, 84)
        Me.tbStart.Name = "tbStart"
        Me.tbStart.Size = New System.Drawing.Size(150, 31)
        Me.tbStart.TabIndex = 14
        '
        'tbEnd
        '
        Me.tbEnd.Location = New System.Drawing.Point(290, 84)
        Me.tbEnd.Name = "tbEnd"
        Me.tbEnd.Size = New System.Drawing.Size(150, 31)
        Me.tbEnd.TabIndex = 15
        '
        'dgTeams
        '
        Me.dgTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTeams.Location = New System.Drawing.Point(1304, 39)
        Me.dgTeams.Name = "dgTeams"
        Me.dgTeams.RowTemplate.Height = 33
        Me.dgTeams.Size = New System.Drawing.Size(833, 808)
        Me.dgTeams.TabIndex = 16
        '
        'gbControls
        '
        Me.gbControls.Controls.Add(Me.cbSplitRounds)
        Me.gbControls.Controls.Add(Me.cbPR)
        Me.gbControls.Controls.Add(Me.cbPS)
        Me.gbControls.Location = New System.Drawing.Point(535, 84)
        Me.gbControls.Name = "gbControls"
        Me.gbControls.Size = New System.Drawing.Size(273, 235)
        Me.gbControls.TabIndex = 17
        Me.gbControls.TabStop = False
        Me.gbControls.Text = "Display Controls"
        '
        'gbPR
        '
        Me.gbPR.Controls.Add(Me.cbPRRS)
        Me.gbPR.Controls.Add(Me.cbPRAW)
        Me.gbPR.Location = New System.Drawing.Point(899, 84)
        Me.gbPR.Name = "gbPR"
        Me.gbPR.Size = New System.Drawing.Size(273, 235)
        Me.gbPR.TabIndex = 18
        Me.gbPR.TabStop = False
        Me.gbPR.Text = "Position Round Controls"
        '
        'cbPRRS
        '
        Me.cbPRRS.AutoSize = True
        Me.cbPRRS.Location = New System.Drawing.Point(32, 58)
        Me.cbPRRS.Name = "cbPRRS"
        Me.cbPRRS.Size = New System.Drawing.Size(126, 29)
        Me.cbPRRS.TabIndex = 8
        Me.cbPRRS.Text = "Reg Sch"
        Me.cbPRRS.UseVisualStyleBackColor = True
        '
        'cbPRAW
        '
        Me.cbPRAW.AutoSize = True
        Me.cbPRAW.Location = New System.Drawing.Point(32, 117)
        Me.cbPRAW.Name = "cbPRAW"
        Me.cbPRAW.Size = New System.Drawing.Size(143, 29)
        Me.cbPRAW.TabIndex = 9
        Me.cbPRAW.Text = "Add Week"
        Me.cbPRAW.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.tbRounds)
        Me.GroupBox1.Controls.Add(Me.cbPET)
        Me.GroupBox1.Location = New System.Drawing.Point(535, 368)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(273, 315)
        Me.GroupBox1.TabIndex = 19
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Split Season Controls"
        '
        'tbRounds
        '
        Me.tbRounds.Location = New System.Drawing.Point(47, 162)
        Me.tbRounds.Name = "tbRounds"
        Me.tbRounds.Size = New System.Drawing.Size(90, 31)
        Me.tbRounds.TabIndex = 15
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(51, 134)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(86, 25)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Rounds"
        '
        'ScheduleBuilder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(2942, 2246)
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
    Friend WithEvents Label3 As Label
    Friend WithEvents tbRounds As TextBox
End Class
