Namespace Web

  ''' <summary>
  ''' Argument object used in the UpdateObject event.
  ''' </summary>
  Public Class UpdateObjectArgs
    Inherits EventArgs

    Private mKeys As System.Collections.IDictionary
    Private mValues As System.Collections.IDictionary
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
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property Keys() As System.Collections.IDictionary
      Get
        Return mKeys
      End Get
    End Property

    ''' <summary>
    ''' The list of data values entered by the user.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property Values() As System.Collections.IDictionary
      Get
        Return mValues
      End Get
    End Property

    ''' <summary>
    ''' The list of old data values maintained by
    ''' data binding.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property OldValues() As System.Collections.IDictionary
      Get
        Return mOldValues
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New(ByVal keys As System.Collections.IDictionary, ByVal values As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary)

      mKeys = keys
      mValues = values
      mOldValues = oldValues

    End Sub

  End Class

End Namespace
