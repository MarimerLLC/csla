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

namespace Bxf
{
  /// <summary>
  /// Shell implementation must implement this
  /// interface to be invoked by the shell
  /// manager.
  /// </summary>
  public interface IPresenter
  {
    /// <summary>
    /// Event raised when a view is to be displayed.
    /// </summary>
    event Action<IView, string> OnShowView;
    /// <summary>
    /// Event raised when an error is to be displayed.
    /// </summary>
    event Action<string, string> OnShowError;
    /// <summary>
    /// Event raised when a new status is to be displayed.
    /// </summary>
    event Action<Status> OnShowStatus;
    /// <summary>
    /// Event raised when a new IPrincipal object
    /// has been set.
    /// </summary>
    event Action OnNewUser;
  }
}
