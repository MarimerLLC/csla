namespace Csla.Analyzers
{
  public static class IsCompleteCalledInAsynchronousBusinessRuleConstants
  {
    public const string Title = "Find Calls to Complete() in Asynchronous Business Rules";
    public const string IdentifierText = "FindCompleteCallsInAsynchronousBusinessRules";
    public const string Message = "Complete() should not be called in an asynchronous business rule";
  }

  public static class IsCompleteCalledInAsynchronousBusinessRuleCodeFixConstants
  {
    public const string RemoveCompleteCalls = "Remove Complete() calls";
  }
}
