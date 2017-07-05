//-----------------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Security.Principal;
using System.Collections.Specialized;
#if !(ANDROID || IOS) && !NETFX_CORE
using System.Configuration;
using System.Web;
#endif
using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public static class ApplicationContext
  { 
    #region Context Manager

    private static IContextManager _contextManager;
#if !(ANDROID || IOS) && !NETFX_CORE
    private static IContextManager _webContextManager;
    private static Type _webManagerType;
#endif

    static ApplicationContext()
    {
#if !(ANDROID || IOS) && !NETFX_CORE
      Type _contextManagerType = null;
      _webManagerType = Type.GetType("Csla.Web.ApplicationContextManager, Csla.Web");
      if (_contextManagerType == null)
        _contextManagerType = Type.GetType("Csla.Windows.ApplicationContextManager, Csla.Windows");
      if (_contextManagerType == null)
        _contextManagerType = Type.GetType("Csla.Xaml.ApplicationContextManager, Csla.Xaml");

      if (_webManagerType != null)
        WebContextManager = (IContextManager)Activator.CreateInstance(_webManagerType);
      if (_contextManagerType != null)
        _contextManager = (IContextManager)Activator.CreateInstance(_contextManagerType);
#endif
      if (_contextManager == null)
        _contextManager = new ApplicationContextManager();
    }

#if !(ANDROID || IOS) && !NETFX_CORE
    /// <summary>
    /// Gets or sets the web context manager.
    /// Will use default WebContextManager. 
    /// Only need to set for non-default WebContextManager.
    /// </summary>
    /// <value>
    /// The web context manager.
    /// </value>
    public static IContextManager WebContextManager
    {
      get { return _webContextManager; }
      set { _webContextManager = value; }
    }
#endif

    /// <summary>
    /// Gets the context manager responsible
    /// for storing user and context information for
    /// the application.
    /// </summary>
    /// <remarks>
    /// �f WebContextManager is not null and IsValid then WebContextManager is returned,
    /// else default ContextManager is returned. 
    /// This behaviour is to support background threads in web applications and return
    /// to use WebContextManager when completed. 
    /// </remarks>
    public static IContextManager ContextManager
    {
      get
      {
#if !(ANDROID || IOS) && !NETFX_CORE
        if (WebContextManager != null && WebContextManager.IsValid)
            return WebContextManager;
#endif
        return  _contextManager;
      }
      set { _contextManager = value; }
    }

    #endregion

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
      get { return ContextManager.GetUser(); }
      set { ContextManager.SetUser(value); }
    }

    #endregion

    #region LocalContext

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
        ContextDictionary ctx = ContextManager.GetLocalContext();
        if (ctx == null)
        {
          ctx = new ContextDictionary();
          ContextManager.SetLocalContext(ctx);
        }
        return ctx;
      }
    }

    #endregion

    #region Client/Global Context

    private static object _syncContext = new object();

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
        lock (_syncContext)
        {
          ContextDictionary ctx = ContextManager.GetClientContext();
          if (ctx == null)
          {
            ctx = new ContextDictionary();
            ContextManager.SetClientContext(ctx);
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
        ContextDictionary ctx = ContextManager.GetGlobalContext();
        if (ctx == null)
        {
          ctx = new ContextDictionary();
          ContextManager.SetGlobalContext(ctx);
        }
        return ctx;
      }
    }

    internal static void SetContext(
      ContextDictionary clientContext,
      ContextDictionary globalContext)
    {
      lock (_syncContext)
        ContextManager.SetClientContext(clientContext);
      ContextManager.SetGlobalContext(globalContext);
    }

    /// <summary>
    /// Clears all context collections.
    /// </summary>
    public static void Clear()
    {
      SetContext(null, null);
      ContextManager.SetLocalContext(null);
    }

    #endregion

    #region Settings

    private static Csla.Server.IDataPortalActivator _dataPortalActivator = null;
    private static object _dataPortalActivatorSync = new object();

    /// <summary>
    /// Gets or sets an instance of the IDataPortalActivator provider.
    /// </summary>
    public static Csla.Server.IDataPortalActivator DataPortalActivator
    {
      get
      {
        if (_dataPortalActivator == null)
        {
#if !(ANDROID || IOS) && !NETFX_CORE
          lock (_dataPortalActivatorSync)
          {
            if (_dataPortalActivator == null)
            {
              var typeName = ConfigurationManager.AppSettings["CslaDataPortalActivator"];
              if (!string.IsNullOrWhiteSpace(typeName))
              {
                var type = Type.GetType(typeName);
                _dataPortalActivator = (Csla.Server.IDataPortalActivator)Activator.CreateInstance(type);
              }
              else
              {
                _dataPortalActivator = new Csla.Server.DefaultDataPortalActivator();
              }
            }
          }
#else
        _dataPortalActivator = new Csla.Server.DefaultDataPortalActivator(); 
#endif
        }
        return _dataPortalActivator;
      }
      set
      {
        _dataPortalActivator = value;
      }
    }

    private static string _dataPortalUrl = null;

    /// <summary>
    /// Gets or sets the data portal URL string.
    /// If not set on Get will read CslaDataPortalUrl from config file. 
    /// </summary>
    /// <value>The data portal URL string.</value>
    public static string DataPortalUrlString
    {
      get
      {
#if !(ANDROID || IOS) && !NETFX_CORE
        if (_dataPortalUrl == null)
        {
          _dataPortalUrl = ConfigurationManager.AppSettings["CslaDataPortalUrl"];
        }
#endif
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
    /// </para>
    /// </remarks>
    public static string DataPortalProxyFactory
    {
      get
      {
        if (string.IsNullOrEmpty(_dataPortalProxyFactory))
        {
#if !(ANDROID || IOS) && !NETFX_CORE
          _dataPortalProxyFactory = ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"];
#endif
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
    /// custom security derived from CslaPrincipal.
    /// </remarks>
    public static string AuthenticationType
    {
      get
      {
#if !(ANDROID || IOS) && !NETFX_CORE
        if (_authenticationType == null)
          _authenticationType = ConfigurationManager.AppSettings["CslaAuthentication"];
#endif
        if (_authenticationType == null)
          _authenticationType = "Csla";
        return _authenticationType; 
      }
      set { _authenticationType = value; }
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
    /// </para>
    /// </remarks>
    public static string DataPortalProxy
    {
      get
      {
#if !(ANDROID || IOS) && !NETFX_CORE
        if (string.IsNullOrEmpty(_dataPortalProxy))
          _dataPortalProxy = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
#endif
        if (string.IsNullOrEmpty(_dataPortalProxy))
          _dataPortalProxy = "Local";
        return _dataPortalProxy;
      }
      set
      {
        _dataPortalProxy = value;
        DataPortal.ResetProxyType();
      }
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
        string result = null;
#if !(ANDROID || IOS) && !NETFX_CORE
        result = ConfigurationManager.AppSettings["CslaIsInRoleProvider"];
#endif
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
#if !(ANDROID || IOS) && !NETFX_CORE
        string setting = ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"];
        if (!string.IsNullOrEmpty(setting))
          result = bool.Parse(setting);
#endif
        return result;
      }
    }

    private static bool _dataPortalReturnObjectOnException = false;
    private static bool _dataPortalReturnObjectOnExceptionSet = false;

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException
    /// (default is false).
    /// </summary>
    public static bool DataPortalReturnObjectOnException
    {
      get
      {
        if (!_dataPortalReturnObjectOnExceptionSet)
        {
#if !(ANDROID || IOS) && !NETFX_CORE
          string setting = ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"];
          if (!string.IsNullOrEmpty(setting))
            DataPortalReturnObjectOnException = bool.Parse(setting);
#endif
          _dataPortalReturnObjectOnExceptionSet = true;
        }
        return _dataPortalReturnObjectOnException;
      }
      set
      {
        _dataPortalReturnObjectOnException = value;
        _dataPortalReturnObjectOnExceptionSet = true;
      }
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
      MobileClient
    }

    /// <summary>
    /// Gets the serialization formatter type used by CSLA .NET
    /// for all explicit object serialization (such as cloning,
    /// n-level undo, etc).
    /// </summary>
    public static SerializationFormatters SerializationFormatter
    {
      get
      {
#if !(ANDROID || IOS) && !NETFX_CORE
        string tmp = ConfigurationManager.AppSettings["CslaSerializationFormatter"];

        if (string.IsNullOrEmpty(tmp))
          tmp = "BinaryFormatter";

        SerializationFormatters serializationFormatter;
        if (Enum.TryParse(tmp, true, out serializationFormatter))
            return serializationFormatter;

        return SerializationFormatters.CustomFormatter;
#else
        return SerializationFormatters.MobileFormatter;
#endif
      }
    }

    /// <summary>
    /// Enum representing the serialization formatters
    /// supported by CSLA .NET.
    /// </summary>
    public enum SerializationFormatters
    {
#if !(ANDROID || IOS) && !NETFX_CORE
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
      NetDataContractSerializer,
      /// <summary>
      /// Use a custom formatter provided by type found
      /// at <appSetting key="CslaSerializationFormatter"></appSetting>
      /// </summary>
      CustomFormatter,
#endif
      /// <summary>
      /// Use the CSLA .NET MobileFormatter
      /// </summary>
      MobileFormatter
    }

    private static PropertyChangedModes _propertyChangedMode = PropertyChangedModes.Xaml;
#if !(ANDROID || IOS) && !NETFX_CORE
    private static bool _propertyChangedModeSet;
#endif
    /// <summary>
    /// Gets or sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    public static PropertyChangedModes PropertyChangedMode
    {
      get
      {
#if !(ANDROID || IOS) && !NETFX_CORE
        if (!_propertyChangedModeSet)
        {
          string tmp = ConfigurationManager.AppSettings["CslaPropertyChangedMode"];
          if (string.IsNullOrEmpty(tmp))
            tmp = "Xaml";
          _propertyChangedMode = (PropertyChangedModes)
            Enum.Parse(typeof(PropertyChangedModes), tmp);
          _propertyChangedModeSet = true;
        }
#endif
        return _propertyChangedMode;
      }
      set
      {
        _propertyChangedMode = value;
#if !(ANDROID || IOS) && !NETFX_CORE
        _propertyChangedModeSet = true;
#endif
      }
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

    private static ExecutionLocations _executionLocation =
#if (ANDROID || IOS) || NETFX_CORE
      ExecutionLocations.MobileClient;
#else
      ExecutionLocations.Client;
#endif

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

    /// <summary>
    /// The default RuleSet name 
    /// </summary>
    public const string DefaultRuleSet = "default";

    /// <summary>
    /// Gets or sets the RuleSet name to use for static HasPermission calls.
    /// </summary>
    /// <value>The rule set.</value>
    public static string RuleSet
    {
      get
      {
        var ruleSet = (string)ClientContext.GetValueOrNull("__ruleSet");
        return string.IsNullOrEmpty(ruleSet) ? ApplicationContext.DefaultRuleSet : ruleSet;
      }
      set
      {
        ApplicationContext.ClientContext["__ruleSet"] = value;
      }
    }

#if !(ANDROID || IOS) && !NETFX_CORE

    /// <summary>
    /// Gets the default transaction isolation level.
    /// </summary>
    /// <value>
    /// The default transaction isolation level.
    /// </value>
    public static TransactionIsolationLevel DefaultTransactionIsolationLevel
    {
      get
      {
        string tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"];
        if (string.IsNullOrEmpty(tmp))
        {
          return TransactionIsolationLevel.Unspecified;
        }
        return (TransactionIsolationLevel)Enum.Parse(typeof(TransactionIsolationLevel), tmp);
      }
    }

    /// <summary>
    /// Gets the default transaction timeout in seconds.
    /// </summary>
    /// <value>
    /// The default transaction timeout in seconds.
    /// </value>
    public static int DefaultTransactionTimeoutInSeconds
    {
      get
      {
        var tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"];
        return string.IsNullOrEmpty(tmp) ? 30 : int.Parse(tmp);
      }
    }

#endif

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
      /// The code is executing on the server.  This includes
      /// Local mode execution
      /// </summary>
      Server
    }

    /// <summary>
    /// Return Logical Execution Location - Client or Server
    /// This is applicable to Local mode as well
    /// </summary>
    public static LogicalExecutionLocations LogicalExecutionLocation
    {
      get
      {
        object location = LocalContext.GetValueOrNull("__logicalExecutionLocation");
        if (location != null)
          return (LogicalExecutionLocations)location;
        else
          return LogicalExecutionLocations.Client;
      }
    }

    /// <summary>
    /// Set logical execution location
    /// </summary>
    /// <param name="location">Location to set context to</param>
    internal static void SetLogicalExecutionLocation(LogicalExecutionLocations location)
    {
      LocalContext["__logicalExecutionLocation"] = location;
    }
    #endregion

    #region Default context manager

#if ((ANDROID || IOS) || NETFX_CORE) && !(ANDROID || IOS)
    private class ApplicationContextManager : IContextManager
    {
      private static ContextDictionary _localContext;
      private static ContextDictionary _clientContext;
      private static ContextDictionary _globalContext;
      private static IPrincipal _principal = new Csla.Security.UnauthenticatedPrincipal();

      public bool IsValid
      {
        get { return true; }
      }

      public IPrincipal GetUser()
      {
        return _principal;
      }

      public void SetUser(IPrincipal principal)
      {
        _principal = principal;
      }

      public ContextDictionary GetLocalContext()
      {
        return _localContext;
      }

      public void SetLocalContext(ContextDictionary localContext)
      {
        _localContext = localContext;
      }

      public ContextDictionary GetClientContext()
      {
        return _clientContext;
      }

      public void SetClientContext(ContextDictionary clientContext)
      {
        _clientContext = clientContext;
      }

      public ContextDictionary GetGlobalContext()
      {
        return _globalContext;
      }

      public void SetGlobalContext(ContextDictionary globalContext)
      {
        _globalContext = globalContext;
      }
    }
#else
    private class ApplicationContextManager : IContextManager
    {
      private const string _localContextName = "Csla.LocalContext";
      private const string _clientContextName = "Csla.ClientContext";
      private const string _globalContextName = "Csla.GlobalContext";

      public bool IsValid
      {
        get { return true; }
      }

      public IPrincipal GetUser()
      {
        IPrincipal current = Thread.CurrentPrincipal;
        return current;
      }

      public void SetUser(IPrincipal principal)
      {
        Thread.CurrentPrincipal = principal;
      }

      public ContextDictionary GetLocalContext()
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        return (ContextDictionary)Thread.GetData(slot);
      }

      public void SetLocalContext(ContextDictionary localContext)
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        Thread.SetData(slot, localContext);
      }

      public ContextDictionary GetClientContext()
      {
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
        {
          return (ContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
        }
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          return (ContextDictionary)Thread.GetData(slot);
        }
      }

      public void SetClientContext(ContextDictionary clientContext)
      {
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
        {
          AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
        }
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          Thread.SetData(slot, clientContext);
        }
      }

      public ContextDictionary GetGlobalContext()
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        return (ContextDictionary)Thread.GetData(slot);
      }

      public void SetGlobalContext(ContextDictionary globalContext)
      {
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        Thread.SetData(slot, globalContext);
      }
    }
#endif

    #endregion
  }
}