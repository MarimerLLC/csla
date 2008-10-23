using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Routing;

namespace PTWebMvc
{
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

      //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      //routes.MapRoute(
      //    "Default",                                              // Route name
      //    "{controller}/{action}/{id}",                           // URL with parameters
      //    new { controller = "Home", action = "Index", id = "" }, // Parameter defaults
      //    new { controller = @"[^\.]*" }                          // Parameter constraints - Do not allow dots in the controller name
      //);
      //routes.MapRoute(
      //    "Default.mvc",                                          // Route name
      //    "{controller}.mvc/{action}/{id}",                       // URL with parameters
      //    new { controller = "Home", action = "Index", id = "" }, // Parameter defaults
      //    new { controller = @"[^\.]*" }                          // Parameter constraints - Do not allow dots in the controller name
      //);

    }

    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);
    }

    protected void Application_AcquireRequestState(
      object sender, EventArgs e)
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
        if (User.Identity.IsAuthenticated && User.Identity is System.Web.Security.FormsIdentity)
        {
          // We should only get here when the session expires after
          // we have logged in (have a valid FormsIdentity)
          FormsAuthentication.SignOut();
          Response.Redirect(Request.Url.PathAndQuery);
        }
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