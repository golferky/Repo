Imports System.Windows.Forms
Public Class CustomMessageBox

    Public Property Message As String
    Public Property ButtonTexts As List(Of String)
    Public Property ButtonResults As List(Of DialogResult)
    Private ButtonList As New List(Of Button)

    Private Sub CustomMessageBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Create a new Label instance
        Dim newLabel As New Label()

        ' Set the properties of the Label
        newLabel.Text = "This is a dynamically added label."
        newLabel.Location = New Point(50, 50) ' Set the desired location on the form
        newLabel.AutoSize = True ' Adjust the size of the Label based on its content

        ' Add the Label to the form's controls collection
        Me.Controls.Add(newLabel)

        AddButtons()
    End Sub

    Private Sub AddButtons()
        Dim buttonHeight As Integer = 30
        Dim buttonWidth As Integer = 75
        Dim spacing As Integer = 10
        Dim startX As Integer = (Me.ClientSize.Width - (buttonWidth * ButtonTexts.Count + spacing * (ButtonTexts.Count - 1))) / 2
        Dim startY As Integer = Label1.Bottom + 10

        For i As Integer = 0 To ButtonTexts.Count - 1
            Dim btn As New Button With {
                .Text = ButtonTexts(i),
                .Width = buttonWidth,
                .Height = buttonHeight,
                .Location = New Point(startX + (buttonWidth + spacing) * i, startY)
            }
            AddHandler btn.Click, AddressOf Button_Click
            Me.Controls.Add(btn)
            ButtonList.Add(btn)
        Next
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs)
        Dim clickedButton As Button = CType(sender, Button)
        Dim index As Integer = ButtonList.IndexOf(clickedButton)
        If index >= 0 AndAlso index < ButtonResults.Count Then
            Me.DialogResult = ButtonResults(index)
            Me.Close()
        End If
    End Sub

End Class
