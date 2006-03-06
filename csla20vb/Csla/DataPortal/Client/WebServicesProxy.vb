Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Namespace DataPortalClient

  ''' <summary>
  ''' Implements a data portal proxy to relay data portal
  ''' calls to a remote application server by using 
  ''' Web services.
  ''' </summary>
  Public Class WebServicesProxy

    Implements DataPortalClient.IDataPortalProxy

    Private Function GetPortal() As WebServiceHost.WebServicePortal

      Dim wsvc As New WebServiceHost.WebServicePortal
      wsvc.Url = ApplicationContext.DataPortalUrl.ToString
      Return wsvc

    End Function

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to create a
    ''' new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Create( _
      ByVal objectType As System.Type, ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

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

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to load an
    ''' existing business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

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

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to update a
    ''' business object.
    ''' </summary>
    ''' <param name="obj">The business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

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

    ''' <summary>
    ''' Called by <see cref="DataPortal" /> to delete a
    ''' business object.
    ''' </summary>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

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

    ''' <summary>
    ''' Get a value indicating whether this proxy will invoke
    ''' a remote data portal server, or run the "server-side"
    ''' data portal in the caller's process and AppDomain.
    ''' </summary>
    Public ReadOnly Property IsServerRemote() As Boolean _
      Implements IDataPortalProxy.IsServerRemote
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
