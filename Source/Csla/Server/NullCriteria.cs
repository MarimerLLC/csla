//-----------------------------------------------------------------------
// <copyright file="NullCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Null criteria used by the data portal as a</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Server
{
  /// <summary>
  /// Null criteria used by the data portal as a
  /// placeholder for a create/fetch request that
  /// has a single null criteria parameter.
  /// </summary>
  [Serializable]
  public sealed class NullCriteria : Csla.Core.MobileObject
  {
    private NullCriteria() { }

    /// <summary>
    /// Gets an instance of NullCriteria
    /// </summary>
    public static NullCriteria Instance { get; } = new NullCriteria();
  }
}