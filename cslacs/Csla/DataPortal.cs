using System;
using System.ComponentModel;
using Csla.Reflection;
using Csla.Properties;
using Csla.Server;
using Csla.Serialization.Mobile;
using System.Windows;

namespace Csla
{

  /// <summary>
  /// This is the client-side DataPortal as described in
  /// Chapter 4.
  /// </summary>
  public static class DataPortal
  {
    #region DataPortal events

    /// <summary>
    /// Raised by DataPortal before it starts
    /// setting up to call a server-side
    /// DataPortal method.
    /// </summary>
    public static event Action<System.Object> DataPortalInitInvoke;

    /// <summary>
    /// Raised by DataPortal prior to calling the 
    /// requested server-side DataPortal method.
    /// </summary>
    public static event Action<DataPortalEventArgs> DataPortalInvoke;

    /// <summary>
    /// Raised by DataPortal after the requested 
    /// server-side DataPortal method call is complete.
    /// </summary>
    public static event Action<DataPortalEventArgs> DataPortalInvokeComplete;

    private static void OnDataPortalInitInvoke(object e)
    {
      Action<System.Object> action = DataPortalInitInvoke;
      if (action != null)
        action(e);
    }

    private static void OnDataPortalInvoke(DataPortalEventArgs e)
    {
      Action<DataPortalEventArgs> action = DataPortalInvoke;
      if (action != null)
        action(e);
    }

    private static void OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Action<DataPortalEventArgs> action = DataPortalInvokeComplete;
      if (action != null)
        action(e);
    }

    #endregion

    #region Data Access methods

