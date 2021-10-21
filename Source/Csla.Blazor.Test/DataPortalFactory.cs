using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Blazor.Test
{
  internal static class DataPortalFactory
  {

    private static IServiceProvider _serviceProvider;

    /// <summary>
    /// Store the root service provider for use in object creation during tests
    /// </summary>
    /// <param name="serviceProvider">The root service provider that has been created</param>
    internal static void SetServiceProvider(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Create an instance of a typed data portal using the DI container
    /// </summary>
    /// <typeparam name="T">The type which the data portal is to service</typeparam>
    /// <returns>An instance of IDataPortal<typeparamref name="T"/> for use in data access during tests</returns>
    public static IDataPortal<T> CreateDataPortal<T>()
    {
      IDataPortal<T> dataPortal;

      dataPortal = _serviceProvider.GetRequiredService<IDataPortal<T>>();
      return dataPortal;
    }

  }
}
