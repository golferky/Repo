'Public Class LastFive
'    Dim oHelper As Helper
'    Dim fromsizeW As Integer, lvsizeW As Integer
'    Dim rs As New Resizer
'    'Dim lv1 As New SI.Controls.LvSort
'    Private Sub frmPlayerStats_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
'        oHelper = Main.oHelper
'        'rs.FindAllControls(Me)

'        For Each item In Main.cbDates.Items
'            If item >= CDate(ctx.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd") Then Continue For
'            cbDates.Items.Add(item)
'        Next
'        If cbDates.Items.Contains(oHelper.dDate.ToString("yyyyMMdd")) Then cbDates.SelectedIndex = cbDates.Items.IndexOf(oHelper.dDate.ToString("yyyyMMdd"))

'        dgLast5.RowTemplate.Resizable = True
'        dgLast5.RowTemplate.Height = 15

'        'Me.Height = 1500
'        Dim sWH As String = oHelper.ScreenResize("614", "1500")
'        Me.Width = sWH.Split(":")(0)
'        Me.Height = sWH.Split(":")(1)
'        LOGIT(String.Format("Screen Height {0} Width {1}", Main.iScreenHeight, Main.iScreenWidth))
'        dgLast5.ClearSelection()
'    End Sub
'    Private Sub dglast5_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgLast5.SortCompare

'        Try
'            oHelper.SortCompare(sender, e)
'        Catch
'            Dim x = ""
'        End Try

'    End Sub
'    Private Sub dgLast5_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgLast5.ColumnHeaderMouseClick
'        LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

'        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
'        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
'        oHelper.status_Msg(lbStatus, Me)

'        lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
'        oHelper.status_Msg(lbStatus, Me)
'    End Sub
'    Private Sub cb2018_CheckedChanged(sender As Object, e As EventArgs) Handles cb2018.CheckedChanged
'        If cb2018.Checked Then

'        End If
'    End Sub
'    Private Sub cbDates_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDates.SelectedIndexChanged
'        Try
'            '20180220-fix issue when no date entered
'            If cbDates.SelectedItem Is Nothing Then
'                MsgBox("Please select a date and try again")
'                Exit Sub
'            End If
'            dgLast5.Rows.Clear()
'            dgLast5.Columns.Clear()

'            'oHelper.DisplayLast5(cbDates.SelectedItem, Me, lv1)
'            Dim dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))

'            dvScores.Sort = "Player, Date desc"
'            dvScores.RowFilter = String.Format("Date <= {0} and date not in ('{1}')", cbDates.SelectedItem, String.Join("','", oHelper.lignoreDates)) '& String.Format(" and Player = 'Howard Gorman'")
'            'If dvScores(0)("Out_Gross") Is DBNull.Value And dvScores(0)("In_Gross") Is DBNull.Value Then
'            '    cbDates.SelectedItem = dvScores(1)("Date")
'            'End If
'            Dim dvPlayers = New DataView(dvScores.ToTable(True, "Player"))
'            dvPlayers.Sort = "Player"

'            Dim dt As DataTable = dvScores.ToTable(False, "Player,Out_Gross,In_Gross".Split(",").ToArray)
'            For Each Score In dt.Rows
'                If Score("Out_Gross") Is DBNull.Value And Score("In_Gross") Is DBNull.Value Then Continue For
'                If Score("Out_Gross") Is DBNull.Value Then
'                    Score("Out_Gross") = Score("In_Gross")
'                    Score("In_Gross") = DBNull.Value
'                End If
'            Next

'            dt.Columns.Remove("In_Gross")
'            dt.Columns("Out_Gross").ColumnName = "Score"

'            Dim dtLast5 As New DataTable
'            dtLast5.Columns.Add("Player")
'            dtLast5.Columns.Add("Last Score")
'            dtLast5.Columns.Add("1")
'            dtLast5.Columns.Add("2")
'            dtLast5.Columns.Add("3")
'            dtLast5.Columns.Add("4")
'            dtLast5.Columns.Add("5")

