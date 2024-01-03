//-----------------------------------------------------------------------
// <copyright file="SessionIdManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages the per-user id value</summary>
//-----------------------------------------------------------------------
using System;
using Csla.State;
using Microsoft.AspNetCore.Http;

#nullable enable
namespace Csla.Blazor.State
{
  /// <summary>
  /// Manages the per-user id value
  /// for state management using a
  /// browser cookie and Guid value.
  /// </summary>
  /// <param name="httpContextAccessor"></param>
  public class SessionIdManager(IHttpContextAccessor httpContextAccessor) : ISessionIdManager
  {
    private readonly IHttpContextAccessor HttpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Gets the per-user id value
    /// for the current user state.
    /// </summary>
    /// <remarks>
    /// Id is a Guid value. The id value is
    /// maintained in a browser cookie for the
    /// current user.
    /// </remarks>
    public string? GetSessionId()
    {
      const string sessionIdName = "cslaSessionId";
      var httpContext = HttpContextAccessor.HttpContext;
      string? result;

      if (httpContext != null)
      {
        if (httpContext.Request.Cookies.ContainsKey(sessionIdName))
        {
          result = httpContext.Request.Cookies[sessionIdName];
        }
        else
        {
          result = Guid.NewGuid().ToString();
          httpContext.Response.Cookies.Append(sessionIdName, result);
        }
      }
      else
      {
        throw new InvalidOperationException("HttpContext == null");
      }
      return result;
    }
  }
}
#nullable disable