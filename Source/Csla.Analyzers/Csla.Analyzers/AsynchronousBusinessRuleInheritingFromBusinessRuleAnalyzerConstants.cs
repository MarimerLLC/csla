using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString Message = new(nameof(Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class AsynchronousBusinessRuleInheritingFromBusinessRuleChangeToBusinessRuleAsyncCodeFixConstants
  {
    public static string UpdateToAsyncEquivalentsDescription => Resources.AsynchronousBusinessRuleInheritingFromBusinessRuleAnalyzer_UpdateToAsyncEquivalentsDescription;
  }
}
