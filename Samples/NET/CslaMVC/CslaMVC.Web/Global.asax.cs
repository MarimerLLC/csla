using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using CslaMVC.Web.Security;

namespace CslaMVC.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
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
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.DefaultBinder = new Csla.Web.Mvc.CslaModelBinder();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (Csla.ApplicationContext.User != null && Csla.ApplicationContext.User.Identity.IsAuthenticated && Csla.ApplicationContext.User.Identity is FormsIdentity)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies.Get(FormsAuthentication.FormsCookieName).Value);
                FormsIdentity id = new FormsIdentity(ticket);
                var principal = new SamplePrincipal(id.Name, id.Ticket.UserData);
                Csla.ApplicationContext.User = principal;
            }
        }

        protected void Application_EndRequest(object sender, EventArgs eventArgs)
        {
            //handle json redirects due to forms ticket timeout
            var app = (HttpApplication)sender;
            var httpContext = new HttpContextWrapper(app.Context);
            if (httpContext.Response.StatusCode == 302 && httpContext.Request.IsAjaxRequest())
            {
                httpContext.Response.StatusCode = 401;
                //TODO: produce json result
            }
        }
    }
}