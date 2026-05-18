Imports System.IO
Imports System.Windows.Forms.AxHost
Imports ClosedXML.Excel
Imports iText.Forms

'Imports Microsoft.Office.Interop.Excel
Public Class Leaders
    'Dim oHelper As New LeagueManager.Helper
    Dim oHelper As New Helper
    Dim rs As New Resizer
    Dim sColFormat = New List(Of String)
    Dim dtScoreCard As Data.DataTable
    Dim dtStats As Data.DataTable
    Dim dtLeaderStats As DataTable
    Dim dtRegulars As DataTable
    Dim LeaderCourses As DataTable
    Dim AllGrossScores As DataTable

    Dim sStatsDesc As String() = {"Eagles", "Birdies", "Pars", "Bogeys", "DoubleBogeys", "Others"}
    Dim sScoreOnlyDesc As String() = {"Rnds", "F9", "B9", "Out_Gross", "Out_Net", "In_Gross", "In_Net"}
    Dim sWH As String = ""
    Dim bload As Boolean = False
    Dim sStats = New List(Of String)
    Dim slstFCTPS = New List(Of String)
    Dim slstBCTPS = New List(Of String)
    Dim sPar3s As List(Of String) = New List(Of String)
    Dim oClosedXML As New ClosedXML
    Private Sub Leaders_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        sPar3s = Main.lPar3s
        sWH = oHelper.ScreenResize()
        If Me.Width >= sWH.Split(":")(0) Then
            Me.Width = sWH.Split(":")(0) - (sWH.Split(":")(0) * 0.1)
        Else
            'Me.Width = sWH.Split(":")(0)
        End If
        If Me.Height >= sWH.Split(":")(1) Then
            Me.Height = sWH.Split(":")(1) - (sWH.Split(":")(1) * 0.1)
        Else
            'Me.Height = sWH.Split(":")(1)
        End If
        Me.AutoScroll = True
        dtRegulars = oHelper.sqlitedaFromSql(ctx.Conn, "", "SELECT * FROM Teams")
        dtRegulars.PrimaryKey = New DataColumn() {dtRegulars.Columns("Player"), dtRegulars.Columns("Year")}

        'create a temp datatable to hold the course data
        oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", $"SELECT * FROM Courses WHERE Name = '{ctx.thiscourse("Name")}'")
        LeaderCourses = New DataTable
        LeaderCourses.Columns.Add($"Name")
        LeaderCourses.Columns.Add($"Par")
        'setup front 9
        For i = 1 To 9
            LeaderCourses.Columns.Add($"Hole{i}")
        Next
        For i = 1 To 9
            LeaderCourses.Columns.Add($"H{i}")
        Next
        Dim newrow As DataRow

        newrow = LeaderCourses.NewRow
        newrow("Name") = "Front"
        newrow("Par") = oHelper.dt(0)("In")
        For i = 1 To 9
            Dim hval = oHelper.dt(0)($"H{i}") - 9
            newrow($"Hole{i}") = oHelper.dt(0)($"Hole{i}")
            newrow($"H{i}") = If(hval > 0, hval, oHelper.dt(0)($"H{i}"))
        Next
        LeaderCourses.Rows.Add(newrow)

        newrow = LeaderCourses.NewRow
        newrow("Name") = "Back"
        newrow("Par") = oHelper.dt(0)("Out")
        For i = 1 To 9
            Dim hval = oHelper.dt(0)($"H{i + 9}") - 9
            newrow($"Hole{i}") = oHelper.dt(0)($"Hole{i + 9}")
            newrow($"H{i}") = If(hval > 0, hval, oHelper.dt(0)($"H{i + 9}"))
        Next
        LeaderCourses.TableName = "Courses"
        LeaderCourses.Rows.Add(newrow)
        LeaderCourses.PrimaryKey = New DataColumn() {LeaderCourses.Columns("Name")}
        Dim sb = New Text.StringBuilder
        sb.AppendLine($"
SELECT * FROM Scores S
LEFT JOIN ScoreMethod SM ON S.League||S.Player||S.Date = SM.League||SM.Player||SM.Date
")
        AllGrossScores = oHelper.sqliteda(ctx.Conn, "Scores")
        AllGrossScores.PrimaryKey = New DataColumn() {AllGrossScores.Columns("League"), AllGrossScores.Columns("Player"), AllGrossScores.Columns("Date")}
        oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", "SELECT * from ScoreMethod WHERE Date > 20171231")
        Dim thisCourse = LeaderCourses.Rows.Find(AllGrossScores(0)("FrontBack"))
        For Each row In oHelper.dt.Rows
            Dim drfind As DataRow = AllGrossScores.Rows.Find({row("League"), row("Player"), row("Date")})
            Dim drold As DataRow = AllGrossScores.Rows.Find({row("League"), row("Player"), row("Date")})
            oHelper.wk = ""
            For i = 1 To 9
                oHelper.wk &= drold($"{i}")
            Next
            Debug.Print($"OLD-{oHelper.wk}")

            If drfind IsNot Nothing Then
                If row("Method") = "Net" Then
                    For i = 1 To 9
                        Dim ihdcp As Integer = drold("Gross") - drold("Net")
                        If drold("Player").ToString.Contains("Eric") Then 'And i = 2 Then
                            oHelper.wk = ""
                        End If

                        If thisCourse($"H{i }") <= ihdcp Then
                            drfind($"{i}") += 1
                        End If
                        ihdcp -= 9
                        If ihdcp > 0 And thisCourse($"H{i }") <= ihdcp Then
                            drfind($"{i}") += 1
                        End If
                    Next
                End If
                If drold("Player").ToString.Contains("Eric") Then
                    oHelper.wk = ""
                    For i = 1 To 9
                        oHelper.wk &= drfind($"{i}")
                    Next
                    Debug.Print($"New-{oHelper.wk}")
                    oHelper.wk = ""
                End If

            End If
        Next

        oHelper.SQLiteCreateTableFromDT(ctx.Conn, LeaderCourses)
        oHelper.dt = oHelper.sqliteda(ctx.Conn, "Courses")
        oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", $"SELECT CF.Hole1,S.[1],* FROM Scores S JOIN Courses CF ON Name = 'Front' ")
        oHelper.SQLiteCreateTableFromDT(ctx.Conn, AllGrossScores)
        oHelper.dt = oHelper.sqliteda(ctx.Conn, "Scores")

        GoTo skip
#Region "Skip"
        'one time fix for CTP / Skin Payouts
        sb = New Text.StringBuilder
        sb.AppendLine($"
SELECT * FROM Earnings E
JOIN Scores S ON S.League = E.League AND S.Player = E.Player AND S.Date = E.Date
WHERE IFNULL(CTP_1,'') <> '' OR IFNULL(CTP_2,'') <> '' or IFNULL(SkinsNum,'') <> ''
ORDER BY E.League, E.Date
")
        Dim earnings As DataTable = oHelper.sqlitedaFromSql(ctx.Conn, "", sb.ToString)
        oHelper.dt = oHelper.sqliteda(ctx.Conn, "Payments")
        ' Find duplicate rows based on specified columns
        Dim duplicateRows = oHelper.dt.AsEnumerable() _
            .GroupBy(Function(row) New With {
                Key .League = row.Field(Of String)("League"),
                Key .Player = row.Field(Of String)("Player"),
                Key .Date = row.Field(Of String)("Date"),
                Key .Desc = row.Field(Of String)("Desc"),
                Key .Detail = row.Field(Of String)("Detail")}) _
            .Where(Function(group) group.Count() > 1) _
            .SelectMany(Function(group) group) _
            .ToList()
        ' Assuming oHelper.dt is already defined and columns are added
        oHelper.dt.PrimaryKey = New DataColumn() {oHelper.dt.Columns("League"), oHelper.dt.Columns("Player"), oHelper.dt.Columns("Date"), oHelper.dt.Columns("Desc"), oHelper.dt.Columns("Detail")}
        Dim irecs = 0, ifound = 0
        For Each row As DataRow In earnings.Rows
            'Dim sleague = row("League").replace("'", "''")
            'Debug.Print($"{row("League")},{row("Player")},{row("Date")},CTP,#{sPar3s(0)}")

            If row("CTP_1") <> "" Then
                If findpmt(row, sPar3s(0), "CTP_1") = "" Then
                    irecs += 1
                Else
                    ifound += 1
                End If
            End If
            If row("CTP_2") <> "" Then
                If findpmt(row, sPar3s(1), "CTP_2") = "" Then
                    irecs += 1
                Else
                    ifound += 1
                End If
            End If
            If row("SkinsNum") <> "" Then
                If findpmt(row, "", "Skin") = "" Then
                    irecs += 1
                Else
                    ifound += 1
                End If
            End If
        Next
        Debug.Print($"Earnings rows {earnings.Rows.Count}-missing {irecs}-already there {ifound}")

#End Region
skip:
        dtLeaderStats = queryLeaders()
        cbDates.Items.Add("Career")
        oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "", "Select Distinct SUBSTR(Date, 1, 4) from Scores ORDER BY Date DESC")
        For Each row As DataRow In oHelper.dt.Rows
            cbDates.Items.Add(row(0))
        Next

        'For Each sYear As String In Main.cbSeasons.Items
        '    cbDates.Items.Add(sYear)
        'Next
        cbDates.SelectedIndex = 0
        bload = True

        'sStats.AddRange("$Earn,$Skins,$Closest,#Skins, Points, Team_Points".Split(",")) ', "CTP_#4", "CTP_#8", "CTP_#10", "CTP_#12", }

    End Sub
    Function findpmt(row As DataRow, p3 As String, CTPnum As String) As String
        Dim sDesc As String = "CTP"
        If CTPnum = "Skin" Then
            sDesc = "Skin"
        End If
        Dim drfind As DataRow = oHelper.dt.Rows.Find({row("League"), row("Player"), row("Date"), $"{CTPnum}", $"#{p3}"})
        If drfind Is Nothing Then
            ' Perform operations if the row is found
            Dim sb = New Text.StringBuilder
            sb.AppendLine($"INSERT INTO Payments 
(League,Player,Date,[Desc],Detail,Earned) 
VALUES ('{row("League").ToString.Replace("'", "''")}','{row("Player")}','{row("Date")}','{sDesc}','#{p3}',{row(CTPnum)})
")
            oHelper.SQLiteInsertIntoTable(ctx.Conn, oHelper.dt, sb.ToString)
            Return "Not found"
            oHelper.wk = "Some operation"
        Else
            ' Handle case where row is not found
            oHelper.wk = "Row not found"
        End If

    End Function

    Private Sub RefreshDataGridViews()

        ' Remove all existing Labels, GroupBoxes, and DataGridViews
        Dim controlsToRemove = Me.Controls.OfType(Of Label)().Cast(Of Control)().
                         Union(Me.Controls.OfType(Of GroupBox)().Cast(Of Control)()).
                         Union(Me.Controls.OfType(Of DataGridView)().Cast(Of Control)()).ToList()

        If controlsToRemove.Count = 0 Then Exit Sub
        For Each ctrl As Control In controlsToRemove
            Me.Controls.Remove(ctrl)
            ctrl.Dispose() ' Dispose of the control to free resources
        Next
        ' Re-add new DataGridViews dynamically
        AddDynamicDataGridViews()
    End Sub
    Private Sub AddDynamicDataGridViews()
        'buildlist("Rounds", dgRounds)
        Dim wstats = "Rounds,Points,F9Avg,B9Avg,$Earn,CTPS".Split(",")
        Dim mergedArray As String() = wstats.Concat(sStatsDesc).ToArray()

        Dim i = 1
        For Each sStat In mergedArray
            Dim dv As New DataView(dtLeaderStats)
            dv.RowFilter = $"DateYear = '{cbDates.SelectedItem}'"
            Dim sort As String = "Desc"
            If sStat.Contains("Avg") Or sStat = "Others" Or sStat.Contains("Bogeys") Then
                sort = "Asc"
            End If

            Dim slst As List(Of Tuple(Of String, Decimal)) = SortList(dv, sStat, sort)
            Dim dt As New Data.DataTable
            oHelper.CreateColumnsWithFormat(Constants.Player, dt, sColFormat)
            oHelper.CreateColumnsWithFormat("Tot", dt, sColFormat)

            For Each row As DataRowView In dv
                Dim nrow As DataRow = dt.NewRow
                nrow(Constants.Player) = row(Constants.Player)
                nrow("Tot") = row(sStat)
            Next
            Dim iwidth As Integer = 140
            Dim iheight As Integer = 600
            Dim ixlocation As Integer = 14
            Dim iylocation As Integer = 62
            ' Create a new Label
            Dim lbl As New Label()
            lbl.Text = sStat

            lbl.Location = New Drawing.Point(ixlocation + 12 + (i - 1) * iwidth, iylocation) ' Position at the top
            lbl.AutoSize = True ' Automatically adjust the size based on text

            Dim dgv As New DataGridView()
            With dgv
                'now add each field from the lst into the datagridview
                .RowHeadersVisible = False
                .Name = $"dg{sStat}"
                .Size = New Drawing.Size(iwidth, iheight) ' Width, Height
                .Location = New Drawing.Point(ixlocation + (i - 1) * iwidth, 92) ' Positioning
                Debug.Print(dgv.Location.ToString)
                .AllowUserToAddRows = False
                .Visible = True
                .Columns.Clear()
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AutoGenerateColumns = False
                .ReadOnly = True
                .RowTemplate.Height = 20
                .DefaultCellStyle.Font = New System.Drawing.Font("Tahoma", 7.5)
                '.Height = 800
                '.Width = 180

                For Each col As DataColumn In dt.Columns
                    Dim dgc As New DataGridViewTextBoxColumn
                    With dgc
                        .Name = col.ColumnName
                        .ValueType = GetType(System.String)
                        If .Name = "Tot" Then
                            .Name = ""
                            .Width = 25
                        Else
                            .Width = 90
                        End If
                    End With
                    .Columns.Add(dgc)
                Next

                For Each row In slst
                    If row.Item2 > 0 Then
                        .Rows.Add(row.Item1, row.Item2)
                        ColorCells(.Rows(.Rows.Count - 1))
                    End If
                Next
            End With
            ' Add the Label and DataGridView to the form's Controls collection
            Me.Controls.Add(lbl)
            Me.Controls.Add(dgv)
            i += 1
            CalculateColumnWidths(dgv)
        Next

    End Sub
    Private Sub CalculateColumnWidths(dgv As DataGridView)
        Dim g As Graphics = dgv.CreateGraphics()

        ' Iterate through each column
        For Each col As DataGridViewColumn In dgv.Columns
            Dim maxWidth As Integer = col.HeaderCell.Size.Width ' Start with header width

            ' Iterate through each cell in the column
            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Continue For ' Skip the new row

                Dim cellValue As String

                If row.Cells(col.Index).Value IsNot Nothing Then
                    cellValue = row.Cells(col.Index).Value.ToString()
                Else
                    cellValue = String.Empty
                End If

                Dim textSize As SizeF = g.MeasureString(cellValue, dgv.Font)

                ' Update maxWidth if the cell's text is wider
                If CInt(textSize.Width) > maxWidth Then
                    maxWidth = CInt(textSize.Width)
                End If
            Next

            ' Add padding for cell margins
            If col.Name = "Player" Then maxWidth -= 10
            col.Width = maxWidth
        Next
    End Sub
    Function queryLeaders() As DataTable
        queryLeaders = New DataTable("Stats")
        Dim bLeagueTotals = False
        oHelper.dt = oHelper.sqliteda(ctx.Conn, "Courses")
        Dim sb = New Text.StringBuilder

        Try
            sb = sb.AppendLine($"
SELECT 
    s.Player,
    LP.Season AS DateYear,
    COUNT(*) AS Rounds,
    COUNT(CASE WHEN S.FrontBack = 'Front' THEN 1 END) AS F9,
    COUNT(CASE WHEN S.FrontBack = 'Back' THEN 1 END) AS B9,
    COALESCE(ROUND(AVG(CASE WHEN S.FrontBack = 'Front' THEN CAST(S.Gross AS REAL) END), 1), 0) AS F9Avg,
    COALESCE(ROUND(AVG(CASE WHEN S.FrontBack = 'Back' THEN CAST(S.Gross AS REAL) END), 1), 0) AS B9Avg,
    SUM(CAST(E.SkinsNum AS REAL)) AS NumSkins,
    COALESCE(SUM(CAST(E.SkinEarned AS REAL)), 0) AS [$Skins],
    SUM(CASE WHEN CAST(E.CTP_2 as REAL) > 0 THEN 1 ELSE 0 END + CASE WHEN CAST(E.CTP_1 as REAL) > 0 THEN 1 ELSE 0 END) AS CTPs,
    SUM(CASE WHEN CAST(E.CTP_1 as REAL) > 0 THEN 1 ELSE 0 END) AS CTP1,
    SUM(CASE WHEN CAST(E.CTP_2 as REAL) > 0 THEN 1 ELSE 0 END) AS CTP2,
    COALESCE(SUM(CAST(E.ClosestEarned AS REAL)), 0) AS [$Closest],
    COALESCE(SUM(CAST(E.Earned AS REAL)), 0) AS [$Earn],
")
            sb.AppendLine(buildstats())
            sb.AppendLine($"
    ROUND(SUM(Points), 1) AS Points
FROM Scores S 
LEFT JOIN LeagueParms LP ON LP.Name = S.League AND substring(S.Date, 1, 4) = LP.Season
LEFT JOIN Earnings E ON E.League = LP.Name AND E.Date = S.Date AND E.Player = S.Player AND S.Date >= '20171231'
LEFT JOIN Matches Ma ON Ma.League = LP.Name AND Ma.Date = S.Date AND S.Player = Ma.Player AND S.Date >= '20171231'
LEFT JOIN Courses CF ON CF.Name = 'Front'
LEFT JOIN Courses CB ON CB.Name = 'Back'
WHERE S.Date >= '20171231' 
AND S.Date NOT IN ({ctx.sExcludeDates}) 
GROUP BY S.Player, LP.Season
UNION ALL
SELECT 
    s.Player,
    'Career' AS DateYear,
    COUNT(*) AS Rounds,
    COUNT(CASE WHEN S.FrontBack = 'Front' THEN 1 END) AS F9,
    COUNT(CASE WHEN S.FrontBack = 'Back' THEN 1 END) AS B9,
    COALESCE(ROUND(AVG(CASE WHEN S.FrontBack = 'Front' THEN CAST(S.Gross AS REAL) END), 1), 0) AS F9Avg,
    COALESCE(ROUND(AVG(CASE WHEN S.FrontBack = 'Back' THEN CAST(S.Gross AS REAL) END), 1), 0) AS B9Avg,
    SUM(CAST(E.SkinsNum AS REAL)) AS NumSkins,
    COALESCE(SUM(CAST(E.SkinEarned AS REAL)), 0) AS [$Skins],
    SUM(CASE WHEN CAST(E.CTP_2 as REAL) > 0 THEN 1 ELSE 0 END + CASE WHEN CAST(E.CTP_1 as REAL) > 0 THEN 1 ELSE 0 END) AS CTPs,
    SUM(CASE WHEN CAST(E.CTP_1 as REAL) > 0 THEN 1 ELSE 0 END) AS CTP1,
    SUM(CASE WHEN CAST(E.CTP_2 as REAL) > 0 THEN 1 ELSE 0 END) AS CTP2,
    COALESCE(SUM(CAST(E.ClosestEarned AS REAL)), 0) AS [$Closest],
    COALESCE(SUM(CAST(E.Earned AS REAL)), 0) AS [$Earn],
")
            sb.AppendLine(buildstats())
            sb.AppendLine($"
    ROUND(SUM(Points), 1) AS Points
FROM Scores S 
LEFT JOIN LeagueParms LP ON LP.Name = S.League AND substring(S.Date, 1, 4) = LP.Season
LEFT JOIN Earnings E ON E.League = LP.Name AND E.Date = S.Date AND E.Player = S.Player AND S.Date >= '20171231'
LEFT JOIN Matches Ma ON Ma.League = LP.Name AND Ma.Date = S.Date AND S.Player = Ma.Player AND S.Date >= '20171231'
LEFT JOIN Courses CF ON CF.Name = 'Front'
LEFT JOIN Courses CB ON CB.Name = 'Back'
WHERE S.Date >= '20171231' 
AND S.Date NOT IN ({ctx.sExcludeDates}) 
GROUP BY S.Player
ORDER BY DateYear DESC;
")
            oHelper.wk = sb.ToString
            queryLeaders = oHelper.sqlitedaFromSql(ctx.Conn, "", sb.ToString)
            oHelper.wk = ""

        Catch ex As Exception

        End Try

    End Function
    Function buildstats() As String
        Dim soffset As Integer = -2

        For Each stat As String In sStatsDesc
            For i = 1 To 9
                If stat = "Others" Then
                    buildstats &= $"SUM(Case When S.FrontBack = 'Front' THEN (CASE WHEN S.[{i}] > CF.Hole{i} + 2 THEN 1 ELSE 0 END) ELSE (CASE WHEN S.[{i}] > CB.Hole{i} + 2 THEN 1 ELSE 0 END) END) "
                    'scorecard prevents this
                    'ElseIf sStat = "Maxs" Then
                    '    'https://stackoverflow.com/questions/15774078/what-is-the-equivalent-of-select-case-in-access-sql
                    '    buildstats &= $"SUM(IIF([S.Hole{i}] >= Switch (C.Hole{i} = 3,LP.Par3Max,C.Hole{i} = 4,LP.Par4Max,C.Hole{i} = 5,LP.Par5Max),1,0)) {vbCrLf} "
                Else
                    buildstats &= $"SUM(CASE WHEN S.FrontBack = 'Front' THEN (CASE WHEN S.[{i}] = CF.Hole{i} + {soffset} THEN 1 ELSE 0 END) ELSE (CASE WHEN S.[{i}] = CB.Hole{i} + {soffset} THEN 1 ELSE 0 END) END)"
                End If
                If i < 9 Then buildstats &= $" + {vbCrLf}"
            Next
            buildstats &= $" as {stat}, {vbCrLf} "
            soffset += 1
        Next
    End Function
    Sub buildlist(fld As String, dg As DataGridView)

        Try
            'build a sortable list because dataview doesnt sort numeric fields properly
            Dim dv As New DataView(dtLeaderStats)
            dv.RowFilter = $"DateYear = '{cbDates.SelectedItem}'"
            Dim sort As String = ""
            If dg.Name.Contains("Scoring") Then
                sort = "Asc"
            End If

            Dim slst As List(Of Tuple(Of String, Decimal)) = SortList(dv, fld, sort)
            Dim dt As New Data.DataTable
            oHelper.CreateColumnsWithFormat(Constants.Player, dt, sColFormat)
            oHelper.CreateColumnsWithFormat("Total", dt, sColFormat)

            For Each row As DataRowView In dv
                Dim nrow As DataRow = dt.NewRow
                nrow(Constants.Player) = row(Constants.Player)
                nrow("Total") = row(fld)
            Next

            With dg
                .RowHeadersVisible = False
                .Visible = True
                .Columns.Clear()
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AutoGenerateColumns = False
                .ReadOnly = True
                .RowTemplate.Height = 20
                .DefaultCellStyle.Font = New System.Drawing.Font("Tahoma", 9)
                .Height = 800
                .Width = 180
                For Each col As DataColumn In dt.Columns
                    Dim dgc As New DataGridViewTextBoxColumn
                    With dgc
                        .Name = col.ColumnName
                        .ValueType = GetType(System.String)
                        If .Name = "Total" Then
                            .Width = 40
                        End If
                    End With
                    .Columns.Add(dgc)
                Next
                'now add each field from the lst into the datagridview
                For Each row In slst
                    If row.Item2 > 0 Then
                        .Rows.Add(row.Item1, row.Item2)
                        ColorCells(.Rows(.Rows.Count - 1))
                    End If
                Next
            End With

            Me.Show()
            Application.DoEvents()

            'Dim sfn As String = oHelper.sReportPath & String.Format("\{0}_", cbDates.SelectedItem & "_" & fld) & DateTime.Now.ToString("yyyyMMdd") & ".csv"
            'oHelper.dgv2csv(dg, sfn)

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    'https://stackoverflow.com/questions/15697282/application-not-quitting-after-calling-quit
    Sub AccumScores(score As DataRow, sselect As String)
        Dim sMethod = oHelper.convDBNulltoSpaces(score("Method")).Trim
        Dim foundRows() As DataRow = dtScoreCard.Select(sselect)

        Try
            If foundRows.Count > 1 Then
                MsgBox(String.Format("Contact Developer, error in {0}{1}", vbCrLf, sselect))
                Exit Sub
            End If

            For Each row In foundRows
                Dim scolprefix As String = ""
                If sMethod = "Score" Then scolprefix = "SO_"

                'row("Rnds") += 1
                row(scolprefix & "Rnds") += 1
                If IsNumeric(score("Out_Gross")) Then
                    'make ctp1 on front nine score #4, ctp2 #8
                    If score(Constants.CTP1) IsNot DBNull.Value Then
                        If score(Constants.CTP1) > 0 Then row(slstFCTPS(0)) += 1
                    End If
                    If score(Constants.CTP2) IsNot DBNull.Value Then
                        If score(Constants.CTP2) > 0 Then row(slstFCTPS(1)) += 1
                    End If
                    row(scolprefix & "F9") += 1
                    row("Out_Gross") = CInt(row("Out_Gross")) + score("Out_Gross")
                End If
                If IsNumeric(score("Out_Net")) Then
                    row(scolprefix & "Out_Net") = CInt(row(scolprefix & "Out_Net")) + score("Out_Net")
                ElseIf IsNumeric(score("Out_Gross")) Then
                    row("Out_Net") = row("Out_Net") + CInt(score("Out_Gross")) - score("Phdcp")
                End If

                If IsNumeric(score("In_Gross")) Then
                    If score(Constants.CTP1) IsNot DBNull.Value Then
                        If score(Constants.CTP1) > 0 Then row(slstBCTPS(0)) += 1
                    End If
                    If score(Constants.CTP2) IsNot DBNull.Value Then
                        If score(Constants.CTP2) > 0 Then row(slstBCTPS(1)) += 1
                    End If
                    row(scolprefix & "B9") += 1
                    row("In_Gross") = CInt(row("In_Gross")) + score("In_Gross")
                End If
                If IsNumeric(score("In_Net")) Then
                    row("In_Net") = CInt(row("In_Net")) + score("In_Net")
                ElseIf IsNumeric(score("In_Gross")) Then
                    row("In_Net") = CInt(row("In_Net")) + CInt(score("In_Gross")) - score("Phdcp")
                End If
                ' calculate skins ctp, etc
                Try
                    For Each stat In sStats
                        Try
                            If score(stat) Is DBNull.Value Then
                                score(stat) = 0
                            End If
                            If IsNumeric(score(stat)) Then
                                row(stat) = CDec(row(stat)) + score(stat)
                            End If
                            If stat = "Points" Then
                                LOGIT($"{score("Player")} {stat} - Total Points {row(stat)} - {CInt(score(stat))}")
                            End If

                        Catch ex As Exception

                        End Try
                    Next
                    'row("CTPS") = CInt(row("CTP_#4")) + CInt(row("CTP_#8")) + CInt(row("CTP_#10")) + CInt(row("CTP_#12"))
                Catch ex As Exception
                    Dim x = ""
                End Try

                For i = 1 To 18
                    Dim sHole = $"Hole{i}"
                    Dim iscore = score(sHole)
                    If iscore Is DBNull.Value Then Continue For
                    'if the handicap > stroke index adjust net score to gross
                    If score("Method") = "Net" Then
                        Dim isi = oHelper.CalcStrokeIndex(sHole)
                        If score("pHdcp") >= isi Then
                            'check stroke index
                            iscore += 1
                            If score("pHdcp") - oHelper.iHoles >= isi Then iscore += 1
                        End If
                    End If
                    row(sHole) = CInt(row(sHole)) + iscore

                    'add to stats rows
                    Dim istart = oHelper.thisCourse(sHole).ToString - 2 'start with `
                    Try
                        For x = 0 To sStatsDesc.Length - 1
                            If iscore = istart Then
                                row(sStatsDesc(x)) += 1
                                Exit For
                            End If
                            istart += 1
                        Next
                    Catch ex As Exception
                        Dim x = ""
                    End Try

                Next
            Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Private Sub ReleaseObject(ByVal obj As Object)
        Try
            Dim intRel As Integer = 0
            Do
                intRel = System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            Loop While intRel > 0
            MsgBox("Final Released obj # " & intRel)
        Catch ex As Exception
            MsgBox("Error releasing object" & ex.ToString)
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub
    Function SortList(dv As DataView, fld As String, sort As String) As List(Of Tuple(Of String, Decimal))
        Try

            'Declare the List Of Tuple with a Tuple of Char, Integer, Integer
            Dim lstToSort As New List(Of Tuple(Of String, Decimal))
            'Example to Add items
            For Each row As DataRowView In dv

                If IsNumeric(row(fld)) Then
                    If cbRegulars.Checked Then
                        Dim sKey() As Object = {row("Player"), row("DateYear")}
                        Dim xplayer = dtRegulars.Rows.Find(sKey)
                        If xplayer Is Nothing Then
                            LOGIT($"{row("Player")} skipped")
                            Continue For
                        End If
                    End If
                    lstToSort.Add(Tuple.Create(row(Constants.Player).ToString, CDec(row(fld))))
                    End If
            Next
            'Sort is just 1 line
            If sort = "Asc" Then
                lstToSort = lstToSort.OrderBy(Function(i) i.Item2).ToList
            Else
                lstToSort = lstToSort.OrderByDescending(Function(i) i.Item2).ToList
            End If
            'Loop through the elements to print them
            'For Each tpl As Tuple(Of String, Integer) In lstToSort
            '    Console.WriteLine(tpl.Item1 & "-" & tpl.Item2)
            'Next
            Return lstToSort
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Function
    Sub ChangeFont(row As DataGridViewRow)
        Dim cell As DataGridViewCell = Nothing
        Dim sFont = "Tahoma"
        Dim iFontSize = 8

        For Each dgc As DataGridViewCell In row.Cells
            If dgc.OwningColumn.Name.Contains("Method") Or dgc.OwningColumn.Name.Contains("Date") Then Continue For
            With dgc
                .Style.Font = New System.Drawing.Font(sFont, iFontSize, FontStyle.Regular)
                .Style.ForeColor = Color.Black
                .Style.BackColor = Color.White
            End With
        Next

    End Sub
    Sub ColorCells(row As DataGridViewRow)
        Try

            If cbDates.SelectedItem = "Career" Then Exit Sub
            If Not cbRegulars.Checked Then
                Dim sKey() As Object = {row.Cells("Player").Value, cbDates.SelectedItem}
                Dim sthisplayer = dtRegulars.Rows.Find(sKey)
                If sthisplayer Is Nothing Then
                    Dim dgc As DataGridViewCell
                    dgc = row.Cells("Player")
                    dgc.Style.BackColor = Color.Aqua
                End If
            End If

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub calcRank(toPar As DataRow, drRank As DataRow)

        Dim sRank As New List(Of rank)
        Dim ii = 0
        Try
            'For Each cell In row.Cells
            For i = 1 To 18
                Dim sscore = toPar("Hole" & i)
                If sscore IsNot DBNull.Value Then
                    sRank.Add(New rank(ii, sscore))
                    ii += 1
                End If
            Next

            '         y, then x is descending, x then y is ascending
            sRank.Sort(Function(x, y) y.toPar.CompareTo(x.toPar))

            For i = 0 To 17 Step 1
                If i > sRank.Count - 1 Then Exit For
                drRank("Hole" & sRank(i).Hole + 1) = i + 1
            Next

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
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
    Sub calcToPar(row As DataRow, toPar As DataRow)
        Dim d2dec As Decimal = 0.0

        Try
            If IsNumeric(row("Out_Gross")) Then
                If row("Out_Gross") > 0 Then
                    d2dec = CInt(row("Out_Gross")) / CInt(row("F9") + CInt(row("SO_F9")))
                    row("Out_Gross") = d2dec.ToString("##.0")
                End If
            End If
            If IsNumeric(row("Out_Net")) Then
                If row("Out_Net") > 0 Then
                    d2dec = CInt(row("Out_Net")) / CInt(row("F9") + CInt(row("SO_F9")))
                    row("Out_Net") = d2dec.ToString("##.0")
                End If
            End If
            If IsNumeric(row("In_Gross")) Then
                If row("In_Gross") > 0 Then
                    d2dec = CInt(row("In_Gross")) / CInt(row("B9") + CInt(row("SO_B9")))
                    row("In_Gross") = d2dec.ToString("##.0")
                End If
            End If
            If IsNumeric(row("In_Net")) Then
                If row("In_Net") > 0 Then
                    d2dec = CInt(row("In_Net")) / CInt(row("B9") + CInt(row("SO_B9")))
                    row("In_Net") = d2dec.ToString("##.0")
                End If
            End If

            For i = 1 To 18
                calcHolebyHole(row, i, toPar)
            Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub calcHolebyHole(row As DataRow, i As Integer, toPar As DataRow)
        Dim d2dec As Decimal = 0.0
        Try
            If Not row("Hole" & i) > 0 Then Exit Sub
            d2dec = CInt(row("Hole" & i)) / CInt(row(IIf(i < 10, "F9", "B9")))
            row("Hole" & i) = d2dec.ToString("#.0")
            d2dec -= oHelper.thisCourse("Hole" & i).ToString
            'this updates the ytd topar
            toPar("Hole" & i) = d2dec.ToString("#.0;-#.0;#.0")
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub createTotalRow(dtscorecard As DataTable, newrow As DataRow, fld As String, splayer As String)
        Try
            LOGIT(String.Format("creating total row for {0}-{1}", splayer, fld))
            newrow = dtscorecard.NewRow
            newrow(0) = splayer
            If UBound(fld.Split(" ")) > 0 Then
                If IsNumeric(fld.Substring(0, 4)) Then
                    newrow("Date") = fld.Substring(0, 4)
                    newrow("Method") = String.Format("{0}", fld.Substring(5))
                End If
            Else
                newrow("Date") = "Career"
                newrow("Method") = fld
            End If

            If fld.Contains("Avg") Then
                newrow("In_Gross") = 0
                newrow("In_Gross") = 0
                newrow("In_Net") = 0
                newrow("Out_Gross") = 0
                newrow("Out_Net") = 0
                newrow("$Earn") = 0
                newrow("$Skins") = 0
                newrow("$Closest") = 0
                newrow("#Skins") = 0
                newrow("Points") = 0
                newrow("Team_Points") = 0
                newrow("CTPS") = 0

                For Each cp In slstFCTPS
                    newrow(cp) = 0
                Next
                For Each cp In slstBCTPS
                    newrow(cp) = 0
                Next
                newrow("Rnds") = 0
                newrow("F9") = 0
                newrow("B9") = 0
                For i = 1 To 18
                    newrow("Hole" & i) = 0
                Next
                For Each col As DataColumn In dtscorecard.Columns
                    If sStatsDesc.Contains(col.ColumnName) Then newrow(col.ColumnName) = 0
                    If col.ColumnName.Contains("SO_") Then
                        newrow(col.ColumnName) = 0
                    End If
                Next
            End If
            dtscorecard.Rows.Add(newrow)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs)
        Dim x = sender
        Dim xx = ""
    End Sub
    Private Sub dgScores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs)

        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
            oHelper.bScoresbyPlayer = True
            Dim dgv = sender
            For Each row As DataGridViewRow In dgv.rows
                'row.Height = 30
                oHelper.ChangeColorsForStrokes(row)
            Next
            oHelper.bScoresbyPlayer = False
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs)
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        '20180725 - dont allow sort for now
        Exit Sub
        Try
            'e.SortResult = If(CInt(e.CellValue1) < CInt(e.CellValue2), -1, 1)
            'e.Handled = True
            Main.oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub Scores_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '    rs.ResizeAllControls(Me)
            LOGIT($"Form Height {Me.Height} Width {Me.Width}")
            Dim controlsToRemove = Me.Controls.OfType(Of DataGridView)().Cast(Of Control)().ToList()
            Dim controlToRemove As Control = controlsToRemove(1)

            Me.Text = String.Format("Leaders {8} - Form {7}-{0}, Resolution {1} x {2}, Menu {3} x {4}, Grid {5} x {6}", My.Computer.Name, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Width, Me.Width, Me.Height, controlToRemove.Width, controlToRemove.Height, Me.Name, oHelper.sPlayer)

        Catch ex As Exception

        End Try
    End Sub
    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        If cbDates.SelectedItem = "Career" Then
            cbRegulars.Checked = False
            cbRegulars.Visible = False
        Else
            cbRegulars.Visible = True
        End If
        If Me.Controls.OfType(Of DataGridView).ToList.Count = 0 Then Exit Sub
        ' Re-add new DataGridViews dynamically
        RefreshDataGridViews()
    End Sub
    Private Sub Leaders_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        'ReleaseObject(oRG)
        'ReleaseObject(oSheet)
        'ReleaseObject(oSheets)
        'ReleaseObject(oYB)
        'ReleaseObject(oWB)
        'ReleaseObject(oXL)
        'Me.Close()

    End Sub
    Private Sub btnDisplay_Click(sender As Object, e As EventArgs) 'Handles btnDisplay.Click
        cbDates_SelectedIndexChanged(sender, e)
    End Sub
    Private Sub cbRegulars_CheckedChanged(sender As Object, e As EventArgs) Handles cbRegulars.CheckedChanged
        cbDates_SelectedIndexChanged(sender, e)
    End Sub
End Class