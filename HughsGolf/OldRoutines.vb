Imports ClosedXML.Excel
Imports HughsGolf.Main
Imports System.Numerics

Public Class OldRoutines
#Region "Scorecard"

    Sub UpdateCTPS(R As DataGridViewRow, bMakeFalse As Boolean)

        'update heirarchy
        '   1)  Automatically add each player as in skins when we load up this grid
        'If ohelper.convDBNulltoSpaces(R.Cells("Skin Paid").Value) = " " Then Exit Sub
        Dim dClosest As Decimal = If(ohelper.sThisDate < spsDate, rLeagueParmrow(closest), rLeagueParmrow($"{closest}PS"))
        Dim dSkin As Decimal = If(ohelper.sThisDate < spsDate, rLeagueParmrow(skin), rLeagueParmrow($"{skin}PS"))

        Dim sClosest As String = "TRUE"
        Dim sSkins As String = "TRUE"
        If ohelper.convDBNulltoSpaces(R.Cells("Skin Paid").Value) = " " Then
            sClosest = "False".ToUpper
            sSkins = sClosest
        ElseIf R.Cells("Skin Paid").Value = dClosest Then
            sClosest = "true".ToUpper
            sSkins = "False".ToUpper
        ElseIf R.Cells("Skin Paid").Value = dSkin Then
            sClosest = "false".ToUpper
            sSkins = "true".ToUpper
        End If

        ohelper.dt = ohelper.sqlitedaFromSql(connection, "", $"Select * from SkinsCTP where Date = {ohelper.sThisDate} And Player = '{ohelper.sPlayer}'")
        sb = New StringBuilder
        If ohelper.dt.Rows.Count = 0 Then
            Dim Lastdt = ohelper.sqlitedaFromSql(connection, "", $"Select ID from SkinsCTP ORDER BY ID DESC LIMIT 1")
            sb.appendline($"
INSERT INTO SkinsCTP (ID,League,Player,Date,Round,Skins,Closest)
VALUES(
{Lastdt(0)(0)},
'{sLeagueName}',
'{ohelper.sPlayer}',
{ohelper.sThisDate},
'',
@Skins,
@Closest
)
")
            Using command As New SQLiteCommand(sb.ToString, connection)
                command.Parameters.AddWithValue("@Skins", sClosest)
                command.Parameters.AddWithValue("@Closest", sSkins)
                command.ExecuteNonQuery()
            End Using
        Else
            sb = New StringBuilder
            sb.appendline(
    $"UPDATE SkinsCTP SET Skins = @Skins, Closest = @Closest
WHERE League = '{sLeagueName}' 
AND Date = {ohelper.sThisDate}
AND Player = '{ohelper.sPlayer}'"
)
            Using command As New SQLiteCommand(sb.ToString, connection)
                command.Parameters.AddWithValue("@Skins", sClosest)
                command.Parameters.AddWithValue("@Closest", sSkins)
                command.ExecuteNonQuery()
            End Using
        End If
        'update the grid instead of datarow cause its already found
        'R.Cells("Skin Paid").Value = dClosest + dSkin
        ohelper.dt = ohelper.sqlitedaFromSql(connection, "", $"Select * from SkinsCTP ORDER BY ID DESC LIMIT 10")
        MarkCPWs(sClosest, R)
        recalcpurse()
        resetSkins()

    End Sub
#Region "needs attention"
    '    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
    ''this subroutine gets a date from the form
    ''gets all scores for that date from scores.csv
    ''format screen colors
    ''matches, subs, noshows, birdies,etc
    ''calculates ctp/skins,purse
    'ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

    'Try
    '    If Not bFormLoad Then
    'If MessageBox.Show(String.Format("Do you want to re-load scores from {0}?", cbDates.SelectedItem), "Scores will be Saved", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand) = DialogResult.OK Then
    '    'save the scores first
    '    btnSave_Click(sender, e)
    '    'oHelper.WaitForFile(DsLeague.Tables("dtScores"), Main.WaitForFile("Scores"), lbStatus, Me)
    'Else
    '    'remove handler so it wont loop(re execute this sub)
    '    RemoveHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged
    '    cbDates.SelectedItem = sThisDate
    '    AddHandler cbDates.SelectedIndexChanged, AddressOf cbDates_SelectedIndexChanged
    '    Exit Sub
    'End If
    ''dont bother reloading if the date didnt change
    'If sThisDate = cbDates.SelectedItem Then Exit Sub
    '    Else
    'bFormLoad = True
    'cbDates.SelectedItem = Main.cbDates.SelectedItem
    '    End If

    '    sThisDate = cbDates.SelectedItem
    '    'set helper date
    '    ohelper.dDate = Date.ParseExact(sThisDate, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    '    ohelper.LOGIT(String.Format("Selected Date {0}", sThisDate))

    '    'check to see if this week overlaps with club championship
    '    ohelper.LOGIT(String.Format("check to see if this week overlaps with club championship"))
    '    If sThisDate >= spsDate And
    '       sThisDate <= CDate(ohelper.rLeagueParmrow("EndDate")).ToString("yyyyMMdd") Then
    'ohelper.bCCLeague = True
    '    Else
    'ohelper.bCCLeague = False
    '    End If

    '    dvScores = New DataView(dsLeague.dtScores)
    '    With dvScores
    '.RowFilter = $"Date = '{sThisDate}'"
    'If sThisDate >= spsDate Then
    '    .RowFilter &= $" and Method <> ''"
    'End If
    '    End With

    '    ohelper.LOGIT(String.Format("Binding Datasource", sThisDate))
    '    ohelper.getMatchScores(cbDates.SelectedItem)
    '    'set front/back radio(set flag so we dont reassign holes)
    '    bSkipFB = True
    '    ohelper.CalcThisHoleMarker(sThisDate, dvScores)

    '    If ohelper.iHoleMarker = 1 Then
    'rbFront.Checked = True
    '    Else
    'rbBack.Checked = True
    '    End If

    '    bSkipFB = False
    '    dgTeams.Visible = True
    '    lbStandingsSnapshot.Visible = True
    '    lbWeek.Visible = True
    '    lbRem.Visible = True
    '    lbWeeksRemaining.Visible = True
    '    If sThisDate >= spsDate Then
    ''this loop looks for players who arent in both skin weeks and are in only one of them
    'lEOYPlayers = New List(Of String)
    'Dim dvp As New DataView(dsLeague.dtPayments)
    ''get both weeks players
    'dvp.RowFilter = $"(Detail = 'Payment' OR Detail = 'Reimbursement') AND SUBSTRING(Date,1,4) = '{sThisDate.ToString.Substring(0, 4)}' AND Desc LIKE 'EOY Skins%' "
    'For Each prow As DataRowView In dvp
    '    If prow("Detail").ToString = "Payment" Then
    'If Not lEOYPlayers.Contains(prow("Player")) Then
    '    If prow("Desc").ToString.Contains("1") Then
    'If sThisDate > spsDate Then
    '    Continue For
    'End If
    '    ElseIf prow("Desc").ToString.Contains("2") Then
    'If sThisDate = spsDate Then
    '    Continue For
    'End If
    '    End If
    '    lEOYPlayers.Add(prow("Player"))
    'End If
    '    ElseIf prow("Detail").ToString = "Reimbursement" Then
    'For i = 0 To lEOYPlayers.Count - 1
    '    If prow("Player") = lEOYPlayers(i) Then
    'lEOYPlayers.RemoveAt(i)
    '    End If
    'Next

    '    End If
    'Next
    'dgTeams.Visible = False
    'lbStandingsSnapshot.Visible = False
    'lbWeek.Visible = False
    'lbRem.Visible = False
    'lbWeeksRemaining.Visible = False
    '    End If

    '    Dim BindingSource = New BindingSource()
    '    BindingSource.DataSource = dvScores
    '    DtScoresBindingSource.DataSource = BindingSource

    '    'fill in 18 totals for league championship
    '    For Each row As DataRowView In DtScoresBindingSource
    'ohelper.LOGIT(String.Format("{0}", row("Player")))
    'ohelper.sPlayer = row("Player").ToString
    ''20220819 players should not be remove from league championship, just skins
    ''remove players not in eoy skins
    'If sThisDate >= spsDate Then
    '    dSkinWorth = 7
    '    dCTPWorth = 3
    '    If Not lEOYPlayers.Contains(ohelper.sPlayer) Then
    ''DtScoresBindingSource.Remove(row)
    ''Continue For
    '    End If
    'End If

    ''if scorecard not already set, then set skins/ctp based on player default 
    'Dim foundrows = dsLeague.dtPlayers.Select($"Name = '{ohelper.sPlayer}'")
    'If foundrows.Count = 0 Then
    '    Throw New Exception(String.Format("Player {0} not found in player file", ohelper.sPlayer))
    'End If
    ''recreate the skins/ctp as checkboxes
    '#Region "Make all checkboxes boolean"
    'If sThisDate < spsDate Then
    '    If ohelper.convDBNulltoSpaces(row(closest)).Trim = "" Then
    'If ohelper.RemoveNulls(foundrows(0)(CTP)) = "Y" And cbDefault.Checked Then
    '    row(closest) = True
    'Else
    '    row(closest) = False
    'End If
    ''End If
    '    End If
    'Else
    '    If lEOYPlayers.Contains(ohelper.sPlayer) Then
    'row(skin) = True
    'row(closest) = True
    '    Else
    'row(skin) = False
    'row(closest) = False
    '    End If
    'End If

    '#End Region
    '    Next
    '    ohelper.LOGIT(String.Format("Calculating 18 hole scores"))

    '    For Each row As DataRowView In DtScoresBindingSource
    'ohelper.sPlayer = row("Player").ToString
    'Dim sccKeys() As Object = {row("Player"), spsDate}
    'Dim arow As DataRow = dsLeague.Tables("dtScores").Rows.Find(sccKeys)
    'Try

    '    For i = 0 To dsLeague.Tables("dtScores").Columns.Count - 1
    ''Debug.Print($"{DsLeague.Tables("dtScores").Columns(i).ColumnName}-{DsLeague.Tables("dtScores").Columns(i).DataType}")
    'If i = 5 Then Continue For
    'If arow(i) Is Nothing Then Continue For
    'If Type.GetTypeCode(dsLeague.Tables("dtScores").Columns(i).DataType) = TypeCode.Int16 Then
    '    row(i) = 0
    '    If arow(i) IsNot DBNull.Value Then
    'row(i) = CInt(arow(i))
    '    End If
    'Else
    '    row(i) = arow(i)
    'End If
    '    Next
    'Catch ex As Exception

    'End Try
    '    Next
    '    'calc18GrossNet()
    '    're-sort grid by low net if post season
    '    If sThisDate >= spsDate Then
    'dvScores.Sort = "18_Net"
    'Dim dv = New DataView(ohelper.dsLeague.Tables("dtScores"))
    ''dv.RowFilter = $"Date = {sThisDate} and Hole1 <> ''"
    ''If dv.Count = 0 Then oHelper.iHoleMarker = 1 Else oHelper.iHoleMarker = 10
    'dv.RowFilter = $"Date = {sThisDate}"
    '    Else
    'If rbStandard.Checked Then
    '    dvScores.Sort = "Partner"
    'Else
    '    dvScores.Sort = "Team_Points Desc, Team Asc"
    'End If
    '    End If


    '    ohelper.LOGIT(String.Format("Set checkboxes, Colors, previous handicap"))
    '    Dim sColumn As DataGridViewColumn
    '    ohelper.iNumClosests = 0
    '    For i = ohelper.iHoleMarker To (ohelper.iHoleMarker - 1) + 9
    'If ohelper.thisCourse("Hole" & i) = 3 Then
    '    ohelper.iNumClosests += 1
    'End If
    '    Next

    '#Region "Add datagridview columns that arent used in the scores table for gridviewview control(clear,match results(unless league championship),Pay,Balance)"
    '    'add a clear scores checkbox on if it already hasnt been built
    '    ohelper.LOGIT(String.Format("add a clear scores button on if it already hasnt been built"))
    '    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = Clear).SingleOrDefault
    '    If sColumn Is Nothing Then
    ''Dim cbClearScores As New DataGridViewCheckBoxColumn
    'Dim cbClearScores As New DataGridViewButtonColumn
    'With cbClearScores
    '    .HeaderText = Clear
    '    .Name = Clear
    '    .DataPropertyName = Clear
    '    .Width = 50
    '    .Text = Clear
    '    .UseColumnTextForButtonValue = True
    '    dgScores.Columns.Insert(0, cbClearScores)
    'End With
    '    End If
    '    If sThisDate < spsDate Then
    'ohelper.CreateDGCol(sPaid, skin)
    'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = sPaid).SingleOrDefault
    ''sColumn.ReadOnly = True
    '    End If
    '    '2021-04-17-replace pay dues with amount Paid
    '    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "TotAmt").SingleOrDefault
    '    If sColumn Is Nothing Then
    'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Opponent").SingleOrDefault
    'Dim tb As New DataGridViewTextBoxColumn
    'With tb
    '    .HeaderText = "Total Amount"
    '    .Name = "TotAmt"
    '    .DataPropertyName = "TotAmt"
    '    .Width = 50
    '    .ReadOnly = True
    '    .Visible = False
    '    .HeaderCell.Style.BackColor = Color.LimeGreen
    '    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    dgScores.Columns.Insert(sColumn.Index, tb)
    'End With
    '    End If

    '    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "AmtPaid").SingleOrDefault
    '    If sColumn Is Nothing Then
    'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "TotAmt").SingleOrDefault
    'Dim tb As New DataGridViewTextBoxColumn
    'With tb
    '    .HeaderText = "Amount Paid"
    '    .Name = "AmtPaid"
    '    .DataPropertyName = "AmtPaid"
    '    .Width = 50
    '    .ReadOnly = False
    '    .Visible = False
    '    .HeaderCell.Style.BackColor = Color.LimeGreen
    '    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    dgScores.Columns.Insert(sColumn.Index, tb)
    'End With
    '    End If

    '    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Balance").SingleOrDefault
    '    If sColumn Is Nothing Then
    'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Opponent").SingleOrDefault
    'Dim tb As New DataGridViewTextBoxColumn
    'With tb
    '    .HeaderText = "Balance"
    '    .Name = "Balance"
    '    .DataPropertyName = "Balance"
    '    .Width = 50
    '    .ReadOnly = True
    '    .Visible = False
    '    .HeaderCell.Style.BackColor = Color.LimeGreen
    '    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    dgScores.Columns.Insert(sColumn.Index, tb)
    'End With
    '    End If

    '    sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "TmNet").SingleOrDefault
    '    If sColumn Is Nothing Then
    'sColumn = (From c In Me.dgScores.Columns Select c Where c.DataPropertyName = "Points").SingleOrDefault
    'Dim tb As New DataGridViewTextBoxColumn
    'With tb
    '    .HeaderText = "Team Net"
    '    .Name = "TmNet"
    '    .DataPropertyName = "TmNet"
    '    .Width = 50
    '    .ReadOnly = True
    '    .HeaderCell.Style.BackColor = Color.LimeGreen
    '    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    '    dgScores.Columns.Insert(sColumn.Index, tb)
    'End With
    '    End If

    '    '20220618-add ctp$ col
    '    Dim iCtps As Int16 = 0
    '    For i = ohelper.iHoleMarker To (ohelper.iHoleMarker - 1) + 9
    'If ohelper.thisCourse($"Hole{i}") = 3 Then
    '    iCtps += 1
    '    Dim scolname = $"CPW{iCtps}"
    '    ohelper.CreateDGCol(scolname, "Player", $"CPW #{i}", Color.LightBlue)
    'End If
    '    Next
    '#End Region

    '    ChangeColAttributes()
    '    Tagclosests()
    '    'reorganize Columns to fit date/parameters(like front 9 and back nine)
    '    '20220716-remove Round from gridview
    '    dgScores.Columns("Round").Visible = False
    '    For Each col As DataGridViewColumn In dgHandicap.Columns
    'col.Width = 20
    '    Next

    '#Region "Calculate CTPs/Skins"
    '    ohelper.CalcLeftovers()
    '    ohelper.LOGIT(String.Format("Calc # CTPS"))
    '    cbMarkPaid.Checked = False
    '    Dim sKeys() As Object
    '    sKeys = {cbDates.Items(cbDates.SelectedIndex)}
    '    thisweeksSkinsCTPS = ohelper.dtnewWklySkins.Rows.Find(sKeys)
    '    'this is a new week if this is nothing
    '    'oHelper.LOGIT(thisweeksSkinsCTPS(ctp2earned))
    '    If thisweeksSkinsCTPS Is Nothing Then
    'thisweeksSkinsCTPS = ohelper.dtnewWklySkins.NewRow
    ''clear this weeks totals, they will be recalculated from datagridview
    'For Each fld As String In {skinscol, skinsearn, skinsextr, "ctp1collected", "ctp1earned", "ctp2collected", "ctp2earned", "ctp1extra", "ctp2extra", kitty}
    '    thisweeksSkinsCTPS(fld) = 0
    'Next
    '    End If

    '    'If cbDates.Items.Count > 1 And cbDates.SelectedIndex < cbDates.Items.Count - 1 Then
    '    RemoveHandler tbPCP1.TextChanged, AddressOf tbPCP1_TextChanged
    '    RemoveHandler tbPCP2.TextChanged, AddressOf tbPCP2_TextChanged
    '    RemoveHandler tbPSkins.TextChanged, AddressOf tbPSkins_TextChanged

    '    If cbDates.Items.Count > 1 And cbDates.SelectedIndex > 0 Then
    'sKeys = {cbDates.Items(cbDates.SelectedIndex - 1)}
    'With ohelper
    '    lastweeksSkinsCTPS = .dtnewWklySkins.Rows.Find(sKeys)
    '    '20210918 - clear last weeks $$ from previous on league Champ
    '    If sThisDate < spsDate Then
    'tbPCP1.Text = lastweeksSkinsCTPS(ctp1extra) '+ lastweeksSkinsCTPS(ctpb1extr)
    'tbPCP2.Text = lastweeksSkinsCTPS(ctp2extra) '+ lastweeksSkinsCTPS(ctpb2extr)
    'tbPSkins.Text = lastweeksSkinsCTPS(skinsextr)
    '    Else
    'tbPCP1.Text = 0
    'tbPCP2.Text = 0
    'tbPSkins.Text = 0
    'tbKitty.Text = 0
    '    End If
    'End With
    '    Else
    ''this is the first week of the year
    'tbPCP1.Text = 0
    'tbPCP2.Text = 0
    'tbPSkins.Text = 0
    'tbKitty.Text = 0
    '    End If
    '    If tbPCP1.Text < 0 Then tbPCP1.Text = 0
    '    If tbPCP2.Text < 0 Then tbPCP2.Text = 0
    '    tbCP1.Text = thisweeksSkinsCTPS(ctp1collected) '+ thisweeksSkinsCTPS(ctpb1col)
    '    tbCP2.Text = thisweeksSkinsCTPS(ctp2collected) ' + thisweeksSkinsCTPS(ctpb2col)
    '    tbCP1Tot.Text = CDec(tbPCP1.Text) + CDec(tbCP1.Text)
    '    tbCP2Tot.Text = CDec(tbPCP2.Text) + CDec(tbCP2.Text)

    '    tbSkins.Text = thisweeksSkinsCTPS(skinscol)
    '    tbSkinTot.Text = CInt(tbPSkins.Text) + CInt(tbSkins.Text)
    '    tbNumSkins.Text = thisweeksSkinsCTPS(skinsextr)
    '    tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)
    '    tbKitty.Text = thisweeksSkinsCTPS(kitty) 'thisweeksSkinsCTPS(skinsextr) + thisweeksSkinsCTPS("ctplo")
    '    If tbKitty.Text > 0 Then
    'Dim mbr = MsgBox($"Kitty has {tbKitty.Text} carried over, want to zero it out?", MsgBoxStyle.YesNo)
    'If mbr = MsgBoxResult.Yes Then
    '    tbKitty.Text = 0
    'End If
    '    End If

    '    ohelper.LOGIT(String.Format("Date:  {0} Leftover Skins {1}", ohelper.dDate, ohelper.dThisWeeksSkins))
    '    AddHandler tbPCP1.TextChanged, AddressOf tbPCP1_TextChanged
    '    AddHandler tbPCP2.TextChanged, AddressOf tbPCP2_TextChanged
    '    AddHandler tbPSkins.TextChanged, AddressOf tbPSkins_TextChanged

    '#End Region
    '#Region "Calculate this weeks CTP/Skins from Gridview"
    '    ohelper.LOGIT(String.Format("Calculate this weeks skins/ctps from Gridview"))

    '    If sThisDate < spsDate Then
    'If thisweeksSkinsCTPS(ctp1earned) + thisweeksSkinsCTPS(ctp2earned) > 0 Then
    '    dgScores.Columns(closest).Visible = False
    'Else
    '    dgScores.Columns(closest).Visible = True
    'End If
    '    Else
    ''recalc kitty from just this weeks if its league championship
    'tbKitty.Text = thisweeksSkinsCTPS("skinPlayers") * 10 - CDec(tbPurse.Text)
    '    End If

    '    ohelper.LOGIT(String.Format("Reset Skins"))
    '    resetSkins()
    '    ohelper.LOGIT(String.Format("Date:    {0} Leftover Skins {1}", ohelper.dDate, tbSkins.Text))
    '#End Region

    '    If Not IO.Directory.Exists("C:\temp") Then
    'My.Computer.FileSystem.CreateDirectory("C:\Temp")
    '    End If
    '    '--------------------
    '    'get Balance info
    '    '--------------------
    '    If IO.File.Exists("c:\temp\temp.txt") Then
    'IO.File.Delete("c:\temp\temp.txt")
    '    End If

    '    ohelper.DataTable2CSV(dsLeague.dtPayments, "c:\temp\temp.txt")
    '    Debug.Flush()
    '    '20210109-calculate balance
    '    Dim strsql As String = String.Format("SELECT Player, SUM([Earned]) as Balance FROM [temp.txt]")
    '    strsql += vbCrLf
    '    'strsql += String.Format("WHERE (Date >= {0} And Date <= {1} And Detail = 'Invoice') Or (Date >= {0} And Date < {1} And Desc Not in ('Food','Drinks'))", CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    'strsql += String.Format("WHERE Date >= {0} And Date <= {1} And Detail in ('Invoice','Payment') And Desc = 'League Dues'", CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    strsql += String.Format("WHERE Date >= {0} And Date <= {1} And Detail in ('Invoice','Payment') ", CDate(ohelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    strsql += vbCrLf & "GROUP BY Player"
    '    Dim cn As New OleDb.OleDbConnection(String.Format("Provider={0};Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';", ohelper.sMACDBVersion))
    '    Dim dap As New Data.OleDb.OleDbDataAdapter(strsql, cn)
    '    Dim et As TimeSpan
    '    Dim sst As DateTime = Now
    '    dap.Fill(dtBalance)
    '    dtBalance.PrimaryKey = New DataColumn() {dtBalance.Columns("Player")}
    '    et = Now - sst
    '    'oHelper.LOGIT(et.ToString)

    '    '--------------------
    '    'get Payment info
    '    '--------------------
    '    If IO.File.Exists("c:\temp\temp.txt") Then
    'IO.File.Delete("c:\temp\temp.txt")
    '    End If

    '    ohelper.DataTable2CSV(dsLeague.dtPayments, "c:\temp\temp.txt")
    '    Debug.Flush()
    '    '20210109-calculate balance
    '    strsql = String.Format("SELECT Player, SUM([Earned]) as AmtPaid FROM [temp.txt]")
    '    strsql += vbCrLf
    '    'strsql += String.Format("WHERE (Date >= {0} And Date <= {1} And Detail = 'Invoice') Or (Date >= {0} And Date < {1} And Desc Not in ('Food','Drinks'))", CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    strsql += String.Format("WHERE Date >= {0} And Date <= {1} And Detail in ('Payment') ", CDate(ohelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    strsql += vbCrLf & "GROUP BY Player"
    '    dap = New Data.OleDb.OleDbDataAdapter(strsql, cn)
    '    et = New TimeSpan
    '    sst = Now
    '    dap.Fill(dtThisYearsPayments)
    '    dtThisYearsPayments.PrimaryKey = New DataColumn() {dtThisYearsPayments.Columns("Player")}
    '    et = Now - sst
    '    'oHelper.LOGIT(et.ToString)

    '    '--------------------
    '    'get Regulars
    '    '--------------------
    '    If IO.File.Exists("c:\temp\temp.txt") Then
    'IO.File.Delete("c:\temp\temp.txt")
    '    End If

    '    ohelper.DataTable2CSV(dsLeague.dtPayments, "c:\temp\temp.txt")
    '    Debug.Flush()
    '    '20210109-calculate balance
    '    strsql = $"SELECT Player FROM [temp.txt]{vbCrLf}"
    '    strsql += String.Format("WHERE Date >= {0} And Date <= {1} And Detail = 'Invoice' ", CDate(ohelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    strsql += vbCrLf & "GROUP BY Player"
    '    dap = New Data.OleDb.OleDbDataAdapter(strsql, cn)
    '    dap.Fill(dtThisYearsRegulars)
    '    dtThisYearsRegulars.PrimaryKey = New DataColumn() {dtThisYearsRegulars.Columns("Player")}

    '    GetTeamNet()

    '    'oHelper.LOGIT(String.Format("{0}-{1}", dgScores.Rows(0).Cells("Player").Value, dgScores.Rows(0).Cells("Hdcp").Value))
    '#Region "Set checkboxes, Colors, previous handicap"
    '    'loop through scores 
    '    ' setting Skins / ctp checkboxes instead of y/n
    '    ' coloring holes for strokes
    '    ' recalculating net scores to be gross
    '    'shade every other match light blue
    '    For Each row As DataGridViewRow In dgScores.Rows
    ''Dim bSub = False
    'If sThisDate < spsDate Then
    '    Dim regrows = dtThisYearsRegulars.Select(String.Format("Player = '{0}'", row.Cells("Player").Value))
    '    If regrows.Length = 0 Then
    'row.Cells("Player").Style.BackColor = Color.Aqua
    ''bSub = True
    '    End If
    'End If

    'If ohelper.iHoleMarker = 1 Then
    '    'https://stackoverflow.com/questions/8255186/how-to-check-empty-and-null-cells-in-datagridview-using-c-sharp
    '    'If String.IsNullOrWhiteSpace(TryCast(row("Out_Gross"), String)) Then
    '    If Not IsNumeric(row.Cells("Out_Gross").Value) Then
    'If sThisDate >= spsDate Then
    '    If Not lEOYPlayers.Contains(row.Cells("Player").Value) Then
    'row.Cells("Player").Style.BackColor = Color.LightBlue
    'Debug.Print($"{row.Cells("Player").Value} - {row.Index}")
    '    End If
    'Else
    '    row.Cells("Player").Style.BackColor = Color.LightPink
    'End If
    '    End If
    'Else
    '    'If String.IsNullOrWhiteSpace(TryCast(row("In_Gross"), String)) Then
    '    If Not IsNumeric(row.Cells("In_Gross").Value) Then
    'If sThisDate >= spsDate Then
    '    If Not lEOYPlayers.Contains(row.Cells("Player").Value) Then
    'row.Cells("Player").Style.BackColor = Color.LightBlue
    'Debug.Print($"{row.Cells("Player").Value} - {row.Index}")
    '    End If
    'Else
    '    row.Cells("Player").Style.BackColor = Color.LightPink
    'End If
    '    End If
    'End If
    'If sThisDate < spsDate Then
    '    If rbStandard.Checked Then
    'If row.Index Mod 4 = 0 Then
    '    getMatchPts(row)
    'End If
    ''oHelper.ColorWinners(dgScores, row.Index)
    'row.Cells("Status").Value = DtScoresBindingSource.List(row.Index)("Status").ToString
    'If row.Cells("Grade").Value = "A" Then
    '    'Dim MyPlayer As DataRow = DsLeague.Tables("dtPlayers").Rows.Find(oHelper.sPlayer)
    '    'If R("Team") = "" Then R("Team") = MyPlayer("Team")
    '    Dim arow As DataRow = dtTeamnet.Rows.Find(row.Cells("Team").Value)
    '    Dim sNet As String = If(row.Cells("Out_Net").Visible, "Out_Net", "In_Net")
    '    row.Cells("TmNet").Value = arow(sNet)
    'End If
    '    End If
    'End If
    'Dim foundrows = dtBalance.Select(String.Format("Player = '{0}'", row.Cells("Player").Value))
    ''oHelper.LOGIT(foundrows(0)("Balance"))
    'If foundrows.Length > 0 Then
    '    '2022-05-11-chck for paid in full
    '    Dim dBalance As Decimal = foundrows(0)("Balance")
    '    row.Cells("Balance").Value = dBalance
    '    If dBalance < 0 Then
    ''If dBalance < oHelper.rLeagueParmrow("") Then

    'If dBalance = -20 Then
    '    row.Cells("Balance").Style.BackColor = Color.Pink
    'Else
    '    row.Cells("Balance").Style.BackColor = Color.Red
    'End If
    '    Else
    ''2021-04-17 col removed
    ''row.Cells("Pay").Value = False
    ''row.Cells("Pay") = New DataGridViewTextBoxCell
    ''row.Cells("Pay").Value = ""
    ''row.Cells("Pay").ReadOnly = True
    '    End If
    'End If
    'foundrows = dtThisYearsPayments.Select(String.Format("Player = '{0}'", row.Cells("Player").Value))
    'If foundrows.Length > 0 Then
    '    row.Cells("TotAmt").Value = foundrows(0)("AmtPaid")
    '    If row.Cells("Balance").Value = 0 Then
    'row.Cells("AmtPaid").ReadOnly = True
    'row.Cells("Balance").Value = ""
    '    End If
    'End If
    ''20210605-make ctp1,2 invisible if not in closest
    ''https://stackoverflow.com/questions/39114987/datagridview-change-cell-of-a-typed-column-to-another-type
    ''If row.Cells(closest).Value.ToString.ToUpper <> "TRUE" Then

    ''row.Cells(CTP2).ReadOnly = True
    '''If TypeOf dgScores.CurrentCell Is DataGridViewTextBoxCell Then Exit Sub
    '''recreate cells as textboxes
    ''Dim ncell As New DataGridViewTextBoxCell
    ''row.Cells(CTP1) = ncell
    ''row.Cells(CTP1).ReadOnly = True
    ''ncell = New DataGridViewTextBoxCell
    ''row.Cells(CTP2) = ncell
    ''row.Cells(CTP2).ReadOnly = True

    ''End If
    ''2021-04-17 col removed
    ''remove checkbox on subs
    ''If bSub Then
    ''    row.Cells("Pay").Value = False
    ''    row.Cells("Pay") = New DataGridViewTextBoxCell
    ''    row.Cells("Pay").Value = ""
    ''    row.Cells("Pay").ReadOnly = True
    ''End If
    ''20201006-move to scorecard
    'Dim r = row
    'r.Cells("Hdcp").ToolTipText = ""
    'For Each score In GetLast5(row, ohelper.dDate)
    '    r.Cells("Hdcp").ToolTipText &= score.Split("-")(0) & "-"
    'Next
    'r.Cells("Hdcp").ToolTipText = r.Cells("Hdcp").ToolTipText.Trim("-")
    ''20201006-end
    'ohelper.LOGIT($"{r.Cells("Player").Value}-Last 5 Scores {r.Cells("Hdcp").ToolTipText}")
    '    Next
    '#End Region
    '    If sThisDate < spsDate Then CalcStandings()
    '    'dgScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.Fill
    '    dgScores.Refresh()

    '    bFormLoad = False

    '    'oHelper.LOGIT(String.Format("exiting {0}-{1}", dgScores.Rows(0).Cells("Player").Value, dgScores.Rows(0).Cells("Hdcp").Value))
    '    ohelper.LOGIT(String.Format("Resize dgScores"))
    '    lbMonitor.Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}, Resolution {1} x {2}, Menu {3} x {4}", My.Computer.Name, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Me.Width, Me.Height)
    '    ohelper.Resizedgv(dgScores, Me)

    '    'For Each ctl In gbScorecard.Controls
    '    '    oHelper.LOGIT(ctl.text)
    '    '    Dim x = ""
    '    'Dim xloc = ctl.location.x
    '    'If ctl.checked = True Then
    '    '    'BuildScoreCardMethods = ctl.text
    '    '    'Exit Function
    '    'End If
    '    'Next

    '    'If dgScores.Width + 25 > gbScores.Width Then
    '    'End If

    '    Dim xloc As Integer = dgTeams.Location.X
    '    Dim yloc As Integer = dgTeams.Location.Y
    '    Dim dgteamsW As Integer = dgTeams.Width
    '    Dim dgteamsH As Integer = dgTeams.Height

    '    gbScores.Width = dgScores.Width + 25
    '    gbScores.Height = dgScores.Height + 55
    '    If gbScorecard.Width > gbScores.Width Then
    'Me.Width = gbScorecard.Width * 1.1
    '    Else
    'Me.Width = gbScores.Width + 40
    '    End If
    '    Me.Height = (gbScorecard.Height * 1.25) + (gbScores.Height * 1.25)
    '    ohelper.LOGIT(String.Format("W/H,dgv-gb-form {0}-{1},{2}-{3},{4}-{5}", dgScores.Width, dgScores.Height, gbScorecard.Width, gbScorecard.Height, Me.Width, Me.Height))
    '    'CalcStandings()

    '    Dim ii = 0
    '    For i = ohelper.iHoleMarker To (ohelper.iHoleMarker - 1) + 9
    'If ohelper.thisCourse("Hole" & i) = 3 Then
    '    dgScores.Columns($"CTP_{ii + 1}").HeaderText = $"CTP# {i}"
    '    dgScores.Columns($"CTP_{ii + 1}").Visible = False
    '    ii += 1
    'End If
    '    Next
    '    '20220622 remove checkboxes
    '    dgScores.Columns(skin).Visible = False
    '    dgScores.Columns(closest).Visible = False


    '    If tbPCP1.Text > 0 Then
    ''If MessageBox.Show($"CTP shows a carryover amount of {tbPCP1.Text}{vbCrLf}Want to keep it in this weeks Total?", "Carryover CTP Money", MessageBoxButtons.YesNo) = DialogResult.No Then
    'tbPCP1.Text = 0

    ''End If
    '    End If
    '    If tbPCP2.Text > 0 Then
    'tbPCP2.Text = 0
    '    End If
    '    If tbPSkins.Text > 0 Then
    ''If MessageBox.Show($"Skins show a carryover amount of {tbPSkins.Text}{vbCrLf}Want to keep it in this weeks Total?", "Carryover Skin Money", MessageBoxButtons.YesNo) = DialogResult.No Then
    'tbPSkins.Text = 0
    ''End If
    '    End If

    '    Debug.Print($"{dgScores.Rows(9).Cells("Player").Style.BackColor} - Color")
    'Catch ex As Exception
    '    MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '    ''MsgBox(oHelper.GetExceptionInfo(ex))
    'End Try
    '    End Sub
    Public Class MatchResults
        Private Match As TeamMatch 'should have entry

        '1-Find Home Team A Player
        '1-Update Team Net score
        '2-Calc Individual Match updating individual points
        '2-     Update this rows points 
        '2-     Update opponents rows points 
        '2-     Calc which row the opponent is in, if the row index(of 0-3) <= 1, its the home team, else its the away team 
        '3-if a player, Calculate Team points in dr(index) row
        '3-     Make team points in b player null  
        '3-     Make team points in b player opponent null
        '3-     Calculate team points in a player opponent 
        '4-if b player, Calculate Team points in a players row 
        '4-     Calculate A players row by subtracting 1 row from index
        '4-     Make team points in b player null  
        '4-     Make team points in b player opponent null
        '4-     Calculate team points in a player opponent 

        'Public Sub New(dv As )
        '    iPlayerPoints =

        'End Sub

    End Class
#Region "todo see label MovedToOldroutimes in Scorecard"
    'payment heirarchy 
    'dues first
    'eoy skins 2nd
    '--------------------
    'get Payment info
    '--------------------
    '    If IO.File.Exists("c:\temp\temp.txt") Then
    'IO.File.Delete("c:\temp\temp.txt")
    '    End If

    '    ohelper.DataTable2CSV(dsLeague.dtPayments, "c:\temp\temp.txt")
    '    Debug.Flush()

    '    Dim dt As New DataTable
    '    Dim strsql = String.Format("SELECT Player, [Earned], Desc, Detail FROM [temp.txt]")
    '    strsql += vbCrLf
    '    'strsql += String.Format("WHERE (Date >= {0} And Date <= {1} And Detail = 'Invoice') Or (Date >= {0} And Date < {1} And Desc Not in ('Food','Drinks'))", CDate(oHelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate)
    '    strsql += String.Format("WHERE Date >= {0} And Date <= {1} And Detail in ('Payment') and Player = '{2}'", CDate(ohelper.rLeagueParmrow("StartDate")).ToString("yyyyMMdd"), sThisDate, ohelper.sPlayer)
    '    'strsql += vbCrLf & "GROUP BY Player"
    '    Dim cn As New OleDb.OleDbConnection(String.Format("Provider={0};Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';", ohelper.sMACDBVersion))
    '    Dim dap = New Data.OleDb.OleDbDataAdapter(strsql, cn)
    '    dap.Fill(dt)
    '    Dim dv = New DataView(dt)
    '    dv.RowFilter = "Desc = 'League Dues'"
    '    Dim iamt = 0
    '    For Each row In dv
    'iamt += row("Earned")
    '    Next

    '    Dim arow = dsLeague.dtPayments.NewRow
    '    If iamt < ohelper.rLeagueParmrow("Cost") Then
    'If sPayamt >= ohelper.rLeagueParmrow("Cost") Then
    '    arow("Player") = ohelper.sPlayer
    '    arow("Date") = ohelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    '    arow("Desc") = "League Dues"
    '    arow("Detail") = "Payment"
    '    arow("Earned") = ohelper.rLeagueParmrow("Cost")
    '    arow("DatePaid") = Now 'oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    '    arow("Comment") = scomment
    '    arow("PayMethod") = sPayMethod
    '    dsLeague.dtPayments.Rows.Add(arow)
    '    ohelper.DataTable2CSV(dsLeague.dtPayments, ohelper.sFilePath & "\Payments.csv")
    '    Debug.Flush()
    '    If sPayamt > ohelper.rLeagueParmrow("Cost") Then
    'arow = dsLeague.dtPayments.NewRow
    'arow("Player") = ohelper.sPlayer
    'arow("Date") = ohelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    'arow("Desc") = "EOY Skins"
    'arow("Detail") = "Payment"
    'arow("Earned") = sPayamt - ohelper.rLeagueParmrow("Cost")
    'arow("DatePaid") = Now 'oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    'arow("Comment") = scomment
    'arow("PayMethod") = sPayMethod
    'dsLeague.dtPayments.Rows.Add(arow)
    'ohelper.DataTable2CSV(dsLeague.dtPayments, ohelper.sFilePath & "\Payments.csv")
    'Debug.Flush()
    '    End If
    'Else
    '    arow = dsLeague.dtPayments.NewRow
    '    arow("Player") = ohelper.sPlayer
    '    arow("Date") = ohelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    '    arow("Desc") = "League Dues"
    '    arow("Detail") = "Payment"
    '    arow("Earned") = sPayamt
    '    arow("DatePaid") = Now 'oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    '    arow("Comment") = scomment
    '    arow("PayMethod") = sPayMethod
    '    dsLeague.dtPayments.Rows.Add(arow)
    '    ohelper.DataTable2CSV(dsLeague.dtPayments, ohelper.sFilePath & "\Payments.csv")
    '    Debug.Flush()
    'End If
    '    Else
    'dv.RowFilter = "Desc = 'EOY Skins'"
    'iamt = 0
    'For Each row In dv
    '    iamt += row("Earned")
    'Next
    'arow = dsLeague.dtPayments.NewRow
    'arow("Player") = ohelper.sPlayer
    'arow("Date") = ohelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    'arow("Desc") = "EOY Skins"
    'arow("Detail") = "Payment"
    'arow("Earned") = sPayamt - iamt
    'arow("DatePaid") = Now 'oHelper.dDate.ToString("yyyyMMdd", Globalization.CultureInfo.CurrentCulture)
    'arow("Comment") = scomment
    'arow("PayMethod") = sPayMethod
    'dsLeague.dtPayments.Rows.Add(arow)
    'ohelper.DataTable2CSV(dsLeague.dtPayments, ohelper.sFilePath & "\Payments.csv")
    'Debug.Flush()
    '    End If
    '    Dim foundrows = dsLeague.dtPlayers.Select(String.Format("Name = '{0}'", ohelper.sPlayer))
    '    If foundrows IsNot Nothing Then
    'Dim ToAddresses = New List(Of String)
    'Dim attachs() As String = Nothing '{"d:\temp_Excell226.xlsx", "d:\temp_Excell224.xlsx", "d:\temp_Excell225.xlsx"}
    'Dim subject As String = "League Payment"
    'Dim body As String = String.Format("Thanks {0} for your payment of {1}{2}Your Balance is {3}", R.Cells("Player").Value, sPayamt, vbCrLf, R.Cells("Balance").Value)
    'Dim bresult = False

    'If foundrows(0)("EmailStats") IsNot DBNull.Value Then
    '    If foundrows(0)("EmailStats") = "Y" Then
    'ToAddresses.Add(foundrows(0)("Email"))
    '    End If
    'End If
    'If foundrows(0)("TextStats") IsNot DBNull.Value Then
    '    If foundrows(0)("TextStats") = "Y" Then
    'ToAddresses.Add(ohelper.TextNumber(foundrows(0)("Name")))
    '    End If
    'End If
    'If ToAddresses.Count > 0 Then
    '    bresult = ohelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
    '    If bresult Then
    'MsgBox("Payment Notice sent successfully", MsgBoxStyle.Information)
    '    Else
    'MsgBox(ohelper.GGmail.ErrorText, MsgBoxStyle.Critical)
    '    End If
    'End If
    'If Not bresult Then
    '    If MessageBox.Show(String.Format("Do you ({0}) want an Email Receipt emailed to {1}?", R.Cells("Player").Value, foundrows(0)("Email")), "Payment Made!", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) = DialogResult.Yes Then
    'ToAddresses.Add(foundrows(0)("Email"))
    'bresult = ohelper.GGmail.SendMail(ToAddresses, subject, body, attachs)
    'If bresult Then
    '    MsgBox("Payment Email sent successfully", MsgBoxStyle.Information)
    'Else
    '    MsgBox(ohelper.GGmail.ErrorText, MsgBoxStyle.Critical)
    'End If
    '    End If
    'End If
    '    End If
    'End If

    'If R.Cells("Balance").Value = 0 Then
    '    R.Cells("AmtPaid").ReadOnly = True
    '    Dim scolor = Color.White
    '    '0-3,8-11,16-19,etc light blue
    '    If R.Index Mod 8 = 0 Then
    'For i = 0 To 3
    '    If R.Index = R.Index + i Then
    'scolor = Color.LightBlue
    'Exit For
    '    End If
    'Next
    '    End If
    '    R.Cells("Balance").Value = ""
    '    R.Cells("Balance").Style.BackColor = scolor
    'End If

#End Region
#End Region
    'Sub ClearDt(index As Integer)
    '    Dim scallingmethod As String = (New Diagnostics.StackTrace).GetFrame(1).GetMethod.Name
    '    ohelper.LOGIT($"Entering {Reflection.MethodBase.GetCurrentMethod.Name} called by {scallingmethod}")

    '    Dim dgr As DataGridViewRow = dgScores.CurrentRow
    '    Dim R As DataRowView = dvScores.Current
    '    R("Hdcp") = DBNull.Value 'R("PHdcp")
    '    R("Method") = DBNull.Value
    '    R("Round") = DBNull.Value
    '    For i = ohelper.iHoleMarker To ohelper.iHoleMarker + 8
    'R("Hole" & i) = DBNull.Value 'dgScores.DefaultCellStyle.Format
    '    Next
    '    If ohelper.iHoleMarker = 1 Then
    'R("Out_Gross") = DBNull.Value
    'R("Out_Net") = DBNull.Value
    '    Else
    'R("In_Gross") = DBNull.Value
    'R("In_Net") = DBNull.Value
    '    End If

    '    R("18_Gross") = DBNull.Value
    '    R("18_Net") = DBNull.Value
    '    R(earned) = DBNull.Value
    '    R(skinamt) = DBNull.Value
    '    R(closestamt) = DBNull.Value
    '    R("Status") = DBNull.Value
    '    R("CTP_1") = DBNull.Value
    '    R("CTP_2") = DBNull.Value
    '    If sThisDate < spsDate Then

    'getMatchPts(dgr)
    'ResetColor(index)
    'Dim soppName As String = R("Opponent")
    'Dim iopp As Int16 = 0
    'For Each row In dvScores
    '    If row("Player") = soppName Then
    'row("Status") = DBNull.Value
    'Dim x = ""
    'If row("Grade") = "A" Then
    '    row(teampoints) = "0.5"
    '    dgr.Cells(teampoints).Style.BackColor = Color.Yellow
    'End If
    'row(points) = "0.5"
    'dgr.Cells(points).Style.BackColor = Color.Yellow
    'dgr = dgScores.Rows(iopp)
    ''dgr.Cells("Opponent").Style.BackColor = Color.Yellow
    'Exit For
    '    End If
    '    iopp += 1
    'Next
    '    End If

    '    resetSkins()

    'End Sub
    'Sub Tagclosests()
    '    Dim scallingmethod As String = (New Diagnostics.StackTrace).GetFrame(1).GetMethod.Name
    '    ohelper.LOGIT($"Entering {Reflection.MethodBase.GetCurrentMethod.Name} called by {scallingmethod}")

    '    '20220511-query dsleague.dtscores to calc this weeks skins/ctp/leftovers/purse
    '    If IO.File.Exists("c:\temp\temp.txt") Then
    'IO.File.Delete("c:\temp\temp.txt")
    '    End If
    '    ohelper.DataTable2CSV(dtScores, "c:\temp\temp.txt")

    '    'Dim dt As New DataTable
    '    Dim strsql = $"SELECT Count(*) as Total FROM [temp.txt]{vbCrLf}WHERE "
    '    strsql += $"Date = {ohelper.sThisDate}{vbCrLf}"
    '    strsql += $"and{vbCrLf}"
    '    strsql += $"{closest} = 'True'{vbCrLf}"
    '    'strsql += "GROUP BY closest"
    '    Dim dt As New DataTable
    '    dt = ohelper.sqliteda(connection, "dtScores", strsql)

    '    If dt.Rows(0)("Total") = 0 Then
    'tbCP1.Text = 0
    'tbCP2.Text = 0
    'For Each col As DataGridViewColumn In dgScores.Columns
    '    If col.Name.StartsWith("CP") Then
    'col.Visible = False
    '    End If
    'Next
    '    Else
    'tbCP1.Text = dt.Rows(0)("Total") / 2
    'tbCP2.Text = dt.Rows(0)("Total") / 2
    'If ohelper.sThisDate >= spsDate Then
    '    tbCP1.Text = (dt.Rows(0)("Total") / 2) * dCTPWorth
    '    tbCP2.Text = (dt.Rows(0)("Total") / 2) * dCTPWorth
    'End If
    'For Each col As DataGridViewColumn In dgScores.Columns
    '    If col.Name.StartsWith("CP") Then
    'col.Visible = True
    '    End If
    'Next
    '    End If
    '    recalcpurse()
    '    For Each row As DataGridViewRow In dgScores.Rows
    'If row.Cells(closest).Value = True Then
    '    For i = 1 To ohelper.iNumClosests
    'row.Cells($"CPW{i}").ReadOnly = False
    '    Next
    '    'row.Cells($"{sPaid}").Value = CStr((dCTPWorth * 2) + dSkinWorth).Split(".")(0)
    'End If
    '    Next
    'End Sub
#Region "MovetoOldrouties2"
    'tbSkins.Text = CStr(iplys * dSkinWorth)

    ''dvScores.RowFilter = $"Date = {cbDates.SelectedItem} And closest = 'true'"

    'tbCP1.Text = CStr((iplys * dCTPWorth) / ohelper.iNumClosests).Split(".")(0)
    'tbCP2.Text = CStr((iplys * dCTPWorth) / ohelper.iNumClosests).Split(".")(0)

    'tbCP1.Text = CStr(iplys * dCTPWorth).Split(".")(0)
    'tbCP2.Text = CStr(iplys * dCTPWorth).Split(".")(0)

    'Try
    '    recalcpurse()
    '    'tbSkinTot.Text = addtexts(tbSkins.Text, tbPSkins.Text)
    '    tbKitty.Text = iplys Mod 2
    '    'tbCP1Tot.Text = addtexts(tbCP1.Text, tbPCP1.Text)
    '    'tbCP2Tot.Text = addtexts(tbCP2.Text, tbPCP2.Text)
    'Catch ex As Exception
    '    MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    'End Try
    'tbPurse.Text = CDec(tbCP1Tot.Text) + CDec(tbCP2Tot.Text) + CDec(tbSkinTot.Text)

#End Region
    'Function ResetColor(dgvrow As Int16) As Color

    '    '20220624-reset player color to pink
    '    dgScores.Rows(dgvrow).Cells("Player").Style.BackColor = Color.Pink
    '    Dim iptr As Integer = dgvrow - dgvrow Mod 4
    '    ResetColor = Color.White
    '    '0-3,8-11,16-19,etc light blue
    '    If CInt((dgvrow / 4)) Mod 2 = 0 Then ResetColor = Color.LightBlue
    'End Function
#End Region
    'Private Sub GetTeamNet()

    '    ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try
    'Dim dtTeamnet = New DataTable("TeamNet")

    'If IO.File.Exists("c: \temp\temp.txt") Then
    '    IO.File.Delete("c:\temp\temp.txt")
    'End If
    'Dim dvScores = New DataView(dsLeague.dtScores)
    'dvScores.RowFilter = String.Format("Date = {0}", cbDates.SelectedItem)
    'Dim dt As DataTable = dvScores.ToTable(False, "Team,Out_Net,In_Net".Split(",").ToArray)
    'For Each Score In dt.Rows
    '    If Score("Out_Net") Is DBNull.Value Then Score("Out_Net") = 999
    '    If Score("In_Net") Is DBNull.Value Then Score("In_Net") = 999
    'Next

    'ohelper.DataTable2CSV(dt, "c:\temp\temp.txt")
    'Dim strsql = String.Format("SELECT Team,SUM(Out_Net) as Out_Net, SUM(In_Net) as In_Net FROM [temp.txt]")
    'strsql += vbCrLf & "GROUP BY Team "
    'Dim cn As New OleDb.OleDbConnection(String.Format("Provider={0};Data Source=C:\temp;Extended Properties='text;HDR=Yes;FMT=Delimited';", ohelper.sMACDBVersion))
    'Dim dap As New Data.OleDb.OleDbDataAdapter(strsql, cn)
    'dap.Fill(dtTeamnet)
    'dtTeamnet.PrimaryKey = New DataColumn() {dtTeamnet.Columns("Team")}

    '    Catch ex As Exception
    'MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '    End Try

    'End Sub
    'Function GetLast5(row As DataGridViewRow, sDate As String) As List(Of String)
    '    ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try

    'GetLast5 = New List(Of String)
    ''this subroutine assumes dvscores is only one players scores and is sorted by date low to high
    ''1) read a score
    ''2) build 5 score array 
    ''3) rounds 1-3
    ''   a)  update 5 scores array
    ''   b)  calculate hdcp each score (1- below)
    ''   c)  update hdcp in dvscores 1-3 (2- below)
    ''   d)  go to 1)
    ''4) round 4
    ''   a)  loop through the array finding the highest score
    ''   b)  loop through the array again totaling the 3 scores that dont match the highest score
    ''   c)  calculate hdcp each score (1- below)
    ''   d)  update hdcp in dvscores row 4 (2- below)
    ''   e)  go to 1)
    ''5) round 5
    ''   a)  loop through the array finding the highest and lowest scores
    ''   b)  loop through the array again totaling the 3 scores that dont match the highest and lowest scores
    ''   c)  calculate hdcp each score (1- below)
    ''   d)  update hdcp in dvscores row 5 (2- below)
    ''   e)  go to 1)
    ''6) rounds 6-99
    ''   a)  remove score 1 from the array
    ''   b)  add score 6 to end of array
    ''   c)  drop highest and lowest scores moving 3 scores into 3 score array
    ''   d)  calculate hdcp each score (1- below)
    ''   e)  update hdcp in dvscores row 6 - 99 (2- below)
    ''   f)  go to 1)
    ''
    ''1-calculate handicap = (scores - par) / rounds * .8 using array in step 2)
    ''2-Update handicap in dvscores for the round were processing
    ''

    'iLast5Scores = New List(Of String)
    ''Dim iPHdcp = 0
    ''Dim sMethod As String = ""
    'Dim sScore = ""

    ''dates past the regular season dont get included in handicap calculations
    'Dim lignoreDates = New List(Of String)
    'For Each lparm As DataRow In dsLeague.Tables("dtLeagueParms").Rows
    '    lignoreDates.Add(CDate(lparm("PostSeasonDt")).ToString("yyyyMMdd"))
    '    lignoreDates.Add(CDate(lparm("PostSeasonDt")).AddDays(7).ToString("yyyyMMdd"))
    'Next
    'Dim dvScores As New DataView(dsLeague.Tables("dtScores")) With {
    '    .RowFilter = String.Format(Globalization.CultureInfo.CurrentCulture, "Player = '{0}' and Date <= '{1}' and Method <> '' and date not in ('{2}')", row.Cells("Player").Value, CDate(sDate).ToString("yyyyMMdd"), String.Join("','", lignoreDates)),
    '    .Sort = "Date desc"
    '}
    'Dim x = ohelper.sPlayer
    'Dim xx = row.Cells("Player").Value
    ''build a total of 5 max scores in our array
    'Dim iScore As Integer = 0
    'For Each score As DataRowView In dvScores
    '    If ohelper.iHoles = 9 Then
    'Try
    '    'this put in cause Ben Wright got bit by a dog on #11 on 20180529 and had to leave in the middle of his round
    '    If score("Out_Gross") Is DBNull.Value And score("In_Gross") Is DBNull.Value Then
    'Continue For
    '    End If
    '    iScore = If(score("Out_Gross") Is DBNull.Value, score("In_Gross"), score("Out_Gross"))
    'Catch ex As Exception

    'End Try
    '    Else
    'iScore = score("18_Gross")
    '    End If
    '    iLast5Scores.Add(iScore)
    '    If iLast5Scores.Count = 5 Then Exit For
    '    ''dont recalculate handicap for 4/11, scorebook was lost
    '    'If sScoreDate = "20170411" Then
    '    '    iPHdcp = GetHdcp(iLast5Scores, iRoundctr, score("Date").ToString)
    '    'End If
    'Next

    'GetLast5 = iLast5Scores
    '    Catch ex As Exception
    'MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Function
    'Sub setLast5()
    '    Dim i = 0
    '    For Each R As DataGridViewRow In dgScores.Rows
    '        ohelper.sPlayer = R.Cells(Player).Value
    '        ohelper.getLast5()
    '        Dim ii = 0
    '        For Each score In ohelper.iLast5Scores
    '            If ii = ohelper.iLast5Scores.Count - 1 Then
    '                R.Cells("Last 5 Scores").Value &= $"{score}"
    '            Else
    '                R.Cells("Last 5 Scores").Value &= $"{score}-"
    '            End If
    '            ii += 1
    '        Next
    '        i += 1
    '    Next

    'End Sub
    'Public Sub LoadDGV(tm As TeamMatch)
    '    ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

    '    Try
    '        Dim dtPoints = New DataTable("Points")
    '        dtPoints.Columns.Add("SortOrder")
    '        dtPoints.Columns.Add("Team")
    '        dtPoints.Columns.Add("APlayer")
    '        dtPoints.Columns.Add("BPlayer")
    '        dtPoints.Columns.Add("Points", GetType(Decimal))
    '        dtPoints.Columns.Add("PtsBack")
    '        dtPoints.PrimaryKey = New DataColumn() {dtPoints.Columns(Constants.Team)}
    '        Dim et As TimeSpan
    '        Dim sst As DateTime = Now
    '        For Each ml As matchforlist In tm.MatchList
    '            ohelper.wk = ""
    '        Next

    '        'https://stackoverflow.com/questions/8705312/datagridview-setting-row-height-doesnt-work
    '        dgTeams.RowTemplate.Height = 15
    '        With dgTeams
    '            .RowHeadersVisible = False
    '            .Visible = True
    '            .Columns.Clear()
    '            .AllowUserToAddRows = False
    '            .AllowUserToDeleteRows = False
    '            .AutoGenerateColumns = True
    '            .ReadOnly = True
    '            .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8, FontStyle.Bold)
    '            .DefaultCellStyle.Font = New Font("Tahoma", 9)
    '            .DataSource = dtPoints
    '            .Columns("SortOrder").Visible = False
    '            .Columns("APlayer").Width = 120
    '            .Columns("BPlayer").Width = 120
    '            .Columns("Team").Width = 50
    '            .Columns("Points").Width = 50
    '            .Columns("PtsBack").Width = 55
    '            Dim x = .RowTemplate.Height
    '            '.RowTemplate.Height = 17
    '            .Width += 10
    '            x = .Height
    '            .Height += 5
    '            x = .Rows.Count
    '            x = .Rows(0).Height
    '            '.Height += (.Rows.Count * .Rows(0).Height)
    '            '.Sort(dgTeams.Columns("Points"), System.ComponentModel.ListSortDirection.Descending)
    '        End With
    '        dgTeams.RowTemplate.Height = 5

    '        For Each row As DataGridViewRow In dgTeams.Rows
    '            Dim x = row.Cells("PtsBack").Value
    '            Dim xx = lbRem.Text
    '            If IsNumeric(lbRem.Text) Then
    '                If IsNumeric(row.Cells("PtsBack").Value) Then
    '                    If row.Cells("PtsBack").Value > lbRem.Text * 3 Then
    '                        row.Cells("PtsBack").Style.BackColor = Color.Yellow
    '                    End If
    '                End If
    '            End If
    '        Next
    '    Catch ex As Exception
    '        MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Sub
    'Sub recreateTextBoxesasCheckBoxes(sColName As String, iCol As Int16)
    '    Try
    '        'If TypeOf dgScores.CurrentCell Is DataGridViewCheckBoxCell Then Exit Sub
    '        'recreate columns as checkboxes
    '        Dim ncol As New DataGridViewCheckBoxColumn
    '        ncol.HeaderText = sColName
    '        ncol.Name = ncol.HeaderText
    '        ncol.DataPropertyName = ncol.HeaderText
    '        ncol.Width = 40

    '        'ncol.ValueType = GetType(Boolean)
    '        ohelper.LOGIT($"removing Col {iCol} {dgScores.Columns(iCol).HeaderText}")
    '        dgScores.Columns.RemoveAt(iCol)
    '        ohelper.LOGIT($"adding Col {iCol} {ncol.Name}")
    '        dgScores.Columns.Insert(iCol, ncol)

    '    Catch ex As Exception
    '        MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Sub
    'Sub recreateCheckBoxesasTextBoxes(sColName As String, iCol As Int16)
    '    Try

    '        'If TypeOf dgScores.CurrentCell Is DataGridViewTextBoxCell Then Exit Sub
    '        'recreate columns as checkboxes
    '        Dim ncol As New DataGridViewTextBoxColumn
    '        ncol.HeaderText = sColName
    '        ncol.Name = ncol.HeaderText
    '        ncol.DataPropertyName = ncol.HeaderText
    '        ncol.Width = 40
    '        dgScores.Columns.RemoveAt(iCol)
    '        dgScores.Columns.Insert(iCol, ncol)
    '    Catch ex As Exception
    '        MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '    End Try
    'End Sub

    '#Region "Not Used"
    '    Function NewmakeCellAmt(cell As Object) As Integer
    '        ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '        NewmakeCellAmt = 0
    '        Try
    '            If cell.Value IsNot DBNull.Value Then
    '                If Not String.IsNullOrEmpty(cell.Value) Then
    '                    If IsNumeric(cell.Value) Then
    '                        NewmakeCellAmt = CInt(cell.Value)
    '                    End If
    '                End If
    '            End If
    '        Catch ex As Exception
    '            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '        End Try
    '    End Function
    '    Function makeCellAmt(cell As DataGridViewCell) As Integer
    '        makeCellAmt = 0

    '        If cell.Value IsNot DBNull.Value Then
    '            If Not String.IsNullOrEmpty(cell.Value) Then
    '                If IsNumeric(cell.Value) Then
    '                    makeCellAmt = CInt(cell.Value)
    '                End If
    '            End If

    '        End If
    '    End Function
    '    'Not in use
    '    Sub calcpurse()
    '        Dim iplys As Int16 = 0
    '        If cbDates.SelectedItem >= spsDate Then
    '            iplys = lEOYPlayers.Count
    '        Else
    '            For Each ply As DataGridViewRow In dgScores.Rows
    '                ohelper.LOGIT($"{ply.Cells("Player").Value}-{ply.Cells("Paid").Value}")
    '                If CStr(ply.Cells("Paid").Value) <> "" Then
    '                    iplys += 1
    '                End If
    '            Next
    '        End If

    '        If iplys > 0 Then
    '            For i = 1 To ohelper.iNumClosests
    '                dgScores.Columns($"CPW{i}").Visible = True
    '            Next
    '        Else
    '            For i = 1 To ohelper.iNumClosests
    '                dgScores.Columns($"CPW{i}").Visible = False
    '            Next
    '        End If
    'MovetoOldrouties2:

    '        SendKeys.Send("+{TAB}")

    '    End Sub
    '    Sub MarkCellsForSkins(e As DataGridViewCellPaintingEventArgs)
    '        If bshowpaint Then
    '            ohelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, e.RowIndex, e.ColumnIndex, dgScores.Columns(e.ColumnIndex).Name))
    '        End If
    '        Try
    '            Dim rectangle As New Rectangle(e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 4, e.CellBounds.Height - 4)
    '            Dim gridBrush As Brush = New SolidBrush(dgScores.GridColor)
    '            Dim backColorBrush As Brush = New SolidBrush(e.CellStyle.BackColor)
    '            For Each srow In lLowRows
    '                If srow > e.RowIndex Then Exit For
    '                If e.RowIndex = srow Then
    '                    If lLowRows.Count > 1 Then
    '                        Dim myFont As New Font("Tahoma", 9, FontStyle.Strikeout)
    '                        e.CellStyle.Font = myFont
    '                        gridBrush = New SolidBrush(dgScores.GridColor)
    '                        backColorBrush = New SolidBrush(Color.Yellow)
    '                    Else
    '                        gridBrush = New SolidBrush(dgScores.GridColor)
    '                        backColorBrush = New SolidBrush(Color.Gold)
    '                    End If
    '                End If
    '                e.Graphics.FillRectangle(backColorBrush, e.CellBounds)

    '            Next
    '        Catch ex As Exception
    '            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '        End Try

    '    End Sub
    '    Sub ColorForSkins(dgc As DataGridViewCell)
    '        ohelper.LOGIT(String.Format("Entering {0} row({1})-Column({2})-Col Name({3}) ", Reflection.MethodBase.GetCurrentMethod.Name, dgc.RowIndex, dgc.ColumnIndex, dgScores.Columns(dgc.ColumnIndex).Name))
    '        Try
    '            For Each srow In lLowRows
    '                If srow > dgc.RowIndex Then Exit For
    '                If dgc.RowIndex = srow Then
    '                    If lLowRows.Count > 1 Then
    '                        dgc.Style.BackColor = Color.Yellow
    '                    Else
    '                        dgc.Style.BackColor = Color.Gold
    '                    End If
    '                End If
    '            Next
    '        Catch ex As Exception
    '            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
    '        End Try

    '    End Sub
    '    Sub calcCellWidth(cell As DataGridViewCell)
    '        ohelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '        Dim textSize As Size = TextRenderer.MeasureText(cell.Value, cell.DataGridView.CurrentCell.Style.Font)
    '        Dim iLargestWidth As Int16 = 0 'dgScores.Columns(cell.OwningColumn.Name).Width
    '        For Each row As DataGridViewRow In dgScores.Rows
    '            Dim xx = TextRenderer.MeasureText(row.Cells(cell.OwningColumn.Name).Value, cell.DataGridView.CurrentCell.Style.Font).Width
    '            If xx > iLargestWidth Then iLargestWidth = xx
    '        Next
    '        'If cell.DataGridView.CurrentCell.Style.Font.Size > 12 Then
    '        If textSize.Width >= iLargestWidth Then
    '            dgScores.Columns(cell.OwningColumn.Name).Width = textSize.Width + 1
    '        Else
    '            dgScores.Columns(cell.OwningColumn.Name).Width = iLargestWidth
    '        End If
    '        'dgScores.CurrentRow.Height = sizef.Height

    '    End Sub
    '    Sub setSkinsCTPs(row As DataRowView, fld As String, irow As Integer)
    '        'oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '        Try

    '            If row(fld) IsNot DBNull.Value Then
    '                If row(fld) = "Y" Then 'Or row(fld).ToString.ToUpper = "TRUE" Then
    '                    dgScores.Rows(irow).Cells(fld).Value = True
    '                    dgScores.Rows(irow).Cells(fld).Style.BackColor = Color.LightBlue
    '                End If
    '            Else
    '                dgScores.Rows(irow).Cells(fld).Value = False
    '                dgScores.Rows(irow).Cells(fld).Style.BackColor = Color.White
    '            End If
    '        Catch ex As Exception
    '            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '        End Try
    '    End Sub

    '    Sub closestswap(dgc As DataGridViewCell)
    '        Dim scallingmethod As String = (New Diagnostics.StackTrace).GetFrame(1).GetMethod.Name
    '        ohelper.LOGIT($"Entering {Reflection.MethodBase.GetCurrentMethod.Name} called by {scallingmethod}")
    '        ohelper.LOGIT($"Old value {dgc.Value}")
    '        If dgc.Value = "True" Then
    '            dgc.Value = False
    '        Else
    '            dgc.Value = True
    '        End If
    '        ohelper.LOGIT($"New value {dgc.Value}")

    '    End Sub

    '#End Region
    '    Private Sub ScoreCard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
    '        Dim x = ""
    '        Exit Sub
    '        If bChanges Then
    '            Dim smsg = New StringBuilder
    '            smsg.AppendLine($"
    'Changes were made to the tables
    'If you quit without saving, all of your changes will be lost
    'Do you want to save your files?

    'Press <Yes> to continue without saving
    'Press <No> to stay on this screen
    '")

    '            '{String.Join(", ", changes)}

    '            If MessageBox.Show(smsg.ToString, "Changes Have been Made to Tables", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
    '                e.Cancel = True
    '            End If

    '        End If
    '    End Sub
