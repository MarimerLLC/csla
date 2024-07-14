//-----------------------------------------------------------------------
// <copyright file="ApplicationContextAccessor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides access to the correct current application</summary>
//-----------------------------------------------------------------------

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
    /// <exception cref="ArgumentNullException"><paramref name="contextManagerList"/>, <paramref name="localContextManager"/> or <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    public ApplicationContextAccessor(
      IEnumerable<IContextManager> contextManagerList, 
      IContextManagerLocal localContextManager, 
      IServiceProvider serviceProvider)
    {
      if (contextManagerList is null)
        throw new ArgumentNullException(nameof(contextManagerList));

      ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      LocalContextManager = localContextManager ?? throw new ArgumentNullException(nameof(localContextManager));

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
        ContextManager = new ApplicationContextManagerAsyncLocal();
      }
    }

    internal IServiceProvider ServiceProvider { get; }
    private IContextManager ContextManager { get; }
    private IContextManager LocalContextManager { get; }

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
