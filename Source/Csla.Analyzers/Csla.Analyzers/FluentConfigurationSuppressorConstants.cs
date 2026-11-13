using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Justifications for the suppressions reported by <see cref="FluentConfigurationSuppressor"/>.
  /// </summary>
  public static class FluentConfigurationSuppressorConstants
  {
    /// <summary>
    /// Justification for suppressing IDE0058.
    /// </summary>
    public static readonly LocalizableResourceString ResultUnusedJustification = new(nameof(Resources.FluentConfigurationSuppressor_ResultUnusedJustification), Resources.ResourceManager, typeof(Resources));
  }
}
