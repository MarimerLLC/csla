Namespace Security

  ''' <summary>
  ''' The access types supported by authorization
  ''' as discussed in Chapter 3.
  ''' </summary>
  Public Enum AccessType
    ''' <summary>
    ''' Roles allowed to read property.
    ''' </summary>
    ReadAllowed
    ''' <summary>
    ''' Roles denied read access to property.
    ''' </summary>
    ReadDenied
    ''' <summary>
    ''' Roles allowed to set property.
    ''' </summary>
    WriteAllowed
    ''' <summary>
    ''' Roles denied write access to property.
    ''' </summary>
    WriteDenied
  End Enum

End Namespace
