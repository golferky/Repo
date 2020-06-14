<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LeaderChoices
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
        Me.rbLeagueStats = New System.Windows.Forms.RadioButton()
        Me.RadioButton1 = New System.Windows.Forms.RadioButton()
        Me.rbBirdies = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'rbLeagueStats
        '
        Me.rbLeagueStats.AutoSize = True
        Me.rbLeagueStats.Location = New System.Drawing.Point(79, 68)
        Me.rbLeagueStats.Name = "rbLeagueStats"
        Me.rbLeagueStats.Size = New System.Drawing.Size(88, 17)
        Me.rbLeagueStats.TabIndex = 0
        Me.rbLeagueStats.TabStop = True
        Me.rbLeagueStats.Text = "League Stats"
        Me.rbLeagueStats.UseVisualStyleBackColor = True
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Location = New System.Drawing.Point(79, 152)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(92, 17)
        Me.RadioButton1.TabIndex = 2
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "Birdie Leaders"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'rbBirdies
        '
        Me.rbBirdies.AutoSize = True
        Me.rbBirdies.Location = New System.Drawing.Point(79, 110)
        Me.rbBirdies.Name = "rbBirdies"
        Me.rbBirdies.Size = New System.Drawing.Size(92, 17)
        Me.rbBirdies.TabIndex = 1
        Me.rbBirdies.TabStop = True
        Me.rbBirdies.Text = "Birdie Leaders"
        Me.rbBirdies.UseVisualStyleBackColor = True
        '
        'LeaderChoices
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.RadioButton1)
        Me.Controls.Add(Me.rbBirdies)
        Me.Controls.Add(Me.rbLeagueStats)
        Me.Name = "LeaderChoices"
        Me.Text = "LeaderChoices"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents rbLeagueStats As RadioButton
    Friend WithEvents RadioButton1 As RadioButton
    Friend WithEvents rbBirdies As RadioButton
End Class
