using System;
using System.EnterpriseServices;

namespace CSLA.Server.ServicedDataPortal
{
  /// <summary>
  /// Implements the transactional server-side DataPortal object as
  /// discussed in Chapter 5.
  /// </summary>
  [Transaction(TransactionOption.Required), EventTrackingEnabled(true)]
  public class DataPortal : ServicedComponent
  {
    /// <summary>
    /// Invokes the server-side DataPortal Create method within
    /// a COM+ transaction.
    /// </summary>
    [AutoComplete(true)]
    public object Create(object criteria, object principal)
    {
      CSLA.Server.DataPortal portal = new CSLA.Server.DataPortal();
      return portal.Create(criteria, principal);
    }

    /// <summary>
    /// Invokes the server-side DataPortal Fetch method within
    /// a COM+ transaction.
    /// </summary>
    [AutoComplete(true)]
    public object Fetch(object criteria, object principal)
    {
      CSLA.Server.DataPortal portal = new CSLA.Server.DataPortal();
      return portal.Fetch(criteria, principal);
    }

    /// <summary>
    /// Invokes the server-side DataPortal Update method within
    /// a COM+ transaction.
    /// </summary>
    [AutoComplete(true)]
    public object Update(object obj, object principal)
    {
      CSLA.Server.DataPortal portal = new CSLA.Server.DataPortal();
      return portal.Update(obj, principal);
    }

    /// <summary>
    /// Invokes the server-side DataPortal Delete method within
    /// a COM+ transaction.
    /// </summary>
    [AutoComplete(true)]
    public void Delete(object criteria, object principal)
    {
      CSLA.Server.DataPortal portal = new CSLA.Server.DataPortal();
      portal.Delete(criteria, principal);
    }
  }
}
