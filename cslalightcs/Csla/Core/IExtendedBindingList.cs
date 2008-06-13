using System;
using System.ComponentModel;

namespace Csla.Core
{
  /// <summary>
  /// Extends <see cref="IBindingList" /> by adding extra
  /// events.
  /// </summary>
  public interface IExtendedBindingList
  {
    /// <summary>
    /// Event indicating that an item is being
    /// removed from the list.
    /// </summary>
    event EventHandler<RemovingItemEventArgs> RemovingItem;
  }
}
