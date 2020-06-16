Imports System.ComponentModel
'Imports GolfManager.Helper
Imports GolfManager.Constants
Imports GolfManager.Helper
Public Class ScoreCard

    Dim oHelper As New Helper
    Dim bFormLoad As Boolean = True
    Dim bSkipFB As Boolean = True
    Dim sOldCellValue As Object
    Dim lskins As List(Of String)
    Dim xlskins As List(Of String)
    Dim sLowRows = New List(Of String)
    Dim bstrokehole As Boolean = False
    Dim thisweeksSkinsCTPS As DataRow
    Dim lastweeksSkinsCTPS As DataRow
    Dim bshowpaint As Boolean = False

    Private Sub ScoreCard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        DsLeague = oHelper.dsLeague
        dgScores.RowTemplate.Height = 23
        dgScores.DefaultCellStyle.Font = New Font("Tahoma", 9)

        'Me.Show()
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'lbStatus.Text = String.Format("Loading Scores from {0}", Main.lbScoresFile.Text)
        'Dim xx = Main.lbScoresFile.Text
        'oHelper.status_Msg(lbStatus, Me)

        'copy main's helper
        DsLeague = oHelper.dsLeague
        RemoveHandler rbFront.CheckedChanged,
            AddressOf rbFront_CheckedChanged
        RemoveHandler rbStandard.CheckedChanged,
            AddressOf rbStandard_CheckedChanged
        cbDates.Items.AddRange(Main.cbDates.Items.Cast(Of String).ToArray)
        'remove non-match dates from dates combobox
        ''cdateToyyyyMMdd converts a string from 1/1/1900 to 19000101
        'Do While cbDates.Items(0) > oHelper.sDateLastScore
        '    cbDates.Items.Remove(cbDates.Items(0))
        'Loop
        cbDates.SelectedIndex = cbDates.Items.IndexOf(oHelper.sDateLastScore) 'cbDates.Items.IndexOf(oHelper.dDate.ToString("yyyyMMdd"))
        Dim dvscores As New DataView(DsLeague.Tables("dtScores"))
        With dvscores
            .RowFilter = String.Format("Date = '{0}'", cbDates.SelectedItem)
        End With
        oHelper.CalcThisHoleMarker(cbDates.SelectedItem, dvscores)
        If oHelper.iHoleMarker = 1 Then
            rbFront.Checked = True
            rbBack.Checked = False
        Else
            rbFront.Checked = False
            rbBack.Checked = True
        End If

        AddHandler rbFront.CheckedChanged,
            AddressOf rbFront_CheckedChanged
        AddHandler rbStandard.CheckedChanged,
            AddressOf rbStandard_CheckedChanged
        lbStatus.Text = ""
        lbUnderPar.BackColor = oHelper.UnderParColor
    End Sub
    Sub recreateTextBoxesasCheckBoxes(sColName As String, iCol As Int16)
        Try
            'If TypeOf dgScores.CurrentCell Is DataGridViewCheckBoxCell Then Exit Sub
            'recreate columns as checkboxes
            Dim ncol As New DataGridViewCheckBoxColumn
            ncol.HeaderText = sColName
            ncol.Name = ncol.HeaderText
            ncol.DataPropertyName = ncol.HeaderText
            ncol.Width = 40
            'ncol.ValueType = GetType(Boolean)
            dgScores.Columns.RemoveAt(iCol)
            dgScores.Columns.Insert(iCol, ncol)
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub recreateCheckBoxesasTextBoxes(sColName As String, iCol As Int16)
        Try

            'If TypeOf dgScores.CurrentCell Is DataGridViewTextBoxCell Then Exit Sub
            'recreate columns as checkboxes
            Dim ncol As New DataGridViewTextBoxColumn
            ncol.HeaderText = sColName
            ncol.Name = ncol.HeaderText
            ncol.DataPropertyName = ncol.HeaderText
            ncol.Width = 40
            dgScores.Columns.RemoveAt(iCol)
            dgScores.Columns.Insert(iCol, ncol)
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Function Stableford(score As String, hole As String, hdcp As Int16) As String
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Stableford = 0
        'double bogey,bogey,par,birdie,eagle, double eagle
        Dim spoints As String = "0,1,2,3,4,8"
        Dim spar = oHelper.thisCourse("Hole" & hole) + 2
        Dim spointer = spar - score
        'check stroke index
        Dim isi As Int16 = oHelper.SBPCalcStrokeIndex("Hole" & hole)
        Dim iHdcpStrokes As Int16 = 0
        'LOGIT(sPlayer & "-" & iHdcp & "-" & iStrokeIndex & "-" & isi & "-" & cell.OwningColumn.Name & "-")
        'if the handicap > stroke index make color beige
        If hdcp >= isi Then
            iHdcpStrokes += 1
            'if double stroke hole, make color b/a
            If hdcp - oHelper.iHoles >= isi Then iHdcpStrokes += 1
        End If
        score -= iHdcpStrokes
        'check score against max points (par - netscore) 
        If score < 0 Then score = 0
        Try
            Dim x = spoints.Split(",")(0)
            Dim xx = (oHelper.thisCourse("Hole" & hole) + 2)
            'get double bogey max score and compare to max in array
            If (oHelper.thisCourse("Hole" & hole) + 2) - score <= 0 Then
                Stableford = spoints.Split(",")(0)
            Else
                Stableford = spoints.Split(",")((oHelper.thisCourse("Hole" & hole) + 2) - score)
            End If

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
        oHelper.LOGIT(String.Format("calculating hole {0} par {1} score {2} stableford {3}", hole, spar, score, Stableford))
    End Function
    Sub EditScores()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bDGSError = False
            oHelper.bAllHolesEntered = False
            Dim dgr As DataGridView = dgScores
            Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name
            If sCurrColName = skin Or sCurrColName = closest Then
                Exit Sub
            ElseIf sCurrColName = Clear Then
                dgr.CurrentCell.Value = sOldCellValue
                Exit Sub
            End If

            'if no change, then exit 
            If sOldCellValue IsNot DBNull.Value Then
                If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
            End If
            Dim R As DataGridViewRow = dgr.CurrentRow
            If R.Cells("Phdcp").Value Is Nothing Then oHelper.IHdcp = 99

            If sCurrColName = "Player" Then
                editPlayer(R, sCurrColName)
            ElseIf sCurrColName = "Method" Then
                editMethod(R, sCurrColName)
            ElseIf sCurrColName.StartsWith("Hole") Then
                editHoles(R, sCurrColName)
                editrest(R, sCurrColName)
            ElseIf sCurrColName = "Team" Then
                editTeam(R, sCurrColName)
                '20180220-do gross on score method
            ElseIf sCurrColName.Contains("Gross") Then
                editGross(R, sCurrColName)
            End If

            If oHelper.bDGSError Then SendKeys.Send("+{TAB}")
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
#Region "Scorecard Editor"
    Sub editPlayer(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bDGSError = False
            '    '1  Get the player name from the scores view with his handicap
            '    '2  if scores havent been updated for this player/date, then update the gridview and mark stroke holes
            '    'try looking for intials if the length is 2 char
            oHelper.sPlayer = oHelper.RemoveNulls(R.Cells(sCurrColName).Value)

            If Not Len(oHelper.sPlayer) >= 2 Then
                MessageBox.Show("Player Must be at least 2 chars", "Player Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                R.Cells(sCurrColName).Value = sOldCellValue
                oHelper.bDGSError = True
            End If
            oHelper.sPlayer = oHelper.fGetPlayer(oHelper.RemoveNulls(R.Cells(sCurrColName).Value), dgScores)

            '20180116-check to see if player is already entered
            Dim rptr As Integer = R.Index
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Index <> rptr And row.Cells(sCurrColName).Value = oHelper.sPlayer Then
                    MsgBox(String.Format("You've already entered this player {0}, Try Again", oHelper.sPlayer))
                    R.Cells(sCurrColName).Value = sOldCellValue
                    oHelper.bDGSError = True
                    Exit Sub
                End If
            Next

            Try
                oHelper.IHdcp = GetPHdcp()
            Catch ex As Exception
                oHelper.IHdcp = 99
            End Try

            R.Cells("PHdcp").Value = oHelper.IHdcp
            R.Cells(sCurrColName).Value = oHelper.sPlayer
            Dim MyPlayer As DataRow = DsLeague.Tables("dtPlayers").Rows.Find(oHelper.sPlayer)
            If R.Cells("Team").Value = "" Then R.Cells("Team").Value = MyPlayer("Team")

            'If rbColors.Checked Then oHelper.bColors = True Else oHelper.bColors = False
            'If rbDots.Checked Then oHelper.bDots = True Else oHelper.bDots = False
            'oHelper.displayStrokes(R)
            '20180121 fix color blue on subs
            Dim dvplayers As New DataView(DsLeague.Tables("dtPlayers"))
            R.Cells(sCurrColName).Style.BackColor = Color.White
            dvplayers.RowFilter = "Name = '" & R.Cells(sCurrColName).Value & "'"
            oHelper.sPlayer = R.Cells(sCurrColName).Value.ToString
            If dvplayers.Count = 0 Then
                Exit Sub
            End If

            'if no team, they are a sub but no subs in post season allowed
            If dvplayers(0).Item("Team") Is DBNull.Value Then
                R.Cells(sCurrColName).Style.BackColor = Color.Aqua
            ElseIf dvplayers(0).Item("Team") <> R.Cells("Team").Value Then
                R.Cells(sCurrColName).Style.BackColor = Color.Aqua
            Else
                R.Cells(sCurrColName).Style.BackColor = Color.White
            End If
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Cells("Opponent").Value = sOldCellValue Then
                    row.Cells("Opponent").Value = row.Cells("Opponent").Value.ToString.Replace(sOldCellValue, R.Cells("Player").Value)
                End If
            Next
            '20200516-no need to edit hole by hole on a player change, removed for now
            'For Each col As DataGridViewCell In R.Cells
            '    If col.OwningColumn.Name.StartsWith("Hole") And col.Visible Then
            '        editHoles(R, col.OwningColumn.Name)
            '    End If
            'Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub editMethod(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Try
            Dim sMethod = ""

            If Not R.Cells(sCurrColName).Value Is DBNull.Value Then
                sMethod = oHelper.convDBNulltoSpaces(R.Cells(sCurrColName).Value.ToString.ToUpper).Trim
            End If

            If Not R.IsNewRow Then

                If sMethod = "Score".ToUpper Then
                    '20180219-clear gross / net score 
                    If oHelper.iHoleMarker = 1 Then
                        R.Cells("Out_Gross").Value = ""
                        R.Cells("Out_Net").Value = ""
                        '20180510-turn off read only for method=score
                        'R.Cells("Out_Gross").ReadOnly = False
                    Else
                        R.Cells("In_Gross").Value = ""
                        R.Cells("In_Net").Value = ""
                        '20180510-turn off read only for method=score
                        'R.Cells("In_Gross").ReadOnly = False
                    End If
                    R.Cells("Hdcp").Value = ""
                    Dim mbr = MsgBox("Score method is being used, hole scores will be cleared", MsgBoxStyle.OkCancel)
                    If mbr = MsgBoxResult.Cancel Then
                        R.Cells(sCurrColName).Value = sOldCellValue
                        oHelper.bDGSError = True
                        Exit Sub
                    End If
                    For i = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
                        R.Cells("Hole" & i).Value = ""
                    Next
                ElseIf sMethod = "Gross".ToUpper Then
                    '20180219-clear out scores from score method
                    If sOldCellValue = "Score" Then
                        If oHelper.iHoleMarker = 1 Then
                            R.Cells("Out_Gross").Value = ""
                            R.Cells("Out_Net").Value = ""
                            '20180219-turn off read only for method=score
                            'R.Cells("Out_Gross").ReadOnly = True
                        Else
                            R.Cells("In_Gross").Value = ""
                            R.Cells("In_Net").Value = ""
                            '20180510-turn off read only for method=score
                            'R.Cells("In_Gross").ReadOnly = False
                        End If
                        R.Cells("Hdcp").Value = ""
                    End If
                    '20180219-clear gross / net score 
                    If oHelper.iHoleMarker = 1 Then
                        R.Cells("Out_Gross").Value = R.Cells("Out_Net").Value
                        If oHelper.convDBNulltoSpaces(R.Cells("Out_Gross").Value).Trim <> "" Then
                            R.Cells("Out_Net").Value = CInt(R.Cells("Out_Gross").Value) - CInt(R.Cells("Phdcp").Value)
                        End If
                    Else
                        R.Cells("In_Gross").Value = R.Cells("In_Net").Value
                        If oHelper.convDBNulltoSpaces(R.Cells("In_Gross").Value).Trim <> "" Then R.Cells("In_Net").Value = CInt(R.Cells("In_Gross").Value) - CInt(R.Cells("Phdcp").Value)
                        R.Cells("In_Net").Value = CInt(R.Cells("In_Gross").Value) - CInt(R.Cells("Phdcp").Value)
                    End If

                ElseIf sMethod = "Net".ToUpper Then
                    '20180219-clear out scores from score method
                    If sOldCellValue = "Score" Then
                        If oHelper.iHoleMarker = 1 Then
                            R.Cells("Out_Gross").Value = ""
                            R.Cells("Out_Net").Value = ""
                            '20180219-turn off read only for method=score
                            R.Cells("Out_Gross").ReadOnly = True
                        Else
                            R.Cells("In_Gross").Value = ""
                            R.Cells("In_Net").Value = ""
                            '20180510-turn off read only for method=score
                            R.Cells("In_Gross").ReadOnly = False
                        End If
                        R.Cells("Hdcp").Value = ""
                    End If
                    '20180219-turn off read only for method=score
                    R.Cells("Out_Gross").ReadOnly = True
                    'if the old method was gross, then recalc net/gross
                    If sOldCellValue.StartsWith("G") Then
                        If oHelper.iHoleMarker = 1 Then
                            R.Cells("Out_Net").Value = R.Cells("Out_Gross").Value
                            If oHelper.convDBNulltoSpaces(R.Cells("Out_Gross").Value).Trim <> "" Then
                                R.Cells("Out_Gross").Value = +CInt(R.Cells("Out_Net").Value) + CInt(R.Cells("Phdcp").Value)
                            End If
                        Else
                            R.Cells("In_Net").Value = R.Cells("In_Gross").Value
                            If oHelper.convDBNulltoSpaces(R.Cells("In_Gross").Value).Trim <> "" Then
                                R.Cells("In_Gross").Value = CInt(R.Cells("In_Net").Value) + CInt(R.Cells("Phdcp").Value)
                            End If
                        End If
                    End If
                Else
                    lbStatus.BackColor = Color.Yellow
                    lbStatus.Text = (String.Format("Score method {0} invalid, changing to default scoring", R.Cells(sCurrColName).Value))
                    oHelper.bDGSError = True
                    If rbGross.Checked Then
                        sMethod = "Gross"
                    ElseIf rbNet.Checked Then
                        sMethod = "Net"
                    End If
                End If
            End If
            R.Cells(sCurrColName).Value = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sMethod.ToLower)

            'oHelper.ChangeColorsForStrokes(R)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub editHoles(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim sScore = oHelper.convDBNulltoSpaces(R.Cells(sCurrColName).Value)
            'set scoring method for each individual score
            'If R.Cells("Method").Value Is DBNull.Value Then Or R.Cells("Method").Value = "" Then
            If oHelper.convDBNulltoSpaces(R.Cells("Method").Value) = " " Then
                If rbGross.Checked Then R.Cells("Method").Value = "Gross"
                If rbNet.Checked Then R.Cells("Method").Value = "Net"
                If rbScore.Checked Then R.Cells("Method").Value = "Score"
            End If
            'if scoring method = score, dont allow hole by hole
            If R.Cells("Method").Value.ToString.StartsWith("S") Then
                MsgBox("Your scoring Method doesnt allow hole by hole scores, change method and re-enter")
                R.Cells(sCurrColName).Value = sOldCellValue
                oHelper.bDGSError = True
                Exit Sub
            End If
            Try
                '20180219-add check for < 0 on #net method
                If Not IsNumeric(sScore) Or (R.Cells("Method").Value <> "Net" And sScore < 1) Then
                    Throw New NotSupportedException
                ElseIf Len(sScore) <> 1 And Len(sScore) <> 9 Then
                    Throw New NotSupportedException
                End If
            Catch ex As Exception
                MsgBox(String.Format("Hole value {0}{1}must be 1-9 and it was {2}, try again", sScore, vbCrLf, sScore.Length))
                'this flag prevents rowleave from being invoked
                'oHelper.bDGSError = True
                R.Cells(sCurrColName).Value = sOldCellValue
                'Exit Sub
            End Try
            '20171003 - allow all scores to be entered as hole 1 
            '20171008 - allow all scores to be entered as hole 10 
            Dim iStableford As Int16 = 0
            If sCurrColName = "Hole1" Or sCurrColName = "Hole10" Then
                If sScore.Length = 9 Then
                    oHelper.bloghelper = True
                    For i = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
                        R.Cells("Hole" & i).Value = CDec(oHelper.ChkForMax(sScore.Substring(i - oHelper.iHoleMarker, 1), "Hole" & i))
                        iStableford += Stableford(CStr(oHelper.ChkForMax(sScore.Substring(i - oHelper.iHoleMarker, 1), "Hole" & i)), i, R.Cells("PHdcp").Value)
                        FCalcLowScore(dgScores, i)
                        If Debugger.IsAttached Then Debug.Print(String.Format("Hole {1} value {0}", R.Cells("Hole" & i).Value, i))
                    Next
                    'Replace column1 or 10 with first digit
                    R.Cells(sCurrColName).Value = oHelper.ChkForMax(sScore.Substring(0, 1), sCurrColName)
                    'iStableford += CInt(Stableford(CInt(sScore.Substring(0, 1)), 9))
                    If Debugger.IsAttached Then Debug.Print(String.Format("Hole {1} value {0}", R.Cells(sCurrColName).Value, 9))
                End If
            End If
            'if all 9 scores are entered, then method must be net or gross
            Dim bgoodScore = True
            For i = oHelper.iHoleMarker + 1 To oHelper.iHoleMarker + 8
                Try
                    Dim ss = R.Cells("Hole" & i).Value
                    R.Cells("Hole" & i).Style.BackColor = Color.White
                Catch ex As Exception
                    bgoodScore = False
                    Exit For
                End Try
            Next
            '20190427-force ihdcp
            oHelper.IHdcp = R.Cells("pHdcp").Value
            If oHelper.iHoleMarker = 1 Then
                R.Cells("Out_Gross").Value = calcScores(R)
            Else
                R.Cells("In_Gross").Value = calcScores(R)
            End If

            If bgoodScore Then
                If R.Cells("Method").Value.ToString.StartsWith("S") Or R.Cells("Method").Value = "" Then R.Cells("Method").Value = oHelper.BuildScoreCardMethods(gbDefMeth)
                If oHelper.iHoleMarker = 1 Then
                    '99 means this is this guys first score
                    If oHelper.IHdcp <> 99 Then
                        R.Cells("Out_Net").Value = R.Cells("Out_Gross").Value - oHelper.IHdcp
                    Else
                        R.Cells("PHdcp").Value = R.Cells("Hdcp").Value
                    End If
                Else
                    If oHelper.IHdcp <> 99 Then
                        R.Cells("In_Net").Value = R.Cells("In_Gross").Value - oHelper.IHdcp
                    Else
                        R.Cells("PHdcp").Value = R.Cells("Hdcp").Value
                    End If
                End If
            End If
            'oHelper.ValidateCell(R.Cells(sCurrColName))
            'R.Cells(sCurrColName).Value = oHelper.ChkForMax(R.Cells(sCurrColName).Value, sCurrColName)

            FCalcLowScore(dgScores, sCurrColName.Substring(4, Len(sCurrColName) - 4))
            iStableford += CInt(Stableford(oHelper.ChkForMax(R.Cells(sCurrColName).Value, "Hole" & oHelper.iHoleMarker), oHelper.iHoleMarker, R.Cells("PHdcp").Value))
            If rbStableford.Checked Then
                R.Cells("Points").Value = iStableford
                'R.Cells("Team_Points").Value = iStableford / 2
            End If
            Dim x = ""
            resetSkins()
            oHelper.CalcMatches(dgScores)
            'calculate match results
            Dim sopp As Int16 = MatchResults(R)
            MatchResults(dgScores.Rows(sopp))

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Function MatchResults(R As DataGridViewRow) As Int16
        Dim saScore As Int16 = 99
        Dim shScore As Int16 = 99
        Dim sfld As String = "Out_Net"
        MatchResults = -1
        If oHelper.iHoleMarker = 10 Then sfld = sfld.Replace("Out", "In")
        If IsNumeric(R.Cells(sfld).Value) Then saScore = R.Cells(sfld).Value
        For Each row As DataGridViewRow In dgScores.Rows
            If row.Cells("Player").Value = R.Cells("Opponent").Value Then
                If IsNumeric(row.Cells(sfld).Value) Then shScore = row.Cells(sfld).Value
                MatchResults = row.Index
                Exit For
            End If
        Next
        If saScore <> 99 Or shScore <> 99 Then
            R.Cells("Match").Value = String.Format("{1} vs {2}-{3}", R.Cells("Player").Value, saScore, R.Cells("Opponent").Value, shScore)
        End If
    End Function
    Sub editTeam(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Try
            'check the team number before leaving a row
            Dim steam As Integer
            steam = Convert.ToInt16(R.Cells("Team").Value)

            If IsNumeric(steam) Then
                If steam > 0 And steam <= oHelper.rLeagueParmrow("Teams") Then
                    Dim icnt = 0
                    For Each row As DataGridViewRow In dgScores.Rows
                        If row.Cells("Team").Value = steam Then
                            icnt += 1
                        End If
                    Next
                    If icnt > 2 Then
                        'MsgBox(String.Format("This Team ({0}) already has 2 players entered, setting back to {1}", R.Cells("Team").Value, sOldCellValue))
                        MsgBox(String.Format("(Team {0}) already has 2 players, setting back to {1}", R.Cells("Team").Value, sOldCellValue))
                        R.Cells("Team").Value = sOldCellValue
                        oHelper.bDGSError = True
                    End If
                Else
                    MsgBox(String.Format("This Team ({0}) must be a number between 1 and {1}", R.Cells("Team").Value, oHelper.rLeagueParmrow("Teams")))
                    R.Cells("Team").Value = ""
                    oHelper.bDGSError = True
                End If
            Else
                Throw New FormatException
            End If

        Catch ex As FormatException
            MsgBox(String.Format("This Team ({0}) must be a number, setting back to {1}", R.Cells("Team").Value, sOldCellValue))
            R.Cells("Team").Value = sOldCellValue
            oHelper.bDGSError = True
        Catch ex As Exception
            'MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub editrest(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If oHelper.bAllHolesEntered Then
                If oHelper.bAllHolesEntered Then
                    R.Cells("Hdcp").Value = oHelper.GetNewHdcp(R, Date.ParseExact(cbDates.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo))
                End If

                ' Dim cell As DataGridViewCell
                '' Set up the ToolTip text for the datagridviewcell.
                'toolTipHdcp.SetToolTip(cell, "My button1")

                Dim sPHdcp = GetPHdcp()
                Try
                    If sPHdcp = "" Then R.Cells("PHdcp").Value = R.Cells("Hdcp").Value
                Catch ex As Exception
                End Try
                calcnetandmatches(R)
                'oHelper.ChangeColorsForStrokes(R)
            End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub calcnetandmatches(R As DataGridViewRow)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim sInOut As String = If(R.Cells("Out_Gross").Visible, "Out", "In")
            R.Cells(sInOut.Replace("Gross", "Net")).Value = R.Cells(sInOut & "_Gross").Value - R.Cells("PHdcp").Value
            If rbStandard.Checked Then
                oHelper.CalcMatches(dgScores)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Sub editGross(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim sScore = ""
        Try
            If dgScores.Columns.Contains("Out_Gross") Then
                sScore = R.Cells("Out_Gross").Value
                If IsNumeric(sScore) And oHelper.bAllHolesEntered Then
                    R.Cells("Out_Net").Value = sScore - R.Cells("PHdcp").Value
                Else
                    Throw New FormatException
                End If
            End If
            If dgScores.Columns.Contains("In_Gross") Then
                sScore = R.Cells("In_Gross").Value
                If IsNumeric(sScore) And oHelper.bAllHolesEntered Then
                    R.Cells("In_Net").Value = sScore - R.Cells("PHdcp").Value
                Else
                    Throw New FormatException
                End If
            End If
            oHelper.bAllHolesEntered = True
            editrest(R, sCurrColName)
        Catch ex As FormatException
            MsgBox(String.Format("This Score ({0}) must be a number, setting back to {1}", sScore, sOldCellValue))
            R.Cells(sCurrColName).Value = sOldCellValue
            oHelper.bDGSError = True
        End Try
    End Sub
#End Region
    Function GetPHdcp() As String
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        GetPHdcp = ""
        Try
            GetPHdcp = ""

            Dim dvscores As New DataView(DsLeague.Tables("dtScores"))
            With dvscores
                '20181014-future try to search internet to find a top row solution
                .RowFilter = String.Format("Player = '{0}' and Date < '{1}'", oHelper.sPlayer, cbDates.SelectedItem)
                .Sort = "Date DESC"
                If .Count > 0 Then GetPHdcp = .Item(0).Item("Hdcp")
            End With
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Function
    Function calcScores(R As DataGridViewRow) As Integer
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        calcScores = 0
        With oHelper
            .bAllHolesEntered = True
            Try

                For i = .iHoleMarker To .iHoleMarker + .iHoles - 1
                    Dim iScore As Int16 = 0
                    If Not IsNumeric(R.Cells("Hole" & i).Value) Then
                        .bAllHolesEntered = False
                        Exit For
                    End If
                    'decrease for net method
                    iScore = R.Cells("Hole" & i).Value
                    If R.Cells("Method").Value.ToString.StartsWith("N") Then
                        Dim isi = .CalcStrokeIndex("Hole" & i)
                        If .IHdcp >= isi Then
                            'check stroke index 
                            iScore += 1
                            'check for 2 strokes
                            If .IHdcp - .iHoles >= isi Then iScore += 1
                        End If
                    End If
                    calcScores += iScore
                Next

            Catch ex As Exception
                MsgBox(oHelper.GetExceptionInfo(ex))
            End Try
        End With
    End Function
    Sub resetSkins()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            xlskins = FCalcSkins(dgScores)
            Debug.Print("")
            lskins = New List(Of String)
            For Each indskin In xlskins
                If Not indskin.Contains("|") Then
                    lskins.Add(indskin)
                End If
            Next
            Dim deachskin As Decimal = 0

            If lskins.Count > 0 Then deachskin = Math.Floor(tbSkinTot.Text / lskins.Count)
            tbNumSkins.Text = lskins.Count
            tbEachSkin.Text = deachskin
            tbKitty.Text = CInt(tbSkinTot.Text) - (lskins.Count * deachskin)
            'clear out skins info
            For Each row As DataGridViewRow In dgScores.Rows
                row.Cells(skinnum).Value = DBNull.Value
                row.Cells(skinamt).Value = DBNull.Value
                row.Cells(earned).Value = DBNull.Value
            Next
            'add to skins dollars
            For Each row As DataGridViewRow In dgScores.Rows
                For Each indskin In lskins
                    If indskin.Split("-")(1) = row.Index Then
                        'row.Cells(skinnum).Value = NewmakeCellAmt(row.Cells(skinnum)) + 1
                        row.Cells(skinnum).Value = makeCellAmt(row.Cells(skinnum)) + 1
                        row.Cells(skinamt).Value = makeCellAmt(row.Cells(skinnum)) * CInt(tbEachSkin.Text)
                    End If
                Next
            Next
            For Each row As DataGridViewRow In dgScores.Rows
                Dim iamt As Integer = makeCellAmt(row.Cells(skinamt)) + makeCellAmt(row.Cells(CTP1)) + makeCellAmt(row.Cells(CTP2))
                If iamt > 0 Then
                    row.Cells(earned).Value = iamt
                Else
                    row.Cells(earned).Value = DBNull.Value
                End If
            Next

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Sub calcColWidth(col As DataGridViewColumn)
        Try

            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            'set default length
            col.Width = 50
            'calc max length of each field
            Dim slen As Int16 = 0
            For Each row As DataRowView In DtScoresBindingSource.List
                If row(col.DataPropertyName).ToString.Length > slen Then slen = row(col.DataPropertyName).ToString.Length
            Next
            '  nulls make this croak Dim maxColumn1Length = dvsscores.AsEnumerable().Max(Function(row) row.Field(Of String)(col.DataPropertyName).Length)
            Dim text1 As String = ""
            If UBound(col.HeaderText.Split(" ")) > 0 Then
                Dim iwordlen As Int16 = 0
                For Each word As String In col.HeaderText.Split(" ")
                    If word.Length > iwordlen Then
                        iwordlen = word.Length
                        Continue For
                    End If
                Next
                If iwordlen > slen Then slen = iwordlen
            Else
                If col.HeaderText.Length > slen Then slen = col.HeaderText.Length
            End If
            text1 = "X".PadRight(slen + 1, "X")

            Dim arialBold As New Font("Tahoma", 9)
            Dim textSize As Size = TextRenderer.MeasureText(text1, arialBold)
            Dim sizef As SizeF
            Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
                sizef = g.MeasureString(text1, arialBold)
            End Using
            col.Width = sizef.Width
            If col.Name.Contains("Gross") Or col.Name.Contains("Net") Then
                col.Width = 35
            End If
            oHelper.LOGIT(String.Format("{0}-{1}", col.Name, col.Width))
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub calcCellWidth(cell As DataGridViewCell)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim textSize As Size = TextRenderer.MeasureText(cell.Value, cell.DataGridView.CurrentCell.Style.Font)
        Dim iLargestWidth As Int16 = 0 'dgScores.Columns(cell.OwningColumn.Name).Width
        For Each row As DataGridViewRow In dgScores.Rows
            Dim xx = TextRenderer.MeasureText(row.Cells(cell.OwningColumn.Name).Value, cell.DataGridView.CurrentCell.Style.Font).Width
            If xx > iLargestWidth Then iLargestWidth = xx
        Next
        'If cell.DataGridView.CurrentCell.Style.Font.Size > 12 Then
        If textSize.Width >= iLargestWidth Then
            dgScores.Columns(cell.OwningColumn.Name).Width = textSize.Width + 1
        Else
            dgScores.Columns(cell.OwningColumn.Name).Width = iLargestWidth
        End If
        'dgScores.CurrentRow.Height = sizef.Height

    End Sub
    Function FCalcLowScore(dgScores As DataGridView, hole As String) As String
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        FCalcLowScore = ""
        Try
            Dim ilowrow As New List(Of String)
            Dim ilowscore = 99
            'calculate a column saving low score
            For i = 0 To dgScores.RowCount - 1
                Dim sGross As Object = Nothing
                Dim sNet As Object = Nothing
                If dgScores.Rows(i).Cells("Player").Value = oHelper.sTotalColumn Then Continue For
                oHelper.sPlayer = dgScores.Rows(i).Cells("Player").Value
                If IsNumeric(dgScores.Rows(i).Cells("PHdcp").Value) Then
                    oHelper.IHdcp = dgScores.Rows(i).Cells("PHdcp").Value
                Else
                    oHelper.IHdcp = sGross - sNet
                    dgScores.Rows(i).Cells("PHdcp").Value = oHelper.IHdcp
                End If

                Dim binskins As String = "N"
                If dgScores.Rows(i).Cells(skin).FormattedValueType.Name = "Boolean" Then
                    If dgScores.Rows(i).Cells(skin).Value = True Then binskins = "Y"
                Else
                    binskins = dgScores.Rows(i).Cells(skin).Value
                End If

                If binskins = "Y" Then
                    'If dgScores.Rows(i).Cells("Hole" & hole).Value IsNot DBNull.Value Then
                    If IsNumeric(dgScores.Rows(i).Cells("Hole" & hole).Value) Then
                        Dim iscore As String = dgScores.Rows(i).Cells("Hole" & hole).Value
                        'this means its a scorecard
                        '2020-01-15- 20180529 Ben Wright played 1 hole
                        If dgScores.Columns.Contains("Method") Then
                            If dgScores.Rows(i).Cells("Method").Value = "Gross" And oHelper.rLeagueParmrow("SkinFmt") = "Handicap" Then
                                Dim isi As Int16 = oHelper.CalcStrokeIndex(hole)
                                If CInt(dgScores.Rows(i).Cells("pHdcp").Value) >= isi Then
                                    'check stroke index
                                    iscore -= 1
                                    If CInt(dgScores.Rows(i).Cells("pHdcp").Value - oHelper.iHoles) >= isi Then iscore -= 1
                                End If
                            End If
                        End If

                        If IsNumeric(iscore) Then
                            If iscore < ilowscore Then
                                ilowscore = iscore
                                ilowrow = New List(Of String)
                                ilowrow.Add(i)
                            ElseIf iscore = ilowscore Then
                                ilowrow.Add(i)
                            End If
                        End If
                    End If
                End If
            Next
            For Each score In ilowrow
                FCalcLowScore &= score & "|"
            Next
            FCalcLowScore = FCalcLowScore.TrimEnd("|")
            'If ilowrow.Count = 1 Then FCalcLowScore = ilowrow(0)

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Function FCalcSkins(dgScores As DataGridView) As List(Of String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'this code goes through the listview and highlights the lowest value on each hole and fron 9, back 9 and total
        FCalcSkins = New List(Of String)
        Try
            For ii = oHelper.iHoleMarker To oHelper.iHoleMarker + oHelper.iHoles - 1
                Dim ilowscore As String = FCalcLowScore(dgScores, ii)
                If ilowscore <> "" Then FCalcSkins.Add(ii & "-" & ilowscore)
            Next
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function
    Sub setSkinsCTPs(row As DataRowView, fld As String, irow As Integer)
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If row(fld) IsNot DBNull.Value Then
                If row(fld) = "Y" Then 'Or row(fld).ToString.ToUpper = "TRUE" Then
                    dgScores.Rows(irow).Cells(fld).Value = True
                    dgScores.Rows(irow).Cells(fld).Style.BackColor = Color.LightBlue
                Else

                End If
            Else
                dgScores.Rows(irow).Cells(fld).Value = False
                dgScores.Rows(irow).Cells(fld).Style.BackColor = Color.White
            End If
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Try
            If Not bFormLoad Then
                If MessageBox.Show(String.Format("Do you want to re-load scores from {0}?", cbDates.SelectedItem), "Scores will be Lost", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand) = DialogResult.OK Then
                    'save the scores first
                    btnSave_Click(sender, e)
                    'oHelper.WaitForFile(DsLeague.Tables("dtScores"), Main.WaitForFile("Scores"), lbStatus, Me)
                End If
                'set helper date
                If cbDates.SelectedItem Is Nothing Then
                    oHelper.dDate = Date.ParseExact(cbDates.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                Else
                    oHelper.dDate = Date.ParseExact(cbDates.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                End If
            Else
                bFormLoad = True
                cbDates.SelectedItem = Main.cbDates.SelectedItem
            End If

            oHelper.LOGIT(String.Format("Selected Date {0}", cbDates.SelectedItem))

            'check to see if this week overlaps with club championship
            oHelper.LOGIT(String.Format("check to see if this week overlaps with club championship"))
            If cbDates.SelectedItem >= CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") And
               cbDates.SelectedItem <= CDate(oHelper.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
                oHelper.bCCLeague = True
            Else
                oHelper.bCCLeague = False
            End If

            oHelper.LOGIT(String.Format("Binding Datasource", cbDates.SelectedItem))
            Dim dv As New DataView(DsLeague.Tables("dtScores"))
            dv.RowFilter = String.Format("Date = '{0}'", cbDates.SelectedItem)
            're-sort grid by low net if post season
            If cbDates.SelectedItem >= CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                dv.Sort = If(oHelper.iHoleMarker = 1, "Method DESC, Out_Net ASC", "Method DESC, In_Net ASC")
            Else
                dv.Sort = "Partner"
            End If
            For Each row As DataRowView In dv
                If IsNumeric(row(CTP1)) Then
                    If row(CTP1) = 0 Then row(CTP1) = DBNull.Value
                End If
                If IsNumeric(row(CTP2)) Then
                    If row(CTP2) = 0 Then row(CTP2) = DBNull.Value
                End If
            Next
            Dim BindingSource = New BindingSource()
            BindingSource.DataSource = dv
            DtScoresBindingSource.DataSource = BindingSource
            'calculate holemarker
            oHelper.CalcThisHoleMarker(cbDates.SelectedItem, dv)

            'get the matches for today
            oHelper.LOGIT(String.Format("Getting Matches for Today {0}", cbDates.SelectedItem))
            If cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                oHelper.getMatchScores(cbDates.SelectedItem)
            End If
#Region "Change Columns"
            'change column characteristics
            oHelper.LOGIT(String.Format("Make Col name = DataPropertyName,Skins/Closest boolean"))
            'recreated ctps for checkboxes for winners as checkboxes if not set yet
            oHelper.LOGIT(String.Format("reset the ctps back to textboxes first"))
            Dim sColumn As DataGridViewColumn
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP1).SingleOrDefault
            recreateCheckBoxesasTextBoxes(sColumn.DataPropertyName, sColumn.Index)
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP2).SingleOrDefault
            recreateCheckBoxesasTextBoxes(sColumn.DataPropertyName, sColumn.Index)

            oHelper.LOGIT(String.Format("recreate the skins/ctp as checkboxes"))
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = skin).SingleOrDefault
            recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = closest).SingleOrDefault
            recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)

            'add a clear scores checkbox on if it already hasnt been built
            oHelper.LOGIT(String.Format("add a clear scores checkbox on if it already hasnt been built"))
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = Clear).SingleOrDefault
            If sColumn Is Nothing Then
                'Dim cbClearScores As New DataGridViewCheckBoxColumn
                Dim cbClearScores As New DataGridViewButtonColumn
                With cbClearScores
                    .HeaderText = Clear
                    .Name = Clear
                    .DataPropertyName = Clear
                    .Width = 50
                    .Text = Clear
                    .UseColumnTextForButtonValue = True
                    dgScores.Columns.Insert(0, cbClearScores)
                End With
            End If
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Match").SingleOrDefault
            If cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                If sColumn Is Nothing Then
                    '2020-01-14 - find points and put next to it
                    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Points").SingleOrDefault
                    Dim tb As New DataGridViewTextBoxColumn
                    With tb
                        .HeaderText = "Match Results"
                        .Name = "Match"
                        .DataPropertyName = "Match"
                        .Width = 150
                        .ReadOnly = True
                        dgScores.Columns.Insert(sColumn.Index, tb)
                    End With
                End If
            Else
                sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Match").SingleOrDefault
                If sColumn IsNot Nothing Then
                    dgScores.Columns.RemoveAt(sColumn.Index)
                End If
                '2020-01-14 - find points and put next to it
                sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "LCT").SingleOrDefault
                If sColumn Is Nothing Then
                    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "PHdcp").SingleOrDefault
                    Dim tb As New DataGridViewTextBoxColumn
                    With tb
                        .HeaderText = "Leag Champ Total"
                        .Name = "LCT"
                        .DataPropertyName = "LCT"
                        .Width = 40
                        .ReadOnly = True
                        dgScores.Columns.Insert(sColumn.Index, tb)
                    End With
                End If
            End If
            ChangeColAttributes()
            'reorganize Columns to fit date/parameters(like front 9 and back nine)
            For Each col As DataGridViewColumn In dgHandicap.Columns
                col.Width = 20
            Next

