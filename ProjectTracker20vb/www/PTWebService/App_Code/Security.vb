Option Strict On

Imports Microsoft.VisualBasic

Friend Module Security

  Public Sub UseAnonymous()

    If UrlIsHostedByVS(HttpContext.Current.Request.Url) Then Exit Sub

    ProjectTracker.Library.Security.PTPrincipal.Logout()

  End Sub

  Public Sub Login(ByVal credentials As CslaCredentials)

    'If UrlIsHostedByVS(HttpContext.Current.Request.Url) Then Exit Sub

    If Len(credentials.Username) = 0 Then
      Throw New System.Security.SecurityException( _
        "Valid credentials not provided")
    End If

    ' set to unauthenticated principal
    ProjectTracker.Library.Security.PTPrincipal.Logout()

    With credentials
      ProjectTracker.Library.Security.PTPrincipal.Login(.Username, .Password)
    End With

    Dim principal As System.Security.Principal.IPrincipal = _
      Threading.Thread.CurrentPrincipal

    If principal.Identity.IsAuthenticated Then
      ' the user is valid - set up the HttpContext
      HttpContext.Current.User = principal

    Else
      ' the user is not valid, raise an error
      Throw New System.Security.SecurityException("Invalid user or password")
    End If

  End Sub

  Private Function UrlIsHostedByVS(ByVal uri As System.Uri) As Boolean

    If uri.Port >= 1024 _
      AndAlso _
      String.Compare(uri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0 Then

      Return True
    End If
    Return False

  End Function

End Module
