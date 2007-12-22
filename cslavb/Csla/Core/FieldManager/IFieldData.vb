Namespace Core.FieldDataManager

  ''' <summary>
  ''' Defines the members required by a field
  ''' data storage object.
  ''' </summary>
  Public Interface IFieldData
    Inherits ITrackStatus

    ''' <summary>
    ''' Gets or sets the field value.
    ''' </summary>
    ''' <value>The value of the field.</value>
    ''' <returns>The value of the field.</returns>
    Property Value() As Object
  End Interface

End Namespace
