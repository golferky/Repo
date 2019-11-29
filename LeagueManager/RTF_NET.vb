'***************************************************************************** 
'* This class was designed to easly create RTF reports using a RTF template 
'* to setup the basic formating. This class will automatically determine the 
'* correct spacing for the report. 
'***************************************************************************** 

Option Strict Off 

Public Class RTF_NET
    Dim _sRTFTemplate As String
    Dim _sRTF As String
    Dim _sSubHdr As String
    Dim _HDRLEFT(5) As String
    Dim _HDRCNTR(5) As String
    Dim _MaxCharCnt As Integer
    Dim _CharCnt As Integer
    Dim _Cols As New ArrayList
    Dim _Data(0) As String
    Dim _ColSpacing As Integer
    Dim _Fill As String

    ' Page Formating 
    Public Enum PageFormat
        LegalPort = 0
        LetterPort = 1
        LegalLand = 2
        LetterLand = 3
    End Enum
    ' Data Justification 
    Public Enum DataJustify
        Left = 0
        Center = 1
        Right = 2
    End Enum



    ''' <summary> 
    ''' Construtor for the RTF_Net Class. This will intailize the main items of the report. 
    ''' </summary> 
    ''' <param name="sTemplate">This is the RTF file that contain the format of the report and the Tags 
    ''' this class will use to place data.</param> 
    ''' <param name="ePgFmt">Page format</param> 
    ''' <remarks></remarks> 
    Public Sub New(ByVal sTemplate As String, ByVal ePgFmt As PageFormat)
        Try
            _sRTFTemplate = sTemplate
            Select Case ePgFmt

                Case PageFormat.LetterPort
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<PAGE FORMAT>>>", "\paperw12240\paperh15840")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEL>>>", "2880")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEC>>>", "6048")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLER>>>", "9360")
                    _MaxCharCnt = 95
                Case PageFormat.LegalPort
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<PAGE FORMAT>>>", "\paperw12240\paperh20160")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEL>>>", "2880")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEC>>>", "6048")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLER>>>", "9360")
                    _MaxCharCnt = 95

                Case PageFormat.LetterLand
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<PAGE FORMAT>>>", "\paperw15840\paperh12240\landscape")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEL>>>", "4200")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEC>>>", "8400")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLER>>>", "12600")
                    _MaxCharCnt = 131
                Case PageFormat.LegalLand
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<PAGE FORMAT>>>", "\paperw20160\paperh12240\landscape")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEL>>>", "5700")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLEC>>>", "11400")
                    _sRTFTemplate = _sRTFTemplate.Replace("<<<TABLER>>>", "17100")
                    _MaxCharCnt = 175
            End Select

            For i As Integer = 0 To 4
                _HDRLEFT(i) = ""
                _HDRCNTR(i) = ""
            Next
            _sSubHdr = ""
        Catch ex As Exception
            Throw New Exception("RTF_NET-New:" & ex.Message)
        End Try
    End Sub
    ''' <summary> 
    ''' Add a column to the report 
    ''' </summary> 
    ''' <param name="sHdr1">Top line of the column header</param> 
    ''' <param name="sHdr2">2nd line of the column header</param> 
    ''' <param name="iFL">Length give to the column on the report</param> 
    ''' <param name="Just">Justification of data under on the report. This is no the hdr justification</param> 
    ''' <remarks></remarks> 
    Public Sub AddCol(ByVal sHdr1 As String, ByVal sHdr2 As String, ByVal iFL As Integer, Optional ByVal Just As DataJustify = DataJustify.Left)
        Try

            Dim Col As New RTFCOL(sHdr1, sHdr2, iFL, Just)
            _Cols.Add(Col)
            _CharCnt = _CharCnt + Col.GetFL()
            If _CharCnt > _MaxCharCnt Then
                Throw New Exception("Max Char exceeded:Count-" & _CharCnt & " Max-" & _MaxCharCnt)
            End If
            _ColSpacing = (_MaxCharCnt - _CharCnt) \ _Cols.Count
            _Fill = Space(_ColSpacing)
            ReDim Preserve _Data(_Cols.Count)
        Catch ex As Exception
            Throw New Exception("RTF_NET-AddCol:" & ex.Message)
        End Try
    End Sub

    ''' <summary> 
    ''' Set report header (Left side) 
    ''' </summary> 
    ''' <param name="sL1">Left header 1</param> 
    ''' <param name="sL2">>Left header 2</param> 
    ''' <param name="sL3">>Left header 3</param> 
    ''' <param name="sL4">>Left header 4</param> 
    ''' <param name="sL5">>Left header 5</param> 
    ''' <remarks></remarks> 
    Public Sub SetLeftHdr(ByVal sL1 As String, ByVal sL2 As String, ByVal sL3 As String, ByVal sL4 As String, ByVal sL5 As String)
        Try
            _HDRLEFT(0) = sL1
            _HDRLEFT(1) = sL2
            _HDRLEFT(2) = sL3
            _HDRLEFT(3) = sL4
            _HDRLEFT(4) = sL5
        Catch ex As Exception
            Throw New Exception("RTF_NET-SetLeftHdr:" & ex.Message)
        End Try
    End Sub
    ''' <summary> 
    ''' Set report header (Center) 
    ''' </summary> 
    ''' <param name="sL1">Center line 1</param> 
    ''' <param name="sL2">Center line 2</param> 
    ''' <param name="sL3">Center line 3</param> 
    ''' <param name="sL4">Center line 4</param> 
    ''' <param name="sL5">Center line 5</param> 
    ''' <remarks></remarks> 
    Public Sub SetCntrHdr(ByVal sL1 As String, ByVal sL2 As String, ByVal sL3 As String, ByVal sL4 As String, ByVal sL5 As String)
        Try
            _HDRCNTR(0) = sL1
            _HDRCNTR(1) = sL2
            _HDRCNTR(2) = sL3
            _HDRCNTR(3) = sL4
            _HDRCNTR(4) = sL5
        Catch ex As Exception
            Throw New Exception("RTF_NET-SetCntrHdr:" & ex.Message)
        End Try
    End Sub
    ''' <summary> 
    ''' Override the automaticaly calculated spacing. 
    ''' </summary> 
    ''' <param name="iSpacing">New spacing</param> 
    ''' <remarks>If you call AddCols after you call this function it will wipe out your override</remarks> 
    Public Sub OverrideColSpacing(ByVal iSpacing As Integer)
        Try
            _ColSpacing = iSpacing
            _Fill = Space(_ColSpacing)
        Catch ex As Exception
            Throw New Exception("RTF_NET-OverrideColSpacing:" & ex.Message)
        End Try
    End Sub
    ''' <summary> 
    ''' Retrive the number of charaters per line. 
    ''' </summary> 
    ''' <returns></returns> 
    ''' <remarks></remarks> 
    Public Function GetMaxCharCnt() As Integer
        GetMaxCharCnt = _MaxCharCnt
    End Function

    ''' <summary> 
    ''' This function will add a report header to the RTF header. If you pass in <<<SKIP>>> it will skip the line 
    ''' if you pass in <<<HEADER1>>>, it will write line 1 of the col headers to the report, and <<<HEADER2>>> 
    ''' will write out the 2nd header line. Anything else will be just written to the report. 
    ''' </summary> 
    ''' <param name="sL1">Header line 1</param> 
    ''' <param name="sL2">Header line 2</param> 
    ''' <param name="sL3">Header line 3</param> 
    ''' <param name="sL4">Header line 4</param> 
    ''' <param name="bAddLine">Add dashline at the end of the header</param> 
    ''' <param name="bBoldHdrLines">Bold the header</param> 
    ''' <remarks></remarks> 
    Public Sub SetSubHdr(ByVal sL1 As String, ByVal sL2 As String, ByVal sL3 As String, ByVal sL4 As String, ByVal bAddLine As Boolean, ByVal bBoldHdrLines As Boolean)
        Try
            Dim sSHdr(4) As String
            Dim sDashes As String = "", sLine As String = ""
            Dim cRTFCOL As RTFCOL
            Dim i As Integer, j As Integer, x As Integer
            sSHdr(0) = sL1
            sSHdr(1) = sL2
            sSHdr(2) = sL3
            sSHdr(3) = sL4


            For i = 0 To 3
                sLine = ""
                Select Case sSHdr(i).Trim.ToUpper
                    Case "<<<SKIP>>>"

                    Case "<<<HEADER1>>>"
                        For j = 0 To _Cols.Count - 1
                            cRTFCOL = _Cols(j)

                            Select Case j
                                Case 0
                                    sLine = sLine & Left(_Fill, _Fill.Length \ 2) & CenterIt(cRTFCOL.GetHDR1, cRTFCOL.GetFL()) & _Fill
                                Case _Cols.Count - 1
                                    sLine = sLine & CenterIt(cRTFCOL.GetHDR1, cRTFCOL.GetFL()) & Left(_Fill, _Fill.Length \ 2)
                                Case Else
                                    sLine = sLine & CenterIt(cRTFCOL.GetHDR1, cRTFCOL.GetFL()) & _Fill
                            End Select
                        Next
                        If bBoldHdrLines Then
                            sLine = "\b " & sLine & "\b0"
                        End If
                        _sSubHdr = _sSubHdr & Left(sLine, _MaxCharCnt) & "\par" & vbCrLf
                    Case "<<<HEADER2>>>"
                        For j = 0 To _Cols.Count - 1
                            cRTFCOL = _Cols(j)

                            Select Case j
                                Case 0
                                    sLine = sLine & Left(_Fill, _Fill.Length \ 2) & CenterIt(cRTFCOL.GetHDR2, cRTFCOL.GetFL()) & _Fill
                                Case _Cols.Count - 1
                                    sLine = sLine & CenterIt(cRTFCOL.GetHDR2, cRTFCOL.GetFL()) & Left(_Fill, _Fill.Length \ 2)
                                Case Else
                                    sLine = sLine & CenterIt(cRTFCOL.GetHDR2, cRTFCOL.GetFL()) & _Fill
                            End Select
                        Next
                        If bBoldHdrLines Then
                            sLine = "\b " & sLine & "\b0"
                        End If
                        _sSubHdr = _sSubHdr & Left(sLine, _MaxCharCnt) & "\par" & vbCrLf
                    Case Else
                        _sSubHdr = _sSubHdr & Left(sSHdr(i), _MaxCharCnt)(i) & "\par" & vbCrLf

                End Select

            Next

            If bAddLine Then
                sLine = ""
                For j = 0 To _Cols.Count - 1
                    cRTFCOL = _Cols(j)
                    sDashes = ""
                    For x = 1 To cRTFCOL.GetFL()
                        sDashes = sDashes & "-"
                    Next
                    Select Case j
                        Case 0
                            sLine = sLine & Left(_Fill, _Fill.Length \ 2) & sDashes & _Fill
                        Case _Cols.Count - 1
                            sLine = sLine & sDashes & Left(_Fill, _Fill.Length \ 2)
                        Case Else
                            sLine = sLine & sDashes & _Fill
                    End Select

                Next
                _sSubHdr = _sSubHdr & Left(sLine, _MaxCharCnt) & "\par" & vbCrLf

            End If
        Catch ex As Exception
            Throw New Exception("RTF_NET-SetSubHdr:" & ex.Message)
        End Try
    End Sub
    ''' <summary> 
    ''' Set report data 
    ''' </summary> 
    ''' <param name="iCol">Position of column in array (0 based)</param> 
    ''' <param name="sData">Value</param> 
    ''' <remarks></remarks> 
    Public Sub SetData(ByVal iCol As Integer, ByVal sData As String)
        Try
            Dim Col As RTFCOL = _Cols(iCol)
            Select Case Col.GetJust
                Case DataJustify.Left
                    _Data(iCol) = LeftIt(sData, Col.GetFL)
                Case DataJustify.Center
                    _Data(iCol) = CenterIt(sData, Col.GetFL)
                Case DataJustify.Right
                    _Data(iCol) = RightIt(sData, Col.GetFL)
            End Select
        Catch ex As Exception
            Throw New Exception("RTF_NET-SetData:" & ex.Message)
        End Try

    End Sub
    ''' <summary> 
    ''' Get the starting position of a column on the report 
    ''' </summary> 
    ''' <param name="iCol">Position of column in array (0 based)</param> 
    ''' <returns>Starting postion of the column on the report</returns> 
    ''' <remarks></remarks> 
    Public Function GetColPosition(ByVal iCol As Integer) As Integer
        Try
            Dim sLine As String = ""
            Dim Col As RTFCOL
            For j As Integer = 0 To _Cols.Count - 1
                Col = _Cols(j)
                If j = iCol Then
                    If j = 0 Then
                        sLine = sLine & Left(_Fill, _Fill.Length \ 2)
                    End If
                    Exit For
                Else
                    Select Case j
                        Case 0
                            Col = _Cols(j)
                            sLine = sLine & Left(_Fill, _Fill.Length \ 2) & Space(Col.GetFL) & _Fill
                        Case _Cols.Count - 1
                            sLine = sLine & Space(Col.GetFL) & Left(_Fill, _Fill.Length \ 2)
                        Case Else
                            sLine = sLine & Space(Col.GetFL) & _Fill
                    End Select

                End If
            Next
            GetColPosition = sLine.Length
        Catch ex As Exception
            Throw New Exception("RTF_NET-GetColPosition:" & ex.Message)
        End Try
    End Function
    ''' <summary> 
    ''' Get the length of a column on the report 
    ''' </summary> 
    ''' <param name="iCol">Position of column in array (0 based)</param> 
    ''' <returns>Lenght of column</returns> 
    ''' <remarks></remarks> 
    Public Function GetFieldLen(ByVal iCol As Integer) As Integer
        Try
            Dim col As RTFCOL = _Cols(iCol)

            GetFieldLen = col.GetFL
        Catch ex As Exception
            Throw New Exception("RTF_NET-GetFieldLen:" & ex.Message)
        End Try
    End Function
    ''' <summary> 
    ''' Write a line to the report. \par will be added after your string 
    ''' </summary> 
    ''' <param name="sLine">String to write to the report</param> 
    ''' <remarks></remarks> 
    Public Sub WriteLine(ByVal sLine As String)
        _sRTF = _sRTF & sLine & "\par" & vbCrLf

    End Sub

    ''' <summary> 
    ''' Write the all columns to the report 
    ''' </summary> 
    ''' <remarks></remarks> 
    Public Sub WriteColData()
        Dim sLine As String = ""
        Try
            For j As Integer = 0 To _Cols.Count - 1
                Select Case j
                    Case 0
                        sLine = sLine & Left(_Fill, _Fill.Length \ 2) & _Data(j) & _Fill
                    Case _Cols.Count - 1
                        sLine = sLine & _Data(j) & Left(_Fill, _Fill.Length \ 2)
                    Case Else
                        sLine = sLine & _Data(j) & _Fill
                End Select
                _Data(j) = ""
            Next
            _sRTF = _sRTF & Left(sLine, _MaxCharCnt) & "\par" & vbCrLf
        Catch ex As Exception
            Throw New Exception("RTF_NET-WriteColData:" & ex.Message)
        End Try

    End Sub
    ''' <summary> 
    ''' Write anything to the report. No RFT characters will be added to your string 
    ''' </summary> 
    ''' <param name="sData">Data to write out</param> 
    ''' <remarks></remarks> 
    Public Sub WriteFreeForm(ByVal sData As String)
        _sRTF = _sRTF & sData
    End Sub

    ''' <summary> 
    ''' Create the RTF report by replacing the templates tags in the RTF. 
    ''' </summary> 
    ''' <returns>The RTF to be written to a output file</returns> 
    ''' <remarks></remarks> 
    Public Function CreateRTF() As String
        Try
            _sRTFTemplate = _sRTFTemplate.Replace("<<<SUBPAGEHDR>>>", _sSubHdr)
            _sRTFTemplate = _sRTFTemplate.Replace("<<<RTFDOCUMENT>>>", _sRTF)
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRLEFT1>>>", _HDRLEFT(0))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRLEFT2>>>", _HDRLEFT(1))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRLEFT3>>>", _HDRLEFT(2))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRLEFT4>>>", _HDRLEFT(3))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRLEFT5>>>", _HDRLEFT(4))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRCNTR1>>>", _HDRCNTR(0))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRCNTR2>>>", _HDRCNTR(1))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRCNTR3>>>", _HDRCNTR(2))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRCNTR4>>>", _HDRCNTR(3))
            _sRTFTemplate = _sRTFTemplate.Replace("<<<HDRCNTR5>>>", _HDRCNTR(4))
            CreateRTF = _sRTFTemplate
        Catch ex As Exception
            Throw New Exception("RTF_NET-CreateRTF:" & ex.Message)
        End Try
    End Function


    ''' <summary> 
    ''' Left justify a value 
    ''' </summary> 
    ''' <param name="TheString">String</param> 
    ''' <param name="TheLen">Length of string</param> 
    ''' <param name="TheFill">Optional fill data</param> 
    ''' <returns>Left justified string</returns> 
    ''' <remarks></remarks> 

    Public Function LeftIt(ByVal TheString As String, ByVal TheLen As Integer, Optional ByVal TheFill As String = "") As String
        Try
            If TheFill <> "" Then
                TheFill = Replace(Space(TheLen), " ", TheFill)
            Else
                TheFill = Space(TheLen)
            End If
            LeftIt = Mid(TheString & TheFill, 1, TheLen)
        Catch ex As Exception
            Throw New Exception("RTF_NET-LeftIt:" & ex.Message)
        End Try
    End Function
    ''' <summary> 
    ''' Right justify a value 
    ''' </summary> 
    ''' <param name="TheString">String</param> 
    ''' <param name="TheLen">Length of string</param> 
    ''' <param name="TheFill">Optional fill data</param> 
    ''' <returns>Right justified string</returns> 
    ''' <remarks></remarks> 
    Public Function RightIt(ByVal TheString As String, ByVal TheLen As Integer, Optional ByVal TheFill As String = "") As String
        Try
            If TheFill <> "" Then
                TheFill = Replace(Space(TheLen), " ", TheFill)
            Else
                TheFill = Space(TheLen)
            End If
            RightIt = Mid(TheFill & TheString, (Len(TheFill & TheString) - TheLen) + 1)
        Catch ex As Exception
            Throw New Exception("RTF_NET-RightIt:" & ex.Message)
        End Try
    End Function
    ''' <summary> 
    ''' Center justify a value 
    ''' </summary> 
    ''' <param name="TheString">String</param> 
    ''' <param name="TheLen">Length of string</param> 
    ''' <param name="TheFill">Optional fill data</param> 
    ''' <returns>Center justified string</returns> 
    ''' <remarks></remarks> 
    Public Function CenterIt(ByVal TheString As String, ByVal TheLen As Integer, Optional ByVal TheFill As String = "") As String
        Try
            Dim iZ As Integer
            iZ = (TheLen - Len(TheString)) / 2
            If TheFill <> "" Then
                TheFill = Replace(Space(TheLen), " ", TheFill)
            Else
                TheFill = Space(TheLen)
            End If
            If iZ > 0 Then
                CenterIt = Mid((Mid(TheFill, 1, iZ) & TheString & TheFill), 1, TheLen)
            Else
                CenterIt = Mid(TheString & TheFill, 1, TheLen)
            End If
        Catch ex As Exception
            Throw New Exception("RTF_NET-CenterIt:" & ex.Message)
        End Try
    End Function

    ''' <summary> 
    ''' You need to have 1 RTFCOL for each column you the RTF report. These are corrected in  RTF_Net.AddCol 
    ''' </summary> 
    ''' <remarks></remarks> 
    Public Class RTFCOL
        Dim _sHeader1 As String
        Dim _sHeader2 As String
        Dim _iFieldLength As Integer
        Dim _Justify As DataJustify
        ''' <summary> 
        '''Creates desciption of a data column 
        ''' </summary> 
        ''' <param name="sHdr1">Top line of the column header</param> 
        ''' <param name="sHdr2">2nd line of the column header</param> 
        ''' <param name="iFL">Length give to the column on the report</param> 
        ''' <param name="cDataJust">Justification of data under on the report. This is no the hdr justification</param> 
        ''' <remarks></remarks> 

        Sub New(ByVal sHdr1 As String, ByVal sHdr2 As String, ByVal iFL As Integer, Optional ByVal cDataJust As DataJustify = DataJustify.Left)
            Try
                _sHeader1 = Left(sHdr1, iFL)
                _sHeader2 = Left(sHdr2, iFL)
                _iFieldLength = iFL
                _Justify = cDataJust
            Catch ex As Exception
                Throw New Exception("RTFCOL-New:" & ex.Message)
            End Try
        End Sub
        Public Function GetHDR1() As String
            GetHDR1 = _sHeader1
        End Function
        Public Function GetHDR2() As String
            GetHDR2 = _sHeader2
        End Function
        Public Function GetFL() As Integer
            GetFL = _iFieldLength
        End Function
        Public Function GetJust() As DataJustify
            GetJust = _Justify
        End Function
    End Class

End Class