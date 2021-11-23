//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace Csla.Blazor.Server
{
  /// <summary>
  /// Default context manager for the user property
  /// and local/client/global context dictionaries.
  /// </summary>
  public class ApplicationContextManager : Csla.Core.ApplicationContextManager
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="provider">IServiceProvider instance</param>
    /// <param name="httpContextAccessor">httpContextAccessor instance</param>
    public ApplicationContextManager(IServiceProvider provider, IHttpContextAccessor httpContextAccessor)
      : base(provider) 
    { 
      HttpContext = httpContextAccessor;
    }

    private IHttpContextAccessor HttpContext { get; }

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public override IPrincipal GetUser() => HttpContext.HttpContext.User;

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    public override void SetUser(IPrincipal principal) => HttpContext.HttpContext.User = (ClaimsPrincipal)principal;
  }
}
