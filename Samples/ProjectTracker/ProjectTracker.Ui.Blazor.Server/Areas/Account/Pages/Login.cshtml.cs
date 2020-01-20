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

namespace ProjectTracker.Ui.Blazor.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class LoginModel : PageModel
  {
    [BindProperty]
    public LoginData loginData { get; set; }
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

      var identity = await Library.Security.PTIdentity.GetPTIdentityAsync(loginData.UserName, loginData.Password);

      // create claimsidentity and claimsprincipal
      var baseidentity = new ClaimsIdentity(identity.AuthenticationType);
      baseidentity.AddClaim(new Claim(ClaimTypes.Name, identity.Name));
      if (identity.Roles != null)
        foreach (var item in identity.Roles)
          baseidentity.AddClaim(new Claim(ClaimTypes.Role, item));
      var principal = new ClaimsPrincipal(baseidentity);

      Csla.ApplicationContext.User = principal;

      //await ProjectTracker.Library.Security.PTPrincipal.LoginAsync(loginData.UserName, loginData.Password);
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
      {
        //var principal = (ClaimsPrincipal)Csla.ApplicationContext.User;
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
