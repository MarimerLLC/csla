using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Csla.Blazor.Client.Authentication
{
  /// <summary>
  /// ASP.NET AuthenticationStateProvider that relies on the
  /// CslaUserService to manage the current user.
  /// </summary>
  public class CslaAuthenticationStateProvider : AuthenticationStateProvider
  {
    private readonly CslaUserService _currentUserService;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="currentUserService">CslaUserService instance</param>
    public CslaAuthenticationStateProvider(CslaUserService currentUserService)
    {
      _currentUserService = currentUserService;
      _currentUserService.CurrentUserChanged += (sender, e) =>
      {
        var authState = Task.FromResult(new AuthenticationState(e.NewUser));
        NotifyAuthenticationStateChanged(authState);
      };
    }

    /// <summary>
    /// Gets the AuthenticationState from the
    /// CslaUserService service.
    /// </summary>
    /// <returns></returns>
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      return Task.FromResult(new AuthenticationState(_currentUserService.CurrentUser));
    }
  }
}