#End Region

            'set front/back radio(set flag so wr dont reassign holes)
            bSkipFB = True
            If oHelper.iHoleMarker = 1 Then
                rbFront.Checked = True
            Else
                rbBack.Checked = True
            End If
            bSkipFB = False

#Region "Calculate CTPs/Skins"
            oHelper.LOGIT(String.Format("Calc # CTPS"))
            cbMarkPaid.Checked = False
            'oHelper.calcTeamsPoints()
            Dim sKeys() As Object = {cbDates.SelectedItem}
            thisweeksSkinsCTPS = oHelper.dtnewWklySkins.Rows.Find(sKeys)
            'this is a new week if this is nothing
            If thisweeksSkinsCTPS Is Nothing Then
                thisweeksSkinsCTPS = oHelper.dtnewWklySkins.NewRow
            End If
            'clear this weeks totals, they will be recalculated from datagridview
            For Each fld As String In {skinscol, skinsearn, skinsextr, ctpf1col, ctpf1earn, ctpf2col, ctpf2earn, ctpb1col, ctpb1earn, ctpb2col, ctpb2earn}
                thisweeksSkinsCTPS(fld) = 0
            Next
            If cbDates.Items.Count > 1 And cbDates.SelectedIndex < cbDates.Items.Count - 1 Then
                sKeys = {cbDates.Items(cbDates.SelectedIndex + 1)}
                With oHelper
                    lastweeksSkinsCTPS = .dtnewWklySkins.Rows.Find(sKeys)
                    tbCP1.Text = thisweeksSkinsCTPS(ctpf1col) + thisweeksSkinsCTPS(ctpb1col)
                    tbCP2.Text = thisweeksSkinsCTPS(ctpf2col) + thisweeksSkinsCTPS(ctpb2col)
                    tbPCP1.Text = lastweeksSkinsCTPS(ctpf1extr) + lastweeksSkinsCTPS(ctpb1extr)
                    tbPCP2.Text = lastweeksSkinsCTPS(ctpf2extr) + lastweeksSkinsCTPS(ctpb2extr)
                    tbPSkins.Text = lastweeksSkinsCTPS(skinsextr)
                    tbCP1Tot.Text = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
                    tbCP2Tot.Text = CDec(tbPCP2.Text) + CDec(tbCP2.Text)

                    tbSkins.Text = thisweeksSkinsCTPS(skinscol)
                    tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
                    tbNumSkins.Text = thisweeksSkinsCTPS(skinsextr)
                    tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)
                    tbKitty.Text = 0
                End With
                'Else
                'lastweeksSkinsCTPS = thisweeksSkinsCTPS
                'For Each col As DataColumn In oHelper.dtnewWklySkins.Columns
                '    If col.ColumnName <> "Date" Then
                '        lastweeksSkinsCTPS(col.ColumnName) = 0
                '    End If
                'Next
            Else
                'this is the first week of the year
                'thisweeksSkinsCTPS = oHelper.dtnewWklySkins.NewRow
                tbPCP1.Text = 0
                tbPCP2.Text = 0
                tbPSkins.Text = 0
            End If
            oHelper.LOGIT(String.Format("Date:  {0} Leftover Skins {1}", oHelper.dDate, oHelper.dThisWeeksSkins))

