<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Scores
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
        Me.dgScores = New System.Windows.Forms.DataGridView()
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgScores
        '
        Me.dgScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgScores.Cursor = System.Windows.Forms.Cursors.Default
        Me.dgScores.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgScores.Location = New System.Drawing.Point(0, 0)
        Me.dgScores.Name = "dgScores"
        Me.dgScores.Size = New System.Drawing.Size(1370, 749)
        Me.dgScores.TabIndex = 0
        '
        'Scores
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1370, 749)
        Me.Controls.Add(Me.dgScores)
        Me.Name = "Scores"
        Me.Text = "Scores"
        CType(Me.dgScores, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgScores As DataGridView
End Class
