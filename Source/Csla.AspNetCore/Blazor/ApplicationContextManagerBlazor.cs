#if NET5_0_OR_GREATER
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
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Csla.AspNetCore.Blazor
{
  /// <summary>
  /// Application context manager that adapts how to manage
  /// per-user state and identity for server-rendered and
  /// server-interactive Blazor modes.
  /// </summary>
  public class ApplicationContextManagerBlazor : IContextManager, IDisposable
  {
    /// <summary>
    /// Creates an instance of the object, initializing it
    /// with the required IServiceProvider.
    /// </summary>
    /// <param name="hca">IHttpContextAccessor service</param>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider service</param>
    /// <param name="activeCircuitState"></param>
    /// <param name="sessionManager"></param>
    public ApplicationContextManagerBlazor(IHttpContextAccessor hca, AuthenticationStateProvider authenticationStateProvider, ActiveCircuitState activeCircuitState, ISessionManager sessionManager)
    {
      _sessionManager = sessionManager;
      HttpContext = hca.HttpContext;
      AuthenticationStateProvider = authenticationStateProvider;
      ActiveCircuitState = activeCircuitState;
      CurrentPrincipal = UnauthenticatedPrincipal;
      AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
      InitializeUser();
    }

    private ContextDictionary LocalContext { get; set; }
    private ContextDictionary ClientContext { get; set; }
    private IPrincipal CurrentPrincipal { get; set; }
    private readonly ClaimsPrincipal UnauthenticatedPrincipal = new();
    private bool disposedValue;
    private readonly ISessionManager _sessionManager;

    /// <summary>
    /// Gets the current HttpContext instance.
    /// </summary>
    protected AuthenticationStateProvider AuthenticationStateProvider { get; private set; }

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Gets the active circuit state.
    /// </summary>
    protected ActiveCircuitState ActiveCircuitState { get; private set; }

    /// <summary>
    /// Gets or sets a reference to the current HttpContext.
    /// </summary>
    protected HttpContext HttpContext { get; set; }

    private void InitializeUser()
    {
      if (ActiveCircuitState.CircuitExists)
      {
        Task<AuthenticationState> task;
        try
        {
          task = AuthenticationStateProvider.GetAuthenticationStateAsync();
        }
        catch (InvalidOperationException ex)
        {
          task = Task.FromResult(new AuthenticationState((ClaimsPrincipal)UnauthenticatedPrincipal));
          string message = ex.Message;
          if (message.Contains(nameof(AuthenticationStateProvider.GetAuthenticationStateAsync))
              && message.Contains(nameof(IHostEnvironmentAuthenticationStateProvider.SetAuthenticationState)))
          {
            SetHostPrincipal(task);
          }
          else
          {
            throw;
          }
        }
        AuthenticationStateProvider_AuthenticationStateChanged(task);
      }
      else if (HttpContext is not null)
      {
        CurrentPrincipal = HttpContext.User;
      }
      else
      {
        throw new InvalidOperationException("HttpContext==null, !CircuitExists");
      }
    }

    private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
    {
      if (task is null)
      {
        CurrentPrincipal = UnauthenticatedPrincipal;
      }
      else
      {
        task.ContinueWith((t) =>
        {
          if (task.IsCompletedSuccessfully && task.Result != null)
            CurrentPrincipal = task.Result.User;
          else
            CurrentPrincipal = UnauthenticatedPrincipal;
        });
      }
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid
    {
      get { return HttpContext is not null || ActiveCircuitState.CircuitExists; }
    }

    /// <summary>
    /// Gets a value indicating whether the context manager
    /// is stateful.
    /// </summary>
    public bool IsStatefulContext => true;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public IPrincipal GetUser()
    {
      return CurrentPrincipal;
    }

    /// <summary>
    /// Attempts to set the current principal on the registered
    /// IHostEnvironmentAuthenticationStateProvider service.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public virtual void SetUser(IPrincipal principal)
    {
      if (!ReferenceEquals(CurrentPrincipal, principal))
      {
        if (ActiveCircuitState.CircuitExists)
        {
          if (principal is ClaimsPrincipal claimsPrincipal)
          {
            SetHostPrincipal(Task.FromResult(new AuthenticationState(claimsPrincipal)));
          }
          else
          {
            throw new ArgumentException("typeof(principal) != ClaimsPrincipal");
          }
        }
        else if (HttpContext is not null)
        {
          HttpContext.User = (ClaimsPrincipal)principal;
        }
        else
        {
          throw new InvalidOperationException("HttpContext==null, !CircuitExists");
        }
        CurrentPrincipal = principal;
      }
    }

    private void SetHostPrincipal(Task<AuthenticationState> task)
    {
      if (AuthenticationStateProvider is IHostEnvironmentAuthenticationStateProvider hostProvider)
        hostProvider.SetAuthenticationState(task);
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      if (LocalContext is null)
      {
        var session = _sessionManager.GetSession().Result;
        session.TryGetValue("localContext", out var result);
        if (result is ContextDictionary context)
          LocalContext = context;
        else
          LocalContext = [];
        SetLocalContext(LocalContext);
      }
      return LocalContext;
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      LocalContext = localContext;
      var session = _sessionManager.GetSession().Result;
      session["localContext"] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      if (ClientContext is null)
      {
        var session = _sessionManager.GetSession().Result;
        session.TryGetValue("clientContext", out var result);
        if (result is ContextDictionary context)
          ClientContext = context;
        else
          ClientContext = [];
        SetClientContext(ClientContext, ApplicationContext.ExecutionLocation);
      }
      return ClientContext;
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      ClientContext = clientContext;
      var session = _sessionManager.GetSession().Result;
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
#endif