#End Region
            'recreate the skins/ctp as checkboxes
#Region "Make all checkboxes boolean"
            Dim iRow = 0
            Dim ictp1 = 0, ictp2 = 0
            For Each row As DataRowView In DtScoresBindingSource
                setSkinsCTPs(row, skin, iRow)
                setSkinsCTPs(row, closest, iRow)
                iRow += 1
                If IsNumeric(row(CTP1)) Then
                    ictp1 += 1
                End If
                If IsNumeric(row(CTP2)) Then
                    ictp2 += 1
                End If
            Next
            If ictp1 > 1 Then
                MessageBox.Show("CTP1 Error, please fix", "CTP Error", MessageBoxButtons.OK)
            End If
            If ictp2 > 1 Then
                MessageBox.Show("CTP2 Error, please fix", "CTP Error", MessageBoxButtons.OK)
            End If

#End Region
#Region "Set checkboxes, Colors, previous handicap"
            oHelper.LOGIT(String.Format("Set checkboxes, Colors, previous handicap"))
            'loop through scores 
            ' setting Skins / ctp checkboxes instead of y/n
            ' coloring holes for strokes
            ' recalculating net scores to be gross
            'shade every other match light blue
            Dim ishade = 4, bbl = False

            For Each row As DataGridViewRow In dgScores.Rows
                'row.Cells("Clear").Value = False
                'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Player").SingleOrDefault
                oHelper.sPlayer = row.Cells("Player").Value
                '2020-01-16-add total column for league championship total
                sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "LCT").SingleOrDefault
                'if first week make total this week, else add last weeks
                If sColumn IsNot Nothing Then
                    Dim iLastWeeksScore As Integer = 0
                    Dim arow As DataRow = Nothing

                    If cbDates.SelectedItem > CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                        Dim sccKeys() As Object = {row.Cells("Player").Value, cbDates.Items(cbDates.SelectedIndex + 1)}
                        arow = DsLeague.Tables("dtScores").Rows.Find(sccKeys)
                        If arow IsNot Nothing Then
                            If String.IsNullOrWhiteSpace(TryCast(arow("In_Net"), String)) And String.IsNullOrWhiteSpace(TryCast(arow("Out_Net"), String)) Then
                                iLastWeeksScore = 0
                            Else
                                iLastWeeksScore = If(oHelper.iHoleMarker = 1, CInt(arow("In_Net")), CInt(arow("Out_Net")))
                            End If
                        Else
                            iLastWeeksScore = 0
                        End If
                    End If

                    If String.IsNullOrWhiteSpace(TryCast(row.Cells("Out_Net").Value, String)) Or String.IsNullOrWhiteSpace(TryCast(row.Cells("In_Net").Value, String)) Then
                        row.Cells(sColumn.Name).Value = If(oHelper.iHoleMarker = 1, row.Cells("Out_Net").Value, row.Cells("In_Net").Value)
                    End If
                    'this accounts for players who are in skins but not league champ
                    If Not String.IsNullOrWhiteSpace(TryCast(row.Cells(sColumn.Name).Value, String)) Then
                        If cbDates.SelectedItem > CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") And iLastWeeksScore = 0 Then
                            row.Cells(sColumn.Name).Value = ""
                        Else
                            row.Cells(sColumn.Name).Value += iLastWeeksScore
                        End If
                    End If
                End If
                'row.Cells("Partner").Value = row.Cells(sColumn.Name).Value
                'End If
                ishade -= 1
                'oHelper.ChangeColorsForStrokes(row)
                'pull last score before this to get previous handicap
                '(String.Format({0}, ""))
                Dim dv2Scores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & cbDates.SelectedItem & "'"
                dv2Scores.Sort = " Date Desc"
                If dv2Scores.Count > 0 Then row.Cells("Phdcp").Value = dv2Scores(0).Item("Hdcp").ToString
                If row.Cells("Method").Value.ToString = " Net" Then
                    If oHelper.iHoleMarker = 1 Then
                        row.Cells("Out_Gross").Value = CInt(row.Cells("Out_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    Else
                        row.Cells("In_Gross").Value = CInt(row.Cells("In_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    End If
                End If
                Dim xx = row.Cells("Hole1").Value
                Dim xxx = row.Cells("Hole10").Value
                oHelper.LOGIT(String.Format("GetNewHdcp {0}", row.Cells("Player").Value.ToString.ToLower))
                row.Cells("Hdcp").Value = oHelper.GetNewHdcp(row, oHelper.dDate)
                'calculate match results
                If cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                    Dim sopp As Int16 = MatchResults(row)
                    If sopp > 0 Then
                        MatchResults(dgScores.Rows(sopp))
                    End If
                End If
                'set default of skins ctps based on player file
                Dim foundrows = DsLeague.dtPlayers.Select(String.Format("Name = '{0}'", oHelper.sPlayer))
                If foundrows IsNot Nothing Then
                    If Not IsNumeric(foundrows(0)("Team")) Then
                        row.Cells("Player").Style.BackColor = Color.Aqua
                    End If
                End If
                If oHelper.RemoveNulls(foundrows(0)(skin)) = "Y" Then
                    If row.Cells(skin) Is DBNull.Value Then row.Cells(skin).Value = True
                End If
                If oHelper.RemoveNulls(foundrows(0)("CTP")) = "Y" Then
                    If row.Cells(closest) Is DBNull.Value Then row.Cells(closest).Value = True
                End If
                'Dim sgross As Int16 = 0
                If oHelper.iHoleMarker = 1 Then
                    'https://stackoverflow.com/questions/8255186/how-to-check-empty-and-null-cells-in-datagridview-using-c-sharp
                    'If String.IsNullOrWhiteSpace(TryCast(row.Cells("Out_Gross").Value, String)) Then
                    If Not IsNumeric(row.Cells("Out_Gross").Value) Then
                        row.Cells("Player").Style.BackColor = Color.Pink
                    End If
                Else
                    'If String.IsNullOrWhiteSpace(TryCast(row.Cells("In_Gross").Value, String)) Then
                    If Not IsNumeric(row.Cells("In_Gross").Value) Then
                        row.Cells("Player").Style.BackColor = Color.Pink
                    End If
                End If
            Next
#End Region

#Region "Calculate this weeks CTP/Skins from Gridview"
            oHelper.LOGIT(String.Format("Calculate this weeks skins/ctps from Gridview"))

            For Each row As DataGridViewRow In dgScores.Rows
                oHelper.sPlayer = row.Cells("Player").Value
                If row.Cells(closest).Value = True Then
                    Dim damt As Decimal = If(cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), 1, "3")
                    If oHelper.iHoleMarker = 1 Then
                        thisweeksSkinsCTPS(ctpf1col) += damt / 2
                        thisweeksSkinsCTPS(ctpf2col) += damt / 2
                        thisweeksSkinsCTPS(ctpf1earn) += makeCellAmt(row.Cells(CTP1))
                        thisweeksSkinsCTPS(ctpf2earn) += makeCellAmt(row.Cells(CTP2))
                        thisweeksSkinsCTPS(ctpb1col) = 0
                        thisweeksSkinsCTPS(ctpb2col) = 0
                        thisweeksSkinsCTPS(ctpb1earn) = 0
                        thisweeksSkinsCTPS(ctpb2earn) = 0
                    Else
                        thisweeksSkinsCTPS(ctpf1col) = 0
                        thisweeksSkinsCTPS(ctpf2col) = 0
                        thisweeksSkinsCTPS(ctpf1earn) = 0
                        thisweeksSkinsCTPS(ctpf2earn) = 0
                        thisweeksSkinsCTPS(ctpb1col) += damt / 2
                        thisweeksSkinsCTPS(ctpb2col) += damt / 2
                        thisweeksSkinsCTPS(ctpb1earn) += makeCellAmt(row.Cells(CTP1))
                        thisweeksSkinsCTPS(ctpb2earn) += makeCellAmt(row.Cells(CTP2))
                    End If
                End If
                If row.Cells(skin).Value = True Then
                    Dim damt As Decimal = If(cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), 2, "7")
                    thisweeksSkinsCTPS(skinscol) += damt
                    thisweeksSkinsCTPS(skinsearn) += makeCellAmt(row.Cells(skinamt))
                End If
            Next

#End Region
            FormatCTP()

            '2020-01-14
            'If thisweeksSkinsCTPS(ctpf1earn) + thisweeksSkinsCTPS(ctpf2earn) + thisweeksSkinsCTPS(ctpb1earn) + thisweeksSkinsCTPS(ctpb2earn) > 0 Then
            '    dgScores.Columns(oHelper.closest).ReadOnly = True
            'Else
            '    dgScores.Columns(oHelper.closest).ReadOnly = False
            'End If

            If thisweeksSkinsCTPS(ctpf1earn) + thisweeksSkinsCTPS(ctpf2earn) + thisweeksSkinsCTPS(ctpb1earn) + thisweeksSkinsCTPS(ctpb2earn) > 0 Then
                dgScores.Columns(closest).Visible = False
            Else
                dgScores.Columns(closest).Visible = True
            End If

            oHelper.LOGIT(String.Format("Calculate Matches"))
            oHelper.CalcMatches(dgScores)

            If cbDates.SelectedIndex > 0 Then
                sKeys = {cbDates.SelectedIndex - 1}
            End If
            With oHelper
                tbCP1.Text = thisweeksSkinsCTPS(ctpf1col) + thisweeksSkinsCTPS(ctpb1col)
                tbCP2.Text = thisweeksSkinsCTPS(ctpf2col) + thisweeksSkinsCTPS(ctpb2col)
                tbCP1Tot.Text = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
                tbCP2Tot.Text = CDec(tbPCP2.Text) + CDec(tbCP2.Text)

                tbSkins.Text = thisweeksSkinsCTPS(skinscol)
                tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
                tbNumSkins.Text = IIf(thisweeksSkinsCTPS(skinsextr) Is DBNull.Value, 0, thisweeksSkinsCTPS(skinsextr))
                tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)
                tbKitty.Text = 0
            End With
            oHelper.LOGIT(String.Format("Reset Skins"))
            resetSkins()

            oHelper.LOGIT(String.Format("Date:  {0} Leftover Skins {1}", oHelper.dDate, tbSkins.Text))

            'oHelper.LOGIT(String.Format("{0}-{1}", dgScores.Rows(0).Cells("Player").Value, dgScores.Rows(0).Cells("Hdcp").Value))

            bFormLoad = False
            'oHelper.LOGIT(String.Format("exiting {0}-{1}", dgScores.Rows(0).Cells("Player").Value, dgScores.Rows(0).Cells("Hdcp").Value))
            oHelper.LOGIT(String.Format("Resize dgScores"))
            oHelper.Resizedgv(dgScores, Me)

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
            ''MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub FormatCTP()
        oHelper.LOGIT(String.Format("recreate the skins/ctp non-winners as checkboxes"))
        Dim sColumn As DataGridViewColumn
        If oHelper.iHoleMarker = 1 Then
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP1).SingleOrDefault
            If thisweeksSkinsCTPS(ctpf1earn) = 0 Then
                recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
                sColumn.ReadOnly = False
            Else
                sColumn.ReadOnly = True
            End If

            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP2).SingleOrDefault
            If thisweeksSkinsCTPS(ctpf2earn) = 0 Then
                recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
                sColumn.ReadOnly = False
            Else
                sColumn.ReadOnly = True
            End If
        End If

        sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP1).SingleOrDefault
        If oHelper.iHoleMarker = 10 Then
            If thisweeksSkinsCTPS(ctpb1earn) = 0 Then
                recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
                sColumn.ReadOnly = False
            Else
                sColumn.ReadOnly = True
            End If
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP2).SingleOrDefault
            If thisweeksSkinsCTPS(ctpb2earn) = 0 Then
                recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
                sColumn.ReadOnly = False
            Else
                sColumn.ReadOnly = True
            End If
        End If
        '20200109-change header text to contain hole number
        Dim ii = 0
        For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
            If oHelper.thisCourse("Hole" & i) = 3 Then
                dgScores.Columns("CTP_" & ii + 1).HeaderText = String.Format("CTP# {0}", i)
                ii += 1
            End If
        Next

    End Sub
