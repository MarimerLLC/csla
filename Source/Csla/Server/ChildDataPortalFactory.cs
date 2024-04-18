﻿//-----------------------------------------------------------------------
// <copyright file="ChildDataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Get an access to a Child data portal
  /// instance.
  /// </summary>
  public class ChildDataPortalFactory : IChildDataPortalFactory
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="serviceProvider">Current ServiceProvider</param>
    public ChildDataPortalFactory(IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider;
    }

    private IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IChildDataPortal<T> GetPortal<T>()
    {
      return (IChildDataPortal<T>)ServiceProvider.GetService(typeof(IChildDataPortal<T>));
    }
  }
}
