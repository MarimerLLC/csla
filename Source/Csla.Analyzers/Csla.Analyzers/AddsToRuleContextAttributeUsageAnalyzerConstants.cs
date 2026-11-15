using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Resources for <see cref="AddsToRuleContextAttributeUsageAnalyzer"/>.
  /// </summary>
  public static class AddsToRuleContextAttributeUsageAnalyzerConstants
  {
    /// <summary>
    /// The diagnostic title.
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message; <c>{0}</c> is the method name and <c>{1}</c> the reason.
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// The diagnostic message for a type that implements an <c>[AddsToRuleContext]</c> interface
    /// method with an inherited method; <c>{0}</c> is the type, <c>{1}</c> the interface method
    /// and <c>{2}</c> the inherited method.
    /// </summary>
    public static readonly LocalizableResourceString InheritedImplementationMessage = new(nameof(Resources.AddsToRuleContextAttributeUsageAnalyzer_InheritedImplementationMessage), Resources.ResourceManager, typeof(Resources));
  }
}
