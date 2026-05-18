Imports ClosedXML.Excel
Imports HughsGolf.Helper
Public Class SchGenerator
    Dim teams As List(Of String)
    Dim dv = New DataView
    Dim iteams As Integer
    Dim schedule As List(Of (String, String, Integer))
    Dim dtSchedule = New DataTable 'ohelper.dsLeague.Tables("wkSchedule")
    Dim startdate As Date
    Sub New()

    End Sub
    Public Sub Main()
        Dim ohelper As New Helper
        startdate = HughsGolf.Main.firstTuesday
        ohelper.dt = ohelper.sqliteda(HughsGolf.ctx.Conn, "Schedule")

        iteams = ohelper.sqlitedaFromSql(HughsGolf.ctx.Conn, "Teams", $"Select count(*) from Teams where grade='A' and year = {HughsGolf.Main.firstTuesday.Year}")(0)(0)
        teams = GenerateTeams(iteams)
        Dim random As New Random()

        ' Shuffle teams to randomize the order
        teams = teams.OrderBy(Function() random.Next()).ToList()
        dtSchedule.Columns.Add("Date")
        For i = 1 To teams.Count / 2
            dtSchedule.Columns.Add($"{i}")
        Next

        For i = 0 To 1
            addtotable()
        Next
        Dim newcol As New DataColumn("ID", GetType(Int64))
        dtSchedule.Columns.Add(newcol)
        newcol.SetOrdinal(0)
        For i = (teams.Count / 2) + 1 To 10
            dtSchedule.columns.add(i)
        Next
        dtSchedule.tablename = "Schedule"
        'ohelper.SQLiteCreateTableFromDT(HughsGolf.ctx.Conn, dtSchedule)
        For i = 0 To dtSchedule.Rows.Count - 1
            Dim sdate = $"{dtSchedule.Rows(i)("Date").ToString:mmdd}".Replace("/", "")
            dtSchedule.Rows(i)("Date") = $"{Now.Year}{sdate}"
        Next
        GoTo skipdebug
        ' Display column properties
        For Each column As DataColumn In dtSchedule.Columns
            Debug.Print("Column Name: " & column.ColumnName)
            Debug.Print("Data Type: " & column.DataType.ToString())
            Debug.Print("Allow DBNull: " & column.AllowDBNull.ToString())
            Debug.Print("Unique: " & column.Unique.ToString())
            Debug.Print("AutoIncrement: " & column.AutoIncrement.ToString())
            Debug.Print("----------")
        Next
        For Each column As DataColumn In ohelper.dt.Columns
            Debug.Print("Column Name: " & column.ColumnName)
            Debug.Print("Data Type: " & column.DataType.ToString())
            Debug.Print("Allow DBNull: " & column.AllowDBNull.ToString())
            Debug.Print("Unique: " & column.Unique.ToString())
            Debug.Print("AutoIncrement: " & column.AutoIncrement.ToString())
            Debug.Print("----------")
        Next

