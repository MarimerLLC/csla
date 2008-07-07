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
using System.Diagnostics;

namespace cslalighttest.Engine
{
#if TESTING
  [DebuggerNonUserCode]
#endif
  public static class Assert
  {
    public static void AreEqual(object expected, object actual)
    {
      AreEqual(expected, actual, "Assert.AreEqual failed.");
    }
    public static void AreEqual(object expected, object actual, string message)
    {
      if (!expected.Equals(actual))
        throw new TestException(message);
    }

    public static void IsNull(object actual)
    {
      IsNull(actual, "Assert.IsNull failed.");
    }
    public static void IsNull(object actual, string message)
    {
      if (actual != null)
        throw new TestException(message);
    }

    public static void IsNotNull(object actual)
    {
      IsNotNull(actual, "Assert.IsNotNull failed.");
    }
    public static void IsNotNull(object actual, string message)
    {
      if (actual == null)
        throw new TestException(message);
    }

    public static void AreSame(object expected, object actual)
    {
      AreSame(expected, actual, "Assert.AreSame failed.");
    }
    public static void AreSame(object expected, object actual, string message)
    {
      if (!object.Equals(expected, actual))
        throw new TestException(message);
    }

    public static void IsTrue(bool value)
    {
      IsTrue(value, "Assert.IsTrue failed.");
    }
    public static void IsTrue(bool value, string message)
    {
      if (!value)
        throw new TestException(message);
    }

    public static void IsFalse(bool value)
    {
      IsFalse(value, "Assert.IsFalse failed.");
    }
    public static void IsFalse(bool value, string message)
    {
      if (value)
        throw new TestException(message);
    }
  }
}
