Imports System.Threading
Imports System.Collections.Specialized

''' <summary>
''' Provides consistent context information between the client
''' and server DataPortal objects. 
''' </summary>
Public NotInheritable Class ApplicationContext

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
  Public Shared ReadOnly Property ClientContext() As HybridDictionary
    Get
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("Csla.ClientContext")
      Dim ctx As HybridDictionary = _
        CType(Thread.GetData(slot), HybridDictionary)
      If ctx Is Nothing Then
        ctx = New HybridDictionary
        Threading.Thread.SetData(slot, ctx)
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
  Public Shared ReadOnly Property GlobalContext() As HybridDictionary
    Get
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot("Csla.GlobalContext")
      Dim ctx As HybridDictionary = _
        CType(Thread.GetData(slot), HybridDictionary)
      If ctx Is Nothing Then
        ctx = New HybridDictionary
        Threading.Thread.SetData(slot, ctx)
      End If
      Return ctx
    End Get
  End Property

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
  Public Shared ReadOnly Property AuthenticationType() As String
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
  Public Shared ReadOnly Property DataPortalProxy() As String
    Get
      Dim result As String = ConfigurationManager.AppSettings("CslaDataPortalProxy")
      'Dim result As String = My.Settings.CslaDataPortalProxy
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
  Public Shared ReadOnly Property DataPortalUrl() As Uri
    Get
      Return New Uri(ConfigurationManager.AppSettings("CslaDataPortalUrl"))
      'Return New Uri(My.Settings.CslaDataPortalUrl)
    End Get
  End Property

  Friend Shared Function GetClientContext() As HybridDictionary

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("Csla.ClientContext")
    Return CType(Thread.GetData(slot), HybridDictionary)

  End Function

  Friend Shared Function GetGlobalContext() As HybridDictionary

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("Csla.GlobalContext")
    Return CType(Thread.GetData(slot), HybridDictionary)

  End Function

  Friend Shared Sub SetContext(ByVal clientContext As Object, ByVal globalContext As Object)

    Dim slot As System.LocalDataStoreSlot = _
      Thread.GetNamedDataSlot("Csla.ClientContext")
    Threading.Thread.SetData(slot, clientContext)

    slot = Thread.GetNamedDataSlot("Csla.GlobalContext")
    Threading.Thread.SetData(slot, globalContext)

  End Sub

  Public Shared Sub Clear()

    Thread.FreeNamedDataSlot("Csla.ClientContext")
    Thread.FreeNamedDataSlot("Csla.GlobalContext")

  End Sub

  Private Sub New()
    ' prevent instantiation
  End Sub

End Class
