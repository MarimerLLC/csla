namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotStatic>
  {
    public readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}
