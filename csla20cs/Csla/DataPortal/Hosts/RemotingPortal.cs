using System;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through .NET Remoting.
  /// </summary>
  public class RemotingPortal : MarshalByRefObject, Server.IDataPortalServer
  {
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Create(objectType, criteria, context);
    }

    public DataPortalResult Fetch(object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Fetch(criteria, context);
    }

    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Update(obj, context);
    }

    public DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      Server.DataPortal portal = new DataPortal();
      return portal.Delete(criteria, context);
    }
  }
}