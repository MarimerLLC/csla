namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedPropertyAndIsNotPublic>
  {
    static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    static readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}
