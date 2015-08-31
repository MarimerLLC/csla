using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicMakeNonPublicCodeFixTests
{
  [Serializable]
  public class VerifyGetFixesWhenClassIsNotSealed
    : BusinessBase<VerifyGetFixesWhenClassIsNotSealed>
  {
    public void DataPortal_Fetch() { }
  }
}