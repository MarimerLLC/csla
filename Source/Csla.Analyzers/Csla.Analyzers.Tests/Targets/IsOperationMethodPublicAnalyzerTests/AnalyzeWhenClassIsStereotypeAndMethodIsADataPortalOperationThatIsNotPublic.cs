using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public class AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic>
  {
    private void DataPortal_Fetch() { }
  }
}