﻿//-----------------------------------------------------------------------
// <copyright file="IChildDataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace Csla
{
  /// <summary>
  /// Defines a data portal service
  /// used to get an access to a client-side data portal
  /// instance.
  /// </summary>
  public interface IChildDataPortalFactory
  {
    /// <summary>
    /// Get a child data portal instance.
    /// </summary>
    /// <typeparam name="T">Child business object type</typeparam>
    IChildDataPortal<T> GetPortal<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    T>();
  }
}
