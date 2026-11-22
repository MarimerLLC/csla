using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Resources for <see cref="NotNullPropertyReadAuthorizationAnalyzer"/>.
  /// </summary>
  public static class NotNullPropertyReadAuthorizationAnalyzerConstants
  {
    /// <summary>
    /// The diagnostic title.
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.NotNullPropertyReadAuthorizationAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message; <c>{0}</c> is the property name.
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.NotNullPropertyReadAuthorizationAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }
}
