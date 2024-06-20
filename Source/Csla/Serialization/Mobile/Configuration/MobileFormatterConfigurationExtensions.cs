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
  public static SerializationOptions UseMobileFormatter(this SerializationOptions config, Action<MobileFormatterOptions> options)
  {
    config.UseSerializationFormatter<MobileFormatter>();
    var mobileFormatterOptions = new MobileFormatterOptions();

    // add default custom serializers
    mobileFormatterOptions.CustomSerializers.Add(
      new TypeMap<ClaimsPrincipal, ClaimsPrincipalSerializer>(ClaimsPrincipalSerializer.CanSerialize));

    options?.Invoke(mobileFormatterOptions);
    config.FormatterOptions = mobileFormatterOptions;
    return config;
  }
}
