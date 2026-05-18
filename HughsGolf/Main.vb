Imports System.Data.SQLite
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports ClosedXML.Excel
Imports HughsGolf.Helper

Public Class Main
    Public oHelper As New Helper
    Dim oClosedXML As New ClosedXML
    'Private MLogit As Action(Of String) = Sub(msg)
    '                                          LOGIT(msg, True) ' Let the method handle the optional parameter
    '                                      End Sub

    'Public tables As New List(Of DataTable)()
    Public lPar3s = New List(Of String)
    Public lPar3sFront = New List(Of String)
    Public lpar3sBack = New List(Of String)
    Public dtLeagueParms As DataTable
    Public dtPar3s As DataTable

    Public connection As SQLiteConnection
    Public iScreenWidth As Integer
    Public iScreenHeight As Integer

    Dim sToday As String = $"{CDate(Now):yyyyMMdd}"
    Public ihalf As Integer
    'Dim half1Start As Integer
    'Dim half1End As String
    'Dim half2Start As String
    'Dim half2End As String
    Dim wk As String
    Dim newYear As Integer = Now.Year
    Dim month As Integer = 4
    Dim dtLP As DataTable
    Dim iTeams As Integer
    Dim sSplitSeason As String
    Dim sDays As Integer
    Public firstTuesday As Date
    Dim EndDate As Date

    Dim sb As StringBuilder
    Dim bFormLoaded As Boolean = False
    Dim eUtil As New eUtilities
    Public Sub SyncToContext()
        ' Push the current state into the Singleton
        ctx = LeagueContext.Instance
        ctx.oHelper = Me.oHelper
        ctx.Conn = Me.connection

        ctx.Conn = New SQLiteConnection("Data Source=C:\HughsGolf\Files\HughsGolf.db;Version=3;")
        ctx.Conn.Open()
        ctx.rLeagueParmrow = ctx.oHelper.sqlitedaFromSql(connection, "", "SELECT * FROM LeagueParms ORDER BY Season DESC LIMIT 1 ")(0)
        ctx.sLeagueName = ctx.rLeagueParmrow("Name").ToString
        ctx.SafeLeagueName = ctx.rLeagueParmrow("Name").ToString.Replace("'", "''")

        Dim dtp As DateTime = DateTime.Parse(ctx.rLeagueParmrow("StartDate"))

        ctx.ActiveDate = dtp.ToString("yyyyMMdd")
        ' 1. Cast the DB object to a real DateTime
        Dim psDate As DateTime = CDate(ctx.rLeagueParmrow("PostSeasonDt"))

        ' 2. Format it into your standard yyyyMMdd string
        ctx.sPSDate = psDate.ToString("yyyyMMdd")

        ' 3. Calculate the "Grace Period" date (7 days later)
        ctx.sPSDate2 = psDate.AddDays(7).ToString("yyyyMMdd")

        ' 4. Set the SeasonYear while you're at it
        ctx.SeasonYear = psDate.Year.ToString()
        Dim allScreens = Screen.AllScreens
        Dim Current_Screen As Screen = Screen.FromControl(Me)
        If Current_Screen.Primary Then
            Dim HCenter = Current_Screen.Bounds.Left +
            (((Current_Screen.Bounds.Right - Current_Screen.Bounds.Left) / 2) - ((Me.Width) / 2))
            Dim VCenter = (Current_Screen.Bounds.Bottom / 2) - ((Me.Height) / 2)
            Me.StartPosition = FormStartPosition.Manual
            Me.Location = New Point(HCenter, VCenter)
        Else
            Me.StartPosition = FormStartPosition.CenterScreen
        End If

        ctx.iScreenWidth = Screen.PrimaryScreen.Bounds.Width
        ctx.iScreenHeight = Screen.PrimaryScreen.Bounds.Height
    End Sub

    Private isInitializing As Boolean = False
    Private Sub LoadLeaguesCombo()
        isInitializing = True ' Stop all events from firing
        Try
            oHelper.SQLiteFillComboFromQuery(
            connection,
            "SELECT DISTINCT Name FROM LeagueParms ORDER BY Name COLLATE NOCASE",
            cbLeagues,
            noDataText:="(No leagues found)"
        )
        Finally
            isInitializing = False ' Re-enable events
        End Try
    End Sub
    Private Sub LoadSeasonsForLeague(leagueName As String)
        ' 1. Create the dictionary for the parameter
        Dim sqlParams As New Dictionary(Of String, Object) From {
        {"@League", leagueName}
    }

        ' 2. Call the sub using named arguments for clarity
        oHelper.SQLiteFillComboFromQuery(
        connection:=connection,
        sql:="SELECT DISTINCT CAST(Season AS TEXT)
              FROM LeagueParms 
              WHERE Name = @League 
              ORDER BY Season DESC",
        targetCombo:=cbSeasons,
        parameters:=sqlParams,
        noDataText:=$"(No seasons found for {leagueName})"
    )
    End Sub

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SyncToContext()
        'GoTo skiploader
        Dim loader As New LeagueDatabaseLoader()
        loader.Initialize()
skiploader:
        Dim dt As New DataTable()

        ' Build TabControl with all worksheets + diagnostics
        'Dim tc As TabControl = loader.BuildTabControl()
        'add TabControl to form
        'Me.Controls.Add(tc)
        'Exit Sub
        'debug carrier
        'Dim ocarrier = New GetCarrier
        'Dim scarrier = ocarrier.GetCarrierInfo("8599628088")
        Dim smsg = ""
        'If Debugger.IsAttached Then
        '    smsg = "Debugging is enabled"
        '    MessageBox.Show(smsg, "Debug Mode", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'End If
        Me.Text = $"{smsg} Hugh's Golf {Now.Year} - {Now.Month}/{Now.Day} {Now.Hour}:{Now.Minute}:{Now.Second}"
        'Dim allScreens = Screen.AllScreens
        'Dim Current_Screen As Screen = Screen.FromControl(Me)
        'If Current_Screen.Primary Then
        '    Dim HCenter = Current_Screen.Bounds.Left +
        '    (((Current_Screen.Bounds.Right - Current_Screen.Bounds.Left) / 2) - ((Me.Width) / 2))
        '    Dim VCenter = (Current_Screen.Bounds.Bottom / 2) - ((Me.Height) / 2)
        '    Me.StartPosition = FormStartPosition.Manual
        '    Me.Location = New Point(HCenter, VCenter)
        'Else
        '    Me.StartPosition = FormStartPosition.CenterScreen
        'End If
        'iScreenWidth = Screen.PrimaryScreen.Bounds.Width
        'iScreenHeight = Screen.PrimaryScreen.Bounds.Height

        'oHelper.sFilePath = "C:\Logs"
        connection = ctx.Conn 'New SQLiteConnection(Constants.connectionString)
        '20260121-use db created earlier
        'connection = New SQLiteConnection("Data Source=C:\HughsGolf\Files\HughsGolf.db;Version=3;")
        'this prevents sqlite warning logging
        'SQLiteConnection.SetTrace(False)
        'connection.Open()

        SQLiteLog.Enabled = False
        'oHelper.bloghelper = False
        tssl.Text = "Loading tables"
        lblProcessMsg.Visible = True
        lblProcessMsg.Text = "Loading tables"
        oHelper.status_Msg(lblProcessMsg, Me)

        tspb.Visible = False
        'oHelper.buildtablesfromworksheet(connection, csvFilePath)

        GoTo skipdebug
