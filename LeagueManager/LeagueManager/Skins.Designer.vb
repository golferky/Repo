<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Skins
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Skins))
        Me.btnSkins = New System.Windows.Forms.Button()
        Me.cbDatesPlayers = New System.Windows.Forms.ComboBox()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.lbPS = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.rbDots = New System.Windows.Forms.RadioButton()
        Me.rbColors = New System.Windows.Forms.RadioButton()
        Me.lbSkinFormat = New System.Windows.Forms.Label()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.tbCP2 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbPurse = New System.Windows.Forms.TextBox()
        Me.tbCP1 = New System.Windows.Forms.TextBox()
        Me.tbSkins = New System.Windows.Forms.TextBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.tbLOCP2 = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.tbExtra = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.tbLOPurse = New System.Windows.Forms.TextBox()
        Me.tbLOCP1 = New System.Windows.Forms.TextBox()
        Me.tbLOSkins = New System.Windows.Forms.TextBox()
        Me.dgScores = New System.Windows.Forms.DataGridView()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnSkins
        '
        Me.btnSkins.Location = New System.Drawing.Point(2018, 23)
        Me.btnSkins.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.btnSkins.Name = "btnSkins"
        Me.btnSkins.Size = New System.Drawing.Size(208, 48)
        Me.btnSkins.TabIndex = 80
        Me.btnSkins.Text = "Calc Skins"
        Me.btnSkins.UseVisualStyleBackColor = True
        '
        'cbDatesPlayers
        '
        Me.cbDatesPlayers.FormattingEnabled = True
        Me.cbDatesPlayers.Location = New System.Drawing.Point(1656, 62)
        Me.cbDatesPlayers.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.cbDatesPlayers.Name = "cbDatesPlayers"
        Me.cbDatesPlayers.Size = New System.Drawing.Size(226, 33)
        Me.cbDatesPlayers.TabIndex = 77
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(2018, 179)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(212, 48)
        Me.btnExit.TabIndex = 75
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'lbPS
        '
        Me.lbPS.AutoSize = True
        Me.lbPS.Location = New System.Drawing.Point(1738, 23)
        Me.lbPS.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbPS.Name = "lbPS"
        Me.lbPS.Size = New System.Drawing.Size(57, 25)
        Me.lbPS.TabIndex = 70
        Me.lbPS.Text = "Date"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.rbDots)
        Me.GroupBox3.Controls.Add(Me.rbColors)
        Me.GroupBox3.Location = New System.Drawing.Point(72, 17)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox3.Size = New System.Drawing.Size(216, 140)
        Me.GroupBox3.TabIndex = 81
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Strokes Identifier"
        '
        'rbDots
        '
        Me.rbDots.AutoSize = True
        Me.rbDots.Checked = True
        Me.rbDots.Location = New System.Drawing.Point(60, 37)
        Me.rbDots.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.rbDots.Name = "rbDots"
        Me.rbDots.Size = New System.Drawing.Size(87, 29)
        Me.rbDots.TabIndex = 1
        Me.rbDots.TabStop = True
        Me.rbDots.Text = "Dots"
        Me.rbDots.UseVisualStyleBackColor = True
        '
        'rbColors
        '
        Me.rbColors.AutoSize = True
        Me.rbColors.Location = New System.Drawing.Point(60, 81)
        Me.rbColors.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.rbColors.Name = "rbColors"
        Me.rbColors.Size = New System.Drawing.Size(105, 29)
        Me.rbColors.TabIndex = 60
        Me.rbColors.Text = "Colors"
        Me.rbColors.UseVisualStyleBackColor = True
        '
        'lbSkinFormat
        '
        Me.lbSkinFormat.AutoSize = True
        Me.lbSkinFormat.Location = New System.Drawing.Point(40, 60)
        Me.lbSkinFormat.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbSkinFormat.Name = "lbSkinFormat"
        Me.lbSkinFormat.Size = New System.Drawing.Size(193, 25)
        Me.lbSkinFormat.TabIndex = 82
        Me.lbSkinFormat.Text = "LeagueSkinFormat"
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(22, 219)
        Me.lbStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(167, 25)
        Me.lbStatus.TabIndex = 84
        Me.lbStatus.Text = "Status Message"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lbSkinFormat)
        Me.GroupBox1.Location = New System.Drawing.Point(324, 23)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox1.Size = New System.Drawing.Size(260, 135)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Skins Format"
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(2018, 100)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(208, 48)
        Me.btnSave.TabIndex = 85
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.tbCP2)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.tbPurse)
        Me.GroupBox2.Controls.Add(Me.tbCP1)
        Me.GroupBox2.Controls.Add(Me.tbSkins)
        Me.GroupBox2.Location = New System.Drawing.Point(696, 17)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox2.Size = New System.Drawing.Size(370, 290)
        Me.GroupBox2.TabIndex = 86
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Prize Money This Week"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(28, 138)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(53, 25)
        Me.Label8.TabIndex = 75
        Me.Label8.Text = "CP2"
        '
        'tbCP2
        '
        Me.tbCP2.Location = New System.Drawing.Point(244, 133)
        Me.tbCP2.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbCP2.Name = "tbCP2"
        Me.tbCP2.ReadOnly = True
        Me.tbCP2.Size = New System.Drawing.Size(74, 31)
        Me.tbCP2.TabIndex = 74
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(28, 185)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(122, 25)
        Me.Label3.TabIndex = 73
        Me.Label3.Text = "Total Purse"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(28, 94)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 25)
        Me.Label2.TabIndex = 72
        Me.Label2.Text = "CP1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(28, 48)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 25)
        Me.Label1.TabIndex = 71
        Me.Label1.Text = "Skins"
        '
        'tbPurse
        '
        Me.tbPurse.Location = New System.Drawing.Point(244, 179)
        Me.tbPurse.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbPurse.Name = "tbPurse"
        Me.tbPurse.ReadOnly = True
        Me.tbPurse.Size = New System.Drawing.Size(74, 31)
        Me.tbPurse.TabIndex = 2
        '
        'tbCP1
        '
        Me.tbCP1.Location = New System.Drawing.Point(244, 88)
        Me.tbCP1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbCP1.Name = "tbCP1"
        Me.tbCP1.ReadOnly = True
        Me.tbCP1.Size = New System.Drawing.Size(74, 31)
        Me.tbCP1.TabIndex = 1
        '
        'tbSkins
        '
        Me.tbSkins.Location = New System.Drawing.Point(244, 40)
        Me.tbSkins.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbSkins.Name = "tbSkins"
        Me.tbSkins.ReadOnly = True
        Me.tbSkins.Size = New System.Drawing.Size(74, 31)
        Me.tbSkins.TabIndex = 0
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label9)
        Me.GroupBox4.Controls.Add(Me.tbLOCP2)
        Me.GroupBox4.Controls.Add(Me.Label7)
        Me.GroupBox4.Controls.Add(Me.tbExtra)
        Me.GroupBox4.Controls.Add(Me.Label4)
        Me.GroupBox4.Controls.Add(Me.Label5)
        Me.GroupBox4.Controls.Add(Me.Label6)
        Me.GroupBox4.Controls.Add(Me.tbLOPurse)
        Me.GroupBox4.Controls.Add(Me.tbLOCP1)
        Me.GroupBox4.Controls.Add(Me.tbLOSkins)
        Me.GroupBox4.Location = New System.Drawing.Point(1170, 17)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.GroupBox4.Size = New System.Drawing.Size(370, 290)
        Me.GroupBox4.TabIndex = 87
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Carryover to Next Week"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(38, 129)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(53, 25)
        Me.Label9.TabIndex = 77
        Me.Label9.Text = "CP2"
        '
        'tbLOCP2
        '
        Me.tbLOCP2.Location = New System.Drawing.Point(254, 123)
        Me.tbLOCP2.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbLOCP2.Name = "tbLOCP2"
        Me.tbLOCP2.ReadOnly = True
        Me.tbLOCP2.Size = New System.Drawing.Size(74, 31)
        Me.tbLOCP2.TabIndex = 76
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(38, 175)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(133, 25)
        Me.Label7.TabIndex = 75
        Me.Label7.Text = "Extra Money"
        '
        'tbExtra
        '
        Me.tbExtra.Location = New System.Drawing.Point(254, 173)
        Me.tbExtra.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbExtra.Name = "tbExtra"
        Me.tbExtra.ReadOnly = True
        Me.tbExtra.Size = New System.Drawing.Size(74, 31)
        Me.tbExtra.TabIndex = 74
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(38, 229)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(164, 25)
        Me.Label4.TabIndex = 73
        Me.Label4.Text = "Total CarryOver"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(38, 85)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(53, 25)
        Me.Label5.TabIndex = 72
        Me.Label5.Text = "CP1"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(38, 37)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 25)
        Me.Label6.TabIndex = 71
        Me.Label6.Text = "Skins"
        '
        'tbLOPurse
        '
        Me.tbLOPurse.Location = New System.Drawing.Point(254, 223)
        Me.tbLOPurse.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbLOPurse.Name = "tbLOPurse"
        Me.tbLOPurse.ReadOnly = True
        Me.tbLOPurse.Size = New System.Drawing.Size(74, 31)
        Me.tbLOPurse.TabIndex = 2
        '
        'tbLOCP1
        '
        Me.tbLOCP1.Location = New System.Drawing.Point(254, 79)
        Me.tbLOCP1.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbLOCP1.Name = "tbLOCP1"
        Me.tbLOCP1.ReadOnly = True
        Me.tbLOCP1.Size = New System.Drawing.Size(74, 31)
        Me.tbLOCP1.TabIndex = 1
        '
        'tbLOSkins
        '
        Me.tbLOSkins.Location = New System.Drawing.Point(254, 29)
        Me.tbLOSkins.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbLOSkins.Name = "tbLOSkins"
        Me.tbLOSkins.ReadOnly = True
        Me.tbLOSkins.Size = New System.Drawing.Size(74, 31)
        Me.tbLOSkins.TabIndex = 0
        '
        'dgScores
        '
        Me.dgScores.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgScores.Location = New System.Drawing.Point(-10, 319)
        Me.dgScores.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.dgScores.Name = "dgScores"
        Me.dgScores.Size = New System.Drawing.Size(2286, 1352)
        Me.dgScores.TabIndex = 0
        '
        'Skins
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(2270, 1302)
        Me.Controls.Add(Me.dgScores)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.btnSkins)
        Me.Controls.Add(Me.cbDatesPlayers)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.lbPS)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Name = "Skins"
        Me.Text = "Skins and Closest to Pin"
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSkins As Button
    Friend WithEvents cbDatesPlayers As ComboBox
    Friend WithEvents btnExit As Button
    Friend WithEvents lbPS As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents rbDots As RadioButton
    Friend WithEvents rbColors As RadioButton
    Friend WithEvents lbSkinFormat As Label
    Friend WithEvents lbStatus As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnSave As Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbPurse As TextBox
    Friend WithEvents tbCP1 As TextBox
    Friend WithEvents tbSkins As TextBox
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents tbLOPurse As TextBox
    Friend WithEvents tbLOCP1 As TextBox
    Friend WithEvents tbLOSkins As TextBox
    Friend WithEvents dgScores As DataGridView
    Friend WithEvents Label7 As Label
    Friend WithEvents tbExtra As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents tbCP2 As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents tbLOCP2 As TextBox
End Class
