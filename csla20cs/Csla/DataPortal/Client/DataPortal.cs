using System;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using Csla.Properties;

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
    /// Raised by DataPortal prior to calling the 
    /// requested server-side DataPortal method.
    /// </summary>
    public static event Action<DataPortalEventArgs> DataPortalInvoke;

    /// <summary>
    /// Raised by DataPortal after the requested 
    /// server-side DataPortal method call is complete.
    /// </summary>
    public static event Action<DataPortalEventArgs> DataPortalInvokeComplete;

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
      return (T)Create(typeof(T), null);
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

    private static object Create(Type objectType, object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = 
        MethodCaller.GetMethod(objectType, "DataPortal_Create", criteria);

      DataPortalClient.IDataPortalProxy proxy;
      proxy = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = 
        new Csla.Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

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
          "DataPortal.Create " + Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (proxy.IsServerRemote)
        ApplicationContext.SetGlobalContext(result.GlobalContext);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

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
      return (T)Fetch(typeof(T), null);
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

    private static object Fetch(Type objectType, object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = MethodCaller.GetMethod(
        objectType, "DataPortal_Fetch", criteria);

      DataPortalClient.IDataPortalProxy proxy;
      proxy = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = 
        new Server.DataPortalContext(GetPrincipal(), 
        proxy.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = proxy.Fetch(objectType, criteria, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);
        throw new DataPortalException("DataPortal.Fetch " + 
          Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (proxy.IsServerRemote)
        ApplicationContext.SetGlobalContext(result.GlobalContext);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

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
    MessageId="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Execute<T>(T obj) where T : CommandBase
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId="Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
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
      Server.DataPortalResult result;

      MethodInfo method;
      string methodName;
      if (obj is CommandBase)
        methodName = "DataPortal_Execute";
      else if (obj is Core.BusinessBase)
      {
        Core.BusinessBase tmp = (Core.BusinessBase)obj;
        if (tmp.IsDeleted)
          methodName = "DataPortal_DeleteSelf";
        else
          if (tmp.IsNew)
            methodName = "DataPortal_Insert";
          else
            methodName = "DataPortal_Update";
      }
      else
        methodName = "DataPortal_Update";
      
      method = MethodCaller.GetMethod(obj.GetType(), methodName);

      DataPortalClient.IDataPortalProxy proxy;
      proxy = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = 
        new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = proxy.Update(obj, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);
        throw new DataPortalException("DataPortal.Update " + 
          Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (proxy.IsServerRemote)
        ApplicationContext.SetGlobalContext(result.GlobalContext);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a Shared (static in C#) method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static void Delete(object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = MethodCaller.GetMethod(
        MethodCaller.GetObjectType(criteria), "DataPortal_Delete", criteria);

      DataPortalClient.IDataPortalProxy proxy;
      proxy = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), proxy.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = proxy.Delete(criteria, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (proxy.IsServerRemote)
          ApplicationContext.SetGlobalContext(result.GlobalContext);
        throw new DataPortalException("DataPortal.Delete " + Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (proxy.IsServerRemote)
        ApplicationContext.SetGlobalContext(result.GlobalContext);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));
    }

    #endregion

    #region DataPortal Proxy

    private static DataPortalClient.IDataPortalProxy _localPortal;
    private static DataPortalClient.IDataPortalProxy _portal;

    private static DataPortalClient.IDataPortalProxy GetDataPortalProxy(bool forceLocal)
    {
      if (forceLocal)
      {
        if (_localPortal == null)
          _localPortal = new DataPortalClient.LocalProxy();
        return _localPortal;
      }
      else
      {
        if (_portal == null)
        {
          string proxyTypeName = ApplicationContext.DataPortalProxy;
          if (proxyTypeName == "Local")
            _portal = new DataPortalClient.LocalProxy();
          else
          {
            string typeName = 
              proxyTypeName.Substring(0, proxyTypeName.IndexOf(",")).Trim();
            string assemblyName = 
              proxyTypeName.Substring(proxyTypeName.IndexOf(",") + 1).Trim();
            _portal = (DataPortalClient.IDataPortalProxy)
              Activator.CreateInstance(assemblyName, typeName).Unwrap();
          }
        }
        return _portal;
      }
    }

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

    #region Helper methods

    private static bool RunLocal(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(RunLocalAttribute));
    }

    #endregion

  }
}