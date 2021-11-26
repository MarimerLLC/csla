//-----------------------------------------------------------------------
// <copyright file="RootServiceProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Static class that makes the root service provider available to factories</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.TestHelpers
{
  public static class RootServiceProvider
  {
    private static IServiceProvider _serviceProvider;

    /// <summary>
    /// Store the root service provider for use in object creation during tests
    /// </summary>
    /// <param name="serviceProvider">The root service provider that has been created</param>
    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Create an object instance of a required type for use during tests
    /// </summary>
    /// <returns>Returns an instance of the required type constructed using the root service provider</returns>
    internal static T GetRequiredService<T>()
    {
      return _serviceProvider.GetRequiredService<T>();
    }

  }
}
