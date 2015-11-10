namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotStatic>
  {
    public readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }
  }
}
