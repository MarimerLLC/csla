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
  public class AsyncAsserter
  {
    public event AsyncAssertComplete Complete;
    private void OnComplete(Exception exception)
    {
      OnComplete(MethodTesterStatus.Fail, exception);
    }
    private void OnComplete(MethodTesterStatus status)
    {
      OnComplete(status, null);
    }
    protected virtual void OnComplete(MethodTesterStatus status, Exception exception)
    {
      if (Complete != null)
        Complete(status, exception);
    }

    public virtual void Success()
    {
      OnComplete(MethodTesterStatus.Success);
    }

    public virtual void Indeterminate()
    {
      OnComplete(MethodTesterStatus.Indeterminate);
    }

    internal void Fail()
    {
      OnComplete(MethodTesterStatus.Fail);
    }

    public void IsNotNull(object actual)
    {
      if (actual == null)
        OnComplete(new TestException("Assert.IsNotNull failed."));
    }

    public void IsNull(object actual)
    {
      if (actual != null)
        OnComplete(new TestException("Assert.IsNull failed."));
    }

    public void AreEqual(object expected, object actual)
    {
      if (!object.Equals(expected, actual))
        OnComplete(new TestException("Assert.AreEqual failed."));
    }

    public void IsFalse(bool actual)
    {
      if (actual)
        OnComplete(new TestException("Assert.IsFalse failed."));
    }

    public void IsTrue(bool actual)
    {
      if (!actual)
        OnComplete(new TestException("Assert.IsTrue failed."));
    }
  }

  public delegate void AsyncAssertComplete(MethodTesterStatus status, Exception exception);
}
