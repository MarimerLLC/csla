using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  public interface INotifyChildChanged
  {
    event EventHandler<ChildChangedEventArgs> ChildChanged;
  }
}
