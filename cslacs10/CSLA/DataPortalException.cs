using System;

namespace CSLA
{
  /// <summary>
  /// This exception is returned for any errors occuring
  /// during the server-side DataPortal invocation.
  /// </summary>
  [Serializable()]
  public class DataPortalException : Exception
	{
    object _businessObject;
                              string _innerStackTrace;

    /// <summary>
    /// Returns a reference to the business object
    /// from the server-side DataPortal.
    /// </summary>
    /// <remarks>
    /// Remember that this object may be in an invalid
    /// or undefined state. This is the business object
    /// (and any child objects) as it existed when the
    /// exception occured on the server. Thus the object
    /// state may have been altered by the server and
    /// may no longer reflect data in the database.
    /// </remarks>
    public object BusinessObject
    {
      get
      {
        return _businessObject;
      }
    }

    public override string StackTrace
    {
      get
      {
        return String.Format("{0}\n{1}", _innerStackTrace, base.StackTrace);
      }
    }

    public DataPortalException(string message, object businessObject) : base(message)
    {
      _innerStackTrace = string.Empty;
      _businessObject = businessObject;
    }

    public DataPortalException(string message, Exception ex, object businessObject) : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
      _businessObject = businessObject;
    }

    protected DataPortalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
      _businessObject = info.GetValue("_businessObject", typeof(Object));
      _innerStackTrace = info.GetString("_innerStackTrace");
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_businessObject", _businessObject);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
  }
}
