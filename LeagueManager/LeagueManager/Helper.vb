'Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Net.Mail
'Imports System.Web
Public Class WA
    Dim fromsizeW As Integer, lvsizeW As Integer

    Public dsLeague As DataSet
    Public sFilePath As String
    Public sLeagueName As String
    Public sGroupNumber As Integer
    Public sFrontBack As String
    Public dDate As Date
    Public iHoles As Integer
    Public iHoleMarker As Integer
    Public iHdcp As Integer
    Public sCourse As String
    Public sTeam As String
    Public sPlayer As String
    Public sPlayerToFind As String
    Public sFindPlayerOption As String
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
    Public dt As DataTable

    'fields with (Number) are key fields
    'field-width-read only-tabstop-MiddleRight
    Public Const cPat40 = "40-false-true-mr"
    Public Const cPat40nt = "40-true-false-mr"
    Public Const cPathole = "40-false-true-ml"
    Public Const cPat60 = "60-false-true-mc"
    Public Const cPatMeth = "80-false-true-mc"
    Public Const cPat120 = "120-false-true-ml"
    Public Const cPat170 = "170-false-false-ml"
    Public Const cPat170nt = "170-true-false-ml"
    Public Const cBaseScoreCard As String = "Player(1)-cPat170,Method-cPatMeth,Team,Group(3),Holes,Hdcp-cPat40nt"
    '20180224-change to read only
    'Public Const cSkinsFields As String = "Skins-cPat40,Closest-cPat60,$Earn-cPat40nt,$Skins-cPat40nt,$Closest-cPat40nt,#Skins-cPat40nt,#Closests-cPat60"
    Public Const cSkinsFields As String = "Skins-cPat40nt,Closest-cPat60nt,$Earn-cPat40nt,$Skins-cPat40nt,$Closest-cPat40nt,#Skins-cPat40nt,#Closests-cPat40nt"
    Public MyCourse() As Data.DataRow
    Public bAllHolesEntered = False
    Public sArrayOfFiles As New List(Of String)
    Public rLeagueParmrow As DataRowView
    Public bsch = False
    Public bscores = False
    Public bplayer = False
    Public bcourses = False
    Public GGmail As GGSMTP_GMAIL
    Public sFileInUseMessage As String
    ' Create the ToolTip and associate with the Form container.
    Public toolTipHdcp As New ToolTip()
    '20180130-num of Closests
    Public iNumClosests = 0
    Public bByeFound = False
