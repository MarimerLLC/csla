using System;
using System.ServiceModel;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.DataPortalClient
{
  public static class WcfProxy
  {
    private static System.ServiceModel.Channels.Binding _defaultBinding;
    private const int TimeoutInMinutes = 10;
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

    private static string _defaultUrl;
    public static string DefaultUrl
    {
      get { return _defaultUrl; }
      set { _defaultUrl = value; }
    }
  }

#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public class WcfProxy<T> : IDataPortalProxy<T> where T : IMobileObject
  {
    public WcfProxy()
    {
      this.DataPortalUrl = WcfProxy.DefaultUrl;
      this.Binding = WcfProxy.DefaultBinding;
    }

    public System.ServiceModel.Channels.Binding Binding { get; protected set; }
    public string DataPortalUrl { get; protected set; }

    private WcfPortal.WcfPortalClient GetProxy()
    {
      if (!string.IsNullOrEmpty(this.DataPortalUrl) && this.Binding != null)
      {
        var address = new EndpointAddress(this.DataPortalUrl);
        return new WcfPortal.WcfPortalClient(this.Binding, address);
      }
      else
        return new WcfPortal.WcfPortalClient();
    }

    #region GlobalContext

    private Csla.Core.ContextDictionary _globalContext;

    public Csla.Core.ContextDictionary GlobalContext
    {
      get { return _globalContext; }
    }

    #endregion

    #region Cirteria
    private WcfPortal.CriteriaRequest GetBaseCriteriaRequest()
    {
      var request = new WcfPortal.CriteriaRequest();
      request.CriteriaData = null;
      request.ClientContext = MobileFormatter.Serialize(ApplicationContext.ClientContext);
      request.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      request.Principal = MobileFormatter.Serialize(ApplicationContext.User);
      return request;
    }
    private WcfPortal.UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var request = new WcfPortal.UpdateRequest();
      request.ObjectData = null;
      request.ClientContext = MobileFormatter.Serialize(ApplicationContext.ClientContext);
      request.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      request.Principal = MobileFormatter.Serialize(ApplicationContext.User);
      return request;
    }
    #endregion

    #region Create

    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      if (CreateCompleted != null)
        CreateCompleted(this, e);
    }

    public void BeginCreate()
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;

      var proxy = GetProxy();
      proxy.CreateCompleted += new EventHandler<Csla.WcfPortal.CreateCompletedEventArgs>(proxy_CreateCompleted);
      proxy.CreateAsync(request);
    }

    public void BeginCreate(object criteria)
    {
      BeginCreate(criteria, null);
    }
    public void BeginCreate(object criteria, object userState)
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
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
          _globalContext =  (ContextDictionary)MobileFormatter.Deserialize(e.Result.GlobalContext);
          OnCreateCompleted(new DataPortalResult<T>(obj, null, e.UserState));
        }
        else if (e.Result.ErrorData != null)
        {
          var ex = new DataPortalException(e.Result.ErrorData);
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

    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      if (FetchCompleted != null)
        FetchCompleted(this, e);
    }

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

    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }
    public void BeginFetch(object criteria, object userState)
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
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

    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      if (UpdateCompleted != null)
        UpdateCompleted(this, e);
    }

    public void BeginUpdate(object criteria)
    {
      BeginUpdate(criteria, null);
    }

    public void BeginUpdate(object criteria, object userState)
    {
      var request = GetBaseUpdateCriteriaRequest();
      request.ObjectData = MobileFormatter.Serialize(criteria);
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

    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      if (DeleteCompleted != null)
        DeleteCompleted(this, e);
    }
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }
    public void BeginDelete(object criteria, object userState)
    {
      var request = GetBaseCriteriaRequest();
      request.TypeName = typeof(T).AssemblyQualifiedName;
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
          _globalContext = (ContextDictionary)MobileFormatter.Deserialize(e.Result.GlobalContext);
          OnDeleteCompleted(new DataPortalResult<T>(default(T), null, e.UserState));
        }
        else if (e.Result.ErrorData != null)
        {
          var ex = new DataPortalException(e.Result.ErrorData);
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

    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      if (ExecuteCompleted != null)
        ExecuteCompleted(this, e);
    }

    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

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

    protected virtual WcfPortal.UpdateRequest ConvertRequest(WcfPortal.UpdateRequest request)
    {
      return request;
    }

    protected virtual WcfPortal.CriteriaRequest ConvertRequest(WcfPortal.CriteriaRequest request)
    {
      return request;
    }

    protected virtual WcfPortal.WcfResponse ConvertResponse(WcfPortal.WcfResponse response)
    {
      return response;
    }

    #endregion
  }
}
