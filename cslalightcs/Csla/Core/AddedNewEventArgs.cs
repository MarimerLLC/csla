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

namespace Csla.Core
{
  /// <summary>
  /// Object containing information about a
  /// newly added object.
  /// </summary>
  /// <typeparam name="T">
  /// Type of the object that was added.
  /// </typeparam>
  public class AddedNewEventArgs<T> : EventArgs
  {
    /// <summary>
    /// Gets a reference to the newly added
    /// object.
    /// </summary>
    public T NewObject { get; protected set; }

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    public AddedNewEventArgs() { }

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    /// <param name="newObject">
    /// The newly added object.
    /// </param>
    public AddedNewEventArgs(T newObject)
    {
      NewObject = newObject;
    }
  }
}
