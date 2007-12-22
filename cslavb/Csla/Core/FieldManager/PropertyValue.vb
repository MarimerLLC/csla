Namespace Core.FieldDataManager

  ''' <summary>
  ''' Indicates how a property's field
  ''' is stored.
  ''' </summary>
  Public Enum PropertyValue
    ''' <summary>
    ''' Indicates a property's value will
    ''' be stored in a field defined by
    ''' the business object.
    ''' </summary>
    Field
    ''' <summary>
    ''' Indicates a property's value will
    ''' be managed by the business object's
    ''' PropertyManager object.
    ''' </summary>
    Managed
  End Enum

End Namespace