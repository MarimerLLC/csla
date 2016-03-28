namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasSetterWithNoMethodCall
    : BusinessBase<AnalyzeWhenClassHasSetterWithNoMethodCall>
  {
    public string Data { set { } }
  }
}
