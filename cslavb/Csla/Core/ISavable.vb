Imports System

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
    ''' Saves the object to the database asynchronously. The saved event will contain
    ''' the new object when the save operation completes.
    ''' </summary>
    sub BeginSave();
    ''' <summary>
    ''' Saves the object to the database asynchronously. The saved event will contain
    ''' the new object when the save operation completes.
    ''' </summary>
    ''' <param name="userState">
    ''' User state object.
    ''' </param>
    sub BeginSave(ByVal userState As Object );
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
