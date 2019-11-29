Imports System.IO.Packaging
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
        Me.Width = sWH.Split(":")(0)
        Me.Height = sWH.Split(":")(1)

        If oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("SplitSeason") <> "Y" Then
            cb1stHalf.Visible = False
            cb1stHalf.Checked = False
            cb2ndHalf.Visible = False
            cb2ndHalf.Checked = False
        End If
        dgStandings.Visible = False
        btnEmail.Visible = False
        dtSchedule = oHelper.dsLeague.Tables("dtSchedule")
        If oHelper.rLeagueParmrow("ScoresLocked") = "Y" Then
            btnStandings_Click(sender, e)
            'Me.Close()
        End If
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
            'Dim dtPoints As DataTable = dvStandingsDGV.ToTable(True, sStdDGV.Split(",").ToArray)
            'end 20170720 future
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
                    'If columnName.Contains("1st Half") Then
                    '    dtStandings.Columns.RemoveAt(index)
                    '    Continue For
                    'End If
                    ' CDate(sHalfwayDate).ToString("yyyyMMdd")
                    If columnName.Contains("/") Then
                        If CDate(columnName).ToString("yyyyMMdd") < CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                            'oHelper.LOGIT(String.Format("not 1st half, removing index {0}, name {1}", index, columnName))
                            dtStandings.Columns.RemoveAt(index)
                            Continue For
                        End If
                    End If
                End If
                If Not cb2ndHalf.Checked Then
                    'If columnName.Contains("2nd Half") Then
                    '    dtStandings.Columns.RemoveAt(index)
                    '    Continue For
                    'End If
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
                    'If row.Item("Team") <> sPrvTeam Then
                    '    sPrvTeam = row.Item("Team")
                    '    bAplayer = False
                    '    bBplayer = False
                    'End If
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
                        .Columns(col.Name).Width = 40   'yep, make width 40
                        'If cbHdcp.Checked Then .Columns(col.Name).Width += 10   'if hdcp, adjust by 10
                        'If cbScore.Checked Then .Columns(col.Name).Width += 10  'if score, adjust by 10
                        .Columns(col.Name).HeaderText = col.Name.Substring(0, col.Name.LastIndexOf("/")) 'just use mm/dd
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
                    row.Cells("WKY Points").Value = drow(oHelper.dDate.ToShortDateString)
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

            Dim sHtml As String = oHelper.Create_Html_From_DGV(dgStandings)
            sHtml = ConvertDataGridViewToHTMLWithFormatting(dgStandings)
            semailfile = semailfile.Replace("csv", "html")
            Dim swhtml As New IO.StreamWriter(semailfile, False)
            swhtml.WriteLine(sHtml)
            swhtml.Close()
            If semailfile <> Nothing Then btnEmail.Visible = True
        Catch ex As Exception
            buildEmailFile = False
            If Not ex.Message.ToUpper.Contains("CANNOT ACCESS THE FILE") Then MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function

    'Function dt_buildEmailFile() As Boolean
    '    dt_buildEmailFile = True
    '    Try
    '        Dim dtStandings = New Data.DataTable
    '        dtStandings = BuilddtPoints(True)
    '        dtStandings.PrimaryKey = New DataColumn() {dtStandings.Columns("Team"), dtStandings.Columns("Player")}
    '        Dim dtStandingsForPts = New Data.DataTable
    '        dtStandingsForPts = BuilddtPoints()
    '        'build this table for points collection
    '        Dim dtPoints As Data.DataTable = dtStandings.Clone
    '        dtPoints.TableName = "dtPoints"
    '        dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns("Team")}
    '        For index As Integer = dtPoints.Columns.Count - 1 To 0 Step -1
    '            Dim columnName As String = dtPoints.Columns(index).ColumnName
    '            If columnName = "Player" Or columnName = "Grade" Or columnName = "Rnds" Or columnName = "1st Half" Or columnName = "2nd Half" Or columnName = "Total" Then
    '                dtPoints.Columns.RemoveAt(index)
    '                Continue For
    '            End If
    '            'If columnName.Contains("Team") Then columnName = columnName.Replace("Team ", "")
    '        Next

    '        Dim sprevteam = 0, sprevgrade = ""
    '        For Each row As DataRow In dtStandingsForPts.Rows
    '            Dim sTeam = row("Team")
    '            Dim sKeys() As Object = {row("Team")}
    '            Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
    '            If drow Is Nothing Then
    '                drow = dtPoints.NewRow
    '                drow("Team") = row("Team")
    '                For Each col As DataColumn In dtStandingsForPts.Columns
    '                    If col.ColumnName <> "Player" And col.ColumnName <> "Team" And col.ColumnName <> "Grade" And col.ColumnName <> "Rnds" And col.ColumnName <> "1st Half" And col.ColumnName <> "2nd Half" And col.ColumnName <> "Total" Then
    '                        drow(col.ColumnName) = 0
    '                    End If
    '                Next
    '                dtPoints.Rows.Add(drow)
    '            End If
    '            For Each col As DataColumn In dtStandingsForPts.Columns
    '                If col.ColumnName.Contains("/") Then
    '                    'accumulate points from gridview
    '                    drow(col.ColumnName) += getPts(row(col.ColumnName))
    '                End If
    '            Next
    '        Next
    '        Dim i = 0
    '        'has to be done from back to front because the collection numbers change from front to back
    '        For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
    '            Dim columnName As String = dtStandings.Columns(index).ColumnName
    '            'If columnName.Contains("Team") Then columnName = columnName.Replace("Team ", "")
    '            If columnName = "Team 1st Half" Or columnName = "Team 2nd Half" Or columnName = "Team Points" Then dtStandings.Columns.RemoveAt(index)
    '            If columnName = "Total" Then
    '                If Not cbTotalPts.Checked Then dtStandings.Columns.RemoveAt(index)
    '            End If
    '        Next

    '        'this loop will roll all team points into the first players totals and clear out the rest
    '        Dim dvStandings As New DataView(dtStandings)
    '        dvStandings.Sort = ("Team,Grade")
    '        Dim iwkly As Decimal = 0.0
    '        Dim saPlayer As String = ""
    '        For Each row As DataRowView In dvStandings
    '            oHelper.sPlayer = row("Player") & "-" & row("Grade")
    '            Dim sKeys() As Object = {row("Team")}
    '            Dim drow As DataRow = dtPoints.Rows.Find(sKeys)
    '            'If cb1stHalf.Checked Then row("1st Half") = ""
    '            'If cb2ndHalf.Checked Then row("2nd Half") = ""

    '            row("1st half") = 0
    '            row("2nd half") = 0
    '            If cbTotalPts.Checked Then row("Total") = 0

    '            If row("team") <> sprevteam Then
    '                Dim ipts As Decimal = 0.0
    '                Dim ifh As Decimal = 0.0
    '                Dim ish As Decimal = 0.0
    '                'total up halves
    '                For Each col As DataColumn In dtPoints.Columns
    '                    If col.ColumnName.Contains("/") Then
    '                        ipts += CDec(oHelper.convDBNulltoSpaces(drow(col.ColumnName)))
    '                        If CDate(col.ColumnName).ToString("yyyyMMdd") >= CDate(sHalfwayDate).ToString("yyyyMMdd") Then
    '                            ish += CDec(oHelper.convDBNulltoSpaces(drow(col.ColumnName)))
    '                        Else
    '                            ifh += CDec(oHelper.convDBNulltoSpaces(drow(col.ColumnName)))
    '                        End If
    '                    End If
    '                Next

    '                row("1st half") = ifh
    '                row("2nd half") = ish
    '                If cbTotalPts.Checked Then row("Total") = ipts
    '                If sprevteam <> 0 Then
    '                    Dim sdsAPlayerKey() As Object = {sprevteam, saPlayer}
    '                    Dim dsr As DataRow = dtStandings.Rows.Find(sdsAPlayerKey)
    '                    Dim sdpKey() As Object = {sprevteam}
    '                    Dim dprow As DataRow = dtPoints.Rows.Find(sdpKey)
    '                    dsr("WKY Points") = dprow(oHelper.dDate.ToShortDateString)
    '                    'End If
    '                    saPlayer = row("Player")
    '                Else
    '                    row("WKY Points") = ""
    '                End If
    '                sprevteam = row("team")
    '            End If
    '        Next

    '        Dim amonths As New List(Of String)("Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sept,Oct,Nov,Dec".Split(","))

    '        semailfile = oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    '        Using sw As New IO.StreamWriter(semailfile, False)
    '            Try

    '                Dim sb1 As New System.Text.StringBuilder
    '                Dim sb2 As New System.Text.StringBuilder
    '                For Each col As DataColumn In dtStandings.Columns
    '                    If col.ColumnName.Contains("/") Then
    '                        'example Apr/11
    '                        sb1.Append(amonths(Convert.ToDateTime(col.ColumnName).ToString("MM") - 1) & ",")
    '                        sb2.Append(Convert.ToDateTime(col.ColumnName).ToString("dd") & ",")
    '                    Else
    '                        Dim scols As New List(Of String)(col.ColumnName.Split(" "))
    '                        If scols.Count > 1 Then
    '                            sb1.Append(scols(0) & ",")
    '                            sb2.Append(scols(1) & ",")
    '                        Else
    '                            sb1.Append(",")
    '                            sb2.Append(scols(0) & ",")
    '                        End If
    '                    End If
    '                Next

    '                sw.WriteLine(sb1.ToString)
    '                sw.WriteLine(sb2.ToString)

    '                Dim sline = ""
    '                sprevteam = 0
    '                For Each row As DataRow In dtStandings.Rows
    '                    If row("Team") <> sprevteam Then
    '                        sline = "Team #" & row("Team")
    '                        sw.WriteLine(sline)
    '                        sprevteam = row("Team")
    '                    Else
    '                        row("WKY Points") = ""
    '                    End If
    '                    sline = ""
    '                    For Each item As DataColumn In dtStandings.Columns
    '                        sline = sline & oHelper.convDBNulltoSpaces(row(item).ToString) & ","
    '                    Next
    '                    sw.WriteLine(sline)
    '                Next
    '                sw.Close()

    '            Catch ex As Exception

    '            End Try

    '        End Using

    '        'Dim sHtml = oHelper.ConvertToHtmlFile(dtStandings)
    '        'sHtml = oHelper.ConvertToHTMLString(dtStandings)
    '        Dim sHtml As String = oHelper.Create_Html(dtStandings)
    '        sHtml = ConvertDataGridViewToHTMLWithFormatting(dgStandings)
    '        Dim swhtml As New IO.StreamWriter(semailfile.replace("csv", "html"), False)
    '        swhtml.WriteLine(sHtml)
    '        swhtml.Close()
    '        If semailfile <> Nothing Then btnEmail.Visible = True
    '    Catch ex As Exception
    '        dt_buildEmailFile = False

    '        If Not ex.Message.ToUpper.Contains("CANNOT ACCESS THE FILE") Then MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)

    '    End Try


    'End Function

    Function BuilddtPoints() As Data.DataTable
        Return BuilddtPoints(False)
    End Function
    Function BuilddtPoints(email As Boolean) As Data.DataTable
        BuilddtPoints = Nothing
        Try
            'copy the schedule table 
            Dim dtStandings As Data.DataTable = oHelper.dsLeague.Tables("dtSchedule").Clone()
            sEndDate = oHelper.rLeagueParmrow("EndDate").ToString

            '20180527-post season makes season 2 weeks earlier
            '20180930-dont adjust postseason Date
            'If oHelper.rLeagueParmrow("PostSeason").ToString.ToUpper = "Y" Then sEndDate = sEndDate.AddDays(-14)

            dtStandings.TableName = "dtStandings"
            'only build the schedule up thru the end date, this eliminates post season tournament from schedule
            For index As Integer = dtStandings.Columns.Count - 1 To 0 Step -1
                Dim columnName As String = dtStandings.Columns(index).ColumnName
                If columnName > sEndDate Or dtSchedule.Rows(0)(columnName) Is DBNull.Value Then
                    dtStandings.Columns.RemoveAt(index)
                End If
            Next

            iNumWeeksSplit = dtStandings.Columns.Count / 2
            '20180527-post season makes season 2 weeks earlier
            'If oHelper.rLeagueParmrow("PostSeason").ToString.ToUpper = "Y" Then iNumWeeksSplit -= 1

            sHalfwayDate = dtStandings.Columns.Item(iNumWeeksSplit).ToString
            '20190605 - hardcode first half date
            Dim x = ""
            'sHalfwayDate = "6/11/2019"
            With dtStandings
                .Columns.Add("Rnds", GetType(Integer)).SetOrdinal(0)
                .Columns.Add("Grade", GetType(String)).SetOrdinal(0)
                .Columns.Add("Team", GetType(Integer)).SetOrdinal(0)
                .Columns.Add("Player", GetType(String)).SetOrdinal(0)

                '20190605 - hardcode first half date 
                '.Columns.Add("Team 1st Half", GetType(String)).SetOrdinal(iNumWeeksSplit + 4)
                '.Columns.Add("1st Half", GetType(String)).SetOrdinal(iNumWeeksSplit + 4)

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
            'dvScores.RowFilter = "Date >= " & sEndDate.ToString("yyyyMMdd").Substring(0, 4) & "0101"
            'this gets a whole years worth of scores
            'dvScores.RowFilter += String.Format("date >= {0} and date <= {1}", sEndDate.ToString("yyyyMMdd").Substring(0, 4) & "0101", sEndDate.ToString("yyyMMdd").Substring(0, 4) + 1) & "0101"
            dvScores.RowFilter += String.Format("date >= {0} and date <= {1}", sEndDate.ToString("yyyyMMdd").Substring(0, 4) & "0101", sEndDate.ToString("yyyMMdd"))
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

                    Dim sColname As String = CInt(score("Date").ToString.Substring(4, 2)).ToString + "/" + CInt(score("Date").ToString.Substring(6, 2)).ToString + "/" + score("Date").ToString.Substring(0, 4)
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
                    If score("date") < CDate(sHalfwayDate).ToString("yyyyMMdd") Then
                        drow("1st Half") += dPoints
                        drow("Team 1st Half") += dTeamPoints
                    Else
                        drow("2nd Half") += dPoints
                        drow("Team 2nd Half") += dTeamPoints
                    End If
                    drow("Total") += dPoints
                    If score("date") = oHelper.dDate.ToString("yyyyMMdd") Then
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
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
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
#Region "Future Changes"

    Public Function ConvertDataGridViewToHTMLWithFormatting(ByVal dgv As DataGridView) As String
        ConvertDataGridViewToHTMLWithFormatting = ""
        Try

            Dim sb As StringBuilder = New StringBuilder()
            sb.AppendLine("<html><body><center><table border='1' cellpadding='0' cellspacing='0'>")
            sb.AppendLine("<tr>")
            For i As Integer = 0 To dgv.Columns.Count - 1
                sb.Append(DGVHeaderCellToHTMLWithFormatting(dgv, i))
                'sb.Append(DGVCellFontAndValueToHTML(dgv.Columns(i).HeaderText, dgv.Columns(i).HeaderCell.Style.Font))
                sb.Append(DGVCellFontAndValueToHTML(dgv.Columns(i).HeaderText, dgStandings.ColumnHeadersDefaultCellStyle.Font))
                sb.AppendLine("</td>")
            Next

            sb.AppendLine("</tr>")
            For rowIndex As Integer = 0 To dgv.Rows.Count - 1
                sb.AppendLine("<tr>")
                For Each dgvc As DataGridViewCell In dgv.Rows(rowIndex).Cells
                    sb.AppendLine(DGVCellToHTMLWithFormatting(dgv, rowIndex, dgvc.ColumnIndex))
                    Dim cellValue As String = If(dgvc.Value Is Nothing, String.Empty, dgvc.Value.ToString())
                    sb.AppendLine(DGVCellFontAndValueToHTML(cellValue, dgvc.Style.Font))
                    sb.AppendLine("</td>")
                Next

                sb.AppendLine("</tr>")
            Next

            sb.AppendLine("</table></center></body></html>")
            Return sb.ToString()
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function

    Public Function DGVHeaderCellToHTMLWithFormatting(ByVal dgv As DataGridView, ByVal col As Integer) As String
        Dim sb As StringBuilder = New StringBuilder()
        sb.Append("<td")
        sb.Append(" width=" & Chr(34) & dgv.Columns(col).Width & Chr(34) & " ")
        'Dim sHdr = dgv.Columns(col).HeaderText
        'If sHdr.Contains("Half") Or sHdr.Contains("Points") Or sHdr.Contains("/") Then
        '    sb.Append(" width=" & Chr(34) & "40" & Chr(34) & " ")
        'End If

        sb.Append(DGVCellColorToHTML(dgv.Columns(col).HeaderCell.Style.ForeColor, dgv.Columns(col).HeaderCell.Style.BackColor))
        sb.Append(DGVCellAlignmentToHTML(dgv.Columns(col).HeaderCell.Style.Alignment))
        sb.Append(">")
        Return sb.ToString()
    End Function

    Public Function DGVCellToHTMLWithFormatting(ByVal dgv As DataGridView, ByVal row As Integer, ByVal col As Integer) As String
        Dim sb As StringBuilder = New StringBuilder()
        sb.Append("<td")
        If dgv.Rows(row).DefaultCellStyle.BackColor <> DefaultBackColor Then
            sb.Append(DGVCellColorToHTML(dgv.Rows(row).DefaultCellStyle.ForeColor, dgv.Rows(row).DefaultCellStyle.BackColor))
        Else
            sb.Append(DGVCellColorToHTML(dgv.Rows(row).Cells(col).Style.ForeColor, dgv.Rows(row).Cells(col).Style.BackColor))
        End If

        sb.Append(DGVCellAlignmentToHTML(dgv.Rows(row).Cells(col).Style.Alignment))
        sb.Append(">")
        Return sb.ToString()
    End Function

    Public Function DGVCellColorToHTML(ByVal foreColor As Color, ByVal backColor As Color) As String
        If foreColor.Name = "0" AndAlso backColor.Name = "0" Then Return String.Empty
        Dim sb As StringBuilder = New StringBuilder()
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
        Return sb.ToString()
    End Function

    Public Function DGVCellFontAndValueToHTML(ByVal value As String, ByVal font As Font) As String
        'If font Is Nothing OrElse font = Me.Font AndAlso Not (font.Bold Or font.Italic Or font.Underline Or font.Strikeout) Then Return value
        If font Is Nothing OrElse Not (font.Bold Or font.Italic Or font.Underline Or font.Strikeout) Then Return value
        Dim sb As StringBuilder = New StringBuilder()
        sb.Append(" ")
        If font.Bold Then sb.Append("<b>")
        If font.Italic Then sb.Append("<i>")
        If font.Strikeout Then sb.Append("<strike>")
        If font.Underline Then sb.Append("<u>")
        Dim size As String = String.Empty
        If font.Size <> Me.Font.Size Then size = "font-size: " & font.Size & "pt;"
        If font.FontFamily.Name <> Me.Font.Name Then
            sb.Append("<span style=""font-family: ")
            sb.Append(font.FontFamily.Name)
            sb.Append("; ")
            sb.Append(size)
            sb.Append(""">")
        End If

        sb.Append(value)
        If font.FontFamily.Name <> Me.Font.Name Then sb.Append("</span>")
        If font.Underline Then sb.Append("</u>")
        If font.Strikeout Then sb.Append("</strike>")
        If font.Italic Then sb.Append("</i>")
        If font.Bold Then sb.Append("</b>")
        Return sb.ToString()
    End Function

    Public Function DGVCellAlignmentToHTML(ByVal align As DataGridViewContentAlignment) As String
        If align = DataGridViewContentAlignment.NotSet Then Return String.Empty
        Dim horizontalAlignment As String = String.Empty
        Dim verticalAlignment As String = String.Empty
        CellAlignment(align, horizontalAlignment, verticalAlignment)
        Dim sb As StringBuilder = New StringBuilder()
        sb.Append(" align='")
        sb.Append(horizontalAlignment)
        sb.Append("' valign='")
        sb.Append(verticalAlignment)
        sb.Append("'")
        Return sb.ToString()
    End Function

    Private Sub CellAlignment(ByVal align As DataGridViewContentAlignment, ByRef horizontalAlignment As String, ByRef verticalAlignment As String)
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
    End Sub

    Private Sub CbExcel_CheckedChanged(sender As Object, e As EventArgs) Handles cbExcel.CheckedChanged
        Try
            Dim oXL As Microsoft.Office.Interop.Excel.Application
            Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
            Dim oWB As Microsoft.Office.Interop.Excel.Workbook
            Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
            Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
            Dim oRG As Microsoft.Office.Interop.Excel.Range
            oXL = New Microsoft.Office.Interop.Excel.Application
            oXL.Visible = False
            oYB = oXL.Workbooks

            oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20181010_Payments.xlsx")
            'oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20181010_Payments.xls")
            oWB = oYB.Item(1)
            oSheets = oWB.Worksheets
            oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
            oSheet.Name = "Sheet1"
            Dim xxx = ""
        Catch ex As Exception
            MsgBox("Excel not on this computer")
        End Try

        Dim x = ""
    End Sub


#End Region
End Class