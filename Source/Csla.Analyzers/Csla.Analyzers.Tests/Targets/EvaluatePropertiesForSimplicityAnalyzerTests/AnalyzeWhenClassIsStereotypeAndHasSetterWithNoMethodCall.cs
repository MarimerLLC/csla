namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasSetterWithNoMethodCall
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasSetterWithNoMethodCall>
  {
    public string Data { set { } }
  }
}
