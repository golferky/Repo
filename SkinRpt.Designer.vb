<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SkinRpt
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
        Me.dgSkins = New System.Windows.Forms.DataGridView()
        Me.lbStatus = New System.Windows.Forms.Label()
        CType(Me.dgSkins, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgSkins
        '
        Me.dgSkins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgSkins.Location = New System.Drawing.Point(12, 68)
        Me.dgSkins.Name = "dgSkins"
        Me.dgSkins.Size = New System.Drawing.Size(1164, 799)
        Me.dgSkins.TabIndex = 0
        '
        'lbStatus
        '
        Me.lbStatus.AutoSize = True
        Me.lbStatus.Location = New System.Drawing.Point(36, 23)
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(39, 13)
        Me.lbStatus.TabIndex = 1
        Me.lbStatus.Text = "Label1"
        '
        'SkinRpt
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1188, 905)
        Me.Controls.Add(Me.lbStatus)
        Me.Controls.Add(Me.dgSkins)
        Me.Name = "SkinRpt"
        Me.Text = "SkinRpt"
        CType(Me.dgSkins, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgSkins As DataGridView
    Friend WithEvents lbStatus As Label
End Class
