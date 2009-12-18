using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  public interface IObservableBindingList
  {
    /// <summary>
    /// Creates and adds a new item to the collection.
    /// </summary>
    void AddNew();
    /// <summary>
    /// Event indicating that an item is being
    /// removed from the list.
    /// </summary>
    event EventHandler<RemovingItemEventArgs> RemovingItem;
  }
}
