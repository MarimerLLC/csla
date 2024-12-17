//-----------------------------------------------------------------------
// <copyright file="TransactionalAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Marks a DataPortal_XYZ method to run within</summary>
//-----------------------------------------------------------------------

using System;
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
  /// Both the EnterpriseServices and TransactionScope options provide
  /// 2-phase distributed transactional support.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public sealed class TransactionalAttribute : Attribute
  {

    /// <summary>
    /// Marks a method to run within a COM+
    /// transactional context.
    /// </summary>
    public TransactionalAttribute()
      : this(TransactionalTypes.TransactionScope)
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
      : this(transactionType, Configuration.DataOptions.GetDefaultTransactionIsolationLevel())
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
    /// Default can be specified in .config file via CslaTransactionIsolationLevel setting
    /// If none specified, Serializable level is used
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel)
      : this(transactionType, transactionIsolationLevel, Configuration.DataOptions.GetDefaultTransactionTimeoutInSeconds())
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
    /// Default can be specified in .config file via CslaTransactionIsolationLevel setting
    /// If none specified, Serializable level is used
    /// </param>
    /// <param name="asyncFlowOption">
    /// Specifies the async flow option used to initialize
    /// the transaction.
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel, TransactionScopeAsyncFlowOption asyncFlowOption)
      : this(transactionType, transactionIsolationLevel, Configuration.DataOptions.GetDefaultTransactionTimeoutInSeconds(), asyncFlowOption)
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
    /// Default can be specified in .config file via CslaTransactionIsolationLevel setting
    /// If none specified, Serializable level is used
    /// </param>
    /// <param name="timeoutInSeconds">
    /// Timeout for transaction, in seconds
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel, int timeoutInSeconds)
      : this(transactionType, transactionIsolationLevel, timeoutInSeconds, Configuration.DataOptions.GetDefaultTransactionScopeAsyncFlowOption())
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
    /// Default can be specified in .config file via CslaTransactionIsolationLevel setting
    /// If none specified, Serializable level is used
    /// </param>
    /// <param name="timeoutInSeconds">
    /// Timeout for transaction, in seconds
    /// </param>
    /// <param name="asyncFlowOption">
    /// Specifies the async flow option used to initialize
    /// the transaction.
    /// </param>
    public TransactionalAttribute(TransactionalTypes transactionType, TransactionIsolationLevel transactionIsolationLevel, int timeoutInSeconds, TransactionScopeAsyncFlowOption asyncFlowOption)
    {
      TransactionType = transactionType;
      TransactionIsolationLevel = transactionIsolationLevel;
      TimeoutInSeconds = timeoutInSeconds;
      AsyncFlowOption = asyncFlowOption;
    }

    /// <summary>
    /// Gets the type of transaction requested by the
    /// business object method.
    /// </summary>
    public TransactionalTypes TransactionType { get; }

    /// <summary>
    /// Specifies override for transaction isolation level.
    /// Default can be specified in .config file via CslaTransactionIsolationLevel setting
    /// If none specified, Serializable level is used
    /// </summary>
    public TransactionIsolationLevel TransactionIsolationLevel { get; }

    /// <summary>
    /// Timeout for transaction, in seconds
    /// </summary>
    /// <value>
    /// The timeout for transaction, in seconds
    /// </value>
    public int TimeoutInSeconds { get; }

    /// <summary>
    /// Gets the AsyncFlowOption for this transaction
    /// </summary>
    public TransactionScopeAsyncFlowOption AsyncFlowOption { get; } =
      TransactionScopeAsyncFlowOption.Suppress;
  }
}