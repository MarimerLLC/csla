Namespace Core.FieldManager

  ''' <summary>
  ''' Defines the members required by a field
  ''' data storage object.
  ''' </summary>
  Public Interface IFieldData(Of T)
    Inherits IFieldData

    ''' <summary>
    ''' Gets or sets the field value.
    ''' </summary>
    ''' <value>The value of the field.</value>
    ''' <returns>The value of the field.</returns>
    Overloads Property Value() As T
  End Interface

End Namespace

