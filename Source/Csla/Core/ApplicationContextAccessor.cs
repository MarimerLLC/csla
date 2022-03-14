//-----------------------------------------------------------------------
// <copyright file="ApplicationContextAccessor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides access to the correct current application</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Core
{
  /// <summary>
  /// Provides access to the correct current application
  /// context manager instance depending on runtime environment.
  /// </summary>
  public class ApplicationContextAccessor
  {
    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    /// <param name="contextManagerList"></param>
    /// <param name="localContextManager"></param>
    /// <param name="serviceProvider"></param>
    public ApplicationContextAccessor(
      IEnumerable<IContextManager> contextManagerList, 
      IContextManagerLocal localContextManager, 
      IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider;
      LocalContextManager = localContextManager;

      foreach (var context in contextManagerList)
      {
        if (context.IsValid)
        {
          ContextManager = context;
          break;
        }
      }
      if (ContextManager is null)
      {
        ContextManager = new Core.ApplicationContextManagerAsyncLocal();
      }
    }

    internal IServiceProvider ServiceProvider { get; private set; }
    private IContextManager ContextManager { get; set; }
    private IContextManager LocalContextManager { get; set; }

    /// <summary>
    /// Gets a reference to the correct current application
    /// context manager instance depending on runtime environment.
    /// </summary>
    public IContextManager GetContextManager()
    {
      var runtimeInfo = ServiceProvider.GetRequiredService<IRuntimeInfo>();
      if (!runtimeInfo.LocalProxyNewScopeExists)
        return ContextManager;
      else
        return LocalContextManager;
    }
  }
}
