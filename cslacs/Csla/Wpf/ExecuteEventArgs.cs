using System;
using System.Windows;

#if SILVERLIGHT
namespace Csla.Silverlight
#else
namespace Csla.Wpf
#endif
{
  /// <summary>
  /// Arguments passed to a method invoked
  /// by the Execute trigger action.
  /// </summary>
  public class ExecuteEventArgs : EventArgs
  {
    /// <summary>
    /// The control that raised the event that
    /// triggered invocation of this method.
    /// </summary>
    public FrameworkElement TriggerSource { get; set; }
    /// <summary>
    /// The MethodParameter value provided by
    /// the designer.
    /// </summary>
    public object MethodParameter { get; set; }
    /// <summary>
    /// The EventArgs parameter from the event
    /// that triggered invocation of this method.
    /// </summary>
    public object TriggerParameter { get; set; }
  }
}
