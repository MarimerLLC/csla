Imports System.IdentityModel.Selectors
Imports System.ServiceModel
Imports ProjectTracker.Library.Security

''' <summary>
''' Summary description for CredentialValidator
''' </summary>
Public Class CredentialValidator
  Inherits UserNamePasswordValidator

  Public Overrides Sub Validate(ByVal userName As String, ByVal password As String)

    If userName <> "anonymous" Then
      PTPrincipal.Logout()
      If (Not PTPrincipal.Login(userName, password)) Then
        Throw New FaultException("Unknown username or password")
      End If

      ' add current principal to rolling cache
      Csla.Security.PrincipalCache.AddPrincipal(Csla.ApplicationContext.User)
    End If

  End Sub

End Class
