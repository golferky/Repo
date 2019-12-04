﻿Imports System.IO
Public Class Matches
    Dim oHelper As New Helper
    Public Const cMatchFields As String = "Team-cPat40nt,Player-cPat170,Out_Gross-v,PHdcp-cPat40nt,Out_Net-cPat40nt,Points-cPat40nt,Team_Points-cPat40nt,Opponent-cPat170nt"
    Dim sMatchFields = New List(Of String)("Points,Team_Points,Opponent".Split(","))
    Dim fromsizeW As Integer, gvSsizeW As Integer, gvSCsizeW As Integer, gbSCsizeW As Integer
    Dim fromsizeH As Integer, gvSsizeH As Integer, gvSCsizeH As Integer, gbSCsizeH As Integer
    Dim rs As New Resizer
    Dim bsave As Boolean = False
    Public sByeOpponent As String
    Private Sub Matches_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        rs.FindAllControls(Me)
        oHelper = Main.oHelper

        'load dates from schedule
        'oHelper.dtSchedule = oHelper.CSV2DataTable(txtLeagueName.Text & "_sch.csv")
        'get the date of the schedule for this week
        'just use the Column names which have dates of the schedule table
        'this loop will compare the league start date and flip the hole marker based on front/back
        If oHelper.bsch Then
            For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                'Dim wkdate As DateTime = DateTime.ParseExact(col.ColumnName, "MM/dd/yy", Globalization.CultureInfo.InvariantCulture)
                Dim wkdate As DateTime = col.ColumnName
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                cbDatesPlayers.Items.Add(reformatted)
            Next
        End If
        cbDatesPlayers.SelectedItem = oHelper.dDate.ToString("yyyyMMdd")
        bsave = False
        lbStatus.Text = ""
        '20180130-check for locked scores
        If oHelper.convDBNulltoSpaces(oHelper.rLeagueParmrow("ScoresLocked")) = "Y" Then
            btnSave.Visible = True
        Else
            btnSave.Visible = False
        End If
        If oHelper.bLockScores Then
            btnMatches_Click(sender, e)
            'Me.Close()
        End If
    End Sub

    Private Sub Matches_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        rs.ResizeAllControls(Me)
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'kicks off formclosing
        Me.Close()

    End Sub

    Private Sub Matches_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not oHelper.bLockScores Then SaveScores()
        'If Not bCloseScreen Then
        '    MsgBox("Press Exit to close this form")
        '    e.Cancel = True
        'Else
        '    e.Cancel = False
        'End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        bsave = True
        SaveScores()
    End Sub

    Private Sub btnMatches_Click(sender As Object, e As EventArgs) Handles btnMatches.Click
        oHelper.LOGIT("--------------------------------------------------------------")
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        With lbStatus
            .Text = "Calculating Matches..."
        End With
        oHelper.status_Msg(lbStatus, Me)

        If cbDatesPlayers.SelectedItem Is Nothing Then
            MsgBox("Please select a date from the dropdown")
            With lbStatus
                .Text = "Enter a date"
            End With
            oHelper.status_Msg(lbStatus, Me)
            Exit Sub
        Else
            oHelper.dDate = Date.ParseExact(cbDatesPlayers.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        End If

        BldMatchesDataGridFromCSV()
        '20180325-remove hardcoded players replacing with league parms
        Dim iPlayers As Integer = oHelper.rLeagueParmrow("Teams") * 2
        If dgScores.RowCount < iPlayers Then
            MsgBox(String.Format("All {0} scores must be entered before you can show or calculate matches for this date {1},exiting", iPlayers, cbDatesPlayers.SelectedItem))
            oHelper.Common_Exit()
            With lbStatus
                .Text = "Error Calculating Matches"
                .BackColor = Color.Red
            End With
            oHelper.status_Msg(lbStatus, Me)
            Me.Close()
            Exit Sub
        End If

        sByeOpponent = ""
        oHelper.bByeFound = False
        '20180325-find byes opponent
        If oHelper.rLeagueParmrow("Byes") IsNot DBNull.Value Then
            For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                Dim wkdate As DateTime = col.ColumnName
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                If cbDatesPlayers.SelectedItem = reformatted Then
                    For Each row In oHelper.dsLeague.Tables("dtSchedule").Rows
                        Dim sMatch = row(col).ToString
                        If sMatch.Split("v")(0) = oHelper.rLeagueParmrow("Byes") Then
                            sByeOpponent = sMatch.Split("v")(1)
                            Exit For
                        End If
                        If sMatch.Split("v")(1) = oHelper.rLeagueParmrow("Byes") Then
                            sByeOpponent = sMatch.Split("v")(0)
                            Exit For
                        End If
                    Next
                    'if byes opponent found, no need to keep searching, exit
                    If sByeOpponent <> "" Then Exit For
                End If
            Next
        End If
        '20180525-resorts by match
        'oHelper.getMatchScores(cbDatesPlayers.SelectedItem)

        If dgScores.RowCount > 0 Then
            For i = 1 To Math.Round(oHelper.rLeagueParmrow("Teams") / 2)
                Dim aPtr = (i - 1) * 4
                '20180325
                If oHelper.bByeFound Then aPtr -= 2
                'A Player Match
                oHelper.getMatchPts(dgScores, aPtr + 0)
                'B Player Match
                oHelper.getMatchPts(dgScores, aPtr + 1)
                Dim ihaNet = 0
                Dim ihbNet = 0
                Dim ioaNet = 0
                Dim iobNet = 0
                '20180325-add variable 
                Dim s9Played As String = "Out_Net"
                If oHelper.iHoleMarker <> 1 Then s9Played = "In_Net"

                ihaNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 0).Cells(s9Played).Value.ToString)
                ihbNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 1).Cells(s9Played).Value.ToString)

                If dgScores.Rows(aPtr + 0).Cells("Opponent").Value = "Bye" Then
                    ioaNet = 999
                    iobNet = 999
                Else
                    ioaNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 2).Cells(s9Played).Value.ToString)
                    iobNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 3).Cells(s9Played).Value.ToString)
                End If

                Dim ihTeam As Integer = ihaNet + ihbNet
                Dim ioTeam As Integer = ioaNet + iobNet
                For irow = 0 To 3
                    dgScores.Rows(aPtr + irow).Cells("Team_Points").Value = ""
                Next

                If ihTeam < ioTeam Then
                    dgScores.Rows(aPtr + 0).Cells("Team_Points").Value = 0.5
                    dgScores.Rows(aPtr + 1).Cells("Team_Points").Value = 0.5
                    dgScores.Rows(aPtr + 0).Cells("Team_Points").Style.BackColor = Color.LightGreen
                    dgScores.Rows(aPtr + 1).Cells("Team_Points").Style.BackColor = Color.LightGreen
                ElseIf ihTeam > ioTeam Then
                    dgScores.Rows(aPtr + 2).Cells("Team_Points").Value = 0.5
                    dgScores.Rows(aPtr + 3).Cells("Team_Points").Value = 0.5
                    dgScores.Rows(aPtr + 2).Cells("Team_Points").Style.BackColor = Color.LightGreen
                    dgScores.Rows(aPtr + 3).Cells("Team_Points").Style.BackColor = Color.LightGreen
                Else
                    For irow = 0 To 3
                        dgScores.Rows(aPtr + irow).Cells("Team_Points").Value = 0.25
                        dgScores.Rows(aPtr + irow).Cells("Team_Points").Style.BackColor = Color.Yellow
                    Next
                End If
            Next
        End If

        bsave = True
        oHelper.rLeagueParmrow("ScoresLocked") = "Y"
        SaveScores()

        oHelper.bNoRowLeave = False
        For Each row In dgScores.Rows
            oHelper.MakeCellsStrings(row)
        Next

        Dim sfn = oHelper.sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) & "_Matches.csv"
        lbStatus.Text = String.Format("Creating spreadsheet({0}) of Matches from this screen...", sfn)
        oHelper.status_Msg(lbStatus, Me)
        oHelper.dgv2csv(dgScores, sfn)
        '20190822 - new html
        Dim sHtml As String = oHelper.Create_Html_From_DGV(dgScores)
        sHtml = oHelper.ConvertDataGridViewToHTMLWithFormatting(dgScores, Me)
        Dim swhtml As New IO.StreamWriter(sfn.Replace(".csv", ".html"), False)
        swhtml.WriteLine(sHtml)
        swhtml.Close()
        lbStatus.Text = "Finished creating Matches spreadsheet from this screen"
        oHelper.status_Msg(lbStatus, Me)

        With lbStatus
            .Text = "Finished Calculating Matches"
        End With
        oHelper.status_Msg(lbStatus, Me)
    End Sub

    Private Sub dgScores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        With lbStatus
            .Text = String.Format("Sorting Columns ...")
            '.BackColor = Color.Red
        End With
        oHelper.status_Msg(lbStatus, Me)
        Dim dgv = sender
        'oHelper.bReorderCols = True
        For Each row As DataGridViewRow In dgv.rows
            row.Height = 30
            oHelper.ChangeColorsForStrokes(row)
        Next
        With lbStatus
            .Text = String.Format("Finished Sorting Columns")
            '.BackColor = Color.LightGreen
        End With
        'oHelper.bReorderCols = False
        oHelper.status_Msg(lbStatus, Me)
    End Sub
    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        Dim cell As DataGridViewTextBoxCell = sender.currentcell
        With lbStatus
            .Text = String.Format("Listing scores for {0} ...", cell.Value)
            .BackColor = Color.Red
        End With
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        If cell.OwningColumn.Name = "Player" Then
            Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
            If mbResult = MsgBoxResult.Yes Then
                oHelper.bScoresbyPlayer = True
                oHelper.sPlayer = cell.Value
                'oHelper.BldScoreCardDataGridFromCSVforPlayer()
                'oHelper.BldScoreCardDataGridFromCSV(dgScores)
                Scores.Show()
                oHelper.bScoresbyPlayer = False
            End If
        End If
        With lbStatus
            .Text = String.Format("Done Listing scores for {0}", cell.Value)
            .BackColor = Color.LightGreen
        End With

        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgScores.SortCompare
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        'Try
        '    e.SortResult = If(CInt(e.CellValue1) < CInt(e.CellValue2), -1, 1)
        '    e.Handled = True
        'Catch ex As Exception

        'End Try
        Try
            oHelper.bNoRowLeave = True
            'If sender.currentrow("Player").value = "*** Total ***" Then Exit Sub
            Dim sc1 = oHelper.RemoveSpcChar(oHelper.convDBNulltoSpaces(e.CellValue1).Trim)
            Dim sc2 = oHelper.RemoveSpcChar(oHelper.convDBNulltoSpaces(e.CellValue2).Trim)
            If IsNumeric(sc1) And IsNumeric(sc2) Then
                oHelper.LOGIT(sc1 & "-" & sc2)
                e.SortResult = If(CInt(sc1) < CInt(sc2), -1, 1)
            Else
                e.SortResult = If(CStr(sc1) < CStr(sc2), -1, 1)
            End If

            e.Handled = True
        Catch ex As Exception
        End Try
    End Sub

    Sub SaveScores()
        Try
            If oHelper.convDBNulltoSpaces(oHelper.rLeagueParmrow("ScoresLocked")) = "Y" Then
                If dgScores.RowCount > 0 Then
                    If Not bsave Then
                        Dim mbr = MsgBox("Do you want to save Match Results before you exit?", MsgBoxStyle.YesNo)
                        If mbr = MsgBoxResult.Yes Then bsave = True
                    End If
                End If
            End If

            If bsave Then
                lbStatus.Text = "Saving scores from this screen..."
                lbStatus.BackColor = Color.Red
                For Each row In dgScores.Rows
                    UpdateScoresFromDataGrid(row)
                Next
                oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
                lbStatus.Text = "Done saving scores from this screen"
                lbStatus.BackColor = Color.LightGreen
                bsave = False
            End If

            oHelper.Common_Exit()

        Catch ex As Exception
            MsgBox("Error updating matches, better check them")
        End Try

    End Sub

    Sub BldMatchesDataGridFromCSV()
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

            dgScores.AllowUserToAddRows = False
            dgScores.AllowUserToDeleteRows = False
            dgScores.EditMode = DataGridViewEditMode.EditProgrammatically
            Dim sdate As String = cbDatesPlayers.Text.ToString

            If sdate = "" Then
                MsgBox("Please enter or select a date")
                Exit Sub
            Else
                cbDatesPlayers.Items.Add(sdate)
                cbDatesPlayers.SelectedItem = sdate
            End If

            'oHelper.CalcHoleMarker(sdate)
            '20181003 - if scores already exist int able, dont use date to determine which 9 were playing, we can swap nines and override schedule
            'oHelper.CalcHoleMarker(sdate)
            Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvscores.RowFilter = String.Format("Date = {0}", oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))

            For Each srow As DataRowView In dvscores
                If srow("Hole1") IsNot DBNull.Value Then
                    If IsNumeric(srow("Hole1")) Then
                        oHelper.iHoleMarker = 1
                        Exit For
                    End If
                ElseIf srow("Hole10") IsNot DBNull.Value Then
                    If IsNumeric(srow("Hole10")) Then
                        oHelper.iHoleMarker = 10
                        Exit For
                    End If
                End If
            Next

            If oHelper.iHoles = 0 Then oHelper.iHoles = oHelper.rLeagueParmrow("Holes")
            Dim sMatchFields = cMatchFields
            If oHelper.iHoleMarker = 10 Then sMatchFields = cMatchFields.Replace("Out_", "In_")
            Dim sArray = New List(Of String)(sMatchFields.Split(","))

            Dim sColFormat = New List(Of String)
            Dim sScoreCardforDGV = ""
            'strip parenthesis and add gross/net for In/Out
            'fields can have a pattern associated for cell length, centering,

            For Each parm As String In sArray
                'set detault pattern
                Dim sPat = Helper.cPat40
                Dim sParm = ""

                If UBound(parm.Split("-")) = 0 Then
                    sParm = parm
                Else
                    sParm = parm.Split("-")(0)
                    sPat = parm.Substring(parm.IndexOf("-") + 1)
                End If

                If parm.Contains("(") Then
                    sParm = parm.Substring(0, parm.IndexOf("("))
                End If

                sScoreCardforDGV = sScoreCardforDGV + sParm + ","
                sColFormat.Add(sPat)
            Next
            'remove trailing comma
            'replace spaces with underscores for csv column matchups
            sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")

            '20180525-resort by match #
            For Each row In dvScores
                Dim x = CStr(row("Partner"))
                Dim xx = row("Partner")
                row("Partner") = CStr(row("Partner").PadLeft(2, "0"))
            Next

            dvScores.Sort = "Partner"
            Dim dtScorecard As DataTable = dvscores.ToTable(False, sScoreCardforDGV.Split(",").ToArray)

            dgScores.Columns.Clear()

            dgScores.RowTemplate.Height = 20
            dgScores.DefaultCellStyle.Font = New Font("Tahoma", 15)

            Dim arr As Array = sScoreCardforDGV.Split(",").ToArray
            For Each col As DataColumn In dtScorecard.Columns
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = col.ColumnName
                dgScores.Columns.Add(dgc)
            Next

            Dim sdel = "-"
            For Each col As DataGridViewColumn In dgScores.Columns
                Dim sformat = ""
                Try
                    Select Case sColFormat(col.Index)
                        Case "cPat40nt"
                            sformat = Helper.cPat40nt
                        Case "cPat60"
                            sformat = Helper.cPat60
                        Case "cPatMeth"
                            sformat = Helper.cPatMeth
                        Case "cPat120"
                            sformat = Helper.cPat120
                        Case "cPat170"
                            sformat = Helper.cPat170
                        Case "cPat170nt"
                            sformat = Helper.cPat170nt
                        Case Else
                            sformat = Helper.cPat40
                    End Select

                Catch ex As Exception
                    Dim x = ""
                End Try

                Dim scolname = col.Name
                Dim sWidth = sformat.Split(sdel)(0)
                Dim sRO = sformat.Split(sdel)(1)
                Dim sTabstop = sformat.Split(sdel)(2)
                Dim sAlign = sformat.Split(sdel)(3)
                col.Width = sWidth

                'from skins which works
                col.DataGridView.ReadOnly = sRO
                col.DataGridView.TabStop = sTabstop

                Select Case sAlign
                    Case "mr"
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                    Case "mc"
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    Case "ml"
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                End Select

                col.HeaderText = col.HeaderText.Replace("_", " ")
            Next

            'this moves the rows into the grid
            For Each row As DataRow In dtScorecard.Rows
                dgScores.Rows.Add(row.ItemArray)
            Next
            'rename previous hdcp to hdcp
            dgScores.Columns("PHdcp").HeaderText = "Hdcp"

            Dim ishade = 4, bbl = False

            For Each row As DataGridViewRow In dgScores.Rows
                'row.Height = 30
                row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

                If ishade = 0 Then
                    If bbl Then
                        bbl = False
                    Else
                        bbl = True
                    End If
                    ishade = 4
                End If

                If bbl Then row.DefaultCellStyle.BackColor = Color.LightBlue

                ishade -= 1

                'this gets the most recent handicap up to this date
                Dim dv2Scores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & sdate & "'"
                dv2Scores.Sort = "Date desc"
                If dv2Scores.Count > 0 Then row.Cells("Phdcp").Value = dv2Scores(0).Item("Hdcp").ToString

                'prevent column sort
                oHelper.MakeCellsStrings(row)
                oHelper.ChangeColorsForStrokes(row)
                '20180325-add variable 
                Dim s9Played As String = "Out_Net"
                If oHelper.iHoleMarker <> 1 Then s9Played = "In_Net"
                If row.Cells(s9Played).Value = "" Then row.Cells("Player").Style.BackColor = Color.Pink
            Next

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub UpdateScoresFromDataGrid(row As DataGridViewRow)
        Try
            If row.Cells("Player").Value Is Nothing Then Exit Sub

            'find the score for this player / date
            Dim sKeys() As Object = {row.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem
            Dim dr As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
            For Each cell As DataGridViewCell In row.Cells
                If oHelper.convDBNulltoSpaces(cell.Value) = "" Then
                    dr(cell.OwningColumn.Name) = DBNull.Value
                Else
                    dr(cell.OwningColumn.Name) = cell.Value
                End If

                'End If
            Next

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
            'MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            'Dim st As New StackTrace(True)
            'st = New StackTrace(ex, True)
            'MessageBox.Show("Line: " & st.GetFrame(0).GetFileLineNumber().ToString, "Error")
        End Try
    End Sub

End Class