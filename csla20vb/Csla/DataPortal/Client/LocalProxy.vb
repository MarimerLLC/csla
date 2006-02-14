Namespace DataPortalClient

  Public Class LocalProxy

    Implements DataPortalClient.IDataPortalProxy

    Private mPortal As Server.IDataPortalServer = _
      New Server.DataPortal

    Public Function Create( _
      ByVal objectType As System.Type, ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Return mPortal.Create(objectType, criteria, context)

    End Function

    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Return mPortal.Fetch(criteria, context)

    End Function

    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Return mPortal.Update(obj, context)

    End Function

    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Return mPortal.Delete(criteria, context)

    End Function

    Public ReadOnly Property IsServerRemote() As Boolean _
      Implements IDataPortalProxy.IsServerRemote
      Get
        Return False
      End Get
    End Property

  End Class

End Namespace
