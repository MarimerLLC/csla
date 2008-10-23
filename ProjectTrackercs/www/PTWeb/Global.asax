<%@ Application Language="C#" %>

<script RunAt="server">

  void Application_Start(object sender, EventArgs e)
  {
    // Code that runs on application startup

  }

  void Application_End(object sender, EventArgs e)
  {
    //  Code that runs on application shutdown

  }

  void Application_Error(object sender, EventArgs e)
  {
    // Code that runs when an unhandled error occurs

  }

  void Session_Start(object sender, EventArgs e)
  {
    // Code that runs when a new session is started

  }

  void Session_End(object sender, EventArgs e)
  {
    // Code that runs when a session ends. 
    // Note: The Session_End event is raised only when the sessionstate mode
    // is set to InProc in the Web.config file. If session mode is set to StateServer 
    // or SQLServer, the event is not raised.

  }

  protected void Application_AuthenticateRequest(object sender, EventArgs e)
  {

  }

  protected void Application_AcquireRequestState(
    object sender, EventArgs e)
  {
    if (HttpContext.Current.Handler is IRequiresSessionState)
    {
      if (Csla.ApplicationContext.AuthenticationType == "Windows")
        return;

      System.Security.Principal.IPrincipal principal;
      try
      {
        principal = (System.Security.Principal.IPrincipal)
          HttpContext.Current.Session["CslaPrincipal"];
      }
      catch
      {
        principal = null;
      }

      if (principal == null)
      {
        if (User.Identity.IsAuthenticated && 
          User.Identity is FormsIdentity)
        {
          // no principal in session, but ASP.NET token
          // still valid - so sign out ASP.NET
          FormsAuthentication.SignOut();
          Response.Redirect(Request.Url.PathAndQuery);
        }
        // didn't get a principal from Session, so
        // set it to an unauthenticted PTPrincipal
        ProjectTracker.Library.Security.PTPrincipal.Logout();
      }
      else
      {
        // use the principal from Session
        Csla.ApplicationContext.User = principal;
      }
    }
  }

</script>

