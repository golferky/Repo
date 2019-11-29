<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTeam
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
        Me.dgPlayers = New System.Windows.Forms.DataGridView()
        Me.Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Handicap = New LeagueManager.DataGridViewMaskedEditColumn()
        Me.Team = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Grade = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Paid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Email = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Phone = New LeagueManager.DataGridViewMaskedEditColumn()
        Me.NickName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnExit = New System.Windows.Forms.Button()
        CType(Me.dgPlayers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgPlayers
        '
        Me.dgPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgPlayers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Name, Me.Handicap, Me.Team, Me.Grade, Me.Paid, Me.Email, Me.Phone, Me.NickName})
        Me.dgPlayers.Location = New System.Drawing.Point(32, 24)
        Me.dgPlayers.Margin = New System.Windows.Forms.Padding(2)
        Me.dgPlayers.Name = "dgPlayers"
        Me.dgPlayers.RowTemplate.Height = 24
        Me.dgPlayers.Size = New System.Drawing.Size(1099, 632)
        Me.dgPlayers.TabIndex = 3
        '
        'Name
        '
        Me.Name.HeaderText = "Name"
        Me.Name.Name = "Name"
        Me.Name.Width = 150
        '
        'Handicap
        '
        Me.Handicap.HeaderText = "Handicap"
        Me.Handicap.Mask = ""
        Me.Handicap.Name = "Handicap"
        Me.Handicap.PromptChar = Global.Microsoft.VisualBasic.ChrW(95)
        Me.Handicap.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Handicap.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Handicap.ValidatingType = GetType(String)
        '
        'Team
        '
        Me.Team.HeaderText = "Team"
        Me.Team.Name = "Team"
        Me.Team.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'Grade
        '
        Me.Grade.HeaderText = "Grade"
        Me.Grade.Items.AddRange(New Object() {"A", "B"})
        Me.Grade.Name = "Grade"
        Me.Grade.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Grade.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'Paid
        '
        Me.Paid.HeaderText = "Paid"
        Me.Paid.Name = "Paid"
        '
        'Email
        '
        Me.Email.HeaderText = "Email"
        Me.Email.Name = "Email"
        Me.Email.Width = 200
        '
        'Phone
        '
        Me.Phone.HeaderText = "Phone"
        Me.Phone.Mask = ""
        Me.Phone.Name = "Phone"
        Me.Phone.PromptChar = Global.Microsoft.VisualBasic.ChrW(95)
        Me.Phone.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Phone.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Phone.ValidatingType = GetType(String)
        '
        'NickName
        '
        Me.NickName.HeaderText = "Nickname"
        Me.NickName.Name = "NickName"
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(1248, 65)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(75, 23)
        Me.btnExit.TabIndex = 4
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'frmTeam
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1430, 703)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.dgPlayers)
        'Me.Name = "frmTeam"
        Me.Text = "Team"
        CType(Me.dgPlayers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents dgPlayers As DataGridView
    Friend WithEvents Name As DataGridViewTextBoxColumn
    Friend WithEvents Handicap As DataGridViewMaskedEditColumn
    Friend WithEvents Team As DataGridViewTextBoxColumn
    Friend WithEvents Grade As DataGridViewComboBoxColumn
    Friend WithEvents Paid As DataGridViewTextBoxColumn
    Friend WithEvents Email As DataGridViewTextBoxColumn
    Friend WithEvents Phone As DataGridViewMaskedEditColumn
    Friend WithEvents NickName As DataGridViewTextBoxColumn
    Friend WithEvents btnExit As Button
End Class
