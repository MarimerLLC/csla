﻿//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManagerTls.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;

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

    /// <summary>
    /// Gets a value indicating whether the context manager
    /// is stateful.
    /// </summary>
    public bool IsStatefulContext => true;

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
    public IContextDictionary GetLocalContext()
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      return (IContextDictionary)Thread.GetData(slot);
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(IContextDictionary localContext)
    {
      LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
      Thread.SetData(slot, localContext);
    }

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    /// <param name="executionLocation"></param>
    public IContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      if (executionLocation == ApplicationContext.ExecutionLocations.Client)
      {
        return (IContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
      }
      else
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
        return (IContextDictionary)Thread.GetData(slot);
      }
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(IContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      if (executionLocation == ApplicationContext.ExecutionLocations.Client)
      {
        AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
      }
      else
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
        Thread.SetData(slot, clientContext);
      }
    }

    private const string _applicationContextName = "Csla.ApplicationContext";

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext ApplicationContext
    {
      get
      {
        var slot = Thread.GetNamedDataSlot(_applicationContextName);
        return Thread.GetData(slot) as ApplicationContext;
      }
      set
      {
        var slot = Thread.GetNamedDataSlot(_applicationContextName);
        Thread.SetData(slot, value);
      }
    }
  }
}
