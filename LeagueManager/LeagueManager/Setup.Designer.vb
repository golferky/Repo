<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Setup
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
        Me.btnLeagueSetup = New System.Windows.Forms.Button()
        Me.btnPlayerSetup = New System.Windows.Forms.Button()
        Me.btnCourseSetup = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnSchedule = New System.Windows.Forms.Button()
        Me.btnScoresSetup = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnLeagueSetup
        '
        Me.btnLeagueSetup.Location = New System.Drawing.Point(33, 42)
        Me.btnLeagueSetup.Name = "btnLeagueSetup"
        Me.btnLeagueSetup.Size = New System.Drawing.Size(144, 23)
        Me.btnLeagueSetup.TabIndex = 25
        Me.btnLeagueSetup.Text = "League"
        Me.btnLeagueSetup.UseVisualStyleBackColor = True
        '
        'btnPlayerSetup
        '
        Me.btnPlayerSetup.Location = New System.Drawing.Point(33, 104)
        Me.btnPlayerSetup.Name = "btnPlayerSetup"
        Me.btnPlayerSetup.Size = New System.Drawing.Size(144, 23)
        Me.btnPlayerSetup.TabIndex = 26
        Me.btnPlayerSetup.Text = "Player"
        Me.btnPlayerSetup.UseVisualStyleBackColor = True
        '
        'btnCourseSetup
        '
        Me.btnCourseSetup.Location = New System.Drawing.Point(33, 166)
        Me.btnCourseSetup.Name = "btnCourseSetup"
        Me.btnCourseSetup.Size = New System.Drawing.Size(144, 23)
        Me.btnCourseSetup.TabIndex = 27
        Me.btnCourseSetup.Text = "Course"
        Me.btnCourseSetup.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnSchedule)
        Me.GroupBox1.Controls.Add(Me.btnScoresSetup)
        Me.GroupBox1.Controls.Add(Me.btnLeagueSetup)
        Me.GroupBox1.Controls.Add(Me.btnCourseSetup)
        Me.GroupBox1.Controls.Add(Me.btnPlayerSetup)
        Me.GroupBox1.Location = New System.Drawing.Point(49, 9)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(200, 357)
        Me.GroupBox1.TabIndex = 28
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Setups"
        '
        'btnSchedule
        '
        Me.btnSchedule.Location = New System.Drawing.Point(33, 228)
        Me.btnSchedule.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSchedule.Name = "btnSchedule"
        Me.btnSchedule.Size = New System.Drawing.Size(144, 24)
        Me.btnSchedule.TabIndex = 29
        Me.btnSchedule.Text = "Schedule"
        Me.btnSchedule.UseVisualStyleBackColor = True
        '
        'btnScoresSetup
        '
        Me.btnScoresSetup.Location = New System.Drawing.Point(33, 291)
        Me.btnScoresSetup.Name = "btnScoresSetup"
        Me.btnScoresSetup.Size = New System.Drawing.Size(144, 23)
        Me.btnScoresSetup.TabIndex = 28
        Me.btnScoresSetup.Text = "Scores Setup"
        Me.btnScoresSetup.UseVisualStyleBackColor = True
        Me.btnScoresSetup.Visible = False
        '
        'Setup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(304, 385)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Setup"
        Me.Text = "Setup"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnLeagueSetup As Button
    Friend WithEvents btnPlayerSetup As Button
    Friend WithEvents btnCourseSetup As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnScoresSetup As Button
    Friend WithEvents btnSchedule As Button
End Class
