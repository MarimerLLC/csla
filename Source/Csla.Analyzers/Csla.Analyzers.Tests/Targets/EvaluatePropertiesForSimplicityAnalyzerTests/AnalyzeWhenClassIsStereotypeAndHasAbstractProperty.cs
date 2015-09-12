namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public abstract class AnalyzeWhenClassIsStereotypeAndHasAbstractProperty
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasAbstractProperty>
  {
    public abstract string Data { get; set; }
  }
}
