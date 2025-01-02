//-----------------------------------------------------------------------
// <copyright file="DataPortalClientOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client-side data portal options.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Client-side data portal options.
  /// </summary>
  public class DataPortalClientOptions
  {
    private readonly DataPortalOptions _parent;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="dataPortalOptions"/> is <see langword="null"/>.</exception>
    public DataPortalClientOptions(DataPortalOptions dataPortalOptions)
    {
      _parent = dataPortalOptions ?? throw new ArgumentNullException(nameof(dataPortalOptions));
    }

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

    /// <summary>
    /// Gets or sets the type that implements 
    /// IDataPortalCache for client-side caching.
    /// </summary>
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    public Type DataPortalCacheType { get; set; } = typeof(DataPortalNoCache);
  }
}
