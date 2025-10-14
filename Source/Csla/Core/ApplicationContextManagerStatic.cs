//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManagerStatic.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default context manager for the user property</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using System.Security.Principal;

namespace Csla.Core
{
  /// <summary>
  /// Context manager for the user property
  /// and local/client/global context dictionaries
  /// that uses static fields to maintain 
  /// per-thread context.
  /// </summary>
  public class ApplicationContextManagerStatic : IContextManager
  {
    /// <summary>
    /// Gets a value indicating whether the context manager
    /// is stateful.
    /// </summary>
    public bool IsStatefulContext => true;

    /// <summary>
    /// Returns a value indicating whether the context is valid.
    /// </summary>
    public bool IsValid => true;

    private static IContextDictionary? _currentLocalContext = new ContextDictionary();
    private static IContextDictionary? _currentClientContext = new ContextDictionary();
    private static IPrincipal _currentPrincipal = new ClaimsPrincipal();
    private static IServiceProvider? _currentDefaultServiceProvider;
    private static IServiceProvider? _currentServiceProvider;

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    /// <param name="executionLocation"></param>
    public IContextDictionary? GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return _currentClientContext;
    }

    /// <summary>
    /// Gets the default IServiceProvider
    /// </summary>
    public IServiceProvider? GetDefaultServiceProvider()
    {
      return _currentDefaultServiceProvider;
    }

    /// <summary>
    /// Gets the local context dictionary.
    /// </summary>
    public IContextDictionary? GetLocalContext()
    {
      return _currentLocalContext;
    }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    /// <returns>The current user principal</returns>
    public IPrincipal GetUser()
    {
      return _currentPrincipal;
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      _currentClientContext = clientContext;
    }

    /// <summary>
    /// Sets the default IServiceProvider
    /// </summary>
    /// <param name="serviceProvider">IServiceProvider instance</param>
    /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    public void SetDefaultServiceProvider(IServiceProvider serviceProvider)
    {
      _currentDefaultServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(IContextDictionary? localContext)
    {
      _currentLocalContext = localContext;
    }

    /// <summary>
    /// Gets the service provider for current scope
    /// </summary>
    public IServiceProvider? GetServiceProvider()
    {
      return _currentServiceProvider ?? GetDefaultServiceProvider();
    }

    /// <summary>
    /// Sets the service provider for current scope
    /// </summary>
    /// <param name="scope">IServiceProvider instance</param>
    /// <exception cref="ArgumentNullException"><paramref name="scope"/> is <see langword="null"/>.</exception>
    public void SetServiceProvider(IServiceProvider scope)
    {
      _currentServiceProvider = scope ?? throw new ArgumentNullException(nameof(scope));
    }

    /// <inheritdoc />
    public void SetUser(IPrincipal principal)
    {
      _currentPrincipal = principal ?? throw new ArgumentNullException(nameof(principal));
    }

    private static ApplicationContext? _applicationContext;

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext? ApplicationContext { get => _applicationContext; set => _applicationContext = value; }
  }
}