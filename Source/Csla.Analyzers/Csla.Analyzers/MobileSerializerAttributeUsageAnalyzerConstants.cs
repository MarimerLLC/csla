using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  ///
  /// </summary>
  public static class MobileSerializerAttributeUsageAnalyzerConstants
  {
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString InvalidDeclarationTitle = new(nameof(Resources.MobileSerializerAttributeUsageAnalyzer_InvalidDeclarationTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString InvalidDeclarationMessage = new(nameof(Resources.MobileSerializerAttributeUsageAnalyzer_InvalidDeclarationMessage), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString ConflictTitle = new(nameof(Resources.MobileSerializerAttributeUsageAnalyzer_ConflictTitle), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    ///
    /// </summary>
    public static readonly LocalizableResourceString ConflictMessage = new(nameof(Resources.MobileSerializerAttributeUsageAnalyzer_ConflictMessage), Resources.ResourceManager, typeof(Resources));
  }
}