#Region "change attributes depending on scoring, and hide irrelevant columns(like points/opponents on league championship nights)"
    Sub ChangeColAttributes()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try

            oHelper.LOGIT(String.Format("change attributes depending on scoring, and hide irrelevant columns(like points/opponents on league championship nights)"))
            For Each col As DataGridViewColumn In dgScores.Columns
                'oHelper.LOGIT(String.Format("col {0} x {1} y {2}", sColumn.DataPropertyName, sColumn.HeaderCell.ContentBounds.X, sColumn.HeaderCell.ContentBounds.Y))
                With col
                    'make column name = property name
                    .Name = .DataPropertyName
                    .SortMode = DataGridViewColumnSortMode.NotSortable
                    If .Name = skin Or .Name = closest Then
                        .ValueType = GetType(System.Boolean)
                    End If
                End With
                oHelper.LOGIT(String.Format("change attributes for col {0}", col.Name))

                'change attributes depending on scoring, and hide irrelevant columns(like points/opponents on league championship nights)
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter
                If col.Name.Contains("Hole") Then
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                    If oHelper.iHoles < 18 Then
                        If UBound(col.HeaderText.Split(" ")) > 0 Then
                            col.HeaderText = col.HeaderText.Split(" ")(0)
                        End If
                        If col.HeaderText >= oHelper.iHoleMarker + 9 Or col.HeaderText < oHelper.iHoleMarker Then
                            col.Visible = False
                            Continue For
                        Else
                            col.Visible = True
                        End If
                    End If
                    'this makes the hole square
                    col.Width = dgScores.RowTemplate.Height
                    col.ValueType = GetType(System.String)
                    col.HeaderText &= (String.Format(" {0}", oHelper.CalcStrokeIndex(col.HeaderText)))
                    'col.HeaderText &= "<br />" & "Hole"
                ElseIf col.Name = "Out" Or col.Name = "In" Or col.Name.Contains("18") Then
                    If oHelper.iHoles <> 18 Then
                        col.Visible = False
                        Continue For
                    End If
                ElseIf col.Name.Contains("_Gross") Or col.Name.Contains("_Net") Then
                    If oHelper.iHoles <> 18 Then
                        If oHelper.iHoleMarker = 1 Then
                            If col.Name.Contains("Out") Then
                                col.Visible = True
                            Else
                                col.Visible = False
                                Continue For
                            End If
                        Else
                            If col.Name.Contains("In") Then
                                col.Visible = True
                            Else
                                col.Visible = False
                                Continue For
                            End If
                        End If
                        col.HeaderText.Replace("Out ", "").Replace("In ", "")
                    End If
                ElseIf col.Name.Contains("Points") Or col.Name = "Opponent" Or col.Name = "Team" Or col.Name = "Grade" Or col.Name = "Hdcp" Then
                    If oHelper.CDateToyyyyMMdd(oHelper.dDate) >= oHelper.CDateToyyyyMMdd(oHelper.rLeagueParmrow("PostSeasonDt")) Then
                        col.Visible = False
                        lblMatchWon.Visible = False
                        lblMatchTied.Visible = False
                        Continue For
                    Else
                        col.Visible = True
                        lblMatchWon.Visible = True
                        lblMatchTied.Visible = True
                    End If
                ElseIf col.Name = Clear Or col.Name = "Match" Or col.Name = "LCT" Then
                    Continue For
                End If
                'dont adjust match column
                If col.Name <> "Match" Then
                    If Not col.Name.StartsWith("Hole") Then
                        calcColWidth(col)
                    End If
                End If

                If col.Name = "Player" Then
                    If cbDates.SelectedItem >= CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                        col.ReadOnly = True
                    Else
                        col.ReadOnly = False
                    End If
                End If
            Next

            '2020-01-14-change colors of headertext depending on match control, skins/ctp control
            'https://stackoverflow.com/questions/21545330/datgridview-header-cells-background-color
            dgScores.EnableHeadersVisualStyles = False
            Dim sflds As Object = {CTP1, CTP2}
            For Each fld As String In sflds
                dgScores.Columns(fld).HeaderCell.Style.BackColor = Color.Cyan
            Next
            If cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                sflds = {"Opponent", points, teampoints, "Match"}
                For Each fld As String In sflds
                    dgScores.Columns(fld).HeaderCell.Style.BackColor = Color.LemonChiffon
                Next
            End If
            sflds = {earned, skinamt, closestamt, skinnum}
            For Each fld As String In sflds
                dgScores.Columns(fld).HeaderCell.Style.BackColor = Color.LavenderBlush
            Next
            'loop through scores 
            ' setting Skins / ctp checkboxes instead of y/n
            ' coloring holes for strokes
            ' recalculating net scores to be gross
            'shade every other match light blue
            If cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                For i = 0 To dgScores.Rows.Count - 1 Step 4
                    If i Mod 8 = 0 Then
                        For ii = 0 To 3
                            dgScores.Rows(i + ii).DefaultCellStyle.BackColor = Color.LightBlue
                        Next
                    End If
                Next
            End If

