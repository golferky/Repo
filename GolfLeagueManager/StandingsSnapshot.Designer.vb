<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StandingsSnapshot
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
        Me.dgTeams = New System.Windows.Forms.DataGridView()
        Me.dgTeams2 = New System.Windows.Forms.DataGridView()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.lbPS = New System.Windows.Forms.Label()
        Me.gb2ndHalf = New System.Windows.Forms.GroupBox()
        Me.gb1stHalf = New System.Windows.Forms.GroupBox()
        CType(Me.dgTeams, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgTeams2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gb2ndHalf.SuspendLayout()
        Me.gb1stHalf.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgTeams
        '
        Me.dgTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTeams.Location = New System.Drawing.Point(5, 14)
        Me.dgTeams.Name = "dgTeams"
        Me.dgTeams.Size = New System.Drawing.Size(386, 226)
        Me.dgTeams.TabIndex = 2
        '
        'dgTeams2
        '
        Me.dgTeams2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTeams2.Location = New System.Drawing.Point(6, 15)
        Me.dgTeams2.Name = "dgTeams2"
        Me.dgTeams2.Size = New System.Drawing.Size(386, 225)
        Me.dgTeams2.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(266, 46)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Second Half"
        '
        'cbDates
        '
        Me.cbDates.FormattingEnabled = True
        Me.cbDates.Location = New System.Drawing.Point(30, 73)
        Me.cbDates.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(81, 21)
        Me.cbDates.TabIndex = 78
        Me.cbDates.TabStop = False
        '
        'lbPS
        '
        Me.lbPS.AutoSize = True
        Me.lbPS.Location = New System.Drawing.Point(32, 56)
        Me.lbPS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbPS.Name = "lbPS"
        Me.lbPS.Size = New System.Drawing.Size(61, 13)
        Me.lbPS.TabIndex = 77
        Me.lbPS.Text = "Score Date"
        '
        'gb2ndHalf
        '
        Me.gb2ndHalf.Controls.Add(Me.dgTeams2)
        Me.gb2ndHalf.Controls.Add(Me.Label2)
        Me.gb2ndHalf.Location = New System.Drawing.Point(561, 56)
        Me.gb2ndHalf.Name = "gb2ndHalf"
        Me.gb2ndHalf.Size = New System.Drawing.Size(400, 272)
        Me.gb2ndHalf.TabIndex = 79
        Me.gb2ndHalf.TabStop = False
        Me.gb2ndHalf.Text = "Second Half"
        '
        'gb1stHalf
        '
        Me.gb1stHalf.Controls.Add(Me.dgTeams)
        Me.gb1stHalf.Location = New System.Drawing.Point(142, 56)
        Me.gb1stHalf.Name = "gb1stHalf"
        Me.gb1stHalf.Size = New System.Drawing.Size(397, 272)
        Me.gb1stHalf.TabIndex = 80
        Me.gb1stHalf.TabStop = False
        Me.gb1stHalf.Text = "First Half"
        '
        'StandingsSnapshot
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(997, 339)
        Me.Controls.Add(Me.gb1stHalf)
        Me.Controls.Add(Me.gb2ndHalf)
        Me.Controls.Add(Me.cbDates)
        Me.Controls.Add(Me.lbPS)
        Me.Name = "StandingsSnapshot"
        Me.Text = "Standings Snapshot"
        CType(Me.dgTeams, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgTeams2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gb2ndHalf.ResumeLayout(False)
        Me.gb2ndHalf.PerformLayout()
        Me.gb1stHalf.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dgTeams As DataGridView
    Friend WithEvents dgTeams2 As DataGridView
    Friend WithEvents Label2 As Label
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents lbPS As Label
    Friend WithEvents gb2ndHalf As GroupBox
    Friend WithEvents gb1stHalf As GroupBox
End Class
