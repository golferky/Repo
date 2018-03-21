Class MaskedEditEditingControl
    Inherits MaskedTextBox
    Implements IDataGridViewEditingControl

    Private dataGridViewControl As DataGridView
    Private valueIsChanged As Boolean = False
    Private rowIndexNum As Integer

    Public Sub New()

    End Sub

    Public Property EditingControlFormattedValue() As Object Implements _
     IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return Me.valueIsChanged.ToString
        End Get
        Set(ByVal value As Object)
            If TypeOf value Is [String] Then
                Me.Text = value.ToString
            End If
        End Set
    End Property

    Public Function GetEditingControlFormattedValue(ByVal context As  _
     DataGridViewDataErrorContexts) As Object Implements _
     IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return Me.Text
    End Function

    Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As  _
    DataGridViewCellStyle) Implements _
    IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        Me.Font = dataGridViewCellStyle.Font
        Me.ForeColor = dataGridViewCellStyle.ForeColor
        Me.BackColor = dataGridViewCellStyle.BackColor
    End Sub

    Public Property EditingControlRowIndex() As Integer Implements _
    IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return rowIndexNum
        End Get
        Set(ByVal value As Integer)
            rowIndexNum = value
        End Set
    End Property

    Public Function EditingControlWantsInputKey(ByVal key As Keys, ByVal _
     dataGridViewWantsInputKey As Boolean) As Boolean Implements _
     IDataGridViewEditingControl.EditingControlWantsInputKey
        Return True
    End Function

    Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements _
     IDataGridViewEditingControl.PrepareEditingControlForEdit
        ' No preparation needs to be done.
    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean _
     Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

    Public Property EditingControlDataGridView() As DataGridView Implements _
     IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return dataGridViewControl
        End Get
        Set(ByVal value As DataGridView)
            dataGridViewControl = value
        End Set
    End Property

    Public Property EditingControlValueChanged() As Boolean Implements _
     IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return valueIsChanged
        End Get
        Set(ByVal value As Boolean)
            valueIsChanged = value
        End Set
    End Property

    Public ReadOnly Property EditingPanelCursor() As Cursor Implements _
     IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return MyBase.Cursor
        End Get
    End Property

    Protected Overrides Sub OnTextChanged(ByVal eventargs As EventArgs)
        ' Notify the DataGridView that the contents of the cell have changed.
        valueIsChanged = True
        Me.EditingControlDataGridView.NotifyCurrentCellDirty(True)
        MyBase.OnTextChanged(eventargs)
    End Sub
End Class