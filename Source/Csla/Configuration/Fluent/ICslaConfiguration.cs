//-----------------------------------------------------------------------
// <copyright file="ICslaConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public interface ICslaConfiguration
  {
    /// <summary>
    /// Gets the current service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Expose configuration for the dataportal
    /// </summary>
    CslaDataPortalConfiguration DataPortalConfiguration { get; }

  }
}
