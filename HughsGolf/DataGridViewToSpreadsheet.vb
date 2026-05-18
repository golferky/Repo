Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports ClosedXML.Excel

Public Class DataGridViewToSpreadsheet
    Public Shared Sub ExportToSpreadsheet(dataGridView As DataGridView, filePath As String)
        ' Create a new workbook and worksheet
        Dim workbook As New XLWorkbook()
        Dim worksheet As IXLWorksheet = workbook.Worksheets.Add("Sheet1")

        ' Add header row and apply styles
        For col As Integer = 0 To dataGridView.Columns.Count - 1
            Dim cell As IXLCell = worksheet.Cell(1, col + 1)
            cell.Value = dataGridView.Columns(col).HeaderText
            ApplyCellStyle(cell, dataGridView.ColumnHeadersDefaultCellStyle)
        Next

        ' Add data rows and apply styles
        For row As Integer = 0 To dataGridView.Rows.Count - 1
            For col As Integer = 0 To dataGridView.Columns.Count - 1
                Dim cell As IXLCell = worksheet.Cell(row + 2, col + 1)
                cell.Value = If(dataGridView.Rows(row).Cells(col).Value?.ToString(), String.Empty)
                ApplyCellStyle(cell, dataGridView.Rows(row).Cells(col).Style)
            Next
        Next

        ' Save the workbook to the specified file path
        workbook.SaveAs(filePath)
    End Sub

    Private Shared Sub ApplyCellStyle(cell As IXLCell, style As DataGridViewCellStyle)
        ' Set font style
        If style.Font IsNot Nothing Then
            cell.Style.Font.Bold = style.Font.Bold
            cell.Style.Font.Italic = style.Font.Italic
            cell.Style.Font.Underline = style.Font.Underline

            ' Set font size
            cell.Style.Font.FontSize = style.Font.Size
        End If

        ' Set font color
        If style.ForeColor <> Color.Empty Then
            cell.Style.Font.FontColor = XLColor.FromColor(style.ForeColor)
        End If

        ' Set background color
        If style.BackColor <> Color.Empty Then
            cell.Style.Fill.BackgroundColor = XLColor.FromColor(style.BackColor)
        End If

        ' Set text alignment
        Select Case style.Alignment
            Case DataGridViewContentAlignment.MiddleLeft
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left
            Case DataGridViewContentAlignment.MiddleCenter
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
            Case DataGridViewContentAlignment.MiddleRight
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right
        End Select
    End Sub
End Class
