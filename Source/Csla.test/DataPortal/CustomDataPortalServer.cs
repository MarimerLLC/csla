using System;
using System.Threading.Tasks;

using Csla.Server;

namespace Csla.Test.DataPortal
{
  public class CustomDataPortalServer : IDataPortalServer
  {
    private readonly DataPortalSelector _dataPortalSelector;

    public CustomDataPortalServer(DataPortalSelector dataPortalSelector)
    {
      _dataPortalSelector = dataPortalSelector;
    }

    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var result = await _dataPortalSelector.Create(objectType, criteria, context, isSync).ConfigureAwait(false);

      TestResults.Add("CustomDataPortalServer", "Create Called");

      return result;
    }

    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var result = await _dataPortalSelector.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);

      TestResults.Add("CustomDataPortalServer", "Fetch Called");

      return result;
    }

    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      var result = await _dataPortalSelector.Update(obj, context, isSync).ConfigureAwait(false);

      TestResults.Add("CustomDataPortalServer", "Update Called");

      return result;
    }

    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var result = await _dataPortalSelector.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);

      TestResults.Add("CustomDataPortalServer", "Delete Called");

      return result;
    }
  }
}