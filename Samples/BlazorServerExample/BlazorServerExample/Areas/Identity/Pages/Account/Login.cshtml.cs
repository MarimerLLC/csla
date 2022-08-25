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

namespace BlazorServerExample.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class LoginModel : PageModel
  {
    [BindProperty]
    public string Username { get; set; } = "";
    [BindProperty]
    public string Password { get; set; } = "";
    [BindProperty]
    public string? ErrorText { get; set; }

    private static Dictionary<string, string> Users = new Dictionary<string, string>
    {
      { "rocky", "mypassword" },
      { "andrew", "otherpassword" }
    };

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        ErrorText = "Form has validation errors.";
        return Page();
      }

      if (!string.IsNullOrWhiteSpace(Username) &&
          Users.TryGetValue(Username.ToLower(), out string? pw))
      {
        if (pw == Password)
        {
          var identity = new ClaimsIdentity("password");
          identity.AddClaim(new Claim(ClaimTypes.Name, Username.ToLower()));
          identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
          var principal = new ClaimsPrincipal(identity);
          var authProperties = new AuthenticationProperties();
          await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties);

          return LocalRedirect(Url.Content("~/"));
        }
      }
      ErrorText = "Invalid credentials";
      return Page();
    }
  }
}
