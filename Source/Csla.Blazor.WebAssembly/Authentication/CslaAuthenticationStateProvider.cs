//-----------------------------------------------------------------------
// <copyright file="CslaAuthenticationStateProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Basic implementation of AuthenticationStateProvider</summary>
//-----------------------------------------------------------------------
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Csla.Blazor.Authentication
{
  /// <summary>
  /// Basic implementation of AuthenticationStateProvider
  /// </summary>
  public class CslaAuthenticationStateProvider : AuthenticationStateProvider
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public CslaAuthenticationStateProvider()
    {
    }

    private AuthenticationState AuthenticationState { get; set; } = new AuthenticationState(new ClaimsPrincipal());

    /// <summary>
    /// Gets the authentication state.
    /// </summary>
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      return Task.FromResult(AuthenticationState);
    }

    /// <summary>
    /// Sets the principal representing the current user identity.
    /// </summary>
    /// <param name="principal">ClaimsPrincipal instance</param>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    public void SetPrincipal(ClaimsPrincipal principal)
    {
      ArgumentNullException.ThrowIfNull(principal);

      AuthenticationState = new AuthenticationState(principal);
      NotifyAuthenticationStateChanged(Task.FromResult(AuthenticationState));
    }
  }
}
