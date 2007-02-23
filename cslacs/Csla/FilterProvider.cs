using System;

namespace Csla
{
  /// <summary>
  /// Defines the method signature for a filter
  /// provider method used by FilteredBindingList.
  /// </summary>
  /// <param name="item">The object to be filtered.</param>
  /// <param name="filter">The filter criteria.</param>
  /// <returns><see langword="true"/> if the item matches the filter.</returns>
  public delegate bool FilterProvider(object item, object filter);
}
