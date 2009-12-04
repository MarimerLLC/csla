using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using 
  /// Web services.
  /// </summary>
  public class WebServicesProxy : DataPortalClient.IDataPortalProxy
  {
    private WebServiceHost.WebServicePortal GetPortal()
    {
      WebServiceHost.WebServicePortal wsvc = 
        new WebServiceHost.WebServicePortal();
      wsvc.Url = ApplicationContext.DataPortalUrl.ToString();
      return wsvc;
    }


    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public Server.DataPortalResult Create(
      Type objectType, object criteria, Server.DataPortalContext context)
    {
      object result;
      Csla.Server.Hosts.WebServicePortal.CreateRequest
        request = new Csla.Server.Hosts.WebServicePortal.CreateRequest();
      request.ObjectType = objectType;
      request.Criteria = criteria;
      request.Context = context;

      using (WebServiceHost.WebServicePortal wsvc = GetPortal())
      {
        byte[] rd = Serialize(request);
        byte[] rp = wsvc.Create(rd);
        result = Deserialize(rp);
      }

      if (result is Exception)
        throw (Exception)result;
      return (Server.DataPortalResult)result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public Server.DataPortalResult Fetch(
      Type objectType, object criteria, Server.DataPortalContext context)
    {
      object result;
      Server.Hosts.WebServicePortal.FetchRequest request = 
        new Server.Hosts.WebServicePortal.FetchRequest();
      request.ObjectType = objectType;
      request.Criteria = criteria;
      request.Context = context;

      using (WebServiceHost.WebServicePortal wsvc = GetPortal())
      {
        result = Deserialize(wsvc.Fetch(Serialize(request)));
      }

      if (result is Exception)
        throw (Exception)result;
      return (Server.DataPortalResult)result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public Server.DataPortalResult Update(object obj, Server.DataPortalContext context)
    {
      object result;
      Server.Hosts.WebServicePortal.UpdateRequest request = new Server.Hosts.WebServicePortal.UpdateRequest();
      request.Object = obj;
      request.Context = context;

      using (WebServiceHost.WebServicePortal wsvc = GetPortal())
      {
        result = Deserialize(wsvc.Update(Serialize(request)));
      }

      if (result is Exception)
        throw (Exception)result;
      return (Server.DataPortalResult)result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public Server.DataPortalResult Delete(Type objectType, object criteria, Server.DataPortalContext context)
    {
      object result;
      Server.Hosts.WebServicePortal.DeleteRequest request = 
        new Server.Hosts.WebServicePortal.DeleteRequest();
      request.ObjectType = objectType;
      request.Criteria = criteria;
      request.Context = context;

      using (WebServiceHost.WebServicePortal wsvc = GetPortal())
      {
        result = Deserialize(wsvc.Delete(Serialize(request)));
      }

      if (result is Exception)
        throw (Exception)result;
      return (Server.DataPortalResult)result;
    }

    /// <summary>
    /// Get a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    public bool IsServerRemote
    {
      get { return true; }
    }


    #region Helper functions

    private static byte[] Serialize(object obj)
    {
      if (obj != null)
      {
        using (MemoryStream buffer = new MemoryStream())
        {
          BinaryFormatter formatter = new BinaryFormatter();
          formatter.Serialize(buffer, obj);
          return buffer.ToArray();
        }
      }
      return null;
    }

    private static object Deserialize(byte[] obj)
    {
      if (obj != null)
      {
        using (MemoryStream buffer = new MemoryStream(obj))
        {
          BinaryFormatter formatter = new BinaryFormatter();
          return formatter.Deserialize(buffer);
        }
      }
      return null;
    }

    #endregion

  }
}
