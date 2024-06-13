//-----------------------------------------------------------------------
// <copyright file="MobileFormatterConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for MobileFormatter configuration</summary>
//-----------------------------------------------------------------------
using System.Security.Claims;
using Csla.Serialization.Mobile;
using Csla.Serialization.Mobile.CustomSerializers;

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
    config.SerializationFormatter<MobileFormatter>();
    var mobileFormatterOptions = new MobileFormatterOptions();

    // add default custom serializers
    mobileFormatterOptions.CustomSerializers.Add(
      new TypeMap { OriginalType = typeof(ClaimsPrincipal), SerializerType = typeof(ClaimsPrincipalSerializer) });

    options?.Invoke(mobileFormatterOptions);
    config.FormatterOptions = mobileFormatterOptions;
    return config;
  }
}
