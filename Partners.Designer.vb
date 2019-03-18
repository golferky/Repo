<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPartners
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
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.btnReset = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ListView3 = New System.Windows.Forms.ListView()
        Me.btnLink = New System.Windows.Forms.Button()
        Me.cbAplayers = New System.Windows.Forms.ComboBox()
        Me.cbBPlayers = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'btnSubmit
        '
        Me.btnSubmit.Location = New System.Drawing.Point(88, 148)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(124, 28)
        Me.btnSubmit.TabIndex = 6
        Me.btnSubmit.Text = "Submit Partners"
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'btnReset
        '
        Me.btnReset.Location = New System.Drawing.Point(423, 148)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(124, 28)
        Me.btnReset.TabIndex = 11
        Me.btnReset.Text = "Reset Partners"
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(161, 50)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(68, 17)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "A Players"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(402, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(68, 17)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "B Players"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(269, 213)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(62, 17)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "Partners"
        '
        'ListView3
        '
        Me.ListView3.FullRowSelect = True
        Me.ListView3.Location = New System.Drawing.Point(88, 245)
        Me.ListView3.Name = "ListView3"
        Me.ListView3.Size = New System.Drawing.Size(459, 156)
        Me.ListView3.TabIndex = 16
        Me.ListView3.UseCompatibleStateImageBehavior = False
        Me.ListView3.View = System.Windows.Forms.View.Details
        '
        'btnLink
        '
        Me.btnLink.Location = New System.Drawing.Point(260, 148)
        Me.btnLink.Name = "btnLink"
        Me.btnLink.Size = New System.Drawing.Size(124, 28)
        Me.btnLink.TabIndex = 18
        Me.btnLink.Text = "Link Partners"
        Me.btnLink.UseVisualStyleBackColor = True
        '
        'cbAplayers
        '
        Me.cbAplayers.FormattingEnabled = True
        Me.cbAplayers.Location = New System.Drawing.Point(88, 82)
        Me.cbAplayers.Name = "cbAplayers"
        Me.cbAplayers.Size = New System.Drawing.Size(214, 24)
        Me.cbAplayers.TabIndex = 19
        '
        'cbBPlayers
        '
        Me.cbBPlayers.FormattingEnabled = True
        Me.cbBPlayers.Location = New System.Drawing.Point(333, 82)
        Me.cbBPlayers.Name = "cbBPlayers"
        Me.cbBPlayers.Size = New System.Drawing.Size(214, 24)
        Me.cbBPlayers.TabIndex = 20
        '
        'frmPartners
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(670, 616)
        Me.Controls.Add(Me.cbBPlayers)
        Me.Controls.Add(Me.cbAplayers)
        Me.Controls.Add(Me.btnLink)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ListView3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.btnSubmit)
        Me.Name = "frmPartners"
        Me.Text = "Partners"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSubmit As System.Windows.Forms.Button
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ListView3 As System.Windows.Forms.ListView
    Friend WithEvents btnLink As System.Windows.Forms.Button
    Friend WithEvents cbAplayers As System.Windows.Forms.ComboBox
    Friend WithEvents cbBPlayers As System.Windows.Forms.ComboBox
End Class
