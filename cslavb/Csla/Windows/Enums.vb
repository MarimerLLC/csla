Namespace Windows

  ''' <summary>
  ''' The possible form actions.
  ''' </summary>
  Public Enum CslaFormAction
    ''' <summary>
    ''' No action.
    ''' </summary>
    None
    ''' <summary>
    ''' Perform a save.
    ''' </summary>
    Save
    ''' <summary>
    ''' Perform a cancel.
    ''' </summary>
    Cancel
    ''' <summary>
    ''' Close the form.
    ''' </summary>
    Close
  End Enum

  ''' <summary>
  ''' The possible options for posting
  ''' and saving.
  ''' </summary>
  Public Enum PostSaveActionType
    ''' <summary>
    ''' No action.
    ''' </summary>
    None
    ''' <summary>
    ''' Also close the form.
    ''' </summary>
    AndClose
    ''' <summary>
    ''' Also create a new object.
    ''' </summary>
    AndNew
  End Enum

End Namespace
