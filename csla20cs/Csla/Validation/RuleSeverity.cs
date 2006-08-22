using System;

namespace Csla.Validation
{
  /// <summary>
  /// Values for validation rule severities.
  /// </summary>
  public enum RuleSeverity
  {
    /// <summary>
    /// Represents a serious
    /// business rule violation that
    /// should cause an object to
    /// be considered invalid.
    /// </summary>
    Error,

    /// <summary>
    /// Represents a business rule
    /// violation that should be
    /// displayed to the user, but which
    /// should not make an object be
    /// invalid.
    /// </summary>
    Warning,

    /// <summary>
    /// Represents a business rule
    /// result that should be displayed
    /// to the user, but which is less
    /// severe than a warning.
    /// </summary>
    Information
  }
}
