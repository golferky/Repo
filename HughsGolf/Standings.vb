Imports System.Text
Imports System.Data
Imports GolfManager.Helper
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
        LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
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

        If ctx.rLeagueParmrow("SplitSeason") <> "Y" Then
            cb1stHalf.Visible = False
            cb1stHalf.Checked = False
            cb2ndHalf.Visible = False
            cb2ndHalf.Checked = False
        End If
        dgStandings.Visible = False
        btnEmail.Visible = False
        dtSchedule = oHelper.sqliteda(ctx.Conn, "Schedule")
        'If cbDates.Items.Contains(oHelper.dDate.ToString("yyyyMMdd")) Then cbDates.SelectedIndex = cbDates.Items.IndexOf(oHelper.dDate.ToString("yyyyMMdd"))
        lbStatus.Text = String.Format("Loading Scores")
        oHelper.status_Msg(lbStatus, Me)
        cbDates.Items.AddRange(Main.cbDates.Items.Cast(Of String).ToArray)
        'remove non-match dates from dates combobox
        'cdateToyyyyMMdd converts a string from 1/1/1900 to 19000101
        Do While cbDates.Items(0) >= oHelper.CDateToyyyyMMdd(ctx.rLeagueParmrow("PostSeasonDt")) ' CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
            cbDates.Items.Remove(cbDates.Items(0))
        Loop
        'select the date from the main screen unless it was not a match date
        If cbDates.Items.Contains(ctx.ActiveDate) Then
            cbDates.SelectedItem = ctx.ActiveDate
        Else
            cbDates.SelectedItem = cbDates.Items(0)
        End If

        lbStatus.Text = String.Format("Finished Loading Scores")
        oHelper.status_Msg(lbStatus, Me)

    End Sub
    Sub doStandings()
        'Sub BldStandings()
        Try
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
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
                    LOGIT(String.Format("--before--team {2} 2nd half {0} team 2nd half {1} ", row("2nd Half"), row("Team 2nd Half"), row("Team")))
                    drow("Team 2nd Half") += CDec(row("2nd Half")) + CDec(row("Team 2nd Half"))
                    'drow("YTD Points") += CDec(row("2nd Half")) + CDec(row("Team 2nd Half"))
                    LOGIT(String.Format("--after--2nd half {0} team 2nd half {1}", row("2nd Half"), row("Team 2nd Half")))
                End If
            Next

            'dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}

            'now lets create a standings table based on checkboxes and radio buttons
            For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtStandings.Columns(index).ColumnName
                'LOGIT(String.Format("processing index {0}, name {1}", index, columnName))
                If Not cb1stHalf.Checked Then
                    If columnName.Contains("/") Then
                        If CDate(columnName).ToString("yyyyMMdd") <= CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                            'LOGIT(String.Format("not 1st half, removing index {0}, name {1}", index, columnName))
                            dtStandings.Columns.RemoveAt(index)
                            Continue For
                        End If
                    End If
                    If columnName.Contains("Team 1st Half") Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
                    End If
                End If

                If Not cb2ndHalf.Checked Then
                    If columnName.Contains("/") Then
                        If CDate(columnName).ToString("yyyyMMdd") > CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                            'LOGIT(String.Format("not 2nd half, removing index {0}, name {1}", index, columnName))
                            dtStandings.Columns.RemoveAt(index)
                            Continue For
                        End If
                    End If
                    If columnName.Contains("Team 2nd Half") Then
                        dtStandings.Columns.RemoveAt(index)
                        Continue For
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

            Dim dv As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dv.Sort = "Date desc"
            Dim sldate As String = dv(0)("Date")
            If dv(0)("Out_Gross") Is DBNull.Value And dv(0)("Out_Gross") Is DBNull.Value Then
                dv.RowFilter = String.Format("Date < {0}", sldate)
                sldate = dv(0)("Date")
            End If
            Dim sldatefmt As String = sldate.Substring(4, 2) & "/" & sldate.Substring(6, 2)
            dtStandings.PrimaryKey = New DataColumn() {dtStandings.Columns("Player"), dtStandings.Columns("Team")}
            dtSchedule.PrimaryKey = New DataColumn() {dtSchedule.Columns("Date")}

            '20200723 add code to show opponent
            For Each row In dtStandings.Rows

                Dim splayer = row("Player")
                Dim xplayer = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(row("Player"))
                If xplayer("Team") Is DBNull.Value Then
                    Continue For
                End If
                For Each col As DataColumn In dtStandings.Columns
                    If col.ColumnName.Contains("/") Then
                        If row(col) IsNot DBNull.Value Then
                            If row(col).ToString.Contains("NS-") Then
                                row(col) = DBNull.Value
                            End If
                        End If

                        If row(col) Is DBNull.Value Then
                            Debug.Print(cbDates.SelectedItem.ToString.Substring(0, 4))
                            Debug.Print("")
                            Dim sYear As String = cbDates.SelectedItem.ToString.Substring(0, 4)
                            Dim sMonth = col.ToString.Substring(0, 2)
                            Dim sDay = col.ToString.Substring(3, 2)
                            Dim schrow = dtSchedule.Rows.Find(sYear & col.ToString.Substring(0, 2) & col.ToString.Substring(3, 2))
                            For Each match As String In schrow.ItemArray
                                Debug.Print(match)
                                If match.Contains("v") Then
                                    If row("Team") = match.Split("v")(0) Then
                                        Debug.Print("")
                                        For Each splayer In oHelper.dsLeague.Tables("dtPlayers").Rows
                                            If splayer("Team") = match.Split("v")(1) And splayer("Grade") = row("Grade") Then
                                                row(col) = splayer("Name")
                                                Exit For
                                            End If
                                        Next
                                    ElseIf row("Team") = match.Split("v")(1) Then
                                        Debug.Print("")
                                        'row(col) = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(match.Split("v")(0))
                                        For Each splayer In oHelper.dsLeague.Tables("dtPlayers").Rows
                                            If splayer("Team") = match.Split("v")(0) And splayer("Grade") = row("Grade") Then
                                                row(col) = splayer("Name")
                                                Exit For
                                            End If
                                        Next
                                        Debug.Print(row(col))
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next
            Next

            Dim dvStandings As New DataView(dtStandings)

            Dim i = 0
            Try
                'Dim sPrvTeam As String = "", bAplayer As Boolean = False, bBplayer As Boolean = False
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
                    If oHelper.dDate.ToString("yyyyMMdd") <= CDate(ctx.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
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
            'test
            'Dim sproc = FindOpenedWorkBooks(semailfile)

            'Dim sproc = oHelper.getFileProcesses("Excel.exe")
            'oHelper.getFileProcesses("Excel")

            If IO.File.Exists(semailfile) Then
                IO.File.Delete(semailfile)
            End If

            Using sw As New IO.StreamWriter(semailfile, False)
                Try

                    Dim sb1 As New System.Text.StringBuilder
                    Dim sb2 As New System.Text.StringBuilder
                    Dim sprevteam As String = ""
                    'this loop will build column headings for each round
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
                    Dim slistsch As List(Of String)
                    slistsch = New List(Of String)
                    '20190628 this adds the schedule to the csv file
                    For Each row As DataRow In oHelper.dsLeague.Tables("dtSchedule").Rows
                        'row ctr
                        Dim ii = 1
                        'col ctr
                        Dim i = 1
                        sline = ",,"

                        For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                            'check if end of week 1
                            If i = ctx.rLeagueParmrow("Teams") Then
                                i = 1
                                ii += 1
                                'rounds 2
                                If ii <= 2 Then sline &= ","
                            End If
                            If row(col) IsNot DBNull.Value Then
                                If i = 1 Then
                                    sline = sline & row(col).ToString.Substring(4, 2) & "-" & row(col).ToString.Substring(6, 2) & ","
                                Else
                                    sline = sline & row(col) & ","
                                End If
                                i += 1
                            End If
                        Next
                        slistsch.Add(sline)
                    Next

                    sw.WriteLine(",,,,,,,,Schedule")
                    For i = 0 To ((slistsch.Count - 2) / 2) - 1
                        Debug.Print(slistsch(i) & slistsch(i + iNumWeeksSplit).Replace(",,", ","))
                        sw.WriteLine(slistsch(i) & slistsch(i + iNumWeeksSplit).Replace(",,", ","))
                    Next
                    sw.WriteLine("")
                    'post season
                    For i = slistsch.Count - 2 To slistsch.Count - 1
                        Debug.Print(slistsch(i) & "League Tournament")
                        sw.WriteLine(slistsch(i) & "League Tournament")
                    Next
                    sw.Close()

                Catch ex As Exception
                    Dim x = ""
                End Try

            End Using

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
            sEndDate = ctx.rLeagueParmrow("EndDate")
            Dim sStartDate As Date = ctx.rLeagueParmrow("StartDate")
            '2019-11-23 - built using schedule rebuilt for a row for each date
            Dim dtStandings As New DataTable
            dtStandings.TableName = "Standings"
            'only build the schedule up thru the end date, this eliminates post season tournament from schedule
            For Each row In dtSchedule.Rows
                If row("Date") <= sEndDate.ToString("yyyyMMdd") And row("Date") >= sStartDate.ToString("yyyyMMdd") Then
                    dtStandings.Columns.Add($"{row("Date").ToString.Substring(4, 2)}/{row("Date").ToString.Substring(6, 2)}")
                End If
            Next

            iNumWeeksSplit = dtStandings.Columns.Count / 2

            sHalfwayDate = dtStandings.Columns.Item(iNumWeeksSplit - 1).ToString
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
            Dim sb = New StringBuilder
            sb.AppendLine($"
SELECT '' SortOrder,A.Player APlayer, B.Player BPlayer,Rpt.*,'' PtsBack from (
SELECT IFNULL(T.Team,S.Team) Team, SUM(CAST(Points AS REAL))+SUM(CAST(Team_Points AS REAL)) Points 
FROM Matches M
LEFT JOIN Teams T ON T.Year = SUBSTRING(M.Date,1,4) AND M.Player = t.Player 
LEFT JOIN Subs S ON S.Date = M.Date AND S.Player = M.Player
WHERE M.Date >= {sStartDate:yyyyMMdd} AND M.Date <= {sEndDate:yyyyMMdd} 
GROUP BY IFNULL(T.Team,S.Team)
) Rpt
JOIN Teams A on A.Team = Rpt.Team AND A.Year = '{ctx.rLeagueParmrow("Season")}' AND A.Grade = 'A'
JOIN Teams B on B.Team = Rpt.Team AND B.Year = '{ctx.rLeagueParmrow("Season")}' AND B.Grade = 'B'
ORDER BY Points DESC
    ")
            Dim dtBPlayers = oHelper.sqlitedaFromSql(ctx.Conn, "", $"SELECT Team,Player from Teams WHERE Year = {sStartDate:yyyy} and Grade = 'B'")
            dtBPlayers.PrimaryKey = New DataColumn() {dtBPlayers.Columns(Constants.Team)}
            oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "Points", sb.ToString)
            Dim firstPlacePoints = oHelper.dt.Rows(0)(Constants.points)
            For Each row As DataRow In oHelper.dt.Rows
                'row("BPlayer") = dtBPlayers.Rows.Find(row(Team))(Player)
                If firstPlacePoints Is DBNull.Value Or row(Constants.points) Is DBNull.Value Then Continue For
                row("PtsBack") = firstPlacePoints - row(Constants.points)
            Next

            'Dim dvScores As New DataView(oHelper.sqlitedaFromSql(ctx.Conn, "Scores", $"SELECT * FROM Scores WHERE Date >= {sStartDate:yyyyMMdd} and Date <= {sEndDate:yyyyMMdd}"))
            ctx.ActiveDate = $"{sEndDate:yyyyMMdd}"
            Dim dvScores = New DataView(oHelper.sqlitedaFromSql(ctx.Conn, "", BuildSqlForMatches()))
            'dvScores.Sort = "Team,Date,Grade"
            Dim sPoints = "", sTeamPoints = ""
            Try

                For Each score As DataRowView In dvScores
                    oHelper.sPlayer = score("Player")

                    'Dim sScore = IIf(score("Out_Gross") Is DBNull.Value, score("In_Gross"), score("Out_Gross"))
                    Dim sScore As String = ""
                    Try
                        sScore = oHelper.convDBNulltoSpaces(score("Gross")).Trim
                    Catch ex As Exception

                    End Try
                    If sScore = "" Then
                        sScore = "NS"
                    End If
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
                    If score("date") <= CDate(sHalfwayDate & "/" & score("date").ToString.Substring(0, 4)).ToString("yyyyMMdd") Then
                        drow("1st Half") += dPoints
                        drow("Team 1st Half") += dTeamPoints
                    Else
                        drow("2nd Half") += dPoints
                        drow("Team 2nd Half") += dTeamPoints
                    End If
                    drow("Total") += dPoints
                    If score("date") = oHelper.dDate.ToString("yyyyMMdd") And score("date") <= CDate(ctx.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
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

    Function BuildSqlForMatches() As String
        Dim StartDate As String = $"{CDate(ctx.rLeagueParmrow("StartDate")):yyyyMMdd}"
        'Dim EndDate As String = $"{CDate(ctx.rLeagueParmrow("PostSeasonDt")).AddDays(14):yyyyMMdd}"
        Dim EndDate As String = $"{ctx.ActiveDate}"

        Dim strsql = New StringBuilder
        strsql.AppendLine($"SELECT ")
        '20250330 - this code put in because after 2024, earnings are all in the payments table
        strsql.AppendLine(
$"Ma.Player
,S.Date
,IFNULL(T.Team,Su.Team) Team
,IFNULL(T.Grade,Su.Grade) Grade
,IFNULL(M.Method,'Gross') Method")
        oHelper.CalcHoleMarker(ctx.ActiveDate)

        For i = 1 To CInt(ctx.rLeagueParmrow("Holes"))
            strsql.AppendLine($",S.[{i}] as [{i + oHelper.iHoleMarker}]")
        Next
        strsql.AppendLine(
$",S.Gross
,S.Net
,H.Phdcp
,H.Hdcp
,'' as Status
,'' as Team_Net
,'' as Points
,'' as Team_Points
,'' as Opponent ")
        strsql.AppendLine(
$"FROM [Matches] Ma
LEFT JOIN [LeagueParms] LP   ON CONCAT(LP.Name,LP.Season) = CONCAT(Ma.League,SUBSTRING(Ma.Date,1,4))
LEFT JOIN [Scores]      S    ON (S.League = Ma.League AND S.Date = Ma.Date AND S.Player = Ma.Player)
LEFT JOIN [Teams]       T    ON (T.League = Ma.League AND T.Year = SUBSTRING(Ma.Date,1,4) AND T.Player = Ma.Player)
LEFT JOIN [Handicaps]   H    ON (H.League = Ma.League AND H.Date = Ma.Date AND H.Player = Ma.Player)
LEFT JOIN [ScoreMethod] M    ON (H.League = Ma.League AND M.Date = Ma.Date AND M.Player = Ma.Player)
LEFT JOIN [Subs]        Su   ON (Su.League = Ma.League AND Su.Date = Ma.Date AND Su.Player = Ma.Player)
WHERE S.Date > {StartDate} and S.Date <= {EndDate}
ORDER BY Ma.Partner")
        BuildSqlForMatches = strsql.ToString
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

    'Private Sub btnStandings_Click(sender As Object, e As EventArgs)
    '    doStandings()
    'End Sub
    Private Sub dgStandings_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgStandings.CellMouseDoubleClick
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
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
                'Scores.Show()
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
        For Each sPlayer In oHelper.dsLeague.Tables("dtPlayers").Rows
            If sPlayer("Name").ToString.Contains("Carr") Then
                Dim x = Player("Name")
            End If

            If oHelper.convDBNulltoSpaces(sPlayer("Email")).Trim <> "" Then
                'If player("Name") = "Gary Scudder" Then 'Or player("Name") = "Greg Lemker" Then
                ToAddresses.Add(sPlayer("Email"))
                LOGIT(String.Format("Email to {0}", Player("Email")))
                'End If
            End If
        Next

        Dim mbr = MsgBox(String.Format("are you ready to sent emails to {0} players?", ToAddresses.Count), MsgBoxStyle.YesNo)
        If mbr <> MsgBoxResult.Yes Then Exit Sub
        Dim sfile As String
        sfile = $"{ctx.ReportPath}Stangings"
        sfile &= $"_{cbDates.SelectedItem}.xlsx"
        If IO.File.Exists(sfile) Then IO.File.Delete(sfile)
        Dim columnsToExclude As List(Of String) = New List(Of String)
        Dim oClosedXML As New ClosedXML
        oClosedXML.ExportDataGridViewToExcel(dgStandings, sfile, columnsToExclude)
        'lblMessage.Text = $"Exported to {sfile}"
        End
        ''Dim attachs() As String = {"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
        'Dim attachs() As String = {semailfile}
        'Dim subject As String = "Standings Sheet"
        'Dim body As String = semailfile
        'Dim bresult = False
        'If ToAddresses.Count > 0 Then
        '    bresult = oHelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
        '    If bresult Then
        '        MsgBox("mails sent successfully", MsgBoxStyle.Information)
        '    Else
        '        MsgBox(oHelper.GGmail.ErrorText, MsgBoxStyle.Critical)
        '    End If
        'End If

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Private Sub dgStandings_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgStandings.ColumnHeaderMouseClick
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

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

    Private Sub CbExcel_CheckedChanged(sender As Object, e As EventArgs) Handles cbExcel.CheckedChanged
        'Try
        '    Dim oXL As Microsoft.Office.Interop.Excel.Application
        '    Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
        '    Dim oWB As Microsoft.Office.Interop.Excel.Workbook
        '    Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
        '    Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
        '    Dim oRG As Microsoft.Office.Interop.Excel.Range
        '    oXL = New Microsoft.Office.Interop.Excel.Application
        '    oXL.Visible = False
        '    oYB = oXL.Workbooks

        '    oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20181010_Payments.xlsx")
        '    oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20181010_Payments.xls")
        '    oWB = oYB.Item(1)
        '    oSheets = oWB.Worksheets
        '    oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
        '    oSheet.Name = "Sheet1"
        '    Dim xxx = ""
        'Catch ex As Exception
        '    MsgBox("Excel not on this computer")
        'End Try

        Dim x = ""
    End Sub

    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        oHelper.dDate = Date.ParseExact(cbDates.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        doStandings()
    End Sub

    Private Sub btnDisplay_Click(sender As Object, e As EventArgs) Handles btnDisplay.Click
        doStandings()
    End Sub
End Class