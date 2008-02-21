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

    Private _identity As IIdentity

    ''' <summary>
    ''' Returns the user's identity object.
    ''' </summary>
    Public Overridable ReadOnly Property Identity() As IIdentity _
      Implements IPrincipal.Identity
      Get
        Return _identity
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

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="identity">Identity object for the user.</param>
    Protected Sub New(ByVal identity As IIdentity)

      _identity = identity

    End Sub

  End Class

End Namespace
