//-----------------------------------------------------------------------
// <copyright file="ICslaBuilder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Define CSLA .NET service builder</summary>
//-----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Define CSLA .NET service builder
  /// </summary>
  public interface ICslaBuilder
  {
    /// <summary>
    /// Gets or sets the services collection
    /// </summary>
#pragma warning disable CS3003 // Type is not CLS-compliant
    IServiceCollection Services { get; set; }
#pragma warning restore CS3003 // Type is not CLS-compliant
  }
}
