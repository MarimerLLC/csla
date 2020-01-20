using System;
using System.Security.Claims;

namespace Csla.Blazor.Client.Authentication
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

    /// <summary>
    /// Creates an instance of the type, setting the current user
    /// to an unauthenticated ClaimsPrincipal.
    /// </summary>
    public CslaUserService()
    {
      Csla.ApplicationContext.User = new ClaimsPrincipal(new ClaimsIdentity());
    }

    /// <summary>
    /// Gets or sets the current user.
    /// </summary>
    public ClaimsPrincipal CurrentUser
    {
      get 
      {
        return (ClaimsPrincipal)Csla.ApplicationContext.User;
      }
      set
      {
        Csla.ApplicationContext.User = value;
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
