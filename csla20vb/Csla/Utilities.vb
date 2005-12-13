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

End Module
