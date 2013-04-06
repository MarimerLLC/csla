using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
    /// Gets the NotifyCollectionChangedEventArgs object from the
    /// child's ListChanged event, if the child is a
    /// collection or list.
    /// </summary>
    public NotifyCollectionChangedEventArgs ListChangedArgs { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="source">
    /// Reference to the object that was changed.
    /// </param>
    /// <param name="listArgs">
    /// NotifyCollectionChangedEventArgs object or null.
    /// </param>
    /// <param name="propertyArgs">
    /// PropertyChangedEventArgs object or null.
    /// </param>
    public ChildChangedEventArgs(object source, PropertyChangedEventArgs propertyArgs,  NotifyCollectionChangedEventArgs listArgs)
    {
      this.ChildObject = source;
      this.PropertyChangedArgs = propertyArgs;
      this.ListChangedArgs = listArgs;
    }
  }
}
