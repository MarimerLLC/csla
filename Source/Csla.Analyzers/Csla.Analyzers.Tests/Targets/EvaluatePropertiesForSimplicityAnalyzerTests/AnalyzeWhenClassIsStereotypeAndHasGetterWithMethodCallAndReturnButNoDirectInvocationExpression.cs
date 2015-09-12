namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);

    public string Data { get { return "x" + this.GetProperty(DataProperty); } }
  }
}
