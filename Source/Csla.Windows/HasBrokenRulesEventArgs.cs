//-----------------------------------------------------------------------
// <copyright file="HasBrokenRulesEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Event args object containing information about a</summary>
//-----------------------------------------------------------------------

namespace Csla.Windows
{
  /// <summary>
  /// Event args object containing information about a
  /// broken rule.
  /// </summary>
  public class HasBrokenRulesEventArgs : CslaActionCancelEventArgs
  {
    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    /// <param name="commandName">
    /// Name of the command.
    /// </param>
    /// <param name="hasErrors">
    /// Indicates whether error severity exists.
    /// </param>
    /// <param name="hasWarnings">
    /// Indicates whether warning severity exists.
    /// </param>
    /// <param name="hasInformation">
    /// Indicates whether information severity exists.
    /// </param>
    /// <param name="autoShowBrokenRules">
    /// Indicates whether to automatically show broken rules.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="commandName"/> is <see langword="null"/>.</exception>
    public HasBrokenRulesEventArgs(string commandName, bool hasErrors, bool hasWarnings, bool hasInformation, bool autoShowBrokenRules)
      : base(false, commandName)
    {
      HasErrors = hasErrors;
      HasWarning = hasWarnings;
      HasInformation = hasInformation;
      AutoShowBrokenRules = autoShowBrokenRules;
    }

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    /// <param name="cancel">
    /// Indicates whether to cancel.
    /// </param>
    /// <param name="commandName">
    /// Name of the command.
    /// </param>
    /// <param name="hasErrors">
    /// Indicates whether error severity exists.
    /// </param>
    /// <param name="hasWarnings">
    /// Indicates whether warning severity exists.
    /// </param>
    /// <param name="hasInformation">
    /// Indicates whether information severity exists.
    /// </param>
    /// <param name="autoShowBrokenRules">
    /// Indicates whether to automatically show broken rules.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="commandName"/> is <see langword="null"/>.</exception>
    public HasBrokenRulesEventArgs(bool cancel, string commandName, bool hasErrors, bool hasWarnings, bool hasInformation, bool autoShowBrokenRules)
      : base(cancel, commandName)
    {
      HasErrors = hasErrors;
      HasWarning = hasWarnings;
      HasInformation = hasInformation;
      AutoShowBrokenRules = autoShowBrokenRules;
    }

    /// <summary>
    /// Gets a value indicating whether
    /// an error severity rule exists.
    /// </summary>
    public bool HasErrors { get; } = false;

    /// <summary>
    /// Gets a value indicating whether
    /// a warning severity rule exists.
    /// </summary>
    public bool HasWarning { get; } = false;

    /// <summary>
    /// Gets a value indicating whether
    /// an information severity rule exists.
    /// </summary>
    public bool HasInformation { get; } = false;

    /// <summary>
    /// Gets a value indicating whether
    /// to show broken rules.
    /// </summary>
    public bool AutoShowBrokenRules { get; } = false;
  }
}