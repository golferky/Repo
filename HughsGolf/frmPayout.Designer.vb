<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPayout
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
        lblPlayer = New Label()
        lblCategory = New Label()
        btnPayout = New Button()
        btnCancel = New Button()
        nudAmount = New NumericUpDown()
        lblPlayerLabel = New Label()
        lblCategoryLabel = New Label()
        lblAmountLabel = New Label()
        CType(nudAmount, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' lblPlayer
        ' 
        lblPlayer.AutoSize = True
        lblPlayer.Location = New Point(80, 15)
        lblPlayer.Name = "lblPlayer"
        lblPlayer.Size = New Size(39, 15)
        lblPlayer.TabIndex = 0
        lblPlayer.Text = "Player"
        ' 
        ' lblCategory
        ' 
        lblCategory.AutoSize = True
        lblCategory.Location = New Point(80, 45)
        lblCategory.Name = "lblCategory"
        lblCategory.Size = New Size(55, 15)
        lblCategory.TabIndex = 1
        lblCategory.Text = "Category"
        ' 
        ' btnPayout
        ' 
        btnPayout.Location = New Point(10, 130)
        btnPayout.Name = "btnPayout"
        btnPayout.Size = New Size(75, 23)
        btnPayout.TabIndex = 2
        btnPayout.Text = "Payout"
        btnPayout.UseVisualStyleBackColor = True
        ' 
        ' btnCancel
        ' 
        btnCancel.Location = New Point(180, 130)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(75, 23)
        btnCancel.TabIndex = 3
        btnCancel.Text = "Cancel"
        btnCancel.UseVisualStyleBackColor = True
        ' 
        ' nudAmount
        ' 
        nudAmount.Location = New Point(80, 77)
        nudAmount.Name = "nudAmount"
        nudAmount.Size = New Size(120, 23)
        nudAmount.TabIndex = 4
        ' 
        ' lblPlayerLabel
        ' 
        lblPlayerLabel.AutoSize = True
        lblPlayerLabel.Location = New Point(10, 15)
        lblPlayerLabel.Name = "lblPlayerLabel"
        lblPlayerLabel.Size = New Size(39, 15)
        lblPlayerLabel.TabIndex = 5
        lblPlayerLabel.Text = "Player"
        ' 
        ' lblCategoryLabel
        ' 
        lblCategoryLabel.AutoSize = True
        lblCategoryLabel.Location = New Point(10, 45)
        lblCategoryLabel.Name = "lblCategoryLabel"
        lblCategoryLabel.Size = New Size(55, 15)
        lblCategoryLabel.TabIndex = 6
        lblCategoryLabel.Text = "Category"
        ' 
        ' lblAmountLabel
        ' 
        lblAmountLabel.AutoSize = True
        lblAmountLabel.Location = New Point(10, 80)
        lblAmountLabel.Name = "lblAmountLabel"
        lblAmountLabel.Size = New Size(55, 15)
        lblAmountLabel.TabIndex = 7
        lblAmountLabel.Text = "Category"
        ' 
        ' frmPayout
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(284, 161)
        Controls.Add(lblAmountLabel)
        Controls.Add(lblCategoryLabel)
        Controls.Add(lblPlayerLabel)
        Controls.Add(nudAmount)
        Controls.Add(btnCancel)
        Controls.Add(btnPayout)
        Controls.Add(lblCategory)
        Controls.Add(lblPlayer)
        Name = "frmPayout"
        Text = "frmPayout"
        CType(nudAmount, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblPlayer As Label
    Friend WithEvents lblCategory As Label
    Friend WithEvents btnPayout As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents nudAmount As NumericUpDown
    Friend WithEvents lblPlayerLabel As Label
    Friend WithEvents lblCategoryLabel As Label
    Friend WithEvents lblAmountLabel As Label
End Class
