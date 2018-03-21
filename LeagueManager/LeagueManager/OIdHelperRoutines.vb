Public Class OIdHelperRoutines
    'Sub BldMatchesDataGridFromCSV()
    '    Try
    '        If bloghelper Then LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '        '10/1/2017 add code to pull in all scores for a given player
    '        'check frmScoreCard for event
    '        '1 - if show scores button pushed, get scores for a given date and check list all scores checklist
    '        '2 - if double click on a playerevent, get scores for a given player

    '        Try
    '            'if file doesnt exist(setup) create it
    '            If Not File.Exists(sFilePath & ("\Scores.csv")) Then
    '                dtScores = fCreateScoreCardDT()
    '            Else
    '                dtScores = CSV2DataTable("\Scores.csv")
    '            End If

    '        Catch ex As Exception
    '            MsgBox(sFilePath & "\Scores.csv in use, close it and try again")
    '            'frmScoreCard.Close()
    '            Exit Sub
    '        End Try

    '        Dim sdate As String = frmScoreCard.cbDatesPlayers.Text.ToString

    '        If sdate = "" Then
    '            MsgBox("Please enter or select a date")
    '            Exit Sub
    '        Else
    '            frmScoreCard.cbDatesPlayers.Items.Add(sdate)
    '            frmScoreCard.cbDatesPlayers.SelectedItem = sdate
    '        End If

    '        If iHoles = 0 Then
    '            iHoles = dtLeagueParms.Rows(0).Item("Holes")
    '        End If

    '        CalcHoleMarker(sdate)
    '        dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}
    '        Dim dvScores As New DataView(dtScores)

    '        dvScores.RowFilter = "Date = '" & sdate & "'"

    '        Dim sArray = New List(Of String)(cMatchFields.Split(","))

    '        Dim sColFormat = New List(Of String)
    '        Dim sScoreCardforDGV = ""
    '        'strip parenthesis and add gross/net for In/Out
    '        'fields can have a pattern associated for cell length, centering,

    '        For Each parm As String In sArray
    '            'set detault pattern
    '            Dim sPat = cPat40
    '            Dim sParm = ""

    '            If UBound(parm.Split("-")) = 0 Then
    '                sParm = parm
    '            Else
    '                sParm = parm.Split("-")(0)
    '                sPat = parm.Substring(parm.IndexOf("-") + 1)
    '            End If

    '            If parm.Contains("(") Then
    '                sParm = parm.Substring(0, parm.IndexOf("("))
    '            End If

    '            sScoreCardforDGV = sScoreCardforDGV + sParm + ","
    '            sColFormat.Add(sPat)
    '        Next
    '        'remove trailing comma
    '        'replace spaces with underscores for csv column matchups
    '        sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_")

    '        Dim dtScorecard As DataTable = dvScores.ToTable(True, sScoreCardforDGV.Split(",").ToArray)

    '        frmScoreCard.dgScores.Columns.Clear()
    '        Dim dg = frmScoreCard.dgScores
    '        bNoRowLeave = True
    '        dg.RowTemplate.Height = 40
    '        dg.DefaultCellStyle.Font = New Font("Tahoma", 15)

    '        With dg
    '            .DataSource = dtScorecard
    '        End With

    '        Dim sdel = "-"
    '        For Each col As DataGridViewColumn In dg.Columns
    '            Dim sformat = sColFormat(col.Index)
    '            Dim scolname = col.Name
    '            Dim sWidth = sformat.Split(sdel)(0)
    '            Dim sRO = sformat.Split(sdel)(1)
    '            Dim sTabstop = sformat.Split(sdel)(2)
    '            Dim sAlign = sformat.Split(sdel)(3)
    '            col.Width = sWidth
    '            If sRO = "y" Then
    '                col.DataGridView.ReadOnly = True
    '            Else
    '                col.DataGridView.ReadOnly = False
    '            End If
    '            If sTabstop = "y" Then
    '                col.DataGridView.ReadOnly = True
    '            Else
    '                col.DataGridView.ReadOnly = False
    '            End If

    '            Select Case sAlign
    '                Case "mr"
    '                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
    '                Case "mc"
    '                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '                Case "ml"
    '                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
    '            End Select

    '            col.HeaderText = col.HeaderText.Replace("_", " ")
    '        Next

    '        For Each row As DataGridViewRow In dg.Rows
    '            row.Height = 30
    '            If row.IsNewRow Then
    '                If frmScoreCard.rbNet.Checked Then
    '                    row.Cells("Method") = BuildScoreCardMethods()
    '                    row.Cells("Method").Value = "Net"
    '                End If
    '                Exit For
    '            End If
    '            row.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

    '            ChangeColorsForStrokes(row)

    '            Dim dv2Scores As New DataView(dtScores)
    '            dv2Scores.RowFilter = "Player = '" & row.Cells("Player").Value & "' And Date < '" & sdate & "'"
    '            dv2Scores.Sort = "Date Desc"
    '            If dv2Scores.Count > 0 Then
    '                row.Cells("Phdcp").Value = dv2Scores(0).Item("Hdcp").ToString
    '            End If

    '            dg.Columns("PHdcp").HeaderText = "Hdcp"
    '            If row.Cells("Out_Gross").Value Is DBNull.Value Then
    '                'row.Cells("Out_Gross").Style.BackColor = Color.Pink
    '                row.Cells("Player").Style.BackColor = Color.Pink
    '            End If
    '        Next

    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Sub
    'Function getMatchScores(sdate As String) As DataView
    '    getMatchScores = Nothing
    '    If bloghelper Then LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    CalcHoleMarker(sdate)
    '    Dim rSch As DataRow = dtSchedule.Rows.Find(DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yy"))
    '    If rSch Is Nothing Or rSch(1) Is DBNull.Value Then
    '        MsgBox("No scheduled matches found for this date, must exit")
    '        Exit Function
    '    End If
    '    Dim sPlayers As New List(Of String)
    '    Dim dvPlayers As DataView
    '    dvPlayers = New DataView(dtPlayers)
    '    Dim sScoresRowFilter = ""
    '    For iMatch = 1 To dtLeagueParms.Rows(0)("#Teams") / 2
    '        Dim sMatch = rSch(iMatch.ToString).ToString
    '        sScoresRowFilter = "Team IN ('#','@') AND ISNULL(DateJoined,'00010101') <= '20170411' AND ISNULL(DateLeft,'99999999') > '20170411'"
    '        sScoresRowFilter = sScoresRowFilter.Replace("#", sMatch.Split("v")(0)).Replace("@", sMatch.Split("v")(1)).Replace("20170411", sdate)
    '        'sScoresRowFilter = "Team in ('" & sMatch.Split("v")(0) & "','" &
    '        '                                  sMatch.Split("v")(1) & "')" &
    '        '                                  " And ISNULL(DateJoined,'" & sdate & "') '" >= sdate & "'" &
    '        '                                  " And ISNULL(DateLeft,'" & sdate & "') '" < sdate & "'"
    '        dvPlayers = New DataView(dtPlayers) With
    '        {
    '         .RowFilter = sScoresRowFilter,
    '         .Sort = "Team, Grade"
    '        }

    '        Dim ip# = 0 'index pointer into scores file to mark matches a players are 0-2, b are 1-3
    '        For Each row As DataRowView In dvPlayers
    '            Dim sKeys() As Object = {row("Name"), sdate}
    '            Dim drow As DataRow = dtScores.Rows.Find(sKeys)
    '            'if no score is found for this player/date, check to see if he has a sub
    '            Dim bsub = False
    '            If drow Is Nothing Then
    '                'find this players partner in players file
    '                For Each player As DataRowView In dvPlayers
    '                    'find this players team number in players file
    '                    If player("Team") = row("Team") Then
    '                        'if the player name <> the missing player, we have his partner
    '                        If player("Name") <> row("Name") Then
    '                            'now find all the scores for that team and eliminate his partner
    '                            Dim dvSubScore As DataView
    '                            dvSubScore = New DataView(dtScores) With
    '                            {
    '                                .RowFilter =
    '                                "Team = " & row("Team") &
    '                                " and Player <> '" & player("Name") & "'" &
    '                                " and Date = '" & sdate & "'"
    '                            }
    '                            If dvSubScore.Count > 0 Then
    '                                'save the regulars name for later
    '                                Dim sSchPlayer = row("Name")
    '                                'save the subs name for later
    '                                Dim ssub = dvSubScore(0)("Player")
    '                                Dim bnoshow = False
    '                                For Each spl As String In sPlayers
    '                                    If spl.Split(",")(0) = dvSubScore(0)("Player") Then
    '                                        bnoshow = True
    '                                        Exit For
    '                                    End If
    '                                Next
    '                                If bnoshow Then
    '                                    Exit For
    '                                End If
    '                                '20171015 - account for player subbing and his partner noshows
    '                                sPlayers.Add(dvSubScore(0)("Player") & "," & iMatch & ip# & "," & row("Name"))
    '                                bsub = True
    '                            End If
    '                            Exit For
    '                        End If
    '                    End If
    '                Next
    '                If bsub Then
    '                    ip += 1
    '                    Continue For
    '                End If
    '                'this assumes the player is a no-show and and empty row gets built
    '                Dim dvscores = New DataView(dtScores)
    '                Dim rowView As DataRowView = dvscores.AddNew
    '                ' Change values in the DataRow.
    '                rowView("League") = sLeagueName
    '                rowView("Player") = row("Name")
    '                rowView("Method") = "Score"
    '                rowView("Group") = 0
    '                rowView("Team") = row("Team")
    '                rowView("Hdcp") = row("Handicap")
    '                rowView("Date") = sdate
    '                rowView("Skins") = "N"
    '                rowView("Closest") = "N"
    '                rowView("Partner") = iMatch & ip#
    '                rowView.EndEdit()
    '            End If
    '            sPlayers.Add(row("Name") & "," & iMatch & ip#)
    '            ip# += 1
    '        Next
    '    Next

    '    'now build a filter for scores
    '    Dim srowfilter = "League = '" & sLeagueName & "' and Date = '" & sdate & "' and Player in ('"
    '    'search the players file and match to the subs array list replacing regulars with subs for the filter
    '    For Each prow As String In sPlayers
    '        srowfilter = srowfilter & prow.Split(",")(0).ToString & "','"
    '    Next
    '    srowfilter = srowfilter & ")"
    '    srowfilter = srowfilter.Replace(",')", ")")
    '    For Each sPlayer In sPlayers
    '        Dim sKeys() As Object = {sPlayer.Split(",")(0), sdate}
    '        Dim drow As DataRow = dtScores.Rows.Find(sKeys)
    '        'if no score is found for this player/date, check to see if he has a sub
    '        If drow IsNot Nothing Then
    '            drow("Partner") = sPlayer.Split(",")(1)
    '        End If
    '    Next

    '    'sort by index built above(we use partner cause it aint used anywhere)
    '    Dim dvMatchScores = New DataView(dtScores)
    '    dvMatchScores = New DataView(dtScores) With {.RowFilter = srowfilter}
    '    dvMatchScores.Sort = "Partner"

    '    Return dvMatchScores

    'End Function
    'Sub BuildNoShow()
    '    Dim dvMatchScores = New DataView(dtScores)

    'End Sub
    'Function oldgetMatchScores(sdate As String, sMatchNum As String) As DataView
    '    oldgetMatchScores = Nothing
    '    LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    CalcHoleMarker(sdate)

    '    Dim sMatch As String = ""
    '    Dim rSch As DataRow = dtSchedule.Rows.Find(DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yy"))
    '    If rSch Is Nothing Then
    '        MsgBox("No scheduled matches found for this date, must exit")
    '        Exit Function
    '    Else
    '        sMatch = rSch(sMatchNum).ToString
    '    End If
    '    'Hole by Hole results
    '    Dim dvPlayers As DataView
    '    dvPlayers = New DataView(dtPlayers) With
    '    {
    '        .RowFilter = "Team = '" & sMatch.Split("v")(0) & "' Or Team = '" & sMatch.Split("v")(1) & "'",
    '        .Sort = "Team, Grade"
    '    }
    '    Dim dvScores = New DataView(dtScores)
    '    Dim sleague = sLeagueName
    '    Dim i = 0 'index pointer into scores file to mark matches a players are 0-2, b are 1-3
    '    Dim aSubs As New List(Of String)
    '    For Each row As DataRowView In dvPlayers
    '        Dim sKeys() As Object = {row("Name"), sdate}
    '        Dim drow As DataRow = dtScores.Rows.Find(sKeys)
    '        'if no score is found for this player/date, check to see if he has a sub
    '        Dim bsub = False
    '        If drow Is Nothing Then
    '            'find this players partner in players file
    '            For Each player As DataRowView In dvPlayers
    '                'find this players team number in players file
    '                If player("Team") = row("Team") Then
    '                    'if the player name <> the missing player, we have his partner
    '                    If player("Name") <> row("Name") Then
    '                        'now find all the scores for that team and eliminate his partner
    '                        Dim dvSubScore As DataView
    '                        dvSubScore = New DataView(dtScores) With {.RowFilter = "Team = " & row("Team") & " and Player <> '" & player("Name") & "'" & " and Date = '" & sdate & "'"}
    '                        'save the subs name and index for later
    '                        If dvSubScore.Count > 0 Then
    '                            aSubs.Add(dvSubScore(0)("Player") & "," & i)
    '                            bsub = True
    '                        End If
    '                        Exit For
    '                    End If
    '                End If
    '            Next
    '            If bsub Then
    '                Continue For
    '            End If
    '            'this assumes the player is a no-show and and empty row gets built
    '            Dim rowView As DataRowView = dvScores.AddNew
    '            ' Change values in the DataRow.
    '            rowView("League") = sleague
    '            rowView("Player") = row("Name")
    '            rowView("Method") = "Score"
    '            rowView("Group") = 0
    '            rowView("Team") = row("Team")
    '            rowView("Hdcp") = row("Handicap")
    '            rowView("Date") = sdate
    '            rowView("Skins") = "N"
    '            rowView("Closest") = "N"
    '            rowView("Partner") = i
    '            rowView.EndEdit()
    '        Else
    '            drow.Item("Partner") = i
    '        End If
    '        i += 1
    '    Next

    '    'now build a filter for scores
    '    Dim srowfilter = "League = '" & sleague & "' and Date = '" & sdate & "' and Player in ('"
    '    i = 0
    '    'search the players file and match to the subs array list replacing regulars with subs for the filter
    '    For Each prow As DataRowView In dvPlayers
    '        Dim bfound = False
    '        For Each ssubName In aSubs
    '            If ssubName.Split(",")(1) = i Then
    '                srowfilter = srowfilter & ssubName.Split(",")(0) & "','"
    '                bfound = True
    '                Exit For
    '            End If
    '        Next
    '        If Not bfound Then
    '            srowfilter = srowfilter & prow("Name") & "','"
    '        End If
    '        i += 1
    '    Next
    '    srowfilter = srowfilter & ")"
    '    srowfilter = srowfilter.Replace(",')", ")")
    '    dvScores = New DataView(dtScores) With {.RowFilter = srowfilter}

    '    'sort by index built above(we use partner cause it aint used anywhere)
    '    dvScores.Sort = "Partner"
    '    oldgetMatchScores = dvScores
    'End Function
    'Sub oldBldScoreCardDataGridFromCSVforPlayer()
    '    '10/1/2017 add code to pull in all scores for a given player
    '    'check frmScoreCard for event
    '    '1 - if show scores button pushed, get scores for a given player
    '    Try
    '        Try
    '            'if file doesnt exist(setup) create it
    '            If Not File.Exists(sFilePath & ("\Scores.csv")) Then
    '                dtScores = fCreateScoreCardDT()
    '            Else
    '                dtScores = CSV2DataTable("\Scores.csv")
    '            End If

    '        Catch ex As Exception
    '            MsgBox(sFilePath & "\Scores.csv in use, close it and try again")
    '            frmScoreCard.Close()
    '            Exit Sub
    '        End Try

    '        If iHoles = 0 Then
    '            iHoles = dtLeagueParms.Rows(0).Item("Holes")
    '        End If

    '        dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}
    '        Dim dvCourses = New DataView(dtCourses)
    '        Dim dvScores As New DataView(dtScores)
    '        dvScores.RowFilter = "Player = '" & sPlayer & "'"
    '        dvScores.RowFilter = dvScores.RowFilter & " And Date >= " & frmScoreCard.cbDatesPlayers.SelectedItem.ToString.Substring(0, 4) & "0101"
    '        dvScores.Sort = "Date"

    '        'create array from above defined fields we want out of scorecard
    '        Dim sArray As String() = sScoreCard.Split(",")
    '        Dim sScoreCardforDGV = ""
    '        'strip parenthesis and add gross/net for In/Out
    '        For Each parm As String In sArray
    '            If parm.Contains("(") Then
    '                parm = parm.Substring(0, parm.IndexOf("("))
    '            End If
    '            If parm = "Holes" Then
    '                For i As Integer = 1 To 18
    '                    sScoreCardforDGV = sScoreCardforDGV + "Hole" & i & ","
    '                    If i = 9 Then
    '                        sScoreCardforDGV = sScoreCardforDGV + "Out Gross,Out Net,"
    '                    ElseIf i = 18 Then
    '                        sScoreCardforDGV = sScoreCardforDGV + "In Gross,In Net,"
    '                        sScoreCardforDGV = sScoreCardforDGV + "18 Gross,18 Net,"
    '                    End If
    '                Next
    '            Else
    '                sScoreCardforDGV = sScoreCardforDGV + parm + ","
    '            End If
    '        Next
    '        'remove trailing comma
    '        sScoreCardforDGV = sScoreCardforDGV.Substring(0, Len(sScoreCardforDGV) - 1).Replace(" ", "_") '.Replace("Player,", "")

    '        Dim dtScorecard As DataTable = dvScores.ToTable(True, sScoreCardforDGV.Split(",").ToArray)

    '        'bNoRowLeave = True

    '        'frmScoreCard.dgScores.Dispose()
    '        'frmScoreCard.dgScores.Columns.Clear()

    '        With frmScoreCard.dgScores
    '            '.Columns.Clear()
    '            .DataSource = dtScorecard
    '            For Each col As DataGridViewTextBoxColumn In .Columns
    '                If col.Name <> "Player" And col.Name <> "Opponent" Then
    '                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
    '                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '                End If
    '                If col.Name.Contains("Hole") Or col.Name.Contains("Team") Or col.Name.Contains("Skins") Or col.Name.Contains("Group") Then
    '                    col.Width = 40
    '                    col.HeaderText = col.HeaderText.Replace("Hole", "")
    '                ElseIf col.Name = "Hdcp" Then
    '                    col.ReadOnly = True
    '                    col.DataGridView.TabStop = False
    '                    col.Width = 40
    '                ElseIf col.Name = "PHdcp" Then
    '                    col.Width = 40
    '                    col.HeaderText = " Prv Hdcp"
    '                ElseIf col.Name.Contains("Gross") Or col.Name.Contains("Net") Then
    '                    col.DataGridView.TabStop = False
    '                    col.Width = 40
    '                    col.HeaderText = col.HeaderText.Replace("_", " ")
    '                Else
    '                    Dim sRoField = New List(Of String)("$Earn,$Skins,$Closest,#Skins,#Closests,Points,Opponent".Split(","))
    '                    For Each fld In sRoField
    '                        If fld = col.Name Then
    '                            col.ReadOnly = True
    '                            col.DataGridView.TabStop = False
    '                            If col.Name <> "Opponent" Then
    '                                col.Width = 50
    '                            End If
    '                            Exit For
    '                        End If
    '                    Next
    '                End If
    '            Next

    '            .Columns("Method").Width = 60
    '            '.Columns("Player").Width = 125
    '            .Columns("Closest").Width = 45

    '        End With
    '        Dim dg = frmScoreCard.dgScores
    '        For Each row As DataGridViewRow In frmScoreCard.dgScores.Rows

    '            If row.IsNewRow Then
    '                If frmScoreCard.rbNet.Checked Then
    '                    row.Cells("Method") = BuildScoreCardMethods()
    '                    row.Cells("Method").Value = "Net"
    '                End If
    '                If frmScoreCard.cbSkins.Checked Then
    '                    row.Cells("Skins").Value = "Y"
    '                End If
    '                If frmScoreCard.cbClosest.Checked Then
    '                    row.Cells("Closest").Value = "Y"
    '                End If
    '                Exit For
    '            End If
    '            CalcHoleMarker(row.Cells("Date").Value)
    '            If row.Cells("Method").Value <> "" Then
    '                ChangeColorsForStrokes(row)
    '            End If

    '            Dim dv2Scores As New DataView(dtScores)
    '            dv2Scores.RowFilter = "Player = '" & sPlayer & "'"
    '            dv2Scores.Sort = "Date"
    '            If row.Cells("Method").Value.ToString = "Net" Then
    '                If iHoleMarker = 1 Then
    '                    If row.Cells("Out_Net").Value IsNot DBNull.Value Then
    '                        row.Cells("Out_Gross").Value = row.Cells("Out_Net").Value + row.Cells("PHdcp").Value
    '                    End If
    '                Else
    '                    If row.Cells("In_Net").Value IsNot DBNull.Value Then
    '                        row.Cells("In_Gross").Value = row.Cells("In_Net").Value + row.Cells("PHdcp").Value
    '                    End If
    '                End If
    '            End If
    '            '20171006 - removed for now
    '            'If frmScoreCard.cbRecalHdcp.Checked Then
    '            '    row.Cells("Hdcp").Value = GetNewHdcp(row, row.Cells("Date").Value)
    '            'End If
    '            If frmScoreCard.cbSkins.Checked Then
    '                row.Cells("Skins").Value = "Y"
    '            End If
    '            If frmScoreCard.cbClosest.Checked Then
    '                row.Cells("Closest").Value = "Y"
    '            End If
    '        Next

    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Sub
    'Sub getVoiceScores()
    '    'this sub uses a file created by phone app or google app
    '    'file name is in format of Leaguename_yyyymmdd_Scores.txt
    '    'scores are in a format of name:"Last initial or name" 123456789101112131415161718,
    '    'if scores length is > 9, we know we have 18

    '    sFilePath = sFilePath & "\"
    '    Dim sDate = dDate.ToString("yyyyMMdd")
    '    Dim sFile = sLeagueName & "_" & sDate & "_Scores.txt"
    '    Dim sPlayer = ""
    '    Dim sholes = ""
    '    Dim iTotal = 0
    '    If dtScores Is Nothing Then
    '        dtScores = CSV2DataTable("Scores.csv")
    '    End If
    '    dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}

    '    Dim dtiPhoneScores = New DataTable
    '    For Each col In sScoreCard.Split(",")
    '        If col = "Holes" Then
    '            For i = 1 To 9
    '                dtiPhoneScores.Columns.Add("Hole" & i)
    '            Next
    '        Else
    '            If (col.Contains("(")) Then
    '                col = col.Substring(0, col.IndexOf("("))
    '            End If
    '            dtiPhoneScores.Columns.Add(col)
    '        End If
    '    Next

    '    'load settings
    '    Using sr As New IO.StreamReader(sFilePath & sFile)
    '        'loop until we get to the end of the file
    '        Do
    '            'pull in one line at a time
    '            Dim sline = sr.ReadLine
    '            'if the line is empty, exit the loop
    '            If sline = Nothing Then
    '                Exit Do
    '            End If
    '            'create a row for each player/score combo
    '            Dim slineparts = sline.Split(",")
    '            'split out each player name,all 9 scores into 2 score buckets (0,1)
    '            'loop through each player score and seperate them
    '            Dim ictr = 0
    '            For Each part In slineparts
    '                ictr += 1
    '                'seperates he player from the score
    '                Dim score = part.TrimStart(" ").Split(" ")
    '                'save the player and all 9 scores
    '                sPlayer = score(0).ToUpper
    '                If sPlayer = "ABSENT" Then
    '                    Continue For
    '                End If

    '                sholes = score(1)
    '                If sholes.Length < 9 Then
    '                    If bloghelper Then LOGIT("Player - " & sPlayer.PadRight(25) & " Scores only " & sholes.Length & ".  Needs to be 9...skipping")
    '                    Continue For
    '                End If
    '                'setup a string to build scores and commas
    '                Dim sbscores As New System.Text.StringBuilder
    '                'initialize total score
    '                iTotal = 0
    '                'loop through each hole
    '                Dim arow As DataRow
    '                arow = dtiPhoneScores.NewRow
    '                For i = 0 To 8
    '                    'add the score and a comma to the string
    '                    arow("Hole" & i + 1) = sholes.Substring(i, 1)
    '                    sbscores.Append(sholes.Substring(i, 1) & ",")
    '                    'accumulate the total score
    '                    iTotal += sholes.Substring(i, 1)
    '                Next
    '                'arow("League") = txtLeagueName.Text
    '                arow("Method") = "Net"
    '                arow("Player") = fGetPlayer(RemoveNulls(sPlayer.Replace("-", " ")))
    '                arow("Date") = sDate
    '                Dim frow As DataRow
    '                Dim sKeys() As Object = {arow("Player"), sDate}

    '                frow = dtScores.Rows.Find(sKeys)
    '                If Not frow Is Nothing Then
    '                    MsgBox("Scores.csv already has scores fo this date, remove them and try again")
    '                    Exit Sub
    '                End If
    '                arow("Group") = Math.Ceiling(ictr / 4)
    '                arow("Team") = fGetTeam(arow("Player"))
    '                If arow("Team") = "" Then
    '                    sPlayerToFind = arow("Player")
    '                    sFindPlayerOption = "Sub"
    '                    frmTeam.ShowDialog()
    '                    arow("Team") = sTeam
    '                End If
    '                'arow("Total") = iTotal
    '                arow("Skins") = "y"
    '                Dim dvScore As New DataView(dtScores)
    '                dvScore.RowFilter = "Player = '" & arow("Player") & "'" & " and Date < '" & sDate & "'"
    '                dvScore.Sort = "Date Desc"
    '                If dvScore.Count > 0 Then
    '                    arow("PHdcp") = dvScore(0)("Hdcp")
    '                Else
    '                    Dim MyCourse() As Data.DataRow
    '                    Dim scourse = dtLeagueParms.Rows(0)("Course")
    '                    MyCourse = dtCourses.Select("Name = '" & scourse & "'")
    '                    Dim iCoursePar = 0
    '                    'accumulate par for each score
    '                    For i As Integer = 1 To iHoles
    '                        iCoursePar += MyCourse(0)("Hole" & i).ToString
    '                    Next
    '                    arow("Phdcp") = Math.Round((iTotal - iCoursePar) * 0.8)
    '                    arow("Hdcp") = arow("PHdcp")
    '                End If
    '                'add the full row to the table
    '                dtiPhoneScores.Rows.Add(arow)
    '                'write the results to the console
    '                LOGIT(sLeagueName & ",,Net,," & sPlayer & "," & sDate & "," & sbscores.ToString & iTotal)
    '            Next
    '        Loop
    '    End Using

    '    If dtiPhoneScores.Rows.Count < 24 Then
    '        Dim mbr As MsgBoxResult = MsgBox(dtiPhoneScores.Rows.Count & " scores found, expected 24, is this correct?", MsgBoxStyle.YesNo)
    '        If mbr = MsgBoxResult.Yes Then
    '            dtScores.Merge(dtiPhoneScores, True, MissingSchemaAction.Ignore)
    '            DataTable2CSV(dtScores, sFilePath & "\Scores.csv")
    '            frmScoreCard.rbAllScores.Checked = True
    '            frmScoreCard.btnUpdate.Visible = True
    '        End If
    '    End If
    '    ' dtScores.Merge(dtiPhoneScores, True, MissingSchemaAction.Ignore)
    '    'BldScoreCardDataGridFromCSV()
    'End Sub
    'Private Sub btnMatches_Click(sender As Object, e As EventArgs) Handles btnMatches.Click
    '    oHelper.LOGIT("--------------------------------------------------------------")
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Me.Cursor = Cursors.WaitCursor
    '    Application.DoEvents()
    '    'oHelper.bNoRowLeave = True
    '    oHelper.bScoresbyPlayer = False
    '    If Not oHelper.bScoresbyPlayer Then
    '        If cbDatesPlayers.SelectedItem Is Nothing Then
    '            oHelper.dDate = Date.ParseExact(cbDatesPlayers.Text, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    '        Else
    '            oHelper.dDate = Date.ParseExact(cbDatesPlayers.SelectedItem, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    '        End If
    '    End If

    '    'Dim rSch As DataRow = dtSchedule.Rows.Find(DateTime.ParseExact(oHelper.dDate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yy"))
    '    'Dim rSch As DataRow = dtSchedule.Rows.Find(oHelper.dDate.ToString("MM\/dd\/yy"))
    '    'If rSch Is Nothing Or rSch(1) Is DBNull.Value Then
    '    '    MsgBox("No scheduled matches found for this date, must exit")
    '    '    Exit Sub
    '    'End If
    '    If dgScores.RowCount > 0 Then
    '        Dim mbResult As MsgBoxResult = MsgBox("Matches require this screen to be reload, do you want to save the screen contents before proceeding?" & vbCrLf & "If you press no, scores will reload and screen contents wont be saved", MsgBoxStyle.YesNoCancel)
    '        If mbResult = MsgBoxResult.Yes Then
    '            For Each row In dgScores.Rows
    '                oHelper.UpdateScoreFromDataGrid(row)
    '            Next
    '        ElseIf mbResult = MsgBoxResult.Cancel Then
    '            Exit Sub
    '        End If
    '    End If

    '    oHelper.bCalcMatches = True
    '    oHelper.BldMatchesDataGridFromCSV()
    '    For i = 1 To oHelper.dtLeagueParms.Rows(0)("#Teams") / 2
    '        Dim xx = oHelper.dtLeagueParms.Rows(0)("#Teams") / 2
    '        Dim aPtr = (i - 1) * 4
    '        'A Player Match
    '        oHelper.getMatchPts(dgScores, aPtr)
    '        'B Player Match
    '        oHelper.getMatchPts(dgScores, aPtr + 1)
    '        Dim ihaNet = 0
    '        Dim ihbNet = 0
    '        Dim ioaNet = 0
    '        Dim iobNet = 0
    '        If oHelper.iHoleMarker = 1 Then
    '            ihaNet = oHelper.FixNullScore(dgScores.Rows(aPtr).Cells("Out_Net").Value.ToString)
    '            ihbNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 1).Cells("Out_Net").Value.ToString)
    '            ioaNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 2).Cells("Out_Net").Value.ToString)
    '            iobNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 3).Cells("Out_Net").Value.ToString)
    '        Else
    '            ihaNet = oHelper.FixNullScore(dgScores.Rows(aPtr).Cells("In_Net").Value.ToString)
    '            ihbNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 1).Cells("In_Net").Value.ToString)
    '            ioaNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 2).Cells("In_Net").Value.ToString)
    '            iobNet = oHelper.FixNullScore(dgScores.Rows(aPtr + 3).Cells("In_Net").Value.ToString)
    '        End If
    '        Dim ihTeam As Integer = ihaNet + ihbNet
    '        Dim ioTeam As Integer = ioaNet + iobNet
    '        Dim x = ""
    '        dgScores.Rows(aPtr).Cells("Team_Points").Value = ""
    '        dgScores.Rows(aPtr + 1).Cells("Team_Points").Value = ""
    '        dgScores.Rows(aPtr + 2).Cells("Team_Points").Value = ""
    '        dgScores.Rows(aPtr + 3).Cells("Team_Points").Value = ""
    '        If ihTeam < ioTeam Then
    '            dgScores.Rows(aPtr).Cells("Team_Points").Value = 0.5
    '            dgScores.Rows(aPtr + 1).Cells("Team_Points").Value = 0.5
    '            dgScores.Rows(aPtr).Cells("Team_Points").Style.BackColor = Color.LightGreen
    '            dgScores.Rows(aPtr + 1).Cells("Team_Points").Style.BackColor = Color.LightGreen
    '        ElseIf ihTeam > ioTeam Then
    '            dgScores.Rows(aPtr + 2).Cells("Team_Points").Value = 0.5
    '            dgScores.Rows(aPtr + 3).Cells("Team_Points").Value = 0.5
    '            dgScores.Rows(aPtr + 2).Cells("Team_Points").Style.BackColor = Color.LightGreen
    '            dgScores.Rows(aPtr + 3).Cells("Team_Points").Style.BackColor = Color.LightGreen
    '        Else
    '            dgScores.Rows(aPtr).Cells("Team_Points").Value = 0.25
    '            dgScores.Rows(aPtr + 1).Cells("Team_Points").Value = 0.25
    '            dgScores.Rows(aPtr + 2).Cells("Team_Points").Value = 0.25
    '            dgScores.Rows(aPtr + 3).Cells("Team_Points").Value = 0.25
    '            dgScores.Rows(aPtr).Cells("Team_Points").Style.BackColor = Color.Yellow
    '            dgScores.Rows(aPtr + 1).Cells("Team_Points").Style.BackColor = Color.Yellow
    '            dgScores.Rows(aPtr + 2).Cells("Team_Points").Style.BackColor = Color.Yellow
    '            dgScores.Rows(aPtr + 3).Cells("Team_Points").Style.BackColor = Color.Yellow
    '        End If
    '    Next
    '    oHelper.bNoRowLeave = False
    '    oHelper.bCalcMatches = False
    '    btnUpdate.Visible = True
    '    Me.Cursor = Cursors.Default
    '    Application.DoEvents()
    'End Sub 
    'Private Sub btnSkins_Click(sender As Object, e As EventArgs) Handles btnSkins.Click
    '    oHelper.LOGIT("--------------------------------------------------------------")
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    ' RemoveHandler dgScores.CellEnter, AddressOf dgScores_CellEnter

    '    Me.Cursor = Cursors.WaitCursor
    '    Application.DoEvents()

    '    If dgScores.RowCount > 0 Then
    '        Dim mbResult As MsgBoxResult = MsgBox("Skins require this screen to be reload, do you want to save the screen contents before proceeding?" & vbCrLf & "If you press no, scores will reload and screen contents wont be saved", MsgBoxStyle.YesNoCancel)
    '        If mbResult = MsgBoxResult.Yes Then
    '            For Each row In dgScores.Rows
    '                If oHelper.bCalcSkins Then
    '                    'UpdateScoreFromDataGrid(row)
    '                Else
    '                    oHelper.UpdateScoreFromDataGrid(row)
    '                End If

    '            Next
    '        ElseIf mbResult = MsgBoxResult.Cancel Then
    '            Exit Sub
    '        End If
    '    End If


    '    oHelper.bCalcSkins = True
    '    oHelper.BldScoreCardDataGridFromCSV(dgScores)

    '    If dgScores.RowCount > 0 And dgScores.Rows.Count - 1 <> oHelper.dtLeagueParms.Rows(0)("#Teams") * 2 Then
    '        Dim mbr2 As MsgBoxResult = MsgBox("You only have " & dgScores.Rows.Count - 1 & " Scores showing, are you sure you want to calculate Skins? ", MsgBoxStyle.YesNo)
    '        If mbr2 <> MsgBoxResult.Yes Then
    '            Exit Sub
    '        End If
    '    End If

    '    If dgScores.Rows.Count = 0 Then
    '        oHelper.bCalcSkins = False
    '        Me.Cursor = Cursors.Default
    '        Application.DoEvents()
    '        Exit Sub
    '    End If
    '    Dim sSkins As List(Of String)

    '    For Each row As DataGridViewRow In dgScores.Rows
    '        row.Cells("$Skins").Value = DBNull.Value
    '        row.Cells("#Skins").Value = DBNull.Value
    '    Next
    '    sSkins = oHelper.CalcSkins(dgScores)
    '    sSkins.Sort()
    '    Dim iSkinVal As Decimal = 0
    '    Dim iTotSkinPlayers = dgScores.RowCount
    '    Dim iTotClosestPlayers = dgScores.RowCount
    '    Dim sCTP As List(Of String)
    '    sCTP = New List(Of String)
    '    'loop through the rows to get how many are in the skin game, ctp
    '    For Each row As DataGridViewRow In dgScores.Rows
    '        If row.Cells("Skins").Value = "N" Or row.IsNewRow Then
    '            iTotSkinPlayers -= 1
    '        End If
    '        If row.Cells("Closest").Value = "N" Or row.IsNewRow Then
    '            iTotClosestPlayers -= 1
    '        End If
    '        If row.Cells("#Closests").Value IsNot DBNull.Value And Not row.IsNewRow Then
    '            sCTP.Add(row.Index)
    '        End If
    '    Next
    '    If sCTP.Count < 2 Then
    '        Dim bbothClosests = False
    '        If sCTP.Count = 1 Then
    '            If dgScores.Rows(sCTP(0)).Cells("#Closests").Value = 2 Then
    '                Dim mbr2 As MsgBoxResult = MsgBox("Did " & dgScores.Rows(sCTP(0)).Cells("Player").Value & " win both Closests", MsgBoxStyle.YesNo)
    '                If mbr2 = MsgBoxResult.Yes Then
    '                    bbothClosests = True
    '                End If
    '            End If
    '        End If
    '        If Not bbothClosests Then
    '            Dim mbr As MsgBoxResult = MsgBox("Need to Mark 2 Closest to pins" & vbCrLf & "Do you have carry overs?", MsgBoxStyle.YesNo)
    '            If mbr <> MsgBoxResult.Yes Then
    '                Exit Sub
    '            End If
    '        End If
    '    End If
    '    'divide by how many greenies there are
    '    '20171013 - must change to ues league parms
    '    Dim iclosests As Decimal = (iTotClosestPlayers / 2)
    '    For Each ctp In sCTP
    '        Dim dCtp As Decimal = dgScores.Rows(sCTP(0)).Cells("#Closests").Value * iclosests
    '        'dgScores.Rows(ctp).Cells("$Closest").Value = dCtp.ToString("$.00")
    '        dgScores.Rows(ctp).Cells("$Closest").Value = dCtp
    '    Next
    '    If sSkins.Count > 0 Then
    '        iSkinVal = iTotSkinPlayers * (oHelper.dtLeagueParms.Rows(0)("Skins") * 1.0) / sSkins.Count
    '        Dim iprevplayer = 99, iTot = 0.0, iSkins = 0
    '        Dim bfirst = True
    '        For Each iplayer As Integer In sSkins
    '            If iplayer = iprevplayer Then
    '                iTot += iSkinVal
    '                iSkins += 1
    '            Else
    '                If bfirst Then
    '                    bfirst = False
    '                Else
    '                    'dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot.ToString("$.00")
    '                    dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot
    '                    dgScores.Rows(iprevplayer).Cells("#Skins").Value = iSkins
    '                End If
    '                iTot = iSkinVal
    '                iSkins = 1
    '                iprevplayer = iplayer
    '            End If
    '        Next
    '        'dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot.ToString("$.00")
    '        dgScores.Rows(iprevplayer).Cells("$Skins").Value = iTot
    '        dgScores.Rows(iprevplayer).Cells("#Skins").Value = iSkins
    '    End If

    '    oHelper.bCalcSkins = False
    '    Me.Cursor = Cursors.Default
    '    Application.DoEvents()

    'End Sub
    'Private Sub txtLeagueName_TextChanged(sender As Object, e As EventArgs)
    '    oHelper.sLeagueName = txtLeagueName.Text
    '    'reload the table from the xml 
    '    If File.Exists(oHelper.sFilePath & "\" & oHelper.sLeagueName & ".xml") Then
    '        If oHelper.dsLeague.Tables.Contains("dtLeagueParms") Then
    '            oHelper.dsLeague.Tables.Remove("dtLeagueParms")
    '        End If

    '        oHelper.dsLeague.Tables.Add("dtLeagueParms").TableName = "dtLeagueParms"
    '        oHelper.dsLeague.Tables("dtLeagueParms").ReadXml(oHelper.sFilePath & "\" & oHelper.sLeagueName & ".xml")
    '    Else
    '        txtLeagueName.Text = "*error*"
    '        Exit Sub
    '    End If

    '    If File.Exists(oHelper.sFilePath & "\" & oHelper.sLeagueName & "_Schedule.xml") Then
    '        If oHelper.dsLeague.Tables.Contains("dtSchedule") Then
    '            oHelper.dsLeague.Tables.Remove("dtSchedule")
    '        End If

    '        oHelper.dsLeague.Tables.Add("dtSchedule").TableName = "dtSchedule"
    '        oHelper.dsLeague.Tables("dtSchedule").ReadXml(oHelper.sFilePath & "\" & oHelper.sLeagueName & "_Schedule.xml")
    '    Else
    '        txtLeagueName.Text = "*error*"
    '        Exit Sub
    '    End If

    '    'If File.Exists(oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv") Then
    '    '    oHelper.dtLeagueParms = oHelper.CSV2DataTable("\" & txtLeagueName.Text & ".csv")
    '    '    If IsNothing(oHelper.dtLeagueParms) Then
    '    '        txtLeagueName.Text = "Error"
    '    '        Exit Sub
    '    '    Else
    '    '    End If
    '    'Else
    '    '    Me.gbScoring.Controls.Clear()
    '    '    ' Me.gbStrokeIndex.Controls.Clear()
    '    'End If
    'End Sub

End Class
