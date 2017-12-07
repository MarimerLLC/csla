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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE 
    private static IContextManager _webContextManager;
#endif
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
    private static Type _webManagerType;
#endif

    static ApplicationContext()
    {
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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

#if !ANDROID && !IOS && !NETFX_CORE 
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
    /// Ïf WebContextManager is not null and IsValid then WebContextManager is returned,
    /// else default ContextManager is returned. 
    /// This behaviour is to support background threads in web applications and return
    /// to use WebContextManager when completed. 
    /// </remarks>
    public static IContextManager ContextManager
    {
      get
      {
#if !ANDROID && !IOS && !NETFX_CORE 
        if (WebContextManager != null && WebContextManager.IsValid)
            return WebContextManager;
#endif
        return _contextManager;
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if !ANDROID && !IOS && !NETFX_CORE && !NETSTANDARD2_0
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
#if (ANDROID || IOS || NETFX_CORE) && !NETSTANDARD
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

#if !ANDROID && !IOS && !NETFX_CORE 

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
#if !NETSTANDARD2_0
        if (!_transactionIsolationLevelSet)
        {
          string tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"];
          if (!string.IsNullOrEmpty(tmp))
          {
            _transactionIsolationLevel = (TransactionIsolationLevel)Enum.Parse(typeof(TransactionIsolationLevel), tmp);
          }
          _transactionIsolationLevelSet = true;
        }
#endif
        return _transactionIsolationLevel;
      }
      set
      {
        _transactionIsolationLevel = value;
        _transactionIsolationLevelSet = true;
      }
    }

    private static int _defaultTransactionTimeoutInSeconds = 30;
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
#if !NETSTANDARD2_0
        if (!_defaultTransactionTimeoutInSecondsSet)
        {
          var tmp = ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"];
          _defaultTransactionTimeoutInSeconds = string.IsNullOrEmpty(tmp) ? 30 : int.Parse(tmp);
          _defaultTransactionTimeoutInSecondsSet = true;
        }
#endif
        return _defaultTransactionTimeoutInSeconds;
      }
      set
      {
        _defaultTransactionTimeoutInSeconds = value;
        _defaultTransactionTimeoutInSecondsSet = true;
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

    /// <summary>
    /// Default context manager for the user property
    /// and local/client/global context dictionaries.
    /// </summary>
    public class ApplicationContextManager : IContextManager
    {
#if NETSTANDARD1_5 || NETSTANDARD1_6 || WINDOWS_UWP
      private AsyncLocal<IPrincipal> _user = new AsyncLocal<IPrincipal>() { Value = new Csla.Security.UnauthenticatedPrincipal() };
      private static ContextDictionary _globalContext;
#endif
#if NET40 || NET45 || PCL46 || PCL259
      private const string _localContextName = "Csla.LocalContext";
      private const string _clientContextName = "Csla.ClientContext";
#else
      private AsyncLocal<ContextDictionary> _localContext = new AsyncLocal<ContextDictionary>();
      private AsyncLocal<ContextDictionary> _clientContext = new AsyncLocal<ContextDictionary>();
#endif
      private const string _globalContextName = "Csla.GlobalContext";

      /// <summary>
      /// Returns a value indicating whether the context is valid.
      /// </summary>
      public bool IsValid
      {
        get { return true; }
      }

      /// <summary>
      /// Gets the current user principal.
      /// </summary>
      /// <returns>The current user principal</returns>
      public virtual IPrincipal GetUser()
      {
#if NETSTANDARD1_5 || NETSTANDARD1_6 || WINDOWS_UWP
        return _user.Value;
#elif PCL46 || PCL259
        throw new NotSupportedException("PCL.GetUser");
#else
        return Thread.CurrentPrincipal;
#endif
      }

      /// <summary>
      /// Sets teh current user principal.
      /// </summary>
      /// <param name="principal">User principal value</param>
      public virtual void SetUser(IPrincipal principal)
      {
#if NETSTANDARD1_5 || NETSTANDARD1_6 || WINDOWS_UWP
        _user.Value = principal;
#elif PCL46 || PCL259
        throw new NotSupportedException("PCL.SetUser");
#else
        Thread.CurrentPrincipal = principal;
#endif
      }

      /// <summary>
      /// Gets the local context dictionary.
      /// </summary>
      public ContextDictionary GetLocalContext()
      {
#if NET40 || NET45
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        return (ContextDictionary)Thread.GetData(slot);
#elif PCL46 || PCL259
        throw new NotSupportedException("PCL.GetLocalContext");
#else
        return _localContext.Value;
#endif
      }

      /// <summary>
      /// Sets the local context dictionary.
      /// </summary>
      /// <param name="localContext">Context dictionary</param>
      public void SetLocalContext(ContextDictionary localContext)
      {
#if NET40 || NET45
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_localContextName);
        Thread.SetData(slot, localContext);
#elif PCL46 || PCL259
        throw new NotSupportedException("PCL.SetLocalContext");
#else
        _localContext.Value = localContext;
#endif
      }

      /// <summary>
      /// Gets the client context dictionary.
      /// </summary>
      public ContextDictionary GetClientContext()
      {
#if NET40 || NET45 
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
        {
          return (ContextDictionary)AppDomain.CurrentDomain.GetData(_clientContextName);
        }
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          return (ContextDictionary)Thread.GetData(slot);
        }
#elif PCL46 || PCL259
        throw new NotSupportedException("PCL.GetClientContext");
#else
        return _clientContext.Value;
#endif
      }

      /// <summary>
      /// Sets the client context dictionary.
      /// </summary>
      /// <param name="clientContext">Context dictionary</param>
      public void SetClientContext(ContextDictionary clientContext)
      {
#if NET40 || NET45 
        if (ApplicationContext.ExecutionLocation == ExecutionLocations.Client)
        {
          AppDomain.CurrentDomain.SetData(_clientContextName, clientContext);
        }
        else
        {
          LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_clientContextName);
          Thread.SetData(slot, clientContext);
        }
#elif PCL46 || PCL259
        throw new NotSupportedException("PCL.SetClientContext");
#else
        _clientContext.Value = clientContext;
#endif
      }

      /// <summary>
      /// Gets the global context dictionary.
      /// </summary>
      public ContextDictionary GetGlobalContext()
      {
#if PCL46 || PCL259
        throw new NotSupportedException("PCL.GetGlobalContext");
#elif NETSTANDARD1_5 || NETSTANDARD1_6 || WINDOWS_UWP
        return _globalContext;
#else
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        return (ContextDictionary)Thread.GetData(slot);
#endif
      }

      /// <summary>
      /// Sets the global context dictionary.
      /// </summary>
      /// <param name="globalContext">Context dictionary</param>
      public void SetGlobalContext(ContextDictionary globalContext)
      {
#if PCL46 || PCL259
        throw new NotSupportedException("PCL.SetGlobalContext");
#elif NETSTANDARD1_5 || NETSTANDARD1_6 || WINDOWS_UWP
        _globalContext = globalContext;
#else
        LocalDataStoreSlot slot = Thread.GetNamedDataSlot(_globalContextName);
        Thread.SetData(slot, globalContext);
#endif
      }
    }

#endregion
  }
}