'Imports System.Net.Mail
Imports MailKit.Net.Smtp
Imports MailKit.Security
Imports MimeKit
Public Class EmailMessage
    Dim ohelper As Helper
    Dim sForm As String
    Dim eUtil = New eUtilities
    Dim attachment = New MimePart("application", "octet-stream")
    ' Set the multipart as the message body
    Dim multipart = New Multipart("mixed")

    Private Sub EmailMessage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dgvEmail.AutoGenerateColumns = True
        ohelper = ctx.oHelper
        cbRegulars.Checked = True
        sForm = Me.Text
        eUtil.sThisEmailService = "Gmail"
        eUtil.sThisEmailUser = ctx.rLeagueParmrow("email")
        eUtil.sThisEmailPassword = ctx.rLeagueParmrow("emailpassword")
        'eUtil.SendEmails()
        'eUtil.Rainout()

    End Sub

    Private Sub BtnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        Dim mbr = MsgBox(String.Format($"Are you ready to send { If(cbText.Checked, "Text", "Email")} to these {dgvEmail.SelectedRows.Count} players?"), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then Exit Sub
        'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
        'Dim attachs() As String = {semailfile}

        'Dim attachs() As String = Nothing
        'If tbAttach.Text <> "" Then attachs = tbAttach.Text.Split(";")
        Dim message As New MimeMessage()
        message.From.Add(New MailboxAddress("Hughs Golf", ctx.rLeagueParmrow("email")))
        message.Subject = tbSubject.Text
        ' Set the email body
        'Dim bodyBuilder = New BodyBuilder()
        'bodyBuilder.TextBody = tbMessage.Text
        'message.Body = bodyBuilder.ToMessageBody()
        ' Create the body text
        Dim body = New TextPart("plain") With {.Text = tbMessage.Text}
        multipart.Add(body)
        message.Body = multipart
        'Dim srecipients As New List(Of String)
        'ohelper.dt = ohelper.sqlitedaFromSql(ctx.Conn, "", $"select * from Players")
        For Each playerrow As DataGridViewRow In dgvEmail.SelectedRows
            'message.To.Add(New MailboxAddress($"{playerrow.Cells("Player").Value}", $"{ohelper.TextNumber(playerrow.Cells("player").Value)}"))
            If cbText.Checked Then
                message.To.Add(New MailboxAddress($"{playerrow.Cells("Player").Value}", $"{ohelper.TextNumber(playerrow.Cells("Player").Value)}"))
            Else
                message.To.Add(New MailboxAddress($"{playerrow.Cells("Player").Value}", $"{playerrow.Cells("Email").Value}"))
            End If
        Next

        Try
            ' Connect to the Gmail SMTP server and authenticate
            Using client = New SmtpClient()
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls)
                client.Authenticate($"{ctx.rLeagueParmrow("Email")}", $"{ctx.rLeagueParmrow("EmailPassword")}")
                Dim x = ctx.rLeagueParmrow("EmailPassword")
                ' Send the email
                client.Send(message)

                ' Disconnect from the server
                client.Disconnect(True)
            End Using

            MessageBox.Show($"{dgvEmail.SelectedRows.Count} { If(cbText.Checked, "Texts", "Emails")} with {message.Attachments.Count} attachments sent successfully!")
        Catch ex As Exception
            MessageBox.Show("Error sending email: " & ex.Message)
        End Try

    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub BtnAttach_Click(sender As Object, e As EventArgs) Handles btnAttach.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.InitialDirectory = ""
        OpenFileDialog.Filter = "Reports (*.pdf)|*.pdf,*.png|All Files (*.*)|*.*"
        OpenFileDialog.FilterIndex = 1
        OpenFileDialog.Multiselect = True
        OpenFileDialog.ShowDialog()
        For Each file In OpenFileDialog.FileNames
            ' Create the attachment
            attachment = New MimePart("application", "octet-stream") With {
                .Content = New MimeContent(System.IO.File.OpenRead(file)),
                .ContentDisposition = New ContentDisposition(ContentDisposition.Attachment),
                .ContentTransferEncoding = ContentEncoding.Base64,
                .FileName = System.IO.Path.GetFileName(file)
            }
            multipart.Add(attachment)
            tbAttach.Text &= file & ";"
        Next

    End Sub

    Private Sub cbRegulars_CheckedChanged(sender As Object, e As EventArgs) Handles cbRegulars.CheckedChanged
        'LoadUsers()
        Dim sb = New System.Text.StringBuilder
        sb.AppendLine($"select T.Player as Player,Email,Phone,CellCarrier as Carrier from Teams T
join players P on T.Player = P.Player
where T.Year = {ctx.SeasonYear} and (email <> '' or phone <> '')
    ")
        Dim dt = ohelper.sqlitedaFromSql(ctx.Conn, "", sb.ToString)
        'If cbRegulars.Checked Then BindingSource.Filter &= "and Team > 0"
        dgvEmail.DataSource = dt
        dgvEmail.Columns("Email").Visible = True
        dgvEmail.Columns("Phone").Visible = False
        btnSend.Text = "Send Email"
        ohelper.Resizedgv(dgvEmail, Me)
        'For Each playerrow As DataGridViewRow In DtPlayersDataGridView.Rows
        '    Dim splyr = ohelper.dsLeague.Tables("dtPlayers").Rows.Find(playerrow.Cells(0).Value)
        '    If Not splyr("Team") IsNot DBNull.Value Then
        '        playerrow.Cells(0).Style.BackColor = Color.LightBlue
        '    Else
        '        playerrow.Selected = True
        '    End If
        'Next

    End Sub

    Private Sub DtPlayersDataGridView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles dgvEmail.MouseDoubleClick
        For Each dr In dgvEmail.SelectedRows
            dgvEmail.Rows.Remove(dr)
        Next
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        cbRegulars_CheckedChanged(sender, e)
    End Sub
    Private Sub Emailmessage_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '    rs.ResizeAllControls(Me)
            'Me.Text = String.Format("Emailmessage -Screen {5}x{6} Form {0}x{1}-Grid {2}x{3} ", Me.Width, Me.Height, DtPlayersDataGridView.Width, DtPlayersDataGridView.Height, Main.oHelper.sPlayer, Main.iScreenWidth, Main.iScreenHeight)
            Me.Text = $"EmailMessage-{My.Computer.Name}, Resolution {Screen.PrimaryScreen.Bounds.Width} x {Screen.PrimaryScreen.Bounds.Height}, Grid {dgvEmail.Width} x {dgvEmail.Height}, Message {tbMessage.Width } x {tbMessage.Height}"

            LOGIT(String.Format("Form Height {0} Width {1}", Me.Height, Me.Width))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DtPlayersDataGridView_Resize(sender As Object, e As EventArgs) Handles dgvEmail.Resize
        Emailmessage_Resize(sender, e)
    End Sub

    Private Sub cbText_CheckedChanged(sender As Object, e As EventArgs) Handles cbText.CheckedChanged
        If cbText.Checked Then
            Dim sb = New System.Text.StringBuilder
            sb.AppendLine($"select T.Player,Phone,CellCarrier Carrier from Teams T
join Players P on T.Player = P.Player
where T.Year = {ctx.SeasonYear}
AND Phone <> '' and CellCarrier <> '' and CellCarrier <> 'None'
    ")
            'If cbRegulars.Checked Then BindingSource.Filter &= "and Team > 0"
            dgvEmail.DataSource = ohelper.sqlitedaFromSql(ctx.Conn, "", sb.ToString)
            'dgvEmail.Columns("Email").Visible = False
            'dgvEmail.Columns("Phone").Visible = True
            btnSend.Text = "Send Text"
            ohelper.Resizedgv(dgvEmail, Me)
            'For Each playerrow As DataGridViewRow In DtPlayersDataGridView.Rows
            '    Dim splyr = ohelper.dsLeague.Tables("dtPlayers").Rows.Find(playerrow.Cells(0).Value)
            '    If Not splyr("Team") IsNot DBNull.Value Then
            '        playerrow.Cells(0).Style.BackColor = Color.LightBlue
            '    Else
            '        playerrow.Selected = True
            '    End If
            'Next
        Else
            btnSend.Text = "Send Email"
            cbRegulars_CheckedChanged(sender, e)
        End If
    End Sub
End Class