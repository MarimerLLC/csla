namespace Csla.Analyzers
{
  public static class EvaluateOperationAttributeUsageAnalyzerConstants
  {
    public const string Title = "Find Operation Attributes In Incorrect Places";
    public const string IdentifierText = "EvaluateOperationAttributeUsage";
    public const string Message = "Operation attributes should only be used on methods within sterotypes or ObjectFactory and should not be static";
  }
}