    private const int EmptyCriteria = 1;

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    public static T Create<T>(object criteria)
    {
      return (T)Create(typeof(T), criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <returns>A new object, populated with default values.</returns>
    public static T Create<T>()
    {
      return (T)Create(typeof(T), EmptyCriteria);
    }

    internal static object Create(Type type)
    {
      return Create(type, EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public static object Create(object criteria)
    {
      return Create(MethodCaller.GetObjectType(criteria), criteria);
    }

    internal static object Create(Type objectType, object criteria)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        OnDataPortalInitInvoke(null);

        if (!Csla.Security.AuthorizationRules.CanCreateObject(objectType))
          throw new System.Security.SecurityException(string.Format(
            Resources.UserNotAuthorizedException,
            "create",
            objectType.Name));

        var method = Server.DataPortalMethodCache.GetCreateMethod(objectType, criteria);

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(method.RunLocal);

        dpContext =
          new Csla.Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Create));

        try
        {
          result = proxy.Create(objectType, criteria, dpContext);
        }
        catch (Server.DataPortalException ex)
        {
          result = ex.Result;
          if (proxy.IsServerRemote)
            ApplicationContext.SetGlobalContext(result.GlobalContext);
          throw new DataPortalException(
            string.Format("DataPortal.Create {0} ({1})", Resources.Failed, ex.InnerException.InnerException),
            ex.InnerException, result.ReturnObject);
        }

        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);

        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Create));
      }
      catch (Exception ex)
      {
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Create, ex));
        throw;
      }
      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Fetch<T>(object criteria)
    {
      return (T)Fetch(typeof(T), criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <returns>An object populated with values from the database.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Fetch<T>()
    {
      return (T)Fetch(typeof(T), EmptyCriteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    public static object Fetch(object criteria)
    {
      return Fetch(MethodCaller.GetObjectType(criteria), criteria);
    }

    internal static object Fetch(Type objectType)
    {
      return Fetch(objectType, EmptyCriteria);
    }

    internal static object Fetch(Type objectType, object criteria)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        OnDataPortalInitInvoke(null);

        if (!Csla.Security.AuthorizationRules.CanGetObject(objectType))
          throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
            "get",
            objectType.Name));

        var method = Server.DataPortalMethodCache.GetFetchMethod(objectType, criteria);

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(method.RunLocal);

        dpContext =
          new Server.DataPortalContext(GetPrincipal(),
          proxy.IsServerRemote);

        OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Fetch));

        try
        {
          result = proxy.Fetch(objectType, criteria, dpContext);
        }
        catch (Server.DataPortalException ex)
        {
          result = ex.Result;
          if (proxy.IsServerRemote)
            ApplicationContext.SetGlobalContext(result.GlobalContext);
          string innerMessage = string.Empty;
          if (ex.InnerException is Csla.Reflection.CallMethodException)
          {
            if (ex.InnerException.InnerException != null)
              innerMessage = ex.InnerException.InnerException.Message;
          }
          else
          {
            innerMessage = ex.InnerException.Message;
          }
          throw new DataPortalException(
            String.Format("DataPortal.Fetch {0} ({1})", Resources.Failed, innerMessage),
            ex.InnerException, result.ReturnObject);
        }

        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);

        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Fetch));
      }
      catch (Exception ex)
      {
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Fetch, ex));
        throw;
      }
      return result.ReturnObject;
    }

    /// <summary>
    /// Called to execute a Command object on the server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To be a Command object, the object must inherit from
    /// <see cref="CommandBase">CommandBase</see>.
    /// </para><para>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </para><para>
    /// On the server, the Command object's DataPortal_Execute() method will
    /// be invoked. Write any server-side code in that method.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Specific type of the Command object.</typeparam>
    /// <param name="obj">A reference to the Command object to be executed.</param>
    /// <returns>A reference to the updated Command object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters",
    MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Execute<T>(T obj)
    // Remove Constraint to simplify asynch data portal  where T : CommandBase
    {
      return (T)Update(obj);
    }

    /// <summary>
    /// Called to execute a Command object on the server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </para><para>
    /// On the server, the Command object's DataPortal_Execute() method will
    /// be invoked. Write any server-side code in that method.
    /// </para>
    /// </remarks>
    /// <param name="obj">A reference to the Command object to be executed.</param>
    /// <returns>A reference to the updated Command object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static CommandBase Execute(CommandBase obj)
    {
      return (CommandBase)Update(obj);
    }

    /// <summary>
    /// Called by the business object's Save() method to
    /// insert, update or delete an object in the database.
    /// </summary>
    /// <remarks>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </remarks>
    /// <typeparam name="T">Specific type of the business object.</typeparam>
    /// <param name="obj">A reference to the business object to be updated.</param>
    /// <returns>A reference to the updated business object.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Update<T>(T obj)
    {
      return (T)Update((object)obj);
    }

    /// <summary>
    /// Called by the business object's Save() method to
    /// insert, update or delete an object in the database.
    /// </summary>
    /// <remarks>
    /// Note that this method returns a reference to the updated business object.
    /// If the server-side DataPortal is running remotely, this will be a new and
    /// different object from the original, and all object references MUST be updated
    /// to use this new object.
    /// </remarks>
    /// <param name="obj">A reference to the business object to be updated.</param>
    /// <returns>A reference to the updated business object.</returns>
    public static object Update(object obj)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      DataPortalOperations operation = DataPortalOperations.Update;
      Type objectType = obj.GetType();
      try
      {
        OnDataPortalInitInvoke(null);
        DataPortalMethodInfo method = null;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
          var bbase = obj as Core.BusinessBase;
          if (bbase != null && bbase.IsDeleted)
          {
            if (!Csla.Security.AuthorizationRules.CanDeleteObject(objectType))
              throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "delete",
                objectType.Name));
            if (factoryType != null)
              method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, new object[] { obj });
          }
          else
          {
            if (!Csla.Security.AuthorizationRules.CanEditObject(objectType))
              throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                "save",
                objectType.Name));
            if (factoryType != null)
              method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, new object[] { obj });
          }
          if (method == null)
            method = new DataPortalMethodInfo();
        }
        else
        {
          string methodName;
          if (obj is CommandBase)
          {
            methodName = "DataPortal_Execute";
            operation = DataPortalOperations.Execute;
            if (!Csla.Security.AuthorizationRules.CanEditObject(objectType))
              throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
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
                if (!Csla.Security.AuthorizationRules.CanDeleteObject(objectType))
                  throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                    "delete",
                    objectType.Name));
              }
              else
                if (bbase.IsNew)
                {
                  methodName = "DataPortal_Insert";
                  if (!Csla.Security.AuthorizationRules.CanCreateObject(objectType))
                    throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                      "create",
                      objectType.Name));
                }
                else
                {
                  methodName = "DataPortal_Update";
                  if (!Csla.Security.AuthorizationRules.CanEditObject(objectType))
                    throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                      "save",
                      objectType.Name));
                }
            }
            else
            {
              methodName = "DataPortal_Update";
              if (!Csla.Security.AuthorizationRules.CanEditObject(objectType))
                throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
                  "save",
                  objectType.Name));
            }
          }
          method = Server.DataPortalMethodCache.GetMethodInfo(obj.GetType(), methodName);
        }

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(method.RunLocal);

        dpContext =
          new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, operation));

        try
        {
          if (!proxy.IsServerRemote && ApplicationContext.AutoCloneOnUpdate)
          {
            // when using local data portal, automatically
            // clone original object before saving
            ICloneable cloneable = obj as ICloneable;
            if (cloneable != null)
              obj = cloneable.Clone();
          }
          result = proxy.Update(obj, dpContext);
        }
        catch (Server.DataPortalException ex)
        {
          result = ex.Result;
          if (proxy.IsServerRemote)
            ApplicationContext.SetGlobalContext(result.GlobalContext);
          throw new DataPortalException(
            String.Format("DataPortal.Update {0} ({1})", Resources.Failed, ex.InnerException.InnerException),
            ex.InnerException, result.ReturnObject);
        }

        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);

        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, operation));
      }
      catch (Exception ex)
      {
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, operation, ex));
        throw;
      }
      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static void Delete<T>(object criteria)
    {
      Delete(typeof(T), criteria);
    }

    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static void Delete(object criteria)
    {
      Type objectType = MethodCaller.GetObjectType(criteria);
      Delete(objectType, criteria);
    }

    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="objectType">Type of business object to delete.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    private static void Delete(Type objectType, object criteria)
    {
      Server.DataPortalResult result = null;
      Server.DataPortalContext dpContext = null;
      try
      {
        OnDataPortalInitInvoke(null);

        if (!Csla.Security.AuthorizationRules.CanDeleteObject(objectType))
          throw new System.Security.SecurityException(string.Format(Resources.UserNotAuthorizedException,
            "delete",
            objectType.Name));

        var method = Server.DataPortalMethodCache.GetMethodInfo(
          objectType, "DataPortal_Delete", criteria);

        DataPortalClient.IDataPortalProxy proxy;
        proxy = GetDataPortalProxy(method.RunLocal);

        dpContext = new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

        OnDataPortalInvoke(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Delete));

        try
        {
          result = proxy.Delete(objectType, criteria, dpContext);
        }
        catch (Server.DataPortalException ex)
        {
          result = ex.Result;
          if (proxy.IsServerRemote)
            ApplicationContext.SetGlobalContext(result.GlobalContext);
          throw new DataPortalException(
            String.Format("DataPortal.Delete {0} ({1})", Resources.Failed, ex.InnerException.InnerException),
            ex.InnerException, result.ReturnObject);
        }

        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);

        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Delete));
      }
      catch (Exception ex)
      {
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext, objectType, DataPortalOperations.Delete, ex));
        throw;
      }
    }

    #endregion

    #region Async Data Access Methods

    #region Begin Create

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginCreate<T>(DataPortal<T>.EmptyCriteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginCreate<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginCreate<T>(DataPortal<T>.EmptyCriteria, callback, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginCreate<T>(criteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// create a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to create.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginCreate<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.CreateCompleted += callback;
      dp.BeginCreate(criteria, userState);
    }

    #endregion

    #region Begin Fetch

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginFetch<T>(DataPortal<T>.EmptyCriteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginFetch<T>(EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      BeginFetch<T>(DataPortal<T>.EmptyCriteria, callback, userState);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to fetch.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginFetch<T>(criteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// fetch a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to fetch.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to fetch.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginFetch<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(criteria, userState);
    }

    #endregion

    #region Begin Update

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// update a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to update.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginUpdate<T>(object obj, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginUpdate<T>(obj, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// update a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to update.
    /// </typeparam>
    /// <param name="obj">
    /// Business object to update.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginUpdate<T>(object obj, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      DataPortal<T> dp = new DataPortal<T>();
      dp.UpdateCompleted += callback;
      dp.BeginUpdate(obj, userState);
    }

    #endregion

    #region Begin Delete

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// delete a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to delete.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to delete.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback)
      where T : IMobileObject
    {
      BeginDelete<T>(criteria, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// delete a business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to delete.
    /// </typeparam>
    /// <param name="criteria">
    /// Criteria describing the object to delete.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginDelete<T>(object criteria, EventHandler<DataPortalResult<T>> callback, object userState)
      where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.DeleteCompleted += callback;
      dp.BeginDelete(criteria, userState);
    }

    #endregion

    #region Begin Execute

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// execute a command object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object to execute.
    /// </typeparam>
    /// <param name="obj">
    /// Reference to the object to execute.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    public static void BeginExecute<T>(T obj, EventHandler<DataPortalResult<T>> callback) where T : IMobileObject
    {
      BeginExecute<T>(obj, callback, null);
    }

    /// <summary>
    /// Starts an asynchronous data portal operation to
    /// execute a command object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of object to execute.
    /// </typeparam>
    /// <param name="obj">
    /// Reference to the object to execute.
    /// </param>
    /// <param name="callback">
    /// Reference to method that will handle the 
    /// asynchronous callback when the operation
    /// is complete.
    /// </param>
    /// <param name="userState">User state object.</param>
    public static void BeginExecute<T>(T obj, EventHandler<DataPortalResult<T>> callback, object userState) where T : IMobileObject
    {
      var dp = new DataPortal<T>();
      dp.ExecuteCompleted += callback;
      dp.BeginExecute(obj, userState);
    }

    #endregion

    #endregion

    #region  Child Data Access methods

    /// <summary>
    /// Creates and initializes a new
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to create.
    /// </typeparam>
    /// <param name="parameters">
    /// Parameters passed to child create method.
    /// </param>
    public static T CreateChild<T>(params object[] parameters)
    {

      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return (T)(portal.Create(typeof(T), parameters));

    }

    /// <summary>
    /// Creates and loads an existing
    /// child business object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of business object to retrieve.
    /// </typeparam>
    /// <param name="parameters">
    /// Parameters passed to child fetch method.
    /// </param>
    public static T FetchChild<T>(params object[] parameters)
    {

      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      return (T)(portal.Fetch(typeof(T), parameters));

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
    public static void UpdateChild(object child, params object[] parameters)
    {

      Server.ChildDataPortal portal = new Server.ChildDataPortal();
      portal.Update(child, parameters);

    }

    #endregion

    #region DataPortal Proxy

    private static Type _proxyType;

    private static DataPortalClient.IDataPortalProxy GetDataPortalProxy(bool forceLocal)
    {
      if (DataPortal.IsInDesignMode)
      {
        return new DataPortalClient.DesignTimeProxy();
      }
      else
      {
        if (forceLocal)
        {
          return new DataPortalClient.LocalProxy();
        }
        else
        {
          if (_proxyType == null)
          {
            string proxyTypeName = ApplicationContext.DataPortalProxy;
            if (proxyTypeName == "Local")
              _proxyType = typeof(DataPortalClient.LocalProxy);
            else
              _proxyType = Type.GetType(proxyTypeName, true, true);
          }
          return (DataPortalClient.IDataPortalProxy)Activator.CreateInstance(_proxyType);
        }
      }
    }

    /// <summary>
    /// Resets the data portal proxy type, so the
    /// next data portal call will reload the proxy
    /// type based on current configuration values.
    /// </summary>
    public static void ResetProxyType()
    {
      _proxyType = null;
    }

    /// <summary>
    /// Releases any remote data portal proxy object, so
    /// the next data portal call will create a new
    /// proxy instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Proxies no longer cached")]
    public static void ReleaseProxy()
    { }

    #endregion

    #region Security

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

    #endregion

    #region Design Time Support

    /// <summary>
    /// Gets a value indicating whether the code is running
    /// in WPF design mode.
    /// </summary>
    public static bool IsInDesignMode
    {
      get 
      {
        return false;
        //if (System.Web.HttpContext.Current != null || System.Windows.Application.Current == null)
        //  return false;
        //else
        //  return DesignerProperties.GetIsInDesignMode(new DependencyObject()); 
      }
    }


    #endregion
  }
}