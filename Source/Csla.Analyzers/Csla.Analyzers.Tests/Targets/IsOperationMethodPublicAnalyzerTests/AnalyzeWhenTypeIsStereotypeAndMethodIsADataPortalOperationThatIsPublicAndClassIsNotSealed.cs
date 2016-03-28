using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public class AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed
    : BusinessBase<AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsPublicAndClassIsNotSealed>
  {
    public void DataPortal_Fetch() { }
  }
}