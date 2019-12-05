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

      await ProjectTracker.Library.Security.PTPrincipal.LoginAsync(loginData.UserName, loginData.Password);
      if (Csla.ApplicationContext.User.Identity.IsAuthenticated)
      {
        //var claims = new List<Claim>
        //{
        //  new Claim(ClaimTypes.Name, Csla.ApplicationContext.User.Identity.Name)
        //};
        //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //var principal = new ClaimsPrincipal(claimsIdentity);
        var principal = (ClaimsPrincipal)Csla.ApplicationContext.User;

        AuthenticationProperties authProperties = new AuthenticationProperties
        {
          //AllowRefresh = <bool>,
          // Refreshing the authentication session should be allowed.

          //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
          // The time at which the authentication ticket expires. A 
          // value set here overrides the ExpireTimeSpan option of 
          // CookieAuthenticationOptions set with AddCookie.

          //IsPersistent = true,
          // Whether the authentication session is persisted across 
          // multiple requests. When used with cookies, controls
          // whether the cookie's lifetime is absolute (matching the
          // lifetime of the authentication ticket) or session-based.

          //IssuedUtc = <DateTimeOffset>,
          // The time at which the authentication ticket was issued.

          //RedirectUri = <string>
          // The full path or absolute URI to be used as an http 
          // redirect response value.
        };

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
