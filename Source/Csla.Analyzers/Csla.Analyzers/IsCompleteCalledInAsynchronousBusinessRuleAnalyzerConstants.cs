using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// 
  /// </summary>
  public static class IsCompleteCalledInAsynchronousBusinessRuleConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Title = new(nameof(Resources.IsCompleteCalledInAsynchronousBusinessRule_Title), Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// 
    /// </summary>
    public static readonly LocalizableResourceString Message = new(nameof(Resources.IsCompleteCalledInAsynchronousBusinessRule_Message), Resources.ResourceManager, typeof(Resources));
  }

  /// <summary>
  /// 
  /// </summary>
  public static class IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants
  {
    /// <summary>
    /// 
    /// </summary>
    public static string RemoveCompleteCalls => Resources.IsCompleteCalledInAsynchronousBusinessRule_RemoveCompleteCalls;
  }
}
