namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod
    : BusinessBase<AnalyzeWhenClassHasSetterWithMethodCallButIsNotCslaPropertyMethod>
  {
    public string Data { set { this.SetProperty(); } }

    public void SetProperty() { }
  }
}
