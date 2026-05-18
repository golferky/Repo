Imports System.Data.SQLite
Imports ClosedXML.Excel
Imports System.Diagnostics
Imports HughsGolf.Main

Public Class LeagueDatabaseLoader
    Private ReadOnly _basePath As String = "C:\HughsGolf\Files"
    Private ReadOnly _dbHashPath As String
    Private ReadOnly _dbPath As String
    Private ReadOnly _xlsxPath As String
    Private ReadOnly _warnings As New List(Of String)
    Private ReadOnly _dashboard As New DataSet("LeagueDashboard")
    Private oHelper As New Helper()
    Public Sub New()
        _dbPath = IO.Path.Combine(_basePath, "HughsGolf.db")
        _dbHashPath = IO.Path.Combine(_basePath, "HughsGolf_Hashes.db")
        _xlsxPath = IO.Path.Combine(_basePath, "Hugh'sLeague.xlsx")
    End Sub
    Private Sub EnsureMetadataTable()
        Using conn As New SQLiteConnection($"Data Source={_dbPath};Version=3;")
            conn.Open()

            Dim sql As String =
            "CREATE TABLE IF NOT EXISTS SheetMetadata (" &
            "SheetName TEXT PRIMARY KEY, " &
            "Hash TEXT" &
            ");"

            Using cmd As New SQLiteCommand(sql, conn)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Public Sub Initialize()
        LOGIT("Initializing LeagueDatabaseLoader...")
        EnsureMetadataTable()
        oHelper.EnsureHashDbExists(_dbHashPath)
        ' ---------------------------------------------------------
        ' 1. Check if Excel file is locked
        ' ---------------------------------------------------------
        If oHelper.IsFileLocked(_xlsxPath) Then
            MessageBox.Show("Please close the Excel file before running the app.",
                        "File In Use",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            LOGIT("Initialization aborted: Excel file is locked.")
            Return
        End If
        GoTo skipwb
        ' ---------------------------------------------------------
        ' 2. Load workbook
        ' ---------------------------------------------------------
        Dim wb As New XLWorkbook(_xlsxPath)
        Dim anySheetChanged As Boolean = False

        ' ---------------------------------------------------------
        ' 3. Loop through each sheet and hash it
        ' ---------------------------------------------------------
        For Each ws In wb.Worksheets
            Dim sheetName As String = ws.Name.Trim()
            Dim currentHash As String = oHelper.GetSheetHash(ws)
            Dim settingName As String = "Hash_" & sheetName
            Dim lastHash As String = oHelper.GetStoredHash(sheetName, _dbHashPath)

            If currentHash <> lastHash Then
                BuildTableFromSheet(ws)
                oHelper.SetStoredHash(sheetName, currentHash, _dbHashPath)
                anySheetChanged = True
            Else
                LOGIT($"{sheetName} unchanged — skipping rebuild.")
            End If
        Next
skipwb:
        CreateViews(New SQLiteConnection($"Data Source={_dbPath};Version=3;"))
        '' ---------------------------------------------------------
        '' 5. Load league data into memory/UI
        '' ---------------------------------------------------------
        'LoadLeagueDataFromExcel()

        LOGIT("LeagueDatabaseLoader initialization complete.")
    End Sub

    Private Sub LoadLeagueDataFromExcel()
        Try
            Using workbook As New XLWorkbook(_xlsxPath)
                For Each ws In workbook.Worksheets
                    Dim tableName As String = ws.Name.Trim()
                    Dim headers = ws.Row(1).Cells().Select(Function(c) c.Value.ToString().Trim()).Where(Function(h) h <> "").ToList()
                    If headers.Count = 0 Then Continue For

                    Dim dt As New DataTable(tableName)
                    For Each h In headers
                        dt.Columns.Add(h)
                    Next

                    For Each row In ws.RowsUsed().Skip(1)
                        Dim dr = dt.NewRow()
                        For i = 0 To headers.Count - 1
                            dr(i) = row.Cell(i + 1).Value.ToString()
                        Next
                        dt.Rows.Add(dr)
                    Next

                    InsertDataTableWithDiagnostics(dt, tableName)
                    _dashboard.Tables.Add(dt)
                    LOGIT($"Loaded {dt.Rows.Count} rows into [{tableName}].")
                Next
            End Using
        Catch ex As Exception
            LOGIT($"Error loading Excel data: {ex.Message}")
            _warnings.Add($"Excel load error: {ex.Message}")
        End Try
    End Sub

    Private Sub InsertDataTableWithDiagnostics(dt As DataTable, tableName As String)
        Try
            Using conn As New SQLiteConnection($"Data Source={_dbPath};Version=3;")
                conn.Open()

                Dim schemaCmd As New SQLiteCommand($"PRAGMA table_info([{tableName}]);", conn)
                Dim reader = schemaCmd.ExecuteReader()
                Dim dbCols As New List(Of String)
                While reader.Read()
                    dbCols.Add(reader("name").ToString())
                End While
                reader.Close()

                For Each row As DataRow In dt.Rows
                    Dim validCols = dt.Columns.Cast(Of DataColumn).Where(Function(c) dbCols.Contains(c.ColumnName)).ToList()
                    If validCols.Count = 0 Then
                        Dim msg = $"Table {tableName}: no matching columns found, row skipped."
                        _warnings.Add(msg)
                        LOGIT(msg)
                        Continue For
                    End If

                    Dim colNames = String.Join(",", validCols.Select(Function(c) $"[{c.ColumnName}]"))
                    Dim paramNames = String.Join(",", validCols.Select(Function(c) "@" & c.ColumnName.Replace(" ", "_")))
                    Dim sql = $"INSERT INTO [{tableName}] ({colNames}) VALUES ({paramNames})"

                    Using cmd As New SQLiteCommand(sql, conn)
                        For Each col In validCols
                            Dim paramName = "@" & col.ColumnName.Replace(" ", "_")
                            cmd.Parameters.AddWithValue(paramName, row(col))
                        Next
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
        Catch ex As Exception
            LOGIT($"Error inserting into {tableName}: {ex.Message}")
            _warnings.Add($"Insert error in {tableName}: {ex.Message}")
        End Try
    End Sub
    Private Sub CreateViews(conn As SQLiteConnection)
        Try
            conn.Open()

            Dim sqlDrops As String =
"
DROP VIEW IF EXISTS vwCourseHoles;
DROP VIEW IF EXISTS vwHoleScores;
DROP VIEW IF EXISTS vwMatchesScores;"

            Using cmd As New SQLiteCommand(sqlDrops, conn)
                cmd.ExecuteNonQuery()
            End Using
            conn.Close()

            conn.Open()
            ' ---------------------------------------------------------
            ' 1. Create vwCourseHoles (foundation)
            ' ---------------------------------------------------------
            Dim sqlCourseHoles As String =
"
CREATE VIEW vwCourseHoles AS
SELECT
    C.Name AS Course,
    H.HoleNum,
    H.Par,
    H.SI,
    CASE 
        WHEN H.SI % 2 = 0 THEN H.SI / 2
        ELSE (H.SI + 1) / 2
    END AS NineHoleSI,
    CASE 
        WHEN H.HoleNum BETWEEN 1 AND 9 THEN 'Front'
        ELSE 'Back'
    END AS FrontBack
FROM (
    SELECT Name, 1 AS HoleNum, Hole1 AS Par, H1 AS SI FROM Courses
    UNION ALL SELECT Name, 2, Hole2, H2 FROM Courses
    UNION ALL SELECT Name, 3, Hole3, H3 FROM Courses
    UNION ALL SELECT Name, 4, Hole4, H4 FROM Courses
    UNION ALL SELECT Name, 5, Hole5, H5 FROM Courses
    UNION ALL SELECT Name, 6, Hole6, H6 FROM Courses
    UNION ALL SELECT Name, 7, Hole7, H7 FROM Courses
    UNION ALL SELECT Name, 8, Hole8, H8 FROM Courses
    UNION ALL SELECT Name, 9, Hole9, H9 FROM Courses
    UNION ALL SELECT Name, 10, Hole10, H10 FROM Courses
    UNION ALL SELECT Name, 11, Hole11, H11 FROM Courses
    UNION ALL SELECT Name, 12, Hole12, H12 FROM Courses
    UNION ALL SELECT Name, 13, Hole13, H13 FROM Courses
    UNION ALL SELECT Name, 14, Hole14, H14 FROM Courses
    UNION ALL SELECT Name, 15, Hole15, H15 FROM Courses
    UNION ALL SELECT Name, 16, Hole16, H16 FROM Courses
    UNION ALL SELECT Name, 17, Hole17, H17 FROM Courses
    UNION ALL SELECT Name, 18, Hole18, H18 FROM Courses
) H
JOIN Courses C
    ON C.Name = H.Name;
"
            Using cmd As New SQLiteCommand(sqlCourseHoles, conn)
                cmd.ExecuteNonQuery()
            End Using

            ' ---------------------------------------------------------
            ' 2. Create vwHoleScores (per-hole scoring engine)
            ' ---------------------------------------------------------
            Dim sqlHoleScores As String =
"
CREATE VIEW vwHoleScores AS
WITH HoleData AS (
    SELECT 
        S.Player,
        S.Date,
        S.League,
        S.FrontBack,
        LP.Course,
        1 AS HoleNum, S.[1] AS GrossHole
    FROM Scores S
    JOIN LeagueParms LP
      ON LP.Name   = S.League
     AND LP.Season = SUBSTR(S.Date, 1, 4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 2, S.[2]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 3, S.[3]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 4, S.[4]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 5, S.[5]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 6, S.[6]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 7, S.[7]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 8, S.[8]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)

    UNION ALL SELECT S.Player, S.Date, S.League, S.FrontBack, LP.Course, 9, S.[9]
    FROM Scores S JOIN LeagueParms LP ON LP.Name = S.League AND LP.Season = SUBSTR(S.Date,1,4)
)
SELECT
    H.Player,
    H.Date,
    H.League,
    H.HoleNum,
    CH.Par,
    CH.SI,
    CH.NineHoleSI,
    H.GrossHole,
    HC.PHdcp,
    HC.Hdcp,
    CASE 
        WHEN CH.NineHoleSI <= HC.PHdcp THEN H.GrossHole - 1
        ELSE H.GrossHole
    END AS NetHole
FROM HoleData H
JOIN vwCourseHoles CH
    ON CH.Course    = H.Course
   AND CH.HoleNum   = H.HoleNum
   AND CH.FrontBack = H.FrontBack
LEFT JOIN Handicaps HC
    ON HC.League = H.League
   AND HC.Player = H.Player
   AND HC.Date   = H.Date;
"
            Using cmd As New SQLiteCommand(sqlHoleScores, conn)
                cmd.ExecuteNonQuery()
            End Using

            ' ---------------------------------------------------------
            ' 3. Create vwMatchesScores (summary view)
            ' ---------------------------------------------------------
            Dim sqlMatchesScores As String
            sqlMatchesScores = "
DROP VIEW IF EXISTS vwMatchesScores;

CREATE VIEW vwMatchesScores AS

SELECT 
    Ma.Date, Ma.League, Ma.Partner,

    CASE WHEN SkP.Earned IS NULL THEN 'N' ELSE 'Y' END AS InSkins,
    CASE WHEN CPPlyrs.Earned IS NULL THEN 'N' ELSE 'Y' END AS InCTPs,
    CASE WHEN Su.Player IS NOT NULL THEN 'Y' ELSE 'N' END AS IsSub,

    Ma.Player,

    COALESCE(T.Team, Su.Team, 0) AS Team,
    COALESCE(T.Grade, Su.Grade, 'N/A') AS Grade,
    COALESCE(M.Method, 'Gross') AS Method,

    S.Round, S.[Group],
    S.[1], S.[2], S.[3], S.[4], S.[5], S.[6], S.[7], S.[8], S.[9],
    S.FrontBack, S.Gross, S.Net, 

    CASE 
        WHEN TRIM(UPPER(COALESCE(T.Grade, Su.Grade))) = 'A' THEN
            (
                SELECT SUM(COALESCE(S2.Net, 0))
                FROM Matches M2
                LEFT JOIN Teams T2
                    ON T2.League = M2.League
                   AND T2.Player = M2.Player
                   AND T2.Year = CAST(SUBSTR(M2.Date,1,4) AS INTEGER)
                LEFT JOIN Subs Su2
                    ON Su2.League = M2.League
                   AND Su2.Date = M2.Date
                   AND Su2.Player = M2.Player
                LEFT JOIN Scores S2
                    ON S2.League = M2.League
                   AND S2.Date = M2.Date
                   AND TRIM(UPPER(S2.Player)) = TRIM(UPPER(M2.Player))
                WHERE M2.League = Ma.League
                  AND M2.Date = Ma.Date
                  AND COALESCE(T2.Team, Su2.Team) = COALESCE(T.Team, Su.Team)
            )
        ELSE NULL
    END AS Team_Net,

    Ma.Team_Points,
    Ma.Points,
    Ma.Opponent,

    -- Handicap
    COALESCE(
        (SELECT H2.Hdcp 
         FROM Handicaps H2
         WHERE H2.Player = Ma.Player 
           AND H2.League = Ma.League 
           AND H2.Date < Ma.Date 
           AND H2.Hdcp <> '' 
         ORDER BY H2.Date DESC LIMIT 1),
        H.Hdcp
    ) AS PHdcp,

    H.Hdcp,

    -- Earnings
    (SUM(CAST(COALESCE(SK.Earned, 0) AS REAL)) + 
     COALESCE((SELECT SUM(CAST(Earned AS REAL)) 
               FROM Payments P
               WHERE P.League = Ma.League 
                 AND P.Date = Ma.Date 
                 AND P.Player = Ma.Player 
                 AND P.[Desc] = 'CTP' 
                 AND P.Detail LIKE '#%'), 0)
    ) AS [$Earn],

    SUM(CAST(COALESCE(SK.Earned, 0) AS REAL)) AS [$Skins],

    COALESCE((SELECT SUM(CAST(Earned AS REAL)) 
              FROM Payments P
              WHERE P.League = Ma.League 
                AND P.Date = Ma.Date 
                AND P.Player = Ma.Player 
                AND P.[Desc] = 'CTP' 
                AND P.Detail LIKE '#%'), 0) AS [$Closest],

    MAX(CPDP.DatePaid) AS CPDP,

    -- League Dues
    COALESCE((
        SELECT SUM(CAST(Earned AS REAL))
        FROM Payments P
        WHERE P.League = Ma.League
          AND P.Player = Ma.Player
          AND P.[Desc] = 'League Dues'
          AND P.Detail = 'Payment'
          AND CAST(LP.Season AS TEXT) = SUBSTR(P.Date, 1, 4)
    ), 0) AS [LD Paid],

    (MAX(LP.Cost) -
     COALESCE((
        SELECT SUM(CAST(Earned AS REAL))
        FROM Payments P
        WHERE P.League = Ma.League
          AND P.Player = Ma.Player
          AND P.[Desc] = 'League Dues'
          AND P.Detail = 'Payment'
          AND CAST(LP.Season AS TEXT) = SUBSTR(P.Date, 1, 4)
    ), 0)
    ) AS [LD Balance],

    -- EOY
    COALESCE((
        SELECT SUM(CAST(Earned AS REAL))
        FROM Payments P
        WHERE P.League = Ma.League
          AND P.Player = Ma.Player
          AND P.[Desc] = 'EOY Skins'
          AND P.Detail = 'Payment'
          AND CAST(LP.Season AS TEXT) = SUBSTR(P.Date, 1, 4)
    ), 0) AS [EOY Paid],

    (MAX(LP.[EOY Skins]) -
     COALESCE((
        SELECT SUM(CAST(Earned AS REAL))
        FROM Payments P
        WHERE P.League = Ma.League
          AND P.Player = Ma.Player
          AND P.[Desc] = 'EOY Skins'
          AND P.Detail = 'Payment'
          AND CAST(LP.Season AS TEXT) = SUBSTR(P.Date, 1, 4)
    ), 0)
    ) AS [EOY Balance],

    (MAX(LP.[EOY Skins]) / 2.0) AS [EOY Week Amt],
    
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Payments P
            WHERE P.League = Ma.League
              AND P.Date = Ma.Date
              AND P.Player = Ma.Player
              AND P.[Desc] = 'EOY Skins'
              AND (
                    P.Comment IS NULL 
                    OR TRIM(P.Comment) = ''
                    OR UPPER(P.Comment) LIKE '%1%'
                  )
        ) THEN 'Y' ELSE 'N'
    END AS InEOY1,

    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Payments P
            WHERE P.League = Ma.League
              AND P.Date = Ma.Date
              AND P.Player = Ma.Player
              AND P.[Desc] = 'EOY Skins'
              AND (
                    P.Comment IS NULL 
                    OR TRIM(P.Comment) = ''
                    OR UPPER(P.Comment) LIKE '%2%'
                  )
        ) THEN 'Y' ELSE 'N'
    END AS InEOY2,

    MAX(LP.Skins) AS [Skins Amt],
    MAX(LP.Closest) AS [CTP Amt]

