//-----------------------------------------------------------------------
// <copyright file="RemovingItemEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains event data for the RemovingItem</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Contains event data for the RemovingItem
  /// event.
  /// </summary>
  public class RemovingItemEventArgs : EventArgs
  {
    private object _removingItem;

    /// <summary>
    /// Gets a reference to the item that was
    /// removed from the list.
    /// </summary>
    public object RemovingItem
    {
      get { return _removingItem; }
    }

    /// <summary>
    /// Create an instance of the object.
    /// </summary>
    /// <param name="removingItem">
    /// A reference to the item that was 
    /// removed from the list.
    /// </param>
    public RemovingItemEventArgs(object removingItem)
    {
      _removingItem = removingItem;
    }
  }
}