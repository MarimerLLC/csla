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

  End Interface

End Namespace