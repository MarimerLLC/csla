//-----------------------------------------------------------------------
// <copyright file="IDataPortalActivator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a type used to activate concrete business instances.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

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
    /// <param name="parameters">Param array for the constructor</param>
    /// <returns>Business object instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="requestedType"/> or <paramref name="requestedType"/> is <see langword="null"/>.</exception>
    object CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type requestedType, params object[] parameters);
    /// <summary>
    /// Initializes an existing business object instance.
    /// </summary>
    /// <param name="obj">Reference to the business object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    void InitializeInstance(object obj);
    /// <summary>
    /// Finalizes an existing business object instance. Called
    /// after a data portal operation is complete.
    /// </summary>
    /// <param name="obj">Reference to the business object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    void FinalizeInstance(object obj);
    /// <summary>
    /// Gets the actual business domain class type based on the
    /// requested type (which might be an interface).
    /// </summary>
    /// <param name="requestedType">Type requested from the data portal.</param>
    /// <exception cref="ArgumentNullException"><paramref name="requestedType"/> is <see langword="null"/>.</exception>
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    Type ResolveType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type requestedType);
  }
}
