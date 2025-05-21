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
using System.Security.Claims;
using System.Security.Principal;

namespace Csla.Blazor.WebAssembly;

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
    AuthenticationState = task;
  }

  /// <summary>
  /// Gets a value indicating whether this
  /// context manager is valid for use in
  /// the current environment.
  /// </summary>
  public bool IsValid => true;

  /// <summary>
  /// Gets a value indicating whether the current runtime
  /// is stateful (e.g. WPF, Blazor, etc.)
  /// </summary>
  public bool IsStatefulContext => true;

  private static readonly IPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

  /// <summary>
  /// Gets the current principal.
  /// </summary>
  public IPrincipal GetUser()
  {
    IPrincipal result;
    if (AuthenticationState.IsCompletedSuccessfully && AuthenticationState.Result != null)
      result = AuthenticationState.Result.User;
    else
      result = _anonymous;
    return result;
  }

  /// <summary>
  /// NOT SUPPORTED: 
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
  public IContextDictionary GetLocalContext()
  {
    IContextDictionary localContext;
    var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
    var session = sessionManager.GetCachedSession();
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
  public void SetLocalContext(IContextDictionary localContext)
  {
    var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
    var session = sessionManager.GetCachedSession();
    session["localContext"] = localContext;
  }

  /// <summary>
  /// Gets the client context.
  /// </summary>
  /// <param name="executionLocation"></param>
  public IContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
  {
    IContextDictionary clientContext;
    var sessionManager = ApplicationContext.GetRequiredService<ISessionManager>();
    var session = sessionManager.GetCachedSession();
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
  public void SetClientContext(IContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation)
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