#if !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="MobileProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Server;
using Csla.WcfPortal;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using WCF.
  /// </summary>
  public class MobileProxy : IDataPortalProxy
  {
    private static System.ServiceModel.Channels.Binding _defaultBinding;
    private const int TimeoutInMinutes = 10;
    private static string _defaultEndPoint = "WcfDataPortal";

    /// <summary>
    /// Gets or sets the default binding used to initialize
    /// future instances of MobileProxy.
    /// </summary>
    public static System.ServiceModel.Channels.Binding DefaultBinding
    {
      get
      {
        if (_defaultBinding == null)
        {
          _defaultBinding = new BasicHttpBinding();
          BasicHttpBinding binding = (BasicHttpBinding)_defaultBinding;
          binding.MaxBufferSize = int.MaxValue;
          binding.MaxReceivedMessageSize = int.MaxValue;
          binding.ReceiveTimeout = TimeSpan.FromMinutes(TimeoutInMinutes);
          binding.SendTimeout = TimeSpan.FromMinutes(TimeoutInMinutes);
          binding.OpenTimeout = TimeSpan.FromMinutes(TimeoutInMinutes);
        };
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
    public MobileProxy()
    {
      this.DataPortalUrl = MobileProxy.DefaultUrl;
      this.Binding = MobileProxy.DefaultBinding;
      this.EndPoint = MobileProxy.DefaultEndPoint;
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
    /// Gets an instance of the WcfPortalClient object
    /// for use in communicating with the server.
    /// </summary>
    /// <remarks>
    /// If both DataPortalUrl and Binding are non-null then
    /// those values are used to initialize the proxy, otherwise
    /// the proxy is initialized using values from the
    /// system.serviceModel element in the Silverlight
    /// client-side config file.
    /// </remarks>
    protected virtual WcfPortal.WcfPortalClient GetProxy()
    {
      if (!string.IsNullOrEmpty(this.DataPortalUrl) && this.Binding != null)
      {
        var address = new EndpointAddress(this.DataPortalUrl);
        return new WcfPortal.WcfPortalClient(this.Binding, address);
      }
      else
      {
        if (string.IsNullOrEmpty(EndPoint))
          return new WcfPortal.WcfPortalClient();
        else
          return new WcfPortal.WcfPortalClient(EndPoint);
      }
    }

    #region Criteria

    private WcfPortal.CriteriaRequest GetBaseCriteriaRequest()
    {
      var request = new WcfPortal.CriteriaRequest();
      request.CriteriaData = null;
      request.ClientContext = MobileFormatter.Serialize(ApplicationContext.ClientContext);
      request.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = MobileFormatter.Serialize(null);
      }
      else
      {
        request.Principal = MobileFormatter.Serialize(ApplicationContext.User);
      }
      request.ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
      return request;
    }

    private WcfPortal.UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var request = new WcfPortal.UpdateRequest();
      request.ObjectData = null;
      request.ClientContext = MobileFormatter.Serialize(ApplicationContext.ClientContext);
      request.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = MobileFormatter.Serialize(null);
      }
      else
      {
        request.Principal = MobileFormatter.Serialize(ApplicationContext.User);
      }
      request.ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
      return request;
    }

    #endregion

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
      var request = GetBaseCriteriaRequest();
      request.TypeName = objectType.AssemblyQualifiedName;
      if (!(criteria is IMobileObject))
      {
        criteria = new PrimitiveCriteria(criteria);
      }
      request.CriteriaData = MobileFormatter.Serialize(criteria);
      request = ConvertRequest(request);

      var proxy = GetProxy();
      DataPortalResult result = null;
      try
      {
        Csla.WcfPortal.WcfResponse response = null;
        if (isSync)
          response = proxy.Create(request);
        else
        {
#if !NET40
          response = await proxy.CreateAsync(request).ConfigureAwait(false);
#else
          var tcs = new TaskCompletionSource<WcfResponse>();
          proxy.CreateCompleted += (o, e) =>
          {
            if (e.Error == null)
              tcs.TrySetResult(e.Result);
            else
              tcs.TrySetException(e.Error);
          };
          proxy.CreateAsync(request);
          await tcs.Task;
          response = tcs.Task.Result;
#endif
        }

        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
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
      var request = GetBaseCriteriaRequest();
      request.TypeName = objectType.AssemblyQualifiedName;
      if (!(criteria is IMobileObject))
      {
        criteria = new PrimitiveCriteria(criteria);
      }
      request.CriteriaData = MobileFormatter.Serialize(criteria);

      request = ConvertRequest(request);

      var proxy = GetProxy();
      DataPortalResult result = null;
      try
      {
        Csla.WcfPortal.WcfResponse response = null;
        if (isSync)
          response = proxy.Fetch(request);
        else
        {
#if !NET40
          response = await proxy.FetchAsync(request).ConfigureAwait(false);
#else
          var tcs = new TaskCompletionSource<WcfResponse>();
          proxy.FetchCompleted += (o, e) =>
          {
            if (e.Error == null)
              tcs.TrySetResult(e.Result);
            else
              tcs.TrySetException(e.Error);
          };
          proxy.FetchAsync(request);
          await tcs.Task;
          response = tcs.Task.Result;
#endif
      }
      response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
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
      var request = GetBaseUpdateCriteriaRequest();
      request.ObjectData = MobileFormatter.Serialize(obj);
      request = ConvertRequest(request);

      var proxy = GetProxy();
      DataPortalResult result = null;
      try
      {
        Csla.WcfPortal.WcfResponse response = null;
        if (isSync)
          response = proxy.Update(request);
        else
        {
#if !NET40
          response = await proxy.UpdateAsync(request).ConfigureAwait(false);
#else
          var tcs = new TaskCompletionSource<WcfResponse>();
          proxy.UpdateCompleted += (o, e) =>
          {
            if (e.Error == null)
              tcs.TrySetResult(e.Result);
            else
              tcs.TrySetException(e.Error);
          };
          proxy.UpdateAsync(request);
          await tcs.Task;
          response = tcs.Task.Result;
#endif
        }
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var newobj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(newobj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
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
      var request = GetBaseCriteriaRequest();
      request.TypeName = objectType.AssemblyQualifiedName;
      if (!(criteria is IMobileObject))
      {
        criteria = new PrimitiveCriteria(criteria);
      }
      request.CriteriaData = MobileFormatter.Serialize(criteria);
      request = ConvertRequest(request);

      var proxy = GetProxy();
      DataPortalResult result = null;
      try
      {
        Csla.WcfPortal.WcfResponse response = null;
        if (isSync)
          response = proxy.Delete(request);
        else
        {
#if !NET40
          response = await proxy.DeleteAsync(request).ConfigureAwait(false);
#else
          var tcs = new TaskCompletionSource<WcfResponse>();
          proxy.DeleteCompleted += (o, e) =>
          {
            if (e.Error == null)
              tcs.TrySetResult(e.Result);
            else
              tcs.TrySetException(e.Error);
          };
          proxy.DeleteAsync(request);
          await tcs.Task;
          response = tcs.Task.Result;
#endif
        }
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          result = new DataPortalResult(null, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
    }

    #region Extension Method for Requests

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Update request data.</param>
    protected virtual WcfPortal.UpdateRequest ConvertRequest(WcfPortal.UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Criteria request data.</param>
    protected virtual WcfPortal.CriteriaRequest ConvertRequest(WcfPortal.CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data returned from the server.
    /// </summary>
    /// <param name="response">Response data.</param>
    protected virtual WcfPortal.WcfResponse ConvertResponse(WcfPortal.WcfResponse response)
    {
      return response;
    }

    #endregion
  }
}
#endif