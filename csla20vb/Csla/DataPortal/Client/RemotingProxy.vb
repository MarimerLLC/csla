Imports System.Threading
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http

Namespace DataPortalClient

  Public Class RemotingProxy

    Implements DataPortalClient.IDataPortalProxy

#Region " Configure Remoting "

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

    Public Function Create( _
      ByVal objectType As System.Type, ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Create

      Return Portal.Create(objectType, criteria, context)

    End Function

    Public Function Fetch( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Fetch

      Return Portal.Fetch(criteria, context)

    End Function

    Public Function Update( _
      ByVal obj As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Update

      Return Portal.Update(obj, context)

    End Function

    Public Function Delete( _
      ByVal criteria As Object, _
      ByVal context As Server.DataPortalContext) As Server.DataPortalResult _
      Implements Server.IDataPortalServer.Delete

      Return Portal.Delete(criteria, context)

    End Function

    Public ReadOnly Property IsServerRemote() As Boolean _
      Implements IDataPortalProxy.IsServerRemote
      Get
        Return True
      End Get
    End Property

  End Class

End Namespace
