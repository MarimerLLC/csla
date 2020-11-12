namespace Csla.Analyzers
{
  public static class OnlyUseCslaPropertyMethodsInGetSetRuleConstants
  {
    public const string Title = "Evaluate Properties for Simplicity";
    public const string IdentifierText = "OnlyUseCslaPropertyMethodsInGetSetRule";
    public const string Message = "Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else";
  }
}
