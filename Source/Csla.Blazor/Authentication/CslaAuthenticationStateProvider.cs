using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Csla.Blazor.Authentication
{
  /// <summary>
  /// ASP.NET AuthenticationStateProvider that relies on the
  /// CslaUserService to manage the current user.
  /// </summary>
  public class CslaAuthenticationStateProvider : AuthenticationStateProvider
  {
    private readonly CslaUserService _currentUserService;
    private AuthenticationState AuthenticationState { get; set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="currentUserService">CslaUserService instance</param>
    public CslaAuthenticationStateProvider(CslaUserService currentUserService)
    {
      _currentUserService = currentUserService;
      AuthenticationState = new AuthenticationState(_currentUserService.User);
      _currentUserService.CurrentUserChanged += (sender, e) =>
      {
        AuthenticationState = new AuthenticationState(e.NewUser);
        OnAuthenticationStateChanged(AuthenticationState);
      };
    }

    /// <summary>
    /// Method invoked when the user service
    /// indicates that the current user has changed,
    /// resulting in a call to NotifyAuthenticationStateChanged.
    /// </summary>
    /// <param name="state">New authentication state</param>
    protected virtual void OnAuthenticationStateChanged(AuthenticationState state)
    {
      NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    /// <summary>
    /// Gets the AuthenticationState from the
    /// CslaUserService service.
    /// </summary>
    /// <returns></returns>
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      return Task.FromResult(AuthenticationState);
    }
  }
}
