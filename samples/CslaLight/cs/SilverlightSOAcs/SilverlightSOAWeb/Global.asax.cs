using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SilverlightSOAWeb
{
  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
    }

    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);
    }
  }
}