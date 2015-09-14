namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnAndDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasGetterWithMethodCallAndReturnAndDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);

    public string Data { get { return this.GetProperty(DataProperty); } }
  }
}
