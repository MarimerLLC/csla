//-----------------------------------------------------------------------
// <copyright file="ICslaBuilder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default CSLA .NET service builder</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
#if !NET40 && !NET45
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Csla.Configuration
{
  /// <summary>
  /// Default CSLA .NET service builder
  /// </summary>
  public class CslaBuilder : ICslaBuilder
  {
#if !NET40 && !NET45
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="services">IServiceCollection instance</param>
#pragma warning disable CS3001 // Argument type is not CLS-compliant
    public CslaBuilder(IServiceCollection services)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
    {
      Services = services;
    }

    /// <summary>
    /// Gets or sets the services collection
    /// </summary>
#pragma warning disable CS3003 // Type is not CLS-compliant
    public IServiceCollection Services { get; set; }
#pragma warning restore CS3003 // Type is not CLS-compliant
#endif
  }
}
