using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines an object that raises the PropertyChanging
  /// event.
  /// </summary>
  public interface INotifyPropertyChanging
  {
    /// <summary>
    /// Event indicating that a property is changing.
    /// </summary>
    event PropertyChangingEventHandler PropertyChanging;
  }
}
