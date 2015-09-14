namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallButIsNotCslaPropertyMethod
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallButIsNotCslaPropertyMethod>
  {
    public string Data { set { this.SetProperty(); } }

    public void SetProperty() { }
  }
}
