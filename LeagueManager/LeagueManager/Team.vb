Public Class frmTeam
    Dim oHelper As Helper.Controls.Helper
    Dim sPlayer As String
    Private Sub frmTeam_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        UpdateGrid()
        If oHelper.sFindplayerOption = "Not Found" Then
            MsgBox("Player not found, pick the player you think relates to this player " & oHelper.sPlayerToFind)
        ElseIf oHelper.sFindPlayerOption = "Sub" Then
            MsgBox("Pick a player who " & oHelper.sPlayerToFind & " is subbing for")
        End If
    End Sub
    Private Sub UpdateGrid()

        Dim rowvalue As String, cellvalue(2) As String

        Try
            Dim sr As IO.StreamReader = New IO.StreamReader(oHelper.sFilePath & "\Players.csv")
            While sr.Peek() <> -1
                rowvalue = sr.ReadLine
                If rowvalue.Contains("Name") Then
                    Continue While
                End If
                cellvalue = rowvalue.Split(","c)

                dgPlayers.Rows.Add(cellvalue)
            End While
            sr.Close()

        Catch ex As Exception
            If ex.Message.Contains("being used by another process") Then
                MsgBox("Close the file " & (oHelper.sFilePath & "\Players.csv" & vbCrLf & "Try again"))
                UpdateGrid()
            End If
        End Try
    End Sub

    Private Sub dgPlayers_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgPlayers.RowEnter
        oHelper.sTeam = dgPlayers.Rows(e.RowIndex).Cells("Team").Value
        sPlayer = dgPlayers.Rows(e.RowIndex).Cells("Name").Value
        oHelper.sPlayer = sPlayer
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Dim sResult As MsgBoxResult
        sResult = MsgBox("Player you want is " & sPlayer, MsgBoxStyle.OkCancel)
        If sResult <> MsgBoxResult.Ok Then
            Me.Close()
            ' Me.ShowDialog()
        End If

        Me.Close()
    End Sub

End Class