//-----------------------------------------------------------------------
// <copyright file="NotifyCollectionChangedEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Android implementation of NotifyCollectionChangedEventArgs</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Specialized
{
  public class NotifyCollectionChangedEventArgs : EventArgs
  {
    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
    {
      Action = action;
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
      : this(action)
    {
      NewItems = new List<object> { changedItem };
      NewStartingIndex = index;
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
      : this(action)
    {
      NewItems = new List<object> { newItem };
      OldItems = new List<object> { oldItem };
      NewStartingIndex = index;
    }

    // Summary:
    //     Gets the description of the action that caused the event.
    //
    // Returns:
    //     The description of the action that caused the event, as a value of the enumeration.
    public NotifyCollectionChangedAction Action { get; private set; }
    //
    // Summary:
    //     Gets the items affected by an action.
    //
    // Returns:
    //     The list of items affected by an action. The default is null.
    public IList NewItems { get; private set; }
    //
    // Summary:
    //     Gets the index at which the change occurred.
    //
    // Returns:
    //     The index at which the change occurred.
    public int NewStartingIndex { get; private set; }
    //
    // Summary:
    //     Gets the item affected by a System.Collections.Specialized.NotifyCollectionChangedAction.Replace
    //     or System.Collections.Specialized.NotifyCollectionChangedAction.Remove action.
    //
    // Returns:
    //     The list of items affected by a System.Collections.Specialized.NotifyCollectionChangedAction.Replace
    //     or System.Collections.Specialized.NotifyCollectionChangedAction.Remove action.
    public IList OldItems { get; private set; }
    //
    // Summary:
    //     Gets the index at which the change occurred for a System.Collections.Specialized.NotifyCollectionChangedAction.Replace
    //     or System.Collections.Specialized.NotifyCollectionChangedAction.Remove action.
    //
    // Returns:
    //     The index at which the change occurred for a System.Collections.Specialized.NotifyCollectionChangedAction.Replace
    //     or System.Collections.Specialized.NotifyCollectionChangedAction.Remove action.
    public int OldStartingIndex { get; private set; }
  }
}