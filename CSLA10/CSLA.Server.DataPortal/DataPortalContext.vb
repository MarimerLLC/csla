Imports System.Security.Principal

''' <summary>
''' 
''' </summary>
Namespace Server

  ''' <summary>
  ''' Provides consistent context information between the client
  ''' and server DataPortal objects. 
  ''' </summary>
  ''' <remarks>
  ''' The context includes the current 
  ''' <see cref="T:CSLA.Security.BusinessPrincipal" />
  ''' object if CSLA security is being used. It also includes a
  ''' flag indicating whether the server-side DataPortal is running
  ''' locally or remotely.
  ''' </remarks>
  <Serializable()> _
  Public Class DataPortalContext

    Private mPrincipal As IPrincipal
    Private mRemotePortal As Boolean

    ''' <summary>
    ''' The current <see cref="T:CSLA.Security.BusinessPrincipal" />
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
    ''' Creates a new DataPortalContext object.
    ''' </summary>
    ''' <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    Public Sub New(ByVal isRemotePortal As Boolean)
      mPrincipal = Nothing
      mRemotePortal = isRemotePortal
    End Sub

    ''' <summary>
    ''' Creates a new DataPortalContext object.
    ''' </summary>
    ''' <param name="principal">The current Principal object.</param>
    ''' <param name="isRemotePortal">Indicates whether the DataPortal is remote.</param>
    Public Sub New(ByVal principal As IPrincipal, ByVal isRemotePortal As Boolean)
      mPrincipal = principal
      mRemotePortal = isRemotePortal
    End Sub

  End Class

End Namespace