#Region "Main"
    '        GoTo skipschbuild
    '        'sched from seperate sch tables
    '        BuildTablesfromCSV("C:\Golf\Files\", "hugh's_schedule")

    '        'onetime build of schedule
    '        'Dim dts = oClosedXML.LoadCsvIntoDataTable($"{csvFilePath}schedule.csv")
    '        'dts.TableName = "Schedule"
    '        'dsLeague.Tables.Add(dts)

    '        'this will build records into dtschedule, problem is they all have diff columns, will comment out
    '        Using workbook As New XLWorkbook($"{csvFilePath}Hugh'sLeague.xlsx")
    '            Dim query As String = "SELECT name FROM sqlite_master WHERE type = 'table' and name like '20%';"
    '            Using command As New SQLiteCommand(query, connection)
    '                Using reader As SQLiteDataReader = command.ExecuteReader()
    '                    While reader.Read()
    '                        Dim dt = oHelper.sqliteda(connection, $"[{reader("name")}]")
    '                        ' Add sheets to the workbook
    '                        workbook.Worksheets.Add(dt, reader("name"))
    '                        'Debug.Print(reader("name"))
    '                        'Console.WriteLine(reader("name"))
    '                    End While
    '                End Using
    '            End Using
    '            workbook.SaveAs($"{csvFilePath}Hugh'sLeague.xlsx")
    '        End Using
    '        'end 1226
    '        'skin payments move to onetime one saved
    '        sb = New StringBuilder
    '        sb.AppendLine($"
    'SELECT * FROM Payments WHERE [Desc] = 'SkinsCTP'
    '"
    '            )
    '        oHelper.dt = oHelper.sqlitedaFromSql(connection, "", sb.ToString)
    '        'oHelper.dt = oHelper.sqliteda(connection, "SkinsCTP")
    '        Dim dtlp As DataTable = oHelper.sqliteda(connection, "LeagueParms")
    '        dtlp.PrimaryKey = New DataColumn() {dtlp.Columns("Season")}
    '        For Each row In oHelper.dt.Rows
    '            Dim dr = dtlp.Rows.Find(row("Date").ToString.Substring(0, 4))
    '            oHelper.sPSDate = $"{CDate(dr("PostSeasonDT")):yyyyMMdd}"
    '            oHelper.dt = oHelper.sqlitedaFromSql(connection, "", $"SELECT * FROM Payments ORDER BY ID DESC LIMIT 1 ")
    '            Dim sID = oHelper.dt.Rows(0)(0)
    '            Dim scols = ""
    '            sID += 1
    '            For i = 0 To oHelper.dt.Columns.Count - 1
    '                Dim col As DataColumn = oHelper.dt.Columns(i)
    '                scols &= If(i = oHelper.dt.Columns.Count - 1, $"[{col.ColumnName}]", $"[{col.ColumnName}],")
    '            Next

    '            Using cmd As New SQLiteCommand(connection)
    '                ' Prepare the SQL command
    '                cmd.CommandText = $"INSERT INTO Payments ({scols})
    'VALUES ({sID },
    ''{oHelper.sLeagueName}',
    ''{row(Constants.Player)}',
    '{row("Date")},
    ''Skin',
    ''Payment',
    '{If(row("Date") >= oHelper.sPSDate, dr("SkinsPS"), dr(Constants.skin))},
    '{row("Date")},
    ''',
    '''
    ')
    '"
    '                'oHelper.LOGIT(cmd.CommandText)
    '                ' Execute the command
    '                cmd.ExecuteNonQuery()
    '            End Using
    '            Using cmd As New SQLiteCommand(connection)
    '                ' Prepare the SQL command
    '                cmd.CommandText = $"INSERT INTO Payments ({scols}) 
    'VALUES (
    '{sID + 1 },
    ''{oHelper.sLeagueName}',
    ''{row(Constants.Player)}',
    '{row("Date")},
    ''CTP',
    ''Payment',
    '{If(row("Date") >= oHelper.sPSDate, dr("ClosestPS"), dr(Constants.closest))},
    '{row("Date")},
    ''',
    '''
    ')
    '"
    '                'oHelper.LOGIT(cmd.CommandText)
    '                ' Execute the command
    '                cmd.ExecuteNonQuery()

    '            End Using
    '            Using cmd As New SQLiteCommand(connection)
    '                ' Prepare the SQL command
    '                cmd.CommandText = $"DELETE FROM Payments 
    'WHERE League = '{oHelper.sLeagueName}' 
    'AND Player = '{row(Constants.Player)}' 
    'AND Date = {row("Date")} 
    'AND [Desc] = 'SkinsCTP' 
    'AND Detail = 'Payment'
    '"
    '                'oHelper.LOGIT(cmd.CommandText)
    '                ' Execute the command
    '                cmd.ExecuteNonQuery()
    '            End Using
    '            oHelper.dt = oHelper.sqliteda(connection, "Payments")
    '            oHelper.wk = ""
    '        Next

    '        oHelper.dt = oHelper.sqliteda(connection, "Payments")
    '        Using workbook As New XLWorkbook($"{csvFilePath}Hugh'sLeague.xlsx")
    '            workbook.Worksheets.Delete("Payments")
    '            workbook.Worksheets.Add(oHelper.dt, "Payments")
    '            workbook.SaveAs($"{csvFilePath}Hugh'sLeague.xlsx")
    '        End Using
    '        oHelper.dt = oHelper.sqlitedaFromSql(connection, "Payments", "select * from payments where desc in ('Skin','CTP')")
    '        GoTo skipschbuild
    Public Sub BuildTablesfromCSV(filepattern As String, wildcard As String)
        Dim oHelper As New Helper
        Dim oFiles() As IO.FileInfo
        Dim oDirectory As New IO.DirectoryInfo(filepattern)

        oFiles = oDirectory.GetFiles($"*{wildcard}.csv")
        Helper.arraySort(oFiles)
        Helper.dsLeague = New DataSet
        For Each file In oFiles
            If Not file.Name.StartsWith("20") Then Continue For
            If file.Name.StartsWith("2024") Then Continue For
            Dim dtname As String = file.Name.Replace("Hugh's_", "").Replace(".csv", "")
            Dim dt As New DataTable(dtname)
            oHelper.SQLiteCreateTable(Main.connection, file.FullName)
            'For Each col As DataColumn In dt.Columns 'oHelper.dsLeague.Tables("Teams").Columns
            '    Debug.Print($"Main {dt.TableName}-{col.ColumnName}-{col.DataType}")
            'Next
            dt = oHelper.sqliteda(Main.connection, $"[{dt.TableName}]")

            Helper.dsLeague.Tables.Add(dt)
            'If dt.TableName = "LeagueParms" Then
            '    Dim dv = New DataView(dt)
            '    dv.RowFilter = "startdate Like '%2024%'"
            '    Helper.rLeagueParmrow = dv(0)
            'End If

            'oHelper.SQLite(connection, file.Name)
            'Debug.Print("")
            'tables.Add(dt)
            'oHelper.SQLite(connection, file.Name)
            'dtScores = oHelper.sqliteda(connection, "Scores", "Select * from Scores")
        Next

        ' SQL query to list all tables
        'oHelper.showtables(connection)
        'Dim query As String = "SELECT name FROM sqlite_master WHERE type = 'table';"
        'Using command As New SQLiteCommand(query, connection)
        '    Using reader As SQLiteDataReader = command.ExecuteReader()
        '        While reader.Read()
        '            Debug.Print(reader("name"))
        '            Console.WriteLine(reader("name"))
        '        End While
        '    End Using
        'End Using

        'oHelper.SQLite(connection, "LeagueParms.csv")
        'oHelper.SQLite(connection, "Scores.csv")
        'oHelper.SQLite(connection, "Earnings.csv")
        'dtScores = oHelper.sqliteda(connection, "Scores", "Select * from Scores")
        'dtEarnings = oHelper.sqliteda(connection, "Earnings", "Select * from Earnings")
        'Dim dvLP = New DataView(oHelper.sqliteda(connection, "LeagueParms", "Select * from LeagueParms where startdate like '%2024%'"))
        'bldSchedule()
        'Dim sch = New SchGenerator
        'sch.Main()

        'oHelper.BldMatchesDataGridFromCSV()
    End Sub
    'Sub bldSchedule()

    '    Dim dtw = New DataTable
    '    dtw.TableName = "wkSchedule"
    '    dtw.Columns.Add("Date")
    '    For i = 1 To rLeagueParmrow("Teams") / 2
    '        dtw.Columns.Add($"{i}")
    '    Next

    '    dsLeague.Tables.Add(dtw)
    '    Dim sdate As Date = rLeagueParmrow("StartDate")
    '    oHelper.SchGenerator(rLeagueParmrow("Teams"), sdate)
    '    Dim sskipWeeks = (rLeagueParmrow("Teams") - 1) * 7
    '    sdate = sdate.AddDays(sskipWeeks)
    '    oHelper.SchGenerator(rLeagueParmrow("Teams"), sdate)
    '    dsLeague.Tables.Add(oHelper.buildSchedule)

    '    Dim newtable = Helper.PivotTable(dsLeague.Tables("wkSchedule"), 0)
    '    Dim newtable2 = Helper.PivotTable(newtable, 0)
    '    Exit Sub
    '    Dim oClosedXML = New ClosedXML
    '    'create an xlsx from a datatable
    '    oClosedXML.ExportDataTableToExcel(dsLeague.Tables("Schedule"), $"{csvFilePath}{sdate.Year}_{rLeagueParmrow("Name")}_Schedule.xlsx")
    '    'create a csv from an xlsx'
    '    oClosedXML.XLS2CSV($"{csvFilePath}{sdate.Year}_{rLeagueParmrow("Name")}_Schedule.xlsx", $"{csvFilePath}{sdate.Year}_{rLeagueParmrow("Name")}_NewSchedule.csv")
    'End Sub
    'Sub finddups(dt As DataTable, skey As String)
    '    Dim lkeys = New List(Of String)
    '    For Each row In dt.Rows
    '        Dim searchkey As String = ""
    '        For Each fld In skey.Split(",")
    '            searchkey &= $"{row(fld)},"
    '        Next
    '        If lkeys.Contains(searchkey) Then
    '            Debug.Print($"{searchkey}")
    '        End If
    '    Next
    'End Sub
    'Sub UpdateIds(dt As DataTable)
    '    If dt.Columns.Contains("Id") Then Exit Sub
    '    ' Insert a new column
    '    Dim newColumn As New DataColumn("Id", GetType(Integer))
    '    newColumn.DefaultValue = 0
    '    dt.Columns.Add(newColumn)
    '    newColumn.SetOrdinal(0) ' Inserts the new column at the first position (0-based index)


    '    For Each col As DataColumn In dt.Columns

    '    Next
    'End Sub

    ' Main code to show the custom message box
    Public Sub ShowCustomMessageBox(message As String, bold As List(Of String))
        'Dim message As String = "This is a " & "BOLD" & " message."
        Dim form As New CustomMessageBoxForm(message)
        GoTo skipbold
        ' Make the word "BOLD" bold in the RichTextBox
        For Each sbold As String In bold
            For Each smsg As String In message.Split(vbCrLf)
                Dim startIndex As Integer = smsg.IndexOf(sbold)
                If startIndex >= 0 Then
                    form.richTextBox.Select(startIndex, Len(sbold))
                    form.richTextBox.SelectionFont = New Font(form.richTextBox.Font, FontStyle.Bold)
                End If
            Next
        Next
