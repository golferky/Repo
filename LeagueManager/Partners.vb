Public Class frmPartners
    Dim lvSave As New ListView, ListView1 As New ListView, ListView2 As ListView
    Private fromIndex As Integer
    Private dragIndex As Integer
    Private dragRect As Rectangle
    Private Sub ListView_ItemDrag(ByVal sender As Object, ByVal e As  _
System.Windows.Forms.ItemDragEventArgs)
        Dim myItem As ListViewItem
        Dim myItems(sender.SelectedItems.Count - 1) As ListViewItem
        Dim i As Integer = 0

        ' Loop though the SelectedItems collection for the source.
        For Each myItem In sender.SelectedItems
            ' Add the ListViewItem to the array of ListViewItems.
            myItems(i) = myItem
            i = i + 1
        Next
        ' Create a DataObject containg the array of ListViewItems.
        sender.DoDragDrop(New  _
        DataObject("System.Windows.Forms.ListViewItem()", myItems), _
        DragDropEffects.Move)
    End Sub

    Private Sub ListView_DragEnter(ByVal sender As Object, ByVal e As  _
    System.Windows.Forms.DragEventArgs)
        ' Check for the custom DataFormat ListViewItem array.
        If e.Data.GetDataPresent("System.Windows.Forms.ListViewItem()") Then
            e.Effect = DragDropEffects.Move
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub ListView_DragDrop(ByVal sender As Object, ByVal e As  _
    System.Windows.Forms.DragEventArgs)
        Dim myItem As ListViewItem
        Dim myItems() As ListViewItem = e.Data.GetData("System.Windows.Forms.ListViewItem()")
        Dim i As Integer = 0

        For Each myItem In myItems
            ' Add the item to the target list.
            sender.Items.Add(myItems(i).Text)
            ' Remove the item from the source list.
            If sender Is ListView1 Then
                ListView2.Items.Remove(ListView2.SelectedItems.Item(0))
            Else
                ListView1.Items.Remove(ListView1.SelectedItems.Item(0))
            End If
            i = i + 1
        Next
    End Sub

    Private Sub ListView2_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        Dim lv As ListView = sender
        ListView2.Items.Add(lv.SelectedItems(0).Clone)
    End Sub

    Private Sub btnSubmit_Click(sender As System.Object, e As System.EventArgs) Handles btnSubmit.Click
        Me.Close()
        Return
    End Sub

    Private Sub btnReset_Click(sender As System.Object, e As System.EventArgs) Handles btnReset.Click
        ListView3.Items.Clear()
        Me.Show()
    End Sub

    Private Sub frmPartners_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        lvSave = ListView1
    End Sub

    Private Sub btnLink_Click(sender As System.Object, e As System.EventArgs) Handles btnLink.Click
        Dim lvrec = New ListViewItem(cbAplayers.SelectedItem.ToString)
        lvrec.SubItems.Add(cbBPlayers.SelectedItem.ToString)
        ListView3.Items.Add(lvrec)
    End Sub

    Private Sub cbBPlayers_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbBPlayers.SelectedIndexChanged
        If cbAplayers.SelectedItem = cbBPlayers.SelectedItem Then
            cbBPlayers.SelectedIndex = -1
            MsgBox("Cant link a player to himself...try again")
        End If
    End Sub

    Private Sub frmPartners_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

    End Sub

    Private Sub ListView3_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles ListView3.MouseDoubleClick
        ListView3.Items.RemoveAt(ListView3.FocusedItem.Index)
    End Sub
End Class