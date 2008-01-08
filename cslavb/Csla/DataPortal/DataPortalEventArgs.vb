''' <summary>
''' Provides information about the DataPortal 
''' call.
''' </summary>
Public Class DataPortalEventArgs
  Inherits EventArgs

  Private mDataPortalContext As Server.DataPortalContext
  Private mOperation As DataPortalOperations

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
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="dataPortalContext">
  ''' Data portal context object.
  ''' </param>
  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext, ByVal operation As DataPortalOperations)
    mDataPortalContext = dataPortalContext
    mOperation = operation
  End Sub

End Class