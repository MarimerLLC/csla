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
    private readonly IContextManager? _contextManager;
    private readonly IContextManager _localContextManager;
    
    internal IServiceProvider ServiceProvider { get; }
    
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
      Guard.NotNull(contextManagerList);

      ServiceProvider = Guard.NotNull(serviceProvider);
      _localContextManager = Guard.NotNull(localContextManager);

      var managers = contextManagerList.ToList();
      for (int i = managers.Count - 1; i >= 0; i--)
      {
        if (managers[i].IsValid)
        {
          _contextManager = managers[i];
          break;
        }
      }
    }

    /// <summary>
    /// Gets a reference to the correct current application
    /// context manager instance depending on runtime environment.
    /// </summary>
    public IContextManager GetContextManager()
    {
      var runtimeInfo = ServiceProvider.GetRequiredService<IRuntimeInfo>();
      if (_contextManager != null && !runtimeInfo.LocalProxyNewScopeExists && _contextManager.IsValid)
        return _contextManager;
      else
        return _localContextManager;
    }
  }
}
