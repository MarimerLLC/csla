using Csla.Core;

namespace Csla.Analyzers.IntegrationTests
{
  public interface PublicForInterface
    : IBusinessObject
  {
    [Fetch]
    void DataPortal_Fetch();
  }
}
