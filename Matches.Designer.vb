<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Matches
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Matches))
        Me.cbDatesPlayers = New System.Windows.Forms.ComboBox()
        Me.lbPS = New System.Windows.Forms.Label()
        Me.btnMatches = New System.Windows.Forms.Button()
        Me.dgScores = New System.Windows.Forms.DataGridView()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cbDatesPlayers
        '
        Me.cbDatesPlayers.FormattingEnabled = True
        Me.cbDatesPlayers.Location = New System.Drawing.Point(52, 43)
        Me.cbDatesPlayers.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDatesPlayers.Name = "cbDatesPlayers"
        Me.cbDatesPlayers.Size = New System.Drawing.Size(115, 21)
        Me.cbDatesPlayers.TabIndex = 66
        '
        'lbPS
        '
        Me.lbPS.AutoSize = True
        Me.lbPS.Location = New System.Drawing.Point(93, 23)
        Me.lbPS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbPS.Name = "lbPS"
        Me.lbPS.Size = New System.Drawing.Size(30, 13)
        Me.lbPS.TabIndex = 63
        Me.lbPS.Text = "Date"
        '
        'btnMatches
        '
        Me.btnMatches.Location = New System.Drawing.Point(601, 14)
        Me.btnMatches.Margin = New System.Windows.Forms.Padding(2)
        Me.btnMatches.Name = "btnMatches"
        Me.btnMatches.Size = New System.Drawing.Size(106, 30)
        Me.btnMatches.TabIndex = 69
        Me.btnMatches.Text = "Calc Matches"
        Me.btnMatches.UseVisualStyleBackColor = True
        '
        'dgScores
        '
        Me.dgScores.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgScores.Location = New System.Drawing.Point(48, 157)
        Me.dgScores.Name = "dgScores"
        Me.dgScores.Size = New System.Drawing.Size(659, 747)
        Me.dgScores.TabIndex = 70
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(601, 100)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(106, 30)
        Me.btnExit.TabIndex = 71
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(49, 90)
        Me.lbStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(37, 13)
        Me.lbStatus.TabIndex = 72
        Me.lbStatus.Text = "Status"
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(601, 59)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(106, 30)
        Me.btnSave.TabIndex = 73
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'Matches
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(756, 958)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.dgScores)
        Me.Controls.Add(Me.btnMatches)
        Me.Controls.Add(Me.cbDatesPlayers)
        Me.Controls.Add(Me.lbPS)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Matches"
        Me.Text = "Matches"
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbDatesPlayers As ComboBox
    Friend WithEvents lbPS As Label
    Friend WithEvents btnMatches As Button
    Friend WithEvents dgScores As DataGridView
    Friend WithEvents btnExit As Button
    Friend WithEvents lbStatus As Label
    Friend WithEvents btnSave As Button
End Class
