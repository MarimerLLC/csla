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
using System.Threading;

namespace Csla.Threading
{
  public class Semaphore : IDisposable
  {
    private static object _lock = typeof(Semaphore); // Use the type to lock across processes (if SL respects this...)
    private static int _active;
    private static int _max = 1;
    public static int Max
    {
      get { return _max; }
      set
      {
        lock (_lock)
          _max = value;
      }
    }

    public static int Active
    {
      get { return _active; }
    }

    public Semaphore()
    {
      bool okToRun = false;
      do
      {
        if (_active < _max)
        {
          lock (_lock)
          {
            if (_active < _max)
            {
              _active++;
              okToRun = true;
            }
          }
        }
        else Thread.Sleep(2);
      }
      while (!okToRun);
    }

    public void Dispose()
    {
      lock (_lock)
        _active--;
    }
  }
}
