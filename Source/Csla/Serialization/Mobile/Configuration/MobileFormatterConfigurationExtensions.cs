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
  /// <exception cref="ArgumentNullException"> is <see langword="null"/>.</exception>
  public static SerializationOptions UseMobileFormatter(this SerializationOptions config)
  {
    return UseMobileFormatter(config, null);
  }

  /// <summary>
  /// Sets the serialization formatter type used by CSLA .NET
  /// </summary>
  /// <param name="config"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"> is <see langword="null"/>.</exception>
  public static SerializationOptions UseMobileFormatter(this SerializationOptions config, Action<MobileFormatterOptions>? options)
  {
    if (config is null)
      throw new ArgumentNullException(nameof(config));
    config.UseSerializationFormatter<MobileFormatter>();
    var mobileFormatterOptions = new MobileFormatterOptions();

    // add the custom serializers registered in CSLA itself, such as ClaimsPrincipal
    mobileFormatterOptions.AddCustomSerializers(typeof(MobileFormatter).Assembly);

    options?.Invoke(mobileFormatterOptions);
    config.Services.AddScoped(_ => mobileFormatterOptions);
    return config;
  }
}
