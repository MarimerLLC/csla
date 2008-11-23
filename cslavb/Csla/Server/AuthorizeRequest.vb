Namespace Server

  ''' <summary>
  ''' Object containing information about the
  ''' client request to the data portal.
  ''' </summary>
  Public Class AuthorizeRequest

    Private _objectType As Type
    ''' <summary>
    ''' Gets the type of business object affected by
    ''' the client request.
    ''' </summary>
    Public Property ObjectType() As Type
      Get
        Return _objectType
      End Get
      Set(ByVal value As Type)
        _objectType = value
      End Set
    End Property

    Private _requestObject As Object
    ''' <summary>
    ''' Gets a reference to the criteria or 
    ''' business object passed from
    ''' the client to the server.
    ''' </summary>
    Public Property RequestObject() As Object
      Get
        Return _requestObject
      End Get
      Set(ByVal value As Object)
        _requestObject = value
      End Set
    End Property

    Private _operation As DataPortalOperations
    ''' <summary>
    ''' Gets the data portal operation requested
    ''' by the client.
    ''' </summary>
    Public Property Operation() As DataPortalOperations
      Get
        Return _operation
      End Get
      Private Set(ByVal value As DataPortalOperations)
        _operation = value
      End Set
    End Property

    Friend Sub New(ByVal objectType As Type, ByVal requestObject As Object, ByRef operation As DataPortalOperations)
      Me.ObjectType = objectType
      Me.RequestObject = requestObject
      Me.Operation = operation
    End Sub

  End Class

End Namespace