'            For Each col As DataColumn In dtLast5.Columns
'                Dim dgc As New DataGridViewTextBoxColumn
'                dgc.Name = col.ColumnName
'                dgc.ValueType = GetType(System.String)
'                If dgc.Name = "Player" Then
'                    dgc.Width = 120
'                    dgc.HeaderText = "Player"
'                ElseIf dgc.Name.StartsWith("Last") Then
'                    dgc.Width = 60
'                Else
'                    dgc.Width = 30
'                End If
'                dgLast5.Columns.Add(dgc)
'            Next

'            Dim slstregs = New List(Of String)
'            Dim slstsubs = New List(Of String)
'            For Each splayer In dvPlayers
'                If cb2018.Checked Then
'                    dvScores.RowFilter = String.Format("Player = '{0}' and Date <= {1} and Date >= {2} and Date not in ('{3}')", splayer(0), cbDates.SelectedItem, "20180101", String.Join("','", oHelper.lignoreDates))
'                Else
'                    dvScores.RowFilter = String.Format("Player = '{0}' and Date <= {1} and Date not in ('{2}')", splayer(0), cbDates.SelectedItem, String.Join("','", oHelper.lignoreDates))
'                End If
'                dvScores.RowFilter &= "and (Out_Net > 0 or In_Net > 0)"
'                If dvScores.Count = 0 Then Continue For
'                Dim newrow As DataRow = dtLast5.NewRow
'                Dim i = 1
'                'this uses its own data table
'                Dim dvthisplayer As New DataView(dt)
'                dvthisplayer.RowFilter = String.Format("Player = '{0}'", splayer("Player"))
'                Dim sKey() As Object = {splayer("Player"), dvScores(0)("Date")}

'                Dim sHdcp As String = dvScores(0)("Hdcp")
'                newrow("Player") = sKey(0)
'                newrow("Last Score") = sKey(1)
'                Dim drow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKey)
'                If drow Is Nothing Then
'                    Throw New Exception(String.Format("LastFive - Cant find a score in dtscores for Player {0} Date {1}{2}Contact Developer ", splayer(0), cbDates.SelectedItem, vbCrLf))
'                Else
'                    newrow("Player") = newrow("Player") & "(" & sHdcp & ")"
'                End If

'                Dim sscores As String = ""
'                For Each score In dvthisplayer
'                    If score("Score") Is DBNull.Value Then Continue For
'                    newrow(i.ToString) = score("Score")
'                    sscores &= score("Score") & ","
'                    i += 1
'                    If i > 5 Then Exit For
'                Next
'                dgLast5.Rows.Add(newrow.ItemArray)
'                Debug.Print($"{splayer("Player")} finding in player file")
'                Dim drthisPlayer As DataRow = oHelper.dsLeague.Tables("dtPlayers").Rows.Find(splayer("Player"))
'                Dim steam As String = If(drthisPlayer("Team") Is DBNull.Value, "99", drthisPlayer("Team").ToString)
'                If steam = "99" Then
'                    slstsubs.Add(splayer("Player").ToString & "(" & sHdcp & ")" & "|" & sKey(1) & "," & sscores.Substring(0, Len(sscores) - 1))
'                Else
'                    slstregs.Add(splayer("Player").ToString & "(" & sHdcp & ")" & "|" & sKey(1) & "," & sscores.Substring(0, Len(sscores) - 1))
'                End If
'                'Debug.Print(steam)
'            Next

'            dgLast5.ClearSelection()

'            Dim sfn = oHelper.sReportPath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "LastFive.csv"
'            Dim swl5 As New IO.StreamWriter(sfn, False)
'            'oHelper.dgv2csv(dgLast5, sfn)
'            swl5.WriteLine(",Regulars,,,,,,,,Subs")
'            swl5.WriteLine("Player,Last Score,1,2,3,4,5,,Player,Last Score,1,2,3,4,5")
'            Dim ictr As Integer = If(slstregs.Count > slstsubs.Count, slstregs.Count, slstsubs.Count)
'            For i = 0 To ictr - 1
'                Dim sline As String = ""
'                'if both lists have 
'                If i <= slstregs.Count - 1 And i <= slstsubs.Count - 1 Then
'                    sline &= slstregs(i).Split("|")(0) & "," & slstregs(i).Split("|")(1) & ",," & slstsubs(i).Split("|")(0) & "," & slstsubs(i).Split("|")(1)
'                ElseIf i >= slstregs.Count - 1 Then
'                    sline &= ",,,,,,,," & slstsubs(i).Split("|")(0) & "," & slstsubs(i).Split("|")(1)
'                ElseIf i >= slstsubs.Count - 1 Then
'                    sline &= slstregs(i).Split("|")(0) & "," & slstregs(i).Split("|")(1)
'                End If
'                swl5.WriteLine(sline)
'            Next
'            swl5.Close()

