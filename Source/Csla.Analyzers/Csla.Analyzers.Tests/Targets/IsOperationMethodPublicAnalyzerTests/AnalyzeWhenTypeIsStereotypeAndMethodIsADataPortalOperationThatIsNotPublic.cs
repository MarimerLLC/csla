using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public class AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic
    : BusinessBase<AnalyzeWhenTypeIsStereotypeAndMethodIsADataPortalOperationThatIsNotPublic>
  {
    private void DataPortal_Fetch() { }
  }
}