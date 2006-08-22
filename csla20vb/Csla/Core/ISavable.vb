Namespace Core

  ''' <summary>
  ''' Specifies that the object can save
  ''' itself.
  ''' </summary>
  Public Interface ISavable
    ''' <summary>
    ''' Saves the object to the database.
    ''' </summary>
    ''' <returns>A new object containing the saved values.</returns>
    Function Save() As Object
    ''' <summary>
    ''' Event raised when an object has been saved.
    ''' </summary>
    Event Saved As EventHandler(Of SavedEventArgs)
  End Interface

End Namespace
