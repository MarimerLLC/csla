//-----------------------------------------------------------------------
// <copyright file="ChildDataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Server
{
  /// <summary>
  /// Get an access to a Child data portal
  /// instance.
  /// </summary>
  public class ChildDataPortalFactory : IChildDataPortalFactory
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="serviceProvider">Current ServiceProvider</param>
    public ChildDataPortalFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    private IServiceProvider _serviceProvider;

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IChildDataPortal<T> GetPortal<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    T>()
    {
      return (IChildDataPortal<T>)_serviceProvider.GetService(typeof(IChildDataPortal<T>));
    }
  }
}
