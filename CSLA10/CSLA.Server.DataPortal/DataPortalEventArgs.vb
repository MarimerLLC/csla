''' <summary>
''' Provides information about the DataPortal 
''' call.
''' </summary>
Public Class DataPortalEventArgs
  Inherits EventArgs

  ''' <summary>
  ''' The DataPortalContext object passed to the
  ''' server-side DataPortal.
  ''' </summary>
  Public DataPortalContext As Server.DataPortalContext

  Public Sub New(ByVal dataPortalContext As Server.DataPortalContext)
    Me.DataPortalContext = dataPortalContext
  End Sub

End Class