skipbold:
        form.ShowDialog()
    End Sub

    ' CustomMessageBoxForm.vb
    Public Class CustomMessageBoxForm
        Inherits Form

        Public richTextBox As New RichTextBox()
        Private button As New Button()

        Public Sub New(message As String)
            If UBound(message.Split(vbCrLf)) < 1 Then
                MessageBox.Show($"message is expected in this format{vbCrLf}{vbCrLf}first part form heading{vbCrLf}the rest is the message body")
            End If
            Me.Text = message.Split(vbCrLf)(0)
            Me.Size = New Size(500, 200)
            With richTextBox
                .ReadOnly = True
                .BorderStyle = BorderStyle.None
                .Font = New Font("Tahoma", 9, FontStyle.Regular)
                .Text = message
                .Location = New Point(20, 20)
                .Size = New Size(340, 100)

            End With
            Me.Controls.Add(richTextBox)

            button.Text = "OK"
            button.Location = New Point(100, 120)
            AddHandler button.Click, AddressOf Me.Button_Click
            Me.Controls.Add(button)
        End Sub

        Private Sub Button_Click(sender As Object, e As EventArgs)
            Me.Close()
        End Sub
    End Class
#Region "Debug"
    'Using command As New SQLiteCommand($"SELECT * FROM LeagueParms ", connection)
    '    Using reader As SQLiteDataReader = command.ExecuteReader()
    '        ' Display the selected records
    '        While reader.Read()
    '            For i As Integer = 0 To reader.FieldCount - 1
    '                Dim fieldName As String = reader.GetName(i)
    '                Dim fieldValue As Object = reader.GetValue(i)
    '                Dim fieldType As Object = reader.GetFieldType(i)
    '                Debug.Print($"{fieldName}: {fieldValue} : {fieldType}")
    '            Next
    '        End While
    '    End Using
    'End Using

#End Region
#End Region
End Class
