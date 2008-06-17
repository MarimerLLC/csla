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
    public TestEngine()
    {
      InitializeComponent();

      ObservableCollection<TypeTester> testers = new ObservableCollection<TypeTester>();

      Assembly a = this.GetType().Assembly;
      foreach (Type t in a.GetTypes())
      {
        if (t.IsPublic && t.IsDefined(typeof(TestClassAttribute), true))
        {
          TypeTester tester = new TypeTester(t);
          testers.Add(tester);
        }
      }
      types.ItemsSource = testers;

      foreach (TypeTester tester in testers)
        tester.RunTests();
    }
  }
}
