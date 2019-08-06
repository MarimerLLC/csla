namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallAndReturnAndDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    public string Data { get { return this.GetProperty(DataProperty); } }

    public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => this.GetProperty(DataProperty);
  }
}
