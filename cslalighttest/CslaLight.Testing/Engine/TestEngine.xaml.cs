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
using System.ComponentModel;

namespace cslalighttest.Engine
{
  public partial class TestEngine : UserControl
  {
    private TestContext _context;
    public TestContext Context
    {
      get { return _context; }
    }

    public TestEngine()
    {
      if (!DesignerProperties.GetIsInDesignMode(this))
      {
        _context = new TestContext();
        DataContext = Context;

        InitializeComponent();        
      }
    }

    private void RunAll_Click(object sender, RoutedEventArgs e)
    {
      _context.Run();
    }

    private void RunType_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      TypeTester tester = (TypeTester)b.DataContext;
      tester.RunTests();
    }

    private void RunMethod_Click(object sender, RoutedEventArgs e)
    {
      Button b = (Button)sender;
      MethodTester tester = (MethodTester)b.DataContext;

      tester.RunTest();
    }
  }
}
