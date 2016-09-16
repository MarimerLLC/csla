namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments
    : BusinessBase<AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments>
  { }

  public class AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArgumentsList
    : DynamicListBase<AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArgumentsList() { }

    public AnalyzeWhenClassIsDynamicListBaseAndHasPublicConstructorWithArgumentsList(int a) { }
  }
}
