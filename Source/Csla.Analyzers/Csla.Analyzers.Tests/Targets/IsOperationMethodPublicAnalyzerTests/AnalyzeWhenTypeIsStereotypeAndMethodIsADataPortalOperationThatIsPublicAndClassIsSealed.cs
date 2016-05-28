using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public sealed class AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed
    : BusinessBase<AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsSealed>
  {
    public void DataPortal_Fetch() { }
  }
}