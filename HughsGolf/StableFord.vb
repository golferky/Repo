Partial Class ScoreCard

    ' =============================================================================
    '  StablefordTab.vb
    '  Read-only Stableford points display — scores entered on Scores tab.
    '  Pulls from dtScores, displays in its own dtStableford (never modifies dtScores).
    '
    '  Grid: Player | PHdcp | H1-H9 (read-only) | P1-P9 | Gross | Net | Pts
    '
    '  Wiring:
    '  ── LoadAllTabs after LoadScoresTab:
    '       LoadStablefordTab(tabControl)
    '  ── BuildTabShells tabs array after "Scores":
    '       "Stableford"
    '  ── BuildTabShells no-dock exclusion:
    '       tabName <> "Stableford" AndAlso
    '  ── TabControl_SelectedIndexChanged Select Case:
    '       Case "Stableford" : RefreshStablefordTab() : Exit Sub
    ' =============================================================================

#Region "Constants"
    Private Const SF_GROSS As String = "Gross"
    Private Const SF_NET As String = "Net"
    Private Const SF_PTOTAL As String = "Pts"
    Private Const SF_POINT_PREFIX As String = "P"
#End Region

#Region "Fields"
    Private dgStableford As DataGridView
    Private dtStableford As DataTable
    Private _holeNumbers() As Integer
