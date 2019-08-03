<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmScoreCard
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmScoreCard))
        Me.lbPS = New System.Windows.Forms.Label()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.btnShowScores = New System.Windows.Forms.Button()
        Me.rbNet = New System.Windows.Forms.RadioButton()
        Me.rbGross = New System.Windows.Forms.RadioButton()
        Me.gbDefMeth = New System.Windows.Forms.GroupBox()
        Me.rbScore = New System.Windows.Forms.RadioButton()
        Me.cbDatesPlayers = New System.Windows.Forms.ComboBox()
        Me.gbDefGames = New System.Windows.Forms.GroupBox()
        Me.cbClosest = New System.Windows.Forms.CheckBox()
        Me.cbSkins = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.rbDots = New System.Windows.Forms.RadioButton()
        Me.rbColors = New System.Windows.Forms.RadioButton()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.tbExtra = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.tbCP2Tot = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.tbCP1Tot = New System.Windows.Forms.TextBox()
        Me.tbSkinTot = New System.Windows.Forms.TextBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.tbPCP2 = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.tbPCP1 = New System.Windows.Forms.TextBox()
        Me.tbPSkins = New System.Windows.Forms.TextBox()
        Me.tbPurse = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.tbCP1 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbCP2 = New System.Windows.Forms.TextBox()
        Me.tbSkins = New System.Windows.Forms.TextBox()
        Me.dgScores = New System.Windows.Forms.DataGridView()
        Me.cbScoresLocked = New System.Windows.Forms.CheckBox()
        Me.gbHoleLegend = New System.Windows.Forms.GroupBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.cbMarkPaid = New System.Windows.Forms.CheckBox()
        Me.cbMatches = New System.Windows.Forms.CheckBox()
        Me.gbFrontBack = New System.Windows.Forms.GroupBox()
        Me.rbFront = New System.Windows.Forms.RadioButton()
        Me.rbBack = New System.Windows.Forms.RadioButton()
        Me.lbParmFile = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.tspb = New System.Windows.Forms.ToolStripProgressBar()
        Me.tssl = New System.Windows.Forms.ToolStripStatusLabel()
        Me.gbDefMeth.SuspendLayout()
        Me.gbDefGames.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbHoleLegend.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.gbFrontBack.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbPS
        '
        Me.lbPS.AutoSize = True
        Me.lbPS.Location = New System.Drawing.Point(1758, 29)
        Me.lbPS.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbPS.Name = "lbPS"
        Me.lbPS.Size = New System.Drawing.Size(57, 25)
        Me.lbPS.TabIndex = 39
        Me.lbPS.Text = "Date"
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(2188, 194)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(4)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(212, 71)
        Me.btnExit.TabIndex = 57
        Me.btnExit.TabStop = False
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'btnShowScores
        '
        Me.btnShowScores.Location = New System.Drawing.Point(2188, 27)
        Me.btnShowScores.Margin = New System.Windows.Forms.Padding(4)
        Me.btnShowScores.Name = "btnShowScores"
        Me.btnShowScores.Size = New System.Drawing.Size(212, 71)
        Me.btnShowScores.TabIndex = 58
        Me.btnShowScores.TabStop = False
        Me.btnShowScores.Text = "Show Scores"
        Me.btnShowScores.UseVisualStyleBackColor = True
        '
        'rbNet
        '
        Me.rbNet.AutoSize = True
        Me.rbNet.Checked = True
        Me.rbNet.Location = New System.Drawing.Point(12, 77)
        Me.rbNet.Margin = New System.Windows.Forms.Padding(6)
        Me.rbNet.Name = "rbNet"
        Me.rbNet.Size = New System.Drawing.Size(76, 29)
        Me.rbNet.TabIndex = 1
        Me.rbNet.TabStop = True
        Me.rbNet.Text = "Net"
        Me.rbNet.UseVisualStyleBackColor = True
        '
        'rbGross
        '
        Me.rbGross.AutoSize = True
        Me.rbGross.Location = New System.Drawing.Point(12, 121)
        Me.rbGross.Margin = New System.Windows.Forms.Padding(6)
        Me.rbGross.Name = "rbGross"
        Me.rbGross.Size = New System.Drawing.Size(100, 29)
        Me.rbGross.TabIndex = 60
        Me.rbGross.Text = "Gross"
        Me.rbGross.UseVisualStyleBackColor = True
        '
        'gbDefMeth
        '
        Me.gbDefMeth.Controls.Add(Me.rbScore)
        Me.gbDefMeth.Controls.Add(Me.rbNet)
        Me.gbDefMeth.Controls.Add(Me.rbGross)
        Me.gbDefMeth.Location = New System.Drawing.Point(24, 17)
        Me.gbDefMeth.Margin = New System.Windows.Forms.Padding(6)
        Me.gbDefMeth.Name = "gbDefMeth"
        Me.gbDefMeth.Padding = New System.Windows.Forms.Padding(6)
        Me.gbDefMeth.Size = New System.Drawing.Size(160, 231)
        Me.gbDefMeth.TabIndex = 61
        Me.gbDefMeth.TabStop = False
        Me.gbDefMeth.Text = "Score Method"
        '
        'rbScore
        '
        Me.rbScore.AutoSize = True
        Me.rbScore.Location = New System.Drawing.Point(12, 165)
        Me.rbScore.Margin = New System.Windows.Forms.Padding(6)
        Me.rbScore.Name = "rbScore"
        Me.rbScore.Size = New System.Drawing.Size(99, 29)
        Me.rbScore.TabIndex = 61
        Me.rbScore.Text = "Score"
        Me.rbScore.UseVisualStyleBackColor = True
        '
        'cbDatesPlayers
        '
        Me.cbDatesPlayers.FormattingEnabled = True
        Me.cbDatesPlayers.Location = New System.Drawing.Point(1676, 62)
        Me.cbDatesPlayers.Margin = New System.Windows.Forms.Padding(4)
        Me.cbDatesPlayers.Name = "cbDatesPlayers"
        Me.cbDatesPlayers.Size = New System.Drawing.Size(226, 33)
        Me.cbDatesPlayers.TabIndex = 76
        Me.cbDatesPlayers.TabStop = False
        '
        'gbDefGames
        '
        Me.gbDefGames.Controls.Add(Me.cbClosest)
        Me.gbDefGames.Controls.Add(Me.cbSkins)
        Me.gbDefGames.Location = New System.Drawing.Point(620, 173)
        Me.gbDefGames.Margin = New System.Windows.Forms.Padding(6)
        Me.gbDefGames.Name = "gbDefGames"
        Me.gbDefGames.Padding = New System.Windows.Forms.Padding(6)
        Me.gbDefGames.Size = New System.Drawing.Size(208, 140)
        Me.gbDefGames.TabIndex = 63
        Me.gbDefGames.TabStop = False
        Me.gbDefGames.Text = "Default Games"
        Me.gbDefGames.Visible = False
        '
        'cbClosest
        '
        Me.cbClosest.AutoSize = True
        Me.cbClosest.Location = New System.Drawing.Point(22, 87)
        Me.cbClosest.Margin = New System.Windows.Forms.Padding(6)
        Me.cbClosest.Name = "cbClosest"
        Me.cbClosest.Size = New System.Drawing.Size(116, 29)
        Me.cbClosest.TabIndex = 61
        Me.cbClosest.TabStop = False
        Me.cbClosest.Text = "Closest"
        Me.cbClosest.UseVisualStyleBackColor = True
        '
        'cbSkins
        '
        Me.cbSkins.AutoSize = True
        Me.cbSkins.Location = New System.Drawing.Point(22, 42)
        Me.cbSkins.Margin = New System.Windows.Forms.Padding(6)
        Me.cbSkins.Name = "cbSkins"
        Me.cbSkins.Size = New System.Drawing.Size(97, 29)
        Me.cbSkins.TabIndex = 60
        Me.cbSkins.TabStop = False
        Me.cbSkins.Text = "Skins"
        Me.cbSkins.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.rbDots)
        Me.GroupBox3.Controls.Add(Me.rbColors)
        Me.GroupBox3.Location = New System.Drawing.Point(612, 21)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox3.Size = New System.Drawing.Size(216, 140)
        Me.GroupBox3.TabIndex = 69
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Strokes Identifier"
        '
        'rbDots
        '
        Me.rbDots.AutoSize = True
        Me.rbDots.Checked = True
        Me.rbDots.Location = New System.Drawing.Point(60, 37)
        Me.rbDots.Margin = New System.Windows.Forms.Padding(6)
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
        Me.rbColors.Margin = New System.Windows.Forms.Padding(6)
        Me.rbColors.Name = "rbColors"
        Me.rbColors.Size = New System.Drawing.Size(105, 29)
        Me.rbColors.TabIndex = 60
        Me.rbColors.Text = "Colors"
        Me.rbColors.UseVisualStyleBackColor = True
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(22, 273)
        Me.lbStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(167, 25)
        Me.lbStatus.TabIndex = 70
        Me.lbStatus.Text = "Status Message"
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(2188, 113)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(212, 71)
        Me.btnSave.TabIndex = 71
        Me.btnSave.TabStop = False
        Me.btnSave.Text = "Save Scores"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.tbExtra)
        Me.GroupBox1.Controls.Add(Me.Label25)
        Me.GroupBox1.Controls.Add(Me.GroupBox7)
        Me.GroupBox1.Controls.Add(Me.GroupBox6)
        Me.GroupBox1.Controls.Add(Me.tbPurse)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Location = New System.Drawing.Point(840, 29)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox1.Size = New System.Drawing.Size(818, 287)
        Me.GroupBox1.TabIndex = 78
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Prize Money"
        '
        'tbExtra
        '
        Me.tbExtra.Location = New System.Drawing.Point(680, 227)
        Me.tbExtra.Margin = New System.Windows.Forms.Padding(6)
        Me.tbExtra.Name = "tbExtra"
        Me.tbExtra.ReadOnly = True
        Me.tbExtra.Size = New System.Drawing.Size(78, 31)
        Me.tbExtra.TabIndex = 77
        Me.tbExtra.TabStop = False
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(676, 167)
        Me.Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(62, 25)
        Me.Label25.TabIndex = 78
        Me.Label25.Text = "Extra"
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.Label22)
        Me.GroupBox7.Controls.Add(Me.tbCP2Tot)
        Me.GroupBox7.Controls.Add(Me.Label23)
        Me.GroupBox7.Controls.Add(Me.Label24)
        Me.GroupBox7.Controls.Add(Me.tbCP1Tot)
        Me.GroupBox7.Controls.Add(Me.tbSkinTot)
        Me.GroupBox7.Location = New System.Drawing.Point(452, 42)
        Me.GroupBox7.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox7.Size = New System.Drawing.Size(190, 235)
        Me.GroupBox7.TabIndex = 76
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "            Total "
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(10, 188)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(53, 25)
        Me.Label22.TabIndex = 80
        Me.Label22.Text = "CP2"
        '
        'tbCP2Tot
        '
        Me.tbCP2Tot.AcceptsReturn = True
        Me.tbCP2Tot.Location = New System.Drawing.Point(76, 183)
        Me.tbCP2Tot.Margin = New System.Windows.Forms.Padding(6)
        Me.tbCP2Tot.Name = "tbCP2Tot"
        Me.tbCP2Tot.ReadOnly = True
        Me.tbCP2Tot.Size = New System.Drawing.Size(78, 31)
        Me.tbCP2Tot.TabIndex = 79
        Me.tbCP2Tot.TabStop = False
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(10, 125)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(53, 25)
        Me.Label23.TabIndex = 76
        Me.Label23.Text = "CP1"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(10, 65)
        Me.Label24.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(65, 25)
        Me.Label24.TabIndex = 75
        Me.Label24.Text = "Skins"
        '
        'tbCP1Tot
        '
        Me.tbCP1Tot.AcceptsReturn = True
        Me.tbCP1Tot.Location = New System.Drawing.Point(76, 119)
        Me.tbCP1Tot.Margin = New System.Windows.Forms.Padding(6)
        Me.tbCP1Tot.Name = "tbCP1Tot"
        Me.tbCP1Tot.ReadOnly = True
        Me.tbCP1Tot.Size = New System.Drawing.Size(78, 31)
        Me.tbCP1Tot.TabIndex = 74
        Me.tbCP1Tot.TabStop = False
        '
        'tbSkinTot
        '
        Me.tbSkinTot.Location = New System.Drawing.Point(76, 56)
        Me.tbSkinTot.Margin = New System.Windows.Forms.Padding(6)
        Me.tbSkinTot.Name = "tbSkinTot"
        Me.tbSkinTot.ReadOnly = True
        Me.tbSkinTot.Size = New System.Drawing.Size(78, 31)
        Me.tbSkinTot.TabIndex = 73
        Me.tbSkinTot.TabStop = False
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Label21)
        Me.GroupBox6.Controls.Add(Me.tbPCP2)
        Me.GroupBox6.Controls.Add(Me.Label18)
        Me.GroupBox6.Controls.Add(Me.Label19)
        Me.GroupBox6.Controls.Add(Me.tbPCP1)
        Me.GroupBox6.Controls.Add(Me.tbPSkins)
        Me.GroupBox6.Location = New System.Drawing.Point(72, 42)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox6.Size = New System.Drawing.Size(190, 235)
        Me.GroupBox6.TabIndex = 75
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Prv Carry Overs"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(10, 188)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(53, 25)
        Me.Label21.TabIndex = 80
        Me.Label21.Text = "CP2"
        '
        'tbPCP2
        '
        Me.tbPCP2.AcceptsReturn = True
        Me.tbPCP2.Location = New System.Drawing.Point(76, 183)
        Me.tbPCP2.Margin = New System.Windows.Forms.Padding(6)
        Me.tbPCP2.Name = "tbPCP2"
        Me.tbPCP2.Size = New System.Drawing.Size(78, 31)
        Me.tbPCP2.TabIndex = 75
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(10, 125)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(53, 25)
        Me.Label18.TabIndex = 76
        Me.Label18.Text = "CP1"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(10, 65)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(65, 25)
        Me.Label19.TabIndex = 75
        Me.Label19.Text = "Skins"
        '
        'tbPCP1
        '
        Me.tbPCP1.AcceptsReturn = True
        Me.tbPCP1.Location = New System.Drawing.Point(76, 119)
        Me.tbPCP1.Margin = New System.Windows.Forms.Padding(6)
        Me.tbPCP1.Name = "tbPCP1"
        Me.tbPCP1.Size = New System.Drawing.Size(78, 31)
        Me.tbPCP1.TabIndex = 74
        '
        'tbPSkins
        '
        Me.tbPSkins.Location = New System.Drawing.Point(76, 56)
        Me.tbPSkins.Margin = New System.Windows.Forms.Padding(6)
        Me.tbPSkins.Name = "tbPSkins"
        Me.tbPSkins.Size = New System.Drawing.Size(78, 31)
        Me.tbPSkins.TabIndex = 73
        '
        'tbPurse
        '
        Me.tbPurse.Location = New System.Drawing.Point(680, 104)
        Me.tbPurse.Margin = New System.Windows.Forms.Padding(6)
        Me.tbPurse.Name = "tbPurse"
        Me.tbPurse.ReadOnly = True
        Me.tbPurse.Size = New System.Drawing.Size(78, 31)
        Me.tbPurse.TabIndex = 2
        Me.tbPurse.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(676, 38)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(122, 25)
        Me.Label3.TabIndex = 73
        Me.Label3.Text = "Total Purse"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label20)
        Me.GroupBox2.Controls.Add(Me.tbCP1)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.tbCP2)
        Me.GroupBox2.Controls.Add(Me.tbSkins)
        Me.GroupBox2.Location = New System.Drawing.Point(262, 42)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox2.Size = New System.Drawing.Size(190, 235)
        Me.GroupBox2.TabIndex = 74
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "This Weeks"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(10, 188)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(53, 25)
        Me.Label20.TabIndex = 78
        Me.Label20.Text = "CP2"
        '
        'tbCP1
        '
        Me.tbCP1.Location = New System.Drawing.Point(76, 119)
        Me.tbCP1.Margin = New System.Windows.Forms.Padding(6)
        Me.tbCP1.Name = "tbCP1"
        Me.tbCP1.ReadOnly = True
        Me.tbCP1.Size = New System.Drawing.Size(78, 31)
        Me.tbCP1.TabIndex = 74
        Me.tbCP1.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 125)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 25)
        Me.Label2.TabIndex = 76
        Me.Label2.Text = "CP1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(10, 65)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 25)
        Me.Label1.TabIndex = 75
        Me.Label1.Text = "Skins"
        '
        'tbCP2
        '
        Me.tbCP2.Location = New System.Drawing.Point(76, 183)
        Me.tbCP2.Margin = New System.Windows.Forms.Padding(6)
        Me.tbCP2.Name = "tbCP2"
        Me.tbCP2.ReadOnly = True
        Me.tbCP2.Size = New System.Drawing.Size(78, 31)
        Me.tbCP2.TabIndex = 77
        Me.tbCP2.TabStop = False
        '
        'tbSkins
        '
        Me.tbSkins.Location = New System.Drawing.Point(76, 56)
        Me.tbSkins.Margin = New System.Windows.Forms.Padding(6)
        Me.tbSkins.Name = "tbSkins"
        Me.tbSkins.ReadOnly = True
        Me.tbSkins.Size = New System.Drawing.Size(78, 31)
        Me.tbSkins.TabIndex = 73
        Me.tbSkins.TabStop = False
        '
        'dgScores
        '
        Me.dgScores.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgScores.Location = New System.Drawing.Point(56, 319)
        Me.dgScores.Margin = New System.Windows.Forms.Padding(6)
        Me.dgScores.Name = "dgScores"
        Me.dgScores.Size = New System.Drawing.Size(3186, 1322)
        Me.dgScores.TabIndex = 0
        '
        'cbScoresLocked
        '
        Me.cbScoresLocked.AutoSize = True
        Me.cbScoresLocked.Location = New System.Drawing.Point(1676, 204)
        Me.cbScoresLocked.Margin = New System.Windows.Forms.Padding(6)
        Me.cbScoresLocked.Name = "cbScoresLocked"
        Me.cbScoresLocked.Size = New System.Drawing.Size(163, 29)
        Me.cbScoresLocked.TabIndex = 79
        Me.cbScoresLocked.TabStop = False
        Me.cbScoresLocked.Text = "Lock Scores"
        Me.cbScoresLocked.UseVisualStyleBackColor = True
        '
        'gbHoleLegend
        '
        Me.gbHoleLegend.Controls.Add(Me.GroupBox4)
        Me.gbHoleLegend.Controls.Add(Me.Label9)
        Me.gbHoleLegend.Controls.Add(Me.Label8)
        Me.gbHoleLegend.Controls.Add(Me.Label7)
        Me.gbHoleLegend.Controls.Add(Me.Label6)
        Me.gbHoleLegend.Controls.Add(Me.Label5)
        Me.gbHoleLegend.Controls.Add(Me.Label4)
        Me.gbHoleLegend.Location = New System.Drawing.Point(192, 173)
        Me.gbHoleLegend.Margin = New System.Windows.Forms.Padding(6)
        Me.gbHoleLegend.Name = "gbHoleLegend"
        Me.gbHoleLegend.Padding = New System.Windows.Forms.Padding(6)
        Me.gbHoleLegend.Size = New System.Drawing.Size(428, 125)
        Me.gbHoleLegend.TabIndex = 80
        Me.gbHoleLegend.TabStop = False
        Me.gbHoleLegend.Text = "Holes Colors Legend"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label14)
        Me.GroupBox4.Controls.Add(Me.Label15)
        Me.GroupBox4.Controls.Add(Me.Label16)
        Me.GroupBox4.Controls.Add(Me.Label17)
        Me.GroupBox4.Location = New System.Drawing.Point(438, 0)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox4.Size = New System.Drawing.Size(326, 125)
        Me.GroupBox4.TabIndex = 81
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Colors Legend"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.Color.OrangeRed
        Me.Label14.ForeColor = System.Drawing.Color.Black
        Me.Label14.Location = New System.Drawing.Point(180, 79)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(109, 25)
        Me.Label14.TabIndex = 81
        Me.Label14.Text = "Under Par"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.BackColor = System.Drawing.Color.Aqua
        Me.Label15.ForeColor = System.Drawing.Color.Black
        Me.Label15.Location = New System.Drawing.Point(180, 40)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(50, 25)
        Me.Label15.TabIndex = 80
        Me.Label15.Text = "Sub"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.BackColor = System.Drawing.Color.Yellow
        Me.Label16.ForeColor = System.Drawing.Color.Black
        Me.Label16.Location = New System.Drawing.Point(28, 79)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(119, 25)
        Me.Label16.TabIndex = 79
        Me.Label16.Text = "Match Tied"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.BackColor = System.Drawing.Color.LightGreen
        Me.Label17.ForeColor = System.Drawing.Color.Black
        Me.Label17.Location = New System.Drawing.Point(28, 40)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(121, 25)
        Me.Label17.TabIndex = 78
        Me.Label17.Text = "Match Won"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.White
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.DarkRed
        Me.Label9.Location = New System.Drawing.Point(316, 83)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(73, 26)
        Me.Label9.TabIndex = 77
        Me.Label9.Text = "Eagle"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.White
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.OrangeRed
        Me.Label8.Location = New System.Drawing.Point(316, 44)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(74, 26)
        Me.Label8.TabIndex = 76
        Me.Label8.Text = "Birdie"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.White
        Me.Label7.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.Label7.Location = New System.Drawing.Point(210, 83)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(63, 25)
        Me.Label7.TabIndex = 75
        Me.Label7.Text = "> Par"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(228, 44)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(45, 25)
        Me.Label6.TabIndex = 74
        Me.Label6.Text = "Par"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.BlanchedAlmond
        Me.Label5.Location = New System.Drawing.Point(38, 83)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(162, 25)
        Me.Label5.TabIndex = 73
        Me.Label5.Text = "Dbl Stroke Hole"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Beige
        Me.Label4.Location = New System.Drawing.Point(38, 44)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(124, 25)
        Me.Label4.TabIndex = 72
        Me.Label4.Text = "Stroke Hole"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label13)
        Me.GroupBox5.Controls.Add(Me.Label12)
        Me.GroupBox5.Controls.Add(Me.Label11)
        Me.GroupBox5.Controls.Add(Me.Label10)
        Me.GroupBox5.Location = New System.Drawing.Point(196, 29)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(6)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(6)
        Me.GroupBox5.Size = New System.Drawing.Size(330, 125)
        Me.GroupBox5.TabIndex = 81
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Colors Legend"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.Color.OrangeRed
        Me.Label13.ForeColor = System.Drawing.Color.Black
        Me.Label13.Location = New System.Drawing.Point(172, 79)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(109, 25)
        Me.Label13.TabIndex = 81
        Me.Label13.Text = "Under Par"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.BackColor = System.Drawing.Color.Aqua
        Me.Label12.ForeColor = System.Drawing.Color.Black
        Me.Label12.Location = New System.Drawing.Point(172, 40)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(50, 25)
        Me.Label12.TabIndex = 80
        Me.Label12.Text = "Sub"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.BackColor = System.Drawing.Color.Yellow
        Me.Label11.ForeColor = System.Drawing.Color.Black
        Me.Label11.Location = New System.Drawing.Point(20, 79)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(119, 25)
        Me.Label11.TabIndex = 79
        Me.Label11.Text = "Match Tied"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.BackColor = System.Drawing.Color.LightGreen
        Me.Label10.ForeColor = System.Drawing.Color.Black
        Me.Label10.Location = New System.Drawing.Point(20, 40)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(121, 25)
        Me.Label10.TabIndex = 78
        Me.Label10.Text = "Match Won"
        '
        'cbMarkPaid
        '
        Me.cbMarkPaid.AutoSize = True
        Me.cbMarkPaid.Location = New System.Drawing.Point(1676, 115)
        Me.cbMarkPaid.Margin = New System.Windows.Forms.Padding(6)
        Me.cbMarkPaid.Name = "cbMarkPaid"
        Me.cbMarkPaid.Size = New System.Drawing.Size(248, 29)
        Me.cbMarkPaid.TabIndex = 77
        Me.cbMarkPaid.TabStop = False
        Me.cbMarkPaid.Text = "Mark CTP/Skins Paid"
        Me.cbMarkPaid.UseVisualStyleBackColor = True
        '
        'cbMatches
        '
        Me.cbMatches.AutoSize = True
        Me.cbMatches.Location = New System.Drawing.Point(1676, 160)
        Me.cbMatches.Margin = New System.Windows.Forms.Padding(6)
        Me.cbMatches.Name = "cbMatches"
        Me.cbMatches.Size = New System.Drawing.Size(246, 29)
        Me.cbMatches.TabIndex = 78
        Me.cbMatches.TabStop = False
        Me.cbMatches.Text = "Club Champ+League"
        Me.cbMatches.UseVisualStyleBackColor = True
        '
        'gbFrontBack
        '
        Me.gbFrontBack.Controls.Add(Me.rbFront)
        Me.gbFrontBack.Controls.Add(Me.rbBack)
        Me.gbFrontBack.Location = New System.Drawing.Point(1938, 31)
        Me.gbFrontBack.Margin = New System.Windows.Forms.Padding(6)
        Me.gbFrontBack.Name = "gbFrontBack"
        Me.gbFrontBack.Padding = New System.Windows.Forms.Padding(6)
        Me.gbFrontBack.Size = New System.Drawing.Size(216, 140)
        Me.gbFrontBack.TabIndex = 92
        Me.gbFrontBack.TabStop = False
        Me.gbFrontBack.Text = "Swap Nines"
        '
        'rbFront
        '
        Me.rbFront.AutoSize = True
        Me.rbFront.Checked = True
        Me.rbFront.Location = New System.Drawing.Point(60, 37)
        Me.rbFront.Margin = New System.Windows.Forms.Padding(6)
        Me.rbFront.Name = "rbFront"
        Me.rbFront.Size = New System.Drawing.Size(93, 29)
        Me.rbFront.TabIndex = 1
        Me.rbFront.TabStop = True
        Me.rbFront.Text = "Front"
        Me.rbFront.UseVisualStyleBackColor = True
        '
        'rbBack
        '
        Me.rbBack.AutoSize = True
        Me.rbBack.Location = New System.Drawing.Point(60, 81)
        Me.rbBack.Margin = New System.Windows.Forms.Padding(6)
        Me.rbBack.Name = "rbBack"
        Me.rbBack.Size = New System.Drawing.Size(91, 29)
        Me.rbBack.TabIndex = 60
        Me.rbBack.Text = "Back"
        Me.rbBack.UseVisualStyleBackColor = True
        '
        'lbParmFile
        '
        Me.lbParmFile.AutoSize = True
        Me.lbParmFile.Location = New System.Drawing.Point(1674, 275)
        Me.lbParmFile.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lbParmFile.Name = "lbParmFile"
        Me.lbParmFile.Size = New System.Drawing.Size(103, 25)
        Me.lbParmFile.TabIndex = 93
        Me.lbParmFile.Text = "Parm File"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tspb, Me.tssl})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 1594)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(3246, 38)
        Me.StatusStrip1.TabIndex = 94
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'tspb
        '
        Me.tspb.Name = "tspb"
        Me.tspb.Size = New System.Drawing.Size(100, 32)
        Me.tspb.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'tssl
        '
        Me.tssl.Name = "tssl"
        Me.tssl.Size = New System.Drawing.Size(241, 33)
        Me.tssl.Text = "ToolStripStatusLabel1"
        '
        'frmScoreCard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(3246, 1632)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.lbParmFile)
        Me.Controls.Add(Me.gbFrontBack)
        Me.Controls.Add(Me.cbMatches)
        Me.Controls.Add(Me.cbMarkPaid)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.gbHoleLegend)
        Me.Controls.Add(Me.cbScoresLocked)
        Me.Controls.Add(Me.dgScores)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.btnShowScores)
        Me.Controls.Add(Me.gbDefGames)
        Me.Controls.Add(Me.cbDatesPlayers)
        Me.Controls.Add(Me.gbDefMeth)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.lbPS)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmScoreCard"
        Me.Text = "ScoreCard"
        Me.gbDefMeth.ResumeLayout(False)
        Me.gbDefMeth.PerformLayout()
        Me.gbDefGames.ResumeLayout(False)
        Me.gbDefGames.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbHoleLegend.ResumeLayout(False)
        Me.gbHoleLegend.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.gbFrontBack.ResumeLayout(False)
        Me.gbFrontBack.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbPS As System.Windows.Forms.Label
    Friend WithEvents btnExit As Button
    Friend WithEvents btnShowScores As Button
    Friend WithEvents rbNet As RadioButton
    Friend WithEvents rbGross As RadioButton
    Friend WithEvents gbDefMeth As GroupBox
    Friend WithEvents rbScore As RadioButton
    Friend WithEvents cbDatesPlayers As ComboBox
    Friend WithEvents gbDefGames As GroupBox
    Friend WithEvents cbClosest As CheckBox
    Friend WithEvents cbSkins As CheckBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents rbDots As RadioButton
    Friend WithEvents rbColors As RadioButton
    Friend WithEvents lbStatus As Label
    Friend WithEvents btnSave As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label3 As Label
    Friend WithEvents tbPurse As TextBox
    Friend WithEvents dgScores As DataGridView
    Friend WithEvents cbScoresLocked As CheckBox
    Friend WithEvents gbHoleLegend As GroupBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Label14 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents Label13 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents tbPCP1 As TextBox
    Friend WithEvents tbPSkins As TextBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbCP1 As TextBox
    Friend WithEvents tbSkins As TextBox
    Friend WithEvents Label21 As Label
    Friend WithEvents tbPCP2 As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents tbCP2 As TextBox
    Friend WithEvents cbMarkPaid As CheckBox
    Friend WithEvents cbMatches As CheckBox
    Friend WithEvents gbFrontBack As GroupBox
    Friend WithEvents rbFront As RadioButton
    Friend WithEvents rbBack As RadioButton
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents Label22 As Label
    Friend WithEvents tbCP2Tot As TextBox
    Friend WithEvents Label23 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents tbCP1Tot As TextBox
    Friend WithEvents tbSkinTot As TextBox
    Friend WithEvents lbParmFile As Label
    Friend WithEvents tbExtra As TextBox
    Friend WithEvents Label25 As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents tspb As ToolStripProgressBar
    Friend WithEvents tssl As ToolStripStatusLabel
End Class
