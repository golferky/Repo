Public Class LeagueSetup
    Dim H As Helper
    Private Sub NewLeague_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        H = Main.oHelper
    End Sub
    Public Sub PasteData(ByRef dgv As DataGridView)
        Dim tArr() As String
        Dim arT() As String
        Dim i, ii As Integer
        Dim c, cc, r As Integer

        tArr = Clipboard.GetText().Split(Environment.NewLine)

        r = dgv.SelectedCells(0).RowIndex
        c = dgv.SelectedCells(0).ColumnIndex
        For i = 0 To tArr.Length - 1
            arT = tArr(i).Split(vbTab)
            cc = c
            For ii = 0 To arT.Length - 1
                With dgv.Item(cc, r)
                    .Value = arT(ii).TrimStart
                End With
                cc = cc + 1
            Next
            r = r + 1
        Next

    End Sub

    Private Sub dgvNL_MouseClick(sender As Object, e As MouseEventArgs) Handles dgvNL.MouseClick
        'If e.Button = Windows.Forms.MouseButtons.Right Then
        '    If sender.currentcell.value = "" Then
        '        PasteData(dgvNL)
        '    Else
        '        Clipboard.SetText(dgvNL(e.columnindex, e.rowindex))
        '    End If
        'End If
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        Dim sHolidays = H.getHolidayList(Now.Year)
        Dim sTuesHol As New List(Of Date)
        For Each sholiday In sHolidays
            If sholiday.DayOfWeek = DayOfWeek.Tuesday Then
                sTuesHol.Add(sholiday)
            End If
        Next

        Dim sLatestRow As DataRow = Nothing
        Dim sYear As String = "0000"
        For Each row As DataRow In H.dsLeague.Tables("dtLeagueParms").Rows
            If CDate(row("StartDate")).ToString("yyyyMMdd") > sYear Then
                sLatestRow = row
                sYear = CDate(row("StartDate")).ToString("yyyyMMdd")
                Continue For
            End If
        Next
        H.dsLeague.Tables("dtLeagueParms").ImportRow(sLatestRow)
        'Dim newrow = H.dsLeague.Tables("dtLeagueParms").NewRow
        ''newrow = sLatestRow
        'For Each cell As DataColumn In sLatestRow.Table.Columns
        '    newrow(cell.ColumnName) = cell
        '    'If
        '    'newrow("StartDate") = "01/01/2000"
        'Next
        'H.dsLeague.Tables("dtLeagueParms").Rows.Add(newrow)
        'dgvNL.DataSource = H.dsLeague.Tables("dtLeagueParms")
        'Dim sYear As String = "0000"
        'Dim rptr As Integer = 0
        'For Each row As DataGridViewRow In dgvNL.Rows
        '    If CDate(row.Cells("StartDate").Value).ToString("yyyyMMdd") > sYear Then
        '        sYear = CDate(row.Cells("StartDate").Value).ToString("yyyyMMdd").ToString.Substring(0, 4)
        '        rptr = row.Index
        '        Continue For
        '    End If
        'Next
        'For Each cell As DataGridViewCell In dgvNL.Rows(rptr).Cells
        '    dgvNL.Rows(dgvNL.RowCount - 1).Cells(cell.OwningColumn.Name).Value = cell.Value
        'Next


    End Sub

    Private Sub LeagueSetup_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        H.DataTable2CSV(H.dsLeague.Tables("dtLeagueParms"), H.sFilePath & "\LeagueParms.csv")
        H.LOGIT(String.Format("Updated {0}\LeagueParms.csv", H.sFilePath))

    End Sub
End Class