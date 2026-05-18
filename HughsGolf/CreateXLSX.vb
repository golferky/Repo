Imports System.Data.SQLite
Imports ClosedXML.Excel
Imports HughsGolf.Constants
Public Class CreateXLSX
    Dim connection As SQLiteConnection
    Dim ohelper As New HughsGolf.Helper
    'Main not used
    Sub Main()
        ' Create and populate DataTables
        Dim dataTable1 As New DataTable("Sheet1")
        dataTable1.Columns.Add("ID", GetType(Integer))
        dataTable1.Columns.Add("Name", GetType(String))
        dataTable1.Rows.Add(1, "John Doe")
        dataTable1.Rows.Add(2, "Jane Smith")

        Dim dataTable2 As New DataTable("Sheet2")
        dataTable2.Columns.Add("ID", GetType(Integer))
        dataTable2.Columns.Add("Country", GetType(String))
        dataTable2.Rows.Add(1, "USA")
        dataTable2.Rows.Add(2, "Canada")

        ' Create a new Excel workbook
        Using workbook As New XLWorkbook()
            ' Add sheets to the workbook
            workbook.Worksheets.Add(dataTable1, "Sheet1")
            workbook.Worksheets.Add(dataTable2, "Sheet2")

            ' Save the workbook to a file
            workbook.SaveAs("path\to\your\file.xlsx")
        End Using

        Console.WriteLine("Excel file created successfully with multiple sheets.")
    End Sub
    Sub CreateFromCSV()
        ' Create and populate DataTables
        Using connection As New SQLiteConnection(ctx.Conn.ConnectionString)
            connection.Open()
            Dim oFiles() As IO.FileInfo
            'Dim oDirectory As New IO.DirectoryInfo(HughsGolf.Main.csvFilePath)
            Dim oDirectory As New IO.DirectoryInfo("C:\Golf\Files")
            oFiles = oDirectory.GetFiles("*.csv")
            Helper.arraySort(oFiles)
            Helper.dsLeague = New DataSet
            For Each file In oFiles
                If Not file.Name = "Scores.csv" Then Continue For
                'If file.Name.StartsWith("2024_") Then Continue For
                Dim dt As New DataTable($"{file.Name.ToString.Substring(0, file.Name.IndexOf(".csv"))}")
                If dt.TableName = "LeagueParms" Then
                    Debug.Print("")
                End If
                ohelper.SQLiteCreateTable(connection, $"{dt.TableName}.csv")

                'dt = ohelper.sqliteda(connection, dt.TableName)
                'Using command As New SQLiteCommand($"SELECT * FROM LeagueParms Where SUBSTRING(FORMAT(StartDate, 'yyyyMMdd'),1,4) = '{cbSeasons.SelectedItem}'", connection)
                'If dt.TableName = "Scores" Then
                '    Using command As New SQLiteCommand($"SELECT * FROM {dt.TableName} ", connection)
                '        Using reader As SQLiteDataReader = command.ExecuteReader()
                '            ' Display the selected records
                '            While reader.Read()
                '                Debug.Print($"Table:{dt.TableName}")
                '                For i As Integer = 0 To reader.FieldCount - 1
                '                    Dim fieldName As String = reader.GetName(i)
                '                    Dim fieldValue As Object = reader.GetValue(i)
                '                    Dim fieldType As Object = reader.GetFieldType(i)
                '                    Debug.Print($"  {fieldName}: {fieldValue} : {fieldType}")
                '                Next
                '            End While
                '        End Using
                '    End Using

                'End If
                'dt = ohelper.sqliteda(connection, dt.TableName)

                'ohelper.dsLeague.Tables.Add(dt)
            Next
            ohelper.CreateWorkbookFromSQLite(connection)
            Console.WriteLine("Excel file created successfully with multiple sheets.")
        End Using
    End Sub
    Sub AddToWBFromCSV()
        ' Create and populate DataTables
        Using connection As New SQLiteConnection(ctx.Conn.ConnectionString)
            connection.Open()
            Dim sFileName As String = ohelper.GetFile
            Dim sTableName As String = sFileName '.Substring(sFileName.LastIndexOf("\") + 1)
            Dim dt As New DataTable($"{sTableName}")
            ohelper.SQLiteCreateTable(connection, $"{dt.TableName}")
            'For Each col As DataColumn In dt.Columns 'oHelper.dsLeague.Tables("Teams").Columns
            '    Debug.Print($"Main {dt.TableName}-{col.ColumnName}-{col.DataType}")
            'Next

            sTableName = $"{sTableName.Substring(sTableName.LastIndexOf("\") + 1)}"
            sTableName = sTableName.Substring(0, sTableName.LastIndexOf("."))
            dt = ohelper.sqliteda(connection, sTableName)

            'dt = ohelper.sqliteda(connection, dt.TableName)
            'Using command As New SQLiteCommand($"Select * FROM LeagueParms Where SUBSTRING(FORMAT(StartDate, 'yyyyMMdd'),1,4) = '{cbSeasons.SelectedItem}'", connection)
            'If dt.TableName = "Scores" Then
            '    Using command As New SQLiteCommand($"SELECT * FROM {dt.TableName} ", connection)
            '        Using reader As SQLiteDataReader = command.ExecuteReader()
            '            ' Display the selected records
            '            While reader.Read()
            '                Debug.Print($"Table:{dt.TableName}")
            '                For i As Integer = 0 To reader.FieldCount - 1
            '                    Dim fieldName As String = reader.GetName(i)
            '                    Dim fieldValue As Object = reader.GetValue(i)
            '                    Dim fieldType As Object = reader.GetFieldType(i)
            '                    Debug.Print($"  {fieldName}: {fieldValue} : {fieldType}")
            '                Next
            '            End While
            '        End Using
            '    End Using

            'End If
            'dt = ohelper.sqliteda(connection, dt.TableName)

            'ohelper.dsLeague.Tables.Add(dt)
            ' Create a new Excel workbook
            Using workbook As New XLWorkbook($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
                ' Add sheets to the workbook
                workbook.Worksheets.Add(dt, dt.TableName)
                '' Save the workbook to a file
                workbook.SaveAs($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            End Using

            Console.WriteLine("Excel file created successfully with multiple sheets.")
        End Using
    End Sub

End Class
