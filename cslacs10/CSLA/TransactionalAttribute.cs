using System;

namespace CSLA
{
  /// <summary>
  /// Allows us to mark the DataPortal_xxx methods in our business
  /// classes as transactional.
  /// </summary>
  /// <remarks>
  /// When a method is marked as transactional, the DataPortal
  /// mechanism runs the method within a COM+ transactional
  /// context, so the data access is protected by a 2-phase
  /// distributed transaction.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public class TransactionalAttribute : Attribute
  {}
}
