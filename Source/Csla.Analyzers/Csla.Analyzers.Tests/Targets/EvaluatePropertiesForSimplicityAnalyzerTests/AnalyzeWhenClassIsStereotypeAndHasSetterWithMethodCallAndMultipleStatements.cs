namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndMultipleStatements
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndMultipleStatements>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    private string _x;

    public string Data { get { return null; } set { _x = "44"; this.SetProperty(DataProperty, value); } }

    public string GetX() { return _x; }
  }
}
