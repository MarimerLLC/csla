using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace PTWpf
{
  /// <summary>
  /// Implement this interface in a Page
  /// to be notified when the currently
  /// logged in user changes.
  /// </summary>
  public interface IRefresh
  {
    /// <summary>
    /// Called by MainPage when the currently
    /// logged in user changes.
    /// </summary>
    void Refresh();
  }
}
