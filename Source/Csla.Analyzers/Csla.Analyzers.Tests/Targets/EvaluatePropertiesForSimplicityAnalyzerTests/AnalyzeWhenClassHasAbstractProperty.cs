namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public abstract class AnalyzeWhenClassHasAbstractProperty
    : BusinessBase<AnalyzeWhenClassHasAbstractProperty>
  {
    public abstract string Data { get; set; }
  }
}
