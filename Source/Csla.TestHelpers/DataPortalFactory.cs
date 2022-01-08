//-----------------------------------------------------------------------
// <copyright file="DataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory for dataportal instances for use in tests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.TestHelpers
{
  public static class DataPortalFactory
  {

    /// <summary>
    /// Create an instance of a typed data portal using the context-specific DI container
    /// </summary>
    /// <typeparam name="T">The type which the data portal is to service</typeparam>
    /// <param name="context">The context from which configuration can be retrieved</param>
    /// <returns>An instance of IDataPortal<typeparamref name="T"/> for use in data access during tests</returns>
    public static IDataPortal<T> CreateDataPortal<T>(TestDIContext context)
    {
      IDataPortal<T> dataPortal;

      dataPortal = context.ServiceProvider.GetRequiredService<IDataPortal<T>>();
      return dataPortal;
    }

    /// <summary>
    /// Create an instance of a typed child data portal using the context-specific DI container
    /// </summary>
    /// <typeparam name="T">The type which the child data portal is to service</typeparam>
    /// <param name="context">The context from which configuration can be retrieved</param>
    /// <returns>An instance of IChildDataPortal<typeparamref name="T"/> for use in data access during tests</returns>
    public static IChildDataPortal<T> CreateChildDataPortal<T>(TestDIContext context)
    {
      IChildDataPortal<T> dataPortal;

      dataPortal = context.ServiceProvider.GetRequiredService<IChildDataPortal<T>>();
      return dataPortal;
    }

  }
}
