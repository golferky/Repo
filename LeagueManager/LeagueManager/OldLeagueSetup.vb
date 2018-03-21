Public Class OldLeagueSetup
    '    Dim TextBoxes As New Dictionary(Of String, MaskedTextBox)
    '    Dim LabelModel As New Label
    '    Dim strConnString = String.Empty
    '    Dim oHelper As Helper.Controls.Helper
    '    Dim scores(18) As String
    '    Dim sInScore As Integer = 0, sOutScore As Integer = 0, sTotScore As Integer = 0

    '    Dim sLeagueParms As String = "Name	Secretary	Format	Cost	#Teams	PayPlaces	StartDate	# HdcpScores	HdcpFormat	CarryLastYears 	MaxHdcp	Par3Max	Par4Max	Par5Max	Course  Holes"
    '    Dim sLeagueParmValues As String = "Hugh's	Sam Dinn	2 Man Medal	30	12	3	4/11/2017	5	Std	Y	16	6	8	9	Boone Links 9"
    '#Region "Custom Methods"
    '    Public Function fFindField(sFieldname) As String
    '        fFindField = Me.gbLeagueInfo.Controls.Find(sFieldname, True).OfType(Of MaskedTextBox).First().Text
    '    End Function
    '    Private Sub LeagueSetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    '        oHelper = Main.oHelper
    '        'ClearCourseData_Click(sender, e)
    '    End Sub
    '    Public Function fCreateLeaguedt() As DataTable
    '        fCreateLeaguedt = Nothing
    '        fCreateLeaguedt = New DataTable
    '        Try
    '            Dim sLPArray As String() = sLeagueParms.Split(vbTab)
    '            For Each parm As String In sLPArray
    '                Dim column As DataColumn
    '                column = New DataColumn()
    '                column.ColumnName = parm
    '                If parm = "Name" Then
    '                    Dim keys(1) As DataColumn
    '                    column.DataType = System.Type.GetType("System.String")
    '                    keys(0) = column
    '                    ' Add the column to the DataTable.Columns collection.
    '                    fCreateLeaguedt.Columns.Add(column)
    '                    fCreateLeaguedt.PrimaryKey = New DataColumn() {fCreateLeaguedt.Columns(0)}
    '                Else
    '                    fCreateLeaguedt.Columns.Add(column)
    '                End If
    '            Next
    '            oHelper.CreateRowfromLine(sLeagueParmValues.Split(vbTab), fCreateLeaguedt)
    '        Catch ex As Exception
    '            MsgBox(ex.Message)
    '        End Try
    '    End Function
    '    Public Sub DisplayLeagueParms()
    '        Dim ileft = 10
    '        Dim itop = 20
    '        Try
    '            oHelper.dtLeagueParms = oHelper.CSV2DataTable(txtLeagueName.Text & ".csv")
    '            If Not oHelper.dtLeagueParms Is Nothing Then
    '                For Each row As DataRow In oHelper.dtLeagueParms.Rows
    '                    For Each column As DataColumn In oHelper.dtLeagueParms.Columns
    '                        oHelper.CreateLabel(Me.gbLeagueInfo.Controls, itop, ileft, 100, column.ColumnName, column.ColumnName, "B", "V")
    '                        itop = oHelper.CreateTextbox(Me.gbLeagueInfo.Controls, itop, ileft + 200, 100, column.ColumnName, row(column.ColumnName), "V")
    '                    Next
    '                Next
    '                If File.Exists(oHelper.sFilePath & "\Courses.csv") Then
    '                    oHelper.dtCourses = oHelper.CSV2DataTable("\Courses.csv")
    '                    Dim sCourses As String() = Nothing

    '                    Try
    '                        If Not oHelper.dtCourses Is Nothing Then
    '                            Dim tbCourse As MaskedTextBox = Me.gbLeagueInfo.Controls.Find("Course", True).OfType(Of MaskedTextBox).First()
    '                            For Each course As DataRow In oHelper.dtCourses.Rows
    '                                For Each coursecolumn As DataColumn In oHelper.dtCourses.Columns
    '                                    If coursecolumn.ColumnName = "Name" Then
    '                                        If course(coursecolumn.ColumnName) = tbCourse.Text Then
    '                                            Exit Sub
    '                                        End If
    '                                    End If
    '                                Next
    '                            Next

    '                            Dim sResult = MsgBox(tbCourse.Text & vbCrLf & "Course not found...Is this a new one?", MsgBoxStyle.YesNo)
    '                            If sResult = MsgBoxResult.Yes Then
    '                                frmCourse.Show()
    '                            Else
    '                                MsgBox("Exiting so you can correct the spelling")
    '                                Exit Sub
    '                            End If

    '                        End If

    '                    Catch ex As Exception
    '                        MsgBox(ex.Message)
    '                    End Try
    '                Else
    '                    'Courses File not found
    '                    Dim sResult = MsgBox("Course file not found...want to create a new one?", MsgBoxStyle.YesNo)
    '                    If sResult = MsgBoxResult.Yes Then
    '                        frmCourse.ShowDialog()
    '                        DisplayLeagueParms()
    '                    Else
    '                        MsgBox("Hey " & fFindField("Secretary") & vbCrLf & "You can't put a course (" & fFindField("Course") & ") in without a course file(Courses.csv)" & vbCrLf & " if you have one put it in " & oHelper.sFilePath & " and try again")
    '                        Exit Sub
    '                    End If
    '                    'oHelper.dtCourses = frmCourse.CreateCourse
    '                End If
    '            End If
    '        Catch ex As Exception
    '            MsgBox(ex.Message)
    '            End Try

    '    End Sub
    '#End Region

    '#Region "Event Driven methods"
    '    Private Sub btnLeagueParms_Click(sender As Object, e As EventArgs) Handles btnLeagueParms.Click
    '        If oHelper.dtLeagueParms Is Nothing Then
    '            Dim sResult = MsgBox("League file not found...want to create a new one?", MsgBoxStyle.YesNo)
    '            If sResult = MsgBoxResult.Yes Then
    '                oHelper.dtLeagueParms = fCreateLeaguedt()
    '                Dim ileft = 100
    '                Dim itop = 20
    '                For Each row As DataRow In oHelper.dtLeagueParms.Rows
    '                    For Each column As DataColumn In oHelper.dtLeagueParms.Columns
    '                        oHelper.CreateLabel(Me.gbLeagueInfo.Controls, itop, ileft, 100, column.ColumnName, column.ColumnName, "B", "V")
    '                        itop = oHelper.CreateTextbox(Me.gbLeagueInfo.Controls, itop, ileft + 100, 100, column.ColumnName, row(column.ColumnName), "V")
    '                    Next
    '                Next
    '                oHelper.DataTable2CSV(oHelper.dtLeagueParms, oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv")
    '                lbMessage.Text = oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv" & " Created"
    '                lbMessage.Font = New Font(lbMessage.Font, FontStyle.Bold)
    '                lbMessage.Visible = True
    '                Me.Show()
    '            Else
    '                MsgBox("Cannot proceed without a course file...if you have one put it in " & oHelper.sFilePath & "and re-type the league name")
    '                Exit Sub
    '            End If
    '        Else
    '            oHelper.DataTable2CSV(oHelper.dtLeagueParms, oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv")
    '            lbMessage.Text = oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv" & " Created"
    '            lbMessage.Font = New Font(lbMessage.Font, FontStyle.Bold)
    '            lbMessage.Visible = True
    '            Me.Show()
    '        End If
    '    End Sub
    '    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    '        Close()
    '    End Sub
    '    Private Sub txtLeagueName_TextChanged(sender As Object, e As EventArgs) Handles txtLeagueName.TextChanged
    '        'this is here because this event is triggered before the form load triggers, which loads the helper
    '        If oHelper Is Nothing Then
    '            oHelper = Main.oHelper
    '        End If
    '        If File.Exists(oHelper.sFilePath & "\" & txtLeagueName.Text & ".csv") Then
    '            DisplayLeagueParms()
    '        Else
    '            Me.gbLeagueInfo.Controls.Clear()
    '        End If


    '    End Sub
    '#End Region
    'End Class
End Class
