namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassIsStereotypeAndHasStaticConstructor
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndHasStaticConstructor>
  {
    static AnalyzeWhenClassIsStereotypeAndHasStaticConstructor() { }

    private AnalyzeWhenClassIsStereotypeAndHasStaticConstructor(int a) { }
  }
}
