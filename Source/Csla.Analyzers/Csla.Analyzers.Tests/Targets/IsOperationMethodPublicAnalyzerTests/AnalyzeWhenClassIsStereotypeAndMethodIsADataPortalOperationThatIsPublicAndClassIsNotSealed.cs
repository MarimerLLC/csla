using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public class AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed>
  {
    public void DataPortal_Fetch() { }
  }
}