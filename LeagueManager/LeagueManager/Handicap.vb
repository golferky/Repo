Imports System.IO
Public Class frmHandicap
    Dim strConnString = String.Empty
    Dim rLeagueParmrow As DataRow
    Dim oHelper As Helper.Controls.Helper
    Private Sub frmHandicap_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            oHelper = Main.oHelper
            oHelper.LOGIT(Reflection.MethodBase.GetCurrentMethod().Name & " -------------------------")
            'strConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & oHelper.sFilePath & ";Extended Properties=Text;"
            'Try
            '    oHelper.dtScores = oHelper.CSV2DataTableOLEDB("Scores.csv")
            '    dtScores = oHelper.dtScores
            'Catch ex As Exception
            '    MsgBox(oHelper.sFilePath & "\Scores.csv In use" & vbCrLf & "Close it And try again")
            '    Me.Close()
            '    Exit Sub
            'End Try

            'Try
            '    oHelper.dtCourses = oHelper.CSV2DataTableOLEDB("Courses.csv")
            'Catch
            '    MsgBox(oHelper.sFilePath & "\Courses.csv In use" & vbCrLf & "Close it And try again")
            '    Me.Close()
            '    Exit Sub
            'End Try
            ''debug
            'oHelper.dtLeagueParms = oHelper.CSV2DataTable("t.csv")
            'Dim mydvScores = New DataView(dtScores)
            'mydvScores.RowFilter = "Player = 'Gary Scudder'"
            'oHelper.UpdateHdcp(mydvScores)
            ''end debug
            'If dtScores Is Nothing Then
            '    Try
            '        dtScores = New DataTable
            '        Dim column As DataColumn

            '        column = New DataColumn()
            '        column.DataType = System.Type.GetType("System.String")
            '        column.ColumnName = "Player"
            '        ' Add the column to the DataTable.Columns collection.
            '        dtScores.Columns.Add(column)

            '        column = New DataColumn()
            '        'column.DataType = System.Type.GetType("System.DateTime")
            '        column.DataType = System.Type.GetType("System.String")
            '        column.ColumnName = "Date"
            '        dtScores.Columns.Add(column)

            '        column = New DataColumn()
            '        column.DataType = System.Type.GetType("System.String")
            '        column.ColumnName = "Course"
            '        ' Add the column to the DataTable.Columns collection.
            '        dtScores.Columns.Add(column)

            '        'column = New DataColumn()
            '        'column.DataType = System.Type.GetType("System.String")
            '        'column.ColumnName = "Round"
            '        '' Add the column to the DataTable.Columns collection.
            '        'dtScores.Columns.Add(column)

            '        dtScores.PrimaryKey = New DataColumn() {dtScores.Columns(0), dtScores.Columns(1), dtScores.Columns(2), dtScores.Columns(3)}

            '        dtScores.Columns.Add("Group")

            '        For i As Integer = 1 To oHelper.iHoles
            '            dtScores.Columns.Add(i)
            '        Next
            '        dtScores.Columns.Add("Out")
            '        dtScores.Columns.Add("In")
            '        dtScores.Columns.Add("Total")
            '    Catch ex As Exception
            '        MsgBox(ex.Message)
            '    End Try
            'Else
            oHelper.dsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtScores").Columns("Player"), oHelper.dsLeague.Tables("dtScores").Columns("Date")}
            Dim dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))
            dvScores.Sort = "Player, Date DESC"
            For Each row As DataRowView In dvScores
                If Not cbPlayer.Items.Contains(row("Player")) Then
                    cbPlayer.Items.Add(row("Player"))
                End If
                If Not cbDate.Items.Contains(row("Date")) Then
                    cbDate.Items.Add(row("Date"))
                End If
            Next

            'End If

            cbPlayer.SelectedIndex = 0
            cbDate.SelectedIndex = 0
            btnShowScores.Text = "Show Detailed Scores"
            'gbScoring.Visible = False
        Catch ex As Exception
            MsgBox(ex.Message)
            Exit Sub
        End Try

    End Sub
    Private Sub lv1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim lv1 As SI.Controls.LvSort = sender
        If lv1.FocusedItem Is Nothing Then
            Exit Sub
        End If
        'If lv1.SelectedItems.Count > 1 Then
        '    btnUpdateScoring.Enabled = False
        '    btnDeleteThisScore.Text = "Delete these Scores"
        'Else
        '    btnUpdateScoring.Enabled = True
        '    btnDeleteThisScore.Text = "Delete this Score"
        '    txtPlayer.Text = lv1.FocusedItem.SubItems.Item("Player").Text
        '    txtCourse.Text = lv1.FocusedItem.SubItems.Item("Course").Text
        '    txtDate.Text = lv1.FocusedItem.SubItems.Item("Date").Text
        '    Dim sKeys() As Object = {txtPlayer.Text, txtDate.Text}
        '    oHelper.dtScores.PrimaryKey = New DataColumn() {oHelper.dtScores.Columns("Player"), oHelper.dtScores.Columns("Date")}
        '    Dim drow As DataRow = oHelper.dtScores.Rows.Find(sKeys)
        '    If drow Is Nothing Then
        '    Else
        '        'txtCourse.Text = drow("Course").ToString
        '        txtGroup.Text = drow.Item("Group").ToString
        '    End If
        'End If

    End Sub
    Private Sub lv1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
        frmEnterScore.ShowDialog()
    End Sub
    'Private Sub lv1_MouseClick(ByVal sender As Object, ByVal e As System.EventArgs)
    '    'frmEnterScore.ShowDialog()
    'End Sub
    Private Sub btnShowScores_Click(sender As System.Object, e As System.EventArgs) Handles btnShowScores.Click

        Dim lv1 As New SI.Controls.LvSort
        With lv1
            .Columns.Clear()
            .Columns.Add("Date", 80, HorizontalAlignment.Center)
            .Columns.Add("Scores", 100, HorizontalAlignment.Center)
            .Columns.Add("Out", 40, HorizontalAlignment.Center)
            .Columns.Add("Scores", 100, HorizontalAlignment.Center)
            .Columns.Add("In", 40, HorizontalAlignment.Center)
            .Columns.Add("New Hdcp", 50, HorizontalAlignment.Center)
            .Columns.Add("Net", 40, HorizontalAlignment.Center)
            .Columns.Add("Hdcp", 50, HorizontalAlignment.Center)
            .Top = 200
            .Width = 700
            .Height = 400
            .ListViewItemSorter = Nothing
            .View = View.Details
            .Visible = True
            .Items.Clear()
            .Sorting = SortOrder.None
            .Name = "HandicapScores"
            AddHandler .SelectedIndexChanged, AddressOf lv1_SelectedIndexChanged
            AddHandler .MouseDoubleClick, AddressOf lv1_MouseDoubleClick
            'AddHandler .MouseClick, AddressOf lv1_MouseClick
        End With

        'disable cbcourses.selectedindex changed event 
        'RemoveHandler cbCourses.SelectedIndexChanged, AddressOf cbCourses_SelectedIndexChanged
        If txtLeagueName.Text = Nothing Then
            MsgBox("Please Select a League")
            txtLeagueName.Focus()
            Exit Sub
        End If
        If cbPlayer.SelectedItem = Nothing Then
            If Not cbPlayer.Items.Contains(cbPlayer.Text) Then
                cbPlayer.Items.Add(cbPlayer.Text)
                cbPlayer.SelectedIndex = cbPlayer.Items.Count - 1
            ElseIf cbPlayer.Text = String.Empty Then
                MsgBox("Please Select a Player")
                cbHPlayer.Focus()
                Exit Sub
            Else
                cbPlayer.SelectedIndex = 0
            End If
        End If
        If cbDate.SelectedItem = Nothing Then
            If Not cbDate.Items.Contains(cbDate.Text) Then
                cbDate.Items.Add(cbDate.Text)
                cbDate.SelectedIndex = cbDate.Items.Count - 1
            ElseIf cbDate.Text = String.Empty Then
                MsgBox("Please Select a Date")
                cbDate.Focus()
                Exit Sub
            End If
        End If

        If btnShowScores.Text = "Add Score" Then
            UpdateScore()
        End If

        'Dim lvRec As ListViewItem = Nothing
        Try

            'oHelper.dtScores = oHelper.CSV2DataTableOLEDB("Scores.csv")
            'Dim dvScores = New DataView(dtScores)
            ''dvScores.RowFilter = "Course = '" & cbCourse.SelectedItem.ToString & "' and Name = '" & cbPlayer.SelectedItem.ToString & "'"
            'dvScores.RowFilter = "Player = '" & cbPlayer.SelectedItem.ToString & "'"
            'dvScores.Sort = "Date"
            'oHelper.dtCourses = oHelper.CSV2DataTableOLEDB("Courses.csv")
            'Dim dvCourse = New DataView(oHelper.dtCourses)

            Dim iscorein = 0
            Dim iscoreout = 0
            Dim iscoretot = 0
            Dim dvCourses = New DataView(oHelper.dsLeague.Tables("dtCourses"))
            dvCourses.RowFilter = "Name = '" & rLeagueParmrow.Item("Course") & "'"
            'oHelper.CalcHdcpNoHoleByHole(lv1, dtScores, dvScores, dvCourse)
            'oHelper.CalcHdcp(lv1, edt, dvScores, dvCourse, "Round
            'oHelper.CalcHdcp(cbPlayer.SelectedItem, Date.Now.ToString("yyyyMMdd"), lv1)
            For Each score In oHelper.dsLeague.Tables("dtScores").Rows
                If lv1 IsNot Nothing Then
                    'add items to listview
                    Dim lvRec = New ListViewItem(score("Date").ToString)
                    'lvRec.SubItems.Add(rLeagueParmrow.Item("Course"))
                    Dim sbScores As New System.Text.StringBuilder
                    For i As Integer = 1 To 9
                        sbScores.Append(score("Hole" & i) & " ")
                    Next
                    'Out
                    lvRec.SubItems.Add(sbScores.ToString)
                    If score("Method") = "Net" Then
                        lvRec.SubItems.Add(score("Out_Net").ToString)
                    Else
                        lvRec.SubItems.Add(score("Out_Gross").ToString)
                    End If
                    sbScores = New System.Text.StringBuilder
                    For i As Integer = 10 To 18
                        sbScores.Append(score("Hole" & i) & " ")
                    Next
                    lvRec.SubItems.Add(sbScores.ToString)
                    'In
                    If score("Method") = "Net" Then
                        lvRec.SubItems.Add(score("In_Net").ToString)
                    Else
                        lvRec.SubItems.Add(score("In_Gross").ToString)
                    End If

                    'Previous Handicap
                    lvRec.SubItems.Add(score("Hdcp").ToString)
                    'In
                    If score("Method") = "Net" Then
                        lvRec.SubItems.Add(score("18_Net").ToString)
                    Else
                        lvRec.SubItems.Add(score("18_Gross").ToString)
                    End If
                    'New Handicap
                    lvRec.SubItems.Add(score("PHdcp").ToString)
                    lv1.Items.Add(lvRec)
                End If
            Next

            'End If
            Me.Controls.RemoveByKey("HandicapScores")
            Me.Controls.Add(lv1)

            'Me.Show()

        Catch ex As Exception
            MsgBox(ex.Message & ex.StackTrace)
        End Try
    End Sub

    Private Sub txtScore_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtScore.TextChanged
        btnShowScores.Text = "Add Score"

    End Sub
    Sub UpdateScore()

        'Dim sKeys() As Object = {cbPlayer.SelectedItem.ToString, cbDate.SelectedItem.ToString}
        'Dim drow As DataRow = dtScores.Rows.Find(sKeys)
        'If drow Is Nothing Then
        '    'New Row added
        '    drow = dtScores.Rows.Add(sKeys)
        '    drow("Player") = cbPlayer.SelectedItem.ToString
        '    drow("Date") = cbDate.SelectedItem.ToString
        '    drow("Course") = rLeagueParmrow("Course").ToString
        'Else
        '    If MsgBox("Score : " & cbPlayer.SelectedItem.ToString & " Already exists" & vbCrLf & "Do you want to update it?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
        '        MsgBox("Update Cancelled")
        '        Exit Sub
        '    End If
        'End If

        'drow("Total") = txtScore.Text

        'oHelper.DataTable2CSV(dtScores, oHelper.sFilePath & "\Scores.csv", ",")

        'Button2_Click(sender, e)
    End Sub

    Private Sub btnShowAllPlayers_Click(sender As System.Object, e As System.EventArgs) Handles btnShowAllPlayers.Click
        Dim lvRec As ListViewItem = Nothing
        Dim sScores As String = Nothing

        Dim lv1 As New SI.Controls.LvSort
        lv1.Columns.Clear()
        lv1.Columns.Add("Player", 120, HorizontalAlignment.Center)
        lv1.Columns.Add("Hdcp", 50, HorizontalAlignment.Center)
        lv1.Columns.Add("# Scores", 70, HorizontalAlignment.Center)
        lv1.Columns.Add("Scores", 500, HorizontalAlignment.Left)
        lv1.View = View.Details
        lv1.Visible = True
        lv1.Items.Clear()
        lv1.Sorting = SortOrder.None
        lv1.Name = "HandicapScores"
        Try
            Dim dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))
            'dvScores.RowFilter = "Course = '" & cbCourse.SelectedItem.ToString & "' and Name = '" & cbPlayer.SelectedItem.ToString & "'"
            dvScores.Sort = "Player, Date DESC"
            Dim dvCourse = New DataView(oHelper.dsLeague.Tables("dtCourses"))
            Dim sPrevName = " ", iPrevHdcp = 0, iScores = 1
            For Each score As DataRowView In dvScores
                If score("Player") = sPrevName Then
                    If score("In_Gross") IsNot DBNull.Value Then
                        sScores = sScores & score("In_Gross") & " "
                    ElseIf score("Out_Gross") IsNot DBNull.Value Then
                        sScores = sScores & score("Out_Gross") & " "
                    End If
                    iScores += 1
                    Continue For
                ElseIf sPrevName = " " Then
                    sPrevName = score("Player")
                    iPrevHdcp = score("Hdcp")
                    If score("In_Gross") IsNot DBNull.Value Then
                        sScores = score("In_Gross") & " "
                        Continue For
                    ElseIf score("Out_Gross") IsNot DBNull.Value Then
                        sScores = score("Out_Gross") & " "
                        Continue For
                    End If
                End If

                lvRec = New ListViewItem(sPrevName)
                If oHelper.fGetTeam(sPrevName) = "" Then
                    lvRec.SubItems(0).BackColor = Color.LightGray
                End If
                lvRec.SubItems.Add(iPrevHdcp)
                lvRec.SubItems.Add(iScores)
                lvRec.SubItems.Add(sScores)

                lv1.Items.Add(lvRec)
                sPrevName = score("Player")
                iPrevHdcp = score("Hdcp")
                If score("In_Gross") IsNot DBNull.Value Then
                    sScores = score("In_Gross") & " "
                    Continue For
                ElseIf score("Out_Gross") IsNot DBNull.Value Then
                    sScores = score("Out_Gross") & " "
                    Continue For
                End If
                iScores = 1
            Next
            lvRec = New ListViewItem(sPrevName)
            lvRec.SubItems.Add(iPrevHdcp)
            lvRec.SubItems.Add(iScores)
            lvRec.SubItems.Add(sScores)
            lv1.Items.Add(lvRec)
            Me.Controls.RemoveByKey("HandicapScores")
            'Add the control to the form
            Me.Controls.Add(lv1)
            lv1.Top = 200
            lv1.Width = 750
            lv1.Height = 500

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub txtLeagueName_TextChanged(sender As Object, e As EventArgs) Handles txtLeagueName.TextChanged
        'If File.Exists(oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv") Then
        '    oHelper.dtLeagueParms = oHelper.CSV2DataTableOLEDB("\" & txtLeagueName.Text & ".csv")
        '    rLeagueParmrow = oHelper.dtLeagueParms.Rows(0)
        '    If IsNothing(oHelper.dtLeagueParms) Then
        '        Exit Sub
        '    End If
        '    Dim iHoleMarker = 0
        '    'just use the Column names which have dates of the schedule table
        '    'advance past the front 9 if not front 9
        '    If Not rLeagueParmrow.Item("Start9").ToString = "F" Then
        '        iHoleMarker = 9
        '    End If
        '    oHelper.iHoles = rLeagueParmrow.Item("Holes")
        'End If
    End Sub

    Private Sub cbPlayer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbPlayer.SelectedIndexChanged
        btnShowScores.Text = "Show " & cbPlayer.SelectedItem & " Scores"
    End Sub
End Class