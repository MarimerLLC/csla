using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTracker.Library.Security;

namespace ProjectTracker.Ui.Blazor.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class LoginModel : PageModel
  {
    [BindProperty]
    public Credentials credentials { get; set; }
    [BindProperty]
    public string AlertMessage { get; set; } = "";

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        AlertMessage = "Form has validation errors.";
        return Page();
      }
      if (User.Identity.IsAuthenticated)
      {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      }

      var validator =
        await Csla.DataPortal.FetchAsync<CredentialValidator>(credentials);
      var principal = validator.GetPrincipal();

      Csla.ApplicationContext.User = principal;

      if (principal.Identity.IsAuthenticated)
      {
        AuthenticationProperties authProperties = new AuthenticationProperties();
        await HttpContext.SignInAsync(
          CookieAuthenticationDefaults.AuthenticationScheme,
          principal,
          authProperties);

        string returnUrl = Url.Content("~/");
        return LocalRedirect(returnUrl);
      }
      else
      {
        AlertMessage = "Credentials could not be verified.";
        return Page();
      }
    }
  }

  public class LoginData
  {
    public string UserName { get; set; }
    public string Password { get; set; }
  }
}
