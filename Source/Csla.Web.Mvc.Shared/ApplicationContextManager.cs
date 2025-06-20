#if NET462
//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Application context manager that uses HttpContext</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using System.Web;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Application context manager that uses HttpContext
  /// to store context values.
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid => HttpContext.Current != null;

    /// <summary>
    /// Gets a value indicating whether the current runtime
    /// is stateful (e.g. WPF, Blazor, etc.)
    /// </summary>
    public bool IsStatefulContext => false;

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public System.Security.Principal.IPrincipal GetUser()
    {
      var result = HttpContext.Current.User;
      if (result == null)
      {
        result = new System.Security.Claims.ClaimsPrincipal();
        SetUser(result);
      }
      return result;
    }

    /// <inheritdoc />
    public void SetUser(System.Security.Principal.IPrincipal principal)
    {
      HttpContext.Current.User = principal ?? throw new ArgumentNullException(nameof(principal));
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public IContextDictionary GetLocalContext()
    {
      return (IContextDictionary)HttpContext.Current.Items[_localContextName];
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(IContextDictionary? localContext)
    {
      HttpContext.Current.Items[_localContextName] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    public IContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation)
    {
      return (IContextDictionary)HttpContext.Current.Items[_clientContextName];
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    public void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation)
    {
      HttpContext.Current.Items[_clientContextName] = clientContext;
    }
    private const string _applicationContextName = "Csla.ApplicationContext";

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    public virtual ApplicationContext? ApplicationContext
    {
      get => (ApplicationContext?)HttpContext.Current.Items[_applicationContextName];
      set => HttpContext.Current.Items[_applicationContextName] = value;
    }

    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    public static ApplicationContext GetApplicationContext()
    {
      return (ApplicationContext)HttpContext.Current.Items[_applicationContextName];
    }
  }
}
#endif