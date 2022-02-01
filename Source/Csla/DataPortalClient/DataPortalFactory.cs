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
    /// <param name="applicationContext">ApplicationContext instance</param>
    public DataPortalFactory(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IDataPortal<T> GetPortal<T>()
    {
      return ApplicationContext.CreateInstanceDI<IDataPortal<T>>();
    }
  }
}