FROM Matches Ma

LEFT JOIN Scores S   
    ON S.League = Ma.League 
   AND S.Date = Ma.Date 
   AND S.Player = Ma.Player

LEFT JOIN LeagueParms LP 
    ON LP.Name = Ma.League 
   AND CAST(LP.Season AS TEXT) = SUBSTR(Ma.Date, 1, 4)

LEFT JOIN Handicaps H 
    ON H.League = Ma.League 
   AND H.Date = Ma.Date 
   AND H.Player = Ma.Player

LEFT JOIN Teams T 
    ON T.League = Ma.League 
   AND CAST(T.Year AS TEXT) = SUBSTR(Ma.Date, 1, 4)
   AND T.Player = Ma.Player

LEFT JOIN ScoreMethod M 
    ON M.League = Ma.League 
   AND M.Date = Ma.Date 
   AND M.Player = Ma.Player

LEFT JOIN Subs Su 
    ON Su.League = Ma.League 
   AND Su.Date = Ma.Date 
   AND Su.Player = Ma.Player

LEFT JOIN Payments SK 
    ON SK.League = Ma.League 
   AND SK.Date = Ma.Date 
   AND SK.Player = Ma.Player 
   AND SK.Detail LIKE '#%' 
   AND SK.[Desc] = 'Skin'

