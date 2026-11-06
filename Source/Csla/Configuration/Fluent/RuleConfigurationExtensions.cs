//-----------------------------------------------------------------------
// <copyright file="RuleConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Configure business rule settings</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for business rule options.
  /// </summary>
  public static class RuleConfigurationExtensions
  {
    /// <summary>
    /// Configures business rule options.
    /// </summary>
    /// <param name="config">CSLA options.</param>
    /// <param name="options">Action that configures the rule options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions Rules(this CslaOptions config, Action<RuleOptions>? options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      options?.Invoke(config.RuleOptions);
      return config;
    }
  }
}
