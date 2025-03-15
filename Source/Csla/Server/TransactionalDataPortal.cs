//-----------------------------------------------------------------------
// <copyright file="TransactionalDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side Serviced </summary>
//-----------------------------------------------------------------------

using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side Serviced 
  /// DataPortal described in Chapter 4.
  /// </summary>
  [UnsupportedOSPlatform("browser")]
  public class TransactionalDataPortal : IDataPortalServer
  {
    private readonly DataPortalBroker _portal;
    private readonly TransactionalAttribute _transactionalAttribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionalDataPortal" /> class.
    /// </summary>
    /// <param name="dataPortalBroker">The broker that is used to perform the data access operation</param>
    /// <param name="transactionalAttribute">
    /// The transactional attribute that defines transaction options to be used with transactions.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="dataPortalBroker"/> or <paramref name="transactionalAttribute"/> is <see langword="null"/>.</exception>
    public TransactionalDataPortal(DataPortalBroker dataPortalBroker, TransactionalAttribute transactionalAttribute)
    {
      _portal = dataPortalBroker ?? throw new ArgumentNullException(nameof(dataPortalBroker));
      _transactionalAttribute = transactionalAttribute ?? throw new ArgumentNullException(nameof(transactionalAttribute));
    }

    /// <summary>
    /// Wraps a Create call in a TransactionScope
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
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
    public async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      using TransactionScope tr = CreateTransactionScope();
      var result = await _portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
      tr.Complete();
      return result;
    }

    private TransactionScope CreateTransactionScope()
    {
      return new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions(), _transactionalAttribute.AsyncFlowOption);
    }

    private TransactionOptions GetTransactionOptions()
    {
      var option = new TransactionOptions
                     {
                       IsolationLevel = GetIsolationLevel(_transactionalAttribute.TransactionIsolationLevel),
                       Timeout = TimeSpan.FromSeconds(_transactionalAttribute.TimeoutInSeconds)
                     };
      return option;
    }

    private IsolationLevel GetIsolationLevel(TransactionIsolationLevel transactionIsolationLevel)
    {
      return transactionIsolationLevel switch
      {
        TransactionIsolationLevel.Unspecified => IsolationLevel.Unspecified,
        TransactionIsolationLevel.Serializable => IsolationLevel.Serializable,
        TransactionIsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
        TransactionIsolationLevel.ReadCommitted => IsolationLevel.ReadCommitted,
        TransactionIsolationLevel.ReadUncommitted => IsolationLevel.ReadUncommitted,
        _ => IsolationLevel.Unspecified
      };
    }

    /// <summary>
    /// Called by the client-side DataPortal to retrieve an object.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Object containing context data from client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>A populated business object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      using TransactionScope tr = CreateTransactionScope();
      var result = await _portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
      tr.Complete();
      return result;
    }

    /// <summary>
    /// Called by the client-side DataPortal to update an object.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="context">Context data from the client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>A reference to the newly updated object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      using TransactionScope tr = CreateTransactionScope();
      var result = await _portal.Update(obj, context, isSync).ConfigureAwait(false);
      tr.Complete();
      return result;
    }

    /// <summary>
    /// Called by the client-side DataPortal to delete an object.
    /// </summary>
    /// <remarks>
    /// This method delegates to 
    /// <see cref="SimpleDataPortal">SimpleDataPortal</see>
    /// but wraps that call within a
    /// <see cref="TransactionScope">TransactionScope</see>
    /// to provide transactional support via
    /// System.Transactions.
    /// </remarks>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Object-specific criteria.</param>
    /// <param name="context">Context data from the client.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      using TransactionScope tr = CreateTransactionScope();
      var result = await _portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
      tr.Complete();
      return result;
    }
  }
}
