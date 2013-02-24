using System;
using System.Runtime.Serialization;

namespace Csla.Server.Hosts.WcfBfChannel
{
  /// <summary>
  /// Response message for returning
  /// the results of a data portal call.
  /// </summary>
  [Serializable]
  public class WcfResponse
  {
    private object _result;

    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="result">Result object to be returned.</param>
    public WcfResponse(object result)
    {
      _result = result;
    }

    /// <summary>
    /// Criteria object describing business object.
    /// </summary>
    public object Result
    {
      get { return _result; }
      set { _result = value; }
    }
  }
}