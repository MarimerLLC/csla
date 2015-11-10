namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldUsedPropertyAndIsNotPublic>
  {
    static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }
  }
}
