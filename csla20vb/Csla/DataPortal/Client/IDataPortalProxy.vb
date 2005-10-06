Namespace DataPortalClient

  Public Interface IDataPortalProxy
    Inherits Server.IDataPortalServer
    ReadOnly Property IsServerRemote() As Boolean
  End Interface

End Namespace
