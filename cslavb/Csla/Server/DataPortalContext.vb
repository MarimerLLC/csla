Imports Csla.Core
Imports System.Security.Principal
Imports System.Threading
Imports System.Collections.Specialized

Namespace Server

  ''' <summary>
  ''' Provides consistent context information between the client
  ''' and server DataPortal objects. 
  ''' </summary>
  <Serializable()> _
  Public Class DataPortalContext

    Private _principal As IPrincipal
    Private _remotePortal As Boolean
    Private _clientCulture As String
    Private _clientUICulture As String
    Private _clientContext As ContextDictionary
    Private _globalContext As ContextDictionary

    <NonSerialized()> _
    Private _transactionalType As TransactionalTypes

    <NonSerialized()> _
    Private _factoryInfo As ObjectFactoryAttribute


    ''' <summary>
    ''' The current principal object
    ''' if CSLA security is being used.
    ''' </summary>
    Public ReadOnly Property Principal() As IPrincipal
      Get
        Return _principal
      End Get
    End Property

    ''' <summary>
    ''' Returns <see langword="true" /> if the 
    ''' server-side DataPortal is running
    ''' on a remote server via remoting.
    ''' </summary>
    Public ReadOnly Property IsRemotePortal() As Boolean
      Get
        Return _remotePortal
      End Get
    End Property

    ''' <summary>
    ''' The culture setting on the client
    ''' workstation.
    ''' </summary>
    Public ReadOnly Property ClientCulture() As String
      Get
        Return _clientCulture
      End Get
    End Property

    ''' <summary>
    ''' The UI culture setting on the client
    ''' workstation.
    ''' </summary>
    Public ReadOnly Property ClientUICulture() As String
      Get
        Return _clientUICulture
      End Get
    End Property

    Friend ReadOnly Property ClientContext() As ContextDictionary
      Get
        Return _clientContext
      End Get
    End Property

    Friend ReadOnly Property GlobalContext() As ContextDictionary
      Get
        Return _globalContext
      End Get
    End Property

    ''' <summary>
    ''' Gets the current transactional type. Only valid
    ''' in the server-side data portal methods after
    ''' the transactional type has been determined.
    ''' </summary>
    Public Property TransactionalType() As TransactionalTypes
      Get
        Return _transactionalType
      End Get
      Friend Set(ByVal value As TransactionalTypes)
        _transactionalType = value
      End Set
    End Property

    ''' <summary>
    ''' Gets the current ObjectFactory attribute
    ''' value (if any). Only valid in the server-side
    ''' data portal methods after the attribute
    ''' value has been determined.
    ''' </summary>
    Public Property FactoryInfo() As ObjectFactoryAttribute
      Get
        Return _factoryInfo
      End Get
      Friend Set(ByVal value As ObjectFactoryAttribute)
        _factoryInfo = value
      End Set
    End Property

    ''' <summary>
    ''' Creates a new DataPortalContext object.
    ''' </summary>
    ''' <param name="principal">The current Principal object.</param>
    ''' <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    Public Sub New( _
      ByVal principal As IPrincipal, ByVal isRemotePortal As Boolean)

      If isRemotePortal Then
        _principal = principal
        _remotePortal = isRemotePortal
        _clientCulture = _
          System.Threading.Thread.CurrentThread.CurrentCulture.Name
        _clientUICulture = _
          System.Threading.Thread.CurrentThread.CurrentUICulture.Name
        _clientContext = Csla.ApplicationContext.GetClientContext
        _globalContext = Csla.ApplicationContext.GetGlobalContext
      End If

    End Sub

  End Class

End Namespace
