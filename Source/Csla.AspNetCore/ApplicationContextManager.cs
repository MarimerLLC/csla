//-----------------------------------------------------------------------
// <copyright file="ApplicationContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Facade application context manager that adapts for AspNetCore/SSB by selecting the correct actual IContextManager based on context</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using System;
using Microsoft.AspNetCore.Http;
#if NET5_0_OR_GREATER
using Csla.AspNetCore.Blazor;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Csla.AspNetCore
{
  /// <summary>
  /// Application context manager that adapts for AspNetCore/SSB.
  /// </summary>
  public class ApplicationContextManager : ApplicationContextManagerFacade
  {
#if NET5_0_OR_GREATER
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="activeCircuitState"></param>
    /// <param name="httpContextAccessor"></param>
    public ApplicationContextManager(IServiceProvider provider, ActiveCircuitState activeCircuitState, IHttpContextAccessor httpContextAccessor, Channels.Local.LocalProxyState localProxyState)
    {
      if (localProxyState.NewScopeExists)
      {
        ActiveContextManager = (IContextManager)ActivatorUtilities.CreateInstance(provider, typeof(ApplicationContextManagerAsyncLocal));
      }
      else if (activeCircuitState.CircuitExists)
      {
        ActiveContextManager = (IContextManager)ActivatorUtilities.CreateInstance(provider, typeof(ApplicationContextManagerBlazor));
      }
      else if (httpContextAccessor.HttpContext is not null)
      {
        ActiveContextManager = (IContextManager)ActivatorUtilities.CreateInstance(provider, typeof(ApplicationContextManagerHttpContext));
      }
      else
      {
        ActiveContextManager = (IContextManager)ActivatorUtilities.CreateInstance(provider, typeof(ApplicationContextManagerAsyncLocal));
      }
    }
#else
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public ApplicationContextManager(IHttpContextAccessor httpContextAccessor)
    {
      ActiveContextManager = new ApplicationContextManagerHttpContext(httpContextAccessor);
    }
#endif

  }
}