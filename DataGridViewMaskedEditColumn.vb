Public Class DataGridViewMaskedEditColumn
    Inherits DataGridViewColumn

    Private pPromptChar As Char = "_"c
    Private pValidatingType As Type = GetType(String)
    Private pMask As String = ""

    Public Sub New()
        MyBase.New(New DataGridViewMaskedEditCell())
    End Sub

    Public Overrides Property CellTemplate() As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(ByVal value As DataGridViewCell)
            ' Ensure that the cell used for the template is a MaskedEditCell
            If Not (value Is Nothing) And Not value.GetType().IsAssignableFrom( _
             GetType(DataGridViewMaskedEditCell)) Then
                Throw New InvalidCastException("Must be a DataGridViewMaskedEditCell")
            End If
            MyBase.CellTemplate = value
        End Set
    End Property

    '
    ' New properties required by the MaskedTextBox control
    '
    Public Property Mask() As String
        Get
            Return pMask
        End Get
        Set(ByVal value As String)
            pMask = value
        End Set
    End Property

    Public Property PromptChar() As Char
        Get
            Return pPromptChar
        End Get
        Set(ByVal value As Char)
            pPromptChar = value
        End Set
    End Property

    Public Property ValidatingType() As Type
        Get
            Return pValidatingType
        End Get
        Set(ByVal value As Type)
            pValidatingType = value
        End Set
    End Property
End Class
