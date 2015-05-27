//-----------------------------------------------------------------------
// <copyright file="IObservableBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines additional elements for an</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Defines additional elements for an
  /// ObservableCollection as required by
  /// CSLA .NET.
  /// </summary>
  public interface IObservableBindingList
  {
#if SILVERLIGHT || NETFX_CORE
    /// <summary>
    /// Creates and adds a new item to the collection.
    /// </summary>
    void AddNew();
#else
    /// <summary>
    /// Creates and adds a new item to the collection.
    /// </summary>
    object AddNew();
#endif
    /// <summary>
    /// Event indicating that an item is being
    /// removed from the list.
    /// </summary>
    event EventHandler<RemovingItemEventArgs> RemovingItem;
  }
}