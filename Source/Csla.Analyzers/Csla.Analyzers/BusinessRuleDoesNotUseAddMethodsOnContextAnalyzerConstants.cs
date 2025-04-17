using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new(nameof(Resources.BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    public static readonly LocalizableResourceString Message = new(nameof(Resources.BusinessRuleDoesNotUseAddMethodsOnContextAnalyzer_Message), Resources.ResourceManager, typeof(Resources));
  }
}
