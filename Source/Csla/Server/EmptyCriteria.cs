﻿//-----------------------------------------------------------------------
// <copyright file="EmptyCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Empty criteria used by the data portal as a</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Empty criteria used by the data portal as a
  /// placeholder for a create/fetch request that
  /// has no criteria.
  /// </summary>
  [Serializable]
  public sealed class EmptyCriteria : Core.MobileObject
  {
    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    public EmptyCriteria() { }

    /// <summary>
    /// Gets an instance of EmptyCriteria
    /// </summary>
    public static EmptyCriteria Instance { get; } = new EmptyCriteria();
  }
}