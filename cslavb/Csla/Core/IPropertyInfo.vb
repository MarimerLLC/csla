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
    ''' <summary>
    ''' Gets the relationship between the declaring object
    ''' and the object reference in the property.
    ''' </summary>
    ReadOnly Property RelationshipType() As RelationshipTypes
    ''' <summary>
    ''' Gets or sets the index position for the managed
    ''' field storage behind the property. FOR
    ''' INTERNAL CSLA .NET USE ONLY.
    ''' </summary>
    Property Index() As Integer
  End Interface

End Namespace