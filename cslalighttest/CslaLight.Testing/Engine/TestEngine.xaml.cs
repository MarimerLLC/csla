using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Collections.ObjectModel;

namespace cslalighttest.Engine
{
  public partial class TestEngine : UserControl
  {
    private TestContext _context = new TestContext();
    public TestContext Context
    {
      get { return _context; }
    }

    public TestEngine()
    {
      DataContext = Context;

      InitializeComponent();
      
      Assembly a = this.GetType().Assembly;
      foreach (Type t in a.GetTypes())
      {
        if (t.IsPublic && t.IsDefined(typeof(TestClassAttribute), true))
        {
          TypeTester tester = new TypeTester(t);
          _context.Testers.Add(tester);
        }
      }

      foreach (TypeTester tester in _context.Testers)
        tester.RunTests(_context);
    }
  }
}
