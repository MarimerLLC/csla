namespace Csla.Analyzers.Tests.Targets.FindOperationsWithNonSerializableArgumentsAnalyzerTests
{
  public class AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithNonSerializableArgument
    : BusinessBase<AnalyzeWhenClassIsMobileObjectAndMethodIsOperationWithNonSerializableArgument>
  {
    private void DataPortal_Fetch(NonSerializableClass x) { }
  }

  public class NonSerializableClass { }
}