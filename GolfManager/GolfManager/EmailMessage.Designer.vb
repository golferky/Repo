<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class EmailMessage
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
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.tbMessage = New System.Windows.Forms.TextBox()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.tbSubject = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnAttach = New System.Windows.Forms.Button()
        Me.tbAttach = New System.Windows.Forms.TextBox()
        Me.cbRegulars = New System.Windows.Forms.CheckBox()
        Me.DtPlayersDataGridView = New System.Windows.Forms.DataGridView()
        Me.NameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Email = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DtPlayersBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.DsLeague = New GolfManager.dsLeague()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.DtPlayersDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtPlayersBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(320, 99)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Message:"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(445, 43)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(75, 23)
        Me.btnSend.TabIndex = 4
        Me.btnSend.Text = "Send Email "
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'tbMessage
        '
        Me.tbMessage.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMessage.Location = New System.Drawing.Point(323, 116)
        Me.tbMessage.Multiline = True
        Me.tbMessage.Name = "tbMessage"
        Me.tbMessage.Size = New System.Drawing.Size(531, 454)
        Me.tbMessage.TabIndex = 5
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(558, 43)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(75, 23)
        Me.btnExit.TabIndex = 6
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'tbSubject
        '
        Me.tbSubject.Location = New System.Drawing.Point(12, 43)
        Me.tbSubject.Name = "tbSubject"
        Me.tbSubject.Size = New System.Drawing.Size(190, 20)
        Me.tbSubject.TabIndex = 7
        Me.tbSubject.Text = "League Announcement!"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 27)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Subject:"
        '
        'btnAttach
        '
        Me.btnAttach.Location = New System.Drawing.Point(323, 606)
        Me.btnAttach.Name = "btnAttach"
        Me.btnAttach.Size = New System.Drawing.Size(75, 23)
        Me.btnAttach.TabIndex = 11
        Me.btnAttach.Text = "Attach File"
        Me.btnAttach.UseVisualStyleBackColor = True
        '
        'tbAttach
        '
        Me.tbAttach.Location = New System.Drawing.Point(324, 581)
        Me.tbAttach.Margin = New System.Windows.Forms.Padding(2)
        Me.tbAttach.Name = "tbAttach"
        Me.tbAttach.Size = New System.Drawing.Size(532, 20)
        Me.tbAttach.TabIndex = 12
        '
        'cbRegulars
        '
        Me.cbRegulars.AutoSize = True
        Me.cbRegulars.Location = New System.Drawing.Point(12, 93)
        Me.cbRegulars.Name = "cbRegulars"
        Me.cbRegulars.Size = New System.Drawing.Size(92, 17)
        Me.cbRegulars.TabIndex = 13
        Me.cbRegulars.Text = "Regulars Only"
        Me.cbRegulars.UseVisualStyleBackColor = True
        '
        'DtPlayersDataGridView
        '
        Me.DtPlayersDataGridView.AllowUserToAddRows = False
        Me.DtPlayersDataGridView.AutoGenerateColumns = False
        Me.DtPlayersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DtPlayersDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NameDataGridViewTextBoxColumn, Me.Email})
        Me.DtPlayersDataGridView.DataSource = Me.DtPlayersBindingSource
        Me.DtPlayersDataGridView.Location = New System.Drawing.Point(15, 116)
        Me.DtPlayersDataGridView.Name = "DtPlayersDataGridView"
        Me.DtPlayersDataGridView.ReadOnly = True
        Me.DtPlayersDataGridView.RowHeadersVisible = False
        Me.DtPlayersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DtPlayersDataGridView.Size = New System.Drawing.Size(302, 454)
        Me.DtPlayersDataGridView.TabIndex = 14
        '
        'NameDataGridViewTextBoxColumn
        '
        Me.NameDataGridViewTextBoxColumn.DataPropertyName = "Name"
        Me.NameDataGridViewTextBoxColumn.HeaderText = "Name"
        Me.NameDataGridViewTextBoxColumn.Name = "NameDataGridViewTextBoxColumn"
        Me.NameDataGridViewTextBoxColumn.ReadOnly = True
        '
        'Email
        '
        Me.Email.DataPropertyName = "Email"
        Me.Email.HeaderText = "Email"
        Me.Email.Name = "Email"
        Me.Email.ReadOnly = True
        Me.Email.Width = 150
        '
        'DtPlayersBindingSource
        '
        Me.DtPlayersBindingSource.DataMember = "dtPlayers"
        Me.DtPlayersBindingSource.DataSource = Me.DsLeague
        '
        'DsLeague
        '
        Me.DsLeague.DataSetName = "dsLeague"
        Me.DsLeague.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(222, 43)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
        Me.btnRefresh.TabIndex = 15
        Me.btnRefresh.Text = "Refresh List"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(137, 93)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(125, 13)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "Double Click to Remove "
        '
        'EmailMessage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 699)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.DtPlayersDataGridView)
        Me.Controls.Add(Me.cbRegulars)
        Me.Controls.Add(Me.tbAttach)
        Me.Controls.Add(Me.btnAttach)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.tbSubject)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.tbMessage)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.Label2)
        Me.Name = "EmailMessage"
        CType(Me.DtPlayersDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtPlayersBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DsLeague, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents btnSend As Button
    Friend WithEvents tbMessage As TextBox
    Friend WithEvents btnExit As Button
    Friend WithEvents tbSubject As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnAttach As Button
    Friend WithEvents tbAttach As TextBox
    Friend WithEvents cbRegulars As CheckBox
    Friend WithEvents DtPlayersDataGridView As DataGridView
    Friend WithEvents DtPlayersBindingSource As BindingSource
    Friend WithEvents DsLeague As dsLeague
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents NameDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents Email As DataGridViewTextBoxColumn
End Class