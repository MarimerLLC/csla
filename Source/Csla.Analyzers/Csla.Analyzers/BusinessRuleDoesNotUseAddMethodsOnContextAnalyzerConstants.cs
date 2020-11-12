namespace Csla.Analyzers
{
  public static class BusinessRuleDoesNotUseAddMethodsOnContextAnalyzerConstants
  {
    public const string Title = "Find Business Rules That Do Not Use Add() Methods on the Context";
    public const string Message = "Business rules should use at least one Add() method on the context";
  }
}
