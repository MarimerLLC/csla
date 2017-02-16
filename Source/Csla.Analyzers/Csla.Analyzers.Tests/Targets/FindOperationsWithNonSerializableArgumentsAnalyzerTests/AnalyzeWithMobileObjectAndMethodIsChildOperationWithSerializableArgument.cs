namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsChildOperationWithSerializableArgument>
  {
    private void Child_Fetch(int x) { }
  }
}