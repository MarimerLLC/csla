Namespace Core

  ''' <summary>
  ''' Specifies that the object is a readonly
  ''' business object.
  ''' </summary>
  Public Interface IReadOnlyObject
    Inherits IBusinessObject

    ''' <summary>
    ''' Returns <see langword="true" /> if the user is allowed to read the
    ''' calling property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property to read.</param>
    Function CanReadProperty(ByVal propertyName As String) As Boolean
  End Interface

End Namespace
