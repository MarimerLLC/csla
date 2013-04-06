using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Csla.Core
{
  /// <summary>
  /// Contains event data about the changed child object.
  /// </summary>
  public class ChildChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Gets a reference to the changed child object.
    /// </summary>
    public object ChildObject { get; private set; }
    /// <summary>
    /// Gets the PropertyChangedEventArgs object from the
    /// child's PropertyChanged event, if the child is
    /// not a collection or list.
    /// </summary>
    public PropertyChangedEventArgs PropertyChangedArgs { get; private set; }
    /// <summary>
    /// Gets the ListChangedEventArgs object from the
    /// child's ListChanged event, if the child is a
    /// collection or list.
    /// </summary>
    public ListChangedEventArgs ListChangedArgs { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="childObject">
    /// Reference to the child object that was changed.
    /// </param>
    /// <param name="listArgs">
    /// ListChangedEventArgs object or null.
    /// </param>
    /// <param name="propertyArgs">
    /// PropertyChangedEventArgs object or null.
    /// </param>
    public ChildChangedEventArgs(object childObject, PropertyChangedEventArgs propertyArgs, ListChangedEventArgs listArgs)
    {
      this.ChildObject = childObject;
      this.PropertyChangedArgs = propertyArgs;
      this.ListChangedArgs = listArgs;
    }
  }
}
