Imports System.Data.Entity.Migrations
Imports System.Data.SQLite
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Office2010.ExcelAc
Imports DocumentFormat.OpenXml.Vml.Office
'Imports DocumentFormat.OpenXml.Office2010.Excel

Imports HughsGolf.Constants
Imports HughsGolf.Main
Imports iText.Barcodes.Dmcode
Imports iText.IO.Codec.Brotli
Imports iText.Layout.Element
Imports System.Text.Json
Imports System.Net.Http
Public Class Helper
    Friend myprocessarray As New ArrayList
    Private myProcess As Process
    'Data related
    Public Shared dsLeague As DataSet
    Public dt As DataTable
    Public dtlo As DataTable
    Public dtWklySkins As DataTable
    Public Shared MyCourse() As Data.DataRow
    Public thisCourse As DataRow
    'Strings
    Public sArrayOfFiles As New List(Of String)
    Public iLast5Scores As List(Of String)
    Public sb As New StringBuilder

    'dates past the regular season dont get included in handicap calculations
    Public lignoreDates = New List(Of String)
    Public wk As String
    Public sexclDates As String
    Public sLeagueName As String
    Public sFilePath As String
    Public sReportPath As String
    Public sFileInUseMessage As String
    Public strsql As String = ""
    Public sFrontBack As String
    Public dDate As Date
    Public sCourse As String
    Public sTeam As String
    Public sPlayer As String
    Public sPlayerToFind As String
    Public sFindPlayerOption As String
    Public sTotalColumn As String = "*** Total ***"
    Public sMessage As String = ""
    Public sPSDate As String
    Public sPSDate2 As String

    'Integers
    Public iNumClosests As Integer = 0
    Public iLogitCounter As Int64
    Public iHoles As Integer
    Public iHoleMarker As Integer
    Public sGroupNumber As Integer
    Private _iHdcp As Integer
    'booleans
    Public bByeFound = False
    Public bCCLeague As Boolean = False
    Public bSwap9 As Boolean
    Public bload As Boolean
    Public bLockScores As Boolean
    Public bexit As Boolean = False
    Public bloghelper As Boolean = False
    Public bReorderCols As Boolean = False
    Public bDGSError As Boolean = False
    Public bNoRowLeave As Boolean = False
    Public bCalcSkins As Boolean = False
    Public bScoreCard As Boolean = False
    Public bScoresbyPlayer As Boolean = False
    Public bDots As Boolean = False
    Public bColors As Boolean = False
    Public bScreenChanged As Boolean = False
    Public bDateOverlap As Boolean = True
    Public bAllHolesEntered = False
    Public bsch = False
    Public bscores = False
    Public bplayer = False
    Public bcourses = False
    Public bpayments = False
    Public bPostSeason = False
    Public bCellPaint As Boolean = False
    Public bCellPaintStrokes As Boolean = False
    Public UnderParColor As Color = Color.OrangeRed
    Public PlayerForm As String
    Public Sub status_Msg(lbStatus As Label, frm As Form)
        LOGIT(lbStatus.Text)
        If lbStatus.Text.Contains("Finished") Then
            lbStatus.BackColor = Color.LightGreen
            'frm.Cursor = Cursors.Default
            Application.UseWaitCursor = False
        Else
            lbStatus.BackColor = Color.Red
            'frm.Cursor = Cursors.WaitCursor
            Application.UseWaitCursor = True
        End If
        Application.DoEvents()
    End Sub
    Public Sub SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs)
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        Try

            Dim c1 As Integer
            If e.CellValue1 Is DBNull.Value Then
                c1 = 0
            Else
                c1 = e.CellValue1
            End If
            Dim c2 As Integer
            If e.CellValue2 Is DBNull.Value Then
                c2 = 0
            Else
                c2 = e.CellValue2
            End If
            e.SortResult = If(CInt(c1) < CInt(c2), -1, 1)

            e.Handled = True
        Catch
            Dim x = ""
        End Try

    End Sub
    Public Sub SortCompare_Dec(sender As Object, e As DataGridViewSortCompareEventArgs)
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        Try

            Dim c1 As Decimal
            If e.CellValue1 Is DBNull.Value Then
                c1 = 0
            Else
                c1 = e.CellValue1
            End If
            Dim c2 As Decimal
            If e.CellValue2 Is DBNull.Value Then
                c2 = 0
            Else
                c2 = e.CellValue2
            End If
            e.SortResult = If(CDec(c1) < CDec(c2), -1, 1)

            e.Handled = True
        Catch
            Dim x = ""
        End Try

    End Sub
    Public Sub DataTable2CSV(ByVal table As DataTable, ByVal filename As String,
ByVal sepChar As String)
        Try
            'If table.TableName = "dtScores" Then
            '    LOGIT(String.Format("saving {0}", filename))
            'End If
            Dim writer As System.IO.StreamWriter = Nothing
            Try
                writer = New System.IO.StreamWriter(filename)
                ' first write a line with the columns name
                Dim sep As String = ""
                Dim builder As New System.Text.StringBuilder
                For Each col As DataColumn In table.Columns
                    builder.Append(sep).Append(col.ColumnName)
                    sep = sepChar
                Next
                writer.WriteLine(builder.ToString())
                ' then write all the rows
                For Each row As DataRow In table.Rows
                    If row.RowState <> DataRowState.Deleted Then
                        sep = ""
                        builder = New System.Text.StringBuilder
                        For Each col As DataColumn In table.Columns
                            builder.Append(sep).Append(row(col.ColumnName))
                            sep = sepChar
                        Next
                        writer.WriteLine(builder.ToString())
                    End If
                Next
            Finally
                If Not writer Is Nothing Then writer.Close()
            End Try

        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub DataTable2CSV(ByVal table As DataTable, ByVal filename As String)
        '6/2/2017 replace vbtab with , because vbtab quit working
        DataTable2CSV(table, filename, ",")
    End Sub
    Public Function CSV2DataTable(ByVal dt As DataTable, strFileName As String) As Boolean

        dt.Reset()
        dt.Dispose()
        CSV2DataTable = False
        Dim line As String, dlinecnt As Double
        Dim aRow As DataRow

        Try
            'LOGIT($"{strFileName} - file to format")
            Dim myStream As System.IO.StreamReader = New System.IO.StreamReader(strFileName)
            Do
                'If strFileName.Contains("Schedule") Then
                '    LOGIT("")
                'End If
                'read a line from the csv
                line = myStream.ReadLine()
                If line Is Nothing Then
                    Exit Do
                ElseIf line.Replace(",", "") = "" Then
                    Exit Do
                End If
                If line.Contains("20170411") Then Console.WriteLine(line)
                dlinecnt += 1
                If line Is Nothing Then Exit Do

                'build a string array of scores using comma delimited
                Dim sAry As String() = Split(line.Trim(","), ",")
                '2019-07-26 this causes issues on schedule because first week could be a rainout like 2018
                'If sAry(0).ToString = "" Then Continue Do
                'if this is the first line, it is a header so save each column header and mark the numeric ones 

                If dlinecnt = 1 Then
                    If dt.Columns.Count = 0 Then
                        For i = 0 To sAry.Count - 1
                            Dim dc = New DataColumn(sAry(i))
                            If sAry(i) = "Team" Or sAry(i).Contains("#") Or sAry(i).Contains("$") Then
                                dc.DataType = System.Type.GetType("System.Int16")
                            ElseIf sAry(i) = skin Or sAry(i) = closest Then
                                dc.DataType = System.Type.GetType("System.Boolean")
                            Else
                                dc.DataType = System.Type.GetType("System.String")
                            End If
                            'LOGIT($"field({sAry(i)}) - {dc.DataType}")
                            dt.Columns.Add(dc)
                        Next
                    End If
                    Continue Do
                End If

                aRow = dt.NewRow
                For i = 0 To sAry.Count - 1
                    If sAry(i).Trim <> "" Then
                        Try
                            aRow(i) = sAry(i)
                        Catch ex As Exception
                            Dim msg As New StringBuilder
                            msg.AppendLine(String.Format("Bad field in file-{0}", strFileName))
                            msg.AppendLine(String.Format("Player {0}", sAry(4)))
                            msg.AppendLine(String.Format("Date {0}", sAry(5)))
                            msg.AppendLine(String.Format("Row {0}", dlinecnt))
                            msg.AppendLine(String.Format("Column-{0}", dt.Columns(i).ColumnName))
                            msg.AppendLine(String.Format("Value {0}", sAry(i)))
                            aRow(i) = DBNull.Value
                            LOGIT(msg.ToString)
                            'MsgBox(msg.ToString)
                            'MsgBox(String.Format("bad field in file-{2}{4}Row {3}{4}Column-{0}{4}value {1}", dt.Columns(i).ColumnName, sAry(i), strFileName, dlinecnt, vbCrLf))
                            Debug.Flush()
                        End Try
                    Else
                        If strFileName.Contains("Payments") Then
                            aRow(i) = " "
                        End If

                    End If
                Next
                dt.Rows.Add(aRow)

                'Try
                '    dt.Rows.Add(aRow)
                'Catch ex As Exception
                '    If Debugger.IsAttached Then LOGIT("")

                'End Try
            Loop
            myStream.Close()
            CSV2DataTable = True

            If strFileName.Contains("Players") Or strFileName.Contains("Courses") Then
                dt.PrimaryKey = New DataColumn() {dt.Columns("Name")}
            ElseIf strFileName.Contains("Schedule") Then
                dt.PrimaryKey = New DataColumn() {dt.Columns(0)}
            ElseIf strFileName.Contains("Scores") Then
                dt.PrimaryKey = New DataColumn() {dt.Columns("Player"), dt.Columns("Date")}
            ElseIf strFileName.Contains("Payments") Then
                'Dim xdata As New List(Of String)
                'xdata = New List(Of String)
                'For Each row As DataRow In dt.Rows
                '    If xdata.Contains(row(Constants.Player) & row(Constants.datecon) & row("Desc") & row(Constants.Detail) & row(Constants.Comment)) Then
                '        logit($"Duplicate {row(Constants.Player) & row(Constants.datecon) & row("Desc") & row(Constants.Detail) & row(Constants.Comment)Then}")
                '    End If
                '    xdata.Add(row(Constants.Player) & row(Constants.datecon) & row("Desc") & row(Constants.Detail) & row(Constants.Comment))
                'Next

                dt.PrimaryKey = New DataColumn() {dt.Columns(Constants.Player), dt.Columns(Constants.datecon), dt.Columns(Constants.Desc), dt.Columns(Constants.Detail)}
            End If

        Catch ex As Exception
            If ex.Message.Contains("being used by another process") Then
                'Dim strFile As String = "c:\windows\system32\msi.dll"
                Dim a As ArrayList = getFilesInUse(strFileName)

                'For Each p As Process In‼ a
                '    If p.ProcessName.Contains("wps") Or p.ProcessName.Contains("excel") Or p.ProcessName.Contains("notepad") Then
                '        LOGIT(p.ProcessName)
                '    End If
                'Next

                sFileInUseMessage = ex.Message
                MsgBox(String.Format("file {0} In use, try later", strFileName))
            ElseIf ex.Message.Contains("These columns don't currently have unique values.") Then
                Dim dv = New DataView(dt)
                dv.Sort = "Name"
                Dim sprv = "", icnt = 0
                For Each row In dv
                    'If icnt = 0 Then sprv = row("Name")
                    If row("Name") <> sprv Then
                        sprv = row("Name")
                        Continue For
                    End If
                    MsgBox(String.Format("Duplicate Error in table {0} - {1}{2}Fix file{3} and try again", dt.TableName, sprv, vbCrLf, strFileName))
                    LOGIT(row("Name") & " - Dup")
                Next
            Else
                MsgBox(String.Format("Error {0} row {1}", strFileName, dlinecnt) & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            End If
            '    MsgBox(String.Format("file {0} In use, try later", strFileName))
            'End If
            'LOGIT("")
        Finally

        End Try

    End Function
    Sub dgv2csv(dgv As DataGridView, filename As String)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Dim csv = ""

        Try
            Dim shdr As String = ""

            Dim headers = (From header As DataGridViewColumn In dgv.Columns.Cast(Of DataGridViewColumn)()
                           Select header.HeaderText).ToArray
            Dim rows = From row As DataGridViewRow In dgv.Rows.Cast(Of DataGridViewRow)()
                       Where Not row.IsNewRow
                       Select Array.ConvertAll(row.Cells.Cast(Of DataGridViewCell).ToArray, Function(c) If(c.Value IsNot Nothing, RemoveSpcChar(c.Value.ToString), ""))
            Using sw As New IO.StreamWriter(filename)
                sw.WriteLine(filename.Substring(filename.LastIndexOf("\") + 1, (filename.LastIndexOf("_") - filename.LastIndexOf("\")) - 1))
                sw.WriteLine(String.Join(",", headers))
                For Each r In rows
                    sw.WriteLine(String.Join(",", r))
                Next
            End Using
            'opens the file 
            ' Process.Start(filename)

            Dim x = ""
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub dgv2rtf(dgv As DataGridView, filename As String)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim csv = ""
        Try
            Dim headers = (From header As DataGridViewColumn In dgv.Columns.Cast(Of DataGridViewColumn)()
                           Select header.HeaderText).ToArray
            Dim rows = From row As DataGridViewRow In dgv.Rows.Cast(Of DataGridViewRow)()
                       Where Not row.IsNewRow
                       Select Array.ConvertAll(row.Cells.Cast(Of DataGridViewCell).ToArray, Function(c) If(c.Value IsNot Nothing, RemoveSpcChar(c.Value.ToString), ""))
            Using sw As New IO.StreamWriter(filename)
                sw.WriteLine(String.Join(" ", headers))
                For Each r In rows
                    sw.WriteLine(String.Join(" ", r))
                Next
            End Using
            'opens the file 
            ' Process.Start(filename)

            Dim x = ""
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Public Function GetExceptionInfo(ex As Exception) As String
        Dim Result As String
        Dim hr As Integer = Runtime.InteropServices.Marshal.GetHRForException(ex)
        Result = ex.GetType.ToString & "(0x" & hr.ToString("X8") & "): " & ex.Message & Environment.NewLine & ex.StackTrace & Environment.NewLine
        Dim st As StackTrace = New StackTrace(ex, True)
        For Each sf As StackFrame In st.GetFrames
            Dim x = sf.GetFileLineNumber
            If sf.GetFileLineNumber() > 0 Then
                Result &= "Line:" & sf.GetFileLineNumber() & " Filename: " & IO.Path.GetFileName(sf.GetFileName) & Environment.NewLine
            End If
        Next
        Return Result
    End Function
    Function GetWklySql(sFrom, sTo) As String
        Try

            'https://stackoverflow.com/questions/22990229/count-iif-access-query
            Dim strsql = New StringBuilder
            Dim psDate As String = CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
            Dim lp3 As List(Of String) = Main.lPar3s
            strsql = New StringBuilder
            strsql.AppendLine(
$"WITH cte AS (
SELECT 
S.Date
,IIF(S.Date < {psDate}, {ctx.rLeagueParmrow("Skins")}, {ctx.rLeagueParmrow("SkinsPS")}) AS SkinPaid
,COUNT(*) AS Total
,SUM(CASE WHEN SP.ID IS NOT NULL THEN 1 ELSE 0 END) AS SkinPlayers
,MAX(S.FrontBack) as FrontBack
,SUM(CASE WHEN SE.ID IS NOT NULL THEN 1 ELSE 0 END) SkinsNum 
,SUM(CAST(SP.Earned AS REAL)) SkinsCollected 
,SUM(CAST(SE.Earned AS REAL)) SkinsEarned 
,SUM(CASE WHEN CP.ID IS NOT NULL THEN 1 ELSE 0 END) AS CTPPlayers
,IFNULL(SUM(CAST(E1.Earned AS real)),0) as CTP1
,IFNULL(SUM(CAST(E2.Earned AS real)),0) as CTP2
,SUM(CAST(CP.Earned AS REAL)) CTPCollected 
From [Scores] S
LEFT JOIN [Payments] SP ON (SP.Date = S.Date AND SP.Player = S.Player AND SP.[Desc] = 'Skin' AND SP.[Detail] = 'Payment')
LEFT JOIN [Payments] SE ON (SE.Date = S.Date AND SE.Player = S.Player AND SE.[Desc] = 'Skin' AND SE.[Detail] LIKE '#%')
LEFT JOIN [Payments] CP ON (CP.Date = S.Date AND CP.Player = S.Player AND CP.[Desc] = 'CTP' AND CP.[Detail] = 'Payment')
LEFT JOIN [Payments] E1 ON (E1.Date = S.Date AND E1.Player = S.Player AND E1.[Desc] = 'CTP' AND E1.[Detail] = CASE WHEN S.FrontBack = 'back' THEN '#10' ELSE '#4' END)
LEFT JOIN [Payments] E2 ON (E2.Date = S.Date AND E2.Player = S.Player AND E2.[Desc] = 'CTP' AND E2.[Detail] = CASE WHEN S.FrontBack = 'back' THEN '#12' ELSE '#8' END)
WHERE S.Date >= {sFrom} AND S.Date <= {sTo}
GROUP BY S.Date
)
SELECT Date,Total,SkinPaid,SkinPlayers,FrontBack,SkinsNum,SkinsCollected,SkinsEarned,
IIF(SkinsNum = 0,0,CAST(SkinsEarned/SkinsNum As Decimal(4,2))) SkinValue,
(SkinsCollected - SkinsEarned) SkinsExtra,
CTPPlayers,
CTPCollected,CTP1,CTP2,
CAST(CTP1 AS REAL) + CAST(CTP2 AS REAL) CTPEarned,
IIF(CTP1 = 0 AND Frontback = 'front',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) FCTP1Extra,
IIF(CTP2 = 0 AND Frontback = 'front',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) FCTP2Extra,
IIF(CTP1 = 0 AND Frontback = 'back',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) BCTP1Extra,
IIF(CTP2 = 0 AND Frontback = 'back',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) BCTP2Extra,
CTPCollected - (CTP1 + CTP2 + 
IIF(CTP1 = 0 AND Frontback = 'front',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) +
IIF(CTP2 = 0 AND Frontback = 'front',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) +
IIF(CTP1 = 0 AND Frontback = 'back',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) +
IIF(CTP2 = 0 AND Frontback = 'back',CAST(FLOOR(CTPCollected / 2) as DECIMAL ),0) 
) AS Kitty
FROM cte
"
)
            Return strsql.ToString
        Catch ex As Exception

        End Try
    End Function

    Sub CalcHoleMarker(sDate As String)
        Try
            If String.IsNullOrEmpty(sDate) Then Exit Sub

            ' 1. Establish the Default based on Month Parms
            ' Logic: If current date month matches the StartDate month's parity, use the Start9 setting.
            Dim currentMonth As Integer = Val(sDate.Substring(4, 2))
            Dim startMonth As Integer = CDate(ctx.rLeagueParmrow("StartDate")).Month
            Dim startSide As String = ctx.rLeagueParmrow("Start9").ToString() ' "F" or "B"

            ' If the parity (even/odd) is the same as the start month, stay on startSide. 
            ' Otherwise, flip it.
            Dim isFlipped As Boolean = (currentMonth Mod 2 <> startMonth Mod 2)

            If startSide = "B" Then
                iHoleMarker = If(isFlipped, 1, 10)
            Else
                iHoleMarker = If(isFlipped, 10, 1)
            End If

            ' 2. Schedule Override (The hierarchy winner)
            ' Using single quotes only for SQL per your requirement
            Dim sql As String = $"SELECT FrontBack FROM Schedule WHERE Date = '{sDate}' LIMIT 1"
            Dim dtSched As DataTable = sqlitedaFromSql(ctx.Conn, "SchedCheck", sql)

            If dtSched.Rows.Count > 0 Then
                Dim fbValue As String = dtSched.Rows(0)("FrontBack").ToString()
                If fbValue.Equals("Front", StringComparison.OrdinalIgnoreCase) Then
                    iHoleMarker = 1
                ElseIf fbValue.Equals("Back", StringComparison.OrdinalIgnoreCase) Then
                    iHoleMarker = 10
                End If
            End If

        Catch ex As Exception
            ' Log it but don't let a calculation crash the whole league load
            Debug.Print("CalcHoleMarker Error: " & ex.Message)
        End Try
    End Sub
    Shared Sub arraySort(ofiles() As IO.FileInfo)
        If ofiles.Count > 0 Then
            Array.Sort(Of FileInfo)(ofiles, New Comparison(Of FileInfo)(Function(f1 As FileInfo, f2 As FileInfo) f2.FullName.CompareTo(f1.FullName)))
        End If
    End Sub
    Function getSubstring(sText As String, sPoint As String, sEndPoint As String)
        'starting point in text

        If IsNumeric(sPoint) Then
            getSubstring = sText.Substring(sPoint, sText.IndexOf(sEndPoint) - sPoint)
            Exit Function
        End If

        Dim x = sText.LastIndexOf(sPoint) + 1
        'length of string
        Dim y = sText.Length
        'ending point
        Dim z = sText.LastIndexOf(sEndPoint)
        'pos of my wanted data
        Dim zz = sText.Length - (sText.LastIndexOf(sPoint) + 1)
        getSubstring = sText.Substring(sText.LastIndexOf(sPoint) + 1, sText.IndexOf(sEndPoint) - sText.LastIndexOf(sPoint) - 1)

    End Function
    Function getLatestFile(sFile As String) As String
        getLatestFile = ""
        If Not Directory.Exists(sFilePath) Then
            Dim mbr = MessageBox.Show($"Path not found {sFilePath}, pick a folder to pull in files from or Cancel", "Warning", MessageBoxButtons.OKCancel)
            If mbr = System.Windows.Forms.DialogResult.Cancel Then End
            Dim dialog As New FolderBrowserDialog With
         {
         .RootFolder = Environment.SpecialFolder.Desktop,
         .SelectedPath = sFilePath,
         .Description = "Select League Files Path"
         }
            If dialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                sFilePath = dialog.SelectedPath
            Else
                End
            End If
        End If

        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(sFilePath)
        oFiles = oDirectory.GetFiles(sFile)
        arraySort(oFiles)
        For Each file In oFiles
            'If file.Name.Substring(0, 4) <= sYear Then
            getLatestFile = file.FullName
            Exit For
            'End If
        Next

    End Function
    Function SBPCalcStrokeIndex(sHole As String) As String
        SBPCalcStrokeIndex = ""
        Try
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            'check stroke index
            SBPCalcStrokeIndex = 0
            Dim isi = ctx.thiscourse.Item(sHole.Replace("Hole", "H"))
            'if 9 holes and its an odd stroke index, adjust for remainder when we divide by 2
            If iHoles = 9 Then
                If isi Mod 2 Then isi += 1
                isi = Math.Round((isi) / 2, 0)
            End If
            Return isi
        Catch ex As Exception
            Dim x = ""
        End Try
    End Function
    Function CalcStrokeIndex(sHole As String) As String
        CalcStrokeIndex = ""
        Try
            'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            'check stroke index
            CalcStrokeIndex = 0
            Dim sHoleDesc As String = ""
            sHoleDesc = sHole.Replace("Hole", "")
            'If bScoresbyPlayer Then
            '    If iHoleMarker = 10 Then sHoleDesc += 9
            'End If
            sHoleDesc = "H" & sHoleDesc
            Dim isi = ctx.thiscourse.Item(sHoleDesc)
            'if 9 holes and its an odd stroke index, adjust for remainder when we divide by 2
            If iHoles = 9 Then
                If isi Mod 2 Then
                    isi += 1
                End If
                isi = Math.Round((isi) / 2, 0)
            End If
            Return isi
        Catch ex As Exception

        End Try
    End Function
    Private Function MapColumnToCourseField(colName As String, dgr As DataGridViewRow, cellValue As Object) As String
        Try

            Dim sCFieldName As String = Nothing

            ' Numeric column names = hole numbers
            Dim holeNum As Integer
            If Integer.TryParse(colName, holeNum) AndAlso IsNumeric(cellValue) Then

                sCFieldName = $"Hole{holeNum}"

                'ElseIf colName.Contains("18") Then
                '    sCFieldName = "Total"

            ElseIf colName.Contains("Gross") Then
                If ctx.ActiveDate >= sPSDate Then
                    sCFieldName = "Total"
                Else
                    sCFieldName = If(sFrontBack = "Front", "In", "Out")
                End If

            ElseIf colName.Contains("Net") Then
                If ctx.ActiveDate >= sPSDate Then
                    sCFieldName = "Total"
                Else
                    sCFieldName = If(sFrontBack = "Front", "In", "Out")
                End If

            ElseIf colName.Contains("Front") Then
                sCFieldName = "In"

            ElseIf colName.Contains("Back") Then
                sCFieldName = "Out"
            End If

            Return sCFieldName
        Catch ex As Exception

        End Try
    End Function
    Public Sub ChangeColorsForUPMoney(dgr As DataGridViewRow)
        Try
            For Each cel As DataGridViewCell In dgr.Cells
                Dim sColName As String = cel.OwningColumn.Name
                Dim sCFieldName As String = MapColumnToCourseField(sColName, dgr, cel.Value)
                'Debug.Print($"{sColName}-{sCFieldName}")
                If sCFieldName IsNot Nothing Then
                    If cel.Value IsNot Nothing AndAlso IsNumeric(cel.Value) Then
                        Dim val As String = cel.Value.ToString()
                        If Not String.IsNullOrEmpty(val) Then
                            ' Safe to use val here
                            MarkSubPar(cel, cel.Value, thisCourse(sCFieldName).ToString())
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Debug.Print("ChangeColorsForUPMoney error: " & ex.Message)
        End Try
    End Sub

    Public Sub ChangeColorsForStrokes(ByVal R As DataGridViewRow)
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'this sub will call marksubpar routine to color the cell for birdies and eagles
        Try
            'keep new rows from being evaluated
            If R.IsNewRow Then Exit Sub
            '20180307-evaluate this statement, should this be here
            If R.Cells.Item("pHdcp").Value.ToString = "" Then Exit Sub

            IHdcp = R.Cells.Item("pHdcp").Value.ToString
            If iHoles > 9 Then IHdcp *= 2
            'figure out hole by hole
            'if holes 1-18 all zeros, then we used a "Score" method and no hole by hole can be done
            'if holes 1-9 are zero but holes 10-18 are populated, we have a back 9 only score
            'if holes 10-18 are zero but holes 1-9 are populated, we have a frnt 9 only score
            If Not bCalcSkins Then
                If CDate(dDate).ToString("yyyyMMdd") < CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                    If convDBNulltoSpaces(R.Cells("Points").Value).Trim <> "" Then
                        'did this player win, change to green?
                        If R.Cells("Points").Value = "1" Then
                            R.Cells("Points").Style.BackColor = Color.LightGreen
                            'did he tie, change to yellow
                        ElseIf R.Cells("Points").Value = "0.5" Then
                            R.Cells("Points").Style.BackColor = Color.Yellow
                        Else
                            R.Cells("Opponent").Style.BackColor = Color.LightGreen
                        End If
                    End If
                    If convDBNulltoSpaces(R.Cells("Team_Points").Value).Trim <> "" Then
                        If R.Cells("Team_Points").Value = "1" Then
                            R.Cells("Team_Points").Style.BackColor = Color.LightGreen
                        ElseIf R.Cells("Team_Points").Value = "0.5" Then
                            R.Cells("Team_Points").Style.BackColor = Color.Yellow
                        End If
                    End If
                End If

            End If

            'If bScoresbyPlayer Then
            '    colorScores(R, "Gross", "Out")
            '    colorScores(R, "Net", "Out")
            'Else
            colorScores(R, "Gross", "", True)
            colorScores(R, "Net", "", True)
            'End If

            For Each cell As DataGridViewCell In R.Cells
                Dim sColName = cell.OwningColumn.Name
                'this changes sub name to color aqua
                If sColName = "Player" Then
                    Dim dvplayers As New DataView(dsLeague.Tables("dtPlayers"))
                    dvplayers.RowFilter = "Name = '" & R.Cells(sColName).Value & "'"
                    sPlayer = R.Cells(sColName).Value.ToString
                    'this shouldnt happen (if no rows returned)
                    If dvplayers.Count = 0 Then Exit Sub
                    'if no team, they are a sub
                    Dim sTeam As String = convDBNulltoSpaces(dvplayers(0).Item("Team")).Trim
                    If sTeam = "" Then
                        R.Cells(sColName).Style.BackColor = Color.Aqua
                    End If
                ElseIf sColName.Contains("Hole") Then
                    If cell.Value IsNot Nothing And cell.Value IsNot DBNull.Value Then
                        Try
                            Dim iScore As String = RemoveSpcChar(convDBNulltoSpaces(cell.Value).Trim)
                            'If iScore <> "0" And iScore <> "" Then MarkSubPar(cell, iScore, MyCourse(0)(sColName).ToString)
                            If IsNumeric(iScore) Then MarkSubPar(cell, iScore, MyCourse(0)(sColName).ToString)
                            'this catches null scores
                        Catch ex As Exception
                            'Dim x = ""
                            'MsgBox(sPlayer & " " & cell.OwningColumn.Name)
                        End Try
                    End If
                End If
            Next

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    'Sub colorScores(r As DataGridViewRow, col As String, compval As String)
    '    colorScores(r, col, compval, False)
    'End Sub

    Sub colorScores(r As DataGridViewRow, col As String, compval As String, bHolemarker As Boolean)
        'if this isnt scoresbyplayer then use the holemarker to determine out/in
        If bHolemarker Then
            If iHoleMarker = 1 Then
                col = "Out_" & col
                compval = "Out"
            Else
                col = "In_" & col
                compval = "In"
            End If
        End If
        'change colors if under par
        If convDBNulltoSpaces(r.Cells(col).Value).Trim <> "" Then
            If r.Cells(col).Value.ToString < MyCourse(0)(compval).ToString Then
                r.Cells(col).Style.BackColor = UnderParColor
            Else
                r.Cells(col).Style.BackColor = Color.White
            End If
        End If
    End Sub
    Public Sub MarkSubPar(cell As DataGridViewCell, iscore As Integer, iPar As Integer)
        Try
            If IHdcp = 99 Then Exit Sub
            'cell.Style.Font = New Font("Arial", 19, FontStyle.Regular)
            cell.Style.ForeColor = Color.Black
            'cell.Style.BackColor = Color.White
            cell.Value = RemoveSpcChar(cell.Value)
            Dim sFont = "Tahoma"
            Dim iFontSize = 9
            Dim bFontStrikeout = False
            If cell.Style.Font IsNot Nothing Then
                If cell.Style.Font.Strikeout = True Then
                    bFontStrikeout = True
                End If
            End If
            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
            'check scores against handicap
            'is this a stroke hole?
            If iHoles = 0 Then iHoles = ctx.rLeagueParmrow("Holes")
            If IsNumeric(cell.OwningColumn.Name) Then
                'check stroke index
                Dim isi = CalcStrokeIndex(cell.OwningColumn.Name)
                'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
                'if the handicap > stroke index make color beige
                If IHdcp >= isi Then
                    If bColors Then cell.Style.BackColor = Color.Beige
                    If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                    'if double stroke hole, make color b/a
                    If IHdcp - iHoles >= isi Then
                        If bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                        If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                    End If
                End If
            End If

            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
            If iscore < iPar Then
                'iFontSize += 3
                cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                cell.Style.ForeColor = UnderParColor
                'if this is a total score (> 10) of some sort, dont check for birdies, eagles
                If iscore > 10 Then Exit Sub
                'birdie
                If iscore < iPar - 1 Then
                    'cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
                    cell.Style.ForeColor = Color.DarkRed
                End If
                'eagle
                If iscore < iPar - 2 Then
                    'iFontSize += 3
                    'cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                End If
                'over par is black,even is gray
            ElseIf iscore > iPar Then
                cell.Style.ForeColor = Color.Blue
            End If
            If bFontStrikeout Then
                cell.Style.Font = New Font(sFont, iFontSize, cell.Style.Font.Style Or FontStyle.Strikeout)
                LOGIT(sPlayer & " hole " & cell.OwningColumn.Name & " has s/o on")
            End If
        Catch ex As Exception

        End Try

    End Sub
    Function FixNullScore(iNet As Object) As Int16
        FixNullScore = 999
        If IsNumeric(iNet) Then FixNullScore = iNet
        'Catch ex As Exception
        '    FixNullScore = 999
        'End Try

    End Function
    Function convDBNulltoSpaces(ByVal sfield) As String
        convDBNulltoSpaces = ""
        Try
            If IsDBNull(sfield) Then
                convDBNulltoSpaces = " "
            ElseIf sfield Is Nothing Then
                convDBNulltoSpaces = " "
            Else
                convDBNulltoSpaces = sfield
            End If
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function
    Function FrontBack(sDate As String) As String
        Return LeagueDateService.GetFrontBack(sDate, ctx.rLeagueParmrow("Start9").ToString())
    End Function
    Public Shared Function ShowStackTrace() As List(Of String)
        ShowStackTrace = New List(Of String)
        ' Capture the stack trace
        Dim stackTrace As New StackTrace(True) ' True to capture file information

        ' Display each frame in the stack trace
        For Each frame As StackFrame In stackTrace.GetFrames()
            Dim method As System.Reflection.MethodBase = frame.GetMethod()
            Dim lineNumber As Integer = frame.GetFileLineNumber()
            If method.DeclaringType IsNot Nothing Then
                If method.DeclaringType.FullName.ToLower.StartsWith("hughsgolf") Then
                    'logit($"{method.DeclaringType.FullName}.{method.Name} at line {lineNumber}")
                    If method.Name.ToLower.Contains("logit") Or method.Name.ToLower.Contains("showstacktrace") Then Continue For
                    ShowStackTrace.Add($"{method.DeclaringType.Name}.{method.Name}:Line {lineNumber}")
                End If
            End If
        Next
    End Function
#Region "Handicap / Last5 Narrative"
    '1) read a score
    '2) build 5 score array 
    '3) rounds 1-3
    '   a)  update 5 scores array
    '   b)  calculate hdcp each score (1- below)
    '   c)  update hdcp in dvscores 1-3 (2- below)
    '   d)  go to 1)
    '4) round 4
    '   a)  loop through the array finding the highest score
    '   b)  loop through the array again totaling the 3 scores that dont match the highest score
    '   c)  calculate hdcp each score (1- below)
    '   d)  update hdcp in dvscores row 4 (2- below)
    '   e)  go to 1)
    '5) round 5
    '   a)  loop through the array finding the highest and lowest scores
    '   b)  loop through the array again totaling the 3 scores that dont match the highest and lowest scores
    '   c)  calculate hdcp each score (1- below)
    '   d)  update hdcp in dvscores row 5 (2- below)
    '   e)  go to 1)
    '6) rounds 6-99
    '   a)  remove score 1 from the array
    '   b)  add score 6 to end of array
    '   c)  drop highest and lowest scores moving 3 scores into 3 score array
    '   d)  calculate hdcp each score (1- below)
    '   e)  update hdcp in dvscores row 6 - 99 (2- below)
    '   f)  go to 1)
    '
    '1-calculate handicap = (scores - par) / rounds * .8 using array in step 2)
    '2-Update handicap table for the round were processing

#End Region
    Function ScoresUsedInHC(lLast5Scores As List(Of String)) As List(Of String)
        ScoresUsedInHC = New List(Of String)
        If lLast5Scores.Count > 0 Then
            If lLast5Scores.Count = 5 Then
                lLast5Scores.Remove(lLast5Scores.Min)
                lLast5Scores.Remove(lLast5Scores.Max)
            ElseIf lLast5Scores.Count = 4 Then
                lLast5Scores.Remove(lLast5Scores.Max)
            End If
        End If
        Return lLast5Scores
    End Function

    Function calcHdcp(dt As DataTable, Optional bUpdate As Boolean = False) As String
        'this sub expects that scores are all entered and 
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        calcHdcp = ""
#Region "Narrative"
        '1) read a score
        '2) build 5 score array 
        '3) rounds 1-3
        '   a)  update 5 scores array
        '   b)  calculate hdcp each score (1- below)
        '   c)  update hdcp in dvscores 1-3 (2- below)
        '   d)  go to 1)
        '4) round 4
        '   a)  loop through the array finding the highest score
        '   b)  loop through the array again totaling the 3 scores that dont match the highest score
        '   c)  calculate hdcp each score (1- below)
        '   d)  update hdcp in dvscores row 4 (2- below)
        '   e)  go to 1)
        '5) round 5
        '   a)  loop through the array finding the highest and lowest scores
        '   b)  loop through the array again totaling the 3 scores that dont match the highest and lowest scores
        '   c)  calculate hdcp each score (1- below)
        '   d)  update hdcp in dvscores row 5 (2- below)
        '   e)  go to 1)
        '6) rounds 6-99
        '   a)  remove score 1 from the array
        '   b)  add score 6 to end of array
        '   c)  drop highest and lowest scores moving 3 scores into 3 score array
        '   d)  calculate hdcp each score (1- below)
        '   e)  update hdcp in dvscores row 6 - 99 (2- below)
        '   f)  go to 1)
        '
        '1-calculate handicap = (scores - par) / rounds * .8 using array in step 2)
        '2-Update handicap table for the round were processing

#End Region
        Try
            ' Assume dt is your DataTable with a column named "Score"
            ' Build numeric score list
            Dim scoreList = dt.AsEnumerable().
    Where(Function(r) Not IsDBNull(r("Score")) AndAlso IsNumeric(r("Score"))).
    OrderByDescending(Function(r) CInt(r("Date"))).
    Select(Function(r) Convert.ToInt32(r("Score"))).
    Take(5).
    ToList()

            Dim parList = dt.AsEnumerable().
    Where(Function(r) Not IsDBNull(r("Par")) AndAlso IsNumeric(r("Par"))).
    OrderByDescending(Function(r) CInt(r("Date"))).
    Select(Function(r) Convert.ToInt32(r("Par"))).
    Take(5).
    ToList()

            calcHdcp = String.Join(",", scoreList)
            If scoreList.Count > 0 Then
                Dim minScore = scoreList.Min()
                Dim maxScore = scoreList.Max()

                ' Apply round rules
                Select Case scoreList.Count
                    Case 5
                        scoreList.Remove(minScore)   ' drop lowest
                        scoreList.Remove(maxScore)   ' drop highest
                    Case 4
                        scoreList.Remove(maxScore)   ' drop highest
                End Select

                ' Now safely total
                IHdcp = ((scoreList.Sum() / scoreList.Count) - (parList.Take(scoreList.Count).Sum() / scoreList.Count)) * 0.8
            Else
                Return ""
            End If
            If IHdcp > 18 Then IHdcp = 18
            Dim pHdcp As String = GetPHdcp()

            '20250708 - cap handicap at 18 for now
            If IHdcp > 18 Then
                IHdcp = 18
            End If
            If bUpdate Then
                If IHdcp <> pHdcp Then
                    Dim sb As New StringBuilder
                    sb.AppendLine($"
UPDATE Handicaps SET 
PHdcp = {pHdcp},
Hdcp = {IHdcp}
WHERE League = '{ctx.SafeLeagueName}' AND
Player = '{sPlayer}' AND
Date = {ctx.ActiveDate}
")
                    'Debug.Print(sb.ToString)
                    Dim lrc = SqliteTrans(sb.ToString)
                End If
            End If

            Return $"{IHdcp}-{calcHdcp}"
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Function GetNewHdcp(sthisScore As String) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        GetNewHdcp = ""
        Try
            Dim pHdcp As String = GetPHdcp()

            ' --- GET LAST 5 FROM PlayerLast5 table ---
            Dim last5Str As String = ""
            Using cmd As New SQLiteCommand("
            SELECT Last5 FROM PlayerLast5
            WHERE League=@League AND Player=@Player AND LastDate = @Date
            ORDER BY LastDate DESC
            LIMIT 1", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                cmd.Parameters.AddWithValue("@Player", ctx.sPlayer)
                cmd.Parameters.AddWithValue("@Date", ctx.ActiveDate)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    last5Str = result.ToString()
                End If
            End Using

            LOGIT($"GetNewHdcp: player={ctx.sPlayer} last5={last5Str} sthisScore={sthisScore}")

            Dim iScoreTot As Integer = 0
            Dim iParTot As Integer = 0

            If Not String.IsNullOrEmpty(last5Str) Then
                ' Parse scores from Last5 string e.g. "32-34-35-36-33"
                Dim parts() As String = last5Str.Split("-"c)
                Dim scores As New List(Of Integer)
                For Each p As String In parts
                    Dim n As Integer
                    If Integer.TryParse(p.Trim(), n) AndAlso n > 0 Then
                        scores.Add(n)
                    End If
                Next

                ' Get par for this course/side
                If ctx.sFrontBack = "Front" Then
                    wk = "Out"
                Else
                    wk = "In"
                End If
                Dim par As Integer = CInt(ctx.thiscourse(wk))

                ' Drop highest/lowest based on count
                If scores.Count = 5 Then
                    scores.Remove(scores.Max())
                    scores.Remove(scores.Min())
                ElseIf scores.Count = 4 Then
                    scores.Remove(scores.Max())
                End If

                ' Calculate hdcp from remaining scores
                For Each s As Integer In scores
                    iScoreTot += s
                    iParTot += par
                Next

                If scores.Count > 0 Then
                    IHdcp = (CDbl(iScoreTot) / scores.Count - CDbl(iParTot) / scores.Count) * 0.8
                End If

                If pHdcp = "" Then pHdcp = IHdcp

            Else
                ' First time player — use this score only
                If pHdcp = "" Then
                    If ctx.sFrontBack = "Front" Then
                        wk = "Out"
                    Else
                        wk = "In"
                    End If
                    iParTot = CInt(ctx.thiscourse(wk))
                    IHdcp = (CInt(sthisScore) - iParTot) * 0.8
                    pHdcp = IHdcp
                End If
            End If

            ' Cap handicap at 18
            If IHdcp > 18 Then IHdcp = 18
            '20260516 - round handicap to nearest whole number for posting
            IHdcp = Math.Round(IHdcp)

            ' Upsert into Handicaps
            Dim _sb As New StringBuilder()
            _sb.AppendLine("INSERT OR REPLACE INTO Handicaps (League, Player, Date, PHdcp, Hdcp)")
            _sb.AppendLine("VALUES (@League, @Player, @Date, @PHdcp, @Hdcp)")

            Dim hParams As New Dictionary(Of String, Object) From {
            {"@League", ctx.sLeagueName},
            {"@Player", ctx.sPlayer},
            {"@Date", ctx.ActiveDate},
            {"@PHdcp", pHdcp},
            {"@Hdcp", IHdcp}
        }

            Try
                SqliteTrans(_sb.ToString(), hParams)
                LOGIT($"Handicap updated for {ctx.sPlayer}: PHdcp={pHdcp} Hdcp={IHdcp}")
            Catch ex As Exception
                LOGIT($"Error updating Handicap for {ctx.sPlayer}: {ex.Message}")
            End Try

            Return IHdcp

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Function getLast5() As List(Of String)
        Try

            Dim sql As String = $"SELECT * FROM vwMatchesScores WHERE Date = '{ctx.ActiveDate}'"
            Dim dtl5 = sqliteda(ctx.Conn, sql)

            dt = sqliteda(ctx.Conn, "Handicaps")
            getLast5 = New List(Of String)
            Dim strsql = New StringBuilder
            '20191216-check for incomplete scores so we can populate toolstrip only
            Dim sHoleWhere = New StringBuilder
            For i = 1 To 9
                sHoleWhere.AppendLine($" AND [{i}] GLOB '*[0-9]*'")
            Next
            'get the last 5 most recent scores to add to the last 5
            strsql = New StringBuilder
            strsql.AppendLine($"
WITH FilteredScores AS (
    SELECT 
        s.Player,
        s.Gross, 
        s.Date,
        -- Get the specific Par for this round (Front or Back)
        CASE WHEN s.FrontBack = 'Front' THEN c.[Out] ELSE c.[In] END as CoursePar,
        -- Calculate Net
        (s.Gross - CASE WHEN s.FrontBack = 'Front' THEN c.[Out] ELSE c.[In] END) as NetScore,
        ROW_NUMBER() OVER (ORDER BY s.Date DESC) as ScoreRank,
        AVG(s.Gross) OVER () as CareerAvg,
        COUNT(s.Gross) OVER () as CareerCount
    FROM Scores s
    INNER JOIN LeagueParms lp ON s.League = lp.Name 
        AND SUBSTR(s.Date, 1, 4) = CAST(lp.Season AS TEXT)
    INNER JOIN Courses c ON lp.Course = c.Name
    WHERE s.Player = '{ctx.sPlayer}'
      AND s.Date < lp.PostSeasonDt
),
Top5 AS (
    SELECT 
        *,
        ROW_NUMBER() OVER (ORDER BY Gross ASC, Date DESC) as LoRank,
        ROW_NUMBER() OVER (ORDER BY Gross DESC, Date DESC) as HiRank,
        COUNT(*) OVER () as cnt
    FROM FilteredScores 
    WHERE ScoreRank <= 5
),
FinalStats AS (
    SELECT 
        Player,
        GROUP_CONCAT(Gross, '-') AS ScoreHistory,
        -- This creates the '39-36, 41-36' format you want
        GROUP_CONCAT(Gross || '-' || CoursePar, ', ') AS GrossParHistory,
        CASE 
            WHEN cnt = 5 THEN (SUM(NetScore) - MIN(NetScore) - MAX(NetScore)) / 3.0
            WHEN cnt = 4 THEN (SUM(NetScore) - MAX(NetScore)) / 3.0
            ELSE AVG(NetScore)
        END AS RawHdcpno80,
        CASE 
            WHEN cnt = 5 THEN (SUM(Gross) - MIN(Gross) - MAX(Gross)) / 3.0
            WHEN cnt = 4 THEN (SUM(Gross) - MAX(Gross)) / 3.0
            ELSE AVG(Gross)
        END AS RecentAvg,
        MAX(CareerAvg) AS CareerAvg,
        MAX(CareerCount) AS TotalRounds
    FROM Top5
    GROUP BY Player
)
SELECT 
    Player,
    ScoreHistory,
    GrossParHistory,
    ROUND(RecentAvg, 1) AS RecentAvg,
    ROUND(RawHdcpno80, 1) AS Hdcpno80,
    CAST(RawHdcpno80 * 0.8 + 0.5 AS INTEGER) AS Hdcp,
    ROUND(CareerAvg, 1) AS CareerAvg,
    TotalRounds,
    CASE 
        WHEN RecentAvg < CareerAvg - 0.5 THEN '↓' 
        WHEN RecentAvg > CareerAvg + 0.5 THEN '↑' 
        ELSE '→' 
    END AS Trend
FROM FinalStats
LIMIT 1;
")
            Try
                ' Execution logic for DataReader
                Try
                    ' 1. Check if we need to open the connection
                    Dim wasClosed As Boolean = (ctx.Conn.State = ConnectionState.Closed)
                    If wasClosed Then ctx.Conn.Open()

                    Using cmd As New SQLiteCommand(strsql.ToString, ctx.Conn)
                        Using dr As SQLiteDataReader = cmd.ExecuteReader()
                            If dr.Read() Then
                                ' Populate your lists from the SQL-formatted strings
                                iLast5Scores = dr("ScoreHistory").ToString().Split("-"c).ToList()

                                ' Splits "39-36, 41-36" into a List
                                getLast5 = dr("GrossParHistory").ToString().Split(","c).Select(Function(s) s.Trim()).ToList()

                                ' Example of grabbing other stats
                                Dim sTrend As String = dr("Trend").ToString()
                                Dim iHdcp As Integer = Convert.ToInt32(dr("Hdcp"))

                            End If
                        End Using
                    End Using

                    ' 2. If we opened it, we close it.
                    If wasClosed Then ctx.Conn.Close()
                    Exit Function
                Catch ex As Exception
                    ' If an error happens, ensure we still try to close it
                    If ctx.Conn.State = ConnectionState.Open Then ctx.Conn.Close()
                    MsgBox("Error reading player data: " & ex.Message)
                End Try
            Catch ex As Exception
                Dim scallingmethod As String = (New Diagnostics.StackTrace).GetFrame(1).GetMethod.Name
                Dim st = ShowStackTrace()
                Dim msg = New StringBuilder
                msg.AppendLine(ex.Message)
                For Each stentry In st
                    msg.AppendLine(stentry)
                Next
                msg.AppendLine(" ************ Critical Error***********")
                msg.AppendLine("Report To Developer")
                msg.AppendLine("************ Critical Error***********")
                MessageBox.Show(msg.ToString)
                End
            End Try
            iLast5Scores = New List(Of String)
            For Each srow In dt.Rows
                iLast5Scores.Add($"{srow("Gross")}")
                getLast5.Add($"{srow("Gross")}-{srow("Par")}")
            Next
            'dt = sqlitedaFromSql(ctx.Conn, "", $"Select * from Scores where player = '{sPlayer}'")
            'wk = ""
            'dt = sqlitedaFromSql(ctx.Conn, "", $"select * from handicaps where player = '{sPlayer}'")
        Catch ex As Exception

        End Try

    End Function

#Region "Update Handicaps"
    Sub updateHandicaps()
        'get the latest handicap from the handicaps table
        Dim sb = New StringBuilder
        sb.AppendLine($"

            SELECT t1.League,t1.Player, t1.Date, t1.Hdcp 
            FROM Handicaps t1
            INNER JOIN (
                SELECT Player, MAX(Date) AS Date
                FROM Handicaps 
                WHERE Hdcp <> ''
                GROUP BY Player
            ) t2 ON t2.Player = t1.Player AND t2.Date = t1.Date
            ORDER BY t1.Player;
            ")
        Dim dtHandicaps = sqlitedaFromSql(ctx.Conn, "Handicaps", sb.ToString)
        dtHandicaps.PrimaryKey = New DataColumn() {dtHandicaps.Columns("Player")}
        'if this player is being replaced
        'delete the old players handcicap record for week 1
        'add a new handicap record for week 1
        'oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "Handicaps", $"SELECT CONCAT(League,Player,Date) FROM Handicaps ") 'WHERE League+Player+Date = '{Main.cbLeagues.SelectedItem.ToString.Replace("'", "''")}{sold}{ctx.ActiveDate}'")
        Using cmd As New SQLiteCommand(ctx.Conn)
            ' Prepare the SQL command
            cmd.CommandText = $"DELETE FROM Handicaps WHERE CONCAT(League,Player,Date) = '{ctx.sLeagueName}{sPlayer}{ctx.ActiveDate}'"
            Dim lrec = cmd.ExecuteNonQuery()
            'LOGIT($"Handicap records {lrec} deleted for {ctx.sLeagueName}{sPlayer}{ctx.ActiveDate}")
        End Using
        Using cmd As New SQLiteCommand(ctx.Conn)
            Dim snewhandicap = 0
            Dim drhdcp = dtHandicaps.Rows.Find(sPlayer)
            If drhdcp IsNot Nothing Then
                snewhandicap = drhdcp("Hdcp")
            End If
            Dim scolValues = $"'{ctx.sLeagueName}','{sPlayer}','{ctx.ActiveDate}','{snewhandicap}'"
            ' Prepare the SQL command
            cmd.CommandText = $"INSERT INTO Handicaps (League,Player,Date,PHdcp) VALUES ({scolValues})"
            Dim lrec = cmd.ExecuteNonQuery()
            'LOGIT($"Handicap added for {ctx.sLeagueName}{sPlayer}{ctx.ActiveDate}")
        End Using
        dt = sqliteda(ctx.Conn, "Handicaps")

#End Region
    End Sub

    Function ChkForMax(sScore As Decimal, sHole As String) As Decimal
        If IsNumeric(sScore) Then
            Return ScoreRulesService.CapScore(
                sScore,
                CInt(ctx.thiscourse($"Hole{sHole}")),
                CDec(ctx.rLeagueParmrow("Par3Max")),
                CDec(ctx.rLeagueParmrow("Par4Max")),
                CDec(ctx.rLeagueParmrow("Par5Max")))
        Else
            MsgBox("Score must be numeric " & sScore)
            Return sScore
        End If
    End Function

    Public Function fGetPlayer(sNameInfo As String, dgv As DataGridView) As String
        'fGetPlayer = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sInitials)
        fGetPlayer = ""
        sPlayer = sNameInfo
        'if only initials were entered, search by initials in player file
        Try
            Dim sRowFilter As String = "SELECT Player,FirstName,LastName FROM Players WHERE "
            If sNameInfo.Length = 2 Then
                sRowFilter &= $"FirstName LIKE '{sNameInfo.Substring(0, 1)}%' AND LastName LIKE '{sNameInfo.Substring(1, 1)}%'"
            Else
                sRowFilter &= $"Player LIKE '{sNameInfo}%'"
            End If
            sRowFilter &= $" AND Player NOT IN (SELECT Player From Teams WHERE League = '{ctx.SafeLeagueName}' AND Year = '{ctx.rLeagueParmrow("Season")}' )"
            Dim dtPlayers As DataTable = sqlitedaFromSql(ctx.Conn, "Players", sRowFilter)
#Region "N/A"
            GoTo checkPlayers

            Dim sTeam = ""
            Dim sGrade = ""
            Dim Id As Integer = 1
            Id = sqlitedaFromSql(ctx.Conn, "Players", $"SELECT Id FROM Subs ORDER BY ID DESC LIMIT 1").Rows(0)(0) + 1
            'this loop will look to see if the player changed was already added to the grid as another  OK
            Dim i = 0
            For Each row As DataGridViewRow In dgv.Rows
                'skip to the player were currently looking at'
                If row.Cells(Player).Value = sNameInfo Then
                    i += 1
                    sTeam = row.Cells(Team).Value
                    sGrade = row.Cells("Grade").Value
                    Continue For
                End If
            Next

            If i > 1 Then
                MsgBox($"You've already entered this player {sPlayer}, Try Again")
                'row.Cells("Player").Value = ""
                bDGSError = True
                Exit Function
            End If
checkPlayers:

#End Region
            Select Case dtPlayers.Rows.Count
                Case 0
                    Dim table = "Subs"
                    If dgv.Name.ToLower = "dgteams" Then
                        table = "Teams"
                    End If
                    Dim sb = New StringBuilder
                    If sPlayer.Length < 3 Then
                        sb.AppendLine($"No Sub with Initials {sPlayer} found, try full name")
                        MessageBox.Show(sb.ToString, "Player Chooser", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                        bDGSError = True
                        fGetPlayer = ""
                        Exit Function
                    Else
                        sb.AppendLine($"{sNameInfo} not found in Player file
Do you want to add {sPlayer} to the Player and {table} files?")
                    End If
                    If MessageBox.Show(sb.ToString, "Player Chooser", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) = DialogResult.Yes Then
                        Dim sFirst = sPlayer.Substring(0, sPlayer.IndexOf(" "))
                        sFirst = Char.ToUpper(sFirst(0)) & sFirst.Substring(1).ToLower()
                        Dim sLast = sPlayer.Substring(sPlayer.IndexOf(" ") + 1)
                        sLast = Char.ToUpper(sLast(0)) & sLast.Substring(1).ToLower()
                        sPlayer = $"{sFirst} {sLast}"
                        Dim insertQuery As String = $"INSERT INTO Players (ID,Player,FirstName,LastName) Values ({Id},'{sPlayer}','{sFirst}','{sLast}')"
                        Dim lrc = SqliteTrans(insertQuery)
                        dt = sqliteda(ctx.Conn, "Players")

                        Dim insertUpdQuery As String = $"INSERT INTO {table} (ID,League,Player,Date,Team,Grade) Values ({Id},'{ctx.SafeLeagueName}','{sPlayer}',{ctx.ActiveDate},'{sTeam}','{sGrade}')"
                        If table.ToLower = "teams" Then
                            insertUpdQuery = $"UPDATE {table} SET Player = '{sPlayer}' WHERE League = '{ctx.SafeLeagueName}' and Year = {ctx.ActiveDate.ToString.Substring(0, 4)} AND Team = {sTeam} AND Grade = '{sGrade}'"
                        End If
                        lrc = SqliteTrans(insertUpdQuery)
                        dt = sqliteda(ctx.Conn, $"{table}")
                        'sPlayer = ""
                        Return sPlayer
                    Else
                        bDGSError = True
                        fGetPlayer = ""
                        Exit Function
                    End If

                Case 1
                    'sPlayer = dtPlayers.Rows(0)(0)
                    'Dim ssql As String = $"Select Player,Team FROM Subs WHERE League = '{ctx.safeLeagueName}' AND Date = '{ctx.ActiveDate}' AND Player = '{dtPlayers.Rows(0)(Player)}'"
                    'Dim dt As DataTable = sqlitedaFromSql(ctx.Conn, "", ssql)
                    'If dt.Rows.Count > 0 Then
                    '    MsgBox($"This Player {sPlayer} is already subbing for Team {dt.Rows(0)(Team)}, Try Again")
                    '    row.Cells("Player").Value = ""
                    '    bDGSError = True
                    '    Return ""
                    '    Exit Function
                    'End If
                    Return dtPlayers.Rows(0)(Player)
                Case > 1
                    'find the player in dataview
                    Dim dv = dtPlayers.DefaultView
                    Using f As New SelectPlayer(dv)
                        If f.ShowDialog() = DialogResult.OK Then
                            Return f.SelectedPlayer
                            wk = ""
                        End If
                    End Using

                    MessageBox.Show("Exiting Player Finder, try another player")
                    Return ""
            End Select

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function
    Function fixPlayer(sNameInfo As String) As String
        fixPlayer = ""
        'create rules for last names like McDonald, O'Reilly, etc
        Dim sFLName As String() = sNameInfo.ToString.Split(" ")
        If sFLName.Count > 2 Then
            MsgBox("Name cant have more than First/Last Name, Fix Manually")
            fixPlayer = sNameInfo
            'ElseIf sFLName.Count = 1 Then
            '    MsgBox("Funky Name Fix Manually")
            '    fixPlayer = sNameInfo
        ElseIf sFLName.Count = 2 Then
            If sFLName(1).Length >= 3 Then
                Dim sPrefixIn As String = sFLName(1).Substring(0, 3)
                Dim sPrefixOut As String = sPrefixIn.Replace(sPrefixIn.Substring(2, 1), sPrefixIn.Substring(2, 1).ToUpper)
                If sPrefixIn.StartsWith("Mc") Then
                    fixPlayer = sNameInfo.ToString.Replace(sPrefixIn, sPrefixOut)
                Else
                    fixPlayer = sNameInfo
                End If
            End If
        End If

    End Function

    Private Function getPlayer(sTeam, sGrade) As String

        getPlayer = ""
        Dim sPlayersRowFilter = String.Format("Team ='{0}' AND Grade = '{1}' AND Date = '{2}'", sTeam, sGrade, dDate.ToString("yyyyMMdd"))
        Try

            Dim dvScores = New DataView(dsLeague.Tables("dtScores")) With
                {
                    .RowFilter = sPlayersRowFilter
                }
            If dvScores.Count > 0 Then
                getPlayer = dvScores(0)("Player")
            Else
                'this code checks to see if a player participated in EOY skins
                Dim dvp As New DataView(dsLeague.Tables("dtPlayers"))
                dvp.RowFilter = String.Format("Team = '{0}' And Grade = '{1}'", sTeam, sGrade)
                If dvp.Count <> 1 Then
                    MsgBox(String.Format("Team {0} is missing an {1} player, fix player file and try again", sTeam, sGrade))
                End If
                getPlayer = dvp(0)("Name")
            End If
        Catch ex As Exception
            MsgBox(GetExceptionInfo(ex))
        End Try

    End Function
    Private Function UpdatePartnerInScore(sthisPlayer As String, sOpponent As String, sDate As String, ip As Short) As Int16
        Dim sKey() As Object = {sthisPlayer.Split(",")(0), sDate}
        Dim drow = dsLeague.Tables("dtScores").Rows.Find(sKey)
        'Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
        'drow is nothing if this is a new score
        If drow Is Nothing Then
            Dim rowView As DataRowView = ScoreCard.dvScores.AddNew
            ' Change values in the DataRow.
            rowView("League") = sLeagueName
            rowView("Player") = sthisPlayer.Split(",")(0)
            rowView("Grade") = sthisPlayer.Split(",")(1)
            rowView("Group") = 0
            rowView("Team") = sTeam
            rowView("Date") = sDate
            rowView("Partner") = CStr(ip).PadLeft(2, "0")
            rowView("Opponent") = sOpponent.Split(",")(0)
            rowView("PHdcp") = GetPHdcp()
            rowView(closest) = False
            rowView(skin) = False
            rowView(CTP1) = ""
            rowView(CTP2) = ""
            rowView.EndEdit()
        Else
            drow("Partner") = CStr(ip).PadLeft(2, "0")
            drow("Opponent") = sOpponent.Split(",")(0)
            drow.EndEdit()
        End If
        UpdatePartnerInScore = ip + 1
    End Function
    Function GetPHdcp() As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        GetPHdcp = ""
        Try
            Dim ssql As String
            '20250413 - get the last handicap from the handicaps table 
            ssql = $"SELECT Hdcp FROM Handicaps"
            ssql &= $"{vbCrLf}WHERE League = '{ctx.SafeLeagueName}'"
            ssql &= $"{vbCrLf}AND Player = '{ctx.sPlayer}'"
            ssql &= $"{vbCrLf}AND Date < {ctx.ActiveDate}"
            ssql &= $"{vbCrLf}AND Hdcp <> ''"
            ssql &= $"{vbCrLf}ORDER BY Date DESC LIMIT 1"
            dt = sqlitedaFromSql(ctx.Conn, "", ssql)
            If dt.Rows.Count = 0 Then
                GetPHdcp = ""
            Else
                GetPHdcp = dt.Rows(0)("Hdcp")
            End If

            'LOGIT($"PHdcp for {sPlayer}-{GetPHdcp}")
        Catch ex As Exception
            MsgBox(GetExceptionInfo(ex))
        End Try
        Return GetPHdcp
    End Function
    Public Property IHdcp As Integer
        Get
            Return _iHdcp
        End Get
        Set(value As Integer)
            _iHdcp = value
        End Set
    End Property
    Function CalcLowScore(dtScores As DataTable, hole As String) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod().Name)
        Dim ilowRows As New List(Of String)
        Dim iLowScore As Integer = Integer.MaxValue

        Try
            For i As Integer = 0 To dtScores.Rows.Count - 1
                'If dtScores(i)(Player).ToString() = sTotalColumn Then Continue For

                Dim sThisPlayer As String = dtScores(i)(Player).ToString()
                Dim iHdcp As Integer = GetPlayerHandicap(dtScores(i))

                If Not HasSkinPayment(sThisPlayer) Then Continue For
                If Not IsNumeric(dtScores(i)(hole)) Then Continue For

                Dim iScore As Integer = CInt(dtScores(i)(hole))
                If iScore <= 0 Then Continue For

                iScore = AdjustScoreForHandicap(iScore, dtScores(i), hole, iHdcp)

                If iScore < iLowScore Then
                    iLowScore = iScore
                    ilowRows = New List(Of String) From {i.ToString()}
                ElseIf iScore = iLowScore Then
                    ilowRows.Add(i.ToString())
                End If
            Next

            CalcLowScore = $"{iLowScore}~{String.Join("|", ilowRows)}"
            LOGIT($"Hole {hole} {If(CalcLowScore = "", "No Scores", CalcLowScore)}")

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return CalcLowScore
    End Function
    Private Function GetPlayerHandicap(row As DataRow) As Integer
        If IsNumeric(row("PHdcp")) Then
            Return CInt(row("PHdcp"))
        ElseIf IsNumeric(row("Gross")) AndAlso IsNumeric(row("Net")) Then
            Return CInt(row("Gross")) - CInt(row("Net"))
        End If
        Return 0
    End Function

    Private Function HasSkinPayment(player As String) As Boolean
        Dim sb As New StringBuilder()
        sb.AppendLine($"SELECT * FROM Payments")
        sb.AppendLine($"WHERE League = '{ctx.SafeLeagueName}' '")
        sb.AppendLine($"AND Date = {ctx.ActiveDate}")
        sb.AppendLine($"AND Player = '{player}'")
        sb.AppendLine($"AND Desc IN ('Skin')")
        sb.AppendLine($"AND Detail = 'Payment'")

        Dim dt As DataTable = sqlitedaFromSql(ctx.Conn, "", sb.ToString())
        Return dt.Rows.Count > 0
    End Function

    Private Function AdjustScoreForHandicap(score As Integer, row As DataRow, hole As String, hcp As Integer) As Integer
        If ctx.rLeagueParmrow("Method") = "Gross" AndAlso ctx.rLeagueParmrow("SkinFmt") = "Handicap" Then
            If row("FrontBack") = "Back" Then hole += 9
            Dim si As Integer = CalcStrokeIndex(hole)
            If hcp >= si Then
                score -= 1
                If hcp - iHoles >= si Then score -= 1
            End If
        End If
        Return score
    End Function
    Function CalcSkins(dtScores As DataTable) As List(Of String)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod().Name)
        Dim skins As New List(Of String)

        Try
            For holeNum As Integer = 1 To 9
                Dim lowScoreRowIds As String = CalcLowScore(dtScores, holeNum.ToString())
                If Not String.IsNullOrEmpty(lowScoreRowIds) Then
                    skins.Add($"{holeNum}-{lowScoreRowIds}")
                End If
            Next
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return skins
    End Function
#Region "Rewritten Functions"
    Function FCalcLowScore(dtScores As DataTable, hole As String) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        FCalcLowScore = ""
        Try
            Dim ilowrow As New List(Of String)
            Dim ilowscore = 99
            'calculate a column saving low score
            For i = 0 To dtScores.Rows.Count - 1
                Dim sGross As Object = Nothing
                Dim sNet As Object = Nothing
                If dtScores(i)(Player) = sTotalColumn Then Continue For
                Dim sthisPlayer = dtScores(i)(Player)
                If IsNumeric(dtScores(i)("PHdcp")) Then
                    IHdcp = dtScores(i)("PHdcp")
                Else
                    IHdcp = sGross - sNet
                    'dtScores(i)("PHdcp") = IHdcp
                End If
                '
                sb = New StringBuilder
                sb.AppendLine($"
SELECT * FROM Payments
WHERE League = '{ctx.SafeLeagueName}' '
AND Date = {ctx.ActiveDate}
AND Player = '{sthisPlayer}'
AND Desc IN ('Skin')
AND Detail = 'Payment'
")
                dt = sqlitedaFromSql(ctx.Conn, "", sb.ToString)

                If dt.Rows.Count > 0 Then
                    'If dvScores(i)("Hole" & hole) IsNot DBNull Then
                    If IsNumeric(dtScores(i)(hole)) Then
                        Dim iscore As String = dtScores(i)(hole)
                        '20250530 - if the score is 0, skip it
                        If iscore <= 0 Then
                            Continue For
                        End If
                        'this means its a scorecard
                        '2020-01-15- 20180529 Ben Wright played 1 hole
                        If ctx.rLeagueParmrow("Method") = "Gross" And ctx.rLeagueParmrow("SkinFmt") = "Handicap" Then
                            Dim isi As Int16 = CalcStrokeIndex(hole)
                            If CInt(dtScores(i)("pHdcp")) >= isi Then
                                'check stroke index
                                iscore -= 1
                                If CInt(dtScores(i)("pHdcp") - iHoles) >= isi Then iscore -= 1
                            End If
                        End If

                        If IsNumeric(iscore) Then
                            If iscore < ilowscore Then
                                ilowscore = iscore
                                ilowrow = New List(Of String)
                                ilowrow.Add(i)
                            ElseIf iscore = ilowscore Then
                                ilowrow.Add(i)
                            End If
                        End If
                    End If
                End If
            Next
            For Each score In ilowrow
                FCalcLowScore &= score & "|"
            Next
            FCalcLowScore = FCalcLowScore.TrimEnd("|")
            'If ilowrow.Count = 1 Then FCalcLowScore = ilowrow(0)
            LOGIT($"Hole {hole} {If(FCalcLowScore = "", "No Scores", FCalcLowScore)}")

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    'Function newFCalcSkins(dtScores As DataTable) As List(Of String)
    '    LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    'Dim dvscoresNet As DataView

    '    'this code goes through the listview and highlights the lowest value on each hole and fron 9, back 9 and total
    '    FCalcSkins = New List(Of String)
    '    Try
    '        '20250530-change for league champtionship
    '        'Dim ifirstHole As Integer = iHoleMarker
    '        'Dim ilastHole As Integer = iHoleMarker + 9
    '        'If ctx.ActiveDate >= sPSDate Then
    '        '    ifirstHole = 1
    '        '    ilastHole = 19
    '        'End If
    '        For ii = 1 To 9
    '            ' Find the lowest value in the "Value" column
    '            Dim ilowscore As String = FCalcLowScore(dtScores, ii)
    '            If ilowscore <> "" Then FCalcSkins.Add(ii & "-" & ilowscore)
    '        Next
    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try

    'End Function


#End Region
    'The trick is to declare the new Random class outside of the function that retrieves the next random number. 
    'This way you generate the seed only once and are getting the “randomizer” formula to cycle through its formula and ensure the next chosen number is truly random.
    'Here’s my code. 
    'Note that you no longer have to declare new objects (such as objRandom, here) at the top of your class or module; 
    'you can do it just above the function, to aid clarity of code:
    Dim objRandom As New System.Random(
CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))

    Public Function GetRandomNumber(
    Optional ByVal Low As Integer = 1,
    Optional ByVal High As Integer = 100) As Integer
        ' Returns a random number,
        ' between the optional Low and High parameters
        Return objRandom.Next(Low, High + 1)
    End Function
    Public Function CDateToyyyyMMdd(sDate)
        CDateToyyyyMMdd = CDate(sDate).ToString("yyyyMMdd")
    End Function
    Function ScreenResize() As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        ScreenResize = ScreenResize(Main.iScreenWidth, Main.iScreenHeight)
    End Function
    Function ScreenResize(sprefwidth, sprefheight) As String
        ScreenResize = ""
        Dim Main As Object = HughsGolf.Main
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '700 X 1550
            Dim x = Main.iScreenWidth
            If Main.iScreenWidth < sprefwidth Then
                ScreenResize = Main.iScreenWidth
            Else
                ScreenResize = sprefwidth
            End If
            If Main.iScreenHeight < sprefheight Then
                ScreenResize = ScreenResize & ":" & Main.iScreenHeight
            Else
                ScreenResize = ScreenResize & ":" & sprefheight
            End If
            'If Main.iScreenWidth = 1920 Then
            '    ScreenResize = sprefwidth
            'ElseIf Main.iScreenWidth = 1366 Then
            '    ScreenResize = "1150"
            'ElseIf Main.iScreenWidth = 2560 Then
            '    ScreenResize = "1150"
            'End If
            'If Main.iScreenHeight = 1200 Or Main.iScreenHeight = 1400 Then Me.Height = 650
            'test for Greg 1366 x 768
            'Me.Width = 1200
            'ScreenResize = ScreenResize & ":" & "650"
            'rs.ResizeAllControls(Me)
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Public Function SumAmts(dt As DataTable, fld As String, sdate As String) As Decimal
        SumAmts = 0
        Try
            'SumAmts = Convert.ToInt32(dt.Compute(String.Format("SUM({0})", fld), String.Format("Date = {0} and {1} > 0", sdate, fld)))
            SumAmts = Convert.ToInt32(dt.Compute(String.Format("SUM({0})", fld), String.Format("Date = {0} ", sdate, fld)))
        Catch ex As Exception

        End Try
    End Function
    ', SUM(IIF(Skins = 'TRUE' or Skins = 'True', 1, 0)) as SkinPlayers
    Public Function tallysumif(fld As String, cflds As String, ofld As String) As String
        Dim cfld As String = ""
        For Each cflditem In cflds.Split(",")
            cfld &= fld & String.Format(" = '{0} ' OR ", cflditem)
        Next
        cfld = cfld.Substring(0, cfld.LastIndexOf(" OR"))
        tallysumif = String.Format("SUM(IIF({0}, 1 , 0)) as {1}", cfld, ofld)

    End Function
    Sub Fixextra(fld1, fld2)
        LOGIT(fld1 & "-" & fld2)
        'if not whole dollar, add cents to extra
        If fld1 Mod 1 <> 0 Then
            LOGIT("Before " & fld1 & "-" & fld2)
            'add the cents
            fld2 += fld1 Mod 1
            fld1 -= fld1 Mod 1
            LOGIT("After " & fld1 & "-" & fld2)
        End If
    End Sub
    Public Sub Resizedgv(dgv As DataGridView, frm As Form)
        Dim iw As Integer = 0, ih As Integer = 0
        For Each col As DataGridViewColumn In dgv.Columns
            If Not col.Visible Then Continue For
            iw += col.Width
            'LOGIT(String.Format("col {0}-{1}", col.Name, col.Width))
        Next
        For Each row As DataGridViewRow In dgv.Rows
            ih += row.Height
        Next
        LOGIT(String.Format("dgv {0}x{1}", iw, ih))
        dgv.Width = iw '* 1.1
        dgv.Height = (ih + dgv.ColumnHeadersHeight) '* 1.1
        If dgv.ScrollBars = ScrollBars.Both Then
            dgv.Width = iw + 25
            dgv.Height = ih + 35
        End If
        'If frm.Width > dgv.Width Then frm.Width = dgv.Width * 1.1
        'If frm.Height > dgv.Height Then frm.Height = dgv.Height * 1.1
    End Sub
#Region "DGV to HTML"
    Function Create_Html_From_DGV(dt As DataGridView) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'Building an HTML string.
        Dim html As New StringBuilder()
        Try
            'Populating a DataTable from database.

            'Table start.
            'html.Append("<table border = '1'>")
            html.Append("<table border='1px' cellpadding='5' cellspacing='0' ")
            html.Append("style='border: solid 1px Silver; font-size: x-small;'>")

            'Building the Header row.
            html.Append("<tr>")
            For Each column As DataGridViewColumn In dt.Columns
                html.Append("<th>")
                html.Append(column.Name)
                html.Append("</th>")
            Next
            html.Append("</tr>")

            'Building the Data rows.
            For Each row As DataGridViewRow In dt.Rows
                html.Append("<tr>")
                For Each column As DataGridViewColumn In dt.Columns
                    html.Append("<td>")
                    html.Append(row.Cells(column.Name))
                    html.Append("</td>")
                Next
                html.Append("</tr>")
            Next

            'Table end.
            html.Append("</table>")

            'Append the HTML string to Placeholder.
            '        PlaceHolder1.Controls.Add(New Literal() With {
            '  .Text = html.ToString()
            '})
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return html.ToString
    End Function
    Function Create_Html(dt As DataTable) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'Populating a DataTable from database.

        'Building an HTML string.
        Dim html As New StringBuilder()

        'Table start.
        'html.Append("<table border = '1'>")
        html.Append("<table border='1px' cellpadding='5' cellspacing='0' ")
        html.Append("style='border: solid 1px Silver; font-size: x-small;'>")

        'Building the Header row.
        html.Append("<tr>")
        For Each column As DataColumn In dt.Columns
            html.Append("<th>")
            html.Append(column.ColumnName)
            html.Append("</th>")
        Next
        html.Append("</tr>")

        'Building the Data rows.
        For Each row As DataRow In dt.Rows
            html.Append("<tr>")
            For Each column As DataColumn In dt.Columns
                html.Append("<td>")
                html.Append(row(column.ColumnName))
                html.Append("</td>")
            Next
            html.Append("</tr>")
        Next

        'Table end.
        html.Append("</table>")

        'Append the HTML string to Placeholder.
        '        PlaceHolder1.Controls.Add(New Literal() With {
        '  .Text = html.ToString()
        '})
        Return html.ToString
    End Function

    Public Function ConvertDataGridViewToHTMLWithFormatting(ByVal dgv As DataGridView, ByVal wf As Form) As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        ConvertDataGridViewToHTMLWithFormatting = ""
        Dim sb As StringBuilder = New StringBuilder()
        Try
            sb.AppendLine("<html><body><center><table border='1' cellpadding='0' cellspacing='0'>")
            '20190822-add circle css for holes with birdies
            Dim sHead = "<style type = ""text/css"" > .round{ -moz-border-radius: 20px;border-radius:   20px; padding: 5px;border: 1px solid #000;} _
                                                            .square {width: 20px;height: 20px;background green;} _ 
                                                            #box {width: 20px;border: green 2px;} _ 
                              </style>"
            sb.AppendLine(sHead)

            sb.AppendLine("<tr>")
            For i As Integer = 0 To dgv.Columns.Count - 1
                If dgv.Columns(i).HeaderText = "Group" Or dgv.Columns(i).HeaderText = "Clear" Then Continue For
                sb.Append(DGVHeaderCellToHTMLWithFormatting(dgv, i))
                'sb.Append(DGVCellFontAndValueToHTML(dgv.Columns(i).HeaderText, dgv.Columns(i).HeaderCell.Style.Font))
                sb.Append(DGVCellFontAndValueToHTML(dgv.Columns(i).HeaderText, dgv.ColumnHeadersDefaultCellStyle.Font, wf))
                sb.AppendLine("</td>")
            Next

            sb.AppendLine("</tr>")
            For rowIndex As Integer = 0 To dgv.Rows.Count - 1
                '20190904 - put in to debug why stats arent being included
                'If dgv.Rows(rowIndex).Cells("Method").Value IsNot DBNull.Value Then
                '    If dgv.Rows(rowIndex).Cells("Method").Value = "2019" Then
                '        Dim x = ""
                '    End If
                'End If
                sb.AppendLine("<tr>")
                'Dim sthisplayer = ""
                For Each dgvc As DataGridViewCell In dgv.Rows(rowIndex).Cells
                    If dgvc.OwningColumn.HeaderText = "Group" Or dgvc.OwningColumn.HeaderText = "Clear" Then Continue For
                    'If dgvc.ColumnIndex = 0 Then sthisplayer = dgvc.Value
                    sb.AppendLine(DGVCellToHTMLWithFormatting(dgv, rowIndex, dgvc.ColumnIndex, wf))
                    Dim cellValue As String = If(dgvc.Value Is Nothing, String.Empty, dgvc.Value.ToString())
                    Dim bMarkit As Boolean = False
                    If dgvc.OwningColumn.Name.Contains("Hole") Then
                        If wf.Name = "Scores" Then
                            If IsNumeric(RemoveSpcChar(cellValue)) Then
                                If RemoveSpcChar(cellValue) < MyCourse(0)(String.Format("Hole{0}", dgvc.OwningColumn.HeaderText)) And
                                    Not cellValue.Contains(".") And
                                     (dgv.Rows(rowIndex).Cells(0).Value = "Gross" Or
                                     dgv.Rows(rowIndex).Cells(0).Value = "Net") Then
                                    'Or IsNumeric(dgv.Rows(rowIndex).Cells(0).Value)
                                    bMarkit = True
                                End If
                            End If
                        ElseIf wf.Name = "frmScoreCard" Then
                            If IsNumeric(RemoveSpcChar(cellValue)) Then
                                If RemoveSpcChar(cellValue) < MyCourse(0)(String.Format("Hole{0}", dgvc.OwningColumn.HeaderText)) Then
                                    bMarkit = True
                                End If
                            End If
                        ElseIf wf.Name = "Skins" Then
                            If IsNumeric(RemoveSpcChar(cellValue)) Then
                                If RemoveSpcChar(cellValue) < MyCourse(0)(String.Format("Hole{0}", dgvc.OwningColumn.HeaderText)) Then
                                    bMarkit = True
                                End If
                            End If
                        End If
                    Else
                        bMarkit = False
                    End If

                    If bMarkit Then
                        'circle-works
                        sb.AppendLine(String.Format("<span Class=""round"">{0}</span>", DGVCellFontAndValueToHTML(RemoveSpcChar(cellValue), dgvc.Style.Font, wf)))
                        'square-doesnt work
                        'sb.AppendLine(String.Format("<div class=""square"">{0}</div>", DGVCellFontAndValueToHTML(RemoveSpcChar(cellValue), dgvc.Style.Font, wf)))
                        'box
                        'sb.AppendLine(String.Format("<div id=""box"">{0}</div>", DGVCellFontAndValueToHTML(RemoveSpcChar(cellValue), dgvc.Style.Font, wf)))
                        'colors in the box red
                        'sb.AppendLine(String.Format("<span style=""background:red;"">{0}</span>", DGVCellFontAndValueToHTML(RemoveSpcChar(cellValue), dgvc.Style.Font, wf)))
                        'puts a red square around the cell
                        'sb.AppendLine(String.Format("<p style=""border:3px; border-style:solid;border-color:red;padding;lem;"">{0}</p>", DGVCellFontAndValueToHTML(RemoveSpcChar(cellValue), dgvc.Style.Font, wf)))
                    Else
                        sb.AppendLine(DGVCellFontAndValueToHTML(RemoveSpcChar(cellValue), dgvc.Style.Font, wf))
                    End If
                    sb.AppendLine("</td>")
                Next

                sb.AppendLine("</tr>")
            Next

            sb.AppendLine("</table></center></body></html>")
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Return sb.ToString()

    End Function

    Public Function DGVHeaderCellToHTMLWithFormatting(ByVal dgv As DataGridView, ByVal col As Integer) As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim sb As StringBuilder = New StringBuilder()
        Try
            sb.Append("<td")
            sb.Append(" width=" & Chr(34) & dgv.Columns(col).Width & Chr(34) & " ")
            'Dim sHdr = dgv.Columns(col).HeaderText
            'If sHdr.Contains("Half") Or sHdr.Contains("Points") Or sHdr.Contains("/") Then
            '    sb.Append(" width=" & Chr(34) & "40" & Chr(34) & " ")
            'End If

            sb.Append(DGVCellColorToHTML(dgv.Columns(col).HeaderCell.Style.ForeColor, dgv.Columns(col).HeaderCell.Style.BackColor))
            sb.Append(DGVCellAlignmentToHTML(dgv.Columns(col).HeaderCell.Style.Alignment))
            sb.Append(">")
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Return sb.ToString()
    End Function

    Public Function DGVCellToHTMLWithFormatting(ByVal dgv As DataGridView, ByVal row As Integer, ByVal col As Integer, ByVal wf As Form) As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Dim sb As StringBuilder = New StringBuilder()
        Try
            sb.Append("<td")
            'If dgv.Rows(row).DefaultCellStyle.BackColor <> wf.DefaultBackColor Then
            '    sb.Append(DGVCellColorToHTML(dgv.Rows(row).DefaultCellStyle.ForeColor, dgv.Rows(row).DefaultCellStyle.BackColor))
            'Else
            '    sb.Append(DGVCellColorToHTML(dgv.Rows(row).Cells(col).Style.ForeColor, dgv.Rows(row).Cells(col).Style.BackColor))
            'End If
            sb.Append(DGVCellColorToHTML(dgv.Rows(row).Cells(col).Style.ForeColor, dgv.Rows(row).Cells(col).Style.BackColor))

            sb.Append(DGVCellAlignmentToHTML(dgv.Rows(row).Cells(col).Style.Alignment))
            sb.Append(">")
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return sb.ToString()
    End Function

    Public Function DGVCellColorToHTML(ByVal foreColor As Color, ByVal backColor As Color) As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        If foreColor.Name = "0" AndAlso backColor.Name = "0" Then Return String.Empty
        Dim sb As StringBuilder = New StringBuilder()
        Try
            sb.Append(" style=""")
            If foreColor.Name <> "0" AndAlso backColor.Name <> "0" Then
                sb.Append("color:#")
                sb.Append(foreColor.R.ToString("X2") + foreColor.G.ToString("X2") + foreColor.B.ToString("X2"))
                sb.Append("; background-color:#")
                sb.Append(backColor.R.ToString("X2") + backColor.G.ToString("X2") + backColor.B.ToString("X2"))
            ElseIf foreColor.Name <> "0" AndAlso backColor.Name = "0" Then
                sb.Append("color:#")
                sb.Append(foreColor.R.ToString("X2") + foreColor.G.ToString("X2") + foreColor.B.ToString("X2"))
            Else
                sb.Append("background-color:#")
                sb.Append(backColor.R.ToString("X2") + backColor.G.ToString("X2") + backColor.B.ToString("X2"))
            End If

            sb.Append(";""")
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return sb.ToString()
    End Function

    Public Function DGVCellFontAndValueToHTML(ByVal value As String, ByVal font As Font, ByVal wf As Form) As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'If font Is Nothing OrElse font = Me.Font AndAlso Not (font.Bold Or font.Italic Or font.Underline Or font.Strikeout) Then Return value
        Dim sb As StringBuilder = New StringBuilder()
        Try
            If font Is Nothing OrElse Not (font.Bold Or font.Italic Or font.Underline Or font.Strikeout) Then Return value
            sb.Append(" ")
            If font.Bold Then sb.Append("<b>")
            If font.Italic Then sb.Append("<i>")
            If font.Strikeout Then sb.Append("<strike>")
            If font.Underline Then sb.Append("<u>")
            Dim size As String = String.Empty
            If font.Size <> wf.Font.Size Then size = "font-size: " & font.Size & "pt;"
            If font.FontFamily.Name <> wf.Font.Name Then
                sb.Append("<span style=""font-family: ")
                sb.Append(font.FontFamily.Name)
                sb.Append("; ")
                sb.Append(size)
                sb.Append(""">")
            End If

            'put a circle around a number
            '
            '    .btn-circle {
            'width: 30px;
            'height: 30px;
            'padding: 6px 0px;
            'border-radius:  15px;
            'Text-align: center;
            'font-size:  12px;
            'line-height:  1.42857;

            '    <div Class="panel-body">
            '                            <h4> Normal Circle Buttons</h4>
            '                            <Button type = "button" Class="btn btn-default btn-circle"><i Class="fa fa-check"></i>
            '                            </button>
            '                            <Button type = "button" Class="btn btn-primary btn-circle"><i Class="fa fa-list"></i>
            '                            </button>
            '</div>

            sb.Append(value)
            If font.FontFamily.Name <> wf.Font.Name Then sb.Append("</span>")
            If font.Underline Then sb.Append("</u>")
            If font.Strikeout Then sb.Append("</strike>")
            If font.Italic Then sb.Append("</i>")
            If font.Bold Then sb.Append("</b>")
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return sb.ToString()
    End Function

    Public Function DGVCellAlignmentToHTML(ByVal align As DataGridViewContentAlignment) As String
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim sb As StringBuilder = New StringBuilder()
        Try
            If align = DataGridViewContentAlignment.NotSet Then Return String.Empty
            Dim horizontalAlignment As String = String.Empty
            Dim verticalAlignment As String = String.Empty
            CellAlignment(align, horizontalAlignment, verticalAlignment)
            sb.Append(" align='")
            sb.Append(horizontalAlignment)
            sb.Append("' valign='")
            sb.Append(verticalAlignment)
            sb.Append("'")
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        Return sb.ToString()
    End Function

    Private Sub CellAlignment(ByVal align As DataGridViewContentAlignment, ByRef horizontalAlignment As String, ByRef verticalAlignment As String)
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Select Case align
                Case DataGridViewContentAlignment.MiddleRight
                    horizontalAlignment = "right"
                    verticalAlignment = "middle"
                Case DataGridViewContentAlignment.MiddleLeft
                    horizontalAlignment = "left"
                    verticalAlignment = "middle"
                Case DataGridViewContentAlignment.MiddleCenter
                    horizontalAlignment = "centre"
                    verticalAlignment = "middle"
                Case DataGridViewContentAlignment.TopCenter
                    horizontalAlignment = "centre"
                    verticalAlignment = "top"
                Case DataGridViewContentAlignment.BottomCenter
                    horizontalAlignment = "centre"
                    verticalAlignment = "bottom"
                Case DataGridViewContentAlignment.TopLeft
                    horizontalAlignment = "left"
                    verticalAlignment = "top"
                Case DataGridViewContentAlignment.BottomLeft
                    horizontalAlignment = "left"
                    verticalAlignment = "bottom"
                Case DataGridViewContentAlignment.TopRight
                    horizontalAlignment = "right"
                    verticalAlignment = "top"
                Case DataGridViewContentAlignment.BottomRight
                    horizontalAlignment = "right"
                    verticalAlignment = "bottom"
                Case Else
                    horizontalAlignment = "left"
                    verticalAlignment = "middle"
            End Select
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub CreateHtmlFromDGV(ByVal dgv As DataGridView, ByVal sScreen As String, wf As System.Windows.Forms.Form, lbStatus As Label)
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '20190824-create html of schedule
            With lbStatus
                Dim sfn = sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) & String.Format("_{0}.csv", sScreen)
                .Text = String.Format("Creating spreadsheet({0}) of {1} from this screen...", sfn, sScreen)
                status_Msg(lbStatus, wf)
                dgv2csv(dgv, sfn)
                '20190822 - new html
                Dim sHtml As String = Create_Html_From_DGV(dgv)
                sHtml = ConvertDataGridViewToHTMLWithFormatting(dgv, wf)
                Dim swhtml As New IO.StreamWriter(sfn.Replace(".csv", ".html"), False)
                swhtml.WriteLine(sHtml)
                swhtml.Close()
                .Text = String.Format("Finished creating {0} spreadsheet from this screen", sScreen)
                status_Msg(lbStatus, wf)
                '.Text = String.Format("Finished Calculating {0}", sScreen)
                'status_Msg(lbStatus, wf)
            End With
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
#End Region
    Sub AddColumnToDGV(dgv As DataGridView, sName As String, iLength As Integer, iWidth As Integer, bRO As Boolean)
        Dim dgc As New DataGridViewTextBoxColumn
        With dgv
            With dgc
                .Name = sName
                .MaxInputLength = iLength
                .ValueType = GetType(System.String)
                .HeaderText = .Name
                .Width = iWidth
                .ReadOnly = bRO
            End With
            .Columns.Add(dgc)
            .Width += dgc.Width
        End With

    End Sub
    Public Class Holiday
        Public Property DateValue As Date
        Public Property Name As String
        Public Property Description As String
        Public Property BusinessesClosed As Boolean

        Public ReadOnly Property DayOfWeek As String
            Get
                Return DateValue.ToString("dddd")
            End Get
        End Property

        Public Function VerbalSummary() As String
            Dim closureNote As String = If(BusinessesClosed, "Most businesses are closed.", "Most businesses remain open.")
            Return $"On {DayOfWeek}, {DateValue:MMMM dd, yyyy}, we observe {Name}. {Description} {closureNote}"
        End Function
    End Class
    Public Function DayOfWeekToNumber(dow As DayOfWeek) As Integer
        ' VB.NET DayOfWeek enum: Sunday = 0, Monday = 1, ..., Saturday = 6
        Return ((CInt(dow) + 6) Mod 7) + 1
    End Function
    Public Function DayNameToNumber(dayName As String) As Integer
        Dim days As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase) From {
        {"Monday", 1},
        {"Tuesday", 2},
        {"Wednesday", 3},
        {"Thursday", 4},
        {"Friday", 5},
        {"Saturday", 6},
        {"Sunday", 7}
    }

        If days.ContainsKey(dayName) Then
            Return days(dayName)
        Else
            Throw New ArgumentException($"Invalid day name: {dayName}")
        End If
    End Function
    Public Function GetHolidayList(ByVal vYear As Integer, sDayOfWeek As String) As List(Of Holiday)
        Dim FirstWeek As Integer = 1
        Dim SecondWeek As Integer = 2
        Dim ThirdWeek As Integer = 3
        Dim FourthWeek As Integer = 4
        Dim LastWeek As Integer = 5

        Dim HolidayList As New List(Of Holiday)
        If DayNameToNumber(sDayOfWeek) <> DayOfWeek.Monday Then
            GoTo skipMon
            HolidayList.Add(New Holiday With {
    .DateValue = GetNthDayOfNthWeek(DateSerial(vYear, 1, 1), DayOfWeek.Monday, ThirdWeek),
    .Name = "Martin Luther King Jr. Day",
    .Description = "Honors the civil rights leader Martin Luther King Jr.",
    .BusinessesClosed = False
})

            HolidayList.Add(New Holiday With {
    .DateValue = GetNthDayOfNthWeek(DateSerial(vYear, 2, 1), DayOfWeek.Monday, ThirdWeek),
    .Name = "Presidents Day",
    .Description = "Honors all U.S. presidents, especially Washington and Lincoln.",
    .BusinessesClosed = False
})
skipmlk:
            HolidayList.Add(New Holiday With {
    .DateValue = GetNthDayOfNthWeek(DateSerial(vYear, 5, 1), DayOfWeek.Monday, LastWeek),
    .Name = "Memorial Day",
    .Description = "Remembers those who died in military service.",
    .BusinessesClosed = True
})
            HolidayList.Add(New Holiday With {
    .DateValue = GetNthDayOfNthWeek(DateSerial(vYear, 9, 1), DayOfWeek.Monday, FirstWeek),
    .Name = "Labor Day",
    .Description = "Celebrates the contributions of American workers.",
    .BusinessesClosed = True
})
            GoTo skipMon
            HolidayList.Add(New Holiday With {
    .DateValue = GetNthDayOfNthWeek(DateSerial(vYear, 10, 1), DayOfWeek.Monday, SecondWeek),
    .Name = "Columbus Day",
    .Description = "Commemorates Christopher Columbus's arrival in the Americas.",
    .BusinessesClosed = False
})
skipMon:
        ElseIf DayNameToNumber(sDayOfWeek) <> Day.Thursday Then
            GoTo skipThurs
            HolidayList.Add(New Holiday With {
        .DateValue = GetNthDayOfNthWeek(DateSerial(vYear, 11, 1), DayOfWeek.Thursday, FourthWeek),
        .Name = "Thanksgiving Day",
        .Description = "Celebrates gratitude and the harvest season.",
        .BusinessesClosed = True
    })
skipThurs:
        End If

        'only include holidays when businesses are closed
        If AdjustWeekend(DateSerial(vYear, 1, 1)).DayOfWeek = DayNameToNumber(sDayOfWeek) Then
            HolidayList.Add(New Holiday With {
            .DateValue = AdjustWeekend(DateSerial(vYear, 1, 1)),
            .Name = "New Year's Day",
            .Description = "Celebrates the beginning of the calendar year.",
            .BusinessesClosed = True
        })
        End If

        If AdjustWeekend(DateSerial(vYear, 7, 4)).DayOfWeek = DayNameToNumber(sDayOfWeek) Then
            HolidayList.Add(New Holiday With {
            .DateValue = AdjustWeekend(DateSerial(vYear, 7, 4)),
            .Name = "Independence Day",
            .Description = "Commemorates the Declaration of Independence in 1776.",
            .BusinessesClosed = True
        })
        End If

        GoTo skipVet
        If AdjustWeekend(DateSerial(vYear, 11, 11)).DayOfWeek = DayNameToNumber(sDayOfWeek) Then
            HolidayList.Add(New Holiday With {
            .DateValue = AdjustWeekend(DateSerial(vYear, 11, 11)),
            .Name = "Veterans Day",
            .Description = "Honors military veterans of the United States.",
            .BusinessesClosed = False
        })
        End If
skipVet:
        If AdjustWeekend(DateSerial(vYear, 12, 25)).DayOfWeek = DayNameToNumber(sDayOfWeek) Then
            HolidayList.Add(New Holiday With {
            .DateValue = AdjustWeekend(DateSerial(vYear, 12, 25)),
            .Name = "Christmas Day",
            .Description = "Celebrates the birth of Jesus Christ.",
            .BusinessesClosed = True
        })
        End If

        Return HolidayList
    End Function

    Private Function AdjustWeekend(dt As Date) As Date
        If dt.DayOfWeek = DayOfWeek.Saturday Then
            Return dt.AddDays(-1)
        ElseIf dt.DayOfWeek = DayOfWeek.Sunday Then
            Return dt.AddDays(1)
        Else
            Return dt
        End If
    End Function

    Private Function GetNthDayOfNthWeek(startDate As Date, dayOfWeek As DayOfWeek, weekNumber As Integer) As Date
        Dim count As Integer = 0
        Dim dt As Date = startDate
        While dt.Month = startDate.Month
            If dt.DayOfWeek = dayOfWeek Then
                count += 1
                If count = weekNumber Then Exit While
            End If
            dt = dt.AddDays(1)
        End While
        Return dt
    End Function

    Private Function GetNthDayOfNthWeek(ByVal dt As Date, ByVal DayofWeek As Integer, ByVal WhichWeek As Integer) As Date
        'specify which day of which week of a month and this function will get the date
        'this function uses the month and year of the date provided

        'get first day of the given date
        Dim dtFirst As Date = DateSerial(dt.Year, dt.Month, 1)

        'get first DayOfWeek of the month
        Dim dtRet As Date = dtFirst.AddDays(6 - dtFirst.AddDays(-(DayofWeek + 1)).DayOfWeek)

        'get which week
        dtRet = dtRet.AddDays((WhichWeek - 1) * 7)

        'if day is past end of month then adjust backwards a week
        If dtRet >= dtFirst.AddMonths(1) Then
            dtRet = dtRet.AddDays(-7)
        End If

        'return
        Return dtRet

    End Function
    Public Function RemoveSpcChar(ByVal chr As String) As String
        Try
            RemoveSpcChar = chr.ToString.Replace(ChrW(&H25CF), String.Empty)
        Catch ex As Exception

        End Try

    End Function
    Public Function RemoveNulls(sfld) As String
        Return IIf(IsDBNull(sfld), "", sfld)
    End Function

    Public Function UpdateINI() As Boolean
        UpdateINI = False
        Try

            Using sw As New IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\Leaguemanager.ini", False)
                sw.WriteLine("LeagueName=" & sLeagueName)
                sw.WriteLine("GroupNumber=" & sGroupNumber)
                sw.WriteLine("Date=" & dDate.ToString("MM-dd-yyyy"))
                sw.WriteLine("FilePath=" & sFilePath)
                sw.WriteLine("ReportPath=" & sReportPath)
                If bloghelper Then
                    sw.WriteLine("Logging=Y")
                Else
                    sw.WriteLine("Logging=N")
                End If
                If bDateOverlap Then
                    sw.WriteLine("DateOverlapReminder=Y")
                Else
                    sw.WriteLine("DateOverlapReminder=N")
                End If
            End Using
            UpdateINI = True
            'MsgBox(String.Format("FilePath={0},ReportsPath={1} saved in {2}", sFilePath, sReportPath, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)))
        Catch ex As Exception
            LOGIT($"{Reflection.MethodBase.GetCurrentMethod.Name}-{ex.Message}{vbCrLf}{ex.StackTrace}")
            UpdateINI = False
        End Try
    End Function
    Public Sub Common_Exit()
        bexit = True
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If UpdateINI() = False Then Throw New Exception("Error Updating " & sFilePath & "\Leaguemanager.ini")

        Catch ex As Exception
            MsgBox("Close the file " & (sFilePath & "\Leaguemanager.ini" & vbCrLf & "Try again"))
        End Try

    End Sub

    Sub MakeCellsStrings(row As DataGridViewRow)
        'this forces each cell to be string prevent errors on resorting columns
        For Each cell As DataGridViewCell In row.Cells
            If cell.Value Is DBNull.Value Then cell.Value = ""
            If cell.FormattedValueType.Name = "String" Then cell.Value = CStr(cell.Value)

            'LOGIT(cell.OwningColumn.Name & "-" & cell.Value & "-" & cell.FormattedValueType.Name)
        Next
    End Sub

    Private Function getFilesInUse(ByVal strFile As String) As ArrayList

        myprocessarray.Clear()
        Dim processes As Process() = Process.GetProcesses()
        Dim i As Integer = 0

        For i = 0 To processes.GetUpperBound(0) - 1
            myProcess = processes(i)
            'Dim x = myProcess.StandardOutput.Read
            'LOGIT(myProcess.ProcessName & "-" & myProcess.MainWindowTitle)
            'If myProcess.ProcessName.ToLower.Contains("wps") Then
            '    Continue For
            'End If
            'Dim myprocess1 As New Process
            ''Program you want to launch execute etc.
            'myprocess1.StartInfo.FileName = "c:\windows\system32\openfiles.exe"
            'myprocess1.StartInfo.Arguments = "/query /s " + strFile + " /v"

            ''This is important. Since this is a windows service it will run
            ''even though no one is logged in.
            ''Therefore there is not desktop available so you better
            ''not show any windows dialogs
            'myprocess1.StartInfo.UseShellExecute = False
            'myprocess1.StartInfo.CreateNoWindow = True
            ''We want to redirect the output from the openfiles call to the program
            ''Since there won't be any window to display it in
            'myprocess1.StartInfo.RedirectStandardOutput = True

            'Dim tmpstr2 As String = String.Empty
            'Dim values(6) As Object 'This storeds the fields from openfiles
            'Dim values2(0) As Object 'This is the current date
            'values2(0) = DateTime.Now
            'Dim cnt As Integer = 0

            'Do
            '    tmpstr2 = myprocess1.StandardOutput.ReadLine
            '    ' Add some text to the file.
            '    If Not (tmpstr2 Is Nothing) Then
            '        cnt += 1
            '        'The output is fixed length
            '        If cnt > 5 Then
            '            values(0) = tmpstr2.Substring(0, 15).Trim 'Host name
            '            values(1) = tmpstr2.Substring(16, 8).Trim 'ID
            '            values(2) = tmpstr2.Substring(25, 20).Trim 'accessed by
            '            values(3) = tmpstr2.Substring(46, 10).Trim 'type
            '            values(4) = tmpstr2.Substring(57, 10).Trim 'locks
            '            values(5) = tmpstr2.Substring(68, 15).Trim 'open mode
            '            values(6) = tmpstr2.Substring(84) 'open file
            '        End If
            '    End If

            'Loop Until tmpstr2 Is Nothing

            If myProcess.Threads.Count > 0 Then

                Try
                    Dim modules As ProcessModuleCollection = myProcess.Modules
                    Dim j As Integer = 0

                    For j = 0 To modules.Count - 1

                        If (modules(j).FileName.ToLower().CompareTo(strFile.ToLower()) = 0) Then
                            myprocessarray.Add(myProcess)
                            Exit For
                        End If
                    Next

                Catch exception As Exception
                End Try
            End If
        Next

        Return myprocessarray
    End Function

    Public Function TextNumber(sName As String) As String

        TextNumber = ""
        Dim CellCarriers As String() = {"ATT:@txt.att.net", "VERIZON:@vtext.com", "SPRINT:@messaging.sprintpcs.com", "TMOBILE:@tmomail.net", "BOOST:@myboostmobile.com", "CBW:@gocbw.com"}

        dt = sqlitedaFromSql(ctx.Conn, "", $"select * from Players where Player = '{sName}'")

        'messaging.sprintpcs.com
        'tmomail.net

        For Each cell In CellCarriers
            If cell.Split(":")(0).ToUpper = dt.Rows(0)("CellCarrier").ToString.ToUpper Then
                Return $"{dt.Rows(0)("phone").ToString.Replace("-", "")}{cell.Split(":")(1)}"
            End If
        Next
    End Function
    Public Sub CreateDGBtn(dgScores As DataGridView,
                           sColName As String,
                           sPrecColName As String,
                           Optional ByVal BtnText As String = "",
                           Optional ByVal sColor As Color = Nothing)

        LOGIT($"add a {sColName} button on if it already hasnt been built")
        Dim icol As Integer = 0
        Dim sColumn = (From c In dgScores.Columns Select c Where c.DataPropertyName = sColName).SingleOrDefault
        If sColumn IsNot Nothing Then
            icol = dgScores.Columns(sColName).Index
            dgScores.Columns.Remove(sColName)
        Else
            Dim swk = (From c In dgScores.Columns Select c Where c.DataPropertyName = sPrecColName).SingleOrDefault
            icol = swk.index
        End If
        Dim btn As New DataGridViewButtonColumn
        With btn
            .HeaderText = sColName
            .Name = sColName
            .DataPropertyName = sColName
            .Width = 50
            .Text = If(BtnText = "", sColName, BtnText)
            .UseColumnTextForButtonValue = True
            dgScores.Columns.Insert(icol, btn)
        End With
    End Sub
    Public Sub CreateDGCol(dgScores As DataGridView, sColName As String, sPrecColName As String, Optional ByVal sHeader As String = "", Optional ByVal sColor As Color = Nothing)
        Try
            ' 1. Check if the column already exists
            Dim existingCol = (From c In dgScores.Columns.Cast(Of DataGridViewColumn)
                               Where c.Name = sColName OrElse c.DataPropertyName = sColName).SingleOrDefault()

            Dim targetIndex As Integer = 0 ' Default to the beginning if nothing else is found

            If existingCol IsNot Nothing Then
                ' If it exists, we grab its current spot before removing it
                targetIndex = existingCol.Index
                dgScores.Columns.Remove(existingCol)
            Else
                ' 2. Find the reference column (e.g., "1") to insert BEFORE
                Dim precCol = (From c In dgScores.Columns.Cast(Of DataGridViewColumn)
                               Where c.Name = sPrecColName OrElse c.DataPropertyName = sPrecColName).SingleOrDefault()

                If precCol IsNot Nothing Then
                    ' Inserting at precCol.Index puts the new column exactly where "1" was, 
                    ' pushing "1" to the right.
                    targetIndex = precCol.Index
                Else
                    ' If reference column "1" isn't found, default to the very end
                    targetIndex = dgScores.Columns.Count
                End If
            End If

            ' 3. Create the new column
            Dim tb As New DataGridViewTextBoxColumn
            With tb
                .Name = sColName
                .DataPropertyName = sColName
                .HeaderText = If(String.IsNullOrEmpty(sHeader), sColName, sHeader)
                .Width = 50
                .ReadOnly = False
                ' Check if color is empty (defaulting to LightPink if so)
                .HeaderCell.Style.BackColor = If(sColor.IsEmpty, Color.LightPink, sColor)
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Visible = True ' Changed to True so you can see it immediately
            End With

            ' 4. Final safety check on the index bounds
            If targetIndex < 0 Then targetIndex = 0
            If targetIndex > dgScores.Columns.Count Then targetIndex = dgScores.Columns.Count

            dgScores.Columns.Insert(targetIndex, tb)

        Catch ex As Exception
            ' Log error if necessary
        End Try
    End Sub
    Public Sub _CreateDGCol(dgScores As DataGridView, sColName As String, sPrecColName As String, Optional ByVal sHeader As String = "", Optional ByVal sColor As Color = Nothing)
        Try

            Dim sColumn = (From c In dgScores.Columns Select c Where c.DataPropertyName = sColName).SingleOrDefault
            Dim icol As Integer = dgScores.Columns.Count
            If sColumn IsNot Nothing Then
                icol = dgScores.Columns(sColName).Index
                dgScores.Columns.Remove(sColName)
            Else
                Dim swk = (From c In dgScores.Columns Select c Where c.DataPropertyName = sPrecColName).SingleOrDefault
                Try
                    icol = swk.index
                Catch ex As Exception

                End Try

            End If
            sColumn = (From c In dgScores.Columns Select c Where c.DataPropertyName = sPrecColName).SingleOrDefault
            If sColumn IsNot Nothing Then
                icol = sColumn.index
            End If
            'MessageBox.Show($"{sColName} not created, couldnt find {sPrecColName}")
            'Exit Sub
            Dim tb As New DataGridViewTextBoxColumn
            With tb
                .HeaderText = If(sHeader = "", sColName, sHeader)
                .Name = sColName
                .DataPropertyName = sColName
                .Width = 50
                .ReadOnly = False
                .HeaderCell.Style.BackColor = If(sColor = Nothing, Color.LightPink, sColor)
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .Visible = False
                dgScores.Columns.Insert(icol, tb)
                'LOGIT($"Adding column name {sColName} with header of {tb.HeaderText} at column {icol}")
            End With
            'Dim x = dgScores
        Catch ex As Exception

        End Try
    End Sub
    Function sIn(sfld As String, s2Compare As String, bcontains As Boolean) As Boolean
        sIn = False
        Dim sflds = s2Compare.Split(",")
        For Each fld As String In s2Compare.Split(",")
            fld = fld.ToLower
            If bcontains Then
                If sfld.ToLower.Contains(fld) Then
                    sIn = True
                    Exit Function
                End If
            Else
                If sfld.ToLower = fld Then
                    sIn = True
                    Exit Function
                End If
            End If
        Next
    End Function
    Public Function GetFile() As String
        ' Create and configure the OpenFileDialog
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.InitialDirectory = "C:\"
            openFileDialog.Filter = "All Files (*.*)|*.*|CSV Files (*.csv)|*.csv"
            openFileDialog.FilterIndex = 1
            openFileDialog.RestoreDirectory = True

            ' Show the dialog and get the result
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                ' Get the selected file path
                Dim selectedFilePath As String = openFileDialog.FileName
                'MessageBox.Show($"Selected file: {selectedFilePath}", "File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return openFileDialog.FileName
            Else
                Return Nothing
            End If
        End Using
    End Function
    Sub ShowCaller()
        ' Create a new StackTrace, excluding the current method (ShowCaller)
        Dim stackTrace As New StackTrace(True)
        ' Get the frame of the caller method (1 level up the call stack)
        Dim callerFrame As StackFrame = stackTrace.GetFrame(1)
        ' Get the method that called this method
        Dim callerMethod As Reflection.MethodBase = callerFrame.GetMethod()
        ' Display the caller method information
        LOGIT($"Caller Method: {callerMethod.Name}")
        LOGIT($"Caller Type: {callerMethod.DeclaringType.FullName}")
        LOGIT($"File: {callerFrame.GetFileName()}")
        LOGIT($"Line: {callerFrame.GetFileLineNumber()}")
    End Sub
#Region "2024"
    Sub CalcLeftovers(con As SQLiteConnection)
        Try
            'calculate leftoverskins and fill text boxes with this weeks amounts
            LOGIT(String.Format("calculate leftoverskins/ctps And fill text boxes with this weeks amounts"))

            Dim sFrom = CDate(ctx.rLeagueParmrow("StartDate")).ToString("yyyyMMdd")
            Dim sTo = CDate(ctx.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd")

            Dim dtScores As DataTable = sqlitedaFromSql(con, "Scores", $"select * from Scores where Date >= '{sFrom}' and Date <= '{sTo}' ORDER BY Date LIMIT 1 ")
            Dim strsql = New StringBuilder
            Dim psDate As String = CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
            Dim lp3 As List(Of String) = Main.lPar3s
            Dim sb As New StringBuilder
            Dim x = tallysumif(skin, "TRUE,True", "SkinPlayers")
            Dim xx As String = String.Format("{0}", x)
            Dim ssql As String = $"{GetWklySql(sFrom, sTo)} "
            'LOGIT(ssql)
            SQLiteCreateTableFromDT(con, sqlitedaFromSql(con, "WklySkins", ssql))
            dtWklySkins = sqliteda(ctx.Conn, "WklySkins")
            Dim lf As New List(Of String)
            lf = Main.lPar3sFront
            Dim sf = String.Join(",", lf)
            Dim lb As New List(Of String)
            lb = Main.lpar3sBack
            Dim sback = String.Join(",", lb)

            Dim dtcps As DataTable = sqlitedaFromSql(con, "CTP", $"select case when FrontBack = 'Front' then '{sf}' ELSE '{sback}' END Par3s, date from Scores where Date >= '{sFrom}' and Date <= '{sTo}' GROUP BY Date")
            sb.AppendLine(
$"SELECT 
S.Date
,CASE WHEN S.FrontBack = 'Front' THEN '{sf}' ELSE '{sback}' END Par3s
,IIF(S.Date < {sPSDate}, {ctx.rLeagueParmrow("Skins")}, {ctx.rLeagueParmrow("SkinsPS")}) AS SkinPaid
,COUNT(*) AS Total
,SUM(CASE WHEN SP.ID IS NOT NULL THEN 1 ELSE 0 END) AS SkinPlayers
,SUM(CASE WHEN SE.ID IS NOT NULL THEN 1 ELSE 0 END) SkinsNum 
,SUM(CAST(SP.Earned AS REAL)) SkinsCollected 
,SUM(CAST(SE.Earned AS REAL)) SkinsEarned 
,SUM(CAST(SP.Earned AS REAL)) - SUM(CAST(SE.Earned AS REAL)) SkinsExtra
,SUM(CASE WHEN CP.ID IS NOT NULL THEN 1 ELSE 0 END) CTPPlayers
,SUM(CAST(CP.Earned AS REAL)) CTPCollected 
,0 CTP1
,0 CTP2
,0 Kitty
From Scores S
LEFT JOIN [Payments] SP ON (SP.Date = S.Date AND SP.Player = S.Player AND SP.[Desc] = 'Skin' AND SP.[Detail] = 'Payment')
LEFT JOIN [Payments] SE ON (SE.Date = S.Date AND SE.Player = S.Player AND SE.[Desc] = 'Skin' AND SE.[Detail] LIKE '#%')
LEFT JOIN [Payments] CP ON (CP.Date = S.Date AND CP.Player = S.Player AND CP.[Desc] = 'CTP' AND CP.[Detail] = 'Payment')
WHERE S.Date >= {sFrom} AND S.Date <= {sTo}
GROUP BY S.Date")
            ssql = sb.ToString
            SQLiteCreateTableFromDT(con, sqlitedaFromSql(con, "WklySkins", ssql))
            dtWklySkins = sqliteda(ctx.Conn, "WklySkins")
            dtWklySkins.PrimaryKey = New DataColumn() {dtWklySkins.Columns("Date")}

            'showtables(con)
            Dim et As TimeSpan
            Dim sst As DateTime = Now
            'fix previous amounts Front
            'For Each row As DataRow In dtWklySkins.Rows
            '    row("Kitty") = row("CTPCollected") Mod Main.lPar3s.count
            '    row("FrontCTP1Extra") = row("CTPCollected") - row("Kitty") - If(row("CTP1") > 0, 0, row("CTP2"))
            '    wk = ""
            'Next
            et = Now - sst
            LOGIT(et.ToString)
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub CalcCarryOvers()
        'create new table for leftovers, skins, CTPs front/back, kitty for front/back for each cycle
        Dim dt = New DataTable
        dt.TableName = "LeftOvers"
        dt.Columns.Add("Kitty", GetType(String))
        SQLiteCreateTableFromDT(ctx.Conn, dt)

        Dim sFrom = CDate(ctx.rLeagueParmrow("StartDate")).ToString("yyyyMMdd")
        Dim sTo = ctx.ActiveDate 'CDate(ctx.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd")

        Dim lp3s = ctx.lPar3sFront
        If iHoleMarker = 10 Then
            lp3s = ctx.lpar3sBack
        End If

        Dim sb = New StringBuilder
        sb.AppendLine(
$"Select DISTINCT Date FROM Scores S
WHERE Date >= '{sFrom}' AND Date < '{ctx.ActiveDate}' AND Frontback = '{If(iHoleMarker = 1, "Front", "Back")}'")
        Dim dtDates As DataTable = sqliteda("Dates", sb.ToString)
        Dim dtPayments As DataTable
        If dtlo IsNot Nothing Then dtlo.Clear()

        For Each row In dtDates.Rows
            'only get front or back for whichever side were on
            sb = New StringBuilder
            sb.AppendLine(
$"Select Payments.* FROM Payments 
            JOIN Scores ON Payments.Player = Scores.Player AND Scores.Date = Payments.Date and scores.frontback = '{If(iHoleMarker = 1, "Front", "Back")}'
            WHERE Payments.Date = '{row("Date")}' AND [DESC]||Detail LIKE 'CTP#%' ORDER BY Detail")

            'dtPayments.PrimaryKey = New DataColumn() {dtPayments.Columns("Date")}
            dtPayments = sqliteda("Payments", sb.ToString)
            If dtPayments.Rows.Count = lp3s.Count Then Exit Sub
            Dim i As Integer = 0
            dtlo = sqliteda("Leftovers")
            For i = 1 To 9
                dtlo.Columns.Add($"CTP{i}")
            Next
            i = 0
            For Each pmt As DataRow In dtPayments.Rows
                If pmt("Detail") <> $"#{lp3s(i)}" Then
                    Dim nr As DataRow
                    'this is the first time we have seen this date, so create a new row
                    If dtlo.Rows.Count = 0 Then
                        nr = dtlo.NewRow
                        dtlo.Rows.Add(nr)
                    End If

                    nr($"CTP{i + 1}") += pmt("Earned")
                    Continue For
                Else
                    i += 1
                End If

            Next
        Next
        Exit Sub
#Region "old code"
        dtlo = sqliteda("Leftovers")
        For i = 1 To 9
            dtlo.Columns.Add($"CTP{i}")
        Next

        Dim lwk = New List(Of String)
        Dim lsv = New List(Of String)
        Dim sprevdt As String = ""
        Dim damt As Decimal = 0, dtot As Decimal = 0
        lsv = Main.lpar3sBack
        If sFrontBack = "Front" Then
            lsv = Main.lPar3sFront
        End If
        For Each row In dtPayments.Rows
            If row("Date") <> sprevdt Then
                sprevdt = row("Date")
                damt = 0
                lwk = lsv
            End If
            lwk = lwk.Where(Function(value) value = row("Detail").ToString.Replace("#", "")).ToList()
            damt += row("Earned")
        Next
        If lwk.Count > 0 Then
            dtot += damt
        End If

        dtlo.PrimaryKey = New DataColumn() {dtlo.Columns("Date")}
        Dim lf As New List(Of String)
        lf = Main.lPar3sFront
        Dim sf = String.Join(",", lf)
        Dim lb As New List(Of String)
        lb = Main.lpar3sBack
        Dim sback = String.Join(",", lb)

        For Each row As DataRow In dtWklySkins.Rows
            If row("Date") Is DBNull.Value Then Exit For
            Dim lfb = sback
            Dim sfb = "B"
            If row("Par3s").ToString.Split(",")(0) < 10 Then
                lfb = sf
                sfb = "F"
            End If
            Dim nr As DataRow
            'this is the first time we have seen this date, so create a new row
            nr = dtlo.NewRow
            nr("Kitty") = row("CTPCollected") Mod row("Par3s").ToString.Split(",").Count

            For i = 1 To row("Par3s").ToString.Split(",").Count
                nr($"CTP{i}") = (row($"CTPCollected") - nr("Kitty")) / row("Par3s").ToString.Split(",").Count
            Next
            For Each prow As DataRow In dtPayments.Rows
                If prow("Date") = row("Date") Then
                    For i = 0 To row("Par3s").ToString.Split(",").Count - 1
                        If prow("Detail").ToString.Replace("#", "") = row("Par3s").ToString.Split(",")(i) Then
                            nr($"CTP{i + 1}") -= (row($"CTPCollected") - nr("Kitty")) / row("Par3s").ToString.Split(",").Count
                        End If
                    Next
                End If
            Next
            For Each col In nr.ItemArray
                If IsNumeric(col) Then
                    If col > 0 Then
                        dtlo.Columns.Add("FB")
                        dtlo.Columns("FB").SetOrdinal(0)
                        nr("FB") = sfb
                        dtlo.Rows.Add(nr)
                        Exit For
                    End If
                Else
                    Exit For
                End If
            Next

        Next

#End Region

    End Sub


#Region "Sqlite Routines"

    Sub CreateChangeLog()
        Dim sql As String = "
CREATE TABLE ChangeLog (" &
    "ChangeID Integer PRIMARY KEY AUTOINCREMENT," &
    "TableName Text," &
    "Operation TEXT," &
    "RecordID Integer," &
    "ChangeTime DATETIME DEFAULT CURRENT_TIMESTAMP" &
");"
        Dim Command = New SQLiteCommand(sql, ctx.Conn)
        Try
            Command.ExecuteNonQuery()
        Catch ex As Exception
            LOGIT($"ChangeLog-{ex.Message}")
        End Try
    End Sub
    Sub CreateTriggers(TableName As String)
        Dim sql = New StringBuilder
        sql.AppendLine($"
CREATE TRIGGER track_insert 
AFTER INSERT ON {TableName}
BEGIN
        INSERT INTO ChangeLog (TableName, Operation, RecordID)
        Values('{TableName}', 'INSERT', NEW.ID);
END;
CREATE TRIGGER track_update
AFTER UPDATE ON {TableName}
BEGIN
        INSERT INTO ChangeLog (TableName, Operation, RecordID)
        Values('{TableName}', 'UPDATE', NEW.ID);
END;
CREATE TRIGGER track_delete
AFTER Delete ON {TableName}
BEGIN
        INSERT INTO ChangeLog (TableName, Operation, RecordID)
        VALUES('{TableName}', 'DELETE', OLD.ID);
END;"
            )
        Dim Command = New SQLiteCommand(sql.ToString, ctx.Conn)
        Try
            Command.ExecuteNonQuery()
        Catch ex As Exception
            LOGIT($"ChangeLog-{ex.Message}")
        End Try
    End Sub
    Sub SQLiteCreateTableFromDT(connection As SQLiteConnection, dataTable As DataTable)
        Try
            ' 1. THE ESSENTIAL FIX: Ensure connection is open
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If

            ' Use a single command object and update its CommandText to be cleaner
            Dim command As New SQLiteCommand("", connection)

            ' Drop Table
            command.CommandText = $"DROP TABLE IF EXISTS [{dataTable.TableName}];"
            Try
                command.ExecuteNonQuery()
            Catch ex As Exception
                LOGIT($"{dataTable.TableName} DROP error: {ex.Message}")
            End Try

            ' Build Create Table String
            Dim createTableQuery As String = $"CREATE TABLE [{dataTable.TableName}] ("
            For Each col As DataColumn In dataTable.Columns
                Dim sColtype As String = "TEXT" ' SQLite prefers TEXT for strings

                ' Your existing type logic
                If sIn(col.ColumnName.ToLower, "team,id", False) Then
                    sColtype = "INTEGER"
                ElseIf sIn(col.ColumnName.ToLower, "points", True) Then
                    sColtype = "NUMERIC"
                End If

                createTableQuery &= $"[{col.ColumnName}] {sColtype}, "
            Next
            createTableQuery = createTableQuery.TrimEnd(",", " ") & ")"

            ' Execute Create
            command.CommandText = createTableQuery
            Try
                command.ExecuteNonQuery()
            Catch ex As Exception
                LOGIT($"{dataTable.TableName} CREATE error: {ex.Message}")
                Exit Sub ' No point in continuing if table creation failed
            End Try

            ' Transaction for Mass Inserts
            Using transaction = connection.BeginTransaction()
                Try
                    For Each row As DataRow In dataTable.Rows
                        Dim insertQuery As String = $"INSERT INTO [{dataTable.TableName}] ("

                        ' Build Columns part
                        For Each col As DataColumn In dataTable.Columns
                            insertQuery &= $"[{col.ColumnName}], "
                        Next
                        insertQuery = insertQuery.TrimEnd(",", " ") & ") VALUES ("

                        ' Build Values part (Handling your rule: No double quotes in SQL)
                        For Each col As DataColumn In dataTable.Columns
                            Dim val As String = row(col.ColumnName).ToString().Replace("'", "''")
                            insertQuery &= $"'{val}', "
                        Next
                        insertQuery = insertQuery.TrimEnd(",", " ") & ")"

                        ' Reuse the command object inside the transaction
                        command.CommandText = insertQuery
                        command.ExecuteNonQuery()
                    Next
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    LOGIT($"Transaction failed: {ex.Message}")
                End Try
            End Using

        Catch ex As Exception
            LOGIT($"Critical error in SQLiteCreateTableFromDT: {ex.Message}")
        End Try
    End Sub
    Function SQLiteInsertIntoTable(connection As SQLiteConnection, dataTable As DataTable, insertQuery As String) As Long
        Dim lastId As Long = 0
        Try
            ' 1. THE FIX: Ensure connection is open before starting the transaction
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If

            Using transaction = connection.BeginTransaction()
                ' Pass the transaction object to the command to avoid "Transaction required" errors
                Using insertCommand As New SQLiteCommand(insertQuery, connection, transaction)
                    Try
                        insertCommand.ExecuteNonQuery()
                        transaction.Commit()

                        ' Get the last inserted ID to return
                        Using cmdId As New SQLiteCommand("SELECT last_insert_rowid();", connection, transaction)
                            lastId = Convert.ToInt64(cmdId.ExecuteScalar())
                        End Using
                    Catch ex As Exception
                        transaction.Rollback()
                        LOGIT($"SQL Execution Error: {ex.Message} | Query: {insertQuery}")
                    End Try
                End Using
            End Using
        Catch ex As Exception
            LOGIT($"Connection/Transaction Error: {ex.Message}")
        End Try

        Return lastId
    End Function   ''' <summary>
    ''' Fills a ComboBox with DISTINCT values from a single column using a provided SQL query.
    ''' Handles connection state, parameters, Begin/EndUpdate, empty results, and basic string cleaning.
    ''' </summary>
    ''' <param name="connection">SQLiteConnection (will open if closed, close if it was closed)</param>
    ''' <param name="sql">The SQL query (should return ONE column, e.g. SELECT DISTINCT Something ...)</param>
    ''' <param name="targetCombo">The ComboBox to populate</param>
    ''' <param name="parameters">Optional Dictionary of parameter name → value</param>
    ''' <param name="noDataText">Text to show when no results</param>
    ''' <param name="trimAndValidate">Optional lambda to clean/validate each value before adding</param>
    Public Sub SQLiteFillComboFromQuery(
    connection As SQLiteConnection,
    sql As String,
    targetCombo As ComboBox,
    Optional parameters As Dictionary(Of String, Object) = Nothing,
    Optional noDataText As String = "(No items found)",
    Optional trimAndValidate As Func(Of String, Boolean) = Nothing)

        If targetCombo Is Nothing Then Return

        Dim originallyClosed As Boolean = (connection.State <> ConnectionState.Open)
        If originallyClosed Then connection.Open()

        Try
            Using cmd As New SQLiteCommand(sql, connection)

                ' Add parameters if provided
                If parameters IsNot Nothing Then
                    For Each kvp In parameters
                        cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                    Next
                End If

                targetCombo.BeginUpdate()
                targetCombo.Items.Clear()

                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        If Not reader.IsDBNull(0) Then
                            Dim value As String = reader.GetString(0)?.Trim()
                            If value IsNot Nothing Then

                                Dim isValid As Boolean = True
                                If trimAndValidate IsNot Nothing Then
                                    isValid = trimAndValidate(value)
                                ElseIf String.IsNullOrWhiteSpace(value) Then
                                    isValid = False
                                End If

                                If isValid Then
                                    targetCombo.Items.Add(value)
                                End If
                            End If
                        End If
                    End While
                End Using

                If targetCombo.Items.Count > 0 Then
                    targetCombo.SelectedIndex = 0
                Else
                    targetCombo.Items.Add(noDataText)
                    targetCombo.SelectedIndex = 0
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show($"Error filling {targetCombo.Name}:{vbCrLf}{ex.Message}",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            targetCombo.EndUpdate()

            If originallyClosed AndAlso connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try

    End Sub
    Sub SQLiteCreateTable(connection As SQLiteConnection, sfile As String)
        LOGIT(sfile)
        'Dim stackTrace As New StackTrace(True)
        '' Get the frame of the caller method (1 level up the call stack)
        'Dim callerFrame As StackFrame = stackTrace.GetFrame(1)
        '' Get the method that called this method
        'Dim callerMethod As Reflection.MethodBase = callerFrame.GetMethod()
        '' Display the caller method information
        'logit($"Caller Method: {callerMethod.Name}")
        'logit($"Caller Type: {callerMethod.DeclaringType.FullName}")
        'logit($"File: {callerFrame.GetFileName()}")
        'logit($"Line: {callerFrame.GetFileLineNumber()}")
        'ShowCaller()
        Dim sTableName As String = $"{sfile}"
        sTableName = $"{sfile.Substring(sfile.LastIndexOf("\") + 1)}"
        sTableName = $"{sTableName.Substring(0, sTableName.IndexOf("."))}"
        sTableName = sTableName.Replace("Hugh's_", "")
        'Dim csvFilePath As String = $"{sfile}".Replace("dt", "")
        Dim csvFilePath As String = "C:\Golf\Files\Scores.csv"
        ' Read the CSV file and insert its data into the SQLite table
        Dim dataTable As DataTable
        If csvFilePath.ToLower.Contains("scores") Then
            dataTable = fixscores(ReadCsvIntoDataTable(csvFilePath))
        Else
            dataTable = ReadCsvIntoDataTable(csvFilePath)
        End If
        ' Create an in-memory SQLite database
        Dim createTableQuery As String = $"CREATE TABLE [{sTableName}] (ID INTEGER, "
        For Each col As DataColumn In dataTable.Columns
            Dim sColtype As String = "String"
            If col.ColumnName.ToLower = "team" Then
                sColtype = "Integer"
            End If
            createTableQuery &= $"[{col.ColumnName}] {sColtype}, "
        Next
        createTableQuery = createTableQuery.TrimEnd(",", " ") & ")"
        LOGIT(createTableQuery)
        Dim command As New SQLiteCommand(createTableQuery, connection)
        command.ExecuteNonQuery()

        Using transaction = connection.BeginTransaction()
            Dim Id As Integer = 1

            For Each row As DataRow In dataTable.Rows
                Dim insertQuery As String = $"INSERT INTO [{sTableName}] (ID, "
                For Each col As DataColumn In dataTable.Columns
                    insertQuery &= $"[{col.ColumnName}], "
                Next
                insertQuery = insertQuery.TrimEnd(",", " ") & ") VALUES ("
                insertQuery &= $"{Id}, "
                For Each col As DataColumn In dataTable.Columns
                    insertQuery &= $"'{row(col.ColumnName).ToString}'.Replace(" '", "''")}', "
                Next
                insertQuery = insertQuery.TrimEnd(",", " ") & ")"
                Dim insertCommand As New SQLiteCommand(insertQuery, connection)
                '$"INSERT INTO Data (ID, Name, Age) VALUES ({row("ID")}, '{row("Name")}', {row("Age")})"
                insertCommand.ExecuteNonQuery()
                Id += 1
            Next
            Try
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
            End Try
#Region "Debug"
            If sTableName = "LeagueParms" Then
                Dim selectcommand As New SQLiteCommand($"select * from {sTableName}", connection)
                Dim x = ""

                Using reader As SQLiteDataReader = selectcommand.ExecuteReader
                    While reader.Read
                        Dim flds = ""
                        For i = 0 To reader.FieldCount - 1
                            flds &= $"{reader.GetName(i)}-{reader.GetValue(i)}-{reader.GetFieldType(i)} "
                        Next
                        'logit(flds)
                    End While
                End Using

            End If

#End Region
        End Using

        '' Execute SQL queries against the SQLite database
        'Dim selectQuery As String = $"SELECT * FROM {sTableName} WHERE Date > '20240901'"
        'command = New SQLiteCommand(selectQuery, connection)

        'Using reader As SQLiteDataReader = command.ExecuteReader()
        '    While reader.Read()
        '        Console.WriteLine($"{reader("ID")}{vbTab}{reader("Player")}{vbTab}{reader("Date")}")
        '        logit($"{reader("ID")}{vbTab}{reader("Player")}{vbTab}{reader("Date")}")
        '    End While
        'End Using
    End Sub
    ' Fix: Ensure the first argument is explicitly a String and the second is Optional
    Function SqliteTrans(ByVal sql As String, Optional ByVal params As Dictionary(Of String, Object) = Nothing) As Integer
        Dim lrc As Integer = 0
        Dim transaction As SQLiteTransaction = Nothing

        Try
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

            transaction = ctx.Conn.BeginTransaction()

            Using cmd As New SQLiteCommand(sql, ctx.Conn)
                cmd.Transaction = transaction

                If params IsNot Nothing Then
                    For Each kvp In params
                        cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                    Next
                End If

                ' Use ExecuteNonQuery if sql is INSERT/UPDATE/DELETE (returns rows affected).
                ' Use ExecuteScalar if sql returns a single value (e.g. SELECT last_insert_rowid()).
                Dim result As Object = cmd.ExecuteScalar()
                lrc = If(result IsNot Nothing AndAlso result IsNot DBNull.Value, Convert.ToInt32(result), 0)
            End Using

            transaction.Commit()

        Catch ex As Exception
            ' Roll back before logging so the DB is left clean
            If transaction IsNot Nothing Then
                Try
                    transaction.Rollback()
                Catch rbEx As Exception
                    LOGIT($"SqliteTrans Rollback Error: {rbEx.Message}")
                End Try
            End If
            LOGIT($"SqliteTrans Error: {ex.Message} | SQL: {sql}")

        Finally
            transaction?.Dispose()
            If ctx.Conn.State = ConnectionState.Open Then ctx.Conn.Close()
        End Try

        Return lrc
    End Function
    ' New Overload: Call this from Main.vb without needing to pass 'connection'
    Public Function sqliteda(tablename As String, Optional seltbl As String = "", Optional params As Dictionary(Of String, Object) = Nothing) As DataTable
        ' It just calls the master function and passes Nothing for the connection 
        ' (since the master uses ctx.Conn anyway)
        Return sqliteda(Nothing, tablename, seltbl, params)
    End Function

    ' Update this signature at the top of the Master function
    Public Function sqliteda(connection As SQLiteConnection, tablename As String, Optional seltbl As String = "", Optional params As Dictionary(Of String, Object) = Nothing) As DataTable

        Dim swhere As String = ""

        ' 1. Logic to determine the SQL string
        If seltbl = "" Then seltbl = tablename

        ' Detect if seltbl is actually a WHERE clause
        If seltbl.ToLower.Contains("where") AndAlso Not seltbl.ToLower.StartsWith("select") Then
            swhere = seltbl
            seltbl = tablename
        End If

        ' Declare selectCommand at the function level so it's accessible everywhere
        Dim selectCommand As String = If(seltbl.ToLower.StartsWith("select"), seltbl, $"SELECT * FROM {seltbl} {swhere}")

        Dim dataTable As New DataTable()
        dataTable.TableName = tablename

        ' 2. Core Execution with Connection Management
        Try
            ' Ensure the connection is open
            If ctx.Conn.State = ConnectionState.Closed Then ctx.Conn.Open()

            Using cmd As New SQLiteCommand(selectCommand, ctx.Conn)
                ' Attach parameters if they exist
                If params IsNot Nothing Then
                    For Each kvp In params
                        cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                    Next
                End If

                Using adapter As New SQLiteDataAdapter(cmd)
                    adapter.Fill(dataTable)
                End Using
            End Using

        Catch ex As Exception
            LOGIT($"SQL Error in [sqliteda - {tablename}]: " & ex.Message)
            LOGIT($"Faulty SQL: {selectCommand}")
        Finally
            ' Always close the connection to prevent hughsgolf.db file locks
            If ctx.Conn.State = ConnectionState.Open Then ctx.Conn.Close()
        End Try

        ' 3. Automatic Score Fixing
        ' This replaces the commented-out logic and ensures your data is always calculated
        If tablename.ToLower = "scores" AndAlso dataTable.Rows.Count > 0 Then
            Try
                dataTable = fixscores(dataTable)
            Catch ex As Exception
                LOGIT("Error in fixscores during sqliteda: " & ex.Message)
            End Try
        End If

        Return dataTable
    End Function
    ' Simplified wrapper
    Public Function sqlitedaFromSql(tablename As String, sql As String, Optional params As Dictionary(Of String, Object) = Nothing) As DataTable
        Return sqliteda(Nothing, tablename, sql, params)
    End Function
    ' Add the Optional params here so the call from Main.vb line 694 is valid
    Public Function sqlitedaFromSql(connection As SQLiteConnection, tablename As String, sql As String, Optional params As Dictionary(Of String, Object) = Nothing) As DataTable
        ' We just pass everything into our updated master function
        Return sqliteda(connection, tablename, sql, params)
    End Function

    Public Function SQLiteExecuteScalar(ByVal sql As String, Optional ByVal params As Dictionary(Of String, Object) = Nothing) As Object
        Dim result As Object = Nothing
        Try
            Using conn As New SQLiteConnection(ctx.Conn.ConnectionString) ' Uses your existing connection string
                Using cmd As New SQLiteCommand(sql, conn)
                    ' Fill parameters if provided
                    If params IsNot Nothing Then
                        For Each kvp In params
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                        Next
                    End If

                    conn.Open()
                    result = cmd.ExecuteScalar()
                    conn.Close()
                End Using
            End Using
        Catch ex As Exception
            ' Log the error using your existing LOGIT routine
            LOGIT("SQLiteExecuteScalar Error: " & ex.Message & " | SQL: " & sql)
        End Try

        ' Returns 0 if result is null/empty; otherwise returns the value
        Return If(result Is DBNull.Value Or result Is Nothing, 0, result)
    End Function
    Function SqliteExec(sql As String,
                   params As Dictionary(Of String, Object),
                   Optional trans As SQLiteTransaction = Nothing) As Integer

        Try
            ' ✅ Ensure connection is open
            If ctx.Conn.State <> ConnectionState.Open Then
                ctx.Conn.Open()
            End If

            Using cmd As New SQLiteCommand(sql, ctx.Conn)

                ' ✅ Attach transaction if provided
                If trans IsNot Nothing Then
                    cmd.Transaction = trans
                End If

                ' ✅ Add parameters
                If params IsNot Nothing Then
                    For Each kvp In params
                        cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                    Next
                End If

                Dim rowsAffected = cmd.ExecuteNonQuery()

                ' 🔍 Debug (optional)
                ' LOGIT($"Rows affected: {rowsAffected}")

                Return rowsAffected

            End Using

        Catch ex As Exception
            LOGIT($"SqliteExec ERROR: {ex.Message} | SQL: {sql}")

            If params IsNot Nothing Then
                For Each kvp In params
                    LOGIT($"  {kvp.Key} = {kvp.Value}")
                Next
            End If

            Return 0
        End Try

    End Function
    Public Function SQLiteGetDataTable(sql As String, Optional params As Dictionary(Of String, Object) = Nothing) As DataTable
        Dim dt As New DataTable
        Try
            Using conn As New SQLiteConnection(ctx.Conn.ConnectionString)
                conn.Open()
                Using cmd As New SQLiteCommand(sql, conn)
                    If params IsNot Nothing Then
                        For Each kvp In params
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value)
                        Next
                    End If
                    Using adapter As New SQLiteDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            LOGIT($"SQLiteGetDataTable Error: {ex.Message} | SQL: {sql}")
        End Try
        Return dt
    End Function
#End Region
    ''' <summary>
    ''' Build a string of the last 5 scores (including current) and calculate the average.
    ''' </summary>
    ''' <param name="dvsPlayers">DataView of past player scores</param>
    ''' <param name="srow">Current DataRow with "Gross" score</param>
    ''' <returns>Tuple: (Joined string, Average as Double)</returns>
    Public Function GetLast5ScoresSummary(dvsPlayers As DataView, srow As DataRow) As (Joined As String, Average As Double)

        Dim llast5Scores As New List(Of Integer)

        ' Take up to 4 previous scores
        If dvsPlayers.Count > 0 Then
            Dim takeCount As Integer = Math.Min(4, dvsPlayers.Count)
            Dim startIndex As Integer = dvsPlayers.Count - takeCount

            For i As Integer = startIndex To dvsPlayers.Count - 1
                llast5Scores.Add(Convert.ToInt32(dvsPlayers(i)("Gross")))
            Next
        End If

        ' Add the current score
        llast5Scores.Add(Convert.ToInt32(srow("Gross")))

        ' Build the joined string
        Dim joined As String = String.Join("-", llast5Scores)

        ' Calculate average
        Dim avg As Double = If(llast5Scores.Count > 0,
                       llast5Scores.Select(Function(s) Convert.ToDouble(s)).Average(),
                       0)

        avg = Math.Round(avg, 1)   ' 1 decimal place
        Return (joined, avg)

    End Function
    Sub createHdcpDetail()
        Dim dtWk = New DataTable
        dtWk.TableName = "HandicapDetail"
        Dim sNewFields = New List(Of String)("ID,Date-1,Player-2,OldHdcp,NewHdcp,Last5,Used,Avg".Split(","))
        For Each fld In sNewFields
            Dim sfld As String = If(UBound(fld.Split("-")) > 0, fld.Split("-")(0), fld)
            dtWk.Columns.Add(sfld)
        Next
        ' Build primary key dynamically from sNewFields
        Dim pkCols As New List(Of DataColumn)

        For Each fld In sNewFields
            Dim sfld As String = If(UBound(fld.Split("-")) > 0, fld.Split("-")(0), fld)
            ' Decide which fields should be part of the primary key
            ' Example: only ID and Date
            If UBound(fld.Split("-")) > 0 Then
                pkCols.Add(dtWk.Columns(sfld))
            End If
        Next
        ' Assign primary key
        dtWk.PrimaryKey = pkCols.ToArray()
        'load old scores for comparison
        Dim dtlp As DataTable = sqliteda(ctx.Conn, "LeagueParms")
        dtlp.PrimaryKey = New DataColumn() {dtlp.Columns("Season")}

        dt = sqliteda(ctx.Conn, "Schedule")
        Dim sb = New StringBuilder
        sb.AppendLine($"Select Date from Schedule 
--WHERE Date > 20179999 
ORDER by Date Desc")
        Dim sql As String = sb.ToString
        Dim dtsch = sqlitedaFromSql(ctx.Conn, "Schedule", sql)
        dtsch.TableName = "Schedule"
        dtsch.PrimaryKey = New DataColumn() {dtsch.Columns("Date")}
        'load old scores for comparison
        Dim dtos As DataTable = ReadCsvIntoDataTable("C:\HughsGolf\Files\Scores.csv")
        dtos.PrimaryKey = New DataColumn() {dtos.Columns("Date"), dtos.Columns("Player")}
        'Dim dvos As New DataView(dtos)
        'dvos.Sort = "Date DESC"
        'Dim x = DateTime.ParseExact(ctx.rLeagueParmrow("Startdate"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)

        Dim dtHdcp As DataTable = sqlitedaFromSql(ctx.Conn, "Handicaps", "SELECT * from Handicaps Order by Date ")
        dtHdcp.PrimaryKey = New DataColumn() {dtHdcp.Columns("Player"), dtHdcp.Columns("Date")}
        Dim dtScores = sqlitedaFromSql(ctx.Conn, "Scores", $"SELECT * FROM Scores WHERE CAST(GROSS AS INTEGER) > 0 ORDER BY Date")
        dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}
        Dim wkid As Integer = 1
        Dim dvos = New DataView(dtos)
        dvos.RowFilter = "Date < 20180101"
        dvos.Sort = "Date ASC"
        For Each score As DataRowView In dvos
            Dim newrow As DataRow = dtScores.NewRow
            newrow("ID") = wkid
            newrow("Date") = score("Date")
            newrow("Player") = score("Player")
            newrow("Gross") = If(IsNumeric(score("Out_Gross")), score("Out_Gross"), score("In_Gross"))
            dtScores.Rows.Add(newrow)
        Next

        Dim dvsPlayers As New DataView(dtScores)
        dvsPlayers.Sort = "Date ASC"
        'loop thru all current scores first
        For Each srow As DataRow In dtScores.Rows
            Dim season As DataRow = dtlp.Rows.Find(srow("Date").ToString.Substring(0, 4))
            sPSDate = $"{CDate(season("PostSeasonDt")):yyyyMMdd}"
            Dim StartDate = $"{CDate(season("StartDate")):yyyyMMdd}"
            Dim EndDate = $"{CDate(season("EndDate")):yyyyMMdd}"
            Dim drfind As DataRow = dtsch.Rows.Find(srow("Date"))
            Dim wkdv As DataView
            If drfind IsNot Nothing Then
                Dim newrow As DataRow = dtWk.NewRow
                newrow("ID") = wkid
                newrow("Date") = srow("Date")
                newrow("Player") = srow("Player")
                'newrow("NewHdcp") = dtHdcp.Rows.Find(New Object() {srow("Player"), srow("Date")})("Hdcp").ToString
                newrow("NewHdcp") = dtHdcp.Rows.Find(New Object() {srow("Player"), srow("Date")})("Hdcp").ToString
                newrow("OldHdcp") = dtHdcp.Rows.Find(New Object() {srow("Player"), srow("Date")})("PHdcp").ToString
                dvsPlayers.RowFilter = $"Player = '{srow("Player")}' AND Date < '{srow("Date")}' AND Gross IS NOT NULL"

                Dim llast5Scores As New List(Of String)

                If dvsPlayers.Count > 0 Then
                    ' Take up to 4 previous scores
                    Dim takeCount As Integer = Math.Min(4, dvsPlayers.Count)
                    Dim startIndex As Integer = dvsPlayers.Count - takeCount

                    For i As Integer = startIndex To dvsPlayers.Count - 1
                        llast5Scores.Add(dvsPlayers(i)("Gross").ToString())
                    Next
                End If

                ' Add the current score
                llast5Scores.Add(srow("Gross").ToString())

                newrow("Last5") = String.Join("-", llast5Scores)
                newrow("Used") = String.Join("-", ScoresUsedInHC(llast5Scores))
                Dim result = GetLast5ScoresSummary(dvsPlayers, srow)

                Dim last5String As String = result.Joined
                Dim last5Average As Double = result.Average
                newrow("Avg") = result.Average
                dtWk.Rows.Add(newrow)
                wk = ""
            End If
        Next
        ' Assume dt is your DataTable+		
        Dim dv As New DataView(dtWk)

        ' Sort by multiple columns
        dv.Sort = "Player ASC,Date Desc"

        ' Convert back to DataTable if needed
        Dim sortedDt As DataTable = dv.ToTable()
        dt = sqliteda(ctx.Conn, "Players")
        'Dim xls = New CreateXLSX
        ' Create a new Excel workbook
        Using workbook As New XLWorkbook($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            ' Add sheets to the workbook
            If workbook.Worksheets.Any(Function(ws) ws.Name.Equals(sortedDt.TableName, StringComparison.OrdinalIgnoreCase)) Then
                workbook.Worksheets.Delete(sortedDt.TableName)
            End If
            workbook.Worksheets.Add(sortedDt, sortedDt.TableName)
            '' Save the workbook to a file
            workbook.SaveAs($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            LOGIT(ctx.csvFilePath)
        End Using
    End Sub

    Sub createRainOuts()
        Dim dtWk = New DataTable
        dtWk.TableName = "RainOuts"
        Dim sNewFields = New List(Of String)("ID,Date".Split(","))
        For Each fld In sNewFields
            dtWk.Columns.Add(fld)
        Next

        dtWk.PrimaryKey = New DataColumn() {dt.Columns("ID"), dt.Columns("Date")}

        dt = sqlitedaFromSql(ctx.Conn, "Schedule", "Select Date from Schedule ORDER by Date")
        Dim dateList As New List(Of Date)

        For Each row As DataRow In dt.Rows
            If Not IsDBNull(row(0)) Then
                'Dim sdate = DateTime.ParseExact(row(0), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                Dim sdt As Date = DateTime.ParseExact(row(0).ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                dateList.Add(Convert.ToDateTime(sdt))
            End If
        Next

        'Dim x = DateTime.ParseExact(ctx.rLeagueParmrow("Startdate"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
        dt = sqliteda(ctx.Conn, "LeagueParms")
        Dim wkid As Integer = 1
        For Each srow As DataRow In dt.Rows
            For Each sdate As Date In GetMissingDates(srow("StartDate"), srow("EndDate"), dateList)
                Dim newrow As DataRow = dtWk.NewRow
                newrow("ID") = wkid
                newrow("Date") = sdate.ToString("yyyyMMdd")
                dtWk.Rows.Add(newrow)
                wkid += 1
            Next
        Next

        'Dim xls = New CreateXLSX
        ' Create a new Excel workbook
        Using workbook As New XLWorkbook($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            ' Add sheets to the workbook
            If workbook.Worksheets.Any(Function(ws) ws.Name.Equals(dtWk.TableName, StringComparison.OrdinalIgnoreCase)) Then
                workbook.Worksheets.Delete(dtWk.TableName)
            End If
            workbook.Worksheets.Add(dtWk, dtWk.TableName)
            '' Save the workbook to a file
            workbook.SaveAs($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            LOGIT(ctx.csvFilePath)
        End Using
    End Sub
    Function GetMissingDates(startDate As Date, endDate As Date, existingDates As List(Of Date)) As List(Of Date)
        Dim missingDates As New List(Of Date)
        Dim currentDate As Date = startDate

        While currentDate <= endDate
            If Not existingDates.Contains(currentDate.Date) Then
                missingDates.Add(currentDate.Date)
            End If
            currentDate = currentDate.AddDays(7)
        End While

        Return missingDates
    End Function
    Sub createteams()
        Dim dtTeams As DataTable = sqliteda(ctx.Conn, "Teams")
        Dim highestValue = dtTeams.AsEnumerable().Select(Function(row) row.Field(Of Int64)("ID")).Max()
        Dim dtScores = ReadCsvIntoDataTable("C:\Golf\Files\Scores.csv")
        dtScores.TableName = "OldScores"
        SQLiteCreateTableFromDT(ctx.Conn, dtScores)
        Dim dtPlayers = ReadCsvIntoDataTable("C:\Golf\Files\Players.csv")
        dtPlayers.TableName = "OldPlayers"
        SQLiteCreateTableFromDT(ctx.Conn, dtPlayers)
        Dim dt2023Players = ReadCsvIntoDataTable("C:\Golf\Files\2023_Players.csv")
        dt2023Players.TableName = "Players2023"
        SQLiteCreateTableFromDT(ctx.Conn, dt2023Players)
        Dim strsql = New StringBuilder
        strsql.AppendLine($"Select ")
        strsql.AppendLine($"0 as Id")
        strsql.AppendLine($",'Hugh''s' as League")
        strsql.AppendLine($",S.Player")
        strsql.AppendLine($",S.Date")
        strsql.AppendLine($",S.Team")
        strsql.AppendLine($",S.Grade")
        strsql.AppendLine($"FROM [OldScores] S")
        strsql.AppendLine($"LEFT JOIN [OldPlayers] P ON SUBSTRING(S.Date,1,4) = P.Year AND S.Player = P.Name")
        strsql.AppendLine($"WHERE SUBSTRING(S.Date,1,4) <> '2017' AND SUBSTRING(S.Date,1,4) < '2023' AND P.Team = ''")
        strsql.AppendLine($"UNION")
        strsql.AppendLine($"Select ")
        strsql.AppendLine($"0 as Id")
        strsql.AppendLine($",'Hugh''s' as League")
        strsql.AppendLine($",S.Player")
        strsql.AppendLine($",S.Date")
        strsql.AppendLine($",S.Team")
        strsql.AppendLine($",S.Grade")
        strsql.AppendLine($"FROM [OldScores] S")
        strsql.AppendLine($"LEFT JOIN [Players2023] P On S.Player = P.Name")
        strsql.AppendLine($"WHERE SUBSTRING(S.Date,1,4) = '2023' AND P.Team = ''")
        strsql.AppendLine($"UNION")
        strsql.AppendLine($"Select ")
        strsql.AppendLine($"0 as Id")
        strsql.AppendLine($",'Hugh''s' as League")
        strsql.AppendLine($",S.Player")
        strsql.AppendLine($",S.Date")
        strsql.AppendLine($",S.Team")
        strsql.AppendLine($",S.Grade")
        strsql.AppendLine($"FROM [OldScores] S")
        strsql.AppendLine($"LEFT JOIN [Players2023] P On S.Player = P.Name")
        strsql.AppendLine($"WHERE SUBSTRING(S.Date,1,4) > '2023' AND P.Team = ''")

        Dim newdt = sqlitedaFromSql(ctx.Conn, "NewTeam", strsql.ToString)
        Dim newrow As DataRow
        Dim lsubs = New List(Of String)

        Dim dtSubs = New DataTable
        For Each col As DataColumn In newdt.Columns
            dtSubs.Columns.Add(col.ColumnName)
        Next
        dtSubs.TableName = "Subs"
        highestValue = 1
        For Each row In newdt.Rows
            newrow = dtSubs.NewRow
            'logit($"{row(Player)} {row("Team")}  {row("Grade")}") ' {Len(row("Grade"))}")
            If row(Player).ToString = "Mike Arthur" Then
                Dim x = ""
            End If
            For Each col As DataColumn In newdt.Columns
                If col.ColumnName = "Id" Then
                    newrow(col.ColumnName) = highestValue
                    highestValue += 1
                    'ElseIf col.ColumnName = "League" Then
                    '    newrow(col.ColumnName) = "Hugh's"
                Else
                    newrow(col.ColumnName) = row(col.ColumnName)
                End If
            Next
            dtSubs.Rows.Add(newrow)
        Next

        'Subs
        'Dim subflds = "Team	Grade	TextStats	EmailStats	Skins	CTP	EOY".Split(vbTab)

        'Dim xls = New CreateXLSX
        ' Create a new Excel workbook
        Using workbook As New XLWorkbook($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
            ' Add sheets to the workbook
            workbook.Worksheets.Delete(dtSubs.TableName)
            workbook.Worksheets.Add(dtSubs, dtSubs.TableName)
            '' Save the workbook to a file
            workbook.SaveAs($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
        End Using
    End Sub

    Function fixscores(dtScores As DataTable) As DataTable
        dt = sqliteda(ctx.Conn, "Scores")
        dt.PrimaryKey = New DataColumn() {dt.Columns("Date"), dt.Columns("Player")}
        Dim newdtScores = New DataTable
        newdtScores.TableName = "Scores"
        Dim sScoreFields = New List(Of String)("League,Player,Date,Round,Group".Split(","))
        For Each fld In sScoreFields
            newdtScores.Columns.Add(fld)
        Next
        newdtScores.Columns.Add("FrontBack")
        For i = 1 To 9
            newdtScores.Columns.Add(i)
        Next
        newdtScores.Columns.Add("Gross")
        newdtScores.Columns.Add("Net")
        newdtScores.Columns.Add("Partner")

        For Each row As DataRow In dtScores.Rows
            'If sexclDates.Contains(row("Date")) Then Continue For
            If Not ctx.sExcludeDates.Contains(row("Date").ToString) Then Continue For
            Dim sfrontscores = "", sbackscores = ""
            For i = 1 To 18
                If i < 10 Then
                    sfrontscores &= row($"Hole{i}")
                Else
                    sbackscores &= row($"Hole{i}")
                End If
            Next

            If Len(sfrontscores) + Len(sbackscores) = 0 Then 'row("Out_Gross") = "" And row("In_Gross") = "" Then
                Continue For
            Else
                Dim newrow As DataRow = newdtScores.NewRow
                For Each fld In sScoreFields
                    newrow(fld) = row(fld)
                Next

                Dim j = 1
                If Len(sfrontscores) = 0 Then
                    newrow("FrontBack") = "Back"
                    newrow("Gross") = row("In_Gross")
                    newrow("Net") = row("In_Net")
                    j = 10
                Else
                    newrow("FrontBack") = "Front"
                    newrow("Gross") = row("Out_Gross")
                    newrow("Net") = row("Out_Net")
                End If
                For i = 0 To 8
                    Dim ptr = j + i
                    newrow($"{i + 1}") = row($"Hole{ptr}")
                Next
                newrow("Partner") = row("Partner")
                Dim drfind = dt.Rows.Find(New Object() {row("Date"), row("Player")})
                If drfind Is Nothing Then
                    newdtScores.Rows.Add(newrow)
                End If

                'logit("")
            End If

        Next
        Return newdtScores
    End Function
    Public Function ReadCsvIntoDataTable(filePath As String) As DataTable
        Dim dataTable As New DataTable()
        Using sr As New StreamReader(filePath)
            Dim headers As String() = sr.ReadLine().Split(","c)
            For Each header As String In headers
                If header = "" Then Continue For
                If header = "Team" And Not filePath.Contains("Players") Then
                    dataTable.Columns.Add(header, GetType(Int32))
                Else
                    dataTable.Columns.Add(header)
                End If
                'LOGIT($"{dataTable.Columns(header)}- {dataTable.Columns(header).DataType}")
            Next
            While Not sr.EndOfStream
                Dim rows As String() = sr.ReadLine().Split(","c)
                Dim dr As DataRow = dataTable.NewRow()
                For i As Integer = 0 To headers.Length - 1
                    If rows(i) <> "" Then
                        dr(i) = rows(i)
                    End If
                Next
                dataTable.Rows.Add(dr)
            End While
            'For Each col In dataTable.Columns
            '    LOGIT($"2-{dataTable.Columns(col.columnname)}- {dataTable.Columns(col.columnname).DataType}")
            'Next
        End Using
        Return dataTable
    End Function

#End Region
#Region "Schedule Generator"

    Function buildSchedule(Optional rebuild As Boolean = False) As DataTable
        buildSchedule = Nothing
        Try

            'Build the column header 
            Dim dtSchedule = New DataTable
            dt = sqliteda(ctx.Conn, "wkSchedule")
            dt.PrimaryKey = New DataColumn() {dt.Columns(0)}
            'build the columns
            For Each row As DataRow In dt.Rows
                dtSchedule.Columns.Add(row(0).ToString.Substring(0, row(0).ToString.LastIndexOf("/")))
            Next
            Dim arow As DataRow
            If Not rebuild Then
                Dim psDate As String = $"{CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("MM/dd")}"
                arow = dt.NewRow
                arow("Date") = psDate
                dt.Rows.Add($"{psDate}/{sPSDate.Substring(0, 4)}")
                dtSchedule.Columns.Add(psDate)
                psDate = $"{CDate(ctx.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("MM/dd")}"
                arow("Date") = psDate
                dt.Rows.Add($"{psDate}/{sPSDate.Substring(0, 4)}")
                dtSchedule.Columns.Add(psDate)
            End If
            'build rows
            For i = 1 To dt.Columns.Count - 1
                arow = dtSchedule.NewRow
                For Each row As DataRow In dt.Rows
                    arow(row(0).ToString.Substring(0, 5)) = row(i)
                Next
                dtSchedule.Rows.Add(arow)
            Next

            buildSchedule = dtSchedule
            buildSchedule.TableName = "Schedule"
        Catch ex As Exception

        End Try
    End Function

    'Random Schedule Generator
    Sub SchGenerator(iteams As Int16, sdate As Date)
        Dim dtSchedule = dsLeague.Tables("wkSchedule")
        Dim teams As List(Of String) = GenerateTeams(ctx.rLeagueParmrow("Teams"))
        Dim schedule As List(Of (String, String, Date)) = GenerateRoundRobinSchedule(teams, sdate)
        ' Display the schedule
        'Date {sdate.ToString("MM-dd-yy")}: 
        Dim newrow As DataRow = Nothing
        Dim sp As Double = 0
        For Each match As (String, String, Date) In schedule
            'Console.WriteLine($"Round {match.Item3}: {match.Item1} vs {match.Item2}")
            If newrow Is Nothing Then
                newrow = dtSchedule.NewRow
                newrow("Date") = match.Item3.ToString("MM/dd/yyyy")
            End If
            If newrow("Date") <> match.Item3.ToString("MM/dd/yyyy") Then
                dtSchedule.Rows.Add(newrow)
                newrow = dtSchedule.NewRow
                newrow("Date") = match.Item3.ToString("MM/dd/yyyy")
                sp = 0
            End If
            newrow($"{sp + 1}") = $"{match.Item1}v{match.Item2}"
            sp += 1
            'logit($"Date {match.Item3.ToString("MM/dd/yyyy")}: {match.Item1} vs {match.Item2}")

        Next
        dtSchedule.Rows.Add(newrow)

    End Sub

    Function GenerateTeams(count As Integer) As List(Of String)
        Dim teams As New List(Of String)()
        For i As Integer = 1 To count
            teams.Add($"{i}")
        Next
        Return teams
    End Function
    'Function GenerateRoundRobinSchedule(teams As List(Of String)) As List(Of (String, String, Integer))

    Function GenerateRoundRobinSchedule(teams As List(Of String), sdate As Date) As List(Of (String, String, Date))
        Dim schedule As New List(Of (String, String, Date))()
        Dim n As Integer = teams.Count

        ' Ensure even number of teams by adding a dummy team if necessary
        If n Mod 2 <> 0 Then
            teams.Add("Bye")
            n += 1
        End If

        For round As Integer = 0 To n - 2
            For i As Integer = 0 To n / 2 - 1
                Dim team1 As String = teams(i)
                Dim team2 As String = teams(n - 1 - i)

                ' Avoid adding a match twice by making sure team1 < team2
                If team1 <> "Bye" AndAlso team2 <> "Bye" Then
                    'schedule.Add((team1, team2, round + 1))
                    schedule.Add((team1, team2, sdate.ToString("MM/dd/yyyy")))
                End If
            Next
            ' Rotate teams
            Dim temp As String = teams(n - 1)
            For j As Integer = n - 1 To 1 Step -1
                teams(j) = teams(j - 1)
            Next
            teams(1) = temp
            sdate = sdate.AddDays(7)
        Next

        Return schedule
    End Function
    'PivotTable
    ''http://www.codingeverything.com/2014/06/PivotDataTable.html
    Shared Function PivotTable(oldTable As DataTable,
                                Optional pivotColumnOrdinal As Integer = 0
                               ) As DataTable
        Dim newTable As New DataTable
        Dim dr As DataRow

        ' add pivot column name
        'newTable.Columns.Add(oldTable.Columns(pivotColumnOrdinal).ColumnName)

        ' add pivot column values in each row as column headers to new Table
        For Each row In oldTable.Rows
            newTable.Columns.Add(row(pivotColumnOrdinal))
        Next

        ' loop through columns
        For col = 0 To oldTable.Columns.Count - 1
            'pivot column doesn't get it's own row (it is already a header)
            If col = pivotColumnOrdinal Then Continue For

            ' each column becomes a new row
            dr = newTable.NewRow()

            ' add the Column Name in the first Column
            'dr(0) = oldTable.Columns(col).ColumnName

            ' add data from every row to the pivoted row
            For row = 0 To oldTable.Rows.Count - 1
                dr(row) = oldTable.Rows(row)(col)
            Next

            'add the DataRow to the new table
            newTable.Rows.Add(dr)
        Next

        Return newTable
    End Function
    Public Sub buildtablesfromworksheet(connection As SQLiteConnection, csvFilePath As String)
        ' Specify the path to the Excel file
        Dim oClosedXML = New ClosedXML
        Dim excelFilePath As String = $"{csvFilePath}Hugh'sLeague.xlsx"
        Dim et As TimeSpan
        Dim st As DateTime = Now
        Try
            Using workbook As New XLWorkbook(excelFilePath)
            End Using

        Catch ex As Exception
            MessageBox.Show($"{excelFilePath} in use{vbCrLf}{vbCrLf}Will scan computer to see which app is using{vbCrLf}This can take up to 1 minute", "File in Use", MessageBoxButtons.OK)
            Dim applications As List(Of String) = FileUsageChecker.GetFileUsage(excelFilePath)
            et = Now - st
            If et.TotalMinutes >= 1 Then
                ScoreCard.dp($"Handle took {CInt(et.TotalMinutes) Mod 60} Min :{CInt(et.TotalSeconds) Mod 60} Secs")
            Else
                ScoreCard.dp($"Handle took {CInt(et.TotalSeconds) Mod 60} Secs")
            End If
            ScoreCard.dp("Applications using the file:")
            For Each app In applications
                MessageBox.Show($"Close {app} and retry", "File in Use", MessageBoxButtons.OK)
                End
                ScoreCard.dp(app)
            Next

        End Try

        File.Copy(excelFilePath, excelFilePath.Replace("Hugh'sLeague", $"Hugh'sLeague_{Now.ToString("yyyyMMdd")}"), True)
        ' Read the Excel file
        Using workbook As New XLWorkbook(excelFilePath)
            ' Create DataTables from the worksheets
            Dim dataTables As Dictionary(Of String, DataTable) = oClosedXML.CreateDataTablesFromWorksheets(workbook)
            ' Display the contents of the DataTables
            Dim i = 1
            For Each sheetName As String In dataTables.Keys
                If Not sheetName.ToLower.StartsWith("nu-") Then
                    Dim dataTable As DataTable = dataTables(sheetName)
                    'this creates a table in memory
                    'If dataTable.TableName.StartsWith("Schedule") Then
                    '    dataTable.TableName = $"[{dataTable.TableName}]"
                    'End If
                    'LOGIT($"{dataTable.TableName}-{i} of {dataTables.Keys.Count}")
                    SQLiteCreateTableFromDT(connection, dataTable)
                    Dim x = ""
                    i += 1
                    'showtables(connection)
                    'LOGIT(sheetName)
                End If
            Next
        End Using
        Exit Sub

        Dim dt = New DataTable
        dt.TableName = "Schedule"
        dt.Columns.Add("ID")
        dt.Columns.Add("Date")
        For i = 1 To 10
            dt.Columns.Add($"{i}")
        Next
        Dim ID As Integer = 0
        Dim lsch = New List(Of String)
        lsch.Add("Schedule")
        Dim query As String = "SELECT * FROM sqlite_master WHERE type = 'table' AND Name LIKE 'Schedule%' ORDER BY Name;"
        Using command As New SQLiteCommand(query, connection)
            Using reader As SQLiteDataReader = command.ExecuteReader()
                While reader.Read()
                    'logit($"{reader("name")}")
                    If reader("name").ToString = "Schedule" Then Continue While
                    lsch.Add(reader("name").ToString)
                    Dim wkdt = New DataTable
                    wkdt = setupwksch(wkdt, reader("Name"))
                    For Each row As DataRow In wkdt.Rows
                        Dim newrow As DataRow
                        newrow = dt.NewRow
                        ID += 1
                        newrow("ID") = ID
                        For Each col As DataColumn In wkdt.Columns
                            newrow(col.ColumnName) = row(col.ColumnName)
                        Next
                        dt.Rows.Add(newrow)
                    Next
                    wk = ""
                    'Console.WriteLine(reader("name"))
                End While
            End Using
        End Using
        For Each tbl As String In lsch
            Using command As New SQLiteCommand(query, connection)
            End Using
            Dim com2 As New SQLiteCommand($"DROP TABLE IF EXISTS {tbl};", connection)
            'logit(com2.CommandText)
            Try
                com2.ExecuteNonQuery()
            Catch ex As Exception
                LOGIT($"{tbl}-{ex.Message}")
            End Try

        Next
        SQLiteCreateTableFromDT(connection, dt)
        CreateWorkbookFromSQLite(connection)

    End Sub
    Function setupwksch(dtSchedule As DataTable, tablename As String) As DataTable
        'begin 1226
        'Dim dt As DataTable = oHelper.sqlitedaFromSql(connection, "ScoreDates", $"SELECT DISTINCT Date FROM Scores WHERE SUBSTRING(Date,1,4) = '{oHelper.sPSDate.Substring(0, 4)}' AND Date NOT IN (SELECT DISTINCT Date FROM ClubChampionship WHERE SUBSTRING(Date,1,4) = '{oHelper.sPSDate.Substring(0, 4)}') ORDER BY Date DESC")
        'new date for rainout
        'Dim sLastScoreDate = dt.Rows(0)(0)
        dtSchedule = New DataTable
        Dim dt = sqliteda(ctx.Conn, tablename)
        dtSchedule.TableName = "Schedule"
        dtSchedule.Columns.Add("Date")
        For Each row In dt.Rows
            dtSchedule.Columns.Add(row("ID"))
        Next
        Dim irow As Integer = 0
        For j = 0 To dt.Columns.Count - 1
            If dt.Columns(j).ColumnName = "ID" Then Continue For
            If dt.Rows(0)(dt.Columns(j).ColumnName) <> "" Then
                Dim newrow As DataRow = dtSchedule.NewRow()
                newrow("Date") = CDate(dt.Columns(j).ColumnName).ToString("yyyyMMdd")
                For i = 0 To dt.Rows.Count - 1
                    newrow(i + 1) = dt.Rows(i)(dt.Columns(j).ColumnName)
                Next
                dtSchedule.Rows.Add(newrow)
            End If

            irow += 1
            If irow = dt.Rows.Count Then irow = 0
        Next
        setupwksch = dtSchedule
    End Function

    Public Sub CreateWorkbookFromSQLite(connection As SQLiteConnection)
        ' Create a new Excel workbook
        Using workbook As New XLWorkbook()
            ' Add sheets to the workbook
            ' SQL query to list all tables
            Dim query As String = "SELECT name FROM sqlite_master WHERE type = 'table';"
            Using command As New SQLiteCommand(query, connection)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    While reader.Read()
                        'logit(reader("name"))
                        'Console.WriteLine(reader("name"))
                        Dim dt = sqliteda(connection, reader("Name"))
                        workbook.Worksheets.Add(dt, dt.TableName)
                    End While
                End Using
            End Using

            '' Save the workbook to a file
            'Dim dv = New DataView(ohelper.dsLeague.Tables("LeagueParms"))
            'dv.RowFilter = "startdate like '%2024%'"
            'ctx.rLeagueParmrow = dv(0)

            workbook.SaveAs($"{ctx.csvFilePath}Hugh'sLeague.xlsx")
        End Using
    End Sub
    Function CreateColumnsWithFormat(fld As String, dt As DataTable, sColFormat As List(Of String)) As DataColumn
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        CreateColumnsWithFormat = New DataColumn()
        With CreateColumnsWithFormat
            Dim sParm = ""
            If UBound(fld.Split("-")) = 0 Then
                sParm = fld
            Else
                sParm = fld.Split("-")(0)
                sColFormat.Add(fld.Substring(fld.IndexOf("-") + 1))
            End If
            CreateColumn(sParm, dt)
        End With

    End Function
    Function CreateNumColumn(fld As String, dt As DataTable) As DataColumn
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        CreateNumColumn = New DataColumn()
        With CreateNumColumn
            Dim sParm = ""
            If UBound(fld.Split("-")) = 0 Then
                sParm = fld
            Else
                sParm = fld.Split("-")(0)
            End If
            .DataType = System.Type.GetType("System.Int16")
            .ColumnName = fld.Split("-")(0)
            ' Add the column to the DataTable.Columns collection.
            LOGIT(String.Format("{0}-{1}", fld.Split("-")(0), .DataType))
            dt.Columns.Add(.ColumnName)
        End With

    End Function
    Function CreateColumn(fld As String, dt As DataTable) As DataColumn
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        CreateColumn = New DataColumn()
        With CreateColumn
            .DataType = System.Type.GetType("System.String")
            .ColumnName = fld
            ' Add the column to the DataTable.Columns collection.
            dt.Columns.Add(.ColumnName)
        End With

    End Function
    Function createDT(tflds As String()) As DataTable
        createDT = New DataTable
        For Each fld As String In tflds
            If fld.Split("-").Length > 1 Then
                If fld.Split("-")(1) = "d" Then
                    createDT.Columns.Add(fld.Split("-")(0), GetType(Decimal))
                Else
                    MsgBox("Unknown field type")
                End If
            Else
                createDT.Columns.Add(fld, GetType(String))
            End If
        Next
        Return createDT
    End Function
    Function createDT(tfldsdel As String) As DataTable
        createDT = New DataTable
        createDT = createDT(tfldsdel.Split(","c))
        Return createDT
    End Function
    Function createDTWithPK(dtName As String, flds As String, Optional sql As String = "") As DataTable

        Dim dt As DataTable
        If sql <> "" Then
            dt = sqlitedaFromSql(ctx.Conn, dtName, sql)
        Else
            dt = sqliteda(ctx.Conn, dtName)
        End If

        ' Set the PrimaryKey property
        createPK(dt, flds)
        Return dt
    End Function
    Sub createPK(dt As DataTable, flds As String)

        ' Split the field list into an array
        Dim fieldNames() As String = flds.Split(","c)

        ' Create an array of DataColumn objects sized to the number of fields
        Dim keys(fieldNames.Length - 1) As DataColumn

        ' Loop through each field name and assign the corresponding DataColumn
        For i As Integer = 0 To fieldNames.Length - 1
            Dim fldName As String = fieldNames(i).Trim()
            keys(i) = dt.Columns(fldName)
        Next

        ' Set the PrimaryKey property
        dt.PrimaryKey = keys
    End Sub
    ' ============================================================
    '   LEAGUE PARMS ENGINE (DYNAMIC)
    ' ============================================================

    Public Shared Property LeagueParms As LeagueParms

    ' ============================================================
    '   ENTRY POINT — called from your existing Init()
    ' ============================================================
    Public Sub InitLeagueParmsFromFiles(configFolder As String, season As Integer, dp As Action(Of String))

        Dim dtTemplate As New DataTable("LeagueTemplate")
        Dim dtLeagues As New DataTable("Leagues")

        ' FIX: use Me. because CSV2DataTable is an instance method
        Me.CSV2DataTable(dtTemplate, IO.Path.Combine(configFolder, "LeagueTemplate.csv"))
        Me.CSV2DataTable(dtLeagues, IO.Path.Combine(configFolder, "Leagues.csv"))

        ValidateTemplateColumns(dtTemplate, dp)
        ValidateLeagueColumns(dtLeagues, dp)

        Dim leagueRow = GetLeagueRowForSeason(dtLeagues, season)
        Dim templateRow = GetTemplateRowForLeague(dtTemplate, leagueRow)

        Me.LeagueParms = MergeLeagueParmsDynamic(leagueRow, templateRow, dp)

        ValidateLeagueParms(Me.LeagueParms, dp)
        DumpLeagueParmsDynamic(Me.LeagueParms, dp)
    End Sub

    ' ============================================================
    '   TYPE INFERENCE
    ' ============================================================
    Private Shared Function InferType(raw As Object) As Object
        If raw Is Nothing OrElse raw Is DBNull.Value Then Return Nothing

        Dim s = raw.ToString().Trim()
        If s = "" Then Return Nothing

        If s.Equals("Y", StringComparison.OrdinalIgnoreCase) Then Return True
        If s.Equals("N", StringComparison.OrdinalIgnoreCase) Then Return False

        Dim b As Boolean
        If Boolean.TryParse(s, b) Then Return b

        Dim i As Integer
        If Integer.TryParse(s, i) Then Return i

        Dim d As Double
        If Double.TryParse(s, d) Then Return d

        Dim dt As Date
        If Date.TryParse(s, dt) Then Return dt

        Return s
    End Function


    ' ============================================================
    '   MERGE ROUTINE
    ' ============================================================
    Private Shared Function MergeLeagueParmsDynamic(leagueRow As DataRow,
                                                templateRow As DataRow,
                                                dp As Action(Of String)) As LeagueParms

        Dim lp As New LeagueParms()

        For Each col As DataColumn In templateRow.Table.Columns
            lp.Values(col.ColumnName) = InferType(templateRow(col.ColumnName))
        Next

        For Each col As DataColumn In leagueRow.Table.Columns
            Dim v = leagueRow(col.ColumnName)
            If v IsNot DBNull.Value AndAlso v.ToString().Trim() <> "" Then
                lp.Values(col.ColumnName) = InferType(v)
            End If
        Next

        dp("Dynamic LeagueParms merged with type inference.")
        Return lp
    End Function


    ' ============================================================
    '   ROW FINDERS
    ' ============================================================
    Private Shared Function GetLeagueRowForSeason(dtLeagues As DataTable, season As Integer) As DataRow
        Dim rows = dtLeagues.Select($"Season = {season}")
        If rows.Length = 0 Then Throw New Exception($"No league row found for season {season}.")
        If rows.Length > 1 Then Throw New Exception($"Multiple league rows found for season {season}.")
        Return rows(0)
    End Function

    Private Shared Function GetTemplateRowForLeague(dtTemplate As DataTable, leagueRow As DataRow) As DataRow
        Dim tname = leagueRow("TemplateName").ToString().Trim()
        If tname = "" Then Throw New Exception("League row has empty TemplateName.")

        Dim safeName = tname.Replace("'", "''")
        Dim rows = dtTemplate.Select($"TemplateName = '{safeName}'")

        If rows.Length = 0 Then Throw New Exception($"No template row found for TemplateName [{tname}].")
        If rows.Length > 1 Then Throw New Exception($"Multiple template rows found for TemplateName [{tname}].")

        Return rows(0)
    End Function


    ' ============================================================
    '   COLUMN VALIDATION
    ' ============================================================
    Private Shared Sub ValidateTemplateColumns(dt As DataTable, dp As Action(Of String))
        Dim required = {
        "TemplateName", "Format", "HdcpFormat", "CarryLastYears", "MaxHdcp",
        "Par3Max", "Par4Max", "Par5Max", "Holes", "Method", "SkinFmt",
        "Closest", "ClosestPS", "Skins", "SkinsPS", "SplitSeason", "Course",
        "Start9", "DayOfWeek", "Secretary", "Email", "WebsitePassword", "EmailPassword"
    }

        For Each col In required
            If Not dt.Columns.Contains(col) Then
                Throw New Exception($"Missing column [{col}] in LeagueTemplate.")
            End If
        Next

        dp("LeagueTemplate columns validated.")
    End Sub

    Private Shared Sub ValidateLeagueColumns(dt As DataTable, dp As Action(Of String))
        Dim required = {
        "LeagueID", "LeagueName", "TemplateName", "Cost", "EOYSkins", "Teams",
        "PayPlaces", "Season", "StartDate", "EndDate", "PostSeasonDt",
        "Rainouts", "HdcpScores", "Byes", "CarryOver"
    }

        For Each col In required
            If Not dt.Columns.Contains(col) Then
                Throw New Exception($"Missing column [{col}] in Leagues.")
            End If
        Next

        dp("Leagues columns validated.")
    End Sub


    ' ============================================================
    '   VALIDATION RULES
    ' ============================================================
    Private Shared Sub ValidateLeagueParms(lp As LeagueParms, dp As Action(Of String))

        If lp.HasKey("MaxHdcp") Then
            Dim maxHdcp = lp.Int("MaxHdcp")
            If maxHdcp <= 0 Then
                Throw New Exception("MaxHdcp must be greater than zero.")
            End If
        End If

        If lp.HasKey("StartDate") Then
            If lp.Date("StartDate") = Date.MinValue Then
                Throw New Exception("StartDate is invalid.")
            End If
        End If

        If lp.HasKey("EndDate") Then
            If lp.Date("EndDate") = Date.MinValue Then
                Throw New Exception("EndDate is invalid.")
            End If
        End If

        dp("LeagueParms validation passed.")
    End Sub


    ' ============================================================
    '   DEBUG OVERLAY
    ' ============================================================
    Private Shared Sub DumpLeagueParmsDynamic(lp As LeagueParms, dp As Action(Of String))
        dp("LeagueParms (dynamic):")
        For Each kv In lp.Values
            dp($"  {kv.Key} = {kv.Value}")
        Next
    End Sub

#End Region
    Public Class rank
        Public Hole As String
        Public toPar As Decimal

        Public Sub New(ByVal hole As String, ByVal toPar As Decimal)
            Me.Hole = hole
            Me.toPar = toPar
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}", Me.Hole, Me.toPar)
        End Function

    End Class
    Public Function IsFileLocked(path As String) As Boolean
        Try
            Using fs As FileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            End Using
            Return False
        Catch ex As IOException
            Return True
        End Try
    End Function

    Public Function GetStringHash(input As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes = Encoding.UTF8.GetBytes(input)
            Dim hashBytes = sha.ComputeHash(bytes)
            Return BitConverter.ToString(hashBytes).Replace("-", "")
        End Using
    End Function
    Public Function GetSheetHash(ws As IXLWorksheet) As String
        Dim sb As New StringBuilder()

        ' 1. Get the header row (first row with actual content)
        Dim headerRow = ws.FirstRowUsed()
        If headerRow Is Nothing Then Return ""

        ' Hash only the header text
        For Each cell In headerRow.CellsUsed()
            sb.Append(cell.GetString().Trim()).Append("|")
        Next
        sb.AppendLine()

        ' 2. Hash only meaningful data rows
        For Each row In ws.RowsUsed().Skip(1)
            ' Skip rows that are completely blank
            If row.CellsUsed().All(Function(c) c.GetString().Trim() = "") Then
                Continue For
            End If

            ' Hash each cell's trimmed value
            For Each cell In row.CellsUsed()
                sb.Append(cell.GetString().Trim()).Append("|")
            Next
            sb.AppendLine()
        Next

        ' 3. Compute SHA256 hash of the normalized content
        Using sha As SHA256 = SHA256.Create()
            Dim bytes = Encoding.UTF8.GetBytes(sb.ToString())
            Dim hash = sha.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "")
        End Using
    End Function
    Public Function TimeAction(action As Action) As TimeSpan
        Dim sw As New Stopwatch()
        sw.Start()
        action()
        sw.Stop()
        Return sw.Elapsed
    End Function
    Private ReadOnly Property HashDbPath As String
        Get
            Return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HughsGolf_Hashes.db")
        End Get
    End Property
    Public Function GetStoredHash(sheetName As String, HashDbPath As String) As String
        Using conn As New SQLiteConnection($"Data Source={HashDbPath};Version=3;")
            conn.Open()
            Using cmd As New SQLiteCommand("
            SELECT HashValue 
            FROM SheetHashes 
            WHERE SheetName=@name;
        ", conn)
                cmd.Parameters.AddWithValue("@name", sheetName)
                Dim result = cmd.ExecuteScalar()
                If result Is Nothing Then Return ""
                Return result.ToString()
            End Using
        End Using
    End Function
    Public Sub SetStoredHash(sheetName As String, hashValue As String, HashDbPath As String)
        Using conn As New SQLiteConnection($"Data Source={HashDbPath};Version=3;")
            conn.Open()
            Using cmd As New SQLiteCommand("
            INSERT INTO SheetHashes (SheetName, HashValue)
            VALUES (@name, @hash)
            ON CONFLICT(SheetName) DO UPDATE SET HashValue=@hash;
        ", conn)
                cmd.Parameters.AddWithValue("@name", sheetName)
                cmd.Parameters.AddWithValue("@hash", hashValue)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Public Sub EnsureHashDbExists(hashDbPath As String)
        'Dim hashDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HughsGolf_Hashes.db")

        If Not File.Exists(hashDbPath) Then
            SQLiteConnection.CreateFile(hashDbPath)
        End If

        Using conn As New SQLiteConnection($"Data Source={hashDbPath};Version=3;")
            conn.Open()
            Using cmd As New SQLiteCommand("
            CREATE TABLE IF NOT EXISTS SheetHashes (
                SheetName TEXT PRIMARY KEY,
                HashValue TEXT NOT NULL
            );
        ", conn)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Public Function ParseGallusScorecard(url As String) As List(Of Dictionary(Of String, Object))
        Dim results As New List(Of Dictionary(Of String, Object))
        Try
            ' Fetch HTML
            Dim html As String = ""
            Dim request As System.Net.HttpWebRequest = System.Net.WebRequest.Create(url)
            request.Method = "GET"
            request.UserAgent = "Mozilla/5.0"
            request.Timeout = 10000
            Using response As System.Net.HttpWebResponse = request.GetResponse()
                Using sr As New System.IO.StreamReader(response.GetResponseStream())
                    html = sr.ReadToEnd()
                End Using
            End Using

            LOGIT($"ParseGallus: fetched {html.Length} chars")

            ' Extract all table rows
            Dim allRows = System.Text.RegularExpressions.Regex.Matches(
            html, "(?s)<tr[^>]*>(.*?)</tr>",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            LOGIT($"ParseGallus: found {allRows.Count} table rows")

            ' Parse cells helper — always add every cell to preserve column alignment
            Dim parseCells As Func(Of String, List(Of String)) = Function(rowHtml As String)
                                                                     Dim cells As New List(Of String)
                                                                     Dim cellMatches = System.Text.RegularExpressions.Regex.Matches(
                rowHtml, "<t[dh][^>]*>(.*?)</t[dh]>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase Or
                System.Text.RegularExpressions.RegexOptions.Singleline)
                                                                     For Each cm As System.Text.RegularExpressions.Match In cellMatches
                                                                         Dim val = System.Text.RegularExpressions.Regex.Replace(cm.Groups(1).Value, "<.*?>", "").Trim()
                                                                         val = System.Text.RegularExpressions.Regex.Replace(val, "\s+", " ").Trim()
                                                                         cells.Add(val)  ' Always add — even empty — to keep column positions intact
                                                                     Next
                                                                     Return cells
                                                                 End Function

            ' Find par, hcp and all player rows
            Dim parRow As String = ""
            Dim hcpRow As String = ""
            Dim playerRows As New List(Of String)

            Dim skipKeywords As New List(Of String) From {
            "Boone Links", "Hole", "Par", "Hcp", "Out", "In", "Total"
        }

            For Each r As System.Text.RegularExpressions.Match In allRows
                Dim rowText = r.Groups(1).Value
                Dim stripped = System.Text.RegularExpressions.Regex.Replace(rowText, "<.*?>", " ")
                stripped = System.Text.RegularExpressions.Regex.Replace(stripped, "\s+", " ").Trim()

                If stripped.Contains("Par") Then
                    parRow = rowText
                ElseIf stripped.Contains("Hcp") Then
                    hcpRow = rowText
                Else
                    Dim isSkip As Boolean = False
                    For Each kw As String In skipKeywords
                        If stripped.StartsWith(kw) Then
                            isSkip = True
                            Exit For
                        End If
                    Next
                    If Not isSkip AndAlso stripped.Length > 3 Then
                        Dim cells = parseCells(rowText)
                        If cells.Count >= 10 Then
                            playerRows.Add(rowText)
                        End If
                    End If
                End If
            Next

            LOGIT($"ParseGallus: parRow={parRow.Length > 0} hcpRow={hcpRow.Length > 0} playerRows={playerRows.Count}")

            Dim parCells = parseCells(parRow)
            Dim hcpCells = parseCells(hcpRow)

            ' 0-based arrays: index 0=hole1(or 10), index 8=hole9(or 18)
            Dim front9Par(8) As Integer
            Dim back9Par(8) As Integer
            Dim front9Hcp(8) As Integer
            Dim back9Hcp(8) As Integer

            For i As Integer = 1 To 9
                If i < parCells.Count Then Integer.TryParse(parCells(i), front9Par(i - 1))
                Dim backIdx = i + 10
                If backIdx < parCells.Count Then Integer.TryParse(parCells(backIdx), back9Par(i - 1))
            Next

            For i As Integer = 1 To 9
                If i < hcpCells.Count Then Integer.TryParse(hcpCells(i), front9Hcp(i - 1))
                Dim backIdx = i + 10
                If backIdx < hcpCells.Count Then Integer.TryParse(hcpCells(backIdx), back9Hcp(i - 1))
            Next

            ' Process each player row
            For Each playerRow As String In playerRows
                Dim scoreCells = parseCells(playerRow)
                If scoreCells.Count = 0 Then Continue For

                Dim playerName As String = scoreCells(0).Trim()
                LOGIT($"ParseGallus: processing player={playerName} cells={scoreCells.Count}")

                ' 0-based: hole 1/10 = index 0, hole 9/18 = index 8
                Dim front9Scores(8) As Integer
                Dim back9Scores(8) As Integer

                ' Front 9 — cells 1-9, stored at 0-8
                For i As Integer = 1 To 9
                    If i < scoreCells.Count Then
                        Dim val As Integer
                        Integer.TryParse(scoreCells(i), val)
                        front9Scores(i - 1) = If(val >= 1 AndAlso val <= 12, val, 0)
                    End If
                Next

                ' Back 9 — cells 11-19 (skip Out at index 10), stored at 0-8
                For i As Integer = 1 To 9
                    Dim idx = i + 10
                    If idx < scoreCells.Count Then
                        Dim val As Integer
                        Integer.TryParse(scoreCells(idx), val)
                        back9Scores(i - 1) = If(val >= 1 AndAlso val <= 12, val, 0)
                    End If
                Next

                Dim frontGross As Integer = front9Scores.Sum()
                Dim backGross As Integer = back9Scores.Sum()

                LOGIT($"ParseGallus: {playerName} frontGross={frontGross} backGross={backGross}")

                Dim result As New Dictionary(Of String, Object)
                result("PlayerName") = playerName
                result("Front9Scores") = front9Scores
                result("Back9Scores") = back9Scores
                result("Front9Par") = front9Par
                result("Back9Par") = back9Par
                result("FrontGross") = frontGross
                result("BackGross") = backGross
                result("PlayedFront") = frontGross > 0
                result("PlayedBack") = backGross > 0
                result("Front9Hcp") = front9Hcp
                result("Back9Hcp") = back9Hcp
                results.Add(result)
            Next

        Catch ex As Exception
            LOGIT($"ParseGallusScorecard Error: {ex.Message}")
        End Try
        Return results
    End Function

    Public Function ParseScorecardViaOCR(imagePath As String) As List(Of Dictionary(Of String, Object))
        Dim results As New List(Of Dictionary(Of String, Object))
        Try
            Dim tessDataPath As String = "C:\HughsGolf\Files\tessdata"
            MessageBox.Show($"Folder exists: {Directory.Exists(tessDataPath)}" & vbCrLf &
                $"File exists: {File.Exists(Path.Combine(tessDataPath, "eng.traineddata"))}")
            Using engine As New Tesseract.TesseractEngine(tessDataPath, "eng", Tesseract.EngineMode.Default)
                Using img = Tesseract.Pix.LoadFromFile(imagePath)
                    Using page = engine.Process(img)
                        Dim fullText As String = page.GetText()
                        Dim confidence As Single = page.GetMeanConfidence()
                        LOGIT($"Tesseract OCR confidence: {confidence} text: {fullText}")

                        ' If confidence too low fall back to Claude
                        If confidence < 0.6 Then
                            LOGIT("Tesseract confidence too low, falling back to Claude")
                            Return New List(Of Dictionary(Of String, Object))
                        End If

                        results = ParseScorecardText(fullText)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            LOGIT($"ParseScorecardViaOCR error: {ex.Message}")
            Return New List(Of Dictionary(Of String, Object))
        End Try
        Return results
    End Function

    Private Function ParseScorecardText(fullText As String) As List(Of Dictionary(Of String, Object))
        Dim results As New List(Of Dictionary(Of String, Object))
        Try
            ' Split into lines and look for rows that have a name followed by 9 numbers
            Dim lines() As String = fullText.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

            For Each line As String In lines
                Dim parts() As String = line.Trim().Split(New Char() {" "c, vbTab(0)}, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 4 Then Continue For

                ' Find where numbers start - assume first part(s) are name
                Dim nameTokens As New List(Of String)
                Dim scoreTokens As New List(Of Integer)

                For Each part As String In parts
                    Dim n As Integer
                    If Integer.TryParse(part, n) AndAlso n >= 1 AndAlso n <= 15 Then
                        scoreTokens.Add(n)
                    ElseIf scoreTokens.Count = 0 Then
                        nameTokens.Add(part)
                    End If
                Next

                ' Need at least 9 scores and a name
                If scoreTokens.Count >= 9 AndAlso nameTokens.Count > 0 Then
                    Dim playerName As String = String.Join(" ", nameTokens)
                    Dim scores(9) As Integer
                    For i As Integer = 1 To 9
                        scores(i) = scoreTokens(i - 1)
                    Next

                    Dim pd As New Dictionary(Of String, Object)
                    pd("PlayerName") = playerName
                    pd("PlayedFront") = True
                    pd("Front9Scores") = scores
                    pd("Back9Scores") = New Integer(9) {}
                    pd("Front9Par") = New Integer(9) {}
                    pd("Back9Par") = New Integer(9) {}
                    pd("Front9Hcp") = New Integer(9) {}
                    pd("Back9Hcp") = New Integer(9) {}
                    results.Add(pd)
                    LOGIT($"OCR parsed: {playerName} scores: {String.Join(",", scores.Skip(1))}")
                End If
            Next
        Catch ex As Exception
            LOGIT($"ParseScorecardText error: {ex.Message}")
        End Try
        Return results
    End Function

    Public Function ParseScorecardViaClaude(imagePath As String, apiKey As String) As List(Of Dictionary(Of String, Object))
        Dim results As New List(Of Dictionary(Of String, Object))
        Try
            ' Convert image to base64
            Dim imageBytes() As Byte = File.ReadAllBytes(imagePath)
            Dim base64Image As String = Convert.ToBase64String(imageBytes)

            ' Determine media type
            Dim ext As String = Path.GetExtension(imagePath).ToLower()
            Dim mediaType As String = "image/jpeg"
            If ext = ".png" Then mediaType = "image/png"
            If ext = ".bmp" Then mediaType = "image/bmp"

            ' Build request using JsonObject to avoid JSON injection issues
            Dim requestObj As New System.Text.Json.Nodes.JsonObject
            requestObj("model") = "claude-haiku-4-5-20251001"
            requestObj("max_tokens") = 1000

            Dim imageSource As New System.Text.Json.Nodes.JsonObject
            imageSource("type") = "base64"
            imageSource("media_type") = mediaType
            imageSource("data") = base64Image

            Dim imageContent As New System.Text.Json.Nodes.JsonObject
            imageContent("type") = "image"
            imageContent("source") = imageSource

            Dim textContent As New System.Text.Json.Nodes.JsonObject
            textContent("type") = "text"
            textContent("text") = "This is a golf scorecard. Extract the player names and their 9 hole scores. Return ONLY a JSON array, no other text, no markdown, no explanation. Format exactly like this: [{""name"":""John Smith"",""scores"":[5,4,3,6,5,4,7,3,5]}]. Include only rows that have a player name and exactly 9 numeric scores. Ignore par rows, handicap rows, yardage rows and totals."

            Dim contentArray As New System.Text.Json.Nodes.JsonArray
            contentArray.Add(imageContent)
            contentArray.Add(textContent)

            Dim messageObj As New System.Text.Json.Nodes.JsonObject
            messageObj("role") = "user"
            messageObj("content") = contentArray

            Dim messagesArray As New System.Text.Json.Nodes.JsonArray
            messagesArray.Add(messageObj)

            requestObj("messages") = messagesArray

            Dim requestBody As String = requestObj.ToJsonString()

            ' Call Claude API
            Dim client As New System.Net.Http.HttpClient()
            client.DefaultRequestHeaders.Add("x-api-key", apiKey)
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01")

            Dim content As New System.Net.Http.StringContent(requestBody, System.Text.Encoding.UTF8, "application/json")
            Dim response = client.PostAsync("https://api.anthropic.com/v1/messages", content).GetAwaiter().GetResult()
            Dim responseBody As String = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()

            LOGIT($"Claude API response: {responseBody}")

            ' Parse response to get the text content
            Dim responseDoc = System.Text.Json.JsonDocument.Parse(responseBody)
            Dim rootEl = responseDoc.RootElement
            Dim contentEl = rootEl.GetProperty("content")
            Dim firstEl = contentEl.Item(0)
            Dim claudeText As String = firstEl.GetProperty("text").GetString()
            ' Strip markdown code fences if present
            claudeText = claudeText.Replace("```json", "").Replace("```", "").Trim()

            LOGIT($"Claude extracted text: {claudeText}")

            ' Parse the JSON array Claude returned
            Dim playersDoc = System.Text.Json.JsonDocument.Parse(claudeText)

            For Each thisPlayer In playersDoc.RootElement.EnumerateArray()
                Dim playerName As String = thisPlayer.GetProperty("name").GetString()
                Dim scoresArray = thisPlayer.GetProperty("scores")
                Dim scores(9) As Integer
                Dim idx As Integer = 1
                For Each scoreEl In scoresArray.EnumerateArray()
                    If idx <= 9 Then
                        scores(idx) = scoreEl.GetInt32()
                        idx += 1
                    End If
                Next
                Dim pd As New Dictionary(Of String, Object)
                pd("PlayerName") = playerName
                pd("PlayedFront") = True
                pd("Front9Scores") = scores
                pd("Back9Scores") = New Integer(9) {}
                pd("Front9Par") = New Integer(9) {}
                pd("Back9Par") = New Integer(9) {}
                pd("Front9Hcp") = New Integer(9) {}
                pd("Back9Hcp") = New Integer(9) {}
                results.Add(pd)
                LOGIT($"Claude parsed: {playerName} scores: {String.Join(",", scores.Skip(1))}")
            Next

        Catch ex As Exception
            LOGIT($"ParseScorecardViaClaude error: {ex.Message}")
        End Try
        Return results
    End Function
    Public Function GetPlayerGroup(playerName As String, league As String, year As String) As Integer
        Try
            Using cmd As New SQLiteCommand("
            SELECT [Group] FROM Teams 
            WHERE League=@League 
            AND Year=@Year 
            AND Player=@Player", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", league)
                cmd.Parameters.AddWithValue("@Year", year)
                cmd.Parameters.AddWithValue("@Player", playerName)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then Return CInt(result)
            End Using
        Catch ex As Exception
            LOGIT($"GetPlayerGroup error: {ex.Message}")
        End Try
        Return 0 ' 0 means not found / sub
    End Function

    Public Function GetGroupPlayers(groupNum As Integer, league As String, year As String) As List(Of String)
        Dim players As New List(Of String)
        Try
            Using cmd As New SQLiteCommand("
            SELECT Player FROM Teams 
            WHERE League=@League 
            AND Year=@Year 
            AND [Group]=@Group
            ORDER BY Grade", ctx.Conn)
                cmd.Parameters.AddWithValue("@League", league)
                cmd.Parameters.AddWithValue("@Year", year)
                cmd.Parameters.AddWithValue("@Group", groupNum)
                If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                Using dr As SQLiteDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        players.Add(dr("Player").ToString())
                    End While
                End Using
            End Using
        Catch ex As Exception
            LOGIT($"GetGroupPlayers error: {ex.Message}")
        End Try
        Return players
    End Function

    Public Function SuggestPlayerFromGroup(claudeName As String, groupNum As Integer,
        resolvedPlayers As List(Of String), league As String, year As String) As String
        Try
            ' Get all players in this group
            Dim groupPlayers = GetGroupPlayers(groupNum, league, year)

            ' Find group members not already resolved
            Dim candidates = groupPlayers.Where(
                Function(p) Not resolvedPlayers.Any(
                    Function(r) r.ToLower() = p.ToLower())).ToList()

            If candidates.Count = 1 Then
                LOGIT($"Group intelligence: {claudeName} → suggested {candidates(0)}")
                Return candidates(0)
            End If

            ' Try last name match as tiebreaker
            Dim claudeLastName As String = claudeName.Split(" "c).Last().ToLower()
            Dim lastNameMatch = candidates.FirstOrDefault(
                Function(p) p.ToLower().Contains(claudeLastName))
            If lastNameMatch IsNot Nothing Then Return lastNameMatch

        Catch ex As Exception
            LOGIT($"SuggestPlayerFromGroup error: {ex.Message}")
        End Try
        Return ""
    End Function
End Class
