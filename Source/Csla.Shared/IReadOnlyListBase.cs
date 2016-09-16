using Csla.Core;
using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// This is the base class from which readonly collections
  /// of readonly objects should be derived.
  /// </summary>
  public interface IReadOnlyListBase<C>
#if !NETFX_CORE && !ANDROID && !IOS || NETSTANDARD
 : IReadOnlyCollection, IBusinessObject, ICloneable, IObservableBindingList,
    INotifyBusy, INotifyUnhandledAsyncException, INotifyChildChanged, ISerializationNotification,
    IMobileObject, INotifyCollectionChanged, INotifyPropertyChanged,
    IList<C>, ICollection<C>, IEnumerable<C>
#else
 : IReadOnlyCollection, IBusinessObject, 
    IExtendedBindingList,
    INotifyBusy, INotifyUnhandledAsyncException, INotifyChildChanged, ISerializationNotification,
    IMobileObject, INotifyCollectionChanged, INotifyPropertyChanged, 
    IList<C>, ICollection<C>, IEnumerable<C>
#endif
  { }
}