#Region "Debug"
        sb = New StringBuilder
        sb.AppendLine("
SELECT 
    Player, 
    substr(date, 1, 4) AS Year,
    SUM(
        CASE WHEN Cast(E.CTP_2 as decimal) > 0 THEN 1 ELSE 0 END +
        CASE WHEN Cast(E.CTP_1 as decimal) > 0 THEN 1 ELSE 0 END
    ) AS CTPs
FROM Earnings E
GROUP BY Player, Year;
")
        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", sb.ToString)

#End Region
skipdebug:
        showtables()
        cbTables.Sorted = True
        lblProcessMsg.Text = "Finished Loading tables, getting league parms"
        oHelper.status_Msg(lblProcessMsg, Me)
        LOGIT("All tables built. getting league parms")
        'oHelper.showtables(connection)

        'SELECT PostSeasonDt,DATE(SUBSTR(PostSeasonDt, 7, 4) || '-' || SUBSTR(PostSeasonDt, 1, 2) || '-' || SUBSTR(PostSeasonDt, 4, 2), '+7 days') AS AdjustedDate FROM LeagueParms;
        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", "SELECT Season FROM LeagueParms ORDER BY Season DESC")
        newYear = ctx.rLeagueParmrow("Season") + 1
        iTeams = ctx.rLeagueParmrow("Teams")
        sDays = iTeams * (If(ctx.rLeagueParmrow("SplitSeason") = "Y", 2, 0)) * 7
        'oHelper.sPSDate = GetFirstTuesday(newYear, month).AddDays(sDays).ToString("M/d/yyyy", CultureInfo.InvariantCulture)
        oHelper.sPSDate = ctx.sPSDate 'CDate(dtLP(0)("PostSeasonDt")).ToString("yyyyMMdd")
        oHelper.sPSDate2 = ctx.sPSDate2 'CDate(dtLP(0)("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd")
        'sEndDate = GetFirstTuesday(newYear, month).AddDays(sDays - 7).ToString("M/d/yyyy", CultureInfo.InvariantCulture)

        'GoTo skipsetup
        If Now.ToString("yyyyMMdd") > ctx.sPSDate2 Then
            SetupSeason()
        End If
        'setupwksch(oHelper.dt, "Schedule")
        'oHelper.dt = oHelper.sqliteda(connection, "wkSchedule")
skipsetup:
        Dim x = ""
        LoadLeaguesCombo()

        ' Remove the auto-select here
        ' cbLeagues.SelectedIndex = 0   ← comment out or remove

        ' Instead, do it manually without triggering recursion
        If cbLeagues.Items.Count > 0 Then
            isLoading = True
            cbLeagues.SelectedIndex = 0
            isLoading = False
        End If
        'Dim dtSchedule = New DataTable
        'setupwksch(dtSchedule, "Schedule")
        'oHelper.SQLiteCreateTableFromDT(connection, dtSchedule)
        'GoTo bldSkinCTP
skipschbuild:
        oHelper.iHoles = ctx.rLeagueParmrow("Holes")
        oHelper.CalcHoleMarker(cbDates.SelectedItem)
        'If cbDates.SelectedItem = "20251007" Then
        'oHelper.iHoleMarker = 10
        'End If

        If ctx.rLeagueParmrow("CarryOver") = "Y" Then
            oHelper.CalcLeftovers(connection)
        End If
        '20260212-not needed?
        '        Using cn As New SQLiteConnection("Data Source=C:\HughsGolf\Files\HughsGolf.db;Version=3;")
        '            cn.Open()

        '            Using cmd As New SQLiteCommand("SELECT * FROM vwScoreDataView --WHERE Date = @dt", cn)
        '                cmd.Parameters.AddWithValue("@dt", ctx.ActiveDate)

        '                Using da As New SQLiteDataAdapter(cmd)
        '                    da.Fill(dt)
        '                End Using
        '            End Using
        '            Using cmd As New SQLiteCommand($"SELECT COUNT(*) FROM [Scores];
        'SELECT COUNT(*) FROM (SELECT DISTINCT * FROM [Scores]);", cn)
        '                Using reader As SQLiteDataReader = cmd.ExecuteReader()
        '                    While reader.Read()
        '                        Dim totalScores As Integer = reader.GetInt32(0)
        '                        reader.NextResult()
        '                        If reader.Read() Then
        '                            Dim distinctScores As Integer = reader.GetInt32(0)
        '                            If totalScores <> distinctScores Then
        '                                LOGIT($"Data integrity issue: Total Scores = {totalScores}, Distinct Scores = {distinctScores}")
        '                            End If
        '                        End If
        '                    End While
        '                End Using
        '            End Using
        '            cn.Close()
        '        End Using
        '20260212-en not needed?
        'oHelper.dt = oHelper.sqliteda(connection, "Courses")
        'AddHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged
        'AddHandler rb2ndHalf.CheckedChanged, AddressOf rb2ndHalf_CheckedChanged
        Dim strsql = New Text.StringBuilder
        GoTo skipFix
#Region "skipfix"
        'sqlite version
        strsql.AppendLine($"
WITH RankedScores AS (
    SELECT 
        LP.Season,
        S.*,
        ROW_NUMBER() OVER (
            PARTITION BY LP.Season 
            ORDER BY S.Date DESC
        ) AS rn
    FROM LeagueParms LP
    JOIN Scores S ON SUBSTR(S.Date, 1, 4) = LP.Season
)
SELECT Date
FROM RankedScores
WHERE rn = 1;
")
        Dim ddates = oHelper.sqlitedaFromSql(connection, "", strsql.ToString)

        For Each lparm As DataRow In ddates.Rows
            oHelper.lignoreDates.Add(CDate(lparm("PostSeasonDt")).ToString("yyyyMMdd"))
            oHelper.lignoreDates.Add(CDate(lparm("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd"))
        Next

        For Each sdate In oHelper.lignoreDates
            Dim lastScoreDate As String
            oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT Date FROM Scores WHERE SUBSTR(Date,1,4) = '{sdate.ToString.Substring(0, 4)}' ORDER BY Date Desc LIMIT 1")
            If oHelper.dt.Rows.Count > 0 Then
                lastScoreDate = oHelper.dt.Rows(0)(0)
            End If

            'Dim PSDate As DateTime = lparm("Post").AddDays(7)
            'If lprow("Season") = Now.Year And parsedDate < Now Then
            '    lastScoreDate = ""
            'Else
            '    If lastScoreDate IsNot Nothing Then
            '        parsedDate = DateTime.ParseExact(lastScoreDate, format, System.Globalization.CultureInfo.InvariantCulture)
            '        PSDate = parsedDate.AddDays(-7)
            '    End If
            'End If
            If Not oHelper.lignoreDates.contains(lastScoreDate) Then
                If sdate = oHelper.lignoreDates(oHelper.lignoreDates.count - 1) Then
                    oHelper.sexclDates &= $"'{sdate}'"
                Else
                    oHelper.sexclDates &= $"'{sdate}',"
                End If

            End If
        Next
        'oHelper.showtables(connection)
#End Region

skipFix:
        strsql.AppendLine($"SELECT EndDate,PostSeasonDT FROM LeagueParms")
        Dim dtdates = oHelper.sqlitedaFromSql(connection, "", strsql.ToString)

        For Each lparm As DataRow In dtdates.Rows
            oHelper.wk = ""
            Dim sendyr As DateTime = lparm("EndDate")
            Dim lastScoreDate As String
            oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT Date FROM Schedule WHERE SUBSTR(Date,1,4) = '{sendyr.Year}' ORDER BY Date Desc LIMIT 1")
            If oHelper.dt.Rows.Count > 0 Then
                lastScoreDate = oHelper.dt.Rows(0)(0)
            End If

            If lastScoreDate <> "" Then
                If lastScoreDate < CDate(lparm("PostSeasonDt")).ToString("yyyyMMdd") Then
                    oHelper.lignoreDates.Add(CDate(lparm("PostSeasonDt")).ToString("yyyyMMdd"))
                    oHelper.lignoreDates.Add(CDate(lparm("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd"))
                End If
            Else
                oHelper.lignoreDates.Add(CDate(lparm("PostSeasonDt")).ToString("yyyyMMdd"))
                oHelper.lignoreDates.Add(CDate(lparm("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd"))
            End If
        Next

        For Each sdate In oHelper.lignoreDates
            'if this is the last date in ignore dates, dont add a comma
            If sdate = oHelper.lignoreDates(oHelper.lignoreDates.count - 1) Then
                ctx.sExcludeDates &= $"'{sdate}'"
            Else
                ctx.sExcludeDates &= $"'{sdate}',"
            End If
            'End If
        Next
        'oHelper.showtables(connection)
        'oHelper.CreateChangeLog()
        'oHelper.dt = oHelper.sqliteda(connection, "Changelog")
        bFormLoaded = True
        eUtil.sThisEmailService = "Gmail"
        eUtil.sThisEmailUser = ctx.rLeagueParmrow("email")
        eUtil.sThisEmailPassword = ctx.rLeagueParmrow("emailpassword")

        'eUtil.SendEmails()
        'eUtil.Rainout()
        '20251104-alread done above
        'If ctx.rLeagueParmrow("CarryOver") = "Y" Then oHelper.CalcCarryOvers()
        oHelper.wk = ""
        'For Each season As String In oHelper.sqliteda(connection, "LeagueParms").AsEnumerable().Select(Function(r) r("Season").ToString()).Distinct()
        '    Dim sql = $"CREATE INDEX IF NOT EXISTS idx_LeagueParms_Season ON LeagueParms({season});"
        '    Using cmd As New SQLiteCommand(sql, connection)
        '        cmd.ExecuteNonQuery()
        '    End Using
        'Next
        Dim cbd = cbDates.Items
        'one time fixes
        Dim lrcu As Long
        'lrcu = oHelper.SqliteTrans("UPDATE Subs SET Team = Teams WHERE Player = 'Tony Fey'")

        Dim btnReplay As New Button()
        btnReplay.Name = "btnReplayScores"
        btnReplay.Text = "🔄 Replay Scores"
        btnReplay.Font = New Font("Segoe UI", 9, FontStyle.Bold)
        btnReplay.Size = New Size(120, 30)
        btnReplay.Location = New Point(btnScoreCard.Right + 10, btnScoreCard.Top)
        btnReplay.BackColor = Color.FromArgb(0, 100, 180)
        btnReplay.ForeColor = Color.White
        btnReplay.FlatStyle = FlatStyle.Flat
        btnReplay.FlatAppearance.BorderSize = 0
        Me.Controls.Add(btnReplay)
        AddHandler btnReplay.Click, Sub(s, ev) ReplayAllScores()
    End Sub
    Sub SetupSeason()
        If MessageBox.Show($"{newYear} not setup yet, want to setup now?",
                       "Season Not Setup",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        ' -------------------------------------------------------------------------
        ' 1. Informational message
        ' -------------------------------------------------------------------------
        Dim msg As New Text.StringBuilder
        msg.AppendLine($"Season Setup for {newYear}")
        msg.AppendLine($"LeagueParms – new row will be created")
        msg.AppendLine($"Teams – last season’s players will be copied")
        msg.AppendLine($"Schedule – dates generated up to postseason + 7 days")
        MessageBox.Show(msg.ToString(), "Season Setup", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Dim lastYear As Integer = newYear - 1

        ' -------------------------------------------------------------------------
        ' 2. Copy Teams from last season
        ' -------------------------------------------------------------------------
        Using conn As New SQLiteConnection(connection)
            conn.Open()

            Using tran = conn.BeginTransaction()
                Try
                    Dim sqlTeams As String =
                    "INSERT INTO Teams (ID, Team, Grade, Player, League, Year) " &
                    "SELECT (SELECT IFNULL(MAX(ID),0)+1 FROM Teams), Team, Grade, Player, League, @newYear " &
                    "FROM Teams WHERE Year = @lastYear"

                    Using cmd As New SQLiteCommand(sqlTeams, conn, tran)
                        cmd.Parameters.AddWithValue("@newYear", newYear)
                        cmd.Parameters.AddWithValue("@lastYear", lastYear)
                        cmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show($"Failed copying Teams: {ex.Message}", "Error")
                    Exit Sub
                End Try
            End Using
        End Using

        oHelper.dt = oHelper.sqliteda(connection, "Teams")

        ' -------------------------------------------------------------------------
        ' 3. Build Schedule (only up to postseason + 7 days)
        ' -------------------------------------------------------------------------
        'firstTuesday = GetFirstTuesday(newYear, month).AddDays(7)
        ' -------------------------------------------------------------------------
        ' 3. Build Schedule (only up to postseason + 7 days)
        ' -------------------------------------------------------------------------
        ' REPLACED: firstTuesday = GetFirstTuesday(newYear, month).AddDays(7)
        ' NEW: Prompt user to select a Tuesday starting in April
        firstTuesday = GetUserSelectedTuesday(newYear)

        If firstTuesday = DateTime.MinValue Then
            ' User cancelled or closed the box, handle exit or default
            Return
        End If
        Dim firstTuesdayDate As Date = firstTuesday

        Dim sch As New SchGenerator
        sch.Main()

        ' Load generated schedule rows
        oHelper.dt = oHelper.sqliteda(connection, "Schedule")
        Dim dv = oHelper.dt.DefaultView
        dv.Sort = "Date ASC"

        ' Determine last regular-season date
        Dim endDateObj As Date =
        DateTime.ParseExact(dv(dv.Count - 1)("Date").ToString(), "yyyyMMdd", Nothing)

        EndDate = endDateObj.ToString("MM/dd/yyyy")

        ' Postseason = last date + 7 days
        oHelper.sPSDate = endDateObj.AddDays(7)

        ' Save archive table ScheduleYY
        oHelper.dt.TableName = $"Schedule{newYear.ToString().Substring(2, 2)}"
        oHelper.SQLiteCreateTableFromDT(connection, dv.ToTable)

        showtables()

        ' -------------------------------------------------------------------------
        ' 4. Copy LeagueParms row (your exact schema)
        ' -------------------------------------------------------------------------
        Dim lp As DataTable = oHelper.sqliteda(connection, "LeagueParms")
        Dim lastLP As DataRow = lp.Select($"Season = '{lastYear}'").First()

        Using conn As New SQLiteConnection(connection)
            conn.Open()

            Using tran = conn.BeginTransaction()
                Try
                    Dim sqlLP As String =
                    "INSERT INTO LeagueParms (" &
                    "ID, Name, Secretary, Format, Cost, [EOY Skins], Teams, PayPlaces, Season, DayofWeek, " &
                    "StartDate, EndDate, PostSeasonDt, Rainouts, HdcpScores, HdcpFormat, CarryLastYears, " &
                    "MaxHdcp, Par3Max, Par4Max, Par5Max, Course, Holes, Start9, Method, Skins, Closest, " &
                    "SkinsPS, ClosestPS, SplitSeason, SkinFmt, Email, EmailPassword, WebsitePassword, " &
                    "Byes, CarryOver) " &
                    "SELECT " &
                    "(SELECT IFNULL(MAX(ID),0)+1 FROM LeagueParms), " &
                    "Name, Secretary, Format, Cost, [EOY Skins], Teams, PayPlaces, @newSeason, DayofWeek, " &
                    "@startDate, @endDate, @psDate, Rainouts, HdcpScores, HdcpFormat, CarryLastYears, " &
                    "MaxHdcp, Par3Max, Par4Max, Par5Max, Course, Holes, Start9, Method, Skins, Closest, " &
                    "SkinsPS, ClosestPS, SplitSeason, SkinFmt, Email, EmailPassword, WebsitePassword, " &
                    "Byes, CarryOver " &
                    "FROM LeagueParms WHERE ID = @oldId"

                    Using cmd As New SQLiteCommand(sqlLP, conn, tran)
                        cmd.Parameters.AddWithValue("@newSeason", newYear)
                        cmd.Parameters.AddWithValue("@startDate", firstTuesdayDate)
                        cmd.Parameters.AddWithValue("@endDate", endDateObj)
                        cmd.Parameters.AddWithValue("@psDate", oHelper.sPSDate)
                        cmd.Parameters.AddWithValue("@oldId", lastLP("ID"))
                        cmd.ExecuteNonQuery()
                    End Using

                    tran.Commit()
                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show($"Failed copying LeagueParms: {ex.Message}", "Error")
                    Exit Sub
                End Try
            End Using
        End Using

        dtLP = oHelper.sqliteda(connection, "LeagueParms")
        oHelper.SQLiteCreateTableFromDT(connection, oHelper.dt)
    End Sub
    Public Function GetUserSelectedTuesday(year As Integer) As DateTime
        Dim tuesdays As New List(Of DateTime)
        ' April is Month 4. Start at April 1st of the newYear
        Dim dt As New DateTime(year, 4, 1)

        ' Find the first Tuesday in April
        While dt.DayOfWeek <> DayOfWeek.Tuesday
            dt = dt.AddDays(1)
        End While

        ' Get all Tuesdays in April (usually 4 or 5)
        While dt.Month = 4
            tuesdays.Add(dt)
            dt = dt.AddDays(7)
        End While

        ' --- Create a Quick Dynamic Form ---
        Dim prompt As New Form()
        prompt.Width = 300
        prompt.Height = 150
        prompt.Text = "Override Start Date"
        prompt.StartPosition = FormStartPosition.CenterScreen
        prompt.FormBorderStyle = FormBorderStyle.FixedDialog
        prompt.MaximizeBox = False

        Dim lbl As New Label() With {.Text = "Select Start Tuesday:", .Left = 20, .Top = 20, .Width = 250}
        Dim cmb As New ComboBox() With {.Left = 20, .Top = 45, .Width = 240, .DropDownStyle = ComboBoxStyle.DropDownList}
        Dim btn As New Button() With {.Text = "OK", .Left = 185, .Top = 80, .DialogResult = DialogResult.OK}

        ' Populate the Combobox
        For Each t In tuesdays
            cmb.Items.Add(t.ToString("MMMM dd, yyyy"))
        Next
        cmb.SelectedIndex = 0

        prompt.Controls.Add(lbl)
        prompt.Controls.Add(cmb)
        prompt.Controls.Add(btn)
        prompt.AcceptButton = btn

        If prompt.ShowDialog() = DialogResult.OK Then
            Return tuesdays(cmb.SelectedIndex)
        Else
            Return DateTime.MinValue
        End If
    End Function
    Sub showtables()
        ' SQL query to list all tables
        Dim query As String = "SELECT * FROM sqlite_master WHERE type = 'table';"
        If ctx.Conn.State = ConnectionState.Closed Then ctx.Conn.Open()
        Using command As New SQLiteCommand(query, ctx.Conn)
            Using reader As SQLiteDataReader = command.ExecuteReader()
                While reader.Read()
                    If reader("Name").ToString.StartsWith("Z_") Or reader("Name").ToString = "SheetMetadata" Then Continue While
                    cbTables.Items.Add(reader("name"))
                End While
            End Using
        End Using
        'query = "SELECT * FROM sqlite_master WHERE type = 'view';"
        'Using command As New SQLiteCommand(query, ctx.Conn)
        '    Using reader As SQLiteDataReader = command.ExecuteReader()
        '        While reader.Read()
        '            ''LOGIT($"{reader("name")}")
        '            'Console.WriteLine(reader("name"))
        '            cbTables.Items.Add(reader("name"))
        '        End While
        '    End Using
        'End Using

    End Sub

#Region "Not Needed"
    Sub InsertPayments()

        '----------------------------------------------------------------------------------------------------------------------------------------------------
        '   Update the payments table for this new year
        '----------------------------------------------------------------------------------------------------------------------------------------------------
        Dim PaymentsFlds = "ID,League,Date,Player,Desc,Detail,Earned"
        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT {PaymentsFlds} FROM Payments ORDER BY ID DESC LIMIT 1 ")
        Dim sID = oHelper.dt.Rows(0)(0)
        Dim scols = ""

        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT {PaymentsFlds} FROM Payments WHERE Desc = 'League Dues' AND Detail = 'Invoice' ORDER BY ID DESC LIMIT 1 ")
        Dim dtTeams As DataTable = oHelper.sqlitedaFromSql(connection, "Teams", $"SELECT * FROM Teams WHERE Year = {ctx.rLeagueParmrow("Season")}")
        For Each team As DataRow In dtTeams.Rows
            sID += 1
            If team(Constants.Player) = dtTeams.Rows(0)(Constants.Player) Then
                For i = 0 To oHelper.dt.Columns.Count - 1
                    Dim col As DataColumn = oHelper.dt.Columns(i)
                    scols &= If(i = oHelper.dt.Columns.Count - 1, $"[{col.ColumnName}]", $"[{col.ColumnName}],")
                Next
            End If
            Using transaction = connection.BeginTransaction()
                Using cmd As New SQLiteCommand(connection)
                    ' Prepare the SQL command
                    cmd.CommandText = $"
INSERT INTO Payments ({scols})
VALUES ({sID },
'{ctx.sLeagueName}',
'{CDate(ctx.rLeagueParmrow("StartDate")):yyyyMMdd}',
'{team(Constants.Player)}',
'{oHelper.dt.Rows(0)("Desc")}',
'{oHelper.dt.Rows(0)("Detail")}',
'{ctx.rLeagueParmrow("Cost")}')"
                    LOGIT(cmd.CommandText)
                    ' Execute the command
                    Dim lrc = cmd.ExecuteNonQuery()
                End Using

                Try
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                End Try
            End Using
        Next
        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT {PaymentsFlds} FROM Payments WHERE Desc = 'EOY Skins' AND Detail = 'Invoice' ORDER BY ID DESC LIMIT 1 ")
        For Each team As DataRow In dtTeams.Rows
            sID += 1
            If team(Constants.Player) = dtTeams.Rows(0)(Constants.Player) Then
                scols = ""
                For i = 0 To oHelper.dt.Columns.Count - 1
                    Dim col As DataColumn = oHelper.dt.Columns(i)
                    scols &= If(i = oHelper.dt.Columns.Count - 1, $"[{col.ColumnName}]", $"[{col.ColumnName}],")
                Next
            End If

            Using cmd As New SQLiteCommand(connection)
                ' Prepare the SQL command
                cmd.CommandText = $"INSERT INTO Payments ({scols})
                               VALUES ({sID },'{ctx.sLeagueName}','{CDate(ctx.rLeagueParmrow("StartDate")):yyyyMMdd}','{team(Constants.Player)}','{oHelper.dt.Rows(0)("Desc")}','{oHelper.dt.Rows(0)("Detail")}','{ctx.rLeagueParmrow("EOY Skins")}')"
                LOGIT(cmd.CommandText)
                ' Execute the command
                cmd.ExecuteNonQuery()
            End Using
        Next
        oHelper.dt = oHelper.sqliteda(connection, "Payments")
        'this updates the sqlite table in memory
        oHelper.SQLiteCreateTableFromDT(connection, oHelper.dt)

    End Sub
#End Region
    Function GetFirstTuesday(year As Integer, month As Integer) As DateTime
        ' Start from the first day of the given month
        Dim firstDay As New DateTime(year, month, 1)

        ' Loop until we find the first Tuesday
        While firstDay.DayOfWeek <> DayOfWeek.Tuesday
            firstDay = firstDay.AddDays(1)
        End While

        Return firstDay
    End Function
    Private Sub rebuildDates(cbDates As ComboBox)
        Try
            Dim seasonPrefix As String = cbSeasons.SelectedItem.ToString()
            Dim nowString As String = DateTime.Now.ToString("yyyyMMdd")
            Dim futureLimit As String = seasonPrefix & "9999"
            Dim endValue As String = If(futureLimit > nowString, nowString, futureLimit)

            Dim sqlParams As New Dictionary(Of String, Object) From {
            {"@Start", seasonPrefix & "0000"},
            {"@End", endValue}
        }

            ' Get ALL scheduled dates for season (including future) for half calculation
            Dim allScheduledDates As New List(Of String)
            Dim schedAllSql As String = "SELECT Date FROM Schedule WHERE Date LIKE @Pattern ORDER BY Date ASC"
            Dim dtSched = oHelper.sqliteda("Schedule", schedAllSql, New Dictionary(Of String, Object) From {
            {"@Pattern", seasonPrefix & "%"}
        })
            For Each row As DataRow In dtSched.Rows
                Dim d As String = row("Date").ToString()
                If Not allScheduledDates.Contains(d) Then allScheduledDates.Add(d)
            Next

            ' Calculate halves from full schedule
            If allScheduledDates.Count > 0 Then
                Dim schedMid As Integer = allScheduledDates.Count \ 2
                ctx.sFirstHalfEndDate = allScheduledDates(schedMid - 1)
                ctx.sSecondHalfEndDate = allScheduledDates(allScheduledDates.Count - 1)
            End If

            ' Get dates from Scores first (up to today only)
            Dim sql As String = "SELECT DISTINCT Date FROM Scores WHERE Date BETWEEN @Start AND @End ORDER BY Date DESC"
            Dim dt As DataTable = oHelper.sqliteda("Dates", sql, sqlParams)

            ' Fallback to Schedule if no scores exist
            If dt.Rows.Count = 0 Then
                Dim schedSql As String = "SELECT Date FROM Schedule WHERE Date LIKE @Pattern AND Date <= @Today ORDER BY Date ASC"
                dt = oHelper.sqliteda("Schedule", schedSql, New Dictionary(Of String, Object) From {
                {"@Pattern", seasonPrefix & "%"},
                {"@Today", nowString}
            })
                ' If still in future keep only first date
                If dt.Rows.Count > 0 Then
                    Dim firstSchedDate As String = dt.Rows(0)("Date").ToString()
                    If firstSchedDate > nowString Then
                        Dim tempDt As DataTable = dt.Clone()
                        tempDt.ImportRow(dt.Rows(0))
                        dt = tempDt
                    End If
                End If
            End If

            ' Build allDates list
            Dim allDates As New List(Of String)
            allDates.Clear()

            For Each row As DataRow In dt.Rows
                Dim d As String = row("Date").ToString()
                If Not allDates.Contains(d) Then allDates.Add(d)
            Next

            ' Add next scheduled date if today is past last score date
            If allDates.Count > 0 Then
                Dim lastScoreDate As String = allDates(0)
                If nowString > lastScoreDate Then
                    Dim nextDate As String = oHelper.SQLiteExecuteScalar(
                    $"SELECT Date FROM Schedule 
                    WHERE Date LIKE '{seasonPrefix}%' 
                    AND Date > '{lastScoreDate}' 
                    ORDER BY Date ASC LIMIT 1").ToString()

                    If nextDate <> "0" AndAlso nextDate <> "" AndAlso Not allDates.Contains(nextDate) Then
                        allDates.Add(nextDate)
                    End If
                End If
            End If

            ' Add Post-Season date if applicable
            If nowString >= ctx.sPSDate AndAlso Not allDates.Contains(ctx.sPSDate) Then
                allDates.Add(ctx.sPSDate)
            End If

            ' Add all postseason dates from Scores table
            If nowString >= ctx.sPSDate Then
                Using cmd As New SQLiteCommand("
        SELECT DISTINCT Date FROM Scores 
        WHERE League=@League
          AND Date >= @PSDate
          AND Date <= @Today
          AND SUBSTR(Date,1,4) = @Season
          AND Gross IS NOT NULL
        ORDER BY Date DESC", ctx.Conn)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@PSDate", ctx.sPSDate)
                    cmd.Parameters.AddWithValue("@Today", nowString)
                    cmd.Parameters.AddWithValue("@Season", ctx.SeasonYear)

                    If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()
                    Using dr As SQLiteDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim d As String = dr("Date").ToString()
                            If Not allDates.Contains(d) Then allDates.Add(d)
                        End While
                    End Using
                End Using
            End If

            ' Sort descending
            allDates.Sort(Function(a, b) String.Compare(b, a))

            ' Remove duplicates
            Dim uniqueDates As New List(Of String)
            For Each d In allDates
                If Not uniqueDates.Contains(d) Then uniqueDates.Add(d)
            Next
            allDates = uniqueDates

            ' Calculate halves for UI filter
            Dim mid As Integer = allDates.Count \ 2
            gbDateFilter.Visible = (allDates.Count > 4)

            Dim filteredDates As New List(Of String)
            If gbDateFilter.Visible Then
                If rb2ndHalf.Checked Then
                    For i As Integer = 0 To mid - 1
                        filteredDates.Add(allDates(i))
                    Next
                ElseIf rb1stHalf.Checked Then
                    For i As Integer = mid To allDates.Count - 1
                        filteredDates.Add(allDates(i))
                    Next
                Else
                    filteredDates = allDates
                End If
            Else
                filteredDates = allDates
            End If

            ctx.lDates = filteredDates  ' ← fixed from blank = filteredDates

            ' Update UI
            RemoveHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged
            cbDates.BeginUpdate()
            cbDates.DataSource = Nothing
            cbDates.Items.Clear()

            If filteredDates IsNot Nothing AndAlso filteredDates.Count > 0 Then
                cbDates.Items.AddRange(filteredDates.ToArray())
                cbDates.SelectedIndex = 0
            Else
                cbDates.Items.Add("(No dates found)")
                cbDates.SelectedIndex = 0
            End If

            cbDates.EndUpdate()
            AddHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged

        Catch ex As Exception
            LOGIT("rebuildDates Error: " & ex.Message)
        Finally
            cbDates.EndUpdate()
        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnScoreCard.Click
        Try

            If cbSeasons.SelectedItem = "2017" Then
                MessageBox.Show($"{cbSeasons.SelectedItem} has no scores, it was only used To create handicaps For 2018", $"Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            Dim newform As New ScoreCard
            newform.Show()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnSchBuilder.Click
        ScheduleBuilder.Show()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btnXLSXBuilder.Click
        Dim xls = New CreateXLSX
        xls.CreateFromCSV()

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles btnAddCSV2XLSX.Click
        Dim xls = New CreateXLSX
        xls.AddToWBFromCSV()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles btnCreateTeams.Click
        oHelper.createteams()
    End Sub
    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        Dim selectedDate As String = cbDates.SelectedItem.ToString()

        ' 1. Let the context handle date, year, and postseason flags

        ctx.SetDate(selectedDate)
        LOGIT($"After SetDate: ActiveDate={ctx.ActiveDate} FrontBack={ctx.sFrontBack} Start9={ctx.rLeagueParmrow("Start9")}")
        ' 2. Sync oHelper (if still needed for legacy parts of the app)

        oHelper.bPostSeason = ctx.IsPostSeason
        oHelper.thisCourse = ctx.thiscourse
        oHelper.CalcHoleMarker(selectedDate)
        oHelper.sFrontBack = oHelper.FrontBack(selectedDate)

        ' 3. UI Logic: Visibility of Buttons
        ' Using your ctx variables to determine if this is the "latest" data
        Dim isLastScoreDate As Boolean = (selectedDate >= oHelper.sqlitedaFromSql(connection, "", "Select Date FROM Scores ORDER BY Date DESC LIMIT 1").Rows(0)(0).ToString())
        Dim isLastMatchDate As Boolean = (selectedDate >= oHelper.sqlitedaFromSql(connection, "", "Select Date FROM Matches ORDER BY Date DESC LIMIT 1").Rows(0)(0).ToString())

        btnTeams.Visible = (cbDates.SelectedIndex = 0 AndAlso isLastScoreDate)
        btnCleanupScores.Visible = isLastMatchDate
        ' Set the date and year dynamically

        'ctx.lPar3s = Me.lPar3s
        'ctx.lPar3sFront = Me.lPar3sFront
        'ctx.lpar3sBack = Me.lpar3sBack

        ' 4. Par 3 / Closest to Pin Logic

        oHelper.CalcCarryOvers()
    End Sub

    '20251230-rerank stroke index
    ' 20260212 - Optimized Stroke Index Reranking
    Public Sub RerankCourseSI()
        ' We loop twice: Start at 1 (Front 9) and then at 10 (Back 9)
        'this is because our league is only 9 holes and we could play front or back
        For Each startHole In {1, 10}
            Dim endHole As Integer = startHole + 8
            Dim rawHandicaps As New Dictionary(Of Integer, Integer)

            ' 1. Collect the raw difficulty values
            For h As Integer = startHole To endHole
                rawHandicaps(h) = ScoreCard.SafeInt(ctx.thiscourse($"H{h}"))
            Next

            ' 2. Calculate the new SI (1, 3, 5... for Front OR 2, 4, 6... for Back)
            Dim rankedSI = RerankSI(rawHandicaps)

            ' 3. Update the Course record in one shot
            For Each kvp In rankedSI
                ctx.thiscourse($"H{kvp.Key}") = kvp.Value
            Next
        Next
    End Sub

    Private Function RerankSI(holes As Dictionary(Of Integer, Integer)) As Dictionary(Of Integer, Integer)
        ' holes: key = hole number, value = raw SI
        Dim sorted = holes.OrderBy(Function(h) h.Value).ToList()
        Dim result As New Dictionary(Of Integer, Integer)

        Dim newSI As Integer = 1
        For Each h In sorted
            result(h.Key) = newSI
            newSI += 1
        Next

        Return result
    End Function
    ' Form-level guard (add this at class level, outside any method)
    Private isUpdating As Boolean = False

    Private Sub cbSeasons_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbSeasons.SelectedIndexChanged
        If isUpdating Then Exit Sub
        If cbSeasons.SelectedIndex = -1 Then Exit Sub

        isUpdating = True
        Try
            Dim selectedSeason As String = Nothing
            Try
                selectedSeason = cbSeasons.SelectedItem?.ToString()?.Trim()
                If String.IsNullOrWhiteSpace(selectedSeason) Then Exit Sub
            Catch
                Exit Sub
            End Try

            Dim selectedLeague As String = cbLeagues.SelectedItem?.ToString()?.Trim()
            If String.IsNullOrWhiteSpace(selectedLeague) Then Exit Sub
            ' 1. Set up the SQL
            Dim sql As String = "SELECT * FROM LeagueParms WHERE Season = @Season AND Name = @League LIMIT 1"

            ' 2. Pack the dictionary
            Dim myParams As New Dictionary(Of String, Object) From {
    {"@Season", selectedSeason},
    {"@League", selectedLeague}
}

            ' 3. Call the function (VB.NET knows to use the new version because of 'myParams')
            Dim dt As DataTable = oHelper.sqlitedaFromSql(ctx.Conn, "LeagueParms", sql, myParams)

            If dt.Rows.Count > 0 Then
                ctx.rLeagueParmrow = dt.Rows(0)

                ' ← Add these
                Dim psDate As DateTime = CDate(ctx.rLeagueParmrow("PostSeasonDt"))
                ctx.sPSDate = psDate.ToString("yyyyMMdd")
                ctx.sPSDate2 = psDate.AddDays(7).ToString("yyyyMMdd")
                ctx.SeasonYear = selectedSeason
                ctx.sLeagueName = ctx.rLeagueParmrow("Name").ToString()
                ctx.SafeLeagueName = ctx.rLeagueParmrow("Name").ToString.Replace("'", "''")
                ctx.IsPostSeason = (ctx.ActiveDate >= ctx.sPSDate)
            Else
                MessageBox.Show($"No parameters found for {selectedLeague} in {selectedSeason}",
                            "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            RemoveHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged
            rebuildDates(cbDates)
            AddHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged

            ctx.thiscourse = oHelper.sqliteda(connection, "Courses").Select($"Name = '{ctx.rLeagueParmrow("Course")}'")(0)

            RerankCourseSI()

            cbDates_SelectedIndexChanged(sender, e)

        Catch ex As Exception
            MessageBox.Show("Error in season change: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            isUpdating = False
        End Try
    End Sub
    Private Sub rbDateFilter_CheckedChanged(sender As Object, e As EventArgs) Handles rb1stHalf.CheckedChanged, rb2ndHalf.CheckedChanged, rbAll.CheckedChanged
        Try
            ' 1. Only run if the form is ready and the radio button was just Checked (not Unchecked)
            Dim rb = DirectCast(sender, RadioButton)
            If Not bFormLoaded OrElse Not rb.Checked Then Exit Sub

            ' 2. Log it using your helper
            LOGIT($"{rb.Name} Checked")

            ' 3. Simply call the routine that handles the filtering logic
            ' This prevents the "Handler Loop" and keeps the logic in one place.
            rebuildDates(cbDates)

            ' 4. Optional: If you need to force the selection to the top
            If cbDates.Items.Count > 0 Then
                cbDates.SelectedIndex = 0
            End If
        Catch ex As Exception

        End Try

    End Sub
    Private Sub btnTeams_Click(sender As Object, e As EventArgs) Handles btnTeams.Click
        Teams.ShowDialog()
    End Sub

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'If MessageBox.Show($"Quit and Save?", $"Quitting {System.Diagnostics.Process.GetCurrentProcess().ProcessName}", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
        '    e.Cancel = True
        '    Exit Sub
        'End If
        'oHelper.CreateWorkbookFromSQLite(connection)
    End Sub

    Private Sub btnPlayers_Click(sender As Object, e As EventArgs) Handles btnPlayers.Click
        Players.ShowDialog()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles btnStandings.Click
        Standings.ShowDialog()
    End Sub

    Private Sub btnLeaders_Click(sender As Object, e As EventArgs) Handles btnLeaders.Click
        Leaders.Show()
    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles btnCleanupScores.Click
        Dim selDate As Long = CLng(cbDates.SelectedItem)

        ' Get latest date from Matches
        Dim lastDate As Long = CLng(oHelper.sqlitedaFromSql(connection, "",
        "SELECT Date FROM Matches ORDER BY Date DESC LIMIT 1").Rows(0)(0))

        If selDate < lastDate Then
            MessageBox.Show($"Only the latest score Date {lastDate} can be deleted",
                        "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim msg = $"{selDate} Matches, Scores, Handicaps, ScoreMethod, Payments (Skins/CTP) will be removed." &
              vbCrLf & "Press OK to proceed?"

        If MessageBox.Show(msg, $"Delete Date {selDate}",
                       MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Exit Sub
        End If

        Dim tablesMain = {"Matches", "Scores", "Handicaps", "ScoreMethod", "Subs", "PlayerLast5"}
        Dim tablesPayments = {"Payments"}

        Using conn As New SQLiteConnection(connection)
            conn.Open()

            Using tran = conn.BeginTransaction()
                Try
                    ' Delete from main tables
                    For Each tbl In tablesMain
                        Dim dfld As String = If(tbl = "PlayerLast5", "LastDate", "Date")
                        Using cmd As New SQLiteCommand(
                        $"DELETE FROM {tbl} WHERE {dfld} = @d", conn, tran)
                            cmd.Parameters.AddWithValue("@d", selDate)
                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    ' Delete Skins/CTP from Payments
                    For Each tbl In tablesPayments
                        Using cmd As New SQLiteCommand(
                        $"DELETE FROM {tbl} WHERE Date = @d AND [Desc] IN ('Skin','CTP')",
                        conn, tran)
                            cmd.Parameters.AddWithValue("@d", selDate)
                            cmd.ExecuteNonQuery()
                        End Using
                    Next

                    tran.Commit()
                    MessageBox.Show($"Cleanup for {selDate} completed.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show($"Cleanup failed: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles btnEmailMsg.Click
        EmailMessage.Show()
    End Sub

    Private Sub cbPostSeason_CheckedChanged(sender As Object, e As EventArgs) Handles cbPostSeason.CheckedChanged
        If bFormLoaded = False Then Exit Sub
        If cbPostSeason.Checked Then
            'oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT PostSeasonDt,* FROM LeagueParms WHERE Season = {cbSeasons.SelectedItem}")
            cbDates.Items.Clear()
            cbDates.Items.Add(oHelper.sPSDate)
            cbDates.SelectedIndex = 0
            gbDateFilter.Visible = False
        Else
            rebuildDates(cbDates)
            gbDateFilter.Visible = True
        End If

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles btnTblCleanup.Click
        Dim ft As New FixTables
        ft.Fixit()

    End Sub

    Private Sub cbLogging_CheckedChanged(sender As Object, e As EventArgs) Handles cbLogging.CheckedChanged
        If cbLogging.Checked Then
            oHelper.bloghelper = True
        Else
            oHelper.bloghelper = False
        End If
    End Sub

    Private Sub cbTestEOY_CheckedChanged(sender As Object, e As EventArgs) Handles cbTestEOY.CheckedChanged
        rebuildDates(cbDates)
        Button1_Click(sender, e)
    End Sub

    Private Sub btnRO_Click(sender As Object, e As EventArgs) Handles btnRO.Click
        oHelper.createRainOuts()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles btnCreateLC.Click
        'oHelper.createLC
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        oHelper.createHdcpDetail()
    End Sub

    Private Sub cbTables_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbTables.SelectedIndexChanged

        ' Create the popup form
        Dim f As New Form()
        f.Text = $"Table Review {cbTables.SelectedItem}"
        f.StartPosition = FormStartPosition.CenterParent
        f.Size = New Size(800, 500)
        f.FormBorderStyle = FormBorderStyle.Sizable ' Let's you stretch it if needed
        ' f.MaximizeBox = False
        f.MinimizeBox = False
        ' Create the dynamic DataGridView
        Dim dgv As New DataGridView()
        dgv.Name = "dgvPopup"
        dgv.Dock = DockStyle.Fill
        Dim datefld As String = "Date"
        ctx.ActiveDate = cbDates.SelectedItem
        Dim tableName As String = cbTables.SelectedItem.ToString()
        Dim sql As String = $"SELECT * FROM {tableName}" ' Default

        ' 1. Build the filter in SQL instead of DataView
        If cbToday.Checked Then
            If Not oHelper.sIn("Courses", tableName, True) Then
                ' Check if it's the Teams table (Year filter) or others (Date filter)
                If oHelper.sIn("Teams", tableName, True) Then
                    sql &= $" WHERE Year = '{ctx.ActiveDate.Substring(0, 4)}'"
                Else
                    ' We assume other tables use the datefld
                    sql &= $" WHERE {datefld} = '{ctx.ActiveDate}'"
                End If
            End If
        End If

        ' 2. Now the "sqliteda" only fetches what is necessary
        Dim dt As DataTable = oHelper.sqliteda(connection, sql)
        dgv.DataSource = dt
        ' ... after dgv.DataSource = dt ...

        ' 1. Force the DGV to calculate its layout so we get accurate numbers
        dgv.ColumnHeadersVisible = True
        dgv.RowHeadersVisible = False ' Usually cleaner for popups

        ' 2. Calculate Required Width
        ' Add up all visible column widths + a small buffer for the scrollbar/border
        'Dim totalWidth As Integer = dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 50

        ' Force the grid to actually build the rows in memory
        dgv.BindingContext = New BindingContext()
        dgv.DataSource = dt
        dgv.PerformLayout() ' <--- THIS IS THE KEY

        ' 1. Bind and Force Column Generation
        dgv.DataSource = dt

        ' 1. Make the columns hug the content
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        ' 2. (Optional) Force it to actually calculate those new widths now
        dgv.AutoResizeColumns()
        dgv.ReadOnly = True
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False

        ' 3. Calculate Width using the NEW shrunk column sizes
        Dim totalWidth As Integer = 0
        For Each col As DataGridViewColumn In dgv.Columns
            totalWidth += col.Width
        Next
        ' Use a smaller buffer now that columns are tight
        totalWidth += 40
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize

        ' 2. Calculate Width (Sum of all columns + scrollbar buffer)
        For Each col As DataGridViewColumn In dgv.Columns
            totalWidth += col.Width
        Next
        totalWidth += 60 ' Buffer for borders and vertical scrollbar

        ' 3. Calculate Height (Header + All Rows + Title Bar)
        Dim totalHeight As Integer = dgv.ColumnHeadersHeight

        If dgv.Rows.Count > 0 Then
            ' Add up the height of every single row
            For Each row As DataGridViewRow In dgv.Rows
                totalHeight += row.Height
            Next
            totalHeight += 40 ' Buffer for the bottom border and title bar
        Else
            totalHeight += 100 ' Default for empty
        End If

        ' 4. Final Constraints
        Dim screenWidth As Integer = Screen.PrimaryScreen.WorkingArea.Width * 0.9
        Dim screenHeight As Integer = Screen.PrimaryScreen.WorkingArea.Height * 0.9

        f.Size = New Size(Math.Min(Math.Max(totalWidth, 300), screenWidth),
                  Math.Min(Math.Max(totalHeight, 200), screenHeight))

        f.StartPosition = FormStartPosition.CenterScreen

        ' Add the grid to the form
        f.Controls.Add(dgv)

        ' Show as modal popup
        f.Show()

    End Sub

    Private Sub rbAll_CheckedChanged(sender As Object, e As EventArgs) Handles rbAll.CheckedChanged, rb1stHalf.CheckedChanged, rb2ndHalf.CheckedChanged
        If bFormLoaded Then
            rebuildDates(cbDates)
        End If

    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        ' Specify the path to the Excel file
        Dim oClosedXML = New ClosedXML
        Dim excelFilePath As String = $"{ctx.csvFilePath}"
        Dim et As TimeSpan
        Dim st As DateTime = Now
        GoTo skipit

skipit:
        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(excelFilePath)
        oFiles = oDirectory.GetFiles("Hugh'sLeague_*.xlsx")
        arraySort(oFiles)
        For Each file In oFiles
            Using workbook As New XLWorkbook(file.FullName)
                ' Create DataTables from the worksheets
                Dim dataTables As Dictionary(Of String, DataTable) = oClosedXML.CreateDataTablesFromWorksheets(workbook)
                ' Display the contents of the DataTables
                Dim i = 1
                For Each sheetName As String In dataTables.Keys
                    If Not sheetName.ToLower.StartsWith("nu-") Then
                        If sheetName = "Scores" Then
                            Dim dataTable As DataTable = dataTables(sheetName)
                            Dim dv = New DataView(dataTable)
                            dv.Sort = "Date Desc"
                            Debug.Print($"{file.Name} - last score ({dv(0)("Date")})")
                            Exit For
                        End If
                        'this creates a table in memory
                        'If dataTable.TableName.StartsWith("Schedule") Then
                        '    dataTable.TableName = $"[{dataTable.TableName}]"
                        'End If
                        'LOGIT($"{dataTable.TableName}-{i} of {dataTables.Keys.Count}")
                        'oHelper.SQLiteCreateTableFromDT(connection, dataTable)
                        Dim x = ""
                        i += 1
                        'showtables(connection)
                        'LOGIT(sheetName)
                    End If
                Next
            End Using
        Next

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
                    oHelper.SQLiteCreateTableFromDT(connection, dataTable)
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
                    wkdt = oHelper.setupwksch(wkdt, reader("Name"))
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

    End Sub
    Private isLoading As Boolean = False   ' Form-level variablec

    Private Sub cbLeagues_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbLeagues.SelectedIndexChanged
        If isLoading Then Exit Sub
        isLoading = True
        Try
            If cbLeagues.SelectedIndex = -1 Then Exit Sub

            ctx.sLeagueName = cbLeagues.SelectedItem?.ToString()?.Trim() '.Replace("'", "''")
            If String.IsNullOrEmpty(ctx.sLeagueName) Then Exit Sub

            LoadSeasonsForLeague(cbLeagues.SelectedItem?.ToString()?.Trim())

            ' Safe now — no recursion risk
            If cbSeasons.Items.Count > 0 Then
                cbSeasons.SelectedIndex = 0
            End If
        Finally
            isLoading = False
        End Try
    End Sub

    Private Sub btnLeagueMessenger_Click(sender As Object, e As EventArgs) Handles btnLeagueMessenger.Click
        frmLeagueMessenger.Show()
    End Sub
    Public Sub ReplayAllScores()
        Try
            Dim season As String = "2026"
            Dim dates = ctx.lDates.Where(Function(d) d.StartsWith(season) AndAlso d < ctx.sPSDate).OrderBy(Function(d) d).ToList()

            LOGIT($"ReplayAllScores: {dates.Count} dates to process")

            For Each d As String In dates
                LOGIT($"ReplayAllScores: processing {d}")

                ' Set the main combobox to this date so ScoreCard_Load picks it up
                Dim idx = cbDates.Items.IndexOf(d)
                If idx < 0 Then
                    LOGIT($"ReplayAllScores: date {d} not found in cbDates, skipping")
                    Continue For
                End If

                cbDates.SelectedIndex = idx
                ctx.SetDate(d)

                ' Open ScoreCard, let it fully load, then close it
                Dim sc As New ScoreCard()
                sc.Show()
                Application.DoEvents() ' Let the form load and process
                sc.Close()
                sc.Dispose()

                LOGIT($"ReplayAllScores: completed {d}")
            Next

            MessageBox.Show("Replay complete!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            LOGIT($"ReplayAllScores Error: {ex.Message}")
            MessageBox.Show("Error: " & ex.Message, "Replay Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class

