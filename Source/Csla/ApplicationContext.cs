//-----------------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using Csla.Core;
using Csla.Configuration;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Csla
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public class ApplicationContext
  {
    /// <summary>
    /// Creates a new instance of the type
    /// </summary>
    /// <param name="contextManager">IContextManager used to manage per-user context</param>
    /// <param name="serviceProvider">Current service provider</param>
    public ApplicationContext(IContextManager contextManager, IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
      ContextManager = contextManager;
    }

    /// <summary>
    /// Gets or sets the context manager responsible
    /// for storing user and context information for
    /// the application.
    /// </summary>
    /// <remarks>
    /// ?f WebContextManager is not null and IsValid then WebContextManager is returned,
    /// else default ContextManager is returned. 
    /// This behaviour is to support background threads in web applications and return
    /// to use WebContextManager when completed. 
    /// </remarks>
    public IContextManager ContextManager { get; private set; }

    internal static void SettingsChanged()
    {
      _dataPortalReturnObjectOnExceptionSet = false;
      _propertyChangedModeSet = false;
      _transactionIsolationLevelSet = false;
      _defaultTransactionTimeoutInSecondsSet = false;
      _authenticationTypeName = null;
      _dataPortalActivator = null;
      //_dataPortalActivator = null;
      _dataPortalUrl = null;
      _dataPortalProxyFactory = null;
      _dataPortalProxy = null;
      _VersionRoutingTag = null;
    }

    /// <summary>
    /// Get or set the current ClaimsPrincipal
    /// object representing the user's identity.
    /// </summary>
    public ClaimsPrincipal Principal
    {
      get { return (ClaimsPrincipal)ContextManager.GetUser(); }
      set { ContextManager.SetUser(value); }
    }

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
    public IPrincipal User
    {
      get { return ContextManager.GetUser(); }
      set { ContextManager.SetUser(value); }
    }

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
    public ContextDictionary LocalContext
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

    private readonly object _syncContext = new object();

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
    public ContextDictionary ClientContext
    {
      get
      {
        lock (_syncContext)
        {
          ContextDictionary ctx = ContextManager.GetClientContext(ExecutionLocation);
          if (ctx == null)
          {
            ctx = new ContextDictionary();
            ContextManager.SetClientContext(ctx, ExecutionLocation);
          }
          return ctx;
        }
      }
    }

    internal void SetContext(ContextDictionary clientContext)
    {
      lock (_syncContext)
        ContextManager.SetClientContext(clientContext, ExecutionLocation);
    }

    /// <summary>
    /// Clears all context collections.
    /// </summary>
    public void Clear()
    {
      SetContext(null);
      ContextManager.SetLocalContext(null);
    }

    #region Settings

    /// <summary>
    /// Gets or sets a value indicating whether the app
    /// should be considered "offline".
    /// </summary>
    /// <remarks>
    /// If this value is true then the client-side data 
    /// portal will direct all calls to the local
    /// data portal. No calls will flow to remote
    /// data portal endpoints.
    /// </remarks>
    public bool IsOffline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    public static bool UseReflectionFallback { get; set; } = false;

    private static Csla.Server.IDataPortalActivator _dataPortalActivator = null;
    private static readonly object _dataPortalActivatorSync = new object();

    /// <summary>
    /// Gets the DataPortalActivator type from configuration.
    /// </summary>
    public static Type DataPortalActivatorType
    {
      get
      {
        var typeName = ConfigurationManager.AppSettings["CslaDataPortalActivator"];
        if (!string.IsNullOrWhiteSpace(typeName))
        {
          return Type.GetType(typeName);
        }
        else
        {
          return typeof(Csla.Server.DefaultDataPortalActivator);
        }
      }
    }

    /// <summary>
    /// Gets or sets an instance of the IDataPortalActivator provider.
    /// </summary>
    public static Csla.Server.IDataPortalActivator DataPortalActivator
    {
      get
      {
        if (_dataPortalActivator == null)
        {
          lock (_dataPortalActivatorSync)
          {
            if (_dataPortalActivator == null)
              _dataPortalActivator = (Csla.Server.IDataPortalActivator)Activator.CreateInstance(DataPortalActivatorType);
          }
        }
        return _dataPortalActivator;
      }
      set
      {
        _dataPortalActivator = value;
      }
    }
    //private static Csla.Server.IDataPortalActivator _dataPortalActivator = null;
    //private static readonly object _dataPortalActivatorSync = new object();

    ///// <summary>
    ///// Gets the DataPortalActivator type from configuration.
    ///// </summary>
    //public static Type DataPortalActivatorType
    //{
    //  get
    //  {
    //    var typeName = ConfigurationManager.AppSettings["CslaDataPortalActivator"];
    //    if (!string.IsNullOrWhiteSpace(typeName))
    //    {
    //      return Type.GetType(typeName);
    //    }
    //    else
    //    {
    //      return typeof(Csla.Server.DefaultDataPortalActivator);
    //    }
    //  }
    //}

    ///// <summary>
    ///// Gets or sets an instance of the IDataPortalActivator provider.
    ///// </summary>
    //public static Csla.Server.IDataPortalActivator DataPortalActivator
    //{
    //  get
    //  {
    //    if (_dataPortalActivator == null)
    //    {
    //      lock (_dataPortalActivatorSync)
    //      {
    //        if (_dataPortalActivator == null)
    //          _dataPortalActivator = (Csla.Server.IDataPortalActivator)CreateInstance(DataPortalActivatorType);
    //      }
    //    }
    //    return _dataPortalActivator;
    //  }
    //  set
    //  {
    //    _dataPortalActivator = value;
    //  }
    //}

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
        if (_dataPortalUrl == null)
        {
          _dataPortalUrl = ConfigurationManager.AppSettings["CslaDataPortalUrl"];
        }
        return _dataPortalUrl;
      }
      set
      {
        _dataPortalUrl = value;
      }
    }

    private static string _VersionRoutingTag = null;

    /// <summary>
    /// Gets or sets a value representing the application version
    /// for use in server-side data portal routing.
    /// </summary>
    public static string VersionRoutingTag
    {
      get
      {
        if (string.IsNullOrWhiteSpace(_VersionRoutingTag))
          _VersionRoutingTag = ConfigurationManager.AppSettings["CslaVersionRoutingTag"];
        return _VersionRoutingTag;
      }
      internal set
      {
        if (!string.IsNullOrWhiteSpace(value))
          if (value.Contains("-") || value.Contains("/"))
            throw new ArgumentException("valueRoutingToken");
        _VersionRoutingTag = value;
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
    public Uri DataPortalUrl
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
    /// The value "Default" is a shortcut for using the default 
    /// Csla.DataPortalClient.DefaultPortalProxyFactory  implementation.
    /// </para>
    /// </remarks>
    public static string DataPortalProxyFactory
    {
      get
      {
        if (string.IsNullOrEmpty(_dataPortalProxyFactory))
        {
          _dataPortalProxyFactory = ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"];
          if (string.IsNullOrEmpty(_dataPortalProxyFactory))
            _dataPortalProxyFactory = "Default";
        }
        return _dataPortalProxyFactory;
      }
      set
      {
        _dataPortalProxyFactory = value;
      }
    }

    private static string _authenticationTypeName;

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
        if (string.IsNullOrWhiteSpace(_authenticationTypeName))
          _authenticationTypeName = ConfigurationManager.AppSettings["CslaAuthentication"];
        if (string.IsNullOrWhiteSpace(_authenticationTypeName))
          _authenticationTypeName = "Csla";
        return _authenticationTypeName;
      }
      set { _authenticationTypeName = value; }
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
        if (string.IsNullOrEmpty(_dataPortalProxy))
          _dataPortalProxy = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
        if (string.IsNullOrEmpty(_dataPortalProxy))
          _dataPortalProxy = "Local";
        return _dataPortalProxy;
      }
      set
      {
        _dataPortalProxy = value;
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
          string setting = ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"];
          if (!string.IsNullOrEmpty(setting))
            DataPortalReturnObjectOnException = bool.Parse(setting);
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
      Server
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
        var result = SerializationFormatters.CustomFormatter;

        string tmp = ConfigurationManager.AppSettings["CslaSerializationFormatter"];
        if (string.IsNullOrWhiteSpace(tmp))
#if NETSTANDARD2_0 || NET5_0 || NET6_0
          tmp = "MobileFormatter";
#else
          tmp = "BinaryFormatter";
#endif
        if (Enum.TryParse(tmp, true, out SerializationFormatters serializationFormatter))
          result = serializationFormatter;

        return result;
      }
    }

    /// <summary>
    /// Enum representing the serialization formatters
    /// supported by CSLA .NET.
    /// </summary>
    public enum SerializationFormatters
    {
#if !NETSTANDARD2_0 && !NET5_0 && !NET6_0
      /// <summary>
      /// Use the Microsoft .NET 3.0
      /// <see cref="System.Runtime.Serialization.NetDataContractSerializer">
      /// NetDataContractSerializer</see> provided as part of WCF.
      /// </summary>
      NetDataContractSerializer,
#endif
      /// <summary>
      /// Use the standard Microsoft .NET
      /// <see cref="BinaryFormatter"/>.
      /// </summary>
      BinaryFormatter,
      /// <summary>
      /// Use a custom formatter provided by type found
      /// at <appSetting key="CslaSerializationFormatter"></appSetting>
      /// </summary>
      CustomFormatter,
      /// <summary>
      /// Use the CSLA .NET MobileFormatter
      /// </summary>
      MobileFormatter
    }

    private static PropertyChangedModes _propertyChangedMode = PropertyChangedModes.Xaml;
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

    private ExecutionLocations _executionLocation =
