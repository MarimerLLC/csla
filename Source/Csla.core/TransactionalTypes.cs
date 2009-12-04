namespace Csla
{

  /// <summary>
  /// Provides a list of possible transactional
  /// technologies to be used by the server-side
  /// DataPortal.
  /// </summary>
  public enum TransactionalTypes
  {
    /// <summary>
    /// Causes the server-side DataPortal to
    /// use Enterprise Services (COM+) transactions.
    /// </summary>
    EnterpriseServices,
    /// <summary>
    /// Causes the server-side DataPortal to
    /// use System.Transactions TransactionScope
    /// style transactions.
    /// </summary>
    TransactionScope,
    /// <summary>
    /// Causes the server-side DataPortal to
    /// use no explicit transactional technology.
    /// </summary>
    /// <remarks>
    /// This option allows the business developer to
    /// implement their own transactions. Common options
    /// include ADO.NET transactions and System.Transactions
    /// TransactionScope.
    /// </remarks>
    Manual
  }
}