using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicAnalyzerTests
{
  [Serializable]
  public class AnalyzeWhenClassIsStereotypeAndMethodIsNotADataPortalOperation
    : BusinessBase<AnalyzeWhenClassIsStereotypeAndMethodIsNotADataPortalOperation>
  {
    public void AMethod() { }
  }
}