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
using Csla;

namespace Csla.Core
{
  /// <summary>
  /// Default context manager for the user property
  /// and local/client/global context dictionaries.
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
#if NET40 || NET45
      private const string _localContextName = "Csla.LocalContext";
      private const string _clientContextName = "Csla.ClientContext";
#else
    private AsyncLocal<ContextDictionary> _localContext = new AsyncLocal<ContextDictionary>();
    private AsyncLocal<ContextDictionary> _clientContext = new AsyncLocal<ContextDictionary>();
#endif
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
        result = new Csla.Security.UnauthenticatedPrincipal();
        SetUser(result);
      }
      return result;
    }

    /// <summary>
    /// Sets teh current user principal.
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
#if NET40 || NET45
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        return (ContextDictionary)Thread.GetData(slot);
#else
      return _localContext.Value;
#endif
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
#if NET40 || NET45
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        Thread.SetData(slot, localContext);
#else
      _localContext.Value = localContext;
#endif
    }

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    public ContextDictionary GetClientContext()
    {
#if NET40 || NET45
        if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
        {
          return (ContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
        }
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          return (ContextDictionary)Thread.GetData(slot);
        }
#else
      return _clientContext.Value;
#endif
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    public void SetClientContext(ContextDictionary clientContext)
    {
#if NET40 || NET45
        if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
        {
          AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
        }
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          Thread.SetData(slot, clientContext);
        }
#else
      _clientContext.Value = clientContext;
#endif
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
    /// Gets the scoped IServiceProvider
    /// </summary>
    public IServiceProvider GetScopedServiceProvider()
    {
      IServiceProvider result = null;
      result = (IServiceProvider)Csla.ApplicationContext.LocalContext["__ssp"];
      if (result == null)
        result = GetDefaultServiceProvider();
      return result;
    }

    /// <summary>
    /// Sets the scoped IServiceProvider
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider instance</param>
    public void SetScopedServiceProvider(IServiceProvider serviceProvider)
    {
      Csla.ApplicationContext.LocalContext["__ssp"] = serviceProvider;
    }
  }
}
