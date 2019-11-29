Imports System.Globalization
Public Class frmEnterScore
    Dim oHelper = Main.oHelper
    Dim sTotScore As Integer, sInScore As Integer, sOutScore As Integer
    Dim scores(18) As String
    Dim dtCourse As DataTable
    Dim dvCourse As DataView
    'Dim Labels As New Dictionary(Of String, Label)
    Dim lv1 As ListView
    Dim sBirdies As Integer = 0, sEagles As Integer = 0, sPars As Integer = 0, sBogeys As Integer = 0, sDBogeys As Integer = 0

    Private Sub EnterScore_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        BuildScoringControls()
        'Update the textboxes from the listviw in frmchoosescores
        Dim lv1 As ListView = frmHandicap.Controls.Find("lv1", True).OfType(Of ListView).First

        If Not lv1.FocusedItem Is Nothing Then
            Me.gbScoring.Controls.Find("Method", True).OfType(Of MaskedTextBox).First().Text = lv1.FocusedItem.SubItems.Item("Method").Text
        End If

        For i As Integer = 1 To oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString

            Dim B As MaskedTextBox = Me.gbScoring.Controls.Find("Hole" & i, True).OfType(Of MaskedTextBox).First()
            Dim x = B.Name
            AddHandler B.TextChanged, AddressOf TextBox_Changed

            B.TabIndex = i - 1 + 5
            Calc(B)
            If Not lv1.FocusedItem Is Nothing Then
                If i < 10 Then
                    B.Text = lv1.FocusedItem.SubItems.Item("Hole" & i).Text.Replace(ChrW(&H25CF), String.Empty)
                Else
                    B.Text = lv1.FocusedItem.SubItems("Hole" & i).Text.Replace(ChrW(&H25CF), String.Empty)
                End If

            End If

        Next
    End Sub
    Public Sub BuildScoringControls()
        Me.gbScoring.Controls.Clear()
        Dim ileft = 30
        Dim itop = 30
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 150, "", "Player")
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 100, "", "Date")
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 150, "", "Course")
        ileft = 30
        itop = 60
        Dim x = frmChooseScores.txtPlayer.Text
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 150, "Player", frmChooseScores.txtPlayer.Text)
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 100, "Date", frmChooseScores.txtDate.Text)
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 150, "Course", frmChooseScores.txtCourse.Text)
        ileft = 30
        itop = 150
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 100, "", "Scoring Method", "B")
        ileft = 30
        itop = 180
        'Dim xx As String = Me.gbScoring.Controls.Find("Method", True).OfType(Of Label).First().Text
        ileft = oHelper.CreateTextbox(Me.gbScoring.Controls, itop, ileft, 100, "Method", "", "")
        itop = 150
        For i As Integer = 1 To oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString
            ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft, 30, "Hole" & i, i, "B")
            Dim A As Label = Me.gbScoring.Controls.Find("Hole" & i, True).OfType(Of Label).First()
            'MsgBox("lb" & A.TabIndex)
            oHelper.Create1DigitNumericTextbox(Me.gbScoring.Controls, itop + 30, ileft - 30, 30, "Hole" & i, String.Empty)
            'MsgBox(B.Name & " " & B.TabIndex)
            If oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString > 9 Then
                If i = 9 Then
                    ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft + 5, 40, "Out", "Out", "B")
                    oHelper.CreateLabel(Me.gbScoring.Controls, itop + 3 + 30, ileft - 35, 30, "OutScore", String.Empty)
                ElseIf i = 18 Then
                    ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft + 5, 40, "In", " In", "B")
                    oHelper.CreateLabel(Me.gbScoring.Controls, itop + 3 + 30, ileft - 35, 30, "InScore", String.Empty)
                End If
            End If
        Next
        'Create Total 
        ileft = oHelper.CreateLabel(Me.gbScoring.Controls, itop, ileft + 5, 40, "Total", "Total", "B")
        oHelper.CreateLabel(Me.gbScoring.Controls, itop + 3 + 30, ileft - 35, 30, "TotScore", String.Empty)

    End Sub
    Private Sub TextBox_Changed(ByVal sender As Object, ByVal e As System.EventArgs)
        Calc(sender)
    End Sub
    Public Sub Calc(stb As MaskedTextBox)
        Dim bcorrection As Boolean = False

        'if a previous score was entered, subtract the score from totals, decrement appropriate scoring bucket(eagle, birdies, etc)
        Dim sHole As Integer = stb.Name.Replace("Hole", String.Empty) - 1
        If scores(sHole) <> String.Empty Then
            bcorrection = True
            If oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString > 9 Then
                If sHole < 9 Then
                    sOutScore = sOutScore - scores(sHole)
                    Dim lbScore As Label = Me.gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First()
                    lbScore.Text = sOutScore
                Else
                    sInScore = sInScore - scores(sHole)
                    Dim lbScore As Label = Me.gbScoring.Controls.Find("InScore", True).OfType(Of Label).First()
                    lbScore.Text = sOutScore
                End If
            End If
            sTotScore = sTotScore - scores(sHole)
                Dim lbScoreTot As Label = Me.gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First()
            lbScoreTot.Text = sTotScore
