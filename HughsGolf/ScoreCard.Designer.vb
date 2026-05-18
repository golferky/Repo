<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ScoreCard
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
        cbDates = New ComboBox()
        Label1 = New Label()
        Button2 = New Button()
        tbPlayer = New TextBox()
        Label10 = New Label()
        TextBox1 = New TextBox()
        btnPrevDate = New Button()
        btnNextDate = New Button()
        lblCurrentDate = New Label()
        SuspendLayout()
        ' 
        ' cbDates
        ' 
        cbDates.FormattingEnabled = True
        cbDates.Location = New Point(384, 28)
        cbDates.Name = "cbDates"
        cbDates.Size = New Size(121, 23)
        cbDates.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(387, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(36, 15)
        Label1.TabIndex = 2
        Label1.Text = "Dates"
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(123, 26)
        Button2.Name = "Button2"
        Button2.Size = New Size(81, 23)
        Button2.TabIndex = 19
        Button2.Text = "Quick Find"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' tbPlayer
        ' 
        tbPlayer.Location = New Point(17, 25)
        tbPlayer.Name = "tbPlayer"
        tbPlayer.Size = New Size(100, 23)
        tbPlayer.TabIndex = 17
        ' 
        ' Label10
        ' 
        Label10.AutoSize = True
        Label10.Location = New Point(332, 9)
        Label10.Name = "Label10"
        Label10.Size = New Size(46, 15)
        Label10.TabIndex = 40
        Label10.Text = "Week #"
        Label10.Visible = False
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(332, 27)
        TextBox1.Name = "TextBox1"
        TextBox1.ReadOnly = True
        TextBox1.Size = New Size(40, 23)
        TextBox1.TabIndex = 41
        TextBox1.Visible = False
        ' 
        ' btnPrevDate
        ' 
        btnPrevDate.Location = New Point(218, 28)
        btnPrevDate.Name = "btnPrevDate"
        btnPrevDate.Size = New Size(46, 23)
        btnPrevDate.TabIndex = 42
        btnPrevDate.Text = "<"
        btnPrevDate.UseVisualStyleBackColor = True
        ' 
        ' btnNextDate
        ' 
        btnNextDate.Location = New Point(280, 28)
        btnNextDate.Name = "btnNextDate"
        btnNextDate.Size = New Size(46, 23)
        btnNextDate.TabIndex = 43
        btnNextDate.Text = ">"
        btnNextDate.UseVisualStyleBackColor = True
        ' 
        ' lblCurrentDate
        ' 
        lblCurrentDate.AutoSize = True
        lblCurrentDate.Location = New Point(221, 5)
        lblCurrentDate.Name = "lblCurrentDate"
        lblCurrentDate.Size = New Size(31, 15)
        lblCurrentDate.TabIndex = 44
        lblCurrentDate.Text = "Date"
        ' 
        ' ScoreCard
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1434, 686)
        Controls.Add(lblCurrentDate)
        Controls.Add(btnNextDate)
        Controls.Add(btnPrevDate)
        Controls.Add(TextBox1)
        Controls.Add(Label10)
        Controls.Add(Button2)
        Controls.Add(Label1)
        Controls.Add(cbDates)
        Controls.Add(tbPlayer)
        Name = "ScoreCard"
        Text = "ScoreCard"
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents cbDates As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents lblNoShow As Label
    Friend WithEvents lblSub As Label
    Friend WithEvents lblMatchWon As Label
    Friend WithEvents lblUnderPar As Label
    Friend WithEvents lblMatchTied As Label
    Friend WithEvents cbEarn As CheckBox
    Friend WithEvents cbMatch As CheckBox
    Friend WithEvents cbFinance As CheckBox
    Friend WithEvents cbScores As CheckBox
    Friend WithEvents gbGrid As GroupBox
    Friend WithEvents cbLast5 As CheckBox
    Friend WithEvents tbPlayer As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents cbSkinCTP As CheckBox
    Friend WithEvents cbEOYSkins As CheckBox
    Friend WithEvents Label10 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents btnPrevDate As Button
    Friend WithEvents btnNextDate As Button
    Friend WithEvents lblCurrentDate As Label
    Friend WithEvents btnPrintScores As Button
    Friend WithEvents btnPrintMatches As Button
    Friend WithEvents btnPrintStandings As Button
End Class
