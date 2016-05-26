namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithSerializableArgument
    : BusinessBase<AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithSerializableArgument>
  {
    private void DataPortal_Fetch(int x) { }
  }
}