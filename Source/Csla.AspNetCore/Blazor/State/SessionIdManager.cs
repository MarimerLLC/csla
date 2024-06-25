﻿//-----------------------------------------------------------------------
// <copyright file="SessionIdManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages the per-user id value</summary>
//-----------------------------------------------------------------------

using Csla.State;
using Microsoft.AspNetCore.Http;

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
    public string GetSessionId()
    {
      const string sessionIdName = "cslaSessionId";
      var httpContext = HttpContextAccessor.HttpContext;
      string? result;

      if (httpContext == null)
        throw new InvalidOperationException("HttpContext == null");

      if (httpContext.Request.Cookies.ContainsKey(sessionIdName))
      {
        result = httpContext.Request.Cookies[sessionIdName];
      }
      else if (httpContext.Items.TryGetValue(sessionIdName, out var item))
      {
        result = item as string;
      }
      else
      {
        result = Guid.NewGuid().ToString();
        httpContext.Response.Cookies.Append(sessionIdName, result);
        httpContext.Items[sessionIdName] = result;
      }

      return result ?? throw new InvalidOperationException(Csla.Properties.Resources.SessionIdManagerIdMustBeNotNull);
    }
  }
}