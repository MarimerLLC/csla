namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments
    : CommandBase<AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments>
  {
    public AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments() { }

    public AnalyzeWhenClassIsCommandBaseAndHasPublicConstructorWithArguments(int a) { }
  }
}
