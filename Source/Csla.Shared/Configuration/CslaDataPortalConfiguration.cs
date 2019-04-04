//-----------------------------------------------------------------------
// <copyright file="CslaDataPortalConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

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
  }

  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data portal.
  /// </summary>
  public class CslaDataPortalConfiguration
  {
    private ICslaConfiguration RootConfiguration { get; set; }

    internal CslaDataPortalConfiguration(ICslaConfiguration root)
    {
      RootConfiguration = root;
    }

    /// <summary>
    /// Configure the default data portal proxy type and URL.
    /// </summary>
    /// <param name="type">Type of data portal proxy</param>
    /// <param name="defaultUrl">Default server URL</param>
    /// <returns></returns>
    public ICslaConfiguration DefaultProxy(Type type, string defaultUrl)
    {
      DefaultProxy(type.AssemblyQualifiedName, defaultUrl);
      return RootConfiguration;
    }

    /// <summary>
    /// Configure the default data portal proxy type and URL.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    /// <param name="defaultUrl">Default server URL</param>
    /// <returns></returns>
    public ICslaConfiguration DefaultProxy(string typeName, string defaultUrl)
    {
      ConfigurationManager.AppSettings["CslaDataPortalProxy"] = typeName;
      ConfigurationManager.AppSettings["CslaDataPortalUrl"] = defaultUrl;
      return RootConfiguration;
    }

    /// <summary>
    /// Adds resource/type to data portal proxy mappings
    /// for use by the data portal.
    /// </summary>
    /// <param name="descriptors">Data portal type/resource to proxy mapping</param>
    /// <returns></returns>
    public ICslaConfiguration ProxyDescriptors(List<Tuple<string, string, string>> descriptors)
    {
      DataPortalClient.DataPortalProxyFactory.DataPortalTypeProxyDescriptors?.Clear();

      foreach (var item in descriptors)
      {
        if (int.TryParse(item.Item1, out int result))
        {
          Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor(
            result,
            new DataPortalClient.DataPortalProxyDescriptor { ProxyTypeName = item.Item2, DataPortalUrl = item.Item3 });
        }
        else
        {
          try
          {
            var type = Type.GetType(item.Item1);
            Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor(
              type,
              new DataPortalClient.DataPortalProxyDescriptor { ProxyTypeName = item.Item2, DataPortalUrl = item.Item3 });
          }
          catch (NullReferenceException ex)
          {
            throw new ArgumentException(item.Item1, ex);
          }
        }
      }
      return RootConfiguration;
    }

    ///<summary>
    /// Sets the full type name (or 'Default') of
    /// the data portal proxy factory object to be used to get 
    /// the DataPortalProxy instance to use when
    /// communicating with the data portal server.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration ProxyFactoryType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the type of the IDataPortalActivator provider.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration ActivatorType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaDataPortalActivator"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets an instance of the IDataPortalActivator provider.
    /// </summary>
    /// <param name="activator">Activator instance</param>
    public ICslaConfiguration Activator(Server.IDataPortalActivator activator)
    {
      ApplicationContext.DataPortalActivator = activator;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <param name="typeName">Authentication type value (defaults to 'Csla')</param>
    public ICslaConfiguration AuthenticationType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaAuthentication"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the type name to be used for server-side data portal
    /// authorization. Type must implement IAuthorizeDataPortal.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration ServerAuthorizationProviderType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaAuthorizationProvider"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the type of interceptor invoked
    /// by the data portal for pre- and post-processing
    /// of each data portal invocation. Type must implement
    /// IInterceptDataPortal.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration InterceptorType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaDataPortalInterceptor"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the type name of the ExceptionInspector class.
    /// Type must implement IDataPortalExceptionInspector.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration ExceptionInspectorType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaDataPortalExceptionInspector"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// IObjectFactoryLoader.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    public ICslaConfiguration FactoryLoaderType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaObjectFactoryLoader"] = typeName;
      return RootConfiguration;
    }

    /// <summary>
    /// Sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    /// <param name="value">Value (defaults to true)</param>
    public ICslaConfiguration AutoCloneOnUpdate(bool value)
    {
      ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = value.ToString();
      return RootConfiguration;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException.
    /// </summary>
    /// <param name="value">Value (default is false)</param>
    public ICslaConfiguration DataPortalReturnObjectOnException(bool value)
    {
      ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"] = value.ToString();
      return RootConfiguration;
    }

    /// <summary>
    /// Sets the assembly qualified type name of the dashboard, or
    /// 'Dashboard' for default, or 'NullDashboard' for the null dashboard.
    /// </summary>
    /// <param name="typeName">Assembly qualified type name</param>
    /// <returns></returns>
    public ICslaConfiguration DashboardType(string typeName)
    {
      ConfigurationManager.AppSettings["CslaDashboardType"] = typeName;
      Csla.Server.Dashboard.DashboardFactory.Reset();
      return RootConfiguration;
    }
  }
}
