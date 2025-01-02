//-----------------------------------------------------------------------
// <copyright file="DataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics.CodeAnalysis;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Get an access to a client-side data portal
  /// instance.
  /// </summary>
  public class DataPortalFactory : IDataPortalFactory
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="serviceProvider">Current ServiceProvider</param>
    /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    public DataPortalFactory(IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    private IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IDataPortal<T> GetPortal<
#if NET8_0_OR_GREATER
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    T>()
    {
      return (IDataPortal<T>)ServiceProvider.GetRequiredService(typeof(IDataPortal<T>));
    }
  }
}
