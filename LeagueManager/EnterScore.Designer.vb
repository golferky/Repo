<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEnterScore
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
        Me.btnUpdatesScoring = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.gbScoring = New System.Windows.Forms.GroupBox()
        Me.SuspendLayout()
        '
        'btnUpdatesScoring
        '
        Me.btnUpdatesScoring.Location = New System.Drawing.Point(269, 463)
        Me.btnUpdatesScoring.Margin = New System.Windows.Forms.Padding(2)
        Me.btnUpdatesScoring.Name = "btnUpdatesScoring"
        Me.btnUpdatesScoring.Size = New System.Drawing.Size(80, 28)
        Me.btnUpdatesScoring.TabIndex = 0
        Me.btnUpdatesScoring.Text = "Update Score"
        Me.btnUpdatesScoring.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(41, 463)
        Me.btnClear.Margin = New System.Windows.Forms.Padding(2)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(80, 28)
        Me.btnClear.TabIndex = 1
        Me.btnClear.Text = "Clear Scores"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'gbScoring
        '
        Me.gbScoring.AutoSize = True
        Me.gbScoring.Location = New System.Drawing.Point(41, 37)
        Me.gbScoring.Margin = New System.Windows.Forms.Padding(2)
        Me.gbScoring.Name = "gbScoring"
        Me.gbScoring.Padding = New System.Windows.Forms.Padding(2)
        Me.gbScoring.Size = New System.Drawing.Size(693, 389)
        Me.gbScoring.TabIndex = 2
        Me.gbScoring.TabStop = False
        '
        'frmEnterScore
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(870, 546)
        Me.Controls.Add(Me.gbScoring)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnUpdatesScoring)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmEnterScore"
        Me.Text = "EnterScore"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnUpdatesScoring As System.Windows.Forms.Button
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents gbScoring As System.Windows.Forms.GroupBox
End Class
