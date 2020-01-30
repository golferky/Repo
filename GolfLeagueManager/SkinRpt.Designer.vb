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
        Me.lvAbbr = New System.Windows.Forms.ListView()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.dgSkins, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgSkins
        '
        Me.dgSkins.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgSkins.Location = New System.Drawing.Point(28, 68)
        Me.dgSkins.Name = "dgSkins"
        Me.dgSkins.RowHeadersVisible = False
        Me.dgSkins.Size = New System.Drawing.Size(890, 618)
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
        'lvAbbr
        '
        Me.lvAbbr.HideSelection = False
        Me.lvAbbr.Location = New System.Drawing.Point(924, 68)
        Me.lvAbbr.Name = "lvAbbr"
        Me.lvAbbr.Size = New System.Drawing.Size(305, 377)
        Me.lvAbbr.TabIndex = 2
        Me.lvAbbr.UseCompatibleStateImageBehavior = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1002, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Abberviations"
        '
        'SkinRpt
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1241, 833)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lvAbbr)
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
    Friend WithEvents lvAbbr As ListView
    Friend WithEvents Label1 As Label
End Class
