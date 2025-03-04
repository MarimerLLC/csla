//-----------------------------------------------------------------------
// <copyright file="CslaDataConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Configure data binding settings</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for data binding
  /// </summary>
  public static class BindingConfigurationExtensions
  {
    /// <summary>
    /// Extension method for CslaDataConfiguration
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions Binding(this CslaOptions config, Action<BindingOptions>? options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      options?.Invoke(config.BindingOptions);
      return config;
    }
  }
}
