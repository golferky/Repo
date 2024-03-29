﻿'Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Net.Mail
Imports GolfManager.Constants
Public Class Helper

    Public dsLeague As DataSet
    Public sFilePath As String
    Public sReportPath As String
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
    Public bload As Boolean
    Public bLockScores As Boolean
    Public sTotalColumn As String = "*** Total ***"
    Public sMessage As String = ""
    Public dLastWeeksSkins As Decimal
    Public dLastWeeksCTPF1 As Decimal
    Public dLastWeeksCTPF2 As Decimal
    Public dLastWeeksCTPB1 As Decimal
    Public dLastWeeksCTPB2 As Decimal
    Public dThisWeeksSkins As Decimal
    Public dThisWeeksCTPF1 As Decimal
    Public dThisWeeksCTPF2 As Decimal
    Public dThisWeeksCTPB1 As Decimal
    Public dThisWeeksCTPB2 As Decimal
    Public dExtraSkins As Decimal
    Public dExtraCTPF1 As Decimal
    Public dExtraCTPF2 As Decimal
    Public dExtraCTPB1 As Decimal
    Public dExtraCTPB2 As Decimal
    Public dtWklySkins As DataTable
    Public dtnewWklySkins As DataTable
    Public sMACDBVersion As String = ""

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
    Friend myprocessarray As New ArrayList
    Private myProcess As Process
    Public thisCourse As DataRow
    Public UnderParColor As Color = Color.OrangeRed
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

                .Credentials = New Net.NetworkCredential("garyrscudder@gmail.com", "8@L0hV&5Oim%LTlh3KD%")
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
                .Provider = sMACDBVersion,
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
                If line.Contains("20170411") Then Console.WriteLine(line)
                dlinecnt += 1
                If line Is Nothing Then Exit Do

                'build a string array of scores using comma delimited
                Dim sAry As String() = Split(line, ",")
                '2019-07-26 this causes issues on schedule because first week could be a rainout like 2018
                'If sAry(0).ToString = "" Then Continue Do
                'if this is the first line, it is a header so save each column header and mark the numeric ones 
                If Debugger.IsAttached Then
                    LOGIT($"table {dt.TableName} has {sAry.Count - 1} columns")
                End If
                If dlinecnt = 1 Then
                    Try
                        'if dt has 0 columns there is no xsd created
                        If dt.Columns.Count = 0 Then
                            For i = 0 To sAry.Count - 1
                                'LOGIT($"column {sAry(i)}")
                                Dim dc = New DataColumn(sAry(i))
                                If sAry(i) = "Team" Or sAry(i).Contains("#") Or sAry(i).Contains("$") Then
                                    dc.DataType = System.Type.GetType("System.Int16")
                                Else
                                    dc.DataType = System.Type.GetType("System.String")
                                End If
                                dt.Columns.Add(dc)
                            Next
                        End If

                    Catch ex As Exception

                    End Try
                    For Each scol In sAry
                        If Not dt.Columns.Contains(scol) Then
                            LOGIT($"sxd for table {dt.TableName} missing col {scol} ")
                        End If
                    Next
                    Continue Do
                End If

                Try
                    aRow = dt.NewRow
                    For i = 0 To sAry.Count - 1
                        If sAry(i).Trim <> "" Then
                            aRow(i) = sAry(i)
                        End If
                    Next
                    dt.Rows.Add(aRow)

                Catch ex As Exception
                    LOGIT($"Record {dlinecnt} Dropped {line} table has {dt.Columns.Count} columns error {ex.Message}")
                    Continue Do
                End Try


                'Try
                '    dt.Rows.Add(aRow)
                'Catch ex As Exception
                '    If Debugger.IsAttached Then Debug.Print("")

                'End Try
            Loop
            myStream.Close()
            CSV2DataTable = True
        Catch ex As Exception
            If ex.Message.Contains("being used by another process") Then sFileInUseMessage = ex.Message
            '    MsgBox(String.Format("file {0} in use, try later", strFileName))
            'Else
            '    MsgBox(String.Format("Error {0} row {1}", strFileName, dlinecnt) & vbCrLf & ex.Message & vbCrLf & ex.StackTrace)
            'End If
            Debug.Print("")
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
        GetNewHdcp = ""
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
        Try
            Dim iLast5Scores As New List(Of String)
            Dim iPHdcp = 0
            Dim sMethod As String = ""
            Dim sScore = ""

            '20191216-check for incomplete scores so we can populate toolstrip only
            Dim bincompletescore As Boolean = False
            If CDate(sDate).ToString("yyyyMMdd") < CDate(rLeagueParmrow("PostseasonDt")).ToString("yyyyMMdd") Then
                For Each hole As DataGridViewCell In row.Cells
                    If hole.OwningColumn.Name.StartsWith("Hole") Then
                        '2019-01-26-check if were only 9 holes
                        If hole.OwningColumn.Name.Replace("Hole", "") >= iHoleMarker + iHoles Then Exit For
                        If hole.OwningColumn.Name.Replace("Hole", "") < iHoleMarker Then Continue For
                        If Not (IsNumeric(hole.Value)) Then
                            bincompletescore = True
                            Exit For
                        End If
                    ElseIf hole.OwningColumn.Name.Contains("Gross") Then
                        If hole.Value IsNot DBNull.Value Then
                            sScore = hole.Value
                        End If
                    End If
                Next
            Else
                bincompletescore = True
            End If

            If Not bincompletescore Then
                Dim iPar = thisCourse("In")
                If iHoleMarker = 1 Then iPar = thisCourse("Out")
                iLast5Scores.Add(sScore & "-" & iPar)
                '20180923-allow for combined club championship and league play dont adjust handicaps for club champ or league play
                If bCCLeague Then
                    GetNewHdcp = row.Cells("pHdcp").Value
                    Exit Function
                End If
            End If
            'dates past the regular season dont get included in handicap calculations
            Dim lignoreDates = New List(Of String)
            For Each lparm As DataRow In dsLeague.Tables("dtLeagueParms").Rows
                lignoreDates.Add(CDate(lparm("PostSeasonDt")).ToString("yyyyMMdd"))
                lignoreDates.Add(CDate(lparm("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd"))
            Next

            Dim dvScores As New DataView(dsLeague.Tables("dtScores"))
            dvScores.RowFilter = String.Format("Player = '{0}' and Date < '{1}' and Method <> '' and date not in ('{2}')", row.Cells("Player").Value, CDate(sDate).ToString("yyyyMMdd"), String.Join("','", lignoreDates))
            dvScores.Sort = "Date desc"
            '2020-01-26-always force hdcp to be hdcp of most recent score
            If dvScores.Count = 0 Then
                IHdcp = 99
            Else
                IHdcp = dvScores(0)("Hdcp")
            End If
            'this compensates for lost scores after week 1 in 2017, i have hardcoded the prev handicap on 4/11 so we dont have to go back
            'If sDate > "20170411" Then
            'dvScores.RowFilter = dvScores.RowFilter & " and Date >= '20170411'"
            'End If
            'build a total of 5 max scores in our array
            Dim iScore As Integer = 0
            For Each score As DataRowView In dvScores
                sMethod = convDBNulltoSpaces(score("Method"))
                If sMethod = "" Then Continue For
                sPlayer = score("Player").ToString
                iPHdcp = score("PHdcp").ToString
                iScore = 0
                If sMethod = "Net" Then iScore = iPHdcp
                If iHoles = 9 Then
                    Try
                        'this put in cause Ben Wright got bit by a dog on #11 on 20180529 and had to leave in the middle of his round
                        If score("Out_Gross") Is DBNull.Value And score("In_Gross") Is DBNull.Value Then
                            Continue For
                        End If
                        iScore += If(score("Out_Gross") Is DBNull.Value, score("In_Gross"), score("Out_Gross"))
                    Catch ex As Exception

                    End Try
                Else
                    iScore += score("18_Gross")
                End If
                Dim iPar = thisCourse("In")
                If score("In_Gross") Is DBNull.Value Then iPar = thisCourse("Out")
                iLast5Scores.Add(iScore & "-" & iPar)
                If iLast5Scores.Count = 5 Then Exit For
                ''dont recalculate handicap for 4/11, scorebook was lost
                'If sScoreDate = "20170411" Then
                '    iPHdcp = GetHdcp(iLast5Scores, iRoundctr, score("Date").ToString)
                'End If
            Next

            row.Cells("Hdcp").ToolTipText = ""
            For Each score In iLast5Scores
                row.Cells("Hdcp").ToolTipText = row.Cells("Hdcp").ToolTipText & score.Split("-")(0) & "-"
            Next
            row.Cells("Hdcp").ToolTipText = row.Cells("Hdcp").ToolTipText.Trim("-")
            If iLast5Scores.Count = 5 Then
                iLast5Scores.Remove(iLast5Scores.Min)
                iLast5Scores.Remove(iLast5Scores.Max)
            ElseIf iLast5Scores.Count = 4 Then
                iLast5Scores.Remove(iLast5Scores.Max)
            End If
            Dim iScoreTot = 0, iParTot = 0
            For Each score In iLast5Scores
                iScoreTot += score.Split("-")(0)
                iParTot += score.Split("-")(1)
            Next

            IHdcp = IIf(iLast5Scores.Count = 0, 99, ((iScoreTot / iLast5Scores.Count) - (iParTot / iLast5Scores.Count)) * 0.8)
            Return IHdcp

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    'Function GetHdcp(ByRef ilast5Scores As List(Of String), iRoundctr As String, sDate As String) As String
    '    GetHdcp = ""
    '    Try
    '        Dim iPlayerHdcp = 0
    '        Dim iPhdcp = 0
    '        Dim iloscore = 999
    '        Dim ilopar = 999
    '        Dim ihiscore = 0
    '        Dim ihipar = 0
    '        Dim iApr11Hdcp = 99
    '        If iRoundctr > 5 Then
    '            ilast5Scores = ilast5Scores.Take(0).Concat(ilast5Scores.Skip(1)).ToList
    '            ihiscore = 0
    '            iloscore = 999
    '        End If
    '        'calculate hi and low for drop
    '        For Each iscore As String In ilast5Scores
    '            'is this the low score?
    '            If iscore.Split("-")(0) < iloscore Then
    '                iloscore = iscore
    '            End If
    '            If iscore.Split("-")(0) > ihiscore Then
    '                ihiscore = iscore
    '            End If
    '        Next

    '        Dim itotScores = 0
    '        Dim blowdropped = False
    '        Dim bhidropped = False
    '        Dim iPar As Integer = 0
    '        Dim iKeptScores As New List(Of Decimal)
    '        'drop the high and low scores
    '        For Each iscore As Integer In ilast5Scores
    '            If ilast5Scores.Count = 5 Then
    '                If iloscore <> 999 Then
    '                    If iscore = iloscore Then
    '                        If Not blowdropped Then
    '                            blowdropped = True
    '                            Continue For
    '                        End If
    '                    End If
    '                End If
    '            ElseIf ilast5Scores.Count >= 4 Then
    '                If ihiscore > 0 Then
    '                    If iscore = ihiscore Then
    '                        If Not bhidropped Then
    '                            bhidropped = True
    '                            Continue For
    '                        End If
    '                    End If
    '                End If
    '            End If
    '            iKeptScores.Add(iscore)
    '        Next

    '        'loop through to get the 3 most recent scores
    '        Dim ictr As Integer = 0
    '        Dim sbScoresKept As New StringBuilder
    '        For Each iscore As Integer In iKeptScores
    '            sbScoresKept.Append(iscore & "-")
    '            itotScores += iscore
    '            ictr += 1
    '            'calc course par
    '            Dim MyCourse() As Data.DataRow
    '            Dim scourse = rLeagueParmrow("Course")
    '            'iHoles = rLeagueParmrow("Holes")
    '            MyCourse = dsLeague.Tables("dtCourses").Select("Name = '" & scourse & "'")
    '            Dim iCoursePar = 0
    '            'accumulate par for each score
    '            For i As Integer = 1 To iHoles
    '                iCoursePar += MyCourse(0)("Hole" & i).ToString
    '            Next
    '            iPar += iCoursePar
    '        Next

    '        'this compensates for lost scores of week 1 in 2017
    '        If ictr = 1 And iApr11Hdcp <> 99 Then
    '            iPlayerHdcp = iApr11Hdcp
    '            iPhdcp = iPlayerHdcp
    '        Else
    '            iPlayerHdcp = Math.Round((itotScores - iPar) / ictr * 0.8)
    '        End If

    '        LOGIT("Date - " & sDate & " Player - " & sPlayer.PadRight(25) & " Prv Handicap " & iPhdcp.ToString.PadRight(2) & " Handicap " & iPlayerHdcp.ToString.PadRight(2) & " All Scores - (" & String.Join("-", ilast5Scores) & ") Scores Kept (" & String.Join("-", sbScoresKept.ToString) & ") High Score - " & ihiscore & " Low Score - " & iloscore)

    '        Return iPlayerHdcp
    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Function
    Public Sub LOGIT(ByVal sMess As String)
        Try

            If bloghelper Then
                If Debugger.IsAttached Then
                    Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss | ") & sMess)
                    Exit Sub
                End If
                If Not Directory.Exists(sFilePath & "\Logs\") Then Directory.CreateDirectory(sFilePath & "\Logs\")

                Using swLog As New StreamWriter(sFilePath & "\Logs\" & My.Computer.Name & "_" & sLeagueName & "_" & DateTime.Now.ToString("yyyyMMdd_HH") & ".log", True)
                    For Each stmpmess As String In sMess.Split(CChar(vbCrLf))
                        swLog.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss | ") & stmpmess.Replace(vbCr, "").Replace(vbLf, ""))
                    Next
                    swLog.Close()
                End Using
            End If
        Catch ex As Exception
            'MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub CalcThisHoleMarker(sdate As String, dv As DataView)
        iHoleMarker = 0
        For Each row As DataRowView In dv
            If IsNumeric(row("Hole1")) Then
                iHoleMarker = 1
                Exit Sub
            End If
            If IsNumeric(row("Hole10")) Then
                iHoleMarker = 10
                Exit Sub
            End If
        Next
        CalcHoleMarker(sdate)
    End Sub
    Sub CalcHoleMarker(sDate As String)
        Try
            Dim sStartMonth As Int16 = 0
            For Each r As DataRow In dsLeague.Tables("dtLeagueParms").Rows
                If CDate(r("StartDate")).Year = sDate.Substring(0, 4) Then
                    sStartMonth = CDate(r("StartDate")).Month
                    If r("Start9") = "F" Then
                        iHoleMarker = 1
                    Else
                        iHoleMarker = 10
                    End If
                    Exit For
                End If
            Next
            Dim scurrMonth As Int16 = CInt(sDate.Substring(4, 2))
            Do While sStartMonth < scurrMonth
                If iHoleMarker = 10 Then
                    iHoleMarker = 1
                Else
                    iHoleMarker = 10
                End If
                sStartMonth += 1
            Loop

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

                'oHelper.LOGIT(row("Player") & "-" & row("Out_Gross") & "-" & row("In_Gross") & "-" & "icnt-" & icnt)
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
                        'oHelper.LOGIT("Score added " & row("Out_Gross"))
                        lvRec.SubItems.Add(row("Out_Gross") & "-" & row("Hdcp"))
                    Else
                        'oHelper.LOGIT("Score added " & row("In_Gross"))
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
                    Player.ShowDialog()
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
                For Each sname In aMisPronouncedPlayers
                    If sNameInfo = sname.Split(":")(0).ToUpper Then
                        sRowFilter = "Name like '" & sname.Split(":")(1) & "%'"
                        dvPlayers.RowFilter = sRowFilter
                        If dvPlayers.Count > 0 Then
                            Exit For
                        End If
                    End If
                Next
            End If

            'try using the 2 char to lookup name by initials 
            If dvPlayers.Count = 0 Then
                If sNameInfo.Length = 2 Then sRowFilter = "Name like '" & sNameInfo & "%'"
                dvPlayers.RowFilter = sRowFilter
            End If

            'dvplayers has a players matching initials
            If dvPlayers.Count = 0 Then
                If dgv.Name = "DtPlayersDataGridView" Then
                    fGetPlayer = fixPlayer(sNameInfo & ":New")
                    Exit Function
                End If
                Dim sResult As MsgBoxResult
                sResult = MsgBox("Player not found " & sNameInfo & vbCrLf & " Do you want to create a player?", MsgBoxStyle.YesNo)
                If sResult = MsgBoxResult.Yes Then
                    'Player.ShowDialog()
                    fGetPlayer = sPlayer
                    Exit Function
                Else
                    sResult = MsgBox("do you want to find a player in the player file?", MsgBoxStyle.YesNo)
                    If sResult = MsgBoxResult.Yes Then
                        sFindPlayerOption = "Not Found"
                        sPlayerToFind = sNameInfo
                        Player.ShowDialog()
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
            LOGIT(sPlayer & " hole " & cell.OwningColumn.Name & " has s/o on")
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
            LOGIT(sPlayer & " hole " & cell.OwningColumn.Name & " has s/o on")
        End If

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
    Sub CalcMatches(dg As DataGridView)
        For i = 0 To dg.Rows.Count - 1 Step 4
            getMatchPts(dg, i)
        Next
    End Sub
    Sub getMatchPts(dg As DataGridView, index As Integer)
        Try

            Dim ipNet = 0
            Dim ioNet = 0
            '20180325
            Dim s9Played As String = "Out_Net"
            If iHoleMarker <> 1 Then s9Played = "In_Net"
            ipNet = FixNullScore(dg.Rows(index + 0).Cells(s9Played).Value.ToString)
            ioNet = FixNullScore(dg.Rows(index + 2).Cells(s9Played).Value.ToString)
            '20180325-bye opponent
            Dim xxx = Matches.sByeOpponent
            If Matches.sByeOpponent = dg.Rows(index + 0).Cells("Team").Value Then
                dg.Rows(index + 0).Cells("Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 0).Cells("Points").Value = 1
                dg.Rows(index + 0).Cells("Team_Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 0).Cells("Team_Points").Value = 1
                dg.Rows(index + 0).Cells("Opponent").Value = "Bye"

                dg.Rows(index + 2).Cells("Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 2).Cells("Points").Value = 1
                dg.Rows(index + 2).Cells("Team_Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 2).Cells("Opponent").Value = "Bye"
                bByeFound = True
                Exit Sub
            End If
            sPlayer = dg.Rows(index).Cells("Player").Value
            ColorWinners(dg, index, ipNet, ioNet)
            ipNet = FixNullScore(dg.Rows(index + 1).Cells(s9Played).Value.ToString)
            ioNet = FixNullScore(dg.Rows(index + 3).Cells(s9Played).Value.ToString)
            ColorWinners(dg, index + 1, ipNet, ioNet)
            ipNet += FixNullScore(dg.Rows(index + 0).Cells(s9Played).Value.ToString)
            ioNet += FixNullScore(dg.Rows(index + 2).Cells(s9Played).Value.ToString)
            If ipNet > ioNet Then
                dg.Rows(index + 2).Cells("Team_Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 2).Cells("Team_Points").Value = 1
                dg.Rows(index + 0).Cells("Team_Points").Style.BackColor = dg.Rows(index + 0).Cells("Method").Style.BackColor
                dg.Rows(index + 0).Cells("Team_Points").Value = 0
            ElseIf ipNet < ioNet Then
                dg.Rows(index + 0).Cells("Team_Points").Style.BackColor = Color.LightGreen
                dg.Rows(index + 0).Cells("Team_Points").Value = 1
                dg.Rows(index + 2).Cells("Team_Points").Style.BackColor = dg.Rows(index + 2).Cells("Method").Style.BackColor
                dg.Rows(index + 2).Cells("Team_Points").Value = 0
            Else
                dg.Rows(index + 0).Cells("Team_Points").Style.BackColor = Color.Yellow
                dg.Rows(index + 2).Cells("Team_Points").Style.BackColor = Color.Yellow
                dg.Rows(index + 0).Cells("Team_Points").Value = 0.5
                dg.Rows(index + 2).Cells("Team_Points").Value = 0.5
            End If
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    'ip = player points, oo = opponents points
    Sub ColorWinners(dg As DataGridView, index As Integer, ipNet As Int16, ioNet As Int16)
        If ipNet > ioNet Then
            dg.Rows(index + 2).Cells("Points").Style.BackColor = Color.LightGreen
            dg.Rows(index + 2).Cells("Points").Value = 1
            dg.Rows(index + 2).Cells("Opponent").Style.BackColor = dg.Rows(index + 2).Cells("Method").Style.BackColor
            dg.Rows(index + 0).Cells("Points").Value = 0
            dg.Rows(index + 0).Cells("Opponent").Style.BackColor = Color.LightGreen
            dg.Rows(index + 0).Cells("Points").Style.BackColor = dg.Rows(index + 0).Cells("Method").Style.BackColor
        ElseIf ipNet < ioNet Then
            dg.Rows(index + 0).Cells("Points").Style.BackColor = Color.LightGreen
            dg.Rows(index + 0).Cells("Points").Value = 1
            dg.Rows(index + 0).Cells("Opponent").Style.BackColor = dg.Rows(index + 0).Cells("Method").Style.BackColor
            dg.Rows(index + 2).Cells("Points").Value = 0
            dg.Rows(index + 2).Cells("Opponent").Style.BackColor = Color.LightGreen
            dg.Rows(index + 2).Cells("Points").Style.BackColor = dg.Rows(index + 2).Cells("Method").Style.BackColor
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
    Function Wait30secs(dt, sfile) As Boolean
        Wait30secs = False
        Dim i = 30
        Do Until i = 0
            If CSV2DataTable(dt, sfile) Then
                Wait30secs = True
                Exit Function
            End If
            sMessage = String.Format("File {0} is in use, will wait up for {1} seconds to free up", sfile, i)
            Popup.Text = "File in Use"
            'CreateObject("WScript.Shell").Popup(sMsg, 5, "File in Use")
            Popup.ShowDialog()
            Threading.Thread.Sleep(1000)
            i -= 1
        Loop
    End Function
    Function buildSchedule() As DataTable
        buildSchedule = Nothing
        Try
            Dim sfilename As String = Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) &
                               "_" & Main.cbLeagues.SelectedItem.ToString.Substring(0, Main.cbLeagues.SelectedItem.ToString.IndexOf("(") - 1) & "_Schedule.csv"

            Dim dt = New DataTable
            Dim sfile As String = sFilePath & "\" & sfilename
            Do Until Wait30secs(dt, sfile)
                'sMessage = String.Format("File {0} is in use, will wait up for 30 seconds to free up", sfile)
                'Popup.Text = "File in Use"
                ''CreateObject("WScript.Shell").Popup(sMsg, 5, "File in Use")
                'Popup.ShowDialog()
                'Threading.Thread.Sleep(1000)
            Loop

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
            ''get each match from the column
            For Each col As DataColumn In dt.Columns
                Dim aRow As DataRow
                aRow = dtSchedule.NewRow
                'set the match number
                Dim iMatch = 1
                'Loop thru each row and pull the match for that day indexed by the column counter
                For Each irow As DataRow In dt.Rows
                    'if were past the row for max matches exit
                    If iMatch > iMatches Then Exit For

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
                    dtSchedule.Rows.Add(aRow)
                End If
                icolCounter += 1
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
                    If sCorScore <> "" Then R.Value = sCorScore
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
                r.Cells(col).Style.BackColor = UnderParColor
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
                If CDate(dDate).ToString("yyyyMMdd") < CDate(rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
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
                    If sTeam = "" Then R.Cells(sColName).Style.BackColor = Color.Aqua
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
                            If iScore Is DBNull.Value Then Continue For
                            'if the handicap > stroke index adjust net score to gross
                            If R.Cells("Method").Value = "Net" Then
                                Dim isi = CalcStrokeIndex(sColName)
                                If R.Cells("pHdcp").Value >= isi Then
                                    'check stroke index
                                    iScore += 1
                                    If R.Cells("pHdcp").Value - iHoles >= isi Then iScore += 1
                                End If
                            End If
                            If iScore <> "" Then
                                cell.Value = iScore
                                SBPMarkSubPar(cell, iScore, MyCourse(0)(sColName).ToString)
                            End If
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
    Function ChkForMax(sScore As Decimal, sHole As String) As Decimal
        ChkForMax = sScore
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
        getSubstring = sText.Substring(sText.LastIndexOf(sPoint) + 1, sText.IndexOf(sEndPoint) - sText.LastIndexOf(sPoint) - 1)

    End Function
    Function getLatestFile(sFile As String) As String
        getLatestFile = ""
        If Not Directory.Exists(sFilePath) Then
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
        For Each file In oFiles
            'If file.Name.Substring(0, 4) <= sYear Then
            getLatestFile = file.FullName
            Exit For
            'End If
        Next

    End Function
    Sub MakeCellsStrings(row As DataGridViewRow)
        'this forces each cell to be string prevent errors on resorting columns
        For Each cell As DataGridViewCell In row.Cells
            If cell.Value Is DBNull.Value Then cell.Value = ""
            If cell.FormattedValueType.Name = "String" Then cell.Value = CStr(cell.Value)

            'oHelper.LOGIT(cell.OwningColumn.Name & "-" & cell.Value & "-" & cell.FormattedValueType.Name)
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
        Try
            Dim ip# = 0
            Dim rSch As DataRow = dsLeague.Tables("dtSchedule").Rows.Find(sdate)
            'this if statement handles post season
            If rSch Is Nothing Or sdate > CDate(rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
                For iTeam = 1 To CInt(rLeagueParmrow("Teams"))
                    sTeam = iTeam
                    'save this players Partner
                    sPlayer = getPlayer(sTeam, "A") & ",A"
                    ip# = UpdatePartnerInScore(sPlayer, "A", sdate, ip#)
                    sPlayer = getPlayer(sTeam, "B") & ",B"
                    ip# = UpdatePartnerInScore(sPlayer, "B", sdate, ip#)
                Next
                getMatchScores = True
                Exit Function
            End If
            'regular season
            Dim sOPlayer As String = ""
            If sdate <= CDate(rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
                Dim imatches As Int16 = rLeagueParmrow("Teams") / 2
                For iMatch = 1 To imatches
                    Dim sMatch = rSch(iMatch.ToString).ToString
                    'right side of match
                    sTeam = sMatch.Split("v")(1)
                    If sTeam <> "Bye" Then
                        'ip# is the index of dgscores grid row
                        sPlayer = getPlayer(sTeam, "A") & ",A"
                        sOPlayer = getPlayer(sMatch.Split("v")(0), "A") & ",A"
                        ip# = UpdatePartnerInScore(sPlayer, sOPlayer, sdate, ip#)
                        sPlayer = getPlayer(sTeam, "B") & ",B"
                        sOPlayer = getPlayer(sMatch.Split("v")(0), "B") & ",B"
                        ip# = UpdatePartnerInScore(sPlayer, sOPlayer, sdate, ip#)
                    End If
                    'left side of match
                    sTeam = sMatch.Split("v")(0)
                    If sTeam <> "Bye" Then
                        sPlayer = getPlayer(sTeam, "A") & ",A"
                        sOPlayer = getPlayer(sMatch.Split("v")(1), "A") & ",A"
                        ip# = UpdatePartnerInScore(sPlayer, sOPlayer, sdate, ip#)
                        sPlayer = getPlayer(sTeam, "B") & ",B"
                        sOPlayer = getPlayer(sMatch.Split("v")(1), "B") & ",B"
                        ip# = UpdatePartnerInScore(sPlayer, sOPlayer, sdate, ip#)
                    End If
                Next
            End If
            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
            dvscores.Sort = "Date,Partner"
            'mark as good scores
            getMatchScores = True
        Catch ex As Exception
            Dim x = ""
        End Try
    End Function
    Private Function getPlayer(sTeam, sGrade) As String

        getPlayer = ""
        Dim sPlayersRowFilter = String.Format("Team ='{0}' AND Grade = '{1}' AND Date = '{2}'", sTeam, sGrade, dDate.ToString("yyyyMMdd"))
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

    End Function
    Private Function UpdatePartnerInScore(sthisPlayer As String, sOpponent As String, sDate As String, ip As Short) As Int16
        Dim sKey() As Object = {sthisPlayer.Split(",")(0), sDate}
        Dim drow = dsLeague.Tables("dtScores").Rows.Find(sKey)
        Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
        'drow is nothing if this is a new score
        If drow Is Nothing Then
            Dim rowView As DataRowView = dvscores.AddNew
            ' Change values in the DataRow.
            rowView("League") = sLeagueName
            rowView("Player") = sthisPlayer.Split(",")(0)
            rowView("Grade") = sthisPlayer.Split(",")(1)
            rowView("Group") = 0
            rowView("Team") = sTeam
            rowView("Date") = sDate
            rowView("Partner") = CStr(ip).PadLeft(2, "0")
            rowView("Opponent") = sOpponent
            rowView.EndEdit()
            ''this finds the players partner
            'Dim srowfilter = String.Format("Player <> '{0}' AND Date = '{1}' AND Team = '{2}'", sthisPlayer, sDate, sTeam)
            'dvscores.RowFilter = srowfilter
            'If dvscores.Count > 0 Then
            '    dvscores(0)("Partner") = CStr(ip).PadLeft(2, "0")
            '    dvscores(0)("Opponent") = sOpponent
            'End If
        Else
            drow("Partner") = CStr(ip).PadLeft(2, "0")
            drow("Opponent") = sOpponent
            drow.EndEdit()
        End If
        UpdatePartnerInScore = ip + 1
    End Function
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
    Function FCalcLowScore(dvScores As DataView, hole As String) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        FCalcLowScore = ""
        Try
            Dim ilowrow As New List(Of String)
            Dim ilowscore = 99
            'calculate a column saving low score
            For i = 0 To dvScores.Count - 1
                Dim sGross As Object = Nothing
                Dim sNet As Object = Nothing
                Dim x = dvScores(i)(Constants.Player)
                If dvScores(i)(Constants.Player) = sTotalColumn Then Continue For
                sPlayer = dvScores(i)(Constants.Player)
                If IsNumeric(dvScores(i)("PHdcp")) Then
                    IHdcp = dvScores(i)("PHdcp")
                Else
                    IHdcp = sGross - sNet
                    dvScores(i)("PHdcp") = IHdcp
                End If
                Dim y = dvScores(i)(skin)
                If sIn(dvScores(i)(skin).ToString.ToUpper, "Y,TRUE", True) Then
                    'If dgScores.Rows(i).Cells("Hole" & hole).Value IsNot DBNull.Value Then
                    If IsNumeric(dvScores(i)("Hole" & hole)) Then
                        Dim iscore As String = dvScores(i)("Hole" & hole)
                        'this means its a scorecard
                        '2020-01-15- 20180529 Ben Wright played 1 hole
                        If dvScores(i)("Method") = "Gross" And rLeagueParmrow("SkinFmt") = "Handicap" Then
                            Dim isi As Int16 = CalcStrokeIndex(hole)
                            If CInt(dvScores(i)("pHdcp")) >= isi Then
                                'check stroke index
                                iscore -= 1
                                If CInt(dvScores(i)("pHdcp") - iHoles) >= isi Then iscore -= 1
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

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Function FCalcSkins(dvScores As DataView) As List(Of String)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'this code goes through the listview and highlights the lowest value on each hole and fron 9, back 9 and total
        FCalcSkins = New List(Of String)
        Try
            For ii = iHoleMarker To iHoleMarker + iHoles - 1
                Dim ilowscore As String = FCalcLowScore(dvScores, ii)
                If ilowscore <> "" Then FCalcSkins.Add(ii & "-" & ilowscore)
            Next
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function
    'Function FCalcSkins(dgScores As DataGridView) As List(Of String)
    '    LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    'this code goes through the gridview and highlights the lowest value on each hole and fron 9, back 9 and total
    '    FCalcSkins = Nothing
    '    Try
    '        FCalcSkins = New List(Of String)
    '        Dim ilowrow As New List(Of String)
    '        'adjust for handicap fields in listview
    '        'loop through each column finding the lowest scores
    '        'Get low 9's and 18 hole
    '        '20171014 - use holemarker to control which 9 or 18 you process
    '        For ii = iHoleMarker To iHoleMarker + iHoles - 1 'lv1.Items(0).SubItems.Count - 1
    '            Dim ilowscore = 99
    '            'calculate a column saving low score
    '            For i = 0 To dgScores.RowCount - 1
    '                If dgScores.Rows(i).Cells("Player").Value = sTotalColumn Then Continue For
    '                sPlayer = dgScores.Rows(i).Cells("Player").Value
    '                If dgScores.Rows(i).Cells("PHdcp").Value.ToString <> "" Then
    '                    IHdcp = dgScores.Rows(i).Cells("PHdcp").Value
    '                Else
    '                    If iHoleMarker = 1 Then
    '                        IHdcp = dgScores.Rows(i).Cells("Out_Gross").Value - dgScores.Rows(i).Cells("Out_Net").Value
    '                    Else
    '                        IHdcp = dgScores.Rows(i).Cells("In_Gross").Value - dgScores.Rows(i).Cells("In_Net").Value
    '                    End If
    '                    dgScores.Rows(i).Cells("PHdcp").Value = IHdcp
    '                End If
    '                If dgScores.Rows(i).Cells("Skins").Value = "Y" Then
    '                    If dgScores.Rows(i).Cells("Hole" & ii).Value IsNot DBNull.Value Then
    '                        Dim iscore As String = RemoveSpcChar(dgScores.Rows(i).Cells("Hole" & ii).Value)
    '                        If IsNumeric(iscore) Then
    '                            If iscore < ilowscore Then
    '                                ilowscore = iscore
    '                                ilowrow = New List(Of String)
    '                                ilowrow.Add(i)
    '                            ElseIf iscore = ilowscore Then
    '                                ilowrow.Add(i)
    '                            End If
    '                        End If
    '                    End If
    '                End If
    '            Next

    '            If ilowrow.Count = 1 Then
    '                Dim score As Integer = ilowrow(0)
    '                dgScores.Rows(score).Cells("Hole" & ii).Style.BackColor = Color.Gold
    '                dgScores.Rows(score).Cells("Player").Style.BackColor = Color.Gold
    '                dgScores.Rows(score).Cells("$Skins").Style.BackColor = Color.Gold
    '                FCalcSkins.Add(score)
    '            Else
    '                'Dim myFont As New Font("BahnSchrift Condensed", 12, FontStyle.Strikeout)
    '                Dim myFont As New Font("Britannica Bold", 12, FontStyle.Strikeout)
    '                'Dim myFont As New Font("Tahoma", 12, FontStyle.Strikeout)
    '                For Each player In ilowrow
    '                    dgScores.Rows(player).Cells("Hole" & ii).Style.Font = myFont
    '                    dgScores.Rows(player).Cells("Hole" & ii).Style.BackColor = Color.Yellow
    '                    '20190630 number for doesnt show as strikethrough
    '                    'If dgScores.Rows(player).Cells("Hole" & ii).Value = "4" Then
    '                    '    dgScores.Rows(player).Cells("Hole" & ii).Value = "4"
    '                    'End If
    '                    'If dgScores.Rows(player).Cells("Hole" & ii).Style.Font.Strikeout Then
    '                    '    oHelper.LOGIT("Strikeout true")
    '                    'Else
    '                    '    oHelper.LOGIT("Strikeout false")
    '                    'End If
    '                    LOGIT(String.Format("setting skins tie for hole {0} {1}", ii, dgScores.Rows(player).Cells("Player").Value))
    '                Next
    '            End If
    '        Next
    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try

    'End Function
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
    Sub CopyDataGridViewToClipboard(ByRef dgv As DataGridView)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
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
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        dgv.SelectAll()
        Dim dataob As DataObject = dgv.GetClipboardContent
        System.IO.File.WriteAllText(filename, dataob.GetText(TextDataFormat.CommaSeparatedValue))
    End Sub
    Sub dgv2csv(dgv As DataGridView, filename As String)
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

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

    Function WaitForFile(dt As DataTable, sFile As String, lbstatus As Label, frm As Form) As Boolean
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try

            WaitForFile = False
            If CSV2DataTable(dt, sFile) Then
                WaitForFile = True
                'If sFile.Contains("LeagueParm") Then
                '    If dDate.ToString("MM/dd/yyyy") < "20190101" Then
                '        If Not dt.Columns.Contains("PostSeasonDt") Then
                '            dt.Columns("PostSeason").ColumnName = "PostSeasonDt"
                '            For Each col In dt.Rows
                '                col("PostSeasonDt") = "09/18/2018"
                '            Next
                '        End If
                '    End If
                'End If
            Else
                Dim processes As Process() = Process.GetProcesses
                Dim i As Integer
                Dim xxdsds = processes.GetUpperBound(0) - 1
                For i = 0 To processes.GetUpperBound(0) - 1
                    myProcess = processes(i)
                    'if module contains an asterisk in notepadm, its been updated
                    Debug.Print(myProcess.ProcessName & "-" & myProcess.Id & "-" & myProcess.MainWindowTitle)
                    Dim yxyxy = sFile.Substring(sFile.LastIndexOf("\") + 1)
                    If myProcess.MainWindowTitle.Contains(sFile.Substring(sFile.LastIndexOf("\") + 1)) Then
                        Debug.Print(String.Format(myProcess.MainWindowTitle))
                        For ii = 0 To 6
                            If CSV2DataTable(dt, sFile) Then
                                WaitForFile = True
                                Exit Function
                            End If

                            lbstatus.Text = String.Format("Waiting for {1} seconds for file {0}", sFile, ii)
                            status_Msg(lbstatus, frm)
                            Threading.Thread.Sleep(1000)
                            'MsgBox(String.Format("File {0} is in use, will wait up for {1} seconds to free up", sFile, i))
                            If ii = 0 Then
                                sMessage = String.Format("File {0} is in use, will wait up for {1} seconds to free up", sFile, 30)
                            Else
                                sMessage = String.Format("File {0} is in use, will wait up for {1} more seconds to free up", sFile, 30 - (ii * 5))
                            End If
                            Popup.Text = "File in Use"
                            'CreateObject("WScript.Shell").Popup(sMsg, 5, "File in Use")
                            If ii < 6 Then Popup.ShowDialog()
                        Next ii
                        Dim mbr = MessageBox.Show(String.Format("File {1} In use by {2}{0}Do you want to keep waiting while you close? Press <Yes> Or to Cancel this session, Press <No>", vbCrLf, sFile & ".csv", myProcess.MainWindowTitle.Split("-")(1)), "File In Use", MessageBoxButtons.YesNo)
                        If mbr = DialogResult.No Then
                            End
                        End If
                    Else
                        Continue For
                    End If
                Next i

                lbstatus.Text = String.Format("Finished Waiting for file {0}", sFile)
                status_Msg(lbstatus, frm)
                Exit Function
            End If

        Catch ex As Exception

        End Try
    End Function

    Private Function getFileProcesses(ByVal strFile As String) As ArrayList
        myprocessarray.Clear()
        Dim processes As Process() = Process.GetProcesses
        Dim i As Integer
        For i = 0 To processes.GetUpperBound(0) - 1
            myProcess = processes(i)
            'Debug.Print("")
            If myProcess.MainWindowTitle.Contains(strFile) Then
                Debug.Print(String.Format(myProcess.MainWindowTitle))
            Else
                Continue For
            End If
            Try
                Dim modules As ProcessModuleCollection = myProcess.Modules
                Dim j As Integer
                For j = 0 To modules.Count - 1

                    If (modules.Item(j).FileName.ToLower.CompareTo(strFile.ToLower) = 0) Then
                        myprocessarray.Add(myProcess)
                        Exit For
                    End If
                Next j
            Catch exception As Exception
                'MsgBox(("Error : " & exception.Message))
            End Try
            If Not myProcess.HasExited Then

            End If
        Next i

        Return myprocessarray
    End Function

    Function getLeagParm(sScoreDate As String, lbstatus As Label, frm As Form) As String 'parmfile,good(got the league file) or roskins:rocp1:rocp2
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        getLeagParm = ""
        Try
            Dim sParmFile As String = ""
            Dim oFiles() As IO.FileInfo
            Dim oDirectory As New IO.DirectoryInfo(sFilePath)
            oFiles = oDirectory.GetFiles("*LeagueParms.csv")
            For Each sfile In oFiles
                If sfile.Name.Substring(0, 8) <= sScoreDate Then
                    sParmFile = sfile.FullName
                Else
                    Exit For
                End If
            Next
            getLeagParm = sParmFile
            If getLeagParm = "" Then Exit Function

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
            '20190905-see if were postseason
            Dim sYear = "01-01-" & Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4)
            'foundrows = dtLeagueParm.Select(String.Format("#{0}# >= StartDate and #{0}# <=EndDate", dDate.ToString("MM/dd/yyyy")))
            foundrows = dtLeagueParm.Select(String.Format("StartDate >= #{0}# and #{1}# <= PostSeasonDt ", sYear, dDate.AddDays(-7).ToString("MM/dd/yyyy")))
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
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function
    Function sf(str As String, flist As String) As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        sf = ""
        Try
            Dim sfl As String
            For Each fld In flist.Split(",")
                sfl = fld
            Next

            sf = String.Format(str, "")
        Catch ex As Exception
            MsgBox(GetExceptionInfo(ex))
        End Try
    End Function
    Function ScreenResize() As String
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        ScreenResize = ScreenResize(Main.iScreenWidth, Main.iScreenHeight)
    End Function
    Function ScreenResize(sprefwidth, sprefheight) As String
        ScreenResize = ""
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
            SumAmts = Convert.ToInt32(dt.Compute(String.Format("SUM({0})", fld), String.Format("Date = {0} and {1} > 0", sdate, fld)))

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
    Sub newCalcLeftovers()
        Try

            'calculate leftoverskins and fill text boxes with this weeks amounts
            LOGIT(String.Format("calculate leftoverskins/ctps And fill text boxes with this weeks amounts"))

            If File.Exists("c:\temp\temp.txt") Then
                File.Delete("c:\temp\temp.txt")
            End If

            Dim sFrom = CDate(rLeagueParmrow("StartDate")).ToString("yyyyMMdd")
            Dim sTo = CDate(rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd")
            DataTable2CSV(dsLeague.Tables("dtScores"), "c:\temp\temp.txt")
            'Dim cn As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';")
            Dim cn As New OleDbConnection(String.Format("Provider={0};Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';", sMACDBVersion))
            'Dim dt As New DataTable("Skins")
            'Dim sWhere = String.Format("Where date >= {0} and date < {1} and [$Skins] > 0 Group By date", sFrom, sTo)
            Dim sWhere = String.Format("Where date >= {0} and date < {1} Group By date", sFrom, sTo)
            Dim x = tallysumif(skin, "TRUE,True", "SkinPlayers")
            Dim xx As String = String.Format("{0}", x)
            Debug.Print("")
            'https://stackoverflow.com/questions/22990229/count-iif-access-query
            Dim strsql As String = String.Format _
            ("SELECT Date
            , COUNT(*) as Total
            , SUM(IIF(Skins = 'TRUE' or Skins = 'True', 1, 0)) as SkinPlayers
            , SUM(IIF(Closest = 'TRUE' or Closest = 'True', 1, 0)) as CTPPlayers
            , IIF(SUM(Hole1) > 0,'Front','Back') as FrontBack 

            , SUM([#Skins]) as wSkins
            , SUM([$Skins]) as wskinsearned 
            , IIF(wskinsearned > 0,wskinsearned,0) as skinsearned
            , (IIF(Date < {0}, {1}, 7)) * SUM(IIF(Skins = 'TRUE' OR Skins = 'True', 1, 0)) - IIF(SUM([$Skins]) > 0, SUM([$Skins]),0) as wskinsextra

            , IIF(SUM(Hole1) > 0,(IIF(Date < {0}, 1, 3)) * SUM(IIF(Closest = 'TRUE' OR Closest = 'True', 1, 0)) / 2, 0) as ctpf1collected 
            , IIF(SUM(Hole1) > 0,(IIF(Date < {0}, 1, 3)) * SUM(IIF(Closest = 'TRUE' OR Closest = 'True', 1, 0)) / 2, 0) as ctpf2collected 
            , IIF(SUM(Hole10) > 0,(IIF(Date < {0}, 1, 3)) * SUM(IIF(Closest = 'TRUE' OR Closest = 'True', 1, 0)) / 2, 0) as ctpb1collected 
            , IIF(SUM(Hole10) > 0,(IIF(Date < {0}, 1, 3)) * SUM(IIF(Closest = 'TRUE' OR Closest = 'True', 1, 0)) / 2, 0) as ctpb2collected 

            , IIF(SUM(Hole1) > 0,SUM([CTP_1]), 0) as wctpf1earned
            , IIF(SUM(Hole1) > 0,SUM([CTP_2]), 0) as wctpf2earned
            , IIF(SUM(Hole10) > 0,SUM([CTP_1]), 0) as wctpb1earned
            , IIF(SUM(Hole10) > 0,SUM([CTP_2]), 0) as wctpb2earned

            , IIF(wSkins > 0,wSkins,0) as Skins
            , (IIF(Date < {0}, {1}, 7)) * SUM(IIF(Skins = 'TRUE' OR Skins = 'True', 1, 0)) as skinscollected
            , IIF(wskinsextra > 0,wskinsextra,0) as skinsextra

            , IIF(wctpf1earned > 0, wctpf1earned,0) as ctpf1earned
            , ctpf1collected - ctpf1earned as ctpf1extra
            , IIF(wctpf2earned > 0, wctpf2earned,0) as ctpf2earned
            , ctpf2collected - ctpf2earned as ctpf2extra

            , IIF(wctpb1earned > 0, wctpb1earned,0) as ctpb1earned
            , ctpb1collected - ctpb1earned as ctpb1extra
            , IIF(wctpb2earned > 0, wctpb2earned,0) as ctpb2earned
            , ctpb2collected - ctpb2earned as ctpb2extra

            FROM [temp.txt]",
             CDate(rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), rLeagueParmrow(skin)
)

            'strsql = String.Format _
            '("SELECT Date
            ', IIF(SUM([$Skins]) > 0,SUM([$Skins]),0) as SkinsExtra
            'FROM [temp.txt]",
            ' CDate(rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), rLeagueParmrow(skin)
            ')

            strsql += vbCrLf &
            String.Format("WHERE Date >= {0} And Date <= {1}", sFrom, sTo)
            strsql += vbCrLf & "GROUP BY Date"
            dtnewWklySkins = New DataTable("SkinsInfo")
            Dim da As New OleDbDataAdapter(strsql, cn)
            da.Fill(dtnewWklySkins)
            dtnewWklySkins.PrimaryKey = New DataColumn() {dtnewWklySkins.Columns("Date")}
            Dim wkctpb1co As Decimal = 0
            Dim wkctpb2co As Decimal = 0
            Dim wkctpf1co As Decimal = 0
            Dim wkctpf2co As Decimal = 0
            For Each row In dtnewWklySkins.Rows
                row(ctpb1extr) += wkctpb1co
                wkctpb1co = row(ctpb1extr)
                row(ctpb2extr) += wkctpb2co
                wkctpb2co = row(ctpb2extr)
                row(ctpf1extr) += wkctpf1co
                wkctpf1co = row(ctpf1extr)
                row(ctpf2extr) += wkctpf2co
                wkctpf2co = row(ctpf2extr)
            Next
            Dim drow = dtnewWklySkins.Rows(dtnewWklySkins.Rows.Count - 1)
            drow(ctpb1extr) += wkctpb1co
            wkctpb1co = drow(ctpb1extr)
            drow(ctpb2extr) += wkctpb2co
            wkctpb2co = drow(ctpb2extr)
            drow(ctpf1extr) += wkctpf1co
            wkctpf1co = drow(ctpf1extr)
            drow(ctpf2extr) += wkctpf2co
            wkctpf2co = drow(ctpf2extr)

            Dim dt1 As New DataTable("Points")
            Dim strsql1 As String = String.Format _
            (
                "SELECT Team
                , SUM(Points) as Points
                , SUM([Team_Points]) as TeamPoints 
                , SUM([Team_Points]) + SUM(Points) as TotalPoints 
                FROM [temp.txt] WHERE Date >= {0} and Date < {1} 
                GROUP BY Team",
                CDate(rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), CDate(rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
            )
            da = New OleDbDataAdapter(strsql1, cn)
            da.Fill(dt1)
            Debug.Print("")
        Catch ex As Exception

        End Try

    End Sub
    Sub CalcLeftOvers()
        Try
            newCalcLeftovers()
            dtWklySkins = New DataTable
            dtWklySkins.Columns.Add("Date")
            For Each fld As String In {skinscol, skinsearn, skinsextr, ctpf1col, ctpf1earn, ctpf1extr, ctpf2col, ctpf2earn, ctpf2extr, ctpb1col, ctpb1earn, ctpb1extr, ctpb2col, ctpb2earn, ctpb2extr, kitty}
                dtWklySkins.Columns.Add(fld, GetType(Decimal))
            Next
            dtWklySkins.PrimaryKey = New DataColumn() {dtWklySkins.Columns("Date")}

            Dim dvSkins = New DataView(dsLeague.Tables("dtScores"))
            'dvSkins.RowFilter = String.Format("Skins = 'Y' and Date >= '{0}' and Date <= '{1}'", sWorkingYear & "0101", oHelper.sDateLastScore)
            dvSkins.RowFilter = String.Format("(Skins = 'Y' or Skins = 'TRUE') and Date >= '{0}' and Date <= '{1}'", CDate(rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), CDate(dDate).ToString("yyyyMMdd"))
            dvSkins.Sort = "Date Asc"
            'Dim dt = dsLeague.Tables("dtScores")
            Dim dtSkins As DataTable = dvSkins.ToTable(True, "Player,Date,$Skins".Split(",").ToArray)
            Dim wkrow As DataRow = Nothing
            Dim sPDate As String = ""
            For Each row In dtSkins.Rows
                If row("Date") <> sPDate Then
                    If sPDate <> "" Then
                        wkrow(skinsextr) = wkrow(skinscol) - wkrow(skinsearn)
                    End If
                    wkrow = dtWklySkins.NewRow
                    wkrow("Date") = row("Date")
                    wkrow(skinscol) = 0
                    wkrow(skinsearn) = 0
                    wkrow(skinsextr) = 0
                    sPDate = row("Date")
                    dtWklySkins.Rows.Add(wkrow)
                End If
                If row("$Skins") IsNot DBNull.Value Then wkrow(skinsearn) += row("$Skins")
                wkrow(skinscol) += If(row("Date") < CDate(rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), rLeagueParmrow("Skins"), "7")
                LOGIT(String.Format("Date {0} Player {1}", row("Date"), row("Player")))
            Next
            'finish off last row
            wkrow = dtWklySkins.Rows(dtWklySkins.Rows.Count - 1)
            wkrow(skinsextr) = wkrow(skinscol) - wkrow(skinsearn)
            'get closests
            dvSkins = New DataView(dsLeague.Tables("dtScores"))
            dvSkins.RowFilter = String.Format("(Closest = 'Y' or Closest = 'TRUE') and Date >= '{0}' and Date <= '{1}'", CDate(rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), CDate(dDate).ToString("yyyyMMdd"))
            dvSkins.Sort = "Date Asc"
            'Dim dt = dsLeague.Tables("dtScores")
            dtSkins = dvSkins.ToTable(True, "Player,Date,Closest,Hole1,Hole10,CTP_1,CTP_2".Split(",").ToArray)
            wkrow = Nothing
            sPDate = ""
            Dim ctpvalue As Decimal
            For Each row In dtSkins.Rows
                If row("Date") <> sPDate Then
                    If sPDate <> "" Then
                        wkrow(ctpf1extr) = wkrow(ctpf1col) - wkrow(ctpf1earn)
                        wkrow(ctpf2extr) = wkrow(ctpf2col) - wkrow(ctpf2earn)
                        wkrow(ctpb1extr) = wkrow(ctpb1col) - wkrow(ctpb1earn)
                        wkrow(ctpb2extr) = wkrow(ctpb2col) - wkrow(ctpb2earn)
                    End If
                    wkrow = dtWklySkins.Rows.Find(row("Date"))
                    ctpvalue = If(row("Date") < CDate(rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), 1, "3")
                    For Each fld As String In {ctpf1col, ctpf1earn, ctpf1extr, ctpf2col, ctpf2earn, ctpf2extr, ctpb1col, ctpb1earn, ctpb1extr, ctpb2col, ctpb2earn, ctpb2extr, kitty}
                        wkrow(fld) = 0
                    Next
                    sPDate = row("Date")
                End If

                If row("Hole1") IsNot DBNull.Value Then
                    wkrow(ctpf1col) += ctpvalue / iNumClosests
                    wkrow(ctpf2col) += ctpvalue / iNumClosests
                    If row(CTP1) IsNot DBNull.Value Then wkrow(ctpf1earn) += row(CTP1)
                    If row(CTP2) IsNot DBNull.Value Then wkrow(ctpf2earn) += row(CTP2)
                ElseIf row("Hole10") IsNot DBNull.Value Then
                    wkrow(ctpb1col) += ctpvalue / iNumClosests
                    wkrow(ctpb2col) += ctpvalue / iNumClosests
                    If row(CTP1) IsNot DBNull.Value Then wkrow(ctpb1earn) += row(CTP1)
                    If row(CTP2) IsNot DBNull.Value Then wkrow(ctpb2earn) += row(CTP2)
                End If
                LOGIT(String.Format("Date {0} Player {1}", row("Date"), row("Player")))
            Next
            'get this weeks amounts
            Dim arow As DataRow = dtWklySkins.Rows.Find(CDate(dDate).ToString("yyyyMMdd"))
            For Each fld As String In {skinscol, skinsearn, skinsextr, ctpf1col, ctpf1earn, ctpf1extr, ctpf2col, ctpf2earn, ctpf2extr, ctpb1col, ctpb1earn, ctpb1extr, ctpb2col, ctpb2earn, ctpb2extr, kitty}
                arow(fld) = 0
            Next

            Exit Sub
            '2020-01-25 - removed not needed
            dLastWeeksSkins = 0
            dLastWeeksCTPF1 = 0
            dLastWeeksCTPF2 = 0
            dLastWeeksCTPB1 = 0
            dLastWeeksCTPB2 = 0

            dThisWeeksSkins = 0
            dThisWeeksCTPF1 = 0
            dThisWeeksCTPF2 = 0
            dThisWeeksCTPB1 = 0
            dThisWeeksCTPB2 = 0

            dExtraSkins = 0
            dExtraCTPF1 = 0
            dExtraCTPF2 = 0
            dExtraCTPB1 = 0
            dExtraCTPB2 = 0

            'get this weeks amounts
            'Dim arow As DataRow = dtWklySkins.Rows.Find(CDate(dDate).ToString("yyyyMMdd"))
            'If arow IsNot Nothing Then
            '    dThisWeeksSkins = arow(skinscol)
            '    dThisWeeksCTPF1 = arow(ctpf1col)
            '    dThisWeeksCTPF2 = arow(ctpf2col)
            '    dThisWeeksCTPB1 = arow(ctpb1col)
            '    dThisWeeksCTPB2 = arow(ctpb2col)
            '    dExtraSkins = arow(skinsextr)
            '    dExtraCTPF1 = arow(ctpf1extr)
            '    dExtraCTPF2 = arow(ctpf2extr)
            '    dExtraCTPB1 = arow(ctpb1extr)
            '    dExtraCTPB2 = arow(ctpb2extr)
            'End If
            'if this is the first date of the season, dont do last weeks amounts

            Dim pseldate = ScoreCard.cbDates.Items(ScoreCard.cbDates.SelectedIndex + 1)
            Dim prow = dtWklySkins.Rows.Find(ScoreCard.cbDates.Items(ScoreCard.cbDates.SelectedIndex + 1))
            If prow IsNot Nothing Then
                dLastWeeksSkins = prow(skinsextr)
                dLastWeeksCTPF1 = prow(ctpf1extr)
                dLastWeeksCTPF2 = prow(ctpf2extr)
                dLastWeeksCTPB1 = prow(ctpb1extr)
                dLastWeeksCTPB2 = prow(ctpb2extr)
            End If
            Dim x = ""
        Catch ex As Exception

        End Try
    End Sub
    Public Sub Resizedgv(dgv As DataGridView, frm As Form)
        Dim iw As Integer = 0, ih As Integer = 0
        For Each col As DataGridViewColumn In dgv.Columns
            iw += col.Width
        Next
        For Each row As DataGridViewRow In dgv.Rows
            ih += row.Height
        Next
        ' oHelper.LOGIT(String.Format("dgv {0}x{1}", iw, ih))
        dgv.Width = iw * 1.1
        dgv.Height = (ih + dgv.ColumnHeadersHeight) * 1.1
        'If frm.Width > dgv.Width Then frm.Width = dgv.Width * 1.1
        'If frm.Height > dgv.Height Then frm.Height = dgv.Height * 1.1
    End Sub
#Region "DGV to HTML"
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
    Sub CreateHtmlFromDGV(ByVal dgv As DataGridView, ByVal sScreen As String, wf As Windows.Forms.Form, lbStatus As Label)
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
    Sub AddColumnToDGV(dgv As DataGridView, sName As String, iLength As Integer, iWidth As Integer)
        Dim dgc As New DataGridViewTextBoxColumn
        With dgv
            With dgc
                .Name = sName
                .MaxInputLength = iLength
                .ValueType = GetType(System.String)
                .HeaderText = .Name
                .Width = iWidth
            End With
            .Columns.Add(dgc)
            .Width += dgc.Width
        End With

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
    'Public Sub CreateMaskField(ByVal dgv As DataGridView, sField As String)

    '    Dim index As Integer
    '    ' find the location of the column
    '    index = dgv.Columns.IndexOf(dgv.Columns(sField))
    '    ' remove the existing column
    '    dgv.Columns.RemoveAt(index)
    '    ' create a new custom column
    '    Dim dgvMaskedEdit As New DataGridViewMaskedEditColumn

    '    If sField = "Phone" Then
    '        dgvMaskedEdit.Mask = "###-###-####"
    '        dgvMaskedEdit.ValidatingType = GetType(String)
    '    ElseIf sField.StartsWith("Hole") Then
    '        dgvMaskedEdit.Mask = "#"      ' this mask will allow only numbers
    '        dgvMaskedEdit.ValidatingType = GetType(String)
    '        dgvMaskedEdit.Width = 40
    '    Else
    '        dgvMaskedEdit.Mask = "##"      ' this mask will allow only numbers
    '        dgvMaskedEdit.ValidatingType = GetType(String)
    '    End If
    '    dgvMaskedEdit.Name = sField
    '    dgvMaskedEdit.DataPropertyName = sField
    '    ' some more tweaking
    '    dgvMaskedEdit.SortMode = DataGridViewColumnSortMode.Automatic

    '    ' insert the new column at the same location
    '    dgv.Columns.Insert(index, dgvMaskedEdit)
    '    dgv.Visible = True

    'End Sub

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
    Sub CreateLabel(controlCollection As Control.ControlCollection, itop As Object, p3 As Integer, p4 As Integer, p5 As String, p6 As Char)
        Throw New NotImplementedException
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
    Function GetInt(o As Object) As Integer
        If IsDBNull(o) Then Return 0 Else Return CInt(o)
    End Function
    Function sIn(sfld As String, s2Compare As String, bcontains As Boolean) As Boolean
        sIn = False
        Dim sflds = s2Compare.Split(",")
        For Each fld As String In s2Compare.Split(",")
            If bcontains Then
                If sfld.Contains(fld) Then
                    sIn = True
                    Exit Function
                End If
            Else
                If sfld = fld Then
                    sIn = True
                    Exit Function
                End If
            End If
        Next
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
    Public Function BuildScoreCardMethodsCB() As DataGridViewComboBoxCell
        Dim gridComboBox As New DataGridViewComboBoxCell
        gridComboBox.Items.Add("Net") 'Populate the Combobox
        gridComboBox.Items.Add("Gross") 'Populate the Combobox
        gridComboBox.Items.Add("Score") 'Populate the Combobox

        BuildScoreCardMethodsCB = gridComboBox
    End Function

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
    Sub UpdatePmts()
        iHoles = rLeagueParmrow("Holes")

        '20200605 - create an Earned money table for use to insure payments get updated
        Dim dtr As New Data.DataTable
        dtr.Columns.Add(Constants.datecon)
        dtr.Columns.Add(Constants.Player)
        dtr.Columns.Add("Desc")
        dtr.Columns.Add("Detail")
        dtr.Columns.Add("Earned", GetType(Decimal))
        dtr.Columns.Add("DatePaid")
        dtr.Columns.Add("Comment")
        dtr.Columns.Add("PayMethod")

        dtr.PrimaryKey = New DataColumn() {dtr.Columns(Constants.Player), dtr.Columns(Constants.datecon), dtr.Columns("Desc"), dtr.Columns(Constants.Detail)}
        dtr.DefaultView.Sort = "Player Asc"

        Dim dvscores = New DataView(dsLeague.Tables("dtScores"))
        dvscores.Sort = "Date"
        'dvscores.RowFilter = String.Format("Date < {0} and $Earn > 0", oHelper.sDateLastScore)
        Dim scoredates = New List(Of String)
        Dim wkdate As String = ""
        For Each row In dvscores
            If row(Constants.datecon) <> wkdate Then
                scoredates.Add(row(Constants.datecon))
                wkdate = row(Constants.datecon)
            End If
        Next

        Dim dvlp = New DataView(dsLeague.Tables("dtLeagueParms"))
        Dim sDetail As String = ""
        Dim sDesc As String = ""

        For Each sDate In scoredates
            LOGIT(String.Format("Date={0}", sDate))
            Dim sdescadd1 As String = ""
            Dim sdescadd2 As String = ""
            For Each lp In dvlp
                If sDate.Substring(0, 4) = CDate(lp("StartDate")).ToString("yyyyMMdd").Substring(0, 4) Then
                    If sDate > CDate(lp("EndDate")).ToString("yyyyMMdd") Then
                        sdescadd1 = "EOY "
                        If sDate < CDate(lp("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd") Then
                            sdescadd2 = "s 1"
                        Else
                            sdescadd2 = "s 2"
                        End If
                    End If
                End If
            Next
            'check gross/net scores only
            Dim dv = New DataView(dsLeague.Tables("dtScores"))
            dv.RowFilter = String.Format("Date = {0}", sDate)
            CalcThisHoleMarker(sDate, dv)
            'xlskins will contain lowest scores for each hole
            Dim xlskins = FCalcSkins(dv)
            'loop through each lowest scores looking for ties, if ties(|) then ignore it
            'lskins contains each hole and the player index in the score dataview
            Dim lSkins = New List(Of String)
            For Each indskin In xlskins
                If Not indskin.Contains("|") Then
                    Debug.Print(dv(indskin.Split("-")(1))(Constants.Player))
                    Debug.Print(indskin.Split("-")(0))
                    sDetail = String.Format("#{0}", indskin.Split("-")(0))
                    sDesc = sdescadd1 & Constants.skinpmt & sdescadd2

                    Dim sKeys() As Object = {dv(indskin.Split("-")(1))(Constants.Player), sDate, sDesc, sDetail}
                    Dim dr = dsLeague.Tables("dtPayments").Rows.Find(sKeys)
                    If dr IsNot Nothing Then
                        Continue For
                    End If
                    Dim sPlayerIndex As String = indskin.Split("-")(1)
                    Dim sHole As String = indskin.Split("-")(0)
                    Dim sNetScore As String = dv(sPlayerIndex)("Hole" & sHole)
                    '2020-01-15- 20180529 Ben Wright played 1 hole
                    If dv(sPlayerIndex)("Method") = "Gross" And rLeagueParmrow("SkinFmt") = "Handicap" Then
                        Dim isi As Int16 = CalcStrokeIndex(sHole)
                        If CInt(dv(sPlayerIndex)("pHdcp")) >= isi Then
                            'check stroke index
                            sNetScore -= 1
                            If CInt(dv(sPlayerIndex)("pHdcp") - iHoles) >= isi Then sNetScore -= 1
                        End If
                    End If
                    Debug.Print(String.Format("Skin Correction Date-{0},Player-{1},Hole-{2}", sKeys(1), sKeys(0), sKeys(3)))
                    Dim arow = dsLeague.Tables("dtPayments").NewRow
                    arow(Constants.datecon) = sDate
                    arow(Constants.Player) = dv(sPlayerIndex)(Constants.Player)
                    arow("Desc") = sDesc
                    arow(Constants.Detail) = sDetail
                    arow(Constants.Earnedcon) = dv(sPlayerIndex)(Constants.skinamt)
                    arow("Comment") = String.Format("Score={0}", sNetScore)
                    dsLeague.Tables("dtPayments").Rows.Add(arow)
                End If
            Next

            dv.RowFilter &= " and $Closest > 0 "
            'ctps

            For Each row In dv
                Dim ipar3cnt = 1
                For i = iHoleMarker To (iHoleMarker - 1) + 9
                    If thisCourse("Hole" & i) = 3 Then
                        If row(String.Format("CTP_{0}", ipar3cnt)) IsNot DBNull.Value Then
                            If ipar3cnt = 1 And sdescadd1 <> "" Then
                                sDesc = sdescadd1 & Constants.CTP & sdescadd2.Substring(1, 2)
                            ElseIf ipar3cnt = 2 And sdescadd1 <> "" Then
                                sDesc = sdescadd1 & Constants.CTP & sdescadd2.Substring(1, 2)
                            Else
                                sDesc = Constants.CTP
                            End If
                            sDetail = String.Format("#{0}", i)
                            Dim sCTPKeys() As Object = {row(Constants.Player), sDate, sDesc, sDetail}
                            Dim drCTP = dsLeague.Tables("dtPayments").Rows.Find(sCTPKeys)
                            If drCTP IsNot Nothing Then
                                Exit For
                            End If

                            Debug.Print(String.Format("CTP_{3} {1} Correction Date-{0},Player-{1},Hole-{2}", sCTPKeys(1), sCTPKeys(0), sCTPKeys(3), ipar3cnt))
                            Dim arow = dsLeague.Tables("dtPayments").NewRow
                            arow(Constants.datecon) = sDate
                            arow(Constants.Player) = row(Constants.Player)
                            arow("Desc") = sDesc
                            arow(Constants.Detail) = sDetail
                            arow(Constants.Earnedcon) = row(Constants.CTP & "_" & ipar3cnt)
                            arow("Comment") = ""
                            dsLeague.Tables("dtPayments").Rows.Add(arow)
                        End If
                        ipar3cnt += 1
                    End If
                Next

            Next

            Debug.Print("")
        Next
        DataTable2CSV(dsLeague.Tables("dtPayments"), sFilePath & "\Payments.csv")

    End Sub

    Public Function GetMACDBVersion() As String
        Dim AccessDBAsValue As String = String.Empty
        Dim rkACDBKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Classes")
        If rkACDBKey IsNot Nothing Then

            For Each subKeyName As String In rkACDBKey.GetSubKeyNames()

                If subKeyName.Contains("Microsoft.ACE.OLEDB") Then
                    If Not subKeyName.Contains("Errors") Then
                        sMACDBVersion = subKeyName
                    End If
                End If
            Next
        End If
        If sMACDBVersion = "" Then
            MessageBox.Show(String.Format("no version of Microsoft Access DB found{0}Contact developer", vbCrLf))
            End
        End If
        Return sMACDBVersion
    End Function
End Class
