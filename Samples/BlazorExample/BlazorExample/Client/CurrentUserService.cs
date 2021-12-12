using System;
using System.Security.Claims;
using Csla;

namespace BlazorExample.Client
{
  /// <summary>
  /// This service provides the application with a place to set who the current user is. It
  /// raises an event when the current user changes.
  /// </summary>
  public class CurrentUserService
  {
    private ApplicationContext ApplicationContext;

    public event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged;

    public CurrentUserService(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
      applicationContext.Principal = new ClaimsPrincipal(new ClaimsIdentity());
    }

    public ClaimsPrincipal CurrentUser
    {
      get
      {
        return ApplicationContext.Principal;
      }
      set
      {
        ApplicationContext.Principal = value;
        CurrentUserChanged?.Invoke(this, new CurrentUserChangedEventArgs() { NewUser = value });
      }
    }
    public class CurrentUserChangedEventArgs : EventArgs
    {
      public ClaimsPrincipal NewUser { get; set; }
    }
  }
}
