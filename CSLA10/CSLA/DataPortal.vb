Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Configuration

Public Class DataPortal
  Private Shared mPortal As Server.DataPortal
  Private Shared mServicedPortal As Server.ServicedDataPortal.DataPortal

#Region " Data Access methods "

  Public Shared Function Create(ByVal Criteria As Object) As Object
    If IsTransactionalMethod(GetMethod(Criteria.GetType.DeclaringType, "DataPortal_Create")) Then
      Return ServicedPortal.Create(Criteria, GetPrincipal)
    Else
      Return Portal.Create(Criteria, GetPrincipal)
    End If
  End Function

  Public Shared Function Fetch(ByVal Criteria As Object) As Object
    If IsTransactionalMethod(GetMethod(Criteria.GetType.DeclaringType, "DataPortal_Fetch")) Then
      Return ServicedPortal.Fetch(Criteria, GetPrincipal)
    Else
      Return Portal.Fetch(Criteria, GetPrincipal)
    End If
  End Function

  Public Shared Function Update(ByVal obj As Object) As Object
    If IsTransactionalMethod(GetMethod(obj.GetType, "DataPortal_Update")) Then
      Return ServicedPortal.Update(obj, GetPrincipal)
    Else
      Return Portal.Update(obj, GetPrincipal)
    End If
  End Function

  Public Shared Sub Delete(ByVal Criteria As Object)
    If IsTransactionalMethod(GetMethod(Criteria.GetType.DeclaringType, "DataPortal_Delete")) Then
      ServicedPortal.Delete(Criteria, GetPrincipal)
    Else
      Portal.Delete(Criteria, GetPrincipal)
    End If
  End Sub

#End Region

#Region " Server-side DataPortal "

  Private Shared Function Portal() As Server.DataPortal
    If mPortal Is Nothing Then
      mPortal = New Server.DataPortal
    End If
    Return mPortal
  End Function

  Private Shared Function ServicedPortal() As Server.ServicedDataPortal.DataPortal
    If mServicedPortal Is Nothing Then
      mServicedPortal = New Server.ServicedDataPortal.DataPortal
    End If
    Return mServicedPortal
  End Function

  Private Shared Function PORTAL_SERVER() As String
    Return ConfigurationSettings.AppSettings("PortalServer")
  End Function

  Private Shared Function SERVICED_PORTAL_SERVER() As String
    Return ConfigurationSettings.AppSettings("ServicedPortalServer")
  End Function

#End Region

#Region " Security "

  Private Shared Function AUTHENTICATION() As String
    Return ConfigurationSettings.AppSettings("Authentication")
  End Function

  Private Shared Function GetPrincipal() As System.Security.Principal.IPrincipal
    If AUTHENTICATION() = "Windows" Then
      ' Windows integrated security 
      Return Nothing

    Else
      ' we assume using the CSLA framework security
      Return System.Threading.Thread.CurrentPrincipal
    End If
  End Function

#End Region

#Region " Helper methods "

  Private Shared Function IsTransactionalMethod(ByVal Method As MethodInfo) As Boolean
    Dim attrib() As Object = Method.GetCustomAttributes(GetType(TransactionalAttribute), True)
    Return (UBound(attrib) > -1)
  End Function

  Private Shared Function GetMethod(ByVal ObjectType As Type, ByVal method As String) As MethodInfo
    Return ObjectType.GetMethod(method, BindingFlags.FlattenHierarchy Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
  End Function

  Shared Sub New()
    ' see if we need to configure remoting at all
    If Len(PORTAL_SERVER) > 0 OrElse Len(SERVICED_PORTAL_SERVER) > 0 Then
      ' create and register our custom HTTP channel
      ' that uses the binary formatter
      Dim properties As New Hashtable
      properties("name") = "HttpBinary"

      Dim formatter As New BinaryClientFormatterSinkProvider

      Dim channel As New HttpChannel(properties, formatter, Nothing)

      ChannelServices.RegisterChannel(channel)

      ' register the data portal types as being remote
      If Len(PORTAL_SERVER) > 0 Then
        RemotingConfiguration.RegisterWellKnownClientType( _
          GetType(Server.DataPortal), PORTAL_SERVER)
      End If
      If Len(SERVICED_PORTAL_SERVER) > 0 Then
        RemotingConfiguration.RegisterWellKnownClientType( _
          GetType(Server.ServicedDataPortal.DataPortal), SERVICED_PORTAL_SERVER)
      End If
    End If

  End Sub

#End Region

End Class
