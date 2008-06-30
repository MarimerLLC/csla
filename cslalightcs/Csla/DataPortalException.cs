using System;
using Csla.WcfPortal;

namespace Csla
{
  public class DataPortalException : Exception
  {
    public WcfPortal.WcfErrorInfo ErrorInfo { get; private set; }

    public DataPortalException(string message, Exception ex)
      : base(message)
    {
      ErrorInfo = ex.ToErrorInfo();
    }

    public DataPortalException(WcfPortal.WcfErrorInfo info)
      : base(info.Message)
    {
      this.ErrorInfo = info;
    }

    public override string ToString()
    {
      var sb = new System.Text.StringBuilder();
      var error = this.ErrorInfo;
      while (error != null)
      {
        sb.AppendFormat("{0}: {1}", error.ExceptionTypeName, error.Message);
        sb.Append(Environment.NewLine);
        sb.Append(error.StackTrace);
        sb.Append(Environment.NewLine);
        error = error.InnerError;
      }
      return sb.ToString();
    }
  }
}
