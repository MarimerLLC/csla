//-----------------------------------------------------------------------
// <copyright file="InterceptorManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Cascades DataPortal interception requests to registered interceptors</summary>
//-----------------------------------------------------------------------


namespace Csla.Server
{
  /// <summary>
  /// Manage data portal interception using DI-registered implementations
  /// </summary>
  public class InterceptorManager
  {
    private readonly IReadOnlyList<IInterceptDataPortal> _interceptors;

    /// <summary>
    /// Creation of the manager, including all interceptors registered with the DI container
    /// </summary>
    /// <param name="interceptors">The IEnumerable of interceptors provided by DI</param>
    /// <exception cref="ArgumentNullException"><paramref name="interceptors"/> is <see langword="null"/>.</exception>
    public InterceptorManager(IEnumerable<IInterceptDataPortal> interceptors)
    {
      Guard.NotNull(interceptors);

      _interceptors = new List<IInterceptDataPortal>(interceptors);
    }

    /// <inheritdoc />
    public async Task InitializeAsync(InterceptArgs e)
    {
      Guard.NotNull(e);

      foreach (IInterceptDataPortal interceptor in _interceptors)
      {
        await interceptor.InitializeAsync(e);
      }
    }

    /// <inheritdoc />
    public void Complete(InterceptArgs e)
    {
      Guard.NotNull(e);

      // Iterate backwards through interceptors, so that they appear to wrap one another, decorator-style
      for (int interceptorIndex = _interceptors.Count - 1; interceptorIndex > -1; interceptorIndex--)
      {
        IInterceptDataPortal interceptor = _interceptors[interceptorIndex];
        interceptor.Complete(e);
      }
    }
  }
}
