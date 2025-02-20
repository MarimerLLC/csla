//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager that uses HttpContextAccessor</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.Runtime;
using Microsoft.AspNetCore.Http;

namespace Csla.AspNetCore
{
  /// <summary>
  /// Application context manager that uses HttpContextAccessor when 
  /// resolving HttpContext to store context values.
  /// </summary>
  public class ApplicationContextManagerHttpContext : IContextManager
  {

    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";

    private readonly IRuntimeInfo runtimeInfo;


#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets the active circuit state.
    /// </summary>
    protected Blazor.ActiveCircuitState ActiveCircuitState { get; }

    /// <summary>
    /// Creates an instance of the object, initializing it
    /// with the required IServiceProvider.
    /// </summary>
    /// <param name="httpContextAccessor">HttpContext accessor</param>
    /// <param name="runtimeInfo"></param>
    /// <param name="activeCircuitState"></param>
    /// <exception cref="ArgumentNullException"><paramref name="httpContextAccessor"/>, <paramref name="runtimeInfo"/> or <paramref name="activeCircuitState"/> is <see langword="null"/>.</exception>
    public ApplicationContextManagerHttpContext(IHttpContextAccessor httpContextAccessor, IRuntimeInfo runtimeInfo, Blazor.ActiveCircuitState activeCircuitState)
    {
      ArgumentNullException.ThrowIfNull(httpContextAccessor);

      HttpContext = httpContextAccessor.HttpContext;
      this.runtimeInfo = runtimeInfo ?? throw new ArgumentNullException(nameof(runtimeInfo));
      ActiveCircuitState = activeCircuitState ?? throw new ArgumentNullException(nameof(activeCircuitState));
    }
#else
    /// <summary>
    /// Creates an instance of the object, initializing it
    /// with the required IServiceProvider.
    /// </summary>
    /// <param name="httpContextAccessor">HttpContext accessor</param>
    /// <param name="runtimeInfo"></param>
    public ApplicationContextManagerHttpContext(IHttpContextAccessor httpContextAccessor, IRuntimeInfo runtimeInfo)
    {
      HttpContext = httpContextAccessor.HttpContext;
      this.runtimeInfo = runtimeInfo;
    }

#endif


    /// <summary>
    /// Gets the current HttpContext instance.
    /// </summary>
    protected virtual HttpContext? HttpContext { get; }

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    [MemberNotNullWhen(true, nameof(HttpContext))]
    public bool IsValid
    {
      get
      {
        if (HttpContext is null)
          return false;

        if (runtimeInfo.LocalProxyNewScopeExists)
          return false;

#if NET8_0_OR_GREATER
        if (ActiveCircuitState.CircuitExists)
          return false;
#endif

        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the context manager
    /// is stateful.
    /// </summary>
    public bool IsStatefulContext => false;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public System.Security.Principal.IPrincipal GetUser()
    {
      var result = HttpContext?.User;
      if (result == null)
      {
        result = new ClaimsPrincipal();
        SetUser(result);
      }
      return result;
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The underlying <see cref="HttpContext"/> is <see langword="null"/>.</exception>
    public void SetUser(System.Security.Principal.IPrincipal principal)
    {
      ArgumentNullException.ThrowIfNull(principal);
      ThrowIfHttpContextIsNull();
      HttpContext.User = (ClaimsPrincipal)principal;
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public IContextDictionary? GetLocalContext()
    {
      return (IContextDictionary?)HttpContext?.Items[_localContextName];
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    /// <exception cref="InvalidOperationException">The underlying <see cref="HttpContext"/> is <see langword="null"/>.</exception>
    public void SetLocalContext(IContextDictionary? localContext)
    {
      ThrowIfHttpContextIsNull();
      HttpContext.Items[_localContextName] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public IContextDictionary? GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return (IContextDictionary?)HttpContext?.Items[_clientContextName];
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    /// <exception cref="InvalidOperationException">The underlying <see cref="HttpContext"/> is <see langword="null"/>.</exception>
    public void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      ThrowIfHttpContextIsNull();
      HttpContext.Items[_clientContextName] = clientContext;
    }

    private const string _applicationContextName = "Csla.ApplicationContext";

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    /// <exception cref="InvalidOperationException">The underlying <see cref="HttpContext"/> is <see langword="null"/>.</exception>
    public virtual ApplicationContext? ApplicationContext
    {
      get
      {
        return (ApplicationContext?)HttpContext?.Items[_applicationContextName];
      }
      set
      {
        ThrowIfHttpContextIsNull();
        HttpContext.Items[_applicationContextName] = value;
      }
    }

    [MemberNotNull(nameof(HttpContext))]
    private void ThrowIfHttpContextIsNull()
    {
      if (HttpContext is null)
        throw new InvalidOperationException($"{nameof(HttpContext)} == null");
    }
  }
}