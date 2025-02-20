//-----------------------------------------------------------------------
// <copyright file="DataPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------

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
    public DataPortalFactory(IServiceProvider serviceProvider)
    {
      this._serviceProvider = serviceProvider;
    }

    private IServiceProvider _serviceProvider;

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
      return (IDataPortal<T>)_serviceProvider.GetService(typeof(IDataPortal<T>));
    }
  }
}
