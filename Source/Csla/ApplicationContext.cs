//-----------------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;
using Csla.Core;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Csla.Server;

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
    /// <param name="applicationContextAccessor"></param>
    public ApplicationContext(ApplicationContextAccessor applicationContextAccessor)
    {
      ApplicationContextAccessor = applicationContextAccessor;
      ApplicationContextAccessor.GetContextManager().ApplicationContext = this;
    }

    internal ApplicationContextAccessor ApplicationContextAccessor { get; set; }

    /// <summary>
    /// Gets the context manager responsible
    /// for storing user and context information for
    /// the application.
    /// </summary>
    public IContextManager ContextManager => ApplicationContextAccessor.GetContextManager();

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
          ctx = [];
          ContextManager.SetLocalContext(ctx);
        }
        return ctx;
      }
    }

    private readonly object _syncContext = new();

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
            ctx = [];
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
    public static Type SerializationFormatter { get; internal set; } = typeof(Serialization.Mobile.MobileFormatter);

    private PropertyChangedModes _propertyChangedMode;
    private bool _propertyChangedModeSet;

    /// <summary>
    /// Gets or sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    public PropertyChangedModes PropertyChangedMode
    {
      get
      {
        if (!_propertyChangedModeSet)
        {
          var options = GetRequiredService<Configuration.CslaOptions>();
          _propertyChangedMode = options.BindingOptions.PropertyChangedMode;
          _propertyChangedModeSet = true;
        }
        return _propertyChangedMode;
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

    /// <summary>
    /// Returns a value indicating whether the application code
    /// is currently executing on the client or server.
    /// </summary>
    public ExecutionLocations ExecutionLocation { get; private set; } =
#if (ANDROID || IOS) && !NETSTANDARD
      ExecutionLocations.MobileClient;
#else
      ExecutionLocations.Client;
#endif

    internal void SetExecutionLocation(ExecutionLocations location)
    {
      ExecutionLocation = location;
    }

    /// <summary>
    /// The default RuleSet name 
    /// </summary>
    public const string DefaultRuleSet = "default";

    /// <summary>
    /// Gets or sets the RuleSet name to use for HasPermission calls.
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

    #endregion

    #region Logical Execution Location
    /// <summary>
    /// Enum representing the logical execution location
    /// The setting is set to server when server is executing
    /// a CRUD operation, otherwise the setting is always client
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

    /// <summary>
    /// Gets a value indicating whether the current 
    /// context manager is used in a stateful
    /// context (e.g. WPF, Blazor, etc.)
    /// </summary>
    public bool IsAStatefulContextManager => ContextManager.IsStatefulContext;

    /// <summary>
    /// Gets the service provider scope for this application context.
    /// </summary>
    internal IServiceProvider CurrentServiceProvider => ApplicationContextAccessor.ServiceProvider;

    /// <summary>
    /// Attempts to get service via DI using ServiceProviderServiceExtensions.GetRequiredService. 
    /// Throws exception if service not properly registered with DI.
    /// </summary>
    /// <typeparam name="T">Type of service/object to create.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public T GetRequiredService<T>()
    {
      if (CurrentServiceProvider == null)
        throw new NullReferenceException(nameof(CurrentServiceProvider));

      try
      {
        var result = CurrentServiceProvider.GetRequiredService<T>();
        return result;
      }
      catch (ObjectDisposedException ex)
      {
        throw new ObjectDisposedException($"GetRequiredService({typeof(T).FullName})", ex);
      }
    }

    /// <summary>
    /// Attempts to get service via DI using ServiceProviderServiceExtensions.GetRequiredService. 
    /// Throws exception if service not properly registered with DI.
    /// </summary>
    /// <param name="serviceType">Type of service/object to create.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object GetRequiredService(Type serviceType)
    {
      if (CurrentServiceProvider == null)
        throw new NullReferenceException(nameof(CurrentServiceProvider));

      try
      {
        return CurrentServiceProvider.GetRequiredService(serviceType);
      }
      catch (ObjectDisposedException ex)
      {
        throw new ObjectDisposedException($"GetRequiredService({serviceType.FullName})", ex);
      }
    }

    /// <summary>
    /// Creates an object using the IDataPortalActivator.
    /// </summary>
    /// <typeparam name="T">Type of object to create.</typeparam>
    /// <param name="parameters">Parameters for constructor</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public T CreateInstanceDI<T>(params object[] parameters)
    {
      return (T)CreateInstanceDI(typeof(T), parameters);
    }

    /// <summary>
    /// Creates an object using the IDataPortalActivator.
    /// </summary>
    /// <param name="objectType">Type of object to create</param>
    /// <param name="parameters">Manually passed in parameters for constructor</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object CreateInstanceDI(Type objectType, params object[] parameters)
    {
      try
      {
        var activator = CurrentServiceProvider.GetRequiredService<IDataPortalActivator>();
        var result = activator.CreateInstance(objectType, parameters);
        return result;
      }
      catch (ObjectDisposedException ex)
      {
        throw new ObjectDisposedException($"CreateInstanceDI({objectType.FullName})", ex);
      }
    }

    /// <summary>
    /// Creates an instance of a generic type
    /// using its default constructor.
    /// </summary>
    /// <param name="type">Generic type to create</param>
    /// <param name="paramTypes">Type parameters</param>
    internal object CreateGenericInstanceDI(Type type, params Type[] paramTypes)
    {
      var genericType = type.GetGenericTypeDefinition();
      var gt = genericType.MakeGenericType(paramTypes);
      return CreateInstanceDI(gt);
    }

    /// <summary>
    /// Creates an object using Activator.
    /// </summary>
    /// <typeparam name="T">Type of object to create.</typeparam>
    /// <param name="parameters">Parameters for constructor</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public T CreateInstance<T>(params object[] parameters)
    {
      return (T)CreateInstance(typeof(T), parameters);
    }

    /// <summary>
    /// Creates an object using Activator.
    /// </summary>
    /// <param name="objectType">Type of object to create</param>
    /// <param name="parameters">Parameters for constructor</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object CreateInstance(Type objectType, params object[] parameters)
    {
      object result;
      result = Activator.CreateInstance(objectType, parameters);
      if (result is IUseApplicationContext tmp)
      {
        tmp.ApplicationContext = this;
      }
      return result;
    }

    /// <summary>
    /// Creates an instance of a generic type using Activator.
    /// </summary>
    /// <param name="type">Generic type to create</param>
    /// <param name="paramTypes">Type parameters</param>
    internal object CreateGenericInstance(Type type, params Type[] paramTypes)
    {
      var genericType = type.GetGenericTypeDefinition();
      var gt = genericType.MakeGenericType(paramTypes);
      return CreateInstance(gt);
    }

  }
}
