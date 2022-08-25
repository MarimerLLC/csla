//-----------------------------------------------------------------------
// <copyright file="IDataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------
namespace Csla
{
  /// <summary>
  /// Defines a data portal service
  /// used to get an access to a client-side data portal
  /// instance.
  /// </summary>
  public interface IDataPortalFactory
  {
    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    IDataPortal<T> GetPortal<T>();
  }
}
