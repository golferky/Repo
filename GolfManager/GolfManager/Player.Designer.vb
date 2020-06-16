<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Player
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.DsLeague = New GolfManager.dsLeague()
        Me.DtPlayersBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.DtPlayersDataGridView = New System.Windows.Forms.DataGridView()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbRegulars = New System.Windows.Forms.RadioButton()
        Me.rbAll = New System.Windows.Forms.RadioButton()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.InSkins = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.InCTP = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CommPref = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.EmailStats = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TextStats = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtPlayersBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtPlayersDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
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
        'DtPlayersDataGridView
        '
        Me.DtPlayersDataGridView.AutoGenerateColumns = False
        Me.DtPlayersDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.DtPlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DtPlayersDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3, Me.InSkins, Me.InCTP, Me.CommPref, Me.DataGridViewTextBoxColumn5, Me.EmailStats, Me.TextStats, Me.DataGridViewTextBoxColumn6, Me.DataGridViewTextBoxColumn10, Me.DataGridViewTextBoxColumn11})
        Me.DtPlayersDataGridView.DataSource = Me.DtPlayersBindingSource
        Me.DtPlayersDataGridView.Location = New System.Drawing.Point(63, 88)
        Me.DtPlayersDataGridView.Name = "DtPlayersDataGridView"
        Me.DtPlayersDataGridView.Size = New System.Drawing.Size(1130, 387)
        Me.DtPlayersDataGridView.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbRegulars)
        Me.GroupBox1.Controls.Add(Me.rbAll)
        Me.GroupBox1.Location = New System.Drawing.Point(63, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(106, 63)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "View Filter"
        '
        'rbRegulars
        '
        Me.rbRegulars.AutoSize = True
        Me.rbRegulars.Location = New System.Drawing.Point(7, 43)
        Me.rbRegulars.Name = "rbRegulars"
        Me.rbRegulars.Size = New System.Drawing.Size(67, 17)
        Me.rbRegulars.TabIndex = 1
        Me.rbRegulars.TabStop = True
        Me.rbRegulars.Text = "Regulars"
        Me.rbRegulars.UseVisualStyleBackColor = True
        '
        'rbAll
        '
        Me.rbAll.AutoSize = True
        Me.rbAll.Location = New System.Drawing.Point(7, 20)
        Me.rbAll.Name = "rbAll"
        Me.rbAll.Size = New System.Drawing.Size(73, 17)
        Me.rbAll.TabIndex = 0
        Me.rbAll.TabStop = True
        Me.rbAll.Text = "All Players"
        Me.rbAll.UseVisualStyleBackColor = True
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
        Me.DataGridViewTextBoxColumn2.Width = 40
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "Grade"
        Me.DataGridViewTextBoxColumn3.HeaderText = "Grade"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.Width = 40
        '
        'InSkins
        '
        Me.InSkins.DataPropertyName = "InSkins"
        Me.InSkins.HeaderText = "Skins"
        Me.InSkins.Name = "InSkins"
        Me.InSkins.Width = 40
        '
        'InCTP
        '
        Me.InCTP.DataPropertyName = "InCTP"
        Me.InCTP.HeaderText = "CTP"
        Me.InCTP.Name = "InCTP"
        Me.InCTP.Width = 40
        '
        'CommPref
        '
        Me.CommPref.DataPropertyName = "CommPref"
        Me.CommPref.HeaderText = "CommPref"
        Me.CommPref.Name = "CommPref"
        '
        'DataGridViewTextBoxColumn5
        '
        Me.DataGridViewTextBoxColumn5.DataPropertyName = "Email"
        Me.DataGridViewTextBoxColumn5.HeaderText = "Email"
        Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        Me.DataGridViewTextBoxColumn5.Width = 150
        '
        'EmailStats
        '
        Me.EmailStats.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.EmailStats.DataPropertyName = "EmailStats"
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.EmailStats.DefaultCellStyle = DataGridViewCellStyle1
        Me.EmailStats.HeaderText = "EmailStats"
        Me.EmailStats.Name = "EmailStats"
        Me.EmailStats.Width = 80
        '
        'TextStats
        '
        Me.TextStats.DataPropertyName = "TextStats"
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TextStats.DefaultCellStyle = DataGridViewCellStyle2
        Me.TextStats.HeaderText = "TextStats"
        Me.TextStats.Name = "TextStats"
        Me.TextStats.Width = 80
        '
        'DataGridViewTextBoxColumn6
        '
        Me.DataGridViewTextBoxColumn6.DataPropertyName = "Phone"
        Me.DataGridViewTextBoxColumn6.HeaderText = "Phone"
        Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
        Me.DataGridViewTextBoxColumn6.Width = 80
        '
        'DataGridViewTextBoxColumn10
        '
        Me.DataGridViewTextBoxColumn10.DataPropertyName = "HomePhone"
        Me.DataGridViewTextBoxColumn10.HeaderText = "HomePhone"
        Me.DataGridViewTextBoxColumn10.Name = "DataGridViewTextBoxColumn10"
        Me.DataGridViewTextBoxColumn10.Width = 80
        '
        'DataGridViewTextBoxColumn11
        '
        Me.DataGridViewTextBoxColumn11.DataPropertyName = "CellCarrier"
        Me.DataGridViewTextBoxColumn11.HeaderText = "CellCarrier"
        Me.DataGridViewTextBoxColumn11.Name = "DataGridViewTextBoxColumn11"
        '
        'Player
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1365, 565)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.DtPlayersDataGridView)
        Me.Name = "Player"
        Me.Text = "Player"
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtPlayersBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtPlayersDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DsLeague As dsLeague
    Friend WithEvents DtPlayersBindingSource As BindingSource
    Friend WithEvents DtPlayersDataGridView As DataGridView
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents rbRegulars As RadioButton
    Friend WithEvents rbAll As RadioButton
    Friend WithEvents DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As DataGridViewTextBoxColumn
    Friend WithEvents InSkins As DataGridViewTextBoxColumn
    Friend WithEvents InCTP As DataGridViewTextBoxColumn
    Friend WithEvents CommPref As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As DataGridViewTextBoxColumn
    Friend WithEvents EmailStats As DataGridViewTextBoxColumn
    Friend WithEvents TextStats As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn6 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn10 As DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn11 As DataGridViewTextBoxColumn
End Class
