using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Web.SessionState;

namespace Mvc3UI
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          "Default", // Route name
          "{controller}/{action}/{id}", // URL with parameters
          new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
      );

    }

    protected void Application_Start()
    {
      ModelBinders.Binders.DefaultBinder = new Csla.Web.Mvc.CslaModelBinder();

      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }

    //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    //{
    //  if (Csla.ApplicationContext.User != null && Csla.ApplicationContext.User.Identity.IsAuthenticated && Csla.ApplicationContext.User.Identity is FormsIdentity)
    //  {
    //    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies.Get(FormsAuthentication.FormsCookieName).Value);
    //    FormsIdentity id = new FormsIdentity(ticket);
    //    var principal = new GenericPrincipal(
    //      new GenericIdentity(id.Name, id.Ticket.UserData), new string[] { "ProjectManager" });
    //    Csla.ApplicationContext.User = principal;
    //  }
    //}

    protected void Application_AcquireRequestState(
      object sender, EventArgs e)
    {
      if (HttpContext.Current.Handler is IRequiresSessionState)
      {
        if (Csla.ApplicationContext.AuthenticationType == "Windows")
          return;

        IPrincipal principal;
        try
        {
          principal = (IPrincipal)HttpContext.Current.Session["CslaPrincipal"];
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