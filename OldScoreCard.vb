Imports System.IO
Public Class frmScoreCard
    Dim oHelper As Helper.Controls.Helper
    Dim iHdcp As String
    Dim iPar As String
    Dim iScore As String
    Dim s9Gross As Integer = 0
    Dim s9Net As Integer = 0
    Dim stot As Integer = 0
    Dim dvOldScores As DataView
    Dim ListView1 As ListView
    Dim ListView2 As ListView
    Dim sScoreCard As String = "League,Nine,Method,Player(1),Date(2),Group(3),Holes,Total,Hdcp,PHdcp,Commit,$Earn,$Skins,$Closest,Partner,Scorecard"
    Dim dtSchedule As DataTable
#Region "Non Event Driven Methods"
    Sub SetHdcps()
        Dim dvHdcp = New DataView(oHelper.dtScores)

        dvHdcp.RowFilter = "Date = '" & dtDate.Value.ToString("yyyyMMdd") & "' and Group = " & cbGroup.SelectedItem

        For Each row As DataGridViewRow In dgScores.Rows
            If row.Cells(0).Value Is Nothing Then
                Continue For
            End If
            If dvHdcp.Count > 0 Then

            End If
            iHdcp = dvHdcp.Item(row.Index).Row("PHdcp").ToString
            row.Cells(0).Value = row.Cells(0).Value & " (" & iHdcp & ")"
            For i = 1 To 9
                If IsNumeric(row.Cells(i).Value) Then
                    If ListView2.Items(0).SubItems(i - 1).Text <= iHdcp Then
                        row.Cells(i).Value = row.Cells(i).Value & ChrW(&H25CF)
                    End If
                End If
            Next
            For i = 10 To 18
                If IsNumeric(row.Cells(i + 1).Value) Then
                    If ListView2.Items(0).SubItems(i).Text <= iHdcp Then
                        row.Cells(i + 1).Value = row.Cells(i + 1).Value & ChrW(&H25CF)
                    End If
                End If
            Next
        Next

    End Sub
    Sub UpdateDataGrid()
        If Not File.Exists(oHelper.sFilePath & ("\Scores.csv")) Then
            oHelper.dtScores = fCreateScoreCardDT()
        Else
            oHelper.dtScores = oHelper.CSV2DataTable("\Scores.csv")
        End If

        oHelper.dtScores.PrimaryKey = New DataColumn() {oHelper.dtScores.Columns("Player"), oHelper.dtScores.Columns("Date")}
        Dim dvCourses = New DataView(oHelper.dtCourses)
        Dim dvScores As New DataView(oHelper.dtScores)
        dvScores.RowFilter = "Date = '" & dtDate.Value.ToString("yyyyMMdd") & "'" & " and group = '" & cbGroup.SelectedItem & "'"
        dvScores.Sort = "Player"
        Dim dtScorecard As DataTable = dvScores.ToTable(True, "Method", "Player", "Hdcp", "Hole1", "Hole2", "Hole3", "Hole4", "Hole5", "Hole6", "Hole7", "Hole8", "Hole9")

        dgScores.Columns.Clear()
        dgScores.DataSource = dtScorecard

        dgScores.Columns.Add("Gross", "Gross")
        dgScores.Columns("Gross").ReadOnly = True
        dgScores.Columns("Gross").Width = 40
        dgScores.Columns.Add("Net", "Net")
        dgScores.Columns("Net").ReadOnly = True
        dgScores.Columns("Net").Width = 40

        'adjust column widths
        dgScores.Columns("Hdcp").ReadOnly = True
        dgScores.Columns("Hdcp").DataGridView.TabStop = False
        dgScores.Columns("Hdcp").Width = 40
        dgScores.Columns("Player").Width = 100
        'add additional columns if were playying 18 holes
        If oHelper.iHoles > 9 Then
            dgScores.Columns.Add("In", "In")
            dgScores.Columns("In").ReadOnly = True
            dgScores.Columns.Add("Total", "Total")
            dgScores.Columns("Total").ReadOnly = True
        End If
        'change edit pattern to numeric 1 char
        For i = 1 To 9
            dgScores.Columns("Hole" & i).HeaderText = i
            'oHelper.CreateMaskField(dgScores, "Hole" & i)
        Next
        dgScores.AutoResizeColumns()

        'reset handicap to 0
        iHdcp = 0

        'If dgScores.Rows.Count = 0 Then
        '    Exit Sub
        'End If

        'If dvScoresWithCourse.Count > 0 Then
        '    SetHdcps()
        'End If

        For Each row As DataGridViewRow In dgScores.Rows
            If row.Cells(row.Index).Value Is Nothing Then
                Continue For
            End If
            AddStrokesIndicator(row)
            'SetColors(row, 1)
            'SetColors(row, 10)
        Next
        dgScores.SelectionMode = DataGridViewSelectionMode.CellSelect
        dgScores.AllowUserToDeleteRows = True
        dgScores.AllowUserToResizeColumns = True
        dgScores.MultiSelect = True

        'dgScores.Focus()
        'dgScores.Rows(0).Cells(0).Selected = True
        'dgScores.SelectionMode = DataGridViewSelectionMode.CellSelect
    End Sub
    Function GetInt(o As Object) As Integer
        If IsDBNull(o) Then Return 0 Else Return CInt(o)
    End Function
    Public Function fCreateScoreCardDT() As DataTable
        fCreateScoreCardDT = Nothing
        fCreateScoreCardDT = New DataTable
        Try
            Dim sArray As String() = sScoreCard.Split(",")
            For Each parm As String In sArray
                If parm.Contains("(") Then
                    parm = parm.Substring(0, parm.IndexOf("("))
                End If
                If parm = "Holes" Then
                    For i As Integer = 1 To oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString
                        Dim column As DataColumn
                        column = New DataColumn()
                        column.ColumnName = "Hole" & i
                        fCreateScoreCardDT.Columns.Add(column)
                    Next
                Else
                    Dim column As DataColumn
                    column = New DataColumn()
                    column.ColumnName = parm
                    fCreateScoreCardDT.Columns.Add(column)
                End If
            Next

            Dim PrimarykeyColumn(0) As DataColumn
            Dim ikey = 0
            For Each parm As String In sArray
                If parm.Contains("(") Then
                    ReDim Preserve PrimarykeyColumn(ikey)
                    PrimarykeyColumn(ikey) = fCreateScoreCardDT.Columns(parm.Substring(0, parm.IndexOf("(")))
                    ikey += 1
                End If
            Next
            fCreateScoreCardDT.PrimaryKey = PrimarykeyColumn
            'oHelper.CreateRowfromLine(sLeagueParmValues.Split(vbTab), fCreateScoreCardDT)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function

