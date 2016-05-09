namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallButIsNotCslaPropertyMethod>
  {
    public string Data { get { return this.GetProperty(); } }

    public string GetProperty()
    {
      return null;
    }
  }
}
