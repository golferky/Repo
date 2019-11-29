Imports System.Data.SqlClient
Imports System.Data.OleDb
Public Class frmScoring
    '   Dim sqlConn As SqlConnection = New SqlConnection("Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\LeagueManager.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True")
    'Dim dMatchDate As Date = Date.Now
    Dim dMatchDate = ""
    Dim sMatch = 0
    Dim sScore = 0
    Dim sFrontBack = "Front"
    Dim TextBoxes As New Dictionary(Of String, TextBox)
    Dim oHelper As Helper
    Dim dvScores As DataView
    Dim dtSchedule As DataTable

    Private Sub Scoring_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        txtLeagueName.Text = Main.oHelper.sLeagueName
        'oHelper.dtLeagueParms = oHelper.CSV2DataTableOLEDB(txtLeagueName.Text & ".csv")
        'oHelper.dtPlayers = oHelper.CSV2DataTableOLEDB("Players.csv")
        'oHelper.dtScores = oHelper.CSV2DataTableOLEDB("Scores.csv")
        ' sqlConn.Open()
        'Dim myCMD As SqlCommand = New SqlCommand("Select * From Schedule", sqlConn)
        ' Dim da As New SqlDataAdapter(myCMD)
        'Dim dt As New DataTable
        'da.Fill(dt)
        'For Each row As DataRow In dt.Rows
        '    cbMatchDate.Items.Add(row("Date"))
        'Next
        'Dim i = 1
        'For Each col As DataColumn In dt.Columns
        '    If col.ColumnName.StartsWith("FrontHome") Or col.ColumnName.StartsWith("BackHome") Then
        '        cbMatchNumber.Items.Add(i)
        '        i += 1
        '    End If

        'Next
        'cbMatchNumber.Enabled = False

        'da.Dispose()
        'populate the match dates
        'just use the first row of the league table
        'Dim rLeagueParmrow As DataRow = oHelper.dtLeagueParms.Rows(0)
        ''get the schedule
        'dtSchedule = oHelper.CSV2DataTableOLEDB(txtLeagueName.Text & "_sch.csv")

        'BuildControls()

    End Sub

    Private Sub cbMatchNumber_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbMatchNumber.SelectedIndexChanged
        'Dim myCMD As SqlCommand = New SqlCommand("Select * From Schedule where Date = '" & dMatchDate & "'", sqlConn)
        'Dim dr As SqlDataReader = myCMD.ExecuteReader
        'dr.Read()
        'sMatch = cbMatchNumber.SelectedItem.ToString
        'Dim sHome = String.Empty
        'Dim sAway = String.Empty
        'If cbMatchNumber.SelectedItem.ToString Then
        '    If sMatch > 5 Then
        '        sFrontBack = "Back"
        '        sHome = dr(sFrontBack & "Home" & sMatch - 5)
        '        sAway = dr(sFrontBack & "Away" & sMatch - 5)
        '    Else
        '        sHome = dr(sFrontBack & "Home" & sMatch)
        '        sAway = dr(sFrontBack & "Away" & sMatch)
        '    End If
        'End If



        'dr.Close()
        'myCMD = New SqlCommand("Select * From Player where TeamNumber = " & sHome, sqlConn)
        'Dim da As New SqlDataAdapter(myCMD)
        'Dim dt As New DataTable
        'da.Fill(dt)
        'For Each row As DataRow In dt.Rows
        '    If row("PlayerType") = "A" Then
        '        txtAHomePlayer.Text = row("Name")
        '    End If
        '    If row("PlayerType") = "B" Then
        '        txtBHomePlayer.Text = row("Name")
        '    End If

        'Next
        'da.Dispose()
        'myCMD = New SqlCommand("Select * From Player where TeamNumber = " & sAway, sqlConn)
        'da = New SqlDataAdapter(myCMD)
        'dt = New DataTable
        'da.Fill(dt)
        'For Each row As DataRow In dt.Rows
        '    If row("PlayerType") = "A" Then
        '        txtAAwayPlayer.Text = row("Name")
        '    End If
        '    If row("PlayerType") = "B" Then
        '        txtBAwayPlayer.Text = row("Name")
        '    End If

        'Next
        'da.Dispose()
        'go build the controls since we have everything
        '1) get schedule of matches for this day selected
        '2) find Scores for each match based on schedule
        '3) Plug a/b players and scores
        Dim sdate As String = cbMatchDate.SelectedItem
        sdate = DateTime.ParseExact(sdate, "yyyyMMdd", Nothing).ToString("MM\/dd\/yy")
        Dim sMatch As String = dtSchedule.Rows(cbMatchNumber.SelectedItem - 1).Item(sdate)
        dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))
        'find the a player for the first side of the match
        Dim dv1Players As New DataView(oHelper.dsLeague.Tables("dtPlayers"))
        dv1Players.RowFilter = "Team = '" & sMatch.Split("v")(0) & "'"
        dv1Players.Sort = "Grade"
        Dim dv2Players As New DataView(oHelper.dsLeague.Tables("dtPlayers"))
        dv2Players.RowFilter = "Team = '" & sMatch.Split("v")(1) & "'"
        dv2Players.Sort = "Grade"
        Dim sTeam1APlayer As String = dv1Players(0).Item("Name")
        Dim sTeam2APlayer As String = dv2Players(0).Item("Name")
        Dim sTeam1BPlayer As String = dv1Players(1).Item("Name")
        Dim sTeam2BPlayer As String = dv2Players(1).Item("Name")
        Dim x = "League = '" & txtLeagueName.Text & "' and Date = " & cbMatchDate.SelectedItem & "' And Player = '" & sTeam1APlayer & "'"
        dvScores.RowFilter = "League = '" & txtLeagueName.Text & "' and Date = '" & cbMatchDate.SelectedItem & "' and Player = '" & sTeam1APlayer & "'"
        AllControls(Me.gbAPlayerScore)

    End Sub

    Private Sub cbMatchDate_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbMatchDate.SelectedIndexChanged
        dMatchDate = cbMatchDate.SelectedItem
        cbMatchNumber.Enabled = True
        'Dim dvScores As New DataView(oHelper.dtScores)
        'dvScores.RowFilter = "date = '" & dMatchDate & "'"
        'cbMatchNumber
    End Sub

    Private Sub ScoreChange(sender As System.Object, e As System.EventArgs)
        Dim stb As TextBox = sender
        If Not IsNumeric(stb.Text) Then
            If stb.Text = String.Empty Then
                Exit Sub
            Else
                MessageBox.Show("Invalid score...re-enter", "Score Invalid", MessageBoxButtons.OK)
                Exit Sub
            End If
        End If
        Dim sHole = stb.Name.Substring(Len(stb.Name) - 1, 1)
        If sFrontBack = "Back" Then
            sHole += 9
        End If
        'Dim myCMD As SqlCommand = New SqlCommand("Select Par" & sHole & " From Course ", sqlConn)
        'Dim dr As SqlDataReader = myCMD.ExecuteReader
        'dr.Read()

        'If dr(0).ToString.Trim > stb.Text Then
        '    stb.BackColor = Color.Red
        'ElseIf dr(0).ToString.Trim < stb.Text Then
        '    stb.BackColor = Color.Black
        '    stb.ForeColor = Color.White
        'Else
        '    stb.BackColor = Color.White
        'End If

        'dr.Close()
        sender = stb
    End Sub

    Private Sub UpdateScoring()

        '    Dim sPlayerName = String.Empty, sCourse = String.Empty, sHoles() As String = Nothing
        '    Dim i = 0
        '    ReDim Preserve sHoles(8)
        '    For Each ctl As Control In Me.gbAPlayerMatch.Controls
        '        If TypeOf ctl Is TextBox Then
        '            If ctl.Name.Contains("AHomePlayerHole") Then
        '                sHoles(i) = ctl.Text
        '                i += 1
        '            End If

        '        End If
        '    Next
        '    Dim myCMD As SqlCommand = New SqlCommand("Select Coursename From Course ", sqlConn)
        '    Dim dr As SqlDataReader = myCMD.ExecuteReader
        '    dr.Read()

        '    sCourse = dr(0).ToString
        '    dr.Close()

        '    Dim sUSql = "Insert into Scoring Values (" &
        '           txtAHomePlayer.Text & "," &
        '           dMatchDate & "," &
        '           sCourse & "," &
        '           sFrontBack & "," &
        '           sHoles(0) & "," & sHoles(1) & "," & sHoles(2) & "," & sHoles(3) & "," & sHoles(4) & "," & sHoles(5) & "," & sHoles(6) & "," & sHoles(7) & "," & sHoles(8) &
        '                    ")"
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        UpdateScoring()
    End Sub
    Sub BuildScores(ByRef ctl As Control)

        Dim i = 0
        If ctl.Name.Contains("Score") Then
            'if this contains scores, chang
            If ctl.Size.Width = 17 Then

            End If
        End If
        Dim stbNamePrefix = ctl.Name.Substring(2, 1)
        For Each aCtl In ctl.Controls

            If aCtl.name.ToString.StartsWith("Text") Then
                aCtl.Name = i
                i += 1
            End If
            oHelper.LOGIT(aCtl.Name & " " & i)
            If aCtl.HasChildren Then

                Call AllControls(aCtl)

            End If

        Next

    End Sub
    Sub AllControls(ByRef ctl As Control)

        Dim i = 1
        Dim stbNamePrefix = ctl.Name.Substring(2, 1)
        For Each aCtl In ctl.Controls
            If aCtl.name.ToString.StartsWith("Text") Then
                If ctl.Name.Contains("Score") Then
                    'if this contains scores, change the textbox to put it there
                    If aCtl.Size.Width = 17 Then
                        aCtl.text = dvScores(0).Item("Hole" & i)
                        aCtl.name = stbNamePrefix & i
                    Else
                        aCtl.text = dvScores(0).Item("Player")
                        aCtl.name = stbNamePrefix & "Player"
                    End If
                End If
                i += 1
            End If
            oHelper.LOGIT(aCtl.Name & " " & i)
            If aCtl.HasChildren Then

                Call AllControls(aCtl)

            End If

        Next

    End Sub
    Private Sub BuildControls()
        Dim gb As New GroupBox
        Dim lb As New Label
        Dim tb As New TextBox
        'Player
        lb.Size = New Drawing.Size(80, 20)
        lb.Text = "Player"
        tb.Size = New Drawing.Size(80, 20)
        gb.Controls.Add(lb)
        gb.Controls.Add(tb)

        'Build Home A Player Boxes
        For i As Integer = 0 To 8
            Dim B As New TextBox
            Me.gbAplayerMatch.Controls.Add(B)
            B.Height = 21
            B.Width = 21
            B.Left = 137 + (i Mod 10) * 22
            B.Top = 73 + (i \ 10) * 22
            'B.Text = Chr((i \ 10) + Asc("H")) & i Mod 10 + 1
            B.Name = "AHHole" & i + 1
            TextBoxes.Add(B.Name, B)
            AddHandler B.TextChanged, AddressOf TextBox_Changed
        Next
        'Build A Player Match Boxes
        For i As Integer = 0 To 8
            Dim B As New Label
            Me.gbAplayerMatch.Controls.Add(B)
            B.Height = 21
            B.Width = 21
            B.Left = 137 + (i Mod 10) * 22
            B.Top = 140 + (i \ 10) * 22
            'B.Text = Chr((i \ 10) + Asc("A")) & i Mod 10 + 1
            B.Name = "AMHole" & i + 1
            B.Enabled = False
            Try
                B.Text = "-"
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        Next
        'Build Away A Player Boxes
        For i As Integer = 0 To 8
            Dim B As New TextBox
            Me.gbAplayerMatch.Controls.Add(B)
            B.Height = 21
            B.Width = 21
            B.Left = 137 + (i Mod 10) * 22
            B.Top = 196 + (i \ 10) * 22
            'B.Text = Chr((i \ 10) + Asc("A")) & i Mod 10 + 1
            B.Name = "AAHole" & i + 1
            Try
                TextBoxes.Add(B.Name, B)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            AddHandler B.TextChanged, AddressOf TextBox_Changed
        Next
    End Sub
    Private Sub TextBox_Changed(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim stb As TextBox = sender

        ScoreChange(sender, e)

        'Dim IsCreated(18) As Boolean
        'Dim B As TextBox = sender
        'IsCreated(CInt(B.Name.Replace("AHHole", String.Empty).Replace("AAHole", String.Empty))) = True
        'B.BackColor = Color.Red
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        'Dim i = 0
        'Dim AHoles(8) As String
        'Dim HHoles(8) As String
        'For Each ctl As Control In Me.gbAplayerMatch.Controls
        '    If TypeOf ctl Is TextBox Then
        '        If ctl.Name.Contains("AHHole") Then

        '            If Not IsNumeric(ctl.Text) Then
        '                MsgBox("Invalid Score in Player Card-Hole " & ctl.Name.Substring(Len(ctl.Name) - 1, 1) & vbCrLf & "Please correct and recalculate", MsgBoxStyle.Critical)
        '                Exit Sub
        '            End If
        '            sScore = sScore + ctl.Text
        '            HHoles(i) = ctl.Text
        '            If ctl.Name.Contains("AHHole9") Then
        '                lblAHScore.Text = sScore
        '                sScore = 0
        '                i = 0
        '            Else
        '                i += 1
        '            End If
        '        End If
        '        If ctl.Name.Contains("AAHole") Then
        '            If Not IsNumeric(ctl.Text) Then
        '                MsgBox("Invalid Score in Opponent Card-Hole " & ctl.Name.Substring(Len(ctl.Name) - 1, 1) & vbCrLf & "Please correct and recalculate", MsgBoxStyle.Critical)
        '                Exit Sub
        '            End If
        '            sScore = sScore + ctl.Text
        '            AHoles(i) = ctl.Text
        '            If ctl.Name.Contains("AAHole9") Then
        '                lblAAScore.Text = sScore
        '                sScore = 0
        '            Else
        '                i += 1
        '            End If
        '        End If

        '    End If
        'Next

        'txtAMatchTot.Text = 0

        'For Each ctl As Control In Me.gbAplayerMatch.Controls
        '    If TypeOf ctl Is Label Then
        '        If ctl.Name.Contains("AMHole") Then
        '            Dim ii = ctl.Name.Substring(Len(ctl.Name) - 1, 1) - 1
        '            If HHoles(ii) = AHoles(ii) Then
        '                ctl.Text = "-"
        '            ElseIf HHoles(ii) < AHoles(ii) Then
        '                ctl.Text = "^"
        '                txtAMatchTot.Text += 1
        '            Else
        '                ctl.Text = "v"
        '                txtAMatchTot.Text -= 1
        '            End If
        '        End If
        '    End If
        'Next
        'If txtAMatchTot.Text > 0 Then
        '    txtAMatchTot.Text = "+" & txtAMatchTot.Text
        'End If

    End Sub

End Class