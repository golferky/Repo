Imports System.Text
Imports System.Threading
Imports MailKit
Imports MailKit.Net.Imap
Imports MailKit.Search
Imports MailKit.Security
Imports MailKit.Net.Smtp
Imports MimeKit
Imports Google.Apis.Auth.OAuth2
Imports Google.Apis.Util.Store
Imports Google.Apis.Util
Imports System.Data.SQLite
Imports System.Data.SqlClient

Public Class eUtilities

    Friend myprocessarray As New ArrayList
    Private myProcess As Process
    Public iLogitCounter As Integer = 0
    Public sLogPath As String = "C:\Log\"
    Public sEmailsCSV As String = $"C:\GarysEmails.CSV"
    Public sDomainsCSV As String = $"C:\GarysDomains.CSV"
    Public dtDomains As DataTable
    Public dtEmails As DataTable
    'Public dtMarkedRead As DataTable
    Public client As ImapClient
    Public sThisEmailServiceaName As String
    Public lEmailClients As List(Of String)
    Public sThisEmailService As String = ""
    Public sThisEmailUser As String = ""
    Public sThisEmailPassword As String = ""
    Public personal As IMailFolder
    Public Junk As IMailFolder
    Public subfolders As Object
    'field seperated by commas
    '1-Name
    '2-Format
    '   O-Date with Offset
    '   I-Integer
    '   D-Date with time YYYY-MM-DD HH:MM:SS
    '3-Key
    '4-Visible
    '5-Updateable
    Public sDomainFields As String = "
Name,,Y,,|
Trusted,,,,|
TotalEmails,I,,,|
LatestDate,O,K,,|
Senders,,,,"
    Public sEmailFields As String =
"
DateReceived,O,Y,,|
From,,Y,,|
Domain,,,N,|
Sender,,,N,|
Subject,,,,|
Uid,I,,N,|
Spam,B,,,Y|
Read,,,,|
PhishLevel,,,,Y|
Folder,,,N,|
DateAdded,D,,N,
"

    Public sYahoo As String = "Yahoo,imap.mail.yahoo.com,gary_scudder,ncaneqoqpbdcuhae"
    Public sGmail As String = "*Gmail,imap.gmail.com,garyrscudder,7diKS7bW6zZ2wcXB8bpJ"
    Public sqlConn As String = "Data Source=192.168.1.31;Initial Catalog=Email;User ID=greg;Password=1234;Encrypt=False"
    Public conn As SqlConnection

    Sub New()
        lEmailClients = New List(Of String)
        lEmailClients.Add(sYahoo)
        lEmailClients.Add(sGmail)
        GoTo Skip
        Try
            conn = New SqlConnection(sqlConn)
            conn.Open()
        Catch ex As Exception

        End Try
