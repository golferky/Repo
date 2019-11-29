<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectPlayers
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
        Me.clbPlayers = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'clbPlayers
        '
        Me.clbPlayers.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.clbPlayers.FullRowSelect = True
        Me.clbPlayers.GridLines = True
        Me.clbPlayers.Location = New System.Drawing.Point(28, 38)
        Me.clbPlayers.Name = "clbPlayers"
        Me.clbPlayers.Size = New System.Drawing.Size(282, 377)
        Me.clbPlayers.TabIndex = 0
        Me.clbPlayers.UseCompatibleStateImageBehavior = False
        Me.clbPlayers.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Player"
        Me.ColumnHeader1.Width = 252
        '
        'frmSelectPlayers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(347, 529)
        Me.Controls.Add(Me.clbPlayers)
        Me.Name = "frmSelectPlayers"
        Me.Text = "SelectPlayers"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents clbPlayers As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
End Class
