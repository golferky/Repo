Imports System.IO
Public Class Scores
    Dim oHelper As New Helper.Controls.Helper
    Dim rs As New Resizer
    Dim sf9Rnds = 0, sb9Rnds = 0, sf9aRnds = 0, sb9aRnds = 0
    Dim f9gAvg As Decimal = 0.0, b9gAvg As Decimal = 0.0, f9nAvg As Decimal = 0.0, b9nAvg As Decimal = 0.0
    Dim earned = 0, skins = 0, ctp = 0
    Dim Points As Decimal = 0.00, tPoints As Decimal = 0.00
    Dim lHoles As New List(Of Decimal)
    Dim ltoPar As New List(Of Decimal)

    Private Sub Scores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        BldScoresDataGridFromFile()
        '    rs.FindAllControls(Me)
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

                If parm.Contains("(") Then sParm = parm.Substring(0, parm.IndexOf("("))

                If sParm = "Holes" Then
                    If sParm = "Holes" Then
                        'sScoreCardforDGV = sScoreCardforDGV + oHelper.CreateHolesFromParm(sColFormat)
                        sScoreCardforDGV = sScoreCardforDGV + "PHdcp,"
                        sColFormat.Add("cPat40nt")
                        For i As Integer = 1 To 18 + oHelper.iHoleMarker - 1
                            sScoreCardforDGV = sScoreCardforDGV + "Hole" & i & ","
                            sColFormat.Add("cPatHole")
                            'if its 18 holes
                            'and were on hole 10
                            If i = 9 Then
                                sScoreCardforDGV = sScoreCardforDGV + "Out Gross,Out Net,"
                                sColFormat.Add("cPatMeth")
                                sColFormat.Add("cPatMeth")
                            ElseIf i = 18 Then
                                sScoreCardforDGV = sScoreCardforDGV + "In Gross,In Net,"
                                sColFormat.Add("cPatMeth")
                                sColFormat.Add("cPatMeth")
                            End If
                        Next
                        Continue For
                    End If
                    Continue For
                End If
                sScoreCardforDGV = sScoreCardforDGV + sParm + ","
                sColFormat.Add(sPat)
            Next
            'remove trailing comma

            'replace spaces with underscores for csv column matchups
            sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")

            Dim dtScorecard As DataTable = dvScores.ToTable(True, sScoreCardforDGV.Split(",").ToArray)

            Dim newrow As DataRow
            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Avg ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** ToPar ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Rank ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Eagles ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Birdies ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Pars ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Bogies ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Dbls ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            newrow = dtScorecard.NewRow
            newrow("Method") = "*** Others ***"
            newrow("Date") = ""
            dtScorecard.Rows.Add(newrow)

            'adjust hole by hole for handicap and back nine scores show on 1-9

            For Each score As DataRow In dtScorecard.Rows
                Dim sMethod = oHelper.convDBNulltoSpaces(score("Method")).Trim
                'skip hole adjustments for "Score" method
                'if outgross is null, this is a back 9 score
                If sMethod = "Score" Then Continue For
                'Dim iadjscoreptr = oHelper.iHoleMarker - 1
                For i = 1 To 18
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

            'dtScorecard.Columns("Out_Gross").ColumnName = "Gross"
            'dtScorecard.Columns("Out_Net").ColumnName = "Net"
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
                If col.Name.Contains("Hole") Then col.HeaderText = col.HeaderText.Replace("Hole", "")
            Next

            For Each row As DataRow In dtScorecard.Rows
                dgScores.Rows.Add(row.ItemArray)
            Next

            For i = 0 To 18 Step 1
                lHoles.Add(0.00)
            Next
            'topar eagles, birdies, pars, bogies, doubles, others
            For i = 0 To 5 Step 1
                ltoPar.Add(0)
            Next

            sf9Rnds = 0
            sb9Rnds = 0
            sf9aRnds = 0
            sb9aRnds = 0
            f9gAvg = 0.0
            b9gAvg = 0.0
            f9nAvg = 0.0
            b9nAvg = 0.0
            For Each row As DataGridViewRow In dgScores.Rows
                row.Height = 30
                If row.Cells("Method").Value.ToString.Contains("*** Avg ***") Then
                    updateAvg(row)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** ToPar ***") Then
                    updateToPar(row)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Rank ***") Then
                    updateRank(row)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Eagles ***") Then
                    row.Cells("Method").Value = row.Cells("Method").Value.replace("*", "").replace(" ", "")
                    row.Cells("Date").Value = ltoPar(0)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Birdies ***") Then
                    row.Cells("Method").Value = row.Cells("Method").Value.replace("*", "").replace(" ", "")
                    row.Cells("Date").Value = ltoPar(1)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Pars ***") Then
                    row.Cells("Method").Value = row.Cells("Method").Value.replace("*", "").replace(" ", "")
                    row.Cells("Date").Value = ltoPar(2)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Bogies ***") Then
                    row.Cells("Method").Value = row.Cells("Method").Value.replace("*", "").replace(" ", "")
                    row.Cells("Date").Value = ltoPar(3)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Dbls ***") Then
                    row.Cells("Method").Value = row.Cells("Method").Value.replace("*", "").replace(" ", "")
                    row.Cells("Date").Value = ltoPar(4)
                    Continue For
                ElseIf row.Cells("Method").Value.ToString.Contains("*** Others ***") Then
                    row.Cells("Method").Value = row.Cells("Method").Value.replace("*", "").replace(" ", "")
                    row.Cells("Date").Value = ltoPar(5)
                    Exit For
                ElseIf row.Cells("Method").Value.ToString = "" Then
                    Continue For
                End If
                row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                oHelper.CalcHoleMarker(row.Cells("Date").Value)
                oHelper.SBPChangeColorsForStrokes(row)
                If oHelper.iHoleMarker = 10 Then
                    sb9aRnds += 1
                    row.Cells("Date").Style.BackColor = Color.LightBlue
                    If row.Cells("Method").Value <> "Score" Then
                        sb9Rnds += 1
                    Else
                        row.Cells("In_Net").Value = row.Cells("In_Gross").Value - row.Cells("Phdcp").Value
                    End If
                Else
                    sf9aRnds += 1
                    If row.Cells("Method").Value <> "Score" Then
                        sf9Rnds += 1
                    Else
                        row.Cells("Out_Net").Value = row.Cells("Out_Gross").Value - row.Cells("Phdcp").Value
                    End If
                End If
                For Each cell As DataGridViewCell In row.Cells
                    If cell.OwningColumn.Name.StartsWith("Hole") Then
                        Dim iHole As String = cell.Value.ToString.Replace(ChrW(&H25CF), "")
                        Dim iholeindex As Integer = cell.OwningColumn.Name.Replace("Hole", "")
                        If IsNumeric(iHole) Then
                            lHoles(iholeindex) = lHoles(iholeindex) + iHole
                            calctoPar(iHole, cell.OwningColumn.Name)
                        End If
                    ElseIf cell.Value Is DBNull.Value Or Not IsNumeric(cell.Value) Then
                        Continue For
                    ElseIf cell.OwningColumn.Name = "Out_Gross" Then
                        f9gAvg += cell.Value
                    ElseIf cell.OwningColumn.Name = "Out_Net" Then
                        f9nAvg += cell.Value
                    ElseIf cell.OwningColumn.Name = "In_Gross" Then
                        b9gAvg += cell.Value
                    ElseIf cell.OwningColumn.Name = "In_Net" Then
                        b9nAvg += cell.Value
                    ElseIf cell.OwningColumn.Name = "$Skins" Then
                        skins += cell.Value
                    ElseIf cell.OwningColumn.Name = "$Closest" Then
                        ctp += cell.Value
                    ElseIf cell.OwningColumn.Name = "$Earn" Then
                        earned += cell.Value
                    ElseIf cell.OwningColumn.Name = "Points" Then
                        Points += cell.Value
                    ElseIf cell.OwningColumn.Name = "Team_Points" Then
                        tPoints += cell.Value
                    End If
                Next
            Next

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub
    Sub calctoPar(ihole As String, holeNum As String)
        Dim bother = True
        For i = -2 To 3 Step 1
            If ihole = oHelper.MyCourse(0)(holeNum).ToString + i Then
                ltoPar(i + 2) += 1
                bother = False
                Exit For
            End If
        Next
        If bother Then ltoPar(5) += 1
    End Sub
    Sub updateAvg(totrow As DataGridViewRow)
        Try
            totrow.DefaultCellStyle.BackColor = Color.LightCyan
            totrow.Cells("Method").Value = "Avg"
            totrow.Cells("Date").Value = "Rnds = " & sf9aRnds + sb9aRnds
            Dim d2dec As Decimal = 0.0
            If sf9aRnds > 0 Then
                d2dec = (f9gAvg / sf9aRnds)
                totrow.Cells("Out_Gross").Value = d2dec.ToString("##.0")
                d2dec = (f9nAvg / sf9aRnds) '.ToString("##.0")
                totrow.Cells("Out_Net").Value = d2dec.ToString("##.0")
            End If

            If sb9aRnds > 0 Then
                d2dec = (b9gAvg / sb9aRnds) '.ToString("##.0")
                totrow.Cells("In_Gross").Value = d2dec.ToString("##.0")
                d2dec = (b9nAvg / sb9aRnds) '.ToString("##.0")
                totrow.Cells("In_Net").Value = d2dec.ToString("##.0")
            End If

            totrow.Cells("$Earn").Value = earned
            totrow.Cells("$Skins").Value = skins
            totrow.Cells("$Closest").Value = ctp

            totrow.Cells("Points").Value = Points '.ToString("##.0")
            totrow.Cells("Team_Points").Value = tPoints '.ToString("##.0")

            Debug.Print("f9 " & sf9Rnds)
            Debug.Print("b9 " & sb9Rnds)

            'dgScores.Rows(dgScores.Rows.Count - 1).Cells("Group").Value = dgScores.Rows.Count - 1

            For i = 1 To 9 Step 1
                Debug.Print("Total " & lHoles(i))
                If lHoles(i) > 0 And sf9aRnds > 0 Then
                    d2dec = lHoles(i) / sf9Rnds
                    totrow.Cells("Hole" & i).Value = d2dec.ToString("##.0")
                End If
            Next

            For i = 10 To 18 Step 1
                Debug.Print("Total " & lHoles(i))
                If lHoles(i) > 0 And sb9aRnds > 0 Then
                    d2dec = lHoles(i) / sb9Rnds
                    totrow.Cells("Hole" & i).Value = d2dec.ToString("##.0")
                End If
            Next


            'oHelper.SBPChangeColorsForStrokes(totrow)
            Dim sbdots = oHelper.bDots
            oHelper.bDots = False
            For Each cell As DataGridViewCell In totrow.Cells
                Dim sColName = cell.OwningColumn.Name
                If sColName.Contains("Hole") Then
                    If oHelper.convDBNulltoSpaces(cell.Value).Trim <> "" Then
                        If cell.Value.ToString < oHelper.MyCourse(0)(sColName).ToString Then
                            cell.Style.BackColor = Color.OrangeRed
                        Else
                            cell.Style.BackColor = Color.White
                        End If
                    End If
                End If

            Next

            oHelper.bDots = sbdots

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub updateToPar(totrow As DataGridViewRow)

        Try
            totrow.DefaultCellStyle.BackColor = Color.LightCyan
            totrow.Cells("Method").Value = "ToPar"

            For Each cell As DataGridViewCell In totrow.Cells

                Dim col = cell.OwningColumn.Name
                Dim dec As Decimal = 0.0

                If col.Contains("Hole") Then
                    Dim i = col.Replace("Hole", "")
                    If i < 10 Then
                        If lHoles(i) > 0 And sf9Rnds > 0 Then
                            Debug.Print("Topar " & col & "-" & lHoles(i) & "-" & sf9Rnds & "-" & oHelper.MyCourse(0)(col).ToString)
                            dec = (lHoles(i) - (sf9Rnds * oHelper.MyCourse(0)(col).ToString)) / sf9Rnds
                            cell.Value = dec.ToString("0.0")
                        End If
                    Else
                        If lHoles(i) > 0 And sb9Rnds > 0 Then
                            Debug.Print("Topar " & col & "-" & lHoles(i) & "-" & sb9Rnds & "-" & oHelper.MyCourse(0)(col).ToString)
                            dec = (lHoles(i) - (sb9Rnds * oHelper.MyCourse(0)(col).ToString)) / sb9Rnds
                            cell.Value = dec.ToString("0.0")
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Sub updateRank(totrow As DataGridViewRow)

        Try
            totrow.DefaultCellStyle.BackColor = Color.LightCyan
            totrow.Cells("Method").Value = "Rank"
            Dim sRank As New List(Of rank)

            Dim srow As DataGridViewRow
            For Each row In dgScores.Rows
                If row.Cells("Method").Value.ToString = "" Then Continue For
                If row.cells("Method").value = "ToPar" Then
                    srow = row
                    Exit For
                End If
            Next
            Dim ii = 0
            For Each cell In srow.Cells
                If cell.owningcolumn.name.contains("Hole") And oHelper.convDBNulltoSpaces(cell.value) <> " " Then
                    sRank.Add(New rank(ii, cell.value))
                    ii += 1
                End If
            Next
            '         y, then x is descending, x then y is ascending
            sRank.Sort(Function(x, y) y.toPar.CompareTo(x.toPar))

            For i = 0 To 17 Step 1
                If i > sRank.Count - 1 Then Exit For
                totrow.Cells("Hole" & sRank(i).Hole + 1).Value = i + 1
            Next
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub oldBldScoresDataGridFromFile()
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

                If parm.Contains("(") Then sParm = parm.Substring(0, parm.IndexOf("("))

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
            For Each score As DataRowView In dv2scores
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
                If col.Name.Contains("Hole") Then col.HeaderText = col.HeaderText.Replace("Hole", "")
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