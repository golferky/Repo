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
        Me.btnExit = New System.Windows.Forms.Button()
        Me.dgTeams = New System.Windows.Forms.DataGridView()
        Me.dgTeams2 = New System.Windows.Forms.DataGridView()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.lbPS = New System.Windows.Forms.Label()
        Me.gb2ndHalf = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        CType(Me.dgTeams, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgTeams2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gb2ndHalf.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(36, 214)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(75, 23)
        Me.btnExit.TabIndex = 1
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'dgTeams
        '
        Me.dgTeams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTeams.Location = New System.Drawing.Point(7, 14)
        Me.dgTeams.Name = "dgTeams"
        Me.dgTeams.Size = New System.Drawing.Size(386, 245)
        Me.dgTeams.TabIndex = 2
        '
        'dgTeams2
        '
        Me.dgTeams2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgTeams2.Location = New System.Drawing.Point(0, 15)
        Me.dgTeams2.Name = "dgTeams2"
        Me.dgTeams2.Size = New System.Drawing.Size(386, 244)
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
        Me.cbDates.Location = New System.Drawing.Point(22, 135)
        Me.cbDates.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(115, 21)
        Me.cbDates.TabIndex = 78
        Me.cbDates.TabStop = False
        '
        'lbPS
        '
        Me.lbPS.AutoSize = True
        Me.lbPS.Location = New System.Drawing.Point(63, 118)
        Me.lbPS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbPS.Name = "lbPS"
        Me.lbPS.Size = New System.Drawing.Size(30, 13)
        Me.lbPS.TabIndex = 77
        Me.lbPS.Text = "Date"
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
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.dgTeams)
        Me.GroupBox2.Location = New System.Drawing.Point(142, 56)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(400, 272)
        Me.GroupBox2.TabIndex = 80
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "First Half"
        '
        'StandingsSnapshot
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(997, 339)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.gb2ndHalf)
        Me.Controls.Add(Me.cbDates)
        Me.Controls.Add(Me.lbPS)
        Me.Controls.Add(Me.btnExit)
        Me.Name = "StandingsSnapshot"
        Me.Text = "Standings Snapshot"
        CType(Me.dgTeams, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgTeams2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gb2ndHalf.ResumeLayout(False)
        Me.gb2ndHalf.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnExit As Button
    Friend WithEvents dgTeams As DataGridView
    Friend WithEvents dgTeams2 As DataGridView
    Friend WithEvents Label2 As Label
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents lbPS As Label
    Friend WithEvents gb2ndHalf As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
End Class
