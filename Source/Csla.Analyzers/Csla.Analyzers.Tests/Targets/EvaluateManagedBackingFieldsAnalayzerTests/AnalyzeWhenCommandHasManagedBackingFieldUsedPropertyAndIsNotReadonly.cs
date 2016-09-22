namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenCommandHasManagedBackingFieldUsedPropertyAndIsNotReadonly
    : CommandBase<AnalyzeWhenCommandHasManagedBackingFieldUsedPropertyAndIsNotReadonly>
  {
    public static PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return ReadProperty(DataProperty); }
      set { LoadProperty(DataProperty, value); }
    }

    public static PropertyInfo<string> ExpressionDataProperty =
      RegisterProperty<string>(_ => _.ExpressionData);
    public string ExpressionData => ReadProperty(ExpressionDataProperty);
  }
}
