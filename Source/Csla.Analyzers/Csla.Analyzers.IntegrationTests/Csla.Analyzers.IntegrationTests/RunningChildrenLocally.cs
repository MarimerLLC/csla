using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class RunningChildrenLocally
    : BusinessBase<RunningChildrenLocally>
  {
    [RunLocal]
    private void DataPortal_Fetch() { }

    [RunLocal]
    private void Child_Fetch() { }
  }
}
