<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Finance
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
        Me.dgFinance = New System.Windows.Forms.DataGridView()
        Me.tbESCollected = New System.Windows.Forms.TextBox()
        Me.tbESPaidOut = New System.Windows.Forms.TextBox()
        Me.tbRSPaidOut = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.tbRSCollected = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.tbDue = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbExpenses = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lbStatus = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbTotalPO = New System.Windows.Forms.TextBox()
        CType(Me.dgFinance, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgFinance
        '
        Me.dgFinance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgFinance.Location = New System.Drawing.Point(31, 51)
        Me.dgFinance.Name = "dgFinance"
        Me.dgFinance.Size = New System.Drawing.Size(866, 759)
        Me.dgFinance.TabIndex = 0
        '
        'tbESCollected
        '
        Me.tbESCollected.Location = New System.Drawing.Point(995, 233)
        Me.tbESCollected.Name = "tbESCollected"
        Me.tbESCollected.ReadOnly = True
        Me.tbESCollected.Size = New System.Drawing.Size(64, 20)
        Me.tbESCollected.TabIndex = 7
        '
        'tbESPaidOut
        '
        Me.tbESPaidOut.Location = New System.Drawing.Point(995, 259)
        Me.tbESPaidOut.Name = "tbESPaidOut"
        Me.tbESPaidOut.ReadOnly = True
        Me.tbESPaidOut.Size = New System.Drawing.Size(64, 20)
        Me.tbESPaidOut.TabIndex = 9
        '
        'tbRSPaidOut
        '
        Me.tbRSPaidOut.Location = New System.Drawing.Point(995, 154)
        Me.tbRSPaidOut.Name = "tbRSPaidOut"
        Me.tbRSPaidOut.ReadOnly = True
        Me.tbRSPaidOut.Size = New System.Drawing.Size(64, 20)
        Me.tbRSPaidOut.TabIndex = 13
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(930, 99)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(51, 13)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "Collected"
        '
        'tbRSCollected
        '
        Me.tbRSCollected.Location = New System.Drawing.Point(995, 96)
        Me.tbRSCollected.Name = "tbRSCollected"
        Me.tbRSCollected.ReadOnly = True
        Me.tbRSCollected.Size = New System.Drawing.Size(64, 20)
        Me.tbRSCollected.TabIndex = 11
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(936, 73)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(27, 13)
        Me.Label8.TabIndex = 16
        Me.Label8.Text = "Due"
        '
        'tbDue
        '
        Me.tbDue.Location = New System.Drawing.Point(995, 70)
        Me.tbDue.Name = "tbDue"
        Me.tbDue.ReadOnly = True
        Me.tbDue.Size = New System.Drawing.Size(64, 20)
        Me.tbDue.TabIndex = 15
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(930, 127)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 13)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "Expenses"
        '
        'tbExpenses
        '
        Me.tbExpenses.Location = New System.Drawing.Point(995, 124)
        Me.tbExpenses.Name = "tbExpenses"
        Me.tbExpenses.ReadOnly = True
        Me.tbExpenses.Size = New System.Drawing.Size(64, 20)
        Me.tbExpenses.TabIndex = 17
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(989, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 13)
        Me.Label2.TabIndex = 19
        Me.Label2.Text = "Regular Season"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(930, 157)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(53, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "Prize P/O"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(930, 262)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(48, 13)
        Me.Label5.TabIndex = 25
        Me.Label5.Text = "Paid Out"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(992, 217)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(67, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Post Season"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(930, 236)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(51, 13)
        Me.Label9.TabIndex = 22
        Me.Label9.Text = "Collected"
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(31, 13)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(37, 13)
        Me.lbStatus.TabIndex = 26
        Me.lbStatus.Text = "Status"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(930, 187)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(54, 13)
        Me.Label4.TabIndex = 28
        Me.Label4.Text = "Total P/O"
        '
        'tbTotalPO
        '
        Me.tbTotalPO.Location = New System.Drawing.Point(995, 184)
        Me.tbTotalPO.Name = "tbTotalPO"
        Me.tbTotalPO.ReadOnly = True
        Me.tbTotalPO.Size = New System.Drawing.Size(64, 20)
        Me.tbTotalPO.TabIndex = 27
        '
        'Finance
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1132, 749)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.tbTotalPO)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbExpenses)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.tbDue)
        Me.Controls.Add(Me.tbRSPaidOut)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.tbRSCollected)
        Me.Controls.Add(Me.tbESPaidOut)
        Me.Controls.Add(Me.tbESCollected)
        Me.Controls.Add(Me.dgFinance)
        Me.Name = "Finance"
        Me.Text = "Finance"
        CType(Me.dgFinance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgFinance As DataGridView
    Friend WithEvents tbESCollected As TextBox
    Friend WithEvents tbESPaidOut As TextBox
    Friend WithEvents tbRSPaidOut As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents tbRSCollected As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents tbDue As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents tbExpenses As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents lbStatus As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents tbTotalPO As TextBox
End Class
