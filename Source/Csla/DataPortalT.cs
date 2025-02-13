//-----------------------------------------------------------------------
// <copyright file="DataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client side data portal used for making asynchronous</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Csla.Configuration;
using Csla.Core;
using Csla.DataPortalClient;
using Csla.Properties;
using Csla.Reflection;

namespace Csla
{
  /// <summary>
  /// Client side data portal used for making asynchronous
  /// data portal calls in .NET.
  /// </summary>
  /// <typeparam name="T">
  /// Type of business object.
  /// </typeparam>
  public class DataPortal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T> : IDataPortal<T>, IChildDataPortal<T>, IDataPortal, IChildDataPortal where T: notnull
  {
    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    private readonly ApplicationContext _applicationContext;
    private IDataPortalProxy DataPortalProxy { get; }
    private IDataPortalCache Cache { get; }
    private DataPortalClientOptions DataPortalClientOptions { get; }

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="proxy"></param>
    /// <param name="dataPortalCache">Data portal cache service</param>
    /// <param name="dataPortalOptions"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="proxy"/>, <paramref name="dataPortalCache"/> or <paramref name="dataPortalOptions"/> is <see langword="null"/>.</exception>
    public DataPortal(ApplicationContext applicationContext, IDataPortalProxy proxy, IDataPortalCache dataPortalCache, DataPortalOptions dataPortalOptions)
    {
      if (dataPortalOptions is null)
        throw new ArgumentNullException(nameof(dataPortalOptions));
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      DataPortalProxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
      Cache = dataPortalCache ?? throw new ArgumentNullException(nameof(dataPortalCache));
      DataPortalClientOptions = dataPortalOptions.DataPortalClientOptions;
    }

    private class DataPortalAsyncRequest
    {
      private ApplicationContext _applicationContext;

      public object Argument { get; set; }
      public System.Security.Principal.IPrincipal Principal { get; set; }
      public Core.IContextDictionary ClientContext { get; set; }
      public object UserState { get; set; }
      // passes CurrentCulture and CurrentUICulture to the async thread
      public CultureInfo CurrentCulture;
      public CultureInfo CurrentUICulture;

      public DataPortalAsyncRequest(ApplicationContext applicationContext, object argument, object userState)
      {
        _applicationContext = applicationContext;
        Argument = argument;
        Principal = _applicationContext.User;
        ClientContext = _applicationContext.ClientContext;
        UserState = userState;
        CurrentCulture = CultureInfo.CurrentCulture;
        CurrentUICulture = CultureInfo.CurrentUICulture;
      }
    }

    private class DataPortalAsyncResult
    {
      public T Result { get; set; }
      public object UserState { get; set; }
      public Exception Error { get; set; }

      public DataPortalAsyncResult(T result, Exception error, object userState)
      {
        Result = result;
        UserState = userState;
        Error = error;
      }
    }

    private Reflection.ServiceProviderMethodCaller? serviceProviderMethodCaller;
    private Reflection.ServiceProviderMethodCaller ServiceProviderMethodCaller
    {
      get
      {
        if (serviceProviderMethodCaller == null)
          serviceProviderMethodCaller = _applicationContext.CreateInstanceDI< ServiceProviderMethodCaller>();
        return serviceProviderMethodCaller;
      }
    }

