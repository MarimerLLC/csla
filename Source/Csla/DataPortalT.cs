//-----------------------------------------------------------------------
// <copyright file="DataPortalT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client side data portal used for making asynchronous</summary>
//-----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Linq;
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
  public class DataPortal<T> : IDataPortal<T>, IChildDataPortal<T>
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext">ApplicationContext</param>
    /// <param name="proxy"></param>
    public DataPortal(ApplicationContext applicationContext, DataPortalClient.IDataPortalProxy proxy)
    {
      ApplicationContext = applicationContext;
      DataPortalProxy = proxy;
    }

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    private ApplicationContext ApplicationContext { get; set; }
    private DataPortalClient.IDataPortalProxy DataPortalProxy { get; set; }

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
          new Csla.Server.DataPortalContext(ApplicationContext, GetPrincipal(), proxy.IsServerRemote);

        try
        {
          result = await proxy.Create(objectType, criteria, dpContext, isSync);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
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
      }
      catch
      {
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
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.GetObject, objectType, Server.DataPortal.GetCriteriaArray(criteria)))
          throw new Csla.Security.SecurityException(string.Format(
            Resources.UserNotAuthorizedException,
            "get",
            objectType.Name));

        var method = ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria), false);
        var proxy = GetDataPortalProxy(method);

        dpContext =
          new Csla.Server.DataPortalContext(ApplicationContext, GetPrincipal(), proxy.IsServerRemote);

        try
        {
          result = await proxy.Fetch(objectType, criteria, dpContext, isSync);
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
      }
      catch
      {
        throw;
      }
      return result.ReturnObject;
    }

    private void HandleFetchDataPortalException(Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      HandleDataPortalException("Fetch", ex, isSync, proxy);
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
      Type objectType = obj.GetType();
      try
      {
        DataPortalClient.IDataPortalProxy proxy = null;
        var factoryInfo = Csla.Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          Csla.Server.DataPortalMethodInfo method = null;
          var factoryType = Csla.Server.FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);

          if (obj is Core.ICommandObject)
          {
            if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
              throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "execute",
                objectType.Name));
            if (factoryType != null)
              method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.ExecuteMethodName, new object[] { obj });
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
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName,
                                                                      new object[] { obj });
              }
              // must check the same authorization rules as for DataPortal_XYZ methods 
              else if (bbase.IsNew)
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.CreateObject, obj))
                  throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                                                                            "create",
                                                                            objectType.Name));
                if (factoryType != null)
                  method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName,
                                                                    new object[] { obj });
              }
              else
              {
                if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
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
              if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, obj))
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
          proxy = GetDataPortalProxy(method.RunLocal);
        }
        else
        {
          Reflection.ServiceProviderMethodInfo method;
          var criteria = Server.DataPortal.GetCriteriaArray(Server.EmptyCriteria.Instance);
          if (obj is Core.ICommandObject)
          {
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
          new Server.DataPortalContext(ApplicationContext, GetPrincipal(), proxy.IsServerRemote);

        try
        {
          if (!proxy.IsServerRemote && ApplicationContext.AutoCloneOnUpdate)
          {
            // when using local data portal, automatically
            // clone original object before saving
            if (obj is ICloneable cloneable)
              obj = (T)cloneable.Clone();
          }
          result = await proxy.Update(obj, dpContext, isSync);
        }
        catch (AggregateException ex)
        {
          if (ex.InnerExceptions.Count > 0)
          {
            if (ex.InnerExceptions[0] is Server.DataPortalException dpe)
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
      }
      catch
      {
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
        if (!Csla.Rules.BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.DeleteObject, objectType, Server.DataPortal.GetCriteriaArray(criteria)))
          throw new Csla.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
            "delete",
            objectType.Name));

        var method = ServiceProviderMethodCaller.FindDataPortalMethod<DeleteAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria), false);
        var proxy = GetDataPortalProxy(method);

        dpContext = new Server.DataPortalContext(ApplicationContext, GetPrincipal(), proxy.IsServerRemote);

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
      }
      catch
      {
        throw;
      }
    }

    private void HandleDeleteDataPortalException(Server.DataPortalException ex, bool isSync, Csla.DataPortalClient.IDataPortalProxy proxy)
    {
      HandleDataPortalException("Delete", ex, isSync, proxy);
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
    public async Task DeleteAsync(params object[] criteria)
    {
      await DoDeleteAsync(typeof(T), Server.DataPortal.GetCriteriaFromArray(criteria), false);
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
    public async Task<T> ExecuteAsync(T command)
    {
      return await DoUpdateAsync(command, false);
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

    private System.Security.Principal.IPrincipal GetPrincipal()
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
  }

  internal static class Extensions
  {
    internal static bool RunLocal(this System.Reflection.MethodInfo t)
    {
      return t.CustomAttributes.Count(a => a.AttributeType.Equals(typeof(RunLocalAttribute))) > 0;
    }
  }
}
