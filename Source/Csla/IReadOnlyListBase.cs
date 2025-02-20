using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla
{
  /// <summary>
  /// This is the base class from which readonly collections
  /// of readonly objects should be derived.
  /// </summary>
  public interface IReadOnlyListBase<C> :
    IReadOnlyCollection,
    ICloneable,
    IObservableBindingList,
    INotifyBusy,
    INotifyChildChanged,
    ISerializationNotification,
    IMobileObject,
    INotifyCollectionChanged,
    INotifyPropertyChanged,
    IList<C>;
}
