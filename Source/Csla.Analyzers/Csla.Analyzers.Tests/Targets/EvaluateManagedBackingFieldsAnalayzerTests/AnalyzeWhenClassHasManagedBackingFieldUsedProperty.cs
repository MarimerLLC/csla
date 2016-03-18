namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassHasManagedBackingFieldUsedProperty
    : BusinessBase<AnalyzeWhenClassHasManagedBackingFieldUsedProperty>
  {
    public static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public static readonly PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => GetProperty(ExpressionDataProperty);
  }
}
