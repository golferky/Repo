Imports System.ComponentModel
Public Class frmScoreCard
    Dim oHelper As New Helper.Controls.Helper
    'fields with (Number) are key fields
    'field width-read only-tabstop-MiddleRight
    'Public Const sScoreCard As String = "Method,Team,Group(3),Player(1),Date(2),Holes,PHdcp,Hdcp,Skins,Closest,$Earn,$Skins,$Closest,#Skins,#Closests,Points,Team_Points,Opponent"
    'Dim sSkinsFields = New List(Of String)("Skins,Closest,$Earn,$Skins,$Closest,#Skins,#Closests".Split(","))
    'Dim sMatchFields = New List(Of String)("Points,Team_Points,Opponent".Split(","))
    Dim fromsizeW As Integer, gvSsizeW As Integer, gvSCsizeW As Integer, gbSCsizeW As Integer
    Dim fromsizeH As Integer, gvSsizeH As Integer, gvSCsizeH As Integer, gbSCsizeH As Integer
    Dim bCloseScreen As Boolean = False
    Dim rs As New Resizer
    Public sOldCellValue As String
    Dim dgvScoreDate As String
    Dim toolTipHdcp As New ToolTip
    Dim bSaveBtn = False
    Dim bMatchesSet = False

    Private Sub ScoreCard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'Me.Size = My.Computer.Screen.WorkingArea.Size
        'Me.WindowState = FormWindowState.Maximized
        '        float widthRatio = Screen.PrimaryScreen.Bounds.Width / 1280;
        'float heightRatio = Screen.PrimaryScreen.Bounds.Height / 800.0F;
        'SizeF Scale() = New SizeF(widthRatio, heightRatio);
        'this.Scale(Scale);
        'foreach(Control control In this.Controls)
        '{
        'Control.Font = New Font("Verdana", Control.Font.SizeInPoints * heightRatio * widthRatio);
        '}

        ' Usage:
        '  Resizing functionality requires only three lines of code on a form:
        '
        '  1. Create a form-level reference to the Resize class:
        '     Dim myResizer as Resizer
        '
        '  2. In the Form_Load event, call the  Resizer class FIndAllControls method:
        '     myResizer.FindAllControls(Me)
        '
        '  3. In the Form_Resize event, call the  Resizer class ResizeAllControls method:
        '     myResizer.ResizeAllControls(Me)
        '
        'rs.FindAllControls(Me)
        'Dim r As New Relative(Me)

        ''width
        'fromsizeW = Me.Size.Width
        'gvSsizeW = dgScores.Size.Width
        'gvSCsizeW = dgScoreCard.Size.Width
        'gbSCsizew = gbScoring.Size.Width
        ''height
        'fromsizeH = Me.Size.Height
        'gvSsizeH = dgScores.Size.Height
        'gvSCsizeH = dgScoreCard.Size.Height
        'gbSCsizeH = gbScoring.Size.Height

        'dgScores.Dispose()
        'frmChooseScores.Font = New Font("tahoma", 15)
        lbStatus.Text = String.Format("Loading Scores")
        lbStatus.BackColor = Color.Red
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()

        dgScores.AllowUserToAddRows = True
        oHelper = Main.oHelper
        oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
        oHelper.dsLeague.Tables("dtPlayers").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtPlayers").Columns("Name")}

        ''get the date of the schedule for this week
        ''just use the Column names which have dates of the schedule table
        ''this loop will compare the league start date and flip the hole marker based on front/back
        '20180126-only build dates for the last date of completed scores
        Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
        dvscores.Sort = "Date desc"
        dvscores.RowFilter = String.Format("Date = {0}", dvscores(0)("Date"))
        Dim lastdate = dvscores(0)("Date")
        'Dim lastdate = CDate(
        '    dvscores(0)("Date").ToString.Substring(0, 4) & "/" &
        '    dvscores(0)("Date").ToString.Substring(4, 2) & "/" &
        '    dvscores(0)("Date").ToString.Substring(6, 2)).AddDays(7).ToString("yyyyMMdd")

        Dim bvalidscores = True
        'For Each row As DataRowView In dvscores
        '    If oHelper.convDBNulltoSpaces(row("Hdcp")).Trim = "" Then
        '        bvalidscores = False
        '        Exit For
        '    End If
        'Next

        Dim dtschedule As New DataTable()
        dtschedule = oHelper.buildSchedule()
        For Each row In dtschedule.Rows
            If row(1) Is DBNull.Value Then
                dtschedule.Rows.Remove(row)
                Continue For
            End If
            Dim wkdate As Date = row("Date")
            Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            row("Date") = reformatted
        Next
        Dim dvsch As New DataView(dtschedule)
        'dvsch.Sort = "Date desc"
        Dim pdate = ""
        cbDatesPlayers.Items.Clear()
        Dim bdone = False
        For Each rv As DataRowView In dvsch
            'check the sch date against the last score date
            cbDatesPlayers.Items.Add(rv(0))
            If bdone Then
                Exit For
            End If
            'lastscore has valid scores
            If rv(0) = lastdate Then bdone = True
        Next

        'oHelper.BuildControls(gbScoringSgboreCard, 1, 18, "Scorecard")
        bCloseScreen = False
        oHelper.bScoreCard = True
        oHelper.bColors = False
        oHelper.bDots = False
        If rbColors.Checked Then oHelper.bColors = True
        If rbDots.Checked Then oHelper.bDots = True

        '20180121-removed this from form
        'gbDefGames.Visible = True
        gbDefMeth.Visible = True
        If oHelper.rLeagueParmrow("Method") = "Net" Then
            rbNet.Checked = True
        ElseIf oHelper.rLeagueParmrow("Method") = "Gross" Then
            rbGross.Checked = True
        End If
        tbCP1.Text = 0
        tbSkins.Text = 0
        tbPurse.Text = 0

        lbStatus.Text = ""
        rbColors.TabStop = False
        rbDots.TabStop = False
        rbGross.TabStop = False
        rbNet.TabStop = False
        rbScore.TabStop = False
        btnShowScores.TabStop = False
        If cbDatesPlayers.Items.Count > 0 Then
            Dim wkdate As Date = oHelper.rLeagueParmrow("startDate")
            Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            If reformatted.Substring(0, 4) = cbDatesPlayers.Items(0).Substring(0, 4) Then
                cbDatesPlayers.SelectedItem = reformatted
            End If
        End If
        'cbDatesPlayers.SelectedIndex = cbDatesPlayers.Items.Count - 1
        cbDatesPlayers.SelectedItem = lastdate
        cbScoresLocked.Checked = False
        BldScoreCardDataGridFromFile()
        If Not bMatchesSet Then Me.Close
        '20180221-calculate number of closests to pins there should be
        oHelper.iNumClosests = 0
        For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
            If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
        Next
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
        lbStatus.Text = String.Format("Finished Loading Scores")
        lbStatus.BackColor = Color.LightGreen
        Me.Cursor = Cursors.Default
        Application.DoEvents()

    End Sub
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        bSaveBtn = False
        'bCloseScreen = True
        'this makes closing form event kick off
        Me.Close()
        oHelper.bScoreCard = False
        Exit Sub
    End Sub

    'this function is called to validate that all holes were entered and to add up scores to net/gross calculations
    Function calcScores(R As DataGridViewRow) As Integer
        calcScores = 0
        oHelper.bAllHolesEntered = True
        Try
            For i = oHelper.iHoleMarker To oHelper.iHoleMarker + oHelper.iHoles - 1
                Dim iScore As String = ""
                iScore = oHelper.RemoveSpcChar(R.Cells("Hole" & i).Value)
                If iScore <> "" Then
                    If Not IsNumeric(iScore) Then
                        oHelper.bAllHolesEntered = False
                        Continue For
                    End If
                    '20180127-decrease for net method
                    If R.Cells("Method").Value.ToString.StartsWith("N") Then
                        Dim isi = oHelper.CalcStrokeIndex("Hole" & i)
                        If oHelper.iHdcp >= isi Then
                            'check stroke index 
                            iScore += 1
                            If oHelper.iHdcp - oHelper.iHoles >= isi Then iScore += 1
                        End If
                    End If
                    calcScores = calcScores + iScore
                Else
                    oHelper.bAllHolesEntered = False
                End If
            Next

        Catch ex As Exception

        End Try
    End Function
    Function GetPHdcp() As String
        GetPHdcp = ""
        Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
        dvscores.RowFilter = "Player = '" & oHelper.sPlayer & "'" & " and Date < '" & cbDatesPlayers.SelectedItem & "'"
        dvscores.Sort = "Date DESC"
        If dvscores.Count > 0 Then
            GetPHdcp = dvscores.Item(0).Item("Hdcp")
        End If

    End Function
    Sub rebuildReadOnly()
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
        dgScores.RowTemplate.Height = 30
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

        If dtScorecard.Rows.Count <> oHelper.rLeagueParmrow("Teams") * 2 Then
            MsgBox(String.Format("Score card file has {0} and should have {1}, fix before continuing", dtScorecard.Rows.Count, oHelper.rLeagueParmrow("Teams") * 2))
            Exit Sub
        End If

        For Each row As DataRow In dtScorecard.Rows
            dgScores.Rows.Add(row.ItemArray)
            oHelper.ChangeColorsForStrokes(dgScores.Rows(dgScores.RowCount - 1))
        Next

    End Sub
    Sub BldScoreCardDataGridFromFile()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try

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

            oHelper.CalcHoleMarker(cbDatesPlayers.SelectedItem)
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

            tbSkins.Text = "0"
            tbCP1.Text = "0"
            tbCP2.Text = "0"
            tbPurse.Text = "0"
            Dim iSkins As Integer = 0, iCTP As Integer = 0, iPurse As Integer = 0
            '20180221-Pull in carry overs from previous weeks
            For i = 0 To oHelper.iNumClosests - 1
                If oHelper.rLeagueParmrow("RolledOverCTP" & i + 1) IsNot DBNull.Value Then iCTP += oHelper.rLeagueParmrow("RolledOverCTP" & i + 1)
            Next
            '20180201-save off y/n values
            Dim sSkins As New List(Of String)
            Dim sCTPs As New List(Of String)
            For Each skin As DataGridViewRow In dgScores.Rows
                sSkins.Add(oHelper.convDBNulltoSpaces(skin.Cells("Skins").Value))
                sCTPs.Add(oHelper.convDBNulltoSpaces(skin.Cells("Closest").Value))
            Next

            'recreate columns as checkboxes
            Dim ncol As New DataGridViewCheckBoxColumn
            ncol.HeaderText = "Skins"
            ncol.Name = "Skins"
            ncol.DataPropertyName = "Skins"
            '20180201-remove hardcoded column 17,18
            ncol.Width = dgScores.Columns(17).Width
            If cbScoresLocked.Checked = True Then ncol.ReadOnly = True
            dgScores.Columns.RemoveAt(17)
            dgScores.Columns.Insert(17, ncol)

            Dim ncol2 As New DataGridViewCheckBoxColumn
            ncol2.HeaderText = "Closest"
            ncol2.Name = "Closest"
            ncol2.DataPropertyName = "Closest"
            ncol2.Width = dgScores.Columns(17).Width
            If cbScoresLocked.Checked = True Then ncol2.ReadOnly = True
            dgScores.Columns.RemoveAt(18)
            dgScores.Columns.Insert(18, ncol2)

            For Each row As DataGridViewRow In dgScores.Rows
                oHelper.sPlayer = row.Cells("Player").Value
                If sSkins(row.Index) = "Y" Then
                    row.Cells("Skins").Value = True
                    iSkins += oHelper.rLeagueParmrow("Skins")
                    tbSkins.Text = iSkins
                    iPurse += oHelper.rLeagueParmrow("Skins")
                    tbPurse.Text = iPurse
                End If
                If sCTPs(row.Index) = "Y" Then
                    row.Cells("Closest").Value = True
                    iCTP += 1
                    tbCP1.Text = iCTP
                    iPurse += 1
                    tbPurse.Text = iPurse
                End If
                oHelper.ChangeColorsForStrokes(row)
                'End If
                'pull last score before this to get previous handicap
                Dim dv2Scores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & cbDatesPlayers.SelectedItem & "'"
                dv2Scores.Sort = " Date Desc"
                If dv2Scores.Count > 0 Then
                    row.Cells("Phdcp").Value = dv2Scores(0).Item("Hdcp").ToString
                End If
                'End If
                'If row.Cells("Out_Net").Value.ToString = "" Then
                If row.Cells("Method").Value.ToString = " Net" Then
                    If oHelper.iHoleMarker = 1 Then
                        row.Cells("Out_Gross").Value = CInt(row.Cells("Out_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    Else
                        row.Cells("In_Gross").Value = CInt(row.Cells("In_Net").Value.ToString) + CInt(row.Cells("PHdcp").Value.ToString)
                    End If
                End If
                oHelper.MakeCellsStrings(row)
            Next
            '20180220-adjust column size
            'dgScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            'dgScores.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
        'oHelper.LOGIT(dgScores.Rows(0).Cells("Player").Value & " -" & dgScores.Rows(0).Cells("Player").ReadOnly)
    End Sub
    Sub SaveScores()
        Try
            '20180201-save when exiting
            'If cbScoresLocked.Checked Then
            '    oHelper.rLeagueParmrow("ScoresLocked") = "Y"
            'Else
            '    oHelper.rLeagueParmrow("ScoresLocked") = "N"
            'End If
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
                    bCloseScreen = False
                    Exit Sub
                End If
                'use ddate because cbdatesplayers.selecteditem has changed the date
                '20180419 - make key index instead of player name
                Dim sKeys() As Object = {row.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
                Dim arow As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
                'if not found, this is a sub
                'Dim bfound = True
                If arow Is Nothing Then
                    Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                    dvscores.RowFilter = String.Format("Date = {0}", oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture))
                    dvscores.Sort = "Partner"
                    For Each srow As DataRowView In dvscores
                        If row.Index = CDbl(srow("Partner")) Then
                            arow = srow.Row
                            Exit For
                        End If
                    Next
                    'arow = oHelper.dsLeague.Tables("dtScores").NewRow
                    'arow("Date") = sKeys(1)
                    'arow("League") = oHelper.rLeagueParmrow("Name")
                    'bfound = False
                End If

                'edit each cell before saving
                For Each cell As DataGridViewCell In row.Cells
                    If cell.OwningColumn.Name = "Team" Then
                        If oHelper.convDBNulltoSpaces(cell.Value).Trim = "" Then
                            lbStatus.Text = String.Format("Invalid Team {0} for Player {1}{2} you need to fix before saving", cell.Value, row.Cells("Player").Value, vbCrLf)
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
                    Else
                        '  20180201 - replace nulls with "" and remove dots
                        arow(cell.OwningColumn.Name) = oHelper.convDBNulltoSpaces(oHelper.RemoveSpcChar(cell.Value).Trim)
                    End If
                Next

                'get the player from the player table, if not found, its a new player(shouldnt happen)
                'Dim spKeys() As Object = {row.Cells("Player").Value}
                'Dim prow As DataRow = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(spKeys)
                'arow("Grade") = prow.Item("Grade")
                ''put a new record from the datagridview
                'If Not bfound Then oHelper.dsLeague.Tables("dtScores").Rows.Add(arow)
            Next
            'oHelper.DataTable2XML("dtScores", "Scores")
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
            oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtPlayers"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Players.csv")
            lbStatus.Text = "Done saving scores from this screen"
            lbStatus.BackColor = Color.LightGreen

        Catch ex As Exception

        End Try

    End Sub

    Private Sub dgScores_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgScores.CellEndEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            oHelper.bDGSError = False
            oHelper.bAllHolesEntered = False
            Dim dgr As DataGridView = sender
            Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name

            '20180201-first time through if boolean changed-skins and ctp get edited in another event
            If sCurrColName = "Skins" Or sCurrColName = "Closest" Then Exit Sub
            '    Dim cell As DataGridViewCheckBoxCell = sender.currentcell
            '    If CBool(sender.currentcell.value) = True Then
            '        calcSkinsCTP(sender.currentcell)
            '    End If
            'End If

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
                'ElseIf sCurrColName = "Skins" Then
                '    editSkins(R, sCurrColName)
                'ElseIf sCurrColName = "Closest" Then
                '    editCTP(R, sCurrColName)
                '20180219-correct spelling on closests - was closest
                'ElseIf sCurrColName = "#Closests" Then
                '    editnumCTP(R, sCurrColName)
            ElseIf sCurrColName = "Team" Then
                editTeam(R, sCurrColName)
                '20180220-do gross on score method
            ElseIf sCurrColName.Contains("Gross") Then
                editGross(R, sCurrColName)
            End If

            If oHelper.bDGSError Then SendKeys.Send("+{TAB}")

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub editPlayer(R As DataGridViewRow, sCurrColName As String)
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
    End Sub
    Sub editMethod(R As DataGridViewRow, sCurrColName As String)

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
    End Sub

    Sub editHoles(R As DataGridViewRow, sCurrColName As String)
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
                    'Dim x As Integer = R.Cells(sCurrColName).Value
                ElseIf Len(sScore) <> 1 And Len(sScore) <> 9 Then
                    Throw New NotSupportedException
                End If
            Catch ex As Exception
                MsgBox(String.Format("Hole value {0}{1} must be 1-9, try again", sScore, vbCrLf))
                'this flag prevents rowleave from being invoked
                oHelper.bDGSError = True
                R.Cells(sCurrColName).Value = sOldCellValue
                Exit Sub
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
            If bgoodScore Then
                If R.Cells("Method").Value.ToString.StartsWith("S") Or R.Cells("Method").Value = "" Then
                    R.Cells("Method").Value = oHelper.BuildScoreCardMethods(gbDefMeth)
                End If
            End If
            If dgScores.Columns.Contains("Out_Gross") Then
                R.Cells("Out_Gross").Value = calcScores(R)
                '99 means this is this guys first score
                If oHelper.iHdcp <> 99 Then R.Cells("Out_Net").Value = R.Cells("Out_Gross").Value - oHelper.iHdcp
            Else
                R.Cells("In_Gross").Value = calcScores(R)
                If oHelper.iHdcp <> 99 Then R.Cells("In_Net").Value = R.Cells("In_Gross").Value - oHelper.iHdcp
            End If

            oHelper.ValidateCell(R.Cells(sCurrColName))

        Catch ex As Exception

        End Try

    End Sub
    Sub editSkins(R As DataGridViewRow, sCurrColName As String)
        '20180201-being done in mouse click event
        'R.Cells(sCurrColName).Value = oHelper.convDBNulltoSpaces(R.Cells("Skins").Value.ToString.ToUpper)
        ''save the league skin amt
        'Dim iamt As Integer = oHelper.rLeagueParmrow("Skins")
        ''if the new value is a y
        'If R.Cells(sCurrColName).Value = "Y" Then
        '    'then check the old value for an n, if it was were good to go, if not zero the amt, nothing changed
        '    If sOldCellValue = "Y" Then iamt = 0
        'ElseIf R.Cells(sCurrColName).Value <> "N" Then
        '    MsgBox(String.Format("{0} must be Y or N, changed to N, change back if needed", sCurrColName))
        '    If sOldCellValue <> "Y" Then
        '        iamt = 0
        '    Else
        '        iamt = iamt * -1
        '    End If
        '    R.Cells(sCurrColName).Value = "N"
        'Else
        '    'the new value is an n, check the old value for an n, if it was zero the amt, nothing changed, else, make the amt negative 
        '    If sOldCellValue <> "Y" Then
        '        iamt = 0
        '    Else
        '        iamt = iamt * -1
        '    End If
        'End If
        ''save it to the textbox
        'Dim ipamt = iamt
        'iamt += tbSkins.Text
        'ipamt += tbPurse.Text
        'tbSkins.Text = iamt
        'tbPurse.Text = ipamt

    End Sub
    Sub editCTP(R As DataGridViewRow, sCurrColName As String)
        'R.Cells(sCurrColName).Value = oHelper.convDBNulltoSpaces(R.Cells(sCurrColName).Value).Trim.ToUpper
        ''save the league closest 
        ''this will change to parm when i get to it
        ''im iamt As Integer = oHelper.rLeagueParmrow(sCurrColName)
        'Dim iamt As Integer = 1
        ''if the new value is a y
        'If R.Cells(sCurrColName).Value = "Y" Then
        '    'then check the old value for an n, if it was were good to go, if not zero the amt, nothing changed
        '    If sOldCellValue = "Y" Then iamt = 0
        'ElseIf R.Cells(sCurrColName).Value <> "N" Then
        '    MsgBox(String.Format("{0} must be Y or N, changed to N, change back if needed", sCurrColName))
        '    If sOldCellValue <> "Y" Then
        '        iamt = 0
        '    Else
        '        iamt = iamt * -1
        '    End If
        '    R.Cells(sCurrColName).Value = "N"
        'Else
        '    'the new value is an n, check the old value for an n, if it was zero the amt, nothing changed, else, make the amt negative 
        '    If sOldCellValue <> "Y" Then
        '        iamt = 0
        '    Else
        '        iamt = iamt * -1
        '    End If
        'End If
        ''save it to the textbox
        'Dim ipamt = iamt
        'iamt += tbCTP.Text
        'ipamt += tbPurse.Text
        'tbCTP.Text = iamt
        'tbPurse.Text = ipamt

    End Sub
    Sub editnumCTP(R As DataGridViewRow, sCurrColName As String)

        Dim iCTP = 0
        For Each row As DataGridViewRow In dgScores.Rows
            Dim cell As DataGridViewCheckBoxCell = row.Cells("#Closests")
            If cell.Value = True Then
                iCTP += 1
            End If
        Next

        If iCTP > oHelper.iNumClosests Then
            MsgBox(String.Format("There are only {0} closests to pins, cant have more, try again", oHelper.iNumClosests))
            oHelper.bDGSError = True
            R.Cells(sCurrColName).Value = sOldCellValue
        End If
    End Sub
    Sub editTeam(R As DataGridViewRow, sCurrColName As String)
        'check the team number before leaving a row
        Dim steam As Integer
        Try
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
            MsgBox(String.Format("This Team ({0}) already has 2 players entered, setting back to {1}", R.Cells("Team").Value, sOldCellValue))
            R.Cells("Team").Value = ""
            oHelper.bDGSError = True
        Catch ex As Exception
            'MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub editrest(R As DataGridViewRow, sCurrColName As String)
        If oHelper.bAllHolesEntered Then
            R.Cells("Hdcp").Value = oHelper.GetNewHdcp(R, cbDatesPlayers.SelectedItem)

            ' Dim cell As DataGridViewCell
            '' Set up the ToolTip text for the datagridviewcell.
            'toolTipHdcp.SetToolTip(cell, "My button1")

            Dim sScore = ""
            Dim sPHdcp = GetPHdcp()
            Try
                If sPHdcp = "" Then R.Cells("PHdcp").Value = R.Cells("Hdcp").Value
            Catch ex As Exception
            End Try
            If dgScores.Columns.Contains("Out_Gross") Then
                sScore = R.Cells("Out_Gross").Value
                R.Cells("Out_Net").Value = sScore - R.Cells("PHdcp").Value
            End If

            If dgScores.Columns.Contains("In_Gross") Then
                sScore = R.Cells("In_Gross").Value
                R.Cells("In_Net").Value = sScore - R.Cells("PHdcp").Value
            End If
            oHelper.ChangeColorsForStrokes(R)
        End If

    End Sub
    Sub editGross(R As DataGridViewRow, sCurrColName As String)
        Dim sScore = ""
        Try
            If dgScores.Columns.Contains("Out_Gross") Then
                sScore = R.Cells("Out_Gross").Value
                If IsNumeric(sScore) Then
                    R.Cells("Out_Net").Value = sScore - R.Cells("PHdcp").Value
                Else
                    Throw New FormatException
                End If
            End If
            If dgScores.Columns.Contains("In_Gross") Then
                sScore = R.Cells("In_Gross").Value
                If IsNumeric(sScore) Then
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
        End Try
    End Sub
    Private Sub dgScores_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowLeave
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex)
        Try
            Dim wrow As DataGridViewRow = sender.currentrow
            oHelper.sPlayer = wrow.Cells("Player").Value
            oHelper.bDGSError = False
            'if flag set to not test row leave, exit without checking
            If oHelper.bNoRowLeave Then
                Exit Sub
            End If

            If wrow.IsNewRow Or oHelper.sPlayer = "" Then
                Exit Sub
            End If
            'chck all the scores with a numeric column heading to see if all holes are entered, if not exit row leave
            For Each score In wrow.Cells
                If score.owningcolumn.name.startswith("Hole") Then
                    If score.value Is Nothing Then Exit Sub
                    If Not IsNumeric(oHelper.RemoveSpcChar(score.value)) Then
                        Exit Sub
                    End If
                End If
            Next

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try


    End Sub

    Private Sub dgScores_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.RowEnter
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex)

        'If dgScores.Rows(e.RowIndex).Cells("Method").Value = "" Then dgScores.Rows(e.RowIndex).Cells("Method") = oHelper.BuildScoreCardMethods()

        If dgScores.Rows(e.RowIndex).IsNewRow Then
            dgScores.Rows(e.RowIndex).Cells("Method").Value = oHelper.BuildScoreCardMethods(gbDefMeth)
            If cbSkins.Checked Then dgScores.Rows(e.RowIndex).Cells("Skins").Value = "Y"
            If cbClosest.Checked Then dgScores.Rows(e.RowIndex).Cells("Closest").Value = "Y"
        End If
    End Sub

    Private Sub btnShowScores_Click(sender As Object, e As EventArgs) Handles btnShowScores.Click

        lbStatus.Text = "Getting scores for this screen..."
        lbStatus.BackColor = Color.Red
        oHelper.LOGIT("--------------------------------------------------------------")
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()

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
        ''20180128-removed, not sure why this is here
        'If dgScores.RowCount >= 24 Then
        '    gbDefGames.Visible = False
        '    gbDefMeth.Visible = False
        'Else("Hdcp"
        '    gbDefGames.Visible = True
        '    gbDefMeth.Visible = True
        'End If
        oHelper.bNoRowLeave = False

        lbStatus.Text = "All Scores read"
        lbStatus.BackColor = Color.LightGreen

        Me.Cursor = Cursors.Default
        Application.DoEvents()
        sOldCellValue = ""
    End Sub
    Sub cleanupScores()
        '20180126-reload scores from csv when not saved
        oHelper.ReloadScores()
    End Sub
    Private Sub btnSkins_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub dgScores_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgScores.CellBeginEdit
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
        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        lbStatus.BackColor = Color.Red
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        Dim dgv = sender
        For Each row As DataGridViewRow In dgv.rows
            'row.Height = 30
            oHelper.ChangeColorsForStrokes(row)
        Next
        lbStatus.Text = String.Format("Resorting of Column {0} done", newColumn.HeaderText)
        lbStatus.BackColor = Color.LightGreen
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub

    Private Sub rbDots_CheckedChanged(sender As Object, e As EventArgs) Handles rbDots.CheckedChanged
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
            'oHelper.displayStrokes(row)
            oHelper.ChangeColorsForStrokes(row)
            '    Dim bvalid = False
            '    If Not row.IsNewRow Then
            '        For i = 1 To 9
            '            If oHelper.RemoveSpcChar(row.Cells("Hole" & i).Value) = "" Then Exit For
            '            bvalid = True
            '        Next
            '    End If
            '    If bvalid Then oHelper.ChangeColorsForStrokes(row)
        Next
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        bSaveBtn = True
        SaveScores()
    End Sub
    Private Sub dgScores_KeyDown(sender As Object, e As KeyEventArgs) Handles dgScores.KeyDown
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
                        'ElseIf sCurrColName = "Skins" Then
                        '    editSkins(R, sCurrColName)
                        'ElseIf sCurrColName = "Closest" Then
                        '    editCTP(R, sCurrColName)
                        'ElseIf sCurrColName = "#Closest" Then
                        '    editnumCTP(R, sCurrColName)
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

    End Sub

    Private Sub dgScores_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles dgScores.UserDeletingRow
        'For Each row As DataGridViewRow In dgScores.Rows
        '    If row.Selected Then
        '        Dim mbr = MsgBox(String.Format("You sure you want to delete this row for {0}", row.Cells("Player").Value), MsgBoxStyle.YesNo)
        '        If mbr = MsgBoxResult.Yes Then
        '            Dim sKeys() As Object = {row.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
        '            Dim arow As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
        '            oHelper.dsLeague.Tables("dtScores").Rows.Remove(arow)
        '            dgScores.Rows.Remove(row)
        '            Exit Sub
        '        Else
        '            e.Cancel = True
        '        End If
        '        Exit For
        '    End If
        'Next
        '20180127 - not allow to delete rows in scorecard
        e.Cancel = True
    End Sub
    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDatesPlayers.SelectedIndexChanged
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        '20180220-turn off lock box if last dropdown
        If cbDatesPlayers.SelectedIndex = cbDatesPlayers.Items.Count - 1 Then
            cbScoresLocked.Checked = False
        End If
    End Sub
    Private Sub dgScores_ColumnDividerDoubleClick(sender As Object, e As DataGridViewColumnDividerDoubleClickEventArgs) Handles dgScores.ColumnDividerDoubleClick
        '20180220-fix auto size cols
        Dim x = ""
        For Each col As DataGridViewColumn In dgScores.Columns
            Dim icolw = col.Width
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            col.Width = icolw
        Next
    End Sub

    Private Sub dgScores_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseClick

        'Try
        '    If e.RowIndex < 0 Then Exit Sub
        '    'if this is a checkbox, check for read only
        '    Dim cell As DataGridViewCell = sender.currentcell

        '    If cell.FormattedValueType.Name = "Boolean" Then
        '        sOldCellValue = cell.EditedFormattedValue
        '        If dgScores.ReadOnly Then
        '            lbStatus.Text = "Scores are locked, unlock to change"
        '            lbStatus.BackColor = Color.Yellow
        '        Else
        '            'Debug.Print(cell.Value & "-Value Before" & cell.EditedFormattedValue)
        '            calcSkinsCTP(cell)
        '        End If
        '    Else
        '        lbStatus.Text = ""
        '        lbStatus.BackColor = Color.White
        '    End If
        'Catch ex As Exception

        'End Try

    End Sub
    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            Dim row As DataGridViewRow = sender.currentrow
            '20180225-fix Mouse click to expand columns
            If e.ColumnIndex = 0 Then
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
            End If
        Catch ex As Exception

        End Try
    End Sub
    Function sf(str As String, flist As String) As String
        Dim sfl As String
        For Each fld In flist.Split(",")
            sfl = fld
        Next

        sf = String.Format(str, "")
    End Function
    Private Sub cbScoresLocked_CheckedChanged(sender As Object, e As EventArgs) Handles cbScoresLocked.CheckedChanged
        lbStatus.Text = String.Format("Start Locking Scores")
        lbStatus.BackColor = Color.Red
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
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
        lbStatus.BackColor = Color.LightGreen
        Me.Cursor = Cursors.Default
        Application.DoEvents()
        cbScoresLocked.AutoCheck = True
    End Sub

    'Private Sub Hdcp_ShowLabel(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgScores.CellMouseEnter
    '    'If sender.currentcell IsNot Nothing Then
    '    '    lbStatus.Text = sender.tooltip.text
    '    'End If

    'End Sub

    'Private Sub Button_ClearLabel(ByVal sender As Object, ByVal _
    'e As System.EventArgs) Handles Button7.MouseLeave,
    'Button8.MouseLeave, Button9.MouseLeave
    '    If lblTip.Text <> "" Then lblTip.Text = ""
    'End Sub

    'Private Sub dgScores_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseClick
    '    Dim x = ""
    'End Sub

    'Private Sub dgScores_MouseClick(sender As Object, e As MouseEventArgs) Handles dgScores.MouseClick
    '    Dim xx = sender.currentcell.owningcolumn.name

    '    Dim x = ""
    'End Sub

    Private Sub dgScores_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgScores.RowValidating
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name & " row - " & e.RowIndex)
        If sender.currentrow.Cells("Player").Value Is DBNull.Value Then
            'e.Cancel = True
            'dgScores.Rows.RemoveAt(e.RowIndex)
            'dgScores.CancelEdit()
        End If
        If oHelper.bDGSError Then
            e.Cancel = True
        End If

        'If oHelper.bexit Then Exit Sub
        'Dim sRoField = New List(Of String)("Method, Player".Split(", "))
        'For Each fld In sRoField
        '    If sender.currentrow.cells(fld).value Is DBNull.Value Then
        '        sender.currentrow.cells(fld).style.backcolor = Color.Red
        '        e.Cancel = True
        '    End If
        'Next
        'For i = oHelper.iHoleMarker To oHelper.iHoleMarker + 8
        '    If sender.currentrow.cells("Hole" & i).value IsNot DBNull.Value Then
        '        sender.currentrow.cells("Hole" & i).style.backcolor = Color.Red
        '        e.Cancel = True
        '    End If
        'Next
        'If e.Cancel Then
        '    MsgBox("fix red fields before updating a row")
        'End If

    End Sub

    Private Sub cbMarkPaid_CheckedChanged(sender As Object, e As EventArgs) Handles cbMarkPaid.CheckedChanged
        For Each row In dgScores.Rows
            calcCTPSkins(row.cells("Skins"))
            calcCTPSkins(row.cells("Closest"))
        Next
    End Sub
    Sub calcCTPSkins(cell As DataGridViewCheckBoxCell)
        Dim iPurse As Integer = tbPurse.Text
        Dim iTotSkins As Integer = tbSkins.Text
        Dim iTotCTP As Integer = tbCP1.Text + tbPCP2.Text
        Dim iamt As Integer = 0
        If cell.OwningColumn.Name = "Skins" Then
            iamt = oHelper.rLeagueParmrow(cell.OwningColumn.Name)
        Else
            iamt = 1
        End If

        If cell.Value = True Then
            cell.Value = False
            iamt = iamt * -1
        Else
            cell.Value = True
        End If
        If cell.OwningColumn.Name = "Skins" Then
            tbSkins.Text = iTotSkins + iamt
        ElseIf cell.OwningColumn.Name = "Closest" Then
            tbCP1.Text = iTotCTP + iamt
        End If
        iPurse += iamt
        tbPurse.Text = iPurse

        dgScores.EndEdit()
    End Sub

    Private Sub frmScoreCard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            SaveScores()
            If lbStatus.BackColor = Color.Red Then
                bCloseScreen = False
                e.Cancel = True
                Exit Sub
            End If
            oHelper.Common_Exit()
        Catch ex As Exception
            MsgBox("Error saving scores, scores not saved")
        End Try
        'If Not bCloseScreen Then
        '    MsgBox("Press Exit to close this form")
        '    e.Cancel = True
        'Else
        '    e.Cancel = False
        'End If
        ''e.Cancel = True
        'btnExit_Click(sender, e)
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
        Try
            'width
            'If gvSsizeW <> 0 Then
            '    dgScores.Width = gvSsizeW + (Me.Size.Width - fromsizeW)
            'End If
            'If gvSCsizeW <> 0 Then
            '    dgScoreCard.Width = gvSCsizeW + (Me.Size.Width - fromsizeW)
            'End If
            'If gbSCsizeW <> 0 Then
            '    gbScoring.Width = gbSCsizeW + (Me.Size.Width - fromsizeW)
            'End If
            'height-not working
            'If gvSsizeH <> 0 Then
            '    dgScores.Height = gvSsizeH + (Me.Size.Height - fromsizeH)
            'End If
            'If gvSCsizeH <> 0 Then
            '    dgScoreCard.Height = gvSCsizeH + (Me.Size.Height - fromsizeH)
            'End If
            'If gbSCsizeH <> 0 Then
            '    gbScoring.Height = gbSCsizeH + (Me.Size.Height - fromsizeH)
            'End If
            rs.ResizeAllControls(Me)

        Catch ex As Exception

        End Try
    End Sub

    Private Sub dgScores_RowHeadersBorderStyleChanged(sender As Object, e As EventArgs) Handles dgScores.RowHeadersBorderStyleChanged

    End Sub
    Private Sub dgScores_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellContentClick
        'calcSkinsCTP(sender.currentcell)
        '20180220-disable checkbox when readonly
        If dgScores.ReadOnly Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub
        Dim dgc As DataGridViewCell = sender.currentcell
        '200180224-ctp
        'If dgc.OwningColumn.Name <> "Skins" And Not dgc.OwningColumn.Name.StartsWith("CTP") Then Exit Sub
        If dgc.OwningColumn.Name <> "Skins" Then
            If Not dgc.OwningColumn.Name.StartsWith("Closest") Then Exit Sub
        End If
        calcCTPSkins(sender.currentcell)
        '20180512 - use calcskinsctp subroutine instead of code below

        'Dim cell As DataGridViewCheckBoxCell = sender.currentcell
        'Dim iPurse As Integer = tbPurse.Text
        'Dim iTotSkins As Integer = tbSkins.Text
        'Dim iTotCTP As Integer = tbCP1.Text + tbPCP2.Text
        'Dim iamt As Integer = 0
        'If cell.OwningColumn.Name = "Skins" Then
        '    iamt = oHelper.rLeagueParmrow(cell.OwningColumn.Name)
        'Else
        '    iamt = 1
        'End If
        ''this checks what it was before you got in here
        ''20180219-fix ckbox and amount on click		[ReadOnly]	True	Boolean

        ''If CBool(sender.currentcell.value) = True Then
        'If cell.Value = True Then
        '    cell.Value = False
        '    iamt = iamt * -1
        'Else
        '    cell.Value = True
        'End If
        'If cell.OwningColumn.Name = "Skins" Then
        '    tbSkins.Text = iTotSkins + iamt
        'ElseIf cell.OwningColumn.Name = "Closest" Then
        '    tbCP1.Text = iTotCTP + iamt
        'End If
        'iPurse += iamt
        'tbPurse.Text = iPurse

        'dgScores.EndEdit()
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
        Else
            If sender.readonly Then MsgBox("This is a calculated field, not editable")
        End If
    End Sub

    'Private Sub dgScores_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellClick
    '    Dim x = ""

    '    'Dim rowsCheckedList As List(Of DataGridViewRow) = GetCheckedRows1(dgScores, "Skins")
    '    'Dim xx = ""
    'End Sub

    'Sub gvGraphics(poDG As DataGridView)
    '    ' Code for printing DataGridView
    '    Dim leftMargin As Integer = 30
    '    Dim position As Integer = leftMargin
    '    Dim yPosition As Integer = 329  ' Change value to change the y-position of the DataGridView
    '    Dim height As Integer = poDG.ColumnHeadersHeight / 2

    '    position = leftMargin
    '    yPosition = yPosition + poDG.ColumnHeadersHeight / 2
    '    For Each dr As DataGridViewColumn In poDG.Columns
    '        Dim totalWidth As Double = dr.Width
    '        e.Graphics.FillRectangle(New SolidBrush(Color.White), New Rectangle(position, yPosition, totalWidth, height))
    '        e.Graphics.DrawRectangle(Pens.Black, New Rectangle(position, yPosition, totalWidth, height))
    '        e.Graphics.DrawString(dr.HeaderText, New Font("Times New Roman", 10, FontStyle.Bold Or FontStyle.Italic), Brushes.Black, position, yPosition, fmt)
    '        position = position + totalWidth
    '    Next

    '    For Each dr As DataGridViewRow In poDG.Rows
    '        position = leftMargin
    '        yPosition = yPosition + poDG.ColumnHeadersHeight / 2
    '        For Each dc As DataGridViewCell In dr.Cells
    '            Dim totalWidth As Double = dc.OwningColumn.Width
    '            e.Graphics.FillRectangle(New SolidBrush(Color.White), New Rectangle(position, yPosition, totalWidth, height))
    '            e.Graphics.DrawRectangle(Pens.White, New Rectangle(position, yPosition, dc.OwningColumn.Width, height))
    '            e.Graphics.DrawString(dc.Value, New Font("Verdana", 8, FontStyle.Regular), Brushes.Black, position, yPosition)
    '            position = position + totalWidth
    '        Next
    '    Next
    'End Sub
    ' Assumes you have created a ToolTip object named ToolTip1
    ' This event fired when the tooltip needs to draw its surface.

End Class
Public Class TeamDuplicate
    Inherits Exception
    Public Overrides ReadOnly Property Message() As String
        Get
            Return "2 Scores for that team have already been entered"
        End Get
    End Property
End Class