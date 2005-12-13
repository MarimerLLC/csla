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
    Dim props() As PropertyInfo = listType.GetProperties( _
        BindingFlags.FlattenHierarchy Or _
        BindingFlags.Public Or _
        BindingFlags.Instance)
    For Each item As PropertyInfo In props
      If Attribute.IsDefined(item, GetType(DefaultMemberAttribute)) Then
        Return Utilities.GetPropertyType(item.PropertyType)
      End If
    Next
    Return Nothing

  End Function

End Module
