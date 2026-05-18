Imports MimeKit
Imports MailKit.Net.Smtp
Imports MailKit.Security
Imports MailKit

Public Class DSNSmtpClient
    Inherits SmtpClient

    Protected Overrides Function GetDeliveryStatusNotifications(message As MimeMessage, mailbox As MailboxAddress) As DeliveryStatusNotification?
        ' Specify the types of notifications you want to receive
        Return DeliveryStatusNotification.Success Or DeliveryStatusNotification.Failure Or DeliveryStatusNotification.Delay
    End Function
End Class
