//-----------------------------------------------------------------------
// <copyright file="ITransactionScopeFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates the TransactionScope used by the data portal</summary>
//-----------------------------------------------------------------------

using System.Runtime.Versioning;
using System.Transactions;

namespace Csla.Server
{
  /// <summary>
  /// Defines a service that creates the <see cref="TransactionScope"/>
  /// used by the server-side data portal to run a data portal
  /// operation marked with
  /// <see cref="TransactionalAttribute"/> using
  /// <see cref="TransactionalTypes.TransactionScope"/>.
  /// </summary>
  [UnsupportedOSPlatform("browser")]
  public interface ITransactionScopeFactory
  {
    /// <summary>
    /// Creates the <see cref="TransactionScope"/> within which the
    /// data portal operation will run.
    /// </summary>
    /// <param name="context">
    /// Information about the data portal operation being run.
    /// </param>
    /// <returns>The transaction scope for the operation.</returns>
    TransactionScope CreateTransactionScope(TransactionScopeFactoryContext context);
  }
}
