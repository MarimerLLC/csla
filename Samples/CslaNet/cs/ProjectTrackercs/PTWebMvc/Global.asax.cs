using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Routing;

namespace PTWebMvc
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          "Default",                                              // Route name
          "{controller}/{action}/{id}",                           // URL with parameters
          new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
      );

    }

    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);
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
          if (User.Identity.IsAuthenticated && User.Identity is FormsIdentity)
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
  }
}