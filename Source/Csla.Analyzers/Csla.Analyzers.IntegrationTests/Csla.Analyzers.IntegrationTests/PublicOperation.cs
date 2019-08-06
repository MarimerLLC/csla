using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class PublicOperation
    : BusinessBase<PublicOperation>
  {
    public void DataPortal_Fetch() { }
  }
}
