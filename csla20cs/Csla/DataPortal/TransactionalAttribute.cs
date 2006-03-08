using System;

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
  /// <see cref="TransactionalTypes.EnterpriseServices">EnterpriseServices</see> 
  /// option is assumed.
  /// </para><para>
  /// Both the EnterpriseServices and TransactionScope options provide
  /// 2-phase distributed transactional support.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class TransactionalAttribute : Attribute
  {
    private TransactionalTypes _type;

    /// <summary>
    /// Marks a method to run within a COM+
    /// transactional context.
    /// </summary>
    public TransactionalAttribute()
    {
      _type = TransactionalTypes.EnterpriseServices;
    }

    /// <summary>
    /// Marks a method to run within the specified
    /// type of transactional context.
    /// </summary>
    /// <param name="transactionType">
    /// Specifies the transactional context within which the
    /// method should run.</param>
    public TransactionalAttribute(TransactionalTypes transactionType)
    {
      _type = transactionType;
    }

    /// <summary>
    /// Gets the type of transaction requested by the
    /// business object method.
    /// </summary>
    public TransactionalTypes TransactionType
    {
      get { return _type; }
    }
  }
}