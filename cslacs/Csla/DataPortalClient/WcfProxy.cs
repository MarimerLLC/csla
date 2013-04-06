using System;
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

    private string _endPoint = "WcfDataPortal";

    /// <summary>
    /// Gets or sets the WCF endpoint used
    /// to contact the server.
    /// </summary>
    /// <remarks>
    /// The default value is WcfDataPortal.
    /// </remarks>
    protected string EndPoint
    {
      get
      {
        return _endPoint;
      }
      set
      {
        _endPoint = value;
      }
    }

    /// <summary>
    /// Returns an instance of the channel factory
    /// used by GetProxy() to create the WCF proxy
    /// object.
    /// </summary>
    protected virtual ChannelFactory<IWcfPortal> GetChannelFactory()
    {
      return new ChannelFactory<IWcfPortal>(_endPoint);
    }

    /// <summary>
    /// Returns the WCF proxy object used for
    /// communication with the data portal
    /// server.
    /// </summary>
    /// <param name="cf">
    /// The ChannelFactory created by GetChannelFactory().
    /// </param>
    protected virtual IWcfPortal GetProxy(ChannelFactory<IWcfPortal> cf)
    {
      return cf.CreateChannel();
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
    public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      IWcfPortal svr = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        response =
          svr.Create(new CreateRequest(objectType, criteria, context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }
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
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      IWcfPortal svr = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        response =
               svr.Fetch(new FetchRequest(objectType, criteria, context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }


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
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      IWcfPortal svr = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        response =
              svr.Update(new UpdateRequest(obj, context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }


      object result = response.Result;
      if (result is Exception)
        throw (Exception)result;
      return (DataPortalResult)result;
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
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      IWcfPortal svr = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        response =
              svr.Delete(new DeleteRequest(objectType, criteria, context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }


      object result = response.Result;
      if (result is Exception)
        throw (Exception)result;
      return (DataPortalResult)result;
    }

    #endregion
  }
}