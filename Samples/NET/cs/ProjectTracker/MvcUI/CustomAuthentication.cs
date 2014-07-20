using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace MvcUI
{
  public class CustomAuthentication : ActionFilterAttribute, IAuthenticationFilter
  {
    public void OnAuthentication(AuthenticationContext filterContext)
    {
    }

    public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
    {
      //if (filterContext.Controller is Controllers.AccountController) return;

      //var user = filterContext.HttpContext.User;
      //if (Csla.ApplicationContext.User != null &&
      //    Csla.ApplicationContext.User.Identity.IsAuthenticated)
      //{
      //  ProjectTracker.Library.Security.PTPrincipal.Load(Csla.ApplicationContext.User.Identity.Name);
      //}
      //if (user == null || !user.Identity.IsAuthenticated)
      //{
      //  filterContext.Result = new HttpUnauthorizedResult();
      //}
      ProjectTracker.Library.Security.PTPrincipal.Login("manager", "manager");
    }
  }
}