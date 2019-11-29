Imports System.Net.Mail
Public Class EmailMessage
    Dim ohelper As Helper
    Dim toAddresses = New List(Of String)

    Private Sub EmailMessage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = Main.oHelper
        'Dim cbToAddresses As New List(Of String)  '{"garyrscudder@gmail.com", "garyrscudder@gmail.com"}
        For Each player In ohelper.dsLeague.Tables("dtPlayers").Rows
            If player("Name").ToString.Contains("Carr") Then
                Dim x = player("Name")
            End If

            If ohelper.convDBNulltoSpaces(player("Email")).Trim <> "" Then
                'If player("Name") = "Gary Scudder" Then 'Or player("Name") = "Greg Lemker" Then
                tbToAddresses.Text &= player("Email") & Environment.NewLine
                ohelper.LOGIT(String.Format("Email to {0}", player("Email")))
                'End If
            End If
        Next

    End Sub
    Private Sub BtnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        Dim mbr = MsgBox(String.Format("are you ready to send emails to these {0} players?", tbToAddresses.Text.Split(Environment.NewLine).Count), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then Exit Sub

        'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
        'Dim attachs() As String = {semailfile}
        Dim attachs() As String = {tbAttach.Text}
        Dim subject As String = tbSubject.Text
        Dim body As String = tbMessage.Text
        Dim bresult = False
        If tbToAddresses.Lines.Count > 0 Then
            toAddresses.Addrange(tbToAddresses.Lines.ToList)
            bresult = ohelper.GGmail.SendMail(toAddresses, subject, body, attachs)
            If bresult Then
                MsgBox("mails sent successfully", MsgBoxStyle.Information)
            Else
                MsgBox(ohelper.GGmail.ErrorText, MsgBoxStyle.Critical)
            End If
        End If

    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
        End
    End Sub

    Private Sub TbToAddresses_TextChanged(sender As Object, e As EventArgs) Handles tbToAddresses.TextChanged

    End Sub

    Private Sub BtnAttach_Click(sender As Object, e As EventArgs) Handles btnAttach.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.InitialDirectory = ""
        OpenFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
        OpenFileDialog.FilterIndex = 1
        OpenFileDialog.ShowDialog()
        If OpenFileDialog.FileName <> "" Then tbAttach.Text = OpenFileDialog.FileName
    End Sub

End Class