Namespace Wpf

  ''' <summary>
  ''' Options controlling how the Authorization
  ''' control alters the visibility of a control
  ''' when read access to the property is not allowed.
  ''' </summary>
  Public Enum VisibilityMode
    ''' <summary>
    ''' Specifies that the non-readable control
    ''' should be collapsed.
    ''' </summary>
    Collapsed
    ''' <summary>
    ''' Specifies that the non-readable control
    ''' should be hidden.
    ''' </summary>
    Hidden
    ''' <summary>
    ''' Specifies that the visibility of the 
    ''' non-readable control should not be
    ''' altered by the Authorizer control.
    ''' </summary>
    Ignore
  End Enum

End Namespace