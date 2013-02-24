using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT
namespace Csla.Silverlight
#else
namespace Csla.Wpf
#endif
{
  /// <summary>
  /// Contains information about the error that
  /// has occurred.
  /// </summary>
  public class ErrorEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the Exception object for the error
    /// that occurred.
    /// </summary>
    public Exception Error { get; internal set; }
  }
}
