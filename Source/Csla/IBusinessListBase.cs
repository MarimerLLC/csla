using System;
using Csla.Core;
using System.ComponentModel;
using Csla.Serialization.Mobile;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Csla
{
  /// <summary>
  /// This is the base class from which most business collections
  /// or lists will be derived.
  /// </summary>
  /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
  public interface IBusinessListBase<C>
    : IEditableCollection, IBusinessObject, ISupportUndo, ITrackStatus, IUndoableObject,
      ICloneable, ISavable, IParent, INotifyBusy, INotifyUnhandledAsyncException,
      IObservableBindingList, INotifyChildChanged, ISerializationNotification, IMobileObject,
      INotifyCollectionChanged, INotifyPropertyChanged,
      ICollection<C>, IList<C>, IEnumerable<C>
  { }
}
