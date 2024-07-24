using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string UpdateToAsyncEquivalentsDescription => Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_UpdateToAsyncEquivalentsDescription;
  }
}
