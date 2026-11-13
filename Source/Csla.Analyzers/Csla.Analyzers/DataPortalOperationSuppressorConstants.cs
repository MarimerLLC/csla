using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Justifications for the suppressions reported by <see cref="DataPortalOperationSuppressor"/>.
  /// </summary>
  public static class DataPortalOperationSuppressorConstants
  {
    /// <summary>
    /// Justification for suppressing IDE0051.
    /// </summary>
    public static readonly LocalizableResourceString UnusedOperationJustification = new(nameof(Resources.DataPortalOperationSuppressor_UnusedOperationJustification), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// Justification for suppressing CA1822.
    /// </summary>
    public static readonly LocalizableResourceString OperationCanBeStaticJustification = new(nameof(Resources.DataPortalOperationSuppressor_OperationCanBeStaticJustification), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// Justification for suppressing IDE0060.
    /// </summary>
    public static readonly LocalizableResourceString UnusedOperationParameterJustification = new(nameof(Resources.DataPortalOperationSuppressor_UnusedOperationParameterJustification), Resources.ResourceManager, typeof(Resources));
  }
}
