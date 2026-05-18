'Imports System.IO.Packaging
'Imports System.Text

Imports System.Data.SQLite
Imports System.Globalization

Public Class ScheduleBuilder

    Dim oHelper As New Helper
    'put here cause 2 subs use these
    Dim iNumWeeksSplit As Integer = 0
    Dim sHalfwayDate As String = 0
    'future move to league parm
    Dim sPositionRound As String = "N"
    Dim sPlayEachTeam As String = "Y"

    Dim iWeeks As Integer
    Dim iRnds As Integer
    Dim sPS As List(Of String)
    Dim bload As Boolean = True
    Dim dtTeam As DataTable
    Public dgv As DataGridView
    'Dim semailfile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\" & Now.ToString("yyyyMMdd") & "_Standings.csv"
    'Dim dtScheduler = New DataTable
    'Dim dvScheduler As DataView

    Private Sub Scheduler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'CenterGroupBoxText(gbCell)
        'CenterGroupBoxText(gbOptions)
        'CenterGroupBoxText(gbPoints)
        'rs.FindAllControls(Me)
        ctx = LeagueContext.Instance

        oHelper = ctx.oHelper
        LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        lbStatus.Text = ""
        If tbStart.Text < oHelper.dDate.ToString("yyyyMMdd") Then
            cbSplitRounds.Enabled = False
            cbPS.Enabled = False
            cbPET.Enabled = False
            cbPR.Enabled = False
            tbRounds.Enabled = False
        End If

        If ctx.rLeagueParmrow("SplitSeason") = "Y" Then cbSplitRounds.Checked = True
        If ctx.rLeagueParmrow("PostSeasonDt").ToString IsNot DBNull.Value Then cbPS.Checked = True
        If sPlayEachTeam = "Y" Then
            tbRounds.Visible = True
            lbRounds.Visible = True
            cbPET.Checked = True
        End If
        If sPositionRound = "Y" Then
            cbPR.Checked = True
            gbPR.Visible = True
        End If
        'change to use league parm future for all 4 controls
        tbRounds.Text = "2"
        dgSchedule.Visible = False

        'dvScheduler = New DataView(dtScheduler)
        'dtScheduler = BuilddtScheduler()
        'Dim dstdate As DateTime = oHelper.dsLeague.Tables("dtSchedule").Columns(1).ColumnName & "/" & oHelper.dDate.ToString("yyyyMMdd").Substring(0, 4)
        Dim dstdate As DateTime = CDate(ctx.rLeagueParmrow("StartDate"))
        tbStart.Text = dstdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

        'Dim denddate = dstdate.AddDays((oHelper.dsLeague.Tables("dtSchedule").Columns.Count - 1) * 7)
        Dim denddate As DateTime = CDate(ctx.rLeagueParmrow("EndDate"))
        tbEnd.Text = denddate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

        iWeeks = DateDiff("w", dstdate, denddate)
        iRnds = iWeeks / ctx.rLeagueParmrow("Teams")
        oHelper.dt = oHelper.sqliteda(ctx.Conn, "Schedule")
        doTeams()
        doScheduler()
        bload = False
    End Sub
    Sub doTeams()

        With dgTeams
            .RowHeadersVisible = False
            .Visible = True
            .Columns.Clear()
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AutoGenerateColumns = False
            .ReadOnly = True
            .Width = 0
            .Height = 20
            .RowTemplate.Height = 15
            .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
            .DefaultCellStyle.Font = New Font("Tahoma", 9)

            Dim dgc As New DataGridViewTextBoxColumn
            With dgc
                .Name = "Team"
                .MaxInputLength = 2
                .ValueType = GetType(System.String)
                .HeaderText = dgc.Name
                .Width = 40
            End With
            .Columns.Add(dgc)
            .Width += dgc.Width

            dgc = New DataGridViewTextBoxColumn
            dgc.Name = "A Player"
            dgc.MaxInputLength = 30
            dgc.ValueType = GetType(System.String)
            dgc.HeaderText = dgc.Name
            dgc.Width = 150
            .Columns.Add(dgc)
            .Width += dgc.Width

            Dim dgc3 As New DataGridViewTextBoxColumn
            dgc3.Name = "B Player"
            dgc3.MaxInputLength = 30
            dgc3.ValueType = GetType(System.String)
            dgc3.HeaderText = dgc3.Name
            dgc3.Width = 150
            .Columns.Add(dgc3)
            .Width += dgc3.Width

            'create array from above defined fields we want out of scorecard
            Dim dvTeam As New DataView(oHelper.sqliteda(ctx.Conn, "Teams"))
            dvTeam.RowFilter = $"Year = '{CDate(ctx.rLeagueParmrow("Startdate")).Year}' And Grade in ('A','B')"
            'added sort by match(partner)
            dvTeam.Sort = "Team"
            dtTeam = dvTeam.ToTable(True, "Team,Player".Split(",").ToArray)
            dtTeam.Columns.Add("BPlayer")
            dtTeam.Columns("Player").ColumnName = "APlayer"
            Dim iTeam = "", aPlayer = ""
            For Each row As DataRow In dtTeam.Rows
                'combine a and b players into 1 row 
                If iTeam <> "" Then
                    If row("Team") = iTeam Then
                        row("BPlayer") = row("APlayer")
                        row("APlayer") = aPlayer
                        .Rows.Add(row.ItemArray)
                    Else
                        aPlayer = row("APlayer")
                        iTeam = row("Team")
                    End If
                Else
                    aPlayer = row("APlayer")
                    iTeam = row("Team")
                    'row.Delete()
                End If
            Next
            Dim rh = .Height
            .Width += 10
            .Height += (.Rows.Count * dgTeams.Rows(0).Height) * 1.1
            rh = .Height
            Dim x = ""
        End With
    End Sub
    Sub doScheduler()
        Try
            LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

            ' 1. Load Schedule Data
            Dim season As String = ctx.rLeagueParmrow("Season").ToString()
            Dim sql As String = $"SELECT * FROM Schedule WHERE substr(Date,1,4) = '{season}' ORDER BY Date"
            oHelper.dt = oHelper.sqlitedaFromSql(ctx.Conn, "Schedule", sql)
            If oHelper.dt Is Nothing OrElse oHelper.dt.Rows.Count = 0 Then Exit Sub

            ' 2. Build Pivot Table (Dates as Columns)
            Dim newTable As New DataTable()
            For Each row As DataRow In oHelper.dt.Rows
                Dim parsedDate As DateTime = DateTime.ParseExact(row("Date").ToString(), "yyyyMMdd", CultureInfo.InvariantCulture)
                newTable.Columns.Add(parsedDate.ToString("MM/dd"))
            Next

            ' 3. Identify match columns ("1" through "24")
            Dim matchCols As New List(Of DataColumn)
            For Each col As DataColumn In oHelper.dt.Columns
                Dim n As Integer
                If Integer.TryParse(col.ColumnName, n) Then
                    matchCols.Add(col)
                End If
            Next

            LOGIT($"Detected {matchCols.Count} match columns")

            ' 4. Pivot match columns into rows (skip empty rows)
            For Each matchCol As DataColumn In matchCols

                ' Check if this match column has ANY data across all dates
                Dim hasData As Boolean = False
                For Each row As DataRow In oHelper.dt.Rows
                    If row(matchCol).ToString().Trim() <> "" Then
                        hasData = True
                        Exit For
                    End If
                Next

                ' Skip columns that are completely empty
                If Not hasData Then Continue For

                ' Build the row
                Dim dr As DataRow = newTable.NewRow()
                For dateIndex As Integer = 0 To oHelper.dt.Rows.Count - 1
                    dr(dateIndex) = oHelper.dt.Rows(dateIndex)(matchCol).ToString().Trim()
                Next

                newTable.Rows.Add(dr)
            Next

            ' 5. UI Configuration
            Dim teamCount As Integer = CInt(ctx.rLeagueParmrow("Teams"))

            With dgSchedule
                .DataSource = Nothing
                .DataSource = newTable
                .Visible = True

                .AllowUserToAddRows = False
                .RowHeadersVisible = False
                .ScrollBars = ScrollBars.None

                Dim halfwayColIndex As Integer = teamCount - 2
                Dim finalColIndex As Integer = .Columns.Count - 1

                .Width = 2
                For Each col As DataGridViewColumn In .Columns
                    col.Width = 45
                    col.SortMode = DataGridViewColumnSortMode.NotSortable
                    .Width += col.Width

                    col.DefaultCellStyle.BackColor = Color.White

                    If (cbSplitRounds.Checked AndAlso col.Index = halfwayColIndex) OrElse
                   col.Index = finalColIndex Then
                        col.DefaultCellStyle.BackColor = Color.LightBlue
                    End If
                Next

                ' 6. Dynamic Height
                Dim totalHeight As Integer = .ColumnHeadersHeight
                For Each r As DataGridViewRow In .Rows
                    totalHeight += r.Height
                Next
                .Height = totalHeight + 2
            End With

        Catch ex As Exception
            LOGIT("Error in doScheduler: " & ex.Message)
        End Try
    End Sub

    Sub CenterGroupBoxText(Groupbox As GroupBox)
        Dim label As New Label
        label.Text = Groupbox.Text
        Groupbox.Text = ""
        label.Left = Groupbox.Left + (Groupbox.Width - label.Width) / 2
        label.Top = Groupbox.Top + 2 ' // 2 Is an example : adjust the constant
        label.Parent = Groupbox.Parent
        label.BringToFront()
    End Sub
    Private Sub dgScheduler_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSchedule.CellMouseDoubleClick
        Try
            Using conn As New SQLiteConnection(ctx.Conn.ConnectionString)
                conn.Open()

                ' Get most recent score date
                Dim lastScoreDateObj As Object = Nothing
                Using cmd As New SQLiteCommand("SELECT Date FROM Scores ORDER BY Date DESC LIMIT 1", conn)
                    lastScoreDateObj = cmd.ExecuteScalar()
                End Using

                If lastScoreDateObj Is Nothing OrElse IsDBNull(lastScoreDateObj) Then Exit Sub

                Dim lastScoreDate As DateTime = DateTime.ParseExact(lastScoreDateObj.ToString(), "yyyyMMdd",
                System.Globalization.DateTimeFormatInfo.InvariantInfo)

                ' Get clicked column date
                Dim clickedDate As DateTime = CDate(dgSchedule.Columns(e.ColumnIndex).Name)

                If clickedDate < lastScoreDate Then
                    MessageBox.Show($"{dgSchedule.Columns(e.ColumnIndex).Name} cannot be skipped, you can only delete a date that has no scores.",
                    "Skip Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                ' Format date as yyyyMMdd using season year
                Dim reformatted As String = clickedDate.ToString("yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture)
                reformatted = reformatted.Replace(Now.Year.ToString(), ctx.rLeagueParmrow("Season").ToString())

                ' Check for scores beyond this date
                Dim scoresAhead As Integer = 0
                Using cmd As New SQLiteCommand("SELECT COUNT(*) FROM Scores WHERE Date > @Date", conn)
                    cmd.Parameters.AddWithValue("@Date", reformatted)
                    scoresAhead = CInt(cmd.ExecuteScalar())
                End Using

                If scoresAhead > 0 Then
                    MessageBox.Show($"Scores beyond {dgSchedule.Columns(e.ColumnIndex).Name} have already been added.{vbCrLf}Cannot flag as rainout.",
                    "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                If MessageBox.Show($"Flag {dgSchedule.Columns(e.ColumnIndex).Name} as a rainout or skip?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If

                lbStatus.Text = $"Processing rainout for {dgSchedule.Columns(e.ColumnIndex).Name}..."
                oHelper.status_Msg(lbStatus, Me)

                ' Rebuild schedule (rainout mode)
                RebuildSchedule(reformatted)

                ' Collect table names
                Dim allTables As New List(Of String)
                Using cmd As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", conn)
                    Using dr As SQLiteDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            allTables.Add(dr("name").ToString())
                        End While
                    End Using
                End Using

                ' Filter tables that have Date or LastDate column
                Dim ltables As New List(Of String)
                For Each tableName As String In allTables
                    If oHelper.sIn(tableName, "Payments,Courses,LeagueParms,Teams,Players,Schedule", False) Then Continue For
                    Using cmd As New SQLiteCommand($"SELECT COUNT(*) FROM pragma_table_info('{tableName}') WHERE name IN ('Date','LastDate')", conn)
                        Dim hasDateCol As Integer = CInt(cmd.ExecuteScalar())
                        If hasDateCol > 0 Then ltables.Add(tableName)
                    End Using
                Next

                ' Delete records for rainout date from all relevant tables
                ctx.ActiveDate = reformatted
                For Each sTableName As String In ltables
                    Try
                        ' Check if table has League column
                        Dim hasLeague As Integer = 0
                        Using cmd As New SQLiteCommand($"SELECT COUNT(*) FROM pragma_table_info('{sTableName}') WHERE name='League'", conn)
                            hasLeague = CInt(cmd.ExecuteScalar())
                        End Using

                        ' Check which date column exists
                        Dim dateCol As String = ""
                        Using cmd As New SQLiteCommand($"SELECT COUNT(*) FROM pragma_table_info('{sTableName}') WHERE name='Date'", conn)
                            If CInt(cmd.ExecuteScalar()) > 0 Then dateCol = "Date"
                        End Using
                        If String.IsNullOrEmpty(dateCol) Then
                            Using cmd As New SQLiteCommand($"SELECT COUNT(*) FROM pragma_table_info('{sTableName}') WHERE name='LastDate'", conn)
                                If CInt(cmd.ExecuteScalar()) > 0 Then dateCol = "LastDate"
                            End Using
                        End If
                        If String.IsNullOrEmpty(dateCol) Then Continue For

                        Dim deleteSql As String = If(hasLeague > 0,
            $"DELETE FROM [{sTableName}] WHERE League=@League AND [{dateCol}]=@Date",
            $"DELETE FROM [{sTableName}] WHERE [{dateCol}]=@Date")

                        Using cmd As New SQLiteCommand(deleteSql, conn)
                            If hasLeague > 0 Then cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                            cmd.Parameters.AddWithValue("@Date", reformatted)
                            Dim deleted As Integer = cmd.ExecuteNonQuery()
                            LOGIT($"Rainout: deleted {deleted} rows from {sTableName} for {reformatted}")
                        End Using
                    Catch ex As Exception
                        LOGIT($"Rainout: skipping {sTableName} - {ex.Message}")
                    End Try
                Next

                lbStatus.Text = $"Rainout complete for {reformatted}"
                oHelper.status_Msg(lbStatus, Me)

            End Using

        Catch ex As Exception
            LOGIT($"dgScheduler_CellMouseDoubleClick Error: {ex.Message}")
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Function RebuildSchedule(removeDate As String, Optional fullRebuild As Boolean = False) As DataTable
        RebuildSchedule = Nothing
        Try
            If ctx.Conn.State <> ConnectionState.Open Then ctx.Conn.Open()

            If fullRebuild Then
                ' Full rebuild logic for new season goes here
            Else
                ' Get all match column names
                Dim matchCols As New List(Of String)
                Using cmd As New SQLiteCommand("SELECT * FROM Schedule LIMIT 1", ctx.Conn)
                    Using dr As SQLiteDataReader = cmd.ExecuteReader()
                        For i As Integer = 0 To dr.FieldCount - 1
                            Dim colName As String = dr.GetName(i)
                            Dim n As Integer
                            If Integer.TryParse(colName, n) Then matchCols.Add(colName)
                        Next
                    End Using
                End Using

                ' Get all rows from removeDate onwards ordered ascending
                Dim dtRows As New DataTable()
                Using cmd As New SQLiteCommand("
                SELECT * FROM Schedule 
                WHERE Date >= @Date 
                AND SUBSTR(Date,1,4) = @Year
                ORDER BY Date ASC", ctx.Conn)
                    cmd.Parameters.AddWithValue("@Date", removeDate)
                    cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                    Using da As New SQLiteDataAdapter(cmd)
                        da.Fill(dtRows)
                    End Using
                End Using

                If dtRows.Rows.Count < 2 Then Exit Function

                ' Shift match content forward — each date gets previous date's matches
                For i As Integer = 1 To dtRows.Rows.Count - 1
                    Dim thisDate As String = dtRows.Rows(i)("Date").ToString()
                    Dim prevRow As DataRow = dtRows.Rows(i - 1)

                    ' Build UPDATE with match columns from previous row
                    Dim setClauses As New List(Of String)
                    Dim updateParams As New List(Of SQLiteParameter)
                    For Each col As String In matchCols
                        setClauses.Add($"[{col}]=@col{col}")
                        Dim param As New SQLiteParameter($"@col{col}")
                        param.Value = If(IsDBNull(prevRow(col)), DBNull.Value, prevRow(col))
                        updateParams.Add(param)
                    Next

                    Dim updateSql As String = $"UPDATE Schedule SET {String.Join(",", setClauses)} WHERE Date=@Date"
                    Using cmd As New SQLiteCommand(updateSql, ctx.Conn)
                        For Each p In updateParams
                            cmd.Parameters.Add(p)
                        Next
                        cmd.Parameters.AddWithValue("@Date", thisDate)
                        cmd.ExecuteNonQuery()
                    End Using
                    LOGIT($"RebuildSchedule: shifted matches to {thisDate}")
                Next

                ' Delete the rainout row
                Using cmd As New SQLiteCommand("DELETE FROM Schedule WHERE Date=@Date", ctx.Conn)
                    cmd.Parameters.AddWithValue("@Date", removeDate)
                    cmd.ExecuteNonQuery()
                End Using
                LOGIT($"RebuildSchedule: deleted {removeDate}")

                ' Step 3: Update PostSeasonDt and EndDate in LeagueParms + 7 days
                Dim currentPSDate As String = ctx.rLeagueParmrow("PostSeasonDt").ToString()
                Dim currentEndDate As String = ctx.rLeagueParmrow("EndDate").ToString()
                Dim psDate As DateTime = DateTime.Parse(currentPSDate)
                Dim endDate As DateTime = DateTime.Parse(currentEndDate)
                Dim newPSDate As String = psDate.AddDays(7).ToString("M/d/yyyy")
                Dim newEndDateFormatted As String = endDate.AddDays(7).ToString("M/d/yyyy")

                Using cmd As New SQLiteCommand("
                UPDATE leagueparms 
                SET PostSeasonDt=@PSDate, EndDate=@EndDate
                WHERE Name=@League AND Season=@Season", ctx.Conn)
                    cmd.Parameters.AddWithValue("@PSDate", newPSDate)
                    cmd.Parameters.AddWithValue("@EndDate", newEndDateFormatted)
                    cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                    cmd.Parameters.AddWithValue("@Season", ctx.SeasonYear)
                    cmd.ExecuteNonQuery()
                End Using
                LOGIT($"RebuildSchedule: PostSeasonDt={newPSDate} EndDate={newEndDateFormatted}")

                ' Step 4: Refresh context
                ctx.rLeagueParmrow("PostSeasonDt") = newPSDate
                ctx.rLeagueParmrow("EndDate") = newEndDateFormatted
                ctx.sPSDate = psDate.AddDays(7).ToString("yyyyMMdd")

                ' Step 5: Add new date at end only if before new PostSeasonDt
                Dim lastDateObj As Object = Nothing
                Using cmd As New SQLiteCommand("
                SELECT MAX(Date) FROM Schedule 
                WHERE SUBSTR(Date,1,4) = @Year", ctx.Conn)
                    cmd.Parameters.AddWithValue("@Year", ctx.SeasonYear)
                    lastDateObj = cmd.ExecuteScalar()
                End Using

                If lastDateObj IsNot Nothing AndAlso Not IsDBNull(lastDateObj) Then
                    Dim lastDate As DateTime = DateTime.ParseExact(lastDateObj.ToString(), "yyyyMMdd",
                    System.Globalization.DateTimeFormatInfo.InvariantInfo)
                    Dim newEndDateTime As DateTime = lastDate.AddDays(7)
                    Dim newPSDateTime As DateTime = psDate.AddDays(7)

                    If newEndDateTime < newPSDateTime Then
                        ' Get last row's match data for new date
                        Dim lastRow As DataRow = dtRows.Rows(dtRows.Rows.Count - 1)
                        Dim newId As Integer = 1
                        Using cmd As New SQLiteCommand("SELECT IFNULL(MAX(ID),0)+1 FROM Schedule", ctx.Conn)
                            newId = CInt(cmd.ExecuteScalar())
                        End Using

                        Dim colList As String = "ID, Date, League, " & String.Join(", ", matchCols.Select(Function(c) $"[{c}]"))
                        Dim valList As String = "@ID, @Date, @League, " & String.Join(", ", matchCols.Select(Function(c) $"@col{c}"))

                        Using cmd As New SQLiteCommand($"INSERT INTO Schedule ({colList}) VALUES ({valList})", ctx.Conn)
                            cmd.Parameters.AddWithValue("@ID", newId)
                            cmd.Parameters.AddWithValue("@Date", newEndDateTime.ToString("yyyyMMdd"))
                            cmd.Parameters.AddWithValue("@League", ctx.sLeagueName)
                            For Each col As String In matchCols
                                cmd.Parameters.AddWithValue($"@col{col}", If(IsDBNull(lastRow(col)), DBNull.Value, lastRow(col)))
                            Next
                            cmd.ExecuteNonQuery()
                        End Using
                        LOGIT($"RebuildSchedule: added new end date {newEndDateTime.ToString("yyyyMMdd")}")
                    Else
                        LOGIT($"RebuildSchedule: new date is postseason, not added")
                    End If
                End If

            End If

            RebuildSchedule = oHelper.sqliteda(ctx.Conn, "Schedule")
            doScheduler()

        Catch ex As Exception
            LOGIT($"RebuildSchedule Error: {ex.Message}")
        End Try
    End Function
    Private Sub dgSchedule_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        'rs.ResizeAllControls(Me)
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Dim sfile As String = $"{ctx.ReportPath}Schedule_{ctx.ActiveDate}.xlsx"
        If IO.File.Exists(sfile) Then IO.File.Delete(sfile)
        'DataGridViewToSpreadsheet.ExportToSpreadsheet(dgSchedule, sfile)
        Dim oClosedXML As New ClosedXML
        Dim columnsToExclude As List(Of String) = New List(Of String) 'From {"Clear SkinCTP", "Clear Scores", "Method", "Status", "PayEOY", "PayLD"}
        oClosedXML.ExportDataGridViewToExcel(dgSchedule, sfile, columnsToExclude)
        Me.Close()
    End Sub

    Private Sub dgSchedule_ColumnDividerDoubleClick(sender As Object, e As DataGridViewColumnDividerDoubleClickEventArgs) Handles dgSchedule.ColumnDividerDoubleClick
        Dim x = ""
    End Sub

    Private Sub btnEmail_Click(sender As Object, e As EventArgs) Handles btnEmail.Click
        ' 1. Define the filename (e.g., "Golf_Schedule_2026")
        Dim myFileName As String = "Golf_Schedule_" & ctx.rLeagueParmrow("Season").ToString()

        ' 2. Call the module function and pass your DataGridView (dgSchedule)
        ' This returns the full file path where the Excel was saved ctx.reportpath in global var
        Dim savedPath As String = modReporting.CreateExcelFromGrid(dgSchedule, $"{ctx.ReportPath}\{myFileName}")

        ' 3. Check if it worked, then notify or open the file
        If Not String.IsNullOrEmpty(savedPath) Then
            Dim result As DialogResult = MessageBox.Show("Excel file created on Desktop. Would you like to open it now?",
                                                     "Export Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

            If result = DialogResult.Yes Then
                ' This opens the file in Excel (or whatever your Mac mini/PC uses for .xlsx)
                Process.Start(New ProcessStartInfo(savedPath) With {.UseShellExecute = True})
            End If
        End If
    End Sub


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Private Sub dgSchedule_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSchedule.ColumnHeaderMouseClick
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        'lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        'oHelper.status_Msg(lbStatus, Me)

        'lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        'oHelper.status_Msg(lbStatus, Me)
    End Sub
    Private Sub dgSchedule_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgSchedule.SortCompare

        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub dgSchedule_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgSchedule.CellValueChanged
        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            'Dim dgr As DataGridViewRow = sender.currentrow
            'If dgr Is Nothing Then Exit Sub
            'If dgr.IsNewRow Then dgr.ReadOnly = False
            'Dim dgc As DataGridViewCell
            'dgc = sender.currentrow.cells(e.ColumnIndex)
            'If dgc.OwningColumn.Name = "Skins" Then
            '    recalcSkins(dgc)
            'ElseIf dgc.OwningColumn.Name = "Closest" Then
            '    recalcCTP(dgc)
            'End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Private Sub cbSplitRounds_CheckedChanged(sender As Object, e As EventArgs) Handles cbSplitRounds.CheckedChanged
        If bload Then Exit Sub
        doScheduler()

    End Sub

    Private Sub cbPS_CheckedChanged(sender As Object, e As EventArgs) Handles cbPS.CheckedChanged
        If bload Then Exit Sub
        doScheduler()
    End Sub

    Private Sub cbPR_CheckedChanged(sender As Object, e As EventArgs) Handles cbPR.CheckedChanged

        If Not bload Then
            If cbPR.Checked Then
                cbPRRS.Checked = True ' future get from parm
                gbPR.Visible = True
            Else
                cbPRRS.Checked = False ' future get from parm
                gbPR.Visible = False
            End If

            doScheduler()
        End If
    End Sub

    Private Sub cbPRRS_CheckedChanged(sender As Object, e As EventArgs) Handles cbPRRS.CheckedChanged

    End Sub

    Private Sub cbPRAW_CheckedChanged(sender As Object, e As EventArgs) Handles cbPRAW.CheckedChanged

    End Sub

    Private Sub cbPET_CheckedChanged(sender As Object, e As EventArgs) Handles cbPET.CheckedChanged
        If Not bload Then
            If cbPET.Checked Then
                tbRounds.Visible = True
                lbRounds.Visible = True
            Else
                tbRounds.Visible = False
                lbRounds.Visible = False
            End If
        End If

    End Sub

    Private Sub tbRounds_TextChanged(sender As Object, e As EventArgs) Handles tbRounds.TextChanged
        If bload Then Exit Sub
        Dim mbResult As MsgBoxResult
        If Not IsNumeric(tbRounds.Text) Then
            mbResult = MsgBox(String.Format("Rounds must be numeric > 0, you entered {0}...try again", tbRounds.Text), MsgBoxStyle.OkCancel)
            If mbResult = MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        mbResult = MsgBox("Note: Default is this will change the ending Date press OK to continue", MsgBoxStyle.OkCancel)
        If mbResult = MsgBoxResult.Ok Then
            'tbRounds.Text = 2
            'Date.ParseExact(cbDatesPlayers.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
            tbEnd.Text = Date.ParseExact(tbStart.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo) _
                        .AddDays(CInt(ctx.rLeagueParmrow("Teams") - 1) * tbRounds.Text * 7).ToString("yyyyMMdd")
            doScheduler()
        End If

    End Sub

    Private Sub BtnCalcSch_Click(sender As Object, e As EventArgs) Handles btnCalcSch.Click
        doScheduler()
    End Sub

    Private Sub dgSchedule_RowHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSchedule.RowHeaderMouseClick
        Dim x = ""
    End Sub

End Class