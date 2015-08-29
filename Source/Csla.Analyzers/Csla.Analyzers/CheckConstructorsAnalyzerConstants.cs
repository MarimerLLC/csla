namespace Csla.Analyzers
{
  public static class PublicNoArgumentConstructorIsMissingConstants
  {
    public const string Category = "Usage";
    public const string DiagnosticId = "CSLA0003";
    public const string Title = "Find CSLA Business Objects That do not Have Public No-Arugment Constructors";
    public const string IdentifierText = "PublicNoArgumentConstructorIsMissing";
    public const string Message = "CSLA business objects must have a public constructor with no arguments.";
  }

  public static class ConstructorHasParametersConstants
  {
    public const string Category = "Usage";
    public const string DiagnosticId = "CSLA0004";
    public const string Title = "Find CSLA Business Objects That Have Constructors With Parameters";
    public const string IdentifierText = "ConstructorHasParameters";
    public const string Message = "CSLA business objects should not have constructors with parameters.";
  }
}
