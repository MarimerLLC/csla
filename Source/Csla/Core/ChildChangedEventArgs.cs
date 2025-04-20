//-----------------------------------------------------------------------
// <copyright file="ChildChangedEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains event data about the changed child object.</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
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
    public object ChildObject { get; }
    /// <summary>
    /// Gets the PropertyChangedEventArgs object from the
    /// child's PropertyChanged event, if the child is
    /// not a collection or list.
    /// </summary>
    public PropertyChangedEventArgs? PropertyChangedArgs { get; }
    /// <summary>
    /// Gets the NotifyCollectionChangedEventArgs object from the
    /// child's CollectionChanged event, if the child is an
    /// ObservableCollection.
    /// </summary>
    public NotifyCollectionChangedEventArgs? CollectionChangedArgs { get; }

    /// <summary>
    /// Gets the ListChangedEventArgs object from the
    /// child's ListChanged event, if the child is a
    /// collection or list.
    /// </summary>
    public ListChangedEventArgs? ListChangedArgs { get; }

    /// <summary>
    /// Creates an instance of the type.
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
    /// <exception cref="ArgumentNullException"><paramref name="childObject"/> is <see langword="null"/>.</exception>
    public ChildChangedEventArgs(object childObject, PropertyChangedEventArgs? propertyArgs, ListChangedEventArgs? listArgs)
      : this(childObject, propertyArgs)
    {
      ListChangedArgs = listArgs;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="childObject">
    /// Reference to the child object that was changed.
    /// </param>
    /// <param name="propertyArgs">
    /// PropertyChangedEventArgs object or null.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="childObject"/> is <see langword="null"/>.</exception>
    public ChildChangedEventArgs(object childObject, PropertyChangedEventArgs? propertyArgs)
    {
      ChildObject = childObject ?? throw new ArgumentNullException(nameof(childObject));
      PropertyChangedArgs = propertyArgs;
    }

    /// <summary>
    /// Creates an instance of the type.
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
    /// <exception cref="ArgumentNullException"><paramref name="childObject"/> is <see langword="null"/>.</exception>
    public ChildChangedEventArgs(object childObject, PropertyChangedEventArgs? propertyArgs, NotifyCollectionChangedEventArgs? listArgs)
      : this(childObject, propertyArgs)
    {
      CollectionChangedArgs = listArgs;
    }
  }
}