//-----------------------------------------------------------------------
// <copyright file="CslaConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaDataPortalConfiguration
  /// </summary>
  public static class CslaConfigurationExtension
  {
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    public static CslaConfiguration Configure(this ICslaConfiguration config)
    {
      return new CslaConfiguration();
    }
  }

  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public class CslaConfiguration : ICslaConfiguration
  {
    /// <summary>
    /// Gets an instance of the configuration root type.
    /// </summary>
    public static CslaConfiguration Configure()
    {
      return new CslaConfiguration();
    }

#if !NETSTANDARD2_0 && !NET5_0
    /// <summary>
    /// Sets the web context manager.
    /// </summary>
    /// <param name="contextManager">Web context manager instance</param>
    /// <remarks>
    /// Will use default WebContextManager. 
    /// Only need to set for non-default WebContextManager.
    /// </remarks>
    /// <returns></returns>
    public CslaConfiguration WebContextManager(IContextManager contextManager)
    {
      ApplicationContext.WebContextManager = contextManager;
      return this;
    }
#endif

    /// <summary>
    /// Sets the context manager.
    /// </summary>
    /// <param name="contextManager">Context manager instance.</param>
    /// <returns>
    /// ContextManager normally defaults to the correct value. Override for
    /// non-standard or custom behaviors.
    /// </returns>
    public CslaConfiguration ContextManager(IContextManager contextManager)
    {
      ApplicationContext.ContextManager = contextManager;
      return this;
    }

    /// <summary>
    /// Sets the context manager type.
    /// </summary>
    /// <param name="contextManagerType">Context manager type.</param>
    /// <returns>
    /// ContextManager normally defaults to the correct value. Override for
    /// non-standard or custom behaviors.
    /// </returns>
    public CslaConfiguration ContextManager(Type contextManagerType)
    {
      ContextManager((IContextManager)Reflection.MethodCaller.CreateInstance(contextManagerType));
      return this;
    }

    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    public CslaConfiguration UseReflectionFallback(bool value)
    {
      ApplicationContext.UseReflectionFallback = value;
      return this;
    }

    /// <summary>
    /// Sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    /// <param name="mode">Property changed mode</param>
    public CslaConfiguration PropertyChangedMode(ApplicationContext.PropertyChangedModes mode)
    {
      ConfigurationManager.AppSettings["CslaPropertyChangedMode"] = mode.ToString();
      return this;
    }

    /// <summary>
    /// Sets a value representing the application version
    /// for use in server-side data portal routing.
    /// </summary>
    /// <param name="version">
    /// Application version used to create data portal
    /// routing tag (can not contain '-').
    /// </param>
    /// <remarks>
    /// If this value is set then you must use the
    /// .NET Core server-side Http data portal endpoint
    /// as a router so the request can be routed to
    /// another app server that is running the correct
    /// version of the application's assemblies.
    /// </remarks>
    public CslaConfiguration VersionRoutingTag(string version)
    {
      if (!string.IsNullOrWhiteSpace(version))
        if (version.Contains("-") || version.Contains("/"))
          throw new ArgumentException("VersionRoutingTag");
      ConfigurationManager.AppSettings["CslaVersionRoutingTag"] = version;
      return this;
    }

    /// <summary>
    /// Sets the RuleSet name to use for static HasPermission calls.
    /// </summary>
    /// <param name="ruleSet">The rule set.</param>
    public CslaConfiguration RuleSet(string ruleSet)
    {
      ApplicationContext.RuleSet = ruleSet;
      return this;
    }

    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    /// <param name="typeName">Factory type name</param>
    public CslaConfiguration PropertyInfoFactory(string typeName)
    {
      ConfigurationManager.AppSettings["CslaPropertyInfoFactory"] = typeName;
      return this;
    }

    /// <summary>
    /// Sets the default service provider which Csla will use.
    /// </summary>
    /// <param name="defaultServiceProvider">IServiceProvider instance</param>
#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public CslaConfiguration DefaultServiceProvider(IServiceProvider defaultServiceProvider)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
      ApplicationContext.DefaultServiceProvider = defaultServiceProvider;
      return this;
    }

    /// <summary>
    /// Resets any ApplicationContext settings so they 
    /// re-read their configuration from AppSettings
    /// on next use.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public CslaConfiguration SettingsChanged()
    {
      ApplicationContext.SettingsChanged();
      return this;
    }
  }
}