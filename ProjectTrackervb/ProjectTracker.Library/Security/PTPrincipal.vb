Imports System.Security.Principal

Namespace Security

  <Serializable()> _
  Public Class PTPrincipal
    Inherits Csla.Security.BusinessPrincipalBase

    Private Sub New(ByVal identity As IIdentity)
      MyBase.New(identity)
    End Sub

    Public Shared Function Login(ByVal username As String, ByVal password As String) As Boolean

      Return SetPrincipal(PTIdentity.GetIdentity(username, password))

    End Function

    Public Shared Sub LoadPrincipal(ByVal username As String)

      SetPrincipal(PTIdentity.GetIdentity(username))

    End Sub

    Private Shared Function SetPrincipal(ByVal identity As PTIdentity) As Boolean

      If identity.IsAuthenticated Then
        Dim principal As PTPrincipal = New PTPrincipal(identity)
        Csla.ApplicationContext.User = principal
      End If
      Return identity.IsAuthenticated

    End Function

    Public Shared Sub Logout()

      Dim identity As PTIdentity = PTIdentity.UnauthenticatedIdentity()
      Dim principal As PTPrincipal = New PTPrincipal(identity)
      Csla.ApplicationContext.User = principal

    End Sub

    Public Overrides Function IsInRole(ByVal role As String) As Boolean

      Dim identity As PTIdentity = CType(Me.Identity, PTIdentity)
      Return identity.IsInRole(role)

    End Function

  End Class

End Namespace
