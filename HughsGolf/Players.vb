'Imports System.Data.SQLite
Imports System.Data.SQLite
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports ClosedXML.Excel
Imports HughsGolf.Constants
Public Class Players

    Dim oHelper As New Helper
    Dim changes As New List(Of String)
    Dim sb = New Text.StringBuilder
    Dim sOldCellValue As Object
    Private Sub Players_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = New Helper
        Dim sc = New HughsGolf.ScoreCard
        oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", $"SELECT * FROM Players ORDER BY Lastname, Firstname")
        With dgPlayers
            .DataSource = oHelper.dt
            .AllowUserToAddRows = True
            .RowHeadersVisible = False
            .Columns("ID").Visible = False
            For Each col As DataGridViewColumn In dgPlayers.Columns
                col.Width = 150
            Next
            .Columns(Player).Width = 170
            .Columns("FirstName").Width = 90
            .Columns("LastName").Width = 90
            .ColumnHeadersDefaultCellStyle.Font = New Font("Verdana", 8, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Verdana", 9)
            'doesnt work
            '.RowTemplate.Height = 5
        End With
        For Each row In dgPlayers.Rows
            row.height = 18
        Next
        CreateMaskField(dgPlayers, "Phone")
        CreateMaskField(dgPlayers, "HomePhone")
        If dgPlayers.Rows.Count > 0 Then
            Dim lastRowIndex As Integer = dgPlayers.Rows.Count - 1
            'dgPlayers.CurrentCell = dgPlayers.Rows(lastRowIndex).Cells(0) ' Set focus to the first cell of the last row
            dgPlayers.Rows(lastRowIndex).Selected = True ' Optionally select the last row
        End If
        oHelper.wk = ""
        AddHandler oHelper.dt.RowChanged, AddressOf RowChanged
        AddHandler dgPlayers.CellValidating, AddressOf dgPlayers_CellValidating
    End Sub

    Private Sub Players_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If changes.Count > 0 Then
            Using workbook As New XLWorkbook($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
                ' Add sheets to the workbook
                workbook.Worksheets.Delete(oHelper.dt.TableName)
                workbook.Worksheets.Add(oHelper.dt, oHelper.dt.TableName)
                ' Save the workbook to a file
                workbook.SaveAs($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            End Using
        End If
    End Sub

    Private Sub RowChanged(ByVal sender As Object, ByVal e As DataRowChangeEventArgs)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim table As DataTable = CType(sender, DataTable)
        Dim row As DataRow = e.Row
        Dim snew As String = ""
        Select Case e.Action
            Case DataRowAction.Add
            Case DataRowAction.Change
                Dim sold As String = ""
                changes = New List(Of String)
                ' Iterate through each column to find the changed cell(s)
                For Each column As DataColumn In table.Columns
                    If Not row(column, DataRowVersion.Original).Equals(row(column, DataRowVersion.Current)) Then
                        sold = row(column, DataRowVersion.Original)
                        snew = row(column, DataRowVersion.Current)
                        changes.Add($"{column.ColumnName}: {row(column, DataRowVersion.Original)} -> {snew}")
                    End If
                Next

                If changes.Count > 0 Then
                    dp($"Row Changed: {e.Action} - Changes: {String.Join(", ", changes)}")
                Else
                    dp("No changes detected.")
                    Exit Sub
                End If
                sb = New Text.StringBuilder
                sb.AppendLine($"{String.Join(", ", changes)}
Press <Yes> to Update Players table
Press <No> to skip Update to Players table
")

                If MessageBox.Show(sb.ToString, "Player Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
                sb = New Text.StringBuilder
                sb.AppendLine($"DELETE FROM Players WHERE Player = '{row(Player)}'")
                oHelper.SqliteTrans(sb.ToString)
        End Select

        RemoveHandler oHelper.dt.RowChanged, AddressOf RowChanged
        sb = New Text.StringBuilder
        sb.AppendLine(
$"INSERT INTO Players (")
        For i = 0 To table.Columns.Count - 1
            Dim scomma = ""
            If i < table.Columns.Count - 1 Then
                scomma = ","
            End If
            sb.appendline($"{table.Columns(i).ColumnName}{scomma}")
            'i += 1
        Next

        sb.AppendLine($")
VALUES (")
        For i = 0 To table.Columns.Count - 1
            Dim scomma = ""
            If i < table.Columns.Count - 1 Then
                scomma = ","
            End If
            sb.appendline($"'{row(table.Columns(i).ColumnName)}'{scomma}")
        Next
        sb.AppendLine($")")
        oHelper.SqliteTrans(sb.ToString)
        AddHandler oHelper.dt.RowChanged, AddressOf RowChanged


        oHelper.dt = oHelper.sqliteda(ctx.Conn, "players")
        dgPlayers.Refresh()
    End Sub
    Sub dp(message As String)
        Debug.Print(message)
    End Sub

    Private Sub dgPlayers_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgPlayers.CellEndEdit
        dp($"{changes.Count} Changes made to table")
        If dgPlayers.Columns(e.ColumnIndex).Name = Player Then
            Dim textInfo As TextInfo = CultureInfo.CurrentCulture.TextInfo
            dgPlayers.Rows(e.RowIndex).Cells(Player).Value = textInfo.ToTitleCase(dgPlayers.Rows(e.RowIndex).Cells(Player).Value.ToLower()) ' Convert to title case
            dgPlayers.Rows(e.RowIndex).Cells("FirstName").Value = dgPlayers.Rows(e.RowIndex).Cells(Player).Value.ToString.Split(" ")(0)
            dgPlayers.Rows(e.RowIndex).Cells("LastName").Value = dgPlayers.Rows(e.RowIndex).Cells(Player).Value.ToString.Split(" ")(1)
        End If
    End Sub
    Private Function IsValidEmail(email As String) As Boolean
        Dim pattern As String = "^[^@\s]+@[^@\s]+\.[^@\s]+$"
        Return Regex.IsMatch(email, pattern)
    End Function
    Private Function IsValidPhoneNumber(phoneNumber As String) As Boolean
        ' Define a regular expression pattern for valid phone numbers
        'Dim pattern As String = "^\(\d{3}\) \d{3}-\d{4}$"  ' Example: (123) 456-7890
        Dim pattern As String = "^\d{3}-\d{3}-\d{4}$"  ' Example: (123) 456-7890
        Return Regex.IsMatch(phoneNumber, pattern)
    End Function

    Private Sub dgPlayers_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)
        If dgPlayers.Columns(e.ColumnIndex).Name = "Email" Then ' Replace with your column name
            Dim email As String = e.FormattedValue.ToString()
            If email = "" Then Exit Sub
            If Not IsValidEmail(email) Then
                MessageBox.Show("Please enter a valid email address.")
                'sender.rows(e.RowIndex).cells("Email").Value = ""
                For Each row As DataRow In oHelper.dt.Rows
                    If row(Constants.Player) = dgPlayers.Rows(e.RowIndex).Cells(Constants.Player).Value Then
                        'dgPlayers.CurrentCell.Value = sOldCellValue
                        RemoveHandler oHelper.dt.RowChanged, AddressOf RowChanged
                        row("Email") = sOldCellValue
                        AddHandler oHelper.dt.RowChanged, AddressOf RowChanged
                        dgPlayers.Refresh()
                        'MessageBox.Show($"Value '{searchValue}' found in Row {row.Index + 1}, Column {cell.ColumnIndex + 1}.")
                        Exit Sub
                    End If
                Next
                e.Cancel = True
            End If
            ' Replace "PhoneColumnName" with the actual column name for phone numbers
        ElseIf dgPlayers.Columns(e.ColumnIndex).Name = "Phone" Then
            Dim phoneNumber As String = e.FormattedValue.ToString()
            If phoneNumber.Replace("-", "").Replace(" ", "") <> "" Then
                If Not IsValidPhoneNumber(phoneNumber) Then
                    MessageBox.Show("Please enter a valid phone number in the format 123-456-7890.")
                    e.Cancel = True ' Prevent the user from leaving the cell until a valid phone number is entered
                End If
            End If
        End If
    End Sub
    Private Sub dgPlayers_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgPlayers.CellBeginEdit
        Dim dgc As DataGridViewCell = sender.currentcell
        sOldCellValue = dgc.Value
        If dgPlayers.Columns(e.ColumnIndex).Name = Player Then
            'dp(e.FormattedValue)
        End If
    End Sub
    Public Sub CreateMaskField(ByVal dgv As DataGridView, sField As String)

        Dim index As Integer
        ' find the location of the column
        index = dgv.Columns.IndexOf(dgv.Columns(sField))
        ' remove the existing column
        dgv.Columns.RemoveAt(index)
        ' create a new custom column
        Dim dgvMaskedEdit As New DataGridViewMaskedEditColumn

        If sField.Contains("Phone") Then
            dgvMaskedEdit.Mask = "###-###-####"
            dgvMaskedEdit.ValueType = GetType(String)
        ElseIf IsNumeric(sField) Then
            dgvMaskedEdit.Mask = "#"      ' this mask will allow only numbers
            dgvMaskedEdit.ValueType = GetType(String)
            dgvMaskedEdit.Width = 40
        Else
            dgvMaskedEdit.Mask = "##"      ' this mask will allow only numbers
            dgvMaskedEdit.ValueType = GetType(String)
        End If
        dgvMaskedEdit.Name = sField
        dgvMaskedEdit.DataPropertyName = sField
        ' some more tweaking
        dgvMaskedEdit.SortMode = DataGridViewColumnSortMode.Automatic

        ' insert the new column at the same location
        dgv.Columns.Insert(index, dgvMaskedEdit)
        dgv.Visible = True

    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If tbPlayer.Text <> "" Then
            For Each row As DataGridViewRow In dgPlayers.Rows
                If row.Cells(Player).Value IsNot Nothing AndAlso row.Cells(Player).Value.ToString().ToLower.Contains(tbPlayer.Text.ToLower) Then
                    ' Set the current cell to the found cell
                    dgPlayers.CurrentCell = row.Cells(Player)
                    ' Scroll the DataGridView to the found cell
                    dgPlayers.FirstDisplayedScrollingRowIndex = row.Index
                    row.Cells(Player).Selected = True
                    dgPlayers.BeginEdit(True)
                    Exit For
                End If
            Next
        End If
    End Sub
End Class
Public Class DataGridViewMaskedEditColumn
    Inherits DataGridViewColumn

    Public Sub New()
        MyBase.New(New DataGridViewMaskedEditCell())
    End Sub

    Public Overrides Property CellTemplate() As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            If (value IsNot Nothing) AndAlso Not TypeOf value Is DataGridViewMaskedEditCell Then
                Throw New InvalidCastException("Must be a DataGridViewMaskedEditCell")
            End If
            MyBase.CellTemplate = value
        End Set
    End Property

    Public Property Mask As String
        Get
            Dim cell As DataGridViewMaskedEditCell = CType(Me.CellTemplate, DataGridViewMaskedEditCell)
            Return cell.Mask
        End Get
        Set(value As String)
            If Me.CellTemplate Is Nothing Then
                Throw New InvalidOperationException("CellTemplate must be initialized")
            End If

            Dim cell As DataGridViewMaskedEditCell = CType(Me.CellTemplate, DataGridViewMaskedEditCell)
            cell.Mask = value

            If DataGridView IsNot Nothing Then
                Dim rowCount As Integer = DataGridView.Rows.Count
                For i As Integer = 0 To rowCount - 1
                    Dim row As DataGridViewRow = DataGridView.Rows.SharedRow(i)
                    Dim dataGridViewCell As DataGridViewMaskedEditCell = TryCast(row.Cells(Me.Index), DataGridViewMaskedEditCell)
                    If dataGridViewCell IsNot Nothing Then
                        dataGridViewCell.Mask = value
                    End If
                Next
            End If
        End Set
    End Property
End Class
Public Class DataGridViewMaskedEditCell
    Inherits DataGridViewTextBoxCell

    Public Property Mask As String

    Public Sub New()
        MyBase.New()
        Me.Mask = ""
    End Sub

    Public Overrides Function Clone() As Object
        Dim cell As DataGridViewMaskedEditCell = CType(MyBase.Clone(), DataGridViewMaskedEditCell)
        cell.Mask = Me.Mask
        Return cell
    End Function

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)
        Dim ctl As MaskedEditingControl = CType(DataGridView.EditingControl, MaskedEditingControl)
        ctl.Mask = Me.Mask
    End Sub

    Public Overrides ReadOnly Property EditType() As Type
        Get
            Return GetType(MaskedEditingControl)
        End Get
    End Property

    Public Overrides ReadOnly Property ValueType() As Type
        Get
            Return GetType(String)
        End Get
    End Property
End Class
Public Class MaskedEditingControl
    Inherits MaskedTextBox
    Implements IDataGridViewEditingControl

    Private dataGridViewControl As DataGridView
    Private valueIsChanged As Boolean = False
    Private rowIndexNum As Integer

    Public Sub New()
        Me.TabStop = False
    End Sub

    Public Property EditingControlFormattedValue As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return Me.Text
        End Get
        Set(value As Object)
            Me.Text = CType(value, String)
        End Set
    End Property

    Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return Me.Text
    End Function

    Public Sub ApplyCellStyleToEditingControl(cellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        Me.Font = cellStyle.Font
        Me.ForeColor = cellStyle.ForeColor
        Me.BackColor = cellStyle.BackColor
    End Sub

    Public Property EditingControlRowIndex As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return rowIndexNum
        End Get
        Set(value As Integer)
            rowIndexNum = value
        End Set
    End Property

    Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
        If (keyData And Keys.KeyCode) = Keys.Left OrElse
           (keyData And Keys.KeyCode) = Keys.Right OrElse
           (keyData And Keys.KeyCode) = Keys.Up OrElse
           (keyData And Keys.KeyCode) = Keys.Down OrElse
           (keyData And Keys.KeyCode) = Keys.Home OrElse
           (keyData And Keys.KeyCode) = Keys.End OrElse
           (keyData And Keys.KeyCode) = Keys.PageDown OrElse
           (keyData And Keys.KeyCode) = Keys.PageUp Then
            Return True
        End If
        Return Not dataGridViewWantsInputKey
    End Function

    Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
        If selectAll Then
            Me.SelectAll()
        Else
            Me.SelectionStart = Me.Text.Length
        End If
    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

    Public Property EditingControlDataGridView As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return dataGridViewControl
        End Get
        Set(value As DataGridView)
            dataGridViewControl = value
        End Set
    End Property

    Public Property EditingControlValueChanged As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return valueIsChanged
        End Get
        Set(value As Boolean)
            valueIsChanged = value
            Me.EditingControlDataGridView.NotifyCurrentCellDirty(value)
        End Set
    End Property

    Public ReadOnly Property EditingPanelCursor As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return Me.Cursor
        End Get
    End Property

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        Me.EditingControlValueChanged = True
    End Sub
End Class


