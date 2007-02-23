Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class PTPrincipal
    Inherits Csla.Security.BusinessPrincipalBase

    Private Sub New(ByVal identity As IIdentity)
      MyBase.New(identity)
    End Sub

    Public Shared Function Login( _
      ByVal username As String, ByVal password As String) As Boolean

      Dim identity As PTIdentity = PTIdentity.GetIdentity(username, password)
      If identity.IsAuthenticated Then
        Dim principal As New PTPrincipal(identity)
        Csla.ApplicationContext.User = principal
      End If
      Return identity.IsAuthenticated

    End Function

    Public Shared Sub Logout()

      Dim identity As PTIdentity = PTIdentity.UnauthenticatedIdentity
      Dim principal As New PTPrincipal(identity)
      Csla.ApplicationContext.User = principal

    End Sub

    Public Overrides Function IsInRole(ByVal role As String) As Boolean

      Dim identity As PTIdentity = DirectCast(Me.Identity, PTIdentity)
      Return identity.IsInRole(role)

    End Function

  End Class

End Namespace
