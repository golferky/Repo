﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbESCollected = New System.Windows.Forms.TextBox()
        Me.tbESPaidOut = New System.Windows.Forms.TextBox()
        Me.tbRSPaidOut = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.tbRSCollected = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.tbDue = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbExpenses = New System.Windows.Forms.TextBox()
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
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(635, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(157, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "EOY Skins Collected / Paid Out"
        '
        'tbESCollected
        '
        Me.tbESCollected.Location = New System.Drawing.Point(638, 25)
        Me.tbESCollected.Name = "tbESCollected"
        Me.tbESCollected.ReadOnly = True
        Me.tbESCollected.Size = New System.Drawing.Size(100, 20)
        Me.tbESCollected.TabIndex = 7
        '
        'tbESPaidOut
        '
        Me.tbESPaidOut.Location = New System.Drawing.Point(744, 25)
        Me.tbESPaidOut.Name = "tbESPaidOut"
        Me.tbESPaidOut.ReadOnly = True
        Me.tbESPaidOut.Size = New System.Drawing.Size(100, 20)
        Me.tbESPaidOut.TabIndex = 9
        '
        'tbRSPaidOut
        '
        Me.tbRSPaidOut.Location = New System.Drawing.Point(502, 25)
        Me.tbRSPaidOut.Name = "tbRSPaidOut"
        Me.tbRSPaidOut.ReadOnly = True
        Me.tbRSPaidOut.Size = New System.Drawing.Size(100, 20)
        Me.tbRSPaidOut.TabIndex = 13
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(393, 9)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(182, 13)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "Regular Season Collected / Paid Out"
        '
        'tbRSCollected
        '
        Me.tbRSCollected.Location = New System.Drawing.Point(396, 25)
        Me.tbRSCollected.Name = "tbRSCollected"
        Me.tbRSCollected.ReadOnly = True
        Me.tbRSCollected.Size = New System.Drawing.Size(100, 20)
        Me.tbRSCollected.TabIndex = 11
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(65, 9)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(27, 13)
        Me.Label8.TabIndex = 16
        Me.Label8.Text = "Due"
        '
        'tbDue
        '
        Me.tbDue.Location = New System.Drawing.Point(31, 25)
        Me.tbDue.Name = "tbDue"
        Me.tbDue.ReadOnly = True
        Me.tbDue.Size = New System.Drawing.Size(100, 20)
        Me.tbDue.TabIndex = 15
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(248, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 13)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "Expenses"
        '
        'tbExpenses
        '
        Me.tbExpenses.Location = New System.Drawing.Point(251, 25)
        Me.tbExpenses.Name = "tbExpenses"
        Me.tbExpenses.ReadOnly = True
        Me.tbExpenses.Size = New System.Drawing.Size(100, 20)
        Me.tbExpenses.TabIndex = 17
        '
        'Finance
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(928, 838)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbExpenses)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.tbDue)
        Me.Controls.Add(Me.tbRSPaidOut)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.tbRSCollected)
        Me.Controls.Add(Me.tbESPaidOut)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.tbESCollected)
        Me.Controls.Add(Me.dgFinance)
        Me.Name = "Finance"
        Me.Text = "Finance"
        CType(Me.dgFinance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgFinance As DataGridView
    Friend WithEvents Label4 As Label
    Friend WithEvents tbESCollected As TextBox
    Friend WithEvents tbESPaidOut As TextBox
    Friend WithEvents tbRSPaidOut As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents tbRSCollected As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents tbDue As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents tbExpenses As TextBox
End Class