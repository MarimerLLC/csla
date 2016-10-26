namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsChildOperationWithNonSerializableArgument>
  {
    private void Child_Fetch(NonSerializableClass x) { }
  }
}