using System;

namespace CSLA.Server
{
  /// <summary>
  /// This exception is returned from the 
  /// server-side DataPortal and contains the exception
  /// and context data from the server.
  /// </summary>
  [Serializable()]
  public class DataPortalException : Exception
	{
    DataPortalResult _result;
                      string _innerStackTrace;


    /// <summary>
    /// Returns the DataPortalResult object from the server.
    /// </summary>
    public DataPortalResult Result
    {
      get
      {
        return _result;
      }
    }

    public override string StackTrace
    {
      get
      {
        return String.Format("{0}\n{1}", _innerStackTrace, base.StackTrace);
      }
    }

		public DataPortalException(string message, Exception ex) : base(message, ex)
		{
      _innerStackTrace = ex.StackTrace;
      _result = new DataPortalResult();
		}

    public DataPortalException(string message, DataPortalResult result) : base(message)
    {
      _innerStackTrace = string.Empty;
      _result = result;
    }

    public DataPortalException(string message, Exception ex, DataPortalResult result) : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
      _result = result;
    }

    protected DataPortalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
      _result = (DataPortalResult)info.GetValue("_result", typeof(DataPortalResult));
      _innerStackTrace = info.GetString("_innerStackTrace");
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_result", _result);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
  }
}
