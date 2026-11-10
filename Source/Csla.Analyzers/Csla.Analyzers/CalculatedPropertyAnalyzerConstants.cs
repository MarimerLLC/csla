using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  ///
  /// </summary>
  public static class CalculatedPropertyAnalyzerConstants
  {
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString MustBeReadOnlyTitle = new(nameof(Resources.CalculatedPropertyAnalyzer_MustBeReadOnlyTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString MustBeReadOnlyMessage = new(nameof(Resources.CalculatedPropertyAnalyzer_MustBeReadOnlyMessage), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString RequiresBusinessBaseTitle = new(nameof(Resources.CalculatedPropertyAnalyzer_RequiresBusinessBaseTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString RequiresBusinessBaseMessage = new(nameof(Resources.CalculatedPropertyAnalyzer_RequiresBusinessBaseMessage), Resources.ResourceManager, typeof(Resources));
  }
}
