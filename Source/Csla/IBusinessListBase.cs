﻿using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla
{
  /// <summary>
  /// This is the base class from which most business collections
  /// or lists will be derived.
  /// </summary>
  /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
  public interface IBusinessListBase<C> :
    IEditableCollection,
    IUndoableObject,
    ICloneable,
    ISavable,
    IParent,
    IObservableBindingList,
    INotifyChildChanged,
    ISerializationNotification,
    IMobileObject,
    INotifyCollectionChanged,
    INotifyPropertyChanged,
    IList<C>;
}