#Region "Calc Old Code"

            'dvCourse = New DataView(dtCourse)
            'dvCourse.RowFilter = "Name = '" & frmChooseScores.txtCourse.Text & "'"

        'For Each row As DataRowView In dvCourse
        '    If scores(sHole) <= row(sHole + 2) - 2 Then
        '        sEagles -= 1
        '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbEagles", True).OfType(Of Label).First()
        '        lbScore.Text = sEagles
        '    ElseIf scores(sHole) = row(sHole + 2) - 1 Then
        '        sBirdies -= 1
        '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbBirdies", True).OfType(Of Label).First()
        '        lbScore.Text = sBirdies
        '    ElseIf scores(sHole) = row(sHole + 2) Then
        '        sPars -= 1
        '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbPars", True).OfType(Of Label).First()
        '        lbScore.Text = sPars
        '    ElseIf scores(sHole) = row(sHole + 2) + 1 Then
        '        sBogeys -= 1
        '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbBogeys", True).OfType(Of Label).First()
        '        lbScore.Text = sBogeys
        '    ElseIf scores(sHole) >= row(sHole + 2) + 2 Then
        '        sDBogeys -= 1
        '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbDoubleBogeys", True).OfType(Of Label).First()
        '        lbScore.Text = sDBogeys
        '    End If
        'Next

#End Region
        scores(sHole) = String.Empty
            stb.BackColor = Color.White
        End If

        If stb.Text <> String.Empty Then
            scores(sHole) = stb.Text
            If oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString > 9 Then
                If sHole < 9 Then
                    sOutScore = sOutScore + stb.Text
                    Dim lbScore As Label = Me.gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First()
                    lbScore.Text = sOutScore
                Else
                    sInScore = sInScore + stb.Text
                    Dim lbScore As Label = Me.gbScoring.Controls.Find("InScore", True).OfType(Of Label).First()
                    lbScore.Text = sInScore
                End If
            End If
            sTotScore = sTotScore + stb.Text
            Dim lbScoreTot As Label = Me.gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First()
            lbScoreTot.Text = sTotScore
#Region "Calc Old Code 2"
            'dvCourse = New DataView(dtCourse)
            'dvCourse.RowFilter = "Name = '" & frmChooseScores.txtCourse.Text & "'"

            'For Each row As DataRowView In dvCourse
            '    If scores(sHole) <= row(sHole + 2) - 2 Then
            '        sEagles += 1
            '        stb.BackColor = Color.SlateBlue
            '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbEagles", True).OfType(Of Label).First()
            '        lbScore.Text = sEagles
            '    ElseIf scores(sHole) = row(sHole + 2) - 1 Then
            '        sBirdies += 1
            '        stb.BackColor = Color.SkyBlue
            '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbBirdies", True).OfType(Of Label).First()
            '        lbScore.Text = sBirdies
            '    ElseIf scores(sHole) = row(sHole + 2) Then
            '        sPars += 1
            '        stb.BackColor = Color.White
            '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbPars", True).OfType(Of Label).First()
            '        lbScore.Text = sPars
            '    ElseIf scores(sHole) = row(sHole + 2) + 1 Then
            '        sBogeys += 1
            '        stb.BackColor = Color.BlanchedAlmond
            '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbBogeys", True).OfType(Of Label).First()
            '        lbScore.Text = sBogeys
            '    ElseIf scores(sHole) >= row(sHole + 2) + 2 Then
            '        sDBogeys += 1
            '        stb.BackColor = Color.DarkSalmon
            '        Dim lbScore As Label = Me.gbScoring.Controls.Find("lbDoubleBogeys", True).OfType(Of Label).First()
            '        lbScore.Text = sDBogeys
            '    End If
            'Next
