Imports System.IO
Public Class Skins
    Dim oHelper As New Helper
    'Dim oHelper = Main.oHelper
    Dim fromsizeW As Integer, gvSsizeW As Integer, gvSCsizeW As Integer, gbSCsizeW As Integer
    Dim bsave As Boolean = False
    Dim rs As New Resizer
    Dim iTotSkinPlayers As Integer = 0
    Dim iTotCTPPlayers As Integer = 0
    Dim iEachClosestAmt As Integer = 0
    Dim sdate As String = ""
    'this is a bucket for saving pointers into the grid for skin winners
    Dim sSkinsIndexes As List(Of String)
    Dim sCTPs As List(Of String)
    Dim sSkinflds As String = ""
    Dim iSkinpot As Integer
    Dim dvScores As DataView

    Private Sub Skins_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        'cant understand why i had to do this because main.helper has all the tables but i get an error
        '-----Unable to cast object of type 'System.Data.DataTable' to type 'dtCoursesDataTable'.
        oHelper = Main.oHelper
        'get the date of the schedule for this week
        'just use the Column names which have dates of the schedule table
        'this loop will compare the league start date and flip the hole marker based on front/back
        cbDatesPlayers.Items.Clear()

        If oHelper.bsch Then
            For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                Dim wkdate As DateTime = col.ColumnName
                'Dim wkdate As DateTime = DateTime.ParseExact(col.ColumnName, "MM/dd/yy", Globalization.CultureInfo.InvariantCulture)
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                cbDatesPlayers.Items.Add(reformatted)
            Next
        End If
        cbDatesPlayers.SelectedItem = oHelper.dDate.ToString("yyyyMMdd")
        cbDatesPlayers.Text = oHelper.sDateLastScore

        dgScores.Visible = False
        'oHelper.MyCourse = oHelper.dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")
        bsave = False
        If rbColors.Checked Then oHelper.bColors = True
        If rbDots.Checked Then oHelper.bDots = True

        If oHelper.rLeagueParmrow("SkinFmt") = "Handicap" Then
            lbSkinFormat.Text = "Skins with Handicap"
        Else
            lbSkinFormat.Text = "Scratch Skins"
        End If
        'rs.FindAllControls(Me)
        lbStatus.Text = ""
        '20180130-check for locked scores
        If oHelper.convDBNulltoSpaces(oHelper.rLeagueParmrow("ScoresLocked")) = "Y" Then
            btnSave.Visible = True
        Else
            btnSave.Visible = False
        End If
        oHelper.bScreenChanged = False
        '20180318-add handler for checking dots
        'AddHandler rbDots.CheckedChanged, AddressOf checkDotsColors
        'Handles rbDots.CheckedChanged
        For Each field In oHelper.cSkinsFields.Split(",")
            sSkinflds = sSkinflds + field.Substring(0, field.IndexOf("-")) & ","
        Next

        btnSkins_Click(sender, e)
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Try
            oHelper.Common_Exit()
        Catch ex As Exception
            MsgBox("Error updating scores, better check them")
        End Try

        'this causes skins_FormCLosing 
        Me.Close()
    End Sub
    Dim fromsizeH As Integer, gvSsizeH As Integer, gvSCsizeH As Integer, gbSCsizeH As Integer
    Private Sub Skins_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SaveScores()
    End Sub
    Private Sub btnSkins_Click(sender As Object, e As EventArgs) Handles btnSkins.Click
        If oHelper.iHoles = 0 Then oHelper.iHoles = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0).Item("Holes")
        sdate = cbDatesPlayers.Text.ToString

        If sdate = "" Then
            MsgBox("Please enter Or select a date")
            Exit Sub
        End If
        '20181003 - if scores already exist int able, dont use date to determine which 9 were playing, we can swap nines and override schedule
        'oHelper.CalcHoleMarker(sdate)
        Dim dvscores As New DataView(oHelper.dsLeague.Tables("dtScores"))
        dvscores.RowFilter = String.Format("Date = {0}", cbDatesPlayers.SelectedItem)
        If dvscores.Count = 0 Then
            MsgBox(String.Format("No scores for {0}", cbDatesPlayers.SelectedItem))
            Exit Sub
        End If
        For Each srow As DataRowView In dvscores
            If srow("Hole1") IsNot DBNull.Value Then
                If IsNumeric(srow("Hole1")) Then oHelper.iHoleMarker = 1
            ElseIf srow("Hole10") IsNot DBNull.Value Then
                If IsNumeric(srow("Hole10")) Then oHelper.iHoleMarker = 10
            End If
            Exit For
        Next
        SaveScores()

        'build the grid
        BldSkinsDataGridFromCSV()

        'For i = 0 To oHelper.iNumClosests
        '    sSkinflds = sSkinflds + "CTP_" & (i + 1) & ","
        'Next

        CalcSkins()
        'NewCalcSkins()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        bsave = True
        SaveScores()
    End Sub

    'Private Sub rbDots_CheckedChanged(sender As Object, e As EventArgs) Handles rbDots.CheckedChanged
    '    checkDotsColors
    'End Sub

    Private Sub dgScores_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.ColumnHeaderMouseClick
        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)

        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim dgv = sender
        oHelper.bReorderCols = True
        oHelper.bCalcSkins = True
        If rbColors.Checked Then
            For Each row As DataGridViewRow In dgv.rows
                oHelper.ChangeColorsForStrokes(row)
            Next
        End If

        '20180222-expand #closests to track each individual hole for carry overs
        For Each field In oHelper.cSkinsFields.Split(",")
            sSkinflds = sSkinflds + field.Substring(0, field.IndexOf("-")) & ","
        Next

        Dim ictpctr = 1
        For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
            If oHelper.MyCourse(0)("Hole" & i) = 3 Then
                sSkinflds = sSkinflds + "CTP_" & ictpctr & ","
                ictpctr += 1
            End If
        Next

        oHelper.bReorderCols = False
        oHelper.bCalcSkins = False
        lbStatus.Text = String.Format("Finished Resorting of Column {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
    End Sub

    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        If e.ColumnIndex = 0 Then
            Dim cell As DataGridViewTextBoxCell = sender.currentcell
            If cell.OwningColumn.Name = "Player" Then
                If cell.Value <> "*** Total ***" Then
                    Dim mbResult As MsgBoxResult = MsgBox("List all scores for for " & cell.Value & "?", MsgBoxStyle.YesNo)
                    If mbResult = MsgBoxResult.Yes Then
                        oHelper.bScoresbyPlayer = True
                        oHelper.sPlayer = cell.Value
                        Scores.Show()
                        oHelper.bScoresbyPlayer = False
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub dgScores_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgScores.SortCompare
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
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
                'Debug.Print(sc1 & "-" & sc2)
                e.SortResult = If(CInt(sc1) < CInt(sc2), -1, 1)
            Else
                e.SortResult = If(CStr(sc1) < CStr(sc2), -1, 1)
            End If

            e.Handled = True
        Catch ex As Exception
        End Try
    End Sub
    Sub NewCalcSkins()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        lbStatus.Text = String.Format("Calculating Skins")
        oHelper.status_Msg(lbStatus, Me)
        'this is for change colors routine and if no scores, no sense calculating anything
        If dgScores.Rows.Count = 1 Then
            oHelper.bCalcSkins = False
            lbStatus.Text = "Finished, No skins or CTP entered, nothing to show"
            oHelper.status_Msg(lbStatus, Me)
            Exit Sub
        End If

        Dim dtSkins As DataTable
        dtSkins = New DataTable
        oHelper.CreateColumn("Hole", dtSkins)
        oHelper.CreateColumn("Name", dtSkins)
        oHelper.CreateColumn("Score", dtSkins)
        oHelper.CreateColumn("Scores", dtSkins)

        dtSkins.PrimaryKey = New DataColumn() {dtSkins.Columns("Hole")}
        For Each col As DataGridViewColumn In dgScores.Columns
            If IsNumeric(col.HeaderText) Then
                Dim newrow As DataRow = dtSkins.NewRow
                newrow("Hole") = col.HeaderText
                dtSkins.Rows.Add(newrow)
            End If
        Next
        For Each row As DataGridViewRow In dgScores.Rows
            If row.Cells("Player").Value.ToString.Contains("***") Then Continue For
            For Each hole As DataGridViewCell In row.Cells
                If hole.OwningColumn.Name.StartsWith("Hole") Then
                    Dim sKeys() As Object = {hole.OwningColumn.Name.Substring(4)}
                    Dim dr As DataRow = dtSkins.Rows.Find(sKeys)
                    If dr IsNot Nothing Then
                        If dr("Score") Is DBNull.Value Then
                            dr("Name") = row.Index
                            dr("Score") = hole.Value.ToString.Replace(ChrW(&H25CF), "")
                            dr("Scores") = 1
                        ElseIf dr("Score") > hole.Value.ToString.Replace(ChrW(&H25CF), "") Then
                            dr("Name") = row.Index
                            dr("Score") = hole.Value.ToString.Replace(ChrW(&H25CF), "")
                            dr("Scores") = 1
                        ElseIf dr("Score") = hole.Value.ToString.Replace(ChrW(&H25CF), "") Then
                            dr("Name") = ""
                            dr("Score") = hole.Value.ToString.Replace(ChrW(&H25CF), "")
                            dr("Scores") += 1
                        End If
                    End If
                End If
            Next
        Next
        Dim x = ""
    End Sub
    Sub CalcSkins()
        'this sub uses the datagridview fields to calculate skins and closests to the pin
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        lbStatus.Text = String.Format("Calculating Skins")
        oHelper.status_Msg(lbStatus, Me)

        'this is for change colors routine and if no scores, no sense calculating anything
        If dgScores.Rows.Count = 1 Then
            oHelper.bCalcSkins = False
            lbStatus.Text = "Finished, No skins or CTP entered, nothing to show"
            oHelper.status_Msg(lbStatus, Me)
            Exit Sub
        End If

        oHelper.bCalcSkins = True
        sSkinsIndexes = oHelper.FCalcSkins(dgScores)
        sSkinsIndexes.Sort()
        Dim iSkinVal As Integer = 0
        Dim iExtra As Integer = 0

        If sSkinsIndexes.Count > 0 Then
            '20180228-Fix extra dollars incorrect, they were missing leftover ctp calc
            iSkinVal = Math.Truncate(iSkinpot / sSkinsIndexes.Count)
            If Not oHelper.bCCLeague Then
                iExtra = iTotCTPPlayers * 1 - (iEachClosestAmt * oHelper.iNumClosests)
                iExtra = iExtra + (iSkinpot - (iSkinVal * sSkinsIndexes.Count))
            End If
        Else
            iExtra = 0
            tbLOSkins.Text = iSkinpot
            tbLOPurse.Text += iSkinpot
        End If

        tbExtra.Text = iExtra
        '20180228-Fix extra dollars incorrect, they were missing leftover ctp calc
        tbLOPurse.Text += iExtra + tbLOCP1.Text + tbLOCP2.Text

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
                    If IsNumeric(row.Cells("CTP_1").Value) Then tbCP1.Text += CInt(row.Cells("$Closest").Value)
                    If IsNumeric(row.Cells("CTP_2").Value) Then tbCP2.Text += CInt(row.Cells("$Closest").Value)
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

        lbStatus.Text = String.Format("Finished Calculating skins")
        oHelper.status_Msg(lbStatus, Me)
        '20180307-commented out for testing
        dgScores.Sort(dgScores.Columns("$Earn"), System.ComponentModel.ListSortDirection.Descending)
        'set color switches for helper routine
        'checkDotsColors()

    End Sub
    Sub SaveScores()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If Not oHelper.bScreenChanged Then Exit Sub
            If oHelper.convDBNulltoSpaces(oHelper.rLeagueParmrow("ScoresLocked")) = "Y" Then
                If dgScores.RowCount > 1 Then
                    If Not bsave Then
                        Dim mbr = MsgBox("Do you want to save skin results before you exit this screen", MsgBoxStyle.YesNo)
                        If mbr = MsgBoxResult.Yes Then bsave = True
                    End If
                    If bsave Then
                        lbStatus.Text = "Saving scores from this screen..."
                        lbStatus.BackColor = Color.Red
                        For Each row In dgScores.Rows
                            UpdateScoresFromDataGrid(row)
                        Next
                        oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Scores.csv")
                        'save extra amounts to league parm table
                        Dim sKeys() As Object = {oHelper.rLeagueParmrow("Name"), oHelper.rLeagueParmrow("StartDate")}
                        oHelper.dsLeague.Tables("dtLeagueParms").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtLeagueParms").Columns("Name"), oHelper.dsLeague.Tables("dtLeagueParms").Columns("StartDate")}
                        Dim dr As DataRow = oHelper.dsLeague.Tables("dtLeagueParms").Rows.Find(sKeys)
                        dr("RolledOverSkins") = tbLOSkins.Text
                        dr("RolledOverCTP1") = tbLOCP1.Text
                        dr("RolledOverCTP2") = tbLOCP2.Text
                        dr("RolledOverDate") = cbDatesPlayers.Text
                        dr("ExtraMoney") = tbExtra.Text
                        oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
                        lbStatus.Text = "Done saving scores from this screen"
                        lbStatus.BackColor = Color.LightGreen
                        bsave = False
                    End If
                End If
                Try
                    oHelper.dDate = Date.ParseExact(cbDatesPlayers.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                Catch ex As Exception

                End Try
            End If

        Catch ex As Exception
            MsgBox("Error updating scores, better check them")
        End Try
    End Sub
    Sub BldSkinsDataGridFromCSV()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            'dgscores.EditMode = DataGridViewEditMode.EditProgrammatically
            dgScores.AllowUserToAddRows = False
            dgScores.AllowUserToDeleteRows = False

            '20180130-calculate how many closests to pins there should be
            oHelper.iNumClosests = 0
            For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
                If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
            Next
            '10/1/2017 add code to pull in all scores for a given player
            'check frmScoreCard for event
            '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
            '2 - if double click on a playerevent, get scores for a given player

            dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))

            dvScores.RowFilter = "Date = '" & sdate & "'"
            dvScores.RowFilter = dvScores.RowFilter & " and (Skins = 'Y' or Closest = 'Y')"

            Dim newRow As DataRowView = dvScores.AddNew()
            newRow("Player") = "*** Total ***"

            'create array from above defined fields we want out of scorecard
            Dim sArray = New List(Of String)
            sArray.AddRange(oHelper.cBaseScoreCard.Split(","))

            '20180222-expand #closests to track each individual hole for carry overs
            Dim ictpctr = 1, swctp = ""
            For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
                If oHelper.MyCourse(0)("Hole" & i) = 3 Then
                    swctp = swctp + "CTP " & ictpctr & "-cPat40nt,"
                    ictpctr += 1
                End If
            Next

            sArray.AddRange(oHelper.cSkinsFields.Replace("#Closests-cPat40nt", swctp.Trim(",")).Split(","))

            Dim sColFormat = New List(Of String)
            Dim sScoreCardforDGV = ""
            'strip parenthesis and add gross/net for In/Out
            'fields can have a pattern associated for cell length, centering,

            For Each parm As String In sArray
                'set detault pattern
                Dim sPat = oHelper.cPat40
                Dim sParm = ""

                'skip hdcp
                If parm.StartsWith("Hdcp") Then Continue For

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
                ElseIf sParm = "Date" Then
                    Continue For
                End If
                sScoreCardforDGV = sScoreCardforDGV + sParm + ","
                sColFormat.Add(sPat)
            Next
            'replace spaces with underscores for csv column matchups
            sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")
            '20180120-remove method from skins screen, scores always reflect that in league parm
            'sScoreCardforDGV = sScoreCardforDGV.Replace("Method,", "")
            Dim dtScorecard As DataTable = dvScores.ToTable(True, sScoreCardforDGV.Split(",").ToArray)
            Try
                '20180228-moved to change datatable instead of grid and make all scores match the skins method(scratch/handicap), then remove method for skin grid
                If oHelper.rLeagueParmrow("SkinFmt") = "Handicap" Then
                    For Each row As DataRow In dtScorecard.Rows
                        If Not row("Player").ToString.StartsWith("*** Total") Then
                            '20180829 - null method on noshows
                            If row("Method") IsNot DBNull.Value Then
                                If row("Method") = "Gross" Then
                                    For i = 1 To 9
                                        Dim cell = row("Hole" & i - 1 + oHelper.iHoleMarker)
                                        'check stroke index
                                        Dim isi As Integer = oHelper.CalcStrokeIndex(i - 1 + oHelper.iHoleMarker)
                                        If CInt(row("pHdcp")) >= isi Then
                                            Dim x = CInt(row("pHdcp")) - 9
                                            If CInt(row("pHdcp")) - 9 >= isi Then
                                                cell = cell - 2 & ChrW(&H25CF) & ChrW(&H25CF)
                                            ElseIf CInt(row("pHdcp")) >= isi Then
                                                Try
                                                    cell = cell - 1 & ChrW(&H25CF)
                                                Catch ex As Exception
                                                    Dim xz = ""
                                                End Try
                                            End If
                                        End If
                                        row("Hole" & i - 1 + oHelper.iHoleMarker) = cell
                                    Next
                                End If
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                MsgBox(ex.Message & vbCrLf & ex.StackTrace)
            End Try

            dtScorecard.Columns.Remove("Method")

            dgScores.Columns.Clear()

            oHelper.bNoRowLeave = True
            dgScores.RowTemplate.Height = 20

            dgScores.DefaultCellStyle.Font = New Font("Tahoma", 12)
            'dgScores.RowTemplate.Height = 30 'row.Height = 30
            'With dg
            '    .DataSource = dtScorecard
            'End With

            Dim arr As Array = sScoreCardforDGV.Split(", ").ToArray
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

            Dim sctps As New List(Of String)
            For i = 0 To oHelper.iNumClosests - 1
                sctps.Add(0)
            Next

            Dim iwinner As Int16 = 1 ' dont make relative to 0
            iTotCTPPlayers = 0
            iTotSkinPlayers = 0

            'this moves the rows into the grid
            For Each row As DataRow In dtScorecard.Rows
                If Not row("Player").ToString.StartsWith("*** Total") Then
                    If row.Item("Closest") = "Y" Then iTotCTPPlayers += 1
                    If row.Item("Skins") = "Y" Then iTotSkinPlayers += 1
                    Try
                        If IsNumeric(row.Item("CTP_1")) Then sctps(0) = iwinner
                        If IsNumeric(row.Item("CTP_2")) Then sctps(1) = iwinner

                    Catch ex As Exception

                    End Try
                    iwinner += 1
                End If
                row("$Skins") = ""
                row("$Earn") = ""
                row("#Skins") = ""
                dgScores.Rows.Add(row.ItemArray)
            Next

            'plug rolled over amounts
            iEachClosestAmt = (iTotCTPPlayers - iTotCTPPlayers Mod 2) / oHelper.iNumClosests
            Dim ictp1 As Int16 = 0
            Dim ictp2 As Int16 = 0

            'change mm/dd/yyyy to yyyymmdd
            Dim reformatted1 As String = cbDatesPlayers.SelectedItem
            Dim wkdate As Date = oHelper.rLeagueParmrow("PostSeasonDt")
            Dim reformatted2 As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            If reformatted1 >= reformatted2 Then
                oHelper.bCCLeague = True
            Else
                oHelper.bCCLeague = False
            End If

            '20180923 - hardcode skins for club championship to be 10
            If oHelper.bCCLeague Then
                iEachClosestAmt = (iTotCTPPlayers * 3) / 2
                iSkinpot = iTotSkinPlayers * 7
            Else
                If oHelper.rLeagueParmrow("RolledOverDate") IsNot DBNull.Value Then
                    If oHelper.rLeagueParmrow("RolledOverDate") < cbDatesPlayers.SelectedItem Then
                        iSkinpot = oHelper.convDBNulltoSpaces(oHelper.rLeagueParmrow("RolledOverSkins"))
                        If IsNumeric(oHelper.rLeagueParmrow("RolledOverCTP1")) Then ictp1 = oHelper.rLeagueParmrow("RolledOverCTP1")
                        If IsNumeric(oHelper.rLeagueParmrow("RolledOverCTP2")) Then ictp2 = oHelper.rLeagueParmrow("RolledOverCTP2")
                    End If
                End If
                iSkinpot += oHelper.rLeagueParmrow("Skins") * iTotSkinPlayers
            End If

            tbLOCP1.Text = ictp1
            tbLOCP2.Text = ictp2

            tbSkins.Text = iSkinpot
            tbPurse.Text = 0
            tbLOSkins.Text = 0
            tbCP1.Text = 0
            tbCP2.Text = 0
            tbLOPurse.Text = 0

            'this loop gets previous handicap
            For Each row As DataGridViewRow In dgScores.Rows
                'oHelper.MakeCellsStrings(row)
                Dim dv2Scores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & sdate & "'"
                dv2Scores.Sort = "Date Desc"
                If dv2Scores.Count > 0 Then row.Cells("PHdcp").Value = dv2Scores(0).Item("Hdcp").ToString
            Next

            'recreate columns as checkboxes
            Dim ncol As New DataGridViewCheckBoxColumn
            ncol.HeaderText = "CTP 1"
            ncol.Name = "CTP_1"
            ncol.DataPropertyName = "CTP1"
            '20180201-remove hardcoded column 22,23
            ncol.Width = dgScores.Columns(21).Width
            dgScores.Columns.RemoveAt(21)
            dgScores.Columns.Insert(21, ncol)

            Dim ncol2 As New DataGridViewCheckBoxColumn
            ncol2.HeaderText = "CTP 2"
            ncol2.Name = "CTP_2"
            ncol2.DataPropertyName = "CTP2"
            ncol2.Width = dgScores.Columns(22).Width
            dgScores.Columns.RemoveAt(22)
            dgScores.Columns.Insert(22, ncol2)

            'loop through each row looking for player who won a ctp
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Cells("Player").Value <> "*** Total ***" Then
                    Dim ictp = 0
                    If row.Index = sctps(0) - 1 Then
                        row.Cells("CTP_1").Value = True
                        ictp += iEachClosestAmt
                        row.Cells("$Closest").Value = iEachClosestAmt
                        'tbLOCP1.Text -= iEachClosestAmt
                    End If
                    If row.Index = sctps(1) - 1 Then
                        row.Cells("CTP_2").Value = True
                        ictp += iEachClosestAmt
                        row.Cells("$Closest").Value = iEachClosestAmt
                        'tbLOCP2.Text -= iEachClosestAmt
                    End If
                End If
            Next

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Sub frmSkins_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
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
    Sub UpdateScoresFromDataGrid(row As DataGridViewRow)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
            If row.Cells("Player").Value Is Nothing Or
             row.Cells("Player").Value.contains("*** Total") Then
                Exit Sub
            End If

            'find the score for this player / date
            Dim sKeys() As Object = {row.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
            Dim dr As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
            For Each cell As DataGridViewCell In row.Cells
                'If oHelper.cSkinsFields.Contains(cell.OwningColumn.Name) Then
                If sSkinflds.Contains(cell.OwningColumn.Name) Then
                    If cell.Value IsNot DBNull.Value Then
                        Try
                            If cell.OwningColumn.Name.StartsWith("CTP") Then
                                If cell.Value = True Then dr(cell.OwningColumn.Name) = iEachClosestAmt
                            Else
                                dr(cell.OwningColumn.Name) = oHelper.RemoveSpcChar(cell.Value)
                            End If

                        Catch ex As Exception

                        End Try
                    End If
                End If
            Next

        Catch ex As Exception
            MsgBox(oHelper.GetExceptionInfo(ex))
            'MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
            'Dim st As New StackTrace(True)
            'st = New StackTrace(ex, True)
            'MessageBox.Show("Line: " & st.GetFrame(0).GetFileLineNumber().ToString, "Error")
        End Try
    End Sub

    Private Sub dgScores_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellEndEdit
        Dim dgr As DataGridView = sender
        Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name
        If sCurrColName.StartsWith("CTP") Then
            Dim iCTP = 0
            For Each row As DataGridViewRow In dgScores.Rows
                Dim cell As DataGridViewCheckBoxCell = row.Cells(sCurrColName)
                If cell.Value = True Then iCTP += 1
            Next

            If iCTP > 1 Then
                MsgBox(String.Format("There are only {0} closests to pins, cant have more, try again", oHelper.iNumClosests))
                oHelper.bDGSError = True
            End If
        End If
    End Sub
    Private Sub dgScores_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgScores.CellContentClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        If e.RowIndex < 0 Then Exit Sub
        Dim dgc As DataGridViewCell = sender.currentcell
        '20180224-ctp
        If Not dgc.OwningColumn.Name.StartsWith("CTP") Then Exit Sub
        If sender.currentrow.cells("Player").value = "*** Total ***" Then Exit Sub

        Dim cell As DataGridViewCheckBoxCell = sender.currentcell
        Dim iamt As Integer = iEachClosestAmt

        'add or subtract the amts from purse
        If cell.Value = True Then iamt = iamt * -1

        'figure out extra money from uneven amount of players
        'iExtra += iTotCTPPlayers - CInt(tbLOCP1.Text) - (sCTP.Count * iclosests)

        'dgScores.Rows(dgScores.Rows.Count - 1).Cells("$Earn").Value = iEarnytd

        If iamt < 0 Then
            'turn off flag for this ctp
            dgScores.Rows(e.RowIndex).Cells(dgc.OwningColumn.Name).Value = False
            dgScores.Rows(e.RowIndex).Cells("$Closest").Style.BackColor = Color.White
            If Val(dgScores.Rows(e.RowIndex).Cells("$Skins").Value) = 0 Then dgScores.Rows(e.RowIndex).Cells("Player").Style.BackColor = Color.White
        Else
            dgScores.Rows(e.RowIndex).Cells(dgc.OwningColumn.Name).Value = True
            dgScores.Rows(e.RowIndex).Cells("$Closest").Style.BackColor = Color.Gold
            dgScores.Rows(e.RowIndex).Cells("Player").Style.BackColor = Color.Gold
        End If

        If Val(oHelper.convDBNulltoSpaces(dgScores.Rows(e.RowIndex).Cells("$Closest").Value)) > 0 Then
            dgScores.Rows(e.RowIndex).Cells("$Closest").Value += iamt
        Else
            dgScores.Rows(e.RowIndex).Cells("$Closest").Value = iamt
        End If

        If Val(oHelper.convDBNulltoSpaces(dgScores.Rows(e.RowIndex).Cells("$Earn").Value)) > 0 Then
            dgScores.Rows(e.RowIndex).Cells("$Earn").Value += iamt
        Else
            dgScores.Rows(e.RowIndex).Cells("$Earn").Value = iamt
        End If

        If cell.OwningColumn.Name = "CTP_1" Then
            tbCP1.Text += iamt
            tbLOCP1.Text -= iamt
            tbLOPurse.Text -= iamt
        ElseIf cell.OwningColumn.Name = "CTP_2" Then
            tbCP2.Text += iamt
            tbLOCP2.Text -= iamt
            tbLOPurse.Text -= iamt
        End If
        tbPurse.Text += iamt

        'now loop through and adjust totals for the unchecked
        If cell.Value = True Then
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Cells("Player").Value = "*** Total ***" Then
                    row.Cells("$Closest").Value += iamt
                    row.Cells("$Earn").Value += iamt
                    Continue For
                End If
                If e.RowIndex <> row.Index Then
                    'this amount needs to be cleared if it used to be checked
                    If Val(oHelper.convDBNulltoSpaces(row.Cells("$Closest").Value)) > 0 Then
                        If row.Cells(dgc.OwningColumn.Name).Value = True Then
                            row.Cells(dgc.OwningColumn.Name).Value = False
                            row.Cells("$Closest").Value -= iamt
                            row.Cells("$Earn").Value -= iamt
                            'if we were changing 1, then adjust 1 else 2
                            If cell.OwningColumn.Name = "CTP_1" Then
                                tbCP1.Text -= iamt
                                tbLOCP1.Text += iamt
                                tbLOPurse.Text += iamt
                            Else
                                tbCP2.Text -= iamt
                                tbLOCP2.Text += iamt
                                tbLOPurse.Text += iamt
                            End If

                            tbPurse.Text -= iamt

                            For Each trow In dgScores.Rows
                                If trow.Cells("Player").Value = "*** Total ***" Then
                                    trow.Cells("$Closest").Value -= iamt
                                    trow.Cells("$Earn").Value -= iamt
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            Next
        Else
            'turn off colors when flag turned off and used to have money
            'this adjustment is to adjust money in total record when turning off flag
            For Each row As DataGridViewRow In dgScores.Rows
                If row.Cells("Player").Value = "*** Total ***" Then
                    row.Cells("$Closest").Value += iamt
                    row.Cells("$Earn").Value += iamt
                    Continue For
                End If
                If Val(oHelper.convDBNulltoSpaces(row.Cells("$Closest").Value)) = 0 Then
                    row.Cells("$Closest").Style.BackColor = Color.White
                    row.Cells("$Earn").Style.BackColor = Color.White
                    If Val(row.Cells("$Skins").Value) = 0 Then row.Cells("Player").Style.BackColor = Color.White
                End If
            Next
        End If

        dgScores.EndEdit()
        oHelper.bScreenChanged = True

        'tbLOCP1.BackColor = Color.LightGreen
        'tbLOCP2.BackColor = Color.LightGreen
        'tbLOPurse.BackColor = Color.LightGreen

        Me.Cursor = Cursors.Default
        Application.DoEvents()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub dgScores_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgScores.CellFormatting
        'updated routine to implement AcceptChanges
        'dt.AcceptChanges() 'note: this causes custom cell font to be cleared
        'DgvAcceptChanges(dgScores)
        'Try
        '    If oHelper.convDBNulltoSpaces(e.Value) <> "" Then
        '        e.FormattingApplied = True
        '    End If
        'Catch ex As Exception
        '    Dim x = ""
        'End Try

    End Sub
    Private Sub dgScores_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgScores.DataError
        'MsgBox(e.Exception.Message)

        Try
            dgScores.EndEdit()
            Debug.Print(e.Context.ToString & e.Exception.Message)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' this routine will take a DataGridView and save the style for each cell, then it will take it's DataTable source and accept any
    ''' changes, then it will re-apply the style font to each DataGridView cell. this is required since DataTable.AcceptChanges will
    ''' clear any DataGridView cell formatting.
    ''' </summary>
    ''' <param name="dgv">DataGridView object</param>
    ''' <remarks>Could be extended to do other things like cell ReadOnly status or cell BackColor.</remarks>
    Public Sub DgvAcceptChanges(dgv As DataGridView)
        Dim dt As DataTable = CType(dgv.DataSource, DataTable)
        If dt IsNot Nothing Then
            'save the DataGridView's cell style font to an array
            Dim cellStyle(dgv.Rows.Count - 1, dgv.Columns.Count - 1) As DataGridViewCellStyle
            For r As Integer = 0 To dgv.Rows.Count - 1
                'the DataGridViewRow.IsNewRow Property = Gets a value indicating whether the row is the row for new records.
                'Remarks: Because the row for new records is in the Rows collection, use the IsNewRow property to determine whether a row
                'is the row for new records or is a populated row. A row stops being the new row when data entry into the row begins.
                If Not dgv.Rows(r).IsNewRow Then
                    For c As Integer = 0 To dgv.Columns.Count - 1
                        cellStyle(r, c) = dgv.Rows(r).Cells(c).Style
                    Next c
                End If
            Next r

            'this causes custom cell font to be cleared in the DataGridView
            dt.AcceptChanges()

            're-apply the DataGridView's cell style font from an array
            For r As Integer = 0 To dgv.Rows.Count - 1
                If Not dgv.Rows(r).IsNewRow Then
                    For c As Integer = 0 To dgv.Columns.Count - 1
                        dgv.Rows(r).Cells(c).Style.Font = cellStyle(r, c).Font
                    Next c
                End If
            Next r
        End If
    End Sub
    Public Sub saveFont(dgv As DataGridView)
        'save the DataGridView's cell style font to an array
        Dim cellStyle(dgv.Rows.Count - 1, dgv.Columns.Count - 1) As DataGridViewCellStyle
        For r As Integer = 0 To dgv.Rows.Count - 1
            'the DataGridViewRow.IsNewRow Property = Gets a value indicating whether the row is the row for new records.
            'Remarks: Because the row for new records is in the Rows collection, use the IsNewRow property to determine whether a row
            'is the row for new records or is a populated row. A row stops being the new row when data entry into the row begins.
            If Not dgv.Rows(r).IsNewRow Then
                For c As Integer = 0 To dgv.Columns.Count - 1
                    cellStyle(r, c) = dgv.Rows(r).Cells(c).Style
                Next c
            End If
        Next r

    End Sub
    Public Sub resetFont(dgv As DataGridView)
        Dim cellStyle(dgv.Rows.Count - 1, dgv.Columns.Count - 1) As DataGridViewCellStyle
        're-apply the DataGridView's cell style font from an array
        For r As Integer = 0 To dgv.Rows.Count - 1
            If Not dgv.Rows(r).IsNewRow Then
                For c As Integer = 0 To dgv.Columns.Count - 1
                    dgv.Rows(r).Cells(c).Style.Font = cellStyle(r, c).Font
                Next c
            End If
        Next r
    End Sub


End Class