using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Mvc3UI.Models;

namespace Mvc3UI.Controllers
{
  public class AccountController : Controller
  {

    public IFormsAuthenticationService FormsService { get; set; }
    public IMembershipService MembershipService { get; set; }

    protected override void Initialize(RequestContext requestContext)
    {
      if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
      if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

      base.Initialize(requestContext);
    }

    // **************************************
    // URL: /Account/LogOn
    // **************************************

    public ActionResult LogOn()
    {
      return View();
    }

    [HttpPost]
    public ActionResult LogOn(LogOnModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        if (MembershipService.ValidateUser(model.UserName, model.Password))
        {
          FormsService.SignIn(model.UserName, model.RememberMe);
          if (Url.IsLocalUrl(returnUrl))
          {
            return Redirect(returnUrl);
          }
          else
          {
            return RedirectToAction("Index", "Home");
          }
        }
        else
        {
          ModelState.AddModelError("", "The user name or password provided is incorrect.");
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }
  }
}
