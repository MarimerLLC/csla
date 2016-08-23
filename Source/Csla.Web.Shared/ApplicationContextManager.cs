//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Application context manager that uses HttpContext</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
#if MVC6
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace Csla.Web
{
  /// <summary>
  /// Application context manager that uses HttpContext
  /// to store context values.
  /// </summary>
  public class ApplicationContextManager : IContextManager
  {
    private const string _localContextName = "Csla.LocalContext";
    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

#if MVC6
    private static IHttpContextAccessor _httpContextAccessor;
    /// <summary>
    /// Configures the context manager for use within
    /// ASP.NET MVC 6 environments.
    /// </summary>
    /// <param name="httpContextAccessor">The ASP.NET MVC context accessor object.</param>
    /// <remarks>
    /// Technique from
    /// http://www.spaprogrammer.com/2015/07/mvc-6-httpcontextcurrent.html
    /// </remarks>
    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }
#endif

    /// <summary>
    /// Gets a reference to the current HttpContext
    /// object.
    /// </summary>
    public HttpContext CurrentHttpContext
    {
#if MVC6
      get
      {
        if (_httpContextAccessor == null)
          throw new NullReferenceException("HttpContextAccessor");
        return _httpContextAccessor.HttpContext;
      }
#else
      get { return HttpContext.Current; }
#endif
    }

    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    public bool IsValid 
    { 
      get { return CurrentHttpContext != null; }
    }

    /// <summary>
    /// Gets the current principal.
    /// </summary>
    public System.Security.Principal.IPrincipal GetUser()
    {
      return CurrentHttpContext.User;
    }

    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    public void SetUser(System.Security.Principal.IPrincipal principal)
    {
#if MVC6
      CurrentHttpContext.User = (System.Security.Claims.ClaimsPrincipal)principal;
#else
      CurrentHttpContext.User = principal;
#endif
    }

    /// <summary>
    /// Gets the local context.
    /// </summary>
    public ContextDictionary GetLocalContext()
    {
      return (ContextDictionary)CurrentHttpContext.Items[_localContextName];
    }

    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    public void SetLocalContext(ContextDictionary localContext)
    {
      CurrentHttpContext.Items[_localContextName] = localContext;
    }

    /// <summary>
    /// Gets the client context.
    /// </summary>
    public ContextDictionary GetClientContext()
    {
      return (ContextDictionary)CurrentHttpContext.Items[_clientContextName];
    }

    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    public void SetClientContext(ContextDictionary clientContext)
    {
      CurrentHttpContext.Items[_clientContextName] = clientContext;
    }

    /// <summary>
    /// Gets the global context.
    /// </summary>
    public ContextDictionary GetGlobalContext()
    {
      return (ContextDictionary)CurrentHttpContext.Items[_globalContextName];
    }

    /// <summary>
    /// Sets the global context.
    /// </summary>
    /// <param name="globalContext">Global context.</param>
    public void SetGlobalContext(ContextDictionary globalContext)
    {
      CurrentHttpContext.Items[_globalContextName] = globalContext;
    }
  }
}