using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Configuration;

namespace CSLA
{
  /// <summary>
  /// This is the client-side DataPortal as described in
  /// Chapter 5.
  /// </summary>
  public class DataPortal
  {
    static Server.DataPortal _Portal;
    static Server.ServicedDataPortal.DataPortal _ServicedPortal;


#region Data Access methods

    /// <summary>
    /// Called by a factory method in a business class to create 
    /// a new object, which is loaded with default
    /// values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>A new object, populated with default values.</returns>
    static public Object Create(Object criteria)
    {
      if(IsTransactionalMethod(GetMethod(criteria.GetType().DeclaringType, "DataPortal_Create")))
        return ServicedPortal.Create(criteria, GetPrincipal());
      else
        return Portal.Create(criteria, GetPrincipal());
    }

    /// <summary>
    /// Called by a factory method in a business class to retrieve
    /// an object, which is loaded with values from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <returns>An object populated with values from the database.</returns>
    static public Object Fetch(Object criteria)
    {
      if(IsTransactionalMethod(GetMethod(criteria.GetType().DeclaringType, "DataPortal_Fetch")))
        return ServicedPortal.Fetch(criteria, GetPrincipal());
      else
        return Portal.Fetch(criteria, GetPrincipal());
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
    static public Object Update(Object obj)
    {
      if(IsTransactionalMethod(GetMethod(obj.GetType(), "DataPortal_Update")))
        return ServicedPortal.Update(obj, GetPrincipal());
      else
        return Portal.Update(obj, GetPrincipal());
    }

    /// <summary>
    /// Called by a <c>Shared</c> method in the business class to cause
    /// immediate deletion of a specific object from the database.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    static public void Delete(Object criteria)
    {
      if(IsTransactionalMethod(GetMethod(criteria.GetType().DeclaringType, "DataPortal_Delete")))
        ServicedPortal.Delete(criteria, GetPrincipal());
      else
        Portal.Delete(criteria, GetPrincipal());
    }

#endregion

#region Server-side DataPortal

    static private Server.DataPortal Portal
    {
      get
      {
        if(_Portal == null)
          _Portal = new Server.DataPortal();
        return _Portal;
      }
    }

    static private Server.ServicedDataPortal.DataPortal ServicedPortal
    {
      get
      {
        if(_ServicedPortal == null)
          _ServicedPortal = new Server.ServicedDataPortal.DataPortal();
        return _ServicedPortal;
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

    static private string AUTHENTICATION()
    {
      return ConfigurationSettings.AppSettings["Authentication"];
    }

    static private System.Security.Principal.IPrincipal GetPrincipal()
    {
      if(AUTHENTICATION() == "Windows")
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

    static private MethodInfo GetMethod(Type objectType, string method)
    {
      return objectType.GetMethod(method, 
        BindingFlags.FlattenHierarchy | 
        BindingFlags.Instance |
        BindingFlags.Public | 
        BindingFlags.NonPublic);
    }

    static DataPortal()
    {
      // see if we need to configure remoting at all
      if(PORTAL_SERVER.Length > 0 || SERVICED_PORTAL_SERVER.Length > 0)
      {
        // create and register our custom HTTP channel
        // that uses the binary formatter
        Hashtable properties = new Hashtable();
        properties["name"] = "HttpBinary";

        BinaryClientFormatterSinkProvider formatter = new BinaryClientFormatterSinkProvider();

        HttpChannel channel = new HttpChannel(properties, formatter, null);

        ChannelServices.RegisterChannel(channel);

        // register the data portal types as being remote
        if(PORTAL_SERVER.Length > 0)
        {
          RemotingConfiguration.RegisterWellKnownClientType(
            typeof(Server.DataPortal), 
            PORTAL_SERVER);
        }
        if(SERVICED_PORTAL_SERVER.Length > 0)
        {
          RemotingConfiguration.RegisterWellKnownClientType(
            typeof(Server.ServicedDataPortal.DataPortal), 
            SERVICED_PORTAL_SERVER);
        }
      }

    }

#endregion

  }
}
