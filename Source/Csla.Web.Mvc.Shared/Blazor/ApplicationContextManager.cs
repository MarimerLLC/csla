//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side Blazor context manager</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Claims;
using System.Security.Principal;
using Csla.Core;
using Microsoft.AspNetCore.Http;

namespace Csla.Blazor.Server
{
  /// <summary>
  /// Server-side Blazor context manager for the user 
  /// property and local/client context dictionaries.
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="provider">IServiceProvider instance</param>
    /// <param name="httpContextAccessor">httpContextAccessor instance</param>
    public ApplicationContextManager(IServiceProvider provider, IHttpContextAccessor httpContextAccessor)
    { 
      ServiceProvider = provider;
      HttpContext = httpContextAccessor.HttpContext;
    }

    private IServiceProvider ServiceProvider { get; }
    private HttpContext HttpContext { get; }

    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid
    {
      get { return HttpContext != null; }
    }

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public IPrincipal GetUser()
    {
      var result = HttpContext.User;
      if (result == null)
      {
        result = new ClaimsPrincipal();
        SetUser(result);
      }
      return result;
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public void SetUser(IPrincipal principal)
    {
      HttpContext.User = (ClaimsPrincipal)principal;
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      return (ContextDictionary)HttpContext.Items[_localContextName];
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      HttpContext.Items[_localContextName] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return (ContextDictionary)HttpContext.Items[_clientContextName];
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      HttpContext.Items[_clientContextName] = clientContext;
    }
  }
}
