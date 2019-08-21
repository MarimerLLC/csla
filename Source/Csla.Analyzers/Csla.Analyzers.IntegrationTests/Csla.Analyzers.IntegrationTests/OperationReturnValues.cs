using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class OperationReturnValues
    : BusinessBase<OperationReturnValues>
  {
    private void Foo() { }
    [Fetch]
    private void DataPortal_Fetch(Guid id) { }
    [Fetch]
    private Task DataPortal_Fetch(int id) => Task.CompletedTask;
    [Fetch]
    private string DataPortal_Fetch() => string.Empty;
  }

  public sealed class OperationReturnValuesNotCsla
  {
    private string DataPortal_Fetch() => string.Empty;
  }
}