<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LastFive
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbDates = New System.Windows.Forms.ComboBox()
        Me.dgLast5 = New System.Windows.Forms.DataGridView()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.cb2018 = New System.Windows.Forms.CheckBox()
        CType(Me.dgLast5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(35, 32)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Score Date"
        '
        'cbDates
        '
        Me.cbDates.FormattingEnabled = True
        Me.cbDates.Location = New System.Drawing.Point(35, 50)
        Me.cbDates.Margin = New System.Windows.Forms.Padding(2)
        Me.cbDates.Name = "cbDates"
        Me.cbDates.Size = New System.Drawing.Size(81, 21)
        Me.cbDates.TabIndex = 14
        '
        'dgLast5
        '
        Me.dgLast5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgLast5.Location = New System.Drawing.Point(35, 152)
        Me.dgLast5.Margin = New System.Windows.Forms.Padding(2)
        Me.dgLast5.Name = "dgLast5"
        Me.dgLast5.RowHeadersVisible = False
        Me.dgLast5.RowTemplate.Height = 15
        Me.dgLast5.RowTemplate.ReadOnly = True
        Me.dgLast5.Size = New System.Drawing.Size(532, 843)
        Me.dgLast5.TabIndex = 17
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(32, 100)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(37, 13)
        Me.lbStatus.TabIndex = 27
        Me.lbStatus.Text = "Status"
        '
        'cb2018
        '
        Me.cb2018.AutoSize = True
        Me.cb2018.Checked = True
        Me.cb2018.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cb2018.Location = New System.Drawing.Point(149, 54)
        Me.cb2018.Margin = New System.Windows.Forms.Padding(2)
        Me.cb2018.Name = "cb2018"
        Me.cb2018.Size = New System.Drawing.Size(100, 17)
        Me.cb2018.TabIndex = 28
        Me.cb2018.Text = "Exclude < 2018"
        Me.cb2018.UseVisualStyleBackColor = True
        '
        'LastFive
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(598, 795)
        Me.Controls.Add(Me.cb2018)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.dgLast5)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbDates)
        Me.Name = "LastFive"
        Me.Text = "LastFive"
        CType(Me.dgLast5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As Label
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents dgLast5 As DataGridView
    Friend WithEvents lbStatus As Label
    Friend WithEvents cb2018 As CheckBox
End Class
