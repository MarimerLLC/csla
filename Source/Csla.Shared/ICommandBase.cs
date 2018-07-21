using Csla.Core;
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
  /// This is the base class from which command 
  /// objects will be derived.
  /// </summary>
  public interface ICommandBase
    : ICommandObject, IBusinessObject, IMobileObject, ICloneable,
      INotifyPropertyChanged
  { }
}
