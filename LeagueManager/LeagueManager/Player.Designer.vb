<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPlayer
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPlayer))
        Me.DsLeague = New LeagueManager.dsLeague()
        Me.DtPlayersBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.DtPlayersBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
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
        Me.DtPlayersBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        Me.DtPlayersDataGridView = New System.Windows.Forms.DataGridView()
        Me.NameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TeamDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GradeDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PaidDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EmailDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PhoneDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NickNameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DateLeftDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DateJoinedDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtPlayersBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtPlayersBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DtPlayersBindingNavigator.SuspendLayout()
        CType(Me.DtPlayersDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DsLeague
        '
        Me.DsLeague.DataSetName = "dsLeague"
        Me.DsLeague.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'DtPlayersBindingSource
        '
        Me.DtPlayersBindingSource.DataMember = "dtPlayers"
        Me.DtPlayersBindingSource.DataSource = Me.DsLeague
        '
        'DtPlayersBindingNavigator
        '
        Me.DtPlayersBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.DtPlayersBindingNavigator.BindingSource = Me.DtPlayersBindingSource
        Me.DtPlayersBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.DtPlayersBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.DtPlayersBindingNavigator.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.DtPlayersBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.DtPlayersBindingNavigatorSaveItem})
        Me.DtPlayersBindingNavigator.Location = New System.Drawing.Point(0, 0)
        Me.DtPlayersBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.DtPlayersBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.DtPlayersBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.DtPlayersBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.DtPlayersBindingNavigator.Name = "DtPlayersBindingNavigator"
        Me.DtPlayersBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.DtPlayersBindingNavigator.Size = New System.Drawing.Size(1083, 47)
        Me.DtPlayersBindingNavigator.TabIndex = 0
        Me.DtPlayersBindingNavigator.Text = "BindingNavigator1"
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
        'DtPlayersBindingNavigatorSaveItem
        '
        Me.DtPlayersBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.DtPlayersBindingNavigatorSaveItem.Image = CType(resources.GetObject("DtPlayersBindingNavigatorSaveItem.Image"), System.Drawing.Image)
        Me.DtPlayersBindingNavigatorSaveItem.Name = "DtPlayersBindingNavigatorSaveItem"
        Me.DtPlayersBindingNavigatorSaveItem.Size = New System.Drawing.Size(44, 44)
        Me.DtPlayersBindingNavigatorSaveItem.Text = "Save Data"
        '
        'DtPlayersDataGridView
        '
        Me.DtPlayersDataGridView.AutoGenerateColumns = False
        Me.DtPlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DtPlayersDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NameDataGridViewTextBoxColumn, Me.TeamDataGridViewTextBoxColumn, Me.GradeDataGridViewTextBoxColumn, Me.PaidDataGridViewTextBoxColumn, Me.EmailDataGridViewTextBoxColumn, Me.PhoneDataGridViewTextBoxColumn, Me.NickNameDataGridViewTextBoxColumn, Me.DateLeftDataGridViewTextBoxColumn, Me.DateJoinedDataGridViewTextBoxColumn})
        Me.DtPlayersDataGridView.DataSource = Me.DtPlayersBindingSource
        Me.DtPlayersDataGridView.Location = New System.Drawing.Point(33, 81)
        Me.DtPlayersDataGridView.Name = "DtPlayersDataGridView"
        Me.DtPlayersDataGridView.Size = New System.Drawing.Size(987, 657)
        Me.DtPlayersDataGridView.TabIndex = 1
        '
        'NameDataGridViewTextBoxColumn
        '
        Me.NameDataGridViewTextBoxColumn.DataPropertyName = "Name"
        Me.NameDataGridViewTextBoxColumn.HeaderText = "Name"
        Me.NameDataGridViewTextBoxColumn.Name = "NameDataGridViewTextBoxColumn"
        '
        'TeamDataGridViewTextBoxColumn
        '
        Me.TeamDataGridViewTextBoxColumn.DataPropertyName = "Team"
        Me.TeamDataGridViewTextBoxColumn.HeaderText = "Team"
        Me.TeamDataGridViewTextBoxColumn.Name = "TeamDataGridViewTextBoxColumn"
        '
        'GradeDataGridViewTextBoxColumn
        '
        Me.GradeDataGridViewTextBoxColumn.DataPropertyName = "Grade"
        Me.GradeDataGridViewTextBoxColumn.HeaderText = "Grade"
        Me.GradeDataGridViewTextBoxColumn.Name = "GradeDataGridViewTextBoxColumn"
        '
        'PaidDataGridViewTextBoxColumn
        '
        Me.PaidDataGridViewTextBoxColumn.DataPropertyName = "Paid"
        Me.PaidDataGridViewTextBoxColumn.HeaderText = "Paid"
        Me.PaidDataGridViewTextBoxColumn.Name = "PaidDataGridViewTextBoxColumn"
        '
        'EmailDataGridViewTextBoxColumn
        '
        Me.EmailDataGridViewTextBoxColumn.DataPropertyName = "Email"
        Me.EmailDataGridViewTextBoxColumn.HeaderText = "Email"
        Me.EmailDataGridViewTextBoxColumn.Name = "EmailDataGridViewTextBoxColumn"
        '
        'PhoneDataGridViewTextBoxColumn
        '
        Me.PhoneDataGridViewTextBoxColumn.DataPropertyName = "Phone"
        Me.PhoneDataGridViewTextBoxColumn.HeaderText = "Phone"
        Me.PhoneDataGridViewTextBoxColumn.Name = "PhoneDataGridViewTextBoxColumn"
        '
        'NickNameDataGridViewTextBoxColumn
        '
        Me.NickNameDataGridViewTextBoxColumn.DataPropertyName = "NickName"
        Me.NickNameDataGridViewTextBoxColumn.HeaderText = "NickName"
        Me.NickNameDataGridViewTextBoxColumn.Name = "NickNameDataGridViewTextBoxColumn"
        '
        'DateLeftDataGridViewTextBoxColumn
        '
        Me.DateLeftDataGridViewTextBoxColumn.DataPropertyName = "DateLeft"
        Me.DateLeftDataGridViewTextBoxColumn.HeaderText = "DateLeft"
        Me.DateLeftDataGridViewTextBoxColumn.Name = "DateLeftDataGridViewTextBoxColumn"
        '
        'DateJoinedDataGridViewTextBoxColumn
        '
        Me.DateJoinedDataGridViewTextBoxColumn.DataPropertyName = "DateJoined"
        Me.DateJoinedDataGridViewTextBoxColumn.HeaderText = "DateJoined"
        Me.DateJoinedDataGridViewTextBoxColumn.Name = "DateJoinedDataGridViewTextBoxColumn"
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "Name"
        Me.DataGridViewTextBoxColumn1.HeaderText = "Name"
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "Team"
        Me.DataGridViewTextBoxColumn2.HeaderText = "Team"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "Grade"
        Me.DataGridViewTextBoxColumn3.HeaderText = "Grade"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.DataPropertyName = "Paid"
        Me.DataGridViewTextBoxColumn4.HeaderText = "Paid"
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.DataPropertyName = "Email"
        Me.DataGridViewTextBoxColumn5.HeaderText = "Email"
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        '
        'DataGridViewTextBoxColumn7
        '
        Me.DataGridViewTextBoxColumn7.DataPropertyName = "NickName"
        Me.DataGridViewTextBoxColumn7.HeaderText = "NickName"
        Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
        '
        'DataGridViewTextBoxColumn8
        '
        Me.DataGridViewTextBoxColumn8.DataPropertyName = "DateLeft"
        Me.DataGridViewTextBoxColumn8.HeaderText = "DateLeft"
        Me.DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8"
        '
        'DataGridViewTextBoxColumn9
        '
        Me.DataGridViewTextBoxColumn9.DataPropertyName = "DateJoined"
        Me.DataGridViewTextBoxColumn9.HeaderText = "DateJoined"
        Me.DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9"
        '
        'frmPlayer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1083, 784)
        Me.Controls.Add(Me.DtPlayersDataGridView)
        Me.Controls.Add(Me.DtPlayersBindingNavigator)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmPlayer"
        Me.Text = "Player"
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtPlayersBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtPlayersBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DtPlayersBindingNavigator.ResumeLayout(False)
        Me.DtPlayersBindingNavigator.PerformLayout()
        CType(Me.DtPlayersDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DsLeague As dsLeague
    Friend WithEvents DtPlayersBindingSource As BindingSource
    Friend WithEvents DtPlayersBindingNavigator As BindingNavigator
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
    Friend WithEvents DtPlayersBindingNavigatorSaveItem As ToolStripButton
    Friend WithEvents DtPlayersDataGridView As DataGridView
    Friend WithEvents DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn6 As DataGridViewMaskedEditColumn
    Friend WithEvents DataGridViewTextBoxColumn7 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn8 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn9 As DataGridViewTextBoxColumn
    Friend WithEvents NameDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents TeamDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents GradeDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PaidDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents EmailDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents PhoneDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents NickNameDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents DateLeftDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents DateJoinedDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
End Class