End Class
'Namespace Helper
Public Class Helper
        'Dim lvwColumnSorter As ListViewColumnSorter
        Dim fromsizeW As Integer, lvsizeW As Integer

        Public dsLeague As DataSet
        Public sFilePath As String
        Public sLeagueName As String
        Public sGroupNumber As Integer
        Public sFrontBack As String
        Public dDate As Date
        Public iHoles As Integer
        Public iHoleMarker As Integer
        Public iHdcp As Integer
        Public sCourse As String
        Public sTeam As String
        Public sPlayer As String
        Public sPlayerToFind As String
        Public sFindPlayerOption As String
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
        Public dt As DataTable

        'fields with (Number) are key fields
        'field-width-read only-tabstop-MiddleRight
        Public Const cPat40 = "40-false-true-mr"
        Public Const cPat40nt = "40-true-false-mr"
        Public Const cPathole = "40-false-true-ml"
        Public Const cPat60 = "60-false-true-mc"
        Public Const cPatMeth = "80-false-true-mc"
        Public Const cPat120 = "120-false-true-ml"
        Public Const cPat170 = "170-false-false-ml"
        Public Const cPat170nt = "170-true-false-ml"
        Public Const cBaseScoreCard As String = "Player(1)-cPat170,Method-cPatMeth,Team,Group(3),Holes,Hdcp-cPat40nt"
        '20180224-change to read only
        'Public Const cSkinsFields As String = "Skins-cPat40,Closest-cPat60,$Earn-cPat40nt,$Skins-cPat40nt,$Closest-cPat40nt,#Skins-cPat40nt,#Closests-cPat60"
        Public Const cSkinsFields As String = "Skins-cPat40nt,Closest-cPat60nt,$Earn-cPat40nt,$Skins-cPat40nt,$Closest-cPat40nt,#Skins-cPat40nt,#Closests-cPat40nt"
        Public MyCourse() As Data.DataRow
        Public bAllHolesEntered = False
        Public sArrayOfFiles As New List(Of String)
        Public rLeagueParmrow As DataRowView
        Public bsch = False
        Public bscores = False
        Public bplayer = False
        Public bcourses = False
        Public GGmail As GGSMTP_GMAIL
        Public sFileInUseMessage As String
        ' Create the ToolTip and associate with the Form container.
        Public toolTipHdcp As New ToolTip()
        '20180130-num of Closests
        Public iNumClosests = 0
        Public bByeFound = False

        Public Function RemoveSpcChar(ByVal chr As String) As String
            RemoveSpcChar = chr.ToString.Replace(ChrW(&H25CF), String.Empty)
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
                End Using
                UpdateINI = True
            Catch ex As Exception
                UpdateINI = False
            End Try
        End Function
        Public Sub Common_Exit()
            bexit = True
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            Try
                If UpdateINI() = False Then
                    Throw New Exception("Error Updating " & sFilePath & "\Leaguemanager.ini")
                End If

            Catch ex As Exception
                MsgBox("Close the file " & (sFilePath & "\Leaguemanager.ini" & vbCrLf & "Try again"))
            End Try

        End Sub

        Public Sub GMmailIT()
            Try
                Dim SmtpServer As New SmtpClient()
                Dim mail As New MailMessage()
                SmtpServer.Credentials = New _
    Net.NetworkCredential("garyrscudder@gmail.com", "4St-SCQ-Jt6-tB6")
                SmtpServer.Port = 587
                SmtpServer.Host = "smtp.gmail.com"
                SmtpServer.EnableSsl = True
                mail = New MailMessage()
                mail.From = New MailAddress("garyrscudder@gmail.com")
                mail.To.Add("gary_scudder@yahoo.com")
                mail.Subject = "Test Mail"
                mail.Body = "This is for testing SMTP mail from GMAIL"
                'mail.Attachments()
                SmtpServer.Send(mail)
                MsgBox("mail send")

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub

        ' Save the input DataTable to a CSV file. By default the values are Tab 
        ' delimited, but you can use the second overload version to use any other 
        ' string you want.
        '
        ' Example:
        ' Dim ds As New DataSet
        ' SqlDataAdapter1.Fill(ds, "Users")
        ' DataTable2CSV(ds.Tables("Users"), "D:\Users.txt")
        Public Sub DataTable2CSV(ByVal table As DataTable, ByVal filename As String)
            '6/2/2017 replace vbtab with , because vbtab quit working
            DataTable2CSV(table, filename, ",")
        End Sub
        'NOTE:  This function requires Microsoft Office to be installed on the computer using it and it has to have the correct install of 32bit or 64bit which must match the operating system of windows
        Public Function CSV2DataTableOLEDB(ByVal filename As String) As DataTable
            CSV2DataTableOLEDB = Nothing
            Try
                'Provider = Microsoft.ACE.OLEDB.12.0;Data Source=c:\myFolder\myExcel2007file.xlsx;
                'builder.add("Extended Properties = "Excel 12.0 Xml;HDR=YES";
                '.Provider = "Microsoft.Jet.OLEDB.4.0",
                'DataSource = sFilePath 'Application.StartupPath & IO.Path.DirectorySeparatorChar

                Dim Builder As New OleDbConnectionStringBuilder With
                {
                    .Provider = "Microsoft.ACE.OLEDB.12.0",
                    .DataSource = sFilePath 'Application.StartupPath & IO.Path.DirectorySeparatorChar
                }

                'Builder.Add("Extended Properties", "text;HDR=Yes;FMT=Delimited(,);readonly=False")
                Builder.Add("Extended Properties", "Excel 12.0 Xml;HDR=YES")
                'builder.add("

                Using cn As New OleDbConnection With
            {
                .ConnectionString = Builder.ConnectionString
            }

                    Using cmd As New OleDbCommand With
                {
                    .Connection = cn,
                    .CommandText =
                    <SQL>
                        SELECT * 
                        FROM <%= sFilePath & "\" & filename %>
                    </SQL>.Value
                }

                        Dim dt As New DataTable

                        cn.Open()

                        dt.Load(cmd.ExecuteReader)
                        CSV2DataTableOLEDB = dt

                    End Using

                End Using
            Catch ex As Exception
                Throw (New Exception(ex.Message))
            End Try
        End Function

        Public Sub CreateRowfromLine(sAry As String(), edt As DataTable)
            Dim aRow As DataRow
            aRow = edt.NewRow
            ' now create an empty datarow
            aRow = edt.NewRow
            'i is the index into the string array
            Dim i = 0
            'loop thru each column in the data table determining if numeric values are present for numeric fields
            For Each col As DataColumn In edt.Columns
                If i > UBound(sAry) Then
                    aRow(col) = ""
                Else
                    Dim scolName = col.ColumnName
                    Select Case col.DataType
                        Case GetType(Int32)
                            If IsNumeric(sAry(i)) Then
                                aRow(col) = sAry(i)
                            Else
                                aRow(col) = 0
                            End If
                        Case Else
                            aRow(col) = sAry(i)
                    End Select
                    i += 1
                End If
            Next
            'add the full row to the table
            edt.Rows.Add(aRow)
        End Sub
        Public Function CSV2DataTableForScoreCard(ByVal strFileName As String, sDate As String, sScoreCard As Integer) As DataTable
            Dim myStream As System.IO.StreamReader = New System.IO.StreamReader(strFileName)
            Dim line As String, dlinecnt As Double
            Dim aRow As DataRow
            CSV2DataTableForScoreCard = New DataTable
            Try
                Do
                    'read a line from the csv
                    line = myStream.ReadLine()
                    dlinecnt += 1
                    If line Is Nothing Then
                        Exit Do
                    End If
                    'build a string array of scores using comma delimited
                    Dim sAry As String() = Split(line, ",")
                    'if this is the first line, it is a header so save each column header and mark the numeric ones 

                    If dlinecnt = 1 Then
                        CSV2DataTableForScoreCard.Columns.Add("Player", GetType(String))
                        For hole = 1 To 9
                            CSV2DataTableForScoreCard.Columns.Add(hole, GetType(String))
                        Next
                        CSV2DataTableForScoreCard.Columns.Add("Out", GetType(String))
                        For hole = 10 To 18
                            CSV2DataTableForScoreCard.Columns.Add(hole, GetType(String))
                        Next
                        CSV2DataTableForScoreCard.Columns.Add("In", GetType(String))
                        CSV2DataTableForScoreCard.Columns.Add("Total", GetType(String))
                        Continue Do
                    End If
                    If sAry(1) <> sDate Then
                        Continue Do
                    End If
                    If sAry(33) <> sScoreCard Then
                        Continue Do
                    End If    ' now create an empty datarow
                    aRow = CSV2DataTableForScoreCard.NewRow
                    'loop thru each column in the data table determining if numeric values are present for numeric fields
                    aRow("Player") = sAry(0)
                    For hole = 1 To 9
                        If IsNumeric(sAry(hole + 4)) Then
                            aRow(hole) = (sAry(hole + 4))
                        Else
                            aRow(hole) = 0
                        End If
                    Next
                    aRow("Out") = sAry(23)
                    For hole = 10 To 18
                        If IsNumeric(sAry(hole + 4)) Then
                            aRow(hole + 1) = (sAry(hole + 4))
                        Else
                            aRow(hole + 1) = 0
                        End If
                    Next
                    aRow("In") = sAry(24)
                    aRow("Total") = sAry(25)
                    'add the full row to the table
                    CSV2DataTableForScoreCard.Rows.Add(aRow)
                Loop
                myStream.Close()
            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Function
        Public Function CSV2DataTable(ByVal dt As DataTable, strFileName As String) As Boolean

            CSV2DataTable = False
            Dim line As String, dlinecnt As Double
            Dim aRow As DataRow

            Try
                Dim myStream As System.IO.StreamReader = New System.IO.StreamReader(strFileName)
                Do

                    'read a line from the csv
                    line = myStream.ReadLine()
                    If line = "" Or line Is Nothing Then Exit Do
                    If line.Contains("20170411") Then
                        Console.WriteLine(line)
                    End If
                    dlinecnt += 1
                    If line Is Nothing Then
                        Exit Do
                    End If
                    'build a string array of scores using comma delimited
                    Dim sAry As String() = Split(line, ",")
                    If sAry(0).ToString = "" Then Continue Do
                    'if this is the first line, it is a header so save each column header and mark the numeric ones 

                    If dlinecnt = 1 Then
                        If dt.Columns.Count = 0 Then
                            For i = 0 To sAry.Count - 1
                                dt.Columns.Add(sAry(i))
                            Next
                        End If
                        Continue Do
                    End If

                    aRow = dt.NewRow
                    For i = 0 To sAry.Count - 1
                        If sAry(i) <> "" Then
                            aRow(i) = sAry(i)
                        End If
                    Next

                    dt.Rows.Add(aRow)
                Loop
                myStream.Close()
                CSV2DataTable = True
            Catch ex As Exception
                sFileInUseMessage = ex.Message
                If ex.Message.Contains("being used by another process") Then
                    ' MsgBox(String.Format("file {0} in use, try later", strFileName))
                Else
                    MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
                End If

            Finally

            End Try

        End Function
        Public Sub DataTable2XML(ByVal table As String, ByVal filename As String)
            'dtLeagueParms = CSV2DataTable(sLeagueName & ".csv")
            'dsLeague.Tables.Add(dtLeagueParms)
            'dsLeague.Tables(0).TableName = table
            'dsLeague.Tables(table).WriteXml(sFilePath & "\" & filename & ".xml", XmlWriteMode.WriteSchema)
            'writes all tables to xml
            For i = 0 To dsLeague.Tables.Count - 1
                If dsLeague.Tables(i).TableName = table Then
                    dsLeague.Tables(i).WriteXml(sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_") & filename & ".xml", XmlWriteMode.WriteSchema)
                End If
            Next
        End Sub
        Public Sub DataTable2CSV(ByVal table As DataTable, ByVal filename As String,
  ByVal sepChar As String)
            Try

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
        Function convDBNulltoSpaces(ByVal sfield) As String
            If IsDBNull(sfield) Then
                convDBNulltoSpaces = " "
            ElseIf sfield Is Nothing Then
                convDBNulltoSpaces = " "
            Else
                convDBNulltoSpaces = sfield
            End If
        End Function
        Function GetNewHdcp(row As DataGridViewRow, sDate As String) As String
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

            'this subroutine assumes dvscores is only one players scores and is sorted by date low to high
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
            '2-Update handicap in dvscores for the round were processing
            '
            GetNewHdcp = ""
            Dim iLast5Scores As New List(Of Decimal)
            Dim iRoundctr = 0
            Dim iPHdcp = 0
            Try
                Dim dvScores As New DataView(dsLeague.Tables("dtScores"))
                dvScores.RowFilter = "Player = '" & row.Cells("Player").Value & "'" & " and Date < '" & sDate & "'"
                dvScores.Sort = "Date desc"
                'this compensates for lost scores after week 1 in 2017, i have hardcoded the prev handicap on 4/11 so we dont have to go back
                'If sDate > "20170411" Then
                'dvScores.RowFilter = dvScores.RowFilter & " and Date >= '20170411'"
                'End If
                'build a total of 5 max scores in our array
                iRoundctr = 0
                For Each score As DataRowView In dvScores
                    '20171009- bypass scores when a noshow
                    'If score("Method") = "Score" And score("Group") = 0 Then
                    '    Continue For
                    'End If
                    iRoundctr += 1
                    sPlayer = score("Player").ToString
                    iPHdcp = score("PHdcp").ToString
                    Dim sScoreDate As String = score("Date")
                    CalcHoleMarker(sScoreDate)
                    '20180120 - fix this later, 
                    'If sScoreDate.ToString.Substring(0, 4) < sDate.Substring(0, 4) Then
                    '    If score("Out_Net") Is DBNull.Value Then
                    '        iHoleMarker = 10
                    '    End If
                    'End If
                    If score("Method").ToString = "Net" Then
                        If iHoles = 9 Then
                            If iHoleMarker = 10 Then
                                iLast5Scores.Add(score("In_Net").ToString + iPHdcp)
                            Else
                                iLast5Scores.Add(score("Out_Net").ToString + iPHdcp)
                            End If
                        Else
                            iLast5Scores.Add(score("18_Net").ToString + iPHdcp)
                        End If
                    ElseIf score("Method").ToString = "Gross" Then
                        If iHoles = 9 Then
                            If iHoleMarker = 10 Then
                                iLast5Scores.Add(score("In_Gross").ToString)
                            Else
                                iLast5Scores.Add(score("Out_Gross").ToString)
                            End If
                        Else
                            iLast5Scores.Add(score("18_Gross").ToString)
                        End If
                        'score always uses 9 holes and front 9 gross
                    ElseIf score("Method").ToString = "Score" Then
                        If iHoles = 9 Then
                            If iHoleMarker = 1 Then
                                iLast5Scores.Add(score("Out_Gross").ToString)
                            Else
                                iLast5Scores.Add(score("In_Gross").ToString)
                            End If
                        Else
                            iLast5Scores.Add(score("18_Gross").ToString)
                        End If

                    End If

                    If iRoundctr = 4 Then Exit For
                    'dont recalculate handicap for 4/11, scorebook was lost
                    'If sScoreDate = "20170411" Then
                    '    iPHdcp = GetHdcp(iLast5Scores, iRoundctr, score("Date").ToString)
                    'End If
                Next
                'calc using screen score

                Dim sScore = ""
                CalcHoleMarker(sDate)
                If iHoleMarker = 1 Then
                    sScore = row.Cells("Out_Gross").Value.ToString()
                Else
                    sScore = row.Cells("In_Gross").Value.ToString()
                End If
                sPlayer = row.Cells("Player").Value.ToString
                'if the players score is blank, use the latest handicap
                If sScore <> "" Then
                    iLast5Scores.Add(sScore)
                    iRoundctr += 1
                    iHdcp = GetHdcp(iLast5Scores, iRoundctr, sDate)
                Else
                    iHdcp = iPHdcp
                End If

                For Each score In iLast5Scores
                    row.Cells("Hdcp").ToolTipText = row.Cells("Hdcp").ToolTipText & score & "-"
                Next
                row.Cells("Hdcp").ToolTipText = row.Cells("Hdcp").ToolTipText.Trim("-")

                Return iHdcp

            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Function
        Function GetHdcp(ByRef ilast5Scores As List(Of Decimal), iRoundctr As String, sDate As String) As String
            Try
                Dim iPlayerHdcp = 0
                Dim iPhdcp = 0
                Dim ilowscore = 999
                Dim ihiscore = 0
                Dim iApr11Hdcp = 99
                If iRoundctr > 5 Then
                    ilast5Scores = ilast5Scores.Take(0).Concat(ilast5Scores.Skip(1)).ToList
                    ihiscore = 0
                    ilowscore = 999
                End If
                'calculate hi and low for drop
                For Each iscore As String In ilast5Scores
                    'is this the low score?
                    If iscore < ilowscore Then
                        ilowscore = iscore
                    End If
                    If iscore > ihiscore Then
                        ihiscore = iscore
                    End If
                Next

                Dim itotScores = 0
                Dim blowdropped = False
                Dim bhidropped = False
                Dim iPar As Integer = 0
                Dim iKeptScores As New List(Of Decimal)
                'drop the high and low scores
                For Each iscore As Integer In ilast5Scores
                    If ilast5Scores.Count = 5 Then
                        If ilowscore <> 999 Then
                            If iscore = ilowscore Then
                                If Not blowdropped Then
                                    blowdropped = True
                                    Continue For
                                End If
                            End If
                        End If
                    End If
                    If ilast5Scores.Count >= 4 Then
                        If ihiscore > 0 Then
                            If iscore = ihiscore Then
                                If Not bhidropped Then
                                    bhidropped = True
                                    Continue For
                                End If
                            End If
                        End If
                    End If
                    iKeptScores.Add(iscore)
                Next

                'loop through to get the 3 most recent scores
                Dim ictr As Integer = 0
                Dim sbScoresKept As New StringBuilder
                For Each iscore As Integer In iKeptScores
                    sbScoresKept.Append(iscore & "-")
                    itotScores += iscore
                    ictr += 1
                    'calc course par
                    Dim MyCourse() As Data.DataRow
                    Dim scourse = rLeagueParmrow("Course")
                    iHoles = rLeagueParmrow("Holes")
                    MyCourse = dsLeague.Tables("dtCourses").Select("Name = '" & scourse & "'")
                    Dim iCoursePar = 0
                    'accumulate par for each score
                    For i As Integer = 1 To iHoles
                        iCoursePar += MyCourse(0)("Hole" & i).ToString
                    Next
                    iPar += iCoursePar
                Next

                'this compensates for lost scores of week 1 in 2017
                If ictr = 1 And iApr11Hdcp <> 99 Then
                    iPlayerHdcp = iApr11Hdcp
                    iPhdcp = iPlayerHdcp
                Else
                    iPlayerHdcp = Math.Round((itotScores - iPar) / ictr * 0.8)
                End If
                'If ictr = 3 Or iRoundctr = ictr Then
                '    'Dim myRow() As Data.DataRow
                '    'myRow = dtScores.Select("Player = '" & sPlayer & "' and Date = '" & sDate & "'")
                '    'myRow(0)("PHdcp") = iPhdcp
                '    'myRow(0)("Hdcp") = iPlayerHdcp
                'End If

                LOGIT("Date - " & sDate & " Player - " & sPlayer.PadRight(25) & " Prv Handicap " & iPhdcp.ToString.PadRight(2) & " Handicap " & iPlayerHdcp.ToString.PadRight(2) & " All Scores - (" & String.Join("-", ilast5Scores) & ") Scores Kept (" & String.Join("-", sbScoresKept.ToString) & ") High Score - " & ihiscore & " Low Score - " & ilowscore)

                Return iPlayerHdcp
            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Function
        Public Sub LOGIT(ByVal sMess As String)
            Try
                If bloghelper Then
                    Using swLog As New StreamWriter(sFilePath & "\Logs\" & sLeagueName & "_" & DateTime.Now.ToString("yyyyMMdd_HH") & ".log", True)
                        For Each stmpmess As String In sMess.Split(CChar(vbCrLf))
                            swLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss | ") & stmpmess.Replace(vbCr, "").Replace(vbLf, ""))
                        Next
                        swLog.Close()
                    End Using
                End If
            Catch ex As Exception

            End Try
        End Sub
        Sub CalcHoleMarker(sDate As String)
            Try

                Dim sStartMonth As String = rLeagueParmrow("startDate").ToString.Substring(0, rLeagueParmrow("startDate").ToString.IndexOf("/")).PadLeft(2, "0")
                Dim wkdate As Date = rLeagueParmrow("startDate")
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                If sDate.Substring(0, 4) < reformatted.Substring(0, 4) Then
                    For Each r As DataRow In dsLeague.Tables("dtLeagueParms").Rows
                        Dim wkdateymd As Date = dsLeague.Tables("dtLeagueParms").Rows(0)("StartDate")
                        Dim reformattedymd As String = wkdateymd.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                        If reformattedymd.Substring(0, 4) = sDate.Substring(0, 4) Then
                            sStartMonth = reformattedymd.Substring(4, 2)
                            Exit For
                        End If
                    Next

                End If
                'If sDate < reformatted Then
                '    iHoleMarker = 1
                '    Exit Sub
                'End If

                'Dim iOddEven = (sDate.ToString.Substring(4, 2) - sStartMonth) Mod 2
                Dim sScoreMonth = sDate.ToString.Substring(4, 2)
                'odd holes are on the back if the starting hole is front
                If (sDate.ToString.Substring(4, 2) - sStartMonth) Mod 2 = 0 Then
                    If rLeagueParmrow("Start9") = "F" Then
                        iHoleMarker = 1
                    Else
                        iHoleMarker = 10
                    End If
                Else
                    If rLeagueParmrow("Start9") = "F" Then
                        iHoleMarker = 10
                    Else
                        iHoleMarker = 1
                    End If
                End If

            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End Sub
        Sub DisplayLast5(sDate As String, frm As Form, lv1 As SI.Controls.LvSort)
            LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
            Try
                Dim lvRec As ListViewItem = Nothing

                With lv1
                    .Visible = True
                    'fromsizeW = frm.Size.Width
                    'lvsizeW = .Size.Width
                    .Width = frm.Width
                    .Height = frm.Height
                    .ListViewItemSorter = Nothing
                    .View = View.Details
                    .Visible = True
                    .Items.Clear()
                    .Sorting = SortOrder.None
                    .Columns.Clear()
                    .Columns.Add("Player-Most Recent 1st", 130, HorizontalAlignment.Left)
                    For i = 1 To 5
                        .Columns.Add(i.ToString, 60, HorizontalAlignment.Left)
                    Next
                End With

                Dim dvScores = New DataView(dsLeague.Tables("dtScores"))

                ' dvScores.RowFilter = "Player = '" & sPlayer & "'" & " and Course = '" & dvCourse.Item(0).Row.Item(0) & "' and Hole1 > 0"
                dvScores.Sort = "Player, Date desc"
                dvScores.RowFilter = String.Format("Date <= {0}", sDate)

                Dim icnt = 0
                For Each row As DataRowView In dvScores
                    row("Out_Gross") = convDBNulltoSpaces(row("Out_Gross"))
                    row("In_Gross") = convDBNulltoSpaces(row("In_Gross"))
                    If row("Out_Gross") = " " And row("In_Gross") = " " Then Continue For
                    'Debug.Print(row("Player") & "-" & row("Out_Gross") & "-" & row("In_Gross") & "-" & "icnt-" & icnt)
                    'if 5 scores, load the score record to the list
                    If icnt = 0 Then
                        lvRec = New ListViewItem(row("Player").ToString)
                    ElseIf lvRec.SubItems(0).Text = row("Player") Then
                        If icnt > 5 Then Continue For
                    Else
                        lv1.Items.Add(lvRec)
                        lvRec = New ListViewItem(row("Player").ToString)
                        icnt = 1
                    End If

                    'if first one this player, then lvrec will be empty
                    If lvRec.SubItems(0).Text = row("Player") Then
                        If row("Out_Gross") <> " " Then
                            'Debug.Print("Score added " & row("Out_Gross"))
                            lvRec.SubItems.Add(row("Out_Gross") & "-" & row("Hdcp"))
                        Else
                            'Debug.Print("Score added " & row("In_Gross"))
                            lvRec.SubItems.Add(row("In_Gross") & "-" & row("Hdcp"))
                        End If
                    End If

                    icnt += 1
                Next
                lv1.Items.Add(lvRec)
                frm.Controls.Add(lv1)
            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub
        Sub DisplayScores(sPlayer As String, frm As Form, lv1 As SI.Controls.LvSort)
            LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
            Try
                Dim lvRec As ListViewItem = Nothing

                With lv1
                    .Visible = True
                    fromsizeW = frm.Size.Width
                    lvsizeW = .Size.Width
                    .ListViewItemSorter = Nothing
                    .View = View.Details
                    .Visible = True
                    .Items.Clear()
                    .Sorting = SortOrder.None
                    .Columns.Clear()
                    .Columns.Add("Date", 100, HorizontalAlignment.Left)
                    'lvFiles.Columns.Add("File Type", 80, HorizontalAlignment.Left) 
                    For i = 1 To 18
                        .Columns.Add(i.ToString, 30, HorizontalAlignment.Left)
                        If i = 9 Then
                            .Columns.Add("Out", 40, HorizontalAlignment.Left)
                        End If
                    Next

                    .Columns.Add("In", 40, HorizontalAlignment.Left)
                    .Columns.Add("Tot", 40, HorizontalAlignment.Left)
                    .Columns.Add("Hdcp", 50, HorizontalAlignment.Left)
                    .Columns.Add("Net", 40, HorizontalAlignment.Left)
                End With

                Dim dvScores = New DataView(dsLeague.Tables("dtScores")), sFirstField = "Date"

                ' dvScores.RowFilter = "Player = '" & sPlayer & "'" & " and Course = '" & dvCourse.Item(0).Row.Item(0) & "' and Hole1 > 0"
                dvScores.RowFilter = String.Format("Player = '{0}'", sPlayer) 'and Hole1 > 0"
                dvScores.Sort = "Date"
                'testing 
                'dvScores.RowFilter = "Date > 20130609 and Name = '" & cbPlayer.SelectedItem.ToString & "'"
                Dim iStats As New List(Of Integer)

                'create 5 stats
                For i = 0 To 5
                    iStats.Add(0)
                Next
                'create a bucket for each hole
                Dim iScores As New List(Of Decimal)
                For i = 0 To 17
                    iScores.Add(0)
                Next
                Dim iFrontScores As Integer = 0, iBackScores As Integer = 0

                iHoles = rLeagueParmrow("Holes")
                Dim iTotScore As Decimal, iInScore As Decimal, iOutScore As Decimal

                For Each row As DataRowView In dvScores
                    '20180123 - show scores with scoring method "Score'
                    'If row("Method") = "Score" Then Continue For
                    Console.WriteLine(row("Date"))
                    'figure out the front/back nine score
                    CalcHoleMarker(row("Date"))
                    'MyCourse = dsLeague.Tables("dtCourses").Select(String.Format("Name = '{0}'", rLeagueParmrow("Course")))
                    'Dim x = "Player = '" & sPlayer & "'" & " and Course = '" & MyCourse(0).Item("Name")
                    Dim ohelperlv As New Helper_LV
                    ohelperlv.ListScores(lv1, row, sFirstField, True, 0, iHoleMarker)
                    If row("Method") <> "Score" Then
                        For ihole = 1 To iHoles
                            Dim ihptr = ihole + (iHoleMarker - 1)
                            Dim iadjscore = row("Hole" & ihptr)
                            If row("Date") = "20170919" Then
                                If ihptr = 17 Then
                                    Dim xxx = ""
                                End If
                            End If
                            If row("Method") = "Net" Then
                                Dim ish As Integer = CalcStrokeIndex(ihptr)
                                If row("Phdcp") >= ish Then
                                    iadjscore += 1
                                    If row("Phdcp") - 9 >= ish Then iadjscore += 1
                                End If
                                iScores(ihptr - 1) += iadjscore
                            Else
                                iScores(ihptr - 1) += iadjscore
                            End If
                            Dim icnt = 2
                            Dim spar = MyCourse(0)("Hole" & ihptr)
                            For i = 0 To iStats.Count - 1
                                If iadjscore <= spar - icnt Then
                                    iStats(i) += 1
                                    Exit For
                                End If
                                icnt -= 1
                                If i = iStats.Count - 1 Then
                                    iStats(i) += 1
                                End If
                            Next
                        Next
                        If iHoleMarker = 1 Then
                            iFrontScores += 1
                        Else
                            iBackScores += 1
                        End If
                    End If

                    If row("Out_Gross") <> "" Then iOutScore += row("Out_Gross")
                    If row("In_Gross") <> "" Then iInScore += row("In_Gross")

                    If iHoles > 9 Then
                        iTotScore += row("18_Gross")
                    End If
                Next

                Dim ilcols = lv1.Columns.Count

                lvRec = New ListViewItem("--------------------")
                For i = 0 To lv1.Columns.Count - 1
                    lvRec.SubItems.Add("---------------------------".Substring(0, lv1.Columns(i).Width / 10))
                Next
                lv1.Items.Add(lvRec)

                lvRec = New ListViewItem("Total")
                For i = 1 To 18
                    'Dim spar = dvCourse.Item(0).Row(i + 2)
                    If i < 10 Then
                        iScores(i - 1) = (iScores(i - 1) / iFrontScores)
                    Else
                        iScores(i - 1) = (iScores(i - 1) / iBackScores)
                    End If
                    lvRec.SubItems.Add(iScores(i - 1).ToString("#.0"))
                    If i = 9 Then
                        lvRec.SubItems.Add((iOutScore / iFrontScores).ToString("##.0"))
                    ElseIf i = 18 Then
                        lvRec.SubItems.Add((iInScore / iBackScores).ToString("##.0"))
                        lvRec.SubItems.Add((iTotScore / dvScores.Count).ToString("###.0"))
                    End If
                Next

                lv1.Items.Add(lvRec)
                Dim sStatsDesc As String() = {"Eagles", "Birdies", "Pars", "Bogeys", "Double Bogeys", "Others"}

                For i = 0 To iStats.Count - 1
                    lvRec = New ListViewItem(sStatsDesc(i))
                    lvRec.SubItems.Add(iStats(i))
                    lv1.Items.Add(lvRec)
                Next
                'CalcLowScore(lv1, Color.OrangeRed, False)
                frm.Controls.Add(lv1)

                'lv1.Top = 200
                'lv1.Left = 35
                'lv1.Width = 1100
                'lv1.Height = 350
                'lv1.FullRowSelect = True
                'lv1.GridLines = True
                'lv1.MultiSelect = False
                'lv1.Name = "lv1"
                'lv1.LabelEdit = True
                'AddHandler lv1.SelectedIndexChanged, AddressOf lv1_SelectedIndexChanged
                'AddHandler lv1.MouseDoubleClick, AddressOf lv1_MouseDoubleClick
                'CalcHdcp(lv1, edt, dvScores, dvCourse, "Name|Date")

            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub


        'Public Function dtBuildScores() As DataTable
        '    dtBuildScores = New DataTable
        '    Dim column As DataColumn

        '    column = New DataColumn()
        '    column.DataType = System.Type.GetType("System.String")
        '    column.ColumnName = "Player"
        '    ' Add the column to the DataTable.Columns collection.
        '    dtScores.Columns.Add(column)

        '    column = New DataColumn()
        '    'column.DataType = System.Type.GetType("System.Date")
        '    column.DataType = System.Type.GetType("System.String")
        '    column.ColumnName = "Date"
        '    dtScores.Columns.Add(column)

        '    dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}

        '    dtScores.Columns.Add("Group")

        '    For i As Integer = 1 To iHoles
        '        dtScores.Columns.Add(i)
        '    Next
        '    If iHoles > 9 Then
        '        dtScores.Columns.Add("Out")
        '        dtScores.Columns.Add("In")
        '    End If
        '    dtScores.Columns.Add("Total")
        '    dtScores.Columns.Add("Hdcp")

        '    For i As Integer = 1 To iHoles
        '        dtScores.Columns.Add(i)
        '    Next

        'End Function
        Public Sub BuildControls(gb As GroupBox, top As Integer, left As Integer, holes As Integer, holetitle As String)
            Try
                Dim ileft = left
                Dim itop = top
                Dim iwidth = 35
                'If holes < 18 Then
                '    gb.Width = gb.Width / 2
                'End If
                For i = 0 To holes - 1
                    ileft = CreateLabel(gb.Controls, itop, ileft, iwidth, holetitle & i + 1, i + 1, "B", "")
                    If i = 8 Then
                        'ileft = CreateLabel(gb.Controls, itop, ileft, 40, "Out", "", "B", "")
                        ileft = CreateLabel(gb.Controls, itop, ileft, iwidth, "OutScore", String.Empty, "B", "")
                    ElseIf i = 17 Then
                        ileft = CreateLabel(gb.Controls, itop, ileft, 40, "In", "", "B", "")
                        CreateLabel(gb.Controls, itop + iwidth, ileft - 40, iwidth, "InScore", String.Empty)
                        ileft = CreateLabel(gb.Controls, itop, ileft, 40, "lbTotal", "", "B", "")
                        CreateLabel(gb.Controls, itop + iwidth, ileft - 40, iwidth, "TotScore", String.Empty)
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub

        Public Sub CreateComboBoxField(ByVal dgv As DataGridView, ByVal sField As String, ByVal imax As Integer)

            Dim index As Integer
            ' find the location of the column
            index = dgv.Columns.IndexOf(dgv.Columns(sField))
            ' remove the existing column
            dgv.Columns.RemoveAt(index)
            ' create a new custom column
            Dim dgvCombox As New DataGridViewComboBoxColumn
            dgvCombox.ValueType = GetType(String)
            dgvCombox.Name = sField
            dgvCombox.DataPropertyName = sField
            ' some more tweaking
            dgvCombox.SortMode = DataGridViewColumnSortMode.Automatic

            For i = 0 To imax
                dgvCombox.Items.Add(i.ToString)
            Next
            ' insert the new column at the same location
            dgv.Columns.Insert(index, dgvCombox)
            dgv.Visible = True

        End Sub
        Public Sub CreateMaskField(ByVal dgv As DataGridView, sField As String)

            Dim index As Integer
            ' find the location of the column
            index = dgv.Columns.IndexOf(dgv.Columns(sField))
            ' remove the existing column
            dgv.Columns.RemoveAt(index)
            ' create a new custom column
            Dim dgvMaskedEdit As New DataGridViewMaskedEditColumn

            If sField = "Phone" Then
                dgvMaskedEdit.Mask = "###-###-####"
                dgvMaskedEdit.ValidatingType = GetType(String)
            ElseIf sField.StartsWith("Hole") Then
                dgvMaskedEdit.Mask = "#"      ' this mask will allow only numbers
                dgvMaskedEdit.ValidatingType = GetType(String)
                dgvMaskedEdit.Width = 40
            Else
                dgvMaskedEdit.Mask = "##"      ' this mask will allow only numbers
                dgvMaskedEdit.ValidatingType = GetType(String)
            End If
            dgvMaskedEdit.Name = sField
            dgvMaskedEdit.DataPropertyName = sField
            ' some more tweaking
            dgvMaskedEdit.SortMode = DataGridViewColumnSortMode.Automatic

            ' insert the new column at the same location
            dgv.Columns.Insert(index, dgvMaskedEdit)
            dgv.Visible = True

        End Sub
        Public Function fGetTeam(sNameInfo) As String
            fGetTeam = ""
            Try
                'If dtPlayers Is Nothing Then
                '    If File.Exists(sFilePath & "\Players.csv") Then
                '        dtPlayers = CSV2DataTableOLEDB("\Players.csv")
                '    End If
                '    'Else
                '    '    'go build the player using the player screen
                '    'frmPlayer.Show()
                'End If

                dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {dsLeague.Tables("dtPlayers").Columns("Name")}
                Dim r As DataRow = dsLeague.Tables("dtPlayers").Rows.Find(sNameInfo)
                If r Is Nothing Then
                    Dim sResult = MsgBox("No player found with this " & sNameInfo & vbCrLf & "Press ok to create Player or cancel to quit", MsgBoxStyle.OkCancel)
                    If sResult = MsgBoxResult.Ok Then
                        frmPlayer.ShowDialog()
                        fGetTeam = fGetTeam(sNameInfo)
                        Exit Function
                    Else
                        fGetTeam = ""
                        Exit Function
                    End If
                Else
                    fGetTeam = r("Team")
                End If

            Catch

            End Try
        End Function
        '20171004-eliminate asking for players who are already used
        Public Function fGetPlayer(sNameInfo) As String
            fGetPlayer(sNameInfo, Nothing)
        End Function
        '20171004-eliminate asking for players who are already used
        Public Function fGetPlayer(sNameInfo As String, dgv As DataGridView) As String
            'fGetPlayer = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sInitials)
            fGetPlayer = ""
            Dim aMisPronouncedPlayers() As String = {"Work:Rork", "and:Dan", "ed:ed"}
            Try
                Dim dvPlayers As New DataView(dsLeague.Tables("dtPlayers"))
                Dim sRowFilter As String = ""
                Dim binitials = False
                'search first initial last initial if length 2, else try first name only
                If sNameInfo.Length = 2 Then
                    If sNameInfo.Length = 2 Then binitials = True
                    sRowFilter = "Name like '" & sNameInfo.Substring(0, 1) & "%' and Name like '% " & sNameInfo.Substring(1, 1) & "%'"
                ElseIf sNameInfo.Split(":").Length = 2 Then
                    sRowFilter = "Name like '" & sNameInfo.Split(":")(0) & "%' and Name like '% " & sNameInfo.Split(":")(1) & "%'"
                Else
                    sRowFilter = "Name like '" & sNameInfo & "%'"
                End If
                dvPlayers.RowFilter = sRowFilter
                'if only one player returned, we have it exit
                'If dvPlayers.Count = 1 Then Exit Function
                If dvPlayers.Count = 0 Then
                    sRowFilter = "NickName like '" & sNameInfo & "%'"
                    dvPlayers.RowFilter = sRowFilter
                End If
                'after nickname used, then search mispronounced players 
                If dvPlayers.Count = 0 Then
                    For Each name In aMisPronouncedPlayers
                        If sNameInfo = name.Split(":")(0).ToUpper Then
                            sRowFilter = "Name like '" & name.Split(":")(1) & "%'"
                            dvPlayers.RowFilter = sRowFilter
                            If dvPlayers.Count > 0 Then
                                Exit For
                            End If
                        End If
                    Next
                End If

                'try using the 2 char to lookup name by that 
                If dvPlayers.Count = 0 Then
                    If sNameInfo.Length = 2 Then sRowFilter = "Name like '" & sNameInfo & "%'"
                    dvPlayers.RowFilter = sRowFilter
                End If

                'dvplayers has a players matching initials
                If dvPlayers.Count = 0 Then
                    Dim sResult As MsgBoxResult
                    sResult = MsgBox("Player not found " & sNameInfo & vbCrLf & " Do you want to create a player?", MsgBoxStyle.YesNo)
                    If sResult = MsgBoxResult.Yes Then
                        'frmPlayer.ShowDialog()
                        Dim aRow As DataRow
                        aRow = dsLeague.Tables("dtPlayers").NewRow
                        aRow("Name") = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sNameInfo)
                        dsLeague.Tables("dtPlayers").Rows.Add(aRow)
                        fGetPlayer = aRow("Name")
                        'read the updated csv back in
                        Exit Function
                    Else
                        sResult = MsgBox("do you want to find a player in the player file?", MsgBoxStyle.YesNo)
                        If sResult = MsgBoxResult.Yes Then
                            sFindPlayerOption = "Not Found"
                            sPlayerToFind = sNameInfo
                            frmTeam.ShowDialog()
                            fGetPlayer = sPlayer
                            Exit Function
                        Else
                            fGetPlayer = ""
                            Exit Function
                        End If
                    End If
                End If

                Dim sUniquePlayers As New List(Of String)
                If dvPlayers.Count > 1 Then
                    For Each dvplayer As DataRowView In dvPlayers
                        If frmScoreCard.sOldCellValue = dvplayer("Name") Then Continue For
                        If dgv IsNot Nothing Then
                            Dim bfound = False
                            'check to see if any of these players is already used
                            For Each row As DataGridViewRow In dgv.Rows
                                If row.Cells("Player").Value = dvplayer("Name") Then
                                    bfound = True
                                    Exit For
                                End If
                            Next
                            If Not bfound Then
                                sUniquePlayers.Add(dvplayer("Name"))
                            End If
                        End If
                    Next

                    If sUniquePlayers.Count = 0 Then
                        Dim sResult As MsgBoxResult
                        sResult = MsgBox("no player not found or already used" & vbCrLf & " Do you want to create a player?", MsgBoxStyle.YesNo)
                        If sResult = MsgBoxResult.Yes Then
                            'frmPlayer.ShowDialog()
                            Dim aRow As DataRow
                            aRow = dsLeague.Tables("dtPlayers").NewRow
                            aRow("Name") = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sNameInfo)
                            dsLeague.Tables("dtPlayers").Rows.Add(aRow)

                            fGetPlayer = aRow("Name")
                            'read the updated csv back in
                            Exit Function
                        Else
                            fGetPlayer = sNameInfo
                        End If
                    Else
                        'if only 1 player left, use it
                        '20180301-fix 1 player left issue
                        If sUniquePlayers.Count = 1 Then
                            Dim smsg = String.Format("1 player match found ( {0} ){1} is the player you want{2} press NO to create a new player {3} YES for this one {4} CANCEL to put things back", sUniquePlayers(0), vbCrLf, vbCrLf, vbCrLf, vbCrLf)
                            Dim sResult = MsgBox(smsg, MsgBoxStyle.YesNoCancel)
                            If sResult = MsgBoxResult.Yes Then
                                fGetPlayer = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sUniquePlayers(0))
                                Exit Function
                            ElseIf sResult = MsgBoxResult.Cancel Then
                                Exit Function
                            Else
                                frmPlayer.ShowDialog()
                                'Dim aRow As DataRow
                                'aRow = dsLeague.Tables("dtPlayers").NewRow
                                'aRow("Name") = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sUniquePlayers(0))
                                'dsLeague.Tables("dtPlayers").Rows.Add(aRow)
                                fGetPlayer = "" '  aRow("Name")
                            End If
                        End If
                        For Each sup In sUniquePlayers
                            Dim sMtype = ""

                            'Dim smsg = "There are {0} players {1}, is <{2}> the player you want, press no for more choices, yes for this one or cancel to stop"
                            Dim smsg As String = ""
                            smsg = "There are {0} unused players {1}" & vbCrLf & vbCrLf
                            For Each player In sUniquePlayers
                                smsg = smsg & player & vbCrLf
                            Next
                            smsg = smsg & vbCrLf & "is <{2}> the player you want?" & vbCrLf & vbCrLf & "Press NO for more choices" & vbCrLf & "YES for this one" & vbCrLf & "CANCEL to stop"
                            'set message type to initials or letters if it had a length > 2
                            If binitials Then
                                sMtype = "with these Initials {0}"
                            Else
                                sMtype = "starting with these letters {0}"
                            End If

                            If sNameInfo.Length = 1 Then
                                sMtype = sMtype.Replace("letters", "letter")
                                sMtype = sMtype.Replace("these", "this")
                            End If
                            Dim sResult = MsgBox(String.Format(smsg, sUniquePlayers.Count, String.Format(sMtype, sNameInfo), sup), MsgBoxStyle.YesNoCancel)
                            If sResult = MsgBoxResult.Yes Then
                                fGetPlayer = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sup)
                                Exit Function
                            ElseIf sResult = MsgBoxResult.Cancel Then
                                Exit Function
                            End If
                        Next
                        MsgBox("Go to the player setup screen and setup the new player and re-type this player")
                    End If
                Else
                    fGetPlayer = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dvPlayers(0).Item("Name"))
                End If

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End Function
        Public Function BuildScoreCardMethods(gb As Control) As String
            BuildScoreCardMethods = ""
            For Each ctl In gb.Controls
                Dim x = ""
                If ctl.checked = True Then
                    BuildScoreCardMethods = ctl.text
                    Exit Function
                End If
            Next

        End Function
        Public Function BuildScoreCardMethodsCB() As DataGridViewComboBoxCell
            Dim gridComboBox As New DataGridViewComboBoxCell
            gridComboBox.Items.Add("Net") 'Populate the Combobox
            gridComboBox.Items.Add("Gross") 'Populate the Combobox
            gridComboBox.Items.Add("Score") 'Populate the Combobox

            BuildScoreCardMethodsCB = gridComboBox
        End Function
        Function SetField(sfld As Object, sValue As String)
            SetField = ""
            If sfld Is DBNull.Value Then
                sfld = sValue
            End If
            If sfld = "" Then
                sfld = sValue
            End If
            SetField = sfld
        End Function
        Function GetInt(o As Object) As Integer
            If IsDBNull(o) Then Return 0 Else Return CInt(o)
        End Function
        'Public Function fCreateScoreCardDT() As DataTable
        '    fCreateScoreCardDT = Nothing
        '    fCreateScoreCardDT = New DataTable
        '    Dim sNewScoreCard As String = "League,Method,Team,Player(1),Date(2),Group(3),Hole1,Hole2,Hole3,Hole4,Hole5,Hole6,Hole7,Hole8,Hole9,Out,Out_Gross,Out_Net,Hole10,Hole11,Hole12,Hole13,Hole14,Hole15,Hole16,Hole17,Hole18,In,In_Gross,In_Net,18_Gross,18_Net,Hdcp,PHdcp,Skins,Closest,$Earn,$Skins,$Closest,Partner,Scorecard"
        '    Try
        '        Dim sArray As String() = sNewScoreCard.Split(",")
        '        For Each parm As String In sArray
        '            If parm.Contains("(") Then
        '                parm = parm.Substring(0, parm.IndexOf("("))
        '            End If

        '            Dim column As DataColumn
        '            column = New DataColumn()
        '            column.ColumnName = parm
        '            fCreateScoreCardDT.Columns.Add(column)
        '        Next

        '        Dim PrimarykeyColumn(0) As DataColumn
        '        Dim ikey = 0
        '        For Each parm As String In sArray
        '            If parm.Contains("(") Then
        '                ReDim Preserve PrimarykeyColumn(ikey)
        '                PrimarykeyColumn(ikey) = fCreateScoreCardDT.Columns(parm.Substring(0, parm.IndexOf("(")))
        '                ikey += 1
        '            End If
        '        Next
        '        fCreateScoreCardDT.PrimaryKey = PrimarykeyColumn
        '        'CreateRowfromLine(sLeagueParmValues.Split(vbTab), fCreateScoreCardDT)
        '    Catch ex As Exception
        '        MsgBox(ex.Message)
        '    End Try
        'End Function
        Function Add2DGV(sScoreCardforDGV As String) As String
            Add2DGV = ""
            If iHoles = 9 Then
                If iHoleMarker = 1 Then
                    Add2DGV = sScoreCardforDGV + "Out Gross,Phdcp,Out Net,"
                Else
                    Add2DGV = sScoreCardforDGV + "In Gross,Phdcp,In Net,"
                End If
            Else
                If iHoleMarker = 10 Then
                    Add2DGV = sScoreCardforDGV + "In Gross,In Net," + "18_Gross,PHdcp,18_Net,"
                Else
                    Add2DGV = sScoreCardforDGV + "Out Gross,PHdcp,Out Net,"
                End If
            End If
        End Function
        Function CreateHolesFromParm(sColFormat As List(Of String)) As String
            CreateHolesFromParm = ""

            For i As Integer = iHoleMarker To iHoles + iHoleMarker - 1
                CreateHolesFromParm = CreateHolesFromParm + "Hole" & i & ","
                sColFormat.Add("cPatHole")
                'if its 18 holes
                If iHoles = 18 Then
                    'and were on hole 10
                    If i = 9 Then
                        iHoleMarker = 1
                        CreateHolesFromParm = Add2DGV(CreateHolesFromParm)
                        sColFormat.Add("cPat40nt")
                        sColFormat.Add("cPat40nt")
                    ElseIf i = 18 Then
                        iHoleMarker = 10
                        CreateHolesFromParm = Add2DGV(CreateHolesFromParm)
                        sColFormat.Add("cPat40nt")
                        sColFormat.Add("cPat40nt")
                        sColFormat.Add("cPat40nt")
                        sColFormat.Add("cPat40nt")
                    End If
                Else
                    If i = 9 Or i = 18 Then
                        CreateHolesFromParm = Add2DGV(CreateHolesFromParm)
                        'add formatting for gross/hdcp/net
                        sColFormat.Add("cPat40nt")
                        sColFormat.Add("cPat40nt")
                        sColFormat.Add("cPat40nt")
                    End If
                End If
            Next
        End Function
        'Sub InOutScore(sInOut As String, row As DataGridViewRow, arow As DataRow)
        '    'arow(sInOut) = row.Cells(sInOut).Value
        '    arow(sInOut & "_Gross") = row.Cells(sInOut & "_Gross").Value
        '    arow(sInOut & "_Net") = row.Cells(sInOut & "_Net").Value

        'End Sub
        Public Sub soMarkSubPar(cell As DataGridViewCell, iscore As Integer, iPar As Integer)
            'cell.Style.Font = New Font("Arial", 19, FontStyle.Regular)
            cell.Style.ForeColor = Color.Black
            cell.Style.BackColor = Color.White
            cell.Value = RemoveSpcChar(cell.Value)
            Dim sFont = "Tahoma"
            Dim iFontSize = 12
            Dim bFontStrikeout = False

            If cell.Style.Font IsNot Nothing Then
                If cell.Style.Font.Strikeout = True Then bFontStrikeout = True
            End If

            'cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
            If iHdcp = 99 Then Exit Sub
            'check scores against handicap
            'is this a stroke hole?
            If iHoles = 0 Then
                iHoles = rLeagueParmrow("Holes")
            End If

            'check stroke index
            Dim isi = ""
            isi = CalcStrokeIndex(cell.OwningColumn.Name)
            'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
            'if the handicap > stroke index make color beige
            If iHdcp >= isi Then
                If bColors Then cell.Style.BackColor = Color.Beige
                If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                'if double stroke hole, make color b/a
                If iHdcp - iHoles >= isi Then
                    If bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                    If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                End If
            End If

            If bColors Then
                If iscore < iPar Then
                    iFontSize += 3
                    If bFontStrikeout Then
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout Or FontStyle.Bold)
                    Else
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                    End If
                    cell.Style.ForeColor = Color.OrangeRed
                    'if this is a total score (> 10) of some sort, dont check for birdies, eagles
                    If iscore > 10 Then Exit Sub
                    'birdie
                    If iscore < iPar - 1 Then
                        'cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
                        cell.Style.ForeColor = Color.DarkRed
                    End If
                    'eagle
                    If iscore < iPar - 2 Then
                        iFontSize += 3
                        If bFontStrikeout Then
                            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout Or FontStyle.Bold)
                        Else
                            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                        End If
                    End If
                    'over par is black,even is gray
                ElseIf iscore > iPar Then
                    If bFontStrikeout Then
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
                    Else
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout Or FontStyle.Regular)
                    End If
                    cell.Style.ForeColor = Color.Blue
                End If
            End If
        End Sub
        Sub SetDefaultCellStyles(dgscores As DataGridView)

            dgscores.DefaultCellStyle.BackColor = Color.White
            dgscores.DefaultCellStyle.ForeColor = Color.Black
            dgscores.DefaultCellStyle.Font = New Font("Tahoma", 12)

            Dim highlightCellStyle = New DataGridViewCellStyle()
            highlightCellStyle.BackColor = Color.Red

            Dim currencyCellStyle = New DataGridViewCellStyle()
            currencyCellStyle.Format = "C"
            currencyCellStyle.ForeColor = Color.Green

            'sample to set select cells
            'dgscores.Rows(3).DefaultCellStyle = highlightCellStyle
            'dgscores.Rows(8).DefaultCellStyle = highlightCellStyle
            'dgscores.Columns("UnitPrice").DefaultCellStyle = currencyCellStyle
            'dgscores.Columns("TotalPrice").DefaultCellStyle = currencyCellStyle

        End Sub
        Public Sub SBPMarkSubPar(cell As DataGridViewCell, iscore As Integer, iPar As Integer)
            If iHdcp = 99 Then Exit Sub
            'cell.Style.Font = New Font("Arial", 19, FontStyle.Regular)
            cell.Style.ForeColor = Color.Black
            cell.Style.BackColor = Color.White
            cell.Value = RemoveSpcChar(cell.Value)
            Dim sFont = "Tahoma"
            Dim iFontSize = 12
            Dim bFontStrikeout = False
            If cell.Style.Font IsNot Nothing Then
                If cell.Style.Font.Strikeout = True Then
                    bFontStrikeout = True
                End If
            End If
            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
            'check scores against handicap
            'is this a stroke hole?
            If iHoles = 0 Then iHoles = rLeagueParmrow("Holes")

            'check stroke index
            Dim isi = SBPCalcStrokeIndex(cell.OwningColumn.Name)
            'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
            'if the handicap > stroke index make color beige
            If iHdcp >= isi Then
                If bColors Then cell.Style.BackColor = Color.Beige
                If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                'if double stroke hole, make color b/a
                If iHdcp - iHoles >= isi Then
                    If bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                    If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                End If
            End If

            If bColors Then
                cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
                If iscore < iPar Then
                    iFontSize += 3
                    cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                    cell.Style.ForeColor = Color.OrangeRed
                    'if this is a total score (> 10) of some sort, dont check for birdies, eagles
                    If iscore > 10 Then Exit Sub
                    'birdie
                    If iscore < iPar - 1 Then
                        'cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
                        cell.Style.ForeColor = Color.DarkRed
                    End If
                    'eagle
                    If iscore < iPar - 2 Then
                        iFontSize += 3
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                    End If
                    'over par is black,even is gray
                ElseIf iscore > iPar Then
                    cell.Style.ForeColor = Color.Blue
                End If
            End If
            If bFontStrikeout Then
                cell.Style.Font = New Font(sFont, iFontSize, cell.Style.Font.Style Or FontStyle.Strikeout)
                Debug.Print(sPlayer & " hole " & cell.OwningColumn.Name & " has s/o on")
            End If

        End Sub

        Public Sub MarkSubPar(cell As DataGridViewCell, iscore As Integer, iPar As Integer)
            If iHdcp = 99 Then Exit Sub
            'cell.Style.Font = New Font("Arial", 19, FontStyle.Regular)
            cell.Style.ForeColor = Color.Black
            cell.Style.BackColor = Color.White
            cell.Value = RemoveSpcChar(cell.Value)
            Dim sFont = "Tahoma"
            Dim iFontSize = 12
            Dim bFontStrikeout = False
            If cell.Style.Font IsNot Nothing Then
                If cell.Style.Font.Strikeout = True Then
                    bFontStrikeout = True
                End If
            End If
            cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
            'check scores against handicap
            'is this a stroke hole?
            If iHoles = 0 Then iHoles = rLeagueParmrow("Holes")

            'check stroke index
            Dim isi = CalcStrokeIndex(cell.OwningColumn.Name)
            'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
            'if the handicap > stroke index make color beige
            If iHdcp >= isi Then
                If bColors Then cell.Style.BackColor = Color.Beige
                If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                'if double stroke hole, make color b/a
                If iHdcp - iHoles >= isi Then
                    If bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                    If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                End If
            End If

            If bColors Then
                cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
                If iscore < iPar Then
                    iFontSize += 3
                    cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                    cell.Style.ForeColor = Color.OrangeRed
                    'if this is a total score (> 10) of some sort, dont check for birdies, eagles
                    If iscore > 10 Then Exit Sub
                    'birdie
                    If iscore < iPar - 1 Then
                        'cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
                        cell.Style.ForeColor = Color.DarkRed
                    End If
                    'eagle
                    If iscore < iPar - 2 Then
                        iFontSize += 3
                        cell.Style.Font = New Font(sFont, iFontSize, FontStyle.Bold)
                    End If
                    'over par is black,even is gray
                ElseIf iscore > iPar Then
                    cell.Style.ForeColor = Color.Blue
                End If
            End If
            If bFontStrikeout Then
                cell.Style.Font = New Font(sFont, iFontSize, cell.Style.Font.Style Or FontStyle.Strikeout)
                Debug.Print(sPlayer & " hole " & cell.OwningColumn.Name & " has s/o on")
            End If

        End Sub
        Public Function GetExceptionInfo(ex As Exception) As String
            Dim Result As String
            Dim hr As Integer = Runtime.InteropServices.Marshal.GetHRForException(ex)
            Result = ex.GetType.ToString & "(0x" & hr.ToString("X8") & "): " & ex.Message & Environment.NewLine & ex.StackTrace & Environment.NewLine
            Dim st As StackTrace = New StackTrace(ex, True)
            For Each sf As StackFrame In st.GetFrames
                If sf.GetFileLineNumber() > 0 Then
                    Result &= "Line:" & sf.GetFileLineNumber() & " Filename: " & IO.Path.GetFileName(sf.GetFileName) & Environment.NewLine
                End If
            Next
            Return Result
        End Function

        Sub getMatchPts(dg As DataGridView, index As Integer)
            Dim ipNet = 0
            Dim ioNet = 0
            '20180325
            Dim s9Played As String = "Out_Net"
            If iHoleMarker <> 1 Then s9Played = "In_Net"
            ipNet = FixNullScore(dg.Rows(index + 0).Cells(s9Played).Value.ToString)
            '20180325-bye opponent
            If Matches.sByeOpponent = dg.Rows(index + 0).Cells("Team").Value Then
                dg.Rows(index + 0).Cells("Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 0).Cells("Points").Value = 1
                dg.Rows(index + 0).Cells("Opponent").Value = "Bye"
                bByeFound = True
                Exit Sub
            End If
            sPlayer = dg.Rows(index).Cells("Player").Value
            ioNet = FixNullScore(dg.Rows(index + 2).Cells(s9Played).Value.ToString)
            If ipNet > ioNet Then
                dg.Rows(index + 2).Cells("Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 2).Cells("Points").Value = 1
                dg.Rows(index + 0).Cells("Points").Value = 0
                dg.Rows(index + 0).Cells("Opponent").Style.BackColor = Color.LightGreen
            ElseIf ipNet < ioNet Then
                dg.Rows(index + 0).Cells("Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 0).Cells("Points").Value = 1
                dg.Rows(index + 2).Cells("Points").Value = 0
                dg.Rows(index + 2).Cells("Opponent").Style.BackColor = Color.LightGreen
            Else
                dg.Rows(index + 0).Cells("Points").Style.BackColor = Color.Yellow
                dg.Rows(index + 2).Cells("Points").Style.BackColor = Color.Yellow
                dg.Rows(index + 0).Cells("Points").Value = 0.5
                dg.Rows(index + 2).Cells("Points").Value = 0.5
                dg.Rows(index + 0).Cells("Opponent").Style.BackColor = Color.Yellow
                dg.Rows(index + 2).Cells("Opponent").Style.BackColor = Color.Yellow
            End If
            dg.Rows(index + 0).Cells("Opponent").Value = dg.Rows(index + 2).Cells("Player").Value
            dg.Rows(index + 2).Cells("Opponent").Value = dg.Rows(index + 0).Cells("Player").Value
        End Sub
        Function FixNullScore(iNet As String) As Int16
            FixNullScore = 999
            If iNet <> "" Then FixNullScore = iNet
            'Catch ex As Exception
            '    FixNullScore = 999
            'End Try

        End Function

        Function buildSchedule() As DataTable

            Try
                'Build the column header 
                Dim dtSchedule = New DataTable
                dtSchedule.Columns.Add("Date")
                '20180301
                Dim dvLeague = New DataView(dsLeague.Tables("dtLeagueParms"))
                Dim sYear = "01-01-" & Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4)
                Dim xsYear = Convert.ToDateTime(sYear).ToString("MM/dd/yyyy")
                dvLeague.RowFilter = "StartDate > '" & xsYear & "'"
                Dim iMatches As Integer = dvLeague.Item(0)("Teams") / 2
                For iMatchNum As Integer = 1 To iMatches
                    dtSchedule.Columns.Add(iMatchNum)
                Next
                'input row column
                Dim icolCounter = 0
                'Dim dtsch As DataTable
                'dtsch = CSV2DataTable(sLeagueName & "_sch.csv")
                ''get each match from the column
                For Each col As DataColumn In dsLeague.Tables("dtSchedule").Columns
                    Dim aRow As DataRow
                    aRow = dtSchedule.NewRow
                    'set the match number
                    Dim iMatch = 1
                    'Loop thru each row and pull the match for that day indexed by the column counter
                    For Each irow As DataRow In dsLeague.Tables("dtSchedule").Rows
                        'if were past the row for max matches exit
                        If iMatch > iMatches Then
                            Exit For
                        End If
                        'this is for a holiday check 
                        If Not IsDBNull(irow(icolCounter)) Then
                            If irow(icolCounter).contains("v") Then
                                aRow(iMatch) = irow(icolCounter)
                            Else
                                aRow(iMatch) = "No Match"
                            End If
                        End If
                        iMatch += 1
                    Next
                    If aRow(1) IsNot DBNull.Value Then
                        'save the date 
                        aRow("Date") = col 'DateTime.ParseExact(col.ToString, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy")
                        icolCounter += 1
                        dtSchedule.Rows.Add(aRow)
                    End If
                Next
                dtSchedule.PrimaryKey = New DataColumn() {dtSchedule.Columns(0)}
                buildSchedule = dtSchedule
            Catch ex As Exception

            End Try
        End Function
        Function SBPCalcStrokeIndex(sHole As String) As String
            Try
                'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
                'check stroke index
                SBPCalcStrokeIndex = 0
                Dim sHoleDesc As String = ""
                sHoleDesc = sHole.Replace("Hole", "")
                sHoleDesc = "H" & sHoleDesc
                Dim isi = MyCourse(0).Item(sHoleDesc)
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
        Function CalcStrokeIndex(sHole As String) As String
            Try
                'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
                'check stroke index
                CalcStrokeIndex = 0
                Dim sHoleDesc As String = ""
                sHoleDesc = sHole.Replace("Hole", "")
                If bScoresbyPlayer Then
                    If iHoleMarker = 10 Then sHoleDesc += 9
                End If
                sHoleDesc = "H" & sHoleDesc
                Dim isi = MyCourse(0).Item(sHoleDesc)
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
        Public Sub ValidateCell(ByVal R As DataGridViewCell)
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            Try
                'figure out hole by hole
                Dim sColName = R.OwningColumn.Name
                If sColName = "Player" Then
                    Dim MyPlayer() As Data.DataRow
                    MyPlayer = dsLeague.Tables("dtPlayers").Select("Name = '" & sPlayer & "'")
                    R.Style.BackColor = Color.White
                    'if no team, they are a sub
                    If MyPlayer("Team") Is DBNull.Value Then
                        R.Style.BackColor = Color.Aqua
                    End If
                    'lets mark sub pars
                ElseIf sColName.Contains("Hole") Then
                    Dim iScore = RemoveSpcChar(convDBNulltoSpaces(R.Value))
                    If iScore <> "" Then
                        Dim sCorScore = ChkForMax(CInt(iScore), sColName)
                        If sCorScore <> "" Then
                            R.Value = sCorScore
                        End If
                        MarkSubPar(R, iScore, MyCourse(0)(sColName).ToString)
                    End If
                End If

            Catch ex As Exception
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End Try

        End Sub
        Sub colorScores(r As DataGridViewRow, col As String, compval As String)
            colorScores(r, col, compval, False)
        End Sub

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
                    r.Cells(col).Style.BackColor = Color.OrangeRed
                Else
                    r.Cells(col).Style.BackColor = Color.White
                End If
            End If
        End Sub
        Public Sub ChangeColorsForStrokes(ByVal R As DataGridViewRow)
            'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            'this sub will call marksubpar routine to color the cell for birdies and eagles
            Try
                'keep new rows from being evaluated
                If R.IsNewRow Then Exit Sub
                '20180307-evaluate this statement, should this be here
                If R.Cells.Item("pHdcp").Value.ToString = "" Then Exit Sub

                iHdcp = R.Cells.Item("pHdcp").Value.ToString
                If iHoles > 9 Then iHdcp *= 2
                'figure out hole by hole
                'if holes 1-18 all zeros, then we used a "Score" method and no hole by hole can be done
                'if holes 1-9 are zero but holes 10-18 are populated, we have a back 9 only score
                'if holes 10-18 are zero but holes 1-9 are populated, we have a frnt 9 only score
                If Not bCalcSkins Then
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

                If bScoresbyPlayer Then
                    colorScores(R, "Gross", "Out")
                    colorScores(R, "Net", "Out")
                Else
                    colorScores(R, "Gross", "", True)
                    colorScores(R, "Net", "", True)
                End If

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
                        If sTeam = "" Then If sTeam <> R.Cells("Team").Value Then R.Cells(sColName).Style.BackColor = Color.Aqua
                    ElseIf sColName.Contains("Hole") Then
                        If cell.Value IsNot Nothing Then
                            Try
                                Dim iScore As String = RemoveSpcChar(convDBNulltoSpaces(cell.Value).Trim)
                                If iScore <> "0" Then MarkSubPar(cell, iScore, MyCourse(0)(sColName).ToString)
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
        Public Sub SBPChangeColorsForStrokes(ByVal R As DataGridViewRow)
            'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            'this sub will call marksubpar routine to color the cell for birdies and eagles
            Try
                'keep new rows from being evaluated
                '20180307-evaluate this statement, should this be here
                If R.Cells.Item("pHdcp").Value.ToString = "" Then Exit Sub

                iHdcp = R.Cells.Item("pHdcp").Value.ToString
                'this takes a 9 hole handicap and makes it 18 hole handicap
                If iHoles > 9 Then iHdcp *= 2
                'figure out hole by hole
                'if holes 1-18 all zeros, then we used a "Score" method and no hole by hole can be done
                'if holes 1-9 are zero but holes 10-18 are populated, we have a back 9 only score
                'if holes 10-18 are zero but holes 1-9 are populated, we have a frnt 9 only score
                If Not bCalcSkins Then
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

                colorScores(R, "Gross", "", True)
                colorScores(R, "Net", "", True)

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
                        If sTeam = "" Then If sTeam <> R.Cells("Team").Value Then R.Cells(sColName).Style.BackColor = Color.Aqua
                    ElseIf sColName.Contains("Hole") Then
                        If cell.Value IsNot Nothing Then
                            Try
                                Dim iScore As String = RemoveSpcChar(convDBNulltoSpaces(cell.Value).Trim)
                                If iScore <> "" Then SBPMarkSubPar(cell, iScore, MyCourse(0)(sColName).ToString)
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
        Function ChkForMax(sScore As String, sHole As String) As String
            ChkForMax = ""
            'check par against hole max
            Dim imax = 0
            Select Case MyCourse(0)(sHole).ToString
                Case 3
                    imax = rLeagueParmrow("Par3Max")
                Case 4
                    imax = rLeagueParmrow("Par4Max")
                Case 5
                    imax = rLeagueParmrow("Par5Max")
                Case Else
                    imax = 99
            End Select

            If IsNumeric(sScore) Then
                If sScore > imax Then
                    'Dim mbr As MsgBoxResult = MsgBox("Score " & sScore & " Exceeds Max " & imax & vbCrLf & "Do you want to replace it with max score?", MsgBoxStyle.YesNo)
                    'If mbr = MsgBoxResult.Yes Then
                    ChkForMax = imax
                    'End If
                End If
            Else
                MsgBox("Score must be numeric " & sScore)
                Exit Function
            End If
        End Function

        Public Function CreateLabel(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String)
            Return CreateLabel(ctrl, top, left, width, name, text, "REG", "H")
        End Function
        Public Function CreateLabel(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String, dir As String)
            Return CreateLabel(ctrl, top, left, width, name, text, "REG", "H")
        End Function
        Public Function CreateLabel(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String, font As String, dir As String)
            Dim B As New Label
            ctrl.Add(B)
            B.Top = top
            B.Height = 21
            B.Width = width
            B.Left = left + 15
            B.TabStop = False
            If font = "B" Then
                'B.Font = New System.Drawing.Font("Arial", 8)
                B.Font = New System.Drawing.Font(B.Font, FontStyle.Bold)
            ElseIf font = "BU" Then
                ' B.Font = New System.Drawing.Font("Arial", 6.5)
                B.Font = New System.Drawing.Font(B.Font, FontStyle.Bold Or FontStyle.Underline)
            Else
                B.Font = New System.Drawing.Font(B.Font, FontStyle.Regular)
            End If
            B.Name = name
            B.Text = text
            If dir = "V" Then
                Return top
            Else
                Return left + B.Width + 5
            End If

        End Function
        Public Function CreateTextbox(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String, dir As String)
            Dim B As New MaskedTextBox
            ctrl.Add(B)
            B.Top = top
            B.Height = 21
            B.Width = width
            B.Left = left
            B.Name = name
            B.Text = text
            B.TabStop = True
            If dir = "V" Then
                Return top + 20
            Else
                Return left + B.Width + 5
            End If
        End Function

        Public Function CreateNumericTextbox(ctrl As Control.ControlCollection, top As Long, left As Long, width As Long, name As String, text As String)
            Dim B As New MaskedTextBox
            ctrl.Add(B)
            B.Top = top
            B.Height = 21
            B.Width = width
            B.Left = left
            B.Name = name
            B.Text = text
            B.Mask = "99"
            B.TabStop = True
            If name.Contains("Par") Then
                B.Mask = "9"
            End If

            B.ValidatingType = GetType(System.Int16)
            Return left + B.Width + 5
        End Function

        Sub CreateLabel(controlCollection As Control.ControlCollection, itop As Object, p3 As Integer, p4 As Integer, p5 As String, p6 As Char)
            Throw New NotImplementedException
        End Sub
        Public Function getHolidayList(ByVal vYear As Integer) As List(Of Date)

            Dim FirstWeek As Integer = 1
            Dim SecondWeek As Integer = 2
            Dim ThirdWeek As Integer = 3
            Dim FourthWeek As Integer = 4
            Dim LastWeek As Integer = 5

            Dim HolidayList As New List(Of Date)

            '   http://www.usa.gov/citizens/holidays.shtml      
            '   http://archive.opm.gov/operating_status_schedules/fedhol/2013.asp

            ' New Year's Day            Jan 1
            HolidayList.Add(DateSerial(vYear, 1, 1))

            ' Martin Luther King, Jr. third Mon in Jan
            HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 1, 1), DayOfWeek.Monday, ThirdWeek))

            ' Washington's Birthday third Mon in Feb
            HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 2, 1), DayOfWeek.Monday, ThirdWeek))

            ' Memorial Day          last Mon in May
            HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 5, 1), DayOfWeek.Monday, LastWeek))

            ' Independence Day      July 4
            HolidayList.Add(DateSerial(vYear, 7, 4))

            ' Labor Day             first Mon in Sept
            HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 9, 1), DayOfWeek.Monday, FirstWeek))

            ' Columbus Day          second Mon in Oct
            HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 10, 1), DayOfWeek.Monday, SecondWeek))

            ' Veterans Day          Nov 11
            HolidayList.Add(DateSerial(vYear, 11, 11))

            ' Thanksgiving Day      fourth Thur in Nov
            HolidayList.Add(GetNthDayOfNthWeek(DateSerial(vYear, 11, 1), DayOfWeek.Thursday, FourthWeek))

            ' Christmas Day         Dec 25
            HolidayList.Add(DateSerial(vYear, 12, 25))

            'saturday holidays are moved to Fri; Sun to Mon
            For i As Integer = 0 To HolidayList.Count - 1
                Dim dt As Date = HolidayList(i)
                If dt.DayOfWeek = DayOfWeek.Saturday Then
                    HolidayList(i) = dt.AddDays(-1)
                End If
                If dt.DayOfWeek = DayOfWeek.Sunday Then
                    HolidayList(i) = dt.AddDays(1)
                End If
            Next

            'return
            Return HolidayList

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

            Dim x = sText.IndexOf(sPoint) + 1
            'length of string
            Dim y = sText.Length
            'ending point
            Dim z = sText.IndexOf(sEndPoint)
            'pos of my wanted data
            Dim zz = sText.Length - (sText.IndexOf(sPoint) + 1)
            getSubstring = sText.Substring(sText.IndexOf(sPoint) + 1, sText.IndexOf(sEndPoint) - sText.IndexOf(sPoint) - 1)

        End Function
        Function getLatestFile(sFile) As String
            Dim oFiles() As IO.FileInfo
            Dim oDirectory As New IO.DirectoryInfo(sFilePath)
            oFiles = oDirectory.GetFiles(sFile)
            arraySort(oFiles)
            getLatestFile = oFiles(0).FullName
        End Function
        Sub MakeCellsStrings(row As DataGridViewRow)
            'this forces each cell to be string prevent errors on resorting columns
            For Each cell As DataGridViewCell In row.Cells
                If cell.Value Is DBNull.Value Then
                    cell.Value = ""
                End If
                If cell.FormattedValueType.Name = "String" Then
                    cell.Value = CStr(cell.Value)
                Else
                    Dim x = ""
                End If
                'Debug.Print(cell.OwningColumn.Name & "-" & cell.Value & "-" & cell.FormattedValueType.Name)
            Next
        End Sub
        Sub MakeCellsStringsDV(row As DataRowView)
            'this forces each cell to be string prevent errors on resorting columns
            Dim x = ""
            For Each cell As String In row.Row.ItemArray
                If cell Is DBNull.Value Then
                    cell = ""
                End If

                '    'Debug.Print(cell.OwningColumn.Name & "-" & cell.Value & "-" & cell.FormattedValueType.Name)
            Next
        End Sub
        Sub displayStrokes(r As DataGridViewRow)
            If r.IsNewRow Then Exit Sub
            iHdcp = r.Cells("PHdcp").Value
            For Each cell As DataGridViewCell In r.Cells
                If Not cell.OwningColumn.Name.StartsWith("Hole") Then Continue For
                cell.Value = RemoveSpcChar(convDBNulltoSpaces(cell.Value))
                cell.Style.BackColor = Color.White
                cell.Style.ForeColor = Color.Black
                'Dim sFont = "Tahoma"
                'Dim iFontSize = 12
                'check stroke index
                Dim isi = ""
                isi = CalcStrokeIndex(cell.OwningColumn.Name)
                'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
                'if the handicap > stroke index make color beige
                If iHdcp >= isi Then
                    If bColors Then cell.Style.BackColor = Color.Beige
                    If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                    'if double stroke hole, make color b/a
                    If iHdcp - iHoles >= isi Then
                        If bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                        If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                    End If
                End If
            Next

        End Sub
        Sub buildSub()
            '            For Each row As DataRowView In dvPlayers

            '                Dim sKeys() As Object = {row("Name"), sdate}
            '                Dim drow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
            '                If no Then score Is found For this player/Date, check To see If he has a Sub
            '                Dim bsub = False
            '                    If drow Is Nothing Then
            '                        find this players partner in players file
            '                    For Each player As DataRowView In dvPlayers
            '                            find this players team number in players file
            '                        If player("Team") = row("Team") Then
            '                                If the Then player name <> the missing player, we have his partner
            '                            If player("Name") <> row("Name") Then
            '                                        Now find all the scores for that team And eliminate his partner
            '                                Dim dvSubScore As DataView
            '                                        dvSubScore = New DataView(dsLeague.Tables("dtScores")) With
            '                                dvSubScore = New DataView() With
            '                                    {
            '                                        .RowFilter = String.Format("Team = {0} and Player <> '{1}' and Date = '{2}'", row("Team"), player("Name"), sdate)
            '                                    }
            '                                        If dvSubScore.Count > 0 Then
            '                                            save the regulars name for later
            '                                    Dim sSchPlayer = row("Name")
            '                                            save the subs name for later
            '                                    Dim ssub = dvSubScore(0)("Player")
            '                                            Dim bnoshow = False
            '                                            For Each spl As String In sPlayers
            '                                                If spl.Split(",")(0) = dvSubScore(0)("Player") Then
            '                                                    bnoshow = True
            '                                                    Exit For
            '                                                End If
            '                                            Next
            '                                            If bnoshow Then
            '                                                Exit For
            '                                            End If
            '20171015 - account for player subbing And his partner noshows
            '                                    sPlayers.Add(dvSubScore(0)("Player") & "," & iMatch & ip# & "," & row("Name"))
            '                                            bsub = True
            '                                        End If
            '                                        Exit For
            '                                    End If
            '                                End If
            '                    Next
            '                        If bsub Then
            '                            ip += 1
            '                            Continue For
            '                        End If
            '                        this assumes the player Is a no-show And And empty row gets built
            '                    Dim dvscores = New DataView(dsLeague.Tables("dtScores"))
            '                        Dim rowView As DataRowView = dvscores.AddNew
            '                        Change values in the DataRow.
            '                    rowView("League") = sLeagueName
            '                        rowView("Player") = row("Name")
            '                        rowView("Method") = "Score"
            '                        rowView("Group") = 0
            '                        rowView("Team") = row("Team")
            '                        rowView("Hdcp") = row("Handicap")
            '                        rowView("Date") = sdate
            '                        rowView("Skins") = "N"
            '                        rowView("Closest") = "N"
            '                        rowView("Partner") = iMatch & ip#
            '                        rowView.EndEdit()
            '                    End If
            '            Next
            '            Now build a filter for scores
            '            Dim srowfilter = "League = '" & sLeagueName.Replace("'", "''") & "' and Date = '" & sdate & "' and Player in ('"
            '            search the players file And match to the subs array list replacing regulars with subs for the filter
            '            For Each prow As String In sPlayers
            '                srowfilter = srowfilter & prow.Split(",")(0).ToString & "','"
            '            Next
            '            srowfilter = srowfilter & ")"
            '            srowfilter = srowfilter.Replace(",')", ")")
            '            For Each sPlayer In sPlayers
            '                Dim sKeys() As Object = {sPlayer.Split(",")(0), sdate}
            '                Dim drow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
            '                If no Then Score Is found For this player/Date, check To see If he has a Sub
            '                If drow IsNot Nothing Then
            '                    drow("Partner") = sPlayer.Split(",")(1)
            '                End If
            '            Next

        End Sub
        '20180325-changes for bye team
        'Sub getMatchScores_20180409(sdate As String)

        '    If bloghelper Then LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        '    CalcHoleMarker(sdate)
        '    Dim dtschedule As New DataTable
        '    dtschedule = buildSchedule()
        '    Dim sKey = DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0")
        '    Dim rSch As DataRow = dtschedule.Rows.Find(sKey)
        '    If rSch Is Nothing Then
        '        MsgBox("No scheduled matches found for this date, must exit")
        '        Exit Sub
        '    End If
        '    Dim dvPlayers As DataView
        '    dvPlayers = New DataView(dsLeague.Tables("dtPlayers"))
        '    Dim sPlayersRowFilter = ""
        '    Dim sSD = rLeagueParmrow("StartDate")
        '    Dim sED = rLeagueParmrow("EndDate")
        '    Dim ip# = 0
        '    Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
        '    For iMatch = 1 To rLeagueParmrow("Teams") / 2
        '        Dim sMatch = rSch(iMatch.ToString).ToString
        '        Dim sHt = sMatch.Split("v")(0)
        '        Dim sVt = sMatch.Split("v")(1)
        '        sPlayersRowFilter = String.Format("Team IN ('{0}','{1}') AND ISNULL(DateJoined,'00010101') <= '{2}' AND ISNULL(DateLeft,'99999999') > '{3}'", sHt, sVt, sSD, sED)
        '        dvPlayers = New DataView(dsLeague.Tables("dtPlayers")) With
        '        {
        '         .RowFilter = sPlayersRowFilter, .Sort = "Team, Grade"
        '        }

        '        For Each player As DataRowView In dvPlayers
        '            Dim sKeys() As Object = {player("Name"), sdate}
        '            Dim drow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
        '            'if player is not in the scores for that day, check to see if he has a sub
        '            If drow Is Nothing Then
        '                'find this players partner in filtered players file
        '                For Each partner As DataRowView In dvPlayers
        '                    'find this players team number in players file
        '                    If partner("Team") = player("Team") Then
        '                        'if the player name <> the missing player, we have his partner
        '                        If player("Name") <> partner("Name") Then
        '                            'now find all the scores for that team and eliminate his partner
        '                            Dim dvSubScore As DataView
        '                            dvSubScore = New DataView(dsLeague.Tables("dtScores")) With
        '                            {
        '                                .RowFilter = String.Format("Team = {0} and Player <> '{1}' and Date = '{2}'", player("Team"), player("Name"), sdate)
        '                            }
        '                            'add the missing player as an empty score
        '                            If dvSubScore.Count <> 2 Then
        '                                Dim rowView As DataRowView = dvscores.AddNew
        '                                ' Change values in the DataRow.
        '                                rowView("League") = sLeagueName
        '                                rowView("Player") = player("Name")
        '                                'rowView("Method") = "Score"
        '                                rowView("Group") = 0
        '                                rowView("Team") = player("Team")
        '                                'rowView("Hdcp") = row("Handicap")
        '                                rowView("Date") = sdate
        '                                'rowView("Skins") = "N"
        '                                'rowView("Closest") = "N"
        '                                rowView("Partner") = ip#
        '                                rowView.EndEdit()
        '                            End If
        '                            Exit For
        '                        End If
        '                    End If
        '                Next
        '            Else
        '                drow("Partner") = CStr(ip#).PadLeft(2, "0")
        '                drow.EndEdit()
        '            End If
        '            ip# += 1
        '        Next
        '    Next

        '    dvscores.Sort = "Partner"

        'End Sub
        Function getMatchScores(sdate As String) As Boolean
            If bloghelper Then LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            getMatchScores = False
            CalcHoleMarker(sdate)
            Dim dtschedule As New DataTable
            dtschedule = buildSchedule()
            Dim sKey = DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0")
        'sKey = sdate.Substring(4, 2).Trim("0") & "/" & sdate.Substring(6, 2).Trim("0") & "/" & sdate.Substring(0, 4)
        sKey = sdate.Substring(4, 2).Trim("0") & "/" & sdate.Substring(6, 2) & "/" & sdate.Substring(0, 4)

        Dim rSch As DataRow = dtschedule.Rows.Find(sKey)
            If rSch Is Nothing Then
                MsgBox(String.Format("No scheduled matches found for this date {0}, must exit", sKey))
                Exit Function
            End If
            Dim ip# = 0

            For iMatch = 1 To rLeagueParmrow("Teams") / 2
                Dim sMatch = rSch(iMatch.ToString).ToString
                sTeam = sMatch.Split("v")(1)
                'save this players Partner in splayer
                sPlayer = getPlayer(sTeam, "B")
                If getPlayer(sTeam, "A") = "" Then Exit Function
                getScore(getPlayer(sTeam, "A"), sdate, ip#)
                ip# += 1
                'save this players Partner in splayer
                sPlayer = getPlayer(sTeam, "A")
                If getPlayer(sTeam, "B") = "" Then Exit Function
                getScore(getPlayer(sTeam, "B"), sdate, ip#)
                ip# += 1
                sTeam = sMatch.Split("v")(0)
                'save this players Partner in splayer
                sPlayer = getPlayer(sTeam, "B")
                If getPlayer(sTeam, "A") = "" Then Exit Function
                getScore(getPlayer(sTeam, "A"), sdate, ip#)
                ip# += 1
                'save this players Partner in splayer
                sPlayer = getPlayer(sTeam, "A")
                If getPlayer(sTeam, "B") = "" Then Exit Function
                getScore(getPlayer(sTeam, "B"), sdate, ip#)
                ip# += 1
            Next
            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
            dvscores.Sort = "Date,Partner"
            'mark as good scores
            getMatchScores = True
        End Function
        Private Function getPlayer(sTeam, sGrade) As String
            getPlayer = ""
            Dim sPlayersRowFilter = String.Format("Team ='{0}' AND Grade = '{1}'", sTeam, sGrade)
            Dim dvPlayers = New DataView(dsLeague.Tables("dtPlayers")) With
                {
                 .RowFilter = sPlayersRowFilter
                }
            If dvPlayers.Count > 0 Then
                getPlayer = dvPlayers(0)("Name")
            Else
                MsgBox(String.Format("Team {0} is missing an {1} player, fix player file and try again", sTeam, sGrade))
            End If

        End Function
        Private Sub getScore(sthisPlayer As String, sDate As String, ip#)
            Dim sKey() As Object = {sthisPlayer, sDate}
            Dim drow = dsLeague.Tables("dtScores").Rows.Find(sKey)
            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
            If drow Is Nothing Then
                Dim srowfilter = String.Format("Player <> '{0}' AND Date = '{1}' AND Team = '{2}'", sPlayer, sDate, sTeam)
                dvscores.RowFilter = srowfilter
                If dvscores.Count = 0 Then
                    Dim rowView As DataRowView = dvscores.AddNew
                    ' Change values in the DataRow.
                    rowView("League") = sLeagueName
                    rowView("Player") = sthisPlayer
                    rowView("Group") = 0
                    rowView("Team") = sTeam
                    rowView("Date") = sDate
                    rowView("Partner") = CStr(ip#).PadLeft(2, "0")
                    rowView.EndEdit()
                Else
                    dvscores(0)("Partner") = CStr(ip#).PadLeft(2, "0")
                End If
            Else
                drow("Partner") = CStr(ip#).PadLeft(2, "0")
                drow.EndEdit()
            End If
        End Sub
        Public Sub ReloadScores()
            If dsLeague.Tables("dtScores") IsNot Nothing Then dsLeague.Tables.Remove("dtScores")
            'dsLeague.Tables.Add("dtScores").ReadXml(getLatestFile("*Scores.xml"))
            dsLeague.Tables.Add("dtScores")
            CSV2DataTable(dsLeague.Tables("dtScores"), getLatestFile("*Scores.csv"))
            dsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {dsLeague.Tables("dtScores").Columns("Player"), dsLeague.Tables("dtScores").Columns("Date")}
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
        Function Create_Html(dt As DataTable) As String

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

        'this code put in for the extra b player, put an * by his name (2nd instance) if hes in there twice
        'Dim hash As New System.Collections.Hashtable()
        '' Dim sDups As New List(Of String)
        'Dim itemKey As String
        ''save all the players in hash
        'For Each R As DataGridViewRow In dgScoreCard.Rows
        '    itemKey = R.Cells("Player").Value
        '    If Not hash.ContainsKey(itemKey) Then
        '        hash.Add(itemKey, R.Cells("Player").Value)
        '    End If
        'Next
    End Class

'End Namespace