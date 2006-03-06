Imports System.Threading
Imports System.Security.Principal
Imports System.Collections.Specialized
Imports System.Web

''' <summary>
''' Provides consistent context information between the client
''' and server DataPortal objects. 
''' </summary>
Public Module ApplicationContext

#Region " User "

  ''' <summary>
  ''' Get or set the current <see cref="IPrincipal" />
  ''' object representing the user's identity.
  ''' </summary>
  ''' <remarks>
  ''' This is discussed in Chapter 5. When running
  ''' under IIS the HttpContext.Current.User value
  ''' is used, otherwise the current Thread.CurrentPrincipal
  ''' value is used.
  ''' </remarks>
  Public Property User() As IPrincipal
    Get
      If HttpContext.Current Is Nothing Then
        Return Thread.CurrentPrincipal

      Else
        Return HttpContext.Current.User
      End If
    End Get
    Set(ByVal value As IPrincipal)
      If HttpContext.Current IsNot Nothing Then
        HttpContext.Current.User = value
      End If
      Thread.CurrentPrincipal = value
    End Set
  End Property

#End Region

#Region " Client/Global Context "

  ''' <summary>
  ''' Returns the application-specific context data provided
  ''' by the client.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' The return value is a HybridDictionary. If one does
  ''' not already exist, and empty one is created and returned.
  ''' </para><para>
  ''' Note that data in this context is transferred from
  ''' the client to the server. No data is transferred from
  ''' the server to the client.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property ClientContext() As HybridDictionary
    Get
      Dim ctx As HybridDictionary = GetClientContext()
      If ctx Is Nothing Then
        ctx = New HybridDictionary
        SetClientContext(ctx)
      End If
      Return ctx
    End Get
  End Property

  ''' <summary>
  ''' Returns the application-specific context data shared
  ''' on both client and server.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' The return value is a HybridDictionary. If one does
  ''' not already exist, and empty one is created and returned.
  ''' </para><para>
  ''' Note that data in this context is transferred to and from
  ''' the client and server. Any objects or data in this context
  ''' will be transferred bi-directionally across the network.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property GlobalContext() As HybridDictionary
    Get
      Dim ctx As HybridDictionary = GetGlobalContext()
      If ctx Is Nothing Then
        ctx = New HybridDictionary
        SetGlobalContext(ctx)
      End If
      Return ctx
    End Get
  End Property

  Friend Function GetClientContext() As HybridDictionary

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("Csla.ClientContext")
      Return CType(Thread.GetData(slot), HybridDictionary)

    Else
      Return CType(HttpContext.Current.Items("Csla.ClientContext"), _
        HybridDictionary)
    End If

  End Function

  Friend Function GetGlobalContext() As HybridDictionary

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("Csla.GlobalContext")
      Return CType(Thread.GetData(slot), HybridDictionary)

    Else
      Return CType(HttpContext.Current.Items("Csla.GlobalContext"), HybridDictionary)
    End If

  End Function

  Private Sub SetClientContext(ByVal clientContext As HybridDictionary)

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("Csla.ClientContext")
      Threading.Thread.SetData(slot, clientContext)

    Else
      HttpContext.Current.Items("Csla.ClientContext") = clientContext
    End If

  End Sub

  Friend Sub SetGlobalContext(ByVal globalContext As HybridDictionary)

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("Csla.GlobalContext")
      Threading.Thread.SetData(slot, globalContext)

    Else
      HttpContext.Current.Items("Csla.GlobalContext") = globalContext
    End If

  End Sub

  Friend Sub SetContext( _
    ByVal clientContext As HybridDictionary, _
    ByVal globalContext As HybridDictionary)

    SetClientContext(clientContext)
    SetGlobalContext(globalContext)

  End Sub

  ''' <summary>
  ''' Clears both global and client context
  ''' values.
  ''' </summary>
  ''' <remarks></remarks>
  Public Sub Clear()

    SetContext(Nothing, Nothing)

  End Sub

#End Region

#Region " Config File Settings "

  ''' <summary>
  ''' Returns the authentication type being used by the
  ''' CSLA .NET framework.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' This value is read from the application configuration
  ''' file with the key value "CslaAuthentication". The value
  ''' "Windows" indicates CSLA .NET should use Windows integrated
  ''' (or AD) security. Any other value indicates the use of
  ''' custom security derived from BusinessPrincipalBase.
  ''' </remarks>
  Public ReadOnly Property AuthenticationType() As String
    Get
      Return ConfigurationManager.AppSettings("CslaAuthentication")
      'Return My.Settings.CslaAuthentication
    End Get
  End Property

  ''' <summary>
  ''' Returns the channel or network protocol
  ''' for the DataPortal server.
  ''' </summary>
  ''' <value>Fully qualified assembly/type name of the proxy class.</value>
  ''' <returns></returns>
  ''' <remarks>
  ''' <para>
  ''' This value is read from the application configuration
  ''' file with the key value "CslaDataPortalProxy". 
  ''' </para><para>
  ''' The proxy class must implement Csla.Server.IDataPortalServer.
  ''' </para><para>
  ''' The value "Local" is a shortcut to running the DataPortal
  ''' "server" in the client process.
  ''' </para><para>
  ''' Other built-in values include:
  ''' <list>
  ''' <item>
  ''' <term>Csla,Csla.DataPortalClient.RemotingProxy</term>
  ''' <description>Use .NET Remoting to communicate with the server</description>
  ''' </item>
  ''' <item>
  ''' <term>Csla,Csla.DataPortalClient.EnterpriseServicesProxy</term>
  ''' <description>Use Enterprise Services (DCOM) to communicate with the server</description>
  ''' </item>
  ''' <item>
  ''' <term>Csla,Csla.DataPortalClient.WebServicesProxy</term>
  ''' <description>Use Web Services (asmx) to communicate with the server</description>
  ''' </item>
  ''' </list>
  ''' Each proxy type does require that the DataPortal server be hosted using the appropriate
  ''' technology. For instance, Web Services and Remoting should be hosted in IIS, while
  ''' Enterprise Services must be hosted in COM+.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property DataPortalProxy() As String
    Get
      Dim result As String = _
        ConfigurationManager.AppSettings("CslaDataPortalProxy")
      If Len(result) = 0 Then
        result = "Local"
      End If
      Return result
    End Get
  End Property

  ''' <summary>
  ''' Returns the URL for the DataPortal server.
  ''' </summary>
  ''' <value></value>
  ''' <returns></returns>
  ''' <remarks>
  ''' This value is read from the application configuration
  ''' file with the key value "CslaDataPortalUrl". 
  ''' </remarks>
  Public ReadOnly Property DataPortalUrl() As Uri
    Get
      Return New Uri(ConfigurationManager.AppSettings("CslaDataPortalUrl"))
    End Get
  End Property

#End Region

#Region " ExecutionLocation Property "

  ''' <summary>
  ''' Enum representing the locations code can execute.
  ''' </summary>
  Public Enum ExecutionLocations
    Client
    Server
  End Enum

  Private mExecutionLocation As ExecutionLocations = ExecutionLocations.Client

  ''' <summary>
  ''' Returns a value indicating whether the application code
  ''' is currently executing on the client or server.
  ''' </summary>
  Public ReadOnly Property ExecutionLocation() As ExecutionLocations
    Get
      Return mExecutionLocation
    End Get
  End Property

  Friend Sub SetExecutionLocation(ByVal location As ExecutionLocations)

    mExecutionLocation = location

  End Sub

#End Region

End Module
