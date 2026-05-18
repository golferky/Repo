Imports System.Data.SQLite
Public Class frmExpenses

    Private _tc As TabControl
    Private _dt As DataTable
    Private _players As New List(Of String)
    Private _payPlaces As Integer
    Private _saving As Boolean = False

    Public Sub New(tc As TabControl)
        InitializeComponent()
        _tc = tc
    End Sub

    Private Sub frmExpenses_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = $"Expenses — {ctx.ActiveDate}"
        Me.Size = New Size(900, 500)

        ' Load players from Teams
        Using cmd As New SQLiteCommand("
            SELECT Player FROM Teams 
            WHERE League=@League AND Year=@Year 
            ORDER BY Player", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Year", CInt(ctx.rLeagueParmrow("Season")))
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            Using dr As SQLiteDataReader = cmd.ExecuteReader()
                _players.Add("")  ' blank option
                While dr.Read()
                    _players.Add(dr("Player").ToString())
                End While
            End Using
        End Using

        _payPlaces = CInt(ctx.rLeagueParmrow("PayPlaces"))

        ' Build DataTable
        _dt = New DataTable()
        _dt.Columns.Add("ID", GetType(Integer))
        _dt.Columns.Add("Category", GetType(String))
        _dt.Columns.Add("Player1", GetType(String))
        _dt.Columns.Add("Player2", GetType(String))
        _dt.Columns.Add("Amount", GetType(Decimal))
        _dt.Columns.Add("Half", GetType(String))
        _dt.Columns.Add("Place", GetType(String))
        _dt.Columns.Add("Comment", GetType(String))
        _dt.Columns.Add("Date", GetType(String))

        ' Load existing expense rows from Payments
        LoadExistingRows()

        ' Build grid
        BuildGrid()
    End Sub

    Private Sub LoadExistingRows()
        _dt.Rows.Clear()

        Dim sql As String = "
        SELECT Desc, Player, Earned, Comment, Detail, Date
        FROM Payments
        WHERE League=@League
          AND SUBSTR(Date, 1, 4) = @Season
          AND Desc IN ('Food','Drinks','Reg Season Champ-1st half',
                      'Reg Season Champ-2nd half','Club Champion-1st',
                      'Club Champion-2nd','Skin','CTP')
          AND Detail IN ('Charge','Earned-1st','Earned-2nd',
                        'Earned-3rd','Earned-CC','#1','#2','#3','#4',
                        '#5','#6','#7','#8','#9')
        ORDER BY Date, Desc"

        Dim dtRaw As New DataTable
        Using cmd As New SQLiteCommand(sql, ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Season", ctx.SeasonYear)
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(dtRaw)
            End Using
        End Using

        ' Track paired rows for Reg Season Champ
        Dim pairedRows As New List(Of DataRow)

        For Each raw As DataRow In dtRaw.Rows
            If pairedRows.Contains(raw) Then Continue For

            Dim desc As String = raw("Desc").ToString()
            Dim detail As String = raw("Detail").ToString()
            Dim player As String = raw("Player").ToString()
            Dim amount As Decimal = CDec(raw("Earned"))
            Dim comment As String = raw("Comment").ToString()
            Dim dateVal As String = raw("Date").ToString()

            Dim newRow As DataRow = _dt.NewRow()
            newRow("ID") = DBNull.Value

            Select Case desc
                Case "Food"
                    newRow("Category") = "Food Charge"
                    newRow("Amount") = amount
                    newRow("Comment") = comment
                    newRow("Date") = dateVal

                Case "Drinks"
                    newRow("Category") = "Drinks Charge"
                    newRow("Amount") = amount
                    newRow("Comment") = comment
                    newRow("Date") = dateVal

                Case "Reg Season Champ-1st half", "Reg Season Champ-2nd half"
                    newRow("Category") = "Reg Season Champ"
                    newRow("Half") = If(desc.Contains("1st"), "1st", "2nd")
                    newRow("Place") = detail.Replace("Earned-", "")
                    newRow("Amount") = amount
                    newRow("Player1") = player
                    newRow("Date") = dateVal

                    ' Find partner with same Desc/Detail/Amount/Date
                    For Each raw2 As DataRow In dtRaw.Rows
                        If raw2 Is raw OrElse pairedRows.Contains(raw2) Then Continue For
                        If raw2("Desc").ToString() = desc AndAlso
                       raw2("Detail").ToString() = detail AndAlso
                       raw2("Date").ToString() = dateVal AndAlso
                       CDec(raw2("Earned")) = amount Then
                            newRow("Player2") = raw2("Player").ToString()
                            pairedRows.Add(raw2)
                            Exit For
                        End If
                    Next

                Case "Club Champion-1st", "Club Champion-2nd"
                    newRow("Category") = "Club Champ"
                    newRow("Place") = If(desc.Contains("1st"), "1st", "2nd")
                    newRow("Amount") = amount
                    newRow("Player1") = player
                    newRow("Date") = dateVal

                Case "Skin"
                    newRow("Category") = "Skins Payout"
                    newRow("Player1") = player
                    newRow("Amount") = amount
                    newRow("Comment") = detail
                    newRow("Date") = dateVal

                Case "CTP"
                    newRow("Category") = "CTP Payout"
                    newRow("Player1") = player
                    newRow("Amount") = amount
                    newRow("Comment") = detail
                    newRow("Date") = dateVal
            End Select

            _dt.Rows.Add(newRow)
        Next
    End Sub


    Private Sub BuildGrid()
        Dim dgv As New DataGridView()
        dgv.Name = "dgExpenses"
        dgv.Dock = DockStyle.Fill
        dgv.AutoGenerateColumns = False
        dgv.RowHeadersVisible = True
        dgv.AllowUserToAddRows = True
        dgv.AllowUserToDeleteRows = True
        dgv.EditMode = DataGridViewEditMode.EditOnEnter

        ' Category dropdown column
        Dim catCol As New DataGridViewComboBoxColumn()
        catCol.Name = "Category"
        catCol.HeaderText = "Category"
        catCol.Width = 160
        catCol.Items.AddRange(
            "Food Charge",
            "Drinks Charge",
            "Reg Season Champ",
            "Club Champ",
            "Skins Payout",
            "CTP Payout")
        catCol.DisplayStyleForCurrentCellOnly = True
        dgv.Columns.Add(catCol)

        ' Player1 dropdown
        Dim p1Col As New DataGridViewComboBoxColumn()
        p1Col.Name = "Player1"
        p1Col.HeaderText = "Player 1"
        p1Col.Width = 130
        For Each p As String In _players
            p1Col.Items.Add(p)
        Next
        p1Col.DisplayStyleForCurrentCellOnly = True
        dgv.Columns.Add(p1Col)

        ' Player2 dropdown
        Dim p2Col As New DataGridViewComboBoxColumn()
        p2Col.Name = "Player2"
        p2Col.HeaderText = "Player 2"
        p2Col.Width = 130
        For Each p As String In _players
            p2Col.Items.Add(p)
        Next
        p2Col.DisplayStyleForCurrentCellOnly = True
        dgv.Columns.Add(p2Col)

        ' Amount
        Dim amtCol As New DataGridViewTextBoxColumn()
        amtCol.Name = "Amount"
        amtCol.HeaderText = "Amount"
        amtCol.Width = 70
        amtCol.DefaultCellStyle.Format = "C2"
        amtCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        dgv.Columns.Add(amtCol)

        ' Half dropdown
        Dim halfCol As New DataGridViewComboBoxColumn()
        halfCol.Name = "Half"
        halfCol.HeaderText = "Half"
        halfCol.Width = 60
        halfCol.Items.AddRange("", "1st", "2nd")
        halfCol.DisplayStyleForCurrentCellOnly = True
        dgv.Columns.Add(halfCol)

        ' Place
        Dim placeCol As New DataGridViewTextBoxColumn()
        placeCol.Name = "Place"
        placeCol.HeaderText = "Place"
        placeCol.Width = 60
        dgv.Columns.Add(placeCol)

        ' Comment
        Dim commentCol As New DataGridViewTextBoxColumn()
        commentCol.Name = "Comment"
        commentCol.HeaderText = "Comment"
        commentCol.Width = 200
        dgv.Columns.Add(commentCol)

        Dim dateCol As New DataGridViewTextBoxColumn()
        dateCol.Name = "Date"
        dateCol.HeaderText = "Date"
        dateCol.Width = 80
        dateCol.ReadOnly = True
        dgv.Columns.Add(dateCol)

        ' Bind data
        dgv.DataSource = _dt

        ' Styling
        dgv.DefaultCellStyle.Font = New Font("Segoe UI", 9)
        dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        dgv.RowTemplate.Height = 24
        dgv.ColumnHeadersHeight = 28
        dgv.BackgroundColor = Color.White
        dgv.BorderStyle = BorderStyle.None
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgv.GridColor = Color.LightGray
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)

        Me.Controls.Add(dgv)

        ' --- EVENTS ---
        AddHandler dgv.RowValidated, AddressOf dgv_RowValidated
        AddHandler dgv.UserDeletingRow, AddressOf dgv_UserDeletingRow
        AddHandler dgv.CellValueChanged, AddressOf dgv_CellValueChanged
    End Sub

    Private Sub dgv_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        ' Hide/show columns based on category
        If e.RowIndex < 0 Then Exit Sub
        Dim dgv = DirectCast(sender, DataGridView)
        Dim cat As String = dgv.Rows(e.RowIndex).Cells("Category").Value?.ToString()
        If String.IsNullOrEmpty(cat) Then Exit Sub

        Select Case cat
            Case "Food Charge", "Drinks Charge"
                dgv.Rows(e.RowIndex).Cells("Player1").Value = ""
                dgv.Rows(e.RowIndex).Cells("Player2").Value = ""
                dgv.Rows(e.RowIndex).Cells("Half").Value = ""
                dgv.Rows(e.RowIndex).Cells("Place").Value = ""
            Case "Club Champ", "Skins Payout", "CTP Payout"
                dgv.Rows(e.RowIndex).Cells("Player2").Value = ""
                dgv.Rows(e.RowIndex).Cells("Half").Value = ""
            Case "Reg Season Champ"
                ' All fields needed
        End Select
    End Sub

    Private Sub dgv_RowValidated(sender As Object, e As DataGridViewCellEventArgs)
        If _saving Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        Dim dgv = DirectCast(sender, DataGridView)
        Dim row = dgv.Rows(e.RowIndex)
        If row.IsNewRow Then Exit Sub

        Dim cat As String = row.Cells("Category").Value?.ToString()
        If String.IsNullOrEmpty(cat) Then Exit Sub

        Dim amount As Decimal = 0
        If Not Decimal.TryParse(row.Cells("Amount").Value?.ToString(), amount) Then Exit Sub
        If amount <= 0 Then Exit Sub

        _saving = True
        Try
            Dim dtRow As DataRow = Nothing
            If row.Index < _dt.Rows.Count Then
                dtRow = _dt.Rows(row.Index)
            End If

            ' Delete existing record if updating
            Dim existingId As Integer = 0
            If dtRow IsNot Nothing AndAlso Not IsDBNull(dtRow("ID")) Then
                existingId = CInt(dtRow("ID"))
                Using cmd As New SQLiteCommand("
                    DELETE FROM Payments WHERE ID=@ID", ctx.Conn)
                    cmd.Parameters.AddWithValue("@ID", existingId)
                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End If

            Dim player1 As String = row.Cells("Player1").Value?.ToString()
            Dim player2 As String = row.Cells("Player2").Value?.ToString()
            Dim half As String = row.Cells("Half").Value?.ToString()
            Dim place As String = row.Cells("Place").Value?.ToString()
            Dim comment As String = row.Cells("Comment").Value?.ToString()

            Select Case cat
                Case "Food Charge"
                    InsertPayment("Food", "Charge", "Kitty", amount, comment)
                Case "Drinks Charge"
                    InsertPayment("Drinks", "Charge", "Kitty", amount, comment)
                Case "Reg Season Champ"
                    Dim desc As String = $"Reg Season Champ-{half} half"
                    Dim detail As String = $"Earned-{place}"
                    If Not String.IsNullOrEmpty(player1) Then
                        InsertPayment(desc, detail, player1, amount, comment)
                    End If
                    If Not String.IsNullOrEmpty(player2) Then
                        InsertPayment(desc, detail, player2, amount, comment)
                    End If
                Case "Club Champ"
                    Dim desc As String = $"Club Champion-{place}"
                    If Not String.IsNullOrEmpty(player1) Then
                        InsertPayment(desc, "Earned-CC", player1, amount, comment)
                    End If
                Case "Skins Payout"
                    If Not String.IsNullOrEmpty(player1) Then
                        InsertPayment("Skin", comment, player1, amount, "")
                    End If
                Case "CTP Payout"
                    If Not String.IsNullOrEmpty(player1) Then
                        InsertPayment("CTP", comment, player1, amount, "")
                    End If
            End Select

            ' Reload to get new IDs
            LoadExistingRows()
            Dim dgv2 = DirectCast(Me.Controls("dgExpenses"), DataGridView)
            dgv2.DataSource = Nothing
            dgv2.DataSource = _dt

        Finally
            _saving = False
        End Try
    End Sub

    Private Sub InsertPayment(desc As String, detail As String, player As String, amount As Decimal, comment As String)
        Using cmd As New SQLiteCommand("
            INSERT INTO Payments 
            (League, Player, Date, Desc, Detail, Earned, DatePaid, Comment, PayMethod)
            VALUES 
            (@League, @Player, @Date, @Desc, @Detail, @Earned, @DatePaid, @Comment, '')", ctx.Conn)
            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
            cmd.Parameters.AddWithValue("@Player", player)
            cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
            cmd.Parameters.AddWithValue("@Desc", desc)
            cmd.Parameters.AddWithValue("@Detail", detail)
            cmd.Parameters.AddWithValue("@Earned", amount)
            cmd.Parameters.AddWithValue("@DatePaid", DateTime.Now.ToString("M/d/yyyy h:mm:ss tt"))
            cmd.Parameters.AddWithValue("@Comment", If(comment, ""))
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            cmd.ExecuteNonQuery()
            LOGIT($"InsertPayment: {player} {desc} {detail} {amount:C2}")
        End Using
    End Sub

    Private Sub dgv_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs)
        If e.Row.IsNewRow Then Exit Sub
        Dim dtRow As DataRow = _dt.Rows(e.Row.Index)
        If IsDBNull(dtRow("ID")) OrElse CInt(dtRow("ID")) = 0 Then Exit Sub

        Dim mbr = MessageBox.Show("Delete this expense?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If mbr <> DialogResult.Yes Then
            e.Cancel = True
            Exit Sub
        End If

        Using cmd As New SQLiteCommand("
            DELETE FROM Payments WHERE ID=@ID", ctx.Conn)
            cmd.Parameters.AddWithValue("@ID", CInt(dtRow("ID")))
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
            cmd.ExecuteNonQuery()
            LOGIT($"Deleted expense ID={dtRow("ID")}")
        End Using
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

End Class