LEFT JOIN Payments CPDP 
    ON CPDP.League = Ma.League 
   AND CPDP.Date = Ma.Date 
   AND CPDP.Player = Ma.Player 
   AND CPDP.Detail LIKE '#%' 
   AND CPDP.[Desc] = 'CTP'

LEFT JOIN Payments SkP 
    ON SkP.League = Ma.League 
   AND SkP.Date = Ma.Date 
   AND SkP.Player = Ma.Player 
   AND SkP.Detail = 'Payment' 
   AND SkP.[Desc] = 'Skin'

LEFT JOIN Payments CPPlyrs 
    ON CPPlyrs.League = Ma.League 
   AND CPPlyrs.Date = Ma.Date 
   AND CPPlyrs.Player = Ma.Player 
   AND CPPlyrs.[Desc] = 'CTP'

WHERE Ma.Date >= '20180101'

GROUP BY Ma.Date, Ma.Player

ORDER BY Ma.Partner;"
            Using cmd As New SQLiteCommand(sqlMatchesScores, conn)
                cmd.ExecuteNonQuery()
            End Using

            Dim sql As String = $"SELECT * FROM vwMatchesScores WHERE Date = 20260414" '{ctx.ActiveDate}'"
            Dim dtl5 = oHelper.sqliteda(ctx.Conn, sql)

            oHelper.wk = ""
            Dim sqlScoresResults As String = "
