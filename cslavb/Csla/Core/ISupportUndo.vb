Imports System

Namespace Core

  ''' <summary>
  ''' Define the common methods used by the UI to 
  ''' interact with n-level undo.
  ''' </summary>
  ''' <remarks>
  ''' This interface is designed to help UI framework
  ''' developers interact with editable business objects.
  ''' The CSLA .NET editable base classes already
  ''' implement this interface and the required n-level
  ''' undo behaviors.
  ''' </remarks>
  Public Interface ISupportUndo

    ''' <summary>
    ''' Starts a nested edit on the object.
    ''' </summary>
    Sub BeginEdit()
    ''' <summary>
    ''' Cancels the current edit process, restoring the object's state to
    ''' its previous values.
    ''' </summary>
    Sub CancelEdit()
    ''' <summary>
    ''' Commits the current edit process.
    ''' </summary>
    Sub ApplyEdit()

  End Interface

End Namespace
