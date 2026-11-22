using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Resources for <see cref="NotNullPropertyNotLoadedAnalyzer"/>.
  /// </summary>
  public static class NotNullPropertyNotLoadedAnalyzerConstants
  {
    /// <summary>
    /// The diagnostic title.
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.NotNullPropertyNotLoadedAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message for an operation; <c>{0}</c> is the operation and <c>{1}</c> the property names.
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.NotNullPropertyNotLoadedAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message for a child type without its own <c>[CreateChild]</c>; <c>{0}</c> is the type and <c>{1}</c> the property names.
    /// </summary>
    public static readonly LocalizableResourceString MissingCreateChildMessage = new(nameof(Resources.NotNullPropertyNotLoadedAnalyzer_MissingCreateChildMessage), Resources.ResourceManager, typeof(Resources));
  }
}
