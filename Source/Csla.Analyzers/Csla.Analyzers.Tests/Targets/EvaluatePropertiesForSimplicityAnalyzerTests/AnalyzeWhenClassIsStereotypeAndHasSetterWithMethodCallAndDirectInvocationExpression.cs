namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndDirectInvocationExpression
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasSetterWithMethodCallAndDirectInvocationExpression>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(_ => _.Data);

    public string Data
    {
      get { return null; }
      set { this.SetProperty(DataProperty, value); }
    }
  }
}
