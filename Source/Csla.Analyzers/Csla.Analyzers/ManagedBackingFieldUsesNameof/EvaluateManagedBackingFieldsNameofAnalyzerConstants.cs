namespace Csla.Analyzers.ManagedBackingFieldUsesNameof
{
  /// <summary>
  /// 
  /// </summary>
  public static class EvaluateManagedBackingFieldsNameofAnalyzerConstants
  {
    /// <summary>
    /// 
    /// </summary>
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
    /// <summary>
    /// 
    /// </summary>
    public const string FixManagedBackingFieldDescription = "Refactor managed backing field to nameof";
  }
}
