using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public sealed class AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed>
  {
    public void DataPortal_Fetch() { }
  }
}