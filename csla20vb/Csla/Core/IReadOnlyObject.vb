Namespace Core

  ''' <summary>
  ''' Specifies that the object is a readonly
  ''' business object.
  ''' </summary>
  ''' <remarks></remarks>
  Public Interface IReadOnlyObject
    Inherits IBusinessObject

    Function CanReadProperty(ByVal propertyName As String) As Boolean
  End Interface

End Namespace
