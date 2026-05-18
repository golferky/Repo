Public Module GlobalCommands
    Public Sub LOGIT(sMess As String, Optional bTrbl As Boolean = False)
        ' This points to the code inside your LeagueContext class
        LeagueContext.Instance.mLOGIT(sMess, bTrbl)
    End Sub
End Module