'            If Not Main.cbOffice.Checked Then Exit Sub
'            Dim CSV_Path As String = sfn
'            Dim XLS_Path As String = sfn.Replace(".csv", ".xls")

'            Dim myCSV_XLS As New CSV_XLS
'            Dim myMemoryStream As System.IO.MemoryStream = myCSV_XLS.CopyTextFile_ToMemoryStream(CSV_Path)

'            If myMemoryStream IsNot Nothing Then
'                Dim CSV_XLS_RC As Boolean = myCSV_XLS.CreateXLS_FromCSV(myMemoryStream, XLS_Path)
'            End If

'            Dim oXL As Microsoft.Office.Interop.Excel.Application
'            Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
'            Dim oWB As Microsoft.Office.Interop.Excel.Workbook
'            Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
'            Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
'            Dim oRG As Microsoft.Office.Interop.Excel.Range
'            oXL = New Microsoft.Office.Interop.Excel.Application
'            oXL.Visible = False
'            oYB = oXL.Workbooks
'            IO.File.Delete(sfn.Replace("csv", "xls"))
'            oWB = oYB.Open(sfn)
'            oWB = oYB.Item(1)
'            oSheets = oWB.Worksheets
'            oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
'            oSheet.Name = "Sheet1"
'            oSheet.Columns("A:ZZ").AutoFit

'            oSheet.PageSetup.PrintArea = ""
'            oSheet.PageSetup.TopMargin = oXL.InchesToPoints(0.5)
'            oSheet.PageSetup.BottomMargin = oXL.InchesToPoints(0.5)
'            oSheet.PageSetup.LeftMargin = oXL.InchesToPoints(0.25)
'            oSheet.PageSetup.RightMargin = oXL.InchesToPoints(0.25)
'            oSheet.PageSetup.FooterMargin = 0
'            oSheet.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape
'            oSheet.PageSetup.FitToPagesWide = True
'            oSheet.PageSetup.FitToPagesTall = True

'            oSheet.Rows(1).font.bold = True
'            oSheet.Rows(1).font.underline = True
'            oSheet.Rows(1).wraptext = True
'            oSheet.Rows(1).font.size = 16
'            oSheet.Rows(2).font.bold = True
'            oSheet.Rows(2).font.underline = True
'            oSheet.Columns("B").ColumnWidth = 11.57

'            With oSheet.Cells.Range(String.Format("a1:g{0}", slstregs.Count + 2))
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical).Weight = 2
'            End With

'            With oSheet.Cells.Range(String.Format("i1:o{0}", slstsubs.Count + 2))
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal).Weight = 2
'                .Borders(Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical).Weight = 2
'                .Interior.Color = Color.LightBlue
'            End With

'            'create a PDF file from XLS
'            oSheet.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, sfn.Replace("\Reports", "\Reports\ToEmail").Replace("csv", "pdf"))
'            oWB.SaveAs(sfn.Replace("csv", "xls"), Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8)
'            oWB.Close()
'            oXL.Quit()

'            If Not Debugger.IsAttached Then
'                'oHelper.CopyDataGridViewToClipboard(dgLast5)
'                '20190822 - new html
'                Dim sHtml As String = oHelper.Create_Html_From_DGV(dgLast5)
'                sHtml = oHelper.ConvertDataGridViewToHTMLWithFormatting(dgLast5, Me)
'                Dim swhtml As New IO.StreamWriter(sfn.Replace(".csv", ".html"), False)
'                swhtml.WriteLine(sHtml)
'                swhtml.Close()
'            End If
'        Catch ex As Exception
'            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
'        End Try


'    End Sub
'    Private Sub LastFive_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
'        'rs.ResizeAllControls(Me)
'    End Sub
'    'Clipboard.SetDataObject(dgv.GetClipboardContent())
'End Class