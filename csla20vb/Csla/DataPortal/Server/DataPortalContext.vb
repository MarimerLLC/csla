Imports System.Security.Principal
Imports System.Threading
Imports System.Collections.Specialized

Namespace Server

  ''' <summary>
  ''' Provides consistent context information between the client
  ''' and server DataPortal objects. 
  ''' </summary>
  ''' <remarks>
  ''' The context includes the current 
  ''' <see cref="T:Csla.Security.BusinessPrincipal" />
  ''' object if CSLA security is being used. It also includes a
  ''' flag indicating whether the server-side DataPortal is running
  ''' locally or remotely.
  ''' </remarks>
  <Serializable()> _
  Public Class DataPortalContext

    Private mPrincipal As IPrincipal
    Private mRemotePortal As Boolean
    Private mClientCulture As String
    Private mClientUICulture As String
    Private mClientContext As HybridDictionary
    Private mGlobalContext As HybridDictionary

    ''' <summary>
    ''' The current <see cref="T:Csla.Security.BusinessPrincipal" />
    ''' if CSLA security is being used.
    ''' </summary>
    Public ReadOnly Property Principal() As IPrincipal
      Get
        Return mPrincipal
      End Get
    End Property

    ''' <summary>
    ''' Returns True if the server-side DataPortal is running
    ''' on a remote server via remoting.
    ''' </summary>
    Public ReadOnly Property IsRemotePortal() As Boolean
      Get
        Return mRemotePortal
      End Get
    End Property

    ''' <summary>
    ''' The culture setting on the client
    ''' workstation.
    ''' </summary>
    Public ReadOnly Property ClientCulture() As String
      Get
        Return mClientCulture
      End Get
    End Property

    ''' <summary>
    ''' The UI culture setting on the client
    ''' workstation.
    ''' </summary>
    Public ReadOnly Property ClientUICulture() As String
      Get
        Return mClientUICulture
      End Get
    End Property

    Friend ReadOnly Property ClientContext() As HybridDictionary
      Get
        Return mClientContext
      End Get
    End Property

    Friend ReadOnly Property GlobalContext() As HybridDictionary
      Get
        Return mGlobalContext
      End Get
    End Property

    ''' <summary>
    ''' Creates a new DataPortalContext object.
    ''' </summary>
    ''' <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    Public Sub New(ByVal isRemotePortal As Boolean)

      If isRemotePortal Then
        mRemotePortal = isRemotePortal
        mClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name
        mClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
        mClientContext = Csla.ApplicationContext.GetClientContext
        mGlobalContext = Csla.ApplicationContext.GetGlobalContext
      End If

    End Sub

    ''' <summary>
    ''' Creates a new DataPortalContext object.
    ''' </summary>
    ''' <param name="principal">The current Principal object.</param>
    ''' <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    Public Sub New(ByVal principal As IPrincipal, ByVal isRemotePortal As Boolean)

      If isRemotePortal Then
        mPrincipal = principal
        mRemotePortal = isRemotePortal
        mClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name
        mClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
        mClientContext = Csla.ApplicationContext.GetClientContext
        mGlobalContext = Csla.ApplicationContext.GetGlobalContext
      End If

    End Sub

  End Class

End Namespace
