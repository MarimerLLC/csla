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
  public abstract class TestBase
  {
    private UnitTestContext _context = new UnitTestContext();
    public UnitTestContext Context { get { return _context; } set { _context = value; } }

    public UnitTestContext GetContext() { return Context; }
  }
}
