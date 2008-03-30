using System;
using System.Configuration;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using the
  /// .NET Remoting technology.
  /// </summary>
  public class RemotingProxy : DataPortalClient.IDataPortalProxy
  {

    #region Configure Remoting

    /// <summary>
    /// Configure .NET Remoting to use a binary
    /// serialization technology even when using
    /// the HTTP channel. Also ensures that the
    /// user's Windows credentials are passed to
    /// the server appropriately.
    /// </summary>
    static RemotingProxy()
    {
      // create and register a custom HTTP channel
      // that uses the binary formatter
      Hashtable properties = new Hashtable();
      properties["name"] = "HttpBinary";

      if (ApplicationContext.AuthenticationType == "Windows" || AlwaysImpersonate)
      {
        // make sure we pass the user's Windows credentials
        // to the server
        properties["useDefaultCredentials"] = true;
      }

      BinaryClientFormatterSinkProvider 
        formatter = new BinaryClientFormatterSinkProvider();
      HttpChannel channel = new HttpChannel(properties, formatter, null);
      ChannelServices.RegisterChannel(channel, EncryptChannel);
    }

    private static bool AlwaysImpersonate
    {
      get
      {
        bool result =
          (ConfigurationManager.AppSettings["CslaAlwaysImpersonate"] == "true");
        return result;
      }
    }

    private static bool EncryptChannel
    {
      get 
      {
        bool encrypt = 
          (ConfigurationManager.AppSettings["CslaEncryptRemoting"] == "true");
        return encrypt; 
      }
    }

    #endregion

    private Server.IDataPortalServer _portal;

    private Server.IDataPortalServer Portal
    {
      get
      {
        if (_portal == null)
          _portal = (Server.IDataPortalServer)Activator.GetObject(
            typeof(Server.Hosts.RemotingPortal),
              ApplicationContext.DataPortalUrl.ToString());
        return _portal;
      }
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
      return Portal.Create(objectType, criteria, context);
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
    public Server.DataPortalResult Fetch(Type objectType, object criteria, Server.DataPortalContext context)
    {
      return Portal.Fetch(objectType, criteria, context);
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
      return Portal.Update(obj, context);
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
      return Portal.Delete(objectType, criteria, context);
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
  }
}