#if (ANDROID || IOS || NETFX_CORE) && !NETSTANDARD
      ExecutionLocations.MobileClient;
#else
      ExecutionLocations.Client;
#endif

    /// <summary>
    /// Returns a value indicating whether the application code
    /// is currently executing on the client or server.
    /// </summary>
    public ExecutionLocations ExecutionLocation
    {
      get { return _executionLocation; }
    }

    internal void SetExecutionLocation(ExecutionLocations location)
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
    public string RuleSet
    {
      get
      {
        var ruleSet = (string)ClientContext.GetValueOrNull("__ruleSet");
        return string.IsNullOrEmpty(ruleSet) ? ApplicationContext.DefaultRuleSet : ruleSet;
      }
      set
      {
        ClientContext["__ruleSet"] = value;
      }
    }

    private static TransactionIsolationLevel _transactionIsolationLevel = TransactionIsolationLevel.Unspecified;
    private static bool _transactionIsolationLevelSet = false;

    /// <summary>
    /// Gets or sets the default transaction isolation level.
    /// </summary>
    /// <value>
    /// The default transaction isolation level.
    /// </value>
    public static TransactionIsolationLevel DefaultTransactionIsolationLevel
    {
      get
      {
        if (!_transactionIsolationLevelSet)
        {
          string tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"];
          if (!string.IsNullOrEmpty(tmp))
          {
            _transactionIsolationLevel = (TransactionIsolationLevel)Enum.Parse(typeof(TransactionIsolationLevel), tmp);
          }
          _transactionIsolationLevelSet = true;
        }
        return _transactionIsolationLevel;
      }
      set
      {
        _transactionIsolationLevel = value;
        _transactionIsolationLevelSet = true;
      }
    }

    private static int _defaultTransactionTimeoutInSeconds = 600;
    private static bool _defaultTransactionTimeoutInSecondsSet = false;

    /// <summary>
    /// Gets or sets the default transaction timeout in seconds.
    /// </summary>
    /// <value>
    /// The default transaction timeout in seconds.
    /// </value>
    public static int DefaultTransactionTimeoutInSeconds
    {
      get
      {
        if (!_defaultTransactionTimeoutInSecondsSet)
        {
          var tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"];
          _defaultTransactionTimeoutInSeconds = string.IsNullOrEmpty(tmp) ? 30 : int.Parse(tmp);
          _defaultTransactionTimeoutInSecondsSet = true;
        }
        return _defaultTransactionTimeoutInSeconds;
      }
      set
      {
        _defaultTransactionTimeoutInSeconds = value;
        _defaultTransactionTimeoutInSecondsSet = true;
      }
    }

    private System.Transactions.TransactionScopeAsyncFlowOption _defaultTransactionAsyncFlowOption;
    private bool _defaultTransactionAsyncFlowOptionSet;

    /// <summary>
    /// Gets or sets the default transaction async flow option
    /// used to create new TransactionScope objects.
    /// </summary>
    public System.Transactions.TransactionScopeAsyncFlowOption DefaultTransactionAsyncFlowOption
    {
      get
      {
        if (!_defaultTransactionAsyncFlowOptionSet)
        {
          _defaultTransactionAsyncFlowOptionSet = true;
          var tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionAsyncFlowOption"];
          if (!Enum.TryParse<System.Transactions.TransactionScopeAsyncFlowOption>(tmp, out _defaultTransactionAsyncFlowOption))
            _defaultTransactionAsyncFlowOption = System.Transactions.TransactionScopeAsyncFlowOption.Suppress;
        }
        return _defaultTransactionAsyncFlowOption;
      }
      set
      {
        _defaultTransactionAsyncFlowOption = value;
        _defaultTransactionAsyncFlowOptionSet = true;
      }
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
      /// The code is executing on the server.  This includes
      /// Local mode execution
      /// </summary>
      Server
    }

    /// <summary>
    /// Return Logical Execution Location - Client or Server
    /// This is applicable to Local mode as well
    /// </summary>
    public LogicalExecutionLocations LogicalExecutionLocation
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
    internal void SetLogicalExecutionLocation(LogicalExecutionLocations location)
    {
      LocalContext["__logicalExecutionLocation"] = location;
    }
    #endregion

    private IServiceProvider _serviceProvider;

    /// <summary>
    /// Sets the service provider scope for this application context.
    /// </summary>
    internal IServiceProvider CurrentServiceProvider
    {
      get => _serviceProvider;
      set => _serviceProvider = value;
    }

    /// <summary>
    /// Uses reflection to create an object using its 
    /// default constructor.
    /// </summary>
    /// <typeparam name="T">Type of object to create.</typeparam>
    public T CreateInstance<T>()
    {
      return (T)CreateInstance(typeof(T));
    }

    /// <summary>
    /// Uses reflection to create an object using its 
    /// default constructor.
    /// </summary>
    /// <param name="objectType">Type of object to create.</param>
    public object CreateInstance(Type objectType)
    {
      object result;
      if (CurrentServiceProvider != null)
        result = ActivatorUtilities.CreateInstance(CurrentServiceProvider, objectType);
      else
        result = Activator.CreateInstance(objectType);
      if (result is IUseApplicationContext tmp)
      {
        tmp.ApplicationContext = this;
      }
      return result;
    }

    /// <summary>
    /// Creates an object using dependency injection, falling back
    /// to Activator if no service provider is available.
    /// </summary>
    /// <typeparam name="T">Type of object to create.</typeparam>
    /// <param name="parameters">Parameters for constructor</param>
    internal T CreateInstance<T>(params object[] parameters)
    {
      return (T)CreateInstance(typeof(T), parameters);
    }

    /// <summary>
    /// Creates an object using dependency injection, falling back
    /// to Activator if no service provider is available.
    /// </summary>
    /// <param name="objectType">Type of object to create</param>
    /// <param name="parameters">Parameters for constructor</param>
    internal object CreateInstance(Type objectType, params object[] parameters)
    {
      object result;
      if (CurrentServiceProvider != null)
        result = ActivatorUtilities.CreateInstance(CurrentServiceProvider, objectType, parameters);
      else
        result = Activator.CreateInstance(objectType, parameters);
      if (result is IUseApplicationContext tmp)
      {
        tmp.ApplicationContext = this;
      }
      return result;
    }

    /// <summary>
    /// Creates an instance of a generic type
    /// using its default constructor.
    /// </summary>
    /// <param name="type">Generic type to create</param>
    /// <param name="paramTypes">Type parameters</param>
    /// <returns></returns>
    internal object CreateGenericInstance(Type type, params Type[] paramTypes)
    {
      var genericType = type.GetGenericTypeDefinition();
      var gt = genericType.MakeGenericType(paramTypes);
      return CreateInstance(gt);
    }

  }
}
