using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Serialization;

namespace Csla.Server.Hosts.HttpChannel
{
  /// <summary>
  /// Message containing details about any
  /// server-side exception.
  /// </summary>
  [Serializable]
  public class HttpErrorInfo : ReadOnlyBase<HttpErrorInfo>
  {
    /// <summary>
    /// Type name of the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> ExceptionTypeNameProperty = RegisterProperty<string>(c => c.ExceptionTypeName);
    /// <summary>
    /// Type name of the exception object.
    /// </summary>
    public string ExceptionTypeName
    {
      get { return GetProperty(ExceptionTypeNameProperty); }
      private set { LoadProperty(ExceptionTypeNameProperty, value); }
    }
    /// <summary>
    /// Message from the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> MessageProperty = RegisterProperty<string>(c => c.Message);
    /// <summary>
    /// Message from the exception object.
    /// </summary>
    public string Message
    {
      get { return GetProperty(MessageProperty); }
      private set { LoadProperty(MessageProperty, value); }
    }
    /// <summary>
    /// Stack trace from the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> StackTraceProperty = RegisterProperty<string>(c => c.StackTrace);
    /// <summary>
    /// Stack trace from the exception object.
    /// </summary>
    public string StackTrace
    {
      get { return GetProperty(StackTraceProperty); }
      private set { LoadProperty(StackTraceProperty, value); }
    }
    /// <summary>
    /// Source of the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> SourceProperty = RegisterProperty<string>(c => c.Source);
    /// <summary>
    /// Source of the exception object.
    /// </summary>
    public string Source
    {
      get { return GetProperty(SourceProperty); }
      private set { LoadProperty(SourceProperty, value); }
    }
    /// <summary>
    /// WcfErrorInfo object containing information
    /// about any inner exception of the original
    /// exception.
    /// </summary>
    public static readonly PropertyInfo<HttpErrorInfo> InnerErrorProperty = RegisterProperty<HttpErrorInfo>(c => c.InnerError);
    /// <summary>
    /// WcfErrorInfo object containing information
    /// about any inner exception of the original
    /// exception.
    /// </summary>
    public HttpErrorInfo InnerError
    {
      get { return GetProperty(InnerErrorProperty); }
      private set { LoadProperty(InnerErrorProperty, value); }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="ex">
    /// The Exception to encapusulate.
    /// </param>
    public HttpErrorInfo(Exception ex)
    {
      this.ExceptionTypeName = ex.GetType().FullName;
      this.Message = ex.Message;
      this.StackTrace = ex.StackTrace;
      this.Source = ex.Source;
      if (ex.InnerException != null)
        this.InnerError = new HttpErrorInfo(ex.InnerException);
    }

    /// <summary>
    /// Creates an empty instance of the type.
    /// </summary>
    public HttpErrorInfo()
    { }
  }
}
