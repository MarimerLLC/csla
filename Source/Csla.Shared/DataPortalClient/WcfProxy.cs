#if !NETFX_PHONE && !NETCORE && !PCL46 && !ANDROID && !NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="WcfProxy.cs" company="Marimer LLC">
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
#if !NETFX_CORE && !(IOS || ANDROID)
using Csla.Server.Hosts;
using Csla.Server.Hosts.WcfChannel;
#endif

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
#if !NETFX_CORE && !(IOS || ANDROID)
          _defaultBinding = new WSHttpBinding();
          WSHttpBinding binding = (WSHttpBinding)_defaultBinding;
#else
          _defaultBinding = new BasicHttpBinding();
          BasicHttpBinding binding = (BasicHttpBinding)_defaultBinding;
          binding.MaxBufferSize = int.MaxValue;
#endif
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

#if !(ANDROID || IOS) && !NETFX_CORE
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
#else
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
#if NETFX_CORE || IOS || ANDROID
        return new WcfPortal.WcfPortalClient();
#else
        if (string.IsNullOrEmpty(EndPoint))
          return new WcfPortal.WcfPortalClient();
        else
          return new WcfPortal.WcfPortalClient(EndPoint);
#endif
      }
    }
#endif

#if (ANDROID || IOS) || NETFX_CORE
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
#if NETFX_CORE
      var language = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages[0];
      request.ClientCulture = language;
      request.ClientUICulture = language;
#else
      request.ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
#endif
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
#if NETFX_CORE
      var language = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages[0];
      request.ClientCulture = language;
      request.ClientUICulture = language;
#else
      request.ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
#endif
      return request;
    }

#endregion
#endif

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
#if !(ANDROID || IOS) && !NETFX_CORE
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
#if NET40
      try
      {
        var request = new CreateRequest(objectType, criteria, context);
        if (isSync)
        {
          response = proxy.Create(request);
        }
        else
        {
          var worker = new Csla.Threading.BackgroundWorker();
          var tcs = new TaskCompletionSource<WcfResponse>();
          worker.RunWorkerCompleted += (o, e) =>
            {
              tcs.SetResult((WcfResponse)e.Result);
            };
          worker.DoWork += (o, e) =>
            {
              e.Result = proxy.Create(request);
            };
          worker.RunWorkerAsync();
          response = await tcs.Task;
        }
        if (cf != null)
          cf.Close();
        object result = response.Result;
        if (result is Exception)
          throw (Exception)result;
        return (DataPortalResult)result;
      }
      catch
      {
        cf.Abort();
        throw;
      }
#else
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
#endif
#else
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
#if !NETFX_CORE && !(IOS || ANDROID)
      var tcs = new TaskCompletionSource<DataPortalResult>();
      proxy.CreateCompleted += (s, e) => 
        {
          try
          {
            Csla.WcfPortal.WcfResponse response = null;
            if (e.Error == null)
              response = ConvertResponse(e.Result);
            ContextDictionary globalContext = null;
            if (response != null)
              globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
            if (e.Error == null && response != null && response.ErrorData == null)
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
              result = new DataPortalResult(null, e.Error, globalContext);
            }
          }
          catch (Exception ex)
          {
            result = new DataPortalResult(null, ex, null);
          }
          finally
          {
            tcs.SetResult(result);
          }
        };
      proxy.CreateAsync(request);
      var finalresult = await tcs.Task;
      if (finalresult.Error != null)
        throw finalresult.Error;
      return finalresult;
#else
      try
      {
        var response = await proxy.CreateAsync(request);
        response = ConvertResponse(response);
        if (response == null)
          throw new DataPortalException("null response", null);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
#endif
#endif
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
#if !(ANDROID || IOS) && !NETFX_CORE
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
#if NET40
      try
      {
        var request = new FetchRequest(objectType, criteria, context);
        if (isSync)
        {
          response = proxy.Fetch(request);
        }
        else
        {
          var worker = new Csla.Threading.BackgroundWorker();
          var tcs = new TaskCompletionSource<WcfResponse>();
          worker.RunWorkerCompleted += (o, e) =>
            {
              tcs.SetResult((WcfResponse)e.Result);
            };
          worker.DoWork += (o, e) =>
            {
              e.Result = proxy.Fetch(request);
            };
          worker.RunWorkerAsync();
          response = await tcs.Task;
        }
        if (cf != null)
          cf.Close();
        object result = response.Result;
        if (result is Exception)
          throw (Exception)result;
        return (DataPortalResult)result;
      }
      catch
      {
        cf.Abort();
        throw;
      }
#else
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
#endif
#else // WinRT and Silverlight
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

#if !NETFX_CORE  && !(IOS || ANDROID)
      var tcs = new TaskCompletionSource<DataPortalResult>();
      proxy.FetchCompleted += (s, e) => 
        {
          try
          {
            Csla.WcfPortal.WcfResponse response = null;
            if (e.Error == null)
              response = ConvertResponse(e.Result);
            ContextDictionary globalContext = null;
            if (response != null)
              globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
            if (e.Error == null && response != null && response.ErrorData == null)
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
              result = new DataPortalResult(null, e.Error, globalContext);
            }
          }
          catch (Exception ex)
          {
            result = new DataPortalResult(null, ex, null);
          }
          finally
          {
            tcs.SetResult(result);
          }
        };
      proxy.FetchAsync(request);
      var finalresult = await tcs.Task;
      if (finalresult.Error != null)
        throw finalresult.Error;
      return finalresult;

