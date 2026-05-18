Public Class LeagueParms

    Public Property Values As Dictionary(Of String, Object)

    Public Sub New()
        Values = New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
    End Sub

    Public Function HasKey(key As String) As Boolean
        Return Values.ContainsKey(key)
    End Function

    ' Strongly-typed accessors with safe defaults

    Public Function Bool(key As String) As Boolean
        If Not Values.ContainsKey(key) Then Return False
        Dim v = Values(key)
        If v Is Nothing OrElse v Is DBNull.Value Then Return False

        If TypeOf v Is Boolean Then Return CBool(v)

        Dim s = v.ToString().Trim()
        If s.Equals("Y", StringComparison.OrdinalIgnoreCase) Then Return True
        If s.Equals("N", StringComparison.OrdinalIgnoreCase) Then Return False

        Dim b As Boolean
        If Boolean.TryParse(s, b) Then Return b

        Return False
    End Function

    Public Function Int(key As String) As Integer
        If Not Values.ContainsKey(key) Then Return 0
        Dim v = Values(key)
        If v Is Nothing OrElse v Is DBNull.Value Then Return 0

        If TypeOf v Is Integer Then Return CInt(v)
        If TypeOf v Is Double Then Return CInt(v)

        Dim i As Integer
        If Integer.TryParse(v.ToString(), i) Then Return i

        Return 0
    End Function

    Public Function [Double](key As String) As Double
        If Not Values.ContainsKey(key) Then Return 0.0R
        Dim v = Values(key)
        If v Is Nothing OrElse v Is DBNull.Value Then Return 0.0R

        If TypeOf v Is Double Then Return CDbl(v)
        If TypeOf v Is Integer Then Return CDbl(v)

        Dim d As Double
        If Double.TryParse(v.ToString(), d) Then Return d

        Return 0.0R
    End Function

    Public Function [String](key As String) As String
        If Not Values.ContainsKey(key) Then Return ""
        Dim v = Values(key)
        If v Is Nothing OrElse v Is DBNull.Value Then Return ""
        Return v.ToString()
    End Function

    Public Function [Date](key As String) As Date
        If Not Values.ContainsKey(key) Then Return Date.MinValue
        Dim v = Values(key)
        If v Is Nothing OrElse v Is DBNull.Value Then Return Date.MinValue

        If TypeOf v Is Date Then Return CDate(v)

        Dim dt As Date
        If Date.TryParse(v.ToString(), dt) Then Return dt

        Return Date.MinValue
    End Function

    ' Generic getter (if you really want it)
    Public Function GetValue(Of T)(key As String) As T
        If Not Values.ContainsKey(key) Then
            Return Nothing
        End If

        Dim v = Values(key)
        If v Is Nothing OrElse v Is DBNull.Value Then
            Return Nothing
        End If

        Return CType(Convert.ChangeType(v, GetType(T)), T)
    End Function

End Class