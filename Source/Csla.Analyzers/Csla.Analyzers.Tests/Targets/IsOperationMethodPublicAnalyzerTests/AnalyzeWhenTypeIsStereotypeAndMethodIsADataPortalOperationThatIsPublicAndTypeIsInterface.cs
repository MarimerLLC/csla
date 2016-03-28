using Csla.Core;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  public interface AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndTypeIsInterface
    : IBusinessObject
  {
    void DataPortal_Fetch();
  }
}