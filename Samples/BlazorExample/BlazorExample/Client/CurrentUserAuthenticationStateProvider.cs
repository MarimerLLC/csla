using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using static BlazorExample.Client.CurrentUserService;

namespace BlazorExample.Client
{
  public class CurrentUserAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
  {
    private readonly CurrentUserService _currentUserService;

    public CurrentUserAuthenticationStateProvider(CurrentUserService currentUserService)
    {
      _currentUserService = currentUserService;
      _currentUserService.CurrentUserChanged += _currentUserService_CurrentUserChanged;
    }

    private void _currentUserService_CurrentUserChanged(object sender, CurrentUserChangedEventArgs e)
    {
      var authState = Task<AuthenticationState>.FromResult(new AuthenticationState(e.NewUser));
      NotifyAuthenticationStateChanged(authState);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      //Note: Your implementation of this depends on your authentication model.
      // For example, perhaps you want to cache a JWT, and then peridocically renew it by re-sending a request to the server etc.
      // In this sample app we keep things very simple.
      return Task.FromResult(new AuthenticationState(_currentUserService.CurrentUser));
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          _currentUserService.CurrentUserChanged -= _currentUserService_CurrentUserChanged;
        }

        disposedValue = true;
      }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
    }
    #endregion
  }
}
