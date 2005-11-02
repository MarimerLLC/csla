Imports System.Security.Principal

Namespace Security

  Public Class PTPrincipal
    Inherits Csla.Security.BusinessPrincipalBase

    Private Sub New(ByVal identity As IIdentity)
      MyBase.New(identity)
    End Sub

    Public Shared Sub Login(ByVal username As String, ByVal password As String)

      Dim identity As PTIdentity = PTIdentity.GetIdentity(username, password)
      If identity.IsAuthenticated Then
        Dim principal As New PTPrincipal(identity)
        System.Threading.Thread.CurrentPrincipal = principal
      End If

    End Sub

    Public Shared Sub Logout()

      Dim identity As PTIdentity = PTIdentity.UnauthenticatedIdentity
      Dim principal As New PTPrincipal(identity)
      System.Threading.Thread.CurrentPrincipal = principal

    End Sub

    Public Overrides Function IsInRole(ByVal role As String) As Boolean

      Dim identity As PTIdentity = DirectCast(Me.Identity, PTIdentity)
      Return identity.IsInRole(role)

    End Function

  End Class

End Namespace
