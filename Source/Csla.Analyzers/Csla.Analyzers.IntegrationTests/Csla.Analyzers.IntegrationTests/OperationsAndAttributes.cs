using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class OperationsAndAttributes
    : BusinessBase<OperationsAndAttributes>
  {
    private void DataPortal_Fetch() { }
    protected override void DataPortal_Update() { }

    protected override void Child_Create() { }
    private void Child_Execute() { }
  }
}