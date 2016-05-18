using System;
using System.Threading.Tasks;

using Csla.Server;

namespace Csla.Test.DataPortal
{
  public class CustomDataPortalServer : IDataPortalServer
  {
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var dp = new DataPortalSelector();
      var result = await dp.Create(objectType, criteria, context, isSync).ConfigureAwait(false);

      ApplicationContext.GlobalContext["CustomDataPortalServer"] = "Create Called";

      return result;
    }

    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var dp = new DataPortalSelector();
      var result = await dp.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);

      ApplicationContext.GlobalContext["CustomDataPortalServer"] = "Fetch Called";

      return result;
    }

    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      var dp = new DataPortalSelector();
      var result = await dp.Update(obj, context, isSync).ConfigureAwait(false);

      ApplicationContext.GlobalContext["CustomDataPortalServer"] = "Update Called";

      return result;
    }

    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var dp = new DataPortalSelector();
      var result = await dp.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);

      ApplicationContext.GlobalContext["CustomDataPortalServer"] = "Delete Called";

      return result;
    }
  }
}