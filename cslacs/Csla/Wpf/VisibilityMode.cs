#if !NET20
using System;

namespace Csla.Wpf
{
  /// <summary>
  /// Options controlling how the AuthorizationPanel
  /// control alters the visibility of a control
  /// when read access to the property is not allowed.
  /// </summary>
  public enum VisibilityMode
  {
    /// <summary>
    /// Specifies that the non-readable control
    /// should be collapsed.
    /// </summary>
    Collapsed,
    /// <summary>
    /// Specifies that the non-readable control
    /// should be hidden.
    /// </summary>
    Hidden
  }
}
#endif