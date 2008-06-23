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

namespace cslalighttest.Engine
{
  public class AsyncTestContext
  {
    public event AsyncTestCompleteDelegate Complete;
    protected virtual void OnComplete(MethodTesterStatus status, Exception error)
    {
      if (_asserter != null)
        _asserter.Complete -= OnComplete;

      if (Complete != null)
        Complete(this, new AsyncTestCompleteEventArgs(status, error));
    }

    private AsyncAsserter _asserter;
    public AsyncAsserter Assert
    {
      get
      {
        if (_asserter == null)
        {
          _asserter = new AsyncAsserter();
          _asserter.Complete += OnComplete;
        }
        
        return _asserter;
      }
    }
  }

  public delegate void AsyncTestCompleteDelegate(object sender, AsyncTestCompleteEventArgs e);

  public sealed class AsyncTestCompleteEventArgs : EventArgs
  {
    private Exception _error;
    private MethodTesterStatus _status;

    public Exception Error { get { return _error; } }
    public MethodTesterStatus Status { get { return _status; } }

    public AsyncTestCompleteEventArgs(MethodTesterStatus status, Exception error)
    {
      _status = status;
      _error = error;
    }
  }
}
