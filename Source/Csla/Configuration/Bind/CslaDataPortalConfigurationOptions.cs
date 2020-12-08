//-----------------------------------------------------------------------
// <copyright file="  public class CslaDataPortalConfigurationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using Csla.DataPortalClient;
using Csla.Server;

namespace Csla.Configuration
{

  /// <summary>
  /// Use this type to configure the settings for the CSLA .NET
  /// data portal using dot net core
  /// </summary>
  public class CslaDataPortalConfigurationOptions
  {
    /// <summary>
    /// Returns the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <remarks>
    /// This value is read from the application configuration
    /// file with the key value "CslaAuthentication". The value
    /// "Windows" indicates CSLA .NET should use Windows integrated
    /// (or AD) security. Any other value indicates the use of
    /// custom security derived from CslaPrincipal.
    /// </remarks>
    public string AuthenticationType
    {
      get { return ConfigurationManager.AppSettings["CslaAuthentication"]; }
      set { ConfigurationManager.AppSettings["CslaAuthentication"] = value; }
    }

    /// <summary>
    /// Gets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()method 
    /// when using a local data portal configuration.
    /// </summary>
    public bool AutoCloneOnUpdate
    {
      get
      {
        if (ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] == null)
          return false;
        else
          return bool.Parse(ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"]);
      }
      set
      {
        ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = value.ToString();
      }
    }

    /// <summary>
    /// Gets or sets an instance of the <see cref="IDataPortalActivator"/> provider.
    /// </summary>
    public string Activator
    {
      get { return ConfigurationManager.AppSettings["CslaDataPortalActivator"]; }
      set { ConfigurationManager.AppSettings["CslaDataPortalActivator"] = value; }
    }

    /// <summary>
    /// Gets or sets the fully qualified name of the ExceptionInspector class.
    /// </summary>
    public string ExceptionInspector
    {
      get { return ConfigurationManager.AppSettings["CslaDataPortalExceptionInspector"]; }
      set { ConfigurationManager.AppSettings["CslaDataPortalExceptionInspector"] = value; }
    }

    /// <summary>
    /// Sets the type name to be used for server-side data portal
    /// authorization. Type must implement <see cref="IAuthorizeDataPortal"/>.
    /// </summary>
    public string AuthorizationProvider
    {
      get { return ConfigurationManager.AppSettings["CslaAuthorizationProvider"]; }
      set { ConfigurationManager.AppSettings["CslaAuthorizationProvider"] = value; }
    }

    /// <summary>
    /// Gets or sets the type of interceptor invoked
    /// by the data portal for pre- and post-processing
    /// of each data portal invocation. Type must implement
    /// IInterceptDataPortal.
    /// </summary>
    public string Interceptor
    {
      get { return ConfigurationManager.AppSettings["CslaDataPortalInterceptor"]; }
      set { ConfigurationManager.AppSettings["CslaDataPortalInterceptor"] = value; }
    }

    /// <summary>
    /// Gets or sets the default data portal proxy type 
    /// </summary>
    public string Proxy
    {
      get { return ConfigurationManager.AppSettings["CslaDataPortalProxy"]; }
      set { ConfigurationManager.AppSettings["CslaDataPortalProxy"] = value; }
    }

    ///<summary>
    /// Gets or sets the full type name of
    /// the data portal proxy factory object to be used to get 
    /// the <see cref="IDataPortalProxy"/> instance to use when
    /// communicating with the data portal server.
    /// </summary>
    public string ProxyFactory
    {
      get { return ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"]; }
      set { ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"] = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the <see cref="DataPortalException"/> 
    /// (default is false).
    /// </summary>
    public bool ReturnObjectOnException
    {
      get
      {
        if (ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"] == null)
          return false;
        else
          return bool.Parse(ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"]);
      }
      set { Csla.ApplicationContext.DataPortalReturnObjectOnException = value; }
    }

    /// <summary>
    /// Gets or sets the data portal URL string.    
    /// </summary>
    /// <value>The data portal URL string.</value>
    public string PortalUrl
    {
      get { return ConfigurationManager.AppSettings["CslaDataPortalUrl"]; }
      set { ConfigurationManager.AppSettings["CslaDataPortalUrl"] = value; }
    }

    /// <summary>
    /// Gets or sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// <see cref="IObjectFactoryLoader"/>.
    /// </summary>
    public string ObjectFactoryLoader
    {
      get { return ConfigurationManager.AppSettings["CslaObjectFactoryLoader"]; }
      set { ConfigurationManager.AppSettings["CslaObjectFactoryLoader"] = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether any
    /// synchronization context should be flowed to
    /// child tasks by LocalProxy. Setting this 
    /// to true may restrict or eliminate the 
    /// use of background threads by LocalProxy.
    /// </summary>
    public bool FlowSynchronizationContext
    {
      get
      {
        if (ConfigurationManager.AppSettings["CslaFlowSynchronizationContext"] == null)
          return false;
        else
          return bool.Parse(ConfigurationManager.AppSettings["CslaFlowSynchronizationContext"]);
      }
      set { ConfigurationManager.AppSettings["CslaFlowSynchronizationContext"] = value.ToString(); }
    }
  }
}