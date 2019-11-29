Public Class DataGridViewMaskedEditCell

    Inherits DataGridViewTextBoxCell

    Dim pColumn As DataGridViewMaskedEditColumn

    Public Sub New()

    End Sub

    Public Overrides Sub InitializeEditingControl(ByVal rowIndex As Integer, ByVal _
     initialFormattedValue As Object, ByVal dataGridViewCellStyle As DataGridViewCellStyle)
        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)

        pColumn = CType(Me.OwningColumn, DataGridViewMaskedEditColumn)
        Dim ctl As MaskedEditEditingControl = CType(DataGridView.EditingControl,  _
         MaskedEditEditingControl)

        ' copy over the properties of the column
        If Not IsNothing(Me.Value) Then
            ctl.Text = Me.Value.ToString
        Else
            ctl.Text = ""
        End If
        ctl.ValidatingType = pColumn.ValidatingType
        ctl.Mask = pColumn.Mask
        ctl.PromptChar = pColumn.PromptChar
    End Sub

    Public Overrides ReadOnly Property EditType() As Type
        Get
            ' Return the type of the editing contol that MaskedEditEditingControl uses.
            Return GetType(MaskedEditEditingControl)
        End Get
    End Property


    Public Overrides ReadOnly Property ValueType() As Type
        Get
            ' Return the type of the value that MaskedEditEditingControl contains.
            Return pColumn.ValidatingType
        End Get
    End Property


    Public Overrides ReadOnly Property DefaultNewRowValue() As Object
        Get
            Return ""
        End Get
    End Property

End Class
