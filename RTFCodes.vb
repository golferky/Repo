Module RTF_CODES

    Public Function RTFHeader(ByVal LEGALorLETTER As String, ByVal PORTRAITorLANDSCAPE As String, ByVal FONTPOINT As Short, ByVal LEFTMARGIN As Short, ByVal RIGHTMARGIN As Short, ByVal TOPMARGIN As Short, ByVal BOTTOMMARGIN As Short) As String

        'ByVal FONTNAME As String, 
        ' NOTE FOR PAGE SIZES AND MARGINS: 1440 pixels = 1 inch 

        Const RTFLegalPortrait As String = "\paperw12240\paperh20160"
        Const RTFLetterPortrait As String = "\paperw12240\paperh15840"
        Const RTFLegalLandscape As String = "\paperw20160\paperh12240\landscape"
        Const RTFLetterLandscape As String = "\paperw15840\paperh12240\landscape"

        Const FONTNAME As String = "Courier New"

        RTFHeader = "{\rtf1\ansi\ansicpg1252\deff0\deflang1033"

        If UCase(LEGALorLETTER) = "LEGAL" Then
            If UCase(PORTRAITorLANDSCAPE) = "LANDSCAPE" Then
                RTFHeader = RTFHeader & RTFLegalLandscape
            Else
                RTFHeader = RTFHeader & RTFLegalPortrait
            End If
        Else
            If UCase(PORTRAITorLANDSCAPE) = "LANDSCAPE" Then
                RTFHeader = RTFHeader & RTFLetterLandscape
            Else
                RTFHeader = RTFHeader & RTFLetterPortrait
            End If
        End If

        RTFHeader = RTFHeader & vbCrLf
        ' Courier New 
        RTFHeader = RTFHeader & "{\fonttbl{\f0\fmodern\fprq1\fcharset0 " & FONTNAME & ";}}"

        RTFHeader = RTFHeader & "\margl" & LEFTMARGIN
        RTFHeader = RTFHeader & "\margr" & RIGHTMARGIN
        RTFHeader = RTFHeader & "\margt" & TOPMARGIN
        RTFHeader = RTFHeader & "\margb" & BOTTOMMARGIN

        RTFHeader = RTFHeader & "\viewkind4\uc1\pard\f0"

        RTFHeader = RTFHeader & "\fs" & (FONTPOINT * 2)

    End Function
    Public Function RTFCrlf() As String
        RTFCrlf = "\par"
    End Function
    Public Function RTFNewPage() As String
        RTFNewPage = "\page"
    End Function
    Public Function RTFClose() As String
        RTFClose = "}"
    End Function
End Module