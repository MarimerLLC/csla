//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager using AsyncLocal</summary>
//-----------------------------------------------------------------------
using System.Security.Principal;

namespace Csla.Core
{
  /// <summary>
  /// Application context manager using AsyncLocal
  /// for user and context dictionaries.
  /// </summary>
  public class ApplicationContextManagerAsyncLocal : IContextManagerLocal
  {
    private readonly AsyncLocal<IContextDictionary?> _localContext = new();
    private readonly AsyncLocal<IContextDictionary?> _clientContext = new();
    private readonly AsyncLocal<IPrincipal?> _principal = new();

    /// <summary>
    /// Gets a value indicating whether the context manager
    /// is stateful.
    /// </summary>
    public bool IsStatefulContext => true;

    /// <summary>
    /// Returns a value indicating whether the context is valid.
    /// </summary>
    public bool IsValid
    {
      get { return true; }
    }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    /// <returns>The current user principal</returns>
    public virtual IPrincipal GetUser()
    {
      IPrincipal? result = _principal.Value;
      if (result == null)
      {
        result = new System.Security.Claims.ClaimsPrincipal();
        SetUser(result);
      }
      return result;
    }

    /// <inheritdoc />
    public virtual void SetUser(IPrincipal principal)
    {
      _principal.Value = principal ?? throw new ArgumentNullException(nameof(principal));
    }

    /// <summary>
    /// Gets the local context dictionary.
    /// </summary>
    public IContextDictionary? GetLocalContext()
    {
      return _localContext.Value;
    }

    /// <summary>
    /// Sets the local context dictionary.
    /// </summary>
    /// <param name="localContext">Context dictionary</param>
    public void SetLocalContext(IContextDictionary? localContext)
    {
      _localContext.Value = localContext;
    }

    /// <summary>
    /// Gets the client context dictionary.
    /// </summary>
    /// <param name="executionLocation"></param>
    public IContextDictionary? GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return _clientContext.Value;
    }

    /// <summary>
    /// Sets the client context dictionary.
    /// </summary>
    /// <param name="clientContext">Context dictionary</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      _clientContext.Value = clientContext;
    }

    private readonly AsyncLocal<ApplicationContext?> _applicationContext = new();

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public ApplicationContext? ApplicationContext 
    {
      get
      {
        return _applicationContext.Value;
      }
      set
      {
        _applicationContext.Value = value;
        OnApplicationContextSet();
      }
    }

    /// <summary>
    /// Method called when the ApplicationContext
    /// property has been set to a new value.
    /// </summary>
    protected virtual void OnApplicationContextSet()
    { }
  }
}
