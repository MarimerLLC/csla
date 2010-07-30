//-----------------------------------------------------------------------
// <copyright file="WcfProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Data portal client-side proxy used to communicate</summary>
//-----------------------------------------------------------------------
using System;
using System.ServiceModel;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;
using Csla.Silverlight;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Data portal client-side proxy used to communicate
  /// with a CSLA .NET for Windows server-side
  /// data portal.
  /// </summary>
  public static class WcfProxy
  {
    private static System.ServiceModel.Channels.Binding _defaultBinding;
    private const int TimeoutInMinutes = 10;
    private static string _defaultUrl;
    private static string _defaultEndPoint;

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
          _defaultBinding  = new BasicHttpBinding();
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
    public static string DefaultUrl
    {
      get { return _defaultUrl; }
      set { _defaultUrl = value; }
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
  }

  /// <summary>
  /// Data portal client-side proxy used to communicate
  /// with a CSLA .NET for Windows server-side
  /// data portal.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public class WcfProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
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

    #region GlobalContext

    private Csla.Core.ContextDictionary _globalContext;

    /// <summary>
    /// Gets the GlobalContext dictionary returned from the
    /// server upon completion of the data portal call.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return _globalContext; }
    }

    #endregion

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

    #region Create

    /// <summary>
    /// Event raised when the asynchronous BeginCreate() call
    /// is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Raises the CreateCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, e);
    }

    /// <summary>
    /// Begins an asynchronous server call to create an
    /// instance of a business object.
    /// </summary>
    public void BeginCreate()
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
      request = ConvertRequest(request);

      var proxy = GetProxy();
      proxy.CreateCompleted += new EventHandler<Csla.WcfPortal.CreateCompletedEventArgs>(proxy_CreateCompleted);
      proxy.CreateAsync(request);
    }

    /// <summary>
    /// Begins an asynchronous server call to create an
    /// instance of a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginCreate(object criteria)
    {
      BeginCreate(criteria, null);
    }

    /// <summary>
    /// Begins an asynchronous server call to create an
    /// instance of a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginCreate(object criteria, object userState)
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
      if (!(criteria is IMobileObject))
      {
        criteria = new PrimitiveCriteria(criteria);
      }

      request.CriteriaData = MobileFormatter.Serialize(criteria);
      request = ConvertRequest(request);

      var proxy = GetProxy();
      proxy.CreateCompleted += new EventHandler<Csla.WcfPortal.CreateCompletedEventArgs>(proxy_CreateCompleted);
      if (userState !=null)
        proxy.CreateAsync(request, userState);
      else
        proxy.CreateAsync(request);
    }

    private void proxy_CreateCompleted(object sender, Csla.WcfPortal.CreateCompletedEventArgs e)
    {
      var response = ConvertResponse(e.Result);
      try
      {
        if (e.Error == null && response.ErrorData == null)
        {
          var buffer = new System.IO.MemoryStream(response.ObjectData);
          var formatter = new MobileFormatter();
          T obj = (T)formatter.Deserialize(buffer);
          _globalContext =  (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
          OnCreateCompleted(new DataPortalResult<T>(obj, null, e.UserState));
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          OnCreateCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else
        {
          OnCreateCompleted(new DataPortalResult<T>(default(T), e.Error, e.UserState));
        }
      }
      catch (Exception ex)
      {
        OnCreateCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
      }
    }

    #endregion

    #region Fetch

    /// <summary>
    /// Event raised when the asynchronous BeginFetch() call
    /// is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Raises the FetchCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, e);
    }

    /// <summary>
    /// Begins an asynchronous server call to retrieve an
    /// instance of a business object.
    /// </summary>
    public void BeginFetch()
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
      request.CriteriaData = null;
      request = ConvertRequest(request);
      var proxy = GetProxy();
      proxy.FetchCompleted += new EventHandler<Csla.WcfPortal.FetchCompletedEventArgs>(proxy_FetchCompleted);
      proxy.FetchAsync(request);
    }

    /// <summary>
    /// Begins an asynchronous server call to retrieve an
    /// instance of a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    /// <summary>
    /// Begins an asynchronous server call to retrieve an
    /// instance of a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginFetch(object criteria, object userState)
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
      if (!(criteria is IMobileObject))
      {
        criteria = new PrimitiveCriteria(criteria);
      }
      request.CriteriaData = MobileFormatter.Serialize(criteria);
      request = ConvertRequest(request);
      var proxy = GetProxy();
      proxy.FetchCompleted += new EventHandler<Csla.WcfPortal.FetchCompletedEventArgs>(proxy_FetchCompleted);
      if (userState != null)
        proxy.FetchAsync(request, userState);
      else
        proxy.FetchAsync(request);
    }

    private void proxy_FetchCompleted(object sender, Csla.WcfPortal.FetchCompletedEventArgs e)
    {
      try
      {
        var response = ConvertResponse(e.Result);
        if (e.Error == null && response.ErrorData == null)
        {
          var buffer = new System.IO.MemoryStream(response.ObjectData);
          var formatter = new MobileFormatter();
          T obj = (T)formatter.Deserialize(buffer);
          _globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
          OnFetchCompleted(new DataPortalResult<T>(obj, null, e.UserState));
        }
        else if (e.Error != null)
        {
          var ex = new DataPortalException(e.Error.ToErrorInfo());
          OnFetchCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else // if (e.Result.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          OnFetchCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
      }
      catch (Exception ex)
      {
        OnFetchCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
      }
    }

    #endregion

    #region Update

    /// <summary>
    /// Event raised when the asynchronous BeginUpdate() call
    /// is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Raises the UpdateCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, e);
    }

    /// <summary>
    /// Begins an asynchronous server call to update an
    /// instance of a business object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    public void BeginUpdate(object obj)
    {
      BeginUpdate(obj, null);
    }

    /// <summary>
    /// Begins an asynchronous server call to update an
    /// instance of a business object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    /// <param name="userState">User state object.</param>
    public void BeginUpdate(object obj, object userState)
    {
      var request = GetBaseUpdateCriteriaRequest();
      request.ObjectData = MobileFormatter.Serialize(obj);
      request = ConvertRequest(request);
      var proxy = GetProxy();
      proxy.UpdateCompleted += new EventHandler<Csla.WcfPortal.UpdateCompletedEventArgs>(proxy_UpdateCompleted);
      if (userState !=null)
        proxy.UpdateAsync(request, userState);
      else
        proxy.UpdateAsync(request);
    }

    private void proxy_UpdateCompleted(object sender, Csla.WcfPortal.UpdateCompletedEventArgs e)
    {
      try
      {
        var response = ConvertResponse(e.Result);
        if (e.Error == null && response.ErrorData == null)
        {
          var buffer = new System.IO.MemoryStream(response.ObjectData);
          var formatter = new MobileFormatter();
          T obj = (T)formatter.Deserialize(buffer);
          _globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
          OnUpdateCompleted(new DataPortalResult<T>(obj, null, e.UserState));
        }
        else if (e.Error != null)
        {
          var ex = new DataPortalException(e.Error.ToErrorInfo());
          OnUpdateCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          OnUpdateCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else
        {
          OnUpdateCompleted(new DataPortalResult<T>(default(T), e.Error, e.UserState));
        }
      }
      catch (Exception ex)
      {
        OnUpdateCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
      }
    }

    #endregion

    #region Delete

    /// <summary>
    /// Event raised when the asynchronous BeginDelete() call
    /// is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    /// <summary>
    /// Raises the DeleteCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, e);
    }

    /// <summary>
    /// Begins an asynchronous server call to delete an
    /// instance of a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    /// <summary>
    /// Begins an asynchronous server call to delete an
    /// instance of a business object.
    /// </summary>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="userState">User state object.</param>
    public void BeginDelete(object criteria, object userState)
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
      if (!(criteria is IMobileObject))
      {
        criteria = new PrimitiveCriteria(criteria);
      }
      request.CriteriaData = MobileFormatter.Serialize(criteria);
      request = ConvertRequest(request);
      var proxy = GetProxy();
      proxy.DeleteCompleted += new EventHandler<Csla.WcfPortal.DeleteCompletedEventArgs>(proxy_DeleteCompleted);
      if (userState != null)
        proxy.DeleteAsync(request, userState);
      else
        proxy.DeleteAsync(request);
    }

    private void proxy_DeleteCompleted(object sender, Csla.WcfPortal.DeleteCompletedEventArgs e)
    {
      var response = ConvertResponse(e.Result);
      try
      {
        if (e.Error == null && response.ErrorData == null)
        {
          _globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
          OnDeleteCompleted(new DataPortalResult<T>(default(T), null, e.UserState));
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          OnDeleteCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else
        {
          OnDeleteCompleted(new DataPortalResult<T>(default(T), e.Error, e.UserState));
        }
      }
      catch (Exception ex)
      {
        OnUpdateCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
      }
    }

    #endregion

    #region Execute

    /// <summary>
    /// Event raised when the asynchronous BeginExecute() call
    /// is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Raises the ExecuteCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    /// <summary>
    /// Begins an asynchronous server call to execute a
    /// command object.
    /// </summary>
    /// <param name="command">Object to execute.</param>
    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    /// <summary>
    /// Begins an asynchronous server call to execute a
    /// command object.
    /// </summary>
    /// <param name="command">Object to execute.</param>
    /// <param name="userState">User state object.</param>
    public void BeginExecute(T command, object userState)
    {
      var request = GetBaseUpdateCriteriaRequest();
      request.ObjectData = MobileFormatter.Serialize(command);
      request = ConvertRequest(request);
      var proxy = GetProxy();
      proxy.UpdateCompleted += new EventHandler<Csla.WcfPortal.UpdateCompletedEventArgs>(proxy_ExecuteCompleted);
      if (userState != null)
        proxy.UpdateAsync(request, userState);
      else
        proxy.UpdateAsync(request);
    }

    private void proxy_ExecuteCompleted(object sender, Csla.WcfPortal.UpdateCompletedEventArgs e)
    {
      try
      {
        var response = ConvertResponse(e.Result);
        if (e.Error == null && response.ErrorData == null)
        {
          var buffer = new System.IO.MemoryStream(response.ObjectData);
          var formatter = new MobileFormatter();
          T obj = (T)formatter.Deserialize(buffer);
          _globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
          OnExecuteCompleted(new DataPortalResult<T>(obj, null, e.UserState));
        }
        else if (e.Error != null)
        {
          var ex = new DataPortalException(e.Error.ToErrorInfo());
          OnExecuteCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          OnExecuteCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
        }
        else
        {
          OnExecuteCompleted(new DataPortalResult<T>(default(T), e.Error, e.UserState));
        }
      }
      catch (Exception ex)
      {
        OnExecuteCompleted(new DataPortalResult<T>(default(T), ex, e.UserState));
      }
    }

    #endregion

    #region Extention Method for Requests

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