#End Region
            'change attributes depending on scoring, and hide irrelevant columns(like points/opponents on league championship nights)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub rbStandard_CheckedChanged(sender As Object, e As EventArgs) Handles rbStandard.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If Not bFormLoad Then
                RemoveHandler rbStandard.CheckedChanged, AddressOf rbStandard_CheckedChanged
                rbStandard.Checked = True
                AddHandler rbStandard.CheckedChanged, AddressOf rbStandard_CheckedChanged
                MessageBox.Show("Cant switch this Scoring Method yet, its still in developement", "Scoring Format Change", MessageBoxButtons.OK)
                Exit Sub

                Dim shole = "Hole1"
                If rbFront.Checked Then shole &= "0"
                Dim dvscores As New DataView(DsLeague.Tables("dtScores"))
                With dvscores
                    .RowFilter = String.Format("Date = '{0}' and {1} > 0", cbDates.SelectedItem, shole)
                    .Sort = "Date DESC"
                End With
                'Switch from standard to stableford
                Dim ssf As String = rbStableford.Text
                Dim sst As String = rbStandard.Text
                If rbStandard.Checked Then
                    ssf = rbStandard.Text
                    sst = rbStableford.Text
                End If
                Dim mbr = MsgBox(String.Format("Switch to {1} from {0}?", ssf, sst), MsgBoxStyle.YesNo)
                If mbr = MsgBoxResult.No Then
                    RemoveHandler rbStandard.CheckedChanged, AddressOf rbStandard_CheckedChanged

                    If rbStandard.Checked Then
                        rbStandard.Checked = False
                        rbStableford.Checked = True
                    Else
                        rbStableford.Checked = False
                        rbStandard.Checked = True
                    End If
                    AddHandler rbStandard.CheckedChanged,
                AddressOf rbStandard_CheckedChanged
                    Exit Sub
                End If

                lbStatus.Text = String.Format("Switching to {1} from {0}?", ssf, sst)
                oHelper.status_Msg(lbStatus, Me)

                If ssf = rbStableford.Text Then
                    dgScores.Columns.Remove("Opponent")
                    dgScores.Columns.Remove("Team_Points")
                Else
                    oHelper.AddColumnToDGV(dgScores, "Team_Points", 0, 50)
                    dgScores.Columns("Team_Points").HeaderText = "Team Points"
                    oHelper.AddColumnToDGV(dgScores, "Opponent", 0, 170)
                    oHelper.CalcMatches(dgScores)
                End If
                For Each row As DataGridViewRow In dgScores.Rows
                    oHelper.sPlayer = row.Cells("Player").Value
                    If ssf = rbStandard.Text Then
                        Dim sKeys() As Object = {row.Cells("Player").Value, cbDates.SelectedItem} 'oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
                        Dim arow As DataRow = DsLeague.Tables("dtScores").Rows.Find(sKeys)
                        row.Cells("Points").Value = arow("Points")
                        row.Cells("Team_Points").Value = arow("Team_Points")
                        row.Cells("Opponent").Value = arow("Opponent")
                        'oHelper.ChangeColorsForStrokes(row)
                    Else
                        row.Cells("Points").Style.BackColor = Color.White
                        'row.Cells("Team_Points").Style.BackColor = Color.White
                        Dim iStableford As Int16 = 0
                        For i = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
                            If Not IsNumeric(oHelper.convDBNulltoSpaces(oHelper.RemoveSpcChar(row.Cells("Hole" & i).Value))) Then Continue For
                            If Debugger.IsAttached Then Debug.Print(String.Format("Hole {1} value {0}", row.Cells("Hole" & i).Value, i))
                            iStableford += CInt(Stableford(oHelper.RemoveSpcChar(row.Cells("Hole" & i).Value), i, row.Cells("PHdcp").Value))
                        Next
                        row.Cells("Points").Value = iStableford
                    End If
                Next
                '2020-01-14
                Dim dv As New DataView(DsLeague.Tables("dtScores"))
                dv.RowFilter = String.Format("Date = '{0}'", cbDates.SelectedItem)
                're-sort grid by Points winners
                dv.Sort = "Points Desc"

                Dim BindingSource = New BindingSource()
                BindingSource.DataSource = dv
                DtScoresBindingSource.DataSource = BindingSource

                'dgScores.Sort(dgScores.Columns("Points"), ListSortDirection.Descending)

                lbStatus.Text = String.Format("Finished Switching to {1} from {0}", ssf, sst)
                oHelper.status_Msg(lbStatus, Me)

            End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub rbFront_CheckedChanged(sender As Object, e As EventArgs) Handles rbFront.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If bFormLoad Or bSkipFB Then Exit Sub
            Dim shole = "Hole1"
            If rbBack.Checked Then shole &= "0"
            Dim dvscores As New DataView(DsLeague.Tables("dtScores"))
            With dvscores
                .RowFilter = String.Format("Date = '{0}' and {1} > 0", cbDates.SelectedItem, shole)
                .Sort = "Date DESC"
            End With
            'if we have front nine scores and the rb is checked no reason to do anything else
            Dim sfb As String = "Front"
            If rbBack.Checked Then sfb = "Back"
            Dim mbr = MsgBox(String.Format("Switch scores to the {0} nine?", sfb), MsgBoxStyle.YesNo)
            If mbr = MsgBoxResult.No Then
                RemoveHandler rbFront.CheckedChanged,
                AddressOf rbFront_CheckedChanged

                If rbFront.Checked Then
                    rbBack.Checked = True
                Else
                    rbFront.Checked = True
                End If
                AddHandler rbFront.CheckedChanged,
                AddressOf rbFront_CheckedChanged
                Exit Sub
            End If

            Dim sFrom = "Out_"
            Dim sTo = "In_"
            If rbFront.Checked Then
                lbStatus.Text = String.Format("Swapping Back 9 to Front 9")
                oHelper.status_Msg(lbStatus, Me)
                oHelper.iHoleMarker = 1
                sFrom = "In_"
                sTo = "Out_"
            Else
                lbStatus.Text = String.Format("Swapping Front 9 to Back 9")
                oHelper.status_Msg(lbStatus, Me)
                oHelper.iHoleMarker = 10
                sFrom = "Out_"
                sTo = "In_"
            End If

            Dim i = oHelper.iHoleMarker
            For Each col As DataGridViewColumn In dgScores.Columns
                Dim scol = col.Name
                If scol.StartsWith("Hole") Then
                    col.Name = "Hole" & i
                    col.HeaderText = i
                    i += 1
                ElseIf scol.StartsWith(sFrom) Then
                    col.Name = col.Name.Replace(sFrom, sTo)
                End If
            Next
            ChangeColAttributes()
            'For Each row As DataGridViewRow In dgScores.Rows
            '    oHelper.ChangeColorsForStrokes(row)
            'Next
            lbStatus.Text = String.Format("Finished Swapping nines")
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub dgScores_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgScores.DataError
        MsgBox(e.Exception.Message & vbCrLf & e.Exception.StackTrace)

        Try
            '    dgScores.EndEdit()
            '    MsgBox(e.Context.ToString)
        Catch ex As Exception
            '    MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub dgScores_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgScores.CurrentCellDirtyStateChanged
        'Dim x = ""
        'Dim dgc As DataGridViewCell
        'dgc = sender.currentcell
        'If dgc.OwningColumn.Name = "Clear" Then
        '    If dgc.Value = "True" Then
        '        dgc.Value = False
        '    Else
        '        dgc.Value = True
        '    End If
        'End If
    End Sub

    Private Sub dgScores_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellContentClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex & " Column - " & sender.currentcell.OwningColumn.Name)
        Try
            'If TypeOf dgScores.CurrentCell Is DataGridViewCheckBoxCell Then Exit Sub
            Dim dgc As DataGridViewCell = sender.currentcell
            oHelper.LOGIT(String.Format("Cell {0} Clicked,readonly={1}", dgc.OwningColumn.Name, dgc.ReadOnly))
            Dim R As DataGridViewRow = sender.CurrentRow

            oHelper.sPlayer = R.Cells("Player").Value
            '20190402-the content of the cell isnt changed yet
            '20180810 added Clear column
            If dgc.OwningColumn.Name = Clear Then
                'RemoveHandler dgScores.CellPainting, AddressOf dgScores_CellPainting
                If MessageBox.Show(String.Format("Do you want to clear scores for {0}", oHelper.sPlayer), "Clear Scores", MessageBoxButtons.YesNo) = DialogResult.No Then Exit Sub
                'If mbr = MsgBoxResult.No Then Exit Sub
                R.Cells("Hdcp").Value = R.Cells("PHdcp").Value
                R.Cells("Method").Value = DBNull.Value
                R.Cells("Round").Value = DBNull.Value
                'For Each col As DataGridViewColumn In dgScores.Columns
                '    If col.Name.Contains("Hole") Then
                '        col.ValueType = GetType(System.String)
                '    End If
                'Next
                For i = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
                    R.Cells("Hole" & i).Value = DBNull.Value 'dgScores.DefaultCellStyle.Format
                Next
                If oHelper.iHoleMarker = "1" Then
                    R.Cells("Out_Gross").Value = DBNull.Value
                    R.Cells("Out_Net").Value = DBNull.Value
                    R.Cells("Out_Gross").Style.BackColor = Color.White
                    R.Cells("Out_Net").Style.BackColor = Color.White
                Else
                    R.Cells("In_Gross").Value = DBNull.Value
                    R.Cells("In_Net").Value = DBNull.Value
                    R.Cells("In_Gross").Style.BackColor = Color.White
                    R.Cells("In_Net").Style.BackColor = Color.White
                End If
                R.Cells(earned).Value = DBNull.Value
                R.Cells(earned).Style.BackColor = Color.White
                R.Cells(skinamt).Value = DBNull.Value
                R.Cells(skinamt).Style.BackColor = Color.White
                R.Cells(closestamt).Value = DBNull.Value
                R.Cells(closestamt).Style.BackColor = Color.White

                If Not TypeOf dgScores.Rows(0).Cells(CTP1) Is DataGridViewCheckBoxCell Then
                    resetCTP(CTP1)
                End If
                If Not TypeOf dgScores.Rows(0).Cells(CTP2) Is DataGridViewCheckBoxCell Then
                    resetCTP(CTP2)
                End If
                resetSkins()
                For Each row As DataGridViewRow In dgScores.Rows
                    If cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                        oHelper.CalcMatches(dgScores)
                        Dim sopp As Int16 = MatchResults(row)
                        If sopp > 0 Then
                            MatchResults(dgScores.Rows(sopp))
                        End If
                    End If

                    Dim foundrows = DsLeague.dtPlayers.Select(String.Format("Name = '{0}'", oHelper.sPlayer))
                    If foundrows IsNot Nothing Then
                        If Not IsNumeric(foundrows(0)("Team")) Then
                            row.Cells("Player").Style.BackColor = Color.Aqua
                        End If
                    End If
                    'Dim sgross As Int16 = 0
                    If oHelper.iHoleMarker = 1 Then
                        'https://stackoverflow.com/questions/8255186/how-to-check-empty-and-null-cells-in-datagridview-using-c-sharp
                        'If String.IsNullOrWhiteSpace(TryCast(row.Cells("Out_Gross").Value, String)) Then
                        row.Cells("Out_Gross").Style.BackColor = row.Cells("Method").Style.BackColor
                        row.Cells("Out_Net").Style.BackColor = row.Cells("Method").Style.BackColor
                        If Not IsNumeric(row.Cells("Out_Gross").Value) Then
                            row.Cells("Player").Style.BackColor = Color.Pink
                        End If
                    Else
                        'If String.IsNullOrWhiteSpace(TryCast(row.Cells("In_Gross").Value, String)) Then
                        If Not IsNumeric(row.Cells("In_Gross").Value) Then
                            row.Cells("Player").Style.BackColor = Color.Pink
                        End If
                    End If
                Next

                'find all the rows that have money in them
                'For Each row As DataGridViewRow In dgScores.Rows
                '    If row.Cells(CTP1).Value <> DBNull.Value Then
                '        Dim dgctp As DataGridViewCell
                '        dgctp = row.Cells(CTP1).Value
                '    End If
                'Next

                'R.Cells("Match").Value = DBNull.Value
                'R.Cells("Match").Style.BackColor = Color.White
                'R.Cells("Points").Value = 0
                'R.Cells("Points").Style.BackColor = Color.White
                'R.Cells("Team_Points").Value = 0
                'R.Cells("Team_Points").Style.BackColor = Color.White
                'Dim value = DirectCast(dgc.EditedFormattedValue,
                '               Nullable(Of Boolean))
                'dgc.Value = False
                'If (value.HasValue AndAlso value = True) Then
                '    dgc.Value = False
                'Else
                '    dgc.Value = True
                'End If
                ChangeColAttributes()
                Dim x = ""
                'dgScores.EndEdit()
                '20190403-this is needed to update the checkbox as marked
                ''20200109-fix skins/ctp resets
            ElseIf dgc.OwningColumn.Name = skin Then
                Dim damt As Decimal = If(cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), oHelper.rLeagueParmrow(skin), "7")
                If Not CbSwap() Then
                    damt *= -1
                End If

                tbSkins.Text += damt
                tbSkinTot.Text += damt
                tbPurse.Text += damt
                resetSkins()

            ElseIf dgc.OwningColumn.Name = closest Then
                Dim damt As Decimal = If(cbDates.SelectedItem < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd"), 1, "3") / 2
                If Not CbSwap() Then
                    damt *= -1
                End If

                tbCP1.Text += damt
                tbCP1Tot.Text += damt
                tbCP2.Text += damt
                tbCP2Tot.Text += damt
                tbPurse.Text += damt
            ElseIf dgc.OwningColumn.Name.Contains("CTP_") Then
                Dim dgcb As DataGridViewCheckBoxCell = dgScores.Rows(e.RowIndex).Cells(closest)
                Dim x = ""
                If dgcb.Value <> "True" Then
                    MessageBox.Show(String.Format("Cant give {0} a closest, hes not in Closests", oHelper.sPlayer), "Player Not in CTPs", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                    dgcb.Value = False
                    Exit Sub
                End If

                Dim sColumn As DataGridViewColumn
                oHelper.LOGIT(String.Format("Put the skins/ctp winners as Textboxes"))
                sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = dgc.OwningColumn.Name).SingleOrDefault
                recreateCheckBoxesasTextBoxes(sColumn.DataPropertyName, sColumn.Index)
                dgScores.Columns(sColumn.DataPropertyName).HeaderText = sColumn.HeaderText
                For Each row As DataGridViewRow In dgScores.Rows
                    If row.Index = sender.currentrow.index Then
                        Dim iamt As Integer = If(sColumn.DataPropertyName = CTP1, CInt(tbCP1.Text), CInt(tbCP2.Text))
                        row.Cells(sColumn.DataPropertyName).Value = iamt
                        row.Cells(closestamt).Value = makeCellAmt(row.Cells(CTP1)) + makeCellAmt(row.Cells(CTP2))
                        row.Cells(earned).Value = makeCellAmt(row.Cells(skinamt)) + makeCellAmt(row.Cells(closestamt))
                    Else
                        row.Cells(sColumn.DataPropertyName).Value = DBNull.Value
                    End If
                Next
            End If

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Function NewmakeCellAmt(cell As Object) As Integer
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        NewmakeCellAmt = 0
        Try
            If cell.Value IsNot DBNull.Value Then
                If Not String.IsNullOrEmpty(cell.Value) Then
                    If IsNumeric(cell.Value) Then
                        NewmakeCellAmt = CInt(cell.Value)
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Function
    Function makeCellAmt(cell As DataGridViewCell) As Integer
        makeCellAmt = 0

        If cell.Value IsNot DBNull.Value Then
            If Not String.IsNullOrEmpty(cell.Value) Then
                If IsNumeric(cell.Value) Then
                    makeCellAmt = CInt(cell.Value)
                End If
            End If

        End If
    End Function
    Function CbSwap() As Boolean
        '20200109-fix skins/ctp resets
        'https://www.daniweb.com/programming/software-development/threads/366217/how-to-check-uncheck-a-datagridviewcolumncheckbox
        Dim checkstate As Boolean = dgScores.CurrentCell.Value
        If checkstate Then
            '2020-01-11 - if this player won a ctp/skin, ask first to take them off
            'Dim value = DirectCast(dgScores.CurrentRow.Cells(CTP1).Value, Nullable(Of Boolean))
            '2020-01-11=future work to reset all ctps or just one ctp
            If dgScores.CurrentCell.OwningColumn.Name.Contains("CTP") Then
                If dgScores.CurrentCell.ReadOnly Then
                    MessageBox.Show(String.Format("You must Reset CTPs before you uncheck this closest", oHelper.sPlayer, vbCrLf), "CTPs Locked!", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                End If
                Return dgScores.CurrentCell.Value
            Else
                If dgScores.CurrentRow.Cells(skin).Value <> False And IsNumeric(dgScores.CurrentRow.Cells(skinamt).Value) Then
                    If MessageBox.Show(String.Format("{0} won a skin{1}Do you want to reset this players skin money", oHelper.sPlayer, vbCrLf), "Skin Money Winner!", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) = DialogResult.Yes Then
                        dgScores.CurrentCell.Value = False
                        dgScores.CurrentCell.Style.BackColor = Color.White
                        dgScores.CurrentRow.Cells(earned).Value -= dgScores.CurrentRow.Cells(skinamt).Value
                        dgScores.CurrentRow.Cells(skinamt).Value = DBNull.Value
                    Else
                        dgScores.CurrentCell.Value = True
                    End If
                End If
                Return dgScores.CurrentCell.Value
            End If
            If dgScores.CurrentRow.Cells(CTP1).Value <> False Or dgScores.CurrentRow.Cells(CTP2).Value <> False Then
                MessageBox.Show(String.Format("{0} won a CTP{1}You must Reset CTPs before you uncheck this closest", oHelper.sPlayer, vbCrLf), "CTP Money Winner!", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                dgScores.CurrentCell.Value = True
            Else
                dgScores.CurrentCell.Value = False
                dgScores.CurrentCell.Style.BackColor = Color.White
            End If
        Else
            dgScores.CurrentCell.Value = True
            dgScores.CurrentCell.Style.BackColor = Color.LightBlue
        End If
        Return dgScores.CurrentCell.Value
    End Function
    Private Sub dgScores_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEndEdit
        oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3} Col R/O({4}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name, dgScores.Columns(e.ColumnIndex).ReadOnly))
        Dim dgc = TryCast(dgScores.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewCheckBoxCell)
        If dgc IsNot Nothing Then Exit Sub
        EditScores()
    End Sub
    Private Sub dgScores_CellEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEnter
        oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3} Col R/O({4}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name, dgScores.Columns(e.ColumnIndex).ReadOnly))
        'If sender.currentrow.cells(e.ColumnIndex).readonly Then
        '    SendKeys.Send("{tab}")
        'End If
    End Sub

    Private Sub dgScores_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgScores.CellPainting
        If Debugger.IsAttached And bshowpaint Then
            oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        Try
            If e.RowIndex < 0 Or oHelper.bDGSError Then Exit Sub
            Dim sColName As String = dgScores.Columns(e.ColumnIndex).Name
            'Dim dgc = TryCast(dgScores.Rows(e.RowIndex).Cells(e.ColumnIndex), DataGridViewCheckBoxCell)
            'If dgc IsNot Nothing Then
            '    If sIn(sColName, "Skins,CTP_1,CTP_2", False) Then
            '        If dgc.Value = "Y" Then
            '            dgc.Value = True
            '        ElseIf dgc.Value = "N" Then
            '            dgc.Value = False
            '        End If
            '    End If
            'End If
            If sColName.Contains("Hole") Then
                bstrokehole = False
                'Erase the current cells attributes
                EraseCell(e)

                If IsNumeric(e.Value) Then
                    Dim sHole As String = sColName.Substring(4)
                    Dim iHole As Int16 = 0
                    If xlskins.Count > 0 Then
                        iHole = xlskins(sHole - oHelper.iHoleMarker).Split("-")(0)
                        sLowRows = xlskins(sHole - oHelper.iHoleMarker).Split("-")(1).Split("|").ToList
                    End If
                    If sHole = iHole Then
                        MarkCellsForSkins(e)
                    End If
                    Dim xx = oHelper.thisCourse(String.Format("Hole{0}", sHole))
                    'mark strokes on the cells upper right corner
                    PaintStrokes(e)
                    'if under par circle it
                    If e.Value < oHelper.thisCourse(String.Format("Hole{0}", sHole)) Then
                        Dim x = ""
                        'oHelper.LOGIT(String.Format("Under Par {0},{1},{2}", dgScores.Rows(e.RowIndex).Cells(e.ColumnIndex).Value, e.RowIndex, e.ColumnIndex))
                        CircleBirdies(e)
                        'If e.Value < oHelper.thisCourse(String.Format("Hole{0}", sHole)) - 1 Then
                        '    CircleEagles(e)
                        'End If
                    Else
                        If e.Value IsNot Nothing Then
                            Dim newRect As Rectangle = New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
                            Using gridBrush As Brush = New SolidBrush(dgScores.GridColor), backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
                                Using gridLinePen As Pen = New Pen(gridBrush)
                                    'this makes the number red and font bold
                                    If e.Value IsNot Nothing Then
                                        Dim myFont As New Font("Tahoma", 9, FontStyle.Regular)
                                        If e.CellStyle.Font.Strikeout Then myFont = New Font("Tahoma", 9, FontStyle.Regular Or FontStyle.Strikeout)
                                        e.CellStyle.Font = myFont
                                        e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                                    End If
                                    'e.Handled = True
                                End Using
                            End Using

                            Using gridBrush As Brush = New SolidBrush(dgScores.GridColor), backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
                                Using gridLinePen As Pen = New Pen(gridBrush)
                                    'this makes the number red and font bold
                                    If e.Value IsNot Nothing Then
                                        Dim myFont As New Font("Tahoma", 9, FontStyle.Regular)
                                        If e.CellStyle.Font.Strikeout Then myFont = New Font("Tahoma", 9, FontStyle.Regular Or FontStyle.Strikeout)
                                        e.CellStyle.Font = myFont
                                        e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                                    End If
                                    e.Handled = True
                                End Using
                            End Using

                            'e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                        End If
                    End If
                Else
                    PaintStrokes(e)
                End If
                'ElseIf dgScores.Columns(e.ColumnIndex).Name.Contains("Gross") Then
                '2020-01-10 - paint red for under par
            ElseIf sIn(sColName, "Gross,Net", True) Then
                If IsNumeric(e.Value) Then
                    If e.Value < If(sColName.Contains("Out"), oHelper.thisCourse(String.Format("Out")), oHelper.thisCourse(String.Format("In"))) Then
                        dgScores.Rows(e.RowIndex).Cells(sColName).Style.ForeColor = oHelper.UnderParColor
                        Dim myFont As New Font("Tahoma", 9, FontStyle.Bold)
                        e.CellStyle.Font = myFont
                    Else
                        Dim myFont As New Font("Tahoma", 9, FontStyle.Regular)
                        e.CellStyle.Font = myFont
                        dgScores.Rows(e.RowIndex).Cells(sColName).Style.BackColor = dgScores.Rows(e.RowIndex).Cells("Method").Style.BackColor
                        dgScores.Rows(e.RowIndex).Cells(sColName).Style.ForeColor = dgScores.Rows(e.RowIndex).Cells("Method").Style.ForeColor
                    End If
                End If
            ElseIf sIn(sColName, skinamt & "," & closestamt & "," & earned, True) Then  '"$Skins,$Closest,$Earn", True) Then
                If IsNumeric(e.Value) Then
                    dgScores.Rows(e.RowIndex).Cells(sColName).Style.BackColor = Color.Gold
                Else
                    dgScores.Rows(e.RowIndex).Cells(sColName).Style.BackColor = dgScores.Rows(e.RowIndex).Cells("Method").Style.BackColor
                End If
                '2020-01-11 - paint checkboxes untested 
                'https://stackoverflow.com/questions/39545483/square-filled-checkbox-in-datagridview
                'ElseIf sIn(scolName, oHelper.skins & "," & ohelper.closest, False) Then
                '    Dim value = DirectCast(e.FormattedValue, Nullable(Of Boolean))
                '    e.Paint(e.CellBounds, DataGridViewPaintParts.All And
                '                          Not (DataGridViewPaintParts.ContentForeground))
                '    Dim state = IIf((value.HasValue And value.Value),
                '                    VisualStyles.CheckBoxState.CheckedNormal,
                '                    VisualStyles.CheckBoxState.MixedNormal)
                '    Dim size = RadioButtonRenderer.GetGlyphSize(e.Graphics, state)
                '    Dim location = New Point((e.CellBounds.Width - size.Width) / 2,
                '                            (e.CellBounds.Height - size.Height) / 2)
                '    location.Offset(e.CellBounds.Location)
                '    CheckBoxRenderer.DrawCheckBox(e.Graphics, location, state)
                '    e.Handled = True
            End If

            If dgScores.CurrentCell.RowIndex = e.RowIndex And dgScores.CurrentCell.ColumnIndex = e.ColumnIndex Then
                Dim focusrect As Rectangle = e.CellBounds
                focusrect.Width -= 1
                focusrect.Height -= 1
                ControlPaint.DrawFocusRectangle(e.Graphics, focusrect)
            End If

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Function sIn(sfld As String, s2Compare As String, bcontains As Boolean) As Boolean
        sIn = False
        Dim sflds = s2Compare.Split(",")
        For Each fld As String In s2Compare.Split(",")
            If bcontains Then
                If sfld.Contains(fld) Then
                    sIn = True
                    Exit Function
                End If
            Else
                If sfld = fld Then
                    sIn = True
                    Exit Function
                End If
            End If
        Next
    End Function
    Sub EraseCell(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        Try
            'erase the cell
            Dim rectangle As New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
            Dim gridBrush As Brush = New SolidBrush(dgScores.GridColor)
            Dim backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
            e.Graphics.FillRectangle(backColorBrush, e.CellBounds)
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub MarkCellsForSkins(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        Try
            Dim rectangle As New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
            Dim gridBrush As Brush = New SolidBrush(dgScores.GridColor)
            Dim backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
            For Each srow In sLowRows
                If srow > e.RowIndex Then Exit For
                If e.RowIndex = srow Then
                    If sLowRows.Count > 1 Then
                        Dim myFont As New Font("Tahoma", 9, FontStyle.Strikeout)
                        e.CellStyle.Font = myFont
                        gridBrush = New SolidBrush(dgScores.GridColor)
                        backColorBrush = New SolidBrush(Color.Yellow)
                    Else
                        gridBrush = New SolidBrush(dgScores.GridColor)
                        backColorBrush = New SolidBrush(Color.Gold)
                    End If
                End If
                e.Graphics.FillRectangle(backColorBrush, e.CellBounds)

            Next
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub CircleBirdies(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        Try
            Dim newRect As Rectangle = New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
            Using gridBrush As Brush = New SolidBrush(dgScores.GridColor), backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
                Using gridLinePen As Pen = New Pen(gridBrush)
                    ' e.Graphics.FillRectangle(backColorBrush, e.CellBounds)
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1)
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom)
                    e.Graphics.DrawEllipse(Pens.Blue, newRect)
                    'this makes the number red and font bold
                    If e.Value IsNot Nothing Then
                        Dim myFont As New Font("Tahoma", 10, FontStyle.Bold)
                        If e.CellStyle.Font.Strikeout Then myFont = New Font("Tahoma", 10, FontStyle.Bold Or FontStyle.Strikeout)
                        e.CellStyle.Font = myFont
                        e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Crimson, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                    End If
                    'e.Handled = True
                End Using
            End Using

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub CircleEagles(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            oHelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
        End If
        'attempt 1
        'Dim centerX, centerY As Integer
        'Dim cornerX, cornerY As Integer
        'Dim radius As Integer
        'Dim greenPen As New Pen(Brushes.Blue)

        'centerX = e.CellBounds.X + 8
        'centerY = e.CellBounds.Y + 8
        'Dim i As Integer
        'For i = 1 To 5 Step 1
        '    greenPen = New Pen(Color.BlueViolet, 25)
        '    radius = i
        '    cornerX = centerX - radius / 2
        '    cornerY = centerY - radius / 2
        '    e.Graphics.DrawEllipse(greenPen, cornerX, cornerY, radius, radius)
        'Next
        'attempt 2
        EraseCell(e)
        Dim r As Rectangle = dgScores.GetCellDisplayRectangle(e.RowIndex, e.ColumnIndex, False)
        Dim b As Brush = Brushes.Purple
        Dim p As New Pen(b)
        'e.Graphics.DrawEllipse(p, r)
        'Redraw the dataGridview to clean other red rectangle

        dgScores.Invalidate()
        Dim g As Graphics = dgScores.CreateGraphics()
        g.DrawEllipse(p, r)
        Exit Sub
        Try
            Dim newRect As Rectangle = New Rectangle(e.CellBounds.X + 3, e.CellBounds.Y + 2, e.CellBounds.Width - 8, e.CellBounds.Height - 8)
            Using gridBrush As Brush = New SolidBrush(dgScores.GridColor), backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
                Using gridLinePen As Pen = New Pen(gridBrush)
                    ' e.Graphics.FillRectangle(backColorBrush, e.CellBounds)
                    'e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 8, e.CellBounds.Right - 8, e.CellBounds.Bottom - 8)
                    'e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 8, e.CellBounds.Top, e.CellBounds.Right - 8, e.CellBounds.Bottom)
                    e.Graphics.DrawEllipse(Pens.Blue, newRect)
                    'this makes the number red and font bold
                    If e.Value IsNot Nothing Then
                        Dim myFont As New Font("Tahoma", 10, FontStyle.Bold)
                        If e.CellStyle.Font.Strikeout Then myFont = New Font("Tahoma", 10, FontStyle.Bold Or FontStyle.Strikeout)
                        e.CellStyle.Font = myFont
                        e.Graphics.DrawString(CType(e.Value, String), e.CellStyle.Font, Brushes.Crimson, e.CellBounds.X + 4, e.CellBounds.Y + 4, StringFormat.GenericDefault)
                    End If
                    'e.Handled = True
                End Using
            End Using

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub PaintStrokes(e As DataGridViewCellPaintingEventArgs)
        If bshowpaint Then
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        End If

        Try
            Dim IHdcp As Int16 = dgScores.Rows(e.RowIndex).Cells("PHdcp").Value
            Dim isi As Int16 = oHelper.CalcStrokeIndex(dgScores.Columns(e.ColumnIndex).Name)
            oHelper.LOGIT(String.Format("row {0} Column {1},", e.RowIndex, dgScores.Columns(e.ColumnIndex).Name))
            'oHelper.LOGIT(String.Format("{0} - Hdcp({1})-SI({2})-{3}", dgScores.Rows(e.RowIndex).Cells("Player").Value, IHdcp, isi, dgScores.Columns(e.ColumnIndex).Name)) '  "-" &  & "-" & isi & "-" & isi & "-" & dgScores.Columns(e.ColumnIndex).Name)
            'if the handicap > stroke index make color beige
            If IHdcp >= isi Then
                Dim mybrush As New SolidBrush(Color.Green)   ' creates a solid fill of shape   
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Border)
                Dim newrect As New Rectangle(e.CellBounds.Right - 8, e.CellBounds.Y + 2, 4, 4)
                e.Graphics.FillEllipse(mybrush, newrect)
                e.Handled = True
                'if double stroke hole, make color b/a
                If IHdcp - oHelper.iHoles >= isi Then
                    newrect = New Rectangle(e.CellBounds.Right - 16, e.CellBounds.Y + 2, 4, 4)
                    e.Graphics.FillEllipse(mybrush, newrect)
                    e.Handled = True
                End If
                bstrokehole = True
            Else
                Dim mybrush As New SolidBrush(dgScores.Rows(e.RowIndex).DefaultCellStyle.BackColor)
                'Dim mybrush As New SolidBrush(Color.White)   ' creates a solid fill of shape   
                'If dgScores.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightBlue Then
                '    mybrush = New SolidBrush(Color.LightBlue)
                'End If

                e.Paint(e.ClipBounds, DataGridViewPaintParts.Border)
                Dim newrect As New Rectangle(e.CellBounds.Right - 8, e.CellBounds.Y + 2, 4, 4)
                e.Graphics.FillEllipse(mybrush, newrect)
                e.Handled = True
            End If
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Sub NewScoreCard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.DataTable2CSV(DsLeague.dtScores, oHelper.sFilePath & "\Scores.csv")
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If TypeOf sender.currentcell Is DataGridViewTextBoxCell Then
                Dim row As DataGridViewRow = sender.currentrow
                Dim cell As DataGridViewTextBoxCell = sender.currentcell
                If cell.OwningColumn.Name = "Player" Then
                    oHelper.sPlayer = cell.Value
                    lbStatus.Text = String.Format("Gathering Scores for {0}", oHelper.sPlayer)
                    oHelper.status_Msg(lbStatus, Me)
                    oHelper.bScoresbyPlayer = True
                    oHelper.IHdcp = row.Cells("Phdcp").Value
                    lbStatus.Text = String.Format("Getting Scores for {0}", cell.Value)
                    oHelper.status_Msg(lbStatus, Me)
                    Scores.Show()
                    oHelper.bScoresbyPlayer = False
                    lbStatus.Text = String.Format("Finished Getting Scores for {0}", oHelper.sPlayer)
                    oHelper.status_Msg(lbStatus, Me)
                ElseIf cell.OwningColumn.Name = "Hdcp" Then
                    MessageBox.Show(cell.ToolTipText, "Last Five Scores")
                ElseIf cell.OwningColumn.Name.Contains("CTP") Then
                    For Each ctprow As DataGridViewRow In dgScores.Rows
                        ctprow.Cells(cell.OwningColumn.Name).Value = DBNull.Value
                    Next
                    If e.RowIndex >= 0 Then
                        dgScores.Rows(e.RowIndex).Cells(cell.OwningColumn.Name).Value = tbCP1Tot.Text
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub


    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgScores.SortCompare
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'If e.Column.Index <> 0 Then
        '    Return
        'End If
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bNoRowLeave = True
            Dim sc1 = oHelper.RemoveSpcChar(e.CellValue1)
            Dim sc2 = oHelper.RemoveSpcChar(e.CellValue2)
            If IsNumeric(sc1) And IsNumeric(sc2) Then
                e.SortResult = If(CInt(sc1) < CInt(sc2), -1, 1)
            Else
                e.SortResult = If(CStr(sc1) < CStr(sc2), -1, 1)
            End If

            e.Handled = True
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub btnResetCTPs_Click(sender As Object, e As EventArgs) Handles btnResetCTPs.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            resetCTP(CTP1)
            resetCTP(CTP2)
            Exit Sub
            Dim sColumn As DataGridViewColumn
            oHelper.LOGIT(String.Format("Recreate the skins/ctp winners as Textboxes"))
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP1).SingleOrDefault
            recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
            For Each row As DataGridViewRow In dgScores.Rows
                row.Cells(sColumn.DataPropertyName).Value = False
                row.Cells(earned).Value = If(makeCellAmt(row.Cells(skinamt)) > 0, row.Cells(skinamt).Value, DBNull.Value)
            Next

            dgScores.Columns(sColumn.DataPropertyName).HeaderText = sColumn.HeaderText
            sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = CTP2).SingleOrDefault
            recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
            dgScores.Columns(sColumn.DataPropertyName).HeaderText = sColumn.HeaderText

            For Each row As DataGridViewRow In dgScores.Rows
                row.Cells(sColumn.DataPropertyName).Value = False
                row.Cells(earned).Value = If(makeCellAmt(row.Cells(skinamt)) > 0, row.Cells(skinamt).Value, DBNull.Value)
                row.Cells(closestamt).Value = DBNull.Value
                row.Cells("$Closest").Style.BackColor = Color.White
            Next
            'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = oHelper.closest).SingleOrDefault
            'sColumn.ReadOnly = False
            dgScores.Columns(closest).ReadOnly = False
            dgScores.Columns(closest).Visible = True
            ChangeColAttributes()

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub resetCTP(sCTP As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.LOGIT(String.Format("Recreate the skins/ctp winners as Textboxes"))
            Dim sColumn As DataGridViewColumn = dgScores.Columns(sCTP)
            'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = sCTP).SingleOrDefault
            recreateTextBoxesasCheckBoxes(sColumn.DataPropertyName, sColumn.Index)
            For Each row As DataGridViewRow In dgScores.Rows
                row.Cells(sColumn.DataPropertyName).ValueType = GetType(Boolean)
                row.Cells(sColumn.DataPropertyName).Value = False
                If IsNumeric(row.Cells(earned).Value) Then
                    row.Cells(earned).Value -= makeCellAmt(row.Cells(closestamt))
                    If IsNumeric(row.Cells(closestamt).Value) Then
                        row.Cells(closestamt).Value -= makeCellAmt(row.Cells(closestamt))
                    End If
                End If
                row.Cells(earned).Value = If(makeCellAmt(row.Cells(earned)) > 0, row.Cells(earned).Value, DBNull.Value)
                row.Cells(closestamt).Value = If(makeCellAmt(row.Cells(closestamt)) > 0, row.Cells(closestamt).Value, DBNull.Value)
            Next
            dgScores.Columns(sColumn.DataPropertyName).HeaderText = sColumn.HeaderText

            dgScores.Columns(closest).ReadOnly = False
            dgScores.Columns(closest).Visible = True
            ChangeColAttributes()

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub CreateRpt(sRpt As String)
        'If Not Debugger.IsAttached Then
        Dim sfn As String = oHelper.sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) & String.Format("_{0}.csv", sRpt)
        lbStatus.Text = String.Format("Creating spreadsheet({0}) of Scorecard from this screen...", sfn)
        oHelper.status_Msg(lbStatus, Me)
        oHelper.dgv2csv(dgScores, sfn)
        Dim sHtml As String = oHelper.Create_Html_From_DGV(dgScores)
        sHtml = oHelper.ConvertDataGridViewToHTMLWithFormatting(dgScores, Me)
        Dim swhtml As New IO.StreamWriter(sfn.Replace(".csv", ".html"), False)
        swhtml.WriteLine(sHtml)
        swhtml.Close()
        lbStatus.Text = "Finished creating spreadsheet of Scorecard from this screen"
        oHelper.status_Msg(lbStatus, Me)
        'End If
    End Sub
    'Private Sub dgScores_MouseDown(sender As Object, e As MouseEventArgs) Handles dgScores.MouseDown
    '    dgScores.Rows(sender.currentrow.index).Cells(sender.currentcell.columnindex).style.backcolor = Color.Blue
    'End Sub
    'Private Sub dgScores_MouseUp(sender As Object, e As MouseEventArgs) Handles dgScores.MouseUp
    '    dgScores.Rows(sender.currentrow.index).Cells(sender.currentcell.columnindex).style.backcolor = Color.White
    'End Sub
    Private Sub StandardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StandardToolStripMenuItem.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim sMsgHdcpCalc = "Handicap Calculation" & vbCrLf
        sMsgHdcpCalc &= "80% Handicap" & vbCrLf
        sMsgHdcpCalc &= "Take Last 5 Scores(rollover prior years)" & vbCrLf
        sMsgHdcpCalc &= "Throw out highest Score and Lowest Score" & vbCrLf
        sMsgHdcpCalc &= "If Less than 5 scores, take scores 1-4" & vbCrLf

        Dim sMsgStandard = ""
        sMsgStandard &= "Format 2 Man teams A-B Players" & vbCrLf
        sMsgStandard &= "1 Point - A Player Match Winner" & vbCrLf
        sMsgStandard &= "1 Point - B Player Match Winner" & vbCrLf
        sMsgStandard &= "1 Point - Team Winner" & vbCrLf
        sMsgStandard &= "-------------------------------" & vbCrLf
        sMsgStandard &= sMsgHdcpCalc

        MsgBox(sMsgStandard, MsgBoxStyle.Information, "Standard Scoring Format")
    End Sub
    Private Sub StablefordToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StablefordToolStripMenuItem.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim sMsgHdcpCalc = "Handicap Calculation" & vbCrLf
        sMsgHdcpCalc &= "80% Handicap" & vbCrLf
        sMsgHdcpCalc &= "Take Last 5 Scores(rollover prior years)" & vbCrLf
        sMsgHdcpCalc &= "Throw out highest Score and Lowest Score" & vbCrLf
        sMsgHdcpCalc &= "If Less than 5 scores, take scores 1-4" & vbCrLf

        Dim sMsgStableford = "Format 2 Man teams Stableford" & vbCrLf
        sMsgStableford &= "Points are accumulated per Hole after handicap" & vbCrLf
        sMsgStableford &= "0 Points - Double Bogey or worse" & vbCrLf
        sMsgStableford &= "1 Points - Bogey" & vbCrLf
        sMsgStableford &= "2 Points - Par" & vbCrLf
        sMsgStableford &= "3 Points - Birdie" & vbCrLf
        sMsgStableford &= "4 Points - Eagle" & vbCrLf
        sMsgStableford &= "8 Points - Double Eagle" & vbCrLf
        sMsgStableford &= "-------------------------------" & vbCrLf
        sMsgStableford &= sMsgHdcpCalc

        MsgBox(sMsgStableford, MsgBoxStyle.Information, "Stableford Scoring Format")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        oHelper.DataTable2CSV(DsLeague.dtScores, oHelper.sFilePath & "\Scores.csv")
    End Sub

    Private Sub dgScores_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgScores.CellBeginEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim dgc As DataGridViewCell = sender.currentcell
        sOldCellValue = dgc.Value
    End Sub

    Private Sub cbMatches_CheckedChanged(sender As Object, e As EventArgs) Handles cbMatches.CheckedChanged

    End Sub

    Private Sub dgScores_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgScores.CellFormatting
        'If dgScores.Columns(e.ColumnIndex).DataPropertyName.Contains("Hole") Then
        '    If String.IsNullOrEmpty(e.Value) = True Then Exit Sub
        'Else
        '    If e.Value = 0 Then
        '        e.Value = String.Empty
        '        e.FormattingApplied = True
        '    End If
        'End If
    End Sub

    Private Sub btnReports_Click(sender As Object, e As EventArgs) Handles btnReports.Click
        CreateRpt("Scores")
    End Sub

    'Private Sub cbMarkPaid_CheckedChanged(sender As Object, e As EventArgs) Handles cbMarkPaid.CheckedChanged
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try
    '        For Each row In dgScores.Rows
    '            oHelper.sPlayer = row.cells("Player").value
    '            Dim Checked As Boolean = CType(row.cells(skins).value, Boolean)
    '            If cbMarkPaid.Checked Then
    '                If Not Checked Then
    '                    row.cells(skins).value = True
    '                    tbSkins.Text += dSkinsamt
    '                End If
    '            Else
    '                If Checked Then
    '                    row.cells(skins).value = False
    '                    tbSkins.Text -= dSkinsamt
    '                End If
    '            End If

    '            tbSkinTot.Text = CDec(tbPSkins.Text) + CDec(tbSkins.Text)
    '            tbPurse.Text = CDec(tbSkinTot.Text) + CDec(tbCP1.Text) + +CDec(tbCP2.Text)
    '            oHelper.LOGIT(String.Format("Skins amt,tot,purse {0}-{1}-{2}", tbPSkins.Text, tbSkinTot.Text, tbPurse.Text))
    '            Checked = CType(row.cells(closest).value, Boolean)
    '            If cbMarkPaid.Checked Then
    '                If Not Checked Then
    '                    row.cells(closest).value = True
    '                    tbCP1.Text += dCTPamt / 2
    '                    tbCP2.Text += dCTPamt / 2
    '                End If
    '            Else
    '                If Checked Then
    '                    row.cells(closest).value = False
    '                    tbCP1.Text -= dCTPamt / 2
    '                    tbCP2.Text -= dCTPamt / 2
    '                End If
    '            End If
    '            oHelper.LOGIT(String.Format("Players {0} amount set to {1}", iCTP, dCTPamt))

    '            'calcExtra(cell, iamt)
    '            Dim dcp1 As Decimal = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
    '            Dim dcp2 As Decimal = CDec(tbPCP2.Text) + CDec(tbCP2.Text)
    '            tbCP1Tot.Text = dcp1
    '            tbCP2Tot.Text = dcp2
    '            tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)

    '            oHelper.LOGIT(String.Format("CTP1 amt,tot,purse {0}-{1}-{2}", tbCP1.Text, tbCP1Tot.Text, tbPurse.Text))
    '            oHelper.LOGIT(String.Format("CTP2 amt,tot,purse {0}-{1}-{2}", tbCP2.Text, tbCP2Tot.Text, tbPurse.Text))

    '        Next

    '        sOldCellValue = "" '20190312 this causes scores to be saved if not blank.
    '    Catch ex As Exception
    '        MsgBox(oHelper.GetExceptionInfo(ex))
    '    End Try
    'End Sub
