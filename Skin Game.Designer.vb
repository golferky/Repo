<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSkins
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
        Me.cbDate = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtAnte = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbHandicap = New System.Windows.Forms.RadioButton()
        Me.rbScratch = New System.Windows.Forms.RadioButton()
        Me.btnCalc = New System.Windows.Forms.Button()
        Me.gbGameType = New System.Windows.Forms.GroupBox()
        Me.rbPreSel = New System.Windows.Forms.RadioButton()
        Me.rbSkins = New System.Windows.Forms.RadioButton()
        Me.rbpp = New System.Windows.Forms.RadioButton()
        Me.rbAB = New System.Windows.Forms.RadioButton()
        Me.rbbd = New System.Windows.Forms.RadioButton()
        Me.btnPrintResults = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.gbGameType.SuspendLayout()
        Me.SuspendLayout()
        '
        'cbDate
        '
        Me.cbDate.FormattingEnabled = True
        Me.cbDate.Location = New System.Drawing.Point(403, 33)
        Me.cbDate.Name = "cbDate"
        Me.cbDate.Size = New System.Drawing.Size(277, 21)
        Me.cbDate.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(483, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "League/Date"
        '
        'txtAnte
        '
        Me.txtAnte.Location = New System.Drawing.Point(715, 33)
        Me.txtAnte.Name = "txtAnte"
        Me.txtAnte.Size = New System.Drawing.Size(50, 19)
        Me.txtAnte.TabIndex = 6
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(712, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(33, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Ante"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbHandicap)
        Me.GroupBox1.Controls.Add(Me.rbScratch)
        Me.GroupBox1.Location = New System.Drawing.Point(874, 29)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(278, 52)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Handicap"
        '
        'rbHandicap
        '
        Me.rbHandicap.AutoSize = True
        Me.rbHandicap.Checked = True
        Me.rbHandicap.Location = New System.Drawing.Point(134, 22)
        Me.rbHandicap.Name = "rbHandicap"
        Me.rbHandicap.Size = New System.Drawing.Size(79, 17)
        Me.rbHandicap.TabIndex = 1
        Me.rbHandicap.TabStop = True
        Me.rbHandicap.Text = "Handicap"
        Me.rbHandicap.UseVisualStyleBackColor = True
        '
        'rbScratch
        '
        Me.rbScratch.AutoSize = True
        Me.rbScratch.Location = New System.Drawing.Point(7, 22)
        Me.rbScratch.Name = "rbScratch"
        Me.rbScratch.Size = New System.Drawing.Size(69, 17)
        Me.rbScratch.TabIndex = 0
        Me.rbScratch.Text = "Scratch"
        Me.rbScratch.UseVisualStyleBackColor = True
        '
        'btnCalc
        '
        Me.btnCalc.Location = New System.Drawing.Point(472, 60)
        Me.btnCalc.Name = "btnCalc"
        Me.btnCalc.Size = New System.Drawing.Size(148, 37)
        Me.btnCalc.TabIndex = 13
        Me.btnCalc.Text = "Calculate"
        Me.btnCalc.UseVisualStyleBackColor = True
        '
        'gbGameType
        '
        Me.gbGameType.Controls.Add(Me.rbPreSel)
        Me.gbGameType.Controls.Add(Me.rbSkins)
        Me.gbGameType.Controls.Add(Me.rbpp)
        Me.gbGameType.Controls.Add(Me.rbAB)
        Me.gbGameType.Controls.Add(Me.rbbd)
        Me.gbGameType.Location = New System.Drawing.Point(12, 9)
        Me.gbGameType.Name = "gbGameType"
        Me.gbGameType.Size = New System.Drawing.Size(385, 117)
        Me.gbGameType.TabIndex = 14
        Me.gbGameType.TabStop = False
        Me.gbGameType.Text = "Game Types"
        '
        'rbPreSel
        '
        Me.rbPreSel.AutoSize = True
        Me.rbPreSel.Location = New System.Drawing.Point(6, 78)
        Me.rbPreSel.Name = "rbPreSel"
        Me.rbPreSel.Size = New System.Drawing.Size(135, 17)
        Me.rbPreSel.TabIndex = 5
        Me.rbPreSel.Text = "Pre Select Partners"
        Me.rbPreSel.UseVisualStyleBackColor = True
        '
        'rbSkins
        '
        Me.rbSkins.AutoSize = True
        Me.rbSkins.Checked = True
        Me.rbSkins.Location = New System.Drawing.Point(134, 51)
        Me.rbSkins.Name = "rbSkins"
        Me.rbSkins.Size = New System.Drawing.Size(85, 17)
        Me.rbSkins.TabIndex = 4
        Me.rbSkins.TabStop = True
        Me.rbSkins.Text = "Skins Only"
        Me.rbSkins.UseVisualStyleBackColor = True
        '
        'rbpp
        '
        Me.rbpp.AutoSize = True
        Me.rbpp.Location = New System.Drawing.Point(134, 25)
        Me.rbpp.Name = "rbpp"
        Me.rbpp.Size = New System.Drawing.Size(101, 17)
        Me.rbpp.TabIndex = 3
        Me.rbpp.Text = "Pick Partners"
        Me.rbpp.UseVisualStyleBackColor = True
        '
        'rbAB
        '
        Me.rbAB.AutoSize = True
        Me.rbAB.Location = New System.Drawing.Point(6, 51)
        Me.rbAB.Name = "rbAB"
        Me.rbAB.Size = New System.Drawing.Size(80, 17)
        Me.rbAB.TabIndex = 2
        Me.rbAB.Text = "Draw A/B"
        Me.rbAB.UseVisualStyleBackColor = True
        '
        'rbbd
        '
        Me.rbbd.AutoSize = True
        Me.rbbd.Location = New System.Drawing.Point(7, 24)
        Me.rbbd.Name = "rbbd"
        Me.rbbd.Size = New System.Drawing.Size(86, 17)
        Me.rbbd.TabIndex = 1
        Me.rbbd.Text = "Blind Draw"
        Me.rbbd.UseVisualStyleBackColor = True
        '
        'btnPrintResults
        '
        Me.btnPrintResults.Location = New System.Drawing.Point(671, 60)
        Me.btnPrintResults.Name = "btnPrintResults"
        Me.btnPrintResults.Size = New System.Drawing.Size(148, 37)
        Me.btnPrintResults.TabIndex = 15
        Me.btnPrintResults.Text = "Print Results"
        Me.btnPrintResults.UseVisualStyleBackColor = True
        '
        'frmSkins
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1317, 768)
        Me.Controls.Add(Me.btnPrintResults)
        Me.Controls.Add(Me.gbGameType)
        Me.Controls.Add(Me.btnCalc)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtAnte)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbDate)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmSkins"
        Me.Text = "Skin Game"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.gbGameType.ResumeLayout(False)
        Me.gbGameType.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cbDate As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtAnte As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rbHandicap As System.Windows.Forms.RadioButton
    Friend WithEvents rbScratch As System.Windows.Forms.RadioButton
    Friend WithEvents btnCalc As System.Windows.Forms.Button
    Friend WithEvents gbGameType As System.Windows.Forms.GroupBox
    Friend WithEvents rbSkins As System.Windows.Forms.RadioButton
    Friend WithEvents rbpp As System.Windows.Forms.RadioButton
    Friend WithEvents rbAB As System.Windows.Forms.RadioButton
    Friend WithEvents rbbd As System.Windows.Forms.RadioButton
    Friend WithEvents rbPreSel As System.Windows.Forms.RadioButton
    Friend WithEvents btnPrintResults As System.Windows.Forms.Button
End Class
