//-----------------------------------------------------------------------
// <copyright file="DataPortalClientOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client-side data portal options.</summary>
//-----------------------------------------------------------------------

using System;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Client-side data portal options.
  /// </summary>
  public class DataPortalClientOptions
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public DataPortalClientOptions(DataPortalOptions dataPortalOptions)
    {
      _parent = dataPortalOptions;
    }

    private readonly DataPortalOptions _parent;

    /// <summary>
    /// Gets a reference to the current services collection.
    /// </summary>
    public IServiceCollection Services => _parent.CslaOptions.Services;

    /// <summary>
    /// Gets or sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    public bool AutoCloneOnUpdate { get; set; } = true;
  }
}