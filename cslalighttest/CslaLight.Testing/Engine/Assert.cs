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
  public static class Assert
  {
    public static void AreEqual(object expected, object actual)
    {
      if (!expected.Equals(actual))
        throw new TestException("Assert.AreEqual failed.");
    }

    public static void IsNull(object actual)
    {
      if (actual != null)
        throw new TestException("Assert.IsNull failed.");
    }

    public static void IsNotNull(object actual)
    {
      if (actual == null)
        throw new TestException("Assert.IsNotNull failed.");
    }

    public static void AreSame(object expected, object actual)
    {
      if (!object.Equals(expected, actual))
        throw new TestException("Assert.AreSame failed.");
    }
  }
}
