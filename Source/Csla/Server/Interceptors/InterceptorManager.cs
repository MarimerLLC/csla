//-----------------------------------------------------------------------
// <copyright file="InterceptorManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Cascades DataPortal interception requests to registered interceptors</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace Csla.Server
{
  /// <summary>
  /// Manage dataportal interception using DI-registered implementations
  /// </summary>
  public class InterceptorManager
  {
    private readonly IReadOnlyList<IInterceptDataPortal> _interceptors;

    /// <summary>
    /// Creation of the manager, including all interceptors registered with the DI container
    /// </summary>
    /// <param name="interceptors">The IEnumerable of interceptors provided by DI</param>
    public InterceptorManager(IEnumerable<IInterceptDataPortal> interceptors)
    {
      _interceptors = new List<IInterceptDataPortal>(interceptors);
    }

    /// <summary>
    /// Cascade the initial interception request prior to the main DataPortal operation
    /// </summary>
    /// <param name="e">The interception arguments provided by the consumer</param>
    public void Initialize(InterceptArgs e)
    {
      foreach (IInterceptDataPortal interceptor in _interceptors)
      {
        interceptor.Initialize(e);
      }
    }

    /// <summary>
    /// Cascade the final interception request after the main DataPortal operation has completed
    /// </summary>
    /// <param name="e">The interception arguments provided by the consumer</param>
    public void Complete(InterceptArgs e)
    {
      // Iterate backwards through interceptors, so that they appear to wrap one another, decorator-style
      for (int interceptorIndex = _interceptors.Count - 1; interceptorIndex > -1; interceptorIndex--)
      {
        IInterceptDataPortal interceptor = _interceptors[interceptorIndex];
        interceptor.Complete(e);
      }
    }
  }
}
