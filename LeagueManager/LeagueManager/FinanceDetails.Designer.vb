<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FinanceDetails
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
        CType(Me.dgFinance, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgFinance
        '
        Me.dgFinance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgFinance.Location = New System.Drawing.Point(63, 60)
        Me.dgFinance.Name = "dgFinance"
        Me.dgFinance.Size = New System.Drawing.Size(957, 708)
        Me.dgFinance.TabIndex = 0
        '
        'FinanceDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1086, 1032)
        Me.Controls.Add(Me.dgFinance)
        Me.Name = "FinanceDetails"
        Me.Text = "FinanceDetails"
        CType(Me.dgFinance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents dgFinance As DataGridView
End Class
