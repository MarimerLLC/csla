using System;

namespace Csla
{
  /// <summary>
  /// Enum used to determine how a given property should be indexed
  /// </summary>
  public enum IndexModeEnum
  {
    /// <summary>
    /// Sets up indexing only when used in a where clause
    /// </summary>
    IndexModeOnDemand,
    /// <summary>
    /// Specifies that the index is built upon creation of a BusinessListBase
    /// that uses this class on this property.
    /// </summary>
    IndexModeAlways,
    /// <summary>
    /// Specifies that this property should never be indexed.  Technically, the
    /// same as not having this attribute at all.
    /// </summary>
    IndexModeNever
  }
}
