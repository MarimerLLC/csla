using System;

namespace CSLA.Server
{
  /// <summary>
  /// This exception is returned from the 
  /// CallMethod method in the server-side DataPortal
  /// and contains the exception thrown by the
  /// underlying business object method that was
  /// being invoked.
  /// </summary>
  [Serializable()]
  public class CallMethodException : Exception
	{
    string _innerStackTrace;

    public override string StackTrace
    {
      get
      {
        return String.Format("{0}\n{1}", _innerStackTrace, base.StackTrace);
      }
    }

    public CallMethodException(string message, Exception ex) : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
    }

    protected CallMethodException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
      _innerStackTrace = info.GetString("_innerStackTrace");
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
	}
}
