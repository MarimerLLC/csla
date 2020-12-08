#if !NETFX_CORE && !(ANDROID || IOS) 
//-----------------------------------------------------------------------
// <copyright file="TransactionIsolationLevel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
    /// Shows that different isolation level than the one specified is being used, but 
    /// the level cannot be determined. An exception is thrown if this value is set.
    /// Default.
    /// </summary>
    Unspecified,
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
#endif