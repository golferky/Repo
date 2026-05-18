Public Class frmPayDues

    ' ── Input properties ─────────────────────────────────────────────────────
    Private _playerName As String
    Private _isSub As Boolean
    Private _mSkins As Decimal
    Private _mCTP As Decimal
    Private _mLeagueDuesBalance As Decimal
    Private _mEOYBalance As Decimal
    Private _mEOYWeekAmt As Decimal
    Private _showSkins As Boolean
    Private _showCTP As Boolean
    Private _inEOY1 As Boolean
    Private _inEOY2 As Boolean

    ' ── Output properties ────────────────────────────────────────────────────
    Public Property PaySkins As Boolean = False
    Public Property PayCTP As Boolean = False
    Public Property PayLeagueDues As Decimal = 0
    Public Property PayEOY As Decimal = 0
    Public Property EOYWeekComment As String = ""

    Public Sub New(playerName As String,
                   isSub As Boolean,
                   showSkins As Boolean, skinAmt As Decimal,
                   showCTP As Boolean, ctpAmt As Decimal,
                   leagueDuesBalance As Decimal,
                   eoyBalance As Decimal,
                   eoyWeekAmt As Decimal,
                   inEOY1 As Boolean,
                   inEOY2 As Boolean)
        InitializeComponent()
        _playerName = playerName
        _isSub = isSub
        _showSkins = showSkins
        _mSkins = skinAmt
        _showCTP = showCTP
        _mCTP = ctpAmt
        _mLeagueDuesBalance = leagueDuesBalance
        _mEOYBalance = eoyBalance
        _mEOYWeekAmt = eoyWeekAmt
        _inEOY1 = inEOY1
        _inEOY2 = inEOY2

    End Sub

    Private Sub frmPayDues_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Player info
        lblPlayerName.Text = _playerName
        lblPlayerSub.Text = If(_isSub, "Sub Player", "Regular Player")
        lblPlayerSub.ForeColor = If(_isSub, Color.SteelBlue, Color.Gray)

        ' League Dues — regulars only
        pnlLeagueDues.Visible = Not _isSub
        If Not _isSub Then
            lblLeagueDuesBalance.Text = $"Balance: {_mLeagueDuesBalance:C2}"
            nudLeagueDues.Maximum = _mLeagueDuesBalance
            nudLeagueDues.Value = _mLeagueDuesBalance
            If _mLeagueDuesBalance <= 0 Then
                pnlLeagueDues.Enabled = False
                lblLeagueDuesBalance.Text = "Paid ✓"
                lblLeagueDuesBalance.ForeColor = Color.DarkGreen
                nudLeagueDues.Value = 0
            End If
        End If

        ' Skins
        chkSkins.Visible = _showSkins
        lblSkinsAmt.Visible = _showSkins
        If _showSkins Then
            chkSkins.Text = "Skins"
            lblSkinsAmt.Text = $"{_mSkins:C2}"
            chkSkins.Checked = True
        End If

        ' CTP
        chkCTP.Visible = _showCTP
        lblCTPAmt.Visible = _showCTP
        If _showCTP Then
            chkCTP.Text = "CTP"
            lblCTPAmt.Text = $"{_mCTP:C2}"
            chkCTP.Checked = True
        End If

        ' EOY — regulars only
        pnlEOY.Visible = Not _isSub
        ' EOY — regulars only
        pnlEOY.Visible = Not _isSub
        If Not _isSub Then
            nudEOY.Minimum = 0
            nudEOY.Maximum = _mEOYWeekAmt * 2  ' ← add this before SetupEOYRadios
            nudEOY.Value = 0
            SetupEOYRadios()
        End If
        ' Reposition controls if sub
        If _isSub Then
            chkSkins.Location = New Point(20, 80)
            lblSkinsAmt.Location = New Point(200, 82)
            chkCTP.Location = New Point(20, 115)
            lblCTPAmt.Location = New Point(200, 117)
            lblSeparator.Location = New Point(20, 150)
            lblTotal.Location = New Point(20, 160)
            btnPay.Location = New Point(20, 195)
            btnCancel.Location = New Point(200, 195)
            Me.ClientSize = New Size(380, 250)
        End If

        UpdateTotal()
    End Sub

    Private Sub SetupEOYRadios()
        ' Both weeks already paid
        If _inEOY1 AndAlso _inEOY2 Then
            pnlEOY.Enabled = False
            chkEOY.Checked = False
            lblEOYBalance.Text = "Paid ✓"
            lblEOYBalance.ForeColor = Color.DarkGreen
            Return
        End If

        lblEOYBalance.Text = $"Balance: {_mEOYBalance:C2}"
        nudEOY.Maximum = _mEOYBalance

        ' Disable already paid weeks
        radEOYWk1.Enabled = Not _inEOY1
        radEOYWk2.Enabled = Not _inEOY2
        radEOYBoth.Enabled = Not _inEOY1 AndAlso Not _inEOY2

        ' Default selection based on what's already paid
        If _inEOY1 AndAlso Not _inEOY2 Then
            ' Week 1 already paid — default to Week 2
            radEOYWk2.Checked = True
        ElseIf _inEOY2 AndAlso Not _inEOY1 Then
            ' Week 2 already paid — default to Week 1
            radEOYWk1.Checked = True
        Else
            ' Nothing paid — default to Both
            radEOYBoth.Checked = True
        End If

        UpdateEOYAmount()
    End Sub

    Private Sub UpdateEOYAmount()
        If radEOYWk1.Checked OrElse radEOYWk2.Checked Then
            nudEOY.Maximum = _mEOYWeekAmt  ' ← cap to one week
            nudEOY.Value = _mEOYWeekAmt
        ElseIf radEOYBoth.Checked Then
            nudEOY.Maximum = _mEOYWeekAmt * 2  ' ← cap to both weeks
            nudEOY.Value = _mEOYWeekAmt * 2
        End If
    End Sub

    Private Sub radEOYWk1_CheckedChanged(sender As Object, e As EventArgs) Handles radEOYWk1.CheckedChanged
        If radEOYWk1.Checked Then
            EOYWeekComment = "(1st Week)"
            UpdateEOYAmount()
            UpdateTotal()
        End If
    End Sub

    Private Sub radEOYWk2_CheckedChanged(sender As Object, e As EventArgs) Handles radEOYWk2.CheckedChanged
        If radEOYWk2.Checked Then
            EOYWeekComment = "(2nd Week)"
            UpdateEOYAmount()
            UpdateTotal()
        End If
    End Sub

    Private Sub radEOYBoth_CheckedChanged(sender As Object, e As EventArgs) Handles radEOYBoth.CheckedChanged
        If radEOYBoth.Checked Then
            EOYWeekComment = ""
            UpdateEOYAmount()
            UpdateTotal()
        End If
    End Sub

    Private Sub UpdateTotal()
        Dim total As Decimal = 0
        If Not _isSub Then
            If chkLeagueDues.Checked Then total += nudLeagueDues.Value
            If chkEOY.Checked Then total += nudEOY.Value
        End If
        If chkSkins.Visible AndAlso chkSkins.Checked Then total += _mSkins
        If chkCTP.Visible AndAlso chkCTP.Checked Then total += _mCTP
        lblTotal.Text = $"Total: {total:C2}"
        btnPay.Text = $"Pay {total:C2}"
        btnPay.Enabled = total > 0
    End Sub

    Private Sub nudLeagueDues_ValueChanged(sender As Object, e As EventArgs) Handles nudLeagueDues.ValueChanged
        UpdateTotal()
    End Sub

    Private Sub nudEOY_ValueChanged(sender As Object, e As EventArgs) Handles nudEOY.ValueChanged
        UpdateTotal()
    End Sub

    Private Sub chkSkins_CheckedChanged(sender As Object, e As EventArgs) Handles chkSkins.CheckedChanged
        UpdateTotal()
    End Sub

    Private Sub chkCTP_CheckedChanged(sender As Object, e As EventArgs) Handles chkCTP.CheckedChanged
        UpdateTotal()
    End Sub

    Private Sub chkLeagueDues_CheckedChanged(sender As Object, e As EventArgs) Handles chkLeagueDues.CheckedChanged
        nudLeagueDues.Enabled = chkLeagueDues.Checked
        If Not chkLeagueDues.Checked Then nudLeagueDues.Value = 0
        UpdateTotal()
    End Sub

    Private Sub chkEOY_CheckedChanged(sender As Object, e As EventArgs) Handles chkEOY.CheckedChanged
        nudEOY.Enabled = chkEOY.Checked
        radEOYWk1.Enabled = chkEOY.Checked AndAlso Not _inEOY1
        radEOYWk2.Enabled = chkEOY.Checked AndAlso Not _inEOY2
        radEOYBoth.Enabled = chkEOY.Checked AndAlso Not _inEOY1 AndAlso Not _inEOY2
        If Not chkEOY.Checked Then nudEOY.Value = 0
        UpdateTotal()
    End Sub

    Private Sub btnPay_Click(sender As Object, e As EventArgs) Handles btnPay.Click
        PaySkins = chkSkins.Visible AndAlso chkSkins.Checked
        PayCTP = chkCTP.Visible AndAlso chkCTP.Checked
        PayLeagueDues = If(Not _isSub AndAlso chkLeagueDues.Checked, nudLeagueDues.Value, 0)
        PayEOY = If(Not _isSub AndAlso chkEOY.Checked, nudEOY.Value, 0)
        ' Determine EOY week comment from selected radio
        If radEOYWk1.Checked Then
            EOYWeekComment = "(1st Week)"
        ElseIf radEOYWk2.Checked Then
            EOYWeekComment = "(2nd Week)"
        Else
            EOYWeekComment = ""
        End If
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class