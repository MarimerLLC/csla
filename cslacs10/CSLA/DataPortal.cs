using System;
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
      object obj;
      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Create");

      bool forceLocal = RunLocal(method);

      if(IsTransactionalMethod(method))
        obj = ServicedPortal(forceLocal).Create(
          criteria, new Server.DataPortalContext(GetPrincipal(), _portalRemote & !forceLocal));
      else
        obj = Portal(forceLocal).Create(
          criteria, new Server.DataPortalContext(GetPrincipal(), _portalRemote & !forceLocal));

      if(_portalRemote & !forceLocal)
        Serialization.SerializationNotification.OnDeserialized(obj);
      return obj;
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    static public object Fetch(object criteria)
    {
      object obj;
      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Fetch");

      bool forceLocal = RunLocal(method);

      if(IsTransactionalMethod(method))
        obj = ServicedPortal(forceLocal).Fetch(
          criteria, new Server.DataPortalContext(GetPrincipal(), _portalRemote & !forceLocal));
      else
        obj = Portal(forceLocal).Fetch(
          criteria, new Server.DataPortalContext(GetPrincipal(), _portalRemote & !forceLocal));

      if(_portalRemote & !forceLocal)
        Serialization.SerializationNotification.OnDeserialized(obj);
      return obj;
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
      object updated;
      MethodInfo method = GetMethod(obj.GetType(), "DataPortal_Update");

      bool forceLocal = RunLocal(method);

      if(_portalRemote & !forceLocal)
        Serialization.SerializationNotification.OnSerializing(obj);

      if(IsTransactionalMethod(method))
        updated = ServicedPortal(forceLocal).Update(
          obj, new Server.DataPortalContext(GetPrincipal(), _portalRemote));
      else
        updated = Portal(forceLocal).Update(
          obj, new Server.DataPortalContext(GetPrincipal(), _portalRemote));

      if(_portalRemote & !forceLocal)
      {
        Serialization.SerializationNotification.OnSerialized(obj);
        Serialization.SerializationNotification.OnDeserialized(updated);
      }
      return updated;
    }

    /// <summary>
    /// Called by a <c>Shared</c> method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    static public void Delete(object criteria)
    {
      MethodInfo method = GetMethod(GetObjectType(criteria), "DataPortal_Delete");

      bool forceLocal = RunLocal(method);

      if(IsTransactionalMethod(method))
        ServicedPortal(forceLocal).Delete(
          criteria, new Server.DataPortalContext(GetPrincipal(), _portalRemote & !forceLocal));
      else
        Portal(forceLocal).Delete(
          criteria, new Server.DataPortalContext(GetPrincipal(), _portalRemote & !forceLocal));
    }

    #endregion

    #region Server-side DataPortal

    static Server.DataPortal _portal;
    static Server.ServicedDataPortal.DataPortal _servicedPortal;
    static Server.DataPortal _remotePortal;
    static Server.ServicedDataPortal.DataPortal _remoteServicedPortal;

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

    private static Server.ServicedDataPortal.DataPortal ServicedPortal(bool forceLocal)
    {
      if(!forceLocal & _portalRemote)
      {
        // return remote instance
        if(_remoteServicedPortal == null)
          _remoteServicedPortal = (Server.ServicedDataPortal.DataPortal)Activator.GetObject(
            typeof(Server.ServicedDataPortal.DataPortal), SERVICED_PORTAL_SERVER);
        return _remoteServicedPortal;
      }
      else
      {
        // return local instance
        if(_servicedPortal == null)
          _servicedPortal = new Server.ServicedDataPortal.DataPortal();
        return _servicedPortal;
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
        if(AUTHENTICATION == "Windows")
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
