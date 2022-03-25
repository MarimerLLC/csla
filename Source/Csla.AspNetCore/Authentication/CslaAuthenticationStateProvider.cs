//-----------------------------------------------------------------------
// <copyright file="CslaAuthenticationStateProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Basic implementation of AuthenticationStateProvider</summary>
//-----------------------------------------------------------------------
#if NET5_0_OR_GREATER
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Csla.AspNetCore.Authentication
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
      SetPrincipal(new ClaimsPrincipal());
    }

    private AuthenticationState AuthenticationState { get; set; }

    /// <summary>
    /// Gets the authentication state.
    /// </summary>
    /// <returns></returns>
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      return Task.FromResult(AuthenticationState);
    }

    /// <summary>
    /// Sets the principal representing the current user identity.
    /// </summary>
    /// <param name="principal">ClaimsPrincipal instance</param>
    public void SetPrincipal(ClaimsPrincipal principal)
    {
      AuthenticationState = new AuthenticationState(principal);
      NotifyAuthenticationStateChanged(Task.FromResult(AuthenticationState));
    }
  }
}
#endif