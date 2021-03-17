//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManagerStatic.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Security.Claims;

namespace Csla.Core
{
  /// <summary>
  /// Context manager for the user property
  /// and local/client/global context dictionaries
  /// that uses static fields to maintain 
  /// per-thread context.
  /// </summary>
  public class ApplicationContextManagerStatic : Csla.Core.IContextManager
  {
    /// <summary>
    /// Returns a value indicating whether the context is valid.
    /// </summary>
    public bool IsValid => true;

    private static ContextDictionary currentLocalContext = new ContextDictionary();
    private static ContextDictionary currentClientContext = new ContextDictionary();
    private static ContextDictionary currentGlobalContext = new ContextDictionary();
    private static IPrincipal currentPrincipal = new ClaimsPrincipal();
    private static IServiceProvider currentDefaultServiceProvider;
    private static IServiceProvider currentServiceProvider;

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    public ContextDictionary GetClientContext()
    {
      return currentClientContext;
    }

    /// <summary>
    /// Gets the default IServiceProvider
    /// </summary>
    public IServiceProvider GetDefaultServiceProvider()
    {
      return currentDefaultServiceProvider;
    }

    /// <summary>
    /// Gets the global context dictionary.
    /// </summary>
    public ContextDictionary GetGlobalContext()
    {
      return currentGlobalContext;
    }

    /// <summary>
    /// Gets the local context dictionary.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      return currentLocalContext;
    }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    /// <returns>The current user principal</returns>
    public IPrincipal GetUser()
    {
      return currentPrincipal;
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    public void SetClientContext(ContextDictionary clientContext)
    {
      currentClientContext = clientContext;
    }

    /// <summary>
    /// Sets the default IServiceProvider
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider instance</param>
    public void SetDefaultServiceProvider(IServiceProvider serviceProvider)
    {
      currentDefaultServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Sets the global context dictionary.
    /// </summary>
    /// <param name="globalContext">Context dictionary</param>
    public void SetGlobalContext(ContextDictionary globalContext)
    {
      currentGlobalContext = globalContext;
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      currentLocalContext = localContext;
    }

    /// <summary>
    /// Gets the service provider for current scope
    /// </summary>
    public IServiceProvider GetServiceProvider()
    {
      return currentServiceProvider ?? GetDefaultServiceProvider();
    }

    /// <summary>
    /// Sets the service provider for current scope
    /// </summary>
    /// <param name="scope">IServiceProvider instance</param>
    public void SetServiceProvider(IServiceProvider scope)
    {
      currentServiceProvider = scope;
    }

    /// <summary>
    /// Sets the current user principal.
    /// </summary>
    /// <param name="principal">User principal value</param>
    public void SetUser(IPrincipal principal)
    {
      currentPrincipal = principal;
    }
  }
}