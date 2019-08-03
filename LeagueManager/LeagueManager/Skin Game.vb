Imports System.Data.OleDb
Imports System.Net.Mail
Imports System.IO
'Imports Microsoft.Office.Interop.word
Public Class frmSkins
    Dim dMatchDate As Date = Date.Now
    Dim sMatch = 0
    Dim sFrontBack = "Front"
    Dim TextBoxes As New Dictionary(Of String, TextBox)
    Dim Labels As New Dictionary(Of String, Label)
    Dim sBirdies As Integer = 0, sEagles As Integer = 0, sPars As Integer = 0, sBogies As Integer = 0, sDBogies As Integer = 0
    Dim edt As DataTable = Nothing
    Dim dvScores As DataView
    Dim strConnString As String

    Const cLabelLeft = 238
    Const cLabelTop = 1
    Const cLabelWidth = 20
    Const cMaxHoles = 9
    Const cDashes = "___________________________________________________________________________________________________________________"
    Dim ileft = cLabelLeft
    Dim itop = cLabelTop
    Dim imaxHoles = cMaxHoles
    Dim iSkins = 0
    Dim sAvgScore As New List(Of Decimal)
    Dim oHelper As New Helper
    Dim lv1 As New ListView
    'this accounts for the extra B player if needed
    Dim iextra As Integer
    Dim cRTFGame As RTF_NET
    Dim sRTFTemplate As String, srRTFTemplate As StreamReader

    Public Sub Resize_cb()
        Dim s_Src As SizeF
        Dim g_Src As Graphics
        Dim iMaxW As Long = 0
        For Each cbSearchItem In cbDate.Items
            g_Src = cbDate.CreateGraphics
            s_Src = g_Src.MeasureString(cbSearchItem, cbDate.Font)
            If iMaxW < s_Src.Width Then
                iMaxW = s_Src.Width
            End If
        Next

        Dim iW As Integer = IIf(iMaxW * 1.1 <= 1160, iMaxW * 1.1, 1160)
        iW = IIf(iW < 256, 256, iW)
        cbDate.DropDownWidth = iW
        cbDate.Width = iW

    End Sub
    Sub BuildScoreIntoLV(rec As Integer)

        Dim dvCourse = New DataView(oHelper.dsLeague.Tables("dtCourse"))
        dvCourse.RowFilter = "Name = '" & oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("Course") & "'"
        Dim bHdcp As Boolean = False
        If rbHandicap.Checked Then
            bHdcp = True
        End If
        'sort the scores by the previous handicap to get the pre-round handicap to play off the lowest score
        dvScores.Sort = "PHdcp "
        Dim iLowHdcp = dvScores.Item(0).Item("PHdcp")
        'For i = 0 To dvScores.Count - 1
        '    dvScores.Item(i).Item("PHdcp") -= iLowHdcp
        'Next
        'Sort = "Total"
        Dim oHelperlv As New Helper_LV
        oHelperlv.ListScoresWithHandicap(lv1, dvScores.Item(rec), dvCourse, "Player", bHdcp, iLowHdcp)

    End Sub

    Function CalcLowRound(score As String)
        Return CalcLowRound(score, dvScores.Count - 1, False, Color.Chartreuse, 0)
    End Function
    Function CalcLowRound(score As String, numscores As Integer, Skins As Boolean, color As Color, iGame As Decimal) As Integer
        CalcLowRound = 0

        Dim ilowrow As New List(Of String)
        Dim ilowscore = 99
        ilowrow = New List(Of String)
        For i = 0 To lv1.Items.Count - 1
            If lv1.Items(i).SubItems.Count >= 21 Then
                Dim x = lv1.Items(i).SubItems(21).Text
                Dim xx = lv1.Items(i).SubItems(0).Text
                'find the lv1 item that contains Best Ball Score and use the last column of it for the total
                If lv1.Items(i).SubItems(0).Text.Contains(score) And _
                   lv1.Items(i).SubItems(21).Text <= ilowscore Then
                    If lv1.Items(i).SubItems(21).Text < ilowscore Then
                        ilowrow = New List(Of String)
                    End If
                    ilowscore = lv1.Items(i).SubItems(21).Text
                    ilowrow.Add(i)
                    'ElseIf lv1.Items(i).SubItems(i + 1).Text.Replace(ChrW(&H25CF), "") <= ilowscore Then
                    '    ilowrow.Add(i)
                End If
            End If
        Next

        'If Skins Then
        '    If ilowrow.Count > 1 Then
        '        Exit Function
        '    End If
        '    CalcLowRound += 1
        'End If

        For Each i As Integer In ilowrow
            Dim lvi As ListViewItem = lv1.Items(i)
            lv1.Items(i).UseItemStyleForSubItems = False
            lv1.Items(i).SubItems(lv1.Items(i).SubItems.Count - 1).BackColor = color
            lv1.Items(i).SubItems(lv1.Items(i).SubItems.Count - 1).Font = New System.Drawing.Font("Times New Roman", 10, System.Drawing.FontStyle.Bold)
            lv1.Items(i - 1).SubItems(25).Text = (iGame / ilowrow.Count / 2) '.ToString("$.00")
            lv1.Items(i - 2).SubItems(25).Text = (iGame / ilowrow.Count / 2) '.ToString("$.00")
        Next
        Return CalcLowRound
    End Function

    Function CalcSkins() As List(Of String)
        CalcSkins = New List(Of String)
        'this code put in for the extra b player, put an * by his name (2nd instance) if hes in there twice
        Dim hash As New System.Collections.Hashtable()
        ' Dim sDups As New List(Of String)
        Dim itemI As ListViewItem
        Dim itemKey As String
        For Each itemI In Me.lv1.Items
            If itemI.SubItems(0).Text = "" Or itemI.SubItems(0).Text.Contains("Best Ball") Then
                Continue For
            End If
            itemKey = itemI.SubItems(0).Text
            If Not hash.ContainsKey(itemKey) Then
                hash.Add(itemKey, itemI)
            Else
                'sDups.Add(itemKey)
                itemI.SubItems(0).Text = "*" & itemI.SubItems(0).Text
                Exit For
            End If
        Next
        'this code goes through the listview and highlights the lowest value on each hole and fron 9, back 9 and total
        Dim ilowrow As New List(Of String)
        'adjust for handicap fields in listview
        'loop through each column finding the lowest scores
        'Get low 9's and 18 hole

        For ii = 1 To 23 'lv1.Items(0).SubItems.Count - 1
            Dim ilowscore = 99
            For i = 0 To lv1.Items.Count - 1
                'If lv1.Items(i).SubItems(0).Text.Contains("*") Then
                '    Continue For
                'End If
                Dim x = lv1.Items(i).SubItems(0).Text
                If ii >= 1 And ii <= 9 Or ii >= 11 And ii <= 19 Then
                    Dim iscore = "0"
                    If rbHandicap.Checked And rbSkins.Checked Then
                        'If lv1.Items(i).SubItems("Hole" & ii).Text.ToString.Contains(ChrW(&H25CF)) Then
                        '    iscore = lv1.Items(i).SubItems(ii).Text.ToString.Replace(ChrW(&H25CF), String.Empty) - 1
                        'Else
                        iscore = lv1.Items(i).SubItems(ii).Text.ToString.Replace(ChrW(&H25CF), String.Empty)
                        'End If
                        'Else
                        '    iscore = lv1.Items(i).SubItems(ii).Text.ToString.Replace(ChrW(&H25CF), String.Empty)
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
                Else
                    Continue for
                End If
            Next

            For Each score As Integer In ilowrow
                If ilowrow.Count = 1 Then
                    lv1.Items(score).UseItemStyleForSubItems = False
                    'Dim xxx As ListViewItem = lv1.Items(score)
                    If IsNumeric(lv1.Columns(ii).ToString.Replace("ColumnHeader: Text: ", "")) Then
                        lv1.Items(score).SubItems(ii).BackColor = Color.Magenta
                        CalcSkins.Add(score)
                    Else
                        lv1.Items(score).SubItems(ii).BackColor = Color.Lavender
                    End If

                End If
            Next
        Next

    End Function
    Sub Skins()
        lv1.ListViewItemSorter = Nothing
        lv1.View = View.Details
        lv1.Visible = True
        lv1.Items.Clear()
        lv1.Sorting = SortOrder.None
        lv1.Name = "Best Ball Scores"

        For rec = 0 To dvScores.Count - 1
            'If dvScores(rec)("Skins") = "y" Then
            BuildScoreIntoLV(rec)
            'End If
        Next
        Dim sSkins As List(Of String)
        sSkins = CalcSkins()
        sSkins.Sort()
        Dim iSkinVal As Decimal = 0
        If sSkins.Count > 0 Then
            iSkinVal = dvScores.Count * (txtAnte.Text * 1.0) / sSkins.Count
            Dim iprevplayer = 99, iTot = 0.0
            Dim bfirst = True
            For Each iplayer As Integer In sSkins
                If iplayer = iprevplayer Then
                    iTot += iSkinVal
                Else
                    If bfirst Then
                        bfirst = False
                    Else
                        lv1.Items(iprevplayer).SubItems(24).Text = iTot '.ToString("$.00")
                    End If

                    iTot = iSkinVal
                    iprevplayer = iplayer
                End If
            Next
            lv1.Items(iprevplayer).SubItems(24).Text = iTot '.ToString("$.00")
        End If

        Dim iTotalPot As Decimal = dvScores.Count * txtAnte.Text
        Dim iTotalPotSkins As Decimal = iSkinVal * sSkins.Count
        Dim lvrec2 = New ListViewItem("Totals")
        For i = 1 To lv1.Items(0).SubItems.Count - 3
            lvrec2.SubItems.Add(" ")
        Next
        lvrec2.SubItems.Add(iTotalPotSkins)
        lv1.Items.Add(lvrec2)
        lv1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
    End Sub
    Public Function BuildPartners(sType As String, iTotPlayers As Integer) As String
        'sPlayers is a list of player names which are indexed in order for a/b game 
        BuildPartners = ""
        Dim iTotAPlayers As Integer = iTotPlayers / 2
        Dim iTotBPlayers As Integer = iTotPlayers - iTotAPlayers

        Dim sAllPlayers As New List(Of String), sAPlayers As New List(Of String), sBPlayers As New List(Of String)
        'Dim Rndm As New Random(System.DateTime.Now.Millisecond)
        Dim J As Integer, AL As New List(Of String)
        'create a string list of A players based on lowest half being A players
        'AL is the list of all players
        For I = 0 To iTotPlayers - 1
            AL.Add(I.ToString)
        Next
        'pull info from pick partner screen
        If sType = "PP" Then
            frmPartners.cbAplayers.Items.Clear()
            frmPartners.cbBPlayers.Items.Clear()
            For Each player In AL
                frmPartners.cbAplayers.Items.Add(dvScores.Item(player).Item("Name").ToString)
                frmPartners.cbBPlayers.Items.Add(dvScores.Item(player).Item("Name").ToString)
            Next
            frmPartners.ListView3.Columns.Add("A Player", 200)
            frmPartners.ListView3.Columns.Add("B Player", 200)
            frmPartners.cbAplayers.SelectedIndex = 0
            frmPartners.cbBPlayers.SelectedIndex = -1
            frmPartners.ShowDialog()

            If frmPartners.ListView3.Items.Count < iTotPlayers / 2 Then
                BuildPartners = ""
                Exit Function
            End If
            For Each player As ListViewItem In frmPartners.ListView3.Items
                For i = 0 To iTotPlayers - 1
                    If dvScores.Item(i).Row("Name") = player.SubItems(0).Text Then
                        sAPlayers.Add(i)
                    End If
                    If dvScores.Item(i).Row("Name") = player.SubItems(1).Text Then
                        sBPlayers.Add(i)
                    End If
                Next
            Next
        ElseIf sType = "D" Then
            Dim dvscores2 = dvScores

            For i = 0 To dvScores.Count / 2 - 1
                For ii = 0 To dvscores2.Count
                    If dvScores.Item(i).Row.Item("Partner") = dvscores2.Item(ii).Row("Name") Then
                        sBPlayers.Add(ii)
                        sAPlayers.Add(i)
                        Exit For
                    End If
                Next
            Next
            Dim x = String.Empty
        ElseIf sType = "AB" Then
            'loop and create an A players list from the lowest half
            For i = 0 To iTotAPlayers - 1
                If i < iTotAPlayers Then
                    sAPlayers.Add(i)
                    AL.Remove(i)
                End If
            Next
            'create a random extra b player if uneven
            If iTotAPlayers <> iTotBPlayers Then
                J = GetRandomNumber(1, AL.Count)
                sBPlayers.Add(AL(J) & " ")
                iextra = AL(J)
            End If

            'make B players random
            For I = 0 To AL.Count - 1
                J = GetRandomNumber(0, AL.Count - 1)
                sBPlayers.Add(AL(J) & " ")
                AL.RemoveAt(J)
            Next
            'ElseIf sType = "PS" Then
            '    frmSelectPlayers.ShowDialog()
            '    If frmSelectPlayers.clbPlayers.Items.Count > 0 Then
            '        For Each player In frmSelectPlayers.clbPlayers.Items

            '        Next
            '    End If
        Else
            For i = 0 To iTotAPlayers - 1
                J = GetRandomNumber(0, AL.Count - 1)
                sAPlayers.Add(AL(J))
                AL.RemoveAt(J)
            Next

            'create a random extra b player if uneven
            If iTotAPlayers <> iTotBPlayers Then
                J = GetRandomNumber(1, AL.Count)
                sBPlayers.Add(AL(J) & " ")
                iextra = AL(J)
            End If

            'make B players random
            For I = 0 To AL.Count - 1
                J = GetRandomNumber(0, AL.Count - 1)
                sBPlayers.Add(AL(J) & " ")
                AL.RemoveAt(J)
            Next
        End If

        For Each player In sAPlayers
            BuildPartners = BuildPartners & player & ";"
        Next
        BuildPartners = BuildPartners & "/"
        For Each player In sBPlayers
            BuildPartners = BuildPartners & player & ";"
        Next
    End Function
    'The trick is to declare the new Random class outside of the function that retrieves the next random number. This way you generate the seed only once and are getting the “randomizer” formula to cycle through its formula and ensure the next chosen number is truly random.
    'Here’s my code. Note that you no longer have to declare new objects (such as objRandom, here) at the top of your class or module; you can do it just above the function, to aid clarity of code:
    Dim objRandom As New System.Random( _
  CType(System.DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))

    Public Function GetRandomNumber( _
      Optional ByVal Low As Integer = 1, _
      Optional ByVal High As Integer = 100) As Integer
        ' Returns a random number,
        ' between the optional Low and High parameters
        Return objRandom.Next(Low, High + 1)
    End Function
    Sub BestBall()
        iextra = 99
        Dim sAllPlayers As New List(Of String), sAPlayers As New List(Of String), sBPlayers As New List(Of String)

        'sort the scores by low total for each player to get the lowest players first
        dvScores.Sort = "Total"

        Dim iTotPlayers As Integer = dvScores.Count
        Dim iTotAPlayers As Integer = dvScores.Count / 2
        Dim iTotBPlayers As Integer = dvScores.Count - iTotAPlayers
        Dim sPlayers As String = ""
        If Not dvScores.Item(0).Row.Item("Partner") Is DBNull.Value Then
            Dim result = MessageBox.Show("Players have already been chosen...want to use them?", "Game", MessageBoxButtons.YesNo)
            If result = Windows.Forms.DialogResult.Yes Then
                sPlayers = BuildPartners("D", iTotAPlayers)
            End If
        End If
        If sPlayers = String.Empty Then
            If rbpp.Checked Then
                sPlayers = BuildPartners("PP", iTotPlayers)
            ElseIf rbAB.Checked Then
                sPlayers = BuildPartners("AB", iTotPlayers)
            ElseIf rbPreSel.Checked Then
                sPlayers = BuildPartners("PS", iTotPlayers)
            Else
                sPlayers = BuildPartners(" ", iTotPlayers)
            End If
        End If
       
        If sPlayers = "" Then
            MsgBox("No Players Selected...try again")
            Exit Sub
        End If
        Dim charSeparators() As Char = {";"c}
        sAPlayers = New List(Of String)(sPlayers.Split("/")(0).Split(charSeparators, StringSplitOptions.RemoveEmptyEntries))
        sBPlayers = New List(Of String)(sPlayers.Split("/")(1).Split(charSeparators, StringSplitOptions.RemoveEmptyEntries))
        lv1.View = View.Details
        lv1.Visible = True
        lv1.Items.Clear()
        lv1.Name = "Best Ball Scores"

        Dim irow = 0

        Dim lvi2 As ListViewItem

        'build the listview for each player
        For Each rec As Integer In sAPlayers

            'build the A players score into the listview
            BuildScoreIntoLV(rec)
            'build the B players score into the listview
            BuildScoreIntoLV(sBPlayers(irow))

            ' Dim sInScore = 0, sOutScore = 0, sTotScore = 0
            Dim lvRec = New ListViewItem("----- Best Ball Score".PadRight(30))
            Dim lvi As ListViewItem = lv1.FindItemWithText(dvScores.Item(rec).Row("Name"))
            dvScores.Item(rec).Row("Partner") = dvScores.Item(sBPlayers(irow)).Row.Item("Name")
            Dim i9Score = 0, iTotScore = 0
            For i = 1 To 23
                Dim iaScore As Integer = lvi.SubItems(i).Text().ToString.Replace(ChrW(&H25CF), "")
                'if ChrW(&H25CF) is in the score, its a handicap hole
                If lvi.SubItems(i).Text.Contains(ChrW(&H25CF) & ChrW(&H25CF)) Then
                    iaScore -= 1
                End If
                If lvi.SubItems(i).Text.Contains(ChrW(&H25CF)) And rbHandicap.Checked Then
                    iaScore -= 1
                End If
                lvi2 = lv1.FindItemWithText(dvScores.Item(sBPlayers(irow)).Row("Name"))
                Dim ibScore As Integer = lvi2.SubItems(i).Text().ToString.Replace(ChrW(&H25CF), "")

                If lvi2.SubItems(i).Text.Contains(ChrW(&H25CF) & ChrW(&H25CF)) Then
                    ibScore -= 1
                End If
                'if ChrW(&H25CF) is in the score, its a handicap hole
                If lvi2.SubItems(i).Text.Contains(ChrW(&H25CF)) And rbHandicap.Checked Then
                    ibScore -= 1
                End If
                If i = 10 Then
                    lvRec.SubItems.Add(i9Score)
                    iTotScore = iTotScore + i9Score
                    i9Score = 0
                Else
                    If i = 20 Then
                        iTotScore = iTotScore + i9Score
                        Exit For
                    End If
                    If iaScore <= ibScore Then
                        i9Score = i9Score + iaScore
                        lvRec.SubItems.Add(iaScore)
                    Else
                        i9Score = i9Score + ibScore
                        lvRec.SubItems.Add(ibScore)
                    End If
                End If

            Next
          
            lvRec.SubItems.Add(i9Score)
            lvRec.SubItems.Add(iTotScore)
            lv1.Items.Add(lvRec)
            lv1.Items.Add("")
            irow += 1
        Next
        If rbPreSel.Checked Then
            Exit Sub
        End If
        Dim sSkins As List(Of String)
        sSkins = CalcSkins()
        sSkins.Sort()
        Dim iSkinVal As Decimal = 0
        If sSkins.Count > 0 Then
            iSkinVal = Math.Round(iTotPlayers * (txtAnte.Text * 0.5) / sSkins.Count, 0)
        End If

        Dim iGame As Decimal = iTotPlayers * txtAnte.Text - (iSkinVal * sSkins.Count)
        Dim iprevplayer = 99, iTot = 0
        Dim bfirst = True
        For Each iplayer As Integer In sSkins

            If iplayer = iprevplayer Then
                iTot += iSkinVal
            Else
                If bfirst Then
                    bfirst = False
                Else
                    lv1.Items(iprevplayer).SubItems(24).Text = iTot '.ToString("$.00")
                    For i = 0 To dvScores.Count
                        Dim sPlayer = lv1.Items(iprevplayer).SubItems(0).Text.Substring(0, lv1.Items(iprevplayer).SubItems(0).Text.IndexOf("("))
                        Dim x As Integer = lv1.Items(iprevplayer).SubItems(0).Text.IndexOf("(")
                        If dvScores.Item(i).Row("Name") = lv1.Items(iprevplayer).SubItems(0).Text.Substring(0, lv1.Items(iprevplayer).SubItems(0).Text.IndexOf("(") - 1) Then
                            dvScores.Item(i).Row("$Skins") = iTot '.ToString("$.00")
                            Exit For
                        End If
                    Next

                End If
                iTot = iSkinVal
                iprevplayer = iplayer
            End If
        Next
        If iprevplayer < 99 Then
            lv1.Items(iprevplayer).SubItems(24).Text = iTot '.ToString("$.00")
            For i = 0 To dvScores.Count
                Dim sPlayer = lv1.Items(iprevplayer).SubItems(0).Text.Substring(0, lv1.Items(iprevplayer).SubItems(0).Text.IndexOf("("))
                Dim x As Integer = lv1.Items(iprevplayer).SubItems(0).Text.IndexOf("(")
                If dvScores.Item(i).Row("Name") = lv1.Items(iprevplayer).SubItems(0).Text.Substring(0, lv1.Items(iprevplayer).SubItems(0).Text.IndexOf("(") - 1) Then
                    dvScores.Item(i).Row("$Skins") = iTot '.ToString("$.00")
                    Exit For
                End If
            Next
        End If

        CalcLowRound("Best Ball Score", iTotAPlayers - 1, False, Color.CadetBlue, iGame)

        Dim iTotalPot As Decimal = iTotPlayers * txtAnte.Text
        Dim iTotalPotSkins As Decimal = iSkinVal * sSkins.Count
        Dim lvrec2 = New ListViewItem("Totals")
        For i = 1 To lv1.Items(0).SubItems.Count - 3
            lvrec2.SubItems.Add(" ")
        Next
        lvrec2.SubItems.Add(iTotalPotSkins)
        lvrec2.SubItems.Add(iGame)
        lv1.Items.Add(lvrec2)
        lv1.ListViewItemSorter = Nothing
        lv1.Sorting = SortOrder.None
        lv1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize)
    End Sub
    Private Sub OnError(ByVal sLoc As String, ByVal ex As Exception)
        On Error Resume Next
        MsgBox("Error at Location " & sLoc & vbCrLf & ex.Message & vbCrLf & ex.StackTrace, MsgBoxStyle.OkOnly)

    End Sub
