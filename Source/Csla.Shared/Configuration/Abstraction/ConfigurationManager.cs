//-----------------------------------------------------------------------
// <copyright file="ConfigurationManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>ConfigurationManager that abstracts underlying configuration</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Specialized;

namespace Csla.Configuration
{
  /// <summary>
  /// ConfigurationManager that abstracts underlying configuration
  /// management implementations and infrastructure.
  /// </summary>
  public static class ConfigurationManager
  {
    private static NameValueCollection _settings = new NameValueCollection();

    static ConfigurationManager()
    {
#if !NETSTANDARD2_0 && !NET5_0
      try
      {
        _settings = System.Configuration.ConfigurationManager.AppSettings;
        foreach (System.Configuration.ConnectionStringSettings item in System.Configuration.ConfigurationManager.ConnectionStrings)
          ConnectionStrings.Add(item.Name, new ConnectionStringSettings(item));
      }
      catch (Exception ex)
      {
        throw new ConfigurationErrorsException(ex.Message, ex);
      }
#endif
    }

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

    /// <summary>
    /// Gets or sets the connection strings from the 
    /// application's default settings.
    /// </summary>
    public static ConnectionStringSettingsCollection ConnectionStrings { get; set; }
      = new ConnectionStringSettingsCollection();
  }
}