namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldNotUsedByProperty>
  {
    public static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data { get; set; }

    public static readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => string.Empty;
  }
}
