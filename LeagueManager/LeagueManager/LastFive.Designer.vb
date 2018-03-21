<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LastFive
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
        Me.btnDisplayScores = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'btnDisplayScores
        '
        Me.btnDisplayScores.Location = New System.Drawing.Point(272, 41)
        Me.btnDisplayScores.Margin = New System.Windows.Forms.Padding(2)
        Me.btnDisplayScores.Name = "btnDisplayScores"
        Me.btnDisplayScores.Size = New System.Drawing.Size(129, 37)
        Me.btnDisplayScores.TabIndex = 16
        Me.btnDisplayScores.Text = "Display Scores"
        Me.btnDisplayScores.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(35, 32)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Select Date"
        '
        'cbDates
        '
        Me.cbDates.FormattingEnabled = True
        Me.cbDates.Location = New System.Drawing.Point(35, 50)
        Me.cbDates.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(168, 21)
        Me.cbDates.Sorted = True
        Me.cbDates.TabIndex = 14
        '
        'LastFive
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(611, 789)
        Me.Controls.Add(Me.btnDisplayScores)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbDates)
        Me.Name = "LastFive"
        Me.Text = "LastFive"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnDisplayScores As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents cbDates As ComboBox
End Class
