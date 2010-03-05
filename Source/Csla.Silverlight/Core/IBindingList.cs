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
