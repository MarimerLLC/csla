using System.ComponentModel;
using Csla.Core;
using Csla.Rules;
using Csla.Security;
using Csla.Serialization.Mobile;

namespace Csla
{
  /// <summary>
  /// Consolidated interface of public elements from the
  /// BusinessBase type.
  /// </summary>
  public interface IBusinessBase : IBusinessObject,
    IMobileObject,
    IEditableBusinessObject,
    IEditableObject,
    ICloneable,
    INotifyPropertyChanged,
    ISavable,
    IAuthorizeReadWrite,
    IParent,
    IHostRules,
    ICheckRules,
    INotifyBusy,
    INotifyChildChanged,
    ISerializationNotification,
    INotifyDataErrorInfo,
    IDataErrorInfo;
}
