using System;
using Csla.Core;
using System.ComponentModel;
using Csla.Security;
using Csla.Rules;
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
    ISerializationNotification
#if NETFX_CORE && !ANDROID && !IOS
    ,INotifyDataErrorInfo
#else
    , IDataErrorInfo
#endif
  {
  }
}
