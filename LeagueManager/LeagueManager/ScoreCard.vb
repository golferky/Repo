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
    Public sOldCellValue As String
    Public sOldPSkins As String
    Public sOldPCP1 As String
    Public sOldPCP2 As String
    Dim dgvScoreDate As String
    Dim toolTipHdcp As New ToolTip
    Dim bSaveBtn = False
    Dim bMatchesSet = False
    Dim bFormLoad = False
    Dim iSkins As Integer = 0, iCTP As Integer = 0, iPCTP As Integer = 0, iPCTP1 As Integer = 0, iPCTP2 As Integer = 0, iPurse As Integer = 0

    Private Sub ScoreCard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        lbStatus.Text = String.Format("Loading Scores from {0}", Main.lbScoresFile.Text)
        oHelper.status_Msg(lbStatus, Me)
        'copy main's helper
        oHelper = Main.oHelper
        oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}

        Dim dtschedule As New DataTable()
        'build a table of schedule with dates in rows instead of columns
        dtschedule = oHelper.buildSchedule()
        'reformat dates into yyyymmdd format
        For Each row In dtschedule.Rows
            If row(1) Is DBNull.Value Then
                dtschedule.Rows.Remove(row)
                Continue For
            End If
            Dim wkdate As Date = row("Date")
            Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            row("Date") = reformatted
        Next

        cbDatesPlayers.Items.Clear()
        'Dim bdone = False
        'Dim wdate As Date = DateTime.ParseExact(oHelper.sDateLastScore, "yyyyMMdd", Nothing).ToString("MM\/dd\/yyyy").Trim("0") 'oHelper.sDateLastScore
        'Dim reformatteddt As String = wdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        Dim dvsch As New DataView(dtschedule)
        'dvsch.Sort = "Date desc"
        For Each rv As DataRowView In dvsch
            'check the sch date against the last score date
            cbDatesPlayers.Items.Add(rv(0))
            'lastscore has last score in scores file
            'If rv(0) >= oHelper.sDateLastScore Then
            '    oHelper.sDateLastScore = rv(0)
            '    Exit For
            'End If
        Next

        Dim wkdate2 = Date.ParseExact(cbDatesPlayers.Items.Item(cbDatesPlayers.Items.Count - 1), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo).AddDays(7)
        cbDatesPlayers.Items.Add(wkdate2.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
        wkdate2 = wkdate2.AddDays(7)
        cbDatesPlayers.Items.Add(wkdate2.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
        'cbDatesPlayers.Items.Add(Date.ParseExact(slastweek, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo).AddDays(14))
        cbDatesPlayers.SelectedItem = oHelper.sDateLastScore

        oHelper.dDate = Date.ParseExact(cbDatesPlayers.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)

        'oHelper.BuildControls(gbScoringSgboreCard, 1, 18, "Scorecard")
        'bCloseScreen = False
        'oHelper.bScoreCard = True

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
        btnShowScores.TabStop = False

        tbCP1.Text = 0
        tbCP2.Text = 0
        tbSkins.Text = 0
        tbPurse.Text = 0

        BldScoreCardDataGridFromFile()

        If Not bMatchesSet Then Me.Close()
        '20180221-calculate number of closests to pins there should be
        oHelper.iNumClosests = 0
        For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
            If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
        Next
        bFormLoad = True
        If oHelper.iHoleMarker = 1 Then
            rbFront.Checked = True
        Else
            rbBack.Checked = True
        End If
        If oHelper.bCCLeague Then cbMatches.Checked = True
        ' Set up the delays for the ToolTip.
        toolTipHdcp = oHelper.toolTipHdcp
        toolTipHdcp.AutoPopDelay = 5000
        toolTipHdcp.InitialDelay = 1000
        toolTipHdcp.ReshowDelay = 500
        ' Force the ToolTip text to be displayed whether or not the form is active.
        toolTipHdcp.ShowAlways = True

        '20180202-add event for checkboxes
        'AddHandler dgScores_CellClick() = New System.Windows.Forms.DataGridViewCellEventHandler(AddressOf dgScores_CellClick)
        'AddHandler lv1.SelectedIndexChanged, AddressOf lv1_SelectedIndexChanged
        'AddHandler dgScores.CellClick, AddressOf dgScores_CellClick

        lbStatus.Text = String.Format("Finished Loading Scores from {0}", Main.lbScoresFile.Text)
        oHelper.status_Msg(lbStatus, Me)
        bFormLoad = False
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
                            If .iHdcp >= isi Then
                                'check stroke index 
                                iScore += 1
                                'check for 2 strokes
                                If .iHdcp - .iHoles >= isi Then iScore += 1
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
        Try
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            GetPHdcp = ""

            Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            With dvscores
                '20181014-future try to search internet to find a top row solution
                .RowFilter = String.Format("Player = '{0}' and Date < '{1}'", oHelper.sPlayer, cbDatesPlayers.SelectedItem)
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

            sArray.Add("Points-cPat40nt")
            sArray.Add("Team_Points-cPat40nt")
            sArray.Add("Opponent-cPat170nt")

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
            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvScores.RowFilter = "Date = '" & cbDatesPlayers.SelectedItem & "'"
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
                                sformat = oHelper.cPat40nt
                            Case "cPatHole"
                                sformat = oHelper.cPathole
                            Case "cPat60"
                                sformat = oHelper.cPat60
                            Case "cPat120"
                                sformat = oHelper.cPat120
                            Case "cPatMeth"
                                sformat = oHelper.cPatMeth
                            Case "cPat170"
                                sformat = oHelper.cPat170
                            Case "cPat170nt"
                                sformat = oHelper.cPat170nt
                            Case Else
                                sformat = oHelper.cPat40
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
        Try
            'change mm/dd/yyyy to yyyymmdd
            Dim reformatted1 As String = cbDatesPlayers.SelectedItem
            Dim wkdate As Date = oHelper.rLeagueParmrow("PostSeasonDt")
            Dim reformatted2 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            If reformatted1 >= reformatted2 Then
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

            bMatchesSet = False
            If Not oHelper.getMatchScores(cbDatesPlayers.SelectedItem) Then Exit Sub
            bMatchesSet = True

            dgvScoreDate = cbDatesPlayers.SelectedItem
            'dg.EditMode = DataGridViewEditMode.EditProgrammatically
            'dgScores.AllowUserToAddRows = False
            'dgScores.AllowUserToDeleteRows = False

            '10/1/2017 add code to pull in all scores for a given player
            'check frmScoreCard for event
            '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
            '2 - if double click on a playerevent, get scores for a given player

            If cbDatesPlayers.Text.ToString = "" Then cbDatesPlayers.SelectedItem = oHelper.dDate.ToString("yyyyMMdd")
            If oHelper.iHoles = 0 Then oHelper.iHoles = oHelper.rLeagueParmrow("Holes")

            'oHelper.CalcHoleMarker(cbDatesPlayers.SelectedItem)
            '20181003 - if scores already exist int able, dont use date to determine which 9 were playing, we can swap nines and override schedule
            'oHelper.CalcHoleMarker(sdate)
            Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvscores.RowFilter = String.Format("Date = {0}", oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))

            For Each srow As DataRowView In dvscores
                If srow("Hole1") IsNot DBNull.Value Then
                    If IsNumeric(srow("Hole1")) Then oHelper.iHoleMarker = 1
                ElseIf srow("Hole10") IsNot DBNull.Value Then
                    If IsNumeric(srow("Hole10")) Then oHelper.iHoleMarker = 10
                End If
                If oHelper.iHoleMarker = 0 Then oHelper.CalcHoleMarker(oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
                Exit For
            Next

            '20180130-moved to main 
            'oHelper.MyCourse = oHelper.dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")
            '20180130-calculate number of closests to pins there should be
            oHelper.iNumClosests = 0
            For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
                If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
            Next

            rebuildReadOnly()

            'dgScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells

            dgScores.AllowUserToAddRows = False
            dgScores.AllowUserToDeleteRows = False

            iSkins = 0
            iCTP = 0
            iPurse = 0
            tbSkins.Text = "0"
            tbCP1.Text = "0"
            tbCP2.Text = "0"
            tbPurse.Text = "0"

            Dim dt = New DataTable
            If Not oHelper.CSV2DataTable(dt, oHelper.getLatestFile("*LeagueParms.csv")) Then
                MsgBox(String.Format("File in use, close file and restart {0} {1}", vbCrLf, oHelper.getLatestFile("*LeagueParms.csv")))
                End
            End If

            Dim foundrows As DataRow()
            foundrows = dt.Select(String.Format("EndDate >= #{0}#", oHelper.dDate.ToString("MM/dd/yyyy")))

            For Each row In foundrows
                iPCTP = CInt(row("RolledOverCTP"))
                iPCTP1 = CInt(row("RolledOverCTP1"))
                iPCTP2 = CInt(row("RolledOverCTP2"))
                tbPSkins.Text = CInt(row("RolledOverSkins"))
            Next

            '20180201-save off y/n values
            Dim sSkins As New List(Of String)
            Dim sCTPs As New List(Of String)
            For Each skin As DataGridViewRow In dgScores.Rows
                sSkins.Add(oHelper.convDBNulltoSpaces(skin.Cells("Skins").Value))
                sCTPs.Add(oHelper.convDBNulltoSpaces(skin.Cells("Closest").Value))
            Next

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

            For Each row As DataGridViewRow In dgScores.Rows
                row.Cells("Clear").Value = False
                oHelper.sPlayer = row.Cells("Player").Value
                If sSkins(row.Index) = "Y" Then
                    row.Cells("Skins").Value = True
                    iSkins += oHelper.rLeagueParmrow("Skins")
                    iPurse += oHelper.rLeagueParmrow("Skins")
                End If
                If sCTPs(row.Index) = "Y" Then
                    row.Cells("Closest").Value = True
                    iCTP += 1
                    If oHelper.iNumClosests > 2 Then
                        MsgBox(String.Format("error we only have 2 ctps And inumclosests = {0}", oHelper.iNumClosests))
                        Exit Sub
                    End If
                    iPurse += 1
                End If
                oHelper.ChangeColorsForStrokes(row)
                'pull last score before this to get previous handicap
                Dim dv2Scores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & cbDatesPlayers.SelectedItem & "'"
                dv2Scores.Sort = " Date Desc"
                If dv2Scores.Count > 0 Then row.Cells("Phdcp").Value = dv2Scores(0).Item("Hdcp").ToString
                If row.Cells("Method").Value.ToString = " Net" Then
                    If oHelper.iHoleMarker = 1 Then
                        row.Cells("Out_Gross").Value = CInt(row.Cells("Out_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    Else
                        row.Cells("In_Gross").Value = CInt(row.Cells("In_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    End If
                End If
                oHelper.MakeCellsStrings(row)
            Next

            'iCTP = (iTotCTPPlayers - iTotCTPPlayers Mod 2) / oHelper.iNumClosests
            If iCTP Mod oHelper.iNumClosests <> 0 Then

            End If

            Dim iCTP1 As Int16 = 0
            Dim iCTP2 As Int16 = 0

            tbPCP1.Text = iPCTP1
            tbPCP2.Text = iPCTP2
            If iCTP1 = 0 And iCTP2 = 0 And iCTP > 0 Then
                'iCTP1 = iCTP Mod oHelper.iNumClosests / oHelper.iNumClosests
                iCTP1 = iCTP / oHelper.iNumClosests
                iCTP2 = iCTP1
            End If

            tbCP1.Text = iCTP1
            tbCP2.Text = iCTP2
            tbSkins.Text = iSkins
            tbSkinTot.Text = iSkins
            tbCP1Tot.Text = iPCTP1 + iCTP1
            tbCP2Tot.Text = iPCTP2 + iCTP2
            tbPurse.Text = CInt(tbSkins.Text) + CInt(tbCP1Tot.Text) + CInt(tbCP2Tot.Text)

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

            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")

            If Not cbScoresLocked.Checked Then
                If dgScores.RowCount > 1 Then
                    If Not bSaveBtn Then
                        Dim mbr = MsgBox("Do you want to save scores before you reload this screen", MsgBoxStyle.YesNo)
                        If mbr = MsgBoxResult.No Then
                            cleanupScores()
                            lbStatus.Text = " Scores cleaned up and released"
                            lbStatus.BackColor = Color.LightGreen
                            Exit Sub
                        End If
                    End If
                End If
            Else
                'Exit Sub
            End If

            lbStatus.Text = " Saving scores from this screen..."
            lbStatus.BackColor = Color.Red
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
                Dim arow As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
                'if not found, this is a sub
                If arow Is Nothing Then
                    Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
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
                '20181003-clear out holes and gross/net
                arow("Out_Gross") = ""
                arow("Out_Net") = ""
                arow("In_Gross") = ""
                arow("In_Net") = ""
                For i = 1 To 18
                    arow("Hole" & i) = ""
                Next
                '20181003 clear scores in table before saving from gridview
                For Each cell As DataGridViewCell In row.Cells

                    If cell.OwningColumn.Name = "Team" Then
                        If oHelper.convDBNulltoSpaces(cell.Value).Trim = "" Then
                            lbStatus.Text = String.Format("Invalid Team {0} for Player {1}{2} you need to fix before saving", cell.Value, row.Cells("Player").Value, vbCrLf)
                            lbStatus.BackColor = Color.Red
                            Exit Sub
                        End If
                        'ElseIf cell.OwningColumn.Name = "Group" Then
                        '    If oHelper.convDBNulltoSpaces(cell.Value).Trim = "" Then
                        '        lbStatus.Text = String.Format("Invalid Group {0} for Player {1}{2} defaulting to 0", cell.Value, row.Cells("Player").Value, vbCrLf)
                        '        Exit Sub
                        '    End If
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
                            arow(cell.OwningColumn.Name) = oHelper.RemoveSpcChar(oHelper.convDBNulltoSpaces(cell.Value).Trim)
                        Catch ex As Exception

                        End Try

                    End If
                Next

            Next
            Dim x = ""
            'oHelper.DataTable2XML("dtScores", "Scores")
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtPlayers"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Players.csv")
            lbStatus.Text = "Done saving scores from this screen"
            lbStatus.BackColor = Color.LightGreen

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
            If sCurrColName = "Clear" Then
                dgr.CurrentCell.Value = sOldCellValue
                Exit Sub
            End If
            '20180201-first time through if boolean changed-skins and ctp get edited in another event
            If sCurrColName = "Skins" Or sCurrColName = "Closest" Or sCurrColName = "Clear" Then Exit Sub
            'if no change, then exit 
            If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
            Dim R As DataGridViewRow = dgr.CurrentRow
            If R.Cells("Phdcp").Value Is Nothing Then oHelper.iHdcp = 99

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
                oHelper.iHdcp = GetPHdcp()
            Catch ex As Exception
                oHelper.iHdcp = 99
            End Try

            R.Cells("PHdcp").Value = oHelper.iHdcp
            R.Cells(sCurrColName).Value = oHelper.sPlayer
            Dim MyPlayer As DataRow = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(oHelper.sPlayer)
            If R.Cells("Team").Value = "" Then R.Cells("Team").Value = MyPlayer("Team")

            If rbColors.Checked Then oHelper.bColors = True Else oHelper.bColors = False
            If rbDots.Checked Then oHelper.bDots = True Else oHelper.bDots = False
            oHelper.displayStrokes(R)
            '20180121 fix color blue on subs
            Dim dvplayers As New DataView(oHelper.dsLeague.Tables("dtPlayers"))
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
            If R.Cells("Method").Value Is DBNull.Value Then
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
                    If oHelper.iHdcp <> 99 Then R.Cells("Out_Net").Value = R.Cells("Out_Gross").Value - oHelper.iHdcp
                Else
                    If oHelper.iHdcp <> 99 Then R.Cells("In_Net").Value = R.Cells("In_Gross").Value - oHelper.iHdcp
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
                If oHelper.bAllHolesEntered Then R.Cells("Hdcp").Value = oHelper.GetNewHdcp(R, cbDatesPlayers.SelectedItem)

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
    Private Sub dgScores_DataError(sender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs)
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
    'Private Sub dgScores_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowLeave

    'End Sub
    Private Sub dgScores_RowLeave(sender As Object, e As DataGridViewCellEventArgs) 'Handles dgScores.RowLeave
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex)

        Try
            Dim sSkinsIndexes = oHelper.FCalcSkins(dgScores)
            Dim iSkinpot As Integer = 0
            sSkinsIndexes.Sort()
            Dim iSkinVal As Integer = 0
            Dim iExtra As Integer = 0

            If sSkinsIndexes.Count > 0 Then
                '20180228-Fix extra dollars incorrect, they were missing leftover ctp calc
                iSkinVal = Math.Truncate(iSkinpot / sSkinsIndexes.Count)
            End If

            If sSkinsIndexes.Count > 0 Then
                Dim iprevplayer = 99, iTot = 0.0, iSkins = 0
                Dim bfirst = True
                For Each iplayer As Integer In sSkinsIndexes
                    If iplayer = iprevplayer Then
                        iTot += iSkinVal
                        iSkins += 1
                    Else
                        If bfirst Then
                            bfirst = False
                        Else
                            'dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot.ToString("$.00")
                            dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot
                            dgScores.Rows(iprevplayer).Cells("#Skins").Value = iSkins
                        End If
                        iTot = iSkinVal
                        iSkins = 1
                        iprevplayer = iplayer
                    End If
                Next
                dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot
                dgScores.Rows(iprevplayer).Cells("#Skins").Value = iSkins

            End If

            'put the total players out
            dgScores.Rows(dgScores.Rows.Count - 1).Cells("Group").Value = dgScores.Rows.Count - 1

            Dim iSkinsNum = 0
            Dim iSkinsDol = 0.0
            Dim iCtpsDol = 0.0
            Dim iEarnDol = 0.00
            Dim inumCTP = 0
            tbCP1.Text = 0
            tbCP2.Text = 0
            'Loop through each row and check to see if check box checked for ctp 1/2
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Cells("Player").Value <> "*** Total ***" Then
                    Dim iEarn = 0.0
                    Dim ithisctp = 0.0
                    If IsNumeric(row.Cells("#Skins").Value) Then iSkinsNum += Val(row.Cells("#Skins").Value)
                    If IsNumeric(oHelper.convDBNulltoSpaces(row.Cells("$Skins").Value)) Then
                        iEarn += Val(row.Cells("$Skins").Value)
                        iSkinsDol += Val(row.Cells("$Skins").Value)
                    End If

                    If IsNumeric(row.Cells("$Closest").Value) Then
                        iEarn += row.Cells("$Closest").Value
                        iCtpsDol += row.Cells("$Closest").Value
                        row.Cells("$Closest").Style.BackColor = Color.Gold
                        row.Cells("Player").Style.BackColor = Color.Gold
                        tbCP1.Text += CInt(row.Cells("$Closest").Value)
                        tbCP2.Text += CInt(row.Cells("$Closest").Value)
                    End If
                    row.Cells("$Earn").Value = iEarn
                    iEarnDol += iEarn
                End If
            Next

            'update the total row now
            For Each row In dgScores.Rows
                If row.Cells("Player").Value = "*** Total ***" Then
                    row.Cells("#Skins").Value = iSkinsNum
                    row.Cells("$Skins").Value = iSkinsDol
                    row.Cells("$Earn").Value = iEarnDol
                    row.Cells("$Closest").Value = iCtpsDol
                    row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow
                End If
            Next
            oHelper.bCalcSkins = False
            dgScores.Visible = True
            tbSkins.Text = iSkinsDol
            tbPurse.Text = iSkinsDol + iCtpsDol

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    'Sub resetGross_Net_Hdcp(wrow As DataGridViewRow)
    '    Debug.Print("Current Player - " & oHelper.sPlayer)
    '    Debug.Print(wrow.Cells("Player").Value)
    '    Debug.Print(wrow.Cells("Hole1").Value)
    '    Debug.Print(wrow.Cells("Out_Gross").Value)
    '    Debug.Print(wrow.Cells("Out_Net").Value)
    '    Debug.Print(wrow.Cells("Phdcp").Value)
    '    Debug.Print(wrow.Cells("Hdcp").Value)

    '    wrow.Cells("Hdcp").Value = wrow.Cells("PHdcp").Value
    '    For Each cell In wrow.Cells
    '        If cell.owningcolumn.name = "In_Gross" Or cell.owningcolumn.name = "Out_Gross" Or cell.owningcolumn.name = "In_Net" Or cell.owningcolumn.name = "Out_Net" Then
    '            cell.Value = DBNull.Value
    '        End If
    '    Next
    '    Debug.Print(wrow.Cells("Phdcp").Value)

    '    oHelper.bAllHolesEntered = False
    'End Sub
    Private Sub dgScores_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowEnter
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            'If dgScores.Rows(e.RowIndex).Cells("Method").Value = "" Then dgScores.Rows(e.RowIndex).Cells("Method") = oHelper.BuildScoreCardMethods()

            If dgScores.Rows(e.RowIndex).IsNewRow Then
                dgScores.Rows(e.RowIndex).Cells("Method").Value = oHelper.BuildScoreCardMethods(gbDefMeth)
                If cbSkins.Checked Then dgScores.Rows(e.RowIndex).Cells("Skins").Value = "Y"
                If cbClosest.Checked Then dgScores.Rows(e.RowIndex).Cells("Closest").Value = "Y"
            End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub btnShowScores_Click(sender As Object, e As EventArgs) Handles btnShowScores.Click

        oHelper.LOGIT("--------------------------------------------------------------")
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            lbStatus.Text = "Getting scores for this screen..."
            oHelper.status_Msg(lbStatus, Me)

            SaveScores()
            oHelper.bNoRowLeave = True
            oHelper.bScoresbyPlayer = False
            bSaveBtn = False
            Try
                If cbDatesPlayers.SelectedItem Is Nothing Then
                    oHelper.dDate = Date.ParseExact(cbDatesPlayers.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                Else
                    oHelper.dDate = Date.ParseExact(cbDatesPlayers.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                End If
            Catch ex As Exception
                Dim x = ""
                MsgBox("Bad Date entered, must be yyyymmdd format")
                Me.Cursor = Cursors.Default
                Application.DoEvents()
                Exit Sub
            End Try

            ' getVoiceScores()
            oHelper.iHoles = oHelper.rLeagueParmrow("Holes")

            If rbColors.Checked Then
                oHelper.bColors = True
                oHelper.bDots = False
            End If
            If rbDots.Checked Then
                oHelper.bDots = True
                oHelper.bColors = False
            End If

            BldScoreCardDataGridFromFile()

            oHelper.bNoRowLeave = False

            lbStatus.Text = "Finished Reading Scores"
            oHelper.status_Msg(lbStatus, Me)

            sOldCellValue = ""
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
                        If R.Cells("Phdcp").Value Is Nothing Then oHelper.iHdcp = 99
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
    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDatesPlayers.SelectedIndexChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        '20180220-turn off lock box if last dropdown
        Try
            If cbDatesPlayers.SelectedIndex = cbDatesPlayers.Items.Count - 1 Then cbScoresLocked.Checked = False
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
                    oHelper.iHdcp = row.Cells("Phdcp").Value
                    Scores.Show()
                    oHelper.bScoresbyPlayer = False
                End If
            End If
            'End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Function sf(str As String, flist As String) As String
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim sfl As String
            For Each fld In flist.Split(",")
                sfl = fld
            Next

            sf = String.Format(str, "")
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Function
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
                    oHelper.rLeagueParmrow("ScoresLocked") = "Y"
                    '20180516 - save scores when locking
                    SaveScores()
                End If
            Else
                oHelper.rLeagueParmrow("ScoresLocked") = "N"
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
        If oHelper.bDGSError Then e.Cancel = True

    End Sub

    Private Sub cbMarkPaid_CheckedChanged(sender As Object, e As EventArgs) Handles cbMarkPaid.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            For Each row In dgScores.Rows
                ResetCTPSkins(row.cells("Skins"))
                ResetCTPSkins(row.cells("Closest"))
            Next
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    'Sub uncheckClear(cell As DataGridViewCheckBoxCell)
    '    cell.Value = False
    'End Sub
    Sub ResetCTPSkins(cell As DataGridViewCheckBoxCell)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            'if checkbox was on, turn it off, subtract $$ from amt
            If cell.OwningColumn.Name = "Clear" Then
                '20181022 - work to be done here for clearing scores, then recalcing skins if skins/ctp checkboxes were on
                cell.Value = False
                dgScores.EndEdit()
                Exit Sub
            End If

            If cell.Value = True Then
                recalcCTPSkins(cell)
                cell.Value = False
            Else
                recalcCTPSkins(cell, False)
                cell.Value = True
            End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try
    End Sub
    Sub recalcCTPSkins(cell As DataGridViewCheckBoxCell)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        recalcCTPSkins(cell, False)
    End Sub
    Sub recalcCTPSkins(cell As DataGridViewCheckBoxCell, bSubAmount As Boolean)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim iPurse As Integer = tbPurse.Text
            If Not IsNumeric(tbPSkins.Text) Then tbPSkins.Text = 0
            Dim iTotSkins As Integer = tbPSkins.Text
            Dim iTotCTP As Integer = CInt(tbPCP1.Text) + CInt(tbPCP2.Text) + CInt(tbCP1.Text) + CInt(tbCP2.Text)
            Dim iamt As Integer = 0

            If cell.OwningColumn.Name = "Skins" Then
                If oHelper.bCCLeague Then
                    iamt = 7
                Else
                    iamt = oHelper.rLeagueParmrow(cell.OwningColumn.Name)
                End If
            ElseIf cell.OwningColumn.Name = "Closest" Then
                If oHelper.bCCLeague Then
                    iamt = 3
                Else
                    iamt = 1
                End If
            End If

            If bSubAmount And cell.Value = False Then
                iamt = iamt * -1
            End If

            If cell.Value = True Then
                If cell.OwningColumn.Name = "Skins" Then
                    tbSkins.Text += iTotSkins + iamt
                ElseIf cell.OwningColumn.Name = "Closest" Then
                    tbCP1.Text += iamt / 2
                    tbCP2.Text += iamt / 2
                End If
                iPurse += iamt
                tbPurse.Text = iPurse
                tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
                tbCP1Tot.Text = CInt(tbPCP1.Text) + CInt(tbCP1.Text)
                tbCP2Tot.Text = CInt(tbPCP2.Text) + CInt(tbCP2.Text)
            End If

            dgScores.EndEdit()
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub
    Private Sub cbMatches_CheckedChanged(sender As Object, e As EventArgs) Handles cbMatches.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If cbMatches.Checked Then
                If Not oHelper.bCCLeague Then
                    Dim mbr = MsgBox(String.Format("Are you doing Club Championship and league matches combined?"), MsgBoxStyle.YesNo)
                    If mbr = MsgBoxResult.No Then
                        oHelper.bCCLeague = False
                        cbMatches.Checked = False
                        Exit Sub
                    End If
                    oHelper.bCCLeague = True
                End If
                tbSkins.Text = 0
                tbCP1.Text = 0
                tbCP2.Text = 0
                tbPurse.Text = 0
                For Each row As DataGridViewRow In dgScores.Rows
                    oHelper.sPlayer = row.Cells("Player").Value
                    recalcCTPSkins(row.Cells("Skins"))
                    recalcCTPSkins(row.Cells("Closest"))
                Next

            End If
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub rbFront_CheckedChanged(sender As Object, e As EventArgs) Handles rbFront.CheckedChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
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

    Private Sub tbPSkins_TextChanged(sender As Object, e As EventArgs) Handles tbPSkins.TextChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim tb As TextBox = sender
            If Not IsNumeric(tb.Text) Then
                If tb.Text <> "" Then
                    tb.BackColor = Color.Red
                    MsgBox(String.Format("Must be a number, try again"))
                    Exit Sub
                Else
                    tb.BackColor = Color.White
                    If sOldPSkins = "" Then Exit Sub
                End If
            End If
            tb.BackColor = Color.White

            Dim ctp1 As Integer = 0
            Dim ctp2 As Integer = 0
            If IsNumeric(tbCP1Tot.Text) Then ctp1 = tbCP1Tot.Text
            If IsNumeric(tbCP2Tot.Text) Then ctp2 = tbCP2Tot.Text
            If tbPSkins.Text <> "" Then
                tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
            Else
                tbSkinTot.Text = CInt(tbSkinTot.Text) - sOldPSkins
            End If
            tbPurse.Text = CInt(tbSkinTot.Text) + ctp1 + ctp2
            sOldPSkins = tbPSkins.Text
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub tbPCP1_TextChanged(sender As Object, e As EventArgs) Handles tbPCP1.TextChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim tb As TextBox = sender
            If Not IsNumeric(tb.Text) Then
                If tb.Text <> "" Then
                    tb.BackColor = Color.Red
                    MsgBox(String.Format("Must be a number, try again"))
                    Exit Sub
                Else
                    tb.BackColor = Color.White
                    If sOldPCP1 = "" Then Exit Sub
                End If
            End If
            tb.BackColor = Color.White

            Dim fld1 As Integer = 0
            Dim fld2 As Integer = 0

            If IsNumeric(tbSkinTot.Text) Then fld1 = tbSkinTot.Text
            If IsNumeric(tbCP2Tot.Text) Then fld2 = tbCP2Tot.Text
            If tbPCP1.Text <> "" Then
                tbCP1Tot.Text = CInt(tbPCP1.Text) + CInt(tbCP1.Text)
            Else
                tbCP1Tot.Text = CInt(tbCP1Tot.Text) - sOldPCP1
            End If
            tbPurse.Text = CInt(tbCP1Tot.Text) + fld1 + fld2
            sOldPCP1 = tbPCP1.Text
        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
        End Try

    End Sub

    Private Sub tbPCP2_TextChanged(sender As Object, e As EventArgs) Handles tbPCP2.TextChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim tb As TextBox = sender
            If Not IsNumeric(tb.Text) Then
                If tb.Text <> "" Then
                    tb.BackColor = Color.Red
                    MsgBox(String.Format("Must be a number, try again"))
                    Exit Sub
                Else
                    tb.BackColor = Color.White
                    If sOldPCP2 = "" Then Exit Sub
                End If
            End If
            tb.BackColor = Color.White

            Dim fld1 As Integer = 0
            Dim fld2 As Integer = 0

            If IsNumeric(tbSkinTot.Text) Then fld1 = tbSkinTot.Text
            If IsNumeric(tbCP1Tot.Text) Then fld2 = tbCP1Tot.Text
            If tbPCP2.Text <> "" Then
                tbCP2Tot.Text = CInt(tbPCP2.Text) + CInt(tbCP2.Text)
            Else
                tbCP2Tot.Text = CInt(tbCP2Tot.Text) - sOldPCP2
            End If
            tbPurse.Text = CInt(tbCP2Tot.Text) + fld1 + fld2
            sOldPCP2 = tbPCP2.Text
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

    'Private Sub dgScores_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgScores.CellValidating
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex & " - " & sender.CurrentCell.OwningColumn.Name)
    '    'If sender.CurrentCell.OwningColumn.Name.ToString.StartsWith("Hole") Then
    '    '    If IsNot IsNumeric(sender.value.ToString) Then
    '    '        sender.CurrentCell.style.backcolor = Color.Red
    '    '        e.Cancel = True
    '    '    End If
    '    'End If
    '    If e.Cancel Then
    '        MsgBox("fix red fields before updating a row")
    '    End If
    'End Sub
    Private Sub frmScoreCard_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            rs.ResizeAllControls(Me)

        Catch ex As Exception

        End Try
    End Sub

    Private Sub dgScores_RowHeadersBorderStyleChanged(sender As Object, e As EventArgs) Handles dgScores.RowHeadersBorderStyleChanged

    End Sub
    Private Sub dgScores_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellContentClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex)
        Try
            'calcSkinsCTP(sender.currentcell)
            '20180220-disable checkbox when readonly
            If dgScores.ReadOnly Or e.RowIndex < 0 Then Exit Sub

            Dim R As DataGridViewRow = sender.CurrentRow

            oHelper.sPlayer = R.Cells("Player").Value
            Dim dgc As DataGridViewCell = sender.currentcell
            '20180810 added Clear column
            If dgc.OwningColumn.Name = "Clear" Or dgc.OwningColumn.Name = "Skins" Or dgc.OwningColumn.Name.StartsWith("Closest") Then
                'recalc skins 
                ResetCTPSkins(sender.currentcell)
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
            Else
                Exit Sub
            End If
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