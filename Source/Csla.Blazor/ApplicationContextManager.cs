//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;

namespace Csla.Blazor
{
  /// <summary>
  /// Default context manager for the user property
  /// in Blazor.
  /// </summary>
  public class ApplicationContextManager : Csla.Core.ApplicationContextManager
  {
    private static IPrincipal _principal;

    public ApplicationContextManager()
    {
      DataPortalClient.HttpProxy.UseTextSerialization = true;
    }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    /// <returns>The current user principal</returns>
    public override IPrincipal GetUser()
    {
      if (_principal == null)
      {
        _principal = new Csla.Security.UnauthenticatedPrincipal();
        SetUser(_principal);
      }
      return _principal;
    }

    /// <summary>
    /// Sets the current user principal.
    /// </summary>
    /// <param name="principal">User principal value</param>
    public override void SetUser(IPrincipal principal)
    {
      _principal = principal;
    }
  }
}
