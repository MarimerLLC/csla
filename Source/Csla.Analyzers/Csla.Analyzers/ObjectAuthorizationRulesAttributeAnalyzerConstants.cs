namespace Csla.Analyzers
{
  public static class ObjectAuthorizationRulesAttributeAnalyzerConstants
  {
    public const string AttributeMissingTitle = "Find Authorization Rules Configuration That Do Not Have an Operation Attribute";
    public const string AttributeMissingMessage = "Authorization rules should use the appropriate operation attribute";
    public const string RulesPublicTitle = "Find Authorization Rules Configuration That Is Not Public";
    public const string RulesPublicMessage = "Authorization rules should be declared as public methods";
    public const string RulesStaticTitle = "Find Authorization Rules Configuration That Is Not Static";
    public const string RulesStaticMessage = "Authorization rules should be declared as static methods";
  }

  public static class ObjectAuthorizationRulesAttributeAnalyzerAddAttributeCodeFixConstants
  {
    public const string AddAttributeAndUsingDescription = "Add attribute and using statement";
    public const string AddAttributeDescription = "Add attribute";
    public const string CslaNamespace = "Csla";
  }
}
