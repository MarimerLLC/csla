using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.TestHelpers
{
  public static class DataPortalFactory
  {

    /// <summary>
    /// Create an instance of a typed data portal using the DI container
    /// </summary>
    /// <typeparam name="T">The type which the data portal is to service</typeparam>
    /// <returns>An instance of IDataPortal<typeparamref name="T"/> for use in data access during tests</returns>
    public static IDataPortal<T> CreateDataPortal<T>()
    {
      IDataPortal<T> dataPortal;

      dataPortal = RootServiceProvider.GetRequiredService<IDataPortal<T>>();
      return dataPortal;
    }

    /// <summary>
    /// Create an instance of a typed child data portal using the DI container
    /// </summary>
    /// <typeparam name="T">The type which the child data portal is to service</typeparam>
    /// <returns>An instance of IChildDataPortal<typeparamref name="T"/> for use in data access during tests</returns>
    public static IChildDataPortal<T> CreateChildDataPortal<T>()
    {
      IChildDataPortal<T> dataPortal;

      dataPortal = RootServiceProvider.GetRequiredService<IChildDataPortal<T>>();
      return dataPortal;
    }

  }
}
