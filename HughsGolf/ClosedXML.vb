Imports System
Imports System.Data
Imports System.IO
Imports ClosedXML.Excel
Imports MailKit

Class ClosedXML
    Public Sub ExportDataGridViewToExcel(ByVal dataGridView As DataGridView, ByVal filePath As String, columnsToExclude As List(Of String))
        Using workbook As New XLWorkbook()
            dataGridView.DefaultCellStyle.BackColor = Color.White

            Dim worksheet = workbook.Worksheets.Add("Sheet1")
            worksheet.ShowGridLines = True ' Enable gridlines

            ' List of columns to exclude
            Dim excelColIndex As Integer = 1 ' Track correct column position in Excel

            ' Add headers while skipping excluded columns
            For col As Integer = 0 To dataGridView.Columns.Count - 1
                Dim headerText As String = dataGridView.Columns(col).HeaderText.Trim()

                ' Skip excluded columns
                If columnsToExclude.Contains(headerText) Then Continue For

                Dim cell = worksheet.Cell(1, excelColIndex)
                cell.Value = headerText
                cell.Style.Alignment.WrapText = True
                cell.Style.Font.Bold = True
                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)

                excelColIndex += 1 ' Only increment index for included columns
            Next

            ' Add rows while skipping excluded columns
            For row As Integer = 0 To dataGridView.Rows.Count - 1
                ' Reset index for row processing
                excelColIndex = 1
                Dim isBlueRow As Boolean = False
                If dataGridView.Name.ToLower.Contains("scores") Then
                    dataGridView.Invalidate()
                    isBlueRow = (row \ 4) Mod 2 = 0 ' Apply light blue every 4 rows
                End If
                For col As Integer = 0 To dataGridView.Columns.Count - 1
                    If columnsToExclude.Contains(dataGridView.Columns(col).HeaderText.Trim()) Then Continue For ' Skip excluded columns

                    Dim dgvCell = dataGridView.Rows(row).Cells(col)
                    Dim cell = worksheet.Cell(row + 2, excelColIndex) ' Use adjusted index

                    ' Preserve cell values and formatting
                    cell.Value = dgvCell.Value?.ToString()
                    Dim cellValue As String = dgvCell.Value?.ToString()
                    cell.Style.Alignment.WrapText = True

                    'Dim hasGreenDot As Boolean = dgvCell.Style.BackColor = Color.Green ' Or use
                    'Debug.Print($"{dataGridView.Rows(row).Cells(col).OwningColumn.Name}-{dgvCell.Tag.ToString}")
                    If dgvCell.Tag IsNot Nothing AndAlso dgvCell.Tag.ToString() = "Stroke" Then
                        cell.Value = $"{cellValue} •" ' Append green dot
                        cell.Style.Font.FontColor = XLColor.Green
                    ElseIf dgvCell.Tag IsNot Nothing AndAlso dgvCell.Tag.ToString() = "Double Stroke" Then
                        cell.Value = $"{cellValue} ••" ' Append 2 green dot
                        cell.Style.Font.FontColor = XLColor.Green
                    End If
                    ' Preserve text color
                    Dim fontColor As Color = dgvCell.Style.ForeColor
                    If fontColor <> Color.Empty Then
                        cell.Style.Font.FontColor = XLColor.FromColor(fontColor)
                    End If

                    ' Preserve font styles (bold, italic, underline)
                    If dgvCell.Style.Font IsNot Nothing Then
                        If dgvCell.Style.Font.Bold Then cell.Style.Font.Bold = True
                        If dgvCell.Style.Font.Italic Then cell.Style.Font.Italic = True
                        If dgvCell.Style.Font.Underline Then cell.Style.Font.Underline = XLFontUnderlineValues.Single
                    End If

                    ' Preserve text alignment
                    Select Case dgvCell.Style.Alignment
                        Case DataGridViewContentAlignment.MiddleCenter
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                        Case DataGridViewContentAlignment.MiddleRight
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right
                        Case Else
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left
                    End Select

                    ' Apply light blue every 4 rows ONLY if the original cell color is white or default
                    Dim bgColor As Color = dgvCell.Style.BackColor
                    If bgColor = Color.Empty Then bgColor = Color.White ' Default to White if Empty

                    If isBlueRow AndAlso (bgColor = Color.White Or bgColor = SystemColors.Control) Then
                        cell.Style.Fill.BackgroundColor = XLColor.LightBlue
                    ElseIf bgColor <> Color.Empty AndAlso bgColor <> SystemColors.Control Then
                        cell.Style.Fill.BackgroundColor = XLColor.FromColor(bgColor) ' Preserve existing color
                    End If

                    'Debug.WriteLine($"Row {row} | isBlueRow={isBlueRow} | bgColor={bgColor}")
                    ' Apply borders for visible gridlines
                    cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)

                    excelColIndex += 1 ' Move only when column is included
                Next

            Next

            ' Auto-size included columns
            excelColIndex = 1 ' Reset index for column sizing
            For col As Integer = 0 To dataGridView.Columns.Count - 1
                Dim headerText As String = dataGridView.Columns(col).HeaderText.Trim()

                ' Skip excluded columns
                If columnsToExclude.Contains(headerText) Then Continue For

                Dim cell = worksheet.Cell(1, excelColIndex)
                cell.Value = headerText
                cell.Style.Alignment.WrapText = True
                cell.Style.Font.Bold = True
                cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)

                ' Find longest word in the header for width adjustment
                Dim longestHeaderPart As String = headerText.Split(" "c).OrderByDescending(Function(s) s.Length).FirstOrDefault()
                Dim headerLength As Double = longestHeaderPart.Length * 1.2 ' Slightly increase for spacing

                ' Find longest cell content in this column
                Dim maxCellLength As Double = 0
                For row As Integer = 0 To dataGridView.Rows.Count - 1
                    Dim cellContent As String = dataGridView.Rows(row).Cells(col).Value?.ToString()
                    If Not String.IsNullOrEmpty(cellContent) Then
                        maxCellLength = Math.Max(maxCellLength, cellContent.Length * 1.2) ' Adjusted for spacing
                    End If
                Next

                ' Set column width to the greater of header or cell content
                worksheet.Column(excelColIndex).Width = Math.Max(headerLength, maxCellLength)
                If headerText.Contains("Date") Then worksheet.Column(excelColIndex).Width = 10
                excelColIndex += 1 ' Move only when column is included
            Next
            If dataGridView.Name.ToLower.Contains("schedule") Then
                ' Get the last used row
                Dim lastUsedRow As Integer = worksheet.LastRowUsed().RowNumber()

                For row As Integer = 0 To ScheduleBuilder.dgTeams.Rows.Count - 1
                    excelColIndex = 1 ' Reset index for schedule processing
                    For col As Integer = 0 To ScheduleBuilder.dgTeams.Columns.Count - 1

                        Dim dgvCell = ScheduleBuilder.dgTeams.Rows(row).Cells(col)
                        Dim cell = worksheet.Cell(row + lastUsedRow + 2, excelColIndex) ' Use adjusted index

                        ' Preserve cell values and formatting
                        cell.Value = dgvCell.Value?.ToString()
                        cell.Style.Alignment.WrapText = True

                        ' Preserve text color
                        Dim fontColor As Color = dgvCell.Style.ForeColor
                        If fontColor <> Color.Empty Then
                            cell.Style.Font.FontColor = XLColor.FromColor(fontColor)
                        End If

                        ' Preserve font styles (bold, italic, underline)
                        If dgvCell.Style.Font IsNot Nothing Then
                            If dgvCell.Style.Font.Bold Then cell.Style.Font.Bold = True
                            If dgvCell.Style.Font.Italic Then cell.Style.Font.Italic = True
                            If dgvCell.Style.Font.Underline Then cell.Style.Font.Underline = XLFontUnderlineValues.Single
                        End If

                        ' Preserve text alignment
                        Select Case dgvCell.Style.Alignment
                            Case DataGridViewContentAlignment.MiddleCenter
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                            Case DataGridViewContentAlignment.MiddleRight
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right
                            Case Else
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left
                        End Select

                        ' Apply borders for visible gridlines
                        cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)

                        excelColIndex += 1 ' Move only when column is included
                    Next
                    Dim wk = ""
                Next

            End If
            ' Set uniform row height
            worksheet.Rows().Height = 15
            worksheet.Rows(1).Height = 30
            ' Save file
            workbook.SaveAs(filePath)
        End Using
    End Sub

    Function LoadCsvIntoDataTable(filePath As String) As DataTable
        Dim dataTable As New DataTable()
        Using reader As New StreamReader(filePath)
            Dim headerLine As String = reader.ReadLine()
            If headerLine IsNot Nothing Then
                Dim headers As String() = headerLine.Split(","c)
                For Each header As String In headers
                    dataTable.Columns.Add(New DataColumn(header))
                Next

                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    If line IsNot Nothing Then
                        Dim values As String() = line.Split(","c)
                        dataTable.Rows.Add(values)
                    End If
                End While
            End If
        End Using
        Return dataTable
    End Function

    Sub ExportDataTableToExcel(dataTable As DataTable, filePath As String)
        Using workbook As New XLWorkbook()
            Dim worksheet As IXLWorksheet = workbook.Worksheets.Add(dataTable, "Sheet1")
            workbook.SaveAs(filePath)
        End Using
    End Sub
    Sub XLS2CSV(xcelfile As String, csvfile As String)
        Dim excelFilePath As String = xcelfile
        Dim csvFilePath As String = csvfile

        ' Read the Excel file
        Using workbook As New XLWorkbook(excelFilePath)
            Dim worksheet As IXLWorksheet = workbook.Worksheet(1)
            Dim dataTable As New DataTable()

            ' Read the header
            Dim firstRow As Boolean = True
            For Each row As IXLRow In worksheet.RowsUsed()
                If firstRow Then
                    For Each cell As IXLCell In row.Cells()
                        dataTable.Columns.Add(cell.Value.ToString())
                    Next
                    firstRow = False
                Else
                    dataTable.Rows.Add()
                    Dim i As Integer = 0
                    For Each cell As IXLCell In row.Cells()
                        dataTable.Rows(dataTable.Rows.Count - 1)(i) = cell.Value.ToString()
                        i += 1
                    Next
                End If
            Next

            ' Write the data to a CSV file
            Using writer As New StreamWriter(csvFilePath)
                For Each column As DataColumn In dataTable.Columns
                    writer.Write(column.ColumnName & ",")
                Next
                writer.WriteLine()

                For Each row As DataRow In dataTable.Rows
                    For Each item As Object In row.ItemArray
                        writer.Write(item.ToString() & ",")
                    Next
                    writer.WriteLine()
                Next
            End Using
        End Using

        Console.WriteLine("CSV file created successfully.")
    End Sub
    Function CreateDataTablesFromWorksheets(workbook As XLWorkbook) As Dictionary(Of String, DataTable)
        Dim dataTables As New Dictionary(Of String, DataTable)()

        ' Iterate through each worksheet in the workbook
        For Each worksheet As IXLWorksheet In workbook.Worksheets
            Dim dataTable As New DataTable(worksheet.Name)
            'If worksheet.Name <> "LeagueParms" Then Continue For
            Dim firstRow As Boolean = True
