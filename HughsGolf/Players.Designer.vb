<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Players
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
        dgPlayers = New DataGridView()
        tbPlayer = New TextBox()
        Button2 = New Button()
        CType(dgPlayers, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dgPlayers
        ' 
        dgPlayers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgPlayers.Location = New Point(74, 37)
        dgPlayers.Name = "dgPlayers"
        dgPlayers.Size = New Size(742, 452)
        dgPlayers.TabIndex = 1
        ' 
        ' tbPlayer
        ' 
        tbPlayer.Location = New Point(144, 8)
        tbPlayer.Name = "tbPlayer"
        tbPlayer.Size = New Size(131, 23)
        tbPlayer.TabIndex = 3
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(299, 7)
        Button2.Name = "Button2"
        Button2.Size = New Size(81, 23)
        Button2.TabIndex = 20
        Button2.Text = "Quick Find"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Players
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(921, 528)
        Controls.Add(Button2)
        Controls.Add(tbPlayer)
        Controls.Add(dgPlayers)
        Name = "Players"
        Text = "Players"
        CType(dgPlayers, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents dgPlayers As DataGridView
    Friend WithEvents tbPlayer As TextBox
    Friend WithEvents Button2 As Button
End Class
