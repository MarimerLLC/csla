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
    public static CslaOptions AddServerSideBlazor(this CslaOptions config)
    {
      return AddServerSideBlazor(config, null);
    }

    /// <summary>
    /// Configures services to provide CSLA Blazor server support
    /// </summary>
    /// <param name="config">CslaOptions instance</param>
    /// <param name="options">Options object</param>
    public static CslaOptions AddServerSideBlazor(this CslaOptions config, Action<BlazorServerConfigurationOptions> options)
    {
      var blazorOptions = new BlazorServerConfigurationOptions();
      options?.Invoke(blazorOptions);

      // minimize PropertyChanged events
      config.BindingOptions.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;

      string managerTypeName;
      if (blazorOptions.UseInMemoryApplicationContextManager)
        managerTypeName = "Csla.AspNetCore.Blazor.ApplicationContextManagerInMemory,Csla.AspNetCore";
      else
        managerTypeName = "Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor,Csla.AspNetCore";
      var managerType = Type.GetType(managerTypeName);
      if (managerType is null)
        throw new TypeLoadException(managerTypeName);
      var contextManagerType = typeof(IContextManager);
      var managers = config.Services.Where(i => i.ServiceType.Equals(contextManagerType)).ToList();
      foreach (var manager in managers)
        config.Services.Remove(manager);
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

  /// <summary>
  /// Options for Blazor server-rendered and server-interactive.
  /// </summary>
  public class BlazorServerConfigurationOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether the app 
    /// should be configured to use CSLA permissions 
    /// policies (default = true).
    /// </summary>
    public bool UseCslaPermissionsPolicy { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to use a
    /// scoped DI container to manage the ApplicationContext; 
    /// false to use the Blazor 8 state management subsystem.
    /// </summary>
    public bool UseInMemoryApplicationContextManager { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of the ISessionManager service.
    /// </summary>
    public Type SessionManagerType { get; set; } = Type.GetType("Csla.Blazor.State.SessionManager, Csla.AspNetCore", true);

    /// <summary>
    /// Gets or sets the type of the ISessionIdManager service.
    /// </summary>
    public Type SessionIdManagerType { get; set; } = Type.GetType("Csla.Blazor.State.SessionIdManager, Csla.AspNetCore", true);
  }
}
