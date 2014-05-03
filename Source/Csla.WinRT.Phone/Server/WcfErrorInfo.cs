using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.WcfPortal
{
  /// <summary>
  /// Message containing details about any
  /// server-side exception.
  /// </summary>
  public class WcfErrorInfo
  {
    /// <summary>
    /// Type name of the exception object.
    /// </summary>
    public string ExceptionTypeName { get; set; }
    /// <summary>
    /// Message from the exception object.
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Stack trace from the exception object.
    /// </summary>
    public string StackTrace { get; set; }
    /// <summary>
    /// Source of the exception object.
    /// </summary>
    public string Source { get; set; }
    /// <summary>
    /// Target site name from the exception object.
    /// </summary>
    public string TargetSiteName { get; set; }
    /// <summary>
    /// WcfErrorInfo object containing information
    /// about any inner exception of the original
    /// exception.
    /// </summary>
    public WcfErrorInfo InnerError { get; internal set; }

    internal WcfErrorInfo()
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="ex">
    /// The Exception to encapusulate.
    /// </param>
    public WcfErrorInfo(Exception ex)
    {
      this.ExceptionTypeName = ex.GetType().FullName;
      this.Message = ex.Message;
      this.StackTrace = ex.StackTrace;
      this.Source = ex.Source;
      if (ex.InnerException != null)
        this.InnerError = new WcfErrorInfo(ex.InnerException);
    }
  }
}