#Region "Event Triggered Routines"
    Private Sub frmSkins_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            oHelper = Main.oHelper
            rbPreSel.Visible = False
            'strConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & oHelper.sFilePath & ";Extended Properties=Text;"
            'edt = oHelper.CSV2DataTableOLEDB("\Scores.csv")
            'If edt Is Nothing Then
            '    MsgBox("No Scores file found, exiting")
            '    Me.Close()
            '    Exit Sub
            'End If
            'If oHelper.dtLeagueParms Is Nothing Then
            '    oHelper.dtLeagueParms = oHelper.CSV2DataTableOLEDB(edt.Rows(0).Item("League") & ".csv")
            'End If
            'dtCourse = oHelper.CSV2DataTableOLEDB("\Courses.csv")
            'dtCourse.PrimaryKey = New DataColumn() {dtCourse.Columns(0)}
            'ileft = 10
            'itop = 20
            'Dim iwidth As Long = 150
            'ileft = oHelper.CreateRadioButton(gbGameType.Controls, itop, ileft, iwidth, True, "Blind Draw")
            'ileft = oHelper.CreateRadioButton(gbGameType.Controls, itop, ileft, iwidth, False, "Draw A/B")
            'itop += 25
            'ileft = 10
            'ileft = oHelper.CreateRadioButton(gbGameType.Controls, itop, ileft, iwidth, False, "Pick Partners")
            'ileft = oHelper.CreateRadioButton(gbGameType.Controls, itop, ileft, iwidth, False, "Skins Only")

            'cbGameType.Items.Add("High/Low")
            'cbGameType.Items.Add("Nassau")
            'cbGameType.Items.Add("Nassau with Skins")
            txtAnte.Text = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0)("Skins")
            Dim dtDates As New DataView(oHelper.dsLeague.Tables("dtScores").DefaultView.ToTable(True, "date"))
            dtDates.Sort = "Date desc"
            'GridView_Search.DataSource = DataView.ToTable(True, "distinctColumn");
            For Each row As DataRowView In dtDates
                cbDate.Items.Add(row("Date"))
            Next

            '' Create a GroupBox and add a TextBox to it. 
            'Dim gbGameType As New GroupBox()
            'Dim lblGameType As New Label()
            'lblGameType.Location = New Point(15, 15)
            'gbGameType.Controls.Add(lblGameType)

            '' Set the Text and Dock properties of the GroupBox.
            'gbGameType.Text = "Skin Game"
            'gbGameType.Dock = DockStyle.Top

            '' Disable the GroupBox (which disables all its child controls)
            'gbGameType.Enabled = True

            '' Add the Groupbox to the form. 
            'Me.Controls.Add(gbGameType)

            cbDate.SelectedIndex = 0
            rbHandicap.Checked = True
            Me.Controls.Add(lv1)
            lv1.Top = 129
            lv1.Left = 15
            lv1.Width = 1100
            lv1.Height = 450
            lv1.Visible = False
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Calculate_Click(sender As System.Object, e As System.EventArgs) Handles btnCalc.Click
        imaxHoles = 18
        If Not IsNumeric(txtAnte.Text) Then
            MsgBox("Enter a Valid Ante")
            Exit Sub
        End If
        lv1.Visible = True
        lv1.Columns.Clear()
        lv1.Columns.Add("Name", 200, HorizontalAlignment.Center)
        For i = 1 To imaxHoles
            lv1.Columns.Add(i, 40, HorizontalAlignment.Center)
            If i = 9 Then
                lv1.Columns.Add("Out", 40, HorizontalAlignment.Center)
            ElseIf imaxHoles > 9 And i = 18 Then
                lv1.Columns.Add("In", 40, HorizontalAlignment.Center)
                lv1.Columns.Add("Tot", 40, HorizontalAlignment.Center)
                lv1.Columns.Add("Hdcp", 50, HorizontalAlignment.Center)
                lv1.Columns.Add("Net", 50, HorizontalAlignment.Center)
                lv1.Columns.Add("Skins", 70, HorizontalAlignment.Center)
                lv1.Columns.Add("", 10, HorizontalAlignment.Center)
                'lv1.Columns.Add("Game", 70, HorizontalAlignment.Center)
            End If
        Next

        'oHelper.CalcHoleMarker(cbDate.SelectedItem)
        Dim iSkinPercentage As String = 0.5
        'pull only scores from the course/date selected
        dvScores = New DataView(edt)
        dvScores.RowFilter = "Date = '" & cbDate.SelectedItem.ToString.Split("-")(0) & "'" & " And Skins = 'y'"
        'determine how many holes played 9/18
        'If Main.oHelper.iHoles > 9 Then
        '    imaxHoles = 18
        'Else
        '    imaxHoles = cMaxHoles
        'End If

        If Not rbSkins.Checked Then
            If dvScores.Count < 4 Then
                MsgBox("Not enough players for best ball")
                Exit Sub
            End If
            BestBall()
        Else
            iSkinPercentage = 1
            Skins()
        End If

        dvScores.Dispose()


    End Sub
    Sub EmailReport()
        Try
            Dim Smtp_Server As New SmtpClient
            Dim e_mail As New MailMessage()
            Smtp_Server.UseDefaultCredentials = False
            Smtp_Server.Credentials = New Net.NetworkCredential("garyrscudder@gmail.com", "5790utube")
            Smtp_Server.Port = 587
            Smtp_Server.EnableSsl = True
            Smtp_Server.Host = "smtp.gmail.com"

            e_mail = New MailMessage()
            e_mail.From = New MailAddress("garyrscudder@gmail.com")
            'James Ruther <jruther@gaic.com>,
            'Dee, Bob <bdee@gaic.com>,
            'Dave Gastright <dgastright@gaic.com>,
            'Jeff Egger <Jeff.Egger@fmr.com>,
            'Bob Kohlman <bogies4@fuse.net>,
            'Greg Witzgall <gwitzgall@gaic.com>,
            'Rice, Gregory" <greg.rice@united.com>
            e_mail.To.Add("garyrscudder@gmail.com")
            e_mail.Subject = "Email Sending"
            e_mail.IsBodyHtml = False
            e_mail.Attachments.Add(New System.Net.Mail.Attachment(oHelper.sFilePath & "20130721 Game-Adjusted(Jeff 14).*"))
            e_mail.Body = "Test Message"
            Smtp_Server.Send(e_mail)
            MsgBox("Mail Sent")

        Catch error_t As Exception
            MsgBox(error_t.ToString)
        End Try
    End Sub
