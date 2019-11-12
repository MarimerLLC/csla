//-----------------------------------------------------------------------
// <copyright file="ICslaBuilder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Define CSLA .NET service builder</summary>
//-----------------------------------------------------------------------
#if !NET40 && !NET45
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Csla.Configuration
{
  /// <summary>
  /// Define CSLA .NET service builder
  /// </summary>
  public interface ICslaBuilder
  {
#if !NET40 && !NET45
    /// <summary>
    /// Gets or sets the services collection
    /// </summary>
    IServiceCollection Services { get; set; }
#endif
  }
}