DROP VIEW IF EXISTS vwScoreResults;

CREATE VIEW vwScoreResults AS

SELECT
    s.ID AS ScoreID,
    s.League,
    s.Player,
    p.FirstName,
    p.LastName,

    -- Team info
    t.Team,
    t.Grade,

    -- Match info
    m.Points,
    m.Team_Points,
    m.Opponent,
    m.Partner AS MatchPartner,
    m.Status AS MatchStatus,

    -- Score info
    s.Date,
    s.ActualDate,
    s.Round,
    s.[Group],        -- FIXED: reserved word
    s.FrontBack,

    -- Hole-by-hole scores
    s.[1] AS Hole1,
    s.[2] AS Hole2,
    s.[3] AS Hole3,
    s.[4] AS Hole4,
    s.[5] AS Hole5,
    s.[6] AS Hole6,
    s.[7] AS Hole7,
    s.[8] AS Hole8,
    s.[9] AS Hole9,

    s.Gross,
    s.Net,
    s.Partner AS ScorePartner

FROM Scores s
LEFT JOIN Players p
    ON p.Player = s.Player

LEFT JOIN Teams t
    ON t.Player = s.Player
    AND t.League = s.League
    AND t.Year = CAST(substr(s.Date, 1, 4) AS INTEGER)

