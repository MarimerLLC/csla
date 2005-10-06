Namespace Server.Hosts

  ''' <summary>
  ''' Exposes server-side DataPortal functionality
  ''' through .NET Remoting.
  ''' </summary>
  Public Class RemotingPortal
    Inherits MarshalByRefObject

    Implements Server.IDataPortalServer

    Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create

      Dim portal As New Server.DataPortal
      Return portal.Create(objectType, criteria, context)

    End Function

    Public Function Fetch(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Fetch

      Dim portal As New Server.DataPortal
      Return portal.Fetch(criteria, context)

    End Function

    Public Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update

      Dim portal As New Server.DataPortal
      Return portal.Update(obj, context)

    End Function

    Public Function Delete(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete

      Dim portal As New Server.DataPortal
      Return portal.Delete(criteria, context)

    End Function

  End Class

End Namespace