skipdebug:
        dtSchedule.merge(ohelper.dt)
        ohelper.SQLiteCreateTableFromDT(HughsGolf.ctx.Conn, dtSchedule)
        Exit Sub

        Dim newtable = PivotTable(dtSchedule, 0)
        newtable.TableName = "Schedule"
        Dim IDCol As New DataColumn("ID")
        newtable.Columns.Add(IDCol)
        newtable.Columns("ID").SetOrdinal(0)
        For i = 0 To newtable.Rows.Count - 1
            newtable.Rows(i)("ID") = i + 1
        Next

    End Sub
    Sub addtotable()
        schedule = GenerateRoundRobinSchedule(teams)
        Dim newrow As DataRow = Nothing
        Dim sp As Double = 0
        Dim ldates = New List(Of String)

        For i = 0 To iteams - 1
            ldates.Add(startdate.ToString("MM/dd"))
            startdate = startdate.AddDays(7)
        Next
        For Each match As (String, String, Integer) In schedule
            'Console.WriteLine($"Round {match.Item3}: {match.Item1} vs {match.Item2}")
            If newrow Is Nothing Then
                newrow = dtSchedule.NewRow
                newrow("Date") = ldates(match.Item3 - 1).ToString.Substring(0, 5)
            End If
            If newrow("Date") <> ldates(match.Item3 - 1) Then
                dtSchedule.Rows.Add(newrow)
                newrow = dtSchedule.NewRow
                newrow("Date") = ldates(match.Item3 - 1).ToString.Substring(0, 5)
                sp = 0
            End If
            newrow($"{sp + 1}") = $"{match.Item1}v{match.Item2}"
            sp += 1
            Debug.Print($"Date {ldates(match.Item3 - 1)}: {match.Item1} vs {match.Item2}")
        Next
        dtSchedule.Rows.Add(newrow)
        ' Choose a DataRow to shuffle (for demonstration, let's shuffle the first row)
        For Each row As DataRow In dtSchedule.Rows
            ' Shuffle the items in the DataRow
            ShuffleDataRowSkippingFirstItem(row)
        Next
    End Sub
    Sub ShuffleDataRow(row As DataRow)
        Dim random As New Random()

        ' Extract the items from the DataRow
        Dim items As Object() = row.ItemArray

        ' Shuffle the items
        items = items.OrderBy(Function() random.Next()).ToArray()

        ' Assign the shuffled items back to the DataRow
        row.ItemArray = items
    End Sub
    Sub ShuffleDataRowSkippingFirstItem(row As DataRow)
        Dim random As New Random()

        ' Extract the items from the DataRow, skipping the first item
        Dim items As Object() = row.ItemArray
        Dim itemsToShuffle As Object() = items.Skip(1).ToArray()

        ' Shuffle the items
        itemsToShuffle = itemsToShuffle.OrderBy(Function() random.Next()).ToArray()

        ' Assign the first item and the shuffled items back to the DataRow
        items = {items(0)}.Concat(itemsToShuffle).ToArray()
        row.ItemArray = items
    End Sub

    Function GenerateTeams(count As Integer) As List(Of String)
        Dim teams As New List(Of String)()
        For i As Integer = 1 To count
            teams.Add($"{i}")
        Next
        Return teams
    End Function

    Function GenerateRoundRobinSchedule(teams As List(Of String)) As List(Of (String, String, Integer))
        Dim schedule As New List(Of (String, String, Integer))()
        Dim n As Integer = teams.Count

        ' Ensure even number of teams by adding a dummy team if necessary
        If n Mod 2 <> 0 Then
            teams.Add("Bye")
            n += 1
        End If

        For round As Integer = 0 To n - 2
            For i As Integer = 0 To n / 2 - 1
                Dim team1 As String = teams(i)
                Dim team2 As String = teams(n - 1 - i)

                ' Avoid adding a match twice by making sure team1 < team2
                If team1 <> "Bye" AndAlso team2 <> "Bye" Then
                    schedule.Add((team1, team2, round + 1))
                End If
            Next
            ' Rotate teams
            Dim temp As String = teams(n - 1)
            For j As Integer = n - 1 To 1 Step -1
                teams(j) = teams(j - 1)
            Next
            teams(1) = temp
        Next

        Return schedule
    End Function
    'Function GenerateRoundRobinSchedule(teams As List(Of String), sdate As Date) As List(Of (String, String, Date))
    '    Dim schedule As New List(Of (String, String, Date))()
    '    Dim n As Integer = teams.Count

    '    ' Ensure even number of teams by adding a dummy team if necessary
    '    If n Mod 2 <> 0 Then
    '        teams.Add("Bye")
    '        n += 1
    '    End If

    '    'For Each team In teams
    '    '    Debug.Print(team)
    '    'Next
    '    For round As Integer = 0 To n - 2
    '        For i As Integer = 0 To n / 2 - 1
    '            Dim team1 As String = teams(i)
    '            Dim team2 As String = teams(n - 1 - i)

    '            ' Avoid adding a match twice by making sure team1 < team2
    '            If team1 <> "Bye" AndAlso team2 <> "Bye" Then
    '                'schedule.Add((team1, team2, round + 1))
    '                schedule.Add((team1, team2, sdate.ToString("MM/dd/yyyy")))
    '            End If
    '        Next
    '        ' Rotate teams
    '        Dim random As New Random()
    '        ' Shuffle teams to randomize the order
    '        teams = teams.OrderBy(Function() random.Next()).ToList()
    '        ''12-13
    '        'Dim temp As String = teams(n - 1)
    '        'For j As Integer = n - 1 To 1 Step -1
    '        '    teams(j) = teams(j - 1)
    '        'Next
    '        'teams(1) = temp
    '        ''12-13
    '        'For Each team In teams
    '        '    Debug.Print(team)
    '        'Next

    '        sdate = sdate.AddDays(7)
    '    Next

    '    Return schedule
    'End Function
End Class
