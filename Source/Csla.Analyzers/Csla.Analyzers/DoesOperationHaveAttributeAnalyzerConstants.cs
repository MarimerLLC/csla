namespace Csla.Analyzers
{
  public static class DoesOperationHaveAttributeAnalyzerConstants
  {
    public const string Title = "Find Operations That Do Not Have an Operation Attribute";
    public const string IdentifierText = "DoesOperationHaveAttribute";
    public const string Message = "Operations should have the appropriate operation attribute";
  }

  public static class DoesOperationHaveAttributeAnalyzerAddAttributeCodeFixConstants
  {
    public const string AddAttributeAndUsingDescription = "Add attribute and using statement";
    public const string AddAttributeDescription = "Add attribute";
    public const string CslaNamespace = "Csla";
  }
}
