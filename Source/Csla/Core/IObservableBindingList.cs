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
