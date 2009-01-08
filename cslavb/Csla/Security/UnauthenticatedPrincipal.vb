Imports System
Imports System.Security.Principal
Imports Csla.Serialization

Namespace Security

  ''' <summary>
  ''' Implementation of a .NET principal object that represents
  ''' an unauthenticated user. Contains an UnauthenticatedIdentity
  ''' object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public NotInheritable Class UnauthenticatedPrincipal
    Inherits BusinessPrincipalBase

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
      MyBase.New(New UnauthenticatedIdentity())
    End Sub

    ''' <summary>
    ''' Returns a value indicating whether the user is in the
    ''' specified role.
    ''' </summary>
    ''' <param name="role">Role name.</param>
    ''' <returns></returns>
    Public Overrides Function IsInRole(ByVal role As String) As Boolean

      If Csla.DataPortal.IsInDesignMode Then
        Return True
      Else
        Return MyBase.IsInRole(role)
      End If
    End Function

  End Class

End Namespace

