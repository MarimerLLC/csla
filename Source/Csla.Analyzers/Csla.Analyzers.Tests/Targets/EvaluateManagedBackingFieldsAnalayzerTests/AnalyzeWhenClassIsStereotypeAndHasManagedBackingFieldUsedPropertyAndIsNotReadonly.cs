namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotReadonly>
  {
    public static PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }
  }
}
