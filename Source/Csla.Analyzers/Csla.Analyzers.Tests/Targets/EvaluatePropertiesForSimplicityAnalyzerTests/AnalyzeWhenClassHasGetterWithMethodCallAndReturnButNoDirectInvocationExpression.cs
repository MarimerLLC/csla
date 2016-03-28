namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassHasGetterWithMethodCallAndReturnButNoDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    public string Data { get { return "x" + this.GetProperty(DataProperty); } }

    public static readonly PropertyInfo<string> ExpressionDataProperty = RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => "x" + this.GetProperty(DataProperty);
  }
}
