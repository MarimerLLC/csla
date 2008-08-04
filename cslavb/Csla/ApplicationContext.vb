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

#Region " Local Context "

  Private Const mLocalContextName As String = "Csla.LocalContext"

  ''' <summary>
  ''' Returns the application-specific context data that
  ''' is local to the current AppDomain.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' The return value is a HybridDictionary. If one does
  ''' not already exist, and empty one is created and returned.
  ''' </para><para>
  ''' Note that data in this context is NOT transferred to and from
  ''' the client and server.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property LocalContext() As HybridDictionary
    Get
      Dim ctx As HybridDictionary = GetLocalContext()
      If ctx Is Nothing Then
        ctx = New HybridDictionary
        SetLocalContext(ctx)
      End If
      Return ctx
    End Get
  End Property

  Private Function GetLocalContext() As HybridDictionary

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot(mLocalContextName)
      Return CType(Thread.GetData(slot), HybridDictionary)

    Else
      Return CType(HttpContext.Current.Items(mLocalContextName), HybridDictionary)
    End If

  End Function

  Private Sub SetLocalContext(ByVal localContext As HybridDictionary)

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot(mLocalContextName)
      Threading.Thread.SetData(slot, localContext)

    Else
      HttpContext.Current.Items(mLocalContextName) = localContext
    End If

  End Sub

#End Region

#Region " Client/Global Context "

  Private _syncClientContext As New Object
  Private Const _clientContextName As String = "Csla.ClientContext"
  Private Const _globalContextName As String = "Csla.GlobalContext"

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
  ''' </para><para>
  ''' This property is thread safe in a Windows client
  ''' setting and on an application server. It is not guaranteed
  ''' to be thread safe within the context of an ASP.NET
  ''' client setting (i.e. in your ASP.NET UI).
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property ClientContext() As HybridDictionary
    Get
      SyncLock _syncClientContext
        Dim ctx As HybridDictionary = GetClientContext()
        If ctx Is Nothing Then
          ctx = New HybridDictionary
          SetClientContext(ctx)
        End If
        Return ctx
      End SyncLock
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
      If ExecutionLocation = ExecutionLocations.Client Then
        SyncLock _syncClientContext
          Return CType(AppDomain.CurrentDomain.GetData(_clientContextName), HybridDictionary)
        End SyncLock

      Else
        Dim slot As System.LocalDataStoreSlot = _
          Thread.GetNamedDataSlot(_clientContextName)
        Return CType(Thread.GetData(slot), HybridDictionary)
      End If

    Else
      Return CType(HttpContext.Current.Items(_clientContextName),  _
        HybridDictionary)
    End If

  End Function

  Friend Function GetGlobalContext() As HybridDictionary

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot(_globalContextName)
      Return CType(Thread.GetData(slot), HybridDictionary)

    Else
      Return CType(HttpContext.Current.Items(_globalContextName), HybridDictionary)
    End If

  End Function

  Private Sub SetClientContext(ByVal clientContext As HybridDictionary)

    If HttpContext.Current Is Nothing Then
      If ExecutionLocation = ExecutionLocations.Client Then
        SyncLock _syncClientContext
          AppDomain.CurrentDomain.SetData(_clientContextName, clientContext)
        End SyncLock

      Else
        Dim slot As System.LocalDataStoreSlot = _
          Thread.GetNamedDataSlot(_clientContextName)
        Threading.Thread.SetData(slot, clientContext)
      End If

    Else
      HttpContext.Current.Items(_clientContextName) = clientContext
    End If

  End Sub

  Friend Sub SetGlobalContext(ByVal globalContext As HybridDictionary)

    If HttpContext.Current Is Nothing Then
      Dim slot As System.LocalDataStoreSlot = _
        Thread.GetNamedDataSlot(_globalContextName)
      Threading.Thread.SetData(slot, globalContext)

    Else
      HttpContext.Current.Items(_globalContextName) = globalContext
    End If

  End Sub

  Friend Sub SetContext( _
    ByVal clientContext As HybridDictionary, _
    ByVal globalContext As HybridDictionary)

    SetClientContext(clientContext)
    SetGlobalContext(globalContext)

  End Sub

  ''' <summary>
  ''' Clears all context values.
  ''' </summary>
  ''' <remarks></remarks>
  Public Sub Clear()

    SetContext(Nothing, Nothing)
    SetLocalContext(Nothing)

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

  ''' <summary>
  ''' Gets a qualified name for a method that implements
  ''' the IsInRole() behavior used for authorization.
  ''' </summary>
  ''' <returns>
  ''' Returns a value in the form
  ''' "Namespace.Class, Assembly, MethodName".
  ''' </returns>
  ''' <remarks>
  ''' The default is to use a simple IsInRole() call against
  ''' the current principal. If another method is supplied
  ''' it must conform to the IsInRoleProvider delegate.
  ''' </remarks>
  Public ReadOnly Property IsInRoleProvider() As String
    Get
      Dim result As String = _
        ConfigurationManager.AppSettings("CslaIsInRoleProvider")
      If Len(result) = 0 Then
        result = ""
      End If
      Return result
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether objects should be
  ''' automatically cloned by the data portal Update()
  ''' method when using a local data portal configuration.
  ''' </summary>
  Public ReadOnly Property AutoCloneOnUpdate() As Boolean
    Get
      Dim result As Boolean = True
      Dim setting As String = _
        ConfigurationManager.AppSettings("CslaAutoCloneOnUpdate")
      If Len(setting) > 0 Then
        result = Boolean.Parse(setting)
      End If
      Return result
    End Get
  End Property

  ''' <summary>
  ''' Gets the serialization formatter type used by CSLA .NET
  ''' for all explicit object serialization (such as cloning,
  ''' n-level undo, etc).
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If you use the DataContract and DataMember attributes
  ''' to specify how your objects should be serialized then
  ''' you <b>must</b> change this setting to use the
  ''' <see cref="System.Runtime.Serialization.NetDataContractSerializer">
  ''' NetDataContractSerializer</see> option. The default is to
  ''' use the standard Microsoft .NET 
  ''' <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>.
  ''' </para>
  ''' <para>
  ''' This setting does <b>not affect</b> the serialization
  ''' formatters used by the various data portal channels.
  ''' </para>
  ''' <para>
  ''' If you are using the Remoting, Web Services or 
  ''' Enterprise Services technologies, they will use the
  ''' BinaryFormatter regardless of this setting, and will
  ''' <b>fail to work</b> if you attempt to use the
  ''' DataContract and DataMember attributes when building
  ''' your business objects.
  ''' </para>
  ''' <para>
  ''' If you want to use DataContract and DataMember, and
  ''' you want a remote data portal server, you <b>must</b>
  ''' use the WCF data portal channel, or create your own
  ''' custom channel that uses the
  ''' <see cref="System.Runtime.Serialization.NetDataContractSerializer">
  ''' NetDataContractSerializer</see> provided as part of WCF.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property SerializationFormatter() As SerializationFormatters
    Get
      Dim tmp As String = ConfigurationManager.AppSettings("CslaSerializationFormatter")
      If String.IsNullOrEmpty(tmp) Then
        tmp = "BinaryFormatter"
      End If
      Return CType(System.Enum.Parse(GetType(SerializationFormatters), tmp), SerializationFormatters)
    End Get
  End Property

  Private _propertyChangedMode As PropertyChangedModes
  Private _propertyChangedModeSet As Boolean
  ''' <summary>
  ''' Gets or sets a value specifying how CSLA .NET should
  ''' raise PropertyChanged events.
  ''' </summary>
  Public Property PropertyChangedMode() As PropertyChangedModes
    Get
      If (Not _propertyChangedModeSet) Then
        Dim tmp As String = ConfigurationManager.AppSettings("CslaPropertyChangedMode")
        If String.IsNullOrEmpty(tmp) Then
          tmp = "Windows"
        End If
        _propertyChangedMode = CType(System.Enum.Parse(GetType(PropertyChangedModes), tmp), PropertyChangedModes)
        _propertyChangedModeSet = True
      End If
      Return _propertyChangedMode
    End Get
    Set(ByVal value As PropertyChangedModes)
      _propertyChangedMode = value
      _propertyChangedModeSet = True
    End Set
  End Property

  ''' <summary>
  ''' Enum representing the serialization formatters
  ''' supported by CSLA .NET.
  ''' </summary>
  Public Enum SerializationFormatters
    ''' <summary>
    ''' Use the standard Microsoft .NET
    ''' <see cref="BinaryFormatter"/>.
    ''' </summary>
    BinaryFormatter
    ''' <summary>
    ''' Use the Microsoft .NET 3.0
    ''' <see cref="System.Runtime.Serialization.NetDataContractSerializer">
    ''' NetDataContractSerializer</see> provided as part of WCF.
    ''' </summary>
    NetDataContractSerializer
  End Enum

  ''' <summary>
  ''' Enum representing the way in which CSLA .NET
  ''' should raise PropertyChanged events.
  ''' </summary>
  Public Enum PropertyChangedModes
    ''' <summary>
    ''' Raise PropertyChanged events as required
    ''' by Windows Forms data binding.
    ''' </summary>
    Windows
    ''' <summary>
    ''' Raise PropertyChanged events as required
    ''' by XAML data binding in WPF.
    ''' </summary>
    Xaml
  End Enum

#End Region

#Region " ExecutionLocation Property "

  ''' <summary>
  ''' Enum representing the locations code can execute.
  ''' </summary>
  Public Enum ExecutionLocations
    ''' <summary>
    ''' The code is executing on the client.
    ''' </summary>
    Client
    ''' <summary>
    ''' The code is executing on the application server.
    ''' </summary>
    Server
  End Enum

  Private _executionLocation As ExecutionLocations = ExecutionLocations.Client

  ''' <summary>
  ''' Returns a value indicating whether the application code
  ''' is currently executing on the client or server.
  ''' </summary>
  Public ReadOnly Property ExecutionLocation() As ExecutionLocations
    Get
      Return _executionLocation
    End Get
  End Property

  Friend Sub SetExecutionLocation(ByVal location As ExecutionLocations)

    _executionLocation = location

  End Sub

#End Region

End Module
