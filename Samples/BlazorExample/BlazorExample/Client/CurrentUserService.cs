using System.Security.Claims;

namespace BlazorExample.Client
{
  /// <summary>
  /// This service provides the application with a place to set who the current user is. It
  /// raises an event when the current user changes.
  /// </summary>
  public class CurrentUserService
  {
    public event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged;

    private ClaimsPrincipal _principal = new ClaimsPrincipal(new ClaimsIdentity());

    public ClaimsPrincipal CurrentUser 
    { get => _principal; 
      set 
      { 
        _principal = value; 
        CurrentUserChanged?.Invoke(this, new CurrentUserChangedEventArgs { NewUser = value }); } 
    }

    public class CurrentUserChangedEventArgs : EventArgs
    {
      public ClaimsPrincipal NewUser { get; set; }
    }
  }
}
