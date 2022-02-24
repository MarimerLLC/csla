//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManagerFacade.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Abstract context manager that stateful environment ApplicationContextManagers can inherit from that conditionally create and set an ActiveContextManager</summary>
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
  public abstract class ApplicationContextManagerFacade : IContextManager
  {
    protected IContextManager ActiveContextManager { get; set; }

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
    /// Gets a value indicating whether the current runtime
    /// is stateful (e.g. WPF, Blazor, etc.)
    /// </summary>
    public bool IsStatefulRuntime => ActiveContextManager.IsStatefulRuntime;

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
