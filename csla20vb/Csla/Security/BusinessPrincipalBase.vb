Imports System.Security.Principal

Namespace Security

  ''' <summary>
  ''' Base class from which custom principal
  ''' objects should inherit to operate
  ''' properly with the data portal.
  ''' </summary>
  <Serializable()> _
  Public Class BusinessPrincipalBase
    Implements IPrincipal

    Private mIdentity As IIdentity

    ''' <summary>
    ''' Returns the user's identity object.
    ''' </summary>
    Public Overridable ReadOnly Property Identity() As IIdentity _
      Implements IPrincipal.Identity
      Get
        Return mIdentity
      End Get
    End Property

    ''' <summary>
    ''' Returns a value indicating whether the
    ''' user is in a given role.
    ''' </summary>
    ''' <param name="role">Name of the role.</param>
    Public Overridable Function IsInRole(ByVal role As String) As Boolean _
      Implements IPrincipal.IsInRole

      Return False

    End Function

    Protected Sub New(ByVal identity As IIdentity)

      mIdentity = identity

    End Sub

  End Class

End Namespace
