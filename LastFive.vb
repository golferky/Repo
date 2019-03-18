Public Class LastFive
    Dim oHelper As Helper
    Dim fromsizeW As Integer, lvsizeW As Integer
    Dim rs As New Resizer
    'Dim lv1 As New SI.Controls.LvSort
    Private Sub frmPlayerStats_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        rs.FindAllControls(Me)
        Dim scbdates As New List(Of String)
        If oHelper.bsch Then
            For Each col As DataColumn In oHelper.dsLeague.Tables("dtSchedule").Columns
                Dim wkdate As DateTime = col.ColumnName
                'Dim wkdate As DateTime = DateTime.ParseExact(col.ColumnName, "MM/dd/yy", Globalization.CultureInfo.InvariantCulture)
                Dim reformatted As String = wkdate.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
                'cbDates.Items.Add(reformatted)
                scbdates.Add(reformatted)
            Next
        End If
        scbdates.Reverse()
        cbDates.Sorted = False
        'Dim wkdate2 As Date = oHelper.rLeagueParmrow("PostSeasonDt")
        'Dim reformatted2 As String = wkdate2.ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        For Each sDate In scbdates
            cbDates.Items.Add(sDate)
        Next
        cbDates.SelectedIndex = 0
        
        dgLast5.RowTemplate.Height = 15
        Me.Height = 1500
        btnDisplayScores_Click(sender, e)
    End Sub

    Private Sub btnDisplayScores_Click(sender As Object, e As EventArgs) Handles btnDisplayScores.Click
        Try
            '20180220-fix issue when no date entered
            If cbDates.SelectedItem Is Nothing Then
                MsgBox("Please select a date and try again")
                Exit Sub
            End If
            dgLast5.Rows.Clear()
            dgLast5.Columns.Clear()

            'oHelper.DisplayLast5(cbDates.SelectedItem, Me, lv1)
            Dim dvScores = New DataView(oHelper.dsLeague.Tables("dtScores"))

            dvScores.Sort = "Player, Date desc"
            dvScores.RowFilter = String.Format("Date <= {0}", cbDates.SelectedItem)

            Dim dvPlayers = New DataView(dvScores.ToTable(True, "Player"))
            dvPlayers.Sort = "Player"

            Dim dt As DataTable = dvScores.ToTable(True, "Player,Out_Gross,In_Gross".Split(",").ToArray)
            For Each Score In dt.Rows
                If Score("Out_Gross") Is DBNull.Value And Score("In_Gross") Is DBNull.Value Then Continue For
                If Score("Out_Gross") Is DBNull.Value Then
                    Score("Out_Gross") = Score("In_Gross")
                    Score("In_Gross") = DBNull.Value
                End If
            Next

            dt.Columns.Remove("In_Gross")
            dt.Columns("Out_Gross").ColumnName = "Score"

            Dim dtLast5 As New DataTable
            dtLast5.Columns.Add("Player")
            dtLast5.Columns.Add("Last Score")
            dtLast5.Columns.Add("1")
            dtLast5.Columns.Add("2")
            dtLast5.Columns.Add("3")
            dtLast5.Columns.Add("4")
            dtLast5.Columns.Add("5")

            For Each col As DataColumn In dtLast5.Columns
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = col.ColumnName
                dgc.ValueType = GetType(System.String)
                If dgc.Name = "Player" Then
                    dgc.Width = 120
                    dgc.HeaderText = "Player"
                ElseIf dgc.Name.StartsWith("Last") Then
                    dgc.Width = 60
                Else
                    dgc.Width = 30
                End If
                dgLast5.Columns.Add(dgc)
            Next

            For Each splayer In dvPlayers
                If cb2018.Checked Then
                    dvScores.RowFilter = String.Format("Player = '{0}' and Date <= {1} and Date >= {2}", splayer(0), cbDates.SelectedItem, "20180101")
                Else
                    dvScores.RowFilter = String.Format("Player = '{0}' and Date <= {1}", splayer(0), cbDates.SelectedItem)
                End If
                If dvScores.Count = 0 Then Continue For
                Dim newrow As DataRow = dtLast5.NewRow
                Dim i = 1
                'this uses its own data table
                Dim dvthisplayer As New DataView(dt)
                dvthisplayer.RowFilter = String.Format("Player = '{0}'", splayer("Player"))
                Dim sKey() As Object = {splayer("Player"), dvScores(0)("Date")}
                Dim sHdcp As String = dvScores(0)("Hdcp")
                newrow("Player") = sKey(0)
                newrow("Last Score") = sKey(1)
                Dim drow = oHelper.dsLeague.Tables("dtScores").Rows.Find(sKey)
                If drow Is Nothing Then
                    Throw New Exception(String.Format("LastFive - Cant find a score in dtscores for Player {0} Date {1}{2}Contact Developer ", splayer(0), cbDates.SelectedItem, vbCrLf))
                Else
                    newrow("Player") = newrow("Player") & "(" & sHdcp & ")"
                End If

                For Each score In dvthisplayer
                    If score("Score") Is DBNull.Value Then Continue For
                    newrow(i.ToString) = score("Score")
                    i += 1
                    If i > 5 Then Exit For
                Next
                dgLast5.Rows.Add(newrow.ItemArray)
            Next

            'oHelper.CopyDataGridViewToClipboard(dgLast5)
            Dim sfilename = oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "LastFive.csv"
            oHelper.dgv2csv(dgLast5, sfilename)

        Catch ex As Exception
            MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Private Sub dglast5_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgLast5.SortCompare

        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub dgLast5_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgLast5.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)

        lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
    End Sub

    Private Sub cb2018_CheckedChanged(sender As Object, e As EventArgs) Handles cb2018.CheckedChanged
        If cb2018.Checked Then

        End If
    End Sub

    Private Sub LastFive_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        rs.ResizeAllControls(Me)
    End Sub

    'Clipboard.SetDataObject(dgv.GetClipboardContent())

End Class