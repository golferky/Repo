Imports System.IO
Public Class frmPlayer
    Dim oHelper As Helper
    Dim stable = "dtPlayers"
    Private Sub frmPlayer_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        '20180110-old code using xml
        'DsLeague.ReadXml(oHelper.getLatestFile("*Players.xml"))
        'write the xml so we can read it back in for xsd
        Dim sfilename = oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "Players.xml"
        oHelper.dsLeague.Tables(stable).WriteXml(sfilename, XmlWriteMode.WriteSchema)
        DsLeague.ReadXml(sfilename)
        DsLeague.dtPlayers.PrimaryKey = New DataColumn() {DsLeague.Tables("dtPlayers").Columns("Name")}
        System.IO.File.Delete(sfilename)
    End Sub
    Private Sub dtPlayersBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles DtPlayersBindingNavigatorSaveItem.Click
        Me.Validate()
        DtPlayersBindingSource.EndEdit()
        'DsLeague.WriteXml(oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_") & "Players.xml", XmlWriteMode.WriteSchema)
        If oHelper.dsLeague.Tables.Contains(stable) Then oHelper.dsLeague.Tables.Remove(stable)
        oHelper.DataTable2CSV(DsLeague.Tables(stable), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Players.csv")
    End Sub

    Private Sub frmPlayer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim result = MsgBox("Are you sure you want to quit without saving?", MsgBoxStyle.YesNo)
        If result = MsgBoxResult.No Then
            e.Cancel = True
        End If
    End Sub
End Class