using Csla;
using System;

namespace Csla.Analyzers.Tests.Targets.IsOperationMethodPublicMakeNonPublicCodeFixTests
{
  [Serializable]
  public sealed class VerifyGetFixesWhenClassIsSealed
    : BusinessBase<VerifyGetFixesWhenClassIsSealed>
  {
    public void DataPortal_Fetch() { }
  }
}