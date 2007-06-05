using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using Csla.Server;
using Csla.Server.Hosts;
using Csla.Server.Hosts.WcfChannel;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using WCF.
  /// </summary>
  public class WcfProxy : Csla.DataPortalClient.IDataPortalProxy
  {
    #region IDataPortalProxy Members

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public bool IsServerRemote
    {
      get { return true; }
    }

    #endregion

    #region IDataPortalServer Members

    const string _endPoint = "WcfDataPortal";

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      ChannelFactory<IWcfPortal> cf = new ChannelFactory<IWcfPortal>(_endPoint);
      IWcfPortal svr = cf.CreateChannel();
      WcfResponse response =
        svr.Create(new CreateRequest(objectType, criteria, context));
      cf.Close();

      object result = response.Result;
      if (result is Exception)
        throw (Exception)result;
      return (DataPortalResult)result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      ChannelFactory<IWcfPortal> cf = new ChannelFactory<IWcfPortal>(_endPoint);
      IWcfPortal svr = cf.CreateChannel();
      WcfResponse response =
        svr.Fetch(new FetchRequest(objectType, criteria, context));
      cf.Close();

      object result = response.Result;
      if (result is Exception)
        throw (Exception)result;
      return (DataPortalResult)result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      ChannelFactory<IWcfPortal> cf = new ChannelFactory<IWcfPortal>(_endPoint);
      IWcfPortal svr = cf.CreateChannel();
      WcfResponse response =
        svr.Update(new UpdateRequest(obj, context));
      cf.Close();

      object result = response.Result;
      if (result is Exception)
        throw (Exception)result;
      return (DataPortalResult)result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Delete(object criteria, DataPortalContext context)
    {
      ChannelFactory<IWcfPortal> cf = new ChannelFactory<IWcfPortal>(_endPoint);
      IWcfPortal svr = cf.CreateChannel();
      WcfResponse response =
        svr.Delete(new DeleteRequest(criteria, context));
      cf.Close();

      object result = response.Result;
      if (result is Exception)
        throw (Exception)result;
      return (DataPortalResult)result;
    }

    #endregion
  }
}
