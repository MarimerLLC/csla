Namespace Core.FieldDataManager

  ''' <summary>
  ''' Manages properties and property data for
  ''' a business object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class FieldDataManager

    Friend Sub New()

      ' prevent creation from outside this assembly

    End Sub

    Private mFields As Dictionary(Of String, IFieldData)
    Private ReadOnly Property FieldData() As Dictionary(Of String, IFieldData)
      Get
        If mFields Is Nothing Then
          mFields = New Dictionary(Of String, IFieldData)
        End If
        Return mFields
      End Get
    End Property

    Protected Friend Function GetFieldData(ByVal prop As IPropertyInfo) As IFieldData

      If FieldData.ContainsKey(prop.Name) Then
        Return FieldData(prop.Name)

      Else
        Return Nothing
      End If

    End Function

    Protected Friend Function FindPropertyName(ByVal value As Object) As String

      For Each item In FieldData
        If ReferenceEquals(item.Value.Value, value) Then
          Return item.Key
        End If
      Next
      Return Nothing

    End Function

    Protected Friend Sub SetFieldData(ByVal prop As IPropertyInfo, ByVal value As Object)

      If Not FieldData.ContainsKey(prop.Name) Then
        FieldData.Add(prop.Name, prop.NewFieldData)
      End If
      FieldData(prop.Name).Value = value

    End Sub

    Protected Friend Sub RemoveField(ByVal propertyName As String)
      FieldData.Remove(propertyName)
    End Sub

    Public Function IsValid() As Boolean

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        If item.Value IsNot Nothing AndAlso Not item.Value.IsValid Then
          Return False
        End If
      Next
      Return True

    End Function

    Public Function IsDirty() As Boolean

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        If item.Value IsNot Nothing AndAlso item.Value.IsDirty Then
          Return True
        End If
      Next
      Return False

    End Function

    Public Function FieldExists(ByVal propertyInfo As IPropertyInfo) As Boolean

      Return FieldData.ContainsKey(propertyInfo.Name)

    End Function

  End Class

End Namespace
