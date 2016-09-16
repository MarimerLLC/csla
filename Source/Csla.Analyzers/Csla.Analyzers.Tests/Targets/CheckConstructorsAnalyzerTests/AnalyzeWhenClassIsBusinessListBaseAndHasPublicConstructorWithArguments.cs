namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments
    : BusinessBase<AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments>
  { }

  public class AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList
    : BusinessListBase<AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList, AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList() { }

    public AnalyzeWhenClassIsBusinessListBaseAndHasPublicConstructorWithArgumentsList(int a) { }
  }
}
