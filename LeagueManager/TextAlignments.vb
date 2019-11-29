Option Strict Off
Option Explicit On 
Module TextAlignments
    Public Function LeftIt(ByVal TheString As String, ByVal TheLen As Short, Optional ByVal TheFill As String = "") As String
        If TheFill <> "" Then
            TheFill = Replace(Space(TheLen), " ", TheFill)
        Else
            TheFill = Space(TheLen)
        End If
        LeftIt = Mid(TheString & TheFill, 1, TheLen)
    End Function
    Public Function RightIt(ByVal TheString As String, ByVal TheLen As Short, Optional ByVal TheFill As String = "") As String
        If TheFill <> "" Then
            TheFill = Replace(Space(TheLen), " ", TheFill)
        Else
            TheFill = Space(TheLen)
        End If
        RightIt = Mid(TheFill & TheString, (Len(TheFill & TheString) - TheLen) + 1)
    End Function
    Public Function CenterIt(ByVal TheString As String, ByVal TheLen As Short, Optional ByVal TheFill As String = "") As String
        Dim iZ As Short
        iZ = (TheLen - Len(TheString)) / 2
        If TheFill <> "" Then
            TheFill = Replace(Space(TheLen), " ", TheFill)
        Else
            TheFill = Space(TheLen)
        End If
        CenterIt = Mid(Mid(TheFill, 1, iZ) & TheString & TheFill, 1, TheLen)
    End Function
End Module