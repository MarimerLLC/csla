//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Threading;

namespace Csla.Core
{
  /// <summary>
  /// Default context manager for the user property
  /// and local/client/global context dictionaries.
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
    private AsyncLocal<ContextDictionary> _localContext = new();
    private AsyncLocal<ContextDictionary> _clientContext = new();

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="provider">IServiceProvider object</param>
    public ApplicationContextManager(IServiceProvider provider)
    {
      _provider = provider;
    }

    /// <summary>
    /// Returns a value indicating whether the context is valid.
    /// </summary>
    public bool IsValid
    {
      get { return true; }
    }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    /// <returns>The current user principal</returns>
    public virtual IPrincipal GetUser()
    {
      IPrincipal result = Thread.CurrentPrincipal;
      if (result == null)
      {
        result = new System.Security.Claims.ClaimsPrincipal();
        SetUser(result);
      }
      return result;
    }

    /// <summary>
    /// Sets the current user principal.
    /// </summary>
    /// <param name="principal">User principal value</param>
    public virtual void SetUser(IPrincipal principal)
    {
      Thread.CurrentPrincipal = principal;
    }

    /// <summary>
    /// Gets the local context dictionary.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      return _localContext.Value;
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      _localContext.Value = localContext;
    }

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    /// <param name="executionLocation"></param>
    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return _clientContext.Value;
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      _clientContext.Value = clientContext;
    }

    private IServiceProvider _provider;

    /// <summary>
    /// Gets the service provider for current scope
    /// </summary>
    /// <returns></returns>
    public IServiceProvider GetServiceProvider()
    {
      return _provider;
    }

    /// <summary>
    /// Sets the service provider for current scope
    /// </summary>
    /// <param name="provider">IServiceProvider instance</param>
    public void SetServiceProvider(IServiceProvider provider)
    {
      _provider = provider;
    }
  }
}
