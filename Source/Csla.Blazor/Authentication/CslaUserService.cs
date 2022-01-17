using System;
using System.Security.Claims;

namespace Csla.Blazor.Authentication
{
  /// <summary>
  /// Expose Csla.ApplicationContext.User for use in
  /// Blazor authentication pages. For use with
  /// CslaAuthenticationStateProvider.
  /// </summary>
  public class CslaUserService
  {
    /// <summary>
    /// Event raised when the current user is changed.
    /// </summary>
    public event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged;

    private ClaimsPrincipal Principal { get; set; }

    /// <summary>
    /// Creates an instance of the type, setting the current user
    /// to an unauthenticated ClaimsPrincipal.
    /// </summary>
    public CslaUserService()
    {
      Principal = new ClaimsPrincipal(new ClaimsIdentity());
    }

    /// <summary>
    /// Gets or sets the current user.
    /// </summary>
    public ClaimsPrincipal User
    {
      get 
      {
        return Principal;
      }
      set
      {
        Principal = value;
        CurrentUserChanged?.Invoke(this, new CurrentUserChangedEventArgs() { NewUser = value });
      }
    }

    /// <summary>
    /// Event args for CurrentUserChanged event.
    /// </summary>
    public class CurrentUserChangedEventArgs : EventArgs
    {
      /// <summary>
      /// Gets the new user value.
      /// </summary>
      public ClaimsPrincipal NewUser { get; internal set; }
    }
  }
}
