using System;
using System.Threading;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace CSLA
{
  /// <summary>
  /// This is the client-side DataPortal as described in
  /// Chapter 5.
  /// </summary>
  public class DataPortal
  {
    static bool _portalRemote = false;

    #region DataPortal events

    /// <summary>
    /// Delegate used by the OnDataPortalInvoke
    /// and OnDataPortalInvokeComplete events.
    /// </summary>
    public delegate void DataPortalInvokeDelegate(DataPortalEventArgs e);

    /// <summary>
    /// Raised by DataPortal prior to calling the 
    /// requested server-side DataPortal method.
    /// </summary>
    public static event DataPortalInvokeDelegate OnDataPortalInvoke;
    /// <summary>
    /// Raised by DataPortal after the requested 
    /// server-side DataPortal method call is complete.
    /// </summary>
    public static event DataPortalInvokeDelegate OnDataPortalInvokeComplete;

    #endregion

    #region Data Access methods

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    static public object Create(object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Create");

      bool forceLocal = RunLocal(method);
      bool isRemotePortal = _portalRemote && !forceLocal;
      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), isRemotePortal);

      if(OnDataPortalInvoke != null)
        OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        if(IsTransactionalMethod(method))
          result = (Server.DataPortalResult)ServicedPortal(forceLocal).Create(criteria, dpContext);
        else
          result = (Server.DataPortalResult)Portal(forceLocal).Create(criteria, dpContext);
      }
      catch(Server.DataPortalException ex)
      {
        result = ex.Result;
        if(isRemotePortal)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Create " + Resources.Strings.GetResourceString("Failed"), ex.InnerException, result.ReturnObject);
      }

      if(isRemotePortal)
      {
        RestoreContext(result);
        Serialization.SerializationNotification.OnDeserialized(result.ReturnObject);
      }

      if(OnDataPortalInvokeComplete != null)
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    static public object Fetch(object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Fetch");

      bool forceLocal = RunLocal(method);
      bool isRemotePortal = _portalRemote && !forceLocal;
      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), isRemotePortal);

      if(OnDataPortalInvoke != null)
        OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        if(IsTransactionalMethod(method))
          result = (Server.DataPortalResult)ServicedPortal(forceLocal).Fetch(criteria, dpContext);
        else
          result = (Server.DataPortalResult)Portal(forceLocal).Fetch(criteria, dpContext);
      }
      catch(Server.DataPortalException ex)
      {
        result = ex.Result;
        if(isRemotePortal)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Fetch " + Resources.Strings.GetResourceString("Failed"), ex.InnerException, result.ReturnObject);
      }

      if(isRemotePortal)
      {
        RestoreContext(result);
        Serialization.SerializationNotification.OnDeserialized(result.ReturnObject);
      }

      if(OnDataPortalInvokeComplete != null)
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by the <see cref="M:CSLA.BusinessBase.Save" /> method to
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
    static public object Update(object obj)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(obj.GetType(), "DataPortal_Update");

      bool forceLocal = RunLocal(method);
      bool isRemotePortal = _portalRemote && !forceLocal;
      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), isRemotePortal);

      if(OnDataPortalInvoke != null)
        OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      if(isRemotePortal)
        Serialization.SerializationNotification.OnSerializing(obj);

      try
      {
        if(IsTransactionalMethod(method))
          result = (Server.DataPortalResult)ServicedPortal(forceLocal).Update(obj, dpContext);
        else
          result = (Server.DataPortalResult)Portal(forceLocal).Update(obj, dpContext);
      }
      catch(Server.DataPortalException ex)
      {
        result = ex.Result;
        if(isRemotePortal)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Update " + Resources.Strings.GetResourceString("Failed"), ex.InnerException, result.ReturnObject);
      }

      if(isRemotePortal)
      {
        RestoreContext(result);
        Serialization.SerializationNotification.OnSerialized(obj);
        Serialization.SerializationNotification.OnDeserialized(result.ReturnObject);
      }

      if(OnDataPortalInvokeComplete != null)
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));

      return result.ReturnObject;
    }

    /// <summary>
    /// Called by a <c>Shared</c> method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    static public void Delete(object criteria)
    {
      Server.DataPortalResult result;

      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Delete");

      bool forceLocal = RunLocal(method);
      bool isRemotePortal = _portalRemote && !forceLocal;
      Server.DataPortalContext dpContext = new Server.DataPortalContext(GetPrincipal(), isRemotePortal);

      if(OnDataPortalInvoke != null)
        OnDataPortalInvoke(new DataPortalEventArgs(dpContext));

      try
      {
        if(IsTransactionalMethod(method))
          result = (Server.DataPortalResult)ServicedPortal(forceLocal).Delete(criteria, dpContext);
        else
          result = (Server.DataPortalResult)Portal(forceLocal).Delete(criteria, dpContext);
      }
      catch(Server.DataPortalException ex)
      {
        result = ex.Result;
        if(isRemotePortal)
          RestoreContext(result);
        throw new DataPortalException("DataPortal.Delete " + Resources.Strings.GetResourceString("Failed"), ex.InnerException, result.ReturnObject);
      }

      if(isRemotePortal)
        RestoreContext(result);

      if(OnDataPortalInvokeComplete != null)
        OnDataPortalInvokeComplete(new DataPortalEventArgs(dpContext));
    }

    #endregion

    #region Server-side DataPortal

    static Server.DataPortal _portal;
    static Server.DataPortal _remotePortal;

    private static Server.DataPortal Portal(bool forceLocal)
    {
      if(!forceLocal & _portalRemote)
      {
        // return remote instance
        if(_remotePortal == null)
          _remotePortal = (Server.DataPortal)Activator.GetObject(typeof(Server.DataPortal), PORTAL_SERVER);
        return _remotePortal;
      }
      else
      {
        // return local instance
        if(_portal == null)
          _portal = new Server.DataPortal();
        return _portal;
      }
    }

    private static Server.ServicedDataPortal.DataPortal 
      ServicedPortal(bool forceLocal)
    {
      if(!forceLocal & _portalRemote)
      {
        // return remote instance
        return 
          (Server.ServicedDataPortal.DataPortal)Activator.GetObject(
            typeof(Server.ServicedDataPortal.DataPortal), 
            SERVICED_PORTAL_SERVER);
      }
      else
      {
        // return local instance
        return new Server.ServicedDataPortal.DataPortal();
      }
    }

    static private string PORTAL_SERVER
    {
      get
      {
        return ConfigurationSettings.AppSettings["PortalServer"];
      }
    }

    static private string SERVICED_PORTAL_SERVER
    {
      get
      {
        return ConfigurationSettings.AppSettings["ServicedPortalServer"];
      }
    }

    #endregion

    #region Security

    static private string AUTHENTICATION
    {
      get
      {
        return ConfigurationSettings.AppSettings["Authentication"];
      }
    }

    static private string ALWAYS_IMPERSONATE
    {
      get
      {
        string tmp = ConfigurationSettings.AppSettings["AlwaysImpersonate"];
        if (tmp != null)
          return tmp.ToLower();
        else
          return "false";
      }
    }

    static private System.Security.Principal.IPrincipal GetPrincipal()
    {
      if(AUTHENTICATION == "Windows")
        // Windows integrated security 
        return null;
      else
        // we assume using the CSLA framework security
        return System.Threading.Thread.CurrentPrincipal;
    }

    #endregion

    #region Helper methods

    private static void RestoreContext(object result)
    {
      System.LocalDataStoreSlot slot = Thread.GetNamedDataSlot("CSLA.GlobalContext");
      Thread.SetData(slot, ((Server.DataPortalResult)result).GlobalContext);
    }

    static private bool IsTransactionalMethod(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(TransactionalAttribute));
    }

    static private bool RunLocal(MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(RunLocalAttribute));
    }

    static private MethodInfo GetMethod(Type objectType, string method)
    {
      return objectType.GetMethod(method, 
        BindingFlags.FlattenHierarchy | 
        BindingFlags.Instance |
        BindingFlags.Public | 
        BindingFlags.NonPublic);
    }

    static private Type GetObjectType(object criteria)
    {
      if(criteria.GetType().IsSubclassOf(typeof(CriteriaBase)))
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

    static DataPortal()
    {
      // see if we need to configure remoting at all
      if(PORTAL_SERVER.Length > 0 || SERVICED_PORTAL_SERVER.Length > 0)
      {
        _portalRemote = true;
        // create and register our custom HTTP channel
        // that uses the binary formatter
        Hashtable properties = new Hashtable();
        properties["name"] = "HttpBinary";
        if(AUTHENTICATION == "Windows" || ALWAYS_IMPERSONATE == "true")
        {
          // make sure we pass the user's Windows credentials
          // to the server
          properties["useDefaultCredentials"] = true;
        }

        BinaryClientFormatterSinkProvider formatter = 
                                        new BinaryClientFormatterSinkProvider();

        HttpChannel channel = new HttpChannel(properties, formatter, null);

        ChannelServices.RegisterChannel(channel);

//        // register the data portal types as being remote
//        if(PORTAL_SERVER.Length > 0)
//        {
//          RemotingConfiguration.RegisterWellKnownClientType(
//            typeof(Server.DataPortal), 
//            PORTAL_SERVER);
//        }
//        if(SERVICED_PORTAL_SERVER.Length > 0)
//        {
//          RemotingConfiguration.RegisterWellKnownClientType(
//            typeof(Server.ServicedDataPortal.DataPortal), 
//            SERVICED_PORTAL_SERVER);
//        }
      }

    }

    #endregion

  }
}
