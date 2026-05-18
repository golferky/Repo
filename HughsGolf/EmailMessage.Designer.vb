<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class EmailMessage
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
        components = New ComponentModel.Container()
        Label2 = New Label()
        btnSend = New Button()
        tbMessage = New TextBox()
        btnExit = New Button()
        tbSubject = New TextBox()
        Label3 = New Label()
        btnAttach = New Button()
        tbAttach = New TextBox()
        cbRegulars = New CheckBox()
        dgvEmail = New DataGridView()
        btnRefresh = New Button()
        Label1 = New Label()
        cbText = New CheckBox()
        CType(dgvEmail, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(373, 114)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(56, 15)
        Label2.TabIndex = 3
        Label2.Text = "Message:"
        ' 
        ' btnSend
        ' 
        btnSend.Location = New Point(519, 50)
        btnSend.Margin = New Padding(4, 3, 4, 3)
        btnSend.Name = "btnSend"
        btnSend.Size = New Size(88, 27)
        btnSend.TabIndex = 4
        btnSend.Text = "Send Email "
        btnSend.UseVisualStyleBackColor = True
        ' 
        ' tbMessage
        ' 
        tbMessage.Font = New Font("Courier New", 9.0F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        tbMessage.Location = New Point(519, 134)
        tbMessage.Margin = New Padding(4, 3, 4, 3)
        tbMessage.Multiline = True
        tbMessage.Name = "tbMessage"
        tbMessage.Size = New Size(619, 523)
        tbMessage.TabIndex = 5
        ' 
        ' btnExit
        ' 
        btnExit.Location = New Point(651, 50)
        btnExit.Margin = New Padding(4, 3, 4, 3)
        btnExit.Name = "btnExit"
        btnExit.Size = New Size(88, 27)
        btnExit.TabIndex = 6
        btnExit.Text = "Exit"
        btnExit.UseVisualStyleBackColor = True
        ' 
        ' tbSubject
        ' 
        tbSubject.Location = New Point(14, 50)
        tbSubject.Margin = New Padding(4, 3, 4, 3)
        tbSubject.Name = "tbSubject"
        tbSubject.Size = New Size(221, 23)
        tbSubject.TabIndex = 7
        tbSubject.Text = "League Announcement!"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(14, 31)
        Label3.Margin = New Padding(4, 0, 4, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(49, 15)
        Label3.TabIndex = 8
        Label3.Text = "Subject:"
        ' 
        ' btnAttach
        ' 
        btnAttach.Location = New Point(519, 700)
        btnAttach.Margin = New Padding(4, 3, 4, 3)
        btnAttach.Name = "btnAttach"
        btnAttach.Size = New Size(88, 27)
        btnAttach.TabIndex = 11
        btnAttach.Text = "Attach File"
        btnAttach.UseVisualStyleBackColor = True
        ' 
        ' tbAttach
        ' 
        tbAttach.Location = New Point(520, 672)
        tbAttach.Margin = New Padding(2)
        tbAttach.Name = "tbAttach"
        tbAttach.Size = New Size(620, 23)
        tbAttach.TabIndex = 12
        ' 
        ' cbRegulars
        ' 
        cbRegulars.AutoSize = True
        cbRegulars.Location = New Point(14, 107)
        cbRegulars.Margin = New Padding(4, 3, 4, 3)
        cbRegulars.Name = "cbRegulars"
        cbRegulars.Size = New Size(99, 19)
        cbRegulars.TabIndex = 13
        cbRegulars.Text = "Regulars Only"
        cbRegulars.UseVisualStyleBackColor = True
        ' 
        ' dgvEmail
        ' 
        dgvEmail.AllowUserToAddRows = False
        dgvEmail.AutoGenerateColumns = False
        dgvEmail.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvEmail.Location = New Point(18, 134)
        dgvEmail.Margin = New Padding(4, 3, 4, 3)
        dgvEmail.Name = "dgvEmail"
        dgvEmail.ReadOnly = True
        dgvEmail.RowHeadersVisible = False
        dgvEmail.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvEmail.Size = New Size(472, 524)
        dgvEmail.TabIndex = 14
        ' 
        ' btnRefresh
        ' 
        btnRefresh.Location = New Point(259, 50)
        btnRefresh.Margin = New Padding(4, 3, 4, 3)
        btnRefresh.Name = "btnRefresh"
        btnRefresh.Size = New Size(88, 27)
        btnRefresh.TabIndex = 15
        btnRefresh.Text = "Refresh List"
        btnRefresh.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(160, 107)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(137, 15)
        Label1.TabIndex = 16
        Label1.Text = "Double Click to Remove "
        ' 
        ' cbText
        ' 
        cbText.AutoSize = True
        cbText.Location = New Point(14, 87)
        cbText.Margin = New Padding(4, 3, 4, 3)
        cbText.Name = "cbText"
        cbText.Size = New Size(96, 19)
        cbText.TabIndex = 17
        cbText.Text = "Text Message"
        cbText.UseVisualStyleBackColor = True
        ' 
        ' EmailMessage
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1244, 807)
        Controls.Add(cbText)
        Controls.Add(Label1)
        Controls.Add(btnRefresh)
        Controls.Add(dgvEmail)
        Controls.Add(cbRegulars)
        Controls.Add(tbAttach)
        Controls.Add(btnAttach)
        Controls.Add(Label3)
        Controls.Add(tbSubject)
        Controls.Add(btnExit)
        Controls.Add(tbMessage)
        Controls.Add(btnSend)
        Controls.Add(Label2)
        Margin = New Padding(4, 3, 4, 3)
        CType(dgvEmail, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents btnSend As Button
    Friend WithEvents tbMessage As TextBox
    Friend WithEvents btnExit As Button
    Friend WithEvents tbSubject As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnAttach As Button
    Friend WithEvents tbAttach As TextBox
    Friend WithEvents cbRegulars As CheckBox
    Friend WithEvents dgvEmail As DataGridView
    Friend WithEvents btnRefresh As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents cbText As CheckBox
    Friend WithEvents Name As DataGridViewTextBoxColumn
    Friend WithEvents Email As DataGridViewTextBoxColumn
End Class