#End Region
    'Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
    '    Try

    '        Select Case ((m.WParam.ToInt64 And &HFFFF) And &HFFF0)
    '            Case &HF060
    '                Me.dgScores.CausesValidation = False
    '        End Select
    '        MyBase.WndProc(m)
    '    Catch ex As Exception
    '        Dim x = ""

    '    End Try
    'End Sub
    Public Function PopulateTable(ByVal sFile As String) As DataTable
        PopulateTable = Nothing
        Dim sfilename = "\" & sFile & ".csv"
        If File.Exists(oHelper.sFilePath & sfilename) Then
            PopulateTable = oHelper.CSV2DataTable(sfilename)
        Else
            My.Forms.frmScoreCard.Close()
            MsgBox("Error - " & sfilename & "file not found, you need to create a csv" & vbCrLf & "Returning to main Menu")
        End If

    End Function
    Private Sub ScoreCard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        dgScores.Dispose()
        dgScores.AllowUserToAddRows = True
        oHelper = Main.oHelper
        dtDate.Text = oHelper.dDate

        oHelper.dtCourses = PopulateTable("Courses")
        If oHelper.dtCourses Is Nothing Then
            Exit Sub
        End If
        oHelper.dtPlayers = PopulateTable("Players")
        If oHelper.dtPlayers Is Nothing Then
            Exit Sub
        End If

        cbGroup.SelectedIndex = oHelper.sGroupNumber
        txtLeagueName.Text = oHelper.sLeagueName
        If txtLeagueName.Text.Contains("Error") Then
            MsgBox("File not found or in use, close file, re-eneter league name and try again ")
            'Me.Close()
            Exit Sub
        End If

        'txtLeagueName.Select()

        'AddHandler dgScores.EditingControlShowing, AddressOf Me.dgScores_EditingControlShowing
    End Sub
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click

        Try
            If oHelper.UpdateINI(oHelper.sFilePath & "\Leaguemanager.ini") = False Then
                Throw New Exception("Error Updating " & oHelper.sFilePath & "\Leaguemanager.ini")
            End If

        Catch ex As Exception
            MsgBox("Close the file " & (oHelper.sFilePath & "\Leaguemanager.ini" & vbCrLf & "Try again"))
            UpdateDataGrid()
        End Try
        Me.Close()
    End Sub
    Public Sub IndicateStrokes(cell As DataGridViewCell, sSMethod As String)

        'check scores against handicap
        'is this a stroke hole?
        Dim iStrokeHole As String = gbScoring.Controls.Find("H" & cell.OwningColumn.Name.Replace("Hole", ""), True).OfType(Of Label).First().Text
        If iHdcp <> "" Then
            'is this a double stroke hole (handicap > 9)?
            For i = 1 To 2
                'adjust if they get more than shot this hole
                Dim iadjhdcp = CInt(iHdcp - ((i - 1) * 9))
                If CInt(iStrokeHole) <= iadjhdcp Then
                    'if scoring method is gross, subtract 1 from net, else add 1 to gross
                    If sSMethod = "Gross" Then
                        s9Net -= 1
                    Else
                        s9Gross += 1
                        cell.Style.BackColor = Color.Beige
                        'this stopped allowing me to do it for an unknown reason so i changed the foreground color on it
                        'cell.Value = iScore.ToString.Replace("0", String.Empty) & ChrW(&H25CF)
                        'cell.Style.ForeColor = Color.DarkBlue
                        'cell.Style.Font = New Font("Arial", 15, FontStyle.Bold)
                    End If
                End If

                If CInt(iStrokeHole) <= CInt(iHdcp) - 9 Then
                    cell.Style.BackColor = Color.BlanchedAlmond
                    'cell.Value = cell.Value & ChrW(&H25CF)
                    'cell.Style.ForeColor = Color.DarkGreen
                    'cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
                End If
                If (iHdcp - 9) <= 0 Then
                    Exit For
                End If
            Next
        End If

    End Sub
    Public Sub MarkPars(cell As DataGridViewCell)

        If iScore < iPar Then
            cell.Style.Font = New Font("Arial", 15, FontStyle.Bold)
            cell.Style.ForeColor = Color.OrangeRed
            If iScore < iPar - 1 Then
                cell.Style.Font = New Font("Arial", 20, FontStyle.Bold)
            End If
            If iScore < iPar - 2 Then
                cell.Style.Font = New Font("Arial", 25, FontStyle.Bold)
            End If
        ElseIf iScore > iPar Then
            cell.Style.ForeColor = Color.Black
        Else
            cell.Style.ForeColor = Color.Gray
        End If

    End Sub
    Public Sub AddStrokesIndicator(ByVal R As DataGridViewRow)

        Try
            s9Gross = 0
            s9Net = 0
            stot = 0
            iHdcp = R.Cells.Item("Hdcp").Value.ToString
            'figure out hole by hole
            For Each cell As DataGridViewCell In R.Cells
                If Not cell.OwningColumn.Name.StartsWith("Hole") Then
                    Continue For
                    ' if we reached the max holes this league, put the 9 hole total in the cell
                End If
                'check scores to par change color to red for birdies, aqua for bogies
                'total up scores
                Dim sCurrentHole As String = cell.OwningColumn.Name.Replace("Hole", "")
                If cell.Value.ToString = String.Empty Then
                    Continue For
                End If
                'strip out the stroke hole indicator
                iScore = cell.Value.ToString.Replace(ChrW(&H25CF), String.Empty).Replace(" ", String.Empty)
                'find par for this hole
                iPar = gbScoring.Controls.Find("Par" & cell.OwningColumn.Name.Replace("Hole", ""), True).OfType(Of Label).First().Text
                MarkPars(cell)
                s9Gross += iScore
                s9net += iScore
                IndicateStrokes(cell, R.Cells.Item("Method").ToString)
                R.Cells.Item("Gross").Value = s9Gross '& ChrW(&H25CF)
                R.Cells.Item("Net").Value = s9Net '& ChrW(&H25CF)
            Next

            'if were done with holes, calculate totals

            'check for 18 holes (future)
            If oHelper.iHoles > 9 Then
                stot += s9Gross
                R.Cells.Item("Total").Value = stot '& ChrW(&H25CF)
            End If
            'add handicap to player name in grid
            'Dim sOutNetScore = gbHoles.Controls.Find("OutScore", True).OfType(Of Label).First().Text
            'Dim sOutGrossScore = R.Cells.Item("Gross").Value
            'find par for this 9
            'R.Cells.Item("Player").Value = R.Cells.Item("Player").Value.ToString & "(" & Math.Round((sOutGrossScore - sOutNetScore) * 0.8, 0) & ")"

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Private Sub dgScores_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEndEdit
        Try
            Dim sCurrColName = dgScores.CurrentCell.OwningColumn.Name
            If sCurrColName.StartsWith("Hole") Then
                ''// MsgBox("Updating column three `Total`")

                Dim R As DataGridViewRow = dgScores.CurrentRow
                AddStrokesIndicator(R)

                'if were done with holes, calculate totals
                'check for 18 holes (future)
                If oHelper.iHoles > 9 Then
                    stot += s9Gross
                    R.Cells.Item("Total").Value = stot '& ChrW(&H25CF)
                End If
                'add handicap to player name in grid
                If R.Cells.Item("Hdcp").Value.ToString = "" Then
                    'find par for this 9
                    R.Cells.Item("Hdcp").Value = Math.Round((R.Cells.Item("Gross").Value - gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First().Text) * 0.8, 0)
                End If

            ElseIf sCurrColName = "Player" Then
                '1  Get the player name from the scores view with his handicap
                '2  if scores havnt been updated for this player/date, then update the gridview and mark stroke holes
                '2a read read the player file to get his handicap

                'try looking for intials if the length is 2 char
                dgScores.CurrentCell.Value = oHelper.fGetPlayerByInitials(dgScores.CurrentCell.Value)
                If Not Len(dgScores.CurrentCell.Value) > 2 Then
                    MsgBox("You didnt pick a player, Try Again")
                    dgScores.CurrentCell = dgScores(e.ColumnIndex, e.RowIndex)
                    ' dgScores.SelectionMode = DataGridViewSelectionMode.CellSelect
                    'dgScores.Focus = dgScores.CurrentCell
                    'dgScores.SelectedCells = True;
                    Exit Sub
                End If
                Dim dvPlayers As New DataView(oHelper.dtPlayers)
                dvPlayers.RowFilter = "Name = '" & dgScores.CurrentCell.Value & "'"
                iHdcp = dvPlayers.Item(0).Row.Item("Handicap").ToString
                dgScores.CurrentRow.Cells.Item("Hdcp").Value = iHdcp
                'Dim dvScores As New DataView(oHelper.dtScores)
                'dvScores.RowFilter = "Date = '" & dtDate.Value.ToString("yyyyMMdd") & "'" & " and Group = '" & cbGroup.SelectedItem & "'" & " and Player = '" & dgScores.CurrentCell.Value & "'"
                ''if no rows returned, this is a new score
                'Dim dvPlayers As New DataView(oHelper.dtPlayers)
                'dvPlayers.RowFilter = "Name = '" & dgScores.CurrentCell.Value & "'"
                'iHdcp = dvPlayers.Item(0).Row("Handicap").ToString

                'If dvScores.Count = 0 Then
                '    Dim R As DataGridViewRow = dgScores.CurrentRow
                '    For Each cell As DataGridViewCell In R.Cells
                '        If cell.OwningColumn.Name.StartsWith("Hole") Then
                '            R.Cells(cell.ColumnIndex).Value = DBNull.Value
                '            R.Cells(cell.ColumnIndex).Style.BackColor = DefaultBackColor
                '            'Dim lbPar As Label = gbHoles.Controls.Find("Par" & cell.OwningColumn.Name.Replace("Hole", ""), True).OfType(Of Label).First()
                '            'Dim lbSi As Label = gbStrokeIndex.Controls.Find("H" & i, True).OfType(Of Label).First()

                '            If gbScoring.Controls.Find("H" & cell.OwningColumn.Name.Replace("Hole", ""), True).OfType(Of Label).First().Text <= iHdcp Then
                '                'If GetInt(R.Cells(cell.ColumnIndex).Value) = 0 Then
                '                'R.Cells(cell.ColumnIndex).Value = " " & ChrW(&H25CF)
                '                'Else
                '                '    R.Cells(cell.ColumnIndex).Value = R.Cells(cell.ColumnIndex).Value.Replace(ChrW(&H25CF), String.Empty) & ChrW(&H25CF)
                '                'End If
                '            End If
                '        ElseIf cell.OwningColumn.Name = "Hdcp" Then
                '            cell.Value = iHdcp
                '        End If
                '    Next

                'End If

            End If

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Private Sub btnUpdate_Click(sender As System.Object, e As System.EventArgs) Handles btnUpdate.Click
        'this sub should:
        '------------------------------------
        'Update scores.csv 
        'Update player.csv(handicap)
        '------------------------------------
        'oHelper.dtLeagueParms = oHelper.CSV2DataTable("\" & txtLeagueName.Text & ".csv")
        'If IsNothing(oHelper.dtLeagueParms) Then
        '    My.Forms.frmScoreCard.Close()
        '    MsgBox("Error - League table" & txtLeagueName.Text & " not found" & vbCrLf & "Returning to main Menu")
        '    Exit Sub
        'End If
        'Dim dtScoresWithCourse As New DataTable
        ''dtScoresWithCourse = oHelper.BuildScoresTable
        'dtScoresWithCourse = oHelper.CSV2DataTable("\Scores.csv")
        For Each row As DataGridViewRow In dgScores.Rows
            If row.Cells(row.Index).Value Is Nothing Then
                Continue For
            End If

            Dim aRow As DataRow
            ' now create an empty datarow
            aRow = oHelper.dtScores.NewRow
            'loop thru each column in the data table determining if numeric values are present for numeric fields
            aRow("Method") = row.Cells("Method").Value.ToString
            aRow("Player") = row.Cells("Player").Value.ToString
            aRow("Date") = dtDate.Value.ToString("yyyyMMdd")
            aRow("Group") = cbGroup.SelectedItem
            aRow("Hdcp") = row.Cells("Hdcp").Value.ToString
            '  aRow("Course") = row.Cells("Course").Value.ToString
            For hole = 1 To oHelper.iHoles
                If IsNumeric(oHelper.RemoveSpcChar(row.Cells("Hole" & hole).Value)) Then
                    aRow("Hole" & hole) = oHelper.RemoveSpcChar(row.Cells("Hole" & hole).Value)
                Else
                    aRow("Hole" & hole) = 0
                End If
            Next
            If oHelper.iHoles > 9 Then
                aRow("Out") = row.Cells("Out").Value
                For hole = 10 To oHelper.iHoles
                    If IsNumeric(oHelper.RemoveSpcChar(row.Cells("Hole" & hole).Value)) Then
                        aRow("Hole" & hole) = oHelper.RemoveSpcChar(row.Cells("Hole" & hole).Value)
                    Else
                        aRow("Hole" & hole) = 0
                    End If
                Next
                aRow("In") = row.Cells("In").Value
            End If
            aRow("Total") = row.Cells("Gross").Value
            'add the full row to the table

            'Dim sKeys() As Object = {row.Cells("Player").Value, dtDate.Value.ToString("yyyyMMdd"), cbGroup.SelectedItem}
            Dim sKeys() As Object = {row.Cells("Player").Value, dtDate.Value.ToString("yyyyMMdd")}

            Dim dr As DataRow = oHelper.dtScores.Rows.Find(sKeys)

            If Not (dr Is Nothing) Then
                dr.Delete()
            End If

            oHelper.dtScores.Rows.Add(aRow)
        Next

        'oHelper.CalcHdcpFromDataTable(oHelper.dtScores)
        oHelper.DataTable2CSV(oHelper.dtScores, oHelper.sFilePath & "\Scores.csv", ",")

    End Sub
    Private Sub cbGroup_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbGroup.SelectedIndexChanged
        UpdateDataGrid()
    End Sub
    Private Sub dgScores_CellEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEnter
        If dgScores.CurrentCell.ReadOnly Then
            SendKeys.Send("{TAB}")
            End If
    End Sub
    Private Sub dgScores_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgScores.DataError
        MsgBox(e.Exception.Message)

        'Try
        '    dgScores.EndEdit()
        '    MsgBox(e.Context.ToString)
        'Catch ex As Exception
        '    MsgBox(ex.Message)
        'End Try
    End Sub


    Private Sub dgScores_CellValidating(sender As System.Object, e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles dgScores.CellValidating
        'If e.FormattedValue = "" Then
        '    Dim dg As DataGridView = sender
        '    If Not dg.CurrentRow Is Nothing Then
        '        MsgBox(dg.CurrentCell.OwningColumn.Name & " Cannot be empty")
        '        e.Cancel = False
        '        _isClosing = True
        '    Else
        '        e.Cancel = False
        '    End If

        'End If
    End Sub

    Private Sub dgScores_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowLeave
        If dgScores.RowCount > 4 Then
            dgScores.AllowUserToAddRows = False
        End If

    End Sub

    Private Sub txtLeagueName_TextChanged(sender As Object, e As EventArgs) Handles txtLeagueName.TextChanged

        If File.Exists(oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv") Then
            oHelper.dtLeagueParms = oHelper.CSV2DataTable("\" & txtLeagueName.Text & ".csv")
            If IsNothing(oHelper.dtLeagueParms) Then
                txtLeagueName.Text = "Error"
                Exit Sub
            Else
                'For Each row As DataRow In oHelper.dtLeagueParms.Rows
                'just use the first row of the league table
                Dim rLeagueParmrow As DataRow = oHelper.dtLeagueParms.Rows(0)
                'get the schedule
                dtSchedule = oHelper.CSV2DataTable(txtLeagueName.Text & "_sch.csv")
                'get the date of the schedule for this week
                Dim iHoleMarker = 0
                'For Each srow As DataRow In dtSchedule.Rows
                'just use the Column names which have dates of the schedule table
                'advance past the front 9 if not front 9
                If Not rLeagueParmrow.Item("Start9").ToString = "F" Then
                    iHoleMarker = 9
                End If
                'Dim x As String = dtDate.Value.ToString.Substring(0, dtDate.Value.ToString.IndexOf("/")).PadLeft(2, "0")
                'this loop will compare the leage start date and flip the hole marker based on front/back
                Dim sPrevMonth As String = rLeagueParmrow("startDate").ToString.Substring(0, rLeagueParmrow("startDate").ToString.IndexOf("/")).PadLeft(2, "0")
                For Each col As DataColumn In dtSchedule.Columns
                    If col.ColumnName.Substring(0, 2) = dtDate.Value.ToString.Substring(0, dtDate.Value.ToString.IndexOf("/")).PadLeft(2, "0") Then
                        Exit For
                    ElseIf col.ColumnName.Substring(0, 2) <> sPrevMonth Then
                        sPrevMonth = col.ColumnName.Substring(0, 2)
                        If iHoleMarker = 9 Then
                            iHoleMarker = 0
                        Else
                            iHoleMarker = 9
                        End If
                    End If
                Next
                'Next
                oHelper.iHoles = rLeagueParmrow.Item("Holes")
                'gbScoring = oHelper.BuildControls(gbScoring, 250, 20, rLeagueParmrow.Item("Holes"), "Hole")
                Dim ileft = oHelper.CreateLabel(gbScoring.Controls, 30, 100, 150, "Par", "Par", "B", "")
                gbScoring = oHelper.BuildControls(gbScoring, 30, ileft, rLeagueParmrow.Item("Holes"), "Par")
                'gbStrokeIndex = oHelper.BuildControls(gbStrokeIndex, 150, 40, rLeagueParmrow.Item("Holes"), "H")
                ileft = oHelper.CreateLabel(gbScoring.Controls, 250, 100, 150, "Stroke Index", "Stroke Index", "B", "")
                gbScoring = oHelper.BuildControls(gbScoring, 250, 250, rLeagueParmrow.Item("Holes"), "H")

                Dim iscorein = 0
                Dim iscoreout = 0
                Dim iscoretot = 0
                Dim dvCourses = New DataView(oHelper.dtCourses)
                dvCourses.RowFilter = "Name = '" & rLeagueParmrow.Item("Course") & "'"
                For i = 1 To oHelper.iHoles
                    'Dim lbHole As Label = gbScoring.Controls.Find("Hole" & i, True).OfType(Of Label).First()
                    'lbHole.Text = i
                    Dim lbPar As Label = gbScoring.Controls.Find("Par" & i, True).OfType(Of Label).First()
                    lbPar.Text = dvCourses.Item(0).Item("Hole" & i + iHoleMarker)
                    If i > 9 Then
                        iscorein = iscorein + lbPar.Text
                    Else
                        iscoreout = iscoreout + lbPar.Text
                    End If
                    Dim lbSi As Label = gbScoring.Controls.Find("H" & i, True).OfType(Of Label).First()
                    'adjust the handicap if its only 9 holes and odd number stroke index
                    If oHelper.iHoles = 9 And Not dvCourses.Item(0).Item("H" & i + iHoleMarker) Mod 2 Then
                        lbSi.Text = Math.Round((dvCourses.Item(0).Item("H" & i + iHoleMarker) + 1) / 2, 0)
                    Else
                        lbSi.Text = dvCourses.Item(0).Item("H" & i + iHoleMarker)
                    End If
                Next
                Dim lbOutScore As Label = gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First()
                lbOutScore.Text = iscoreout
                If oHelper.iHoles > 9 Then
                    Dim lbInScore As Label = gbScoring.Controls.Find("InScore", True).OfType(Of Label).First()
                    lbInScore.Text = iscorein
                    Dim lbTotScore As Label = gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First()
                    lbTotScore.Text = iscorein + iscoreout
                End If
                'Next

            End If
        Else
            Me.gbScoring.Controls.Clear()
            ' Me.gbStrokeIndex.Controls.Clear()
        End If
    End Sub

    Private Sub frmScoreCard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Dim x = ""
    End Sub
    'Private Sub dgscores_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles dgScores.MouseDown
    '    If e.Button = Windows.Forms.MouseButtons.Right Then
    '    '    Dim hti As DataGridView.HitTestInfo = sender.HitTest(e.X, e.Y)
    '    '    If hti.Type = DataGridViewHitTestType.Cell Then
    '    '        If Not dgScores.Rows(hti.RowIndex).Selected Then
    '    '            ' User right clicked a row that is not selected, so throw away all other selections and select this row
    '    '            dgScores.ClearSelection()
    '    '            dgScores.Rows(hti.RowIndex).Selected = True
    '    '        End If
    '    '    End If
    '    'End If
    'End Sub

    Private Sub dgScores_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowEnter
        'if the row is odd then prefill with partner info
        If e.RowIndex = 0 Or e.RowIndex = 2 Then
            dgScores.Rows(e.RowIndex).Cells("Method").Value = "Net"
        ElseIf e.RowIndex = 1 Or e.RowIndex = 3 Then
            'use the player name to look for his partner in the player file
            Dim dvPlayers As New DataView(oHelper.dtPlayers)
            dvPlayers.RowFilter = "Name = '" & dgScores.Rows(e.RowIndex - 1).Cells("Player").Value & "'"
            Dim iteam = dvPlayers.Item(0).Row.Item("Team").ToString
            dvPlayers = New DataView(oHelper.dtPlayers)
            dvPlayers.RowFilter = "Team = '" & iteam & "' and Grade = 'B'"

            If dvPlayers.Count = 1 Then
                dgScores.Rows(e.RowIndex).Cells("Method").Value = "Net"
                dgScores.Rows(e.RowIndex).Cells("Player").Value = dvPlayers.Item(0).Row.Item("Name").ToString
                dgScores.Rows(e.RowIndex).Cells("Hdcp").Value = dvPlayers.Item(0).Row.Item("Handicap").ToString
                If dvPlayers.Item(0).Row.Item("Handicap") IsNot DBNull.Value Then
                    iHdcp = dvPlayers.Item(0).Row.Item("Handicap").ToString
                End If
            End If
        End If
    End Sub
    Private Sub dgScores_KeyPress(sender As Object, e As KeyPressEventArgs)
        If dgScores.CurrentCell.OwningColumn.Name.StartsWith("Hole") Then
            SendKeys.Send("{TAB}")
        End If
    End Sub


#Region "Old Code"
    Sub SetColors(row As DataGridViewRow, istart As Integer)
        Dim iend = istart + 8
        Dim ilvindex = 1
        Dim icellindex = 0
        If istart = 10 Then
            ilvindex = 0
            icellindex = 1
        End If
        For i = 1 To oHelper.iHoles
            'row.Cells(i + icellindex).Style.BackColor = DefaultBackColor
            'check the stroke indx against the handicap
            If gbScoring.Controls.Find("H" & i, True).OfType(Of Label).First().Text <= row.Cells("Hdcp").Value.ToString Then
                'If row.Cells(i + icellindex).Value.ToString.Replace(ChrW(&H25CF), String.Empty) < ListView1.Items(0).SubItems(i - ilvindex).Text Then
                row.Cells(i).Style.BackColor = Color.OrangeRed
                'ElseIf row.Cells(i + icellindex).Value.ToString.Replace(ChrW(&H25CF), String.Empty) > ListView1.Items(0).SubItems(i - ilvindex).Text Then
            ElseIf gbScoring.Controls.Find("H" & i, True).OfType(Of Label).First().Text > row.Cells("Hdcp").Value.ToString Then
                row.Cells(i + icellindex).Style.BackColor = Color.Aqua
            Else
                row.Cells(i + icellindex).Style.BackColor = DefaultBackColor
            End If
        Next
    End Sub

    Private Sub dgScores_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgScores.CellBeginEdit

    End Sub

    Private Sub dgScores_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellContentClick

    End Sub
    'Private Sub dgScores_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs)

    '    If dgScores.CurrentCell.OwningColumn.Name.StartsWith("Hole") Then
    '        AddHandler dgScores.Columns.Item(dgScores.CurrentCell.OwningColumn.Name)., col_TextChanged(), AddressOf Me.col_TextChanged
    '    End If
    'End Sub
    'Private Sub col_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim row As Integer = Me.dgScores.CurrentCell.RowIndex
    '    Dim col As Integer = Me.dgScores.CurrentCell.ColumnIndex
    '    Select Case col
    '        Case 0
    '            If Len(tb.Text) = 2 Then
    '                Me.dgScores.CurrentCell = Me.dgScores(col + 1, row)
    '            End If
    '        Case 1
    '            If Len(tb.Text) = 3 Then
    '                Me.dgScores.CurrentCell = Me.dgScores(col + 1, row)
    '            End If
    '        Case 2
    '            If Len(tb.Text) = 4 Then
    '                Me.dgScores.CurrentCell = Me.dgScores(0, row + 1)
    '            End If
    '    End Select
    'End Sub

    'Private Sub cbCourse_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
    '    Try
    '        'Dim A As ListView = Me.gbHoles.Controls.Find("Listview1", True).OfType(Of ListView).First()
    '        Dim dvCourses = New DataView(oHelper.dtCourses)

    '        dvCourses.RowFilter = "Name = '" & cbCourse.SelectedItem.ToString & "'"
    '        Dim lvrec As ListViewItem = Nothing
    '        lvrec = New ListViewItem(dvCourses.Item(0).Row.Item(2).ToString)
    '        Dim i9Par As Integer = dvCourses.Item(0).Row.Item(2).ToString
    '        For i = 2 To 18
    '            lvrec.SubItems.Add(dvCourses.Item(0).Row.Item(i + 1).ToString)
    '            i9Par += dvCourses.Item(0).Row.Item(i + 1).ToString
    '            If i = 9 Then
    '                lvrec.SubItems.Add(i9Par)
    '                i9Par = 0
    '            End If
    '        Next
    '        'lvrec.SubItems.Add(i9Par)
    '        'lvrec.SubItems.Add(lvrec.SubItems(9).Text + i9Par)
    '        'ListView1.Items.Clear()
    '        'ListView1.Items.Add(lvrec)

    '        'lvrec = New ListViewItem(dvCourses.Item(0).Row.Item(18 + 2).ToString)
    '        'For i = 2 To 18
    '        '    lvrec.SubItems.Add(dvCourses.Item(0).Row.Item(i + 18 + 1).ToString)
    '        '    If i = 9 Then
    '        '        lvrec.SubItems.Add(" ")
    '        '    End If
    '        'Next
    '        'lvrec.SubItems.Add(" ")
    '        'lvrec.SubItems.Add(" ")
    '        'ListView2.Items.Clear()
    '        'ListView2.Items.Add(lvrec)

    '        'Yardage to be added later
    '        'lvrec = New ListViewItem(dvCourses.Item(0).Row.Item(18 + 2).ToString)
    '        'For i = 2 To 18
    '        '    lvrec.SubItems.Add(dvCourses.Item(0).Row.Item(i + 18 + 1).ToString)
    '        '    If i = 9 Then
    '        '        lvrec.SubItems.Add(" ")
    '        '    End If
    '        'Next
    '        'lvrec.SubItems.Add(" ")
    '        'lvrec.SubItems.Add(" ")
    '        'ListView3.Items.Clear()
    '        'ListView3.Items.Add(lvrec)
    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Sub

    'Sub BuildHoles()
    '    Me.gbScoring.Controls.Clear()
    '    Me.gbHoles.Controls.Clear()
    '    Dim ileft = 20
    '    Dim itop = 30
    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 150, "", "Hole")
    '    'Build the Hole #
    '    For i = 1 To 9
    '        ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "Hole" & i, i)
    '        Dim A As Label = Me.gbHoles.Controls.Find("Hole" & i, True).OfType(Of Label).First()
    '        A.Text = i
    '    Next
    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "HOut", "Out")
    '    For i = 10 To 18
    '        ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "Hole" & i, i)
    '        Dim A As Label = Me.gbHoles.Controls.Find("Hole" & i, True).OfType(Of Label).First()
    '        A.Text = i
    '    Next
    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "HIn", "In")
    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "HTot", "Tot")
    '    ileft = 20
    '    itop = itop + 20
    '    'Build Par values
    '    Dim dvCourses = New DataView(dtCourses)
    '    dvCourses.RowFilter = "Name = '" & cbCourse.SelectedItem.ToString & "'"
    '    Dim i9par = 0
    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 150, "", "Par")
    '    For i = 1 To 9
    '        ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "Par" & i, i)
    '        Dim A As Label = Me.gbHoles.Controls.Find("Par" & i, True).OfType(Of Label).First()
    '        A.Text = dvCourses.Item(0).Row.Item(i + 1).ToString
    '        i9par += A.Text
    '    Next

    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "Out", i9par)
    '    i9par = 0
    '    For i = 10 To 18
    '        ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "Par" & i, i)
    '        Dim A As Label = Me.gbHoles.Controls.Find("Par" & i, True).OfType(Of Label).First()
    '        A.Text = dvCourses.Item(0).Row.Item(i + 1).ToString
    '        i9par += A.Text
    '    Next

    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "In", i9par)
    '    Dim B As Label = Me.gbHoles.Controls.Find("Out", True).OfType(Of Label).First()
    '    i9par += B.Text

    '    ileft = oHelper.CreateLabel(Me.gbHoles.Controls, itop, ileft, 40, "Tot", i9par)

    '    For i = 0 To 3
    '        BuildScoringControls(i)
    '    Next
    '    'For i = 1 To 18

    '    '    Dim B As ComboBox = Me.gbScoring.Controls.Find("Hole" & i, True).OfType(Of ComboBox).First()
    '    '    Dim x = B.Name
    '    '    'AddHandler B.TextChanged, AddressOf TextBox_Changed
    '    '    'Update the textboxes from the listviw in frmchoosescores
    '    '    Dim lv1 As SI.Controls.LvSort = Nothing
    '    '    'now update teh scores
    '    '    'If Not lv1.FocusedItem Is Nothing Then
    '    '    '    If Not lv1.FocusedItem.SubItems(0).Text = frmChooseScores.txtPlayer.Text Then
    '    '    '        Exit Sub
    '    '    '    End If
    '    '    'End If

    '    '    B.TabIndex = i - 1 + 5
    '    '    'Calc(B)
    '    '    If Not lv1.FocusedItem Is Nothing Then
    '    '        If i < 10 Then
    '    '            B.Text = lv1.FocusedItem.SubItems(i + 2).Text.Replace(ChrW(&H25CF), String.Empty)
    '    '        Else
    '    '            B.Text = lv1.FocusedItem.SubItems(i + 3).Text.Replace(ChrW(&H25CF), String.Empty)
    '    '        End If

    '    '    End If
    '    'Next
    'End Sub
    ''Public Sub BuildScoringControls(i As Integer)
    '    'Me.gbScoring.Controls.Clear()
    '    Dim ileft = 20
    '    Dim itop = 30 * (i + 1)
    '    ileft = oHelper.CreateComboBox(Me.gbScoring.Controls, itop, ileft, 150, "Player" & i + 1, String.Empty)
    '    Dim cbPlayers As ComboBox = Me.gbScoring.Controls.Find("Player" & i + 1, True).OfType(Of ComboBox).First()
    '    Dim dvScores As New DataView(dtScores)
    '    dvScores.Sort = "Name"
    '    Dim sprevPlayer = String.Empty
    '    For Each player As DataRowView In dvScores
    '        If player("Name") <> sprevPlayer Then
    '            cbPlayers.Items.Add(player("Name"))
    '            sprevPlayer = player("Name")
    '        End If
    '    Next
    '    For ii = 1 To 9
    '        ileft = oHelper.CreateComboBox(Me.gbScoring.Controls, itop, ileft, 40, "Hole" & i + 1 & ii, i + 1 & ii)
    '        Dim A As ComboBox = Me.gbScoring.Controls.Find("Hole" & i + 1 & ii, True).OfType(Of ComboBox).First()
    '        For iii = 1 To 12
    '            A.Items.Add(iii)
    '        Next
    '    Next
    '    ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 40, "In" & i, String.Empty)
    '    For ii = 10 To 18
    '        ileft = oHelper.CreateComboBox(Me.gbScoring.Controls, itop, ileft, 40, "Hole" & i + 1 & ii, i + 1 & ii)
    '        Dim A As ComboBox = Me.gbScoring.Controls.Find("Hole" & i + 1 & ii, True).OfType(Of ComboBox).First()
    '        For iii = 1 To 12
    '            A.Items.Add(iii)
    '        Next
    '    Next
    '    ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 40, "Out" & i, String.Empty)
    '    ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 40, "Tot" & i, String.Empty)
    'End Sub
    'Private Sub TextBox_Changed(ByVal sender As Object, ByVal e As System.EventArgs)
    '    'Calc(sender)
    'End Sub
    ''Public Sub Calc(stb As MaskedTextBox)
    '    Dim bcorrection As Boolean = False

    '    'if a previous score was entered, subtract the score from totals, decrement appropriate scoring bucket(eagle, birdies, etc)
    '    Dim sHole As Integer = stb.Name.Replace("Hole", String.Empty) - 1
    '    If scores(sHole) <> String.Empty Then
    '        bcorrection = True
    '        If sHole < 9 Then
    '            sOutScore = sOutScore - scores(sHole)
    '            Dim lbScore As Label = Me.gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First()
    '            lbScore.Text = sOutScore
    '        Else
    '            sInScore = sInScore - scores(sHole)
    '            Dim lbScore As Label = Me.gbScoring.Controls.Find("InScore", True).OfType(Of Label).First()
    '            lbScore.Text = sOutScore
    '        End If
    '        sTotScore = sTotScore - scores(sHole)
    '        Dim lbScoreTot As Label = Me.gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First()
    '        lbScoreTot.Text = sTotScore

    '        scores(sHole) = String.Empty
    '        stb.BackColor = Color.White
    '    End If

    '    If stb.Text <> String.Empty Then
    '        scores(sHole) = stb.Text
    '        If sHole < 9 Then
    '            sOutScore = sOutScore + stb.Text
    '            Dim lbScore As Label = Me.gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First()
    '            lbScore.Text = sOutScore
    '        Else
    '            sInScore = sInScore + stb.Text
    '            Dim lbScore As Label = Me.gbScoring.Controls.Find("InScore", True).OfType(Of Label).First()
    '            lbScore.Text = sInScore
    '        End If
    '        sTotScore = sTotScore + stb.Text
    '        Dim lbScoreTot As Label = Me.gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First()
    '        lbScoreTot.Text = sTotScore

    '    End If

    '    If Not bcorrection Then
    '        SendKeys.Send("{TAB}")
    '    End If

    'End Sub
#End Region

End Class