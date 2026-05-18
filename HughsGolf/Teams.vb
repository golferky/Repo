Imports System.Data.SQLite
Imports System.Numerics

Public Class Teams
    Dim oHelper As New Helper
    Dim changes As New List(Of String)
    Dim sb = New Text.StringBuilder
    Dim dtTeams As New DataTable
    Private Sub Teams_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = New Helper
        Dim sc = New HughsGolf.ScoreCard
        dtTeams = oHelper.sqlitedaFromSql(ctx.Conn, "", $"SELECT * FROM Teams WHERE Year = {ctx.ActiveDate.ToString.Substring(0, 4)}")
        With dgTeams
            .DataSource = dtTeams
            .AllowUserToAddRows = False
            .RowHeadersVisible = False
            .Columns("ID").Visible = False
            .Columns("League").Visible = False
            .Columns("Year").Visible = False
            .Columns("Team").ReadOnly = True
            .Columns("Grade").ReadOnly = True
            For Each col As DataGridViewColumn In dgTeams.Columns
                col.Width = 50
            Next
            .Columns("Player").Width = 170
            .Columns("TextStats").Width = 75
            .Columns("EmailStats").Width = 75
            .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Tahoma", 9)
            'doesnt work
            '.RowTemplate.Height = 5
        End With
        For Each row In dgTeams.Rows
            row.height = 15
        Next
        'AddHandler dtTeams.RowChanged, AddressOf RowChanged
        oHelper.wk = ""

    End Sub

    Private Sub Teams_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If changes.Count > 0 Then

        End If
    End Sub
    Sub dp(message As String)
        Debug.Print(message)
    End Sub
End Class