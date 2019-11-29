<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmScoreCard
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
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.gbYardage = New System.Windows.Forms.GroupBox()
        Me.gbScoring = New System.Windows.Forms.GroupBox()
        Me.dgScores = New System.Windows.Forms.DataGridView()
        Me.dtDate = New System.Windows.Forms.DateTimePicker()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.cbGroup = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtLeagueName = New System.Windows.Forms.TextBox()
        'Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn12 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn13 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn14 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn15 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn16 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn17 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn18 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn19 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn20 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn21 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        'Me.DataGridViewTextBoxColumn22 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.gbScoring.SuspendLayout()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(413, 34)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(30, 13)
        Me.Label2.TabIndex = 39
        Me.Label2.Text = "Date"
        '
        'gbYardage
        '
        Me.gbYardage.Location = New System.Drawing.Point(9, 84)
        Me.gbYardage.Margin = New System.Windows.Forms.Padding(2)
        Me.gbYardage.Name = "gbYardage"
        Me.gbYardage.Padding = New System.Windows.Forms.Padding(2)
        Me.gbYardage.Size = New System.Drawing.Size(1038, 81)
        Me.gbYardage.TabIndex = 44
        Me.gbYardage.TabStop = False
        Me.gbYardage.Text = "Yardage"
        '
        'gbScoring
        '
        Me.gbScoring.Controls.Add(Me.dgScores)
        Me.gbScoring.Location = New System.Drawing.Point(9, 238)
        Me.gbScoring.Margin = New System.Windows.Forms.Padding(2)
        Me.gbScoring.Name = "gbScoring"
        Me.gbScoring.Padding = New System.Windows.Forms.Padding(2)
        Me.gbScoring.Size = New System.Drawing.Size(1038, 378)
        Me.gbScoring.TabIndex = 46
        Me.gbScoring.TabStop = False
        Me.gbScoring.Text = "Scores"
        '
        'dgScores
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgScores.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.dgScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgScores.DefaultCellStyle = DataGridViewCellStyle5
        Me.dgScores.Location = New System.Drawing.Point(4, 74)
        Me.dgScores.Margin = New System.Windows.Forms.Padding(2)
        Me.dgScores.Name = "dgScores"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgScores.RowHeadersDefaultCellStyle = DataGridViewCellStyle6
        Me.dgScores.RowTemplate.Height = 24
        Me.dgScores.Size = New System.Drawing.Size(793, 141)
        Me.dgScores.TabIndex = 0
        '
        'dtDate
        '
        Me.dtDate.Location = New System.Drawing.Point(407, 51)
        Me.dtDate.Margin = New System.Windows.Forms.Padding(2)
        Me.dtDate.Name = "dtDate"
        Me.dtDate.Size = New System.Drawing.Size(184, 20)
        Me.dtDate.TabIndex = 54
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(645, 42)
        Me.btnUpdate.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(106, 37)
        Me.btnUpdate.TabIndex = 56
        Me.btnUpdate.Text = "Update Scores"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'cbGroup
        '
        Me.cbGroup.FormattingEnabled = True
        Me.cbGroup.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cbGroup.Location = New System.Drawing.Point(318, 51)
        Me.cbGroup.Margin = New System.Windows.Forms.Padding(2)
        Me.cbGroup.Name = "cbGroup"
        Me.cbGroup.Size = New System.Drawing.Size(49, 21)
        Me.cbGroup.TabIndex = 52
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(315, 31)
        Me.Label5.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(36, 13)
        Me.Label5.TabIndex = 51
        Me.Label5.Text = "Group"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(88, 31)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(74, 13)
        Me.Label6.TabIndex = 53
        Me.Label6.Text = "League Name"
        '
        'txtLeagueName
        '
        Me.txtLeagueName.Location = New System.Drawing.Point(91, 52)
        Me.txtLeagueName.Name = "txtLeagueName"
        Me.txtLeagueName.Size = New System.Drawing.Size(205, 20)
        Me.txtLeagueName.TabIndex = 49
        ''
        ''DataGridViewTextBoxColumn1
        ''
        'Me.DataGridViewTextBoxColumn1.HeaderText = "Player"
        'Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        'Me.DataGridViewTextBoxColumn1.Width = 150
        ''
        ''DataGridViewTextBoxColumn2
        ''
        'Me.DataGridViewTextBoxColumn2.HeaderText = "1"
        'Me.DataGridViewTextBoxColumn2.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        'Me.DataGridViewTextBoxColumn2.Width = 40
        ''
        ''DataGridViewTextBoxColumn3
        ''
        'Me.DataGridViewTextBoxColumn3.HeaderText = "2"
        'Me.DataGridViewTextBoxColumn3.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        'Me.DataGridViewTextBoxColumn3.Width = 40
        ''
        ''DataGridViewTextBoxColumn4
        ''
        'Me.DataGridViewTextBoxColumn4.HeaderText = "3"
        'Me.DataGridViewTextBoxColumn4.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        'Me.DataGridViewTextBoxColumn4.Width = 40
        ''
        ''DataGridViewTextBoxColumn5
        ''
        'Me.DataGridViewTextBoxColumn5.HeaderText = "4"
        'Me.DataGridViewTextBoxColumn5.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
        'Me.DataGridViewTextBoxColumn5.Width = 40
        ''
        ''DataGridViewTextBoxColumn6
        ''
        'Me.DataGridViewTextBoxColumn6.HeaderText = "5"
        'Me.DataGridViewTextBoxColumn6.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
        'Me.DataGridViewTextBoxColumn6.Width = 40
        ''
        ''DataGridViewTextBoxColumn7
        ''
        'Me.DataGridViewTextBoxColumn7.HeaderText = "6"
        'Me.DataGridViewTextBoxColumn7.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
        'Me.DataGridViewTextBoxColumn7.Width = 40
        ''
        ''DataGridViewTextBoxColumn8
        ''
        'Me.DataGridViewTextBoxColumn8.HeaderText = "7"
        'Me.DataGridViewTextBoxColumn8.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8"
        'Me.DataGridViewTextBoxColumn8.Width = 40
        ''
        ''DataGridViewTextBoxColumn9
        ''
        'Me.DataGridViewTextBoxColumn9.HeaderText = "8"
        'Me.DataGridViewTextBoxColumn9.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9"
        'Me.DataGridViewTextBoxColumn9.Width = 40
        ''
        ''DataGridViewTextBoxColumn10
        ''
        'Me.DataGridViewTextBoxColumn10.HeaderText = "9"
        'Me.DataGridViewTextBoxColumn10.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn10.Name = "DataGridViewTextBoxColumn10"
        'Me.DataGridViewTextBoxColumn10.Width = 40
        ''
        ''DataGridViewTextBoxColumn11
        ''
        'Me.DataGridViewTextBoxColumn11.HeaderText = "Out"
        'Me.DataGridViewTextBoxColumn11.Name = "DataGridViewTextBoxColumn11"
        'Me.DataGridViewTextBoxColumn11.ReadOnly = True
        'Me.DataGridViewTextBoxColumn11.Width = 40
        ''
        ''DataGridViewTextBoxColumn12
        ''
        'Me.DataGridViewTextBoxColumn12.HeaderText = "10"
        'Me.DataGridViewTextBoxColumn12.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn12.Name = "DataGridViewTextBoxColumn12"
        'Me.DataGridViewTextBoxColumn12.Width = 40
        ''
        ''DataGridViewTextBoxColumn13
        ''
        'Me.DataGridViewTextBoxColumn13.HeaderText = "11"
        'Me.DataGridViewTextBoxColumn13.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn13.Name = "DataGridViewTextBoxColumn13"
        'Me.DataGridViewTextBoxColumn13.Width = 40
        ''
        ''DataGridViewTextBoxColumn14
        ''
        'Me.DataGridViewTextBoxColumn14.HeaderText = "12"
        'Me.DataGridViewTextBoxColumn14.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn14.Name = "DataGridViewTextBoxColumn14"
        'Me.DataGridViewTextBoxColumn14.Width = 40
        ''
        ''DataGridViewTextBoxColumn15
        ''
        'Me.DataGridViewTextBoxColumn15.HeaderText = "13"
        'Me.DataGridViewTextBoxColumn15.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn15.Name = "DataGridViewTextBoxColumn15"
        'Me.DataGridViewTextBoxColumn15.Width = 40
        ''
        ''DataGridViewTextBoxColumn16
        ''
        'Me.DataGridViewTextBoxColumn16.HeaderText = "14"
        'Me.DataGridViewTextBoxColumn16.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn16.Name = "DataGridViewTextBoxColumn16"
        'Me.DataGridViewTextBoxColumn16.Width = 40
        ''
        ''DataGridViewTextBoxColumn17
        ''
        'Me.DataGridViewTextBoxColumn17.HeaderText = "15"
        'Me.DataGridViewTextBoxColumn17.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn17.Name = "DataGridViewTextBoxColumn17"
        'Me.DataGridViewTextBoxColumn17.Width = 40
        ''
        ''DataGridViewTextBoxColumn18
        ''
        'Me.DataGridViewTextBoxColumn18.HeaderText = "16"
        'Me.DataGridViewTextBoxColumn18.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn18.Name = "DataGridViewTextBoxColumn18"
        'Me.DataGridViewTextBoxColumn18.Width = 40
        ''
        ''DataGridViewTextBoxColumn19
        ''
        'Me.DataGridViewTextBoxColumn19.HeaderText = "17"
        'Me.DataGridViewTextBoxColumn19.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn19.Name = "DataGridViewTextBoxColumn19"
        'Me.DataGridViewTextBoxColumn19.Width = 40
        ''
        ''DataGridViewTextBoxColumn20
        ''
        'Me.DataGridViewTextBoxColumn20.HeaderText = "18"
        'Me.DataGridViewTextBoxColumn20.MaxInputLength = 2
        'Me.DataGridViewTextBoxColumn20.Name = "DataGridViewTextBoxColumn20"
        'Me.DataGridViewTextBoxColumn20.Width = 40
        ''
        ''DataGridViewTextBoxColumn21
        ''
        'Me.DataGridViewTextBoxColumn21.HeaderText = "In"
        'Me.DataGridViewTextBoxColumn21.Name = "DataGridViewTextBoxColumn21"
        'Me.DataGridViewTextBoxColumn21.ReadOnly = True
        'Me.DataGridViewTextBoxColumn21.Width = 40
        ''
        ''DataGridViewTextBoxColumn22
        ''
        'Me.DataGridViewTextBoxColumn22.HeaderText = "Total"
        'Me.DataGridViewTextBoxColumn22.Name = "DataGridViewTextBoxColumn22"
        'Me.DataGridViewTextBoxColumn22.ReadOnly = True
        'Me.DataGridViewTextBoxColumn22.Width = 60
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(811, 42)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(2)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(106, 37)
        Me.btnExit.TabIndex = 57
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'frmScoreCard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1154, 689)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.txtLeagueName)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.cbGroup)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.dtDate)
        Me.Controls.Add(Me.gbScoring)
        Me.Controls.Add(Me.gbYardage)
        Me.Controls.Add(Me.Label2)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmScoreCard"
        Me.Text = "ScoreCard"
        Me.gbScoring.ResumeLayout(False)
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents gbYardage As System.Windows.Forms.GroupBox
    Friend WithEvents gbScoring As System.Windows.Forms.GroupBox
    Friend WithEvents dtDate As System.Windows.Forms.DateTimePicker
    Friend WithEvents dgScores As System.Windows.Forms.DataGridView
    'Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn9 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn10 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn11 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn12 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn13 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn14 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn15 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn16 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn17 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn18 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn19 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn20 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn21 As System.Windows.Forms.DataGridViewTextBoxColumn
    'Friend WithEvents DataGridViewTextBoxColumn22 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents cbGroup As System.Windows.Forms.ComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As Label
    Friend WithEvents txtLeagueName As TextBox
    Friend WithEvents btnExit As Button
End Class
