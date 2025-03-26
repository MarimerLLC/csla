//-----------------------------------------------------------------------
// <copyright file="CslaSerializationConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings</summary>
//-----------------------------------------------------------------------

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
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions Serialization(this CslaOptions config)
    {
      return Serialization(config, null);
    }

    /// <summary>
    /// Extension method for CslaSerializationConfiguration
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions Serialization(this CslaOptions config, Action<SerializationOptions>? options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      options?.Invoke(config.SerializationOptions);
      return config;
    }
  }
}
