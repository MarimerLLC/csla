using System.Web;
using System.Web.Mvc;

namespace ProjectTracker.AppServerHost
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }
  }
}
