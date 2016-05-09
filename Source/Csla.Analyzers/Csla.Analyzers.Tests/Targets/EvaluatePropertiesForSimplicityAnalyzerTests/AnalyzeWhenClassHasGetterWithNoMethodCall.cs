namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithNoMethodCall
    : BusinessBase<AnalyzeWhenClassHasGetterWithNoMethodCall>
  {
    public string Data { get; }

    public string ExpressionData => string.Empty;
  }
}
