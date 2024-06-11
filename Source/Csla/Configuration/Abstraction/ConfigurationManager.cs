﻿//-----------------------------------------------------------------------
// <copyright file="ConfigurationManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>ConfigurationManager that abstracts underlying configuration</summary>
//-----------------------------------------------------------------------

using System.Collections.Specialized;

namespace Csla.Configuration
{
  /// <summary>
  /// ConfigurationManager that abstracts underlying configuration
  /// management implementations and infrastructure.
  /// </summary>
  public static class ConfigurationManager
  {
    static ConfigurationManager()
    {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
      try
      {
        AppSettings = System.Configuration.ConfigurationManager.AppSettings;
        foreach (System.Configuration.ConnectionStringSettings item in System.Configuration.ConfigurationManager.ConnectionStrings)
          ConnectionStrings.Add(item.Name, new ConnectionStringSettings(item));
      }
      catch (Exception ex)
      {
        throw new ConfigurationErrorsException(ex.Message, ex);
      }
#else
      AppSettings = new NameValueCollection();
#endif
    }

    /// <summary>
    /// Gets or sets the app settings for the application's default settings.
    /// </summary>
    public static NameValueCollection AppSettings { get; set; }

    /// <summary>
    /// Gets or sets the connection strings from the 
    /// application's default settings.
    /// </summary>
    public static ConnectionStringSettingsCollection ConnectionStrings { get; set; }
      = new ConnectionStringSettingsCollection();
  }
}