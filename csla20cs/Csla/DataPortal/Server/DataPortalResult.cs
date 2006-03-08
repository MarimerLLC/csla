using System;
using System.Collections.Specialized;
using System.Text;

namespace Csla.Server
{
  /// <summary>
  /// Returns data from the server-side DataPortal to the 
  /// client-side DataPortal. Intended for internal CSLA .NET
  /// use only.
  /// </summary>
  [Serializable()]
  public class DataPortalResult
  {
    private object _returnObject;
    private HybridDictionary _globalContext;

    /// <summary>
    /// The business object being returned from
    /// the server.
    /// </summary>
    public object ReturnObject
    {
      get { return _returnObject; }
    }

    /// <summary>
    /// The global context being returned from
    /// the server.
    /// </summary>
    public HybridDictionary GlobalContext
    {
      get { return _globalContext; }
    }

    public DataPortalResult()
    {
      _globalContext = ApplicationContext.GetGlobalContext();
    }

    public DataPortalResult(object returnObject)
    {
      _returnObject = returnObject;
      _globalContext = ApplicationContext.GetGlobalContext();
    }
  }
}
