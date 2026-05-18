Imports ClosedXML.Excel
Imports System.IO

Module modReporting

    ''' <summary>
    ''' Exports DataGridView to a formatted Excel file using ClosedXML.
    ''' </summary>
    Public Function CreateExcelFromGrid(dgv As DataGridView, fileName As String) As String
        Dim fullPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName & ".xlsx")

        Try
            Using workbook As New XLWorkbook()
                Dim worksheet = workbook.Worksheets.Add("League Schedule")

                ' 1. Create Headers (Dates)
                For colIndex As Integer = 0 To dgv.ColumnCount - 1
                    Dim cell = worksheet.Cell(1, colIndex + 1)
                    cell.Value = dgv.Columns(colIndex).HeaderText
                    cell.Style.Font.Bold = True
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                Next

                ' 2. Transfer Data and Colors
                For rowIndex As Integer = 0 To dgv.Rows.Count - 1
                    If Not dgv.Rows(rowIndex).IsNewRow Then
                        For colIndex As Integer = 0 To dgv.ColumnCount - 1
                            Dim dgvCell = dgv.Rows(rowIndex).Cells(colIndex)
                            Dim xlCell = worksheet.Cell(rowIndex + 2, colIndex + 1)

                            xlCell.Value = dgvCell.Value?.ToString()
                            xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                            ' Map System.Drawing.Color to XLColor
                            Dim bColor As Color = dgvCell.InheritedStyle.BackColor

                            ' Only apply background if it's not the default white
                            If bColor <> Color.White AndAlso bColor.Name <> "0" Then
                                xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(bColor)
                            End If

                            ' Handle White text for the blue milestone columns
                            If dgvCell.InheritedStyle.ForeColor = Color.White Then
                                xlCell.Style.Font.FontColor = XLColor.White
                            End If
                        Next
                    End If
                Next

                ' 3. Final Formatting
                worksheet.Columns().AdjustToContents() ' Auto-fit columns
                workbook.SaveAs(fullPath)
            End Using

            Return fullPath

        Catch ex As Exception
            MsgBox("Excel Error: " & ex.Message)
            Return ""
        End Try
    End Function

End Module