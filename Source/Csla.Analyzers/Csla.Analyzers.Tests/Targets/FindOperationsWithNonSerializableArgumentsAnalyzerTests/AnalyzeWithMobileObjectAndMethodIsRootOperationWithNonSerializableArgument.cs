namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsRootOperationWithNonSerializableArgument>
  {
    private void DataPortal_Fetch(NonSerializableClass x) { }
  }
}