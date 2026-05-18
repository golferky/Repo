<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPayDues
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        pnlMain = New Panel()
        lblPlayerName = New Label()
        lblPlayerSub = New Label()
        pnlLeagueDues = New Panel()
        chkLeagueDues = New CheckBox()
        lblLeagueDuesBalance = New Label()
        lblLeagueDuesPrompt = New Label()
        nudLeagueDues = New NumericUpDown()
        chkSkins = New CheckBox()
        lblSkinsAmt = New Label()
        chkCTP = New CheckBox()
        lblCTPAmt = New Label()
        pnlEOY = New Panel()
        chkEOY = New CheckBox()
        lblEOYBalance = New Label()
        lblEOYPrompt = New Label()
        nudEOY = New NumericUpDown()
        lblSeparator = New Label()
        lblTotal = New Label()
        btnPay = New Button()
        btnCancel = New Button()
        radEOYWk1 = New RadioButton()
        radEOYWk2 = New RadioButton()
        radEOYBoth = New RadioButton()
        pnlMain.SuspendLayout()
        pnlLeagueDues.SuspendLayout()
        CType(nudLeagueDues, ComponentModel.ISupportInitialize).BeginInit()
        pnlEOY.SuspendLayout()
        CType(nudEOY, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlMain
        ' 
        pnlMain.BackColor = Color.White
        pnlMain.Controls.Add(lblPlayerName)
        pnlMain.Controls.Add(lblPlayerSub)
        pnlMain.Controls.Add(pnlLeagueDues)
        pnlMain.Controls.Add(chkSkins)
        pnlMain.Controls.Add(lblSkinsAmt)
        pnlMain.Controls.Add(chkCTP)
        pnlMain.Controls.Add(lblCTPAmt)
        pnlMain.Controls.Add(pnlEOY)
        pnlMain.Controls.Add(lblSeparator)
        pnlMain.Controls.Add(lblTotal)
        pnlMain.Controls.Add(btnPay)
        pnlMain.Controls.Add(btnCancel)
        pnlMain.Dock = DockStyle.Fill
        pnlMain.Location = New Point(0, 0)
        pnlMain.Name = "pnlMain"
        pnlMain.Padding = New Padding(20)
        pnlMain.Size = New Size(380, 415)
        pnlMain.TabIndex = 0
        ' 
        ' lblPlayerName
        ' 
        lblPlayerName.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        lblPlayerName.ForeColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        lblPlayerName.Location = New Point(20, 20)
        lblPlayerName.Name = "lblPlayerName"
        lblPlayerName.Size = New Size(340, 28)
        lblPlayerName.TabIndex = 0
        lblPlayerName.Text = "Player Name"
        ' 
        ' lblPlayerSub
        ' 
        lblPlayerSub.AutoSize = True
        lblPlayerSub.Font = New Font("Segoe UI", 8.0F, FontStyle.Italic)
        lblPlayerSub.ForeColor = Color.Gray
        lblPlayerSub.Location = New Point(20, 50)
        lblPlayerSub.Name = "lblPlayerSub"
        lblPlayerSub.Size = New Size(0, 13)
        lblPlayerSub.TabIndex = 1
        ' 
        ' pnlLeagueDues
        ' 
        pnlLeagueDues.BackColor = Color.FromArgb(CByte(245), CByte(248), CByte(255))
        pnlLeagueDues.BorderStyle = BorderStyle.FixedSingle
        pnlLeagueDues.Controls.Add(chkLeagueDues)
        pnlLeagueDues.Controls.Add(lblLeagueDuesBalance)
        pnlLeagueDues.Controls.Add(lblLeagueDuesPrompt)
        pnlLeagueDues.Controls.Add(nudLeagueDues)
        pnlLeagueDues.Location = New Point(20, 75)
        pnlLeagueDues.Name = "pnlLeagueDues"
        pnlLeagueDues.Size = New Size(340, 70)
        pnlLeagueDues.TabIndex = 2
        ' 
        ' chkLeagueDues
        ' 
        chkLeagueDues.AutoSize = True
        chkLeagueDues.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        chkLeagueDues.Location = New Point(10, 8)
        chkLeagueDues.Name = "chkLeagueDues"
        chkLeagueDues.Size = New Size(97, 19)
        chkLeagueDues.TabIndex = 0
        chkLeagueDues.Text = "League Dues"
        ' 
        ' lblLeagueDuesBalance
        ' 
        lblLeagueDuesBalance.AutoSize = True
        lblLeagueDuesBalance.Font = New Font("Segoe UI", 9.0F)
        lblLeagueDuesBalance.ForeColor = Color.DarkRed
        lblLeagueDuesBalance.Location = New Point(200, 8)
        lblLeagueDuesBalance.Name = "lblLeagueDuesBalance"
        lblLeagueDuesBalance.Size = New Size(81, 15)
        lblLeagueDuesBalance.TabIndex = 1
        lblLeagueDuesBalance.Text = "Balance: $0.00"
        ' 
        ' lblLeagueDuesPrompt
        ' 
        lblLeagueDuesPrompt.AutoSize = True
        lblLeagueDuesPrompt.Font = New Font("Segoe UI", 8.0F)
        lblLeagueDuesPrompt.ForeColor = Color.Gray
        lblLeagueDuesPrompt.Location = New Point(10, 38)
        lblLeagueDuesPrompt.Name = "lblLeagueDuesPrompt"
        lblLeagueDuesPrompt.Size = New Size(79, 13)
        lblLeagueDuesPrompt.TabIndex = 2
        lblLeagueDuesPrompt.Text = "Pay this week:"
        ' 
        ' nudLeagueDues
        ' 
        nudLeagueDues.DecimalPlaces = 2
        nudLeagueDues.Enabled = False
        nudLeagueDues.Font = New Font("Segoe UI", 9.0F)
        nudLeagueDues.Location = New Point(110, 35)
        nudLeagueDues.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        nudLeagueDues.Name = "nudLeagueDues"
        nudLeagueDues.Size = New Size(80, 23)
        nudLeagueDues.TabIndex = 3
        ' 
        ' chkSkins
        ' 
        chkSkins.Font = New Font("Segoe UI", 9.0F)
        chkSkins.Location = New Point(20, 160)
        chkSkins.Name = "chkSkins"
        chkSkins.Size = New Size(160, 24)
        chkSkins.TabIndex = 3
        chkSkins.Text = "Skins"
        ' 
        ' lblSkinsAmt
        ' 
        lblSkinsAmt.AutoSize = True
        lblSkinsAmt.Font = New Font("Segoe UI", 9.0F)
        lblSkinsAmt.ForeColor = Color.DarkGreen
        lblSkinsAmt.Location = New Point(200, 162)
        lblSkinsAmt.Name = "lblSkinsAmt"
        lblSkinsAmt.Size = New Size(34, 15)
        lblSkinsAmt.TabIndex = 4
        lblSkinsAmt.Text = "$0.00"
        ' 
        ' chkCTP
        ' 
        chkCTP.Font = New Font("Segoe UI", 9.0F)
        chkCTP.Location = New Point(20, 179)
        chkCTP.Name = "chkCTP"
        chkCTP.Size = New Size(160, 24)
        chkCTP.TabIndex = 5
        chkCTP.Text = "CTP"
        ' 
        ' lblCTPAmt
        ' 
        lblCTPAmt.AutoSize = True
        lblCTPAmt.Font = New Font("Segoe UI", 9.0F)
        lblCTPAmt.ForeColor = Color.DarkGreen
        lblCTPAmt.Location = New Point(200, 181)
        lblCTPAmt.Name = "lblCTPAmt"
        lblCTPAmt.Size = New Size(34, 15)
        lblCTPAmt.TabIndex = 6
        lblCTPAmt.Text = "$0.00"
        ' 
        ' pnlEOY
        ' 
        pnlEOY.BackColor = Color.FromArgb(CByte(255), CByte(248), CByte(235))
        pnlEOY.BorderStyle = BorderStyle.FixedSingle
        pnlEOY.Controls.Add(radEOYBoth)
        pnlEOY.Controls.Add(radEOYWk2)
        pnlEOY.Controls.Add(radEOYWk1)
        pnlEOY.Controls.Add(chkEOY)
        pnlEOY.Controls.Add(lblEOYBalance)
        pnlEOY.Controls.Add(lblEOYPrompt)
        pnlEOY.Controls.Add(nudEOY)
        pnlEOY.Location = New Point(20, 209)
        pnlEOY.Name = "pnlEOY"
        pnlEOY.Size = New Size(340, 91)
        pnlEOY.TabIndex = 7
        ' 
        ' chkEOY
        ' 
        chkEOY.AutoSize = True
        chkEOY.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        chkEOY.Location = New Point(10, 8)
        chkEOY.Name = "chkEOY"
        chkEOY.Size = New Size(138, 19)
        chkEOY.TabIndex = 0
        chkEOY.Text = "EOY Skins (Optional)"
        ' 
        ' lblEOYBalance
        ' 
        lblEOYBalance.AutoSize = True
        lblEOYBalance.Font = New Font("Segoe UI", 9.0F)
        lblEOYBalance.ForeColor = Color.DarkOrange
        lblEOYBalance.Location = New Point(200, 8)
        lblEOYBalance.Name = "lblEOYBalance"
        lblEOYBalance.Size = New Size(81, 15)
        lblEOYBalance.TabIndex = 1
        lblEOYBalance.Text = "Balance: $0.00"
        ' 
        ' lblEOYPrompt
        ' 
        lblEOYPrompt.AutoSize = True
        lblEOYPrompt.Font = New Font("Segoe UI", 8.0F)
        lblEOYPrompt.ForeColor = Color.Gray
        lblEOYPrompt.Location = New Point(10, 38)
        lblEOYPrompt.Name = "lblEOYPrompt"
        lblEOYPrompt.Size = New Size(79, 13)
        lblEOYPrompt.TabIndex = 2
        lblEOYPrompt.Text = "Pay this week:"
        ' 
        ' nudEOY
        ' 
        nudEOY.DecimalPlaces = 2
        nudEOY.Enabled = False
        nudEOY.Font = New Font("Segoe UI", 9.0F)
        nudEOY.Location = New Point(110, 35)
        nudEOY.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        nudEOY.Name = "nudEOY"
        nudEOY.Size = New Size(80, 23)
        nudEOY.TabIndex = 3
        ' 
        ' lblSeparator
        ' 
        lblSeparator.BorderStyle = BorderStyle.Fixed3D
        lblSeparator.Location = New Point(20, 315)
        lblSeparator.Name = "lblSeparator"
        lblSeparator.Size = New Size(340, 2)
        lblSeparator.TabIndex = 8
        ' 
        ' lblTotal
        ' 
        lblTotal.AutoSize = True
        lblTotal.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblTotal.ForeColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        lblTotal.Location = New Point(20, 325)
        lblTotal.Name = "lblTotal"
        lblTotal.Size = New Size(92, 20)
        lblTotal.TabIndex = 9
        lblTotal.Text = "Total: $0.00"
        ' 
        ' btnPay
        ' 
        btnPay.BackColor = Color.FromArgb(CByte(0), CByte(120), CByte(215))
        btnPay.Cursor = Cursors.Hand
        btnPay.FlatAppearance.BorderSize = 0
        btnPay.FlatStyle = FlatStyle.Flat
        btnPay.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        btnPay.ForeColor = Color.White
        btnPay.Location = New Point(20, 360)
        btnPay.Name = "btnPay"
        btnPay.Size = New Size(160, 35)
        btnPay.TabIndex = 10
        btnPay.Text = "Pay $0.00"
        btnPay.UseVisualStyleBackColor = False
        ' 
        ' btnCancel
        ' 
        btnCancel.BackColor = Color.FromArgb(CByte(200), CByte(200), CByte(200))
        btnCancel.Cursor = Cursors.Hand
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.Font = New Font("Segoe UI", 10.0F)
        btnCancel.ForeColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        btnCancel.Location = New Point(200, 360)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(160, 35)
        btnCancel.TabIndex = 11
        btnCancel.Text = "Cancel"
        btnCancel.UseVisualStyleBackColor = False
        ' 
        ' radEOYWk1
        ' 
        radEOYWk1.AutoSize = True
        radEOYWk1.Location = New Point(10, 64)
        radEOYWk1.Name = "radEOYWk1"
        radEOYWk1.Size = New Size(51, 19)
        radEOYWk1.TabIndex = 4
        radEOYWk1.TabStop = True
        radEOYWk1.Text = "Wk 1"
        radEOYWk1.UseVisualStyleBackColor = True
        ' 
        ' radEOYWk2
        ' 
        radEOYWk2.AutoSize = True
        radEOYWk2.Location = New Point(67, 64)
        radEOYWk2.Name = "radEOYWk2"
        radEOYWk2.Size = New Size(51, 19)
        radEOYWk2.TabIndex = 5
        radEOYWk2.TabStop = True
        radEOYWk2.Text = "Wk 2"
        radEOYWk2.UseVisualStyleBackColor = True
        ' 
        ' radEOYBoth
        ' 
        radEOYBoth.AutoSize = True
        radEOYBoth.Location = New Point(124, 64)
        radEOYBoth.Name = "radEOYBoth"
        radEOYBoth.Size = New Size(50, 19)
        radEOYBoth.TabIndex = 6
        radEOYBoth.TabStop = True
        radEOYBoth.Text = "Both"
        radEOYBoth.UseVisualStyleBackColor = True
        ' 
        ' frmPayDues
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(380, 415)
        Controls.Add(pnlMain)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmPayDues"
        StartPosition = FormStartPosition.CenterParent
        Text = "Process Payment"
        pnlMain.ResumeLayout(False)
        pnlMain.PerformLayout()
        pnlLeagueDues.ResumeLayout(False)
        pnlLeagueDues.PerformLayout()
        CType(nudLeagueDues, ComponentModel.ISupportInitialize).EndInit()
        pnlEOY.ResumeLayout(False)
        pnlEOY.PerformLayout()
        CType(nudEOY, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlMain As Panel
    Friend WithEvents lblPlayerName As Label
    Friend WithEvents lblPlayerSub As Label
    Friend WithEvents pnlLeagueDues As Panel
    Friend WithEvents chkLeagueDues As CheckBox
    Friend WithEvents lblLeagueDuesBalance As Label
    Friend WithEvents lblLeagueDuesPrompt As Label
    Friend WithEvents nudLeagueDues As NumericUpDown
    Friend WithEvents chkSkins As CheckBox
    Friend WithEvents lblSkinsAmt As Label
    Friend WithEvents chkCTP As CheckBox
    Friend WithEvents lblCTPAmt As Label
    Friend WithEvents pnlEOY As Panel
    Friend WithEvents chkEOY As CheckBox
    Friend WithEvents lblEOYBalance As Label
    Friend WithEvents lblEOYPrompt As Label
    Friend WithEvents nudEOY As NumericUpDown
    Friend WithEvents lblSeparator As Label
    Friend WithEvents lblTotal As Label
    Friend WithEvents btnPay As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents radEOYWk1 As RadioButton
    Friend WithEvents radEOYBoth As RadioButton
    Friend WithEvents radEOYWk2 As RadioButton
End Class