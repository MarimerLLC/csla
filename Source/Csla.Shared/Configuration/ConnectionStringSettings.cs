#if NETSTANDARD2_0
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
    /// Gets or sets the connection string text.
    /// </summary>
    public string ConnectionString { get; set; }
  }
}
#endif