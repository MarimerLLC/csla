Imports System
Imports System.Collections.Generic
Imports System.Runtime.Serialization

Namespace Core.FieldManager

  <Serializable()> _
  Friend Class FieldDataList

    Implements ISerializable

    <NonSerialized()> _
    Private _fieldIndex As New Dictionary(Of String, Integer)
    Private _fields As New List(Of IFieldData)

    Public Sub New()
      ' required due to serialization ctor
    End Sub

    Public Function TryGetValue(ByVal key As String, ByRef result As IFieldData) As Boolean

      Dim index As Integer
      If _fieldIndex.TryGetValue(key, index) Then
        result = _fields(index)
        Return True

      Else
        result = Nothing
        Return False
      End If

    End Function

    Public Function ContainsKey(ByVal key As String) As Boolean
      Return _fieldIndex.ContainsKey(key)
    End Function

    Public Function GetValue(ByVal key As String) As IFieldData

      Return _fields(_fieldIndex(key))

    End Function

    Public Sub Add(ByVal key As String, ByVal value As IFieldData)

      _fields.Add(value)
      _fieldIndex.Add(key, _fields.Count - 1)

    End Sub

    Friend Function FindPropertyName(ByVal value As Object) As String

      For Each item In _fields
        If ReferenceEquals(item.Value, value) Then
          Return item.Name
        End If
      Next
      Return Nothing

    End Function

    Public Function GetFieldDataList() As List(Of IFieldData)

      Return _fields

    End Function

#Region " ISerializable "

    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

      _fields = DirectCast(info.GetValue("Fields", GetType(List(Of IFieldData))), List(Of IFieldData))
      RebuildIndex()

    End Sub

    Protected Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) _
      Implements System.Runtime.Serialization.ISerializable.GetObjectData

      info.AddValue("Fields", _fields)

    End Sub

    Private Sub RebuildIndex()

      Dim position = 0
      For Each item As IFieldData In _fields
        _fieldIndex.Add(item.Name, position)
        position += 1
      Next

    End Sub

#End Region

  End Class

End Namespace
