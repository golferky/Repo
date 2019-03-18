'Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Net.Mail

Public Class Helper

    Public dsLeague As DataSet
    Public sFilePath As String
    Public sLeagueName As String
    Public sGroupNumber As Integer
    Public sFrontBack As String
    Public dDate As Date
    Public iHoles As Integer
    Public iHoleMarker As Integer
    Private _iHdcp As Integer
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
    Public bScreenChanged As Boolean = False
    Public bDateOverlap As Boolean = True
    'fields with (Number) are key fields
    'field-width-read only-tabstop-MiddleRight
    Public Const cPat20 = "25-false-true-mr"
    Public Const cPat30 = "30-false-true-mr"
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
    Public Const cStatsFields As String = "Eagles-cPat40nt,Birdies-cPat60nt,Pars-cPat40nt,Bogeys-cPat40nt,DoubleBogeys-cPat40nt,Others-cPat40nt"
    Public MyCourse() As Data.DataRow
    Public bAllHolesEntered = False
    Public sArrayOfFiles As New List(Of String)
    Public rLeagueParmrow As DataRowView
    Public bsch = False
    Public bscores = False
    Public bplayer = False
    Public bcourses = False
    Public bpayments = False
    Public GGmail As GGSMTP_GMAIL
    Public sFileInUseMessage As String
    ' Create the ToolTip and associate with the Form container.
    Public toolTipHdcp As New ToolTip()
    '20180130-num of Closests
    Public iNumClosests = 0
    Public bByeFound = False
    Public bCCLeague As Boolean = False
    Public bSwap9 As Boolean
    Public sDateLastScore As String

    Public Property IHdcp As Integer
        Get
            Return _iHdcp
        End Get
        Set(value As Integer)
            _iHdcp = value
        End Set
    End Property

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
        Catch ex As Exception
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

    Public Sub GMmailIT()
        Try

            Dim SmtpServer As New SmtpClient()
            With SmtpServer
                Dim mail As New MailMessage()
                With mail
                    mail = New MailMessage()
                    .From = New MailAddress("garyrscudder@gmail.com")
                    .To.Add("gary_scudder@yahoo.com")
                    .Subject = "Test Mail"
                    .Body = "This is for testing SMTP mail from GMAIL"
                    'mail.Attachments()
                End With

                .Credentials = New Net.NetworkCredential("garyrscudder@gmail.com", "4St-SCQ-Jt6-tB6")
                .Port = 587
                .Host = "smtp.gmail.com"
                .EnableSsl = True
                .Send(mail)
                MsgBox("mail send")

            End With

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
                            Dim dc = New DataColumn(sAry(i))
                            If sAry(i) = "Team" Or sAry(i).Contains("#") Or sAry(i).Contains("$") Then
                                dc.DataType = System.Type.GetType("System.Int16")
                            Else
                                dc.DataType = System.Type.GetType("System.String")
                            End If
                            dt.Columns.Add(dc)
                        Next
                    End If
                    Continue Do
                End If

                aRow = dt.NewRow
                For i = 0 To sAry.Count - 1
                    If sAry(i).Trim <> "" Then
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
        '20180923-allow for combined club championship and league play dont adjust handicaps for club champ or league play
        If bCCLeague Then
            GetNewHdcp = row.Cells("pHdcp").Value
            Exit Function
        End If
        GetNewHdcp = ""
        Dim iLast5Scores As New List(Of Decimal)
        Dim iRoundctr = 0
        Dim iPHdcp = 0
        Try
            Dim dvScores As New DataView(dsLeague.Tables("dtScores"))
            'dvScores.RowFilter = "Player = '" & row.Cells("Player").Value & "'" & " and Date < '" & sDate & "'"
            dvScores.RowFilter = String.Format("Player = '{0}' and Date < '{1}' and Method <> '' ", row.Cells("Player").Value, sDate)
            dvScores.Sort = "Date desc"
            'this compensates for lost scores after week 1 in 2017, i have hardcoded the prev handicap on 4/11 so we dont have to go back
            'If sDate > "20170411" Then
            'dvScores.RowFilter = dvScores.RowFilter & " and Date >= '20170411'"
            'End If
            'build a total of 5 max scores in our array
            iRoundctr = 0
            Dim sMethod As String = ""
            For Each score As DataRowView In dvScores
                sMethod = convDBNulltoSpaces(score("Method"))
                If sMethod = "" Then Continue For
                iRoundctr += 1
                sPlayer = score("Player").ToString
                iPHdcp = score("PHdcp").ToString
                Dim sScoreDate As String = score("Date")
                CalcHoleMarker(sScoreDate)
                '20181004 - override calcholemarker because of cc and league in same sch
                If sMethod = "Gross" Or sMethod = "Net" Then
                    If score("Hole1") IsNot DBNull.Value Then
                        iHoleMarker = 1
                    Else
                        iHoleMarker = 10
                    End If
                End If
                If sMethod = "Net" Then
                    If iHoles = 9 Then
                        If iHoleMarker = 10 Then
                            iLast5Scores.Add(score("In_Net").ToString + iPHdcp)
                        Else
                            iLast5Scores.Add(score("Out_Net").ToString + iPHdcp)
                        End If
                    Else
                        iLast5Scores.Add(score("18_Net").ToString + iPHdcp)
                    End If
                ElseIf sMethod = "Gross" Then
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
                ElseIf sMethod = "Score" Then
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
            sMethod = row.Cells("Method").Value
            '20181004 - override calcholemarker because of cc and league in same sch
            If sMethod = "Gross" Or sMethod = "Net" Then
                If row.Cells("Hole1") IsNot DBNull.Value Then
                    iHoleMarker = 1
                Else
                    iHoleMarker = 10
                End If
            End If

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
                IHdcp = GetHdcp(iLast5Scores, iRoundctr, sDate)
            Else
                IHdcp = iPHdcp
            End If

            For Each score In iLast5Scores
                row.Cells("Hdcp").ToolTipText = row.Cells("Hdcp").ToolTipText & score & "-"
            Next
            row.Cells("Hdcp").ToolTipText = row.Cells("Hdcp").ToolTipText.Trim("-")

            Return IHdcp

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Function GetHdcp(ByRef ilast5Scores As List(Of Decimal), iRoundctr As String, sDate As String) As String
        GetHdcp = ""
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
                If row("Method") Is DBNull.Value Then Continue For
                row("Out_Gross") = row("Out_Gross")
                row("In_Gross") = row("In_Gross")
                If row("Out_Gross") Is DBNull.Value And row("In_Gross") Is DBNull.Value Then Continue For

                'Debug.Print(row("Player") & "-" & row("Out_Gross") & "-" & row("In_Gross") & "-" & "icnt-" & icnt)
                'if 5 scores, load the score record to the list
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
                    If row("Out_Gross") IsNot DBNull.Value Then
                        'Debug.Print("Score added " & row("Out_Gross"))
                        lvRec.SubItems.Add(row("Out_Gross") & "-" & row("Hdcp"))
                    Else
                        'Debug.Print("Score added " & row("In_Gross"))
                        lvRec.SubItems.Add(row("In_Gross") & "-" & row("Hdcp"))
                    End If
                End If

                icnt += 1
            Next
            lv1.Height = lv1.Items.Count * 15

            lv1.Items.Add(lvRec)
            frm.Controls.Add(lv1)
            frm.Height = lv1.Height + 200
            frm.Width = lv1.Width + 100
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Public Function fGetTeam(sNameInfo) As String
        fGetTeam = ""
        Try
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
        fGetPlayer = ""
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
                binitials = True
                sRowFilter = "Name like '" & sNameInfo.Substring(0, 1) & "%' and Name like '% " & sNameInfo.Substring(1, 1) & "%'"
            ElseIf sNameInfo.Split(":").Length = 2 Then
                sRowFilter = "Name like '" & sNameInfo.Split(":")(0) & "%' and Name like '% " & sNameInfo.Split(":")(1) & "%'"
            Else
                sRowFilter = "Name like '" & sNameInfo & "%'"
            End If

            dvPlayers.RowFilter = sRowFilter
            'try nickname
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
                    fGetPlayer = fixPlayer(sNameInfo & ":New")
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
                    'If frmScoreCard.sOldCellValue = dvplayer("Name") Then Continue For
                    If dgv IsNot Nothing Then
                        Dim bfound = False
                        'check to see if any of these players is already used
                        For Each row As DataGridViewRow In dgv.Rows
                            'CurrentCell.OwningColumn.Name
                            Dim sName = ""
                            If dgv.Name = "dgScores" Then
                                sName = row.Cells("Player").Value
                            ElseIf dgv.Name = "DtPlayersDataGridView" Then
                                sName = row.Cells(0).Value
                            End If
                            If sName = dvplayer("Name") Then
                                bfound = True
                                Exit For
                            End If
                        Next
                        If Not bfound Then sUniquePlayers.Add(dvplayer("Name"))
                    End If
                Next

                If sUniquePlayers.Count = 0 Then
                    Dim sResult As MsgBoxResult
                    sResult = MsgBox("Player not found or is already used" & vbCrLf & " Do you want to create a player?", MsgBoxStyle.YesNo)
                    If sResult = MsgBoxResult.Yes Then
                        fGetPlayer = fixPlayer(sNameInfo & ":New")
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
                            'frmPlayer.ShowDialog()
                            fixPlayer(sNameInfo)
                            dgv.Update()
                            fGetPlayer = "" '  aRow("Name")
                        End If
                    End If
                    For Each sup In sUniquePlayers
                        Dim sMtype = ""
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
                            fGetPlayer = sup 'Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sup)
                            Exit Function
                        ElseIf sResult = MsgBoxResult.Cancel Then
                            Exit Function
                        End If
                    Next
                    MsgBox("Go to the player setup screen and setup the new player and re-type this player")
                End If
            Else 'fGetPlayer = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dvPlayers(0).Item("Name"))
                fGetPlayer = dvPlayers(0).Item("Name")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function
    'create rules for last names like McDonald, O'Reilly, etc
    Function fixPlayer(sNameInfo As String) As String
        fixPlayer = ""
        'create rules for last names like McDonald, O'Reilly, etc
        Dim sFLName As String() = sNameInfo.ToString.Split(" ")
        If sFLName.Count > 2 Then
            MsgBox("Name cant have more than First/Last Name, Fix Manually")
            fixPlayer = sNameInfo
        ElseIf sFLName.Count = 1 Then
            MsgBox("Funky Name Fix Manually")
            fixPlayer = sNameInfo
        ElseIf sFLName.Count = 2 Then
            If sFLName(1).Length >= 3 Then
                Dim sPrefixIn As String = sFLName(1).Substring(0, 3)
                Dim sPrefixOut As String = sPrefixIn.Replace(sPrefixIn.Substring(2, 1), sPrefixIn.Substring(2, 1).ToUpper)
                If sPrefixIn.StartsWith("Mc") Then fixPlayer = sNameInfo.ToString.Replace(sPrefixIn, sPrefixOut)
            End If
        End If
        'Dim aRow As DataRow
        'aRow = dsLeague.Tables("dtPlayers").NewRow
        'aRow("Name") = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sNameInfo)
        'dsLeague.Tables("dtPlayers").Rows.Add(aRow)
        'fixPlayer = aRow("Name")

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

    Public Sub SBPMarkSubPar(cell As DataGridViewCell, iscore As Integer, iPar As Integer)
        If IHdcp = 99 Then Exit Sub
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
        If IHdcp >= isi Then
            If bColors Then cell.Style.BackColor = Color.Beige
            If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
            'if double stroke hole, make color b/a
            If IHdcp - iHoles >= isi Then
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
        If IHdcp = 99 Then Exit Sub
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
        If IHdcp >= isi Then
            If bColors Then cell.Style.BackColor = Color.Beige
            If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
            'if double stroke hole, make color b/a
            If IHdcp - iHoles >= isi Then
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
        buildSchedule = Nothing
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
        SBPCalcStrokeIndex = ""
        Try
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            'check stroke index
            SBPCalcStrokeIndex = 0
            Dim isi = MyCourse(0).Item(sHole.Replace("Hole", "H"))
            'if 9 holes and its an odd stroke index, adjust for remainder when we divide by 2
            If iHoles = 9 Then
                If isi Mod 2 Then isi += 1
                isi = Math.Round((isi) / 2, 0)
            End If
            Return isi
        Catch ex As Exception

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

            IHdcp = R.Cells.Item("pHdcp").Value.ToString
            If iHoles > 9 Then IHdcp *= 2
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
                    If sTeam = "" Then R.Cells(sColName).Style.BackColor = Color.Aqua
                ElseIf sColName.Contains("Hole") Then
                    If cell.Value IsNot Nothing And cell.Value IsNot DBNull.Value Then
                        Try
                            Dim iScore As String = RemoveSpcChar(convDBNulltoSpaces(cell.Value).Trim)
                            If iScore <> "0" Then MarkSubPar(cell, iScore, MyCourse(0)(sColName).ToString)
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
    Public Sub SBPChangeColorsForStrokes(ByVal R As DataGridViewRow)
        'LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'this sub will call marksubpar routine to color the cell for birdies and eagles
        Try
            'keep new rows from being evaluated
            '20180307-evaluate this statement, should this be here
            If R.Cells.Item("pHdcp").Value.ToString = "" Then Exit Sub

            IHdcp = R.Cells.Item("pHdcp").Value.ToString
            'this takes a 9 hole handicap and makes it 18 hole handicap
            If iHoles > 9 Then IHdcp *= 2
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
                    If R.Cells("Team_Points").Value = "0.5" Then
                        R.Cells("Team_Points").Style.BackColor = Color.LightGreen
                    ElseIf R.Cells("Team_Points").Value = "0.25" Then
                        R.Cells("Team_Points").Style.BackColor = Color.Yellow
                    End If
                    'If R.Cells("Team_Points").Value = "0.5" Then R.Cells("Team_Points").Style.BackColor = Color.LightGreen
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
        getSubstring = sText.Substring(sText.lastIndexOf(sPoint) + 1, sText.IndexOf(sEndPoint) - sText.lastIndexOf(sPoint) - 1)

    End Function
    Function getLatestFile(sFile) As String
        If Directory.Exists(sFilePath) Then
        Else
            Dim mbr = MessageBox.Show(String.Format("Path not found {0}, pick a folder to pull in files from or Cancel", sFilePath), "Warning", MessageBoxButtons.OKCancel)
            If mbr = Windows.Forms.DialogResult.Cancel Then End
            Dim dialog As New FolderBrowserDialog With
         {
         .RootFolder = Environment.SpecialFolder.Desktop,
         .SelectedPath = sFilePath,
         .Description = "Select League Files Path"
         }
            If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                sFilePath = dialog.SelectedPath
            Else
                End
            End If
        End If

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

    Sub displayStrokes(r As DataGridViewRow)
        If r.IsNewRow Then Exit Sub
        IHdcp = r.Cells("PHdcp").Value
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
            If IHdcp >= isi Then
                If bColors Then cell.Style.BackColor = Color.Beige
                If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                'if double stroke hole, make color b/a
                If IHdcp - iHoles >= isi Then
                    If bColors Then cell.Style.BackColor = Color.BlanchedAlmond
                    If bDots Then cell.Value = cell.Value & ChrW(&H25CF)
                End If
            End If
        Next

    End Sub

    Function getMatchScores(sdate As String) As Boolean
        If bloghelper Then LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        getMatchScores = False
        'CalcHoleMarker(sdate)

        Dim dtschedule As New DataTable
        dtschedule = buildSchedule()
        Dim sKey = DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0")
        'sKey = sdate.Substring(4, 2).Trim("0") & "/" & sdate.Substring(6, 2).Trim("0") & "/" & sdate.Substring(0, 4)
        sKey = sdate.Substring(4, 2).TrimStart("0") & "/" & sdate.Substring(6, 2).TrimStart("0") & "/" & sdate.Substring(0, 4)

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
    Function FCalcSkins(dgScores As DataGridView) As List(Of String)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        FCalcSkins = New List(Of String)
        'this code goes through the listview and highlights the lowest value on each hole and fron 9, back 9 and total
        Dim ilowrow As New List(Of String)
        'adjust for handicap fields in listview
        'loop through each column finding the lowest scores
        'Get low 9's and 18 hole
        '20171014 - use holemarker to control which 9 or 18 you process
        For ii = iHoleMarker To iHoleMarker + iHoles - 1 'lv1.Items(0).SubItems.Count - 1
            Dim ilowscore = 99
            'calculate a column saving low score
            For i = 0 To dgScores.RowCount - 1
                If dgScores.Rows(i).Cells("Player").Value = "*** Total ***" Then Continue For
                sPlayer = dgScores.Rows(i).Cells("Player").Value
                If dgScores.Rows(i).Cells("PHdcp").Value.ToString <> "" Then
                    IHdcp = dgScores.Rows(i).Cells("PHdcp").Value
                Else
                    If iHoleMarker = 1 Then
                        IHdcp = dgScores.Rows(i).Cells("Out_Gross").Value - dgScores.Rows(i).Cells("Out_Net").Value
                    Else
                        IHdcp = dgScores.Rows(i).Cells("In_Gross").Value - dgScores.Rows(i).Cells("In_Net").Value
                    End If
                    dgScores.Rows(i).Cells("PHdcp").Value = IHdcp
                End If
                If dgScores.Rows(i).Cells("Skins").Value = "Y" Then
                    If dgScores.Rows(i).Cells("Hole" & ii).Value IsNot DBNull.Value Then
                        Dim iscore As String = RemoveSpcChar(dgScores.Rows(i).Cells("Hole" & ii).Value)
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

            If ilowrow.Count = 1 Then
                Dim score As Integer = ilowrow(0)
                dgScores.Rows(score).Cells("Hole" & ii).Style.BackColor = Color.Gold
                dgScores.Rows(score).Cells("Player").Style.BackColor = Color.Gold
                dgScores.Rows(score).Cells("$Skins").Style.BackColor = Color.Gold
                FCalcSkins.Add(score)
            Else
                Dim sFont = "Tahoma"
                Dim iFontSize = 12
                For Each player In ilowrow
                    dgScores.Rows(player).Cells("Hole" & ii).Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout)
                    dgScores.Rows(player).Cells("Hole" & ii).Style.BackColor = Color.Yellow
                    Debug.Print(String.Format("setting skins tie for hole {0} {1}", ii, dgScores.Rows(player).Cells("Player").Value))
                Next
            End If
        Next

    End Function
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
    Function CreateColumnsWithFormat(fld As String, dt As DataTable, sColFormat As List(Of String)) As DataColumn
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

    Function CreateColumn(fld As String, dt As DataTable) As DataColumn
        CreateColumn = New DataColumn()
        With CreateColumn
            .DataType = System.Type.GetType("System.String")
            .ColumnName = fld
            ' Add the column to the DataTable.Columns collection.
            dt.Columns.Add(.ColumnName)
        End With

    End Function

    Sub status_Msg(lbStatus As Label, frm As Form)

        If lbStatus.Text.Contains("Finished") Then
            lbStatus.BackColor = Color.LightGreen
            frm.Cursor = Cursors.Default
        Else
            lbStatus.BackColor = Color.Red
            frm.Cursor = Cursors.WaitCursor
        End If
        Application.DoEvents()
    End Sub
    Sub CopyDataGridViewToClipboard(ByRef dgv As DataGridView)
        Try
            Dim s As String = ""
            Dim oCurrentCol As DataGridViewColumn    'Get header
            oCurrentCol = dgv.Columns.GetFirstColumn(DataGridViewElementStates.Visible)
            Do
                s &= oCurrentCol.HeaderText & Chr(Keys.Tab)
                oCurrentCol = dgv.Columns.GetNextColumn(oCurrentCol,
               DataGridViewElementStates.Visible, DataGridViewElementStates.None)
            Loop Until oCurrentCol Is Nothing
            s = s.Substring(0, s.Length - 1)
            s &= Environment.NewLine    'Get rows
            For Each row As DataGridViewRow In dgv.Rows
                oCurrentCol = dgv.Columns.GetFirstColumn(DataGridViewElementStates.Visible)
                Do
                    If row.Cells(oCurrentCol.Index).Value IsNot Nothing Then
                        s &= row.Cells(oCurrentCol.Index).Value.ToString
                    End If
                    s &= Chr(Keys.Tab)
                    oCurrentCol = dgv.Columns.GetNextColumn(oCurrentCol,
                      DataGridViewElementStates.Visible, DataGridViewElementStates.None)
                Loop Until oCurrentCol Is Nothing
                s = s.Substring(0, s.Length - 1)
                s &= Environment.NewLine
            Next    'Put to clipboard
            Dim o As New DataObject
            o.SetText(s)
            Clipboard.SetDataObject(o, True)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Sub SaveDataGridViewToCSV(dgv As DataGridView, filename As String)
        dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        dgv.SelectAll()
        Dim dataob As DataObject = dgv.GetClipboardContent
        System.IO.File.WriteAllText(filename, dataob.GetText(TextDataFormat.CommaSeparatedValue))
    End Sub
    Sub dgv2csv(dgv As DataGridView, filename As String)
        Dim csv = ""
        Try
            Dim headers = (From header As DataGridViewColumn In dgv.Columns.Cast(Of DataGridViewColumn)()
                           Select header.HeaderText).ToArray
            Dim rows = From row As DataGridViewRow In dgv.Rows.Cast(Of DataGridViewRow)()
                       Where Not row.IsNewRow
                       Select Array.ConvertAll(row.Cells.Cast(Of DataGridViewCell).ToArray, Function(c) If(c.Value IsNot Nothing, RemoveSpcChar(c.Value.ToString), ""))
            Using sw As New IO.StreamWriter(filename)
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

    Function WaitForFile(dt As DataTable, sFile As String, lbstatus As Label, frm As Form) As Boolean
        WaitForFile = False
        If CSV2DataTable(dt, sFile) Then
            WaitForFile = True
            If sFile.Contains("LeagueParm") Then
                If dDate.ToString("MM/dd/yyyy") < "20190101" Then
                    If Not dt.Columns.Contains("PostSeasonDt") Then
                        dt.Columns("PostSeason").ColumnName = "PostSeasonDt"
                        For Each col In dt.Rows
                            col("PostSeasonDt") = "09/18/2018"
                        Next
                    End If
                End If
            End If
        Else
            Dim i = 30
            MsgBox(String.Format("File {0} is in use, will wait up for {1} seconds to free up", sFile, i))
            Do Until i = 0
                If CSV2DataTable(dt, sFile) Then
                    WaitForFile = True
                    Exit Function
                End If

                lbstatus.Text = String.Format("Waiting for file {0} for {1} seconds", sFile, i)
                status_Msg(lbstatus, frm)
                Threading.Thread.Sleep(1000)
                i -= 1
            Loop
            lbstatus.Text = String.Format("Finished Waiting for file {0}", sFile)
            status_Msg(lbstatus, frm)
            Exit Function
        End If
    End Function
    Function getLeagParm(sScoreDate As String, lbstatus As Label, frm As Form) As String 'parmfile,good(got the league file) or roskins:rocp1:rocp2
        Dim sParmFile As String = ""
        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(sFilePath)
        oFiles = oDirectory.GetFiles("*LeagueParms.csv")
        For Each sfile In oFiles
            If sfile.Name.Substring(0, 8) < sScoreDate Then
                sParmFile = sfile.FullName
            Else
                Exit For
            End If
        Next
        getLeagParm = sParmFile
        Dim dtLeagueParm = New DataTable
        Dim bwait = True
        Do While bwait
            If Not WaitForFile(dtLeagueParm, sParmFile, lbstatus, frm) Then
                Dim mbr = MessageBox.Show(String.Format("File in use {0}Press <OK> to close file and proceed or <Cancel>", vbCrLf, sParmFile), sParmFile, MessageBoxButtons.OKCancel)
                If mbr = DialogResult.Cancel Then
                    getLeagParm = getLeagParm & ",bad"
                    Exit Function
                End If
            Else
                bwait = False
            End If
        Loop
        If Not IO.Directory.Exists(sFilePath) Then
            Dim mbr = MessageBox.Show(String.Format("Path not found {0}, pick a folder to pull in files from or Cancel", sFilePath), "Warning", MessageBoxButtons.OKCancel)
            If mbr = Windows.Forms.DialogResult.Cancel Then End
            Dim dialog As New FolderBrowserDialog With
            {
             .RootFolder = Environment.SpecialFolder.Desktop,
             .SelectedPath = sFilePath,
             .Description = "Select League Files Path"
            }
            If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                sFilePath = dialog.SelectedPath
            Else
                End
            End If
        End If
        'get this years parm file record
        Dim foundrows As DataRow()
        foundrows = dtLeagueParm.Select(String.Format("#{0}# >= StartDate and #{0}# <=EndDate", dDate.ToString("MM/dd/yyyy")))
        'accumulate rolled over amounts
        If foundrows.Count = 0 Then
            Throw New Exception(String.Format("No League Parameter record found for this date {0}", dDate.ToString("MM/dd/yyyy")))
        Else
            For Each row In foundrows
                getLeagParm = getLeagParm & ","
                getLeagParm = getLeagParm & IIf(convDBNulltoSpaces(row("RolledOverCTP1")) = " ", 0, row("RolledOverCTP1")) & ":"
                getLeagParm = getLeagParm & IIf(convDBNulltoSpaces(row("RolledOverCTP2")) = " ", 0, row("RolledOverCTP2")) & ":"
                getLeagParm = getLeagParm & IIf(convDBNulltoSpaces(row("RolledOverSkins")) = " ", 0, row("RolledOverSkins")) & ":"
                rLeagueParmrow = dtLeagueParm.DefaultView(dtLeagueParm.Rows.IndexOf(row))
            Next
        End If

    End Function
End Class