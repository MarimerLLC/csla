namespace Csla.Analyzers.ManagedBackingFieldUsesNameof
{
  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsNameofAnalyzerConstants
  {
    public const string Title = "Evaluate Managed Backing Fields using nameof";
    /// <summary>
    /// 
    /// </summary>
    public const string IdentifierText = "ManagedBackingFieldShouldUseNameof";
    /// <summary>
    /// 
    /// </summary>
    public const string Message = "Managed backing fields should use nameof";
  }

  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsCodeFixConstants
  {
    public const string FixManagedBackingFieldDescription = "Refactor managed backing field to nameof";
  }
}
