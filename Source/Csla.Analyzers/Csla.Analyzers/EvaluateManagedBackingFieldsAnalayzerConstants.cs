namespace Csla.Analyzers
{
  public static class EvaluateManagedBackingFieldsAnalayzerConstants
  {
    public const string Title = "Evaluate Managed Backing Fields";
    public const string IdentifierText = "ManagedBackingFieldMustBePublicStaticAndReadOnlyRule";
    public const string Message = "Managed backing fields must be public, static and read-only";
  }

  public static class EvaluateManagedBackingFieldsCodeFixConstants
  {
    public const string FixManagedBackingFieldDescription = "Fix managed backing field declaration";
  }
}
