#if !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="WcfProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Csla.Server;
using Csla.Server.Hosts.WcfChannel;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using WCF.
  /// </summary>
  public class WcfProxy : IDataPortalProxy
  {
    private static System.ServiceModel.Channels.Binding _defaultBinding;
    private const int TimeoutInMinutes = 10;
    private static string _defaultEndPoint = "WcfDataPortal";

    /// <summary>
    /// Gets or sets the default binding used to initialize
    /// future instances of WcfProxy.
    /// </summary>
    public static System.ServiceModel.Channels.Binding DefaultBinding
    {
      get 
      {
        if (_defaultBinding == null)
        {
          _defaultBinding = new WSHttpBinding();
          WSHttpBinding binding = (WSHttpBinding)_defaultBinding;
          binding.MaxReceivedMessageSize = int.MaxValue;
          binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
          {
            MaxBytesPerRead = int.MaxValue,
            MaxDepth = int.MaxValue,
            MaxArrayLength = int.MaxValue,
            MaxStringContentLength = int.MaxValue,
            MaxNameTableCharCount = int.MaxValue
          };

          binding.ReceiveTimeout = TimeSpan.FromMinutes(TimeoutInMinutes);
          binding.SendTimeout = TimeSpan.FromMinutes(TimeoutInMinutes);
          binding.OpenTimeout = TimeSpan.FromMinutes(TimeoutInMinutes);
        } 
        return _defaultBinding; 
      }
      set { _defaultBinding = value; }
    }

    /// <summary>
    /// Gets or sets the default URL address
    /// for the data portal server.
    /// </summary>
    /// <remarks>
    /// Deprecated: use ApplicationContext.DataPortalUrlString
    /// </remarks>
    public static string DefaultUrl
    {
      get { return ApplicationContext.DataPortalUrlString; }
      set { ApplicationContext.DataPortalUrlString = value; }
    }

    /// <summary>
    /// Gets or sets the default WCF endpoint
    /// name for the data portal server.
    /// </summary>
    public static string DefaultEndPoint
    {
      get { return _defaultEndPoint; }
      set { _defaultEndPoint = value; }
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the DefaultUrl and DefaultBinding
    /// values.
    /// </summary>
    public WcfProxy()
    {
      this.DataPortalUrl = WcfProxy.DefaultUrl;
      this.Binding = WcfProxy.DefaultBinding;
      this.EndPoint = WcfProxy.DefaultEndPoint;
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied URL and DefaultBinding
    /// values.
    /// </summary>
    /// <param name="dataPortalUrl">Server endpoint URL</param>
    public WcfProxy(string dataPortalUrl)
    {
      this.DataPortalUrl = dataPortalUrl;
      this.Binding = WcfProxy.DefaultBinding;
      this.EndPoint = WcfProxy.DefaultEndPoint;
    }
    
    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public bool IsServerRemote
    {
      get { return true; }
    }

    /// <summary>
    /// Gets the binding object used by this proxy instance.
    /// </summary>
    public System.ServiceModel.Channels.Binding Binding { get; protected set; }

    /// <summary>
    /// Gets the URL address for the data portal server
    /// used by this proxy instance.
    /// </summary>
    public string DataPortalUrl { get; protected set; }

    /// <summary>
    /// Gets the WCF endpoint name for the data portal
    /// server used by this proxy instance.
    /// </summary>
    public string EndPoint { get; protected set; }

    /// <summary>
    /// Returns an instance of the channel factory
    /// used by GetProxy() to create the WCF proxy
    /// object.
    /// </summary>
    /// <remarks>
    /// If DataPortalUrl is given, the factory will be created using it
    /// and the default binding. Otherwise, it will use the endpoint
    /// config from app/web.config.
    /// </remarks>
    protected virtual ChannelFactory<IWcfPortal> GetChannelFactory()
    {
      if (!string.IsNullOrEmpty(ApplicationContext.DataPortalUrlString))
        return new ChannelFactory<IWcfPortal>(Binding, new EndpointAddress(ApplicationContext.DataPortalUrl));
      else
        return new ChannelFactory<IWcfPortal>(EndPoint);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        var request = new CreateRequest(objectType, criteria, context);
        if (isSync)
          response = proxy.Create(request);
        else
          response = await proxy.CreateAsync(request);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
#pragma warning disable 1998
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        var request = new FetchRequest(objectType, criteria, context);
        if (isSync)
          response = proxy.Fetch(request);
        else
          response = await proxy.FetchAsync(request);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
#pragma warning disable 1998
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        var request = new UpdateRequest(obj, context);
        if (isSync)
          response = proxy.Update(request);
        else
          response = await proxy.UpdateAsync(request);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
#pragma warning disable 1998
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
      try
      {
        var request = new DeleteRequest(objectType, criteria, context);
        if (isSync)
          response = proxy.Delete(request);
        else
          response = await proxy.DeleteAsync(request);
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
  }
}
#endif
