<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPlayerStats
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
        Me.btnDisplayStats = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbPlayer = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'btnDisplayStats
        '
        Me.btnDisplayStats.Location = New System.Drawing.Point(268, 28)
        Me.btnDisplayStats.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnDisplayStats.Name = "btnDisplayStats"
        Me.btnDisplayStats.Size = New System.Drawing.Size(129, 37)
        Me.btnDisplayStats.TabIndex = 13
        Me.btnDisplayStats.Text = "display stats"
        Me.btnDisplayStats.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(31, 19)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(67, 13)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Player Name"
        '
        'cbPlayer
        '
        Me.cbPlayer.FormattingEnabled = True
        Me.cbPlayer.Location = New System.Drawing.Point(31, 37)
        Me.cbPlayer.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.cbPlayer.Name = "cbPlayer"
        Me.cbPlayer.Size = New System.Drawing.Size(168, 21)
        Me.cbPlayer.Sorted = True
        Me.cbPlayer.TabIndex = 9
        '
        'frmPlayerStats
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(944, 598)
        Me.Controls.Add(Me.btnDisplayStats)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbPlayer)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "frmPlayerStats"
        Me.Text = "PlayerStats"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnDisplayStats As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cbPlayer As System.Windows.Forms.ComboBox
End Class
