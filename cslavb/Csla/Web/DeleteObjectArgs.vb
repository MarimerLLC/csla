Namespace Web

  ''' <summary>
  ''' Argument object used in the DeleteObject event.
  ''' </summary>
  Public Class DeleteObjectArgs
    Inherits EventArgs

    Private mKeys As System.Collections.IDictionary
    Private mOldValues As System.Collections.IDictionary
    Private mRowsAffected As Integer

    ''' <summary>
    ''' Gets or sets the number of rows affected
    ''' while handling this event.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' The code handling the event should set this
    ''' value to indicate the number of rows affected
    ''' by the operation.
    ''' </remarks>
    Public Property RowsAffected() As Integer
      Get
        Return mRowsAffected
      End Get
      Set(ByVal value As Integer)
        mRowsAffected = value
      End Set
    End Property

    ''' <summary>
    ''' The list of key values entered by the user.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to use the values to identify the 
    ''' object to be deleted.</remarks>
    Public ReadOnly Property Keys() As System.Collections.IDictionary
      Get
        Return mKeys
      End Get
    End Property

    ''' <summary>
    ''' The list of old data values maintained by
    ''' data binding.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to use the values to identify the 
    ''' object to be deleted.</remarks>
    Public ReadOnly Property OldValues() As System.Collections.IDictionary
      Get
        Return mOldValues
      End Get
    End Property

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    Public Sub New(ByVal keys As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary)

      mKeys = keys
      mOldValues = oldValues

    End Sub

  End Class

End Namespace