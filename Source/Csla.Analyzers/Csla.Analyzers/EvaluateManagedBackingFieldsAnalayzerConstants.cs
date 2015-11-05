namespace Csla.Analyzers
{
  public static class EvaluateManagedBackingFieldsAnalayzerConstants
  {
    public const string Category = "Usage";
    public const string DiagnosticId = "CSLA0008";
    public const string IsStatic = "IsStatic";
    public const string IsPublic = "IsPublic";
    public const string IsReadonly = "IsReadonly";
    public const string Title = "Evaluate Managed Backing Fields";
    public const string IdentifierText = "ManagedBackingFieldMustBePublicStaticAndReadOnlyRule";
    public const string Message = "Managed backing fields must be public, static and read-only.";
  }
}
