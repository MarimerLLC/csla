namespace Csla.Analyzers.ManagedBackingFieldUsesNameof
{
  public static class EvaluateManagedBackingFieldsNameofAnalyzerConstants
  {
    public const string Title = "Evaluate Managed Backing Fields using nameof";
    public const string IdentifierText = "ManagedBackingFieldShouldUseNameof";
    public const string Message = "Managed backing fields should use nameof";
  }

  public static class EvaluateManagedBackingFieldsCodeFixConstants
  {
    public const string FixManagedBackingFieldDescription = "Refactor managed backing field to nameof";
  }
}
