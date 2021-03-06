﻿'Imports Microsoft.Office.Interop.Excel
Imports System.Runtime.CompilerServices

Public Class SkinRpt
    'Dim sdesc As String
    'Dim sDetail As String
    'Dim idue = 0
    'Dim iesCollected = 0
    'Dim iLeagueBalance = 0
    'Dim irscollected = 0
    'Dim irspaidout = 0
    'Dim iccpaidout = 0
    'Dim iSkins = 0
    'Dim iSkinspaidout = 0
    'Dim iCTP = 0
    'Dim iCTPpaidout = 0
    'Dim iesPaidOut1 = 0
    'Dim iesPaidOut2 = 0
    'Dim iecPaidOut1 = 0
    'Dim iecPaidOut2 = 0
    'Dim iExpenses = 0
    'Dim bfirst As Boolean = False
    Dim oHelper As GolfManager.Helper
    Const cFields As String = "Date-s,$CO,$E,SP,NS,$SC,$SP,LOS,CP,NC,$CC,$CP,$C1C,$C1P,$C2C,$C2P,LOF1,LOF2,LOB1,LOB2,LOK"
    Const Datecon As String = "Date"
    Const MoneyCollected As String = "$CO"
    Const MoneyPaidOut As String = "$E"
    '--- Skins
    Const SkinPlayers As String = "SP"
    Const NumberOfSkins As String = "NS"
    Const MoneyCollectedforSkins As String = "$SC"
    Const MoneyPaidOutforSkins As String = "$SP"
    Const MoneyCarriedOverforSkins As String = "LOS"
    '--- Closest To the Pins
    Const ClosesttothePinPlayers As String = "CP"
    Const NumberOfClosesttothePins As String = "NC"
    Const MoneyCollectedforClosesttothePins As String = "$CC"
    Const MoneyPaidOutforClosesttothePins As String = "$CP"
    Const MoneyCollectedforClosesttothePin1 As String = "$C1C"
    Const MoneyPaidOutforClosesttothePin1 As String = "$C1P"
    Const MoneyCollectedforClosesttothePin2 As String = "$C2C"
    Const MoneyPaidOutforClosesttothePin2 As String = "$C2P"
    Const MoneyCarriedoverforClosesttothePin1Front9 As String = "LOF1"
    Const MoneyCarriedoverforClosesttothePin2Front9 As String = "LOF2"
    Const MoneyCarriedoverforClosesttothePin1Back9 As String = "LOB1"
    Const MoneyCarriedoverforClosesttothePin2Back9 As String = "LOB2"
    Const MoneyCarriedoverforKitty As String = "LOK"

    Dim savedrow As DataRow
    Dim dKitty As Decimal = 0
    Dim dLOS As Decimal = 0
    Dim dLOF1 As Decimal = 0
    Dim dLOF2 As Decimal = 0
    Dim dLOB1 As Decimal = 0
    Dim dLOB2 As Decimal = 0
    Dim sArray As List(Of String)
    Dim spsdt As String
    Dim iCollected As Decimal = 3
    Dim dSkinValueThisDate As Decimal = 0
    Dim dCTPValueThisDate As Decimal = 0

    Private Sub SkinRpt_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try

            '20180928 to use interop excel for worksheets
            '        Dim oXL As Microsoft.Office.Interop.Excel.Application
            '        Dim oYB As Microsoft.Office.Interop.Excel.Workbooks
            '        Dim oWB As Microsoft.Office.Interop.Excel.Workbook
            '        Dim oSheets As Microsoft.Office.Interop.Excel.Sheets
            '        Dim oSheet As Microsoft.Office.Interop.Excel.Worksheet
            '        Dim oRG As Microsoft.Office.Interop.Excel.Range
            '        oXL = New Microsoft.Office.Interop.Excel.Application
            '        oXL.Visible = False
            '        oYB = oXL.Workbooks

            '        oWB = oYB.Open("\\wdmycloud\Gary\LeagueManager\Files\20180917_Payments.csv")
            '        oWB = oYB.Item(1)
            '        oSheets = oWB.Worksheets
            '        oSheet = CType(oSheets.Item(1), Microsoft.Office.Interop.Excel.Worksheet)
            '        oSheet.Name = "Sheet1"

            '        oWB.SaveAs("\\wdmycloud\Gary\LeagueManager\Files\20180917_Payments_" +
            'Date.Now.ToString("yyyyMMddhhmmss") + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7)
            '        oWB.Close()
            '        oXL.Quit()

            oHelper = Main.oHelper
            Dim dtr As New DataTable
            sArray = New List(Of String)
            spsdt = CDate(oHelper.rLeagueParmrow("PostSeasonDt")).ToString("yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
            Dim sfields As String() = cFields.Split(",")
            For Each fld As String In sfields
                sArray.Add(0)
                If UBound(fld.Split("-")) <> 0 Then
                    dtr.Columns.Add(fld.Split("-")(0))
                Else
                    dtr.Columns.Add(fld, GetType(Decimal))
                End If
            Next

            'build the table
            dtr.PrimaryKey = New DataColumn() {dtr.Columns(Datecon)}
            dtr.DefaultView.Sort = "Date Asc"

            Dim sdate = Main.cbLeagues.SelectedItem.ToString.Substring(Main.cbLeagues.SelectedItem.ToString.IndexOf("(") + 1, 4) & "0101"

            Dim dv As New DataView(oHelper.dsLeague.Tables("dtPayments"))
            'dv.RowFilter = String.Format("Date > {0} And Date < {1} and $Earn > 0", sdate, sdate.Replace("0101", "1231"))
            dv.RowFilter = String.Format("Date > {0} And Date < {1} and Earned > 0", sdate, sdate.Replace("0101", "1231"))
            dv.Sort = ("Date Asc")
            For Each row As DataRowView In dv
                calcAmount(dtr, row)
            Next

            'build the datagrid from the table
            'build grid columns and set column sizes
            For Each col As DataColumn In dtr.Columns
                Dim dgc As New DataGridViewTextBoxColumn
                dgc.Name = col.ColumnName
                dgc.ValueType = GetType(System.String)
                dgc.Width = 40
                If dgc.Name = Datecon Then dgc.Width = 60
                'If dgc.Name = "Player" Then
                '    dgc.Width = 100
                '    dgc.HeaderText = "Player      (Blue <> EOY)"
                'ElseIf dgc.Name = "Earned" Then
                '    dgc.HeaderText = "Earned (Pays Excl)"
                'Else
                '    dgc.Width = 60
                'End If

                dgSkins.Columns.Add(dgc)
            Next

            For Each row As DataRow In dtr.Rows
                dgSkins.Rows.Add(row.ItemArray)
            Next

            For Each row As DataGridViewRow In dgSkins.Rows
                Dim x As String = row.Cells(Datecon).Value
                If row.Cells(MoneyPaidOutforSkins).Value = 0 Or row.Cells(MoneyPaidOutforClosesttothePin1).Value = 0 Or row.Cells(MoneyPaidOutforClosesttothePin2).Value = 0 Then
                    'row("Player").Value = row.Cells(Datecon).Value = "20180710"splayer.Replace("* ", "")
                    row.Cells(Datecon).Style.BackColor = Color.LightBlue
                End If
            Next
            Dim al = New List(Of String)
            al.Add("MoneyCollected-$CO")
            al.Add("MoneyPaidOut-$E")
            al.Add("SkinPlayers-SP")
            al.Add("NumberOfSkins-NS")
            al.Add("MoneyCollectedforSkins-$SC")
            al.Add("MoneyPaidOutforSkins-$SP")
            al.Add("MoneyCarriedOverforSkins-LOS")
            al.Add("ClosesttothePinPlayers-CP")
            al.Add("NumberOfClosesttothePins-NC")
            al.Add("MoneyCollectedforClosesttothePins-$CC")
            al.Add("MoneyPaidOutforClosesttothePins-$CP")
            al.Add("MoneyCollectedforClosesttothePin1-$C1C")
            al.Add("MoneyPaidOutforClosesttothePin1-$C1P")
            al.Add("MoneyCollectedforClosesttothePin2-$C2C")
            al.Add("MoneyPaidOutforClosesttothePin2-$C2P")
            al.Add("MoneyCarriedoverforClosesttothePin1Front9-LOF1")
            al.Add("MoneyCarriedoverforClosesttothePin2Front9-LOF2")
            al.Add("MoneyCarriedoverforClosesttothePin1Back9-LOB1")
            al.Add("MoneyCarriedoverforClosesttothePin2Back9-LOB2")
            al.Add("MoneyCarriedoverforKitty-LOK")

            lvAbbr.View = View.Details
            lvAbbr.Columns.Clear()
            lvAbbr.Columns.Add("Abbr", 50)
            lvAbbr.Columns.Add("Description", 250)
            lvAbbr.Items.Clear()
            For Each item In al
                Dim lvi As ListViewItem = lvAbbr.Items.Add(item.Split("-")(1))
                lvi.SubItems.Add(item.Split("-")(0))
            Next

            Me.Text = Me.Text & " - " & Main.cbLeagues.SelectedItem
            lbStatus.Text = ""
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub
    Sub calcAmount(dtr As DataTable, row As DataRowView)
        Dim sKeys() As Object = {row(Datecon)}
        Dim dr As DataRow = dtr.Rows.Find(sKeys)

        Try
            'build a new row if it doesnt exist
            'Date,#Skins,#CTP,$Earned,$Skins,$CTP1,$CTP2
            If dr Is Nothing Then
                'this loop will restore leftover fields to the current row
                If row(Datecon) < spsdt Then
                    dSkinValueThisDate = oHelper.rLeagueParmrow("Skins")
                    dCTPValueThisDate = 1
                Else
                    dSkinValueThisDate = 7
                    dCTPValueThisDate = 3
                End If
                sArray.Item(0) = row(Datecon).ToString
                dtr.Rows.Add(sArray.ToArray)
                dr = dtr.Rows.Find(sKeys)

                Dim sKitty = 0
                Dim sSkins = 0
                Dim sF9ctp1 = 0
                Dim sF9ctp2 = 0
                Dim sB9ctp1 = 0
                Dim sB9ctp2 = 0

                If savedrow IsNot Nothing Then
                    sKitty = savedrow(MoneyCarriedoverforKitty)
                    sSkins = savedrow(MoneyCarriedOverforSkins)
                    sF9ctp1 = savedrow(MoneyCarriedoverforClosesttothePin1Front9)
                    sF9ctp2 = savedrow(MoneyCarriedoverforClosesttothePin2Front9)
                    sB9ctp1 = savedrow(MoneyCarriedoverforClosesttothePin1Back9)
                    sB9ctp2 = savedrow(MoneyCarriedoverforClosesttothePin2Back9)
                End If

                Dim dv As New DataView(oHelper.dsLeague.Tables("dtScores"))
                'SP / collected
                dv.RowFilter = String.Format("Date = {0} and (Skins = 'Y' or Skins = 'True') ", row(Datecon))
                dr(SkinPlayers) = dv.Count
                dr(MoneyCollectedforSkins) = (dv.Count * dSkinValueThisDate)

                '#CTPPlayers / collected
                dv.RowFilter = String.Format("Date = {0} and (Closest = 'Y' or Closest = 'True')", row(Datecon))
                dr(ClosesttothePinPlayers) += dv.Count
                dr(MoneyCollectedforClosesttothePins) += dv.Count * dCTPValueThisDate
                'the 2 must be changed in future in case there are not 2 ctps
                Dim d1 = Math.Round(dr(ClosesttothePinPlayers) / 2) * dCTPValueThisDate
                'Dim d2 = (dr(ClosesttothePinPlayers) * dCTPValueThisDate Mod 2) * dCTPValueThisDate
                'Dim loctp = d1 - d2
                Dim loctp = (dr(ClosesttothePinPlayers) * dCTPValueThisDate Mod 2) * dCTPValueThisDate

                dr(MoneyCollectedforClosesttothePin1) = d1
                dr(MoneyCollectedforClosesttothePin2) = d1
                dr(MoneyCollected) = dr(MoneyCollectedforSkins) + dr(MoneyCollectedforClosesttothePins)

                '#Skins-cannot refer to row name #Skins because row filter cannot deal with a field named with A # sign
                dv.RowFilter = String.Format("Date = {0} and $Skins > 0", row(Datecon))
                For Each drow As DataRowView In dv
                    dr(NumberOfSkins) += drow("#Skins")
                Next
                'Earned $$$ is Total Paid out
                'dr(MoneyPaidOut) = SumAmts("$Earn", row(Datecon))
                dv.RowFilter = String.Format("Date = {0} and $Earn > 0", row(Datecon))
                For Each drow As DataRowView In dv
                    dr(MoneyPaidOut) += drow("$Earn")
                Next

                '$Skins is what was paid out 
                'dr(MoneyPaidOutforSkins) = SumAmts("$Skins", row(Datecon))
                'dr(MoneyPaidOut) = SumAmts("$Earn", row(Datecon))
                dv.RowFilter = String.Format("Date = {0} and $Skins > 0", row(Datecon))
                For Each drow As DataRowView In dv
                    dr(MoneyPaidOutforSkins) += drow("$Skins")
                Next

                '#CTPs
                dv.RowFilter = String.Format("Date = {0} and CTP_1 > 0", row(Datecon))
                dr(NumberOfClosesttothePins) = dv.Count
                dv.RowFilter = String.Format("Date = {0} and CTP_2 > 0", row(Datecon))
                dr(NumberOfClosesttothePins) += dv.Count
                '$Closest is what was paid out
                dv.RowFilter = String.Format("Date = {0} and $Closest > 0", row(Datecon))
                For Each drow As DataRowView In dv
                    dr(MoneyPaidOutforClosesttothePins) += drow("$Closest")
                Next

                'dr(MoneyPaidOutforClosesttothePin1) = SumAmts("CTP_1", row(Datecon))
                dv.RowFilter = String.Format("Date = {0} and CTP_1 > 0", row(Datecon))
                For Each drow As DataRowView In dv
                    dr(MoneyPaidOutforClosesttothePin1) += drow("CTP_1")
                Next
                'dr(MoneyPaidOutforClosesttothePin2) = SumAmts("CTP_2", row(Datecon))
                dv.RowFilter = String.Format("Date = {0} and CTP_2 > 0", row(Datecon))
                For Each drow As DataRowView In dv
                    dr(MoneyPaidOutforClosesttothePin2) += drow("CTP_2")
                Next
                'Calc leftovers
                'ctp1 -2 collected calculation
                'determine Front/Back 9
                dv.RowFilter = String.Format("Date = {0} and Hole1 > 0", row(Datecon))
                If dv.Count > 0 Then
                    dr(MoneyCarriedoverforClosesttothePin1Front9) = dr(MoneyCollectedforClosesttothePin1) - dr(MoneyPaidOutforClosesttothePin1)
                    If dr(MoneyCarriedoverforClosesttothePin1Front9) < 0 Then dr(MoneyCarriedoverforClosesttothePin1Front9) = 0
                    dr(MoneyCarriedoverforClosesttothePin2Front9) = dr(MoneyCollectedforClosesttothePin2) - dr(MoneyPaidOutforClosesttothePin2)
                    If dr(MoneyCarriedoverforClosesttothePin2Front9) < 0 Then dr(MoneyCarriedoverforClosesttothePin2Front9) = 0
                Else
                    dr(MoneyCarriedoverforClosesttothePin1Back9) = dr(MoneyCollectedforClosesttothePin1) - dr(MoneyPaidOutforClosesttothePin1)
                    If dr(MoneyCarriedoverforClosesttothePin1Back9) < 0 Then dr(MoneyCarriedoverforClosesttothePin1Back9) = 0
                    dr(MoneyCarriedoverforClosesttothePin2Back9) = dr(MoneyCollectedforClosesttothePin2) - dr(MoneyPaidOutforClosesttothePin2)
                    If dr(MoneyCarriedoverforClosesttothePin2Back9) < 0 Then dr(MoneyCarriedoverforClosesttothePin2Back9) = 0
                End If

                dr(MoneyCarriedOverforSkins) = sSkins

                If dr(NumberOfSkins) > 0 Then
                    dr(MoneyCarriedOverforSkins) += (dr(MoneyCollectedforSkins) - dr(MoneyPaidOutforSkins)) - (dr(SkinPlayers) * dSkinValueThisDate) Mod dr(NumberOfSkins)
                Else
                    dr(MoneyCarriedOverforSkins) += dr(MoneyCollectedforSkins) - dr(MoneyPaidOutforSkins)
                End If
                'dr(MoneyCarriedOverforSkins) += dr(MoneyPaidOutforSkins) - 
                'Dim sloctp = 0
                'If dr(NumberOfClosesttothePins) > 0 Then
                '    sloctp = (dr(ClosesttothePinPlayers) * dCTPValueThisDate) Mod dr(NumberOfClosesttothePins)
                'End If

                'if left over CTPs were to go to kitty, add in here - dr("LOF1") - dr("LOF2") - dr("LOB1") - dr("LOB2")
                dr(MoneyCarriedoverforKitty) = sKitty
                If dr(NumberOfSkins) > 0 Then
                    dr(MoneyCarriedoverforKitty) += (dr(SkinPlayers) * dSkinValueThisDate) Mod dr(NumberOfSkins)
                End If
            End If

            savedrow = dr

        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Sub

    Public Function SumAmts(fld As String, sdate As String) As Decimal
        SumAmts = 0
        Try
            oHelper.LOGIT("summing-" & "-" & fld)
            SumAmts = Convert.ToInt32(oHelper.dsLeague.Tables("dtScores").Compute(String.Format("SUM({0})", fld), String.Format("Date = {0} and {1} > 0", sdate, fld)))

        Catch ex As Exception
            Dim x = ex.StackTrace.IndexOf("line:")
            oHelper.LOGIT(ex.Message & "-" & ex.StackTrace.Substring(ex.StackTrace.IndexOf(":line")) & "-" & fld)
        End Try
    End Function
    'Private Sub dgSkins_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSkins.CellMouseDoubleClick
    '    oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)
    '    Try
    '        Dim cell As DataGridViewTextBoxCell = sender.currentcell
    '        Dim row As DataGridViewRow = sender.currentrow
    '        '20180225-fix Mouse click to expand columns
    '        If e.ColumnIndex = 0 Then
    '            If cell.OwningColumn.Name = "Date" Then
    '                oHelper.dDate = Date.ParseExact(cell.Value, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
    '                Skins.Show()
    '            End If
    '        End If
    '    Catch ex As Exception

    '    End Try
    'End Sub
    Private Sub dgSkins_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles dgSkins.SortCompare

        Try
            oHelper.SortCompare(sender, e)
        Catch
            Dim x = ""
        End Try

    End Sub
    Private Sub dgSkins_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgSkins.ColumnHeaderMouseClick
        oHelper.LOGIT("Entering " & Reflection.MethodBase.GetCurrentMethod.Name)

        Dim newColumn As DataGridViewColumn = sender.Columns(e.ColumnIndex)
        lbStatus.Text = String.Format("Resorting Columns by {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)

        lbStatus.Text = String.Format("Finished Resorting Column {0}", newColumn.HeaderText)
        oHelper.status_Msg(lbStatus, Me)
    End Sub

End Class