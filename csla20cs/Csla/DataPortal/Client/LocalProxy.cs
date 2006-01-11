using System;
using Csla.Server;

namespace Csla.DataPortalClient
{
  public class LocalProxy : DataPortalClient.IDataPortalProxy
  {
    private Server.IDataPortalServer _portal =
      new Server.DataPortal();

    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      return _portal.Create(objectType, criteria, context);
    }

    public DataPortalResult Fetch(object criteria, DataPortalContext context)
    {
      return _portal.Fetch(criteria, context);
    }

    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      return _portal.Update(obj, context);
    }

    public DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      return _portal.Delete(criteria, context);
    }

    public bool IsServerRemote
    {
      get { return false; }
    }

  }
}
