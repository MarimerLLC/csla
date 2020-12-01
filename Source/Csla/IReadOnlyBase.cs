using Csla.Core;
using Csla.Rules;
using Csla.Security;
using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla
{
  /// <summary>
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  public interface IReadOnlyBase
 : ICloneable, IReadOnlyObject, IBusinessObject, ISerializationNotification,
    IAuthorizeReadWrite, INotifyBusy, INotifyUnhandledAsyncException, IHostRules,
    INotifyPropertyChanged, INotifyPropertyChanging, IMobileObject
  { }
}
