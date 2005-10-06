Namespace DataPortalClient

  Public Class LocalProxy

    Implements DataPortalClient.IDataPortalProxy

    Private mPortal As Server.IDataPortalServer

    Private ReadOnly Property Portal() As Server.IDataPortalServer
      Get
        If mPortal Is Nothing Then
          mPortal = New Server.DataPortal
        End If
        Return mPortal
      End Get
    End Property

    Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create

      Return Portal.Create(objectType, criteria, context)

    End Function

    Public Function Fetch(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Fetch

      Return Portal.Fetch(criteria, context)

    End Function

    Public Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update

      Return Portal.Update(obj, context)

    End Function

    Public Function Delete(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete

      Return Portal.Delete(criteria, context)

    End Function

    Public ReadOnly Property IsServerRemote() As Boolean Implements IDataPortalProxy.IsServerRemote
      Get
        Return False
      End Get
    End Property

  End Class

End Namespace
