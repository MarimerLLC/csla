//-----------------------------------------------------------------------
// <copyright file="DataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client side data portal used for making asynchronous</summary>
//-----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// Client side data portal used for making asynchronous
  /// data portal calls in .NET.
  /// </summary>
  /// <typeparam name="T">
  /// Type of business object.
  /// </typeparam>
  public class DataPortal<T> : IDataPortal<T>
  {
    internal static Csla.Server.EmptyCriteria EmptyCriteria = new Server.EmptyCriteria();

    /// <summary>
    /// Gets a reference to the global context returned from
    /// the background thread and/or server.
    /// </summary>
    public Csla.Core.ContextDictionary GlobalContext { get; set; }

    private class DataPortalAsyncRequest
    {
      public object Argument { get; set; }
      public System.Security.Principal.IPrincipal Principal { get; set; }
      public Csla.Core.ContextDictionary ClientContext { get; set; }
      public Csla.Core.ContextDictionary GlobalContext { get; set; }
      public object UserState { get; set; }
      // passes CurrentCulture and CurrentUICulture to the async thread
      public CultureInfo CurrentCulture;
      public CultureInfo CurrentUICulture;

      public DataPortalAsyncRequest(object argument, object userState)
      {
        this.Argument = argument;
        this.Principal = Csla.ApplicationContext.User;
        this.ClientContext = Csla.ApplicationContext.ClientContext;
        this.GlobalContext = Csla.ApplicationContext.GlobalContext;
        this.UserState = userState;
        this.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
        this.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;
      }
    }

    private class DataPortalAsyncResult
    {
      public T Result { get; set; }
      public Csla.Core.ContextDictionary GlobalContext { get; set; }
      public object UserState { get; set; }
      public Exception Error { get; set; }

      public DataPortalAsyncResult(T result, Csla.Core.ContextDictionary globalContext, Exception error, object userState)
      {
        this.Result = result;
        this.GlobalContext = globalContext;
        this.UserState = userState;
        this.Error = error;
      }
    }

    private void SetThreadContext(DataPortalAsyncRequest request)
    {
      Csla.ApplicationContext.User = request.Principal;
      Csla.ApplicationContext.SetContext(request.ClientContext, request.GlobalContext);
      Thread.CurrentThread.CurrentCulture = request.CurrentCulture;
      Thread.CurrentThread.CurrentUICulture = request.CurrentUICulture;
    }

    private async Task<object> DoCreateAsync(Type objectType, object criteria, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        DataPortal.OnDataPortalInitInvoke(null);

        if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, objectType))
          throw new Csla.Security.SecurityException(string.Format(
            Resources.UserNotAuthorizedException,
            "create",
            objectType.Name));

        var method = Server.DataPortalMethodCache.GetCreateMethod(objectType, criteria);

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(objectType, method.RunLocal);

        dpContext =
          new Csla.Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        DataPortal.OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Create));

        try
        {
          result = await proxy.Create(objectType, criteria, dpContext, isSync);
          GlobalContext = result.GlobalContext;
          if (isSync && proxy.IsServerRemote)
            ApplicationContext.ContextManager.SetGlobalContext(GlobalContext);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            var dpe = ex.InnerExceptions[0] as Server.DataPortalException;
            if (dpe != null)
              HandleCreateDataPortalException(dpe, isSync, proxy);
          }
          throw new DataPortalException(
            string.Format("DataPortal.Create {0}", Resources.Failed),
            ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleCreateDataPortalException(ex, isSync, proxy);
        }
        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Create));
      }
      catch (Exception ex)
      {
        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Create, ex));
        throw;
      }
      return result.ReturnObject;
    }

    private void HandleCreateDataPortalException(Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      HandleDataPortalException("Create", ex, isSync, proxy);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <returns>A new object, populated with default values.</returns>
    public T Create()
    {
      return Create(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public T Create(object criteria)
    {
      return (T)Create(typeof(T), criteria);
    }

    internal static object Create(Type objectType, object criteria)
    {
      var dp = new DataPortal<object>();
      try
      {
        return dp.DoCreateAsync(objectType, criteria, true).Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    public async Task<T> CreateAsync()
    {
      return await CreateAsync(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> CreateAsync(object criteria)
    {
      return (T)await DoCreateAsync(typeof(T), criteria, false);
    }

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> CreateCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnCreateCompleted(DataPortalResult<T> e)
    {
      CreateCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    public void BeginCreate()
    {
      BeginCreate(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void BeginCreate(object criteria)
    {
      BeginCreate(criteria, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to create a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    public async void BeginCreate(object criteria, object userState)
    {
      try
      {
        var obj = await CreateAsync(criteria);
        OnCreateCompleted(new DataPortalResult<T>(obj, null, userState));
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          OnCreateCompleted(new DataPortalResult<T>(default(T), ex.Flatten().InnerExceptions[0], userState));
        else
          OnCreateCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
      catch (Exception ex)
      {
        OnCreateCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
    }

    private async Task<object> DoFetchAsync(Type objectType, object criteria, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        DataPortal.OnDataPortalInitInvoke(null);

        if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.GetObject, objectType))
          throw new Csla.Security.SecurityException(string.Format(
            Resources.UserNotAuthorizedException,
            "get",
            objectType.Name));

        var method = Server.DataPortalMethodCache.GetFetchMethod(objectType, criteria);

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(objectType, method.RunLocal);

        dpContext =
          new Csla.Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        DataPortal.OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Fetch));

        try
        {
          result = await proxy.Fetch(objectType, criteria, dpContext, isSync);
          GlobalContext = result.GlobalContext;
          if (isSync && proxy.IsServerRemote)
            ApplicationContext.ContextManager.SetGlobalContext(GlobalContext);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            var dpe = ex.InnerExceptions[0] as Server.DataPortalException;
            if (dpe != null)
              HandleFetchDataPortalException(dpe, isSync, proxy);
          }
          throw new DataPortalException(
            string.Format("DataPortal.Fetch {0}", Resources.Failed),
            ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleFetchDataPortalException(ex, isSync, proxy);
        }
        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Fetch));
      }
      catch (Exception ex)
      {
        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Fetch, ex));
        throw;
      }
      return result.ReturnObject;
    }

    private void HandleFetchDataPortalException(Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      HandleDataPortalException("Fetch", ex, isSync, proxy);
    }

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> FetchCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnFetchCompleted(DataPortalResult<T> e)
    {
      FetchCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class to Fetch 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <returns>A new object, populated with default values.</returns>
    public T Fetch()
    {
      return Fetch(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to Fetch 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public T Fetch(object criteria)
    {
      return (T)Fetch(typeof(T), criteria);
    }

    internal static object Fetch(Type objectType, object criteria)
    {
      var dp = new DataPortal<object>();
      try
      {
        return dp.DoFetchAsync(objectType, criteria, true).Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to Fetch a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    public async Task<T> FetchAsync()
    {
      return await FetchAsync(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to Fetch a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> FetchAsync(object criteria)
    {
      return (T)await DoFetchAsync(typeof(T), criteria, false);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to Fetch a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    public void BeginFetch()
    {
      BeginFetch(EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to Fetch a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void BeginFetch(object criteria)
    {
      BeginFetch(criteria, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to Fetch a new object, which is loaded 
    /// with default values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    public async void BeginFetch(object criteria, object userState)
    {
      try
      {
        var obj = await FetchAsync(criteria);
        OnFetchCompleted(new DataPortalResult<T>(obj, null, userState));
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          OnFetchCompleted(new DataPortalResult<T>(default(T), ex.Flatten().InnerExceptions[0], userState));
        else
          OnFetchCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
      catch (Exception ex)
      {
        OnFetchCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
    }

    internal async Task<T> DoUpdateAsync(T obj, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      DataPortalOperations operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      try
      {
        DataPortal.OnDataPortalInitInvoke(null);
        Csla.Server.DataPortalMethodInfo method = null;
        var factoryInfo = Csla.Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          var factoryType = Csla.Server.FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);

          if (obj is Core.ICommandObject)
          {
            if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, obj))
              throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "execute",
                objectType.Name));
            if (factoryType != null)
              method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.ExecuteMethodName, new object[] { obj });
          }
          else
          {
            var bbase = obj as Core.BusinessBase;
            if (bbase != null)
            {
              if (bbase.IsDeleted)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "delete",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName,
                                                                      new object[] { obj });
              }
              // must check the same authorization rules as for DataPortal_XYZ methods 
              else if (bbase.IsNew)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "create",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName,
                                                                    new object[] { obj });
              }
              else
              {
                if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "save",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName,
                                                                    new object[] { obj });
              }
            }
            else
            {
              if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, obj))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                          "save",
                                                                          objectType.Name));

              if (factoryType != null)
                method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName,
                                                                      new object[] { obj });
            }
          }
          if (method == null)
            method = new Csla.Server.DataPortalMethodInfo();
        }
        else
        {
          string methodName;
          if (obj is Core.ICommandObject)
          {
            methodName = "DataPortal_Execute";
            operation = DataPortalOperations.Execute;
            if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, obj))
              throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "execute",
                objectType.Name));
          }
          else
          {
            var bbase = obj as Core.BusinessBase;
            if (bbase != null)
            {
              if (bbase.IsDeleted)
              {
                methodName = "DataPortal_DeleteSelf";
                if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                    "delete",
                    objectType.Name));
              }
              else
                if (bbase.IsNew)
                {
                  methodName = "DataPortal_Insert";
                  if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, obj))
                    throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                      "create",
                      objectType.Name));
                }
                else
                {
                  methodName = "DataPortal_Update";
                  if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, obj))
                    throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                      "save",
                      objectType.Name));
                }
            }
            else
            {
              methodName = "DataPortal_Update";
              if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, obj))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                  "save",
                  objectType.Name));
            }
          }
          method = Server.DataPortalMethodCache.GetMethodInfo(obj.GetType(), methodName);
        }

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(objectType, method.RunLocal);

        dpContext =
          new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        DataPortal.OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, obj, operation));

        try
        {
          if (!proxy.IsServerRemote && ApplicationContext.AutoCloneOnUpdate)
          {
            // when using local data portal, automatically
            // clone original object before saving
            ICloneable cloneable = obj as ICloneable;
            if (cloneable != null)
              obj = (T)cloneable.Clone();
          }
          result = await proxy.Update(obj, dpContext, isSync);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            var dpe = ex.InnerExceptions[0] as Server.DataPortalException;
            if (dpe != null)
              HandleUpdateDataPortalException(dpe, isSync, proxy);
          }
          throw new DataPortalException(
            string.Format("DataPortal.Update {0}", Resources.Failed),
            ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleUpdateDataPortalException(ex, isSync, proxy);
        }

        GlobalContext = result.GlobalContext;
        if (proxy.IsServerRemote && isSync)
          ApplicationContext.ContextManager.SetGlobalContext(GlobalContext);

        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, obj, operation));
      }
      catch (Exception ex)
      {
        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, obj, operation, ex));
        throw;
      }
      return (T)result.ReturnObject;
    }

    private void HandleUpdateDataPortalException(Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      HandleDataPortalException("Update", ex, isSync, proxy);
    }

    private void HandleDataPortalException(string operation, Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      var result = ex.Result;
      GlobalContext = result.GlobalContext;
      if (proxy.IsServerRemote && isSync)
        ApplicationContext.ContextManager.SetGlobalContext(GlobalContext);
      var original = ex.InnerException;
      if (original.InnerException != null)
        original = original.InnerException;
      throw new DataPortalException(
        String.Format("DataPortal.{2} {0} ({1})", Resources.Failed, original.Message, operation),
        ex.InnerException, result.ReturnObject);
    }

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> UpdateCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnUpdateCompleted(DataPortalResult<T> e)
    {
      UpdateCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    public T Update(T obj)
    {
      try
      {
        return DoUpdateAsync(obj, true).Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    public void BeginUpdate(T obj)
    {
      BeginUpdate(obj, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    /// <param name="userState">User state data.</param>
    public async void BeginUpdate(T obj, object userState)
    {
      try
      {
        var result = await UpdateAsync(obj);
        OnUpdateCompleted(new DataPortalResult<T>(result, null, userState));
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          OnUpdateCompleted(new DataPortalResult<T>(default(T), ex.Flatten().InnerExceptions[0], userState));
        else
          OnUpdateCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
      catch (Exception ex)
      {
        OnUpdateCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to update an object.
    /// </summary>
    /// <param name="obj">Object to update.</param>
    public async Task<T> UpdateAsync(T obj)
    {
      return await DoUpdateAsync(obj, false);
    }

    internal async Task DoDeleteAsync(Type objectType, object criteria, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        DataPortal.OnDataPortalInitInvoke(null);

        if (!Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, objectType))
          throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
            "delete",
            objectType.Name));

        var method = Server.DataPortalMethodCache.GetDeleteMethod(objectType, criteria);

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(objectType, method.RunLocal);

        dpContext = new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        DataPortal.OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Delete));

        try
        {
          result = await proxy.Delete(objectType, criteria, dpContext, isSync);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            var dpe = ex.InnerExceptions[0] as Server.DataPortalException;
            if (dpe != null)
              HandleDeleteDataPortalException(dpe, isSync, proxy);
          }
          throw new DataPortalException(
            string.Format("DataPortal.Delete {0}", Resources.Failed),
            ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleDeleteDataPortalException(ex, isSync, proxy);
        }

        GlobalContext = result.GlobalContext;
        if (proxy.IsServerRemote && isSync)
          ApplicationContext.ContextManager.SetGlobalContext(result.GlobalContext);

        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Delete));
      }
      catch (Exception ex)
      {
        DataPortal.OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, criteria, DataPortalOperations.Delete, ex));
        throw;
      }
    }

    private void HandleDeleteDataPortalException(Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      HandleDataPortalException("Delete", ex, isSync, proxy);
    }

    /// <summary>
    /// Event raised when the operation has completed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    public event EventHandler<DataPortalResult<T>> DeleteCompleted;

    /// <summary>
    /// Raises the event.
    /// </summary>
    /// <param name="e">
    /// The parameter provided to the event handler.
    /// </param>
    /// <remarks>
    /// <para>
    /// If your application is running in WPF, this event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// If your application is running in Windows Forms,
    /// this event will be raised on a background thread.
    /// If you also set DataPortal.SynchronizationObject
    /// to a Windows Forms form or control, then the event
    /// will be raised on the UI thread automatically.
    /// </para><para>
    /// In any other environment (such as ASP.NET), this
    /// event will be raised on a background thread.
    /// </para>
    /// </remarks>
    protected virtual void OnDeleteCompleted(DataPortalResult<T> e)
    {
      DeleteCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void Delete(object criteria)
    {
      Delete(typeof(T), criteria);
    }

    internal static void Delete(Type objectType, object criteria)
    {
      var dp = new DataPortal<object>();
      try
      {
        var task = dp.DoDeleteAsync(objectType, criteria, true);
        if (!task.IsCompleted)
          task.RunSynchronously();
        if (task.Exception != null)
          throw task.Exception;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          throw ex.InnerExceptions[0];
        else
          throw;
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void BeginDelete(object criteria)
    {
      BeginDelete(criteria, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="userState">User state data.</param>
    public async void BeginDelete(object criteria, object userState)
    {
      try
      {
        await DoDeleteAsync(typeof(T), criteria, true);
        OnDeleteCompleted(new DataPortalResult<T>(default(T), null, userState));
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          OnDeleteCompleted(new DataPortalResult<T>(default(T), ex.Flatten().InnerExceptions[0], userState));
        else
          OnDeleteCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
      catch (Exception ex)
      {
        OnDeleteCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task DeleteAsync(object criteria)
    {
      await DoDeleteAsync(typeof(T), criteria, false);
    }

    /// <summary>
    /// Event indicating an execute operation is complete.
    /// </summary>
    public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

    /// <summary>
    /// Raises the ExecuteCompleted event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnExecuteCompleted(DataPortalResult<T> e)
    {
      ExecuteCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    public T Execute(T command)
    {
      return Update(command);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    public void BeginExecute(T command)
    {
      BeginExecute(command, null);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    /// <param name="userState">User state data.</param>
    public async void BeginExecute(T command, object userState)
    {
      try
      {
        var result = await UpdateAsync(command);
        OnExecuteCompleted(new DataPortalResult<T>(result, null, userState));
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
          OnExecuteCompleted(new DataPortalResult<T>(default(T), ex.Flatten().InnerExceptions[0], userState));
        else
          OnExecuteCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
      catch (Exception ex)
      {
        OnExecuteCompleted(new DataPortalResult<T>(default(T), ex, userState));
      }
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to execute a command object.
    /// </summary>
    /// <param name="command">Command object to execute.</param>
    public async Task<T> ExecuteAsync(T command)
    {
      return await DoUpdateAsync(command, false);
    }

    private static DataPortalClient.IDataPortalProxy GetDataPortalProxy(Type objectType, bool forceLocal)
    {
      if (forceLocal || ApplicationContext.IsOffline)
      {
        return new DataPortalClient.LocalProxy();
      }
      else
      {
        // load dataportal factory if loaded 
        if (DataPortal.ProxyFactory == null)
          DataPortal.LoadDataPortalProxyFactory();

        return DataPortal.ProxyFactory.Create(objectType);
      }
    }

    private static System.Security.Principal.IPrincipal GetPrincipal()
    {
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        // Windows integrated security
        return null;
      }
      else
      {
        // we assume using the CSLA framework security
        return ApplicationContext.User;
      }
    }
  }
}