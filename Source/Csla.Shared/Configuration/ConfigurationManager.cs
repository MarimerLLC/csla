//-----------------------------------------------------------------------
// <copyright file="ConfigurationManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>ConfigurationManager that abstracts underlying configuration</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Csla.Configuration
{
  /// <summary>
  /// ConfigurationManager that abstracts underlying configuration
  /// management implementations and infrastructure.
  /// </summary>
  public static class ConfigurationManager
  {
#if NETSTANDARD2_0
    private static NameValueCollection _settings = new NameValueCollection();
#else
    private static NameValueCollection _settings = System.Configuration.ConfigurationManager.AppSettings;
#endif

    /// <summary>
    /// Gets or sets the app settings for the application's default settings.
    /// </summary>
    public static NameValueCollection AppSettings
    {
      get
      {
        return _settings;
      }
      set
      {
        _settings = value;
        ApplicationContext.SettingsChanged();
      }
    }

#if NETSTANDARD2_0
    private static ConnectionStringSettingsCollection _connectionStrings = new ConnectionStringSettingsCollection();
#else
    private static System.Configuration.ConnectionStringSettingsCollection  _connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings;
#endif

    /// <summary>
    /// Gets or sets the connection strings from the 
    /// application's default settings.
    /// </summary>
#if NETSTANDARD2_0
    public static ConnectionStringSettingsCollection ConnectionStrings
#else
    public static System.Configuration.ConnectionStringSettingsCollection ConnectionStrings
#endif
    {
      get
      {
        return _connectionStrings;
      }
      set
      {
        _connectionStrings = value;
      }
    }
  }
}