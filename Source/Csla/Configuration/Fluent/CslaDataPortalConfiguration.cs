//-----------------------------------------------------------------------
// <copyright file="CslaDataPortalConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaDataPortalConfiguration
  /// </summary>
  public static class CslaDataPortalConfigurationExtension
  {
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    public static CslaDataPortalConfiguration DataPortal(this ICslaConfiguration config)
    {
      return new CslaDataPortalConfiguration(config);
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="builder"></param>
    public static CslaDataPortalConfiguration AddServerSideDataPortal(this CslaDataPortalConfiguration builder)
    {
      var services = builder.Services;
      services.TryAddTransient(typeof(Server.IDataPortalServer), typeof(Csla.Server.DataPortal));
      services.TryAddTransient<Server.DataPortalSelector>();
      services.TryAddTransient<Server.SimpleDataPortal>();
      services.TryAddTransient<Server.FactoryDataPortal>();
      services.TryAddTransient<Server.DataPortalBroker>();
      services.TryAddSingleton(typeof(Server.Dashboard.IDashboard), typeof(Csla.Server.Dashboard.NullDashboard));
      return builder;
    }
  }

  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data portal.
  /// </summary>
  public class CslaDataPortalConfiguration
  {
    /// <summary>
    /// Gets or sets the current configuration object.
    /// </summary>
    public ICslaConfiguration CslaConfiguration { get; set; }

    /// <summary>
    /// Gets the current service collection.
    /// </summary>
    public IServiceCollection Services { get => CslaConfiguration.Services; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="config">Current configuration object</param>
    public CslaDataPortalConfiguration(ICslaConfiguration config)
    {
      CslaConfiguration = config;
    }

    ///// <summary>
    ///// Sets the type of the IDataPortalActivator provider.
    ///// </summary>
    ///// <param name="type">Activator type</param>
    //public CslaDataPortalConfiguration ActivatorType(Type type)
    //{
    //  ActivatorType(type.AssemblyQualifiedName);
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type of the IDataPortalActivator provider.
    ///// </summary>
    ///// <param name="typeName">Assembly qualified type name</param>
    //public CslaDataPortalConfiguration ActivatorType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaDataPortalActivator"] = typeName;
    //  return this;
    //}

    ///// <summary>
    ///// Sets the authentication type being used by the
    ///// CSLA .NET framework.
    ///// </summary>
    ///// <param name="typeName">Authentication type value (defaults to 'Csla')</param>
    //public CslaDataPortalConfiguration AuthenticationType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaAuthentication"] = typeName;
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type name to be used for server-side data portal
    ///// authorization. Type must implement IAuthorizeDataPortal.
    ///// </summary>
    ///// <param name="type">Authorization provider type</param>
    //public CslaDataPortalConfiguration ServerAuthorizationProviderType(Type type)
    //{
    //  ServerAuthorizationProviderType(type.AssemblyQualifiedName);
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type name to be used for server-side data portal
    ///// authorization. Type must implement IAuthorizeDataPortal.
    ///// </summary>
    ///// <param name="typeName">Assembly qualified type name</param>
    //public CslaDataPortalConfiguration ServerAuthorizationProviderType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaAuthorizationProvider"] = typeName;
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type of interceptor invoked
    ///// by the data portal for pre- and post-processing
    ///// of each data portal invocation. Type must implement
    ///// IInterceptDataPortal.
    ///// </summary>
    ///// <param name="typeName">Assembly qualified type name</param>
    //public CslaDataPortalConfiguration InterceptorType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaDataPortalInterceptor"] = typeName;
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type of interceptor invoked
    ///// by the data portal for pre- and post-processing
    ///// of each data portal invocation. Type must implement
    ///// IInterceptDataPortal.
    ///// </summary>
    ///// <param name="type">Interceptor type</param>
    //public CslaDataPortalConfiguration InterceptorType(Type type)
    //{
    //  InterceptorType(type.AssemblyQualifiedName);
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type of the ExceptionInspector class.
    ///// Type must implement IDataPortalExceptionInspector.
    ///// </summary>
    ///// <param name="type">Inspector type</param>
    //public CslaDataPortalConfiguration ExceptionInspectorType(Type type)
    //{
    //  ExceptionInspectorType(type.AssemblyQualifiedName);
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type name of the ExceptionInspector class.
    ///// Type must implement IDataPortalExceptionInspector.
    ///// </summary>
    ///// <param name="typeName">Assembly qualified type name</param>
    //public CslaDataPortalConfiguration ExceptionInspectorType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaDataPortalExceptionInspector"] = typeName;
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type of the factor loader used to create
    ///// server-side instances of business object factories when using
    ///// the FactoryDataPortal model. Type must implement
    ///// IObjectFactoryLoader.
    ///// </summary>
    ///// <param name="type">Factory loader type</param>
    //public CslaDataPortalConfiguration FactoryLoaderType(Type type)
    //{
    //  FactoryLoaderType(type.AssemblyQualifiedName);
    //  return this;
    //}

    ///// <summary>
    ///// Sets the type name of the factor loader used to create
    ///// server-side instances of business object factories when using
    ///// the FactoryDataPortal model. Type must implement
    ///// IObjectFactoryLoader.
    ///// </summary>
    ///// <param name="typeName">Assembly qualified type name</param>
    //public CslaDataPortalConfiguration FactoryLoaderType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaObjectFactoryLoader"] = typeName;
    //  return this;
    //}

    /// <summary>
    /// Sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    /// <param name="value">Value (defaults to true)</param>
    public CslaDataPortalConfiguration AutoCloneOnUpdate(bool value)
    {
      ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = value.ToString();
      return this;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException.
    /// </summary>
    /// <param name="value">Value (default is false)</param>
    public CslaDataPortalConfiguration DataPortalReturnObjectOnException(bool value)
    {
      ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"] = value.ToString();
      return this;
    }

    ///// <summary>
    ///// Sets the assembly qualified type name of the dashboard, or
    ///// 'Dashboard' for default, or 'NullDashboard' for the null dashboard.
    ///// </summary>
    ///// <param name="type">Dashboard type</param>
    ///// <returns></returns>
    //public CslaDataPortalConfiguration DashboardType(Type type)
    //{
    //  DashboardType(type.AssemblyQualifiedName);
    //  return this;
    //}

    ///// <summary>
    ///// Sets the assembly qualified type name of the dashboard, or
    ///// 'Dashboard' for default, or 'NullDashboard' for the null dashboard.
    ///// </summary>
    ///// <param name="typeName">Assembly qualified type name</param>
    ///// <returns></returns>
    //public CslaDataPortalConfiguration DashboardType(string typeName)
    //{
    //  ConfigurationManager.AppSettings["CslaDashboardType"] = typeName;
    //  //Csla.Server.Dashboard.DashboardFactory.Reset();
    //  return this;
    //}

    /// <summary>
    /// Sets a value indicating whether any
    /// synchronization context should be flowed to
    /// child tasks by LocalProxy. Setting this 
    /// to true may restrict or eliminate the 
    /// use of background threads by LocalProxy.
    /// </summary>
    /// <param name="flow">True to flow context</param>
    /// <returns></returns>
    public CslaDataPortalConfiguration FlowSynchronizationContext(bool flow)
    {
      ConfigurationManager.AppSettings["CslaFlowSynchronizationContext"] = flow.ToString().ToLower();
      return this;
    }
  }
}