End Class
'copied from https://stackoverflow.com/questions/44271490/sorting-a-dgv-column-ascending-while-putting-null-values-at-the-bottom
Public Class DataGridViewColumnComparer
    Implements IComparer

    Private SortOrderModifier As Integer = 1
    Private ColumnName As String

    Public Sub New(ByVal ColumnName As String, ByVal Order As SortOrder)
        Me.ColumnName = ColumnName
        If Order = SortOrder.Descending Then
            SortOrderModifier = -1
        ElseIf Order = SortOrder.Ascending Then
            SortOrderModifier = 1
        End If
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim Row1 As DataGridViewRow = CType(x, DataGridViewRow)
        Dim Row2 As DataGridViewRow = CType(y, DataGridViewRow)

        Dim Row1Value As String = Row1.Cells(ColumnName).Value.ToString()
        Dim Row2Value As String = Row2.Cells(ColumnName).Value.ToString()

        'If CompareResult = 1 that means that Row1 should be placed BELOW Row2.
        'If CompareResult = -1 that means that Row1 should be placed ABOVE Row2.
        Dim CompareResult As Integer = String.Compare(Row1Value, Row2Value)

        If String.IsNullOrEmpty(Row1Value) = True Then
            CompareResult = 1 'Row1 has an empty/null value, place it below Row2.
        ElseIf String.IsNullOrEmpty(Row2Value) = True Then
            CompareResult = -1 'Row2 has an empty/null value, place Row1 above.
        End If

        Return CompareResult * SortOrderModifier
    End Function
