using System;
using System.Threading;
using System.Security.Principal;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public static class ApplicationContext
  {
    #region User

    private static IPrincipal _principal;

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
        IPrincipal current;
#if !CLIENTPROFILE
        if (HttpContext.Current != null)
          current = HttpContext.Current.User;
        else if (System.Windows.Application.Current != null)
        {
          if (_principal == null)
          {
            if (ApplicationContext.AuthenticationType != "Windows")
              _principal = new Csla.Security.UnauthenticatedPrincipal();
            else
              _principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
          }
          current = _principal;
        }
        else
          current = Thread.CurrentPrincipal;
#else
        if (System.Windows.Application.Current != null)
        {
          if (_principal == null)
          {
            if (ApplicationContext.AuthenticationType != "Windows")
              _principal = new Csla.Security.UnauthenticatedPrincipal();
            else
              _principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
          }
          current = _principal;
        }
        else
          current = Thread.CurrentPrincipal;
#endif
        return current;
      }
      set
      {
#if !CLIENTPROFILE
        if (HttpContext.Current != null)
          HttpContext.Current.User = value;
        else if (System.Windows.Application.Current != null)
          _principal = value;
        Thread.CurrentPrincipal = value;
#else
        if (System.Windows.Application.Current != null)
          _principal = value;
        Thread.CurrentPrincipal = value;
#endif
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
    public static ContextDictionary LocalContext
    {
      get
      {
        ContextDictionary ctx = GetLocalContext();
        if (ctx == null)
        {
          ctx = new ContextDictionary();
          SetLocalContext(ctx);
        }
        return ctx;
      }
    }

    private static ContextDictionary GetLocalContext()
    {
#if !CLIENTPROFILE
      if (HttpContext.Current == null)
      {
#endif
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        return (ContextDictionary)Thread.GetData(slot);
#if !CLIENTPROFILE
      }
      else
        return (ContextDictionary)HttpContext.Current.Items[_localContextName];
#endif
    }

    private static void SetLocalContext(ContextDictionary localContext)
    {
#if !CLIENTPROFILE
      if (HttpContext.Current == null)
      {
#endif
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        Thread.SetData(slot, localContext);
#if !CLIENTPROFILE
      }
      else
        HttpContext.Current.Items[_localContextName] = localContext;
#endif
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
    public static ContextDictionary ClientContext
    {
      get
      {
        lock (_syncClientContext)
        {
          ContextDictionary ctx = GetClientContext();
          if (ctx == null)
          {
            ctx = new ContextDictionary();
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
    public static ContextDictionary GlobalContext
    {
      get
      {
        ContextDictionary ctx = GetGlobalContext();
        if (ctx == null)
        {
          ctx = new ContextDictionary();
          SetGlobalContext(ctx);
        }
        return ctx;
      }
    }

    internal static ContextDictionary GetClientContext()
    {
#if !CLIENTPROFILE
      if (HttpContext.Current == null)
      {
#endif
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
          lock (_syncClientContext)
            return (ContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
        else
        {
          LocalDataStoreSlot slot =
            Thread.GetNamedDataSlot(_clientContextName);
          return (ContextDictionary)Thread.GetData(slot);
        }
#if !CLIENTPROFILE
    }
      else
        return (ContextDictionary)
          HttpContext.Current.Items[_clientContextName];
#endif
    }

    internal static ContextDictionary GetGlobalContext()
    {
#if !CLIENTPROFILE
      if (HttpContext.Current == null)
      {
#endif
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        return (ContextDictionary)Thread.GetData(slot);
#if !CLIENTPROFILE
}
      else
        return (ContextDictionary)HttpContext.Current.Items[_globalContextName];
#endif
    }

    private static void SetClientContext(ContextDictionary clientContext)
    {
#if !CLIENTPROFILE
      if (HttpContext.Current == null)
      {
#endif
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
          lock (_syncClientContext)
            AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          Thread.SetData(slot, clientContext);
        }
#if !CLIENTPROFILE
    }
      else
        HttpContext.Current.Items[_clientContextName] = clientContext;
#endif
    }

    internal static void SetGlobalContext(ContextDictionary globalContext)
    {
#if !CLIENTPROFILE
      if (HttpContext.Current == null)
      {
#endif
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        Thread.SetData(slot, globalContext);
#if !CLIENTPROFILE
      }
      else
        HttpContext.Current.Items[_globalContextName] = globalContext;
#endif
    }

    internal static void SetContext(
      ContextDictionary clientContext,
      ContextDictionary globalContext)
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

    private static string _authenticationType;

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
      get 
      {
        if (_authenticationType == null)
        {
          _authenticationType = ConfigurationManager.AppSettings["CslaAuthentication"];
          _authenticationType = _authenticationType ?? "Csla";
        }
        return _authenticationType;
      }
    }

    private static string _dataPortalProxy;

    /// <summary>
    /// Gets or sets the full type name (or 'Local') of
    /// the data portal proxy object to be used when
    /// communicating with the data portal server.
    /// </summary>
    /// <value>Fully qualified assembly/type name of the proxy class
    /// or 'Local'.</value>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// If this value is empty or null, a new value is read from the 
    /// application configuration file with the key value 
    /// "CslaDataPortalProxy".
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
        if (string.IsNullOrEmpty(_dataPortalProxy))
        {
          _dataPortalProxy = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
          if (string.IsNullOrEmpty(_dataPortalProxy))
            _dataPortalProxy = "Local";
        }
        return _dataPortalProxy;
      }
      set
      {
        _dataPortalProxy = value;
        DataPortal.ResetProxyType();
      }
    }

    private static string _dataPortalProxyFactory;
    ///<summary>
    /// Gets or sets the full type name (or 'Default') of
    /// the data portal proxy factory object to be used to get 
    /// the DataPortalProxy instance to use when
    /// communicating with the data portal server.
    /// </summary>
    /// <value>Fully qualified assembly/type name of the proxy factory class
    /// or 'Default'.</value>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// If this value is empty or null, a new value is read from the 
    /// application configuration file with the key value 
    /// "CslaDataPortalProxyFactory".
    /// </para><para>
    /// The proxy class must implement Csla.DataPortalClient.IDataPortalProxyFactory.
    /// </para><para>
    /// The value "Defaukt" is a shortcut for using the default 
    /// Csla.DataPortalClient.DefaultPortalProxyFactory  implementation.
    /// </para><para>
    /// <value>The data portal proxy factory fully qualified name.</value>
    public static string DataPortalProxyFactory
    {
      get
      {
        if (string.IsNullOrEmpty(_dataPortalProxyFactory))
        {
          _dataPortalProxy = ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"];
          if (string.IsNullOrEmpty(_dataPortalProxyFactory))
            _dataPortalProxyFactory = "Default";
        }
        return _dataPortalProxyFactory;
      }
      set
      {
        _dataPortalProxyFactory = value;
        DataPortal.ResetProxyFactory();
      }
    }


    private static string _dataPortalUrl = null;

    public static string DataPortalUrlString
    {
      get { 
        if (_dataPortalUrl == null) 
        {
          _dataPortalUrl =  ConfigurationManager.AppSettings["CslaDataPortalUrl"];
        }
        return _dataPortalUrl;
      }
      set
      {
        _dataPortalUrl = value;
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
      get { return new Uri(DataPortalUrlString); }
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
        if (string.IsNullOrEmpty(result))
          result = string.Empty;
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

    private static PropertyChangedModes _propertyChangedMode;
    private static bool _propertyChangedModeSet;
    /// <summary>
    /// Gets or sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    public static PropertyChangedModes PropertyChangedMode
    {
      get
      {
        if (!_propertyChangedModeSet)
        {
          string tmp = ConfigurationManager.AppSettings["CslaPropertyChangedMode"];
          if (string.IsNullOrEmpty(tmp))
            tmp = "Xaml";
          _propertyChangedMode = (PropertyChangedModes)
            Enum.Parse(typeof(PropertyChangedModes), tmp);
          _propertyChangedModeSet = true;
        }
        return _propertyChangedMode;
      }
      set
      {
        _propertyChangedMode = value;
        _propertyChangedModeSet = true;
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
      /// <summary>
      /// Use the Microsoft .NET 3.0
      /// <see cref="System.Runtime.Serialization.NetDataContractSerializer">
      /// NetDataContractSerializer</see> provided as part of WCF.
      /// </summary>
      NetDataContractSerializer
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
      Server,
      /// <summary>
      /// The code is executing on the Silverlight client.
      /// </summary>
      Silverlight
    }

    /// <summary>
    /// Enum representing the way in which CSLA .NET
    /// should raise PropertyChanged events.
    /// </summary>
    public enum PropertyChangedModes
    {
      /// <summary>
      /// Raise PropertyChanged events as required
      /// by Windows Forms data binding.
      /// </summary>
      Windows,
      /// <summary>
      /// Raise PropertyChanged events as required
      /// by XAML data binding in WPF.
      /// </summary>
      Xaml
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

    #region Logical Execution Location

    /// <summary>
    /// Enum representing the logical execution location
    /// The setting is set to server when server is execting
    /// a CRUD opertion, otherwise the setting is always client
    /// </summary>
    public enum LogicalExecutionLocations
    {
      /// <summary>
      /// The code is executing on the client.
      /// </summary>
      Client,
      /// <summary>
      /// The code is executing on the server.  This inlcudes
      /// Local mode execution
      /// </summary>
      Server
    }
    
    /// <summary>
    /// Gets a value indicating the logical execution location
    /// of the currently executing code.
    /// </summary>
    public static LogicalExecutionLocations LogicalExecutionLocation
    {
      get 
      {
        if (LocalContext.Contains("__logicalExecutionLocation"))
          return (LogicalExecutionLocations)LocalContext["__logicalExecutionLocation"];
        else
          return LogicalExecutionLocations.Client;
      }
    }
    
    internal static void SetLogicalExecutionLocation(LogicalExecutionLocations location)
    {
      LocalContext["__logicalExecutionLocation"] = location;
    }

    #endregion

  }
}