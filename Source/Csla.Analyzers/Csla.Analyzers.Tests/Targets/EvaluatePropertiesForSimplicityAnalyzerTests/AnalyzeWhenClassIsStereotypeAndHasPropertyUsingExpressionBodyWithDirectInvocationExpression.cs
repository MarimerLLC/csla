namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasPropertyUsingExpressionBodyWithDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasPropertyUsingExpressionBodyWithDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);
    public string Data => this.GetProperty(DataProperty);
  }
}
