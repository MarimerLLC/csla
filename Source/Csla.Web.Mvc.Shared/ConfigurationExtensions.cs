//-----------------------------------------------------------------------
// <copyright file="MvcConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for configuration</summary>
//-----------------------------------------------------------------------

#if NET462
using Csla.Web.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

namespace Csla.Configuration;

/// <summary>
/// Implement extension methods for ASP.NET
/// </summary>
public static class MvcConfigurationExtensions
{
  /// <summary>
  /// Registers services necessary for Windows Forms
  /// environments.
  /// </summary>
  /// <param name="config">CslaConfiguration object</param>
  public static CslaOptions AddAspNetMvc(this CslaOptions config)
  {
    return AddAspNetMvc(config, null);
  }

  /// <summary>
  /// Registers services necessary for Windows Forms
  /// environments.
  /// </summary>
  /// <param name="config">CslaConfiguration object</param>
  /// <param name="options">XamlOptions action</param>
  public static CslaOptions AddAspNetMvc(this CslaOptions config, Action<AspNetMvcOptions>? options)
  {
    var webOptions = new AspNetMvcOptions();
    options?.Invoke(webOptions);

#if NET462
    // use correct IContextManager
    config.Services.TryAddSingleton<Csla.Core.IContextManager, ApplicationContextManager>();
#endif

    return config;
  }
}

/// <summary>
/// Options for ASP.NET
/// </summary>
public class AspNetMvcOptions;