Imports System.ComponentModel
Public Class frmScoreCard
    Dim oHelper As New Helper
    'fields with (Number) are key fields
    'field width-read only-tabstop-MiddleRight
    'Public Const sScoreCard As String = "Method,Team,Group(3),Player(1),Date(2),Holes,PHdcp,Hdcp,Skins,Closest,$Earn,$Skins,$Closest,#Skins,#Closests,Points,Team_Points,Opponent"
    'Dim sSkinsFields = New List(Of String)("Skins,Closest,$Earn,$Skins,$Closest,#Skins,#Closests".Split(","))
    'Dim sMatchFields = New List(Of String)("Points,Team_Points,Opponent".Split(","))
    Dim fromsizeW As Integer, gvSsizeW As Integer, gvSCsizeW As Integer, gbSCsizeW As Integer
    Dim fromsizeH As Integer, gvSsizeH As Integer, gvSCsizeH As Integer, gbSCsizeH As Integer
    'Dim bCloseScreen As Boolean = False
    Dim rs As New Resizer
    Public sOldCellValue As Object

    Dim dgvScoreDate As String
    Dim toolTipHdcp As New ToolTip
    Dim bSaveBtn = False
    Dim bMatchesSet = False
    Dim bFormLoad = True
    Dim iSkins As Integer = 0, iCTP As Integer = 0, iPCTP1 As Integer = 0, iPCTP2 As Integer = 0, iPurse As Integer = 0
    Dim dsLeague As New dsLeague
    Dim dCTPamt As Decimal = 0
    Dim dSkinsamt As Decimal = 0

    Private Sub ScoreCard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Show()
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            lbStatus.Text = String.Format("Loading Scores from {0}", Main.lbScoresFile.Text)
            Dim xx = Main.lbScoresFile.Text
            oHelper.status_Msg(lbStatus, Me)

            'copy main's helper
            oHelper = Main.oHelper
            dsLeague = oHelper.dsLeague
            RemoveHandler rbFront.CheckedChanged,
            AddressOf rbFront_CheckedChanged

            'Me.Show()
            ''700 X 1550
            'If Main.iScreenWidth = 1920 Then
            '    Me.Width = 1560
            'ElseIf Main.iScreenWidth = 1366 Then
            '    Me.Width = 1200
            'ElseIf Main.iScreenWidth = 2560 Then
            '    Me.Width = 1575
            'End If
            'If Main.iScreenHeight = 1200 Or Main.iScreenHeight = 1400 Then Me.Height = 650
            ''test for Greg 1366 x 768
            'Me.Width = 1200
            'rs.ResizeAllControls(Me)
            Dim sWH As String = oHelper.ScreenResize()
            If Me.Width >= sWH.Split(":")(0) Then
                Me.Width = sWH.Split(":")(0) - (sWH.Split(":")(0) * 0.1)
            Else
                ' Me.Width = sWH.Split(":")(0)
            End If
            If Me.Height >= sWH.Split(":")(1) Then
                Me.Height = sWH.Split(":")(1) - (sWH.Split(":")(1) * 0.1)
            Else
                'Me.Height = sWH.Split(":")(1)
            End If
            oHelper.LOGIT(String.Format("Screen Height {0} Width {1}", Me.Height, Me.Width))
            lbMonitor.Text = String.Format("{0}, Resolution {1} x {2}, Menu {3} x {4}", My.Computer.Name, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Width, Me.Width, Me.Height)
            'If Me.Width > Main.iScreenWidth Then
            '    Me.Width = Main.iScreenWidth
            '    dgScores.Width = Me.Width - 100
            'End If
            'Me.Height = Main.iScreenHeight
            cbDates.Items.AddRange(Main.cbDates.Items.Cast(Of String).ToArray)
            cbDates.SelectedIndex = cbDates.Items.IndexOf(oHelper.dDate.ToString("yyyyMMdd"))

            'set defaults for form
            oHelper.bColors = False
            oHelper.bDots = False
            If rbColors.Checked Then oHelper.bColors = True
            If rbDots.Checked Then oHelper.bDots = True

            gbDefMeth.Visible = True
            If oHelper.rLeagueParmrow("Method") = "Net" Then
                rbNet.Checked = True
            ElseIf oHelper.rLeagueParmrow("Method") = "Gross" Then
                rbGross.Checked = True
            End If

            lbStatus.Text = ""
            rbColors.TabStop = False
            rbDots.TabStop = False
            rbGross.TabStop = False
            rbNet.TabStop = False
            rbScore.TabStop = False
            rbBack.TabStop = False
            rbFront.TabStop = False
            cbScoresLocked.Checked = False

            dSkinsamt = oHelper.rLeagueParmrow("Skins")
            If oHelper.bCCLeague Or CDate(oHelper.dDate).ToString("yyyyMMdd") >= CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then dSkinsamt = 7
            dCTPamt = 1
            If oHelper.bCCLeague Or CDate(oHelper.dDate).ToString("yyyyMMdd") >= CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then dCTPamt = 3

            'BldScoreCardDataGridFromFile()
            'GetScoresforDate()

            'If Not bMatchesSet Then Me.Close()

            If oHelper.bCCLeague Then cbMatches.Checked = True
            ' Set up the delays for the ToolTip.
            toolTipHdcp = oHelper.toolTipHdcp
            toolTipHdcp.AutoPopDelay = 5000
            toolTipHdcp.InitialDelay = 1000
            toolTipHdcp.ReshowDelay = 500
            ' Force the ToolTip text to be displayed whether or not the form is active.
            toolTipHdcp.ShowAlways = True
            ' Dim cell As DataGridViewCell
            '' Set up the ToolTip text for the datagridviewcell.
            toolTipHdcp.SetToolTip(gbColors, "Colors ")
            '20180202-add event for checkboxes
            'AddHandler dgScores_CellClick() = New System.Windows.Forms.DataGridViewCellEventHandler(AddressOf dgScores_CellClick)
            'AddHandler lv1.SelectedIndexChanged, AddressOf lv1_SelectedIndexChanged
            'AddHandler dgScores.CellClick, AddressOf dgScores_CellClick
            AddHandler rbFront.CheckedChanged,
            AddressOf rbFront_CheckedChanged

            bFormLoad = False
            lbParmFile.Text = ""
            lbStatus.Text = String.Format("Finished Loading Scores from {0}", Main.lbScoresFile.Text)
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            Dim x = ""
        End Try
    End Sub
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        bSaveBtn = False
        'this makes closing form event kick off
        Me.Close()
        Exit Sub
    End Sub

    'this function is called to validate that all holes were entered and to add up scores to net/gross calculations
    Function calcScores(R As DataGridViewRow) As Integer
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        calcScores = 0
        With oHelper
            .bAllHolesEntered = True
            Try

                For i = .iHoleMarker To .iHoleMarker + .iHoles - 1
                    Dim iScore As String = .RemoveSpcChar(R.Cells("Hole" & i).Value)
                    If iScore <> "" Then
                        If Not IsNumeric(iScore) Then
                            .bAllHolesEntered = False
                            Continue For
                        End If
                        '20180127-decrease for net method
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
                    Else
                        .bAllHolesEntered = False
                        Exit For
                    End If
                Next

            Catch ex As Exception
                MsgBox(oHelper.GetExceptionInfo(ex))
            End Try
        End With
    End Function
    Function GetPHdcp() As String
        GetPHdcp = ""
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            GetPHdcp = ""

            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
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
    Sub rebuildReadOnly()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If cbScoresLocked.Checked = True Then
                dgScores.ReadOnly = True
                'Exit Sub
            Else
                dgScores.ReadOnly = False
            End If

            'create array from above defined fields we want out of scorecard
            Dim sArray = New List(Of String)
            sArray.AddRange(Helper.cBaseScoreCard.Replace("Holes", "Date(2)-cPat120,Holes").Split(","))
            sArray.AddRange(Helper.cSkinsFields.Split(","))
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
            If CDate(oHelper.dDate).ToString("yyyyMMdd") < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
                sArray.Add("Points-cPat40nt")
                sArray.Add("Team_Points-cPat40nt")
                sArray.Add("Opponent-cPat170nt")
            End If

            Dim sColFormat = New List(Of String)
            Dim sScoreCardforDGV = ""
            'strip parenthesis and add gross/net for In/Out
            'fields can have a pattern associated for cell length, centering,

            For Each parm As String In sArray
                'set detault pattern
                Dim sPat = "cPat40"
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
                ElseIf sParm = "Date" Then
                    If oHelper.bScoresbyPlayer Then
                        sColFormat.Add(sPat)
                    Else
                        Continue For
                    End If
                End If
                sScoreCardforDGV = sScoreCardforDGV + sParm + ","
                sColFormat.Add(sPat)
            Next

            'replace spaces with underscores for csv column matchups
            sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")
            Dim dvScores As New DataView(dsLeague.Tables("dtScores"))
            dvScores.RowFilter = "Date = '" & cbDates.SelectedItem & "'"
            'added sort by match(partner)
            dvScores.Sort = "Partner"
            Dim dtScorecard As DataTable = dvScores.ToTable(True, sScoreCardforDGV.Split(",").ToArray)

            dgScores.Columns.Clear()

            oHelper.bNoRowLeave = True
            dgScores.RowTemplate.Height = 20
            dgScores.DefaultCellStyle.Font = New Font("Tahoma", 11)

            'With dgScores
            '    .DataSource = dtScorecard
            'End With
            Dim arr As Array = sScoreCardforDGV.Split(",").ToArray
            For Each col As DataColumn In dtScorecard.Columns
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = col.ColumnName
                If dgc.Name.Contains("Hole") Then dgc.MaxInputLength = 9
                dgc.ValueType = GetType(System.String)
                dgScores.Columns.Add(dgc)
            Next

            Dim sdel = "-"
            For Each col As DataGridViewColumn In dgScores.Columns
                With col
                    Dim sformat = sColFormat(.Index)
                    Try
                        Select Case sColFormat(.Index)
                            Case "cPat40nt"
                                sformat = Helper.cPat40nt
                            Case "cPatHole"
                                sformat = Helper.cPathole
                            Case "cPat60"
                                sformat = Helper.cPat60
                            Case "cPat120"
                                sformat = Helper.cPat120
                            Case "cPatMeth"
                                sformat = Helper.cPatMeth
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

                    Dim scolname = .Name
                    Dim sWidth = sformat.Split(sdel)(0)
                    Dim sRO = sformat.Split(sdel)(1)
                    Dim sTabstop = sformat.Split(sdel)(2)
                    Dim sAlign = sformat.Split(sdel)(3)

                    .Width = sWidth
                    'oHelper.LOGIT(dgScores.Rows(0).Cells(.Name).Value & "-" & dgScores.Rows(0).Cells(.Name).ReadOnly)
                    .ReadOnly = sRO
                    .DataGridView.TabStop = sTabstop
                    Select Case sAlign
                        Case "mr"
                            .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
                            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                        Case "mc"
                            .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                        Case "ml"
                            .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                    End Select

                    .HeaderText = .HeaderText.Replace("_", " ")
                    If .Name.Contains("Hole") Then
                        .HeaderText = .HeaderText.Replace("Hole", "")
                    End If

                    '20180201-force read only of scores are locked
                    'If cbScoresLocked.Checked = True Then
                    '    .ReadOnly = True
                    'End If
                    .SortMode = DataGridViewColumnSortMode.NotSortable

                End With
            Next

            For Each row As DataRow In dtScorecard.Rows
                dgScores.Rows.Add(row.ItemArray)
                oHelper.ChangeColorsForStrokes(dgScores.Rows(dgScores.RowCount - 1))
            Next

            If dtScorecard.Rows.Count <> oHelper.rLeagueParmrow("Teams") * 2 Then
                MsgBox(String.Format("Score card file has {0} and should have {1}, fix before saving", dtScorecard.Rows.Count, oHelper.rLeagueParmrow("Teams") * 2))
                Throw New Exception("Not Enough scores")
            End If

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub BldScoreCardDataGridFromFile()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        lbStatus.Text = "Building Scorecard Datagrid..."
        oHelper.status_Msg(lbStatus, Me)

        Try
            If cbDates.SelectedItem >= CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") And
               cbDates.SelectedItem <= CDate(oHelper.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
                oHelper.bCCLeague = True
            Else
                oHelper.bCCLeague = False
            End If

            If oHelper.rLeagueParmrow("ScoresLocked") = "Y" Then
                cbScoresLocked.Checked = True
                btnSave.Visible = False
            Else
                cbScoresLocked.Checked = False
                btnSave.Visible = True
            End If
            '20190905-skip matches if League Championship
            bMatchesSet = False
            'If oHelper.dDate.ToString("yyyyMMdd") < CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then
            If oHelper.getMatchScores(cbDates.SelectedItem) Then
                bMatchesSet = True
            End If
            'End If

            oHelper.LOGIT(String.Format("Selected Date {0}", cbDates.SelectedItem))
            dgvScoreDate = cbDates.SelectedItem

            '10/1/2017 add code to pull in all scores for a given player
            'check frmScoreCard for event
            '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
            '2 - if double click on a playerevent, get scores for a given player

            If cbDates.Text.ToString = "" Then cbDates.SelectedItem = oHelper.dDate.ToString("yyyyMMdd")
            '20190318 - not sure why we need this, may remove
            If oHelper.iHoles = 0 Then oHelper.iHoles = oHelper.rLeagueParmrow("Holes")

            '20181003 - if scores already exist in table, dont use date to determine which 9 were playing, we can swap nines and override schedule
            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
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
                'If oHelper.iHoleMarker = 0 Then oHelper.CalcHoleMarker(oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
            Next

            oHelper.LOGIT(String.Format("hole marker {0}", oHelper.iHoleMarker))
            '20180130-calculate number of closests to pins there should be
            oHelper.iNumClosests = 0
            For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
                If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
            Next

            rebuildReadOnly()

            'dgScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells

            dgScores.AllowUserToAddRows = False
            dgScores.AllowUserToDeleteRows = False

            '20190724-progress bar
            tspb.ProgressBar.Value = 0
            tspb.ProgressBar.Minimum = 0
            tspb.ProgressBar.Maximum = dvscores.Count
            tssl.Text = String.Format("Loading {0} Scores", tspb.ProgressBar.Maximum)
            Dim et As TimeSpan
            Dim sStartTime As DateTime = Now
            '20180201-save off y/n values
            '20190311 - need to sav y/n off because we delete them to turn them into checkboxes
            Dim sSkins As New List(Of String)
            Dim sCTPs As New List(Of String)
            For Each skin As DataGridViewRow In dgScores.Rows
                sSkins.Add(oHelper.convDBNulltoSpaces(skin.Cells("Skins").Value))
                sCTPs.Add(oHelper.convDBNulltoSpaces(skin.Cells("Closest").Value))
                tspb.ProgressBar.Value += 1
                tssl.Text = String.Format("Loading score {0} of {1}", tspb.ProgressBar.Value, tspb.ProgressBar.Maximum)
                tspb.ProgressBar.Refresh()
                Application.DoEvents()
            Next

            et = Now - sStartTime
            If et.TotalMinutes > 1 Then
                tssl.Text = String.Format("Loaded {0} scores {1} elapsed time", tspb.ProgressBar.Maximum, CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs")
            Else
                tssl.Text = String.Format("Loaded {0} scores {1} elapsed time", tspb.ProgressBar.Maximum, CInt(et.TotalSeconds) Mod 60 & " Secs")
            End If

            'add a clear scores checkbox
            Dim cbClearScores As New DataGridViewCheckBoxColumn
            With cbClearScores
                .HeaderText = "Clear"
                .Name = "Clear"
                .DataPropertyName = "Clear"
                .Width = 50
                If cbScoresLocked.Checked = True Then .ReadOnly = True
                dgScores.Columns.Insert(0, cbClearScores)
            End With

            'save the column index of skins/ctp
            Dim iSkinCol As Int16 = dgScores.Columns("Skins").Index
            Dim iCTPCol As Int16 = dgScores.Columns("Closest").Index

            'recreate columns as checkboxes
            Dim ncol As New DataGridViewCheckBoxColumn
            ncol.HeaderText = "Skins"
            ncol.Name = "Skins"
            ncol.DataPropertyName = "Skins"
            '20180201-remove hardcoded column 17,18
            ncol.Width = dgScores.Columns(iSkinCol).Width
            If cbScoresLocked.Checked = True Then ncol.ReadOnly = True
            dgScores.Columns.RemoveAt(iSkinCol)
            dgScores.Columns.Insert(iSkinCol, ncol)

            Dim ncol2 As New DataGridViewCheckBoxColumn
            ncol2.HeaderText = "Closest"
            ncol2.Name = "Closest"
            ncol2.DataPropertyName = "Closest"
            ncol2.Width = dgScores.Columns(iCTPCol).Width
            If cbScoresLocked.Checked = True Then ncol2.ReadOnly = True
            dgScores.Columns.RemoveAt(iCTPCol)
            dgScores.Columns.Insert(iCTPCol, ncol2)

            'loop through scores setting skins/ctp checkboxes instead of y/n
            Dim bscoresentered As Boolean = False

            For Each row As DataGridViewRow In dgScores.Rows
                row.Cells("Clear").Value = False
                oHelper.sPlayer = row.Cells("Player").Value
                If sSkins(row.Index) = "Y" Then
                    'Dim dgcSkins As DataGridViewCheckBoxCell
                    'dgcSkins = row.Cells("Skins")
                    'dgcSkins.ThreeState = True
                    row.Cells("Skins").Value = True
                    'iSkins += oHelper.rLeagueParmrow("Skins")
                    'iPurse += oHelper.rLeagueParmrow("Skins")
                End If
                If sCTPs(row.Index) = "Y" Then
                    row.Cells("Closest").Value = True
                    'iCTP += 1
                    'If oHelper.iNumClosests > 2 Then
                    '    MsgBox(String.Format("error we only have 2 ctps And inumclosests = {0}", oHelper.iNumClosests))
                    '    Exit Sub
                    'End If
                    'iPurse += 1
                End If
                oHelper.ChangeColorsForStrokes(row)
                'pull last score before this to get previous handicap
                Dim dv2Scores As New DataView(dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & cbDates.SelectedItem & "'"
                dv2Scores.Sort = " Date Desc"
                If dv2Scores.Count > 0 Then row.Cells("Phdcp").Value = dv2Scores(0).Item("Hdcp").ToString
                If row.Cells("Method").Value.ToString = " Net" Then
                    bscoresentered = True
                    If oHelper.iHoleMarker = 1 Then
                        row.Cells("Out_Gross").Value = CInt(row.Cells("Out_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    Else
                        row.Cells("In_Gross").Value = CInt(row.Cells("In_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    End If
                End If
                oHelper.MakeCellsStrings(row)
            Next

            If bscoresentered Then
                cbScoresLocked.Checked = True
            Else
                cbScoresLocked.Checked = False
            End If

            'adjCTP()
            '20180220-adjust column size
            'dgScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            'dgScores.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
        'oHelper.LOGIT(dgScores.Rows(0).Cells("Player").Value & " -" & dgScores.Rows(0).Cells("Player").ReadOnly)
    End Sub

    Sub SaveScores()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If dgScores.RowCount = 0 Then Exit Sub

            If Not cbScoresLocked.Checked Then
                If dgScores.RowCount > 1 Then
                    If sOldCellValue Is Nothing Then Exit Sub
                    ' If sOldCellValue = "" Then Exit Sub
                    If Not bSaveBtn Then
                        Dim mbr = MsgBox("Do you want to save scores before you reload this screen", MsgBoxStyle.YesNo)
                        If mbr = MsgBoxResult.No Then
                            cleanupScores()
                            lbStatus.Text = " Scores cleaned up and released"
                            lbStatus.BackColor = Color.LightGreen
                            oHelper.bDGSError = False
                            Exit Sub
                        End If
                    End If
                End If
            Else
                'Exit Sub
            End If

            lbStatus.Text = "Saving scores from this screen..."
            oHelper.status_Msg(lbStatus, Me)
            'oHelper.bNoRowLeave = True
            For Each row As DataGridViewRow In dgScores.Rows
                oHelper.sPlayer = oHelper.convDBNulltoSpaces(row.Cells("Player").Value)
                If oHelper.sPlayer.Trim = "" Then
                    lbStatus.Text = String.Format("Invalid Player {0},  fix before saving", oHelper.sPlayer)
                    Exit Sub
                End If
                'use ddate because cbdatesplayers.selecteditem has changed the date
                '20180419 - make key index instead of player name
                Dim sKeys() As Object = {row.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
                Dim arow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sKeys)
                'if not found, this is a sub
                If arow Is Nothing Then
                    Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
                    dvscores.RowFilter = String.Format("Date = {0}", oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
                    dvscores.Sort = "Partner"
                    Dim bfound = False
                    For Each srow As DataRowView In dvscores
                        If row.Index = CDbl(srow("Partner")) Then
                            arow = srow.Row
                            bfound = True
                            Exit For
                        End If
                    Next
                    If Not bfound Then Throw New Exception("Partner not found")
                End If

                '20180525-add grade
                If row.Index Mod 2 = 0 Then
                    arow("Grade") = "A"
                Else
                    arow("Grade") = "B"
                End If

                '20181003 clear scores in table before saving from gridview
                For Each cell As DataGridViewCell In row.Cells

                    If cell.OwningColumn.Name = "Team" Then
                        If oHelper.convDBNulltoSpaces(cell.Value).Trim = "" Then
                            lbStatus.Text = String.Format("Invalid Team {0} for Player {1}{2} you need to fix before saving", cell.Value, row.Cells("Player").Value, vbCrLf)
                            lbStatus.BackColor = Color.Red
                            Exit Sub
                        End If
                        '  20180201 - reformat checkboxes into y/n
                    ElseIf cell.OwningColumn.Name = "Skins" Or cell.OwningColumn.Name = "Closest" Then
                        If cell.Value = True Then
                            arow(cell.OwningColumn.Name) = "Y"
                        Else
                            arow(cell.OwningColumn.Name) = "N"
                        End If
                    ElseIf cell.OwningColumn.Name = "Clear" Then
                        Continue For
                    ElseIf cell.OwningColumn.Name = "Hdcp" Then
                        If oHelper.RemoveSpcChar(cell.Value) = "" Then
                            arow(cell.OwningColumn.Name) = row.Cells("PHdcp").Value
                        Else
                            arow(cell.OwningColumn.Name) = cell.Value
                        End If
                    Else '20180201 - replace nulls with "" and remove dots
                        Try
                            If cell.Value.ToString <> "" Then
                                If cell.OwningColumn.Name.StartsWith("Hole") Then
                                    If oHelper.RemoveSpcChar(cell.Value.trim) <> "" Then
                                        arow(cell.OwningColumn.Name) = oHelper.RemoveSpcChar(cell.Value.Trim)
                                    End If
                                Else
                                    arow(cell.OwningColumn.Name) = cell.Value.ToString.Trim
                                End If
                            End If

                        Catch ex As Exception

                        End Try

                    End If
                Next

            Next

            'oHelper.CopyDataGridViewToClipboard(dgLast5)
            Dim dv As New DataView(dsLeague.dtScores)
            dv.Sort = "Date DESC,Partner"
            'oHelper.DataTable2CSV(dv.ToTable, oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
            oHelper.DataTable2CSV(dv.ToTable, oHelper.sFilePath & "\Scores.csv")
            'oHelper.DataTable2CSV(dsLeague.dtScores, oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
            oHelper.DataTable2CSV(dsLeague.Tables("dtPlayers"), oHelper.sFilePath & "\Players.csv")
            lbStatus.Text = "Finished saving scores from this screen"
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub dgScores_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEndEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bDGSError = False
            oHelper.bAllHolesEntered = False
            Dim dgr As DataGridView = sender
            Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name
            If sCurrColName = "Skins" Or sCurrColName = "Closest" Then
                Exit Sub
            ElseIf sCurrColName = "Clear" Then
                dgr.CurrentCell.Value = sOldCellValue
                Exit Sub
            End If

            'if no change, then exit 
            If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
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
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub editPlayer(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bDGSError = False
            '    '1  Get the player name from the scores view with his handicap
            '    '2  if scores havent been updated for this player/date, then update the gridview and mark stroke holes
            '    'try looking for intials if the length is 2 char
            oHelper.sPlayer = oHelper.RemoveNulls(R.Cells(sCurrColName).Value)
            If oHelper.sPlayer <> "" Then oHelper.sPlayer = oHelper.fGetPlayer(oHelper.RemoveNulls(R.Cells(sCurrColName).Value), dgScores)

            If Not Len(oHelper.sPlayer) > 2 Then
                If oHelper.sPlayer = sOldCellValue Then MsgBox("Player not found, Try Again")
                R.Cells(sCurrColName).Value = sOldCellValue
                oHelper.bDGSError = True
                Exit Sub
            End If
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
            Dim MyPlayer As DataRow = dsLeague.Tables("dtPlayers").Rows.Find(oHelper.sPlayer)
            If R.Cells("Team").Value = "" Then R.Cells("Team").Value = MyPlayer("Team")

            If rbColors.Checked Then oHelper.bColors = True Else oHelper.bColors = False
            If rbDots.Checked Then oHelper.bDots = True Else oHelper.bDots = False
            oHelper.displayStrokes(R)
            '20180121 fix color blue on subs
            Dim dvplayers As New DataView(dsLeague.Tables("dtPlayers"))
            R.Cells(sCurrColName).Style.BackColor = Color.White
            dvplayers.RowFilter = "Name = '" & R.Cells(sCurrColName).Value & "'"
            oHelper.sPlayer = R.Cells(sCurrColName).Value.ToString
            If dvplayers.Count = 0 Then
                Exit Sub
            End If

            'if no team, they are a sub
            If dvplayers(0).Item("Team") Is DBNull.Value Then
                R.Cells(sCurrColName).Style.BackColor = Color.Aqua
            ElseIf dvplayers(0).Item("Team") <> R.Cells("Team").Value Then
                R.Cells(sCurrColName).Style.BackColor = Color.Aqua
            Else
                R.Cells(sCurrColName).Style.BackColor = Color.White
            End If
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
                        R.Cells("Out_Gross").ReadOnly = False
                    Else
                        R.Cells("In_Gross").Value = ""
                        R.Cells("In_Net").Value = ""
                        '20180510-turn off read only for method=score
                        R.Cells("In_Gross").ReadOnly = False
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
                            R.Cells("Out_Gross").ReadOnly = True
                        Else
                            R.Cells("In_Gross").Value = ""
                            R.Cells("In_Net").Value = ""
                            '20180510-turn off read only for method=score
                            R.Cells("In_Gross").ReadOnly = False
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

            oHelper.ChangeColorsForStrokes(R)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Sub editHoles(R As DataGridViewRow, sCurrColName As String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim sScore = oHelper.RemoveSpcChar(oHelper.convDBNulltoSpaces(R.Cells(sCurrColName).Value))
            If R.Cells("Method").Value Is DBNull.Value Or R.Cells("Method").Value = "" Then
                If rbGross.Checked Then R.Cells("Method").Value = "Gross"
                If rbNet.Checked Then R.Cells("Method").Value = "Net"
                If rbScore.Checked Then R.Cells("Method").Value = "Score"
            End If
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
                MsgBox(String.Format("Hole value {0}{1} must be 1-9, try again", sScore, vbCrLf))
                'this flag prevents rowleave from being invoked
                oHelper.bDGSError = True
                R.Cells(sCurrColName).Value = sOldCellValue
                'Exit Sub
            End Try
            '20171003 - allow all scores to be entered as hole 1 
            '20171008 - allow all scores to be entered as hole 10 
            If sCurrColName = "Hole1" Or sCurrColName = "Hole10" Then
                If sScore.Length = 9 Then
                    For i = oHelper.iHoleMarker + 1 To oHelper.iHoleMarker + 8
                        R.Cells("Hole" & i).Value = CInt(sScore.Substring(i - oHelper.iHoleMarker, 1))
                        oHelper.ValidateCell(R.Cells("Hole" & i))
                    Next
                    R.Cells(sCurrColName).Value = sScore.Substring(0, 1)
                End If
            End If
            'if all 9 scores are entered, then method must be net or gross
            Dim bgoodScore = True
            For i = oHelper.iHoleMarker + 1 To oHelper.iHoleMarker + 8
                Try
                    Dim ss = CInt(oHelper.RemoveSpcChar(R.Cells("Hole" & i).Value))
                Catch ex As Exception
                    bgoodScore = False
                    Exit For
                End Try
            Next
            '20190427-force ihdcp
            oHelper.IHdcp = R.Cells("pHdcp").Value
            If dgScores.Columns.Contains("Out_Gross") Then
                R.Cells("Out_Gross").Value = calcScores(R)
            Else
                R.Cells("In_Gross").Value = calcScores(R)
            End If

            If bgoodScore Then
                If R.Cells("Method").Value.ToString.StartsWith("S") Or R.Cells("Method").Value = "" Then
                    R.Cells("Method").Value = oHelper.BuildScoreCardMethods(gbDefMeth)
                End If

                If dgScores.Columns.Contains("Out_Gross") Then
                    '99 means this is this guys first score
                    If oHelper.IHdcp <> 99 Then R.Cells("Out_Net").Value = R.Cells("Out_Gross").Value - oHelper.IHdcp
                Else
                    If oHelper.IHdcp <> 99 Then R.Cells("In_Net").Value = R.Cells("In_Gross").Value - oHelper.IHdcp
                End If
            End If
            oHelper.ValidateCell(R.Cells(sCurrColName))

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

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
                        Throw New TeamDuplicate
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
        Catch ex As TeamDuplicate
            'MsgBox(String.Format("This Team ({0}) already has 2 players entered, setting back to {1}", R.Cells("Team").Value, sOldCellValue))
            MsgBox(String.Format("(Team {0}) {1}, setting back to {2}", R.Cells("Team").Value, ex.Message, sOldCellValue))
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
                If oHelper.bAllHolesEntered Then R.Cells("Hdcp").Value = oHelper.GetNewHdcp(R, cbDates.SelectedItem)

                ' Dim cell As DataGridViewCell
                '' Set up the ToolTip text for the datagridviewcell.
                'toolTipHdcp.SetToolTip(cell, "My button1")

                Dim sScore = ""
                Dim sPHdcp = GetPHdcp()
                Try
                    If sPHdcp = "" Then R.Cells("PHdcp").Value = R.Cells("Hdcp").Value
                Catch ex As Exception
                End Try
                If dgScores.Columns.Contains("Out_Gross") And oHelper.bAllHolesEntered Then
                    sScore = R.Cells("Out_Gross").Value
                    R.Cells("Out_Net").Value = sScore - R.Cells("PHdcp").Value
                End If

                If dgScores.Columns.Contains("In_Gross") And oHelper.bAllHolesEntered Then
                    sScore = R.Cells("In_Gross").Value
                    R.Cells("In_Net").Value = sScore - R.Cells("PHdcp").Value
                End If
                oHelper.ChangeColorsForStrokes(R)
            End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
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

    Private Sub dgScores_CellEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEnter
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'oHelper.LOGIT(sender.currentcell.owningcolumn.name & "-" & sender.currentcell.value & "-" & sender.currentcell.readonly)
        'If sender.CurrentCell.ReadOnly And oHelper.bDGSError = False Then
        '    SendKeys.Send("{TAB}")
        'End If
    End Sub
    Private Sub dgScores_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgScores.DataError
        'MsgBox(e.Exception.Message)

        Try
            '    dgScores.EndEdit()
            '    MsgBox(e.Context.ToString)
        Catch ex As Exception
            '    MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgScores.SortCompare
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

    Private Sub dgScores_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowEnter
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'Try
        '    'If dgScores.Rows(e.RowIndex).Cells("Method").Value = "" Then dgScores.Rows(e.RowIndex).Cells("Method") = oHelper.BuildScoreCardMethods()

        '    If dgScores.Rows(e.RowIndex).IsNewRow Then
        '        dgScores.Rows(e.RowIndex).Cells("Method").Value = oHelper.BuildScoreCardMethods(gbDefMeth)
        '        If cbSkins.Checked Then dgScores.Rows(e.RowIndex).Cells("Skins").Value = "Y"
        '        If cbClosest.Checked Then dgScores.Rows(e.RowIndex).Cells("Closest").Value = "Y"
        '    End If
        'Catch ex As Exception
        '    MsgBox(oHelper.GetExceptionInfo(ex))
        'End Try

    End Sub

    Sub GetScoresforDate()
        oHelper.LOGIT("--------------------------------------------------------------")
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            lbStatus.Text = "Getting scores for this screen..."
            oHelper.status_Msg(lbStatus, Me)

            SaveScores()
            oHelper.bNoRowLeave = True
            oHelper.bScoresbyPlayer = False
            bSaveBtn = False

            ' getVoiceScores()
            oHelper.CalcHoleMarker(cbDates.SelectedItem)
            oHelper.LOGIT(String.Format("Changing dots/colors"))
            If rbColors.Checked Then
                oHelper.bColors = True
                oHelper.bDots = False
            End If
            If rbDots.Checked Then
                oHelper.bDots = True
                oHelper.bColors = False
            End If

            '20190724-progress bar
            'tspb.ProgressBar.Value = 0
            'tspb.ProgressBar.Minimum = 0
            'tspb.ProgressBar.Maximum = dvscores.Count
            'tssl.Text = String.Format("Loading {0} Scores", tspb.ProgressBar.Maximum)
            Dim et As TimeSpan
            Dim sStartTime As DateTime = Now
            'tspb.ProgressBar.Value += 1
            'tssl.Text = String.Format("Loading score {0} of {1}", tspb.ProgressBar.Value, dvscores.Count)
            'tspb.ProgressBar.Refresh()
            'Application.DoEvents()

            BldScoreCardDataGridFromFile()

            et = Now - sStartTime
            If et.TotalMinutes >= 1 Then
                oHelper.LOGIT(String.Format("Built ScorecardGrid in {0}", CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs"))
            Else
                oHelper.LOGIT(String.Format("Built ScorecardGrid in {0}", CInt(et.TotalSeconds) Mod 60 & " Secs"))
            End If

            '20190312 -recalc skins for league champ

            oHelper.bNoRowLeave = False

            lbStatus.Text = "Finished Reading Scores"
            oHelper.status_Msg(lbStatus, Me)
            '20190311
            'sOldCellValue = ""
            If oHelper.rLeagueParmrow("ScoresLocked") = "Y" Then
                cbScoresLocked.Checked = True
            Else
                cbScoresLocked.Checked = False
            End If

            If Not Debugger.IsAttached Then
                sStartTime = Now
                Dim sfn = oHelper.sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture) & "_Scorecard.csv"
                lbStatus.Text = String.Format("Creating spreadsheet/html({0}) of scores from this screen...", sfn)
                oHelper.status_Msg(lbStatus, Me)
                oHelper.dgv2csv(dgScores, sfn)
                '20190822 - new html
                Dim sHtml As String = oHelper.Create_Html_From_DGV(dgScores)
                sHtml = oHelper.ConvertDataGridViewToHTMLWithFormatting(dgScores, Me)
                Dim swhtml As New IO.StreamWriter(sfn.Replace(".csv", ".html"), False)
                swhtml.WriteLine(sHtml)
                swhtml.Close()
                et = Now - sStartTime
                If et.TotalMinutes >= 1 Then
                    lbStatus.Text = String.Format("Finished creating spreadsheet/html({0}) in {1}", sfn, CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs")
                Else
                    lbStatus.Text = String.Format("Finished creating spreadsheet/html({0}) in {1}", sfn, CInt(et.TotalSeconds) Mod 60 & " Secs")
                End If
                oHelper.status_Msg(lbStatus, Me)

                tssl.Text = String.Format("Finished creating spreadsheet/html({0}) in {1}", sfn, CInt(et.TotalSeconds) Mod 60 & " Secs")
                tspb.ProgressBar.Refresh()
                Application.DoEvents()
            End If

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Sub cleanupScores()
        '20180126-reload scores from csv when not saved
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        oHelper.ReloadScores()
    End Sub
    Private Sub btnSkins_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub dgScores_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgScores.CellBeginEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        sOldCellValue = ""
        If sender.currentcell.value IsNot DBNull.Value Then
            If sender.currentcell.value IsNot Nothing Then sOldCellValue = sender.currentcell.value
        End If

    End Sub

    'Private SubdgScores_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgScores.CurrentCellDirtyStateChanged
    '    dgScores.CommitEdit(DataGridViewDataErrorContexts.Commit)
    'End Sub

    Private Sub dgScores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            MsgBox(String.Format("Resorting not allowed on scorecard"))
            Exit Sub

            Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
            lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
            oHelper.status_Msg(lbStatus, Me)
            Dim dgv = sender
            For Each row As DataGridViewRow In dgv.rows
                oHelper.ChangeColorsForStrokes(row)
            Next
            lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub rbDots_CheckedChanged(sender As Object, e As EventArgs) Handles rbDots.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            '20190318-prevent changing colors in datagridview if just producing spreadsheet
            'If lbstatus.text.contains("Creating spreadsheet") Then Exit Sub
            lbStatus.Text = String.Format("Start Changing Dots/Colors ")
            oHelper.status_Msg(lbStatus, Me)

            If rbColors.Checked Then
                oHelper.bColors = True
                oHelper.bDots = False
                gbHoleLegend.Visible = True
            Else
                oHelper.bColors = False
                oHelper.bDots = True
                gbHoleLegend.Visible = False
            End If

            For Each row As DataGridViewRow In dgScores.Rows
                oHelper.ChangeColorsForStrokes(row)
            Next
            lbStatus.Text = String.Format("Finished Changing Dots/Colors ")
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            bSaveBtn = True
            SaveScores()
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub dgScores_KeyDown(sender As Object, e As KeyEventArgs) Handles dgScores.KeyDown
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Selected Then Exit Sub
            Next
            Dim dgr As DataGridView = sender
            'If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
            Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name
            Dim R As DataGridViewRow = dgr.CurrentRow
            Dim dgc As DataGridViewCell = sender.currentcell
            If e.KeyCode = Keys.Delete Then
                If Not dgc.ReadOnly Then
                    Try
                        'save old value in case we fail an edit
                        sOldCellValue = dgc.Value
                        dgc.Value = ""
                        oHelper.bDGSError = False
                        oHelper.bAllHolesEntered = False
                        If R.Cells("Phdcp").Value Is Nothing Then oHelper.IHdcp = 99
                        If sCurrColName = "Player" Then
                            MsgBox("Cant Delete a player, try entering a player")
                            'editPlayer(R, sCurrColName)
                            R.Cells(sCurrColName).Value = sOldCellValue
                        ElseIf sCurrColName = "Method" Then
                            editMethod(R, sCurrColName)
                        ElseIf sCurrColName.StartsWith("Hole") Then
                            editHoles(R, sCurrColName)
                            editrest(R, sCurrColName)
                        ElseIf sCurrColName = "Team" Then
                            editTeam(R, sCurrColName)
                        End If
                        If oHelper.bDGSError Then oHelper.bDGSError = False
                    Catch ex As Exception
                        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
                    End Try
                End If
            End If
            Select Case e.KeyCode
                Case Keys.Left
                    SelectNextControl(ActiveControl, False, True, False, True)
                Case Keys.Right
                    SelectNextControl(ActiveControl, True, True, False, True)
                Case Keys.Up, Keys.Down
                    'Case Keys.ControlKey + Keys.Z
                    '    dgc.Value = sOldCellValue
                Case Else
                    'lbStatus.Text = String.Format("This cell {0} is read only, maybe unlocking scores will allow edit", sCurrColName)
                    'lbStatus.BackColor = Color.Yellow
            End Select
            'dgScores.Focus()
            'MyBase.OnKeyDown(e)
            'If e.KeyCode = Keys.Right Then
            '    SendKeys.Send("{TAB}")
            'ElseIf e.KeyCode = Keys.Left Then
            '    SendKeys.Send("+{TAB}")
            'End If
            'if e.KeyCode = Keys.Left Then SendKeys.Send("+{TAB}")
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub dgScores_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles dgScores.UserDeletingRow
        '20180127 - not allow to delete rows in scorecard
        e.Cancel = True
    End Sub
    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        '20180220-turn off lock box if last dropdown
        Try
            If cbDates.SelectedIndex = cbDates.Items.Count - 1 Then cbScoresLocked.Checked = False
            Try
                If cbDates.SelectedItem Is Nothing Then
                    oHelper.dDate = Date.ParseExact(cbDates.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                Else
                    oHelper.dDate = Date.ParseExact(cbDates.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                End If
                bFormLoad = True
                GetScoresforDate()
                bFormLoad = False
                cbMarkPaid.Checked = False
                '20180221-calculate number of closests to pins there should be
                oHelper.iNumClosests = 0
                For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
                    If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
                Next

                If oHelper.iHoleMarker = 1 Then
                    rbFront.Checked = True
                Else
                    rbBack.Checked = True
                End If
                oHelper.recalcLeftOvers()
                With oHelper
                    tbCP1.Text = oHelper.dThisWeeksCTPF1 + oHelper.dThisWeeksCTPB1
                    tbCP2.Text = oHelper.dThisWeeksCTPF2 + oHelper.dThisWeeksCTPB2
                    tbPCP1.Text = oHelper.dLastWeeksCTPB1
                    tbPCP2.Text = oHelper.dLastWeeksCTPB2
                    tbCP1Tot.Text = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
                    tbCP2Tot.Text = CDec(tbPCP2.Text) + CDec(tbCP2.Text)

                    tbSkins.Text = oHelper.dThisWeeksSkins
                    tbPSkins.Text = oHelper.dLastWeeksSkins
                    tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
                    tbExtraSkins.Text = oHelper.dExtraSkins
                    tbExtraCP1.Text = oHelper.dExtraCTPF1 + oHelper.dExtraCTPB1
                    tbExtraCP2.Text = oHelper.dExtraCTPF2 + oHelper.dExtraCTPB2
                    tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text) '+ CDec(tbExtraSkins.Text) + CDec(tbExtraCP1.Text) + CDec(tbExtraCP2.Text)
                End With
                oHelper.LOGIT(String.Format("Date: {0} Leftover Skins {1}", oHelper.dDate, oHelper.dThisWeeksSkins))
            Catch ex As Exception
                Dim x = ""
                MsgBox("Bad Date entered, must be yyyymmdd format")
                Me.Cursor = Cursors.Default
                Application.DoEvents()
                Exit Sub
            End Try
            Try

            Catch ex As Exception
                MsgBox(oHelper.GetExceptionInfo(ex))
            End Try
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub dgScores_ColumnDividerDoubleClick(sender As Object, e As DataGridViewColumnDividerDoubleClickEventArgs) Handles dgScores.ColumnDividerDoubleClick
        '20180220-fix auto size cols
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            For Each col As DataGridViewColumn In dgScores.Columns
                Dim icolw = col.Width
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                col.Width = icolw
            Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub dgScores_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseClick
        Dim x = ""
    End Sub
    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            Dim row As DataGridViewRow = sender.currentrow
            '20180225-fix Mouse click to expand columns
            'If e.ColumnIndex = 0 Then
            If cell.OwningColumn.Name = "Player" Then
                Dim mbResult As MsgBoxResult = MsgBox("List all scores For For " & cell.Value & "?", MsgBoxStyle.YesNo)
                If mbResult = MsgBoxResult.Yes Then
                    oHelper.bScoresbyPlayer = True
                    oHelper.sPlayer = cell.Value
                    oHelper.IHdcp = row.Cells("Phdcp").Value
                    Scores.Show()
                    oHelper.bScoresbyPlayer = False
                End If
            End If
            'End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub

    Private Sub cbScoresLocked_CheckedChanged(sender As Object, e As EventArgs) Handles cbScoresLocked.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            lbStatus.Text = String.Format("Start Locking Scores")
            oHelper.status_Msg(lbStatus, Me)
            cbScoresLocked.AutoCheck = False
            If cbScoresLocked.Checked Then
                btnSave.Visible = False
                dgScores.ReadOnly = True
                If oHelper.rLeagueParmrow("ScoresLocked") <> "Y" Then
                    'save extra amounts to league parm table
                    lbStatus.Text = String.Format("Saving leagueParameter, turning on ScoresLocked Flag")
                    oHelper.LOGIT(String.Format("Saving leagueParameter, turning on ScoresLocked Flag"))
                    oHelper.status_Msg(lbStatus, Me)
                    Dim sKeys() As Object = {oHelper.rLeagueParmrow("Name"), oHelper.rLeagueParmrow("StartDate")}
                    oHelper.dsLeague.Tables("dtLeagueParms").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtLeagueParms").Columns("Name"), oHelper.dsLeague.Tables("dtLeagueParms").Columns("StartDate")}
                    Dim dr As DataRow = oHelper.dsLeague.Tables("dtLeagueParms").Rows.Find(sKeys)
                    dr("ScoresLocked") = "Y"
                    oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & oHelper.dDate.ToString("yyyyMMdd") & "_LeagueParms.csv")
                    oHelper.rLeagueParmrow("ScoresLocked") = "Y"
                    lbStatus.Text = "Finished saving league parameter"
                    oHelper.status_Msg(lbStatus, Me)
                    'oHelper.DataTable2CSV(dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
                    '20180516 - save scores when locking
                    oHelper.bLockScores = True
                    Matches.Show()
                    Standings.Show()
                    SaveScores()

                End If
            Else
                'save extra amounts to league parm table
                lbStatus.Text = String.Format("Saving leagueParameter,  turning off ScoresLocked Flag")
                oHelper.LOGIT(String.Format("Saving leagueParameter, turning off ScoresLocked Flag"))
                oHelper.status_Msg(lbStatus, Me)
                Dim sKeys() As Object = {oHelper.rLeagueParmrow("Name"), oHelper.rLeagueParmrow("StartDate")}
                oHelper.dsLeague.Tables("dtLeagueParms").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtLeagueParms").Columns("Name"), oHelper.dsLeague.Tables("dtLeagueParms").Columns("StartDate")}
                Dim dr As DataRow = oHelper.dsLeague.Tables("dtLeagueParms").Rows.Find(sKeys)
                dr("ScoresLocked") = "N"
                'oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & oHelper.dDate.ToString("yyyyMMdd") & "_LeagueParms.csv")
                oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\LeagueParms.csv")
                oHelper.rLeagueParmrow("ScoresLocked") = "N"
                lbStatus.Text = "Finished saving league parameter"
                oHelper.status_Msg(lbStatus, Me)

                '20180228-rebuild read only fields
                Dim sNotROFlds = "Player,Method,Team,Group,Skins,Closest"
                dgScores.ReadOnly = False
                Dim sb As New System.Text.StringBuilder
                For Each row As DataGridViewRow In dgScores.Rows
                    For Each cell As DataGridViewCell In row.Cells
                        If TypeOf (cell.Value) Is Boolean Then Continue For
                        If Not sNotROFlds.Contains(cell.OwningColumn.Name) And Not cell.OwningColumn.Name.Contains("Hole") Then
                            sb.Append(String.Format("Field was False, setting to True for {0}", cell.OwningColumn.Name) & vbCrLf)
                            cell.ReadOnly = True
                            sb.Append(String.Format("Value {0} in grid for {1}", cell.ReadOnly, cell.OwningColumn.Name) & vbCrLf)
                        End If
                    Next
                Next
                oHelper.LOGIT(sb.ToString)
                btnSave.Visible = True
            End If
            lbStatus.Text = String.Format("Finished Locking Scores")
            oHelper.status_Msg(lbStatus, Me)
            cbScoresLocked.AutoCheck = True
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub dgScores_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgScores.RowValidating
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex)
        If sender.currentrow.Cells("Player").Value Is DBNull.Value Then
            'e.Cancel = True
            'dgScores.Rows.RemoveAt(e.RowIndex)
            'dgScores.CancelEdit()
        End If
        '       If oHelper.bDGSError Then e.Cancel = True

    End Sub

    Private Sub cbMarkPaid_CheckedChanged(sender As Object, e As EventArgs) Handles cbMarkPaid.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            'If bFormLoad Then Exit Sub
            '20190724-progress bar
            tspb.ProgressBar.Value = 0
            tspb.ProgressBar.Minimum = 0
            tspb.ProgressBar.Maximum = dgScores.Rows.Count
            tssl.Text = String.Format("Marking Skins/CTP {0} Paid", tspb.ProgressBar.Maximum)
            Dim et As TimeSpan
            Dim sStartTime As DateTime = Now
            For Each row In dgScores.Rows
                oHelper.sPlayer = row.cells("Player").value
                'Dim cbSkins As DataGridViewCheckBoxCell
                'cbSkins = row.cells("Skins")
                'Dim cbCTP As DataGridViewCheckBoxCell
                'cbCTP = row.cells("Closest")
                'If cbMarkPaid.Checked Then
                '    sOldCellValue = cbSkins.Value
                '    cbSkins.Value = True
                '    sOldCellValue = cbCTP.Value
                '    cbCTP.Value = True
                'Else
                '    sOldCellValue = cbSkins.Value
                '    cbSkins.Value = False
                '    sOldCellValue = cbCTP.Value
                '    cbCTP.Value = False
                'End If
                tspb.ProgressBar.Value += 1
                Dim Checked As Boolean = CType(row.cells("Skins").value, Boolean)
                If cbMarkPaid.Checked Then
                    If Not Checked Then
                        row.cells("Skins").value = True
                        tbSkins.Text += dSkinsamt
                    End If
                Else
                    If Checked Then
                        row.cells("Skins").value = False
                        tbSkins.Text -= dSkinsamt
                    End If
                End If

                tbSkinTot.Text = CDec(tbPSkins.Text) + CDec(tbSkins.Text)
                tbPurse.Text = CDec(tbSkinTot.Text) + CDec(tbCP1.Text) + +CDec(tbCP2.Text)
                oHelper.LOGIT(String.Format("Skins amt,tot,purse {0}-{1}-{2}", tbPSkins.Text, tbSkinTot.Text, tbPurse.Text))
                Checked = CType(row.cells("Closest").value, Boolean)
                If cbMarkPaid.Checked Then
                    If Not Checked Then
                        row.cells("Closest").value = True
                        tbCP1.Text += dCTPamt / 2
                        tbCP2.Text += dCTPamt / 2
                    End If
                Else
                    If Checked Then
                        row.cells("Closest").value = False
                        tbCP1.Text -= dCTPamt / 2
                        tbCP2.Text -= dCTPamt / 2
                    End If
                End If
                oHelper.LOGIT(String.Format("Players {0} amount set to {1}", iCTP, dCTPamt))

                'calcExtra(cell, iamt)
                Dim dcp1 As Decimal = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
                Dim dcp2 As Decimal = CDec(tbPCP2.Text) + CDec(tbCP2.Text)
                tbCP1Tot.Text = dcp1
                tbCP2Tot.Text = dcp2
                tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)

                oHelper.LOGIT(String.Format("CTP1 amt,tot,purse {0}-{1}-{2}", tbCP1.Text, tbCP1Tot.Text, tbPurse.Text))
                oHelper.LOGIT(String.Format("CTP2 amt,tot,purse {0}-{1}-{2}", tbCP2.Text, tbCP2Tot.Text, tbPurse.Text))

                tssl.Text = String.Format("Marking Skins/CTP {0} Paid {0} of {1}", tspb.ProgressBar.Value, dgScores.Rows.Count)
                'tspb.ProgressBar.Refresh()
                Application.DoEvents()
            Next

            et = Now - sStartTime
            If et.TotalMinutes >= 1 Then
                tssl.Text = String.Format("Marked Skins/CTP {0} Paid {1} elapsed time", dgScores.Rows.Count, CInt(et.TotalMinutes) Mod 60 & " Min :" & CInt(et.TotalSeconds) Mod 60 & " Secs")
            Else
                tssl.Text = String.Format("Marked Skins/CTP {0} Paid {1} elapsed time", dgScores.Rows.Count, CInt(et.TotalSeconds) Mod 60 & " Secs")
            End If
            'adjCTP()

            sOldCellValue = "" '20190312 this causes scores to be saved if not blank.
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Private Sub dgScores_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellValueChanged
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex & " Column - " & e.ColumnIndex)
        'Try
        '    If bFormLoad Then Exit Sub
        '    If sender.currentrow Is Nothing Then Exit Sub
        '    Dim dgc As DataGridViewCell
        '    dgc = sender.currentrow.cells(e.ColumnIndex)
        '    If dgc.OwningColumn.Name = "Skins" Then
        '        recalcSkins(dgc)
        '    ElseIf dgc.OwningColumn.Name = "Closest" Then
        '        'recalcCTP(dgc)
        '    End If
        'Catch ex As Exception
        '    MsgBox(oHelper.GetExceptionInfo(ex))
        'End Try
    End Sub

    Private Sub dgScores_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgScores.CurrentCellDirtyStateChanged
        Dim x = ""
        RemoveHandler dgScores.CurrentCellDirtyStateChanged,
            AddressOf dgScores_CurrentCellDirtyStateChanged

        If TypeOf dgScores.CurrentCell Is DataGridViewCheckBoxCell Then
            dgScores.EndEdit()
            Dim Checked As Boolean = CType(dgScores.CurrentCell.Value, Boolean)
            If dgScores.CurrentCell.OwningColumn.Name = "Skins" Then
                If Checked Then
                    tbSkins.Text += dSkinsamt
                Else
                    tbSkins.Text -= dSkinsamt
                End If
                tbSkinTot.Text = CDec(tbPSkins.Text) + CDec(tbSkins.Text)
                tbPurse.Text = CDec(tbSkinTot.Text) + CDec(tbCP1.Text) + +CDec(tbCP2.Text)
                oHelper.LOGIT(String.Format("Skins amt,tot,purse {0}-{1}-{2}", tbPSkins.Text, tbSkinTot.Text, tbPurse.Text))
            ElseIf dgScores.CurrentCell.OwningColumn.Name = "Closest" Then
                If Checked Then
                    tbCP1.Text += dCTPamt / 2
                    tbCP2.Text += dCTPamt / 2
                Else
                    tbCP1.Text -= dCTPamt / 2
                    tbCP2.Text -= dCTPamt / 2
                End If
                oHelper.LOGIT(String.Format("Players {0} amount set to {1}", iCTP, dCTPamt))

                'calcExtra(cell, iamt)
                Dim dcp1 As Decimal = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
                Dim dcp2 As Decimal = CDec(tbPCP2.Text) + CDec(tbCP2.Text)
                tbCP1Tot.Text = dcp1
                tbCP2Tot.Text = dcp2
                tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)

                oHelper.LOGIT(String.Format("CTP1 amt,tot,purse {0}-{1}-{2}", tbCP1.Text, tbCP1Tot.Text, tbPurse.Text))
                oHelper.LOGIT(String.Format("CTP2 amt,tot,purse {0}-{1}-{2}", tbCP2.Text, tbCP2Tot.Text, tbPurse.Text))
            End If
        End If

        AddHandler dgScores.CurrentCellDirtyStateChanged,
            AddressOf dgScores_CurrentCellDirtyStateChanged

    End Sub
    'Sub recalcSkins(cell As DataGridViewCheckBoxCell) ', bSwitchTF As Boolean)
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try
    '        Dim sCname As String = cell.OwningColumn.Name
    '        If sCname <> "Skins" Then Exit Sub
    '        oHelper.LOGIT(String.Format("checking {0} for {1}", sCname, oHelper.sPlayer))

    '        If Not cell.Value Then
    '            If sOldCellValue Is Nothing Or sOldCellValue <> "" Or sOldCellValue = True Then dSkinsamt *= -1
    '        End If

    '        oHelper.LOGIT(String.Format("amount set to {0}", dSkinsamt))

    '        tbSkins.Text += dSkinsamt
    '        tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
    '        tbPurse.Text = CInt(tbCP1Tot.Text) + CInt(tbCP2Tot.Text) + CInt(tbSkinTot.Text)
    '        oHelper.LOGIT(String.Format("Skins amt,tot,purse {0}-{1}-{2}", tbSkins.Text, tbSkinTot.Text, tbPurse.Text))

    '        'dgScores.EndEdit()
    '    Catch ex As Exception
    '        MsgBox(oHelper.GetExceptionInfo(ex))
    '    End Try

    'End Sub

    'Sub recalcCTP(cell As DataGridViewCheckBoxCell) ', bSwitchTF As Boolean)
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try
    '        Dim sCname As String = cell.OwningColumn.Name
    '        If sCname <> "Closest" Then Exit Sub
    '        oHelper.LOGIT(String.Format("checking {0} for {1}", sCname, oHelper.sPlayer))

    '        oHelper.LOGIT(String.Format("{0} value={1}", sCname, cell.Value))
    '        dCTPamt /= oHelper.iNumClosests
    '        If Not cell.Value Then
    '            If TypeOf sOldCellValue Is String Then
    '                If sOldCellValue Is Nothing Or sOldCellValue <> "" Then dCTPamt *= -1
    '            ElseIf TypeOf sOldCellValue Is Boolean Then
    '                If sOldCellValue = True Then dCTPamt *= -1
    '            End If
    '        End If

    '        oHelper.LOGIT(String.Format("Players {0} amount set to {1}", iCTP, dCTPamt))

    '        'calcExtra(cell, iamt)
    '        tbCP1.Text += dCTPamt
    '        tbCP2.Text += dCTPamt
    '        Dim dcp1 As Decimal = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
    '        Dim dcp2 As Decimal = CDec(tbPCP2.Text) + CDec(tbCP2.Text)
    '        tbCP1Tot.Text = dcp1
    '        tbCP2Tot.Text = dcp2
    '        tbSkinTot.Text = CDec(tbPSkins.Text) + CDec(tbSkins.Text)
    '        tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)

    '        oHelper.LOGIT(String.Format("CTP1 amt,tot,purse {0}-{1}-{2}", tbCP1.Text, tbCP1Tot.Text, tbPurse.Text))
    '        oHelper.LOGIT(String.Format("CTP2 amt,tot,purse {0}-{1}-{2}", tbCP2.Text, tbCP2Tot.Text, tbPurse.Text))

    '    Catch ex As Exception
    '        MsgBox(oHelper.GetExceptionInfo(ex))
    '    End Try

    'End Sub
    'Function calcExtra(cell As DataGridViewCheckBoxCell, iamt As Integer) As Integer
    '    'if odd number of players, dont add to cp1/2
    '    Dim iexamt = If(iamt < 0, iamt * -1, iamt) / oHelper.iNumClosests
    '    If iCTP Mod 2 Then
    '        'odd unchecked
    '        If Not cell.Value Then
    '            tbCP1.Text -= iexamt
    '            tbCP2.Text -= iexamt
    '        End If
    '        'even
    '        calcExtra = iamt
    '    Else
    '        If cell.Value Then
    '            tbCP1.Text += iexamt
    '            tbCP2.Text += iexamt
    '            'Else
    '            '    tbCP1.Text -= iexamt
    '            '    tbCP2.Text -= iexamt
    '        End If
    '        calcExtra = 0
    '    End If

    'End Function

    Private Sub rbFront_CheckedChanged(sender As Object, e As EventArgs) Handles rbFront.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If bFormLoad Then Exit Sub
            Dim dvscores As New DataView(dsLeague.Tables("dtScores"))
            With dvscores
                .RowFilter = String.Format("Date = '{0}' and Hole1 <> ''", cbDates.SelectedItem)
                .Sort = "Date DESC"
            End With
            'if we have front nine scores and the rb is checked no reason to do anything else
            If dvscores.Count > 0 And rbFront.Checked Then Exit Sub
            If dvscores.Count >= 0 And rbBack.Checked Then Exit Sub
            Dim sfb As String = "Front"
            If rbBack.Checked Then sfb = "Back"
            Dim mbr = MsgBox(String.Format("Do you really want to switch scores to the {0} nine?", sfb), MsgBoxStyle.YesNo)
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
            For Each row As DataGridViewRow In dgScores.Rows
                oHelper.ChangeColorsForStrokes(row)
            Next
            lbStatus.Text = String.Format("Finished Swapping nines")
            oHelper.status_Msg(lbStatus, Me)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub frmScoreCard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            SaveScores()
            If lbStatus.BackColor = Color.Red Then
                e.Cancel = True
                Exit Sub
            End If
            oHelper.Common_Exit()
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub dgScores_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgScores.CellValidating
        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " Row - " & e.RowIndex & " Column - " & " - " & sender.CurrentCell.OwningColumn.Name)
        '    'If sender.CurrentCell.OwningColumn.Name.ToString.StartsWith("Hole") Then
        '    '    If IsNot IsNumeric(sender.value.ToString) Then
        '    '        sender.CurrentCell.style.backcolor = Color.Red
        '    '        e.Cancel = True
        '    '    End If
        '    'End If
        '    If e.Cancel Then
        '        MsgBox("fix red fields before updating a row")
        '    End If
    End Sub
    Private Sub frmScoreCard_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            rs.ResizeAllControls(Me)
            oHelper.LOGIT(String.Format("Form Height {0} Width {1}", Me.Height, Me.Width))
            lbMonitor.Text = String.Format("{0}, Resolution {1} x {2}, Menu {3} x {4}", My.Computer.Name, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Width, Me.Width, Me.Height)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub dgScores_RowHeadersBorderStyleChanged(sender As Object, e As EventArgs) Handles dgScores.RowHeadersBorderStyleChanged

    End Sub
    Private Sub dgScores_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellContentClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex & " Column - " & sender.currentcell.OwningColumn.Name)
        Try
            If TypeOf dgScores.CurrentCell Is DataGridViewCheckBoxCell Then Exit Sub
            Dim dgc As DataGridViewCell = sender.currentcell
            'calcSkinsCTP(sender.currentcell)
            '20180220-disable checkbox when readonly
            oHelper.LOGIT(String.Format("Cell {0} Clicked", dgc.OwningColumn.Name))
            If dgScores.ReadOnly Or e.RowIndex < 0 Then
                If cbScoresLocked.Checked Then
                    MsgBox(String.Format("Scores are locked, cant change value unless you uncheck Scores locked box"))
                End If
                Exit Sub
            End If

            Dim R As DataGridViewRow = sender.CurrentRow

            oHelper.sPlayer = R.Cells("Player").Value
            '20190402-the content of the cell isnt changed yet
            '20180810 added Clear column
            If dgc.OwningColumn.Name = "Clear" Then
                Dim mbr = MsgBox(String.Format("Do you want to clear scores for {0}", oHelper.sPlayer), MsgBoxStyle.YesNo)
                If mbr = MsgBoxResult.No Then Exit Sub
                R.Cells("Hdcp").Value = R.Cells("PHdcp").Value
                For i = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
                    R.Cells("Hole" & i).Value = DBNull.Value
                Next
                If oHelper.iHoleMarker = "1" Then
                    R.Cells("Out_Gross").Value = DBNull.Value
                    R.Cells("Out_Net").Value = DBNull.Value
                Else
                    R.Cells("In_Gross").Value = DBNull.Value
                    R.Cells("In_Net").Value = DBNull.Value
                End If
            End If
            '20190403-this is needed to update the checkbox as marked
            '20190828-fix skins/ctp resets
            If dgc.OwningColumn.Name = "Skins" Then
                Dim dgcSkins As DataGridViewCheckBoxCell
                dgcSkins = sender.currentrow.Cells("Skins")
                If dgcSkins.ThreeState = True Then
                    dgcSkins.ThreeState = False
                    dgcSkins.Value = False
                    sOldCellValue = True
                Else
                    dgcSkins.ThreeState = True
                    dgcSkins.Value = True
                    sOldCellValue = False
                End If
            End If
            If dgc.OwningColumn.Name = "Closest" Then
                Dim dgcCTP As DataGridViewCheckBoxCell
                dgcCTP = sender.currentrow.Cells("Closest")
                If dgcCTP.ThreeState = True Then
                    dgcCTP.ThreeState = False
                    dgcCTP.Value = False
                    sOldCellValue = False
                Else
                    dgcCTP.ThreeState = True
                    dgcCTP.Value = True
                    sOldCellValue = False
                End If
            End If

            'dgScores.CommitEdit(DataGridViewDataErrorContexts.Commit)
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Public Function GetCheckedRows1(
        ByVal GridView As DataGridView,
        ByVal ColumnName As String) As List(Of DataGridViewRow)
        Return _
            (
                From SubRows In
                    (
                        From Rows In GridView.Rows.Cast(Of DataGridViewRow)()
                        Where Not Rows.IsNewRow
                    ).ToList
                Where CBool(SubRows.Cells(ColumnName).Value) = True
            ).ToList
    End Function

    Private Sub dgScores_KeyPress(sender As Object, e As KeyPressEventArgs) Handles dgScores.KeyPress
        '20180224-check for locked scores
        If cbScoresLocked.Checked = True Then
            MsgBox("You must unlock scores to edit this field" & vbCrLf & "NOTE: certain fields arent allowed to be edited")
            '20181014 -this logic will replace above
            'Dim mbr = MsgBox("You must unlock scores to edit this field" & vbCrLf & "Do you want to unlock them?", MsgBoxStyle.YesNo)
            'If mbr = MsgBoxResult.No Then
            '    Exit Sub
            'Else
            '    cbScoresLocked.Checked = False
            '    btnSave.Visible = False
            '    sender.currentcell.value = e.KeyChar
            'End If

        Else
            If sender.readonly Then MsgBox("This is a calculated field, not editable")
        End If
    End Sub

End Class
Public Class TeamDuplicate
    Inherits Exception
    Public Overrides ReadOnly Property Message() As String
        Get
            Return "2 Scores have already been entered"
        End Get
    End Property
End Class