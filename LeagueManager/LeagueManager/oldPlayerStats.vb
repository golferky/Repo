Imports System.Data.OleDb
Public Class oldfrmPlayerStats

    Dim dMatchDate As Date = Date.Now
    Dim sMatch = 0
    Dim sInScore As Integer = 0, sOutScore As Integer = 0, sTotScore As Integer = 0
    Dim sFrontBack = "Front"
    Dim TextBoxes As New Dictionary(Of String, TextBox)
    Dim Labels As New Dictionary(Of String, Label)
    Dim sBirdies As Integer = 0, sEagles As Integer = 0, sPars As Integer = 0, sBogies As Integer = 0, sDBogies As Integer = 0
    Dim edt As DataTable = Nothing
    Dim dvScores As DataView
    Dim dtCourse As DataTable = Nothing
    Dim strConnString As String

    Const cLabelLeft = 238
    Const cLabelTop = 70
    Const cLabelWidth = 20
    Const cMaxHoles = 18
    Const cDashes = "___________________________________________________________________________________________________________________"
    Dim ileft = cLabelLeft
    Dim itop = cLabelTop
    Dim imaxHoles = cMaxHoles
    Dim iSkins = 0
    Dim sAvgScore As New List(Of Decimal)
    Dim iAvgOut = 0
    Dim iAvgIn = 0
    Dim iAvgTot = 0
    Dim iPar = 0
    Dim iTotScore = 0
    Dim lv1 As New SI.Controls.LvSort

    Private Sub frmPlayerStats_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'strConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & oHelper.sFilePath & ";Extended Properties=Text;"
        Dim oHelper = Main.oHelper
        'edt = oHelper.GetCsvData(strConnString, "Scores.csv")
        edt = oHelper.CSV2DataTableOLEDB("\Scores.csv")
        For Each row As DataRow In edt.Rows
            If Not cbPlayer.Items.Contains(row("Name")) Then
                cbPlayer.Items.Add(row("Name"))
            End If
            If Not cbCourse.Items.Contains(row("Course")) Then
                cbCourse.Items.Add(row("Course"))
            End If
        Next
        cbPlayer.SelectedIndex = 0
        cbCourse.SelectedIndex = 0
        dtCourse = oHelper.CSV2DataTableOLEDB("\Courses.csv")
        dtCourse.PrimaryKey = New DataColumn() {dtCourse.Columns(0)}

        Me.Controls.Add(lv1)
        lv1.Top = 129
        lv1.Left = 15
        lv1.Width = 1187
        lv1.Height = 300
        lv1.View = View.Details
        'lv1.Scrollable = True
        Try
            'If Not edt Is Nothing Then
            '    With cbCourse
            '        .DataSource = edt
            '        .DisplayMember = "Course"
            '        .ValueMember = "Course"
            '        .SelectedIndex = 0
            '    End With
            'End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Private Sub btnDisplayStats_Click(sender As System.Object, e As System.EventArgs) Handles btnDisplayStats.Click
        'lv1.controls.Clear()
        'For i As Integer = lv1.controls.Count - 1 To 0 Step -1
        '    If lv1.controls(i).Name.Contains("Hole") Or _
        '        lv1.controls(i).Name.Contains("Player") Or _
        '        lv1.controls(i).Name.Contains("Score") Or _
        '        lv1.controls(i).Name.Contains("Out") Or _
        '        lv1.controls(i).Name.Contains("Tot") Or _
        '        lv1.controls(i).Name.Contains("Low") Or _
        '        lv1.controls(i).Name.Contains("BB") Or _
        '        lv1.controls(i).Name.Contains("Dash") Or _
        '        lv1.controls(i).Name.Contains("In") Then
        '        lv1.controls.RemoveAt(i)
        '    End If
        'Next
        lv1.Controls.Clear()
        dvScores = New DataView(edt)
        dvScores.RowFilter = "Name = '" & cbPlayer.SelectedItem & "' and Course = '" & cbCourse.SelectedItem & "'"
        dvScores.Sort = "Date"
        If dvScores.Count = 0 Then
            MsgBox(cbPlayer.SelectedItem & " hasnt played " & cbCourse.SelectedItem & " try again")
            Exit Sub
        End If

        imaxHoles = cMaxHoles

        ileft = cLabelLeft
        itop = 1
        Dim oHelper = Main.oHelper
        For i = 0 To imaxHoles - 1
            ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 10, "Hole" & i + 1, i + 1, "BU")
            If i = 8 Then
                ileft = ileft - 5
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "Out", "Out", "BU")
            ElseIf i = 17 Then
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "In", "In", "BU")
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "lbTotal", "Tot", "BU")
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "lbHdcp", "Hdcp", "BU")
            End If
        Next

        Dim drow As DataRow = dtCourse.Rows.Find(cbCourse.SelectedItem)

        itop += 20
        ileft = cLabelLeft
        oHelper.CreateLabel(lv1.Controls, itop, 50, 13 * 9, "lbHandHoles", "Handicap Holes")
        sAvgScore = New List(Of Decimal)
        For i = 1 To imaxHoles
            ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 10, "lbH" & i, drow("H" & i))
            sAvgScore.Add(0)
            iPar += drow(i + 1)
            If i = 9 Then
                ileft += 40
            End If
        Next

        ileft = cLabelLeft
        itop += 15
        oHelper.CreateLabel(lv1.Controls, itop, ileft - 5, 775, "AvgDash", cDashes)

        itop += 30

        iTotScore = 0
        'iPar = 0
        iAvgIn = 0
        iAvgOut = 0
        iAvgTot = 0
        Dim iStats As String() = {0, 0, 0, 0, 0, 0}
        For rec = 0 To dvScores.Count - 1
            sInScore = 0
            sOutScore = 0
            sTotScore = 0
            If dvScores(rec).Item(5) > 0 Then
                BuildScore(rec, iStats)
                itop += 20
            End If
        Next

        CalcLowRound("OutScore", dvScores.Count - 1, False, Color.Chartreuse)
        If imaxHoles = 18 Then
            CalcLowRound("InScore", dvScores.Count - 1, False, Color.Chartreuse)
            CalcLowRound("TotScore", dvScores.Count - 1, False, Color.Chartreuse)
        End If

        ileft = cLabelLeft
        'itop -= 15
        'Produce average scores at the end
        oHelper.CreateLabel(lv1.Controls, itop, ileft - 5, 775, "AvgDash", cDashes)
        itop += 20

        oHelper.CreateLabel(lv1.Controls, itop, 50, 13 * 9, "lblAvgScore", "Average Score")

        Dim b18Holes = True
        For Each score In sAvgScore
            If score = 0 Then
                b18Holes = False
                Exit For
            End If
        Next
        ileft = ileft - 5
        sInScore = 0
        sOutScore = 0
        sTotScore = 0
        For i = 0 To imaxHoles - 1

            If i < 9 Then
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 10, "AvgScore" & i, FormatNumber(sAvgScore(i) / iAvgOut), 1)
            Else
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 10, "AvgScore" & i, FormatNumber(sAvgScore(i) / iAvgIn), 1)
            End If
            ' x = x & "Score" & i & irow & vbCrLf
            If i < 9 Then
                sOutScore += sAvgScore(i)
            Else
                sInScore = sInScore + sAvgScore(i)
            End If
            If i = 8 Then
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "AvgOutScore" & i, FormatNumber(sOutScore / iAvgOut), 1)
            ElseIf i = 17 Then
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "AvgInScore" & i, FormatNumber(sInScore / iAvgIn), 1)
                If b18Holes Then
                    sTotScore = sOutScore + sInScore
                    ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "AvgTotScore" & i, FormatNumber(sTotScore / iAvgTot), 1)
                End If
            End If
        Next

        For line = 0 To imaxHoles - 1
            CalcLowRound("Score" & line, dvScores.Count - 1, False, Color.OrangeRed)
        Next
        dvScores.Dispose()

    End Sub
    Sub CreateHoleLabelCtl(ByVal lblCtl)
        Dim oHelper = Main.oHelper
        Dim lblCtls() As String
        lblCtls = Split(lblCtl, ",")
        oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 10, lblCtl(0), lblCtl(1), lblCtl(2))
    End Sub
    Sub BuildScore(rec As Integer, iStats As String())
        Dim oHelper = Main.oHelper
        'Dim sOutScore = 0
        'Dim sInScore = 0
        'Dim sTotScore = 0
        'Dim dScoreDate1 As String = dvScores.Item(rec).Row("Date")
        Dim ddate As Date = DateTime.ParseExact(dvScores.Item(rec).Row("Date"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        'Dim dScoreDate = dScoreDate1.ToString("d", Globalization.DateTimeFormatInfo.InvariantInfo)
        oHelper.CreateLabel(lv1.Controls, itop, 50, 100, "Date" & rec, ddate)
        ileft = cLabelLeft
        Dim coloffsettohole1 = 5

        For i = 0 To imaxHoles - 1
            Dim dvCourse = New DataView(dtCourse)
            dvCourse.RowFilter = "Name = '" & dvScores.Item(rec).Row(2) & "'"
            If IsNumeric(dvScores.Item(rec).Row(i + coloffsettohole1)) Then
                ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 10, "Score" & i & rec, dvScores.Item(rec).Row(i + coloffsettohole1))
                Dim x = dvScores.Item(rec).Row(i + coloffsettohole1)
                'If i < 9 Then
                '    sOutScore = sOutScore + dvScores.Item(rec).Row(i + coloffsettohole1)
                'Else
                '    sInScore = sInScore + dvScores.Item(rec).Row(i + coloffsettohole1)
                'End If

                'sTotScore = sTotScore + dvScores.Item(rec).Row(i + coloffsettohole1)
                sAvgScore(i) += dvScores.Item(rec).Row(i + coloffsettohole1)
            Else
                ileft += cLabelWidth + 15
            End If

            If i = 8 Then
                If sOutScore > 0 Then
                    ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "OutScore" & rec, dvScores.Item(rec).Row(i))
                    iAvgOut += 1
                Else
                    ileft += cLabelWidth + 25
                End If
            End If
        Next
        ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "InScore" & rec, dvScores.Item(rec).Row("In"))
        iAvgIn += 1
        ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "TotScore" & rec, dvScores.Item(rec).Row("Total"))
        iAvgTot += 1
        ileft = oHelper.CreateLabel(lv1.Controls, itop, ileft, cLabelWidth + 20, "Hdcp" & rec, dvScores.Item(rec).Row("Hdcp"))

    End Sub
    Function CalcLowRound(score As String)
        Return CalcLowRound(score, dvScores.Count - 1, False, Color.Chartreuse)
    End Function
    Function CalcLowRound(score As String, numscores As Integer, Skins As Boolean, color As Color) As Integer
        CalcLowRound = 0
        Dim ilowrow As New List(Of String)
        Dim ilowscore = 99
        For i = 0 To numscores
            Try
                Dim lbScore As Label = lv1.Controls.Find(score & i, True).OfType(Of Label).First
                If lbScore.Text < ilowscore Then
                    ilowscore = lbScore.Text
                    ilowrow = New List(Of String)
                    ilowrow.Add(i)
                ElseIf lbScore.Text = ilowscore Then
                    ilowrow.Add(i)
                End If
            Catch ex As Exception

            End Try

        Next
        If Skins Then
            If ilowrow.Count > 1 Then
                Exit Function
            End If
            CalcLowRound += 1
        End If
        For Each i As Integer In ilowrow
            Dim lbScore As Label = lv1.Controls.Find(score & i, True).OfType(Of Label).First
            If lbScore.Name.Contains("In") Or _
                lbScore.Name.Contains("Out") Or _
                lbScore.Name.Contains("Tot") Then
                lbScore.Width = 30
            Else
                lbScore.Width = 15
            End If

            lbScore.BackColor = color
            lbScore.Font = New System.Drawing.Font(Label1.Font, FontStyle.Bold)
            'Dim rect As New Rectangle(lbScore.Left - 5, lbScore.Top - 5, lbScore.Width + 10, lbScore.Height + 10)
            'Using g As Graphics = Me.CreateGraphics
            '    g.DrawEllipse(Pens.Black, rect)
            'End Using
        Next
        Return CalcLowRound
    End Function
    Sub CalcHandicap(ByVal iPar As Integer)

        Try
            iTotScore += sTotScore
            Dim oHelper = Main.oHelper
            Dim xx = Math.Round((iTotScore / (iAvgTot) - iPar) * 0.8, 0)
            oHelper.CreateLabel(lv1.Controls, itop, ileft, 30, "Hdcp" & iAvgTot, Math.Round((iTotScore / (iAvgTot) - iPar) * 0.8, 0))

        Catch ex As Exception

        End Try

    End Sub
    'Sub CalcLowRound(score As String, numscores As Integer, unique As Boolean, color As Color)
    '    Dim ilowrow(0) As String
    '    Dim ilowscore = 99
    '    Dim ii = 0
    '    For i = 0 To numscores
    '        Dim lbScore As Label = lv1.Controls.Find(score & i, True).OfType(Of Label).First
    '        If lbScore.Text < ilowscore Then
    '            ReDim ilowrow(0)
    '            ii = 0
    '            ilowscore = lbScore.Text
    '            ilowrow(ii) = i
    '        ElseIf lbScore.Text = ilowscore Then
    '            ii += 1
    '            ReDim Preserve ilowrow(ii)
    '            ilowrow(ii) = i
    '        End If
    '    Next
    '    If unique Then
    '        If UBound(ilowrow) > 0 Then
    '            Exit Sub
    '        End If
    '    End If
    '    For Each i As Integer In ilowrow
    '        Dim lbScore As Label = lv1.Controls.Find(score & i, True).OfType(Of Label).First
    '        lbScore.BackColor = color
    '        lbScore.Font = New System.Drawing.Font(Label1.Font, FontStyle.Bold)
    '        Dim rect As New Rectangle(lbScore.Left - 5, lbScore.Top - 5, lbScore.Width + 10, lbScore.Height + 10)
    '        Using g As Graphics = Me.CreateGraphics
    '            g.DrawEllipse(Pens.Black, rect)
    '        End Using
    '    Next

    'End Sub

    'Private Sub DrawRoundRect(ByVal g As Graphics, ByVal r As Rectangle)
    '    Dim hDC As IntPtr = g.GetHdc
    '    Dim hPen As IntPtr = CreatePen(PS_SOLID, 0, ColorTranslator.ToWin32(Color.Red))
    '    Dim hOldPen As IntPtr = SelectObject(hDC, hPen)
    '    SelectObject(hDC, GetStockObject(NULL_BRUSH))
    '    RoundRect(hDC, r.Left, r.Top, r.Right - 1, r.Bottom - 1, 12, 12)
    '    SelectObject(hDC, hOldPen)
    '    DeleteObject(hPen)
    '    g.ReleaseHdc(hDC)
    'End Sub
    Public Sub DrawEllipseFloat(e As PaintEventArgs, w As Single, h As Single)
        ' Create pen.
        Dim blackPen As New Pen(Color.Black, 3)
        ' Create location and size of ellipse.
        Dim x As Single = 0.0F
        Dim y As Single = 0.0F
        Dim width As Single = w
        Dim height As Single = h
        ' Draw ellipse to screen.
        e.Graphics.DrawEllipse(blackPen, x, y, width, height)
    End Sub
End Class