#End Region

    Private Sub btnPrintResults_Click(sender As System.Object, e As System.EventArgs) Handles btnPrintResults.Click

        Dim headers = (From ch In lv1.Columns _
        Let header = DirectCast(ch, ColumnHeader) _
        Select header.Text).ToArray()

        Dim items = (From item In lv1.Items _
              Let lvi = DirectCast(item, ListViewItem) _
              Select (From subitem In lvi.SubItems _
                  Let si = DirectCast(subitem, ListViewItem.ListViewSubItem) _
                  Select si.Text).ToArray()).ToArray()

        Dim table As String = String.Join(vbTab, headers) & Environment.NewLine

        For Each a In items
            table &= String.Join(vbTab, a) & Environment.NewLine
        Next
        table = table.TrimEnd(CChar(Environment.NewLine))
        Clipboard.SetText(table)
        'lv2word()
        Dim x = String.Empty

        'Dim oWord As wford.Application
        'Dim oDoc As word.Document
        'Dim oTable As word.Table
        'Dim oPara1 As word.Paragraph
        'Dim oPara3 As word.Paragraph
        'Dim oRng As word.Range
        'Dim oShape As word.InlineShape
        'Dim oChart As Object
        'Dim Pos As Double

        ''Start Word and open the document template.
        'oWord = CreateObject("Word.Application")
        'oWord.Visible = True
        'oDoc = oWord.Documents.Add

        ''Insert a paragraph at the beginning of the document.
        'oPara1 = oDoc.Content.Paragraphs.Add
        'oPara1.Range.Text = "Weekly Results"
        'oPara1.Range.Font.Bold = True
        'oPara1.Format.SpaceAfter = 24    '24 pt spacing after paragraph.
        'oPara1.Range.InsertParagraphAfter()

        '''Insert a paragraph at the end of the document.
        '''** \endofdoc is a predefined bookmark.
        ''oPara2 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
        ''oPara2.Range.Text = "Heading 2"
        ''oPara2.Format.SpaceAfter = 6
        ''oPara2.Range.InsertParagraphAfter()

        ''Insert another paragraph.
        'oPara3 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
        'oPara3.Range.Text = "Player           1  2  3  4  5  6  7  8  9  Out 10 11 12 13 14 15 16 17 18 Out Total Net"
        'oPara3.Range.Font.Bold = False
        'oPara3.Format.SpaceAfter = 24
        'oPara3.Range.InsertParagraphAfter()

        ''Insert a 3 x 5 table, fill it with data, and make the first row
        ''bold and italic.
        'Dim r As Integer, c As Integer
        'oTable = oDoc.Tables.Add(oDoc.Bookmarks.Item("\endofdoc").Range, 3, 5)
        'oTable.Range.ParagraphFormat.SpaceAfter = 6
        'For r = 1 To lv1.Items.Count
        '    oTable.Rows.Add(lv1.Items(r))

        'Next
        'oTable.Rows.Item(1).Range.Font.Bold = True
        'oTable.Rows.Item(1).Range.Font.Italic = True

        ''Add some text after the table.
        'oTable.Range.InsertParagraphAfter()

        'All done. Close this form.
        'Me.Close()
    End Sub
    'Sub lv2word()
    '    Dim tmp As String = Nothing
    '    Dim i As Integer, j As Integer
    '    Dim itmX As ListViewItem
    '    Dim oApp As word.Application
    '    Dim oDoc As word.Document
    '    oApp = New word.Application
    '    oApp.Visible = True
    '    oDoc = oApp.Documents.Add
    '    oDoc.Activate()
    '    oDoc.Select()
    '    For i = 0 To lv1.Columns.Count - 1
    '        If i = 0 Then
    '            tmp = tmp & lv1.Columns.Item(i).Text.PadRight(30)
    '        Else
    '            tmp = tmp & vbTab & lv1.Columns.Item(i).Text
    '        End If
    '    Next i
    '    tmp = tmp & vbCrLf & "***" & vbCrLf
    '    For i = 0 To lv1.Items.Count - 1
    '        itmX = lv1.Items.Item(i)
    '        For j = 0 To lv1.Columns.Count - 1
    '            If j = 0 Then
    '                tmp = tmp & itmX.Text.PadRight(30)
    '            Else
    '                If j < itmX.SubItems.Count Then
    '                    tmp = tmp & vbTab & itmX.SubItems(j).Text
    '                End If
    '            End If
    '        Next j
    '        tmp = tmp & vbCrLf
    '    Next i
    '    Clipboard.SetText(tmp)
    '    oApp.Selection.Paste() 'paste from the clipboard...
    '    oDoc.SaveAs2(oHelper.sFilePath & "\Scores.doc")
    '    oDoc = Nothing
    '    oApp = Nothing
    'End Sub

    'Sub SampleWordDoc()
    '    Dim oWord As word.Application
    '    Dim oDoc As word.Document
    '    Dim oTable As word.Table
    '    Dim oPara1 As word.Paragraph, oPara2 As word.Paragraph
    '    Dim oPara3 As word.Paragraph, oPara4 As word.Paragraph
    '    Dim oRng As word.Range
    '    Dim oShape As word.InlineShape
    '    Dim oChart As Object
    '    Dim Pos As Double

    '    'Start Word and open the document template.
    '    oWord = CreateObject("Word.Application")
    '    oWord.Visible = True
    '    oDoc = oWord.Documents.Add

    '    'Insert a paragraph at the beginning of the document.
    '    oPara1 = oDoc.Content.Paragraphs.Add
    '    oPara1.Range.Text = "Heading 1"
    '    oPara1.Range.Font.Bold = True
    '    oPara1.Format.SpaceAfter = 24    '24 pt spacing after paragraph.
    '    oPara1.Range.InsertParagraphAfter()

    '    'Insert a paragraph at the end of the document.
    '    '** \endofdoc is a predefined bookmark.
    '    oPara2 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
    '    oPara2.Range.Text = "Heading 2"
    '    oPara2.Format.SpaceAfter = 6
    '    oPara2.Range.InsertParagraphAfter()

    '    'Insert another paragraph.
    '    oPara3 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
    '    oPara3.Range.Text = "This is a sentence of normal text. Now here is a table:"
    '    oPara3.Range.Font.Bold = False
    '    oPara3.Format.SpaceAfter = 24
    '    oPara3.Range.InsertParagraphAfter()

    '    'Insert a 3 x 5 table, fill it with data, and make the first row
    '    'bold and italic.
    '    Dim r As Integer, c As Integer
    '    oTable = oDoc.Tables.Add(oDoc.Bookmarks.Item("\endofdoc").Range, 3, 5)
    '    oTable.Range.ParagraphFormat.SpaceAfter = 6
    '    For r = 1 To 3
    '        For c = 1 To 5
    '            oTable.Cell(r, c).Range.Text = "r" & r & "c" & c
    '        Next
    '    Next
    '    oTable.Rows.Item(1).Range.Font.Bold = True
    '    oTable.Rows.Item(1).Range.Font.Italic = True

    '    'Add some text after the table.
    '    'oTable.Range.InsertParagraphAfter()
    '    oPara4 = oDoc.Content.Paragraphs.Add(oDoc.Bookmarks.Item("\endofdoc").Range)
    '    oPara4.Range.InsertParagraphBefore()
    '    oPara4.Range.Text = "And here's another table:"
    '    oPara4.Format.SpaceAfter = 24
    '    oPara4.Range.InsertParagraphAfter()

    '    'Insert a 5 x 2 table, fill it with data, and change the column widths.
    '    oTable = oDoc.Tables.Add(oDoc.Bookmarks.Item("\endofdoc").Range, 5, 2)
    '    oTable.Range.ParagraphFormat.SpaceAfter = 6
    '    For r = 1 To 5
    '        For c = 1 To 2
    '            oTable.Cell(r, c).Range.Text = "r" & r & "c" & c
    '        Next
    '    Next
    '    oTable.Columns.Item(1).Width = oWord.InchesToPoints(2)   'Change width of columns 1 & 2
    '    oTable.Columns.Item(2).Width = oWord.InchesToPoints(3)

    '    'Keep inserting text. When you get to 7 inches from top of the
    '    'document, insert a hard page break.
    '    Pos = oWord.InchesToPoints(7)
    '    oDoc.Bookmarks.Item("\endofdoc").Range.InsertParagraphAfter()
    '    Do
    '        oRng = oDoc.Bookmarks.Item("\endofdoc").Range
    '        oRng.ParagraphFormat.SpaceAfter = 6
    '        oRng.InsertAfter("A line of text")
    '        oRng.InsertParagraphAfter()
    '    Loop While Pos >= oRng.Information(word.WdInformation.wdVerticalPositionRelativeToPage)
    '    oRng.Collapse(word.WdCollapseDirection.wdCollapseEnd)
    '    oRng.InsertBreak(word.WdBreakType.wdPageBreak)
    '    oRng.Collapse(word.WdCollapseDirection.wdCollapseEnd)
    '    oRng.InsertAfter("We're now on page 2. Here's my chart:")
    '    oRng.InsertParagraphAfter()

    '    'Insert a chart and change the chart.
    '    oShape = oDoc.Bookmarks.Item("\endofdoc").Range.InlineShapes.AddOLEObject( _
    '        ClassType:="MSGraph.Chart.8", FileName _
    '        :="", LinkToFile:=False, DisplayAsIcon:=False)
    '    oChart = oShape.OLEFormat.Object
    '    oChart.charttype = 4 'xlLine = 4
    '    oChart.Application.Update()
    '    oChart.Application.Quit()
    '    'If desired, you can proceed from here using the Microsoft Graph 
    '    'Object model on the oChart object to make additional changes to the
    '    'chart.
    '    oShape.Width = oWord.InchesToPoints(6.25)
    '    oShape.Height = oWord.InchesToPoints(3.57)

    '    'Add text after the chart.
    '    oRng = oDoc.Bookmarks.Item("\endofdoc").Range
    '    oRng.InsertParagraphAfter()
    '    oRng.InsertAfter("THE END.")

    '    'All done. Close this form.
    '    'Me.Close()
    'End Sub
End Class