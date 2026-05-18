Imports System
Imports System.Drawing
Imports System.Windows.Forms
Public Class MarkSkin

    Private Sub dataGridView1_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        ' Check if this is the value column
        If e.ColumnIndex = ScoreCard.dgScores.Columns("Value").Index AndAlso e.RowIndex >= 0 Then
            ' Retrieve the cell value
            Dim cellValue As Integer = Convert.ToInt32(e.Value)

            ' Perform custom painting only for certain values
            If cellValue > 15 Then
                ' Paint the cell background
                e.Paint(e.CellBounds, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)

                ' Draw a circle around the cell content
                Dim radius As Integer = Math.Min(e.CellBounds.Width, e.CellBounds.Height) / 2 - 2
                Dim center As New Point(e.CellBounds.Left + e.CellBounds.Width / 2, e.CellBounds.Top + e.CellBounds.Height / 2)
                Using pen As New Pen(Color.Red, 2)
                    e.Graphics.DrawEllipse(pen, center.X - radius, center.Y - radius, radius * 2, radius * 2)
                End Using

                ' Draw the cell content
                e.PaintContent(e.CellBounds)

                ' Indicate that the painting is handled
                e.Handled = True
            End If
        End If
    End Sub
End Class
