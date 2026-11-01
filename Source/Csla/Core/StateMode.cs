//-----------------------------------------------------------------------
// <copyright file="StateMode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicates the reason the MobileFormatter</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Indicates the reason the MobileFormatter
  /// functionality has been invoked.
  /// </summary>
  [Obsolete(MobileObject.StateModeObsoleteMessage)]
  public enum StateMode
  {
    /// <summary>
    /// The object is being serialized for
    /// a clone or data portal operation.
    /// </summary>
    Serialization,
    /// <summary>
    /// No longer used. N-level undo doesn't
    /// call OnGetState or OnSetState.
    /// </summary>
    Undo
  }
}