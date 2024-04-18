//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager that uses HttpContextAccessor</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.State;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Csla.Blazor.WebAssembly
{
  /// <summary>
  /// Application context manager that uses HttpContextAccessor when 
  /// resolving HttpContext to store context values.
  /// </summary>
  public class ApplicationContextManager : IContextManager, IDisposable
  {
    /// <summary>
    /// Creates an instance of the object, initializing it
    /// with the required IServiceProvider.
    /// </summary>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider service</param>
    public ApplicationContextManager(AuthenticationStateProvider authenticationStateProvider)
    {
      AuthenticationStateProvider = authenticationStateProvider;
      AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
      InitializeUser();
    }

    private Task<AuthenticationState> AuthenticationState { get; set; }
    private ClaimsPrincipal _currentPrincipal;
    private bool disposedValue;

    /// <summary>
    /// Gets the current AuthenticationStateProvider instance.
    /// </summary>
    protected AuthenticationStateProvider AuthenticationStateProvider { get; }

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext ApplicationContext { get; set; }

    private void InitializeUser()
    {
      AuthenticationStateProvider_AuthenticationStateChanged(AuthenticationStateProvider.GetAuthenticationStateAsync());
    }

    private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
      _currentPrincipal = null;
      AuthenticationState = task;
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid
    {
      get { return true; }
    }

    /// <summary>
    /// Gets a value indicating whether the current runtime
    /// is stateful (e.g. WPF, Blazor, etc.)
    /// </summary>
    public bool IsStatefulContext => true;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public IPrincipal GetUser()
    {
      if (_currentPrincipal == null)
      {
        if (AuthenticationState.IsCompleted && AuthenticationState.Result != null)
          _currentPrincipal = AuthenticationState.Result.User;
        else
          _currentPrincipal = new ClaimsPrincipal();
      }
      return _currentPrincipal;
    }

    /// <summary>
    /// Sets the current principal ONLY IN APPLICATIONCONTEXT.
    /// To set the value correctly, use your specific
    /// ApplicationContextStateProvider implementation.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public virtual void SetUser(IPrincipal principal)
    {
      throw new NotSupportedException(nameof(SetUser));
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      ContextDictionary localContext;
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetCachedSession();
      session.TryGetValue("localContext", out var result);
      if (result is ContextDictionary context)
      {
        localContext = context;
      }
      else
      {
        localContext = [];
        SetLocalContext(localContext);
      }
      return localContext;
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetCachedSession();
      session["localContext"] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      ContextDictionary clientContext;
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetCachedSession();
      session.TryGetValue("clientContext", out var result);
      if (result is ContextDictionary context)
      {
        clientContext = context;
      }
      else
      {
        clientContext = [];
        SetClientContext(clientContext, ApplicationContext.ExecutionLocation);
      }
      return clientContext;
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetCachedSession();
      session["clientContext"] = clientContext;
    }

    /// <summary>
    /// Dispose this object's resources.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          AuthenticationStateProvider.AuthenticationStateChanged -= AuthenticationStateProvider_AuthenticationStateChanged;
        }
        disposedValue = true;
      }
    }

    /// <summary>
    /// Dispose this object's resources.
    /// </summary>
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}