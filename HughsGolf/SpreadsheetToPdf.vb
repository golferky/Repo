Imports System
Imports System.IO
Imports ClosedXML.Excel
Imports iText.Kernel.Pdf
Imports iText.Layout
Imports iText.Layout.Element

Public Class SpreadsheetToPdf
    Public Shared Sub ExportToPdf(spreadsheetPath As String, pdfPath As String)
        ' Open the spreadsheet
        Using workbook As New XLWorkbook(spreadsheetPath)
            Dim worksheet As IXLWorksheet = workbook.Worksheet(1)

            ' Create a new PDF document
            Using pdfWriter As New PdfWriter(pdfPath)
                Using pdfDocument As New PdfDocument(pdfWriter)
                    Dim document As New Document(pdfDocument)

                    ' Add header row
                    Dim headerRow As New Paragraph()
                    For Each cell In worksheet.Row(1).CellsUsed()
                        headerRow.Add(New Text(cell.GetValue(Of String)() & vbTab))
                    Next
                    document.Add(headerRow)

                    ' Add data rows
                    For Each row In worksheet.RowsUsed().Skip(1)
                        Dim dataRow As New Paragraph()
                        For Each cell In row.CellsUsed()
                            dataRow.Add(New Text(cell.GetValue(Of String)() & vbTab))
                        Next
                        document.Add(dataRow)
                    Next

                    document.Close()
                End Using
            End Using
        End Using
    End Sub
End Class
