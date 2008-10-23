using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{

  public class ErrorEventArgs : EventArgs
  {
    public object OriginalSender { get; protected set; }
    public Exception Error { get; protected set; }

    public ErrorEventArgs(object originalSender, Exception error)
    {
      OriginalSender = originalSender;
      Error = error;
    }
  }
}
