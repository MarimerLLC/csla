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
    public event AsyncAssertError Error;
    protected virtual void OnError(Exception exception)
    {
      if (Error != null)
        Error(exception);
    }

    public void IsNotNull(object actual)
    {
      if (actual == null)
        Error(new TestException("Assert.IsNotNull failed."));
    }

    public void IsNull(object actual)
    {
      if (actual != null)
        Error(new TestException("Assert.IsNull failed."));
    }

    public void AreEqual(object expected, object actual)
    {
      if (!object.Equals(expected, actual))
        Error(new TestException("Assert.AreEqual failed."));
    }

    public void IsFalse(bool actual)
    {
      if (actual)
        Error(new TestException("Assert.IsFalse failed."));
    }

    public void IsTrue(bool actual)
    {
      if (!actual)
        Error(new TestException("Assert.IsTrue failed."));
    }
  }

  public delegate void AsyncAssertError(Exception exception);
}
