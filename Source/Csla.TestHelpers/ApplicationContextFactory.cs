//-----------------------------------------------------------------------
// <copyright file="ApplicationContextFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory for creation of ApplicationContext instances for use in tests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.TestHelpers
{
  public static class ApplicationContextFactory
  {

    /// <summary>
    /// Create an instance of ApplicationContext for use in testing, using a specific DI container
    /// </summary>
    /// <param name="context">The context from which configuration can be retrieved</param>
    /// <returns>An instance of ApplicationContext for use in testing of Csla</returns>
    public static ApplicationContext CreateTestApplicationContext(TestDIContext context)
    {
      ApplicationContext applicationContext;

      applicationContext = context.ServiceProvider.GetRequiredService<ApplicationContext>();
      return applicationContext;
    }

  }
}