Skip:
    End Sub

    Function UpdateIt(uidid As Integer, folder As String, message As MimeKit.MimeMessage, Optional spam As Boolean = False) As Boolean
        Try
            Dim utf8Encoding As New System.Text.UTF8Encoding(True)
            Dim encodedString() As Byte
            encodedString = utf8Encoding.GetBytes(message.Subject)
            'Debug.Print($"Date:{message.Date} From:{message.From} Subject: {message.Subject}")
            'Dim x As String = message.From.ToString
            'If x.Contains("The Home Depot") Then
            '    Debug.Print("")
            'End If
            If spam Then
                If Not encodedString.Contains(173) Then
                    Exit Function
                End If
            End If
            Dim drow As DataRow = Nothing
            If dtEmails IsNot Nothing Then
                Dim sKey() As Object = {message.Date, message.From}
                drow = dtEmails.Rows.Find(sKey)
            End If
            If drow Is Nothing Then
                drow = dtEmails.NewRow
                drow("DateReceived") = message.Date
                drow("From") = message.From
                drow("Domain") = message.From.ToString.Substring(message.From.ToString.IndexOf("@") + 1).Replace(">", "")
                drow("Sender") = message.From.ToString.Substring(0, message.From.ToString.IndexOf("@")).Replace("<", "")
                drow("Subject") = message.Subject
                drow("EmailClient") = "Yahoo"
                'drow("TextBody") = message.TextBody
                drow("uid") = uidid
                If encodedString.Contains(173) Then
                    drow("Spam") = True
                Else
                    drow("Spam") = False
                End If
                drow("Read") = False
                drow("PhishLevel") = ""
                drow("Folder") = folder
                'drow("Headers") = message.Headers
                'drow("HtmlBody") = message.HtmlBody
                drow("DateAdded") = Now()
                dtEmails.Rows.Add(drow)
            End If
            'UpdateEmailData(drow)
            If drow("Spam") = True Then
                'MoveToFolder(message)
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
        Finally

        End Try

    End Function

    Function Connect() As IImapClient
        Try

            'Dim oFiles() As IO.FileInfo
            'Dim oDirectory As New IO.DirectoryInfo("Y:\")
            'oFiles = oDirectory.GetFiles("*.csv")
            'EmailSetup()
            'BuildTables()
            'UpdateEmailTbl.ShowNamespaces(sThisEmailService, sThisEmailUser, sThisEmailPassword)

            'client = New ImapClient(New ProtocolLogger($"{sLogPath}imap.log"))
            client = New ImapClient(New ProtocolLogger($"{sLogPath}imap.log"))
            'setup yahoo Generate App Password called Emailscreener and copy that password and paste here
            client.Connect(sThisEmailService, 993, SecureSocketOptions.SslOnConnect)
            Return client '$"Connected to {sThisEmailService} for User {sThisEmailUser}"
            'tsStatusText.Text = $"Connecting to {sThisEmailService} for User {sThisEmailUser}"
            'Application.DoEvents()
            'Debug.Print($"{client}-{client.IsConnected}")

        Catch ex As Exception
            MessageBox.Show($"{ex.Message}", "Connect Failed", MessageBoxButtons.OK)
            Return Nothing '$"Connect Failed {sThisEmailService} for User {sThisEmailUser}"
        End Try

    End Function
    Function Authenticate() As String
        'tsStatusText.Text = $"Authenticating User {sThisEmailUser}"
        'Application.DoEvents()
        Try
            client.Authenticate(sThisEmailUser, sThisEmailPassword)
            Return $"Authenticated User {sThisEmailUser}"
        Catch ex As Exception
            Return $"Authenticating User {sThisEmailUser} Failed"
        End Try

    End Function
    Function OpenInbox(Optional readwrite As Boolean = False) As String
        Try
            If readwrite Then
                client.Inbox.Open(FolderAccess.ReadWrite)
            Else
                client.Inbox.Open(FolderAccess.ReadOnly)
            End If

            Return $"Inbox Opened for {client.Inbox.Access}"
        Catch ex As Exception
            Return $"Inbox Opened failed for {client.Inbox.Access}"
        End Try
        'tsStatusText.Text = $"Opening {client.Inbox.FullName}"
        'Application.DoEvents()

    End Function
    Sub SetupFolders()
        'Dim folder As IMailFolder = client.GetFolder(New FolderNamespace("/", "INBOX"))
        'folder.Open(FolderAccess.ReadOnly)
        'Dim subs = folder.GetSubfolders
        personal = client.GetFolder(client.PersonalNamespaces(0))
        subfolders = personal.GetSubfolders()
    End Sub
    Sub CreateFolder(client As ImapClient, sfolder As String)
        Dim personal = client.GetFolder(client.PersonalNamespaces(0))
        personal.Create(sfolder, True)
    End Sub
    Sub MoveToFolder(suid As Integer, folder As IMailFolder)
        'uid,destination folder
        folder.MoveTo(suid, folder)
    End Sub
    Sub MoveTojunk(client As ImapClient)
        MoveToPersonalFolder(client, "Junk")
    End Sub
    Sub MoveToPersonalFolder(client As ImapClient, destinationFolder As String)
        For i As Integer = 0 To client.Inbox.Count - 1
            Dim message = client.Inbox.GetMessage(i)
            'client.Inbox.AddFlags(i, MessageFlags.Seen, True)
            For Each subfolder In subfolders
                If subfolder.Name = destinationFolder Then
                    client.Inbox.MoveTo(i, subfolder)
                End If
            Next
        Next
    End Sub
    Public Sub ShowNamespaces(sThisEmailService, sThisEmailUser, sThisEmailPassword)
        Using client = New ImapClient(New ProtocolLogger("imap.log"))
            client.Connect(sThisEmailService, 993, SecureSocketOptions.SslOnConnect)
            client.Authenticate(sThisEmailUser, sThisEmailPassword)
            Debug.Print("Personal namespaces:")

            For Each ns In client.PersonalNamespaces
                Debug.Print($"* \{ns.Path}\ \{ns.DirectorySeparator}\")
            Next

            Debug.Print("")
            Debug.Print("Shared namespaces:")

            For Each ns In client.SharedNamespaces
                Debug.Print($"* \{ns.Path}\ \{ns.DirectorySeparator}\")
            Next

            Debug.Print("")
            Debug.Print("Other namespaces:")

            For Each ns In client.OtherNamespaces
                Debug.Print($"* \{ns.Path}\ \{ns.DirectorySeparator}\")
            Next

            Debug.Print("")
            Dim personal = client.GetFolder(client.PersonalNamespaces(0))
            Dim subfolders = personal.GetSubfolders()
            'EmailScreener.subfolders = personal.GetSubfolders()
            Debug.Print("The list of folders that are direct children of the first personmal namespace:")

            For Each folder In subfolders
                Debug.Print($"* {folder.Name}")
            Next

            client.Disconnect(True)
        End Using
    End Sub

#Region "Utilities"
    Sub getdbInfo()
        Dim sqlText As String = "select column_name, data_type, character_maximum_length from information_schema.columns where table_name = 'emails'" '= "select * From Emails order by uid desc"
        Dim ds = New DataSet
        Dim da As New SqlDataAdapter(sqlText, sqlConn)
        'da.SelectCommand.CommandTimeout = 5
        'da.Fill(ds, "EmailInfo")

        'For Each row As DataRow In ds.Tables("Emailinfo").Rows

        'Next

        sqlText = "select * from Emails"
        ds = New DataSet
        da = New SqlDataAdapter(sqlText, sqlConn)
        da.SelectCommand.CommandTimeout = 5
        da.Fill(ds, "Email")
        dtEmails = ds.Tables("Email")
        dtEmails.PrimaryKey = New DataColumn() {dtEmails.Columns("DateReceived"), dtEmails.Columns("From")}

    End Sub

#End Region

#Region "Mail Utilities"

    'Opens client, gets access to inbox as read only
    Sub readmessages()
        Using client = New ImapClient(New ProtocolLogger("imap.log"))
            Dim j As Integer = 1
l1:
            client.Connect("imap.mail.yahoo.com", 993, True)
            'setup yahoo Generate App Password called Emailscreener and copy that password and paste here
            client.Authenticate("gary_scudder", "ncaneqoqpbdcuhae")
            Dim inbox = client.Inbox
            inbox.Open(FolderAccess.[ReadOnly])
            Dim uids = client.Inbox.Search(SearchQuery.NotSeen)
            'GetFlags(inbox)
            Console.WriteLine("Total messages:  {0}", inbox.Count)
            Console.WriteLine("Recent messages: {0}", inbox.Recent)
            Debug.Print("Total messages: {0}", inbox.Count)
            Debug.Print("Recent messages: {0}", inbox.Recent)
            Debug.Print("You have {0} unread message(s).", uids.Count)

            For i As Integer = 0 To uids.Count - 1
                Dim message = inbox.GetMessage(uids(i))
                Dim sdomain = message.From.ToString.Substring(message.From.ToString.IndexOf("@") + 1).Replace(">", "")
                UpdatedtTable(sdomain, message)

                'Console.WriteLine("Subject: {0}", message.Subject)

                'this will mark each unread email as read
                'client.Inbox.AddFlags(New UniqueId() {uids(i)}, MessageFlags.Seen, True)
                'Console.WriteLine("You have {0} unread message(s).", uids.Count - i)
                'Debug.Print("You have {0} unread message(s).", uids.Count - i)

                If j Mod 500 = 0 Then
                    client.Disconnect(True)
                    Console.WriteLine("Disconnected")
                    Thread.Sleep(10)
                    GoTo l1
                End If
                j += 1
            Next

            'Using sw As StreamWriter()
            'GetAllUnread(inbox)
            'End Using
            client.Disconnect(True)
        End Using
    End Sub
    Sub GetAllUnread(inbox As IMailFolder)
        For i As Integer = 0 To inbox.Count - 1
            Dim message = inbox.GetMessage(i)
            'Console.WriteLine("Subject: {0}", message.Subject)
            Debug.Print($"Date:{message.Date} From:{message.From}{vbCrLf}Subject: {message.Subject}")
            If i = 10 Then
                Dim x = ""
            End If
        Next
    End Sub
    Sub UpdatedtTable(sdomain As String, message As MimeKit.MimeMessage)
        UpdatedtTable(sdomain, message, False)
    End Sub
    Sub UpdatedtTable(sdomain As String, message As MimeKit.MimeMessage, Optional bTrusted As Boolean = False)
        Try

            'Dim x As String = message.From.ToString
            'If x.Contains("The Home Depot") Then
            '    Debug.Print("")
            'End If
            'Debug.Print($"Date:{message.Date} From:{message.From} Subject: {message.Subject}")
            Dim drow As DataRow
            drow = dtDomains.Rows.Find(sdomain)
            If drow Is Nothing Then
                drow = dtDomains.NewRow
                If bTrusted Then
                    drow("Trusted") = True
                Else
                    drow("Trusted") = False
                End If
                'Dim mbr = MsgBox($"Keep messages from this domain{vbCrLf}{vbCrLf}{sdomain}?", MsgBoxStyle.YesNo)
                'If mbr = MsgBoxResult.Yes Then
                '    drow("Trusted") = True
                'Else
                '    drow("Trusted") = False
                'End If
                drow("Name") = sdomain
                drow("TotalEmails") = 1
                drow("LatestDate") = message.Date
                Dim sender = message.From.ToString.Substring(0, message.From.ToString.IndexOf("@")).Replace("<", "")
                drow("Senders") &= sender
                dtDomains.Rows.Add(drow)
            Else
                drow("TotalEmails") += 1
                If message.Date > drow("LatestDate") Then
                    drow("LatestDate") = message.Date
                End If
                Dim sender As String = message.From.ToString.Substring(0, message.From.ToString.IndexOf("@")).Replace("<", "")
                If Not drow("Senders").ToString.Contains(sender) Then
                    drow("Senders") &= sender & vbCrLf
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    '    Sub CountEmails()
    '        Try
    '            Dim j = 1
    '            'Using client = New ImapClient(New ProtocolLogger("imap.log"))
    '            Using client = New ImapClient()
    '                EmailSetup()
    'l1:
    '                tsStatusText.Text = $"Connecting To {sThisEmailService} For User {sThisEmailUser}"
    '                Application.DoEvents()
    '                client.Connect(sThisEmailService, 993, True)
    '                'setup yahoo Generate App Password called Emailscreener and copy that password and paste here
    '                tsStatusText.Text = $"Authenticating User {sThisEmailUser}"
    '                Application.DoEvents()
    '                client.Authenticate(sThisEmailUser, sThisEmailPassword)
    '                Dim inbox = client.Inbox
    '                tsStatusText.Text = $"Opening Inbox {inbox.FullName}"
    '                Application.DoEvents()
    '                inbox.Open(FolderAccess.[ReadOnly])
    '                tsStatusText.Text = $"Total Emails {inbox.Count}"
    '                tsStatusBar.Value = 0
    '                tsStatusBar.Maximum = inbox.Count - 1
    '                For i = 0 To inbox.Count - 1
    '                    LOGIT($"{i} Of {inbox.Count} ", True)
    '                    tsStatusText.Text = $"Getting email message {i} Of {inbox.Count}"
    '                    Application.DoEvents()
    '                    Dim message = inbox.GetMessage(i)
    '                    Dim domain = message.From.ToString.Substring(message.From.ToString.IndexOf("@") + 1).Replace(">", "")
    '                    Dim sender = message.From.ToString.Substring(0, message.From.ToString.IndexOf("@")).Replace("<", "")
    '                    UpdatedtTable(domain, message)
    '                    'UpdateEmailTbl.UpdateIt(dtEmails, message)
    '                    If j Mod 500 = 0 Then
    '                        client.Disconnect(True)
    '                        Console.WriteLine("Disconnected")
    '                        Thread.Sleep(10)
    '                        GoTo l1
    '                    End If
    '                    j += 1
    '                    'tsStatusText.Text = $"Saved And counted {i} email Of {inbox.Count}"
    '                    tsStatusBar.Value += 1
    '                    Application.DoEvents()
    '                Next
    '                client.Disconnect(True)
    '            End Using

    '        Catch ex As Exception

    '        End Try
    '        Dim x = ""
    '    End Sub
    Private Sub GetFlags(inbox As IMailFolder)
        Dim info = inbox.Fetch({4442}, MessageSummaryItems.Flags Or MessageSummaryItems.Flags)

        If info(0).Flags.Value.HasFlag(MessageFlags.Flagged) Then
        End If

        If info(0).Flags.Value.HasFlag(MessageFlags.Draft) Then
        End If

        If info(0).GMailLabels.Contains("Important") Then
        End If
    End Sub
    Sub SendEmails(message As MimeMessage)
        Try
            Dim rs As New Resizer
            '        Dim semailfile = oHelper.sFilePath & "\" & Main.cbLeagues.Text.Substring(Main.cbLeagues.Text.IndexOf("(") + 1, 4) & "_" &
            'Main.cbLeagues.Text.Substring(0, Main.cbLeagues.Text.IndexOf("(") - 1) & "_Schedule.csv"

            Dim oHelper = Main.oHelper
            Dim sb = New StringBuilder
            sb.AppendLine($"
SELECT * FROM Teams T
JOIN Players P ON T.Player = P.Player 
WHERE 
    T.League = '{ctx.rLeagueParmrow("Name").ToString.Replace("'", "''")}'
AND T.Year = {Main.cbSeasons.SelectedItem}    
AND P.Player = T.Player 
AND P.Email <> ''
--AND EmailStats = 'Y'
AND P.Player = 'Gary Scudder'
")
            oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", sb.ToString)
            ' Create the email message
            'Dim message As New MimeMessage()
            message.From.Add(New MailboxAddress("Hughs Golf", ctx.rLeagueParmrow("email")))
            ' Create the body text
            Dim body = New TextPart("plain") With {
            .Text = "Hello! This is an email with an attachment."
        }
            GoTo skipattachment
            ' Create the attachment
            Dim attachment = New MimePart("application", "octet-stream") With {
            .Content = New MimeContent(System.IO.File.OpenRead("path_to_attachment")),
            .ContentDisposition = New ContentDisposition(ContentDisposition.Attachment),
            .ContentTransferEncoding = ContentEncoding.Base64,
            .FileName = System.IO.Path.GetFileName("path_to_attachment")
        }

            ' Create the multipart/mixed container to hold the message text and the attachment
            Dim multipart = New Multipart("mixed")
            multipart.Add(body)
            multipart.Add(attachment)

            ' Set the multipart as the message body
            message.Body = multipart
skipattachment:
            For Each player As DataRow In oHelper.dt.Rows
                'Message.To.Add(New MailboxAddress($"{player("Player")}", $"{player("email")}"))
                message.From.Add(New MailboxAddress("Hughs Secretary", ctx.rLeagueParmrow("email")))
                message.To.Add(New MailboxAddress($"{player("Player")}", $"{oHelper.TextNumber(player("player"))}"))
                message.Subject = $"{ctx.rLeagueParmrow("Season")} League Rules"
            Next

            ' Set the email body
            Dim bodyBuilder = New BodyBuilder()
            bodyBuilder.TextBody = "This is a test email sent using MailKit."
            message.Body = bodyBuilder.ToMessageBody()

            ' Connect to the Gmail SMTP server and authenticate
            Using client = New SmtpClient()
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls)
                client.Authenticate(ctx.rLeagueParmrow("email"), "rezc gzpo wjru kqnr")

                ' Send the email
                client.Send(message)

                ' Disconnect from the server
                client.Disconnect(True)
            End Using

            Console.WriteLine("Email sent successfully!")
        Catch ex As Exception
            Console.WriteLine("Error sending email: " & ex.Message)
        End Try


    End Sub
    Sub Rainout()
        Try
            Dim rs As New Resizer
            '        Dim semailfile = oHelper.sFilePath & "\" & Main.cbLeagues.Text.Substring(Main.cbLeagues.Text.IndexOf("(") + 1, 4) & "_" &
            'Main.cbLeagues.Text.Substring(0, Main.cbLeagues.Text.IndexOf("(") - 1) & "_Schedule.csv"

            Dim oHelper = Main.oHelper
            Dim sb = New StringBuilder
            sb.AppendLine($"
SELECT * FROM Teams T
JOIN Players P ON T.Player = P.Player 
WHERE 
    T.League = '{ctx.rLeagueParmrow("Name").ToString.Replace("'", "''")}'
AND T.Year = {Main.cbSeasons.SelectedItem}    
AND P.Player = T.Player 
AND P.Email <> ''
--AND EmailStats = 'Y'
--AND P.Player in ('Gary Scudder','Greg Lemker')
--AND P.Player in ('Gary Scudder')
")
            oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", sb.ToString)
            ' Create the email message
            Dim message As New MimeMessage()
            message.From.Add(New MailboxAddress("Hughs Golf", ctx.rLeagueParmrow("email")))
            ' Create the body text
            sb = New StringBuilder
            sb.AppendLine($"
{Main.cbLeagues.SelectedItem} Golf league for {ctx.ActiveDate} has been cancelled for this week
Any questions, contact league secretary
")
            Dim body = New TextPart("plain") With {
            .Text = sb.ToString
        }
            GoTo skipattachment
            ' Create the attachment
            Dim attachment = New MimePart("application", "octet-stream") With {
            .Content = New MimeContent(System.IO.File.OpenRead("path_to_attachment")),
            .ContentDisposition = New ContentDisposition(ContentDisposition.Attachment),
            .ContentTransferEncoding = ContentEncoding.Base64,
            .FileName = System.IO.Path.GetFileName("path_to_attachment")
        }

            ' Create the multipart/mixed container to hold the message text and the attachment
            Dim multipart = New Multipart("mixed")
            multipart.Add(body)
            multipart.Add(attachment)

            ' Set the multipart as the message body
            message.Body = multipart
skipattachment:
            message.From.Add(New MailboxAddress("Hughs Secretary", $"{ctx.rLeagueParmrow("Email")}"))
            For Each player As DataRow In oHelper.dt.Rows
                'Message.To.Add(New MailboxAddress($"{player("Player")}", $"{player("email")}"))
                message.To.Add(New MailboxAddress($"{player("Player")}", $"{oHelper.TextNumber(player("player"))}"))
                message.Subject = $"{Main.cbLeagues.SelectedItem} Golf league for {ctx.ActiveDate} has been cancelled for this week"
            Next

            ' Set the email body
            Dim bodyBuilder = New BodyBuilder()
            sb = New StringBuilder
            sb.AppendLine($"
{Main.cbLeagues.SelectedItem} Golf league for {ctx.ActiveDate} has been cancelled for this week

Any questions, contact the league secretary
")
            bodyBuilder.TextBody = sb.ToString
            message.Body = bodyBuilder.ToMessageBody()

            ' Connect to the Gmail SMTP server and authenticate
            Using client = New SmtpClient()
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls)
                client.Authenticate($"{ctx.rLeagueParmrow("Email")}", "rezc gzpo wjru kqnr")

                ' Send the email
                client.Send(message)

                ' Disconnect from the server
                client.Disconnect(True)
            End Using

            Console.WriteLine("Email sent successfully!")
        Catch ex As Exception
            Console.WriteLine("Error sending email: " & ex.Message)
        End Try


    End Sub
#End Region

End Class
