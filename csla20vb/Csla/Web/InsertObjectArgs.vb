Namespace Web

  ''' <summary>
  ''' Argument object used in the InsertObject event.
  ''' </summary>
  Public Class InsertObjectArgs
    Inherits EventArgs

    Private mValues As System.Collections.IDictionary
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
    ''' Create an instance of the object.
    ''' </summary>
    Public Sub New(ByVal values As System.Collections.IDictionary)

      mValues = values

    End Sub

  End Class

End Namespace