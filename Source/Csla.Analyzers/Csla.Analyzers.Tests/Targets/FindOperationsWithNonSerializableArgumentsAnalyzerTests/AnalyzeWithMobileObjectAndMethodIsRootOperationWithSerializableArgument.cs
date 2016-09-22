namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument
    : BusinessBase<AnalyzeWithMobileObjectAndMethodIsRootOperationWithSerializableArgument>
  {
    private void DataPortal_Fetch(int x) { }
  }
}