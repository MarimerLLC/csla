using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Security
{
  /// <summary>
  /// Options available for handling no
  /// access to a property due to
  /// authorization rules.
  /// </summary>
  public enum NoAccessBehavior
  {
    /// <summary>
    /// Suppress exceptions.
    /// </summary>
    SuppressException,
    /// <summary>
    /// Throw security exception.
    /// </summary>
    ThrowException
  }
}
