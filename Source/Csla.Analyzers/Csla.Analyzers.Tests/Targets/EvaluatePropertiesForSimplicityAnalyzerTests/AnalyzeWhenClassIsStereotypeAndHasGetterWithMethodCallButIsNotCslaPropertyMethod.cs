namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallButIsNotCslaPropertyMethod
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallButIsNotCslaPropertyMethod>
  {
    public string Data { get { return this.GetProperty(); } }

    public string GetProperty()
    {
      return null;
    }
  }
}
