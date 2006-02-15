Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class BusinessPrincipalBase
    Implements IPrincipal

    Private mIdentity As IIdentity

    Public Overridable ReadOnly Property Identity() As IIdentity _
      Implements IPrincipal.Identity
      Get
        Return mIdentity
      End Get
    End Property

    Public Overridable Function IsInRole(ByVal role As String) As Boolean _
      Implements IPrincipal.IsInRole

      Return False

    End Function

    Protected Sub New(ByVal identity As IIdentity)

      mIdentity = identity

    End Sub

  End Class

End Namespace
