<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLeagueSetup
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
        Dim NameLabel As System.Windows.Forms.Label
        Dim SecretaryLabel As System.Windows.Forms.Label
        Dim FormatLabel As System.Windows.Forms.Label
        Dim CostLabel As System.Windows.Forms.Label
        Dim TeamsLabel As System.Windows.Forms.Label
        Dim PayPlacesLabel As System.Windows.Forms.Label
        Dim StartDateLabel As System.Windows.Forms.Label
        Dim EndDateLabel As System.Windows.Forms.Label
        Dim HdcpScoresLabel As System.Windows.Forms.Label
        Dim HdcpFormatLabel As System.Windows.Forms.Label
        Dim CarryLastYearsLabel As System.Windows.Forms.Label
        Dim MaxHdcpLabel As System.Windows.Forms.Label
        Dim Par3MaxLabel As System.Windows.Forms.Label
        Dim Par4MaxLabel As System.Windows.Forms.Label
        Dim Par5MaxLabel As System.Windows.Forms.Label
        Dim CourseLabel As System.Windows.Forms.Label
        Dim HolesLabel As System.Windows.Forms.Label
        Dim Start9Label As System.Windows.Forms.Label
        Dim MethodLabel As System.Windows.Forms.Label
        Dim SkinsLabel As System.Windows.Forms.Label
        Dim ClosestLabel As System.Windows.Forms.Label
        Dim SplitSeasonLabel As System.Windows.Forms.Label
        Dim PostSeasonLabel As System.Windows.Forms.Label
        Dim EmailLabel As System.Windows.Forms.Label
        Dim EmailPasswordLabel As System.Windows.Forms.Label
        Dim SkinFmtLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLeagueSetup))
        Me.DtLeagueParmsBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
        Me.DtLeagueParmsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.DsLeague = New LeagueManager.dsLeague()
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.DtLeagueParmsBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        Me.NameTextBox = New System.Windows.Forms.TextBox()
        Me.SecretaryTextBox = New System.Windows.Forms.TextBox()
        Me.FormatTextBox = New System.Windows.Forms.TextBox()
        Me.CostTextBox = New System.Windows.Forms.TextBox()
        Me.TeamsTextBox = New System.Windows.Forms.TextBox()
        Me.PayPlacesTextBox = New System.Windows.Forms.TextBox()
        Me.StartDateTextBox = New System.Windows.Forms.TextBox()
        Me.EndDateTextBox = New System.Windows.Forms.TextBox()
        Me.HdcpScoresTextBox = New System.Windows.Forms.TextBox()
        Me.HdcpFormatTextBox = New System.Windows.Forms.TextBox()
        Me.CarryLastYearsTextBox = New System.Windows.Forms.TextBox()
        Me.MaxHdcpTextBox = New System.Windows.Forms.TextBox()
        Me.Par3MaxTextBox = New System.Windows.Forms.TextBox()
        Me.Par4MaxTextBox = New System.Windows.Forms.TextBox()
        Me.Par5MaxTextBox = New System.Windows.Forms.TextBox()
        Me.CourseTextBox = New System.Windows.Forms.TextBox()
        Me.HolesTextBox = New System.Windows.Forms.TextBox()
        Me.Start9TextBox = New System.Windows.Forms.TextBox()
        Me.MethodTextBox = New System.Windows.Forms.TextBox()
        Me.SkinsTextBox = New System.Windows.Forms.TextBox()
        Me.ClosestTextBox = New System.Windows.Forms.TextBox()
        Me.SplitSeasonTextBox = New System.Windows.Forms.TextBox()
        Me.PostSeasonDtTextBox = New System.Windows.Forms.TextBox()
        Me.EmailTextBox = New System.Windows.Forms.TextBox()
        Me.EmailPasswordTextBox = New System.Windows.Forms.TextBox()
        Me.SkinFmtTextBox = New System.Windows.Forms.TextBox()
        NameLabel = New System.Windows.Forms.Label()
        SecretaryLabel = New System.Windows.Forms.Label()
        FormatLabel = New System.Windows.Forms.Label()
        CostLabel = New System.Windows.Forms.Label()
        TeamsLabel = New System.Windows.Forms.Label()
        PayPlacesLabel = New System.Windows.Forms.Label()
        StartDateLabel = New System.Windows.Forms.Label()
        EndDateLabel = New System.Windows.Forms.Label()
        HdcpScoresLabel = New System.Windows.Forms.Label()
        HdcpFormatLabel = New System.Windows.Forms.Label()
        CarryLastYearsLabel = New System.Windows.Forms.Label()
        MaxHdcpLabel = New System.Windows.Forms.Label()
        Par3MaxLabel = New System.Windows.Forms.Label()
        Par4MaxLabel = New System.Windows.Forms.Label()
        Par5MaxLabel = New System.Windows.Forms.Label()
        CourseLabel = New System.Windows.Forms.Label()
        HolesLabel = New System.Windows.Forms.Label()
        Start9Label = New System.Windows.Forms.Label()
        MethodLabel = New System.Windows.Forms.Label()
        SkinsLabel = New System.Windows.Forms.Label()
        ClosestLabel = New System.Windows.Forms.Label()
        SplitSeasonLabel = New System.Windows.Forms.Label()
        PostSeasonLabel = New System.Windows.Forms.Label()
        EmailLabel = New System.Windows.Forms.Label()
        EmailPasswordLabel = New System.Windows.Forms.Label()
        SkinFmtLabel = New System.Windows.Forms.Label()
        CType(Me.DtLeagueParmsBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DtLeagueParmsBindingNavigator.SuspendLayout()
        CType(Me.DtLeagueParmsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'NameLabel
        '
        NameLabel.AutoSize = True
        NameLabel.Location = New System.Drawing.Point(22, 57)
        NameLabel.Name = "NameLabel"
        NameLabel.Size = New System.Drawing.Size(38, 13)
        NameLabel.TabIndex = 7
        NameLabel.Text = "Name:"
        '
        'SecretaryLabel
        '
        SecretaryLabel.AutoSize = True
        SecretaryLabel.Location = New System.Drawing.Point(22, 83)
        SecretaryLabel.Name = "SecretaryLabel"
        SecretaryLabel.Size = New System.Drawing.Size(55, 13)
        SecretaryLabel.TabIndex = 9
        SecretaryLabel.Text = "Secretary:"
        '
        'FormatLabel
        '
        FormatLabel.AutoSize = True
        FormatLabel.Location = New System.Drawing.Point(22, 163)
        FormatLabel.Name = "FormatLabel"
        FormatLabel.Size = New System.Drawing.Size(42, 13)
        FormatLabel.TabIndex = 11
        FormatLabel.Text = "Format:"
        '
        'CostLabel
        '
        CostLabel.AutoSize = True
        CostLabel.Location = New System.Drawing.Point(22, 189)
        CostLabel.Name = "CostLabel"
        CostLabel.Size = New System.Drawing.Size(31, 13)
        CostLabel.TabIndex = 13
        CostLabel.Text = "Cost:"
        '
        'TeamsLabel
        '
        TeamsLabel.AutoSize = True
        TeamsLabel.Location = New System.Drawing.Point(22, 215)
        TeamsLabel.Name = "TeamsLabel"
        TeamsLabel.Size = New System.Drawing.Size(42, 13)
        TeamsLabel.TabIndex = 15
        TeamsLabel.Text = "Teams:"
        '
        'PayPlacesLabel
        '
        PayPlacesLabel.AutoSize = True
        PayPlacesLabel.Location = New System.Drawing.Point(22, 241)
        PayPlacesLabel.Name = "PayPlacesLabel"
        PayPlacesLabel.Size = New System.Drawing.Size(63, 13)
        PayPlacesLabel.TabIndex = 17
        PayPlacesLabel.Text = "Pay Places:"
        '
        'StartDateLabel
        '
        StartDateLabel.AutoSize = True
        StartDateLabel.Location = New System.Drawing.Point(22, 267)
        StartDateLabel.Name = "StartDateLabel"
        StartDateLabel.Size = New System.Drawing.Size(58, 13)
        StartDateLabel.TabIndex = 19
        StartDateLabel.Text = "Start Date:"
        '
        'EndDateLabel
        '
        EndDateLabel.AutoSize = True
        EndDateLabel.Location = New System.Drawing.Point(22, 293)
        EndDateLabel.Name = "EndDateLabel"
        EndDateLabel.Size = New System.Drawing.Size(55, 13)
        EndDateLabel.TabIndex = 21
        EndDateLabel.Text = "End Date:"
        '
        'HdcpScoresLabel
        '
        HdcpScoresLabel.AutoSize = True
        HdcpScoresLabel.Location = New System.Drawing.Point(22, 319)
        HdcpScoresLabel.Name = "HdcpScoresLabel"
        HdcpScoresLabel.Size = New System.Drawing.Size(72, 13)
        HdcpScoresLabel.TabIndex = 23
        HdcpScoresLabel.Text = "Hdcp Scores:"
        '
        'HdcpFormatLabel
        '
        HdcpFormatLabel.AutoSize = True
        HdcpFormatLabel.Location = New System.Drawing.Point(22, 345)
        HdcpFormatLabel.Name = "HdcpFormatLabel"
        HdcpFormatLabel.Size = New System.Drawing.Size(71, 13)
        HdcpFormatLabel.TabIndex = 25
        HdcpFormatLabel.Text = "Hdcp Format:"
        '
        'CarryLastYearsLabel
        '
        CarryLastYearsLabel.AutoSize = True
        CarryLastYearsLabel.Location = New System.Drawing.Point(22, 371)
        CarryLastYearsLabel.Name = "CarryLastYearsLabel"
        CarryLastYearsLabel.Size = New System.Drawing.Size(87, 13)
        CarryLastYearsLabel.TabIndex = 27
        CarryLastYearsLabel.Text = "Carry Last Years:"
        '
        'MaxHdcpLabel
        '
        MaxHdcpLabel.AutoSize = True
        MaxHdcpLabel.Location = New System.Drawing.Point(22, 397)
        MaxHdcpLabel.Name = "MaxHdcpLabel"
        MaxHdcpLabel.Size = New System.Drawing.Size(59, 13)
        MaxHdcpLabel.TabIndex = 29
        MaxHdcpLabel.Text = "Max Hdcp:"
        '
        'Par3MaxLabel
        '
        Par3MaxLabel.AutoSize = True
        Par3MaxLabel.Location = New System.Drawing.Point(22, 423)
        Par3MaxLabel.Name = "Par3MaxLabel"
        Par3MaxLabel.Size = New System.Drawing.Size(52, 13)
        Par3MaxLabel.TabIndex = 31
        Par3MaxLabel.Text = "Par3Max:"
        '
        'Par4MaxLabel
        '
        Par4MaxLabel.AutoSize = True
        Par4MaxLabel.Location = New System.Drawing.Point(22, 449)
        Par4MaxLabel.Name = "Par4MaxLabel"
        Par4MaxLabel.Size = New System.Drawing.Size(52, 13)
        Par4MaxLabel.TabIndex = 33
        Par4MaxLabel.Text = "Par4Max:"
        '
        'Par5MaxLabel
        '
        Par5MaxLabel.AutoSize = True
        Par5MaxLabel.Location = New System.Drawing.Point(22, 475)
        Par5MaxLabel.Name = "Par5MaxLabel"
        Par5MaxLabel.Size = New System.Drawing.Size(52, 13)
        Par5MaxLabel.TabIndex = 35
        Par5MaxLabel.Text = "Par5Max:"
        '
        'CourseLabel
        '
        CourseLabel.AutoSize = True
        CourseLabel.Location = New System.Drawing.Point(22, 501)
        CourseLabel.Name = "CourseLabel"
        CourseLabel.Size = New System.Drawing.Size(43, 13)
        CourseLabel.TabIndex = 37
        CourseLabel.Text = "Course:"
        '
        'HolesLabel
        '
        HolesLabel.AutoSize = True
        HolesLabel.Location = New System.Drawing.Point(22, 527)
        HolesLabel.Name = "HolesLabel"
        HolesLabel.Size = New System.Drawing.Size(37, 13)
        HolesLabel.TabIndex = 39
        HolesLabel.Text = "Holes:"
        '
        'Start9Label
        '
        Start9Label.AutoSize = True
        Start9Label.Location = New System.Drawing.Point(22, 553)
        Start9Label.Name = "Start9Label"
        Start9Label.Size = New System.Drawing.Size(38, 13)
        Start9Label.TabIndex = 41
        Start9Label.Text = "Start9:"
        '
        'MethodLabel
        '
        MethodLabel.AutoSize = True
        MethodLabel.Location = New System.Drawing.Point(22, 579)
        MethodLabel.Name = "MethodLabel"
        MethodLabel.Size = New System.Drawing.Size(46, 13)
        MethodLabel.TabIndex = 43
        MethodLabel.Text = "Method:"
        '
        'SkinsLabel
        '
        SkinsLabel.AutoSize = True
        SkinsLabel.Location = New System.Drawing.Point(22, 605)
        SkinsLabel.Name = "SkinsLabel"
        SkinsLabel.Size = New System.Drawing.Size(36, 13)
        SkinsLabel.TabIndex = 45
        SkinsLabel.Text = "Skins:"
        '
        'ClosestLabel
        '
        ClosestLabel.AutoSize = True
        ClosestLabel.Location = New System.Drawing.Point(22, 631)
        ClosestLabel.Name = "ClosestLabel"
        ClosestLabel.Size = New System.Drawing.Size(44, 13)
        ClosestLabel.TabIndex = 47
        ClosestLabel.Text = "Closest:"
        '
        'SplitSeasonLabel
        '
        SplitSeasonLabel.AutoSize = True
        SplitSeasonLabel.Location = New System.Drawing.Point(22, 657)
        SplitSeasonLabel.Name = "SplitSeasonLabel"
        SplitSeasonLabel.Size = New System.Drawing.Size(69, 13)
        SplitSeasonLabel.TabIndex = 49
        SplitSeasonLabel.Text = "Split Season:"
        '
        'PostSeasonLabel
        '
        PostSeasonLabel.AutoSize = True
        PostSeasonLabel.Location = New System.Drawing.Point(22, 683)
        PostSeasonLabel.Name = "PostSeasonLabel"
        PostSeasonLabel.Size = New System.Drawing.Size(84, 13)
        PostSeasonLabel.TabIndex = 51
        PostSeasonLabel.Text = "Post Season Dt:"
        '
        'EmailLabel
        '
        EmailLabel.AutoSize = True
        EmailLabel.Location = New System.Drawing.Point(22, 111)
        EmailLabel.Name = "EmailLabel"
        EmailLabel.Size = New System.Drawing.Size(35, 13)
        EmailLabel.TabIndex = 53
        EmailLabel.Text = "Email:"
        '
        'EmailPasswordLabel
        '
        EmailPasswordLabel.AutoSize = True
        EmailPasswordLabel.Location = New System.Drawing.Point(22, 137)
        EmailPasswordLabel.Name = "EmailPasswordLabel"
        EmailPasswordLabel.Size = New System.Drawing.Size(84, 13)
        EmailPasswordLabel.TabIndex = 54
        EmailPasswordLabel.Text = "Email Password:"
        '
        'SkinFmtLabel
        '
        SkinFmtLabel.AutoSize = True
        SkinFmtLabel.Location = New System.Drawing.Point(23, 709)
        SkinFmtLabel.Name = "SkinFmtLabel"
        SkinFmtLabel.Size = New System.Drawing.Size(51, 13)
        SkinFmtLabel.TabIndex = 52
        SkinFmtLabel.Text = "Skin Fmt:"
        '
        'DtLeagueParmsBindingNavigator
        '
        Me.DtLeagueParmsBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.DtLeagueParmsBindingNavigator.BindingSource = Me.DtLeagueParmsBindingSource
        Me.DtLeagueParmsBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.DtLeagueParmsBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.DtLeagueParmsBindingNavigator.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.DtLeagueParmsBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.DtLeagueParmsBindingNavigatorSaveItem})
        Me.DtLeagueParmsBindingNavigator.Location = New System.Drawing.Point(0, 0)
        Me.DtLeagueParmsBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.DtLeagueParmsBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.DtLeagueParmsBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.DtLeagueParmsBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.DtLeagueParmsBindingNavigator.Name = "DtLeagueParmsBindingNavigator"
        Me.DtLeagueParmsBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.DtLeagueParmsBindingNavigator.Size = New System.Drawing.Size(436, 47)
        Me.DtLeagueParmsBindingNavigator.TabIndex = 6
        Me.DtLeagueParmsBindingNavigator.Text = "BindingNavigator1"
        '
        'BindingNavigatorAddNewItem
        '
        Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
        Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(44, 44)
        Me.BindingNavigatorAddNewItem.Text = "Add new"
        '
        'DtLeagueParmsBindingSource
        '
        Me.DtLeagueParmsBindingSource.DataMember = "dtLeagueParms"
        Me.DtLeagueParmsBindingSource.DataSource = Me.DsLeague
        '
        'DsLeague
        '
        Me.DsLeague.DataSetName = "dsLeague"
        Me.DsLeague.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(35, 44)
        Me.BindingNavigatorCountItem.Text = "of {0}"
        Me.BindingNavigatorCountItem.ToolTipText = "Total number of items"
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
        Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(44, 44)
        Me.BindingNavigatorDeleteItem.Text = "Delete"
        '
        'BindingNavigatorMoveFirstItem
        '
        Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
        Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(44, 44)
        Me.BindingNavigatorMoveFirstItem.Text = "Move first"
        '
        'BindingNavigatorMovePreviousItem
        '
        Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
        Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(44, 44)
        Me.BindingNavigatorMovePreviousItem.Text = "Move previous"
        '
        'BindingNavigatorSeparator
        '
        Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
        Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 47)
        '
        'BindingNavigatorPositionItem
        '
        Me.BindingNavigatorPositionItem.AccessibleName = "Position"
        Me.BindingNavigatorPositionItem.AutoSize = False
        Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
        Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 23)
        Me.BindingNavigatorPositionItem.Text = "0"
        Me.BindingNavigatorPositionItem.ToolTipText = "Current position"
        '
        'BindingNavigatorSeparator1
        '
        Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
        Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 47)
        '
        'BindingNavigatorMoveNextItem
        '
        Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
        Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(44, 44)
        Me.BindingNavigatorMoveNextItem.Text = "Move next"
        '
        'BindingNavigatorMoveLastItem
        '
        Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
        Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(44, 44)
        Me.BindingNavigatorMoveLastItem.Text = "Move last"
        '
        'BindingNavigatorSeparator2
        '
        Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
        Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 47)
        '
        'DtLeagueParmsBindingNavigatorSaveItem
        '
        Me.DtLeagueParmsBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.DtLeagueParmsBindingNavigatorSaveItem.Image = CType(resources.GetObject("DtLeagueParmsBindingNavigatorSaveItem.Image"), System.Drawing.Image)
        Me.DtLeagueParmsBindingNavigatorSaveItem.Name = "DtLeagueParmsBindingNavigatorSaveItem"
        Me.DtLeagueParmsBindingNavigatorSaveItem.Size = New System.Drawing.Size(44, 44)
        Me.DtLeagueParmsBindingNavigatorSaveItem.Text = "Save Data"
        '
        'NameTextBox
        '
        Me.NameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Name", True))
        Me.NameTextBox.Location = New System.Drawing.Point(115, 54)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.Size = New System.Drawing.Size(122, 20)
        Me.NameTextBox.TabIndex = 8
        '
        'SecretaryTextBox
        '
        Me.SecretaryTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Secretary", True))
        Me.SecretaryTextBox.Location = New System.Drawing.Point(115, 80)
        Me.SecretaryTextBox.Name = "SecretaryTextBox"
        Me.SecretaryTextBox.Size = New System.Drawing.Size(122, 20)
        Me.SecretaryTextBox.TabIndex = 10
        '
        'FormatTextBox
        '
        Me.FormatTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Format", True))
        Me.FormatTextBox.Location = New System.Drawing.Point(115, 160)
        Me.FormatTextBox.Name = "FormatTextBox"
        Me.FormatTextBox.Size = New System.Drawing.Size(85, 20)
        Me.FormatTextBox.TabIndex = 12
        '
        'CostTextBox
        '
        Me.CostTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Cost", True))
        Me.CostTextBox.Location = New System.Drawing.Point(115, 186)
        Me.CostTextBox.Name = "CostTextBox"
        Me.CostTextBox.Size = New System.Drawing.Size(30, 20)
        Me.CostTextBox.TabIndex = 14
        '
        'TeamsTextBox
        '
        Me.TeamsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Teams", True))
        Me.TeamsTextBox.Location = New System.Drawing.Point(115, 212)
        Me.TeamsTextBox.Name = "TeamsTextBox"
        Me.TeamsTextBox.Size = New System.Drawing.Size(30, 20)
        Me.TeamsTextBox.TabIndex = 16
        '
        'PayPlacesTextBox
        '
        Me.PayPlacesTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "PayPlaces", True))
        Me.PayPlacesTextBox.Location = New System.Drawing.Point(115, 238)
        Me.PayPlacesTextBox.Name = "PayPlacesTextBox"
        Me.PayPlacesTextBox.Size = New System.Drawing.Size(30, 20)
        Me.PayPlacesTextBox.TabIndex = 18
        '
        'StartDateTextBox
        '
        Me.StartDateTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "StartDate", True))
        Me.StartDateTextBox.Location = New System.Drawing.Point(115, 264)
        Me.StartDateTextBox.Name = "StartDateTextBox"
        Me.StartDateTextBox.Size = New System.Drawing.Size(63, 20)
        Me.StartDateTextBox.TabIndex = 20
        '
        'EndDateTextBox
        '
        Me.EndDateTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "EndDate", True))
        Me.EndDateTextBox.Location = New System.Drawing.Point(115, 290)
        Me.EndDateTextBox.Name = "EndDateTextBox"
        Me.EndDateTextBox.Size = New System.Drawing.Size(63, 20)
        Me.EndDateTextBox.TabIndex = 22
        '
        'HdcpScoresTextBox
        '
        Me.HdcpScoresTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "HdcpScores", True))
        Me.HdcpScoresTextBox.Location = New System.Drawing.Point(115, 316)
        Me.HdcpScoresTextBox.Name = "HdcpScoresTextBox"
        Me.HdcpScoresTextBox.Size = New System.Drawing.Size(30, 20)
        Me.HdcpScoresTextBox.TabIndex = 24
        '
        'HdcpFormatTextBox
        '
        Me.HdcpFormatTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "HdcpFormat", True))
        Me.HdcpFormatTextBox.Location = New System.Drawing.Point(115, 342)
        Me.HdcpFormatTextBox.Name = "HdcpFormatTextBox"
        Me.HdcpFormatTextBox.Size = New System.Drawing.Size(30, 20)
        Me.HdcpFormatTextBox.TabIndex = 26
        '
        'CarryLastYearsTextBox
        '
        Me.CarryLastYearsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "CarryLastYears", True))
        Me.CarryLastYearsTextBox.Location = New System.Drawing.Point(115, 368)
        Me.CarryLastYearsTextBox.Name = "CarryLastYearsTextBox"
        Me.CarryLastYearsTextBox.Size = New System.Drawing.Size(30, 20)
        Me.CarryLastYearsTextBox.TabIndex = 28
        '
        'MaxHdcpTextBox
        '
        Me.MaxHdcpTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "MaxHdcp", True))
        Me.MaxHdcpTextBox.Location = New System.Drawing.Point(115, 394)
        Me.MaxHdcpTextBox.Name = "MaxHdcpTextBox"
        Me.MaxHdcpTextBox.Size = New System.Drawing.Size(30, 20)
        Me.MaxHdcpTextBox.TabIndex = 30
        '
        'Par3MaxTextBox
        '
        Me.Par3MaxTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Par3Max", True))
        Me.Par3MaxTextBox.Location = New System.Drawing.Point(115, 420)
        Me.Par3MaxTextBox.Name = "Par3MaxTextBox"
        Me.Par3MaxTextBox.Size = New System.Drawing.Size(30, 20)
        Me.Par3MaxTextBox.TabIndex = 32
        '
        'Par4MaxTextBox
        '
        Me.Par4MaxTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Par4Max", True))
        Me.Par4MaxTextBox.Location = New System.Drawing.Point(115, 446)
        Me.Par4MaxTextBox.Name = "Par4MaxTextBox"
        Me.Par4MaxTextBox.Size = New System.Drawing.Size(30, 20)
        Me.Par4MaxTextBox.TabIndex = 34
        '
        'Par5MaxTextBox
        '
        Me.Par5MaxTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Par5Max", True))
        Me.Par5MaxTextBox.Location = New System.Drawing.Point(115, 472)
        Me.Par5MaxTextBox.Name = "Par5MaxTextBox"
        Me.Par5MaxTextBox.Size = New System.Drawing.Size(30, 20)
        Me.Par5MaxTextBox.TabIndex = 36
        '
        'CourseTextBox
        '
        Me.CourseTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Course", True))
        Me.CourseTextBox.Location = New System.Drawing.Point(115, 498)
        Me.CourseTextBox.Name = "CourseTextBox"
        Me.CourseTextBox.Size = New System.Drawing.Size(122, 20)
        Me.CourseTextBox.TabIndex = 38
        '
        'HolesTextBox
        '
        Me.HolesTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Holes", True))
        Me.HolesTextBox.Location = New System.Drawing.Point(115, 524)
        Me.HolesTextBox.Name = "HolesTextBox"
        Me.HolesTextBox.Size = New System.Drawing.Size(30, 20)
        Me.HolesTextBox.TabIndex = 40
        '
        'Start9TextBox
        '
        Me.Start9TextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Start9", True))
        Me.Start9TextBox.Location = New System.Drawing.Point(115, 550)
        Me.Start9TextBox.Name = "Start9TextBox"
        Me.Start9TextBox.Size = New System.Drawing.Size(30, 20)
        Me.Start9TextBox.TabIndex = 42
        '
        'MethodTextBox
        '
        Me.MethodTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Method", True))
        Me.MethodTextBox.Location = New System.Drawing.Point(115, 576)
        Me.MethodTextBox.Name = "MethodTextBox"
        Me.MethodTextBox.Size = New System.Drawing.Size(63, 20)
        Me.MethodTextBox.TabIndex = 44
        '
        'SkinsTextBox
        '
        Me.SkinsTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Skins", True))
        Me.SkinsTextBox.Location = New System.Drawing.Point(115, 602)
        Me.SkinsTextBox.Name = "SkinsTextBox"
        Me.SkinsTextBox.Size = New System.Drawing.Size(30, 20)
        Me.SkinsTextBox.TabIndex = 46
        '
        'ClosestTextBox
        '
        Me.ClosestTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Closest", True))
        Me.ClosestTextBox.Location = New System.Drawing.Point(115, 628)
        Me.ClosestTextBox.Name = "ClosestTextBox"
        Me.ClosestTextBox.Size = New System.Drawing.Size(30, 20)
        Me.ClosestTextBox.TabIndex = 48
        '
        'SplitSeasonTextBox
        '
        Me.SplitSeasonTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "SplitSeason", True))
        Me.SplitSeasonTextBox.Location = New System.Drawing.Point(115, 654)
        Me.SplitSeasonTextBox.Name = "SplitSeasonTextBox"
        Me.SplitSeasonTextBox.Size = New System.Drawing.Size(30, 20)
        Me.SplitSeasonTextBox.TabIndex = 50
        '
        'PostSeasonDtTextBox
        '
        Me.PostSeasonDtTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "PostSeasonDt", True))
        Me.PostSeasonDtTextBox.Location = New System.Drawing.Point(115, 680)
        Me.PostSeasonDtTextBox.Name = "PostSeasonDtTextBox"
        Me.PostSeasonDtTextBox.Size = New System.Drawing.Size(63, 20)
        Me.PostSeasonDtTextBox.TabIndex = 52
        '
        'EmailTextBox
        '
        Me.EmailTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "Email", True))
        Me.EmailTextBox.Location = New System.Drawing.Point(115, 108)
        Me.EmailTextBox.Name = "EmailTextBox"
        Me.EmailTextBox.Size = New System.Drawing.Size(157, 20)
        Me.EmailTextBox.TabIndex = 54
        '
        'EmailPasswordTextBox
        '
        Me.EmailPasswordTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "EmailPassword", True))
        Me.EmailPasswordTextBox.Location = New System.Drawing.Point(115, 134)
        Me.EmailPasswordTextBox.Name = "EmailPasswordTextBox"
        Me.EmailPasswordTextBox.Size = New System.Drawing.Size(134, 20)
        Me.EmailPasswordTextBox.TabIndex = 55
        '
        'SkinFmtTextBox
        '
        Me.SkinFmtTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DtLeagueParmsBindingSource, "SkinFmt", True))
        Me.SkinFmtTextBox.Location = New System.Drawing.Point(115, 709)
        Me.SkinFmtTextBox.Name = "SkinFmtTextBox"
        Me.SkinFmtTextBox.Size = New System.Drawing.Size(76, 20)
        Me.SkinFmtTextBox.TabIndex = 53
        '
        'frmLeagueSetup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(436, 799)
        Me.Controls.Add(EmailPasswordLabel)
        Me.Controls.Add(Me.EmailPasswordTextBox)
        Me.Controls.Add(EmailLabel)
        Me.Controls.Add(Me.EmailTextBox)
        Me.Controls.Add(SkinFmtLabel)
        Me.Controls.Add(Me.SkinFmtTextBox)
        Me.Controls.Add(NameLabel)
        Me.Controls.Add(Me.NameTextBox)
        Me.Controls.Add(SecretaryLabel)
        Me.Controls.Add(Me.SecretaryTextBox)
        Me.Controls.Add(FormatLabel)
        Me.Controls.Add(Me.FormatTextBox)
        Me.Controls.Add(CostLabel)
        Me.Controls.Add(Me.CostTextBox)
        Me.Controls.Add(TeamsLabel)
        Me.Controls.Add(Me.TeamsTextBox)
        Me.Controls.Add(PayPlacesLabel)
        Me.Controls.Add(Me.PayPlacesTextBox)
        Me.Controls.Add(StartDateLabel)
        Me.Controls.Add(Me.StartDateTextBox)
        Me.Controls.Add(EndDateLabel)
        Me.Controls.Add(Me.EndDateTextBox)
        Me.Controls.Add(HdcpScoresLabel)
        Me.Controls.Add(Me.HdcpScoresTextBox)
        Me.Controls.Add(HdcpFormatLabel)
        Me.Controls.Add(Me.HdcpFormatTextBox)
        Me.Controls.Add(CarryLastYearsLabel)
        Me.Controls.Add(Me.CarryLastYearsTextBox)
        Me.Controls.Add(MaxHdcpLabel)
        Me.Controls.Add(Me.MaxHdcpTextBox)
        Me.Controls.Add(Par3MaxLabel)
        Me.Controls.Add(Me.Par3MaxTextBox)
        Me.Controls.Add(Par4MaxLabel)
        Me.Controls.Add(Me.Par4MaxTextBox)
        Me.Controls.Add(Par5MaxLabel)
        Me.Controls.Add(Me.Par5MaxTextBox)
        Me.Controls.Add(CourseLabel)
        Me.Controls.Add(Me.CourseTextBox)
        Me.Controls.Add(HolesLabel)
        Me.Controls.Add(Me.HolesTextBox)
        Me.Controls.Add(Start9Label)
        Me.Controls.Add(Me.Start9TextBox)
        Me.Controls.Add(MethodLabel)
        Me.Controls.Add(Me.MethodTextBox)
        Me.Controls.Add(SkinsLabel)
        Me.Controls.Add(Me.SkinsTextBox)
        Me.Controls.Add(ClosestLabel)
        Me.Controls.Add(Me.ClosestTextBox)
        Me.Controls.Add(SplitSeasonLabel)
        Me.Controls.Add(Me.SplitSeasonTextBox)
        Me.Controls.Add(PostSeasonLabel)
        Me.Controls.Add(Me.PostSeasonDtTextBox)
        Me.Controls.Add(Me.DtLeagueParmsBindingNavigator)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmLeagueSetup"
        Me.Text = "LeagueSetup"
        CType(Me.DtLeagueParmsBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DtLeagueParmsBindingNavigator.ResumeLayout(False)
        Me.DtLeagueParmsBindingNavigator.PerformLayout()
        CType(Me.DtLeagueParmsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DsLeague As dsLeague
    Friend WithEvents DtLeagueParmsBindingSource As BindingSource
    Friend WithEvents DtLeagueParmsBindingNavigator As BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As ToolStripSeparator
    Friend WithEvents DtLeagueParmsBindingNavigatorSaveItem As ToolStripButton
    Friend WithEvents NameTextBox As TextBox
    Friend WithEvents SecretaryTextBox As TextBox
    Friend WithEvents FormatTextBox As TextBox
    Friend WithEvents CostTextBox As TextBox
    Friend WithEvents TeamsTextBox As TextBox
    Friend WithEvents PayPlacesTextBox As TextBox
    Friend WithEvents StartDateTextBox As TextBox
    Friend WithEvents EndDateTextBox As TextBox
    Friend WithEvents HdcpScoresTextBox As TextBox
    Friend WithEvents HdcpFormatTextBox As TextBox
    Friend WithEvents CarryLastYearsTextBox As TextBox
    Friend WithEvents MaxHdcpTextBox As TextBox
    Friend WithEvents Par3MaxTextBox As TextBox
    Friend WithEvents Par4MaxTextBox As TextBox
    Friend WithEvents Par5MaxTextBox As TextBox
    Friend WithEvents CourseTextBox As TextBox
    Friend WithEvents HolesTextBox As TextBox
    Friend WithEvents Start9TextBox As TextBox
    Friend WithEvents MethodTextBox As TextBox
    Friend WithEvents SkinsTextBox As TextBox
    Friend WithEvents ClosestTextBox As TextBox
    Friend WithEvents SplitSeasonTextBox As TextBox
    Friend WithEvents PostSeasonDtTextBox As TextBox
    Friend WithEvents EmailTextBox As TextBox
    Friend WithEvents EmailPasswordTextBox As TextBox
    Friend WithEvents SkinFmtTextBox As TextBox
End Class
