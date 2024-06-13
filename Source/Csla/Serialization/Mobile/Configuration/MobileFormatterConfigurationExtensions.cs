//-----------------------------------------------------------------------
// <copyright file="MobileFormatterConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for MobileFormatter configuration</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration;

/// <summary>
/// Implement extension methods for MobileFormatter configuration
/// </summary>
public static class MobileFormatterConfigurationExtensions
{
  /// <summary>
  /// Sets the serialization formatter type used by CSLA .NET
  /// </summary>
  /// <param name="config"></param>
  /// <returns></returns>
  public static SerializationOptions AddMobileFormatter(this SerializationOptions config)
  {
    return AddMobileFormatter(config, null);
  }

  /// <summary>
  /// Sets the serialization formatter type used by CSLA .NET
  /// </summary>
  /// <param name="config"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static SerializationOptions AddMobileFormatter(this SerializationOptions config, Action<MobileFormatterOptions> options)
  {
    ApplicationContext.SerializationFormatter = typeof(MobileFormatter);
    var mobileFormatterOptions = new MobileFormatterOptions();
    options?.Invoke(mobileFormatterOptions);
    config.FormatterOptions = mobileFormatterOptions;
    return config;
  }
}
