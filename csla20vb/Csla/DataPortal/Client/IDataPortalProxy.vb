Namespace DataPortalClient

  ''' <summary>
  ''' Interface implemented by client-side 
  ''' data portal proxy objects.
  ''' </summary>
  Public Interface IDataPortalProxy
    Inherits Server.IDataPortalServer
    ''' <summary>
    ''' Get a value indicating whether this proxy will invoke
    ''' a remote data portal server, or run the "server-side"
    ''' data portal in the caller's process and AppDomain.
    ''' </summary>
    ReadOnly Property IsServerRemote() As Boolean
  End Interface

End Namespace
