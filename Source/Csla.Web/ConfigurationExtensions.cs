//-----------------------------------------------------------------------
// <copyright file="WebConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for configuration</summary>
//-----------------------------------------------------------------------

using Csla.Web;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration;

/// <summary>
/// Implement extension methods for ASP.NET
/// </summary>
public static class WebConfigurationExtensions
{
  /// <summary>
  /// Registers services necessary for Windows Forms
  /// environments.
  /// </summary>
  /// <param name="config">CslaConfiguration object</param>
  public static CslaOptions AddAspNet(this CslaOptions config)
  {
    return AddAspNet(config, null);
  }

  /// <summary>
  /// Registers services necessary for Windows Forms
  /// environments.
  /// </summary>
  /// <param name="config">CslaConfiguration object</param>
  /// <param name="options">XamlOptions action</param>
  public static CslaOptions AddAspNet(this CslaOptions config, Action<AspNetOptions> options)
  {
    var webOptions = new AspNetOptions();
    options?.Invoke(webOptions);

    // use correct IContextManager
    config.Services.TryAddSingleton<Core.IContextManager, ApplicationContextManager>();

    return config;
  }
}

/// <summary>
/// Options for ASP.NET
/// </summary>
public class AspNetOptions;