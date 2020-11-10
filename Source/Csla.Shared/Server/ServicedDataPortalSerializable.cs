#if !NETFX_CORE && !MONO && !(ANDROID || IOS) && !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="ServicedDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side Serviced </summary>
//-----------------------------------------------------------------------
using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
    [AutoComplete(true)]
    public async Task<DataPortalResult> Create(
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var portal = new DataPortalBroker();
      return await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
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
    [AutoComplete(true)]
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var portal = new DataPortalBroker();
      return await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
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
    [AutoComplete(true)]
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      var portal = new DataPortalBroker();
      return await portal.Update(obj, context, isSync).ConfigureAwait(false);
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
    [AutoComplete(true)]
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      var portal = new DataPortalBroker();
      return await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
    }
  }
}
#endif