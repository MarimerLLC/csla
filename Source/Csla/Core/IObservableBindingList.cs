//-----------------------------------------------------------------------
// <copyright file="IObservableBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines additional elements for an</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines additional elements for an
  /// ObservableCollection as required by
  /// CSLA .NET.
  /// </summary>
  public interface IObservableBindingList
  {
    /// <summary>
    /// Creates and adds a new item to the collection.
    /// </summary>
    object AddNew();
    /// <summary>
    /// Event indicating that an item is being
    /// removed from the list.
    /// </summary>
    event EventHandler<RemovingItemEventArgs> RemovingItem;
  }
}