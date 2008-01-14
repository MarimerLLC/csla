''' <summary>
''' Provides information about the DataPortal 
''' call.
''' </summary>
Public Class DataPortalEventArgs
  Inherits EventArgs

  Private mDataPortalContext As Server.DataPortalContext
  Private mOperation As DataPortalOperations
  Private mException As Exception

  ''' <summary>
  ''' The DataPortalContext object passed to the
  ''' server-side DataPortal.
  ''' </summary>
  Public ReadOnly Property DataPortalContext() As Server.DataPortalContext
    Get
      Return mDataPortalContext
    End Get
  End Property

  ''' <summary>
  ''' Gets the requested data portal operation.
  ''' </summary>
  Public ReadOnly Property Operation() As DataPortalOperations
    Get
      Return mOperation
    End Get
  End Property

  ''' <summary>
  ''' Gets a reference to any exception that occurred
  ''' during the data portal call.
  ''' </summary>
  ''' <remarks>
  ''' This property will return Nothing (null in C#) if no
  ''' exception occurred. Exceptions are returned only as part
  ''' of a data portal complete event or method.
  ''' </remarks>
  Public ReadOnly Property Exception() As Exception
    Get
      Return mException
    End Get
  End Property

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="dataPortalContext">
  ''' Data portal context object.
  ''' </param>
  ''' <param name="operation">
  ''' Data portal operation being performed.
  ''' </param>
  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext, ByVal operation As DataPortalOperations)
    mDataPortalContext = dataPortalContext
    mOperation = operation
  End Sub

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="dataPortalContext">
  ''' Data portal context object.
  ''' </param>
  ''' <param name="operation">
  ''' Data portal operation being performed.
  ''' </param>
  ''' <param name="exception">
  ''' Exception encountered during processing.
  ''' </param>
  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext, _
                 ByVal operation As DataPortalOperations, _
                 ByVal exception As Exception)
    Me.New(dataPortalContext, operation)
    mException = exception
  End Sub

End Class