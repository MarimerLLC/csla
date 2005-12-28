Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Namespace DataPortalClient

  Public Class WebServicesProxy

    Implements DataPortalClient.IDataPortalProxy

    Private Function GetPortal() As WebServiceHost.WebServicePortal

      Dim wsvc As New WebServiceHost.WebServicePortal
      wsvc.Url = ApplicationContext.DataPortalUrl.ToString
      Return wsvc

    End Function

    Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Create

      Dim result As Object
      Dim request As New Server.Hosts.WebServicePortal.CreateRequest
      request.ObjectType = objectType
      request.Criteria = criteria
      request.Context = context

      Using wsvc As WebServiceHost.WebServicePortal = GetPortal()
        Dim rd As Byte() = Serialize(request)
        Dim rp As Byte() = wsvc.Create(rd)
        result = Deserialize(rp)
      End Using

      If TypeOf result Is Exception Then
        Throw DirectCast(result, Exception)
      End If
      Return DirectCast(result, Server.DataPortalResult)

    End Function

    Public Function Fetch(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Fetch

      Dim result As Object
      Dim request As New Server.Hosts.WebServicePortal.FetchRequest
      request.Criteria = criteria
      request.Context = context

      Using wsvc As WebServiceHost.WebServicePortal = GetPortal()
        result = Deserialize(wsvc.Fetch(Serialize(request)))
      End Using

      If TypeOf result Is Exception Then
        Throw DirectCast(result, Exception)
      End If
      Return DirectCast(result, Server.DataPortalResult)

    End Function

    Public Function Update(ByVal obj As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Update

      Dim result As Object
      Dim request As New Server.Hosts.WebServicePortal.UpdateRequest
      request.Object = obj
      request.Context = context

      Using wsvc As WebServiceHost.WebServicePortal = GetPortal()
        result = Deserialize(wsvc.Update(Serialize(request)))
      End Using

      If TypeOf result Is Exception Then
        Throw DirectCast(result, Exception)
      End If
      Return DirectCast(result, Server.DataPortalResult)

    End Function

    Public Function Delete(ByVal criteria As Object, ByVal context As Server.DataPortalContext) As Server.DataPortalResult Implements Server.IDataPortalServer.Delete

      Dim result As Object
      Dim request As New Server.Hosts.WebServicePortal.DeleteRequest
      request.Criteria = criteria
      request.Context = context

      Using wsvc As WebServiceHost.WebServicePortal = GetPortal()
        result = Deserialize(wsvc.Delete(Serialize(request)))
      End Using

      If TypeOf result Is Exception Then
        Throw DirectCast(result, Exception)
      End If
      Return DirectCast(result, Server.DataPortalResult)

    End Function

    Public ReadOnly Property IsServerRemote() As Boolean Implements IDataPortalProxy.IsServerRemote
      Get
        Return True
      End Get
    End Property

#Region " Helper functions "

    Private Shared Function Serialize(ByVal obj As Object) As Byte()

      If Not obj Is Nothing Then
        Using buffer As New MemoryStream
          Dim formatter As New BinaryFormatter
          formatter.Serialize(buffer, obj)
          Return buffer.ToArray
        End Using

      Else
        Return Nothing
      End If

    End Function

    Private Shared Function Deserialize(ByVal obj As Byte()) As Object

      If Not obj Is Nothing Then
        Using buffer As New MemoryStream(obj)
          Dim formatter As New BinaryFormatter
          Return formatter.Deserialize(buffer)
        End Using

      Else
        Return Nothing
      End If

    End Function

#End Region

  End Class

End Namespace
