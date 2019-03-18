Imports System.Data.OleDb

Public Class frmChooseScores
    Dim dMatchDate As Date = Date.Now
    Dim sMatch = 0
    Dim sInScore As Integer = 0, sOutScore As Integer = 0, sTotScore As Integer = 0
    Dim TextBoxes As New Dictionary(Of String, MaskedTextBox)
    Dim Labels As New Dictionary(Of String, Label)
    Dim sBirdies As Integer = 0, sEagles As Integer = 0, sPars As Integer = 0, sBogeys As Integer = 0, sDBogeys As Integer = 0
    Dim strConnString = String.Empty
    Dim dvScores As DataView
    Dim dvCourses As DataView
    Public oHelper As Helper
    Public oHelperLV As Helper_LV

    Dim scores(18) As String
    Dim fromsizeW As Integer, lvsizeW As Integer
    Dim lv1 As New SI.Controls.LvSort

    Private Sub frmChooseScores_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

    End Sub

    Private Sub txtPlayer_TextChanged(sender As Object, e As EventArgs) Handles txtPlayer.TextChanged
        Dim sPlayer = oHelper.fGetPlayer(txtPlayer.Text)
        If sPlayer <> "" Then
            txtPlayer.Text = oHelper.fGetPlayer(txtPlayer.Text)
        End If
    End Sub

    Private Sub Choose_Scores_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            oHelper = Main.oHelper
            btnUpdateScoring.TabStop = False
            btnExit.TabStop = False

            'strConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & oHelper.sFilePath & _
            '                  ";Extended Properties=Text;"
            'Dim edt1 = oHelper.GetCsvData(strConnString, "Scores.csv")
            'oHelper.dtCourses = oHelper.CSV2DataTableOLEDB("Courses.csv")
            'If oHelper.dtCourses Is Nothing Then

            'End If
            ''dtScores = oHelper.dtBuildScores
            'oHelper.dtScores = oHelper.CSV2DataTableOLEDB("Scores.csv")
            'If oHelper.dtScores Is Nothing Then

            'End If

            'BuildControls()
            'gbScoring.Visible = False
            'load the scores into the listview
            lv1.Visible = True
            fromsizeW = Me.Size.Width
            lvsizeW = lv1.Size.Width
            lv1.ListViewItemSorter = Nothing
            lv1.View = View.Details
            lv1.Visible = True
            lv1.Items.Clear()
            lv1.Sorting = SortOrder.None

            Dim lvRec As ListViewItem = Nothing
            lv1.Columns.Add("Player", 120, HorizontalAlignment.Left)
            lv1.Columns.Add("Course", 100, HorizontalAlignment.Left)
            lv1.Columns.Add("Date", 75, HorizontalAlignment.Left)
            lv1.Columns.Add("Method", 50, HorizontalAlignment.Left)
            lv1.Columns.Add("Team", 50, HorizontalAlignment.Left)
            lv1.Columns.Add("Skins", 50, HorizontalAlignment.Left)

            'lvFiles.Columns.Add("File Type", 80, HorizontalAlignment.Left) 
            For i = 1 To 9
                lv1.Columns.Add(i.ToString, 30, HorizontalAlignment.Left)
            Next

            lv1.Columns.Add("Out", 40, HorizontalAlignment.Left)
            For i = 10 To 18
                lv1.Columns.Add(i.ToString, 30, HorizontalAlignment.Left)
            Next
            lv1.Columns.Add("In", 40, HorizontalAlignment.Left)
            lv1.Columns.Add("Tot", 40, HorizontalAlignment.Left)
            lv1.Columns.Add("Hdcp", 50, HorizontalAlignment.Left)
            lv1.Columns.Add("Net", 40, HorizontalAlignment.Left)
            Try
                Me.Controls.Add(lv1)

                lv1.Top = 200
                lv1.Left = 35
                lv1.Width = 1100
                lv1.Height = 350
                lv1.FullRowSelect = True
                lv1.GridLines = True
                lv1.MultiSelect = True
                lv1.Name = "lv1"
                lv1.LabelEdit = True
                AddHandler lv1.SelectedIndexChanged, AddressOf lv1_SelectedIndexChanged
                AddHandler lv1.MouseDoubleClick, AddressOf lv1_MouseDoubleClick
                'AddHandler lv1.
                Dim sFirstField = "Player|Date"
                'testing 
                'dvScores.RowFilter = "Date > 20130609 and Name = '" & cbPlayer.SelectedItem.ToString & "'"
                oHelperLV.BuildScoreLV(lv1, sFirstField)
                For Each col In lv1.Columns
                    Dim x = ""
                    If IsNumeric(col.text) Then
                        col.width = 31
                    End If

                Next

                'oHelper.CalcHdcp(lv1, edt, dvScores, dvCourse, "Name|Date")

            Catch
            End Try

        Catch ex As Exception
            MsgBox(ex.Message)
            Exit Sub
        End Try
    End Sub
    Private Sub Exit_Click(sender As System.Object, e As System.EventArgs) Handles btnExit.Click
        Close()
    End Sub

    'Private Sub btnShowHdcp_Click(sender As System.Object, e As System.EventArgs)
    '    'disable cbcourses.selectedindex changed event 
    '    'RemoveHandler cbCourses.SelectedIndexChanged, AddressOf cbCourses_SelectedIndexChanged
    '    If txtPlayer.Text = "" Then
    '        MsgBox("Please Select a Player")
    '        txtPlayer.Focus()
    '        Exit Sub
    '    End If
    '    If txtCourse.Text = "" Then
    '        MsgBox("Please Select a Course")
    '        txtCourse.Focus()
    '        Exit Sub
    '    End If
    '    If txtDate.Text = "" Then
    '        MsgBox("Please Select a Date")
    '        txtDate.Focus()
    '        Exit Sub
    '    End If
    '    If txtGroup.Text = "" Then
    '        MsgBox("Please Select a Group")
    '        txtGroup.Focus()
    '        Exit Sub
    '    End If

    '    'lvwColumnSorter = New ListViewColumnSorter()
    '    lv1.Visible = True
    '    'Dim soHold As Windows.Forms.SortOrder = lvwColumnSorter.Order
    '    'Dim iHldOrder As Integer = lvwColumnSorter.SortColumn
    '    fromsizeW = Me.Size.Width
    '    lvsizeW = lv1.Size.Width
    '    lv1.ListViewItemSorter = Nothing
    '    lv1.View = View.Details
    '    lv1.Visible = True
    '    lv1.Items.Clear()
    '    lv1.Sorting = SortOrder.None

    '    Dim lvRec As ListViewItem = Nothing

    '    Try
    '        Dim dvScores = New DataView(edt), sFirstField = "Name|Date"
    '        dvScores.Sort = "Date"
    '        If txtPlayer.Text.ToUpper <> "ALL" Then
    '            dvScores.RowFilter = "Name = '" & txtPlayer.Text & "'"
    '        Else
    '            dvScores.RowFilter = "Date = '" & txtDate.Text & "'"
    '            sFirstField = "Name"
    '        End If

    '        Dim dvCourse = New DataView(dtCourse)

    '        'testing 
    '        'dvScores.RowFilter = "Date > 20130609 and Name = '" & cbPlayer.SelectedItem.ToString & "'"
    '        'lv1.Columns.Clear()
    '        oHelper.CalcHdcp(lv1, edt, dvScores, dvCourse, sFirstField)

    '        lv1.Top = 200
    '        lv1.Left = 35
    '        lv1.Width = 1100
    '        lv1.Height = 350
    '        lv1.FullRowSelect = True
    '        lv1.GridLines = True
    '        lv1.Name = "lv1"
    '    Catch
    '    End Try
    'End Sub
    Private Sub btnDeleteThisScore_Click(sender As System.Object, e As System.EventArgs) Handles btnDeleteThisScore.Click
        If lv1.FocusedItem Is Nothing Then
            MsgBox("No Score Selected to Delete...try again")
            Exit Sub
        End If

        Dim sDelMessage = ""

        For Each item In lv1.SelectedItems
            sDelMessage = sDelMessage + item.text & vbCrLf
        Next

        Dim sKeys() As Object = {lv1.FocusedItem.SubItems("Player").Text, lv1.FocusedItem.SubItems("Date").Text}
        Dim drow As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
        If drow Is Nothing Then
            MsgBox("Score : " & txtPlayer.Text & "-" & txtDate.Text & " Not Found" & vbCrLf & "Delete Cancelled")
            Exit Sub
        End If

        If lv1.SelectedItems.Count = 1 Then
            sDelMessage = "Are you sure you want to delete this score Score : " & txtPlayer.Text & "-" & txtDate.Text & "?"
        Else

            sDelMessage = "Are you sure you want to delete these Scores : " & txtPlayer.Text & "-" & txtDate.Text & "?"
        End If

        If MessageBox.Show(sDelMessage, "Delete Score", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
            oHelper.dsLeague.Tables("dtScores").Rows.Remove(drow)
            lv1.FocusedItem.Remove()
        End If

        oHelper.DataTable2CSV(oHelper.dsLeague.Tables("dtScores"), oHelper.sFilePath & "\Scores.csv", ",")

    End Sub

    Private Sub lv1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim lv1 As SI.Controls.LvSort = sender
        If lv1.FocusedItem Is Nothing Then
            Exit Sub
        End If
        If lv1.SelectedItems.Count > 1 Then
            btnUpdateScoring.Enabled = False
            btnDeleteThisScore.Text = "Delete these Scores"
        Else
            btnUpdateScoring.Enabled = True
            btnDeleteThisScore.Text = "Delete this Score"
            txtPlayer.Text = lv1.FocusedItem.SubItems.Item("Player").Text
            txtCourse.Text = lv1.FocusedItem.SubItems.Item("Course").Text
            txtDate.Text = lv1.FocusedItem.SubItems.Item("Date").Text
            Dim sKeys() As Object = {txtPlayer.Text, txtDate.Text}
            oHelper.dsLeague.Tables("dtScores").PrimaryKey = New DataColumn() {oHelper.dsLeague.Tables("dtScores").Columns("Player"), oHelper.dsLeague.Tables("dtScores").Columns("Date")}
            Dim drow As DataRow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKeys)
            If drow Is Nothing Then
            Else
                'txtCourse.Text = drow("Course").ToString
                txtGroup.Text = drow.Item("Group").ToString
            End If
        End If

    End Sub
    Private Sub lv1_MouseDoubleClick(ByVal sender As Object, ByVal e As System.EventArgs)
        frmEnterScore.ShowDialog()
    End Sub

    Private Sub btnUpdateScoring_Click(sender As System.Object, e As System.EventArgs) Handles btnUpdateScoring.Click
        If txtPlayer.Text = "" Then
            MsgBox("Please Select a Player")
            txtPlayer.Focus()
            Exit Sub
        End If
        If txtCourse.Text = "" Then
            MsgBox("Please Select a Course")
            txtCourse.Focus()
            Exit Sub
        End If
        If txtDate.Text = "" Then
            MsgBox("Please Select a Date")
            txtDate.Focus()
            Exit Sub
        End If
        If txtGroup.Text = "" Then
            MsgBox("Please Select a Group")
            txtGroup.Focus()
            Exit Sub
        End If

        'txtDate.Text = Now.ToString("yyyyMMdd")
        frmEnterScore.ShowDialog()
        'Rebuild the datatable
        'Dim dtScores = oHelper.dsLeague.Tables("dtScores")
        'dtScores = oHelper.CSV2DataTableOLEDB("\Scores.csv")
        'dtScores.PrimaryKey = New DataColumn() {dtScores.Columns("Player"), dtScores.Columns("Date")}
        lv1.Refresh()
    End Sub

End Class