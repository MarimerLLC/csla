using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  public delegate void BusyChangedEventHandler(object sender, BusyChangedEventArgs e);

  public class BusyChangedEventArgs : EventArgs
  {
    public bool Busy { get; protected set; }
    public string PropertyName { get; protected set; }

    public BusyChangedEventArgs(string propertyName, bool busy)
    {
      PropertyName = propertyName;
      Busy = busy;
    }
  }
}
