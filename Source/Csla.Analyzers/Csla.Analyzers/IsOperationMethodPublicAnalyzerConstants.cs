namespace Csla.Analyzers
{
  public static class IsOperationMethodPublicAnalyzerConstants
  {
    public const string Title = "Find CSLA Operations That are Public";
    public const string IdentifierText = "IsOperationMethodPublic";
    public const string IsSealed = "IsSealed";
    public const string Message = "CSLA operations should not be public";
  }

  public static class IsOperationMethodPublicAnalyzerMakeNonPublicCodeFixConstants
  {
    public const string InternalDescription = "Make CSLA operation internal";
    public const string ProtectedDescription = "Make CSLA operation protected";
    public const string PrivateDescription = "Make CSLA operation private";
  }
}
