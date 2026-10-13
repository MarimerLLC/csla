//-----------------------------------------------------------------------
// <copyright file="TransactionalAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Marks a DataPortal_XYZ method to run within</summary>
//-----------------------------------------------------------------------

using System.Transactions;

namespace Csla
{
  /// <summary>
  /// Marks a DataPortal_XYZ method to run within
  /// the specified transactional context.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each business object method may be marked with this attribute
  /// to indicate which type of transactional technology should
  /// be used by the server-side DataPortal. The possible options
  /// are listed in the
  /// <see cref="TransactionalTypes">TransactionalTypes</see> enum.
  /// </para><para>
  /// If the Transactional attribute is not applied to a
  /// DataPortal_XYZ method then the
  /// <see cref="TransactionalTypes.Manual">Manual</see> option
  /// is assumed.
  /// </para><para>
  /// If the Transactional attribute is applied with no explicit
  /// choice for transactionType then the
  /// TransactionScope option is assumed.
  /// </para><para>
  /// Any transaction option that is not explicitly specified
  /// (isolation level <see cref="TransactionIsolationLevel.Unspecified"/>,
  /// or a <see langword="null"/> timeout or async flow option) is
  /// resolved when the transaction is created, using the defaults
  /// from <see cref="Configuration.DataOptions"/>. Transaction creation
  /// for the TransactionScope option can be customized by registering
  /// an <see cref="Server.ITransactionScopeFactory"/> service.
  /// </para><para>
  /// Both the EnterpriseServices and TransactionScope options provide
  /// 2-phase distributed transactional support.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public sealed class TransactionalAttribute : Attribute
  {

    /// <summary>
    /// Marks a method to run within a
    /// TransactionScope transactional context.
    /// </summary>
    public TransactionalAttribute()
      :this(TransactionalTypes.TransactionScope)
    {
    }

    /// <summary>
    /// Marks a method to run within the specified
    /// type of transactional context.
    /// </summary>
    /// <param name="transactionType">
    /// Specifies the transactional context within which the
    /// method should run.</param>
    public TransactionalAttribute(TransactionalTypes transactionType)
      : this(transactionType, TransactionIsolationLevel.Unspecified)
    {
    }

    /// <summary>
    /// Marks a method to run within the specified
    /// type of transactional context.
    /// </summary>
    /// <param name="transactionType">
    /// Specifies the transactional context within which the
    /// method should run.</param>
    /// <param name="transactionIsolationLevel">
    /// Specifies override for transaction isolation level.
    /// If <see cref="TransactionIsolationLevel.Unspecified"/>, the
    /// default from <see cref="Configuration.DataOptions"/> is used.
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel)
    {
      TransactionType = transactionType;
      TransactionIsolationLevel = transactionIsolationLevel;
    }

    /// <summary>
    /// Marks a method to run within the specified
    /// type of transactional context.
    /// </summary>
    /// <param name="transactionType">
    /// Specifies the transactional context within which the
    /// method should run.</param>
    /// <param name="transactionIsolationLevel">
    /// Specifies override for transaction isolation level.
    /// If <see cref="TransactionIsolationLevel.Unspecified"/>, the
    /// default from <see cref="Configuration.DataOptions"/> is used.
    /// </param>
    /// <param name="asyncFlowOption">
    /// Specifies the async flow option used to initialize
    /// the transaction.
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel, TransactionScopeAsyncFlowOption asyncFlowOption)
      : this(transactionType, transactionIsolationLevel)
    {
      AsyncFlowOption = asyncFlowOption;
    }

    /// <summary>
    /// Marks a method to run within the specified
    /// type of transactional context.
    /// </summary>
    /// <param name="transactionType">
    /// Specifies the transactional context within which the
    /// method should run.</param>
    /// <param name="transactionIsolationLevel">
    /// Specifies override for transaction isolation level.
    /// If <see cref="TransactionIsolationLevel.Unspecified"/>, the
    /// default from <see cref="Configuration.DataOptions"/> is used.
    /// </param>
    /// <param name="timeoutInSeconds">
    /// Timeout for transaction, in seconds
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel, int timeoutInSeconds)
      : this(transactionType, transactionIsolationLevel)
    {
      TimeoutInSeconds = timeoutInSeconds;
    }

    /// <summary>
    /// Marks a method to run within the specified
    /// type of transactional context.
    /// </summary>
    /// <param name="transactionType">
    /// Specifies the transactional context within which the
    /// method should run.</param>
    /// <param name="transactionIsolationLevel">
    /// Specifies override for transaction isolation level.
    /// If <see cref="TransactionIsolationLevel.Unspecified"/>, the
    /// default from <see cref="Configuration.DataOptions"/> is used.
    /// </param>
    /// <param name="timeoutInSeconds">
    /// Timeout for transaction, in seconds
    /// </param>
    /// <param name="asyncFlowOption">
    /// Specifies the async flow option used to initialize
    /// the transaction.
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel, int timeoutInSeconds, TransactionScopeAsyncFlowOption asyncFlowOption)
      : this(transactionType, transactionIsolationLevel)
    {
      TimeoutInSeconds = timeoutInSeconds;
      AsyncFlowOption = asyncFlowOption;
    }

    /// <summary>
    /// Gets the type of transaction requested by the
    /// business object method.
    /// </summary>
    public TransactionalTypes TransactionType { get; }

    /// <summary>
    /// Gets the transaction isolation level override.
    /// If <see cref="TransactionIsolationLevel.Unspecified"/>, the
    /// default from <see cref="Configuration.DataOptions"/> is used.
    /// </summary>
    public TransactionIsolationLevel TransactionIsolationLevel { get; }

    /// <summary>
    /// Gets the transaction timeout override, in seconds.
    /// If <see langword="null"/>, the default from
    /// <see cref="Configuration.DataOptions"/> is used.
    /// </summary>
    public int? TimeoutInSeconds { get; }

    /// <summary>
    /// Gets the async flow option override for this transaction.
    /// If <see langword="null"/>, the default from
    /// <see cref="Configuration.DataOptions"/> is used.
    /// </summary>
    public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; }
  }
}
