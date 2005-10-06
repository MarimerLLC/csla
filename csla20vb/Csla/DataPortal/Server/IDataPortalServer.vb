Namespace Server

  Public Interface IDataPortalServer
    Function Create(ByVal objectType As Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult
    Function Fetch(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult
    Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult
    Function Delete(ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult
  End Interface

End Namespace
