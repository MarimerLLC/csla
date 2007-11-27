using System;
using System.Threading;
using System.Security.Principal;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;

namespace Csla
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public static class ApplicationContext
  {
    #region User

    /// <summary>
    /// Get or set the current <see cref="IPrincipal" />
    /// object representing the user's identity.
    /// </summary>
    /// <remarks>
    /// This is discussed in Chapter 5. When running
    /// under IIS the HttpContext.Current.User value
    /// is used, otherwise the current Thread.CurrentPrincipal
    /// value is used.
    /// </remarks>
    public static IPrincipal User
    {
      get
      {
        if (HttpContext.Current == null)
          return Thread.CurrentPrincipal;
        else
          return HttpContext.Current.User;
      }
      set
      {
        if (HttpContext.Current != null)
          HttpContext.Current.User = value;
        Thread.CurrentPrincipal = value;
      }
    }

    #endregion

    #region LocalContext

    private const string _localContextName = "Csla.LocalContext";

    /// <summary>
    /// Returns the application-specific context data that
    /// is local to the current AppDomain.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is NOT transferred to and from
    /// the client and server.
    /// </para>
    /// </remarks>
    public static HybridDictionary LocalContext
    {
      get
      {
        HybridDictionary ctx = GetLocalContext();
        if (ctx == null)
        {
          ctx = new HybridDictionary();
          SetLocalContext(ctx);
        }
        return ctx;
      }
    }

    private static HybridDictionary GetLocalContext()
    {
      if (HttpContext.Current == null)
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        return (HybridDictionary)Thread.GetData(slot);
      }
      else
        return (HybridDictionary)HttpContext.Current.Items[_localContextName];
    }

    private static void SetLocalContext(HybridDictionary localContext)
    {
      if (HttpContext.Current == null)
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        Thread.SetData(slot, localContext);
      }
      else
        HttpContext.Current.Items[_localContextName] = localContext;
    }

    #endregion

    #region Client/Global Context

    private static object _syncClientContext = new object();
    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

    /// <summary>
    /// Returns the application-specific context data provided
    /// by the client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred from
    /// the client to the server. No data is transferred from
    /// the server to the client.
    /// </para><para>
    /// This property is thread safe in a Windows client
    /// setting and on an application server. It is not guaranteed
    /// to be thread safe within the context of an ASP.NET
    /// client setting (i.e. in your ASP.NET UI).
    /// </para>
    /// </remarks>
    public static HybridDictionary ClientContext
    {
      get
      {
        lock (_syncClientContext)
        {
          HybridDictionary ctx = GetClientContext();
          if (ctx == null)
          {
            ctx = new HybridDictionary();
            SetClientContext(ctx);
          }
          return ctx;
        }
      }
    }

    /// <summary>
    /// Returns the application-specific context data shared
    /// on both client and server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a HybridDictionary. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred to and from
    /// the client and server. Any objects or data in this context
    /// will be transferred bi-directionally across the network.
    /// </para>
    /// </remarks>
    public static HybridDictionary GlobalContext
    {
      get
      {
        HybridDictionary ctx = GetGlobalContext();
        if (ctx == null)
        {
          ctx = new HybridDictionary();
          SetGlobalContext(ctx);
        }
        return ctx;
      }
    }

    internal static HybridDictionary GetClientContext()
    {
      if (HttpContext.Current == null)
      {
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
          lock (_syncClientContext)
            return (HybridDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
        else
        {
          LocalDataStoreSlot slot =
            Thread.GetNamedDataSlot(_clientContextName);
          return (HybridDictionary)Thread.GetData(slot);
        }
      }
      else
        return (HybridDictionary)
          HttpContext.Current.Items[_clientContextName];
    }

    internal static HybridDictionary GetGlobalContext()
    {
      if (HttpContext.Current == null)
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        return (HybridDictionary)Thread.GetData(slot);
      }
      else
        return (HybridDictionary)HttpContext.Current.Items[_globalContextName];
    }

    private static void SetClientContext(HybridDictionary clientContext)
    {
      if (HttpContext.Current == null)
      {
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
          lock (_syncClientContext)
            AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          Thread.SetData(slot, clientContext);
        }
      }
      else
        HttpContext.Current.Items[_clientContextName] = clientContext;
    }

    internal static void SetGlobalContext(HybridDictionary globalContext)
    {
      if (HttpContext.Current == null)
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        Thread.SetData(slot, globalContext);
      }
      else
        HttpContext.Current.Items[_globalContextName] = globalContext;
    }

    internal static void SetContext(
      HybridDictionary clientContext, 
      HybridDictionary globalContext)
    {
      SetClientContext(clientContext);
      SetGlobalContext(globalContext);
    }

    /// <summary>
    /// Clears all context collections.
    /// </summary>
    public static void Clear()
    {
      SetContext(null, null);
      SetLocalContext(null);
    }

    #endregion

    #region Config Settings

    /// <summary>
    /// Returns the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This value is read from the application configuration
    /// file with the key value "CslaAuthentication". The value
    /// "Windows" indicates CSLA .NET should use Windows integrated
    /// (or AD) security. Any other value indicates the use of
    /// custom security derived from BusinessPrincipalBase.
    /// </remarks>
    public static string AuthenticationType
    {
      get { return ConfigurationManager.AppSettings["CslaAuthentication"]; }
    }

    /// <summary>
    /// Returns the channel or network protocol
    /// for the DataPortal server.
    /// </summary>
    /// <value>Fully qualified assembly/type name of the proxy class.</value>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This value is read from the application configuration
    /// file with the key value "CslaDataPortalProxy". 
    /// </para><para>
    /// The proxy class must implement Csla.Server.IDataPortalServer.
    /// </para><para>
    /// The value "Local" is a shortcut to running the DataPortal
    /// "server" in the client process.
    /// </para><para>
    /// Other built-in values include:
    /// <list>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.RemotingProxy</term>
    /// <description>Use .NET Remoting to communicate with the server</description>
    /// </item>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.EnterpriseServicesProxy</term>
    /// <description>Use Enterprise Services (DCOM) to communicate with the server</description>
    /// </item>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.WebServicesProxy</term>
    /// <description>Use Web Services (asmx) to communicate with the server</description>
    /// </item>
    /// </list>
    /// Each proxy type does require that the DataPortal server be hosted using the appropriate
    /// technology. For instance, Web Services and Remoting should be hosted in IIS, while
    /// Enterprise Services must be hosted in COM+.
    /// </para>
    /// </remarks>
    public static string DataPortalProxy
    {
      get
      {
        string result = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
        if (string.IsNullOrEmpty(result))
          result = "Local";
        return result;
      }
    }

    /// <summary>
    /// Returns the URL for the DataPortal server.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This value is read from the application configuration
    /// file with the key value "CslaDataPortalUrl". 
    /// </remarks>
    public static Uri DataPortalUrl
    {
      get { return new Uri(ConfigurationManager.AppSettings["CslaDataPortalUrl"]); }
    }

    /// <summary>
    /// Gets a qualified name for a method that implements
    /// the IsInRole() behavior used for authorization.
    /// </summary>
    /// <returns>
    /// Returns a value in the form
    /// "Namespace.Class, Assembly, MethodName".
    /// </returns>
    /// <remarks>
    /// The default is to use a simple IsInRole() call against
    /// the current principal. If another method is supplied
    /// it must conform to the IsInRoleProvider delegate.
    /// </remarks>
    public static string IsInRoleProvider
    {
      get
      {
        string result = ConfigurationManager.AppSettings["CslaIsInRoleProvider"];
        if (result.Length == 0)
        {
          result = string.Empty;
        }
        return result;
      }
    }

    /// <summary>
    /// Gets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    public static bool AutoCloneOnUpdate
    {
      get
      {
        bool result = true;
        string setting = ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"];
        if (!string.IsNullOrEmpty(setting))
          result = bool.Parse(setting);
        return result;
      }
    }

