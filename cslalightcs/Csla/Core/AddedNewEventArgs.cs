using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Csla.Core
{
  public class AddedNewEventArgs<T> : EventArgs
  {
    public T NewObject { get; protected set; }

    public AddedNewEventArgs() { }
    public AddedNewEventArgs(T newObject)
    {
      NewObject = newObject;
    }
  }
}
