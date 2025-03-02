//-----------------------------------------------------------------------
// <copyright file="BlazorConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------

using Csla.Blazor;
using Csla.Blazor.State;
using Csla.Core;
using Csla.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class BlazorConfigurationExtensions
  {
    /// <summary>
    /// Configures services to provide CSLA Blazor server support
    /// </summary>
    /// <param name="config">CslaOptions instance</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions AddServerSideBlazor(this CslaOptions config)
    {
      return AddServerSideBlazor(config, null);
    }

    /// <summary>
    /// Configures services to provide CSLA Blazor server support
    /// </summary>
    /// <param name="config">CslaOptions instance</param>
    /// <param name="options">Options object</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions AddServerSideBlazor(this CslaOptions config, Action<BlazorServerConfigurationOptions>? options)
    {
      ArgumentNullException.ThrowIfNull(config);

      var blazorOptions = new BlazorServerConfigurationOptions();
      options?.Invoke(blazorOptions);

      config.BindingOptions.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;

      // set context manager
      string managerTypeName;
      if (blazorOptions.UseInMemoryApplicationContextManager)
        managerTypeName = "Csla.AspNetCore.Blazor.ApplicationContextManagerInMemory,Csla.AspNetCore";
      else
        managerTypeName = "Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor,Csla.AspNetCore";
      var managerType = Type.GetType(managerTypeName);
      if (managerType is null)
        throw new TypeLoadException(managerTypeName);
      config.Services.AddScoped(typeof(IContextManager), managerType);

      if (blazorOptions.UseInMemoryApplicationContextManager)
      {
        // do not use any Blazor state management
        config.Services.AddSingleton<ISessionManager, NoOpSessionManager>();
      }
      else
      {
        // use Blazor state management
        config.Services.AddTransient(typeof(ISessionIdManager), blazorOptions.SessionIdManagerType);
        config.Services.AddSingleton(typeof(ISessionStore), blazorOptions.SessionStoreType);
        config.Services.AddSingleton(typeof(ISessionManager), blazorOptions.SessionManagerType);
        config.Services.AddTransient<StateManager>();
      }

      // use Blazor viewmodel
      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      if (blazorOptions.UseCslaPermissionsPolicy)
      {
        config.Services.AddTransient<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
        config.Services.AddTransient<IAuthorizationHandler, CslaPermissionsHandler>();
      }
      return config;
    }
  }
}
