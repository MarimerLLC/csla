using Csla.Core;
using Csla.Rules;
using Csla.Security;
using Csla.Serialization.Mobile;
using System.ComponentModel;

namespace Csla
{
  /// <summary>
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  public interface IReadOnlyBase
 : ICloneable, IReadOnlyObject, IBusinessObject, ISerializationNotification,
    IAuthorizeReadWrite, INotifyBusy, INotifyUnhandledAsyncException, IHostRules,
    INotifyPropertyChanged, INotifyPropertyChanging, IMobileObject;
}