End Class

Public Class DataGridViewNumericComparer
    Implements IComparer

    Private SortOrderModifier As Integer = 1
    Private ColumnName As String

    Public Sub New(ByVal ColumnName As String, ByVal Order As SortOrder)
        Me.ColumnName = ColumnName
        If Order = SortOrder.Descending Then
            SortOrderModifier = -1
        ElseIf Order = SortOrder.Ascending Then
            SortOrderModifier = 1
        End If
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim Row1 As DataGridViewRow = CType(x, DataGridViewRow)
        Dim Row2 As DataGridViewRow = CType(y, DataGridViewRow)

        Dim Row1Value As String = Row1.Cells(ColumnName).Value.ToString()
        Dim Row2Value As String = Row2.Cells(ColumnName).Value.ToString()

        Dim Row1NumVal As Nullable(Of Long) = Nothing
        Dim Row2NumVal As Nullable(Of Long) = Nothing

        Dim Row1Temp As Long = 0
        Dim Row2Temp As Long = 0

        If Long.TryParse(Row1Value, Row1Temp) = True Then Row1NumVal = Row1Temp
        If Long.TryParse(Row2Value, Row2Temp) = True Then Row2NumVal = Row2Temp

        'If CompareResult = 1 that means that Row1 should be placed BELOW Row2.
        'If CompareResult = -1 that means that Row1 should be placed ABOVE Row2.
        Dim CompareResult As Integer

        If Row1NumVal.HasValue = True AndAlso Row2NumVal.HasValue = True Then
            CompareResult = Row1NumVal.Value.CompareTo(Row2NumVal.Value)

        ElseIf Row1NumVal.HasValue = False Then
            CompareResult = 1

        ElseIf Row2NumVal.HasValue = False Then
            CompareResult = -1

        End If

        Return CompareResult * SortOrderModifier
    End Function
End Class
