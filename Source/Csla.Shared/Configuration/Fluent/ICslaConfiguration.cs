﻿//-----------------------------------------------------------------------
// <copyright file="ICslaConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public interface ICslaConfiguration
  {
#if !NETSTANDARD2_0
    /// <summary>
    /// Sets the web context manager.
    /// </summary>
    /// <param name="contextManager">Web context manager instance</param>
    /// <remarks>
    /// Will use default WebContextManager. 
    /// Only need to set for non-default WebContextManager.
    /// </remarks>
    /// <returns></returns>
    ICslaConfiguration WebContextManager(IContextManager contextManager);
#endif
    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    ICslaConfiguration UseReflectionFallback(bool value);
    /// <summary>
    /// Sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    /// <param name="mode">Property changed mode</param>
    ICslaConfiguration PropertyChangedMode(ApplicationContext.PropertyChangedModes mode);
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
    ICslaConfiguration VersionRoutingTag(string version);
    /// <summary>
    /// Sets the RuleSet name to use for static HasPermission calls.
    /// </summary>
    /// <param name="ruleSet">The rule set.</param>
    ICslaConfiguration RuleSet(string ruleSet);
    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    /// <param name="typeName">Factory type name</param>
    ICslaConfiguration PropertyInfoFactory(string typeName);
    /// <summary>
    /// Resets any ApplicationContext settings so they 
    /// re-read their configuration from AppSettings
    /// on next use.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    ICslaConfiguration SettingsChanged();
  }
}
