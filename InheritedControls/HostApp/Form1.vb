Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Form1
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents FunLabel1 As FunLabel.FunLabel
    Friend WithEvents FunLabel2 As FunLabel.FunLabel
    Friend WithEvents FunLabel3 As FunLabel.FunLabel
    Friend WithEvents FunLabel4 As FunLabel.FunLabel
    Friend WithEvents FunLabel5 As FunLabel.FunLabel
    Friend WithEvents FunLabel6 As FunLabel.FunLabel
    Friend WithEvents FunLabel7 As FunLabel.FunLabel
    Friend WithEvents FunLabel8 As FunLabel.FunLabel
    Friend WithEvents Cylons As FunLabel.FunLabel
    Friend WithEvents FunLabel9 As FunLabel.FunLabel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.FunLabel1 = New FunLabel.FunLabel
        Me.FunLabel2 = New FunLabel.FunLabel
        Me.FunLabel3 = New FunLabel.FunLabel
        Me.FunLabel4 = New FunLabel.FunLabel
        Me.FunLabel5 = New FunLabel.FunLabel
        Me.FunLabel6 = New FunLabel.FunLabel
        Me.FunLabel7 = New FunLabel.FunLabel
        Me.FunLabel8 = New FunLabel.FunLabel
        Me.Cylons = New FunLabel.FunLabel
        Me.FunLabel9 = New FunLabel.FunLabel
        Me.SuspendLayout()
        '
        'FunLabel1
        '
        Me.FunLabel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel1.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel1.BackColor1 = System.Drawing.Color.Navy
        Me.FunLabel1.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel1.GradientFallOff = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.FunLabel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        Me.FunLabel1.GradientSpeed = New Decimal(New Integer() {0, 0, 0, 0})
        Me.FunLabel1.Location = New System.Drawing.Point(0, 8)
        Me.FunLabel1.Name = "FunLabel1"
        Me.FunLabel1.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel1.TabIndex = 3
        '
        'FunLabel2
        '
        Me.FunLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel2.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel2.BackColor1 = System.Drawing.Color.Green
        Me.FunLabel2.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel2.GradientFallOff = New Decimal(New Integer() {3, 0, 0, 65536})
        Me.FunLabel2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.FunLabel2.GradientSpeed = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.FunLabel2.Location = New System.Drawing.Point(0, 48)
        Me.FunLabel2.Name = "FunLabel2"
        Me.FunLabel2.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel2.TabIndex = 4
        '
        'FunLabel3
        '
        Me.FunLabel3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel3.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel3.BackColor1 = System.Drawing.Color.Teal
        Me.FunLabel3.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel3.GradientFallOff = New Decimal(New Integer() {3, 0, 0, 65536})
        Me.FunLabel3.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.FunLabel3.GradientSpeed = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.FunLabel3.Location = New System.Drawing.Point(0, 88)
        Me.FunLabel3.Name = "FunLabel3"
        Me.FunLabel3.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel3.TabIndex = 5
        '
        'FunLabel4
        '
        Me.FunLabel4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel4.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel4.BackColor1 = System.Drawing.Color.Purple
        Me.FunLabel4.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel4.GradientFallOff = New Decimal(New Integer() {3, 0, 0, 65536})
        Me.FunLabel4.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal
        Me.FunLabel4.GradientSpeed = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.FunLabel4.Location = New System.Drawing.Point(0, 128)
        Me.FunLabel4.Name = "FunLabel4"
        Me.FunLabel4.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel4.TabIndex = 6
        '
        'FunLabel5
        '
        Me.FunLabel5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel5.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel5.BackColor1 = System.Drawing.Color.FromArgb(CType(64, Byte), CType(0, Byte), CType(64, Byte))
        Me.FunLabel5.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel5.GradientFallOff = New Decimal(New Integer() {8, 0, 0, 65536})
        Me.FunLabel5.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        Me.FunLabel5.GradientSpeed = New Decimal(New Integer() {9, 0, 0, 65536})
        Me.FunLabel5.Location = New System.Drawing.Point(0, 168)
        Me.FunLabel5.Name = "FunLabel5"
        Me.FunLabel5.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel5.TabIndex = 7
        '
        'FunLabel6
        '
        Me.FunLabel6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel6.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel6.BackColor1 = System.Drawing.Color.Maroon
        Me.FunLabel6.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel6.GradientFallOff = New Decimal(New Integer() {9, 0, 0, 65536})
        Me.FunLabel6.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        Me.FunLabel6.GradientSpeed = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.FunLabel6.Location = New System.Drawing.Point(0, 208)
        Me.FunLabel6.Name = "FunLabel6"
        Me.FunLabel6.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel6.TabIndex = 8
        '
        'FunLabel7
        '
        Me.FunLabel7.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel7.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel7.BackColor1 = System.Drawing.Color.FromArgb(CType(64, Byte), CType(0, Byte), CType(0, Byte))
        Me.FunLabel7.Backcolor2 = System.Drawing.Color.LavenderBlush
        Me.FunLabel7.GradientFallOff = New Decimal(New Integer() {3, 0, 0, 65536})
        Me.FunLabel7.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        Me.FunLabel7.GradientSpeed = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.FunLabel7.Location = New System.Drawing.Point(0, 248)
        Me.FunLabel7.Name = "FunLabel7"
        Me.FunLabel7.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel7.TabIndex = 9
        '
        'FunLabel8
        '
        Me.FunLabel8.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel8.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel8.BackColor1 = System.Drawing.Color.Lime
        Me.FunLabel8.Backcolor2 = System.Drawing.Color.Blue
        Me.FunLabel8.GradientFallOff = New Decimal(New Integer() {9, 0, 0, 65536})
        Me.FunLabel8.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.FunLabel8.GradientSpeed = New Decimal(New Integer() {6, 0, 0, 65536})
        Me.FunLabel8.Location = New System.Drawing.Point(0, 288)
        Me.FunLabel8.Name = "FunLabel8"
        Me.FunLabel8.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel8.TabIndex = 10
        '
        'Cylons
        '
        Me.Cylons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cylons.BackColor = System.Drawing.SystemColors.ControlText
        Me.Cylons.BackColor1 = System.Drawing.Color.DarkGray
        Me.Cylons.Backcolor2 = System.Drawing.Color.FromArgb(CType(192, Byte), CType(0, Byte), CType(0, Byte))
        Me.Cylons.GradientFallOff = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.Cylons.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Cylons.GradientSpeed = New Decimal(New Integer() {8, 0, 0, 65536})
        Me.Cylons.Location = New System.Drawing.Point(0, 328)
        Me.Cylons.Name = "Cylons"
        Me.Cylons.Size = New System.Drawing.Size(336, 32)
        Me.Cylons.TabIndex = 11
        '
        'FunLabel9
        '
        Me.FunLabel9.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FunLabel9.BackColor = System.Drawing.SystemColors.ControlText
        Me.FunLabel9.BackColor1 = System.Drawing.Color.FromArgb(CType(255, Byte), CType(192, Byte), CType(128, Byte))
        Me.FunLabel9.Backcolor2 = System.Drawing.Color.FromArgb(CType(255, Byte), CType(128, Byte), CType(0, Byte))
        Me.FunLabel9.GradientFallOff = New Decimal(New Integer() {9, 0, 0, 65536})
        Me.FunLabel9.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical
        Me.FunLabel9.GradientSpeed = New Decimal(New Integer() {9, 0, 0, 65536})
        Me.FunLabel9.Location = New System.Drawing.Point(0, 368)
        Me.FunLabel9.Name = "FunLabel9"
        Me.FunLabel9.Size = New System.Drawing.Size(336, 32)
        Me.FunLabel9.TabIndex = 12
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(336, 406)
        Me.Controls.Add(Me.FunLabel9)
        Me.Controls.Add(Me.Cylons)
        Me.Controls.Add(Me.FunLabel8)
        Me.Controls.Add(Me.FunLabel7)
        Me.Controls.Add(Me.FunLabel6)
        Me.Controls.Add(Me.FunLabel5)
        Me.Controls.Add(Me.FunLabel4)
        Me.Controls.Add(Me.FunLabel3)
        Me.Controls.Add(Me.FunLabel2)
        Me.Controls.Add(Me.FunLabel1)
        Me.Name = "Form1"
        Me.Text = "FunLabel Host Application"
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
