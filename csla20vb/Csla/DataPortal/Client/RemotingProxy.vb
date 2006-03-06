Imports System.Threading
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http

Namespace DataPortalClient

  ''' <summary>
  ''' Implements a data portal proxy to relay data portal
  ''' calls to a remote application server by using the
  ''' .NET Remoting technology.
  ''' </summary>
  Public Class RemotingProxy

    Implements DataPortalClient.IDataPortalProxy

#Region " Configure Remoting "

    ''' <summary>
    ''' Configure .NET Remoting to use a binary
    ''' serialization technology even when using
    ''' the HTTP channel. Also ensures that the
    ''' user's Windows credentials are passed to
    ''' the server appropriately.
    ''' </summary>
    Shared Sub New()

      ' create and register a custom HTTP channel
      ' that uses the binary formatter
      Dim properties As New Hashtable
      properties("name") = "HttpBinary"

      If ApplicationContext.AuthenticationType = "Windows" Then
        ' make sure we pass the user's Windows credentials
        ' to the server
        properties("useDefaultCredentials") = True
      End If

      Dim formatter As New BinaryClientFormatterSinkProvider

      Dim channel As New HttpChannel(properties, formatter, Nothing)

      ChannelServices.RegisterChannel(channel, EncryptChannel)

    End Sub

    Private Shared ReadOnly Property EncryptChannel() As Boolean
      Get
        Dim encrypt As Boolean = _
          (ConfigurationManager.AppSettings("CslaEncryptRemoting") = "true")
        Return encrypt
      End Get
    End Property

#End Region

    Private mPortal As Server.IDataPortalServer

    Private ReadOnly Property Portal() As Server.IDataPortalServer
      Get
        If mPortal Is Nothing Then
          mPortal = CType( _
            Activator.GetObject(GetType(Server.Hosts.RemotingPortal), _
            ApplicationContext.DataPortalUrl.ToString), _
            Server.IDataPortalServer)
        End If
        Return mPortal
      End Get
    End Property

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

      Return Portal.Create(objectType, criteria, context)

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

      Return Portal.Fetch(criteria, context)

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

      Return Portal.Update(obj, context)

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

      Return Portal.Delete(criteria, context)

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

  End Class

End Namespace
