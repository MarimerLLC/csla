#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="ServicedDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side Serviced </summary>
//-----------------------------------------------------------------------
using System.EnterpriseServices;
using System.Runtime.InteropServices;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side Serviced 
  /// DataPortal described in Chapter 4.
  /// </summary>
  [Transaction(TransactionOption.Required, Isolation = System.EnterpriseServices.TransactionIsolationLevel.Serializable)]
  [EventTrackingEnabled(true)]
  [ComVisible(true)]
  public class ServicedDataPortalSerializable : ServicedComponent, IDataPortalServer
  {
    private DataPortalBroker portal { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataPortalBroker"></param>
    /// <exception cref="ArgumentNullException"><paramref name="dataPortalBroker"/> is <see langword="null"/>.</exception>
    public ServicedDataPortalSerializable(DataPortalBroker dataPortalBroker)
    {
      portal = dataPortalBroker ?? throw new ArgumentNullException(nameof(dataPortalBroker));
    }

    /// <summary>
    /// Wraps a Create call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="objectType">A <see cref="Type">Type</see> object
    /// indicating the type of business object to be created.</param>
    /// <param name="criteria">A custom criteria object providing any
    /// extra information that may be required to properly create
    /// the object.</param>
    /// <param name="context">Context data from the client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>A populated business object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    [AutoComplete(true)]
    public Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      return portal.Create(objectType, criteria, context, isSync);
    }

    /// <summary>
    /// Wraps a Fetch call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Object containing context data from client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>A populated business object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    [AutoComplete(true)]
    public Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      return portal.Fetch(objectType, criteria, context, isSync);
    }

    /// <summary>
    /// Wraps an Update call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="context">Context data from the client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>A reference to the newly updated object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    [AutoComplete(true)]
    public Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      return portal.Update(obj, context, isSync);
    }

    /// <summary>
    /// Wraps a Delete call in a ServicedComponent.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a COM+ transaction
    /// to provide transactional support.
    /// </remarks>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    [AutoComplete(true)]
    public Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      return portal.Delete(objectType, criteria, context, isSync);
    }
  }
}
#endif