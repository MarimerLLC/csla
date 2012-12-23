//-----------------------------------------------------------------------
// <copyright file="TransactionIsolationLevel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Specifies an isolation level for transactions</summary>
namespace Csla
{
  /// <summary>
  /// Specifies an isolation level for transactions
  /// controlled by <see cref="TransactionalAttribute"/>
  /// </summary>
  public enum TransactionIsolationLevel
  {
    /// <summary>
    /// Prevents updating or inserting until the transaction is complete.
    /// </summary>
    Serializable,
    /// <summary>
    /// Locks are placed on all data that is used in a query, 
    /// preventing other users from updating the data. 
    /// Prevents non-repeatable reads, but phantom rows are still possible.
    /// </summary>
    RepeatableRead,
    /// <summary>
    /// Shared locks are held while the data is being read to avoid reading modified data, 
    /// but the data can be changed before the end of the transaction, 
    /// resulting in non-repeatable reads or phantom data.
    /// </summary>
    ReadCommitted,
    /// <summary>
    /// Shared locks are issued and no exclusive locks are honored.
    /// </summary>
    ReadUncommitted
  }
}
