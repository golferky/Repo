Imports System.IO.Packaging
Public Class Standings
    Dim oHelper As New Helper.Controls.Helper
    Dim dtSchedule As DataTable
    'put here cause 2 subs use these
    Dim sPlayer = ""
    'Dim iRnds = 0
    Dim iTot As Decimal = 0.0
    Dim iHalf1 As Decimal = 0.0
    Dim iHalf2 As Decimal = 0.0
    Dim sEndDate As Date
    Dim iNumWeeksSplit As Integer = 0
    Dim sHalfwayDate As String = 0
    'Dim semailfile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    Dim semailfile = "" 'oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"

    Dim rs As New Resizer
    Sub doStandings()
        'Sub BldStandings()
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            dgStandings.Visible = True

            Dim dtStandings = New DataTable
            dtStandings = BuilddtPoints()
            'build this table for points collection
            Dim dtPoints As DataTable = dtStandings.Clone
            dtPoints.TableName = "dtPoints"
            dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}
            For index As Integer = dtPoints.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtPoints.Columns(index).ColumnName
                If columnName = "Player" Or columnName = "Grade" Or columnName = "Rnds" Or columnName = "1st Half" Or columnName = "2nd Half" Or columnName = "Total" Then
                    dtPoints.Columns.RemoveAt(index)
                    Continue For
                End If
            Next

            Dim sprevteam = 0, sprevgrade = ""
            For Each row As DataRow In dtStandings.Rows
                Dim sTeam = row("Team")
                Dim sKeys() As Object = {row("Team")}
                Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
                If drow Is Nothing Then
                    drow = dtPoints.NewRow
                    drow("Team") = row("Team")
                    For Each col As DataColumn In dtStandings.Columns
                        If col.ColumnName <> "Player" And col.ColumnName <> "Team" And col.ColumnName <> "Grade" And col.ColumnName <> "Rnds" And col.ColumnName <> "1st Half" And col.ColumnName <> "2nd Half" And col.ColumnName <> "Total" Then
                            drow(col.ColumnName) = 0
                        End If
                    Next
                    dtPoints.Rows.Add(drow)
                End If
                For Each col As DataColumn In dtStandings.Columns
                    If col.ColumnName.Contains("/") Then
                        'accumulate points from gridview
                        drow(col.ColumnName) += getPts(row(col.ColumnName))
                    End If
                Next
                'combine individual points to team points


                If cb1stHalf.Checked Then
                    drow("Team 1st Half") += CDec(row("1st Half")) + CDec(row("Team 1st Half"))
                    drow("Team Points") += CDec(row("1st Half")) + CDec(row("Team 1st Half"))
                End If

                If cb2ndHalf.Checked Then
                    drow("Team 2nd Half") += CDec(row("2nd Half")) + CDec(row("Team 2nd Half"))
                    drow("Team Points") += CDec(row("2nd Half")) + CDec(row("Team 2nd Half"))
                End If
            Next

            dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}
            For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtStandings.Columns(index).ColumnName
                If Not cb1stHalf.Checked Then
                    If columnName.Contains("1st Half") Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
                    End If
                    If columnName.Contains("/") And columnName < sHalfwayDate Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
                    End If
                End If
                If Not cb2ndHalf.Checked Then
                    If columnName.Contains("2nd Half") Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
                    End If
                    If columnName.Contains("/") And columnName >= sHalfwayDate Then
                        dtStandings.Columns.RemoveAt(index)
                    End If
                End If
            Next

            Dim dvStandings As New DataView(dtStandings)

            Dim i = 0
            For Each row As DataRowView In dvStandings
                'loop through and delete subs if its not checked
                If Not cbSubs.Checked Then
                    oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}
                    Dim sKeys() As Object = {row.Item("Player")}
                    Dim drow As DataRow = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(sKeys)
                    If drow.Item("Grade") = "S" Then
                        dvStandings.Table.Rows(i).Delete()
                        i -= 1
                    End If
                    i += 1
                End If
            Next
            dgStandings.Columns.Clear()
            dgStandings.AllowUserToAddRows = False
            dgStandings.AllowUserToDeleteRows = False

            With dgStandings
                '.Columns.Clear()
                .DataSource = dvStandings
                .Columns("Grade").Width = 36
                .Columns("Team").Width = 35
                .Columns("Total").Width = 36

                For Each col As DataGridViewTextBoxColumn In dgStandings.Columns
                    col.ReadOnly = True
                    If col.Name.Contains("/") Then
                        .Columns(col.Name).Width = 40
                        If cbHdcp.Checked Then
                            .Columns(col.Name).Width += 10
                        End If
                        If cbScore.Checked Then
                            .Columns(col.Name).Width += 10
                        End If
                        .Columns(col.Name).HeaderText = col.Name.Substring(0, col.Name.LastIndexOf("/"))
                        Dim x = ""
                    ElseIf col.Name.Contains("Half") Or col.Name.Contains("Total") Or col.Name = "Rnds" Or col.Name = "Team Points" Then
                        .Columns(col.Name).Width = 36
                    End If
                Next

            End With

            buildEmailFile()

            For Each row As DataGridViewRow In dgStandings.Rows
                'checkbox order ind pts, team pts, score, handicap
                oHelper.sPlayer = row.Cells("Player").Value & "-" & row.Cells("Grade").Value
                Dim sKeys() As Object = {row.Cells("Team").Value}
                Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
                If cb1stHalf.Checked Then row.Cells("Team 1st Half").Value = ""
                If cb2ndHalf.Checked Then row.Cells("Team 2nd Half").Value = ""

                row.Cells("Team Points").Value = ""
                'alternate color to differentiate teams
                If row.Cells("Team").Value Mod 2 Then
                    row.DefaultCellStyle.BackColor = Color.LightGray
                End If
                Dim ipts = 0.0
                Dim itpts = 0.0
                'loop through each column and decide which items to delete from gridview cell (Score/hdcp/ind points/team points)
                For Each col As DataGridViewCell In row.Cells
                    'opponent
                    If Not cbOpp.Checked Then
                        If col.OwningColumn.Name.Contains("/") Then
                            Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                            If sAry.Count > 1 Then
                                sAry.RemoveAt(sAry.Count - 1)
                                col.Value = ""
                                For Each item In sAry
                                    col.Value = col.Value & item & "-"
                                Next
                                col.Value = col.Value.ToString.Substring(0, col.Value.ToString.Length - 1)
                            End If
                        End If
                    End If
                    'handicap
                    If Not cbHdcp.Checked Then
                        If col.OwningColumn.Name.Contains("/") Then
                            Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                            If sAry.Count > 1 Then
                                sAry.RemoveAt(3)
                                col.Value = ""
                                For Each item In sAry
                                    col.Value = col.Value & item & "-"
                                Next
                                col.Value = col.Value.ToString.Substring(0, col.Value.ToString.Length - 1)
                            End If
                        End If
                    End If
                    If Not cbScore.Checked Then
                        If col.OwningColumn.Name.Contains("/") Then
                            Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                            If sAry.Count > 1 Then
                                sAry.RemoveAt(2)
                                col.Value = ""
                                For Each item In sAry
                                    col.Value = col.Value & item & "-"
                                Next
                                col.Value = col.Value.ToString.Substring(0, col.Value.ToString.Length - 1)
                            End If
                        End If
                    End If
                    If Not cbTeam.Checked Then
                        If col.OwningColumn.Name.Contains("/") Then
                            Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                            If sAry.Count > 1 Then
                                sAry.RemoveAt(1)
                                col.Value = ""
                                For Each item In sAry
                                    col.Value = col.Value & item & "-"
                                Next
                                If sAry.Count > 0 Then
                                    col.Value = col.Value.ToString.Substring(0, col.Value.ToString.Length - 1)
                                End If
                            End If
                        End If
                    Else
                        If rbCum.Checked Then
                            If col.OwningColumn.Name.Contains("/") Then
                                'if nothing is in the cell, put the team accumulated points there
                                If col.Value Is DBNull.Value Then
                                    col.Value = "0" & "-" & itpts
                                Else
                                    'col.value or sAry has ipts-tpts-score-hdcp
                                    itpts += col.Value.ToString.Split("-")(1)
                                    Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                                    If sAry.Count > 1 Then
                                        sAry(1) = itpts
                                        col.Value = ""
                                        For Each sfld In sAry
                                            col.Value = col.Value & sfld & "-"
                                        Next
                                        col.Value = col.Value.ToString.Substring(0, col.Value.ToString.Length - 1)
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If Not cbIPoints.Checked Then
                        If col.OwningColumn.Name.Contains("/") Then
                            Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                            If sAry.Count > 1 Then
                                sAry.RemoveAt(0)
                                col.Value = ""
                                For Each item In sAry
                                    col.Value = col.Value & item & "-"
                                Next
                                If sAry.Count > 0 Then
                                    col.Value = col.Value.ToString.Substring(0, col.Value.ToString.Length - 1)
                                End If
                            End If
                        End If
                    Else
                        If rbCum.Checked Then
                            'make sure this is a date column
                            If col.OwningColumn.Name.Contains("/") Then
                                'if its null, then put points in it
                                If col.Value Is DBNull.Value Then
                                    col.Value = ipts
                                Else
                                    'accumulate individual points
                                    ipts += col.Value.ToString.Split("-")(0)

                                    Dim sAry As New List(Of String)(col.Value.ToString.Split("-"))
                                    col.Value = ipts
                                    Dim bfirst = True
                                    For Each sfld In sAry
                                        If Not bfirst Then
                                            col.Value = col.Value & "-" & sfld
                                        Else
                                            bfirst = False
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    End If
                Next

                If row.Cells("Team").Value <> sprevteam Then
                    If cb1stHalf.Checked Then
                        row.Cells("Team 1st Half").Value = drow("Team 1st Half")
                    End If
                    If cb2ndHalf.Checked Then
                        row.Cells("Team 2nd Half").Value = drow("Team 2nd Half")
                    End If

                    row.Cells("Team Points").Value = drow("Team Points")
                    sprevteam = row.Cells("Team").Value
                End If

            Next

            'Using sw As New IO.StreamWriter(semailfile, False)
            '    Try
            '        Dim sline = ""
            '        For Each col As DataGridViewColumn In dgStandings.Columns
            '            sline = sline & col.HeaderText & ","
            '        Next
            '        sw.WriteLine(sline)

            '        For Each row As DataGridViewRow In dgStandings.Rows
            '            sline = ""
            '            For Each item As DataGridViewTextBoxCell In row.Cells
            '                sline = sline & item.Value & ","
            '            Next
            '            sw.WriteLine(sline)
            '        Next
            '        sw.Close()

            '    Catch ex As Exception

            '    End Try

            'End Using
            '20180220 - fix issue with no fiel to email crash
            'If semailfile <> Nothing Then btnEmail.Visible = True
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub buildEmailFile()
        Try
            Dim dtStandings = New DataTable
            dtStandings = BuilddtPoints(True)
            Dim dtStandingsForPts = New DataTable
            dtStandingsForPts = BuilddtPoints()
            'build this table for points collection
            Dim dtPoints As DataTable = dtStandings.Clone
            dtPoints.TableName = "dtPoints"
            dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}
            For index As Integer = dtPoints.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtPoints.Columns(index).ColumnName
                If columnName = "Player" Or columnName = "Grade" Or columnName = "Rnds" Or columnName = "1st Half" Or columnName = "2nd Half" Or columnName = "Total" Then
                    dtPoints.Columns.RemoveAt(index)
                    Continue For
                End If
                'If columnName.Contains("Team") Then columnName = columnName.Replace("Team ", "")
            Next

            Dim sprevteam = 0, sprevgrade = ""
            For Each row As DataRow In dtStandingsForPts.Rows
                Dim sTeam = row("Team")
                Dim sKeys() As Object = {row("Team")}
                Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
                If drow Is Nothing Then
                    drow = dtPoints.NewRow
                    drow("Team") = row("Team")
                    For Each col As DataColumn In dtStandingsForPts.Columns
                        If col.ColumnName <> "Player" And col.ColumnName <> "Team" And col.ColumnName <> "Grade" And col.ColumnName <> "Rnds" And col.ColumnName <> "1st Half" And col.ColumnName <> "2nd Half" And col.ColumnName <> "Total" Then
                            drow(col.ColumnName) = 0
                        End If
                    Next
                    dtPoints.Rows.Add(drow)
                End If
                For Each col As DataColumn In dtStandingsForPts.Columns
                    If col.ColumnName.Contains("/") Then
                        'accumulate points from gridview
                        drow(col.ColumnName) += getPts(row(col.ColumnName))
                    End If
                Next
            Next
            Dim i = 0
            'has to be done from back to front because the collection numbers change from front to back
            For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtStandings.Columns(index).ColumnName
                'If columnName.Contains("Team") Then columnName = columnName.Replace("Team ", "")
                If columnName = "Team 1st Half" Or columnName = "Team 2nd Half" Or columnName = "Team Points" Then
                    dtStandings.Columns.RemoveAt(index)
                End If
            Next

            Dim dvStandings As New DataView(dtStandings)
            For Each row As DataRowView In dvStandings
                oHelper.sPlayer = row("Player") & "-" & row("Grade")
                Dim sKeys() As Object = {row("Team")}
                Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
                If cb1stHalf.Checked Then row("1st Half") = ""
                If cb2ndHalf.Checked Then row("2nd Half") = ""

                row("1st half") = ""
                row("2nd half") = ""
                row("Total") = ""

                If row("team") <> sprevteam Then
                    Dim ipts As Decimal = 0.0
                    Dim ifh As Decimal = 0.0
                    Dim ish As Decimal = 0.0
                    Dim bfh = True, bsh = False
                    'total up halves
                    For Each col As DataColumn In dtPoints.Columns
                        If col.ColumnName.Contains("2nd") Then bsh = True
                        If col.ColumnName.Contains("/") Then
                            ipts += CDec(oHelper.convDBNulltoSpaces(drow(col.ColumnName)))
                            If bsh Then
                                ish += CDec(oHelper.convDBNulltoSpaces(drow(col.ColumnName)))
                            Else
                                ifh += CDec(oHelper.convDBNulltoSpaces(drow(col.ColumnName)))
                            End If
                        End If
                    Next

                    row("1st half") = ifh
                    row("2nd half") = ish
                    row("Total") = ipts
                    sprevteam = row("team")
                End If

            Next

            Dim amonths As New List(Of String)("Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sept,Oct,Nov,Dec".Split(","))

            semailfile = oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
            Using sw As New IO.StreamWriter(semailfile, False)
                Try

                    Dim sb1 As New System.Text.StringBuilder
                    Dim sb2 As New System.Text.StringBuilder
                    For Each col As DataColumn In dtStandings.Columns
                        If col.ColumnName.Contains("/") Then
                            'example Apr/11
                            sb1.Append(amonths(Convert.ToDateTime(col.ColumnName).ToString("MM") - 1) & ",")
                            sb2.Append(Convert.ToDateTime(col.ColumnName).ToString("dd") & ",")
                        Else
                            Dim scols As New List(Of String)(col.ColumnName.Split(" "))
                            If scols.Count > 1 Then
                                sb1.Append(scols(0) & ",")
                                sb2.Append(scols(1) & ",")
                            Else
                                sb1.Append(",")
                                sb2.Append(scols(0) & ",")
                            End If
                        End If
                    Next

                    sw.WriteLine(sb1.ToString)
                    sw.WriteLine(sb2.ToString)

                    Dim sline = ""
                    sprevteam = 0
                    For Each row As DataRow In dvStandings.Table.Rows
                        If row("Team") <> sprevteam Then
                            sline = "Team #" & row("Team")
                            sw.WriteLine(sline)
                            sprevteam = row("Team")
                        End If
                        sline = ""
                        For Each item As DataColumn In dvStandings.Table.Columns
                            sline = sline & oHelper.convDBNulltoSpaces(row(item).ToString) & ","
                        Next
                        sw.WriteLine(sline)
                    Next
                    sw.Close()

                Catch ex As Exception

                End Try

            End Using
            Dim sHtml = oHelper.ConvertToHtmlFile(dtStandings)
            sHtml = oHelper.ConvertToHTMLString(dtStandings)
            If semailfile <> Nothing Then btnEmail.Visible = True
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try


    End Sub
    Function BuilddtPoints() As DataTable
        Return BuilddtPoints(False)
    End Function
    Function BuilddtPoints(email As Boolean) As DataTable
        BuilddtPoints = Nothing
        Try
            'copy the schedule table 
            Dim dtStandings As DataTable = oHelper.dsLeague.Tables("dtSchedule").Clone()
            sEndDate = oHelper.rLeagueParmrow("EndDate")
            dtStandings.TableName = "dtStandings"
            'only build the schedule up thru the end date
            For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtStandings.Columns(index).ColumnName
                If columnName > sEndDate Then
                    dtStandings.Columns.RemoveAt(index)
                End If
            Next

            iNumWeeksSplit = dtStandings.Columns.Count / 2
            sHalfwayDate = dtStandings.Columns.Item(iNumWeeksSplit).ToString

            With dtStandings
                .Columns.Add("Rnds", GetType(String)).SetOrdinal(0)
                .Columns.Add("Grade", GetType(String)).SetOrdinal(0)
                .Columns.Add("Team", GetType(Integer)).SetOrdinal(0)
                .Columns.Add("Player", GetType(String)).SetOrdinal(0)

                .Columns.Add("Team 1st Half", GetType(String)).SetOrdinal(iNumWeeksSplit + 4)
                .Columns.Add("1st Half", GetType(String)).SetOrdinal(iNumWeeksSplit + 4)
                .Columns.Add("2nd Half", GetType(String))
                .Columns.Add("Team 2nd Half", GetType(String))
                .Columns.Add("Total", GetType(String))
                .Columns.Add("Team Points", GetType(String))
                .DefaultView.Sort = "Team"
            End With

            dtStandings.PrimaryKey = New DataColumn() {dtStandings.Columns("Player"), dtStandings.Columns("Team")}
            'If Not oHelper.dsLeague.Tables.Contains("dtScores") Then oHelper.dsLeague.Tables.Add("dtScores").ReadXml(oHelper.getLatestFile("*Scores.xml"))
            If Not oHelper.dsLeague.Tables.Contains("dtScores") Then oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            'dvScores.RowFilter = "Date >= " & sEndDate.ToString("yyyyMMdd").Substring(0, 4) & "0101"
            dvScores.RowFilter += String.Format("date >= {0} and date < {1}", sEndDate.ToString("yyyyMMdd").Substring(0, 4) & "0101", sEndDate.ToString("yyyMMdd").Substring(0, 4) + 1) & "0101"
            'dvScores.Sort = "Team,Date,Grade"
            Dim sPoints = "", sTeamPoints = ""
            For Each score As DataRowView In dvScores
                oHelper.sPlayer = score("Player")

                'Dim sScore = IIf(score("Out_Gross") Is DBNull.Value, score("In_Gross"), score("Out_Gross"))
                Dim sScore = IIf(oHelper.convDBNulltoSpaces(score("Out_Gross")).Trim = "", oHelper.convDBNulltoSpaces(score("In_Gross")).Trim, oHelper.convDBNulltoSpaces(score("Out_Gross")).Trim)
                If sScore = "" Then sScore = "NA"
                Dim sKeys() As Object = {score("Player"), score("Team")}
                Dim drow As DataRow = dtStandings.Rows.Find(sKeys)
                If drow Is Nothing Then
                    drow = dtStandings.NewRow
                    drow("Player") = score("Player")
                    drow("Team") = score("Team")
                    drow("Grade") = score("Grade")
                    drow("Rnds") = 0
                    drow("1st Half") = 0
                    drow("Team 1st Half") = 0
                    drow("2nd Half") = 0
                    drow("Team 2nd Half") = 0
                    drow("Total") = 0
                    drow("Team Points") = 0
                    dtStandings.Rows.Add(drow)
                End If
                If sScore IsNot DBNull.Value Then
                    If score("date") < CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                        If cb1stHalf.Checked Then
                            drow("Rnds") += 1
                        End If
                    Else
                        If cb2ndHalf.Checked Then
                            drow("Rnds") += 1
                        End If
                    End If
                End If

                Dim sColname As String = CInt(score("Date").ToString.Substring(4, 2)).ToString + "/" + CInt(score("Date").ToString.Substring(6, 2)).ToString + "/" + score("Date").ToString.Substring(0, 4)

                Dim dPoints As Decimal = 0.00
                Dim dTeamPoints As Decimal = 0.00
                If score("Points") IsNot DBNull.Value Then
                    If score("Points") <> "" Then dPoints = score("Points")
                End If
                If score("Team_Points") IsNot DBNull.Value Then
                    If score("Team_Points") <> "" Then dTeamPoints = score("Team_Points")
                End If

                If sScore IsNot DBNull.Value Then
                    drow(sColname) = ""
                    'default to points and team points 
                    sPoints = dPoints
                    If dPoints < 1 Then
                        sPoints = sPoints.Replace("0.", ".")
                    End If
                    sTeamPoints = dTeamPoints
                    If dTeamPoints < 1 Then
                        sTeamPoints = sTeamPoints.Replace("0.", ".")
                    End If
                    '20180302-if email run, dont add pts, tp, opp
                    If email Then
                        drow(sColname) = sScore & "-" & score("Hdcp")
                    Else
                        drow(sColname) = sPoints & "-" & sTeamPoints & "-" & sScore & "-" & score("Hdcp") & "-" & score("Opponent")
                    End If

                End If

                'calculate team first half, second half, total points 
                If score("date") < CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                    drow("1st Half") += dPoints
                    drow("Team 1st Half") += dTeamPoints
                Else
                    drow("2nd Half") += dPoints
                    drow("Team 2nd Half") += dTeamPoints
                End If
                drow("Total") += dPoints
                drow("Team Points") += dTeamPoints
            Next
            'dtStandings.DefaultView.Sort = "Team"
            Return dtStandings.DefaultView.ToTable
        Catch ex As Exception
            If ex.Message.ToUpper.Contains("CANNOT OPEN THE FILE") Then
                MsgBox("File is in use, close it and retry" & vbCrLf & oHelper.sFilePath & "Scores.csv" Or "-Players.csv")
            Else
                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            End If
        End Try
    End Function
    Sub CenterGroupBoxText(Groupbox As GroupBox)
        Dim label As New Label
        label.Text = Groupbox.Text
        Groupbox.Text = ""
        label.Left = Groupbox.Left + (Groupbox.Width - label.Width) / 2
        label.Top = Groupbox.Top + 2 ' // 2 Is an example : adjust the constant
        label.Parent = Groupbox.Parent
        label.BringToFront()
    End Sub
    Private Sub Standings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'CenterGroupBoxText(gbCell)
        'CenterGroupBoxText(gbOptions)
        'CenterGroupBoxText(gbPoints)
        rs.FindAllControls(Me)

        oHelper = Main.oHelper
        oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        If oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("SplitSeason") <> "Y" Then
            cb1stHalf.Visible = False
            cb1stHalf.Checked = False
            cb2ndHalf.Visible = False
            cb2ndHalf.Checked = False
        End If
        dgStandings.Visible = False
        '20180220 - fix issue with no field to email crash
        btnEmail.Visible = False
    End Sub
    Private Sub btnStandings_Click(sender As Object, e As EventArgs) Handles btnStandings.Click
        doStandings()
    End Sub
    Private Sub dgStandings_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgStandings.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'If MousePosition.Y < 0 Then
        'If sender.mouseenteredcelladdress < 0 Then
        '    Exit Sub
        'End If

        If e.ColumnIndex = 0 Then
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            If cell.OwningColumn.Name = "Player" Then
                Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
                If mbResult = MsgBoxResult.Yes Then
                    oHelper.bScoresbyPlayer = True
                    oHelper.sPlayer = cell.Value
                    Scores.Show()
                    oHelper.bScoresbyPlayer = False
                End If
            End If
        End If

    End Sub
    Function getPts(sPts) As Decimal
        getPts = 0.0
        If sPts Is DBNull.Value Then Exit Function
        If sPts.Contains("-") Then
            Dim aPts As New List(Of String)(sPts.ToString.Split("-"))
            getPts += CDbl(aPts(0))
            If aPts.Count > 0 Then getPts += CDbl(aPts(1))
        Else
            getPts = sPts
        End If
    End Function
    Private Sub Standings_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        rs.ResizeAllControls(Me)
    End Sub

    Private Sub cbPoints_CheckedChanged(sender As Object, e As EventArgs) Handles cbIPoints.CheckedChanged
        If cbIPoints.Checked Then
            gbPoints.Visible = True
        Else
            If Not cbTeam.Checked Then
                gbPoints.Visible = False
            End If
        End If
    End Sub
    Private Sub cbTeam_CheckedChanged(sender As Object, e As EventArgs) Handles cbTeam.CheckedChanged
        If cbTeam.Checked Then
            gbPoints.Visible = True
        Else
            If Not cbIPoints.Checked Then
                gbPoints.Visible = False
            End If
        End If
    End Sub
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub dgStandings_ColumnDividerDoubleClick(sender As Object, e As DataGridViewColumnDividerDoubleClickEventArgs) Handles dgStandings.ColumnDividerDoubleClick
        Dim x = ""
    End Sub

    Private Sub btnEmail_Click(sender As Object, e As EventArgs) Handles btnEmail.Click

        Dim ToAddresses As New List(Of String)  '{"garyrscudder@gmail.com", "garyrscudder@gmail.com"}
        For Each player In oHelper.dsLeague.Tables("dtPlayers").Rows
            If oHelper.convDBNulltoSpaces(player("Email")).Trim <> "" Then
                ToAddresses.Add(player("Email"))
            End If
        Next

        Dim mbr = MsgBox(String.Format("are you ready to sent emails to {0} players?", ToAddresses.Count), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then
            Exit Sub
        End If
        'Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
        Dim attachs() As String = {semailfile}
        Dim subject As String = "Standings Sheet"
        Dim body As String = semailfile
        Dim bresult = False
        If ToAddresses.Count > 0 Then
            bresult = oHelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
            If bresult Then
                MsgBox("mails sent successfully", MsgBoxStyle.Information)
            Else
                MsgBox(oHelper.GGmail.ErrorText, MsgBoxStyle.Critical)
            End If
        End If

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class