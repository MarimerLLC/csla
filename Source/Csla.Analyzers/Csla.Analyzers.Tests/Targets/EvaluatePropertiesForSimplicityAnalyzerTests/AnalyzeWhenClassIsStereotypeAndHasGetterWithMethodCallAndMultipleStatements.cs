namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndMultipleStatements
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndMultipleStatements>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    private string _x;

    public string Data { get { _x = "44"; return this.GetProperty(DataProperty); } }

    public string GetX() { return _x; }
  }
}
