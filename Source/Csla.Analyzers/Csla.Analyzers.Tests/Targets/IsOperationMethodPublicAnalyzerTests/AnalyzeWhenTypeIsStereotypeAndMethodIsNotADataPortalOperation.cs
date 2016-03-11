using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public class AnalyzeWhenTypeIsStereotypeAndMethodIsNotADataPortalOperation
    : BusinessBase<AnalyzeWhenTypeIsStereotypeAndMethodIsNotADataPortalOperation>
  {
    public void AMethod() { }
  }
}