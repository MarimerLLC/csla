<%@ Application Language="C#" %>

<script runat="server">

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

    protected void Application_AcquireRequestState(object sender, EventArgs e)
    {
        try
        {
            System.Threading.Thread.CurrentPrincipal =
                (System.Security.Principal.IPrincipal)Session["CslaPrincipal"];
            HttpContext.Current.User = System.Threading.Thread.CurrentPrincipal;
        }
        catch
        {
            // do nothing - this really shouldn't happen
            // but it does on the first login.aspx call...
        }
    }
       
</script>
