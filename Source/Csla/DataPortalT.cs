//-----------------------------------------------------------------------
// <copyright file="DataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client side data portal used for making asynchronous</summary>
//-----------------------------------------------------------------------

using System.Globalization;
using Csla.Configuration;
using Csla.DataPortalClient;
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
  public class DataPortal<T> : IDataPortal<T>, IChildDataPortal<T>, IDataPortal, IChildDataPortal
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="proxy"></param>
    /// <param name="dataPortalCache">Data portal cache service</param>
    /// <param name="dataPortalOptions"></param>
    public DataPortal(ApplicationContext applicationContext, IDataPortalProxy proxy, IDataPortalCache dataPortalCache, DataPortalOptions dataPortalOptions)
    {
      ApplicationContext = applicationContext;
      DataPortalProxy = proxy;
      Cache = dataPortalCache;
      DataPortalClientOptions = dataPortalOptions.DataPortalClientOptions;
    }

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    private ApplicationContext ApplicationContext { get; set; }
    private DataPortalClient.IDataPortalProxy DataPortalProxy { get; set; }

    private IDataPortalCache Cache { get; set; }
    private DataPortalClientOptions DataPortalClientOptions { get; set; }

    private class DataPortalAsyncRequest
    {
      private ApplicationContext ApplicationContext { get; set; }

      public object Argument { get; set; }
      public System.Security.Principal.IPrincipal Principal { get; set; }
      public Csla.Core.ContextDictionary ClientContext { get; set; }
      public object UserState { get; set; }
      // passes CurrentCulture and CurrentUICulture to the async thread
      public CultureInfo CurrentCulture;
      public CultureInfo CurrentUICulture;

      public DataPortalAsyncRequest(ApplicationContext applicationContext, object argument, object userState)
      {
        ApplicationContext = applicationContext;
        this.Argument = argument;
        this.Principal = ApplicationContext.User;
        this.ClientContext = ApplicationContext.ClientContext;
        this.UserState = userState;
        this.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
        this.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;
      }
    }

    private class DataPortalAsyncResult
    {
      public T Result { get; set; }
      public object UserState { get; set; }
      public Exception Error { get; set; }

      public DataPortalAsyncResult(T result, Exception error, object userState)
      {
        this.Result = result;
        this.UserState = userState;
        this.Error = error;
      }
    }

    private Reflection.ServiceProviderMethodCaller serviceProviderMethodCaller;
    private Reflection.ServiceProviderMethodCaller ServiceProviderMethodCaller
    {
      get
      {
        if (serviceProviderMethodCaller == null)
          serviceProviderMethodCaller = (Reflection.ServiceProviderMethodCaller)ApplicationContext.CreateInstanceDI(typeof(Reflection.ServiceProviderMethodCaller));
        return serviceProviderMethodCaller;
      }
    }

    private async Task<object> DoCreateAsync(Type objectType, object criteria, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.CreateObject, objectType, Server.DataPortal.GetCriteriaArray(criteria)))
          throw new Csla.Security.SecurityException(string.Format(
            Resources.UserNotAuthorizedException,
            "create",
            objectType.Name));
        Reflection.ServiceProviderMethodInfo method;
        if (criteria is Server.EmptyCriteria)
          method = ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(objectType, null, false);
        else
          method = ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria), false);
        var proxy = GetDataPortalProxy(method);

        dpContext =
          new Csla.Server.DataPortalContext(ApplicationContext, proxy.IsServerRemote);

        try
        {
          result = await Cache.GetDataPortalResultAsync(objectType, criteria, DataPortalOperations.Create, 
            async () => await proxy.Create(objectType, criteria, dpContext, isSync));
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
              HandleCreateDataPortalException(dpe);
          }
          throw new DataPortalException($"DataPortal.Create {Resources.Failed}", ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleCreateDataPortalException(ex);
        }
      }
      catch
      {
        throw;
      }
      return result.ReturnObject;
    }

    private void HandleCreateDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Create", ex);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public T Create(params object[] criteria)
    {
      return (T)Create(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria));
    }

    /// <summary>
    /// Manager method for synchronous Create operation; delegating to async version
    /// </summary>
    /// <param name="objectType">The type of object to instantiate and initialise</param>
    /// <param name="criteria">The criteria required to perform the creation operation</param>
    /// <returns>Returns a new, initialised object of the type requested</returns>

    private object Create(Type objectType, object criteria)
    {
      try
      {
        return DoCreateAsync(objectType, criteria, true).Result;
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
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> CreateAsync(params object[] criteria)
    {
      return (T)await DoCreateAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
    }

    private async Task<object> DoFetchAsync(Type objectType, object criteria, bool isSync)
    {
      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await DoExecuteAsync(objectType, criteria, isSync);
      }

      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      Reflection.ServiceProviderMethodInfo method = null;
      try
      {
        if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.GetObject, objectType, Server.DataPortal.GetCriteriaArray(criteria)))
          throw new Csla.Security.SecurityException(string.Format(
            Resources.UserNotAuthorizedException,
            "get",
            objectType.Name));
        method = ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria), false);

        var proxy = GetDataPortalProxy(method);

        dpContext =
          new Csla.Server.DataPortalContext(ApplicationContext, proxy.IsServerRemote);

        try
        {
          result = await Cache.GetDataPortalResultAsync(objectType, criteria, DataPortalOperations.Fetch,
            async () => await proxy.Fetch(objectType, criteria, dpContext, isSync));
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
              HandleFetchDataPortalException(dpe);
          }
          throw new DataPortalException($"DataPortal.Fetch {Resources.Failed}", ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleFetchDataPortalException(ex);
        }
      }
      catch
      {
        throw;
      }
      return result.ReturnObject;
    }

    private async Task<object> DoExecuteAsync(Type objectType, object criteria, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      Reflection.ServiceProviderMethodInfo method = null;
      try
      {
        if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, objectType, Server.DataPortal.GetCriteriaArray(criteria)))
          throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
            "execute",
            objectType.Name));
        method = ServiceProviderMethodCaller.FindDataPortalMethod<ExecuteAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria), false);

        var proxy = GetDataPortalProxy(method);

        dpContext =
          new Csla.Server.DataPortalContext(ApplicationContext, proxy.IsServerRemote);

        try
        {
          result = await Cache.GetDataPortalResultAsync(objectType, criteria, DataPortalOperations.Execute,
            async () => await proxy.Fetch(objectType, criteria, dpContext, isSync));
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
              HandleDataPortalException("Execute", dpe);
          }
          throw new DataPortalException($"DataPortal.Execute {Resources.Failed}", ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleDataPortalException("Execute", ex);
        }
      }
      catch
      {
        throw;
      }
      return result.ReturnObject;
    }

    private void HandleFetchDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Fetch", ex);
    }

    /// <summary>
    /// Called by a factory method in a business class to Fetch 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public T Fetch(params object[] criteria)
    {
      return (T)Fetch(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria));
    }

    /// <summary>
    /// Manager method for synchronous fetch operation; delegating to async version
    /// </summary>
    /// <param name="objectType">The type of object to instantiate and load</param>
    /// <param name="criteria">The criteria required to perform the load operation</param>
    /// <returns>Returns a populated object of the type requested</returns>
    private object Fetch(Type objectType, object criteria)
    {
      try
      {
        return DoFetchAsync(objectType, criteria, true).Result;
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
    /// <param name="criteria">Object-specific criteria.</param>
    public async Task<T> FetchAsync(params object[] criteria)
    {
      return (T)await DoFetchAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
    }

    internal async Task<T> DoUpdateAsync(T obj, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      var operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      try
      {
        DataPortalClient.IDataPortalProxy proxy = null;
        var factoryInfo = Csla.Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          Csla.Server.DataPortalMethodInfo method = null;
          var factoryLoader = ApplicationContext.CurrentServiceProvider.GetService(typeof(Server.IObjectFactoryLoader)) as Server.IObjectFactoryLoader;
          var factoryType = factoryLoader?.GetFactoryType(factoryInfo.FactoryTypeName);

          if (obj is Core.ICommandObject)
          {
            operation = DataPortalOperations.Execute;
            if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
              throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "execute",
                objectType.Name));
            if (factoryType != null)
              method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.ExecuteMethodName, [obj]);
          }
          else
          {
            if (obj is Core.BusinessBase bbase)
            {
              if (bbase.IsDeleted)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.DeleteObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "delete",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, [obj]);
              }
              // must check the same authorization rules as for DataPortal_XYZ methods 
              else if (bbase.IsNew)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.CreateObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "create",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, [obj]);
              }
              else
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "save",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, [obj]);
              }
            }
            else
            {
              if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                          "save",
                                                                          objectType.Name));

              if (factoryType != null)
                method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, [obj]);
            }
          }
          if (method == null)
            method = new Csla.Server.DataPortalMethodInfo();
          proxy = GetDataPortalProxy(method.RunLocal);
        }
        else
        {
          Reflection.ServiceProviderMethodInfo method;
          var criteria = Server.DataPortal.GetCriteriaArray(Server.EmptyCriteria.Instance);
          if (obj is Core.ICommandObject)
          {
            operation = DataPortalOperations.Execute;
            if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
              throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "execute",
                objectType.Name));
            method = ServiceProviderMethodCaller.FindDataPortalMethod<ExecuteAttribute>(objectType, criteria, false);
          }
          else
          {
            if (obj is Core.BusinessBase bbase)
            {
              if (bbase.IsDeleted)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.DeleteObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                    "delete",
                    objectType.Name));
                method = ServiceProviderMethodCaller.FindDataPortalMethod<DeleteSelfAttribute>(objectType, criteria, false);
              }
              else if (bbase.IsNew)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.CreateObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                    "create",
                    objectType.Name));
                method = ServiceProviderMethodCaller.FindDataPortalMethod<InsertAttribute>(objectType, criteria, false);
              }
              else
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                    "save",
                    objectType.Name));
                method = ServiceProviderMethodCaller.FindDataPortalMethod<UpdateAttribute>(objectType, criteria, false);
              }
            }
            else
            {
              method = ServiceProviderMethodCaller.FindDataPortalMethod<UpdateAttribute>(objectType, criteria, false);
            }
          }
          proxy = GetDataPortalProxy(method);
        }

        dpContext =
          new Server.DataPortalContext(ApplicationContext, proxy.IsServerRemote);

        try
        {
          if (!proxy.IsServerRemote && DataPortalClientOptions.AutoCloneOnUpdate)
          {
            // when using local data portal, automatically
            // clone original object before saving
            if (obj is ICloneable cloneable)
              obj = (T)cloneable.Clone();
          }
          result = await Cache.GetDataPortalResultAsync(objectType, obj, operation,
            async () => await proxy.Update(obj, dpContext, isSync));
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
              HandleUpdateDataPortalException(dpe);
          }
          throw new DataPortalException($"DataPortal.Update {Resources.Failed}", ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleUpdateDataPortalException(ex);
        }
      }
      catch
      {
        throw;
      }
      return (T)result.ReturnObject;
    }

    private void HandleUpdateDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Update", ex);
    }

    private void HandleDataPortalException(string operation, Server.DataPortalException ex)
    {
      var result = ex.Result;
      var original = ex.InnerException;
      if (original.InnerException != null)
        original = original.InnerException;
      throw new DataPortalException(
        String.Format("DataPortal.{2} {0} ({1})", Resources.Failed, original.Message, operation),
        ex.InnerException, result.ReturnObject);
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
    public Task<T> UpdateAsync(T obj)
    {
      return DoUpdateAsync(obj, false);
    }

    internal async Task DoDeleteAsync(Type objectType, object criteria, bool isSync)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.DeleteObject, objectType, Server.DataPortal.GetCriteriaArray(criteria)))
          throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
            "delete",
            objectType.Name));

        var method = ServiceProviderMethodCaller.FindDataPortalMethod<DeleteAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria), false);
        var proxy = GetDataPortalProxy(method);

        dpContext = new Server.DataPortalContext(ApplicationContext, proxy.IsServerRemote);

        try
        {
          result = await Cache.GetDataPortalResultAsync(objectType, criteria, DataPortalOperations.Delete,
            async () => await proxy.Delete(objectType, criteria, dpContext, isSync));
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            var dpe = ex.InnerExceptions[0] as Server.DataPortalException;
            if (dpe != null)
              HandleDeleteDataPortalException(dpe);
          }
          throw new DataPortalException($"DataPortal.Delete {Resources.Failed}", ex, null);
        }
        catch (Server.DataPortalException ex)
        {
          HandleDeleteDataPortalException(ex);
        }
      }
      catch
      {
        throw;
      }
    }

    private void HandleDeleteDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Delete", ex);
    }

    /// <summary>
    /// Called by a factory method in a business class or
    /// by the UI to delete an object.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    public void Delete(params object[] criteria)
    {
      Delete(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria));
    }

    /// <summary>
    /// Manager method for synchronous Delete operation; delegating to async version
    /// </summary>
    /// <param name="objectType">The type of object to instantiate and load</param>
    /// <param name="criteria">The criteria required to perform the load operation</param>
    /// <returns>Returns a populated object of the type requested</returns>
    private void Delete(Type objectType, object criteria)
    {
      try
      {
        var task = DoDeleteAsync(objectType, criteria, true);
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
    public Task DeleteAsync(params object[] criteria)
    {
      return DoDeleteAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
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
    public Task<T> ExecuteAsync(T command)
    {
      return DoUpdateAsync(command, false);
    }

    /// <summary>
    /// Execute a command on the logical server.
    /// </summary>
    /// <param name="criteria">
    /// Criteria provided to the command object.
    /// </param>
    /// <returns>The resulting command object.</returns>
    public async Task<T> ExecuteAsync(params object[] criteria)
    {
      return (T)await DoFetchAsync(typeof(T), criteria, false);
    }

    private DataPortalClient.IDataPortalProxy GetDataPortalProxy(Reflection.ServiceProviderMethodInfo method)
    {
      if (method != null)
        return GetDataPortalProxy(method.MethodInfo.RunLocal());
      else
        return GetDataPortalProxy(false);
    }

    private DataPortalClient.IDataPortalProxy GetDataPortalProxy(bool forceLocal)
    {
      if (forceLocal || ApplicationContext.IsOffline)
        return ApplicationContext.CreateInstanceDI<Csla.Channels.Local.LocalProxy>();
      else
        return DataPortalProxy;
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    public T CreateChild()
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return (T)(portal.Create(typeof(T)));
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    /// <param name="parameters">
    /// Parameters passed to child create method.
    /// </param>
    public T CreateChild(params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return (T)(portal.Create(typeof(T), parameters));
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    public async Task<T> CreateChildAsync()
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return await portal.CreateAsync<T>();
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    /// <param name="parameters">
    /// Parameters passed to child create method.
    /// </param>
    public async Task<T> CreateChildAsync(params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return await portal.CreateAsync<T>(parameters);
    }

    /// <summary>
    /// Fetches an existing
    /// child business object.
    /// </summary>
    public T FetchChild()
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return (T)(portal.Fetch(typeof(T)));
    }

    /// <summary>
    /// Fetches an existing
    /// child business object.
    /// </summary>
    /// <param name="parameters">
    /// Parameters passed to child fetch method.
    /// </param>
    public T FetchChild(params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return (T)(portal.Fetch(typeof(T), parameters));
    }

    /// <summary>
    /// Fetches an existing
    /// child business object.
    /// </summary>
    public async Task<T> FetchChildAsync()
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return await portal.FetchAsync<T>();
    }

    /// <summary>
    /// Fetches an existing
    /// child business object.
    /// </summary>
    /// <param name="parameters">
    /// Parameters passed to child fetch method.
    /// </param>
    public async Task<T> FetchChildAsync(params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      return await portal.FetchAsync<T>(parameters);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    public void UpdateChild(T child)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      portal.Update(child);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    /// <param name="parameters">
    /// Parameters passed to child update method.
    /// </param>
    public void UpdateChild(object child, params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      portal.Update(child, parameters);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    /// <param name="parameters">
    /// Parameters passed to child update method.
    /// </param>
    public void UpdateChild(T child, params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      portal.Update(child, parameters);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    public async Task UpdateChildAsync(T child)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      await portal.UpdateAsync(child).ConfigureAwait(false);
    }

    /// <summary>
    /// Inserts, updates or deletes an existing
    /// child business object.
    /// </summary>
    /// <param name="child">
    /// Business object to update.
    /// </param>
    /// <param name="parameters">
    /// Parameters passed to child update method.
    /// </param>
    public async Task UpdateChildAsync(T child, params object[] parameters)
    {
      var portal = new Server.ChildDataPortal(ApplicationContext);
      await portal.UpdateAsync(child, parameters).ConfigureAwait(false);
    }

    async Task<object> IDataPortal.CreateAsync(params object[] criteria) => Task.FromResult(await CreateAsync(criteria));
    async Task<object> IDataPortal.FetchAsync(params object[] criteria) => Task.FromResult(await FetchAsync(criteria));
    async Task<object> IDataPortal.UpdateAsync(object obj) => Task.FromResult(await UpdateAsync((T)obj));
    async Task<object> IDataPortal.ExecuteAsync(object command) => Task.FromResult(await ExecuteAsync((T)command));
    async Task<object> IDataPortal.ExecuteAsync(params object[] criteria) => Task.FromResult(await ExecuteAsync(criteria));
    object IDataPortal.Create(params object[] criteria) => Create(criteria);
    object IDataPortal.Fetch(params object[] criteria) => Fetch(criteria);
    object IDataPortal.Execute(object obj) => Execute((T)obj);
    object IDataPortal.Update(object obj) => Update((T)obj);

    async Task<object> IChildDataPortal.CreateChildAsync(params object[] criteria) => Task.FromResult(await CreateChildAsync(criteria));
    async Task<object> IChildDataPortal.FetchChildAsync(params object[] criteria) => Task.FromResult(await FetchChildAsync(criteria));
    async Task IChildDataPortal.UpdateChildAsync(object obj, params object[] parameters) => await UpdateChildAsync((T)obj);
    object IChildDataPortal.CreateChild(params object[] criteria) => CreateChild(criteria);
    object IChildDataPortal.FetchChild(params object[] criteria) => FetchChild(criteria);
    void IChildDataPortal.UpdateChild(object obj, params object[] parameters) => UpdateChild(obj, parameters);
  }

  internal static class Extensions
  {
    internal static bool RunLocal(this System.Reflection.MethodInfo t)
    {
      return t.CustomAttributes.Count(a => a.AttributeType.Equals(typeof(RunLocalAttribute))) > 0;
    }
  }
}
