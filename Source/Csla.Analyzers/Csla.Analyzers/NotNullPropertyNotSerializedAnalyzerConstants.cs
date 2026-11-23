using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Resources for <see cref="NotNullPropertyNotSerializedAnalyzer"/>.
  /// </summary>
  public static class NotNullPropertyNotSerializedAnalyzerConstants
  {
    /// <summary>
    /// The diagnostic title.
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.NotNullPropertyNotSerializedAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message; <c>{0}</c> is the property name.
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.NotNullPropertyNotSerializedAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }
}