#End Region
        End If

        If Not bcorrection Then
            SendKeys.Send("{TAB}")
        End If

    End Sub
    Private Sub btnUpdatesScoring_Click(sender As System.Object, e As System.EventArgs) Handles btnUpdatesScoring.Click
        'BuildControls()
        Dim lv1 As ListView = frmChooseScores.Controls.Find("lv1", True).OfType(Of ListView).First

        frmChooseScores.txtPlayer.Text = StrConv(frmChooseScores.txtPlayer.Text, VbStrConv.ProperCase)
        Dim sKeys() As Object = {frmChooseScores.txtPlayer.Text, frmChooseScores.txtDate.Text}
        Dim edt As DataTable = oHelper.CSV2DataTable("Scores.csv")
        edt.PrimaryKey = New DataColumn() {edt.Columns("Player"), edt.Columns("Date")}
        Dim drow As DataRow = edt.Rows.Find(sKeys)
        If drow Is Nothing Then
            'New Row added

            'sKeys 
            Dim sNewKeys() As Object = {"t", "", "Net", frmChooseScores.txtPlayer.Text, frmChooseScores.txtDate.Text}
            drow = edt.Rows.Add(sNewKeys)

            drow("Player") = frmChooseScores.txtPlayer.Text
            drow("Date") = frmChooseScores.txtDate.Text
            drow("Group") = frmChooseScores.txtGroup.Text
        Else
            If MsgBox("Score : " & frmChooseScores.txtPlayer.Text & "-" & frmChooseScores.txtDate.Text & " Already exists" & vbCrLf & "Do you want to update it?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                MsgBox("Update Cancelled")
                Exit Sub
            End If
        End If
        Dim lvrec As ListViewItem = Nothing

        Dim ballscores = True
        For i As Integer = 1 To oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString
            Dim tbScore As MaskedTextBox = Me.gbScoring.Controls.Find("Hole" & i, True).OfType(Of MaskedTextBox).First()
            If tbScore.Text = String.Empty Then
                ballscores = False
                MsgBox("Enter a Score for " & tbScore.Name)
                tbScore.Focus()
                Exit Sub
            End If
            drow("Hole" & i) = tbScore.Text
        Next

        If oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString > 9 Then
            drow("Out") = Me.gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First().Text
            drow("In") = Me.gbScoring.Controls.Find("InScore", True).OfType(Of Label).First().Text
        End If
        drow("Total") = Me.gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First().Text
        'Update the CSV File
        oHelper.DataTable2CSV(edt, oHelper.sFilePath & "\Scores.csv", ",")
        'dtCourse = oHelper.CSV2DataTable("\Courses.csv")
        ' Dim dvScores As New DataView(edt), dvCourse As New DataView(dtCourse)
        'add columns to listview
        oHelper.buildscorelv(lv1, "Player|Date")
        MsgBox("Update Successful...returning to Choose Scores Form")
        Me.Close()
    End Sub

    Private Sub btnClear_Click(sender As System.Object, e As System.EventArgs) Handles btnClear.Click
        For i As Integer = 1 To oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString
            Dim B As MaskedTextBox = Me.gbScoring.Controls.Find("Hole" & i, True).OfType(Of MaskedTextBox).First()
            B.Text = String.Empty
        Next
        Dim lbScore As Label = Nothing
        'Dim lbScore As Label = Me.gbScoring.Controls.Find("lbEagles", True).OfType(Of Label).First()
        'lbScore.Text = 0
        'lbScore = Me.gbScoring.Controls.Find("lbBirdies", True).OfType(Of Label).First()
        'lbScore.Text = 0
        'lbScore = Me.gbScoring.Controls.Find("lbPars", True).OfType(Of Label).First()
        'lbScore.Text = 0
        'lbScore = Me.gbScoring.Controls.Find("lbBogeys", True).OfType(Of Label).First()
        'lbScore.Text = 0
        'lbScore = Me.gbScoring.Controls.Find("lbDoubleBogeys", True).OfType(Of Label).First()
        'lbScore.Text = 0

        If oHelper.dtLeagueParms.Rows(0).Item("Holes").ToString > 9 Then
            lbScore = Me.gbScoring.Controls.Find("OutScore", True).OfType(Of Label).First()
            lbScore.Text = 0
            lbScore = Me.gbScoring.Controls.Find("InScore", True).OfType(Of Label).First()
            lbScore.Text = 0
        End If

        lbScore = Me.gbScoring.Controls.Find("TotScore", True).OfType(Of Label).First()
        lbScore.Text = 0

    End Sub
End Class