#End Region

    ' ---------------------------------------------------------------------------
    '  Main entry point
    ' ---------------------------------------------------------------------------
    Private Sub LoadStablefordTab(tc As TabControl)

        LOGIT($"LoadStablefordTab called from: {New System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}")

        Dim tp = tc.TabPages("Stableford")
        If tp Is Nothing Then Exit Sub
        tp.Location = New Point(10, 10)

        ResolveHoleNumbers()

        Dim dgv As DataGridView = Nothing
        For Each ctrl As Control In tp.Controls
            If TypeOf ctrl Is DataGridView Then
                dgv = DirectCast(ctrl, DataGridView)
                Exit For
            End If
        Next
        If dgv Is Nothing Then Exit Sub

        dtStableford = BuildStablefordTable()
        PopulateFromDtScores()

        dgv.DataSource = dtStableford
        dgv.ReadOnly = True
        dgStableford = dgv

        RemoveHandler dgStableford.DataError, AddressOf dgStableford_DataError
        AddHandler dgStableford.DataError, AddressOf dgStableford_DataError
        AddHandler dgStableford.CellPainting, AddressOf dgStableford_CellPainting

        ChangeStablefordColAttributes(dgv)
        AddStablefordLegend(tp, dgv)

        ' Size to content — identical to LoadScoresTab
        dgStableford.BackgroundColor = Color.White
        dgStableford.Size = New Size(
            dgStableford.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) +
                dgStableford.RowHeadersWidth + 3,
            (dgStableford.Rows.Count * dgStableford.RowTemplate.Height) +
                dgStableford.ColumnHeadersHeight + 3)

        ' Row colouring — same 4-in-8 banding as LoadScoresTab
        dgStableford.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty
        For i As Integer = 0 To dgStableford.Rows.Count - 1 Step 8
            For ii As Integer = 0 To 3
                Dim rowIndex As Integer = i + ii
                If rowIndex < dgStableford.Rows.Count Then
                    dgStableford.Rows(rowIndex).DefaultCellStyle.BackColor = Color.LightBlue
                End If
            Next
        Next

        ColourStablefordPoints()
        dgStableford.Refresh()
        dgStableford.Invalidate()

    End Sub

    ' ---------------------------------------------------------------------------
    '  Called from TabControl_SelectedIndexChanged
    ' ---------------------------------------------------------------------------
    Friend Sub RefreshStablefordTab()
        If dtStableford Is Nothing OrElse dgStableford Is Nothing Then Exit Sub
        LOGIT("RefreshStablefordTab")
        PopulateFromDtScores()

        ' Reapply row banding after repopulate
        dgStableford.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty
        For i As Integer = 0 To dgStableford.Rows.Count - 1 Step 8
            For ii As Integer = 0 To 3
                Dim rowIndex As Integer = i + ii
                If rowIndex < dgStableford.Rows.Count Then
                    dgStableford.Rows(rowIndex).DefaultCellStyle.BackColor = Color.LightBlue
                End If
            Next
        Next

        ColourStablefordPoints()
        dgStableford.Refresh()
        dgStableford.Invalidate()
    End Sub

    ' ---------------------------------------------------------------------------
    '  DataTable — own table, dtScores never touched
    ' ---------------------------------------------------------------------------
    Private Function BuildStablefordTable() As DataTable
        Dim dt As New DataTable("Stableford")
        dt.Columns.Add("Player", GetType(String))
        dt.Columns.Add("PHdcp", GetType(Integer))

        ' Hole gross columns "1"-"9"
        For i As Integer = 1 To 9
            Dim col As New DataColumn(i.ToString(), GetType(Integer))
            col.AllowDBNull = True
            dt.Columns.Add(col)
        Next

        ' Per-hole point columns P1-P9
        For i As Integer = 1 To 9
            Dim col As New DataColumn($"{SF_POINT_PREFIX}{i}", GetType(Integer))
            col.AllowDBNull = True
            dt.Columns.Add(col)
        Next

        ' Totals
        For Each colName As String In {SF_GROSS, SF_NET, SF_PTOTAL}
            Dim col As New DataColumn(colName, GetType(Integer))
            col.AllowDBNull = True
            dt.Columns.Add(col)
        Next

        Return dt
    End Function

    ' ---------------------------------------------------------------------------
    '  Pull scores from dtScores, calculate points
    ' ---------------------------------------------------------------------------
    Private Sub PopulateFromDtScores()
        If dtScores Is Nothing OrElse dtStableford Is Nothing Then Exit Sub
        dtStableford.Rows.Clear()

        For Each scoreRow As DataRow In dtScores.Rows
            Dim row As DataRow = dtStableford.NewRow()
            row("Player") = scoreRow("Player")
            Dim phdcp As Int16 = If(IsDBNull(scoreRow("PHdcp")), 0, CShort(scoreRow("PHdcp")))
            row("PHdcp") = phdcp

            Dim totalGrs As Integer = 0
            Dim totalPts As Integer = 0
            Dim holesIn As Integer = 0

            For i As Integer = 1 To 9
                Dim colName As String = i.ToString()
                Dim gross As Object = If(dtScores.Columns.Contains(colName), scoreRow(colName), DBNull.Value)

                row(colName) = gross

                If gross Is Nothing OrElse gross Is DBNull.Value OrElse
                   gross.ToString() = "0" OrElse gross.ToString() = "" Then
                    row($"{SF_POINT_PREFIX}{i}") = DBNull.Value
                    Continue For
                End If

                Dim g As Integer = CInt(gross)
                Dim actualHole As Integer = _holeNumbers(i - 1)
                Dim strokes As Integer = showStrokeHoles(actualHole, phdcp)
                Dim par As Integer = CInt(ctx.thiscourse($"Hole{actualHole}"))
                Dim net As Integer = g - strokes
                Dim diff As Integer = par - net   ' positive = under par
                Dim spoints As String = "0,1,2,3,4,8"
                Dim idx As Integer = Math.Max(0, Math.Min(diff + 2, UBound(spoints.Split(","))))
                Dim iPts As Integer = CInt(spoints.Split(",")(idx))

                LOGIT($"SF: player={scoreRow("Player")} hole={actualHole} par={par} gross={g} strokes={strokes} net={net} pts={iPts}")

                row($"{SF_POINT_PREFIX}{i}") = iPts
                totalGrs += g
                totalPts += iPts
                holesIn += 1
            Next

            row(SF_GROSS) = If(holesIn > 0, CObj(totalGrs), DBNull.Value)
            row(SF_NET) = If(holesIn > 0, CObj(totalGrs - phdcp), DBNull.Value)
            row(SF_PTOTAL) = If(holesIn > 0, CObj(totalPts), DBNull.Value)

            dtStableford.Rows.Add(row)
        Next
    End Sub

    ' ---------------------------------------------------------------------------
    '  Column formatting — all read-only
    ' ---------------------------------------------------------------------------
    Private Sub ChangeStablefordColAttributes(dgv As DataGridView)
        If dgv.Columns.Count = 0 Then Return

        With dgv.Columns("Player")
            .HeaderText = "Player"
            .Width = 120
            .DefaultCellStyle.BackColor = Color.FromArgb(220, 230, 210)
        End With

        With dgv.Columns("PHdcp")
            .HeaderText = "HCP"
            .Width = 42
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End With

        ' Hole columns — header shows actual hole number
        For i As Integer = 0 To 8
            Dim colName As String = (i + 1).ToString()
            If Not dgv.Columns.Contains(colName) Then Continue For
            With dgv.Columns(colName)
                .HeaderText = $"{_holeNumbers(i)}"
                .Width = 36
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .DefaultCellStyle.NullValue = ""
            End With
        Next

        ' Point columns
        For i As Integer = 1 To 9
            Dim colName As String = $"{SF_POINT_PREFIX}{i}"
            If Not dgv.Columns.Contains(colName) Then Continue For
            With dgv.Columns(colName)
                .HeaderText = $"P{i}"
                .Width = 28
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .DefaultCellStyle.BackColor = Color.FromArgb(235, 245, 230)
                .DefaultCellStyle.NullValue = ""
            End With
        Next

        For Each pair As (col As String, hdr As String) In {(SF_GROSS, "Gross"), (SF_NET, "Net")}
            If Not dgv.Columns.Contains(pair.col) Then Continue For
            With dgv.Columns(pair.col)
                .HeaderText = pair.hdr
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                .DefaultCellStyle.BackColor = Color.FromArgb(235, 245, 230)
                .DefaultCellStyle.NullValue = ""
            End With
        Next

        With dgv.Columns(SF_PTOTAL)
            .HeaderText = "Pts"
            .Width = 48
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.BackColor = Color.FromArgb(210, 230, 210)
            .DefaultCellStyle.Font = New Font(dgv.Font, FontStyle.Bold)
            .DefaultCellStyle.NullValue = ""
        End With
    End Sub

    ' ---------------------------------------------------------------------------
    '  Legend groupbox — no Clear Scores button, no EditScoresEngine
    ' ---------------------------------------------------------------------------
    Private Sub AddStablefordLegend(tp As TabPage, dgv As DataGridView)
        Dim gb As New GroupBox()
        gb.Name = "gbStablefordInfo"
        gb.Text = "Stableford Points"
        gb.Size = New Size(550, 50)
        gb.Location = New Point(10, 10)

        Dim items As (Text As String, Color As Color)() = {
            ("Eagle+ (4pts)", Color.Gold),
            ("Birdie (3pts)", Color.LightGreen),
            ("Par (2pts)", Color.FromArgb(210, 230, 210)),
            ("Bogey (1pt)", Color.LightBlue),
            ("No Score (0)", Color.FromArgb(255, 210, 210))
        }

        Dim x As Integer = 10
        For Each item In items
            Dim lbl As New Label()
            lbl.AutoSize = True
            lbl.Text = item.Text
            lbl.BackColor = item.Color
            lbl.Location = New Point(x, 20)
            gb.Controls.Add(lbl)
            x += lbl.Width + 15
        Next

        tp.Controls.Add(gb)
        tp.Controls.Add(dgv)
        dgv.Location = New Point(gb.Left, gb.Bottom + 5)
    End Sub

    ' ---------------------------------------------------------------------------
    '  Colour Pts cell
    ' ---------------------------------------------------------------------------
    Private Sub ColourStablefordPoints()
        If dgStableford Is Nothing Then Exit Sub
        For Each row As DataGridViewRow In dgStableford.Rows
            Dim cell As DataGridViewCell = row.Cells(SF_PTOTAL)
            If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then Continue For
            Dim pts As Integer = CInt(cell.Value)
            Select Case True
                Case pts >= 23 : cell.Style.BackColor = Color.Gold
                Case pts >= 18 : cell.Style.BackColor = Color.LightGreen
                Case pts >= 14 : cell.Style.BackColor = Color.FromArgb(210, 230, 210)
                Case Else : cell.Style.BackColor = Color.FromArgb(255, 210, 210)
            End Select
        Next
    End Sub

    ' ---------------------------------------------------------------------------
    '  Cell painter — green handicap stroke dots on hole columns only
    ' ---------------------------------------------------------------------------
    Private Sub dgStableford_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        Try
            If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

            Dim colName As String = dgStableford.Columns(e.ColumnIndex).Name
            Dim holeNum As Integer
            If Not Integer.TryParse(colName, holeNum) Then Exit Sub
            If holeNum < 1 OrElse holeNum > 9 Then Exit Sub

            e.PaintBackground(e.ClipBounds, True)

            Dim row As DataRow = dtStableford.Rows(e.RowIndex)
            Dim phdcp As Integer = If(IsDBNull(row("PHdcp")), 0, CInt(row("PHdcp")))
            Dim actualHole As Integer = _holeNumbers(holeNum - 1)
            Dim si As Integer = CInt(ctx.thiscourse($"H{actualHole}"))

            ' Mirror Stableford() stroke logic exactly
            Dim strokeCount As Integer = 0
            If phdcp >= si Then strokeCount += 1
            If phdcp - 9 >= si Then strokeCount += 1

            If strokeCount > 0 Then
                Dim dotDiameter As Integer = 4
                Dim spacing As Integer = 2
                Dim totalWidth As Integer = strokeCount * dotDiameter + (strokeCount - 1) * spacing
                Dim startX As Integer = e.CellBounds.Left + (e.CellBounds.Width - totalWidth) \ 2
                For i As Integer = 0 To strokeCount - 1
                    Dim x = startX + i * (dotDiameter + spacing)
                    e.Graphics.FillEllipse(Brushes.Green, x, e.CellBounds.Top + 2, dotDiameter, dotDiameter)
                Next
            End If

            Dim scoreText As String = If(e.Value IsNot Nothing, e.Value.ToString(), "")
            If scoreText <> "" AndAlso scoreText <> "0" Then
                Using f As New Font("Segoe UI", 9, FontStyle.Regular)
                    Dim textRect As New Rectangle(e.CellBounds.X, e.CellBounds.Y + 4,
                                                  e.CellBounds.Width, e.CellBounds.Height - 4)
                    TextRenderer.DrawText(e.Graphics, scoreText, f, textRect, Color.Black,
                        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
                End Using
            End If

            e.Paint(e.CellBounds, DataGridViewPaintParts.Border)
            e.Handled = True

        Catch ex As Exception
            ' swallow
        End Try
    End Sub

    ' ---------------------------------------------------------------------------
    '  Helpers
    ' ---------------------------------------------------------------------------
    Private Sub ResolveHoleNumbers()
        Dim startHole As Integer = ctx.oHelper.iHoleMarker
        ReDim _holeNumbers(8)
        For i As Integer = 0 To 8
            _holeNumbers(i) = startHole + i
        Next
        LOGIT($"Stableford: {ctx.sFrontBack} nine — holes {_holeNumbers(0)}-{_holeNumbers(8)}")
    End Sub

    Private Sub dgStableford_DataError(sender As Object, e As DataGridViewDataErrorEventArgs)
        LOGIT($"dgStableford DataError col={e.ColumnIndex} row={e.RowIndex}: {e.Exception?.Message}")
        e.Cancel = True
    End Sub

End Class