LEFT JOIN Matches m
    ON m.Player = s.Player
    AND m.League = s.League
    AND m.Date = s.Date;"
            Using cmd As New SQLiteCommand(sqlScoresResults, conn)
                cmd.ExecuteNonQuery()
            End Using

            conn.Close()

        Catch ex As Exception
            _warnings.Add("View creation error: " & ex.Message)
        End Try
    End Sub
    Public Function GetWarnings() As List(Of String)
        Return _warnings
    End Function

    Public Function GetLeagueDashboard() As DataSet
        Return _dashboard
    End Function
    Public Function BuildTabControl() As TabControl
        Dim tc As New TabControl() With {.Dock = DockStyle.Fill}

        ' ImageList for icons
        Dim imgList As New ImageList()
        'imgList.Images.Add(My.Resources.golfball)   ' index 0
        'imgList.Images.Add(My.Resources.scorecard)  ' index 1
        'imgList.Images.Add(My.Resources.warning)    ' index 2
        imgList.Images.Add(SystemIcons.Application.ToBitmap()) ' index 0
        imgList.Images.Add(SystemIcons.Information.ToBitmap()) ' index 1
        imgList.Images.Add(SystemIcons.Warning.ToBitmap())     ' index 2
        tc.ImageList = imgList

        ' Create a ToolTip object
        Dim tabToolTip As New ToolTip()

        ' Add worksheet tabs
        For Each tbl As DataTable In _dashboard.Tables
            Dim grid As New DataGridView() With {
            .Dock = DockStyle.Fill,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .ReadOnly = True
        }
            grid.DataSource = tbl
            Dim tab As New TabPage(tbl.TableName)

            Select Case tbl.TableName.ToLower()
                Case "scores" : tab.ImageIndex = 1
                Case Else : tab.ImageIndex = 0
            End Select

            ' Tooltip shows row count
            tabToolTip.SetToolTip(tab, $"{tbl.TableName}: {tbl.Rows.Count} rows loaded")

            tab.Controls.Add(grid)
            tc.TabPages.Add(tab)
        Next

        ' Diagnostics tab
        Dim diagTable As New DataTable("Diagnostics")
        diagTable.Columns.Add("Table")
        diagTable.Columns.Add("RowCount", GetType(Integer))
        diagTable.Columns.Add("LastLoad", GetType(String))

        For Each tbl As DataTable In _dashboard.Tables
            Dim dr = diagTable.NewRow()
            dr("Table") = tbl.TableName
            dr("RowCount") = tbl.Rows.Count
            dr("LastLoad") = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            diagTable.Rows.Add(dr)
        Next

        For Each warn In _warnings
            Dim dr = diagTable.NewRow()
            dr("Table") = "⚠ Warning"
            dr("RowCount") = 0
            dr("LastLoad") = warn
            diagTable.Rows.Add(dr)
        Next

        Dim diagGrid As New DataGridView() With {
        .Dock = DockStyle.Fill,
        .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        .ReadOnly = True
    }
        diagGrid.DataSource = diagTable

        Dim diagTab As New TabPage("Diagnostics") With {.ImageIndex = 2}
        diagTab.Controls.Add(diagGrid)
        tc.TabPages.Add(diagTab)

        ' Tooltip for diagnostics tab
        tabToolTip.SetToolTip(diagTab, $"Diagnostics: {_warnings.Count} warnings logged")

        ' Enable owner-draw for custom colors + hover
        tc.DrawMode = TabDrawMode.OwnerDrawFixed
        AddHandler tc.DrawItem, AddressOf DrawColoredTabs
        AddHandler tc.MouseMove, AddressOf TabMouseMove
        AddHandler tc.MouseLeave, AddressOf TabMouseLeave

        Return tc
    End Function
    ' Handles custom coloring of tabs
    Private Sub DrawColoredTabs(sender As Object, e As DrawItemEventArgs)
        Dim tc = CType(sender, TabControl)
        Dim tab = tc.TabPages(e.Index)

        ' Default background color
        Dim backColor As Brush
        Select Case tab.Text.ToLower()
            Case "scores" : backColor = Brushes.LightGreen
            Case "diagnostics" : backColor = Brushes.LightYellow
            Case Else : backColor = Brushes.LightBlue
        End Select

        ' Highlight if hovered
        If e.Index = hoveredIndex Then
            backColor = Brushes.Orange
        End If

        e.Graphics.FillRectangle(backColor, e.Bounds)

        ' Draw tab text centered
        Dim textColor As Brush = Brushes.Black
        Dim sf As New StringFormat() With {
            .Alignment = StringAlignment.Center,
            .LineAlignment = StringAlignment.Center
        }
        e.Graphics.DrawString(tab.Text, tc.Font, textColor, e.Bounds, sf)
    End Sub
    Private hoveredIndex As Integer = -1

    Private Sub TabMouseMove(sender As Object, e As MouseEventArgs)
        Dim tc = CType(sender, TabControl)
        For i As Integer = 0 To tc.TabPages.Count - 1
            If tc.GetTabRect(i).Contains(e.Location) Then
                hoveredIndex = i
                tc.Invalidate()
                Exit Sub
            End If
        Next
    End Sub

    Private Sub TabMouseLeave(sender As Object, e As EventArgs)
        hoveredIndex = -1
        CType(sender, TabControl).Invalidate()
    End Sub
    Private Sub BuildTableFromSheet(ws As IXLWorksheet)
        Try
            Dim tableName As String = ws.Name.Trim()
            LOGIT("REBUILD: " & tableName & " at " & Now.ToString)
            ' ---------------------------------------------------------
            ' 1. Read header row
            ' ---------------------------------------------------------
            Dim headerRow = ws.FirstRowUsed()
            Dim headers As New List(Of String)

            For Each cell In headerRow.CellsUsed()
                Dim colName = cell.Value.ToString().Trim()
                If colName = "" Then Continue For
                headers.Add(colName)
            Next

            If headers.Count = 0 Then
                LOGIT($"Skipping sheet '{tableName}' — no headers found.")
                Return
            End If

            ' ---------------------------------------------------------
            ' 2. Auto-detect column types (INTEGER, REAL, TEXT)
            ' ---------------------------------------------------------
            Dim colTypes As New Dictionary(Of String, String)

            For i = 0 To headers.Count - 1
                Dim detectedType As String = "TEXT"

                For Each row In ws.RowsUsed().Skip(1)
                    Dim raw = row.Cell(i + 1).GetValue(Of String)().Trim()

                    If raw = "" Then Continue For

                    Dim intVal As Integer
                    Dim dblVal As Double

                    If Integer.TryParse(raw, intVal) Then
                        detectedType = "INTEGER"
                    ElseIf Double.TryParse(raw, dblVal) Then
                        detectedType = "REAL"
                    Else
                        detectedType = "TEXT"
                        Exit For
                    End If
                Next

                colTypes(headers(i)) = detectedType
            Next

            ' ---------------------------------------------------------
            ' 3. Drop existing table
            ' ---------------------------------------------------------
            Using conn As New SQLiteConnection($"Data Source={_dbPath};Version=3;")
                conn.Open()

                Using dropCmd As New SQLiteCommand($"DROP TABLE IF EXISTS [{tableName}];", conn)
                    dropCmd.ExecuteNonQuery()
                End Using

                ' ---------------------------------------------------------
                ' 4. Create table with typed columns
                ' ---------------------------------------------------------
                Dim colDefs As New List(Of String)
                For Each h In headers
                    colDefs.Add($"[{h}] {colTypes(h)}")
                Next

                Dim createSql = $"CREATE TABLE [{tableName}] ({String.Join(",", colDefs)});"

                Using createCmd As New SQLiteCommand(createSql, conn)
                    createCmd.ExecuteNonQuery()
                End Using

                ' ---------------------------------------------------------
                ' 5. Build parameterized insert with sanitized parameter names
                ' ---------------------------------------------------------
                Dim paramNames As New Dictionary(Of String, String)

                For Each h In headers
                    ' Replace spaces and punctuation for parameter names
                    Dim safeName = h
                    safeName = safeName.Replace(" ", "_")
                    safeName = safeName.Replace("-", "_")
                    safeName = safeName.Replace("/", "_")
                    safeName = safeName.Replace("(", "_").Replace(")", "_")

                    paramNames(h) = "@" & safeName
                Next

                Dim insertSql =
                $"INSERT INTO [{tableName}] ({String.Join(",", headers.Select(Function(h) $"[{h}]"))}) " &
                $"VALUES ({String.Join(",", headers.Select(Function(h) paramNames(h)))});"

                Using insertCmd As New SQLiteCommand(insertSql, conn)
                    ' Create parameters once
                    For Each h In headers
                        insertCmd.Parameters.Add(New SQLiteParameter(paramNames(h), DbType.String))
                    Next

                    ' ---------------------------------------------------------
                    ' 6. Insert rows with NULL support
                    ' ---------------------------------------------------------
                    For Each row In ws.RowsUsed().Skip(1)
                        For i = 0 To headers.Count - 1
                            Dim raw = row.Cell(i + 1).GetValue(Of String)().Trim()

                            If raw = "" Then
                                insertCmd.Parameters(paramNames(headers(i))).Value = DBNull.Value
                            Else
                                insertCmd.Parameters(paramNames(headers(i))).Value = raw
                            End If
                        Next

                        insertCmd.ExecuteNonQuery()
                    Next
                End Using

                ' ---------------------------------------------------------
                ' 7. Row count logging
                ' ---------------------------------------------------------
                Dim rowCount As Integer = 0
                Using countCmd As New SQLiteCommand($"SELECT COUNT(*) FROM [{tableName}];", conn)
                    rowCount = CInt(countCmd.ExecuteScalar())
                End Using

                LOGIT($"Table '{tableName}' rebuilt successfully with {rowCount} rows.")
            End Using
            LOGIT("REBUILD COMPLETE: " & tableName & " at " & Now.ToString)
        Catch ex As Exception
            _warnings.Add($"Error building table '{ws.Name}': {ex.Message}")
            LOGIT($"Error building table '{ws.Name}': {ex.Message}")
        End Try

    End Sub
End Class