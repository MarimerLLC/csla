//-----------------------------------------------------------------------
// <copyright file="TransactionalDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side Serviced </summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side Serviced 
  /// DataPortal described in Chapter 4.
  /// </summary>
  public class TransactionalDataPortal : IDataPortalServer
  {
    private readonly TransactionalAttribute _transactionalAttribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionalDataPortal" /> class.
    /// </summary>
    /// <param name="transactionalAttribute">
    /// The transactional attribute that defines transaction options to be used with transactions.
    /// </param>
    public TransactionalDataPortal(TransactionalAttribute transactionalAttribute)
    {
      _transactionalAttribute = transactionalAttribute;
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
    public async Task<DataPortalResult> Create(
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      using (TransactionScope tr = CreateTransactionScope())
      {
        var portal = new DataPortalBroker();
        result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
        tr.Complete();
      }
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
      switch (transactionIsolationLevel)
      {
        case TransactionIsolationLevel.Unspecified:
          return IsolationLevel.Unspecified;
        case TransactionIsolationLevel.Serializable:
          return IsolationLevel.Serializable;
        case TransactionIsolationLevel.RepeatableRead:
          return IsolationLevel.RepeatableRead;
        case TransactionIsolationLevel.ReadCommitted:
          return IsolationLevel.ReadCommitted;
        case TransactionIsolationLevel.ReadUncommitted:
          return IsolationLevel.ReadUncommitted;
        default:
          return IsolationLevel.Unspecified;
      }
    }

    /// <summary>
    /// Called by the client-side DataProtal to retrieve an object.
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
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      using (TransactionScope tr = CreateTransactionScope())
      {
        var portal = new DataPortalBroker();
        result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
        tr.Complete();
      }
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
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      using (TransactionScope tr = CreateTransactionScope())
      {
        var portal = new DataPortalBroker();
        result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
        tr.Complete();
      }
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
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      using (TransactionScope tr = CreateTransactionScope())
      {
        var portal = new DataPortalBroker();
        result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
        tr.Complete();
      }
      return result;
    }
  }
}
