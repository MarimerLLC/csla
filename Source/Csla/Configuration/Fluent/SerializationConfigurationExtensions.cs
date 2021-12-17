//-----------------------------------------------------------------------
// <copyright file="CslaSerializationConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaSerializationConfiguration
  /// </summary>
  public static class SerializationConfigurationExtensions
  {
    /// <summary>
    /// Extension method for CslaSerializationConfiguration
    /// </summary>
    public static CslaOptions Serialization(this CslaOptions config, Action<SerializationOptions> options)
    {
      options?.Invoke(config.SerializationOptions);
      return config;
    }
  }
}
