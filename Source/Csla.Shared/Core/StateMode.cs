//-----------------------------------------------------------------------
// <copyright file="StateMode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicates the reason the MobileFormatter</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Indicates the reason the MobileFormatter
  /// functionality has been invoked.
  /// </summary>
  public enum StateMode
  {
    /// <summary>
    /// The object is being serialized for
    /// a clone or data portal operation.
    /// </summary>
    Serialization,
    /// <summary>
    /// The object is being serialized for
    /// an n-level undo operation.
    /// </summary>
    Undo
  }
}