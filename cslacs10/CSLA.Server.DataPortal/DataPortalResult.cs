using System;

namespace CSLA.Server
{
  /// <summary>
  /// Returns data from the server-side DataPortal to the 
  /// client-side DataPortal. Intended for internal CSLA .NET
  /// use only.
  /// </summary>
  [Serializable()]
  public class DataPortalResult
  {
    public object ReturnObject;
    public object GlobalContext;

    public DataPortalResult()
    {
      GlobalContext = ApplicationContext.GetGlobalContext();
    }

    public DataPortalResult(object returnObject)
    {
      ReturnObject = returnObject;
      GlobalContext = ApplicationContext.GetGlobalContext();
    }
  }
}