#else // WinRT
      try
      {
        var response = await proxy.FetchAsync(request);
        response = ConvertResponse(response);
        if (response == null)
          throw new DataPortalException("null response", null);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
#endif
#endif
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
#if !(ANDROID || IOS) && !NETFX_CORE
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
#if NET40
      try
      {
        var request = new UpdateRequest(obj, context);
        if (isSync)
        {
          response = proxy.Update(request);
        }
        else
        {
          var worker = new Csla.Threading.BackgroundWorker();
          var tcs = new TaskCompletionSource<WcfResponse>();
          worker.RunWorkerCompleted += (o, e) =>
            {
              tcs.SetResult((WcfResponse)e.Result);
            };
          worker.DoWork += (o, e) =>
            {
              e.Result = proxy.Update(request);
            };
          worker.RunWorkerAsync();
          response = await tcs.Task;
        }
        if (cf != null)
          cf.Close();
        object result = response.Result;
        if (result is Exception)
          throw (Exception)result;
        return (DataPortalResult)result;
      }
      catch
      {
        cf.Abort();
        throw;
      }
#else
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
#endif
#else
      var request = GetBaseUpdateCriteriaRequest();
      request.ObjectData = MobileFormatter.Serialize(obj);
      request = ConvertRequest(request);

      var proxy = GetProxy();
      DataPortalResult result = null;
#if !NETFX_CORE && !(IOS || ANDROID)
      var tcs = new TaskCompletionSource<DataPortalResult>();
      proxy.UpdateCompleted += (s, e) => 
        {
          try
          {
            Csla.WcfPortal.WcfResponse response = null;
            if (e.Error == null)
              response = ConvertResponse(e.Result);
            ContextDictionary globalContext = null;
            if (response != null)
              globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
            if (e.Error == null && response != null && response.ErrorData == null)
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
              result = new DataPortalResult(null, e.Error, globalContext);
            }
          }
          catch (Exception ex)
          {
            result = new DataPortalResult(null, ex, null);
          }
          finally
          {
            tcs.SetResult(result);
          }
        };
      proxy.UpdateAsync(request);
      var finalresult = await tcs.Task;
      if (finalresult.Error != null)
        throw finalresult.Error;
      return finalresult;
#else
      try
      {
        var response = await proxy.UpdateAsync(request);
        response = ConvertResponse(response);
        if (response == null)
          throw new DataPortalException("null response", null);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          var newobj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(newobj, null, globalContext);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
#endif
#endif
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
#if !(ANDROID || IOS) && !NETFX_CORE
      ChannelFactory<IWcfPortal> cf = GetChannelFactory();
      var proxy = GetProxy(cf);
      WcfResponse response = null;
#if NET40
      try
      {
        var request = new DeleteRequest(objectType, criteria, context);
        if (isSync)
        {
          response = proxy.Delete(request);
        }
        else
        {
          var worker = new Csla.Threading.BackgroundWorker();
          var tcs = new TaskCompletionSource<WcfResponse>();
          worker.RunWorkerCompleted += (o, e) =>
            {
              tcs.SetResult((WcfResponse)e.Result);
            };
          worker.DoWork += (o, e) =>
            {
              e.Result = proxy.Delete(request);
            };
          worker.RunWorkerAsync();
          response = await tcs.Task;
        }
        if (cf != null)
          cf.Close();
        object result = response.Result;
        if (result is Exception)
          throw (Exception)result;
        return (DataPortalResult)result;
      }
      catch
      {
        cf.Abort();
        throw;
      }
#else
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
#endif
#else
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
#if !NETFX_CORE && !(IOS || ANDROID)
      var tcs = new TaskCompletionSource<DataPortalResult>();
      proxy.DeleteCompleted += (s, e) => 
        {
          try
          {
            Csla.WcfPortal.WcfResponse response = null;
            if (e.Error == null)
              response = ConvertResponse(e.Result);
            ContextDictionary globalContext = null;
            if (response != null)
              globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
            if (e.Error == null && response != null && response.ErrorData == null)
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
              result = new DataPortalResult(null, e.Error, globalContext);
            }
          }
          catch (Exception ex)
          {
            result = new DataPortalResult(null, ex, null);
          }
          finally
          {
            tcs.SetResult(result);
          }
        };
      proxy.DeleteAsync(request);
      var finalresult = await tcs.Task;
      if (finalresult.Error != null)
        throw finalresult.Error;
      return finalresult;
#else
      try
      {
        var response = await proxy.DeleteAsync(request);
        response = ConvertResponse(response);
        if (response == null)
          throw new DataPortalException("null response", null);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          result = new DataPortalResult(null, null, globalContext);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
#endif
#endif
    }

#if ANDROID || IOS || NETFX_CORE
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
#endif
  }
}
#endif
