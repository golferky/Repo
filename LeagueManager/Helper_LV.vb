Public Class Helper_LV
    Dim oHelper As New Helper

    Sub ListScoresWithHandicap(ByVal ListView1 As ListView, Score As DataRowView, dvCourse As DataView, ByVal sfirstField As String, ByVal bHdcp As Boolean, iLowHdcp As Integer)

        Dim lvRec As ListViewItem = Nothing
        Try
            'iScore is gross score
            Dim iPlayerHdcp = 0
            Dim iScore = 0, iRounds = 0, iPar = 0

            'Player Name 
            Dim sPlayer = (Score(sfirstField).ToString & " (" & Score("Phdcp").ToString & ")").PadRight(30)
            lvRec = New ListViewItem(sPlayer)
            'front 9
            For i = 1 To 9
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("Hole" & i)))
            Next
            'back 9
            If bHdcp Then
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("Out_Net")))
            Else
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("Out_Gross")))
            End If
            For i = 10 To 18
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("Hole" & i)))
            Next
            If bHdcp Then
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("In_Net")))
            Else
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("In_Gross")))
            End If
            'Total
            If bHdcp Then
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("18_Net")))
            Else
                lvRec.SubItems.Add(ohelper.convDBNulltoSpaces(Score("18_Gross")))
            End If

            oHelper.sCourse = oHelper.dsLeague.Tables("dtLeagueParms").Rows(0).Item("Course").ToString

            dvCourse.RowFilter = "Name = '" & oHelper.sCourse & "'"
            If dvCourse.Count < 1 Then
                MessageBox.Show(oHelper.dsLeague.Tables("dtLeagueParms").Rows(0).Item("Course").ToString & "'" & vbCrLf & "Course not found in course table...please add it with course button ")
                Exit Sub
            End If

            'Handicap
            lvRec.SubItems.Add(Score("PHdcp"))
            'Net total
            If Score("18_Net") IsNot DBNull.Value Then
                lvRec.SubItems.Add(Score("18_Net") - Score("PHdcp"))
            End If
            'for skins
            lvRec.SubItems.Add("")
            'for Partner Game
            lvRec.SubItems.Add("")
            Dim bHdcpGr18 = False
            If Score("PHdcp") > 18 Then
                bHdcpGr18 = True
                Score("PHdcp") = Score("PHdcp") - 18
            End If
            'If bHdcp Then
            '    For i = 0 To 8
            '        If bHdcpGr18 Then
            '            lvRec.SubItems.Item(i + 1).Text = lvRec.SubItems.Item(i + 1).Text & ChrW(&H25CF)
            '        End If
            '        '18+2 Points past par
            '        If dvCourse.Item(0).Row(i + 18 + 2) <= Score("PHdcp") - iLowHdcp Then
            '            lvRec.SubItems.Item(i + 1).Text = lvRec.SubItems.Item(i + 1).Text & ChrW(&H25CF)
            '        End If
            '    Next
            '    For i = 10 To 18
            '        If bHdcpGr18 Then
            '            lvRec.SubItems.Item(i + 1).Text = lvRec.SubItems.Item(i + 1).Text & ChrW(&H25CF)
            '        End If
            '        If dvCourse.Item(0).Row(i - 1 + 18 + 2) <= Score("PHdcp") - iLowHdcp Then
            '            lvRec.SubItems.Item(i + 1).Text = lvRec.SubItems.Item(i + 1).Text & ChrW(&H25CF)
            '        End If
            '    Next
            'End If

            ListView1.Items.Add(lvRec)

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Function AdjustScore(row As DataRowView, ihptr As Integer) As Integer
        AdjustScore = row("Hole" & ihptr)
        If row("Method") = "Net" Then
            Dim ish As Integer = oHelper.CalcStrokeIndex(ihptr)
            If row("Phdcp") >= ish Then
                AdjustScore += 1
                If row("Phdcp") - 9 >= ish Then AdjustScore += 1
            End If
        End If

    End Function
    Sub ListScores(ByVal ListView1 As ListView, Score As DataRowView, ByVal sfirstField As String, ByVal bHdcp As Boolean, iLowHdcp As Integer, iHoleMarker As String)
        oHelper = Main.oHelper
        Dim lvRec As ListViewItem = Nothing
        Try
            'iScore is gross score
            Dim iPlayerHdcp = 0
            Dim iScore = 0, iRounds = 0, iPar = 0

            'Player Name 
            lvRec = New ListViewItem(Score(sfirstField).ToString)

            oHelper.LOGIT("Holemarker" & iHoleMarker)

            For i = 1 To 18
                Dim iadjScore As Integer = 0
                'fill the front 9 with spaces if were only 9 holes and on the back
                If iHoleMarker > 9 Then
                    'back 9
                    If oHelper.iHoles = 9 Then
                        '9 holes only adjust pointer of score 
                        If i <= 10 Then
                            'add a blank column for out_gross
                            lvRec.SubItems.Add("")
                            If i = 10 Then
                                lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(AdjustScore(Score, i)))
                            End If
                        Else
                            lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(AdjustScore(Score, i)))
                        End If
                    Else
                        lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(AdjustScore(Score, i)))
                    End If
                    'front 9
                ElseIf oHelper.iHoles = 9 Then
                    'were on the front 9
                    If i <= 9 Then
                        lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(AdjustScore(Score, i)))
                    ElseIf i = 10 Then
                        If Score.Item("Out_Gross" & i) IsNot DBNull.Value Then
                            lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("Out_Gross")))
                        Else
                            lvRec.SubItems.Add("")
                        End If
                        lvRec.SubItems.Add("")
                    Else
                        lvRec.SubItems.Add("")
                    End If
                Else
                    lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(AdjustScore(Score, i - (iHoleMarker - 1))))
                End If
                'If i = 9 Then
                '    If oHelper.iHoles = 18 Then
                '        lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("Out")))
                '    End If
                'End If
            Next

            If iHoleMarker = 1 Then
                lvRec.SubItems.Add("")
            Else
            End If

            'Total              
            If oHelper.iHoles > 9 Then
                lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("18_Gross")))
            Else
                If iHoleMarker = 10 Then lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("In_Gross")))
                lvRec.SubItems.Add("")
            End If

            'Handicap
            lvRec.SubItems.Add(Score("PHdcp"))
            'Net total

            If Score("18_Gross") = "" Then
                If iHoleMarker = 1 Then
                    lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("Out_Net")))
                Else
                    lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("In_Net")))
                End If
            Else
                lvRec.SubItems.Add(oHelper.convDBNulltoSpaces(Score("18_Gross")))
            End If
            'for skins
            lvRec.SubItems.Add("")
            'for Partner Game
            lvRec.SubItems.Add("")
            If bHdcp Then
                For i = CInt(iHoleMarker) To CInt(iHoleMarker) - 1 + 9
                    Try
                        If oHelper.thisCourse("H" & i) <= Score("PHdcp") - iLowHdcp Then
                            lvRec.SubItems.Item(i).BackColor = Color.BlanchedAlmond
                        End If
                    Catch ex As Exception

                    End Try
                Next

                'For i = 10 To oHelper.iHoles
                '    If oHelper.iHoles > 9 Then
                '        If oHelper.thisCourse("H" & i) <= Score("PHdcp") - iLowHdcp Then
                '            lvRec.SubItems.Item(i).BackColor = Color.BlanchedAlmond
                '        End If
                '    End If
                'Next
            End If

            ListView1.Items.Add(lvRec)

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
    Sub BuildScoreLV(ByVal ListView1 As SI.Controls.LvSort, ByVal sfirstField As String)

        ListView1.Items.Clear()
        Dim lvRec As ListViewItem = Nothing

        Dim dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))
        'Dim dvCourse = New DataView(oHelper.dsLeague.Tables("dtCourses"))

        Try
            'iScore is gross score
            Dim iPlayerHdcp = 0, iPrevHdcp = 0
            Dim iScore = 0, iRounds = 0, iPar = 0
            Dim sPrevPlayer As String = String.Empty
            Dim dtlp = oHelper.dsLeague.Tables("dtLeagueParms")
            dvScores.Sort = "Player, Date"
            For Each score As DataRowView In dvScores
                iPlayerHdcp = score("Hdcp")
                'dtLeagueParms = CSV2DataTable(score("League") & ".csv")
                'dvCourse.RowFilter = "Name = '" & dtlp.Rows(0).Item("Course").ToString & "'"
                'If dvCourse.Count < 1 Then
                '    MessageBox.Show(score("Course") & "Course not found in course table...please add it with course button ")
                '    Exit Sub
                'End If

                Dim subItem As New ListViewItem.ListViewSubItem

                'Player Name or date or both
                If sfirstField.Contains("|") Then
                    'Player
                    lvRec = New ListViewItem(score(sfirstField.Split("|")(0)).ToString) With
                    {
                        .UseItemStyleForSubItems = False
                    }
                    'lvRec = New ListViewItem
                    lvRec.SubItems(0).Name = sfirstField.Split("|")(0).ToString
                    lvRec.SubItems("Player").Text = score(sfirstField.Split("|")(0)).ToString

                    'Course
                    subItem = New ListViewItem.ListViewSubItem With
                    {
                        .Name = "Course",
                        .Text = dtlp.Rows(0).Item("Course").ToString
                    }
                    lvRec.SubItems.Add(subItem)
                    'Date
                    subItem = New ListViewItem.ListViewSubItem
                    subItem.Name = sfirstField.Split("|")(1).ToString
                    subItem.Text = score(sfirstField.Split("|")(1)).ToString
                    lvRec.SubItems.Add(subItem)
                Else
                    lvRec = New ListViewItem(score(sfirstField).ToString)
                End If
                If sPrevPlayer <> score(sfirstField.Split("|")(0)) Then
                    sPrevPlayer = score(sfirstField.Split("|")(0))
                    iScore = 0
                    iRounds = 0
                    iPar = 0
                End If
                'Method
                subItem = New ListViewItem.ListViewSubItem
                subItem.Name = "Method"
                subItem.Text = score("Method").ToString
                lvRec.SubItems.Add(subItem)
                'Team
                subItem = New ListViewItem.ListViewSubItem
                subItem.Name = "Team"
                subItem.Text = score("Team").ToString
                lvRec.SubItems.Add(subItem)
                'Skins
                subItem = New ListViewItem.ListViewSubItem
                subItem.Name = "Skins"
                subItem.Text = score("Skins").ToString
                lvRec.SubItems.Add(subItem) '
                'calculate hole by hole score
                For i As Integer = 1 To dtlp.Rows(0).Item("Holes").ToString
                    iPar += oHelper.thisCourse.Item("Hole" & i)
                    subItem = New ListViewItem.ListViewSubItem
                    subItem.Name = "Hole" & i
                    subItem.Text = score("Hole" & i)
                    lvRec.SubItems.Add(subItem)

                    If score("Hole" & i) < oHelper.thisCourse.Item("Hole" & i) Then
                        lvRec.SubItems.Item("Hole" & i).ForeColor = Color.Red
                        lvRec.SubItems.Item("Hole" & i).Font = New Font("Arial", 10, FontStyle.Bold)
                    ElseIf score("Hole" & i) < oHelper.thisCourse.Item("Hole" & i) - 1 Then
                        lvRec.SubItems.Item("Hole" & i).ForeColor = Color.DarkRed
                        lvRec.SubItems.Item("Hole" & i).Font = New Font("Arial", 15, FontStyle.Bold)
                    End If

                    'check stroke index
                    Dim isi = 0

                    If dtlp.Rows(0).Item("Holes") = 9 And Not oHelper.thisCourse.Item("H" & i + oHelper.iHoleMarker) Mod 2 Then
                        isi = Math.Round((oHelper.thisCourse.Item("H" & i + oHelper.iHoleMarker) + 1) / 2, 0)
                    Else
                        isi = oHelper.thisCourse.Item("H" & i + oHelper.iHoleMarker)
                    End If
                    If isi <= iPlayerHdcp Then
                        If score("Method") = "Net" Then
                            If (iPlayerHdcp - 9) >= isi Then
                                lvRec.SubItems.Item("Hole" & i).Text = score("Hole" & i) + 2
                                'lvRec.SubItems.Item("Hole" & i).BackColor = Color.Beige
                            Else
                                lvRec.SubItems.Item("Hole" & i).Text = score("Hole" & i) + 1
                                'lvRec.SubItems.Item("Hole" & i).BackColor = Color.BlanchedAlmond
                            End If
                        End If
                    Else
                        lvRec.SubItems.Item("Hole" & i).Text = score("Hole" & i)
                    End If

                    'check par against hole max
                    iScore = oHelper.ChkForMax(lvRec.SubItems.Item("Hole" & i).Text, i)

                Next

                'If 9 Then holes only, fill In the other 9 holes With blanks
                If dtlp.Rows(0).Item("Holes").ToString = 9 Then
                    'Out total
                    subItem = New ListViewItem.ListViewSubItem
                    subItem.Name = "Out"
                    subItem.Text = " "
                    lvRec.SubItems.Add(" ")

                    For i = 10 To 18
                        subItem = New ListViewItem.ListViewSubItem
                        subItem.Name = "Hole" & i
                        subItem.Text = " "
                        lvRec.SubItems.Add(subItem)
                    Next

                    'In total
                    subItem = New ListViewItem.ListViewSubItem
                    subItem.Name = "In"
                    subItem.Text = " "
                    lvRec.SubItems.Add(" ")
                End If

                iRounds += 1

                Dim sScores As New List(Of String)
                Dim ilowscore = 99, irow = 0
                'calc handicap
                iPlayerHdcp = (iScore - iPar) / iRounds * 0.8
                If iRounds = 1 Then
                    iPrevHdcp = iPlayerHdcp
                ElseIf iRounds >= 20 Then
                    For Each item In ListView1.Items
                        For i = 0 To sScores.Count - 1
                            If item("Total").ToString <= sScores(i) Then
                                sScores.Add(item.ToString)
                            End If
                        Next
                    Next
                ElseIf iRounds >= 10 Then

                End If

                Dim sHdcp As String = iPlayerHdcp
                Dim sPrevHdcp As String = iPrevHdcp

                'Total
                subItem = New ListViewItem.ListViewSubItem
                subItem.Name = "Total"
                subItem.Text = ohelper.convDBNulltoSpaces(score("Total"))
                lvRec.SubItems.Add(subItem)

                'Handicap
                subItem = New ListViewItem.ListViewSubItem
                subItem.Name = "Handicap"
                subItem.Text = iPlayerHdcp
                lvRec.SubItems.Add(subItem)

                'Net total
                subItem = New ListViewItem.ListViewSubItem
                subItem.Name = "Net"
                subItem.Text = score("Total") - iPlayerHdcp
                lvRec.SubItems.Add(subItem)

                ListView1.Items.Add(lvRec)
            Next
            'DataTable2CSV(edt, sFilePath & "\Scores.csv", ", ")

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub
End Class
