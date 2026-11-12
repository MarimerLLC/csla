//-----------------------------------------------------------------------
// <copyright file="ISupportsAllowNew.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes whether a list accepts new items.</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Exposes whether a list accepts new items, so code
  /// that only knows the list as an <see cref="System.Collections.IList"/>
  /// can check before creating an item.
  /// </summary>
  internal interface ISupportsAllowNew
  {
    /// <summary>
    /// Gets a value indicating whether new items can be added to the list.
    /// </summary>
    bool AllowNew { get; }
  }
}
