using Csla.Core;
using Csla.Serialization.Mobile;
using System.ComponentModel;

namespace Csla
{
  /// <summary>
  /// This is the base class from which command 
  /// objects will be derived.
  /// </summary>
  public interface ICommandBase
    : ICommandObject, IBusinessObject, IMobileObject, ICloneable,
      INotifyPropertyChanged
  { }
}
