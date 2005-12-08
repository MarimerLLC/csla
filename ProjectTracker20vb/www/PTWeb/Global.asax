<%@ Application Language="VB" %>

<script RunAt="server">

  Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    ' Code that runs on application startup
  End Sub
    
  Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
    ' Code that runs on application shutdown
  End Sub
        
  Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
    ' Code that runs when an unhandled error occurs
  End Sub

  Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
    ' Code that runs when a new session is started
  End Sub

  Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
    ' Code that runs when a session ends. 
    ' Note: The Session_End event is raised only when the sessionstate mode
    ' is set to InProc in the Web.config file. If session mode is set to StateServer 
    ' or SQLServer, the event is not raised.
  End Sub
       
  Protected Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As System.EventArgs)
    
  End Sub
  
  Protected Sub Application_AcquireRequestState(ByVal sender As Object, ByVal e As System.EventArgs)

    If Csla.ApplicationContext.AuthenticationType <> "Windows" Then
      ReloadPrincipal()
    End If
    
  End Sub

  ''' <summary>
  ''' Reload the principal from Session if Session
  ''' is available and if there's a principal 
  ''' object already in there.  
  ''' </summary>
  Private Sub ReloadPrincipal()
    
    Dim principal As System.Security.Principal.IPrincipal = Nothing
    Try
      principal = _
        CType(Session("CslaPrincipal"), System.Security.Principal.IPrincipal)

    Catch
      ' do nothing - this really shouldn't happen
      ' but it does on the first login.aspx call
      ' because Session isn't set up yet for some reason
    End Try
    
    If principal Is Nothing Then
      ' didn't get a principal from Session, so
      ' set it to an unauthenticted PTPrincipal

      ' setting an unauthenticated principal when running
      ' under the VShost causes serialization issues
      ' and isn't strictly necessary anyway
      ProjectTracker.Library.Security.PTPrincipal.Logout()
      HttpContext.Current.User = System.Threading.Thread.CurrentPrincipal
      
    Else
      ' use the principal from Session
      System.Threading.Thread.CurrentPrincipal = principal
      HttpContext.Current.User = principal
    End If

    
  End Sub

</script>

