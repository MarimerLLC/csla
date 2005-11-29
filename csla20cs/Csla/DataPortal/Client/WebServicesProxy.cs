using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Csla.WebServiceHost;

namespace Csla.DataPortalClient
{
  public class WebServicesProxy : DataPortalClient.IDataPortalProxy
  {
    private WebServiceHost.WebServicePortal GetPortal()
    {
      WebServiceHost.WebServicePortal wsvc = 
        new WebServiceHost.WebServicePortal();
      wsvc.Url = ApplicationContext.DataPortalUrl.ToString();
      return wsvc;
    }


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

    public Server.DataPortalResult Fetch(
      object criteria, Server.DataPortalContext context)
    {
      object result;
      Server.Hosts.WebServicePortal.FetchRequest request = 
        new Server.Hosts.WebServicePortal.FetchRequest();
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

    public Server.DataPortalResult Delete(object criteria, Server.DataPortalContext context)
    {
      object result;
      Server.Hosts.WebServicePortal.DeleteRequest request = new Server.Hosts.WebServicePortal.DeleteRequest();
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

    public bool IsServerRemote
    {
      get { return true; }
    }


    #region Helper functions

    private static byte[] Serialize(object obj)
    {
      if (obj != null)
      {
        MemoryStream buffer = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(buffer, obj);
        return buffer.ToArray();
      }
      return null;
    }

    private static object Deserialize(byte[] obj)
    {
      if (obj != null)
      {
        MemoryStream buffer = new MemoryStream(obj);
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter.Deserialize(buffer);
      }
      return null;
    }

    #endregion

  }
}
