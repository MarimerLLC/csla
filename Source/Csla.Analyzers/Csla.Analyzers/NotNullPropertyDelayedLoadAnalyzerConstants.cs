using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Resources for <see cref="NotNullPropertyDelayedLoadAnalyzer"/>.
  /// </summary>
  public static class NotNullPropertyDelayedLoadAnalyzerConstants
  {
    /// <summary>
    /// The diagnostic title.
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.NotNullPropertyDelayedLoadAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message; <c>{0}</c> is the property name and <c>{1}</c> the method.
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.NotNullPropertyDelayedLoadAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }
}
