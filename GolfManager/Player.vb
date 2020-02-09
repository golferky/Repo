Imports System.IO
Public Class Player
    Dim oHelper As New Helper
    Dim stable = "dtPlayers"
    Dim sOldCellValue = ""
    Dim bError = False
    Private Sub Player_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        '20180110-old code using xml
        'DsLeague.ReadXml(oHelper.getLatestFile("*Players.xml"))
        'write the xml so we can read it back in for xsd
        'Dim sfilename = oHelper.sFilePath & "\Players.xml"
        'oHelper.dsLeague.Tables(stable).WriteXml(sfilename, XmlWriteMode.WriteSchema)
        'DsLeague.ReadXml(sfilename)
        oHelper.CSV2DataTable(DsLeague.dtPlayers, oHelper.sFilePath & "\Players.csv")
        DsLeague.dtPlayers.PrimaryKey = New DataColumn() {DsLeague.Tables("dtPlayers").Columns("Name")}

        DsLeague.dtPlayers.DefaultView.Sort = "Team Asc, Grade Asc"
        rebind()
        'oHelper.SortCompare(DtPlayersDataGridView, e)
    End Sub
    'Private Sub dtPlayersBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles DtPlayersBindingNavigatorSaveItem.Click
    '    Try
    '        Me.Validate()
    '        If Not bError Then
    '            DtPlayersBindingSource.EndEdit()
    '            'DsLeague.WriteXml(oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_") & "Players.xml", XmlWriteMode.WriteSchema)
    '            If oHelper.dsLeague.Tables.Contains(stable) Then oHelper.dsLeague.Tables.Remove(stable)
    '            oHelper.DataTable2CSV(DsLeague.Tables(stable), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Players.csv")
    '        End If
    '    Catch ex As Exception
    '        'MsgBox(oHelper.GetExceptionInfo(ex))
    '        MsgBox(ex.Message)
    '    End Try
    'End Sub

    Private Sub frmPlayer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            'Dim result = MsgBox("Are you sure you want to quit without saving?", MsgBoxStyle.YesNo)
            'If result = MsgBoxResult.Yes Then
            '    Me.Dispose()
            'Else
            '    e.Cancel = True
            'End If
            'If oHelper.dsLeague.Tables.Contains(stable) Then oHelper.dsLeague.Tables.Remove(stable)
            oHelper.DataTable2CSV(DsLeague.Tables(stable), oHelper.sFilePath & "\Players.csv")
        End If
    End Sub
    'this sub is initiated if there us an error on a row change
    Private Sub DtPlayersDataGridView_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles DtPlayersDataGridView.DataError
        MsgBox(String.Format(e.Exception.Message & vbCrLf & "Changing back to {0}", sOldCellValue))
        bError = True
    End Sub
    'Private Sub dtPlayersDataGridView_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DtPlayersDataGridView.KeyPress
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try
    '        For Each row As DataGridViewRow In DtPlayersDataGridView.Rows
    '            If row.Selected Then Exit Sub
    '        Next
    '        Dim sOldCellValue As String = ""
    '        Dim dgr As DataGridView = sender
    '        'If dgr.CurrentCell.Value = sOldCellValue Then Exit Sub
    '        Dim sCurrColName = dgr.CurrentCell.OwningColumn.Name
    '        Dim R As DataGridViewRow = dgr.CurrentRow
    '        Dim dgc As DataGridViewCell = sender.currentcell
    '        If Not dgc.ReadOnly Then
    '            Try
    '                'save old value in case we fail an edit
    '                sOldCellValue = dgc.Value
    '                dgc.Value = ""
    '                oHelper.bDGSError = False
    '                'If sCurrColName = "Player" Then oHelper.fGetPlayer(dgc.Value, dgr)
    '                If oHelper.bDGSError Then oHelper.bDGSError = False
    '            Catch ex As Exception
    '                MsgBox("Error " & ex.Message & vbCrLf & ex.StackTrace)
    '            End Try
    '        End If
    '    Catch ex As Exception
    '        MsgBox(oHelper.GetExceptionInfo(ex))
    '    End Try

    'End Sub
    'Private Sub dtPlayersDataGridView_KeyDown(sender As Object, e As KeyEventArgs) Handles DtPlayersDataGridView.KeyDown
    '    Dim dgc As DataGridViewCell = sender.currentcell
    '    sOldCellValue = dgc.Value
    'End Sub
    Private Sub dtPlayersDataGridView_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DtPlayersDataGridView.CellBeginEdit
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        If sender.currentcell.value IsNot DBNull.Value Then
            If sender.currentcell.value IsNot Nothing Then sOldCellValue = sender.currentcell.value
        End If

    End Sub

    Private Sub dtPlayersDataGridView_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DtPlayersDataGridView.CellEndEdit

        Dim dgc As DataGridViewCell = sender.currentcell
        If dgc.OwningColumn.HeaderText = "Name" Then
            Dim sov = dgc.Value
            Dim sgp = oHelper.fGetPlayer(dgc.Value, sender)
            If sgp.Contains(":New") Then
                dgc.Value = sOldCellValue
                Dim BindingSource = New BindingSource()
                BindingSource.DataSource = DsLeague.dtPlayers
                DtPlayersDataGridView.DataSource = BindingSource
                Dim aRow As DataRow
                aRow = DsLeague.dtPlayers.NewRow
                aRow("Name") = sgp.Split(":")(0) 'Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sNameInfo)
                DsLeague.dtPlayers.Rows.Add(aRow)
                BindingSource.ResetBindings(False)
            End If

            If sov = sgp Then
                dgc.Value = sOldCellValue
            Else
                dgc.Value = sgp
            End If
        End If

    End Sub
    Private Sub dtPlayersDataGridView_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles DtPlayersDataGridView.SortCompare
        oHelper.SortCompare(sender, e)
    End Sub
    Sub rebind()
        Dim BindingSource = New BindingSource()
        BindingSource.DataSource = DsLeague.dtPlayers
        DtPlayersDataGridView.DataSource = BindingSource
    End Sub

    Private Sub rbRegulars_CheckedChanged(sender As Object, e As EventArgs) Handles rbRegulars.CheckedChanged
        Dim BindingSource = New BindingSource()
        BindingSource.DataSource = DsLeague.dtPlayers
        BindingSource.Filter = "Team > 0 "
        DtPlayersDataGridView.DataSource = BindingSource
    End Sub

    Private Sub rbAll_CheckedChanged(sender As Object, e As EventArgs) Handles rbAll.CheckedChanged
        Dim BindingSource = New BindingSource()
        BindingSource.DataSource = DsLeague.dtPlayers
        BindingSource.Filter = ""
        DtPlayersDataGridView.DataSource = BindingSource
    End Sub
End Class