Namespace Server

  Public Interface IDataPortalServer
    Function Create( _
      ByVal objectType As Type, _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
    Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
    Function Update( _
      ByVal obj As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
    Function Delete( _
      ByVal criteria As Object, _
      ByVal context As DataPortalContext) As DataPortalResult
  End Interface

End Namespace
