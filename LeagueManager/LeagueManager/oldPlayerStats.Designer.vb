<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class oldfrmPlayerStats
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
        Me.cbPlayer = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cbCourse = New System.Windows.Forms.ComboBox()
        Me.btnDisplayStats = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cbPlayer
        '
        Me.cbPlayer.FormattingEnabled = True
        Me.cbPlayer.Location = New System.Drawing.Point(44, 48)
        Me.cbPlayer.Name = "cbPlayer"
        Me.cbPlayer.Size = New System.Drawing.Size(222, 24)
        Me.cbPlayer.Sorted = True
        Me.cbPlayer.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(44, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(89, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Player Name"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(296, 24)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 17)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Course"
        '
        'cbCourse
        '
        Me.cbCourse.FormattingEnabled = True
        Me.cbCourse.Location = New System.Drawing.Point(299, 47)
        Me.cbCourse.Name = "cbCourse"
        Me.cbCourse.Size = New System.Drawing.Size(233, 24)
        Me.cbCourse.Sorted = True
        Me.cbCourse.TabIndex = 6
        '
        'btnDisplayStats
        '
        Me.btnDisplayStats.Location = New System.Drawing.Point(599, 35)
        Me.btnDisplayStats.Name = "btnDisplayStats"
        Me.btnDisplayStats.Size = New System.Drawing.Size(172, 46)
        Me.btnDisplayStats.TabIndex = 8
        Me.btnDisplayStats.Text = "display stats"
        Me.btnDisplayStats.UseVisualStyleBackColor = True
        '
        'frmPlayerStats
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1266, 737)
        Me.Controls.Add(Me.btnDisplayStats)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cbCourse)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbPlayer)
        Me.Name = "frmPlayerStats"
        Me.Text = "PlayerStats"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cbPlayer As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cbCourse As System.Windows.Forms.ComboBox
    Friend WithEvents btnDisplayStats As System.Windows.Forms.Button
End Class
