<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Payments
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
        Me.dgPayments = New System.Windows.Forms.DataGridView()
        Me.cbPlayers = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbDate = New System.Windows.Forms.ComboBox()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.Player = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.PayDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Description = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Detail = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Amount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PayMethod = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.Comment = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.dgPayments, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgPayments
        '
        Me.dgPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgPayments.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Player, Me.PayDate, Me.Description, Me.Detail, Me.Amount, Me.PayMethod, Me.Comment})
        Me.dgPayments.Location = New System.Drawing.Point(52, 352)
        Me.dgPayments.Name = "dgPayments"
        Me.dgPayments.RowHeadersWidth = 30
        Me.dgPayments.RowTemplate.Height = 23
        Me.dgPayments.Size = New System.Drawing.Size(1287, 1412)
        Me.dgPayments.TabIndex = 0
        '
        'cbPlayers
        '
        Me.cbPlayers.FormattingEnabled = True
        Me.cbPlayers.Location = New System.Drawing.Point(52, 67)
        Me.cbPlayers.Name = "cbPlayers"
        Me.cbPlayers.Size = New System.Drawing.Size(300, 33)
        Me.cbPlayers.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(52, 36)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 25)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Player"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(485, 36)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 25)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Date"
        '
        'cbDate
        '
        Me.cbDate.FormattingEnabled = True
        Me.cbDate.Location = New System.Drawing.Point(485, 67)
        Me.cbDate.Name = "cbDate"
        Me.cbDate.Size = New System.Drawing.Size(150, 33)
        Me.cbDate.TabIndex = 3
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(47, 204)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(0, 25)
        Me.lbStatus.TabIndex = 5
        '
        'btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(749, 67)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(138, 42)
        Me.btnLoad.TabIndex = 6
        Me.btnLoad.Text = "Load List"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(1130, 67)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(138, 42)
        Me.btnExit.TabIndex = 7
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(935, 67)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(138, 42)
        Me.btnSave.TabIndex = 8
        Me.btnSave.Text = "Save "
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'Player
        '
        Me.Player.HeaderText = "Player"
        Me.Player.Name = "Player"
        Me.Player.Width = 130
        '
        'PayDate
        '
        Me.PayDate.HeaderText = "Date"
        Me.PayDate.Name = "PayDate"
        '
        'Description
        '
        Me.Description.HeaderText = "Description"
        Me.Description.Items.AddRange(New Object() {"League Dues", "EOY Skins", "EOY Skins Wk1", "EOY Skins Wk2", "Food", "Drinks"})
        Me.Description.Name = "Description"
        Me.Description.Width = 150
        '
        'Detail
        '
        Me.Detail.HeaderText = "Detail"
        Me.Detail.Name = "Detail"
        '
        'Amount
        '
        Me.Amount.HeaderText = "Amount"
        Me.Amount.Name = "Amount"
        '
        'PayMethod
        '
        Me.PayMethod.HeaderText = "Payment Method"
        Me.PayMethod.Items.AddRange(New Object() {"Cash", "Check", "Rollover"})
        Me.PayMethod.Name = "PayMethod"
        '
        'Comment
        '
        Me.Comment.HeaderText = "Comment"
        Me.Comment.Name = "Comment"
        Me.Comment.Width = 150
        '
        'Payments
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1388, 1887)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbDate)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbPlayers)
        Me.Controls.Add(Me.dgPayments)
        Me.Name = "Payments"
        Me.Text = "Payments"
        CType(Me.dgPayments, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgPayments As DataGridView
    Friend WithEvents cbPlayers As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents cbDate As ComboBox
    Friend WithEvents lbStatus As Label
    Friend WithEvents btnLoad As Button
    Friend WithEvents btnExit As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents Player As DataGridViewComboBoxColumn
    Friend WithEvents PayDate As DataGridViewTextBoxColumn
    Friend WithEvents Description As DataGridViewComboBoxColumn
    Friend WithEvents Detail As DataGridViewTextBoxColumn
    Friend WithEvents Amount As DataGridViewTextBoxColumn
    Friend WithEvents PayMethod As DataGridViewComboBoxColumn
    Friend WithEvents Comment As DataGridViewTextBoxColumn
End Class
