Public Class frm_Popup
    Dim ohelper As New Helper
    Private Sub Frm_Popup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ohelper = Main.oHelper
        lbl_Message.Visible = True
        lbl_Message.Text = ohelper.sMessage
        Dim myTimer As New Timer
        myTimer.Interval = 1000 * 5
        AddHandler myTimer.Tick, AddressOf myTimer_Tick
        myTimer.Start()
    End Sub
    Private Sub myTimer_Tick(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class