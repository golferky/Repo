Imports System.IO
Public Class Skins
    Dim oHelper As New Helper.Controls.Helper
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
        dgScores.Visible = False
        'oHelper.MyCourse = oHelper.dsLeague.Tables("dtCourses").Select("Name = '" & oHelper.rLeagueParmrow("Course") & "'")
        bsave = False
        If rbColors.Checked Then
            oHelper.bColors = True
        End If
        If rbDots.Checked Then
            oHelper.bDots = True
        End If

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
        '20180318-add handler for checking dots
        'AddHandler rbDots.CheckedChanged, AddressOf checkDotsColors
        'Handles rbDots.CheckedChanged
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
        CalcSkins()
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
        lbStatus.BackColor = Color.Red
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Dim dgv = sender
        oHelper.bReorderCols = True
        oHelper.bCalcSkins = True
        If rbColors.Checked Then
            For Each row As DataGridViewRow In dgv.rows
                oHelper.ChangeColorsForStrokes(row)
            Next
        End If
        oHelper.bReorderCols = False
        oHelper.bCalcSkins = False
        lbStatus.Text = String.Format("Resorting of Column {0} done", newColumn.HeaderText)
        lbStatus.BackColor = Color.LightGreen
        Me.Cursor = Cursors.Default
        Application.DoEvents()
    End Sub

    Private Sub dgScores_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgScores.CellMouseDoubleClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
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
        Me.Cursor = Cursors.Default
        Application.DoEvents()
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
    'Sub checkDotsColors()
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

    '    lbStatus.Text = String.Format("Changing dots/colors")
    '    lbStatus.BackColor = Color.Red
    '    Me.Cursor = Cursors.WaitCursor
    '    Application.DoEvents()

    '    oHelper.bCalcSkins = True
    '    If rbColors.Checked Then
    '        oHelper.bColors = True
    '        oHelper.bDots = False
    '    Else
    '        oHelper.bColors = False
    '        oHelper.bDots = True
    '    End If
    '    For Each row As DataGridViewRow In dgScores.Rows
    '        oHelper.ChangeColorsForStrokes(row)
    '    Next
    '    '20180228-recolor and strikeout
    '    CalcSkins()
    '    oHelper.bCalcSkins = False
    '    lbStatus.Text = String.Format("Finished Changing dots/colors")
    '    lbStatus.BackColor = Color.LightGreen
    '    Me.Cursor = Cursors.Default
    '    Application.DoEvents()

    'End Sub
    Sub CalcSkins()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        lbStatus.Text = String.Format("Calculating Skins")
        lbStatus.BackColor = Color.Red
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
        ' RemoveHandler dgScores.CellEnter, AddressOf dgScores_CellEnter

        iTotCTPPlayers = 0
        iTotSkinPlayers = 0

        '20180224-
        Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))

        dvScores.RowFilter = "Date = '" & cbDatesPlayers.Text.ToString & "'"
        dvScores.RowFilter = dvScores.RowFilter & " and Skins = 'Y'"
        iTotSkinPlayers = dvScores.Count

        dvScores.RowFilter = "Date = '" & cbDatesPlayers.Text.ToString & "'"
        dvScores.RowFilter = dvScores.RowFilter & " and Closest = 'Y'"
        iTotCTPPlayers = dvScores.Count

        If oHelper.iHoles = 0 Then oHelper.iHoles = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0).Item("Holes")
        sdate = cbDatesPlayers.Text.ToString

        If sdate = "" Then
            MsgBox("Please enter Or select a date")
            Exit Sub
        End If

        oHelper.CalcHoleMarker(sdate)

        '20180130-calculate how many closests to pins there should be
        oHelper.iNumClosests = 0
        For i = oHelper.iHoleMarker To (oHelper.iHoleMarker - 1) + 9
            If oHelper.MyCourse(0)("Hole" & i) = 3 Then oHelper.iNumClosests += 1
        Next

        iEachClosestAmt = (iTotCTPPlayers - iTotCTPPlayers Mod 2) / oHelper.iNumClosests

        Dim iSkinpot As Integer = oHelper.rLeagueParmrow("Skins") * iTotSkinPlayers + oHelper.rLeagueParmrow("RolledOverSkins")
        tbSkins.Text = 0
        tbPurse.Text = 0
        tbLOSkins.Text = 0
        tbCP1.Text = 0
        tbCP2.Text = 0
        tbLOPurse.Text = 0
        tbLOCP1.Text = 0
        tbLOCP2.Text = 0
        'tbLOCP1.Text = iEachClosestAmt
        'tbLOCP2.Text = iEachClosestAmt
        'tbLOPurse.Text += iEachClosestAmt * oHelper.iNumClosests
        Dim iExtra As Integer = 0

        SaveScores()

        dgScores.Columns.Clear()
        dgScores.Visible = True
        oHelper.bCalcSkins = True
        'build the grid
        BldSkinsDataGridFromCSV()

        'this is for change colors routine and if no scores, no sense calculating anything
        If dgScores.Rows.Count = 1 Then
            oHelper.bCalcSkins = False
            lbStatus.Text = "No skins or CTP entered, nothing to show"
            Me.Cursor = Cursors.Default
            Application.DoEvents()
            Exit Sub
        End If
        '20180508
        For Each row In dgScores.Rows

        Next
        'sSkinsIndexes = oHelper.CalcSkins(dgScores)
        sSkinsIndexes = FCalcSkins()
        sSkinsIndexes.Sort()
        'Dim iSkinpot As Integer = oHelper.rLeagueParmrow("Skins") * iTotSkinPlayers + oHelper.rLeagueParmrow("RolledOverSkins")
        Dim iSkinVal As Integer = 0

        If sSkinsIndexes.Count > 0 Then
            '20180130-save to add to pot
            '20180228-Fix extra dollars incorrect, they were missing leftover ctp calc
            iSkinVal = Math.Truncate(iSkinpot / sSkinsIndexes.Count)
            iExtra = iTotCTPPlayers * 1 - (iEachClosestAmt * oHelper.iNumClosests)
            iExtra = iExtra + (iSkinpot - (iSkinVal * sSkinsIndexes.Count))
        Else
            iExtra = 0
            tbLOSkins.Text = iSkinpot
            tbLOPurse.Text += iSkinpot
        End If

        tbExtra.Text = iExtra
        '20180228-Fix extra dollars incorrect, they were missing leftover ctp calc
        tbLOPurse.Text += iExtra

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
        Dim iEarnytd = 0.00

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
                '2018042-this wont ever be true because these check boxes dont get populated when calc skins button is pushed(Which is how we got here)
                'If row.Cells("CTP_1").Value = True Then
                '    iEarn += iEachClosestAmt
                '    iCtpsDol += iEachClosestAmt
                'End If
                'If row.Cells("CTP_2").Value = True Then
                '    iEarn += iEachClosestAmt
                '    iCtpsDol += iEachClosestAmt
                'End If

                '20180424 - Fix if already done
                If IsNumeric(row.Cells("$Closest").Value) Then
                    iEarn += row.Cells("$Closest").Value
                    iCtpsDol += row.Cells("$Closest").Value
                    row.Cells("$Closest").Style.BackColor = Color.Gold
                    row.Cells("Player").Style.BackColor = Color.Gold
                    tbCP1.Text += CInt(row.Cells("$Closest").Value)
                End If

                row.Cells("$Earn").Value = iEarn
                iEarnytd += iEarn

            End If
        Next

        'update the total row now
        For Each row In dgScores.Rows
            If row.Cells("Player").Value = "*** Total ***" Then
                row.Cells("#Skins").Value = iSkinsNum
                row.Cells("$Skins").Value = iSkinsDol
                row.Cells("$Earn").Value = iEarnytd
                row.Cells("$Closest").Value = iCtpsDol
                row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow
            End If
        Next
        oHelper.bCalcSkins = False

        tbSkins.Text = iSkinsDol
        tbPurse.Text = iSkinsDol + iCtpsDol

        lbStatus.Text = String.Format("Done Calculating skins")
        lbStatus.BackColor = Color.LightGreen
        Me.Cursor = Cursors.Default
        Application.DoEvents()
        '20180307-commented out for testing
        dgScores.Sort(dgScores.Columns("$Earn"), System.ComponentModel.ListSortDirection.Descending)
        'set color switches for helper routine
        'checkDotsColors()

    End Sub
    Sub SaveScores()
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        Try
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
            oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
            '10/1/2017 add code to pull in all scores for a given player
            'check frmScoreCard for event
            '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
            '2 - if double click on a playerevent, get scores for a given player

            Dim dvScores As New DataView(oHelper.dsLeague.Tables("dtScores"))

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

                If parm.Contains("(") Then
                    sParm = parm.Substring(0, parm.IndexOf("("))
                End If

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
                            '20180228-missing this check caused john huening issue
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
                                            cell = cell - 1 & ChrW(&H25CF)
                                        End If
                                    End If
                                    row("Hole" & i - 1 + oHelper.iHoleMarker) = cell
                                    'If cell.Contains(ChrW(&H25CF) & ChrW(&H25CF)) Then
                                    '    cell = oHelper.RemoveSpcChar(cell.Value) - 2 & ChrW(&H25CF) & ChrW(&H25CF)
                                    'ElseIf cell.ToString.Contains(ChrW(&H25CF)) Then
                                    '    cell = oHelper.RemoveSpcChar(cell.Value) - 1 & ChrW(&H25CF)
                                    'End If
                                Next
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
            dgScores.RowTemplate.Height = 35

            dgScores.DefaultCellStyle.Font = New Font("Tahoma", 12)

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
                If col.Name.Contains("Hole") Then
                    col.HeaderText = col.HeaderText.Replace("Hole", "")
                End If
            Next

            'this moves the rows into the grid
            For Each row As DataRow In dtScorecard.Rows
                dgScores.Rows.Add(row.ItemArray)
            Next

            For Each row As DataGridViewRow In dgScores.Rows
                row.Height = 30
                'oHelper.MakeCellsStrings(row)
                Dim dv2Scores As New DataView(oHelper.dsLeague.Tables("dtScores"))
                dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & sdate & "'"
                dv2Scores.Sort = "Date Desc"
                If dv2Scores.Count > 0 Then row.Cells("PHdcp").Value = dv2Scores(0).Item("Hdcp").ToString
                '20171230 visit this today 
                '20180307-removed because its done at checkdotscolors
                'oHelper.ChangeColorsForStrokes(row)
                row.Cells("$Skins").Value = ""
                row.Cells("$Earn").Value = ""
                row.Cells("#Skins").Value = ""
                '20180227-remove not needed
                'row.Cells("$Closest").Value = ""
            Next

            '20180224-save off y/n values from gridview
            Dim sCTP1 As New List(Of String)
            Dim sCTP2 As New List(Of String)
            For Each skin As DataGridViewRow In dgScores.Rows
                sCTP1.Add(oHelper.convDBNulltoSpaces(skin.Cells("CTP_1").Value))
                sCTP2.Add(oHelper.convDBNulltoSpaces(skin.Cells("CTP_2").Value))
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

            'lop through each row loo
            For Each row As DataGridViewRow In dgScores.Rows
                Dim ictp = 0
                If sCTP1(row.Index) = "Y" Then
                    row.Cells("CTP_1").Value = True
                    ictp += iEachClosestAmt
                Else
                    row.Cells("CTP_1").Value = False
                End If
                If sCTP2(row.Index) = "Y" Then
                    row.Cells("CTP_2").Value = True
                    ictp += iEachClosestAmt
                Else
                    row.Cells("CTP_1").Value = False
                End If
                If ictp > 0 Then row.Cells("$Closest").Value = ictp
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
            If row.Cells("Player").Value Is Nothing Then
                Exit Sub
            End If

            'find the score for this player / date
            If row.Cells("Player").Value.contains("*** Total") Then Exit Sub
            Dim sKeys() As Object = {row.Cells("Player").Value, oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)} 'cbDatesPlayers.SelectedItem}
            Dim dr As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
            For Each cell As DataGridViewCell In row.Cells
                If oHelper.cSkinsFields.Contains(cell.OwningColumn.Name) Then
                    If cell.Value IsNot DBNull.Value Then
                        Try
                            dr(cell.OwningColumn.Name) = oHelper.RemoveSpcChar(cell.Value)
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
    Function FCalcSkins() As List(Of String)
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
        FCalcSkins = New List(Of String)
        'this code goes through the listview and highlights the lowest value on each hole and fron 9, back 9 and total
        Dim ilowrow As New List(Of String)
        'adjust for handicap fields in listview
        'loop through each column finding the lowest scores
        'Get low 9's and 18 hole
        '20171014 - use holemarker to control which 9 or 18 you process
        For ii = oHelper.iHoleMarker To oHelper.iHoleMarker + oHelper.iHoles - 1 'lv1.Items(0).SubItems.Count - 1
            Dim ilowscore = 99
            'calculate a column saving low score
            For i = 0 To dgScores.RowCount - 1
                If dgScores.Rows(i).Cells("Player").Value = "*** Total ***" Then Continue For
                oHelper.sPlayer = dgScores.Rows(i).Cells("Player").Value
                oHelper.iHdcp = dgScores.Rows(i).Cells("PHdcp").Value
                If dgScores.Rows(i).Cells("Skins").Value = "Y" Then
                    If dgScores.Rows(i).Cells("Hole" & ii).Value IsNot DBNull.Value Then
                        Dim iscore As String = oHelper.RemoveSpcChar(dgScores.Rows(i).Cells("Hole" & ii).Value)
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

            If ilowrow.Count = 1 Then
                Dim score As Integer = ilowrow(0)
                dgScores.Rows(score).Cells("Hole" & ii).Style.BackColor = Color.Gold
                dgScores.Rows(score).Cells("Player").Style.BackColor = Color.Gold
                dgScores.Rows(score).Cells("$Skins").Style.BackColor = Color.Gold
                FCalcSkins.Add(score)
            Else
                Dim sFont = "Tahoma"
                Dim iFontSize = 12
                For Each player In ilowrow
                    dgScores.Rows(player).Cells("Hole" & ii).Style.Font = New Font(sFont, iFontSize, FontStyle.Strikeout)
                    Debug.Print(String.Format("setting skins tie for hole {0} {1}", ii, dgScores.Rows(player).Cells("Player").Value))
                Next
            End If
        Next

    End Function
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
        '200180224-ctp
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

        dgScores.Rows(e.RowIndex).Cells("$Earn").Value += iamt

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

        'tbLOCP1.BackColor = Color.LightGreen
        'tbLOCP2.BackColor = Color.LightGreen
        'tbLOPurse.BackColor = Color.LightGreen

        Me.Cursor = Cursors.Default
        Application.DoEvents()

    End Sub

    Private Sub cbSkins_CheckedChanged(sender As Object, e As EventArgs) Handles cbSkins.CheckedChanged

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