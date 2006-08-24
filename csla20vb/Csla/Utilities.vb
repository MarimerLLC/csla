Imports System.Reflection

''' <summary>
''' Contains utility methods used by the
''' CSLA .NET framework.
''' </summary>
Public Module Utilities

  ''' <summary>
  ''' Returns a property's type, dealing with
  ''' Nullable(Of T) if necessary.
  ''' </summary>
  ''' <param name="propertyType">Type of the
  ''' property as returned by reflection.</param>
  Public Function GetPropertyType(ByVal propertyType As Type) As Type

    Dim type As Type = propertyType
    If (type.IsGenericType AndAlso _
      (type.GetGenericTypeDefinition Is GetType(Nullable(Of )))) Then

      Return Nullable.GetUnderlyingType(type)
    End If

    Return type

  End Function

  ''' <summary>
  ''' Returns the type of child object
  ''' contained in a collection or list.
  ''' </summary>
  ''' <param name="listType">Type of the list.</param>
  Public Function GetChildItemType(ByVal listType As Type) As Type

    Dim result As Type = Nothing
    If listType.IsArray Then
      result = listType.GetElementType()
    Else
      Dim indexer As DefaultMemberAttribute = _
        CType(Attribute.GetCustomAttribute(listType, _
        GetType(DefaultMemberAttribute)), DefaultMemberAttribute)
      If indexer IsNot Nothing Then
        For Each prop As PropertyInfo In listType.GetProperties( _
          BindingFlags.Public Or _
          BindingFlags.Instance Or _
          BindingFlags.FlattenHierarchy)
          If prop.Name = indexer.MemberName Then
            result = Utilities.GetPropertyType(prop.PropertyType)
          End If
        Next
      End If
    End If
    Return result

  End Function

End Module
