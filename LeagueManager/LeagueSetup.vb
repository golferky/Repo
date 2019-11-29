Public Class frmLeagueSetup
    Dim oHelper As Helper
    Dim stable = "dtLeagueParms"
    Private Sub frmLeagueSetup_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        'write the xml so we can read it back in for xsd
        'If Not oHelper.CSV2DataTable(oHelper.dsLeague.Tables("dtLeagueParms"), oHelper.getLatestFile("*LeagueParms.csv")) Then
        '    MsgBox(String.Format("File in use, close file and restart {0} {1}", vbCrLf, oHelper.getLatestFile("*LeagueParms.csv")))
        '    End
        'End If
        Dim sfilename = oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "LeagueParms.xml"
        'Dim sfilename = oHelper.getLatestFile("*LeagueParms.csv")
        oHelper.dsLeague.Tables(stable).WriteXml(sfilename, XmlWriteMode.WriteSchema)
        DsLeague.ReadXml(sfilename)
        System.IO.File.Delete(sfilename)
        '20180110-old code using xml
        'DsLeague.ReadXml(oHelper.getLatestFile("*LeagueParms.xml"))
    End Sub
    'Private Sub dtPlayersBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles DtLeagueParmsBindingNavigatorSaveItem.Click
    Private Sub dtLeagueParmsBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles DtLeagueParmsBindingNavigatorSaveItem.Click
        Me.Validate()
        DtLeagueParmsBindingSource.EndEdit()
        'create a file name for a temp xml
        Dim sfilename = oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "LeagueParms.xml"
        'write the temp xml file
        DsLeague.Tables(stable).WriteXml(sfilename, XmlWriteMode.WriteSchema)
        'remove the helper table from the main dataset
        oHelper.dsLeague.Tables.Remove(stable)
        'read it back into the main dataset
        oHelper.dsLeague.ReadXml(sfilename)
        'delete the temp file
        System.IO.File.Delete(sfilename)
        'now create the csv from it
        oHelper.DataTable2CSV(DsLeague.Tables(stable), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_LeagueParms.csv")
        'DsLeague.WriteXml(oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_") & "LeagueParms.xml", XmlWriteMode.WriteSchema)
    End Sub

End Class
