Option Strict On

Imports Microsoft.VisualBasic
Imports ProjectTracker.Library.Security

Friend Module Security

  Public Sub UseAnonymous()

    ProjectTracker.Library.Security.PTPrincipal.Logout()

  End Sub

  Public Sub Login(ByVal credentials As CslaCredentials)

    If Len(credentials.Username) = 0 Then
      Throw New System.Security.SecurityException( _
        "Valid credentials not provided")
    End If

    ' set to unauthenticated principal
    PTPrincipal.Logout()

    With credentials
      PTPrincipal.Login(.Username, .Password)
    End With

    If Not Csla.ApplicationContext.User.Identity.IsAuthenticated Then
      ' the user is not valid, raise an error
      Throw New System.Security.SecurityException("Invalid user or password")
    End If

  End Sub

End Module
