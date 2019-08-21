using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class PublicOperation
    : BusinessBase<PublicOperation>
  {
    [Fetch]
    public void DataPortal_Fetch() { }
  }
}
