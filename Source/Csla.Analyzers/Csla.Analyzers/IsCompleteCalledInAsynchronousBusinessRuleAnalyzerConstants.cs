using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class IsCompleteCalledInAsynchronousBusinessRuleConstants
  {
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.IsCompleteCalledInAsynchronousBusinessRule_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.IsCompleteCalledInAsynchronousBusinessRule_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants
  {
    public static string RemoveCompleteCalls => Resources.IsCompleteCalledInAsynchronousBusinessRule_RemoveCompleteCalls;
  }
}
