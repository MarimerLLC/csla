using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString Message = new(nameof(Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants
  {
    public static string UpdateToAsyncEquivalentsDescription => Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_UpdateToAsyncEquivalentsDescription;
  }
}
