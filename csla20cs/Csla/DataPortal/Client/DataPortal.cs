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
  /// Chapter 5.
  /// </summary>
  public sealed class DataPortal
  {

    #region Constructors

    private DataPortal()
    {

    }

    #endregion

    #region DataPortal events

    /// <summary>
    /// Raised by DataPortal prior to calling the 
    /// requested server-side DataPortal method.
    /// </summary>
    public static event DataPortalInvokeEventHandler DataPortalInvoke;
    public delegate void DataPortalInvokeEventHandler(DataPortalEventArgs e);

    /// <summary>
    /// Raised by DataPortal after the requested 
    /// server-side DataPortal method call is complete.
    /// </summary>
    public static event DataPortalInvokeCompleteEventHandler DataPortalInvokeComplete;
    public delegate void DataPortalInvokeCompleteEventHandler(DataPortalEventArgs e);

    private static void OnDataPortalInvoke(DataPortalEventArgs e)
    {
      if (DataPortalInvoke != null)
        DataPortalInvoke(e);
    }

    private static void OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      if (DataPortalInvokeComplete != null)
        DataPortalInvokeComplete(e);
    }

    #endregion

    #region Data Access methods

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
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
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public static object Create(object criteria)
    {
      return Create(GetObjectType(criteria), criteria);
    }

    private static object Create(Type objectType, object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(objectType, "DataPortal_Create");

      DataPortalClient.IDataPortalProxy portal;
      portal = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = new Csla.Server.DataPortalContext(GetPrincipal(), portal.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = (Server.DataPortalResult)portal.Create(objectType, criteria, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (portal.IsServerRemote)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Create " + Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (portal.IsServerRemote)
        RestoreContext(result);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2223:MembersShouldDifferByMoreThanReturnType")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Fetch<T>(object criteria)
    {
      return (T)Fetch(criteria);
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    public static object Fetch(object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Fetch");

      DataPortalClient.IDataPortalProxy portal;
      portal = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), portal.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = (Server.DataPortalResult)portal.Fetch(criteria, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (portal.IsServerRemote)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Fetch " + Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (portal.IsServerRemote)
        RestoreContext(result);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by the <see cref="Csla.BusinessBase.Save" /> method to
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static T Update<T>(T obj)
    {
      return (T)Update((object)obj);
    }

    /// <summary>
    /// Called by the <see cref="M:Csla.BusinessBase.Save" /> method to
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
      if (obj is CommandBase)
        method = GetMethod(obj.GetType(), "DataPortal_Execute");
      else
        method = GetMethod(obj.GetType(), "DataPortal_Update");

      DataPortalClient.IDataPortalProxy portal;
      portal = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), portal.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = (Server.DataPortalResult)portal.Update(obj, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (portal.IsServerRemote)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Update " + Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (portal.IsServerRemote)
        RestoreContext(result);

      OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a <c>Shared</c> method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="criteria">Object-specific criteria.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Csla.DataPortalException.#ctor(System.String,System.Exception,System.Object)")]
    public static void Delete(object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Delete");

      DataPortalClient.IDataPortalProxy portal;
      portal = GetDataPortalProxy(RunLocal(method));

      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), portal.IsServerRemote);

      OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        result = (Server.DataPortalResult)portal.Delete(criteria, dpContext);
      }
      catch (Server.DataPortalException ex)
      {
        result = ex.Result;
        if (portal.IsServerRemote)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Delete " + Resources.Failed, ex.InnerException, result.ReturnObject);
      }

      if (portal.IsServerRemote)
        RestoreContext(result);

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
            string typeName = proxyTypeName.Substring(0, proxyTypeName.IndexOf(",")).Trim();
            string assemblyName = proxyTypeName.Substring(proxyTypeName.IndexOf(",") + 1).Trim();
            _portal = (DataPortalClient.IDataPortalProxy)Activator.CreateInstance(assemblyName, typeName).Unwrap();
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
        return System.Threading.Thread.CurrentPrincipal;
      }
    }

    #endregion

    #region Helper methods

    private static void RestoreContext(object result)
    {
      System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("Csla.GlobalContext");
      Thread.SetData(slot, ((Server.DataPortalResult)result).GlobalContext);
    }

    private static bool RunLocal(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(RunLocalAttribute));
    }

    private static MethodInfo GetMethod(Type objectType, string method)
    {
      return objectType.GetMethod(
          method,
          BindingFlags.FlattenHierarchy |
          BindingFlags.Instance |
          BindingFlags.Public |
          BindingFlags.NonPublic);
    }

    private static Type GetObjectType(object criteria)
    {
      if (criteria.GetType().IsSubclassOf(typeof(CriteriaBase)))
      {
        // get the type of the actual business object
        // from CriteriaBase (using the new scheme)
        return ((CriteriaBase)criteria).ObjectType;
      }
      else
      {
        // get the type of the actual business object
        // based on the nested class scheme in the book
        return criteria.GetType().DeclaringType;
      }
    }

    #endregion

  }
}