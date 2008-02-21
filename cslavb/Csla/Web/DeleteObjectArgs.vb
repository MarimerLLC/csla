Namespace Web

  ''' <summary>
  ''' Argument object used in the DeleteObject event.
  ''' </summary>
  Public Class DeleteObjectArgs
    Inherits EventArgs

    Private _keys As System.Collections.IDictionary
    Private _oldValues As System.Collections.IDictionary
    Private _rowsAffected As Integer

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
        Return _rowsAffected
      End Get
      Set(ByVal value As Integer)
        _rowsAffected = value
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
        Return _keys
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
        Return _oldValues
      End Get
    End Property

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    Public Sub New(ByVal keys As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary)

      _keys = keys
      _oldValues = oldValues

    End Sub

  End Class

End Namespace