Imports System.IO
Public Class Scores
    Dim oHelper As New Helper.Controls.Helper
    Dim rs As New Resizer
    Private Sub Scores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        BldScoresDataGridFromFile()
        rs.FindAllControls(Me)
    End Sub
    Sub BldScoresDataGridFromFile()
        Try
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            '10/1/2017 add code to pull in all scores for a given player
            'check frmScoreCard for event
            '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
            '2 - if double click on a playerevent, get scores for a given player

            'scores by player only keeps 9 holes for some leagues
            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvScores.RowFilter = "Player = '" & oHelper.sPlayer & "'"
            dvScores.RowFilter = dvScores.RowFilter
            Me.Text = "Scores for Player-" & oHelper.sPlayer
            oHelper.iHoles = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0).Item("Holes")
            oHelper.iHoleMarker = 1
            dvScores.Sort = "Date"

            'create array from above defined fields we want out of scorecard
            Dim sArray = New List(Of String)
            sArray.AddRange(oHelper.cBaseScoreCard.Replace("Holes", "Date(2)-cPat120,Holes").Split(","))
            sArray.AddRange(oHelper.cSkinsFields.Split(","))
            '20180222-expand #closests to track each individual hle for carry overs
            Dim ictpctr = 1
            For Each fld In sArray
                If fld.Contains("#Closests") Then
                    For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
                        If oHelper.MyCourse(0)("Hole" & i) = 3 Then
                            sArray.Add("CTP " & ictpctr & "-cPat40nt")
                            ictpctr += 1
                        End If
                    Next
                    sArray.Remove(fld)
                    Exit For
                End If
            Next

            sArray.Add("Points-40-true-false-ml")
            sArray.Add("Team_Points-40-true-false-ml")
            sArray.Add("Opponent-170-true-false-ml")
            'player column not needed, its in the heading 
            If oHelper.bScoresbyPlayer Then sArray.Remove("Player(1)-cPat170")

            Dim sColFormat = New List(Of String)
            Dim sScoreCardforDGV = ""
            'strip parenthesis and add gross/net for In/Out
            'fields can have a pattern associated for cell length, centering,
            For Each parm As String In sArray
                'set detault pattern
                Dim sPat = oHelper.cPat40
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

                If sParm = "Holes" Then
                    sScoreCardforDGV = sScoreCardforDGV + oHelper.CreateHolesFromParm(sColFormat)
                    Continue For
                End If
                sScoreCardforDGV = sScoreCardforDGV + sParm + ","
                sColFormat.Add(sPat)
            Next
            'remove trailing comma

            'replace spaces with underscores for csv column matchups
            sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")

            Dim dtScorecard As DataTable = dvScores.ToTable(True, sScoreCardforDGV.Split(",").ToArray)
            'dtScorecard.PrimaryKey = New DataColumn() {dtScorecard.Columns("Date")}
            Dim dv2scores As New DataView(dtScorecard)
            For Each row As DataRowView In dv2scores
                oHelper.CalcHoleMarker(row("Date"))
                If oHelper.iHoleMarker = 10 Then
                    Dim sKeys() As Object = {oHelper.sPlayer, row("Date")}
                    Dim iscore As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
                    row("Out_Gross") = iscore("In_Gross")
                    row("Out_Net") = iscore("In_Net")
                    For i = 1 To 9
                        row("Hole" & i) = iscore("Hole" & i - 1 + oHelper.iHoleMarker)
                    Next
                End If
            Next
            'adjust hole by hole for handicap and back nine scores show on 1-9
            For Each score As DataRowView In dv2Scores
                Dim sMethod = oHelper.convDBNulltoSpaces(score("Method")).Trim
                'skip hole adjustments for "Score" method
                'if outgross is null, this is a back 9 score
                If sMethod = "Score" Then Continue For
                'Dim iadjscoreptr = oHelper.iHoleMarker - 1
                For i = 1 To 9
                    Dim iscore = score("Hole" & i)
                    'if the handicap > stroke index adjust net score to gross
                    If sMethod = "Net" Then
                        Dim isi = oHelper.CalcStrokeIndex("Hole" & i)
                        If oHelper.iHdcp >= isi Then
                            'check stroke index
                            iscore += 1
                            If oHelper.iHdcp - oHelper.iHoles >= isi Then iscore += 1
                        End If
                    End If
                    'replace teh score with adjustment and hole pointer
                    score("Hole" & i) = iscore
                Next

            Next

            dtScorecard.Columns("Out_Gross").ColumnName = "Gross"
            dtScorecard.Columns("Out_Net").ColumnName = "Net"
            dgScores.Columns.Clear()
            dgScores.RowTemplate.Height = 40
            dgScores.DefaultCellStyle.Font = New Font("Tahoma", 15)

            Dim arr As Array = sScoreCardforDGV.Split(",").ToArray
            For Each col As DataColumn In dtScorecard.Columns
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = col.ColumnName
                dgc.ValueType = GetType(System.String)
                dgScores.Columns.Add(dgc)
            Next

            'With dgScores
            '    .DataSource = dtScorecard
            'End With

            Dim sdel = "-"
            For Each col As DataGridViewColumn In dgScores.Columns
                Dim sformat = ""
                Try
                    If sColFormat(col.Index).StartsWith("cPat") Then
                        Select Case sColFormat(col.Index)
                            Case "cPat40nt"
                                sformat = oHelper.cPat40nt
                            Case "cPat60"
                                sformat = oHelper.cPat60
                            Case "cPatMeth"
                                sformat = oHelper.cPatMeth
                            Case "cPat120"
                                sformat = oHelper.cPat120
                            Case "cPat170"
                                sformat = oHelper.cPat170
                            Case Else
                                sformat = oHelper.cPat40
                        End Select
                    Else
                        sformat = sColFormat(col.Index)
                    End If

                Catch ex As Exception
                    Dim x = ""
                End Try
                Dim scolname = col.Name
                Dim sWidth = sformat.Split(sdel)(0)
                Dim sRO = sformat.Split(sdel)(1)
                Dim sTabstop = sformat.Split(sdel)(2)
                Dim sAlign = sformat.Split(sdel)(3)
                col.Width = sWidth
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
                If col.Name.Contains("Hole") Then
                    col.HeaderText = col.HeaderText.Replace("Hole", "")
                End If
            Next

            For Each row As DataRow In dtScorecard.Rows
                dgScores.Rows.Add(row.ItemArray)
            Next

            For Each row As DataGridViewRow In dgScores.Rows
                row.Height = 30
                If row.IsNewRow Then Continue For
                row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                oHelper.CalcHoleMarker(row.Cells("Date").Value)
                oHelper.ChangeColorsForStrokes(row)
                If oHelper.iHoleMarker = 10 Then row.Cells("Date").Style.BackColor = Color.LightBlue
            Next

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        'future expansion, must figure out a way to load the scorecard dgscores datagrid
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'Dim cell As DataGridViewTextBoxCell = sender.currentcell
        'If cell.OwningColumn.Name = "Date" Then
        '    Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
        '    If mbResult = MsgBoxResult.Yes Then
        '        oHelper.bScoresbyPlayer = False
        '        oHelper.dDate = Date.ParseExact(cell.Value, "yyyymmdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        '        'oHelper.BldScoreCardDataGridFromCSVforPlayer()
        '        'oHelper.BldScoreCardDataGridFromCSV(dgScores)
        '        frmScoreCard.Show()
        '    End If
        'End If
    End Sub
    Private Sub dgScores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        oHelper.bScoresbyPlayer = True
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        Dim dgv = sender
        For Each row As DataGridViewRow In dgv.rows
            'row.Height = 30
            oHelper.ChangeColorsForStrokes(row)
        Next
        oHelper.bScoresbyPlayer = False
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgScores.SortCompare
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        Try
            e.SortResult = If(CInt(e.CellValue1) < CInt(e.CellValue2), -1, 1)
            e.Handled = True
        Catch
        End Try
    End Sub
    Private Sub Scores_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        rs.ResizeAllControls(Me)
    End Sub
End Class