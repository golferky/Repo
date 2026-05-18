Imports System.Data.SQLite
Imports System.Globalization

Public Class LeagueContext
    Private Shared _instance As LeagueContext
    Public Shared ReadOnly Property Instance As LeagueContext
        Get
            If _instance Is Nothing Then _instance = New LeagueContext()
            Return _instance
        End Get
    End Property

    ' --- Core Objects ---
    Public Property oHelper As Helper
    Public Property Conn As SQLiteConnection
    Public Property rLeagueParmrow As DataRow ' The "Golden Record" from LeagueParms

    ' --- Pathing & Environment ---
    ' Reminder: You can update these when you move to the Mac mini 4TB drive
    Public Property csvFilePath As String = "C:\HughsGolf\Files\" '"C:\Users\Gary\OneDrive\Documents\Hugh's League\"
    Public Property ReportPath As String = "C:\HughsGolf\Reports\" '"C:\Users\Gary\OneDrive\Documents\Hugh's League\"

    'Public Property oHelper.sFilePath = "C:\Logs"
    'csvFilePath = "C:\HughsGolf\Files\"
    'ReportPath = "C:\HughsGolf\Reports\"
    'oHelper.sFilePath = "C:\Logs"

    Public Property sLeagueName As String = ""
    Public Property SafeLeagueName As String = ""
    Public Property sPlayer As String = ""
    ' --- Active State Variables ---
    Public Property ActiveDate As String = ""       ' yyyyMMdd
    Public Property SeasonYear As String = ""       ' yyyy
    Public Property IsPostSeason As Boolean = False
    Public Property sPSDate As String               ' yyyyMMdd
    Public Property sPSDate2 As String              ' yyyyMMdd
    Public Property sFirstHalfEndDate As String
    Public Property sSecondHalfEndDate As String
    Public Property sExcludeDates As String = ""
    Public Property lDates As New List(Of String)
    Public Property sFrontBack As String = ""
    ' --- List Data ---
    Public Property lPar3s As New List(Of String)
    Public Property lPar3sFront As New List(Of String)
    Public Property lpar3sBack As New List(Of String)

    ' --- Screen Info ---
    Public Property iScreenWidth As Integer
    Public Property iScreenHeight As Integer
    Public Property thiscourse As DataRow
    Public Property bStableford As Boolean = False
    Public Property ShowLegacyTabs As Boolean = False
    Public tabControl As TabControl
    ''' <summary>
    ''' Sets the active date and automatically calculates Year, PostSeason status, and Par 3s.
    ''' </summary>
    ''' <param name="selectedDate">Date string in yyyyMMdd format.</param>
    Public Sub SetDate(selectedDate As String)
        If String.IsNullOrEmpty(selectedDate) Then Exit Sub

        Me.ActiveDate = selectedDate
        Me.sFrontBack = LeagueDateService.GetFrontBack(selectedDate, rLeagueParmrow("Start9").ToString())
        ' 1. Set the Season Year (yyyyMMdd -> yyyy)
        Dim tempDate As DateTime
        If DateTime.TryParseExact(selectedDate, "yyyyMMdd",
                                  Nothing,
                                  DateTimeStyles.None,
                                  tempDate) Then
            Me.SeasonYear = tempDate.Year.ToString()
        End If

        ' Uses the sPSDate already set by Main
        If Not String.IsNullOrEmpty(Me.sPSDate) Then
            Me.IsPostSeason = (Me.ActiveDate >= Me.sPSDate)
        End If
        If Me.thiscourse Is Nothing Then
            LoadCourseData()
        Else
            RefreshPar3s()
        End If
    End Sub

    ''' <summary>
    ''' Fetches the current course DataRow from the database based on the League Parameters.
    ''' </summary>
    Public Sub LoadCourseData()
        Try
            If rLeagueParmrow IsNot Nothing AndAlso Conn IsNot Nothing Then
                Dim courseName As String = rLeagueParmrow("Course").ToString()
                Dim sql As String = "SELECT * FROM Courses WHERE Name = @CourseName"

                Using cmd As New SQLiteCommand(sql, Conn)
                    cmd.Parameters.AddWithValue("@CourseName", courseName)
                    Using adapter As New SQLiteDataAdapter(cmd)
                        Dim dt As New DataTable()
                        adapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Me.thiscourse = dt.Rows(0)
                            ' After loading the course, update the Par 3 list
                            RefreshPar3s()
                        End If
                    End Using
                End Using
            End If
        Catch ex As Exception
            ' Log error without notification sounds
            If oHelper IsNot Nothing Then mLOGIT("Error loading course: " & ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' Identifies which holes are Par 3s based on the current course and hole marker.
    ''' </summary>
    Private Sub RefreshPar3s()
        Try

            lPar3s.Clear()
            lpar3sBack.Clear()
            lPar3sFront.Clear()

            ' Ensure we have the course data before looping
            If thiscourse IsNot Nothing Then
                oHelper.CalcHoleMarker(ActiveDate)
                ' Loop through 9 holes (starting at 1 or 10 via iHoleMarker)
                For i As Integer = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
                    If CInt(ctx.thiscourse($"Hole{i }")) = 3 Then
                        lPar3s.Add(i.ToString())
                        If i <= 9 Then
                            lPar3sFront.Add(i.ToString())
                        Else
                            lpar3sBack.Add(i.ToString())
                        End If
                    End If
                Next

                ' Sync back to helper for legacy support
                oHelper.iNumClosests = lPar3s.Count
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public iLogitCounter As Integer = 0
    Public bloghelper As Boolean = True

    Public Sub mLOGIT(ByVal sMess As String, Optional bTrblSht As Boolean = False)
        ' Reference the Singleton Instance
        Dim ctx = LeagueContext.Instance

        Try
            ' 1. Get the stack trace from the helper inside the context
            Dim st = ctx.oHelper.ShowStackTrace()

            If Debugger.IsAttached Then
                Debug.WriteLine(sMess)
                Exit Sub
            End If

            Dim stdisplay = String.Join("|", st)

            ' 2. Check the logging flag
            If Not ctx.bloghelper AndAlso Not bTrblSht Then Exit Sub

            ctx.iLogitCounter += 1

            ' 3. FIX: Move the " | " outside the date format brackets
            Dim ds As String = $"{ctx.iLogitCounter.ToString.PadLeft(5)} {DateTime.Now:yyyy-MM-dd HH:mm:ss} | {stdisplay} {sMess}"

            ' 4. Pathing
            Dim logDir As String = IO.Path.Combine(ctx.csvFilePath, "Logs")
            If Not IO.Directory.Exists(logDir) Then IO.Directory.CreateDirectory(logDir)

            ' 5. Filename
            Dim fileName As String = $"{My.Computer.Name}_{ctx.sLeagueName}_{DateTime.Now:yyyyMMdd_HH}.log"
            Dim fullPath As String = IO.Path.Combine(logDir, fileName)

            Using swLog As New IO.StreamWriter(fullPath, True)
                If sMess IsNot Nothing Then
                    ' Split by any newline character
                    For Each line In sMess.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                        swLog.WriteLine(ds)
                    Next
                End If
            End Using

        Catch ex As Exception
            ' Silent fail for logger
        End Try
    End Sub
End Class
