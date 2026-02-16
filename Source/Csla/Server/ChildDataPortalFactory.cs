//-----------------------------------------------------------------------
// <copyright file="ChildDataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Server
{
  /// <summary>
  /// Get an access to a Child data portal
  /// instance.
  /// </summary>
  public class ChildDataPortalFactory : IChildDataPortalFactory
  {
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="serviceProvider">Current ServiceProvider</param>
    /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    public ChildDataPortalFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IChildDataPortal<T> GetPortal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]T>()
      where T : ICslaObject
    {
      return _serviceProvider.GetRequiredService<IChildDataPortal<T>>();
    }
  }
}