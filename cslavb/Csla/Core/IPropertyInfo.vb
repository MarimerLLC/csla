Namespace Core

  ''' <summary>
  ''' Maintains metadata about a property.
  ''' </summary>
  Public Interface IPropertyInfo

    ''' <summary>
    ''' Gets the property name value.
    ''' </summary>
    ReadOnly Property Name() As String
    ''' <summary>
    ''' Gets the type of the property.
    ''' </summary>
    ReadOnly Property Type() As Type
    ''' <summary>
    ''' Gets the friendly display name
    ''' for the property.
    ''' </summary>
    ReadOnly Property FriendlyName() As String
    ''' <summary>
    ''' Gets the default initial value for the property.
    ''' </summary>
    ''' <remarks>
    ''' This value is used to initialize the property's
    ''' value, and is returned from a property get
    ''' if the user is not authorized to 
    ''' read the property.
    ''' </remarks>
    ReadOnly Property DefaultValue() As Object
    ''' <summary>
    ''' Gets a new field data container for the property.
    ''' </summary>
    Function NewFieldData(ByVal name As String) As Core.FieldManager.IFieldData

  End Interface

End Namespace