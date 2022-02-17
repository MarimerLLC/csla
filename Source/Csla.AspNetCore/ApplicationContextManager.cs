//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager that adapts for AspNetCore/SSB</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using System;
using Microsoft.AspNetCore.Http;
#if NET5_0_OR_GREATER
using Csla.AspNetCore.Blazor;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Csla.AspNetCore
{
  /// <summary>
  /// Application context manager that adapts for AspNetCore/SSB.
  /// </summary>
  public class ApplicationContextManager : IContextManager, IDisposable
  {
    private IContextManager ActiveContextManager { get; set; }

#if NET5_0_OR_GREATER
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="activeCircuitState"></param>
    /// <param name="httpContextAccessor"></param>
    public ApplicationContextManager(IServiceProvider provider, Core.Blazor.ActiveCircuitState activeCircuitState, IHttpContextAccessor httpContextAccessor)
    {
      if (activeCircuitState.CircuitExists || httpContextAccessor.HttpContext is null)
      {
        ActiveContextManager = (IContextManager)ActivatorUtilities.CreateInstance(provider, typeof(ApplicationContextManagerBlazor));
      }
      else
      {
        ActiveContextManager = (IContextManager)ActivatorUtilities.CreateInstance(provider, typeof(ApplicationContextManagerHttpContext));
      }
    }
#else
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public ApplicationContextManager(IHttpContextAccessor httpContextAccessor)
    {
      ActiveContextManager = new ApplicationContextManagerHttpContext(httpContextAccessor);
    }
#endif

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid
    {
      get { return ActiveContextManager.IsValid; }
    }

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public System.Security.Principal.IPrincipal GetUser()
    {
      return ActiveContextManager.GetUser();
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public void SetUser(System.Security.Principal.IPrincipal principal)
    {
      ActiveContextManager.SetUser(principal);
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      return ActiveContextManager.GetLocalContext();
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      ActiveContextManager.SetLocalContext(localContext);
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return ActiveContextManager.GetClientContext(executionLocation);
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      ActiveContextManager.SetClientContext(clientContext, executionLocation);
    }

    private bool disposedValue;

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public virtual ApplicationContext ApplicationContext
    {
      get
      {
        return ActiveContextManager.ApplicationContext;
      }
      set
      {
        ActiveContextManager.ApplicationContext = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          if (ActiveContextManager is IDisposable disposable)
            disposable.Dispose();
        }

        disposedValue = true;
      }
    }

    /// <summary>
    /// Dispose this instance.
    /// </summary>
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}