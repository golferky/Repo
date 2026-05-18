Public Class SelectPlayer

    Private _players As DataView
    Private _hole As Integer
    Public Property SelectedPlayer As String = ""
    Dim ohelper As Helper
    Public Property Hole As Integer
        Get
            Return _hole
        End Get
        Set(value As Integer)
            _hole = value
        End Set
    End Property

    ' Constructor that accepts a DataView
    Public Sub New(players As DataView)
        InitializeComponent()
        _players = players
    End Sub

    Private Sub frmSelectPlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim scallingmethod As String = (New Diagnostics.StackTrace).GetFrame(1).GetMethod.Name
        Debug.Print($"Entering {Reflection.MethodBase.GetCurrentMethod.Name} called by {scallingmethod}")

        cbPlayers.DataSource = _players
        cbPlayers.DisplayMember = "Player"
        cbPlayers.ValueMember = "Player"
        Exit Sub
        If MessageBox.Show("Want to pick a CTP winner?",
                           "CTP Winner",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) = DialogResult.Yes Then

            cbPlayers.DataSource = _players
            cbPlayers.DisplayMember = "Player"
            cbPlayers.ValueMember = "Player"
        Else
            Me.Close()
        End If

    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SelectedPlayer = cbPlayers.SelectedValue.ToString
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SelectedPlayer = ""
        DialogResult = DialogResult.Cancel
        Close()

    End Sub
End Class