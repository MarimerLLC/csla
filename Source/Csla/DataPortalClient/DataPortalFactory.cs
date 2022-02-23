//-----------------------------------------------------------------------
// <copyright file="DataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Get an access to a client-side data portal
  /// instance.
  /// </summary>
  public class DataPortalFactory : IDataPortalFactory
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="serviceProvider">Current ServiceProvider</param>
    public DataPortalFactory(IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider;
    }

    private IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IDataPortal<T> GetPortal<T>()
    {
      return (IDataPortal<T>)ServiceProvider.GetService(typeof(IDataPortal<T>));
    }
  }
}
