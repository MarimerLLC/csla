//-----------------------------------------------------------------------
// <copyright file="IDataPortalActivator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a type used to activate concrete business instances.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Server
{
  /// <summary>
  /// Defines a type used to activate concrete business
  /// instances.
  /// </summary>
  public interface IDataPortalActivator
  {
    /// <summary>
    /// Gets a new instance of the requested type.
    /// </summary>
    /// <param name="requestedType">Requested business type (class or interface).</param>
    /// <returns>Business object instance.</returns>
    object CreateInstance(Type requestedType);
    /// <summary>
    /// Initializes an existing business object instance.
    /// </summary>
    /// <param name="obj">Reference to the business object.</param>
    void InitializeInstance(object obj);
    /// <summary>
    /// Finalizes an existing business object instance. Called
    /// after a data portal operation is complete.
    /// </summary>
    /// <param name="obj">Reference to the business object.</param>
    void FinalizeInstance(object obj);
    /// <summary>
    /// Gets the actual business domain class type based on the
    /// requested type (which might be an interface).
    /// </summary>
    /// <param name="requestedType">Type requested from the data portal.</param>
    Type ResolveType(Type requestedType);
  }
}
