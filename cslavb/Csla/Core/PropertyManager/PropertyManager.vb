Namespace Core.PropertyManager

  ''' <summary>
  ''' Manages properties and property data for
  ''' a business object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class PropertyManager

    Private mFields As Dictionary(Of String, Object)
    Private mChildren As Dictionary(Of String, IBusinessObject)

    Friend Sub New()

      ' prevent creation from outside this assembly

    End Sub

    Friend ReadOnly Property FieldValues() As Dictionary(Of String, Object)
      Get
        If mFields Is Nothing Then
          mFields = New Dictionary(Of String, Object)
        End If
        Return mFields
      End Get
    End Property

    Friend ReadOnly Property ChildValues() As Dictionary(Of String, IBusinessObject)
      Get
        If mChildren Is Nothing Then
          mChildren = New Dictionary(Of String, IBusinessObject)
        End If
        Return mChildren
      End Get
    End Property

#Region " Child management "

    Public Function ChildrenValid() As Boolean

      For Each item As KeyValuePair(Of String, IBusinessObject) In ChildValues
        Dim list As IEditableCollection = TryCast(item.Value, IEditableCollection)
        If list IsNot Nothing Then
          If Not list.IsValid Then
            Return False
          End If

        Else
          Dim obj As IEditableBusinessObject = TryCast(item.Value, IEditableBusinessObject)
          If obj IsNot Nothing Then
            If Not obj.IsValid Then
              Return False
            End If
          End If
        End If
      Next
      Return True

    End Function

    Public Function ChildrenDirty() As Boolean

      For Each item As KeyValuePair(Of String, IBusinessObject) In ChildValues
        Dim list As IEditableCollection = TryCast(item.Value, IEditableCollection)
        If list IsNot Nothing Then
          If list.IsDirty Then
            Return True
          End If

        Else
          Dim obj As IEditableBusinessObject = TryCast(item.Value, IEditableBusinessObject)
          If obj IsNot Nothing Then
            If obj.IsDirty Then
              Return True
            End If
          End If
        End If
      Next
      Return False

    End Function

    Public Function PropertyFieldExists(ByVal propertyInfo As IPropertyInfo) As Boolean

      Return ChildValues.ContainsKey(propertyInfo.Name) OrElse FieldValues.ContainsKey(propertyInfo.Name)

    End Function

#End Region

  End Class

End Namespace
