Public Class NewPlayer
    Dim oHelper As New Helper
    Dim bClose As Boolean = False
    Private Sub NewPlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        oHelper.bload = True
        'cant understand why i had to do this because main.helper has all the tables but i get an error
        '-----Unable to cast object of type 'System.Data.DataTable' to type 'dtCoursesDataTable'.
        oHelper = Main.oHelper

        '700 X 1550
        'If Main.iScreenWidth = 1920 Then
        '    Me.Width = 1150
        'ElseIf Main.iScreenWidth = 1366 Then
        '    Me.Width = 1150
        'ElseIf Main.iScreenWidth = 2560 Then
        '    Me.Width = 1150
        'End If
        'If Main.iScreenHeight = 1200 Or Main.iScreenHeight = 1400 Then Me.Height = 650
        'test for Greg 1366 x 768
        'Me.Width = 1200
        'Me.Height = 650
        'rs.ResizeAllControls(Me)
        Dim sWH As String = oHelper.ScreenResize("1150", "650")
        Me.Width = sWH.Split(":")(0)
        Me.Height = sWH.Split(":")(1)
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim dvPlayers As New DataView(oHelper.dsLeague.Tables("dtPlayers"))
        Dim sRowFilter As String = ""

        Dim rowView As DataRowView = dvPlayers.AddNew
        ' Change values in the DataRow.
        rowView("Name") = tbName.Text
        rowView("Phone") = tbPhone.Text
        rowView("Email") = tbEmail.Text
        rowView.EndEdit()
        oHelper.sPlayer = tbName.Text
        bClose = True
        BtnExit_Click(sender, e)
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        If bClose Then
            Me.Close()
            Exit Sub
        Else
            Dim sResult As MsgBoxResult
            sResult = MsgBox("Player not saved, still want to exit? ", MsgBoxStyle.YesNo)
            If sResult = MsgBoxResult.Yes Then
                Exit Sub
            End If
        End If

    End Sub
End Class