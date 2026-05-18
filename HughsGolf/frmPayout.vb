Public Class frmPayout

    Private _playerName As String
    Private _category As String
    Private _suggestedAmount As Decimal

    Public Property PayoutAmount As Decimal = 0

    Public Sub New(playerName As String, category As String, Optional suggestedAmount As Decimal = 0)
        InitializeComponent()
        _playerName = playerName
        _category = category
        _suggestedAmount = suggestedAmount
    End Sub

    Private Sub frmPayout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblPlayer.Text = _playerName
        lblCategory.Text = _category
        nudAmount.Minimum = 0
        nudAmount.Maximum = 9999
        nudAmount.DecimalPlaces = 2
        nudAmount.Value = _suggestedAmount
        UpdateButton()
    End Sub

    Private Sub nudAmount_ValueChanged(sender As Object, e As EventArgs) Handles nudAmount.ValueChanged
        UpdateButton()
    End Sub

    Private Sub UpdateButton()
        btnPayout.Enabled = nudAmount.Value > 0
        btnPayout.Text = $"Record Payout {nudAmount.Value:C2}"
    End Sub

    Private Sub btnPayout_Click(sender As Object, e As EventArgs) Handles btnPayout.Click
        PayoutAmount = nudAmount.Value
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class