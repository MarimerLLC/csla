//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManagerTls.cs" company="Marimer LLC">
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
  /// Context manager for the user property
  /// and local/client/global context dictionaries
  /// that uses thread local storage (TLS) to
  /// maintain per-thread context.
  /// </summary>
  public class ApplicationContextManagerTls : IContextManager
  {
    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

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
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      return (ContextDictionary)Thread.GetData(slot);
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      Thread.SetData(slot, localContext);
    }

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    public ContextDictionary GetClientContext()
    {
      if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
      {
        return (ContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
      }
      else
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
        return (ContextDictionary)Thread.GetData(slot);
      }
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    public void SetClientContext(ContextDictionary clientContext)
    {
      if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
      {
        AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
      }
      else
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
        Thread.SetData(slot, clientContext);
      }
    }

    /// <summary>
    /// Gets the global context dictionary.
    /// </summary>
    public ContextDictionary GetGlobalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
      return (ContextDictionary)Thread.GetData(slot);
    }

    /// <summary>
    /// Sets the global context dictionary.
    /// </summary>
    /// <param name="globalContext">Context dictionary</param>
    public void SetGlobalContext(ContextDictionary globalContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
      Thread.SetData(slot, globalContext);
    }

    private static IServiceProvider _provider;

    /// <summary>
    /// Gets the default IServiceProvider
    /// </summary>
    public IServiceProvider GetDefaultServiceProvider()
    {
      return _provider;
    }

    /// <summary>
    /// Sets the default IServiceProvider
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider instance</param>
    public void SetDefaultServiceProvider(IServiceProvider serviceProvider)
    {
      _provider = serviceProvider;
    }

    /// <summary>
    /// Gets the service provider for current scope
    /// </summary>
    /// <returns></returns>
#pragma warning disable CS3002 // Return type is not CLS-compliant
    public IServiceProvider GetServiceProvider()
#pragma warning restore CS3002 // Return type is not CLS-compliant
    {
      return (IServiceProvider)ApplicationContext.LocalContext["__sps"] ?? GetDefaultServiceProvider();
    }

    /// <summary>
    /// Sets the service provider for current scope
    /// </summary>
    /// <param name="scope">IServiceProvider instance</param>
#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public void SetServiceProvider(IServiceProvider scope)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
      Csla.ApplicationContext.LocalContext["__sps"] = scope;
    }
  }
}
