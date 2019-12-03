using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Csla;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ProjectTracker.Ui.Blazor
{
  public static class AuthenticationBuilderExtensions
  {
    // Custom authentication extension method
    public static AuthenticationBuilder AddCustomAuth(
      this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
    {
      // Add custom authentication scheme with custom options and custom handler
      return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>(CustomAuthOptions.DefaultScheme, configureOptions);
    }
  }

  public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
  {
    public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> m, ILoggerFactory l, UrlEncoder e, ISystemClock c)
      : base(m, l, e, c)
    { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      if (!ApplicationContext.User.Identity.IsAuthenticated)
        await ProjectTracker.Library.Security.PTPrincipal.LoadAsync("manager");
      var ticket = new AuthenticationTicket((ClaimsPrincipal)ApplicationContext.User, Options.Scheme);
      return AuthenticateResult.Success(ticket);
    }
  }

  public class CustomAuthOptions : AuthenticationSchemeOptions
  {
    public static string DefaultScheme { get => "PTPrincipal"; }
    public string Scheme { get; set; } = DefaultScheme;
    public StringValues AuthKey { get; set; }
  }
}
