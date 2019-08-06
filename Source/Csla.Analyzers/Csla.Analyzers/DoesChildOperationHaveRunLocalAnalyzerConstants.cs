namespace Csla.Analyzers
{
  public static class DoesChildOperationHaveRunLocalAnalyzerConstants
  {
    public const string Title = "Find Child Operations That Have [RunLocal]";
    public const string IdentifierText = "DoesChildOperationHaveRunLocal";
    public const string Message = "Child operations should not have [RunLocal]";
  }

  public static class DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants
  {
    public const string RemoveRunLocalDescription = "Remove [RunLocal]";
  }
}
