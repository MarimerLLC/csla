namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments
    : BusinessBase<AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArguments>
  { }

  public class AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList
    : BusinessBindingListBase<AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList, AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList() { }

    public AnalyzeWhenClassIsBusinessBindingListBaseAndHasPublicConstructorWithArgumentsList(int a) { }
  }
}
