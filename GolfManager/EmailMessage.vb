Imports System.Net.Mail
Public Class EmailMessage
    Dim ohelper As Helper

    Private Sub EmailMessage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = Main.oHelper
        cbRegulars.Checked = True

    End Sub

    Private Sub BtnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click

        Dim mbr = MsgBox(String.Format("are you ready to send emails to these {0} players?", DtPlayersDataGridView.SelectedRows.Count), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then Exit Sub

        'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
        'Dim attachs() As String = {semailfile}
        Dim attachs() As String = Nothing
        If tbAttach.Text <> "" Then attachs = tbAttach.Text.Split(";")
        Dim subject As String = tbSubject.Text
        Dim body As String = tbMessage.Text
        Dim bresult = False

        Dim srecipients As New List(Of String)
        For Each playerrow As DataGridViewRow In DtPlayersDataGridView.SelectedRows
            srecipients.Add(playerrow.Cells("Email").Value)
        Next

        If srecipients.Count > 0 Then
            bresult = ohelper.GGmail.SendMail(srecipients, subject, body, attachs)
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

    Private Sub BtnAttach_Click(sender As Object, e As EventArgs) Handles btnAttach.Click
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.InitialDirectory = ""
        OpenFileDialog.Filter = "Reports (*.html)|*.html|All Files (*.*)|*.*"
        OpenFileDialog.FilterIndex = 1
        OpenFileDialog.Multiselect = True
        OpenFileDialog.ShowDialog()
        For Each file In OpenFileDialog.FileNames
            tbAttach.Text += file & ";"
        Next
        tbAttach.Text = tbAttach.Text.Substring(0, tbAttach.Text.Length - 1)
    End Sub

    Private Sub cbRegulars_CheckedChanged(sender As Object, e As EventArgs) Handles cbRegulars.CheckedChanged
        'LoadUsers()
        Dim BindingSource = New BindingSource()
        Dim dt = ohelper.dsLeague.Tables("dtPlayers").Copy
        BindingSource.DataSource = dt
        BindingSource.Filter = "Email <> ''"
        If cbRegulars.Checked Then BindingSource.Filter &= "and Team > 0"
        DtPlayersDataGridView.DataSource = BindingSource
        ohelper.Resizedgv(DtPlayersDataGridView, Me)
        For Each playerrow As DataGridViewRow In DtPlayersDataGridView.Rows
            Dim splyr = ohelper.dsLeague.Tables("dtPlayers").Rows.Find(playerrow.Cells(0).Value)
            If Not splyr("Team") IsNot DBNull.Value Then playerrow.Cells(0).Style.BackColor = Color.LightBlue
        Next

    End Sub

    Private Sub DtPlayersDataGridView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles DtPlayersDataGridView.MouseDoubleClick
        For Each dr In DtPlayersDataGridView.SelectedRows
            DtPlayersDataGridView.Rows.Remove(dr)
        Next
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        cbRegulars_CheckedChanged(sender, e)
    End Sub
    Private Sub Emailmessage_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Main.oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '    rs.ResizeAllControls(Me)
            'Me.Text = String.Format("Emailmessage -Screen {5}x{6} Form {0}x{1}-Grid {2}x{3} ", Me.Width, Me.Height, DtPlayersDataGridView.Width, DtPlayersDataGridView.Height, Main.oHelper.sPlayer, Main.iScreenWidth, Main.iScreenHeight)
            Me.Text = String.Format("Form {7}-{0}, Resolution {1} x {2}, Menu {3} x {4}, Grid {5} x {6}", My.Computer.Name, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Width, Me.Width, Me.Height, DtPlayersDataGridView.Width, DtPlayersDataGridView.Height, Me.Name)

            Main.oHelper.LOGIT(String.Format("Form Height {0} Width {1}", Me.Height, Me.Width))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DtPlayersDataGridView_Resize(sender As Object, e As EventArgs) Handles DtPlayersDataGridView.Resize
        Emailmessage_Resize(sender, e)
    End Sub
End Class