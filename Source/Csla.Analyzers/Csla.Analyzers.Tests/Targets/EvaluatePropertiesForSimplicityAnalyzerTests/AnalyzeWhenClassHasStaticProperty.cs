namespace Csla.Analyzers.Tests.Targets.CheckConstructorsAnalyzerTests
{
  public class AnalyzeWhenClassHasStaticProperty
    : BusinessBase<AnalyzeWhenClassHasStaticProperty>
  {
    public static string Data { get; set; }
  }
}
