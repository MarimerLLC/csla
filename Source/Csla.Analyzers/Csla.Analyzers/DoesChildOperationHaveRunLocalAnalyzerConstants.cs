namespace Csla.Analyzers
{
  public static class DoesChildOperationHaveRunLocalAnalyzerConstants
  {
    public const string Title = "Find Child Operations That Have [RunLocal]";
    public const string IdentifierText = "DoesChildOperationHaveRunLocal";
    public const string Message = "Child operations should not have [RunLocal]";
  }

  public static class DoesChildOperationHaveRunLocalAnalyzerCodeFixConstants
  {
    public const string AddSerializableAndUsingDescription = "Add [Serializable] and using statement(s)";
    public const string SerializableName = "Serializable";
    public const string SystemNamespace = "System";
  }
}
