Imports System.Security.Principal

Namespace Server

  <Serializable()> _
  Public Class DataPortalContext

    Private mPrincipal As IPrincipal
    Private mRemotePortal As Boolean

    Public ReadOnly Property Principal() As IPrincipal
      Get
        Return mPrincipal
      End Get
    End Property

    Public ReadOnly Property IsRemotePortal() As Boolean
      Get
        Return mRemotePortal
      End Get
    End Property

    Public Sub New(ByVal isRemotePortal As Boolean)
      mPrincipal = Nothing
      mRemotePortal = isRemotePortal
    End Sub

    Public Sub New(ByVal principal As IPrincipal, ByVal isRemotePortal As Boolean)
      mPrincipal = principal
      mRemotePortal = isRemotePortal
    End Sub

  End Class

End Namespace
