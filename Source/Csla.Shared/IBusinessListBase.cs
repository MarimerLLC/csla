using System;
using Csla.Core;
using System.ComponentModel;
using Csla.Security;
using Csla.Rules;
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
#if !NETFX_CORE && !WINDOWS_PHONE
 : IEditableCollection, IBusinessObject, ISupportUndo, ITrackStatus, IUndoableObject,
    ICloneable, ISavable, IParent, INotifyBusy, INotifyUnhandledAsyncException,
    IObservableBindingList, INotifyChildChanged, ISerializationNotification, IMobileObject,
    INotifyCollectionChanged, INotifyPropertyChanged,
    ICollection<C>, IList<C>, IEnumerable<C>
#else
 : IEditableCollection, IBusinessObject, ISupportUndo, ITrackStatus, IUndoableObject,
		ISavable, IParent, INotifyBusy, INotifyUnhandledAsyncException,
		IExtendedBindingList, ISerializationNotification, INotifyChildChanged, IMobileObject,
		INotifyCollectionChanged, INotifyPropertyChanged,
		IList<C>, ICollection<C>, IEnumerable<C>
#endif
  { }
}
