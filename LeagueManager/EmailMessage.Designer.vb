<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EmailMessage
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
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.tbMessage = New System.Windows.Forms.TextBox()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.tbSubject = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbToAddresses = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnAttach = New System.Windows.Forms.Button()
        Me.tbAttach = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(446, 52)
        Me.Label2.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(106, 25)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Message:"
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(24, 829)
        Me.btnSend.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(150, 44)
        Me.btnSend.TabIndex = 4
        Me.btnSend.Text = "Send Email "
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'tbMessage
        '
        Me.tbMessage.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMessage.Location = New System.Drawing.Point(452, 85)
        Me.tbMessage.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbMessage.Multiline = True
        Me.tbMessage.Name = "tbMessage"
        Me.tbMessage.Size = New System.Drawing.Size(1058, 696)
        Me.tbMessage.TabIndex = 5
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(223, 829)
        Me.btnExit.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(150, 44)
        Me.btnExit.TabIndex = 6
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'tbSubject
        '
        Me.tbSubject.Location = New System.Drawing.Point(24, 83)
        Me.tbSubject.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbSubject.Name = "tbSubject"
        Me.tbSubject.Size = New System.Drawing.Size(376, 31)
        Me.tbSubject.TabIndex = 7
        Me.tbSubject.Text = "League Announcement!"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(24, 52)
        Me.Label3.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 25)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Subject:"
        '
        'tbToAddresses
        '
        Me.tbToAddresses.Location = New System.Drawing.Point(24, 200)
        Me.tbToAddresses.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.tbToAddresses.Multiline = True
        Me.tbToAddresses.Name = "tbToAddresses"
        Me.tbToAddresses.Size = New System.Drawing.Size(376, 527)
        Me.tbToAddresses.TabIndex = 9
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(24, 169)
        Me.Label1.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(102, 25)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Email To:"
        '
        'btnAttach
        '
        Me.btnAttach.Location = New System.Drawing.Point(452, 918)
        Me.btnAttach.Margin = New System.Windows.Forms.Padding(6)
        Me.btnAttach.Name = "btnAttach"
        Me.btnAttach.Size = New System.Drawing.Size(150, 44)
        Me.btnAttach.TabIndex = 11
        Me.btnAttach.Text = "Attach File"
        Me.btnAttach.UseVisualStyleBackColor = True
        '
        'tbAttach
        '
        Me.tbAttach.Location = New System.Drawing.Point(451, 836)
        Me.tbAttach.Name = "tbAttach"
        Me.tbAttach.Size = New System.Drawing.Size(1059, 31)
        Me.tbAttach.TabIndex = 12
        '
        'EmailMessage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1600, 1055)
        Me.Controls.Add(Me.tbAttach)
        Me.Controls.Add(Me.btnAttach)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbToAddresses)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.tbSubject)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.tbMessage)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.Label2)
        Me.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.Name = "EmailMessage"
        Me.Text = "EmailMessage"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents btnSend As Button
    Friend WithEvents tbMessage As TextBox
    Friend WithEvents btnExit As Button
    Friend WithEvents tbSubject As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents tbToAddresses As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnAttach As Button
    Friend WithEvents tbAttach As TextBox
End Class
