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
    ''' INTERNAL CSLA .NET USE ONLY.
    ''' </summary>
    ''' <param name="newObject">
    ''' The new object returned as a result of the save.</param>
    Sub SaveComplete(ByVal newObject As Object)
    ''' <summary>
    ''' Event raised when an object has been saved.
    ''' </summary>
    Event Saved As EventHandler(Of SavedEventArgs)
  End Interface

End Namespace
