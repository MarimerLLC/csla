#if NET8_0_OR_GREATER
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
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;

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
    /// <exception cref="ArgumentNullException"><paramref name="hca"/>, <paramref name="authenticationStateProvider"/> or <paramref name="activeCircuitState"/> is <see langword="null"/>.</exception>
    public ApplicationContextManagerBlazor(IHttpContextAccessor hca, AuthenticationStateProvider authenticationStateProvider, ActiveCircuitState activeCircuitState)
    {
      ArgumentNullException.ThrowIfNull(hca);

      HttpContext = hca.HttpContext;
      AuthenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
      ActiveCircuitState = activeCircuitState ?? throw new ArgumentNullException(nameof(activeCircuitState));
      CurrentPrincipal = UnauthenticatedPrincipal;
      AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
      _ = InitializeUser();
    }

    private IPrincipal CurrentPrincipal { get; set; }
    private readonly ClaimsPrincipal UnauthenticatedPrincipal = new();
    private bool disposedValue;

    /// <summary>
    /// Gets the current HttpContext instance.
    /// </summary>
    protected AuthenticationStateProvider AuthenticationStateProvider { get; }

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext? ApplicationContext { get; set; }

    /// <summary>
    /// Gets the active circuit state.
    /// </summary>
    protected ActiveCircuitState ActiveCircuitState { get; }

    /// <summary>
    /// Gets or sets a reference to the current HttpContext.
    /// </summary>
    protected HttpContext? HttpContext { get; set; }

    private async Task InitializeUser()
    {
      if (HttpContext != null)
      {
        var user = HttpContext.User;
        if (user != null)
          CurrentPrincipal = user;
      }
      else
      {
        Task<AuthenticationState> task;
        try
        {
          task = AuthenticationStateProvider.GetAuthenticationStateAsync();
          await task;
        }
        catch (InvalidOperationException)
        {
          task = Task.FromResult(new AuthenticationState(UnauthenticatedPrincipal));
        }
        AuthenticationStateProvider_AuthenticationStateChanged(task);
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
        task.ContinueWith(t =>
        {
          if (t.IsCompletedSuccessfully && t.Result != null)
            CurrentPrincipal = t.Result.User;
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
    /// Not supported in Blazor.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public virtual void SetUser(IPrincipal? principal) => throw new NotSupportedException(nameof(SetUser));

    /// <summary>
    /// Gets the local context.
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="ApplicationContext"/> is <see langword="null"/>.</exception>
    public IContextDictionary GetLocalContext()
    {
      ThrowIfApplicationContextIsNull();
      IContextDictionary localContext;
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetSession();
      session.TryGetValue("localContext", out var result);
      if (result is IContextDictionary context)
      {
        localContext = context;
      }
      else
      {
        localContext = new ContextDictionary();
        SetLocalContext(localContext);
      }
      return localContext;
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    /// <exception cref="InvalidOperationException"><see cref="ApplicationContext"/> is <see langword="null"/>.</exception>
    public void SetLocalContext(IContextDictionary? localContext)
    {
      ThrowIfApplicationContextIsNull();
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetSession();
      session["localContext"] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    /// <exception cref="InvalidOperationException"><see cref="ApplicationContext"/> is <see langword="null"/>.</exception>
    public IContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      ThrowIfApplicationContextIsNull();
      IContextDictionary clientContext;
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetSession();
      session.TryGetValue("clientContext", out var result);
      if (result is IContextDictionary context)
      {
        clientContext = context;
      }
      else
      {
        clientContext = new ContextDictionary();
        SetClientContext(clientContext, ApplicationContext.ExecutionLocation);
      }
      return clientContext;
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    /// <exception cref="InvalidOperationException"><see cref="ApplicationContext"/> is <see langword="null"/>.</exception>
    public void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      ThrowIfApplicationContextIsNull();
      var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
      var session = sessionManager.GetSession();
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

    [MemberNotNull(nameof(ApplicationContext))]
    private void ThrowIfApplicationContextIsNull()
    {
      if (ApplicationContext is null)
        throw new InvalidOperationException($"{nameof(ApplicationContext)} == null");
    }
  }
}
#endif