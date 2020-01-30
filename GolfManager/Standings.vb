Imports System.Text
Imports System.Data
'Imports Microsoft.Office.Interop.Excel
Public Class Standings
    Dim oHelper As New Helper
    Dim dtSchedule As Data.DataTable
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
    Dim semailfile As String = "" 'oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    Dim rs As New Resizer
    Private Sub Standings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'CenterGroupBoxText(gbCell)
        'CenterGroupBoxText(gbOptions)
        'CenterGroupBoxText(gbPoints)
        rs.FindAllControls(Me)

        oHelper = Main.oHelper
        oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        Dim sWH As String = oHelper.ScreenResize()
        If Me.Width >= sWH.Split(":")(0) Then
            Me.Width = sWH.Split(":")(0) - (sWH.Split(":")(0) * 0.1)
            'Else
            '    Me.Width = sWH.Split(":")(0)
        End If
        If Me.Height >= sWH.Split(":")(1) Then
            Me.Height = sWH.Split(":")(1) - (sWH.Split(":")(1) * 0.1)
            'Else
            '    Me.Height = sWH.Split(":")(1)
        End If

        If oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("SplitSeason") <> "Y" Then
            cb1stHalf.Visible = False
            cb1stHalf.Checked = False
            cb2ndHalf.Visible = False
            cb2ndHalf.Checked = False
        End If
        dgStandings.Visible = False
        btnEmail.Visible = False
        dtSchedule = oHelper.dsLeague.Tables("dtSchedule")
        'If cbDates.Items.Contains(oHelper.dDate.ToString("yyyyMMdd")) Then cbDates.SelectedIndex = cbDates.Items.IndexOf(oHelper.dDate.ToString("yyyyMMdd"))
        lbStatus.Text = String.Format("Loading Scores")
        oHelper.status_Msg(lbStatus, Me)
        cbDates.Items.AddRange(Main.cbDates.Items.Cast(Of String).ToArray)
        'remove non-match dates from dates combobox
        'cdateToyyyyMMdd converts a string from 1/1/1900 to 19000101
        Do While cbDates.Items(0) >= oHelper.CDateToyyyyMMdd(oHelper.rLeagueParmrow("PostSeasonDt")) ' CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
            cbDates.Items.Remove(cbDates.Items(0))
        Loop
        'select the date from the main screen unless it was not a match date
        If cbDates.Items.Contains(Main.cbDates.SelectedItem) Then
            cbDates.SelectedItem = Main.cbDates.SelectedItem
        Else
            cbDates.SelectedItem = cbDates.Items(0)
        End If

        lbStatus.Text = String.Format("Finished Loading Scores")
        oHelper.status_Msg(lbStatus, Me)

    End Sub
    Sub doStandings()
        'Sub BldStandings()
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            lbStatus.Text = String.Format("Building standings columns")
            oHelper.status_Msg(lbStatus, Me)

            dgStandings.Visible = True

            Dim dtStandings = New Data.DataTable
            dtStandings = BuilddtPoints()
            'future 20180720
            Dim dvStandingsDGV As New DataView(dtStandings)
            Dim sStdDGV As String = "Player,Team 1st Half,Team 2nd Half,Team Points,"
            '20190627 - optional columns
            If Not cbTotalPts.Checked Then sStdDGV = sStdDGV.Replace(",Team Points", "")

            For Each col As DataColumn In dtStandings.Columns
                If col.ColumnName.Contains("/") Then sStdDGV = sStdDGV & col.ColumnName & ","
            Next
            sStdDGV = sStdDGV.Substring(0, Len(sStdDGV) - 1).Replace(" ", "_")
            'build this table for points collection
            'points table is for team points only
            Dim dtPoints As Data.DataTable = dtStandings.Clone
            dtPoints.TableName = "dtPoints"
            dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}
            For index As Integer = dtPoints.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtPoints.Columns(index).ColumnName
                If columnName = "Player" Or columnName = "Grade" Or columnName = "Rnds" Or columnName = "1st Half" Or columnName = "2nd Half" Or columnName = "Total" Then
                    dtPoints.Columns.RemoveAt(index)
                    Continue For
                End If
            Next
            dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}
            'dtpoints has just team number and a column for points each week
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

                'accumulate points from gridview
                For Each col As DataColumn In dtStandings.Columns
                    If col.ColumnName.Contains("/") Then drow(col.ColumnName) += getPts(row(col.ColumnName))
                Next
                'combine individual points to team points and update dtpoints table
                If cb1stHalf.Checked Then
                    drow("Team 1st Half") += CDec(row("1st Half")) + CDec(row("Team 1st Half"))
                    ' drow("YTD Points") += CDec(row("1st Half")) + CDec(row("Team 1st Half"))
                End If

                If cb2ndHalf.Checked Then
                    oHelper.LOGIT(String.Format("--before--team {2} 2nd half {0} team 2nd half {1} ", row("2nd Half"), row("Team 2nd Half"), row("Team")))
                    drow("Team 2nd Half") += CDec(row("2nd Half")) + CDec(row("Team 2nd Half"))
                    'drow("YTD Points") += CDec(row("2nd Half")) + CDec(row("Team 2nd Half"))
                    oHelper.LOGIT(String.Format("--after--2nd half {0} team 2nd half {1}", row("2nd Half"), row("Team 2nd Half")))
                End If
            Next

            'dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}

            'now lets create a standings table based on checkboxes and radio buttons
            For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtStandings.Columns(index).ColumnName
                'oHelper.LOGIT(String.Format("processing index {0}, name {1}", index, columnName))
                If Not cb1stHalf.Checked Then
                    If columnName.Contains("/") Then
                        If CDate(columnName).ToString("yyyyMMdd") < CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                            'oHelper.LOGIT(String.Format("not 1st half, removing index {0}, name {1}", index, columnName))
                            dtStandings.Columns.RemoveAt(index)
                            Continue For
                        End If
                    End If
                End If
                If Not cb2ndHalf.Checked Then
                    If columnName.Contains("/") Then
                        If CDate(columnName).ToString("yyyyMMdd") >= CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                            'oHelper.LOGIT(String.Format("not 2nd half, removing index {0}, name {1}", index, columnName))
                            dtStandings.Columns.RemoveAt(index)
                            Continue For
                        End If
                    End If
                End If
                '20190627-remove optional columns if not checked
                If Not cbTotalPts.Checked Then
                    If columnName = "Total" Or columnName = "Team Points" Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
                    End If
                End If
                If Not cbIndPts.Checked Then
                    If columnName = "1st Half" Or columnName = "2nd Half" Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
                    End If
                End If
            Next

            Dim dvStandings As New DataView(dtStandings)

            Dim i = 0
            Try
                Dim sPrvTeam As String = "", bAplayer As Boolean = False, bBplayer As Boolean = False
                For Each row As DataRowView In dvStandings
                    'loop through and delete subs if its not checked
                    If Not cbSubs.Checked Then
                        oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}
                        Dim sKeys() As Object = {row.Item("Player")}
                        Dim drow As DataRow = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(sKeys)
                        Try
                            If drow.Item("Grade") Is DBNull.Value Then
                                dvStandings.Table.Rows(i).Delete()
                                i -= 1
                            ElseIf drow.Item("Grade") = "S" Then
                                dvStandings.Table.Rows(i).Delete()
                                i -= 1
                            End If
                        Catch ex As Exception

                        End Try
                        i += 1
                    End If
                Next

            Catch ex As Exception

            End Try

            dgStandings.Columns.Clear()
            dgStandings.AllowUserToAddRows = False
            dgStandings.AllowUserToDeleteRows = False

            With dgStandings
                '.Columns.Clear()
                .DataSource = dvStandings
                .Columns("Grade").Width = 36
                .Columns("Team").Width = 35
                If cbTotalPts.Checked Then .Columns("Total").Width = 36
                dgStandings.ColumnHeadersDefaultCellStyle.Font = New System.Drawing.Font("Tahoma", 8, FontStyle.Bold)
                'adjust columns for each field being added
                For Each col As DataGridViewTextBoxColumn In dgStandings.Columns
                    col.ReadOnly = True                 'make all columns read only
                    If col.Name.Contains("/") Then      'is this a date column
                        .Columns(col.Name).Width = 50   'yep, make width 40
                    ElseIf col.Name.Contains("Half") Or col.Name.Contains("Total") Or col.Name = "Rnds" Or col.Name = "Team Points" Or col.Name = "WKY Points" Then
                        .Columns(col.Name).Width = 40
                    End If
                Next

            End With

            Dim saPlayer As String = ""
            For Each row As DataGridViewRow In dgStandings.Rows
                'checkbox order ind pts, team pts, score, handicap
                oHelper.sPlayer = row.Cells("Player").Value & "-" & row.Cells("Grade").Value
                Dim sKeys() As Object = {row.Cells("Team").Value}
                Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
                If cb1stHalf.Checked Then row.Cells("Team 1st Half").Value = ""
                If cb2ndHalf.Checked Then row.Cells("Team 2nd Half").Value = ""

                If cbTotalPts.Checked Then row.Cells("Total").Value = ""
                'alternate color to differentiate teams
                If row.Cells("Team").Value Mod 2 Then row.DefaultCellStyle.BackColor = Color.LightBlue

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
                    If cb1stHalf.Checked Then row.Cells("Team 1st Half").Value = drow("Team 1st Half")
                    If cb2ndHalf.Checked Then row.Cells("Team 2nd Half").Value = drow("Team 2nd Half")
                    If cbTotalPts.Checked Then row.Cells("Total").Value = CDec(drow("Team 1st Half")) + CDec(drow("Team 2nd Half"))
                    If oHelper.dDate.ToString("yyyyMMdd") <= CDate(oHelper.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
                        row.Cells("WKY Points").Value = drow(oHelper.dDate.ToString("yyyyMMdd").Substring(4, 2) & "/" & oHelper.dDate.ToString("yyyyMMdd").Substring(6, 2))
                    End If
                    sprevteam = row.Cells("Team").Value
                    saPlayer = row.Cells("Player").Value
                Else
                    row.Cells("WKY Points").Value = ""
                End If

            Next

            If Not buildEmailFile() Then MsgBox("Couldnt update the standings file because its in use")

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

        lbStatus.Text = String.Format("Finished Building standings columns")
        oHelper.status_Msg(lbStatus, Me)

    End Sub
    Function buildEmailFile() As Boolean
        Try
            buildEmailFile = True
            Dim amonths As New List(Of String)("Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sept,Oct,Nov,Dec".Split(","))

            semailfile = oHelper.sReportPath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
            Using sw As New IO.StreamWriter(semailfile, False)
                Try

                    Dim sb1 As New System.Text.StringBuilder
                    Dim sb2 As New System.Text.StringBuilder
                    Dim sprevteam As String = ""
                    For Each col As DataGridViewColumn In dgStandings.Columns
                        If col.Name.Contains("/") Then
                            'example Apr/11
                            sb1.Append(amonths(Convert.ToDateTime(col.Name).ToString("MM") - 1) & ",")
                            sb2.Append(Convert.ToDateTime(col.Name).ToString("dd") & ",")
                        Else
                            Dim scols As New List(Of String)(col.Name.Split(" "))
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
                    For Each row As DataGridViewRow In dgStandings.Rows
                        If row.Cells("Team").Value <> sprevteam Then
                            sline = "Team #" & row.Cells("Team").Value
                            sw.WriteLine(sline)
                            sprevteam = row.Cells("Team").Value
                        Else
                            row.Cells("WKY Points").Value = ""
                        End If
                        sline = ""
                        For Each item As DataGridViewColumn In dgStandings.Columns
                            sline = sline & oHelper.convDBNulltoSpaces(row.Cells(item.Name).Value) & ","
                        Next
                        sw.WriteLine(sline)
                    Next

                    sw.WriteLine("")
                    '20190628 this adds the schedule to the csv file
                    For Each row As DataRow In oHelper.dsLeague.Tables("dtSchedule").Rows
                        Dim ii = 1
                        Dim i = 1
                        sline = ",,,,"
                        For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                            oHelper.LOGIT(String.Format("i = {0} - {1}", i, oHelper.rLeagueParmrow("Teams")))
                            If i = oHelper.rLeagueParmrow("Teams") Then
                                i = 1
                                ii += 1
                                'rounds 2
                                If ii <= 2 Then sline &= ","
                            End If
                            If row(col) IsNot DBNull.Value Then
                                sline = sline & row(col) & ","
                                i += 1
                            End If
                        Next
                        sw.WriteLine(sline)
                    Next
                    sw.Close()

                Catch ex As Exception
                    Dim x = ""
                End Try

            End Using
            If Not Debugger.IsAttached Then
                Dim sHtml As String = oHelper.Create_Html_From_DGV(dgStandings)
                sHtml = oHelper.ConvertDataGridViewToHTMLWithFormatting(dgStandings, Me)
                semailfile = semailfile.Replace("csv", "html")
                Dim swhtml As New IO.StreamWriter(semailfile, False)
                swhtml.WriteLine(sHtml)
                swhtml.Close()
            End If
            If semailfile <> Nothing Then btnEmail.Visible = True
        Catch ex As Exception
            buildEmailFile = False
            If Not ex.Message.ToUpper.Contains("CANNOT ACCESS THE FILE") Then MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function

    Function BuilddtPoints() As Data.DataTable
        Return BuilddtPoints(False)
    End Function
    Function BuilddtPoints(email As Boolean) As Data.DataTable
        BuilddtPoints = Nothing
        Try
            sEndDate = oHelper.rLeagueParmrow("EndDate")

            '2019-11-23 - built using schedule rebuilt for a row for each date
            Dim dtStandings As New DataTable
            dtStandings.TableName = "dtStandings"
            'only build the schedule up thru the end date, this eliminates post season tournament from schedule
            For Each row In oHelper.dsLeague.Tables("dtSchedule").Rows
                If row("Date") <= sEndDate.ToString("yyyyMMdd") Then
                    dtStandings.Columns.Add(row("Date").ToString.Substring(4, 2) & "/" & row("Date").ToString.Substring(6, 2))
                End If
            Next

            iNumWeeksSplit = dtStandings.Columns.Count / 2

            sHalfwayDate = dtStandings.Columns.Item(iNumWeeksSplit).ToString
            With dtStandings
                .Columns.Add("Rnds", GetType(Integer)).SetOrdinal(0)
                .Columns.Add("Grade", GetType(String)).SetOrdinal(0)
                .Columns.Add("Team", GetType(Integer)).SetOrdinal(0)
                .Columns.Add("Player", GetType(String)).SetOrdinal(0)
                .Columns.Add("Team 1st Half", GetType(String)).SetOrdinal(iNumWeeksSplit + 4)
                .Columns.Add("1st Half", GetType(String)).SetOrdinal(iNumWeeksSplit + 4)
                .Columns.Add("2nd Half", GetType(String))
                .Columns.Add("Team 2nd Half", GetType(String))
                .Columns.Add("Total", GetType(String))
                .Columns.Add("WKY Points", GetType(String))
                '.Columns.Add("YTD Points", GetType(String))
                .DefaultView.Sort = "Team, Grade"
            End With

            dtStandings.PrimaryKey = New DataColumn() {dtStandings.Columns("Player"), dtStandings.Columns("Team")}
            'If Not oHelper.dsLeague.Tables.Contains("dtScores") Then oHelper.dsLeague.Tables.Add("dtScores").ReadXml(oHelper.getLatestFile("*Scores.xml"))
            If Not oHelper.dsLeague.Tables.Contains("dtScores") Then oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvScores.RowFilter += String.Format("date >= {0} and date <= {1}", CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), oHelper.dDate.ToString("yyyyMMdd"))
            'dvScores.Sort = "Team,Date,Grade"
            Dim sPoints = "", sTeamPoints = ""
            Try

                For Each score As DataRowView In dvScores
                    oHelper.sPlayer = score("Player")

                    'Dim sScore = IIf(score("Out_Gross") Is DBNull.Value, score("In_Gross"), score("Out_Gross"))
                    Dim sScore As String = ""
                    Try
                        sScore = IIf(oHelper.convDBNulltoSpaces(score("Out_Gross")).Trim = "", oHelper.convDBNulltoSpaces(score("In_Gross")).Trim, oHelper.convDBNulltoSpaces(score("Out_Gross")).Trim)
                    Catch ex As Exception

                    End Try
                    If sScore = "" Then sScore = "NS"
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
                        drow("WKY Points") = 0
                        'drow("YTD Points") = 0
                        dtStandings.Rows.Add(drow)
                    End If

                    If sScore <> "NS" Then drow("Rnds") += 1

                    Dim dPoints As Decimal = 0.00
                    Dim dTeamPoints As Decimal = 0.00
                    Dim dWeeklyPoints As Decimal = 0.00
                    Try
                        If score("Points") IsNot DBNull.Value Then dPoints = score("Points")
                    Catch ex As Exception

                    End Try
                    Try
                        If IsNumeric(score("Team_Points")) Then dTeamPoints = score("Team_Points")
                    Catch ex As Exception

                    End Try

                    'Dim sColname As String = CInt(score("Date").ToString.Substring(4, 2)).ToString + "/" + CInt(score("Date").ToString.Substring(6, 2)).ToString + "/" + score("Date").ToString.Substring(0, 4)
                    Dim sColname As String = score("Date").ToString.Substring(4, 2) + "/" + score("Date").ToString.Substring(6, 2) '+ "/" + score("Date").ToString.Substring(0, 4)
                    drow(sColname) = ""
                    'default to points and team points 
                    sPoints = dPoints
                    'get rid of leading zero in front of decimal point
                    If dPoints < 1 Then sPoints = sPoints.Replace("0.", ".")
                    sTeamPoints = dTeamPoints
                    If dTeamPoints < 1 Then sTeamPoints = sTeamPoints.Replace("0.", ".")
                    '20180302-if email run, dont add pts, tp, opp
                    If email Then
                        drow(sColname) = sScore & "-" & score("Hdcp")
                    Else
                        drow(sColname) = sPoints & "-" & sTeamPoints & "-" & sScore & "-" & score("Hdcp") & "-" & score("Opponent")
                    End If

                    'End If

                    'calculate team first half, second half, total points 
                    Dim x = CDate(sHalfwayDate & "/" & score("date").ToString.Substring(0, 4)).ToString("yyyyMMdd")
                    If score("date") < CDate(sHalfwayDate & "/" & score("date").ToString.Substring(0, 4)).ToString("yyyyMMdd") Then
                        drow("1st Half") += dPoints
                        drow("Team 1st Half") += dTeamPoints
                    Else
                        drow("2nd Half") += dPoints
                        drow("Team 2nd Half") += dTeamPoints
                    End If
                    drow("Total") += dPoints
                    If score("date") = oHelper.dDate.ToString("yyyyMMdd") And score("date") <= CDate(oHelper.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
                        drow("WKY Points") += dTeamPoints
                        drow("WKY Points") += dPoints
                    End If

                    'drow("YTD Points") += dTeamPoints
                Next

            Catch ex As Exception

            End Try
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
        Dim label As New System.Windows.Forms.Label
        label.Text = Groupbox.Text
        Groupbox.Text = ""
        label.Left = Groupbox.Left + (Groupbox.Width - label.Width) / 2
        label.Top = Groupbox.Top + 2 ' // 2 Is an example : adjust the constant
        label.Parent = Groupbox.Parent
        label.BringToFront()
    End Sub

    'Private Sub btnStandings_Click(sender As Object, e As EventArgs)
    '    doStandings()
    'End Sub
    Private Sub dgStandings_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgStandings.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'If MousePosition.Y < 0 Then
        'If sender.mouseenteredcelladdress < 0 Then
        '    Exit Sub
        'End If

        If e.ColumnIndex = 0 Then
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            If cell.OwningColumn.Name = "Player" Then
                'Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
                'If mbResult = MsgBoxResult.Yes Then
                oHelper.bScoresbyPlayer = True
                oHelper.sPlayer = cell.Value
                Scores.Show()
                oHelper.bScoresbyPlayer = False
                'End If
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
        'rs.ResizeAllControls(Me)
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

    Private Sub dgStandings_ColumnDividerDoubleClick(sender As Object, e As DataGridViewColumnDividerDoubleClickEventArgs) Handles dgStandings.ColumnDividerDoubleClick
        Dim x = ""
    End Sub

    Private Sub btnEmail_Click(sender As Object, e As EventArgs) Handles btnEmail.Click

        Dim ToAddresses As New List(Of String)  '{"garyrscudder@gmail.com", "garyrscudder@gmail.com"}
        For Each player In oHelper.dsLeague.Tables("dtPlayers").Rows
            If player("Name").ToString.Contains("Carr") Then
                Dim x = player("Name")
            End If

            If oHelper.convDBNulltoSpaces(player("Email")).Trim <> "" Then
                'If player("Name") = "Gary Scudder" Then 'Or player("Name") = "Greg Lemker" Then
                ToAddresses.Add(player("Email"))
                oHelper.LOGIT(String.Format("Email to {0}", player("Email")))
                'End If
            End If
        Next

        Dim mbr = MsgBox(String.Format("are you ready to sent emails to {0} players?", ToAddresses.Count), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then Exit Sub

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
    Private Sub dgStandings_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgStandings.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)

        lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
    End Sub
    Private Sub dgStandings_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgStandings.SortCompare

        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub

    'Private Sub CbExcel_CheckedChanged(sender As Object, e As EventArgs) Handles cbExcel.CheckedChanged
    '    Try
    '        Dim oXL As Microsoft.Office.Interop.Excel.Application
    '        Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
    '        Dim oWB As Microsoft.Office.Interop.Excel.Workbook
    '        Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
    '        Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
    '        Dim oRG As Microsoft.Office.Interop.Excel.Range
    '        oXL = New Microsoft.Office.Interop.Excel.Application
    '        oXL.Visible = False
    '        oYB = oXL.Workbooks

    '        oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20181010_Payments.xlsx")
    '        oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20181010_Payments.xls")
    '        oWB = oYB.Item(1)
    '        oSheets = oWB.Worksheets
    '        oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
    '        oSheet.Name = "Sheet1"
    '        Dim xxx = ""
    '    Catch ex As Exception
    '        MsgBox("Excel not on this computer")
    '    End Try

    '    Dim x = ""
    'End Sub

    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        oHelper.dDate = Date.ParseExact(cbDates.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        doStandings()
    End Sub

    Private Sub btnDisplay_Click(sender As Object, e As EventArgs) Handles btnDisplay.Click
        doStandings()
    End Sub
End Class