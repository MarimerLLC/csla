//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runtime support for source-generated data portal dispatch</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Properties;
using Csla.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Server
{
  /// <summary>
  /// Runtime support used by source-generated data portal operation
  /// dispatch code. Not intended to be called directly.
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DataPortalOperationHelper
  {
    /// <summary>
    /// Throws when a synchronous data portal call on the client
    /// tries to invoke an asynchronous operation method, matching
    /// the behavior of reflection-based dispatch.
    /// </summary>
    /// <param name="target">Business object declaring the method.</param>
    /// <param name="isSync">True if the client made a synchronous call.</param>
    /// <param name="serviceProvider">Current service provider.</param>
    /// <param name="methodName">Name of the operation method.</param>
    public static void ThrowIfAsyncMethodOnSyncClient(object target, bool isSync, IServiceProvider serviceProvider, string methodName)
    {
      if (!isSync)
        return;

      if (serviceProvider?.GetService(typeof(ApplicationContext)) is ApplicationContext applicationContext
        && applicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Server)
        return;

      throw new CallMethodException(
        target.GetType().Name + "." + methodName + " " + Resources.MethodCallFailed,
        new NotSupportedException(string.Format(Resources.AsyncMethodOnSyncClientNotAllowed, methodName)));
    }

    /// <summary>
    /// Resolves a service for an injected operation method parameter.
    /// </summary>
    /// <param name="serviceProvider">Current service provider.</param>
    /// <param name="serviceType">Type of service.</param>
    /// <param name="allowNull">True if the parameter allows a null value.</param>
    /// <exception cref="InvalidOperationException">The service is required and not registered.</exception>
    public static object? GetService(IServiceProvider serviceProvider, Type serviceType, bool allowNull)
    {
      if (serviceProvider is null)
        throw new ArgumentNullException(nameof(serviceProvider));

      return allowNull
        ? serviceProvider.GetService(serviceType)
        : serviceProvider.GetRequiredService(serviceType);
    }

    /// <summary>
    /// Resolves a keyed service for an injected operation method parameter.
    /// </summary>
    /// <param name="serviceProvider">Current service provider.</param>
    /// <param name="serviceType">Type of service.</param>
    /// <param name="serviceKey">Service key.</param>
    /// <param name="allowNull">True if the parameter allows a null value.</param>
    /// <exception cref="InvalidOperationException">The service is required and not registered, or the provider does not support keyed services.</exception>
    public static object? GetKeyedService(IServiceProvider serviceProvider, Type serviceType, object? serviceKey, bool allowNull)
    {
      if (serviceProvider is null)
        throw new ArgumentNullException(nameof(serviceProvider));

      if (allowNull)
      {
        if (serviceProvider is IKeyedServiceProvider keyedProvider)
          return keyedProvider.GetKeyedService(serviceType, serviceKey);
        throw new InvalidOperationException("Service provider must implement IKeyedServiceProvider to support keyed services.");
      }

      return serviceProvider.GetRequiredKeyedService(serviceType, serviceKey);
    }
  }
}
