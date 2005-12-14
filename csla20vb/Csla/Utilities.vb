Imports System.Reflection

Friend Module Utilities

  ''' <summary>
  ''' Returns a property's type, dealing with
  ''' Nullable(Of T) if necessary.
  ''' </summary>
  Public Function GetPropertyType(ByVal propertyType As Type) As Type

    Dim type As Type = propertyType
    If (type.IsGenericType AndAlso _
      (type.GetGenericTypeDefinition Is GetType(Nullable))) Then

      Return type.GetGenericArguments(0)
    End If

    Return type

  End Function

  ''' <summary>
  ''' Returns the type of child object
  ''' contained in a collection or list.
  ''' </summary>
  Public Function GetChildItemType(ByVal listType As Type) As Type

    If listType.IsArray Then
      Return listType.GetElementType()
    End If
    Dim indexer As DefaultMemberAttribute = _
      CType(Attribute.GetCustomAttribute(listType, _
      GetType(DefaultMemberAttribute)), DefaultMemberAttribute)
    If indexer IsNot Nothing Then
      Dim prop As PropertyInfo = listType.GetProperty(indexer.MemberName)
      Return Utilities.GetPropertyType(prop.PropertyType)

    Else
      Dim props() As PropertyInfo = listType.GetProperties( _
          BindingFlags.FlattenHierarchy Or _
          BindingFlags.Public Or _
          BindingFlags.Instance)
      For Each item As PropertyInfo In props
        If (item.GetIndexParameters.Length > 0) Then
          Return Utilities.GetPropertyType(item.PropertyType)
        End If
      Next
    End If
    Return Nothing

  End Function

End Module
