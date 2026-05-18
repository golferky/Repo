Friend Module DataReaderExtensions
    Friend Function ToList(Of T)(reader As IDataReader, columnIndex As Integer) As List(Of T)
        Dim list As New List(Of T)
        While reader.Read()
            list.Add(If(reader.IsDBNull(columnIndex), Nothing, CTypeDynamic(Of T)(reader.GetValue(columnIndex))))
        End While
        Return list
    End Function
End Module