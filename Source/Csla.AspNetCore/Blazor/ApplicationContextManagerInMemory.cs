﻿#if NET5_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManagerInMemory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager that uses HttpContextAccessor</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Security.Principal;

namespace Csla.AspNetCore.Blazor
{
  /// <summary>
  /// Application context manager that uses HttpContextAccessor when 
  /// resolving HttpContext to store context values.
  /// </summary>
  public class ApplicationContextManagerInMemory : IContextManager, IDisposable
  {
    private ContextDictionary LocalContext { get; set; }
    private ContextDictionary ClientContext { get; set; }
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
    public ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Gets the active circuit state.
    /// </summary>
    protected ActiveCircuitState ActiveCircuitState { get; }

    /// <summary>
    /// Creates an instance of the object, initializing it
    /// with the required IServiceProvider.
    /// </summary>
    /// <param name="authenticationStateProvider">AuthenticationStateProvider service</param>
    /// <param name="activeCircuitState"></param>
    public ApplicationContextManagerInMemory(AuthenticationStateProvider authenticationStateProvider, ActiveCircuitState activeCircuitState)
    {
      AuthenticationStateProvider = authenticationStateProvider;
      ActiveCircuitState = activeCircuitState;
      CurrentPrincipal = UnauthenticatedPrincipal;
      AuthenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
      _ = InitializeUser();
    }

    private async Task InitializeUser()
    {
      Task<AuthenticationState> task = default;
      try
      {
        task = AuthenticationStateProvider.GetAuthenticationStateAsync();
        await task;
      }
      catch (InvalidOperationException ex)
      {
        task = Task.FromResult(new AuthenticationState(UnauthenticatedPrincipal));

        string message = ex.Message;
        //see ms error https://github.com/dotnet/aspnetcore/blob/87e324a61dcd15db4086b8a8ca7bd74ca1e0a513/src/Components/Server/src/Circuits/ServerAuthenticationStateProvider.cs#L16
        //not much safe to test on except the error type and the use of this method name in message.
        if (message.Contains(nameof(AuthenticationStateProvider.GetAuthenticationStateAsync)))
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
      get { return ActiveCircuitState.CircuitExists; }
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
        if (principal is ClaimsPrincipal claimsPrincipal)
        {
          CurrentPrincipal = principal;
          SetHostPrincipal(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
        else
        {
          throw new ArgumentException("typeof(principal) != ClaimsPrincipal");
        }
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
      if (LocalContext == null)
        LocalContext = new ContextDictionary();
      return LocalContext;
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      LocalContext = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      if (ClientContext == null)
        ClientContext = new ContextDictionary();
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