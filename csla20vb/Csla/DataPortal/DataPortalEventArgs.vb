''' <summary>
''' Provides information about the DataPortal 
''' call.
''' </summary>
Public Class DataPortalEventArgs
  Inherits EventArgs

  Private mDataPortalContext As Server.DataPortalContext

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
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <param name="dataPortalContext">
  ''' Data portal context object.
  ''' </param>
  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext)
    mDataPortalContext = dataPortalContext
  End Sub

End Class