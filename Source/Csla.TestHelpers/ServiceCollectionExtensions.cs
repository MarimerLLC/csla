//-----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Wiring up of DI for Csla, for enabling test projects</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Configuration;
using Csla.TestHelpers;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{

  /// <summary>
  /// Extensions to IServiceCollection to support testability of Csla
  /// </summary>
  public static class ServiceCollectionExtensions
  {

    /// <summary>
    /// Add the most basic of DI configurations to support testing
    /// </summary>
    /// <param name="services">The service collection into which dependencies are registered</param>
    /// <returns>The instance of IServiceCollection being extended, to support method chaining</returns>
    public static IServiceCollection AddCslaTesting(this IServiceCollection services)
    {
      services.AddTransient<IHostEnvironment, TestHostEnvironment>();
      services.AddLogging();
      services.AddCsla();
      services.AddSingleton<Csla.Core.IContextManager, Csla.Core.ApplicationContextManagerStatic>();
      services.AddSingleton<Csla.Server.Dashboard.IDashboard, Csla.Server.Dashboard.Dashboard>();

      return services;
    }

  }
}
