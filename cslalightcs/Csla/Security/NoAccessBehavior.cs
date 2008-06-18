using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
