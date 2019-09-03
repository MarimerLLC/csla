//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using Csla.Core;

namespace Csla.Uno
{
  /// <summary>
  /// Default context manager for the user property
  /// in Blazor.
  /// </summary>
  public class ApplicationContextManager : Csla.Core.IContextManager
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public ApplicationContextManager()
    {
      DataPortalClient.HttpProxy.UseTextSerialization = true;
    }

    public bool IsValid => true;

    private static IPrincipal _principal;
    private static ContextDictionary clientContext = new ContextDictionary();
    private static ContextDictionary globalContext = new ContextDictionary();
    private static ContextDictionary localContext = new ContextDictionary();
    private static IServiceProvider defaultServiceProvider;
    private static IServiceProvider scopedServiceProvider;

    public ContextDictionary GetClientContext()
    {
      return clientContext;
    }

    public IServiceProvider GetDefaultServiceProvider()
    {
      return defaultServiceProvider;
    }

    public ContextDictionary GetGlobalContext()
    {
      return globalContext;
    }

    public ContextDictionary GetLocalContext()
    {
      return localContext;
    }

    public IServiceProvider GetScopedServiceProvider()
    {
      return scopedServiceProvider;
    }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    /// <returns>The current user principal</returns>
    public IPrincipal GetUser()
    {
      if (_principal == null)
      {
        _principal = new Csla.Security.UnauthenticatedPrincipal();
        SetUser(_principal);
      }
      return _principal;
    }

    public void SetClientContext(ContextDictionary clientContext)
    {
      ApplicationContextManager.clientContext = clientContext;
    }

    public void SetDefaultServiceProvider(IServiceProvider serviceProvider)
    {
      defaultServiceProvider = serviceProvider;
    }

    public void SetGlobalContext(ContextDictionary globalContext)
    {
      ApplicationContextManager.globalContext = globalContext;
    }

    public void SetLocalContext(ContextDictionary localContext)
    {
      ApplicationContextManager.localContext = localContext;
    }

    public void SetScopedServiceProvider(IServiceProvider serviceProvider)
    {
      scopedServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Sets the current user principal.
    /// </summary>
    /// <param name="principal">User principal value</param>
    public void SetUser(IPrincipal principal)
    {
      _principal = principal;
    }
  }
}
