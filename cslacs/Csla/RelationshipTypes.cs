using System;

namespace Csla
{
  /// <summary>
  /// List of valid relationship types
  /// between a parent object and another
  /// object through a managed property.
  /// </summary>
  [Flags]
  public enum RelationshipTypes
  {
    /// <summary>
    /// Property is a reference to a child
    /// object contained by the parent.
    /// </summary>
    Child = 1,
    /// <summary>
    /// Property is a reference to a lazy
    /// loaded object. Attempting to get
    /// or read the property value
    /// prior to a set or load will result in 
    /// an exception.
    /// </summary>
    LazyLoad = 2
  }
}
