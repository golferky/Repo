<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ScoreCard
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
        Me.components = New System.ComponentModel.Container()
        Me.dgScores = New System.Windows.Forms.DataGridView()
        Me.SkinsDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ClosestDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CTP1DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CTP2DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PlayerDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Partner = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TeamDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GradeDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OpponentDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PointsDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TeamPointsDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MethodDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole1DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole2DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole3DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole4DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole5DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole6DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole7DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole8DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole9DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OutDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OutGrossDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OutNetDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole10DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole11DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole12DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole13DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole14DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole15DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole16DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole17DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Hole18DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.InDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.InGrossDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.InNetDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GrossDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NetDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PHdcpDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HdcpDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EarnDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SkinsDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ClosestDataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SkinsDataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RoundDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DtScoresBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.DsLeague = New GolfManager.dsLeague()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ScoringFormatHelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StandardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StablefordToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbStandard = New System.Windows.Forms.RadioButton()
        Me.rbStableford = New System.Windows.Forms.RadioButton()
        Me.gbFrontBack = New System.Windows.Forms.GroupBox()
        Me.rbFront = New System.Windows.Forms.RadioButton()
        Me.rbBack = New System.Windows.Forms.RadioButton()
        Me.cbMatches = New System.Windows.Forms.CheckBox()
        Me.cbMarkPaid = New System.Windows.Forms.CheckBox()
        Me.gbColors = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lbUnderPar = New System.Windows.Forms.Label()
        Me.lblSub = New System.Windows.Forms.Label()
        Me.lblMatchTied = New System.Windows.Forms.Label()
        Me.lblMatchWon = New System.Windows.Forms.Label()
        Me.gbHoleLegend = New System.Windows.Forms.GroupBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.gbPrizeMoney = New System.Windows.Forms.GroupBox()
        Me.tbKitty = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.tbEachSkin = New System.Windows.Forms.TextBox()
        Me.tbNumSkins = New System.Windows.Forms.TextBox()
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
        Me.btnSave = New System.Windows.Forms.Button()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.cbStrokesId = New System.Windows.Forms.GroupBox()
        Me.rbDots = New System.Windows.Forms.RadioButton()
        Me.rbColors = New System.Windows.Forms.RadioButton()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.gbDefMeth = New System.Windows.Forms.GroupBox()
        Me.rbScore = New System.Windows.Forms.RadioButton()
        Me.rbNet = New System.Windows.Forms.RadioButton()
        Me.rbGross = New System.Windows.Forms.RadioButton()
        Me.lbPS = New System.Windows.Forms.Label()
        Me.dgHandicap = New System.Windows.Forms.DataGridView()
        Me.H1DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H2DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H3DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H4DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H5DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H6DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H7DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H8DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H9DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H10DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H11DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H12DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H13DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H14DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H15DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H16DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H17DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.H18DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DtCoursesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.btnResetCTPs = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btnReports = New System.Windows.Forms.Button()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtScoresBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.gbFrontBack.SuspendLayout()
        Me.gbColors.SuspendLayout()
        Me.gbHoleLegend.SuspendLayout()
        Me.gbPrizeMoney.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.cbStrokesId.SuspendLayout()
        Me.gbDefMeth.SuspendLayout()
        CType(Me.dgHandicap, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtCoursesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgScores
        '
        Me.dgScores.AllowUserToAddRows = False
        Me.dgScores.AllowUserToDeleteRows = False
        Me.dgScores.AutoGenerateColumns = False
        Me.dgScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgScores.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.SkinsDataGridViewTextBoxColumn, Me.ClosestDataGridViewTextBoxColumn, Me.CTP1DataGridViewTextBoxColumn, Me.CTP2DataGridViewTextBoxColumn, Me.PlayerDataGridViewTextBoxColumn, Me.Partner, Me.TeamDataGridViewTextBoxColumn, Me.GradeDataGridViewTextBoxColumn, Me.OpponentDataGridViewTextBoxColumn, Me.PointsDataGridViewTextBoxColumn, Me.TeamPointsDataGridViewTextBoxColumn, Me.MethodDataGridViewTextBoxColumn, Me.Hole1DataGridViewTextBoxColumn, Me.Hole2DataGridViewTextBoxColumn, Me.Hole3DataGridViewTextBoxColumn, Me.Hole4DataGridViewTextBoxColumn, Me.Hole5DataGridViewTextBoxColumn, Me.Hole6DataGridViewTextBoxColumn, Me.Hole7DataGridViewTextBoxColumn, Me.Hole8DataGridViewTextBoxColumn, Me.Hole9DataGridViewTextBoxColumn, Me.OutDataGridViewTextBoxColumn, Me.OutGrossDataGridViewTextBoxColumn, Me.OutNetDataGridViewTextBoxColumn, Me.Hole10DataGridViewTextBoxColumn, Me.Hole11DataGridViewTextBoxColumn, Me.Hole12DataGridViewTextBoxColumn, Me.Hole13DataGridViewTextBoxColumn, Me.Hole14DataGridViewTextBoxColumn, Me.Hole15DataGridViewTextBoxColumn, Me.Hole16DataGridViewTextBoxColumn, Me.Hole17DataGridViewTextBoxColumn, Me.Hole18DataGridViewTextBoxColumn, Me.InDataGridViewTextBoxColumn, Me.InGrossDataGridViewTextBoxColumn, Me.InNetDataGridViewTextBoxColumn, Me.GrossDataGridViewTextBoxColumn, Me.NetDataGridViewTextBoxColumn, Me.PHdcpDataGridViewTextBoxColumn, Me.HdcpDataGridViewTextBoxColumn, Me.EarnDataGridViewTextBoxColumn, Me.SkinsDataGridViewTextBoxColumn1, Me.ClosestDataGridViewTextBoxColumn1, Me.SkinsDataGridViewTextBoxColumn2, Me.RoundDataGridViewTextBoxColumn})
        Me.dgScores.DataSource = Me.DtScoresBindingSource
        Me.dgScores.Location = New System.Drawing.Point(29, 250)
        Me.dgScores.Name = "dgScores"
        Me.dgScores.RowHeadersVisible = False
        Me.dgScores.Size = New System.Drawing.Size(1525, 462)
        Me.dgScores.TabIndex = 0
        '
        'SkinsDataGridViewTextBoxColumn
        '
        Me.SkinsDataGridViewTextBoxColumn.DataPropertyName = "Skins"
        Me.SkinsDataGridViewTextBoxColumn.HeaderText = "Skins"
        Me.SkinsDataGridViewTextBoxColumn.Name = "SkinsDataGridViewTextBoxColumn"
        '
        'ClosestDataGridViewTextBoxColumn
        '
        Me.ClosestDataGridViewTextBoxColumn.DataPropertyName = "Closest"
        Me.ClosestDataGridViewTextBoxColumn.HeaderText = "Closest"
        Me.ClosestDataGridViewTextBoxColumn.Name = "ClosestDataGridViewTextBoxColumn"
        '
        'CTP1DataGridViewTextBoxColumn
        '
        Me.CTP1DataGridViewTextBoxColumn.DataPropertyName = "CTP_1"
        Me.CTP1DataGridViewTextBoxColumn.HeaderText = "CTP_1"
        Me.CTP1DataGridViewTextBoxColumn.Name = "CTP1DataGridViewTextBoxColumn"
        Me.CTP1DataGridViewTextBoxColumn.ReadOnly = True
        '
        'CTP2DataGridViewTextBoxColumn
        '
        Me.CTP2DataGridViewTextBoxColumn.DataPropertyName = "CTP_2"
        Me.CTP2DataGridViewTextBoxColumn.HeaderText = "CTP_2"
        Me.CTP2DataGridViewTextBoxColumn.Name = "CTP2DataGridViewTextBoxColumn"
        Me.CTP2DataGridViewTextBoxColumn.ReadOnly = True
        '
        'PlayerDataGridViewTextBoxColumn
        '
        Me.PlayerDataGridViewTextBoxColumn.DataPropertyName = "Player"
        Me.PlayerDataGridViewTextBoxColumn.HeaderText = "Player"
        Me.PlayerDataGridViewTextBoxColumn.Name = "PlayerDataGridViewTextBoxColumn"
        '
        'Partner
        '
        Me.Partner.DataPropertyName = "Partner"
        Me.Partner.HeaderText = "Partner"
        Me.Partner.Name = "Partner"
        Me.Partner.Visible = False
        '
        'TeamDataGridViewTextBoxColumn
        '
        Me.TeamDataGridViewTextBoxColumn.DataPropertyName = "Team"
        Me.TeamDataGridViewTextBoxColumn.HeaderText = "Team"
        Me.TeamDataGridViewTextBoxColumn.Name = "TeamDataGridViewTextBoxColumn"
        Me.TeamDataGridViewTextBoxColumn.ReadOnly = True
        '
        'GradeDataGridViewTextBoxColumn
        '
        Me.GradeDataGridViewTextBoxColumn.DataPropertyName = "Grade"
        Me.GradeDataGridViewTextBoxColumn.HeaderText = "Grade"
        Me.GradeDataGridViewTextBoxColumn.Name = "GradeDataGridViewTextBoxColumn"
        Me.GradeDataGridViewTextBoxColumn.ReadOnly = True
        '
        'OpponentDataGridViewTextBoxColumn
        '
        Me.OpponentDataGridViewTextBoxColumn.DataPropertyName = "Opponent"
        Me.OpponentDataGridViewTextBoxColumn.HeaderText = "Opponent"
        Me.OpponentDataGridViewTextBoxColumn.Name = "OpponentDataGridViewTextBoxColumn"
        Me.OpponentDataGridViewTextBoxColumn.ReadOnly = True
        '
        'PointsDataGridViewTextBoxColumn
        '
        Me.PointsDataGridViewTextBoxColumn.DataPropertyName = "Points"
        Me.PointsDataGridViewTextBoxColumn.HeaderText = "Points"
        Me.PointsDataGridViewTextBoxColumn.Name = "PointsDataGridViewTextBoxColumn"
        Me.PointsDataGridViewTextBoxColumn.ReadOnly = True
        '
        'TeamPointsDataGridViewTextBoxColumn
        '
        Me.TeamPointsDataGridViewTextBoxColumn.DataPropertyName = "Team_Points"
        Me.TeamPointsDataGridViewTextBoxColumn.HeaderText = "Team Points"
        Me.TeamPointsDataGridViewTextBoxColumn.Name = "TeamPointsDataGridViewTextBoxColumn"
        Me.TeamPointsDataGridViewTextBoxColumn.ReadOnly = True
        Me.TeamPointsDataGridViewTextBoxColumn.Width = 80
        '
        'MethodDataGridViewTextBoxColumn
        '
        Me.MethodDataGridViewTextBoxColumn.DataPropertyName = "Method"
        Me.MethodDataGridViewTextBoxColumn.HeaderText = "Method"
        Me.MethodDataGridViewTextBoxColumn.Name = "MethodDataGridViewTextBoxColumn"
        '
        'Hole1DataGridViewTextBoxColumn
        '
        Me.Hole1DataGridViewTextBoxColumn.DataPropertyName = "Hole1"
        Me.Hole1DataGridViewTextBoxColumn.HeaderText = "1"
        Me.Hole1DataGridViewTextBoxColumn.MaxInputLength = 9
        Me.Hole1DataGridViewTextBoxColumn.Name = "Hole1DataGridViewTextBoxColumn"
        '
        'Hole2DataGridViewTextBoxColumn
        '
        Me.Hole2DataGridViewTextBoxColumn.DataPropertyName = "Hole2"
        Me.Hole2DataGridViewTextBoxColumn.HeaderText = "2"
        Me.Hole2DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole2DataGridViewTextBoxColumn.Name = "Hole2DataGridViewTextBoxColumn"
        '
        'Hole3DataGridViewTextBoxColumn
        '
        Me.Hole3DataGridViewTextBoxColumn.DataPropertyName = "Hole3"
        Me.Hole3DataGridViewTextBoxColumn.HeaderText = "3"
        Me.Hole3DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole3DataGridViewTextBoxColumn.Name = "Hole3DataGridViewTextBoxColumn"
        '
        'Hole4DataGridViewTextBoxColumn
        '
        Me.Hole4DataGridViewTextBoxColumn.DataPropertyName = "Hole4"
        Me.Hole4DataGridViewTextBoxColumn.HeaderText = "4"
        Me.Hole4DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole4DataGridViewTextBoxColumn.Name = "Hole4DataGridViewTextBoxColumn"
        '
        'Hole5DataGridViewTextBoxColumn
        '
        Me.Hole5DataGridViewTextBoxColumn.DataPropertyName = "Hole5"
        Me.Hole5DataGridViewTextBoxColumn.HeaderText = "5"
        Me.Hole5DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole5DataGridViewTextBoxColumn.Name = "Hole5DataGridViewTextBoxColumn"
        '
        'Hole6DataGridViewTextBoxColumn
        '
        Me.Hole6DataGridViewTextBoxColumn.DataPropertyName = "Hole6"
        Me.Hole6DataGridViewTextBoxColumn.HeaderText = "6"
        Me.Hole6DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole6DataGridViewTextBoxColumn.Name = "Hole6DataGridViewTextBoxColumn"
        '
        'Hole7DataGridViewTextBoxColumn
        '
        Me.Hole7DataGridViewTextBoxColumn.DataPropertyName = "Hole7"
        Me.Hole7DataGridViewTextBoxColumn.HeaderText = "7"
        Me.Hole7DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole7DataGridViewTextBoxColumn.Name = "Hole7DataGridViewTextBoxColumn"
        '
        'Hole8DataGridViewTextBoxColumn
        '
        Me.Hole8DataGridViewTextBoxColumn.DataPropertyName = "Hole8"
        Me.Hole8DataGridViewTextBoxColumn.HeaderText = "8"
        Me.Hole8DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole8DataGridViewTextBoxColumn.Name = "Hole8DataGridViewTextBoxColumn"
        '
        'Hole9DataGridViewTextBoxColumn
        '
        Me.Hole9DataGridViewTextBoxColumn.DataPropertyName = "Hole9"
        Me.Hole9DataGridViewTextBoxColumn.HeaderText = "9"
        Me.Hole9DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole9DataGridViewTextBoxColumn.Name = "Hole9DataGridViewTextBoxColumn"
        '
        'OutDataGridViewTextBoxColumn
        '
        Me.OutDataGridViewTextBoxColumn.DataPropertyName = "Out"
        Me.OutDataGridViewTextBoxColumn.HeaderText = "Out"
        Me.OutDataGridViewTextBoxColumn.Name = "OutDataGridViewTextBoxColumn"
        Me.OutDataGridViewTextBoxColumn.ReadOnly = True
        '
        'OutGrossDataGridViewTextBoxColumn
        '
        Me.OutGrossDataGridViewTextBoxColumn.DataPropertyName = "Out_Gross"
        Me.OutGrossDataGridViewTextBoxColumn.HeaderText = "Out Gross"
        Me.OutGrossDataGridViewTextBoxColumn.Name = "OutGrossDataGridViewTextBoxColumn"
        Me.OutGrossDataGridViewTextBoxColumn.ReadOnly = True
        '
        'OutNetDataGridViewTextBoxColumn
        '
        Me.OutNetDataGridViewTextBoxColumn.DataPropertyName = "Out_Net"
        Me.OutNetDataGridViewTextBoxColumn.HeaderText = "Out Net"
        Me.OutNetDataGridViewTextBoxColumn.Name = "OutNetDataGridViewTextBoxColumn"
        Me.OutNetDataGridViewTextBoxColumn.ReadOnly = True
        '
        'Hole10DataGridViewTextBoxColumn
        '
        Me.Hole10DataGridViewTextBoxColumn.DataPropertyName = "Hole10"
        Me.Hole10DataGridViewTextBoxColumn.HeaderText = "10"
        Me.Hole10DataGridViewTextBoxColumn.MaxInputLength = 9
        Me.Hole10DataGridViewTextBoxColumn.Name = "Hole10DataGridViewTextBoxColumn"
        '
        'Hole11DataGridViewTextBoxColumn
        '
        Me.Hole11DataGridViewTextBoxColumn.DataPropertyName = "Hole11"
        Me.Hole11DataGridViewTextBoxColumn.HeaderText = "11"
        Me.Hole11DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole11DataGridViewTextBoxColumn.Name = "Hole11DataGridViewTextBoxColumn"
        '
        'Hole12DataGridViewTextBoxColumn
        '
        Me.Hole12DataGridViewTextBoxColumn.DataPropertyName = "Hole12"
        Me.Hole12DataGridViewTextBoxColumn.HeaderText = "12"
        Me.Hole12DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole12DataGridViewTextBoxColumn.Name = "Hole12DataGridViewTextBoxColumn"
        '
        'Hole13DataGridViewTextBoxColumn
        '
        Me.Hole13DataGridViewTextBoxColumn.DataPropertyName = "Hole13"
        Me.Hole13DataGridViewTextBoxColumn.HeaderText = "13"
        Me.Hole13DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole13DataGridViewTextBoxColumn.Name = "Hole13DataGridViewTextBoxColumn"
        '
        'Hole14DataGridViewTextBoxColumn
        '
        Me.Hole14DataGridViewTextBoxColumn.DataPropertyName = "Hole14"
        Me.Hole14DataGridViewTextBoxColumn.HeaderText = "14"
        Me.Hole14DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole14DataGridViewTextBoxColumn.Name = "Hole14DataGridViewTextBoxColumn"
        '
        'Hole15DataGridViewTextBoxColumn
        '
        Me.Hole15DataGridViewTextBoxColumn.DataPropertyName = "Hole15"
        Me.Hole15DataGridViewTextBoxColumn.HeaderText = "15"
        Me.Hole15DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole15DataGridViewTextBoxColumn.Name = "Hole15DataGridViewTextBoxColumn"
        '
        'Hole16DataGridViewTextBoxColumn
        '
        Me.Hole16DataGridViewTextBoxColumn.DataPropertyName = "Hole16"
        Me.Hole16DataGridViewTextBoxColumn.HeaderText = "16"
        Me.Hole16DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole16DataGridViewTextBoxColumn.Name = "Hole16DataGridViewTextBoxColumn"
        '
        'Hole17DataGridViewTextBoxColumn
        '
        Me.Hole17DataGridViewTextBoxColumn.DataPropertyName = "Hole17"
        Me.Hole17DataGridViewTextBoxColumn.HeaderText = "17"
        Me.Hole17DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole17DataGridViewTextBoxColumn.Name = "Hole17DataGridViewTextBoxColumn"
        '
        'Hole18DataGridViewTextBoxColumn
        '
        Me.Hole18DataGridViewTextBoxColumn.DataPropertyName = "Hole18"
        Me.Hole18DataGridViewTextBoxColumn.HeaderText = "18"
        Me.Hole18DataGridViewTextBoxColumn.MaxInputLength = 1
        Me.Hole18DataGridViewTextBoxColumn.Name = "Hole18DataGridViewTextBoxColumn"
        '
        'InDataGridViewTextBoxColumn
        '
        Me.InDataGridViewTextBoxColumn.DataPropertyName = "In"
        Me.InDataGridViewTextBoxColumn.HeaderText = "In"
        Me.InDataGridViewTextBoxColumn.Name = "InDataGridViewTextBoxColumn"
        Me.InDataGridViewTextBoxColumn.ReadOnly = True
        '
        'InGrossDataGridViewTextBoxColumn
        '
        Me.InGrossDataGridViewTextBoxColumn.DataPropertyName = "In_Gross"
        Me.InGrossDataGridViewTextBoxColumn.HeaderText = "In Gross"
        Me.InGrossDataGridViewTextBoxColumn.Name = "InGrossDataGridViewTextBoxColumn"
        Me.InGrossDataGridViewTextBoxColumn.ReadOnly = True
        '
        'InNetDataGridViewTextBoxColumn
        '
        Me.InNetDataGridViewTextBoxColumn.DataPropertyName = "In_Net"
        Me.InNetDataGridViewTextBoxColumn.HeaderText = "In Net"
        Me.InNetDataGridViewTextBoxColumn.Name = "InNetDataGridViewTextBoxColumn"
        Me.InNetDataGridViewTextBoxColumn.ReadOnly = True
        '
        'GrossDataGridViewTextBoxColumn
        '
        Me.GrossDataGridViewTextBoxColumn.DataPropertyName = "18_Gross"
        Me.GrossDataGridViewTextBoxColumn.HeaderText = "18 Gross"
        Me.GrossDataGridViewTextBoxColumn.Name = "GrossDataGridViewTextBoxColumn"
        Me.GrossDataGridViewTextBoxColumn.ReadOnly = True
        '
        'NetDataGridViewTextBoxColumn
        '
        Me.NetDataGridViewTextBoxColumn.DataPropertyName = "18_Net"
        Me.NetDataGridViewTextBoxColumn.HeaderText = "18 Net"
        Me.NetDataGridViewTextBoxColumn.Name = "NetDataGridViewTextBoxColumn"
        Me.NetDataGridViewTextBoxColumn.ReadOnly = True
        '
        'PHdcpDataGridViewTextBoxColumn
        '
        Me.PHdcpDataGridViewTextBoxColumn.DataPropertyName = "PHdcp"
        Me.PHdcpDataGridViewTextBoxColumn.HeaderText = "Prev Hdcp"
        Me.PHdcpDataGridViewTextBoxColumn.Name = "PHdcpDataGridViewTextBoxColumn"
        Me.PHdcpDataGridViewTextBoxColumn.ReadOnly = True
        '
        'HdcpDataGridViewTextBoxColumn
        '
        Me.HdcpDataGridViewTextBoxColumn.DataPropertyName = "Hdcp"
        Me.HdcpDataGridViewTextBoxColumn.HeaderText = "Hdcp"
        Me.HdcpDataGridViewTextBoxColumn.Name = "HdcpDataGridViewTextBoxColumn"
        Me.HdcpDataGridViewTextBoxColumn.ReadOnly = True
        '
        'EarnDataGridViewTextBoxColumn
        '
        Me.EarnDataGridViewTextBoxColumn.DataPropertyName = "$Earn"
        Me.EarnDataGridViewTextBoxColumn.HeaderText = "Total Earned"
        Me.EarnDataGridViewTextBoxColumn.Name = "EarnDataGridViewTextBoxColumn"
        Me.EarnDataGridViewTextBoxColumn.ReadOnly = True
        '
        'SkinsDataGridViewTextBoxColumn1
        '
        Me.SkinsDataGridViewTextBoxColumn1.DataPropertyName = "$Skins"
        Me.SkinsDataGridViewTextBoxColumn1.HeaderText = "Total Skins"
        Me.SkinsDataGridViewTextBoxColumn1.Name = "SkinsDataGridViewTextBoxColumn1"
        Me.SkinsDataGridViewTextBoxColumn1.ReadOnly = True
        '
        'ClosestDataGridViewTextBoxColumn1
        '
        Me.ClosestDataGridViewTextBoxColumn1.DataPropertyName = "$Closest"
        Me.ClosestDataGridViewTextBoxColumn1.HeaderText = "Total Closest"
        Me.ClosestDataGridViewTextBoxColumn1.Name = "ClosestDataGridViewTextBoxColumn1"
        Me.ClosestDataGridViewTextBoxColumn1.ReadOnly = True
        '
        'SkinsDataGridViewTextBoxColumn2
        '
        Me.SkinsDataGridViewTextBoxColumn2.DataPropertyName = "#Skins"
        Me.SkinsDataGridViewTextBoxColumn2.HeaderText = "Num Skins"
        Me.SkinsDataGridViewTextBoxColumn2.Name = "SkinsDataGridViewTextBoxColumn2"
        Me.SkinsDataGridViewTextBoxColumn2.ReadOnly = True
        '
        'RoundDataGridViewTextBoxColumn
        '
        Me.RoundDataGridViewTextBoxColumn.DataPropertyName = "Round"
        Me.RoundDataGridViewTextBoxColumn.HeaderText = "Round"
        Me.RoundDataGridViewTextBoxColumn.MaxInputLength = 1
        Me.RoundDataGridViewTextBoxColumn.Name = "RoundDataGridViewTextBoxColumn"
        '
        'DtScoresBindingSource
        '
        Me.DtScoresBindingSource.DataMember = "dtScores"
        Me.DtScoresBindingSource.DataSource = Me.DsLeague
        '
        'DsLeague
        '
        Me.DsLeague.DataSetName = "dsLeague"
        Me.DsLeague.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ScoringFormatHelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1665, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ScoringFormatHelpToolStripMenuItem
        '
        Me.ScoringFormatHelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StandardToolStripMenuItem, Me.StablefordToolStripMenuItem})
        Me.ScoringFormatHelpToolStripMenuItem.Name = "ScoringFormatHelpToolStripMenuItem"
        Me.ScoringFormatHelpToolStripMenuItem.Size = New System.Drawing.Size(128, 20)
        Me.ScoringFormatHelpToolStripMenuItem.Text = "Scoring Format Help"
        '
        'StandardToolStripMenuItem
        '
        Me.StandardToolStripMenuItem.Name = "StandardToolStripMenuItem"
        Me.StandardToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.StandardToolStripMenuItem.Text = "Standard"
        '
        'StablefordToolStripMenuItem
        '
        Me.StablefordToolStripMenuItem.Name = "StablefordToolStripMenuItem"
        Me.StablefordToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.StablefordToolStripMenuItem.Text = "Stableford"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbStandard)
        Me.GroupBox1.Controls.Add(Me.rbStableford)
        Me.GroupBox1.Location = New System.Drawing.Point(25, 37)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(100, 73)
        Me.GroupBox1.TabIndex = 109
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scoring  Format"
        '
        'rbStandard
        '
        Me.rbStandard.AutoSize = True
        Me.rbStandard.Checked = True
        Me.rbStandard.Location = New System.Drawing.Point(15, 17)
        Me.rbStandard.Name = "rbStandard"
        Me.rbStandard.Size = New System.Drawing.Size(68, 17)
        Me.rbStandard.TabIndex = 1
        Me.rbStandard.TabStop = True
        Me.rbStandard.Text = "Standard"
        Me.rbStandard.UseVisualStyleBackColor = True
        '
        'rbStableford
        '
        Me.rbStableford.AutoSize = True
        Me.rbStableford.Location = New System.Drawing.Point(15, 40)
        Me.rbStableford.Name = "rbStableford"
        Me.rbStableford.Size = New System.Drawing.Size(73, 17)
        Me.rbStableford.TabIndex = 60
        Me.rbStableford.Text = "Stableford"
        Me.rbStableford.UseVisualStyleBackColor = True
        '
        'gbFrontBack
        '
        Me.gbFrontBack.Controls.Add(Me.rbFront)
        Me.gbFrontBack.Controls.Add(Me.rbBack)
        Me.gbFrontBack.Location = New System.Drawing.Point(234, 37)
        Me.gbFrontBack.Name = "gbFrontBack"
        Me.gbFrontBack.Size = New System.Drawing.Size(100, 73)
        Me.gbFrontBack.TabIndex = 108
        Me.gbFrontBack.TabStop = False
        Me.gbFrontBack.Text = "Swap Nines"
        '
        'rbFront
        '
        Me.rbFront.AutoSize = True
        Me.rbFront.Checked = True
        Me.rbFront.Location = New System.Drawing.Point(30, 19)
        Me.rbFront.Name = "rbFront"
        Me.rbFront.Size = New System.Drawing.Size(49, 17)
        Me.rbFront.TabIndex = 1
        Me.rbFront.TabStop = True
        Me.rbFront.Text = "Front"
        Me.rbFront.UseVisualStyleBackColor = True
        '
        'rbBack
        '
        Me.rbBack.AutoSize = True
        Me.rbBack.Location = New System.Drawing.Point(29, 40)
        Me.rbBack.Name = "rbBack"
        Me.rbBack.Size = New System.Drawing.Size(50, 17)
        Me.rbBack.TabIndex = 60
        Me.rbBack.Text = "Back"
        Me.rbBack.UseVisualStyleBackColor = True
        '
        'cbMatches
        '
        Me.cbMatches.AutoSize = True
        Me.cbMatches.Location = New System.Drawing.Point(189, 126)
        Me.cbMatches.Name = "cbMatches"
        Me.cbMatches.Size = New System.Drawing.Size(125, 17)
        Me.cbMatches.TabIndex = 104
        Me.cbMatches.TabStop = False
        Me.cbMatches.Text = "Club Champ+League"
        Me.cbMatches.UseVisualStyleBackColor = True
        Me.cbMatches.Visible = False
        '
        'cbMarkPaid
        '
        Me.cbMarkPaid.AutoSize = True
        Me.cbMarkPaid.Location = New System.Drawing.Point(25, 127)
        Me.cbMarkPaid.Name = "cbMarkPaid"
        Me.cbMarkPaid.Size = New System.Drawing.Size(129, 17)
        Me.cbMarkPaid.TabIndex = 103
        Me.cbMarkPaid.TabStop = False
        Me.cbMarkPaid.Text = "Mark CTP/Skins Paid"
        Me.cbMarkPaid.UseVisualStyleBackColor = True
        Me.cbMarkPaid.Visible = False
        '
        'gbColors
        '
        Me.gbColors.Controls.Add(Me.Label4)
        Me.gbColors.Controls.Add(Me.lbUnderPar)
        Me.gbColors.Controls.Add(Me.lblSub)
        Me.gbColors.Controls.Add(Me.lblMatchTied)
        Me.gbColors.Controls.Add(Me.lblMatchWon)
        Me.gbColors.Location = New System.Drawing.Point(380, 181)
        Me.gbColors.Name = "gbColors"
        Me.gbColors.Size = New System.Drawing.Size(317, 63)
        Me.gbColors.TabIndex = 107
        Me.gbColors.TabStop = False
        Me.gbColors.Text = "Colors Legend"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Pink
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(17, 21)
        Me.Label4.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 13)
        Me.Label4.TabIndex = 82
        Me.Label4.Text = "NoShow"
        '
        'lbUnderPar
        '
        Me.lbUnderPar.AutoSize = True
        Me.lbUnderPar.BackColor = System.Drawing.Color.OrangeRed
        Me.lbUnderPar.ForeColor = System.Drawing.Color.Black
        Me.lbUnderPar.Location = New System.Drawing.Point(257, 21)
        Me.lbUnderPar.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbUnderPar.Name = "lbUnderPar"
        Me.lbUnderPar.Size = New System.Drawing.Size(55, 13)
        Me.lbUnderPar.TabIndex = 81
        Me.lbUnderPar.Text = "Under Par"
        '
        'lblSub
        '
        Me.lblSub.AutoSize = True
        Me.lblSub.BackColor = System.Drawing.Color.Aqua
        Me.lblSub.ForeColor = System.Drawing.Color.Black
        Me.lblSub.Location = New System.Drawing.Point(17, 41)
        Me.lblSub.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSub.Name = "lblSub"
        Me.lblSub.Size = New System.Drawing.Size(26, 13)
        Me.lblSub.TabIndex = 80
        Me.lblSub.Text = "Sub"
        '
        'lblMatchTied
        '
        Me.lblMatchTied.AutoSize = True
        Me.lblMatchTied.BackColor = System.Drawing.Color.Yellow
        Me.lblMatchTied.ForeColor = System.Drawing.Color.Black
        Me.lblMatchTied.Location = New System.Drawing.Point(142, 41)
        Me.lblMatchTied.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblMatchTied.Name = "lblMatchTied"
        Me.lblMatchTied.Size = New System.Drawing.Size(61, 13)
        Me.lblMatchTied.TabIndex = 79
        Me.lblMatchTied.Text = "Match Tied"
        '
        'lblMatchWon
        '
        Me.lblMatchWon.AutoSize = True
        Me.lblMatchWon.BackColor = System.Drawing.Color.LightGreen
        Me.lblMatchWon.ForeColor = System.Drawing.Color.Black
        Me.lblMatchWon.Location = New System.Drawing.Point(142, 21)
        Me.lblMatchWon.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblMatchWon.Name = "lblMatchWon"
        Me.lblMatchWon.Size = New System.Drawing.Size(63, 13)
        Me.lblMatchWon.TabIndex = 78
        Me.lblMatchWon.Text = "Match Won"
        '
        'gbHoleLegend
        '
        Me.gbHoleLegend.Controls.Add(Me.Label9)
        Me.gbHoleLegend.Controls.Add(Me.Label8)
        Me.gbHoleLegend.Controls.Add(Me.Label7)
        Me.gbHoleLegend.Controls.Add(Me.Label6)
        Me.gbHoleLegend.Location = New System.Drawing.Point(810, 184)
        Me.gbHoleLegend.Name = "gbHoleLegend"
        Me.gbHoleLegend.Size = New System.Drawing.Size(140, 64)
        Me.gbHoleLegend.TabIndex = 106
        Me.gbHoleLegend.TabStop = False
        Me.gbHoleLegend.Text = "Holes Colors Legend"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.Color.White
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.Color.DarkRed
        Me.Label9.Location = New System.Drawing.Point(62, 38)
        Me.Label9.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(39, 13)
        Me.Label9.TabIndex = 77
        Me.Label9.Text = "Eagle"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.Color.White
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.Color.OrangeRed
        Me.Label8.Location = New System.Drawing.Point(62, 18)
        Me.Label8.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(39, 13)
        Me.Label8.TabIndex = 76
        Me.Label8.Text = "Birdie"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.White
        Me.Label7.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.Label7.Location = New System.Drawing.Point(9, 38)
        Me.Label7.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(32, 13)
        Me.Label7.TabIndex = 75
        Me.Label7.Text = "> Par"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(18, 18)
        Me.Label6.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(23, 13)
        Me.Label6.TabIndex = 74
        Me.Label6.Text = "Par"
        '
        'gbPrizeMoney
        '
        Me.gbPrizeMoney.Controls.Add(Me.tbKitty)
        Me.gbPrizeMoney.Controls.Add(Me.Label25)
        Me.gbPrizeMoney.Controls.Add(Me.GroupBox8)
        Me.gbPrizeMoney.Controls.Add(Me.GroupBox7)
        Me.gbPrizeMoney.Controls.Add(Me.GroupBox6)
        Me.gbPrizeMoney.Controls.Add(Me.tbPurse)
        Me.gbPrizeMoney.Controls.Add(Me.Label3)
        Me.gbPrizeMoney.Controls.Add(Me.GroupBox2)
        Me.gbPrizeMoney.Location = New System.Drawing.Point(439, 26)
        Me.gbPrizeMoney.Name = "gbPrizeMoney"
        Me.gbPrizeMoney.Size = New System.Drawing.Size(526, 149)
        Me.gbPrizeMoney.TabIndex = 105
        Me.gbPrizeMoney.TabStop = False
        Me.gbPrizeMoney.Text = "Prize Money"
        '
        'tbKitty
        '
        Me.tbKitty.Location = New System.Drawing.Point(470, 82)
        Me.tbKitty.Name = "tbKitty"
        Me.tbKitty.ReadOnly = True
        Me.tbKitty.Size = New System.Drawing.Size(41, 20)
        Me.tbKitty.TabIndex = 80
        Me.tbKitty.TabStop = False
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(431, 87)
        Me.Label25.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(27, 13)
        Me.Label25.TabIndex = 81
        Me.Label25.Text = "Kitty"
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label27)
        Me.GroupBox8.Controls.Add(Me.Label28)
        Me.GroupBox8.Controls.Add(Me.tbEachSkin)
        Me.GroupBox8.Controls.Add(Me.tbNumSkins)
        Me.GroupBox8.Location = New System.Drawing.Point(321, 21)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(105, 122)
        Me.GroupBox8.TabIndex = 79
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Skin Detail"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(5, 65)
        Me.Label27.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(29, 13)
        Me.Label27.TabIndex = 76
        Me.Label27.Text = "$Per"
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(5, 34)
        Me.Label28.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(40, 13)
        Me.Label28.TabIndex = 75
        Me.Label28.Text = "#Skins"
        '
        'tbEachSkin
        '
        Me.tbEachSkin.AcceptsReturn = True
        Me.tbEachSkin.Location = New System.Drawing.Point(50, 62)
        Me.tbEachSkin.Name = "tbEachSkin"
        Me.tbEachSkin.ReadOnly = True
        Me.tbEachSkin.Size = New System.Drawing.Size(41, 20)
        Me.tbEachSkin.TabIndex = 74
        Me.tbEachSkin.TabStop = False
        '
        'tbNumSkins
        '
        Me.tbNumSkins.Location = New System.Drawing.Point(50, 29)
        Me.tbNumSkins.Name = "tbNumSkins"
        Me.tbNumSkins.ReadOnly = True
        Me.tbNumSkins.Size = New System.Drawing.Size(41, 20)
        Me.tbNumSkins.TabIndex = 73
        Me.tbNumSkins.TabStop = False
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.Label22)
        Me.GroupBox7.Controls.Add(Me.tbCP2Tot)
        Me.GroupBox7.Controls.Add(Me.Label23)
        Me.GroupBox7.Controls.Add(Me.Label24)
        Me.GroupBox7.Controls.Add(Me.tbCP1Tot)
        Me.GroupBox7.Controls.Add(Me.tbSkinTot)
        Me.GroupBox7.Location = New System.Drawing.Point(226, 22)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(95, 122)
        Me.GroupBox7.TabIndex = 76
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "            Total "
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(5, 98)
        Me.Label22.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(27, 13)
        Me.Label22.TabIndex = 80
        Me.Label22.Text = "CP2"
        '
        'tbCP2Tot
        '
        Me.tbCP2Tot.AcceptsReturn = True
        Me.tbCP2Tot.Location = New System.Drawing.Point(38, 95)
        Me.tbCP2Tot.Name = "tbCP2Tot"
        Me.tbCP2Tot.ReadOnly = True
        Me.tbCP2Tot.Size = New System.Drawing.Size(41, 20)
        Me.tbCP2Tot.TabIndex = 79
        Me.tbCP2Tot.TabStop = False
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(5, 65)
        Me.Label23.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(27, 13)
        Me.Label23.TabIndex = 76
        Me.Label23.Text = "CP1"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(5, 34)
        Me.Label24.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(33, 13)
        Me.Label24.TabIndex = 75
        Me.Label24.Text = "Skins"
        '
        'tbCP1Tot
        '
        Me.tbCP1Tot.AcceptsReturn = True
        Me.tbCP1Tot.Location = New System.Drawing.Point(38, 62)
        Me.tbCP1Tot.Name = "tbCP1Tot"
        Me.tbCP1Tot.ReadOnly = True
        Me.tbCP1Tot.Size = New System.Drawing.Size(41, 20)
        Me.tbCP1Tot.TabIndex = 74
        Me.tbCP1Tot.TabStop = False
        '
        'tbSkinTot
        '
        Me.tbSkinTot.Location = New System.Drawing.Point(38, 29)
        Me.tbSkinTot.Name = "tbSkinTot"
        Me.tbSkinTot.ReadOnly = True
        Me.tbSkinTot.Size = New System.Drawing.Size(41, 20)
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
        Me.GroupBox6.Location = New System.Drawing.Point(36, 22)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(95, 122)
        Me.GroupBox6.TabIndex = 75
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Prv Carry Overs"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(5, 98)
        Me.Label21.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(27, 13)
        Me.Label21.TabIndex = 80
        Me.Label21.Text = "CP2"
        '
        'tbPCP2
        '
        Me.tbPCP2.AcceptsReturn = True
        Me.tbPCP2.Location = New System.Drawing.Point(38, 95)
        Me.tbPCP2.Name = "tbPCP2"
        Me.tbPCP2.ReadOnly = True
        Me.tbPCP2.Size = New System.Drawing.Size(41, 20)
        Me.tbPCP2.TabIndex = 75
        Me.tbPCP2.TabStop = False
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(5, 65)
        Me.Label18.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(27, 13)
        Me.Label18.TabIndex = 76
        Me.Label18.Text = "CP1"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(5, 34)
        Me.Label19.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(33, 13)
        Me.Label19.TabIndex = 75
        Me.Label19.Text = "Skins"
        '
        'tbPCP1
        '
        Me.tbPCP1.AcceptsReturn = True
        Me.tbPCP1.Location = New System.Drawing.Point(38, 62)
        Me.tbPCP1.Name = "tbPCP1"
        Me.tbPCP1.ReadOnly = True
        Me.tbPCP1.Size = New System.Drawing.Size(41, 20)
        Me.tbPCP1.TabIndex = 74
        Me.tbPCP1.TabStop = False
        '
        'tbPSkins
        '
        Me.tbPSkins.Location = New System.Drawing.Point(38, 29)
        Me.tbPSkins.Name = "tbPSkins"
        Me.tbPSkins.ReadOnly = True
        Me.tbPSkins.Size = New System.Drawing.Size(41, 20)
        Me.tbPSkins.TabIndex = 73
        Me.tbPSkins.TabStop = False
        '
        'tbPurse
        '
        Me.tbPurse.Location = New System.Drawing.Point(470, 48)
        Me.tbPurse.Name = "tbPurse"
        Me.tbPurse.ReadOnly = True
        Me.tbPurse.Size = New System.Drawing.Size(41, 20)
        Me.tbPurse.TabIndex = 2
        Me.tbPurse.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(431, 55)
        Me.Label3.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(34, 13)
        Me.Label3.TabIndex = 73
        Me.Label3.Text = "Purse"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label20)
        Me.GroupBox2.Controls.Add(Me.tbCP1)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.tbCP2)
        Me.GroupBox2.Controls.Add(Me.tbSkins)
        Me.GroupBox2.Location = New System.Drawing.Point(131, 22)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(95, 122)
        Me.GroupBox2.TabIndex = 74
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "This Rounds"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(5, 98)
        Me.Label20.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(27, 13)
        Me.Label20.TabIndex = 78
        Me.Label20.Text = "CP2"
        '
        'tbCP1
        '
        Me.tbCP1.Location = New System.Drawing.Point(38, 62)
        Me.tbCP1.Name = "tbCP1"
        Me.tbCP1.ReadOnly = True
        Me.tbCP1.Size = New System.Drawing.Size(41, 20)
        Me.tbCP1.TabIndex = 74
        Me.tbCP1.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(5, 65)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(27, 13)
        Me.Label2.TabIndex = 76
        Me.Label2.Text = "CP1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 34)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 13)
        Me.Label1.TabIndex = 75
        Me.Label1.Text = "Skins"
        '
        'tbCP2
        '
        Me.tbCP2.Location = New System.Drawing.Point(38, 95)
        Me.tbCP2.Name = "tbCP2"
        Me.tbCP2.ReadOnly = True
        Me.tbCP2.Size = New System.Drawing.Size(41, 20)
        Me.tbCP2.TabIndex = 77
        Me.tbCP2.TabStop = False
        '
        'tbSkins
        '
        Me.tbSkins.Location = New System.Drawing.Point(38, 29)
        Me.tbSkins.Name = "tbSkins"
        Me.tbSkins.ReadOnly = True
        Me.tbSkins.Size = New System.Drawing.Size(41, 20)
        Me.tbSkins.TabIndex = 73
        Me.tbSkins.TabStop = False
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(989, 75)
        Me.btnSave.Margin = New System.Windows.Forms.Padding(2)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(81, 37)
        Me.btnSave.TabIndex = 101
        Me.btnSave.TabStop = False
        Me.btnSave.Text = "Save Scores"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(30, 184)
        Me.lbStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(83, 13)
        Me.lbStatus.TabIndex = 100
        Me.lbStatus.Text = "Status Message"
        '
        'cbStrokesId
        '
        Me.cbStrokesId.Controls.Add(Me.rbDots)
        Me.cbStrokesId.Controls.Add(Me.rbColors)
        Me.cbStrokesId.Location = New System.Drawing.Point(1095, 105)
        Me.cbStrokesId.Name = "cbStrokesId"
        Me.cbStrokesId.Size = New System.Drawing.Size(100, 69)
        Me.cbStrokesId.TabIndex = 99
        Me.cbStrokesId.TabStop = False
        Me.cbStrokesId.Text = "Strokes Qualifier"
        Me.cbStrokesId.Visible = False
        '
        'rbDots
        '
        Me.rbDots.Location = New System.Drawing.Point(25, 17)
        Me.rbDots.Name = "rbDots"
        Me.rbDots.Size = New System.Drawing.Size(47, 17)
        Me.rbDots.TabIndex = 1
        Me.rbDots.Text = "Dots"
        Me.rbDots.UseVisualStyleBackColor = True
        '
        'rbColors
        '
        Me.rbColors.AutoSize = True
        Me.rbColors.Checked = True
        Me.rbColors.Location = New System.Drawing.Point(25, 40)
        Me.rbColors.Name = "rbColors"
        Me.rbColors.Size = New System.Drawing.Size(54, 17)
        Me.rbColors.TabIndex = 60
        Me.rbColors.TabStop = True
        Me.rbColors.Text = "Colors"
        Me.rbColors.UseVisualStyleBackColor = True
        '
        'cbDates
        '
        Me.cbDates.FormattingEnabled = True
        Me.cbDates.Location = New System.Drawing.Point(989, 42)
        Me.cbDates.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(81, 21)
        Me.cbDates.TabIndex = 102
        '
        'gbDefMeth
        '
        Me.gbDefMeth.Controls.Add(Me.rbScore)
        Me.gbDefMeth.Controls.Add(Me.rbNet)
        Me.gbDefMeth.Controls.Add(Me.rbGross)
        Me.gbDefMeth.Location = New System.Drawing.Point(131, 37)
        Me.gbDefMeth.Name = "gbDefMeth"
        Me.gbDefMeth.Size = New System.Drawing.Size(97, 86)
        Me.gbDefMeth.TabIndex = 98
        Me.gbDefMeth.TabStop = False
        Me.gbDefMeth.Text = "Score Method"
        '
        'rbScore
        '
        Me.rbScore.AutoSize = True
        Me.rbScore.Location = New System.Drawing.Point(15, 60)
        Me.rbScore.Name = "rbScore"
        Me.rbScore.Size = New System.Drawing.Size(53, 17)
        Me.rbScore.TabIndex = 61
        Me.rbScore.Text = "Score"
        Me.rbScore.UseVisualStyleBackColor = True
        '
        'rbNet
        '
        Me.rbNet.AutoSize = True
        Me.rbNet.Location = New System.Drawing.Point(15, 40)
        Me.rbNet.Name = "rbNet"
        Me.rbNet.Size = New System.Drawing.Size(42, 17)
        Me.rbNet.TabIndex = 1
        Me.rbNet.Text = "Net"
        Me.rbNet.UseVisualStyleBackColor = True
        '
        'rbGross
        '
        Me.rbGross.AutoSize = True
        Me.rbGross.Checked = True
        Me.rbGross.Location = New System.Drawing.Point(15, 19)
        Me.rbGross.Name = "rbGross"
        Me.rbGross.Size = New System.Drawing.Size(52, 17)
        Me.rbGross.TabIndex = 60
        Me.rbGross.TabStop = True
        Me.rbGross.Text = "Gross"
        Me.rbGross.UseVisualStyleBackColor = True
        '
        'lbPS
        '
        Me.lbPS.AutoSize = True
        Me.lbPS.Location = New System.Drawing.Point(990, 26)
        Me.lbPS.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lbPS.Name = "lbPS"
        Me.lbPS.Size = New System.Drawing.Size(61, 13)
        Me.lbPS.TabIndex = 97
        Me.lbPS.Text = "Score Date"
        '
        'dgHandicap
        '
        Me.dgHandicap.AllowUserToAddRows = False
        Me.dgHandicap.AllowUserToDeleteRows = False
        Me.dgHandicap.AutoGenerateColumns = False
        Me.dgHandicap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgHandicap.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.H1DataGridViewTextBoxColumn, Me.H2DataGridViewTextBoxColumn, Me.H3DataGridViewTextBoxColumn, Me.H4DataGridViewTextBoxColumn, Me.H5DataGridViewTextBoxColumn, Me.H6DataGridViewTextBoxColumn, Me.H7DataGridViewTextBoxColumn, Me.H8DataGridViewTextBoxColumn, Me.H9DataGridViewTextBoxColumn, Me.H10DataGridViewTextBoxColumn, Me.H11DataGridViewTextBoxColumn, Me.H12DataGridViewTextBoxColumn, Me.H13DataGridViewTextBoxColumn, Me.H14DataGridViewTextBoxColumn, Me.H15DataGridViewTextBoxColumn, Me.H16DataGridViewTextBoxColumn, Me.H17DataGridViewTextBoxColumn, Me.H18DataGridViewTextBoxColumn})
        Me.dgHandicap.DataSource = Me.DtCoursesBindingSource
        Me.dgHandicap.Location = New System.Drawing.Point(29, 718)
        Me.dgHandicap.Name = "dgHandicap"
        Me.dgHandicap.ReadOnly = True
        Me.dgHandicap.RowHeadersWidth = 400
        Me.dgHandicap.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.dgHandicap.Size = New System.Drawing.Size(1525, 48)
        Me.dgHandicap.TabIndex = 110
        Me.dgHandicap.Visible = False
        '
        'H1DataGridViewTextBoxColumn
        '
        Me.H1DataGridViewTextBoxColumn.DataPropertyName = "H1"
        Me.H1DataGridViewTextBoxColumn.HeaderText = "H1"
        Me.H1DataGridViewTextBoxColumn.Name = "H1DataGridViewTextBoxColumn"
        Me.H1DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H2DataGridViewTextBoxColumn
        '
        Me.H2DataGridViewTextBoxColumn.DataPropertyName = "H2"
        Me.H2DataGridViewTextBoxColumn.HeaderText = "H2"
        Me.H2DataGridViewTextBoxColumn.Name = "H2DataGridViewTextBoxColumn"
        Me.H2DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H3DataGridViewTextBoxColumn
        '
        Me.H3DataGridViewTextBoxColumn.DataPropertyName = "H3"
        Me.H3DataGridViewTextBoxColumn.HeaderText = "H3"
        Me.H3DataGridViewTextBoxColumn.Name = "H3DataGridViewTextBoxColumn"
        Me.H3DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H4DataGridViewTextBoxColumn
        '
        Me.H4DataGridViewTextBoxColumn.DataPropertyName = "H4"
        Me.H4DataGridViewTextBoxColumn.HeaderText = "H4"
        Me.H4DataGridViewTextBoxColumn.Name = "H4DataGridViewTextBoxColumn"
        Me.H4DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H5DataGridViewTextBoxColumn
        '
        Me.H5DataGridViewTextBoxColumn.DataPropertyName = "H5"
        Me.H5DataGridViewTextBoxColumn.HeaderText = "H5"
        Me.H5DataGridViewTextBoxColumn.Name = "H5DataGridViewTextBoxColumn"
        Me.H5DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H6DataGridViewTextBoxColumn
        '
        Me.H6DataGridViewTextBoxColumn.DataPropertyName = "H6"
        Me.H6DataGridViewTextBoxColumn.HeaderText = "H6"
        Me.H6DataGridViewTextBoxColumn.Name = "H6DataGridViewTextBoxColumn"
        Me.H6DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H7DataGridViewTextBoxColumn
        '
        Me.H7DataGridViewTextBoxColumn.DataPropertyName = "H7"
        Me.H7DataGridViewTextBoxColumn.HeaderText = "H7"
        Me.H7DataGridViewTextBoxColumn.Name = "H7DataGridViewTextBoxColumn"
        Me.H7DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H8DataGridViewTextBoxColumn
        '
        Me.H8DataGridViewTextBoxColumn.DataPropertyName = "H8"
        Me.H8DataGridViewTextBoxColumn.HeaderText = "H8"
        Me.H8DataGridViewTextBoxColumn.Name = "H8DataGridViewTextBoxColumn"
        Me.H8DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H9DataGridViewTextBoxColumn
        '
        Me.H9DataGridViewTextBoxColumn.DataPropertyName = "H9"
        Me.H9DataGridViewTextBoxColumn.HeaderText = "H9"
        Me.H9DataGridViewTextBoxColumn.Name = "H9DataGridViewTextBoxColumn"
        Me.H9DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H10DataGridViewTextBoxColumn
        '
        Me.H10DataGridViewTextBoxColumn.DataPropertyName = "H10"
        Me.H10DataGridViewTextBoxColumn.HeaderText = "H10"
        Me.H10DataGridViewTextBoxColumn.Name = "H10DataGridViewTextBoxColumn"
        Me.H10DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H11DataGridViewTextBoxColumn
        '
        Me.H11DataGridViewTextBoxColumn.DataPropertyName = "H11"
        Me.H11DataGridViewTextBoxColumn.HeaderText = "H11"
        Me.H11DataGridViewTextBoxColumn.Name = "H11DataGridViewTextBoxColumn"
        Me.H11DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H12DataGridViewTextBoxColumn
        '
        Me.H12DataGridViewTextBoxColumn.DataPropertyName = "H12"
        Me.H12DataGridViewTextBoxColumn.HeaderText = "H12"
        Me.H12DataGridViewTextBoxColumn.Name = "H12DataGridViewTextBoxColumn"
        Me.H12DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H13DataGridViewTextBoxColumn
        '
        Me.H13DataGridViewTextBoxColumn.DataPropertyName = "H13"
        Me.H13DataGridViewTextBoxColumn.HeaderText = "H13"
        Me.H13DataGridViewTextBoxColumn.Name = "H13DataGridViewTextBoxColumn"
        Me.H13DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H14DataGridViewTextBoxColumn
        '
        Me.H14DataGridViewTextBoxColumn.DataPropertyName = "H14"
        Me.H14DataGridViewTextBoxColumn.HeaderText = "H14"
        Me.H14DataGridViewTextBoxColumn.Name = "H14DataGridViewTextBoxColumn"
        Me.H14DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H15DataGridViewTextBoxColumn
        '
        Me.H15DataGridViewTextBoxColumn.DataPropertyName = "H15"
        Me.H15DataGridViewTextBoxColumn.HeaderText = "H15"
        Me.H15DataGridViewTextBoxColumn.Name = "H15DataGridViewTextBoxColumn"
        Me.H15DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H16DataGridViewTextBoxColumn
        '
        Me.H16DataGridViewTextBoxColumn.DataPropertyName = "H16"
        Me.H16DataGridViewTextBoxColumn.HeaderText = "H16"
        Me.H16DataGridViewTextBoxColumn.Name = "H16DataGridViewTextBoxColumn"
        Me.H16DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H17DataGridViewTextBoxColumn
        '
        Me.H17DataGridViewTextBoxColumn.DataPropertyName = "H17"
        Me.H17DataGridViewTextBoxColumn.HeaderText = "H17"
        Me.H17DataGridViewTextBoxColumn.Name = "H17DataGridViewTextBoxColumn"
        Me.H17DataGridViewTextBoxColumn.ReadOnly = True
        '
        'H18DataGridViewTextBoxColumn
        '
        Me.H18DataGridViewTextBoxColumn.DataPropertyName = "H18"
        Me.H18DataGridViewTextBoxColumn.HeaderText = "H18"
        Me.H18DataGridViewTextBoxColumn.Name = "H18DataGridViewTextBoxColumn"
        Me.H18DataGridViewTextBoxColumn.ReadOnly = True
        '
        'DtCoursesBindingSource
        '
        Me.DtCoursesBindingSource.DataMember = "dtCourses"
        Me.DtCoursesBindingSource.DataSource = Me.DsLeague
        '
        'btnResetCTPs
        '
        Me.btnResetCTPs.Location = New System.Drawing.Point(989, 128)
        Me.btnResetCTPs.Margin = New System.Windows.Forms.Padding(2)
        Me.btnResetCTPs.Name = "btnResetCTPs"
        Me.btnResetCTPs.Size = New System.Drawing.Size(81, 37)
        Me.btnResetCTPs.TabIndex = 111
        Me.btnResetCTPs.TabStop = False
        Me.btnResetCTPs.Text = "Reset CTPs"
        Me.btnResetCTPs.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.GroupBox1)
        Me.GroupBox3.Controls.Add(Me.gbDefMeth)
        Me.GroupBox3.Controls.Add(Me.gbFrontBack)
        Me.GroupBox3.Controls.Add(Me.cbMatches)
        Me.GroupBox3.Controls.Add(Me.cbMarkPaid)
        Me.GroupBox3.Location = New System.Drawing.Point(52, 26)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(357, 149)
        Me.GroupBox3.TabIndex = 112
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Scoring Controls"
        '
        'btnReports
        '
        Me.btnReports.Location = New System.Drawing.Point(989, 184)
        Me.btnReports.Margin = New System.Windows.Forms.Padding(2)
        Me.btnReports.Name = "btnReports"
        Me.btnReports.Size = New System.Drawing.Size(81, 37)
        Me.btnReports.TabIndex = 113
        Me.btnReports.TabStop = False
        Me.btnReports.Text = "Create Reports"
        Me.btnReports.UseVisualStyleBackColor = True
        '
        'ScoreCard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1665, 846)
        Me.Controls.Add(Me.btnReports)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.btnResetCTPs)
        Me.Controls.Add(Me.dgHandicap)
        Me.Controls.Add(Me.gbColors)
        Me.Controls.Add(Me.gbHoleLegend)
        Me.Controls.Add(Me.gbPrizeMoney)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.cbStrokesId)
        Me.Controls.Add(Me.cbDates)
        Me.Controls.Add(Me.lbPS)
        Me.Controls.Add(Me.dgScores)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "ScoreCard"
        Me.Text = "ScoreCard"
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtScoresBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.gbFrontBack.ResumeLayout(False)
        Me.gbFrontBack.PerformLayout()
        Me.gbColors.ResumeLayout(False)
        Me.gbColors.PerformLayout()
        Me.gbHoleLegend.ResumeLayout(False)
        Me.gbHoleLegend.PerformLayout()
        Me.gbPrizeMoney.ResumeLayout(False)
        Me.gbPrizeMoney.PerformLayout()
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.cbStrokesId.ResumeLayout(False)
        Me.cbStrokesId.PerformLayout()
        Me.gbDefMeth.ResumeLayout(False)
        Me.gbDefMeth.PerformLayout()
        CType(Me.dgHandicap, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtCoursesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgScores As DataGridView
    Friend WithEvents DtScoresBindingSource As BindingSource
    Friend WithEvents DsLeague As dsLeague
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ScoringFormatHelpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StandardToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StablefordToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents rbStandard As RadioButton
    Friend WithEvents rbStableford As RadioButton
    Friend WithEvents gbFrontBack As GroupBox
    Friend WithEvents rbFront As RadioButton
    Friend WithEvents rbBack As RadioButton
    Friend WithEvents cbMatches As CheckBox
    Friend WithEvents cbMarkPaid As CheckBox
    Friend WithEvents gbColors As GroupBox
    Friend WithEvents lbUnderPar As Label
    Friend WithEvents lblSub As Label
    Friend WithEvents lblMatchTied As Label
    Friend WithEvents lblMatchWon As Label
    Friend WithEvents gbHoleLegend As GroupBox
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents gbPrizeMoney As GroupBox
    Friend WithEvents tbKitty As TextBox
    Friend WithEvents Label25 As Label
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents Label27 As Label
    Friend WithEvents Label28 As Label
    Friend WithEvents tbEachSkin As TextBox
    Friend WithEvents tbNumSkins As TextBox
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents Label22 As Label
    Friend WithEvents tbCP2Tot As TextBox
    Friend WithEvents Label23 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents tbCP1Tot As TextBox
    Friend WithEvents tbSkinTot As TextBox
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents Label21 As Label
    Friend WithEvents tbPCP2 As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents tbPCP1 As TextBox
    Friend WithEvents tbPSkins As TextBox
    Friend WithEvents tbPurse As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label20 As Label
    Friend WithEvents tbCP1 As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbCP2 As TextBox
    Friend WithEvents tbSkins As TextBox
    Friend WithEvents btnSave As Button
    Friend WithEvents lbStatus As Label
    Friend WithEvents cbStrokesId As GroupBox
    Friend WithEvents rbDots As RadioButton
    Friend WithEvents rbColors As RadioButton
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents gbDefMeth As GroupBox
    Friend WithEvents rbScore As RadioButton
    Friend WithEvents rbNet As RadioButton
    Friend WithEvents rbGross As RadioButton
    Friend WithEvents lbPS As Label
    Friend WithEvents dgHandicap As DataGridView
    Friend WithEvents H1DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H2DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H3DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H4DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H5DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H6DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H7DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H8DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H9DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H10DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H11DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H12DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H13DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H14DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H15DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H16DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H17DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents H18DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents DtCoursesBindingSource As BindingSource
    Friend WithEvents btnResetCTPs As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents SkinsDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents ClosestDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents CTP1DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents CTP2DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PlayerDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Partner As DataGridViewTextBoxColumn
    Friend WithEvents TeamDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents GradeDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents OpponentDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PointsDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents TeamPointsDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents MethodDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole1DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole2DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole3DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole4DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole5DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole6DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole7DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole8DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole9DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents OutDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents OutGrossDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents OutNetDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole10DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole11DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole12DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole13DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole14DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole15DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole16DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole17DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Hole18DataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents InDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents InGrossDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents InNetDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents GrossDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents NetDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PHdcpDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents HdcpDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents EarnDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents SkinsDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents ClosestDataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents SkinsDataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
    Friend WithEvents RoundDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents btnReports As Button
End Class
