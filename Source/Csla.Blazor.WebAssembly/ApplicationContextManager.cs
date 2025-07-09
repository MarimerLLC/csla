//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager that uses HttpContextAccessor</summary>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using Csla.Blazor.State;
using Csla.Core;
using Csla.State;
using Microsoft.AspNetCore.Components.Authorization;

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
  /// <exception cref="ArgumentNullException"><paramref name="authenticationStateProvider"/> is <see langword="null"/>.</exception>
  public ApplicationContextManager(AuthenticationStateProvider authenticationStateProvider)
  {
    AuthenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
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
  public ApplicationContext? ApplicationContext { get; set; }

  [MemberNotNull(nameof(AuthenticationState))]
  private void InitializeUser()
  {
    AuthenticationStateProvider_AuthenticationStateChanged(AuthenticationStateProvider.GetAuthenticationStateAsync());
  }

  [MemberNotNull(nameof(AuthenticationState))]
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
    ThrowIoeIfApplicationContextIsNull();

    var session = GetSession();
    IContextDictionary localContext;
    if (session.TryGetValue("localContext", out var result) && result is IContextDictionary context)
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
  public void SetLocalContext(IContextDictionary? localContext)
  {
    ThrowIoeIfApplicationContextIsNull();

    var session = GetSession();
    session["localContext"] = localContext;
  }

  /// <summary>
  /// Gets the client context.
  /// </summary>
  /// <param name="executionLocation"></param>
  public IContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
  {
    ThrowIoeIfApplicationContextIsNull();

    IContextDictionary clientContext;
    var session = GetSession();
    if (session.TryGetValue("clientContext", out var result) && result is IContextDictionary context)
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
  public void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation)
  {
    var session = GetSession();
    session["clientContext"] = clientContext;
  }

  private Session GetSession()
  {
    const string SessionRetrievalHint = $"await {nameof(StateManager)}.{nameof(StateManager.InitializeAsync)}()";
    ThrowIoeIfApplicationContextIsNull();
    return ApplicationContext.GetRequiredService<ISessionManager>().GetCachedSession() ?? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.WasmApplicationContextManagerSessionNotRetrieved, SessionRetrievalHint));
  }

  [MemberNotNull(nameof(ApplicationContext))]
  private void ThrowIoeIfApplicationContextIsNull() => _ = ApplicationContext ?? throw new InvalidOperationException($"{nameof(ApplicationContext)} == null");

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