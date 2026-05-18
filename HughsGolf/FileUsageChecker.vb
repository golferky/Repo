Imports System.Diagnostics
Imports System.IO
Imports System.Text.RegularExpressions

Public Class FileUsageChecker
    Private Shared HandleUtilityPath As String = $"{ctx.csvFilePath}handle.exe" ' Update with the actual path

    Public Shared Function GetFileUsage(filePath As String) As List(Of String)
        Dim output As New List(Of String)()

        Try
            ' Create a new process to run the handle utility
            Dim process As New Process()
            process.StartInfo.FileName = HandleUtilityPath
            process.StartInfo.Arguments = $"""{filePath}"""
            process.StartInfo.UseShellExecute = False
            process.StartInfo.RedirectStandardOutput = True
            process.StartInfo.CreateNoWindow = True

            ' Start the process and read the output
            process.Start()
            Dim reader As StreamReader = process.StandardOutput
            Dim line As String = reader.ReadLine()

            ' Read and store the output lines
            While line IsNot Nothing
                output.Add(line)
                line = reader.ReadLine()
            End While

            process.WaitForExit()
        Catch ex As Exception
            Console.WriteLine("Error: " & ex.Message)
        End Try

        Return ParseHandleOutput(output)
    End Function

    Private Shared Function ParseHandleOutput(output As List(Of String)) As List(Of String)
        Dim result As New List(Of String)()
        Dim regex As New Regex("\s*(\S+)\s+pid:\s*(\d+)\s*type:\s*\S+\s*\w+:\s*(.+)")

        ' Skip the first few lines which are just headers or other info
        For i As Integer = 4 To output.Count - 1
            Dim line As String = output(i)
            If Not String.IsNullOrEmpty(line) Then
                Dim match As Match = regex.Match(line)
                If match.Success Then
                    Dim processName As String = match.Groups(1).Value
                    Dim pid As String = match.Groups(2).Value
                    Dim fileName As String = match.Groups(3).Value
                    result.Add($"Process Name: {processName}, PID: {pid}, File: {fileName}")
                End If
            End If
        Next

        Return result
    End Function
End Class
