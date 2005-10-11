using System;

namespace Csla
{
    // this attribute allows us to mark dataportal methods
    // as transactional to trigger use of EnterpriseServices
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
    public sealed class TransactionalAttribute : Attribute
    {

    }
}