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
  public static class Extensions
  {
    public static Exception Innermost(this Exception ex)
    {
      Exception innerMost = ex;
      while (innerMost.InnerException != null)
        innerMost = innerMost.InnerException;

      return innerMost;
    }
  }
}
