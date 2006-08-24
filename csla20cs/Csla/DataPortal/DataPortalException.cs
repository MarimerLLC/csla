using System;
using System.Security.Permissions;

namespace Csla
{

  /// <summary>
  /// This exception is returned for any errors occuring
  /// during the server-side DataPortal invocation.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
  [Serializable()]
  public class DataPortalException : Exception
  {
    private object _businessObject;
    private string _innerStackTrace;

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
      get { return _businessObject; }
    }

    /// <summary>
    /// Gets the original server-side exception.
    /// </summary>
    /// <returns>An exception object.</returns>
    /// <remarks>
    /// When an exception occurs in business code behind
    /// the data portal, it is wrapped in a 
    /// <see cref="Csla.Server.DataPortalException"/>, which 
    /// is then wrapped in a 
    /// <see cref="Csla.DataPortalException"/>. This property
    /// unwraps and returns the original exception 
    /// thrown by the business code on the server.
    /// </remarks>
    public Exception BusinessException
    {
      get
      {
        return this.InnerException.InnerException;
      }
    }

    /// <summary>
    /// Get the combined stack trace from the server
    /// and client.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
    public override string StackTrace
    {
      get { return String.Format("{0}{1}{2}", _innerStackTrace, Environment.NewLine, base.StackTrace); }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="businessObject">The business object
    /// as it was at the time of the exception.</param>
    public DataPortalException(string message, object businessObject)
      : base(message)
    {
      _innerStackTrace = String.Empty;
      _businessObject = businessObject;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <param name="businessObject">The business object
    /// as it was at the time of the exception.</param>
    public DataPortalException(string message, Exception ex, object businessObject)
      : base(message, ex)
    {
      _innerStackTrace = ex.StackTrace;
      _businessObject = businessObject;
    }

    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    protected DataPortalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
      _businessObject = info.GetValue("_businessObject", typeof(object));
      _innerStackTrace = info.GetString("_innerStackTrace");
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_businessObject", _businessObject);
      info.AddValue("_innerStackTrace", _innerStackTrace);
    }
  }
}
