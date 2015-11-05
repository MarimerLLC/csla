namespace Csla.Analyzers.Tests.Targets.EvaluateManagedBackingFieldsAnalayzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasManagedBackingFieldNotUsedByProperty>
  {
    public static readonly PropertyInfo<string> DataProperty =
      RegisterProperty<string>(_ => _.Data);
    public string Data { get; set; }
  }
}
