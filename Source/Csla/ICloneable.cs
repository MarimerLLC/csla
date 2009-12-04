using System;

namespace Csla
{
  /// <summary>
  /// Defines a cloneable object.
  /// </summary>
  public interface ICloneable
  {
    /// <summary>
    /// Gets a clone of the object.
    /// </summary>
    object Clone();
  }
}
