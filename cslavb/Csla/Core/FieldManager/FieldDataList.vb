Namespace Core.FieldManager

  <Serializable()> _
  Friend Class FieldDataList

    Implements ISerializable

    <NonSerialized()> _
    Private mFieldIndex As New Dictionary(Of String, Integer)
    Private mFields As New List(Of IFieldData)

    Public Sub New()

    End Sub

    Public Function ContainsKey(ByVal key As String) As Boolean
      Return mFieldIndex.ContainsKey(key)
    End Function

    Public Function GetValue(ByVal key As String) As IFieldData

      Return mFields(mFieldIndex(key))

    End Function

    Public Sub Add(ByVal key As String, ByVal value As IFieldData)

      mFields.Add(value)
      mFieldIndex.Add(key, mFields.Count - 1)

    End Sub

    Friend Function FindPropertyName(ByVal value As Object) As String

      For Each item In mFields
        If ReferenceEquals(item.Value, value) Then
          Return item.Name
        End If
      Next
      Return Nothing

    End Function

    Public Function GetFieldDataList() As List(Of IFieldData)

      Return mFields

    End Function

#Region " ISerializable "

    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      mFields = DirectCast(info.GetValue("Fields", GetType(List(Of IFieldData))), List(Of IFieldData))
      RebuildIndex()

    End Sub

    Protected Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) _
      Implements System.Runtime.Serialization.ISerializable.GetObjectData

      info.AddValue("Fields", mFields)

    End Sub

    Private Sub RebuildIndex()

      Dim position = 0
      For Each item As IFieldData In mFields
        mFieldIndex.Add(item.Name, position)
        position += 1
      Next

    End Sub

#End Region

  End Class

End Namespace
