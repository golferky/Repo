<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHandicap
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
        Me.cbHPlayer = New System.Windows.Forms.ComboBox()
        Me.cbHCourse = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cbDate = New System.Windows.Forms.ComboBox()
        Me.cbPlayer = New System.Windows.Forms.ComboBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.btnShowScores = New System.Windows.Forms.Button()
        Me.txtScore = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnShowAllPlayers = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtLeagueName = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'cbHPlayer
        '
        Me.cbHPlayer.FormattingEnabled = True
        Me.cbHPlayer.Location = New System.Drawing.Point(6, 66)
        Me.cbHPlayer.Name = "cbHPlayer"
        Me.cbHPlayer.Size = New System.Drawing.Size(203, 21)
        Me.cbHPlayer.TabIndex = 3
        '
        'cbHCourse
        '
        Me.cbHCourse.FormattingEnabled = True
        Me.cbHCourse.Location = New System.Drawing.Point(270, 92)
        Me.cbHCourse.Name = "cbHCourse"
        Me.cbHCourse.Size = New System.Drawing.Size(186, 21)
        Me.cbHCourse.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(195, 72)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(30, 13)
        Me.Label5.TabIndex = 18
        Me.Label5.Text = "Date"
        '
        'cbDate
        '
        Me.cbDate.FormattingEnabled = True
        Me.cbDate.Location = New System.Drawing.Point(195, 90)
        Me.cbDate.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDate.Name = "cbDate"
        Me.cbDate.Size = New System.Drawing.Size(140, 21)
        Me.cbDate.TabIndex = 2
        '
        'cbPlayer
        '
        Me.cbPlayer.FormattingEnabled = True
        Me.cbPlayer.Location = New System.Drawing.Point(356, 90)
        Me.cbPlayer.Margin = New System.Windows.Forms.Padding(2)
        Me.cbPlayer.Name = "cbPlayer"
        Me.cbPlayer.Size = New System.Drawing.Size(153, 21)
        Me.cbPlayer.TabIndex = 3
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(354, 72)
        Me.Label9.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(35, 13)
        Me.Label9.TabIndex = 25
        Me.Label9.Text = "Name"
        '
        'btnShowScores
        '
        Me.btnShowScores.Location = New System.Drawing.Point(89, 124)
        Me.btnShowScores.Margin = New System.Windows.Forms.Padding(2)
        Me.btnShowScores.Name = "btnShowScores"
        Me.btnShowScores.Size = New System.Drawing.Size(125, 30)
        Me.btnShowScores.TabIndex = 26
        Me.btnShowScores.Text = "Show Detailed Scores"
        Me.btnShowScores.UseVisualStyleBackColor = True
        '
        'txtScore
        '
        Me.txtScore.Location = New System.Drawing.Point(143, 90)
        Me.txtScore.Margin = New System.Windows.Forms.Padding(2)
        Me.txtScore.Name = "txtScore"
        Me.txtScore.Size = New System.Drawing.Size(35, 20)
        Me.txtScore.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(141, 72)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(35, 13)
        Me.Label2.TabIndex = 28
        Me.Label2.Text = "Score"
        '
        'btnShowAllPlayers
        '
        Me.btnShowAllPlayers.Location = New System.Drawing.Point(261, 124)
        Me.btnShowAllPlayers.Margin = New System.Windows.Forms.Padding(2)
        Me.btnShowAllPlayers.Name = "btnShowAllPlayers"
        Me.btnShowAllPlayers.Size = New System.Drawing.Size(97, 30)
        Me.btnShowAllPlayers.TabIndex = 29
        Me.btnShowAllPlayers.Text = "All Players"
        Me.btnShowAllPlayers.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 72)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(43, 13)
        Me.Label1.TabIndex = 31
        Me.Label1.Text = "League"
        '
        'txtLeagueName
        '
        Me.txtLeagueName.Location = New System.Drawing.Point(11, 90)
        Me.txtLeagueName.Margin = New System.Windows.Forms.Padding(2)
        Me.txtLeagueName.Name = "txtLeagueName"
        Me.txtLeagueName.Size = New System.Drawing.Size(107, 20)
        Me.txtLeagueName.TabIndex = 32
        '
        'frmHandicap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 600)
        Me.Controls.Add(Me.txtLeagueName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnShowAllPlayers)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtScore)
        Me.Controls.Add(Me.btnShowScores)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.cbPlayer)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.cbDate)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmHandicap"
        Me.Text = "Handicap"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cbHPlayer As System.Windows.Forms.ComboBox
    Friend WithEvents cbHCourse As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cbDate As System.Windows.Forms.ComboBox
    Friend WithEvents cbPlayer As System.Windows.Forms.ComboBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents btnShowScores As System.Windows.Forms.Button
    Friend WithEvents txtScore As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnShowAllPlayers As System.Windows.Forms.Button
    Friend WithEvents Label1 As Label
    Friend WithEvents txtLeagueName As TextBox
End Class
