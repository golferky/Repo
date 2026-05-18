Public Class LeagueDateService
    Public Shared Function GetFrontBack(selectedDate As String, start9 As String) As String
        Dim frontBack As String = "Front"

        If start9 = "F" Then
            If Not selectedDate.Substring(4, 2) Mod 2 = 0 Then
                frontBack = "Back"
            End If
        Else
            If Not selectedDate.Substring(4, 2) Mod 2 > 0 Then
                frontBack = "Back"
            End If
        End If

        If selectedDate = "20251007" Then
            frontBack = "Back"
        End If

        Return frontBack
    End Function
End Class
