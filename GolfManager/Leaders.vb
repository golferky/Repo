﻿Imports System.IO
Public Class Leaders
    'Dim oHelper As New LeagueManager.Helper
    Dim oHelper As New Helper
    Dim rs As New Resizer
    Dim sColFormat = New List(Of String)
    Dim dtScoreCard As DataTable
    Dim sStatsDesc As String() = {"Eagles", "Birdies", "Pars", "Bogeys", "DoubleBogeys", "Others"}
    Dim sScoreOnlyDesc As String() = {"Rnds", "F9", "B9"}
    Dim sWH As String = ""
    Dim dtNewWklySkins
    Private Sub Scores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        leaders()
        BldScoresDataGridFromFile()
        sWH = oHelper.ScreenResize()
        If Me.Width >= sWH.Split(":")(0) Then
            Me.Width = sWH.Split(":")(0) - (sWH.Split(":")(0) * 0.1)
        Else
            'Me.Width = sWH.Split(":")(0)
        End If
        If Me.Height >= sWH.Split(":")(1) Then
            Me.Height = sWH.Split(":")(1) - (sWH.Split(":")(1) * 0.1)
        Else
            'Me.Height = sWH.Split(":")(1)
        End If
        oHelper.Resizedgv(dgScores, Me)
        oHelper.bDots = False
        'Dim iw As Integer = 0, ih As Integer = 0
        'For Each col As DataGridViewColumn In dgScores.Columns
        '    iw += col.Width
        'Next
        'For Each row As DataGridViewRow In dgScores.Rows
        '    ih += row.Height
        'Next
        '' oHelper.LOGIT(String.Format("dgScores {0}x{1}", iw, ih))
        'dgScores.Width = iw
        'dgScores.Height = ih
        'If Me.Width > dgScores.Width Then Me.Width = dgScores.Width * 1.1
        'If Me.Height > dgScores.Height Then Me.Height = dgScores.Height * 1.1

        'Me.Text &= String.Format(" Form {0}x{1}-Grid{2}x{3} ", Me.Width, Me.Height, dgScores.Width, dgScores.Height)
        If Not Debugger.IsAttached Then
            Dim sfn = oHelper.sReportPath & "\" & String.Format(DateTime.Now.ToString("yyyyMMdd_hhmmss_{0}_") & "ScoresAll.csv", oHelper.sPlayer)
            oHelper.dgv2csv(dgScores, sfn)
            '20190822 - new html
            Dim sHtml As String = oHelper.Create_Html_From_DGV(dgScores)
            sHtml = oHelper.ConvertDataGridViewToHTMLWithFormatting(dgScores, Me)
            Dim swhtml As New IO.StreamWriter(sfn.Replace(".csv", ".html"), False)
            swhtml.WriteLine(sHtml)
            swhtml.Close()
        End If
        ' rs.FindAllControls(Me)
    End Sub

    Sub BldScoresDataGridFromFile()
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            '10/1/2017 add code to pull in all scores for a given player
            'check frmScoreCard for event
            '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
            '2 - if double click on a playerevent, get scores for a given player
            'create array from above defined fields we want out of scorecard
            Dim sArray = New List(Of String)
            sArray.AddRange(Helper.cBaseScoreCard.Split(","))
            sArray.AddRange(Helper.cSkinsFields.Split(","))

            '20180222-expand #closests to track each individual hle for carry overs
            'For Each fld In sArray
            '    If fld.Contains("#Closests") Then
            '        For i = 1 To 18
            '            If oHelper.thisCourse("Hole" & i) = 3 Then
            '                sArray.Add(String.Format("CTP #{0}-cPat40nt", i))
            '            End If
            '        Next
            '        sArray.Remove(fld)
            '        Exit For
            '    End If
            'Next

            sArray.Remove("#Closests-cPat40nt")
            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            Dim sScoreCardforDGV = ""
            'strip parenthesis and add gross/net for In/Out
            'fields can have a pattern associated for cell length, centering,
            For Each parm As String In sArray
                ''set default pattern
                'Dim sPat = Helper.cPat40
                Dim sParm = ""

                If UBound(parm.Split("-")) = 0 Then
                    sParm = parm
                Else
                    sParm = parm.Split("-")(0)
                    'sPat = parm.Substring(parm.IndexOf("-") + 1)
                End If

                If parm.Contains("(") Then
                    sParm = parm.Substring(0, parm.IndexOf("("))
                End If

                If sParm = "Holes" Then
                    If sParm = "Holes" Then
                        For i As Integer = 1 To 18
                            sScoreCardforDGV = sScoreCardforDGV + "Hole" & i & ","
                            If i = 9 Then
                                sScoreCardforDGV = sScoreCardforDGV + "Out Gross,Out Net,"
                            ElseIf i = 18 Then
                                sScoreCardforDGV = sScoreCardforDGV + "In Gross,In Net,"
                            End If
                        Next
                        Continue For
                    End If

                    Continue For
                End If
                sScoreCardforDGV = sScoreCardforDGV + sParm + ","
            Next
            'remove trailing comma
            'replace spaces with underscores for csv column matchups
            'need to rename ctp_1, _2 and have 4 columns for Ctp(one for each par 3)
            sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")
            dtScoreCard = dvScores.ToTable(False, sScoreCardforDGV.Split(",").ToArray)
            dtScoreCard.Clear()
            oHelper.CreateColumn("Period", dtScoreCard)
            oHelper.CreateColumn("#Closests", dtScoreCard)
            oHelper.CreateColumn("Rnds", dtScoreCard)
            oHelper.CreateColumn("F9", dtScoreCard)
            oHelper.CreateColumn("B9", dtScoreCard)

            ''add col for each stat
            For Each fld As String In Helper.cStatsFields.Split(",")
                oHelper.CreateColumn(fld.Split("-")(0), dtScoreCard)
            Next

            'add col for each Score only field
            For Each fld As String In sScoreOnlyDesc
                oHelper.CreateColumn("SO_" & fld, dtScoreCard)
            Next

            Dim ictpctr = 1
            For i = 1 To 9
                If oHelper.thisCourse("Hole" & i) = 3 Then
                    oHelper.CreateColumn("CTP " & ictpctr, dtScoreCard)
                    sArray.Add("CTP " & ictpctr & "-cPat40nt")
                    ictpctr += 1
                End If
            Next

            oHelper.CreateColumn("Points", dtScoreCard)
            oHelper.CreateColumn("Team_Points", dtScoreCard)
            oHelper.CreateColumn("Opponent", dtScoreCard)

            For Each player In oHelper.dsLeague.Tables("dtPlayers").Rows
                Dim newrow As DataRow = Nothing
                'create avg ytd and scores only (so) avg ytd
                For Each sDate As String In Main.cbLeagues.Items
                    'createTotalRow(dtScoreCard, newrow, String.Format("{0} Avg", sDate.Substring(sDate.IndexOf("(") + 1, 4)))
                    'createTotalRow(dtScoreCard, newrow, String.Format("{0} ToPar", sDate.Substring(sDate.IndexOf("(") + 1, 4)))
                    'createTotalRow(dtScoreCard, newrow, String.Format("{0} Rank", sDate.Substring(sDate.IndexOf("(") + 1, 4)))
                    createTotalRow(dtScoreCard, newrow, String.Format("{0} Avg", player(Constants.name)))
                    createTotalRow(dtScoreCard, newrow, String.Format("{0} ToPar", player(Constants.name)))
                    createTotalRow(dtScoreCard, newrow, String.Format("{0} Rank", player(Constants.name)))
                Next
                createTotalRow(dtScoreCard, newrow, "Avg")
                createTotalRow(dtScoreCard, newrow, "ToPar")
                createTotalRow(dtScoreCard, newrow, "Rank")
            Next


            oHelper.iHoles = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0).Item("Holes")
            oHelper.iHoleMarker = 1
            dvScores.Sort = "Date Desc"

            Dim xx = String.Format("Player = '{0}' and Method in ({1}) ", oHelper.sPlayer, "'Score','Gross','Net'")
            For Each player In oHelper.dsLeague.Tables("dtPlayers").Rows
                dvScores.RowFilter = String.Format("Player = '{0}' and Method in ({1}) ", player, "'Score','Gross','Net'")
            Next
            'scores by player only keeps 9 holes for some leagues

            'Turn all Net scores into Gross Scores and accum stats
            For Each score As DataRow In dtScoreCard.Rows
                'if non numeric date(method), were done
                If Not IsNumeric(score("Date")) Then Exit For
                If score("Method") Is DBNull.Value Then Continue For

                'if this scores method = score only(no hole by hole) then add so
                AccumScores(score, String.Format("Method = {0} And Date = 'Avg'", "'" & score("Date").ToString.Substring(0, 4) & "'"))
                'career stats
                AccumScores(score, String.Format("Method = 'Career' And Date = 'Avg'"))
            Next

            Dim foundRows() As DataRow = Nothing
            'calculate total rounds, front 9 and back 9, average scores per holes
            foundRows = dtScoreCard.Select(String.Format("Date Like {0}", "'%Avg%'"))
            If foundRows Is Nothing Then
                MsgBox("Error calculating Career Avg...contact programmer")
                Exit Sub
            End If

            'process each year
            For Each row In foundRows
                Dim drRounds() As DataRow = dtScoreCard.Select(String.Format("Method = '{0}' and Date = '{1}'", row("Method"), row("Date").ToString.Replace("Avg", "ToPar"))) '"*** Y-2017 Avg ***"
                calcToPar(row, drRounds)
                Dim drRank() As DataRow = dtScoreCard.Select(String.Format("Method = '{0}' and Date = '{1}'", row("Method"), row("Date").ToString.Replace("Avg", "Rank"))) '"*** Y-2017 Avg ***"
                calcRank(drRounds(0), drRank(0))
            Next

            dgScores.Columns.Clear()
            dgScores.RowTemplate.Height = 40
            dgScores.DefaultCellStyle.Font = New Font("Tahoma", 15)

            Dim arr As Array = sScoreCardforDGV.Split(",").ToArray
            For Each col As DataColumn In dtScoreCard.Columns
                If Not col.ColumnName.StartsWith("SO_") Then
                    Dim dgc As New DataGridViewTextBoxColumn
                    With dgc
                        .Name = col.ColumnName
                        .ValueType = GetType(System.String)
                    End With
                    dgScores.Columns.Add(dgc)
                End If
            Next

            Dim sdel = "-"
            For Each col As DataGridViewColumn In dgScores.Columns
                Dim sformat = ""
                Try
                    If sColFormat(col.Index).StartsWith("cPat") Then
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
                            Case "cPatHole"
                                sformat = Helper.cPat20
                            Case Else
                                sformat = Helper.cPat40
                        End Select
                    Else
                        sformat = sColFormat(col.Index)
                    End If

                Catch ex As Exception
                    Dim x = ""
                End Try
                '20181009 - these are new fields added for accumulation only
                If sformat.Split(sdel)(0) = "" Then
                    col.Width = 40
                    col.DataGridView.ReadOnly = True
                    col.DataGridView.TabStop = True
                    Continue For
                End If
                Dim scolname = col.Name
                Dim sWidth = sformat.Split(sdel)(0)
                Dim sRO = sformat.Split(sdel)(1)
                Dim sTabstop = sformat.Split(sdel)(2)
                Dim sAlign = sformat.Split(sdel)(3)
                col.Width = sWidth
                col.DataGridView.ReadOnly = sRO
                col.DataGridView.TabStop = sTabstop
                col.SortMode = DataGridViewColumnSortMode.NotSortable
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

            For Each row As DataRow In dtScoreCard.Rows
                dgScores.Rows.Add(row.ItemArray)
            Next

            Try
                Dim sFont = "Tahoma"
                Dim iFontSize = 8
                oHelper.bColors = True
                For Each row As DataGridViewRow In dgScores.Rows
                    row.Height = 25
                    If row.Cells("Method").Value Is DBNull.Value Then Continue For
                    Dim sStat As String = row.Cells("Date").Value
                    row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    'numeric date indicates a score card
                    If Not IsNumeric(sStat) Then
                        If sStat <> "Rank" Then
                            row.Cells("Method").Style.BackColor = Color.LightGray
                            ChangeFont(row)
                            ColorCells(row)
                            Continue For
                        Else
                            row.Cells("Method").Style.BackColor = Color.LightGray
                            ChangeFont(row)
                            Continue For
                        End If
                    End If
                    '20181004 - override calcholemarker because of cc and league in same sch
                    oHelper.CalcHoleMarker(row.Cells("Date").Value)
                    If row.Cells("Method").Value = "Gross" Or row.Cells("Method").Value = "Net" Then
                        If row.Cells("Hole1").Value IsNot DBNull.Value Then
                            oHelper.iHoleMarker = 1
                        Else
                            oHelper.iHoleMarker = 10
                        End If
                    End If
                    If oHelper.iHoleMarker = 10 Then
                        For i = 0 To 3
                            row.Cells(i).Style.BackColor = Color.LightBlue
                        Next
                    End If
                    oHelper.SBPChangeColorsForStrokes(row)
                    '20200121-blank out opponent for post season
                    For Each lprow As DataRow In oHelper.dsLeague.Tables("dtLeagueParms").Rows
                        Dim x = CDate(lprow("StartDate")).ToString("yyyyMMdd").Substring(0, 4)
                        If CDate(lprow("StartDate")).ToString("yyyyMMdd").Substring(0, 4) <> row.Cells("Date").Value.Substring(0, 4) Then
                            Continue For
                        End If
                        x = CDate(lprow("PostSeasonDt")).ToString("yyyyMMdd")
                        If row.Cells("Date").Value >= CDate(lprow("PostSeasonDt")).ToString("yyyyMMdd") Then
                            row.Cells("Opponent").Value = ""
                        End If
                    Next

                    If row.Cells("Skins").Value.ToString.ToUpper = "TRUE" Then row.Cells("Skins").Value = "Y"
                    If row.Cells("Skins").Value.ToString.ToUpper = "FALSE" Then row.Cells("Skins").Value = "N"

                    If row.Cells("Closest").Value.ToString.ToUpper = "TRUE" Then row.Cells("Closest").Value = "Y"
                    If row.Cells("Closest").Value.ToString.ToUpper = "FALSE" Then row.Cells("Closest").Value = "N"
                Next
                oHelper.bColors = False

            Catch ex As Exception
                Dim x = ""
            End Try

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub ChangeFont(row As DataGridViewRow)
        Dim cell As DataGridViewCell = Nothing
        Dim sFont = "Tahoma"
        Dim iFontSize = 8

        For Each dgc As DataGridViewCell In row.Cells
            If dgc.OwningColumn.Name.Contains("Method") Or dgc.OwningColumn.Name.Contains("Date") Then Continue For
            With dgc
                .Style.Font = New Font(sFont, iFontSize, FontStyle.Regular)
                .Style.ForeColor = Color.Black
                .Style.BackColor = Color.White
            End With
        Next

    End Sub
    Sub ColorCells(row As DataGridViewRow)
        Try
            For i = 1 To 18
                Dim cell As DataGridViewCell = row.Cells("Hole" & i)
                If IsNumeric(row.Cells("Hole" & i).Value) Then
                    If row.Cells("Hole" & i).Value > 0 Then
                        Dim iPar As Integer = oHelper.thisCourse("Hole" & i).ToString
                        Dim iScore As String = row.Cells("Hole" & i).Value
                        If Not row.Cells("Date").Value.ToString.Contains("ToPar") Then
                            If cell.Value < iPar Then cell.Style.BackColor = Color.Red
                        End If
                    Else
                        If row.Cells("Date").Value.ToString.Contains("ToPar") Then
                            If row.Cells("Hole" & i).Value < 0 Then
                                cell.Value = cell.Value * -1
                                cell.Style.BackColor = Color.Red
                            End If
                        Else
                            row.Cells("Hole" & i).Value = ""
                        End If

                    End If
                End If
            Next
            If IsNumeric(row.Cells("Out_Gross").Value) Then
                If row.Cells("Out_Gross").Value < oHelper.thisCourse("Out").ToString Then row.Cells("Out_Gross").Style.BackColor = Color.Red
            End If
            If IsNumeric(row.Cells("Out_Net").Value) Then
                If row.Cells("Out_Net").Value < oHelper.thisCourse("Out").ToString Then row.Cells("Out_Net").Style.BackColor = Color.Red
            End If

            If IsNumeric(row.Cells("In_Gross").Value) Then
                If row.Cells("In_Gross").Value < oHelper.thisCourse("In").ToString Then row.Cells("In_Gross").Style.BackColor = Color.Red
            End If

            If IsNumeric(row.Cells("In_Net").Value) Then
                If row.Cells("In_Net").Value < oHelper.thisCourse("In").ToString Then row.Cells("In_Net").Style.BackColor = Color.Red
            End If

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub calcRank(toPar As DataRow, drRank As DataRow)

        Dim sRank As New List(Of rank)
        Dim ii = 0
        Try
            'For Each cell In row.Cells
            For i = 1 To 18
                Dim sscore = toPar("Hole" & i)
                If sscore IsNot DBNull.Value Then
                    sRank.Add(New rank(ii, sscore))
                    ii += 1
                End If
            Next

            '         y, then x is descending, x then y is ascending
            sRank.Sort(Function(x, y) y.toPar.CompareTo(x.toPar))

            For i = 0 To 17 Step 1
                If i > sRank.Count - 1 Then Exit For
                drRank("Hole" & sRank(i).Hole + 1) = i + 1
            Next

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub calcToPar(row As DataRow, drToPar() As DataRow)
        Dim d2dec As Decimal = 0.0

        Try
            If IsNumeric(row("Out_Gross")) Then
                If row("Out_Gross") > 0 Then
                    d2dec = CInt(row("Out_Gross")) / CInt(row("F9") + CInt(row("SO_F9")))
                    row("Out_Gross") = d2dec.ToString("##.0")
                End If
            End If
            If IsNumeric(row("Out_Net")) Then
                If row("Out_Net") > 0 Then
                    d2dec = CInt(row("Out_Net")) / CInt(row("F9") + CInt(row("SO_F9")))
                    row("Out_Net") = d2dec.ToString("##.0")
                End If
            End If
            If IsNumeric(row("In_Gross")) Then
                If row("In_Gross") > 0 Then
                    d2dec = CInt(row("In_Gross")) / CInt(row("B9") + CInt(row("SO_B9")))
                    row("In_Gross") = d2dec.ToString("##.0")
                End If
            End If
            If IsNumeric(row("In_Net")) Then
                If row("In_Net") > 0 Then
                    d2dec = CInt(row("In_Net")) / CInt(row("B9") + CInt(row("SO_B9")))
                    row("In_Net") = d2dec.ToString("##.0")
                End If
            End If

            For i = 1 To 18
                calcHolebyHole(row, i, drToPar(0))
            Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub calcHolebyHole(row As DataRow, i As Integer, drToPar As DataRow)
        Dim d2dec As Decimal = 0.0
        Try
            If Not row("Hole" & i) > 0 Then Exit Sub
            d2dec = CInt(row("Hole" & i)) / CInt(row(IIf(i < 10, "F9", "B9")))
            row("Hole" & i) = d2dec.ToString("#.0")
            d2dec -= oHelper.thisCourse("Hole" & i).ToString
            'this updates the ytd topar
            drToPar("Hole" & i) = d2dec.ToString("#.0")
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub AccumScores(score As DataRow, sselect As String)
        Dim sMethod = oHelper.convDBNulltoSpaces(score("Method")).Trim
        Dim foundRows() As DataRow = dtScoreCard.Select(sselect)

        Try
            If foundRows.Count > 1 Then
                MsgBox(String.Format("Contact Developer, error in {0}{1}", vbCrLf, sselect))
                Exit Sub
            End If
            For Each row In foundRows

                Dim scolprefix As String = ""
                If sMethod = "Score" Then scolprefix = "SO_"

                'row("Rnds") += 1
                row(scolprefix & "Rnds") += 1
                If IsNumeric(score("Out_Gross")) Then
                    row(scolprefix & "F9") += 1
                    row("Out_Gross") = CInt(row("Out_Gross")) + score("Out_Gross")
                End If
                If IsNumeric(score("Out_Net")) Then
                    row(scolprefix & "Out_Net") = CInt(row(scolprefix & "Out_Net")) + score("Out_Net")
                ElseIf IsNumeric(score("Out_Gross")) Then
                    row("Out_Net") = row("Out_Net") + CInt(score("Out_Gross")) - score("Phdcp")
                End If

                If IsNumeric(score("In_Gross")) Then
                    row(scolprefix & "B9") += 1
                    row("In_Gross") = CInt(row("In_Gross")) + score("In_Gross")
                End If
                If IsNumeric(score("In_Net")) Then
                    row("In_Net") = CInt(row("In_Net")) + score("In_Net")
                ElseIf IsNumeric(score("In_Gross")) Then
                    row("In_Net") = CInt(row("In_Net")) + CInt(score("In_Gross")) - score("Phdcp")
                End If
                ' calculate skins ctp, etc
                Try
                    Dim sStats As String() = {"$Earn", "$Skins", "$Closest", "#Skins", "CTP_1", "CTP_2", "Points", "Team_Points"}
                    For Each stat In sStats
                        If IsNumeric(score(stat)) Then
                            row(stat) = CDec(row(stat)) + score(stat)
                        End If
                    Next

                Catch ex As Exception
                    Dim x = ""
                End Try

                For i = 1 To 18
                    Dim iscore = score("Hole" & i)
                    If iscore Is DBNull.Value Then Continue For
                    'if the handicap > stroke index adjust net score to gross
                    If score("Method") = "Net" Then
                        Dim isi = oHelper.CalcStrokeIndex("Hole" & i)
                        If score("pHdcp") >= isi Then
                            'check stroke index
                            iscore += 1
                            If score("pHdcp") - oHelper.iHoles >= isi Then iscore += 1
                        End If
                    End If
                    row("Hole" & i) = CInt(row("Hole" & i)) + iscore

                    'add to stats rows
                    Dim istart = oHelper.thisCourse("Hole" & i).ToString - 2 'start with `
                    Try
                        For x = 0 To sStatsDesc.Length - 1
                            If iscore = istart Then
                                row(sStatsDesc(x)) += 1
                                Exit For
                            End If
                            istart += 1
                        Next
                    Catch ex As Exception
                        Dim x = ""
                    End Try

                Next
            Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Sub createTotalRow(dtscorecard As DataTable, newrow As DataRow, fld As String)
        Try
            newrow = dtscorecard.NewRow
            newrow(Constants.Player) = fld
            If UBound(fld.Split(" ")) > 0 Then
                If IsNumeric(fld.Substring(0, 4)) Then
                    newrow("Method") = fld.Substring(0, 4)
                End If
            Else
                newrow("Method") = "Career"
            End If

            If fld.Contains("Avg") Then
                newrow("In_Gross") = 0
                newrow("In_Gross") = 0
                newrow("In_Net") = 0
                newrow("Out_Gross") = 0
                newrow("Out_Net") = 0
                newrow("$Earn") = 0
                newrow("$Skins") = 0
                newrow("$Closest") = 0
                newrow("#Skins") = 0
                '                newrow("CTP_1") = 0
                '                newrow("CTP_2") = 0
                newrow("Points") = 0
                newrow("Team_Points") = 0
                newrow("Rnds") = 0
                newrow("F9") = 0
                newrow("B9") = 0
                For i = 1 To 18
                    newrow("Hole" & i) = 0
                Next
                For Each col As DataColumn In dtscorecard.Columns
                    If sStatsDesc.Contains(col.ColumnName) Then newrow(col.ColumnName) = 0
                    If col.ColumnName.Contains("SO_") Then
                        newrow(col.ColumnName) = 0
                    End If
                Next
            End If
            dtscorecard.Rows.Add(newrow)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        Dim x = sender
        Dim xx = ""
    End Sub
    Private Sub dgScores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.ColumnHeaderMouseClick

        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
            oHelper.bScoresbyPlayer = True
            Dim dgv = sender
            For Each row As DataGridViewRow In dgv.rows
                'row.Height = 30
                oHelper.ChangeColorsForStrokes(row)
            Next
            oHelper.bScoresbyPlayer = False
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgScores.SortCompare
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        '20180725 - dont allow sort for now
        Exit Sub
        Try
            'e.SortResult = If(CInt(e.CellValue1) < CInt(e.CellValue2), -1, 1)
            'e.Handled = True
            Main.oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub Scores_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '    rs.ResizeAllControls(Me)
            oHelper.LOGIT(String.Format("Form Height {0} Width {1}", Me.Height, Me.Width))
            Me.Text = String.Format("Player {8} - Form {7}-{0}, Resolution {1} x {2}, Menu {3} x {4}, Grid {5} x {6}", My.Computer.Name, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Width, Me.Width, Me.Height, dgScores.Width, dgScores.Height, Me.Name, oHelper.sPlayer)

        Catch ex As Exception

        End Try
    End Sub
    Private Sub leaders()
        'calculate leftoverskins and fill text boxes with this weeks amounts
        oHelper.LOGIT(String.Format("calculate stats for all players"))

        Try
            If File.Exists("c:\temp\scores.txt") Then File.Delete("c:\temp\scores.txt")
            If File.Exists("c:\temp\courses.txt") Then File.Delete("c:\temp\courses.txt")

            Dim sFrom = CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd")
            Dim sTo = CDate(oHelper.rLeagueParmrow("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd")
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), "c:\temp\scores.txt")
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtCourses"), "c:\temp\courses.txt")
            'Dim cn As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';")
            Dim cn As New OleDb.OleDbConnection(String.Format("Provider={0};Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';", oHelper.GetMACDBVersion))
            Dim sWhere = String.Format("Where date >= {0} and date < {1} Group By date", sFrom, sTo)
            Debug.Print("")
            'https://stackoverflow.com/questions/22990229/count-iif-access-query
            Dim sSkinIIf = "IIf(Ucase(Skins) = 'TRUE', 1, 0)"
            Dim strsql As String = String.Format _
        ("SELECT IIF(SUM(Hole1) > 0,'Front','Back') as FrontBack 
            , Player
            , COUNT(*) as Rounds
            , SUM({1}) as SkinRounds
            , SUM({2}) as CTPRounds
            , SUM([#Skins]) as NumSkins
            , SUM([$Skins]) as SkinsAmount
            , IIF(FrontBack = 'Front',SUM(CTP_1), 0) as CTP_4
            , IIF(FrontBack = 'Front',SUM(CTP_2), 0) as CTP_8
            , IIF(FrontBack = 'Back',SUM(CTP_1), 0) as CTP_10
            , IIF(FrontBack = 'Back',SUM(CTP_2), 0) as CTP_12

            , SUM([Points]) as IndPoints
            , SUM([Team_Points]) as TeamPoints
            , SUM([Team_Points]) + SUM(Points) as TotalPoints 
            , ROUND(TotalPoints / Rounds,2) as PointsPerRnd

            , ROUND(AVG(Out_Gross),2) as AvgFront9 
            , ROUND(AVG(In_Gross),2) as AvgBack9 
            , ROUND((AVG(Out_Gross) + AVG(In_Gross)) / 2,2) as AvgScore
            , MIN(Out_Gross) as LowF9Gr
            , MIN(In_Gross) as LowB9Gr
            , MAX(Out_Gross) as HiF9Gr
            , MAX(In_Gross) as HiB9Gr
            , MIN(Out_Net) as LowF9N
            , MIN(In_Net) as LowB9N
            , MAX(Out_Net) as HiF9N
            , MAX(In_Net) as HiB9N

            FROM [Scores.txt] Scores
            WHERE Date >= 20180101",
         CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"),
         sSkinIIf,
         sSkinIIf.Replace(Constants.skin, Constants.closest)
)
            'INNER JOIN [Courses.txt] Courses ON Scores.Course='Boone Links'
            ', FORMAT(Date,'yyyymmdd') 

            strsql += vbCrLf & "GROUP BY Frontback,Player"

            dtNewWklySkins = New DataTable("Stats")
            Dim da As New OleDb.OleDbDataAdapter(strsql, cn)
            da.Fill(dtnewWklySkins)
            dtnewWklySkins.PrimaryKey = New DataColumn() {dtnewWklySkins.Columns("Date")}
            Dim dt1 As New DataTable("Points")
            Dim strsql1 As String = String.Format _
        (
            "SELECT Team
                , SUM(Points) as Points
                , SUM([Team_Points]) as TeamPoints 
                , SUM([Team_Points]) + SUM(Points) as TotalPoints 
                FROM [temp.txt] WHERE Date >= {0} and Date < {1} 
                GROUP BY Team",
            CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd")
        )
            da = New OleDb.OleDbDataAdapter(strsql1, cn)
            da.Fill(dt1)
            Debug.Print("")
        Catch ex As Exception

        End Try
    End Sub
End Class

'Public Class rank
'    Public Hole As String
'    Public toPar As Decimal

'    Public Sub New(ByVal hole As String, ByVal toPar As Decimal)
'        Me.Hole = hole
'        Me.toPar = toPar
'    End Sub

'    Public Overrides Function ToString() As String
'        Return String.Format("{0}, {1}", Me.Hole, Me.toPar)
'    End Function
'End Class