#Region "bad loop"
            '' Add columns to the DataTable
            'For Each row As IXLRow In worksheet.RowsUsed()
            '    If firstRow Then
            '        For Each cell As IXLCell In row.Cells()
            '            dataTable.Columns.Add(cell.Value.ToString())
            '        Next
            '        firstRow = False
            '    Else
            '        ' Add rows to the DataTable
            '        Dim dataRow As DataRow = dataTable.NewRow()
            '        Dim i As Integer = 0
            '        Debug.Print($"{dataTable.Columns.Count} column headers {row.Cells().Count} {row.CellsUsed().Count}")
            '        For Each cell As IXLCell In row.CellsUsed()
            '            dataRow(i) = cell.Value.ToString()
            '            i += 1
            '        Next
            '        dataTable.Rows.Add(dataRow)
            '    End If
            'Next
#End Region
#Region "good loop"
            For Each row As IXLRow In worksheet.RowsUsed()
                If firstRow Then
                    For Each cell As IXLCell In row.CellsUsed()
                        'dataTable.Columns.Add(cell.WorksheetColumn().ColumnLetter())
                        dataTable.Columns.Add(cell.Value.ToString())
                    Next
                    firstRow = False
                Else
                    Dim newRow As DataRow = dataTable.NewRow()
                    For i As Integer = 0 To dataTable.Columns.Count - 1
                        Dim cell As IXLCell = row.Cell(i + 1)
                        Dim cellValue As String = If(cell.IsEmpty(), "", cell.GetString())
                        newRow(i) = cellValue
                    Next
                    dataTable.Rows.Add(newRow)
                End If
            Next
#End Region

            ' Add the DataTable to the dictionary
            dataTables.Add(worksheet.Name, dataTable)
        Next


        Return dataTables
    End Function


End Class

