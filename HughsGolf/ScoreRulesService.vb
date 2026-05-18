Public Class ScoreRulesService
    Public Shared Function CalculateGross(holeScores As IEnumerable(Of Object)) As Integer
        Dim gross As Integer = 0

        For Each value In holeScores
            If value IsNot Nothing AndAlso Not IsDBNull(value) AndAlso IsNumeric(value) Then
                gross += CInt(value)
            End If
        Next

        Return gross
    End Function

    Public Shared Function CalculateNet(gross As Integer, handicap As Integer) As Integer
        Return gross - handicap
    End Function

    Public Shared Function CtpCarryoverDetail(slot As Integer, frontBack As String) As String
        Return $"Carryover{slot}-{frontBack}"
    End Function

    Public Shared Function CapScore(score As Decimal,
                                   par As Integer,
                                   par3Max As Decimal,
                                   par4Max As Decimal,
                                   par5Max As Decimal) As Decimal
        Dim maxScore As Decimal

        Select Case par
            Case 3
                maxScore = par3Max
            Case 4
                maxScore = par4Max
            Case 5
                maxScore = par5Max
            Case Else
                maxScore = 99
        End Select

        If score > maxScore Then
            Return maxScore
        End If

        Return score
    End Function
End Class