#if NET20
    /// <summary>
    /// Gets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    /// <remarks>
    /// Always uses the standard Microsoft .NET 
    /// <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>.
    /// </remarks>
#else
    /// <summary>
    /// Gets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you use the DataContract and DataMember attributes
    /// to specify how your objects should be serialized then
    /// you <b>must</b> change this setting to use the
    /// <see cref="System.Runtime.Serialization.NetDataContractSerializer">
    /// NetDataContractSerializer</see> option. The default is to
    /// use the standard Microsoft .NET 
    /// <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>.
    /// </para>
    /// <para>
    /// This setting does <b>not affect</b> the serialization
    /// formatters used by the various data portal channels.
    /// </para>
    /// <para>
    /// If you are using the Remoting, Web Services or 
    /// Enterprise Services technologies, they will use the
    /// BinaryFormatter regardless of this setting, and will
    /// <b>fail to work</b> if you attempt to use the
    /// DataContract and DataMember attributes when building
    /// your business objects.
    /// </para>
    /// <para>
    /// If you want to use DataContract and DataMember, and
    /// you want a remote data portal server, you <b>must</b>
    /// use the WCF data portal channel, or create your own
    /// custom channel that uses the
    /// <see cref="System.Runtime.Serialization.NetDataContractSerializer">
    /// NetDataContractSerializer</see> provided as part of WCF.
    /// </para>
    /// </remarks>
#endif
    public static SerializationFormatters SerializationFormatter
    {
      get
      {
        string tmp = ConfigurationManager.AppSettings["CslaSerializationFormatter"];
        if (string.IsNullOrEmpty(tmp))
          tmp = "BinaryFormatter";
        return (SerializationFormatters)
          Enum.Parse(typeof(SerializationFormatters), tmp);
      }
    }

    /// <summary>
    /// Enum representing the serialization formatters
    /// supported by CSLA .NET.
    /// </summary>
    public enum SerializationFormatters
    {
      /// <summary>
      /// Use the standard Microsoft .NET
      /// <see cref="BinaryFormatter"/>.
      /// </summary>
      BinaryFormatter,
#if !NET20
      /// <summary>
      /// Use the Microsoft .NET 3.0
      /// <see cref="System.Runtime.Serialization.NetDataContractSerializer">
      /// NetDataContractSerializer</see> provided as part of WCF.
      /// </summary>
      NetDataContractSerializer
#endif
    }

    /// <summary>
    /// Enum representing the locations code can execute.
    /// </summary>
    public enum ExecutionLocations
    {
      /// <summary>
      /// The code is executing on the client.
      /// </summary>
      Client,
      /// <summary>
      /// The code is executing on the application server.
      /// </summary>
      Server
    }

    #endregion

    #region In-Memory Settings

    private static ExecutionLocations _executionLocation = 
      ExecutionLocations.Client;

    /// <summary>
    /// Returns a value indicating whether the application code
    /// is currently executing on the client or server.
    /// </summary>
    public static ExecutionLocations ExecutionLocation
    {
      get { return _executionLocation; }
    }

    internal static void SetExecutionLocation(ExecutionLocations location)
    {
      _executionLocation = location;
    }

    #endregion

  }
}