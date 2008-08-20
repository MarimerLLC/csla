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

namespace Csla.Silverlight
{
  public class CslaDataProviderQueryCompletedEventArgs : EventArgs
  {
    private CslaDataProviderQueryCompletedEventArgs() { }
    public CslaDataProviderQueryCompletedEventArgs(object data, Exception error)
    {
      _data = data;
      _error = error;
    }
    private object _data;
    public object Data
    {
      get { return _data; }
    }

    private Exception _error;
    public Exception Error
    {
      get { return _error; }
    }
  }
}
