using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines the method signature for the
  /// PropertyChanging event handler.
  /// </summary>
  /// <param name="sender">Object raising the event.</param>
  /// <param name="e">EventArgs object.</param>
  public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}
