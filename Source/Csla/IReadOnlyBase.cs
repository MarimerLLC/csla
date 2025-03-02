using Csla.Core;
using Csla.Rules;
using Csla.Security;
using Csla.Serialization.Mobile;

namespace Csla
{
  /// <summary>
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  public interface IReadOnlyBase :
    ICloneable,
    IReadOnlyObject,
    ISerializationNotification,
    IAuthorizeReadWrite,
    INotifyBusy,
    IHostRules,
    INotifyPropertyChanged,
    INotifyPropertyChanging,
    IMobileObject;
}
