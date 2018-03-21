Imports System.IO
Public Class frmCourse
    Dim oHelper As Helper.Controls.Helper
    Dim stable = "dtCourses"
    Private Sub frmCourse_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        oHelper = Main.oHelper
        'write the xml so we can read it back in for xsd
        Dim sfilename = oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_hhmmss_") & "Courses.xml"
        oHelper.dsLeague.Tables(stable).WriteXml(sfilename, XmlWriteMode.WriteSchema)
        DsLeague.ReadXml(sfilename)
        System.IO.File.Delete(sfilename)
        '20180110-old code using xml
        'DsLeague.ReadXml(oHelper.getLatestFile("*Courses.xml"))
    End Sub
    Private Sub dtCoursesBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs) Handles dtCoursesBindingNavigatorSaveItem.Click
        Me.Validate()
        dtCoursesBindingSource.EndEdit()
        oHelper.DataTable2CSV(DsLeague.Tables(stable), oHelper.sFilePath & "\" & Now.ToString("yyyyMMdd") & "_Courses.csv")
        'DsLeague.WriteXml(oHelper.sFilePath & "\" & DateTime.Now.ToString("yyyyMMdd_") & "Courses.xml", XmlWriteMode.WriteSchema)
    End Sub
End Class