    private async Task<object> DoCreateAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, bool isSync, CancellationToken ct = default)
    {

      if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.CreateObject, objectType, Server.DataPortal.GetCriteriaArray(criteria), ct))
        throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException, "create", objectType.Name));

      _ = ServiceProviderMethodCaller.TryGetProviderMethodInfoFor<CreateAttribute>(objectType, criteria, out var method);
      var proxy = GetDataPortalProxy(method);

      var dpContext = new Server.DataPortalContext(_applicationContext, proxy.IsServerRemote);

      Server.DataPortalResult result = default!;
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

      return result.ReturnObject!;
    }

    [DoesNotReturn]
    private void HandleCreateDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Create", ex);
    }

    /// <inheritdoc />
    public T Create(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return (T)Create(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria));
    }

    /// <summary>
    /// Manager method for synchronous Create operation; delegating to async version
    /// </summary>
    /// <param name="objectType">The type of object to instantiate and initialise</param>
    /// <param name="criteria">The criteria required to perform the creation operation</param>
    /// <returns>Returns a new, initialised object of the type requested</returns>

    private object Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria)
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

    /// <inheritdoc />
    public async Task<T> CreateAsync(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return (T)await DoCreateAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
    }

    private async Task<object> DoFetchAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, bool isSync, CancellationToken ct = default)
    {
      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await DoExecuteAsync(objectType, criteria, isSync);
      }


      if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.GetObject, objectType, Server.DataPortal.GetCriteriaArray(criteria), ct))
        throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException, "get", objectType.Name));

      _ = ServiceProviderMethodCaller.TryGetProviderMethodInfoFor<FetchAttribute>(objectType, criteria, out var method);

      var proxy = GetDataPortalProxy(method);
      var dpContext = new Server.DataPortalContext(_applicationContext, proxy.IsServerRemote);

      Server.DataPortalResult result = default!;
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
      
      return result.ReturnObject!;
    }

    private async Task<object> DoExecuteAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, bool isSync, CancellationToken ct = default)
    {

      if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.EditObject, objectType, Server.DataPortal.GetCriteriaArray(criteria), ct))
        throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
          "execute",
          objectType.Name));

      _ = ServiceProviderMethodCaller.TryGetProviderMethodInfoFor<ExecuteAttribute>(objectType, criteria, out var method);
      var proxy = GetDataPortalProxy(method);
      var dpContext = new Server.DataPortalContext(_applicationContext, proxy.IsServerRemote);

      Server.DataPortalResult result = default!;
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

      return result.ReturnObject!;
    }

    [DoesNotReturn]
    private void HandleFetchDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Fetch", ex);
    }

    /// <inheritdoc />
    public T Fetch(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return (T)Fetch(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria));
    }

    /// <summary>
    /// Manager method for synchronous fetch operation; delegating to async version
    /// </summary>
    /// <param name="objectType">The type of object to instantiate and load</param>
    /// <param name="criteria">The criteria required to perform the load operation</param>
    /// <returns>Returns a populated object of the type requested</returns>
    private object Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria)
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

    /// <inheritdoc />
    public async Task<T> FetchAsync(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return (T)await DoFetchAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
    }

    private async Task<T> DoUpdateAsync(T obj, bool isSync, CancellationToken ct = default)
    {
      var operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      
      IDataPortalProxy proxy;
      var factoryInfo = Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
      if (factoryInfo != null)
      {
        Server.DataPortalMethodInfo? method = null;
        var factoryLoader = _applicationContext.CurrentServiceProvider.GetService(typeof(Server.IObjectFactoryLoader)) as Server.IObjectFactoryLoader;
        var factoryType = factoryLoader?.GetFactoryType(factoryInfo.FactoryTypeName);

        if (obj is Core.ICommandObject)
        {
          operation = DataPortalOperations.Execute;
          if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.EditObject, obj, ct))
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
              if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.DeleteObject, obj, ct))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                          "delete",
                                                                          objectType.Name));
              if (factoryType != null)
                method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, [obj]);
            }
            // must check the same authorization rules as for DataPortal_XYZ methods 
            else if (bbase.IsNew)
            {
              if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.CreateObject, obj, ct))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                          "create",
                                                                          objectType.Name));
              if (factoryType != null)
                method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, [obj]);
            }
            else
            {
              if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.EditObject, obj, ct))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                          "save",
                                                                          objectType.Name));
              if (factoryType != null)
                method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, [obj]);
            }
          }
          else
          {
            if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.EditObject, obj, ct))
              throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                        "save",
                                                                        objectType.Name));

            if (factoryType != null)
              method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, [obj]);
          }
        }
          
        var runLocal = method?.RunLocal ?? false;
        proxy = GetDataPortalProxy(runLocal);
      }
      else
      {
        Reflection.ServiceProviderMethodInfo? method;
        var criteria = Server.DataPortal.GetCriteriaArray(Server.EmptyCriteria.Instance);
        if (obj is Core.ICommandObject)
        {
          operation = DataPortalOperations.Execute;
          if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.EditObject, obj, ct))
            throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
              "execute",
              objectType.Name));
          _ = ServiceProviderMethodCaller.TryFindDataPortalMethod<ExecuteAttribute>(objectType, criteria, out method);
        }
        else
        {
          if (obj is Core.BusinessBase bbase)
          {
            if (bbase.IsDeleted)
            {
              if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.DeleteObject, obj, ct))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                  "delete",
                  objectType.Name));
              _ = ServiceProviderMethodCaller.TryFindDataPortalMethod<DeleteSelfAttribute>(objectType, criteria, out method);
            }
            else if (bbase.IsNew)
            {
              if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.CreateObject, obj, ct))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                  "create",
                  objectType.Name));
              _ = ServiceProviderMethodCaller.TryFindDataPortalMethod<InsertAttribute>(objectType, criteria, out method);
            }
            else
            {
              if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.EditObject, obj, ct))
                throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                  "save",
                  objectType.Name));
              _ = ServiceProviderMethodCaller.TryFindDataPortalMethod<UpdateAttribute>(objectType, criteria, out method);
            }
          }
          else
          {
            _ = ServiceProviderMethodCaller.TryFindDataPortalMethod<UpdateAttribute>(objectType, criteria, out method);
          }
        }
        proxy = GetDataPortalProxy(method);
      }

      var dpContext = new Server.DataPortalContext(_applicationContext, proxy.IsServerRemote);

      Server.DataPortalResult result = default!;
      try
      { 
        if (!proxy.IsServerRemote && DataPortalClientOptions.AutoCloneOnUpdate)
        {
          // when using local data portal, automatically
          // clone original object before saving
          if (obj is ICloneable cloneable)
            obj = (T)cloneable.Clone();
          result = await Cache.GetDataPortalResultAsync(objectType, obj, operation,
            async () => await proxy.Update(obj, dpContext, isSync));
        }
        else
        {
          var contextManager = _applicationContext.ApplicationContextAccessor;
          try
          {
            result = await Cache.GetDataPortalResultAsync(objectType, obj, operation,
              async () => await proxy.Update(obj, dpContext, isSync));
          }
          catch
          {
            if (obj is IUseApplicationContext useContext)
              useContext.ApplicationContext.ApplicationContextAccessor = contextManager;
            throw;
          }
        }
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
     
    return (T)result.ReturnObject!;
  }

    [DoesNotReturn]
    private void HandleUpdateDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Update", ex);
    }

    [DoesNotReturn]
    private void HandleDataPortalException(string operation, Server.DataPortalException ex)
    {
      var result = ex.Result;
      var original = ex.InnerException!;
      if (original.InnerException != null)
        original = original.InnerException;
      throw new DataPortalException(String.Format("DataPortal.{2} {0} ({1})", Resources.Failed, original.Message, operation), ex.InnerException!, result.ReturnObject);
    }

    /// <inheritdoc />
    public T Update(T obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

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

    /// <inheritdoc />
    public Task<T> UpdateAsync(T obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return DoUpdateAsync(obj, false);
    }

    private async Task DoDeleteAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, bool isSync, CancellationToken ct = default)
    {

      if (!await Csla.Rules.BusinessRules.HasPermissionAsync(_applicationContext, Rules.AuthorizationActions.DeleteObject, objectType, Server.DataPortal.GetCriteriaArray(criteria), ct))
        throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException, "delete", objectType.Name));

      _ = ServiceProviderMethodCaller.TryGetProviderMethodInfoFor<DeleteAttribute>(objectType, criteria, out var method);
      var proxy = GetDataPortalProxy(method);

      var dpContext = new Server.DataPortalContext(_applicationContext, proxy.IsServerRemote);
      try
      {
        var result = await Cache.GetDataPortalResultAsync(objectType, criteria, DataPortalOperations.Delete,
          async () => await proxy.Delete(objectType, criteria, dpContext, isSync));
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.Count > 0)
        {
          if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
            HandleDeleteDataPortalException(dpe);
        }
        throw new DataPortalException($"DataPortal.Delete {Resources.Failed}", ex, null);
      }
      catch (Server.DataPortalException ex)
      {
        HandleDeleteDataPortalException(ex);
      }
    }

    [DoesNotReturn]
    private void HandleDeleteDataPortalException(Server.DataPortalException ex)
    {
      HandleDataPortalException("Delete", ex);
    }

    /// <inheritdoc />
    public void Delete(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      Delete(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria));
    }

    /// <summary>
    /// Manager method for synchronous Delete operation; delegating to async version
    /// </summary>
    /// <param name="objectType">The type of object to instantiate and load</param>
    /// <param name="criteria">The criteria required to perform the load operation</param>
    /// <returns>Returns a populated object of the type requested</returns>
    private void Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria)
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

    /// <inheritdoc />
    public Task DeleteAsync(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return DoDeleteAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
    }

    /// <inheritdoc />
    public T Execute(T command)
    {
      if (command is null)
        throw new ArgumentNullException(nameof(command));

      return Update(command);
    }

    /// <inheritdoc />
    public T Execute(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return (T)DoFetchAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), true).Result;
    }

    /// <inheritdoc />
    public Task<T> ExecuteAsync(T command)
    {
      if (command is null)
        throw new ArgumentNullException(nameof(command));

      return DoUpdateAsync(command, false);
    }

    /// <inheritdoc />
    public async Task<T> ExecuteAsync(params object?[] criteria)
    {
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));

      return (T)await DoFetchAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
    }

    private IDataPortalProxy GetDataPortalProxy(Reflection.ServiceProviderMethodInfo? method)
    {
      if (method != null)
        return GetDataPortalProxy(method.MethodInfo.RunLocal());
      else
        return GetDataPortalProxy(false);
    }

    private IDataPortalProxy GetDataPortalProxy(bool forceLocal)
    {
      if (forceLocal || _applicationContext.IsOffline)
        return _applicationContext.CreateInstanceDI<Channels.Local.LocalProxy>();
      else
        return DataPortalProxy;
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    public T CreateChild()
    {
      var portal = new Server.ChildDataPortal(_applicationContext);
      return (T)(portal.Create(typeof(T)));
    }

    /// <inheritdoc />
    public T CreateChild(params object?[] parameters)
    {
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      return (T)(portal.Create(typeof(T), parameters));
    }

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    public async Task<T> CreateChildAsync()
    {
      var portal = new Server.ChildDataPortal(_applicationContext);
      return await portal.CreateAsync<T>();
    }

    /// <inheritdoc />
    public async Task<T> CreateChildAsync(params object?[] parameters)
    {
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      return await portal.CreateAsync<T>(parameters);
    }

    /// <summary>
    /// Fetches an existing
    /// child business object.
    /// </summary>
    public T FetchChild()
    {
      var portal = new Server.ChildDataPortal(_applicationContext);
      return (T)(portal.Fetch(typeof(T)));
    }

    /// <inheritdoc />
    public T FetchChild(params object?[] parameters)
    {
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      return (T)(portal.Fetch(typeof(T), parameters));
    }

    /// <summary>
    /// Fetches an existing
    /// child business object.
    /// </summary>
    public async Task<T> FetchChildAsync()
    {
      var portal = new Server.ChildDataPortal(_applicationContext);
      return await portal.FetchAsync<T>();
    }

    /// <inheritdoc />
    public async Task<T> FetchChildAsync(params object?[] parameters)
    {
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      return await portal.FetchAsync<T>(parameters);
    }

    /// <inheritdoc />
    public void UpdateChild(T child)
    {
      if (child is null) 
        throw new ArgumentNullException(nameof(child));

      var portal = new Server.ChildDataPortal(_applicationContext);
      portal.Update(child);
    }

    /// <inheritdoc />
    public void UpdateChild(object child, params object?[] parameters)
    {
      if (child is null)
        throw new ArgumentNullException(nameof(child));
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      portal.Update(child, parameters);
    }

    /// <inheritdoc />
    public void UpdateChild(T child, params object?[] parameters)
    {
      if (child is null) 
        throw new ArgumentNullException(nameof(child));
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      portal.Update(child, parameters);
    }

    /// <inheritdoc />
    public async Task UpdateChildAsync(T child)
    {
      if (child is null) 
        throw new ArgumentNullException(nameof(child));

      var portal = new Server.ChildDataPortal(_applicationContext);
      await portal.UpdateAsync(child).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateChildAsync(T child, params object?[] parameters)
    {
      if (child is null) 
        throw new ArgumentNullException(nameof(child));
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var portal = new Server.ChildDataPortal(_applicationContext);
      await portal.UpdateAsync(child, parameters).ConfigureAwait(false);
    }

    async Task<object> IDataPortal.CreateAsync(params object?[] criteria) => Task.FromResult(await CreateAsync(criteria));
    async Task<object> IDataPortal.FetchAsync(params object?[] criteria) => Task.FromResult(await FetchAsync(criteria));
    async Task<object> IDataPortal.UpdateAsync(object obj) => Task.FromResult(await UpdateAsync((T)obj));
    async Task<object> IDataPortal.ExecuteAsync(object command) => Task.FromResult(await ExecuteAsync((T)command));
    async Task<object> IDataPortal.ExecuteAsync(params object?[] criteria) => Task.FromResult(await ExecuteAsync(criteria));
    object IDataPortal.Create(params object?[] criteria) => Create(criteria);
    object IDataPortal.Fetch(params object?[] criteria) => Fetch(criteria);
    object IDataPortal.Execute(object obj) => Execute((T)obj);
    object IDataPortal.Execute(params object?[] criteria) => Execute(criteria);
    object IDataPortal.Update(object obj) => Update((T)obj);

    async Task<object> IChildDataPortal.CreateChildAsync(params object?[] criteria) => Task.FromResult(await CreateChildAsync(criteria));
    async Task<object> IChildDataPortal.FetchChildAsync(params object?[] criteria) => Task.FromResult(await FetchChildAsync(criteria));
    async Task IChildDataPortal.UpdateChildAsync(object obj, params object?[] parameters) => await UpdateChildAsync((T)obj);
    object IChildDataPortal.CreateChild(params object?[] criteria) => CreateChild(criteria);
    object IChildDataPortal.FetchChild(params object?[] criteria) => FetchChild(criteria);
    void IChildDataPortal.UpdateChild(object obj, params object?[] parameters) => UpdateChild(obj, parameters);
  }

  internal static class Extensions
  {
    internal static bool RunLocal(this System.Reflection.MethodInfo t)
    {
      return t.CustomAttributes.Count(a => a.AttributeType.Equals(typeof(RunLocalAttribute))) > 0;
    }
  }
}
