//-----------------------------------------------------------------------
// <copyright file="ConnectionStringSettings.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Information about a connection string</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Information about a connection string
  /// </summary>
  [Serializable]
  public class ConnectionStringSettings
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public ConnectionStringSettings()
    { }

#if !NETSTANDARD2_0 && !NET5_0
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public ConnectionStringSettings(System.Configuration.ConnectionStringSettings source)
    {
      Name = source.Name;
      ConnectionString = source.ConnectionString;
      ProviderName = source.ProviderName;
    }
#endif

    /// <summary>
    /// Gets or sets the connection name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the connection string text.
    /// </summary>
    public string ConnectionString { get; set; }
    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string ProviderName { get; set; }
  }
}
