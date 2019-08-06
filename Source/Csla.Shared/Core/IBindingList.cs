#if NETFX_CORE || (ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="IBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a bindable collection</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;

namespace Csla.Core
{
  /// <summary>
  /// Defines a bindable collection
  /// object.
  /// </summary>
  public interface IBindingList : IList
  {
    /// <summary>
    /// Adds a new item to the collection.
    /// </summary>
    void AddNew();
  }
}
#endif