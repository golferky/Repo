Public Class FileLayout
    Enum FieldType
        Key = 0
        Comp = 1
        Sum = 2
        Control = 3
        Ignore = 4
        fmtDate = 5
    End Enum
    Protected _eFieldType As FieldType
    Protected _sInputTable As String
    Protected _sInputField As String
    Protected _sOutputtable As String
    Protected _sOutputField As String
    Protected _sOutputValue As String
    Protected _sDataField As String
    Protected _iDataPos As Integer
    Protected _iDataLen As Integer
    Protected _tDataType As Type

    Sub New(ByVal sDataDField As String, iDataPos As Integer, iDataLen As Integer, tDataType As Type, efieldType As FieldType)
        _sDataField = sDataDField
        _iDataPos = iDataPos
        _iDataLen = iDataLen
        _eFieldType = efieldType
    End Sub
    'Sub New(ByVal sInputTable As String, sInputField As String, sOutputTable As String, sOutputField As String, Optional sFieldType As FieldType = FieldType.Ignore)
    '    _sInputTable = sInputTable
    '    _sInputField = sInputField
    '    _sOutputField = eOutputField
    '    _sOutputtable = eOutputTable
    '    _sOutputValue = eOutputValue
    '    _eFieldType = eFieldType
    'End Sub
    ReadOnly Property eFieldType() As FieldType
        Get
            Return _eFieldType
        End Get
    End Property
    ReadOnly Property eInputTable() As String
        Get
            Return _sInputTable
        End Get
    End Property
    ReadOnly Property eInputField() As String
        Get
            Return _sInputField
        End Get
    End Property
    ReadOnly Property eOutputTable() As String
        Get
            Return _sOutputtable
        End Get
    End Property
    ReadOnly Property eOutputField() As String
        Get
            Return _sOutputField
        End Get
    End Property
    ReadOnly Property eOutputValue() As String
        Get
            Return _sOutputValue
        End Get
    End Property
    ReadOnly Property sDataField() As String
        Get
            Return _sDataField
        End Get
    End Property
    ReadOnly Property iDataPos As Integer
        Get
            Return _iDataPos
        End Get
    End Property
    ReadOnly Property iDataLen As Integer
        Get
            Return _iDataLen
        End Get
    End Property
    ReadOnly Property tDataType As Type
        Get
            Return _tDataType
        End Get
    End Property

#Enable Warning
End Class
